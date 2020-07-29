using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VendingMachine
{
    /// <summary>
    /// 自動販賣機
    /// 題目來源:https://ck101.com/thread-2871312-1-1.html
    /// 參考資料:
    /// 中英數文字混雜的截字或補空白方法[Big5版] : https://dotblogs.com.tw/wadehuang36/2010/12/03/big5_process
    /// C# - foreach Exception "集合已修改; 列舉作業可能尚未執行" 解決方法 : http://limitedcode.blogspot.com/2014/10/foreach-exception.html  
    /// </summary>
    class Program
    {
        static Dictionary<string, Product> Order = null;
        static Dictionary<int, int> CoinsBox = null;
        static int iTotal = 0;
        static int iCoinTotal = 0;
         
        static void Main(string[] args)
        {
            Inititle();
            ShowData();

            while (true)
            {
                Console.Write("請選擇動作 <a>投幣 <b>選擇商品 <c>取消商品 <d>購買 <e>取消交易 : ");
                var Readkey = Console.ReadKey();
                
                if (Readkey.KeyChar.ToString() == "e")
                {
                    Console.WriteLine("\n");
                    Console.WriteLine(string.Format("投幣金額：{0} < 1元: {1}, 5元: {2}, 10元: {3}, 50元: {4} >", iCoinTotal, CoinsBox[1], CoinsBox[5], CoinsBox[10], CoinsBox[50]));
                    Console.Write("結束交易，請按任一鍵離開...");
                    Console.ReadKey();
                    break;
                }

                Console.WriteLine("\n"); 
                switch (Readkey.KeyChar.ToString())
                {
                    case "a":
                        Console.WriteLine("請投入 1 元、5 元、10 元、或 50 元硬幣。輸入 0 時，表示投幣結束。");
                        InsertCoins();
                        break;
                    case "b":
                        Console.WriteLine("請按商品項目按鈕以選擇要購買的商品。輸入 'Q' 時，表示商品選擇結束。");
                        AddItems();
                        break;
                    case "c":
                        Console.Write("請按商品項目按鈕以選擇要取消購買的商品 : ");
                        RemoveItem();
                        break;
                    case "d":
                        Console.WriteLine("結帳中...");
                        while (iCoinTotal < iTotal)
                        {
                            Console.WriteLine("金額不足，請繼續投幣。輸入 0 時，表示投幣結束...");
                            InsertCoins();
                        }
                        SettleAccounts();
                        break;
                }

                Console.WriteLine();
                ShowData();
            }
        }
       
        public static void Inititle() 
        {
            iTotal = 0;
            iCoinTotal = 0;
            
            Order = new Dictionary<string, Product>() 
            {
                {"A", new Product("可樂", 30)},
                {"B", new Product("雪碧", 30)},
                {"C", new Product("礦泉水", 20)},
                {"D", new Product("雀巢檸檬紅茶", 25)},
                {"E", new Product("科學麵", 10)},
                {"F", new Product("卡迪那", 20)},
                {"G", new Product("阿Q泡麵", 50)},
                {"H", new Product("小熊餅乾", 35)}
            };
        
            CoinsBox = new Dictionary<int, int>() 
            {
                {1, 0},
                {5, 0},
                {10, 0},
                {50, 0}
            };
        }

        public static void ShowData()
        {
            int width = 8;
            Console.WriteLine(FixedWidth("代碼", 4) + "  " + FixedWidth("品名", 10, true) + FixedWidth("單價", width) + FixedWidth("訂購", width) + FixedWidth("小計", width));
            Console.WriteLine(FixedWidth("====", 4) + "  " + FixedWidth("==========", 10, true) + FixedWidth("====", width) + FixedWidth("====", width) + FixedWidth("====", width));
            foreach (KeyValuePair<string, Product> item in Order)
            {
                Console.WriteLine
                (
                    FixedWidth(item.Key, 4) + "  " +
                    FixedWidth(item.Value.ProductName, 10, true) +
                    FixedWidth(item.Value.Price.ToString(), width) +
                    FixedWidth(item.Value.OrderQuantity.ToString(), width) +
                    FixedWidth(item.Value.SubTotal.ToString(), width)
                );
            }

            Console.WriteLine("\n訂購金額：" + iTotal);
            Console.WriteLine(string.Format("投幣金額：{0} < 1元: {1}, 5元: {2}, 10元: {3}, 50元: {4} >", iCoinTotal, CoinsBox[1], CoinsBox[5], CoinsBox[10], CoinsBox[50]));
            Console.WriteLine();
        }

        /// <summary>
        /// 輸出固定長度字串
        /// </summary>
        /// <param name="value">傳入的值</param>
        /// <param name="maxLength">欄位寬度</param>
        /// <param name="leftAlign">指定靠左或靠右對齊，預設為否</param>
        /// <returns>截字或補空白之資料字串</returns>
        public static string FixedWidth(string value, int maxLength, bool leftAlign = false)
        {
            int padding = 0;
            var buffer = Encoding.Default.GetBytes(value);
            if (buffer.Length > maxLength)
            {
                int charStartIndex = maxLength - 1;
                int charEndIndex = 0;
                //跑回圈去算出結尾。
                for (int i = 0; i < maxLength; )
                {
                    if (buffer[i] <= 128)
                    {
                        charEndIndex = i; //英數1Byte
                        i += 1;
                    }
                    else
                    {
                        charEndIndex = i + 1; //中文2Byte
                        i += 2;
                    }
                }

                //如果開始不同與結尾，表示截到2Byte的中文字了，要捨棄1Byte
                if (charStartIndex != charEndIndex)
                {
                    value = Encoding.Default.GetString(buffer, 0, charStartIndex);
                    padding = 1;
                }
                else
                {
                    value = Encoding.Default.GetString(buffer, 0, maxLength);
                }
            }
            else
            {
                value = Encoding.Default.GetString(buffer);

                padding = maxLength - buffer.Length;
            }

            if (padding != 0)
            {
                value = leftAlign ? (value + "".PadRight(padding)) : ("".PadRight(padding) + value);
            }
            
            return value;
        }

        public static void InsertCoins() 
        {
            while (true)
            {
                int iCoin = 0;
                if (!int.TryParse(Console.ReadLine().ToString(), out iCoin)) continue;

                if (iCoin == 0)
                {
                    //計算投入的硬幣總金額
                    iCoinTotal = 0;
                    foreach (KeyValuePair<int, int> item in CoinsBox)
                    {
                        iCoinTotal += item.Key * item.Value;
                    }

                    break;
                }
                else if (CoinsBox.Keys.Contains(iCoin))
                {
                    CoinsBox[iCoin]++;                
                }
            }
            Console.WriteLine();
        }

        public static void AddItems() 
        {
            while (true)
            {
                string sReadkey = Console.ReadKey().KeyChar.ToString();
                if (sReadkey == "Q")
                {
                    break;
                }
                else if(Order.Keys.Contains(sReadkey))
                {
                    Order[sReadkey].OrderQuantity++;
                    Order[sReadkey].SubTotal = Order[sReadkey].Price * Order[sReadkey].OrderQuantity;
                    iTotal += Order[sReadkey].Price;
                }
            }
            Console.WriteLine();
        }

        public static void RemoveItem() 
        {
            string sReadkey = Console.ReadKey().KeyChar.ToString();
            if (Order.Keys.Contains(sReadkey)) 
            {
                if (Order[sReadkey].OrderQuantity > 0)
                {
                    Order[sReadkey].OrderQuantity--;
                    Order[sReadkey].SubTotal = Order[sReadkey].Price * Order[sReadkey].OrderQuantity;
                    iTotal -= Order[sReadkey].Price;
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// 結帳: 自動販賣機能以最少數量的硬幣來找零
        /// </summary>
        public static void SettleAccounts()
        {
            int width = 10;
            int iChange = iCoinTotal - iTotal;
            Console.WriteLine("選購的商品：\n");
            Console.WriteLine(FixedWidth("品名", width, true) + FixedWidth("訂購數量", width));
            Console.WriteLine(FixedWidth("==========", width, true) + FixedWidth("========", width));
 
            foreach (KeyValuePair<string, Product> item in Order)
            {
                if (item.Value.OrderQuantity != 0)
                {
                    Console.WriteLine
                    (
                        FixedWidth(item.Value.ProductName, width, true) +
                        FixedWidth(item.Value.OrderQuantity.ToString(), width)
                    );     
                }
            }
            Console.WriteLine();

            foreach (KeyValuePair<int, int> item in CoinsBox.ToArray())
            {
                CoinsBox[item.Key] = 0;
            }

            for (int intA = iChange; intA > 0; )
            {
                if (intA > 50)
                {
                    CoinsBox[50]++;
                    intA -= 50;
                }
                else if (intA > 10)
                {
                    CoinsBox[10]++;
                    intA -= 10;
                }
                else if (intA > 5)
                {
                    CoinsBox[5]++;
                    intA -= 5;
                }
                else if (intA >= 1)
                {
                    CoinsBox[1]++;
                    intA -= 1;
                }
            }

            Console.WriteLine(string.Format("找零金額：{0} < 1元: {1}, 5元: {2}, 10元: {3}, 50元: {4} >", iChange, CoinsBox[1], CoinsBox[5], CoinsBox[10], CoinsBox[50]));
        
            //初始化資料
            Inititle();
        }
    }

    class Product
    {
        public string ProductName;
        public int Price;
        public int OrderQuantity;
        public int SubTotal;

        public Product(string sProductName, int iPrice)
        {
            ProductName = sProductName;
            Price = iPrice;
        }
    }
}