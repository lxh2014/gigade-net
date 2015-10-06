using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Web;
using System.Security.Principal;

namespace WindowsService
{
    partial class Service : ServiceBase
    {
        private System.ComponentModel.IContainer components = null;
        private bool servicePaused = false;//服務是否停止
        private System.Timers.Timer time;//計時器
        Thread thread, thread2, thread3;
        int preSuCount = 0, saleCount = 0, productCount = 0;
        public Service()
        {
            InitializeComponent();
        }

        static void Main()
        {
            ServiceBase[] myservice = new ServiceBase[] { new Service() };
            ServiceBase.Run(myservice);
        }


        protected override void OnStart(string[] args)
        {
            if (!servicePaused)
            {
                time = new System.Timers.Timer(1000 * 59);//設置定時重複發送的時間                time.Enabled = true;
                time.Elapsed += this.Elapsed;
                time.Start();
            }
        }

        /// <summary>
        /// 希望要執行的代碼
        /// </summary>
        private void Elapsed(object sender, EventArgs e)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan saleTime1 = DateTime.MinValue.TimeOfDay;
            TimeSpan saleTime2 = DateTime.MinValue.TimeOfDay;
            TimeSpan preSuTime = TimeSpan.Parse(ConfigurationManager.AppSettings["PreSuTime"]);
            TimeSpan productTimes = TimeSpan.Parse(ConfigurationManager.AppSettings["StockTime"]);
            ///販售狀態一天需要執行兩次,所以更改關於時間的獲得方式
            ///設置獲取的為一個存儲時間的字符串時間之間用','分割
            string saleTimes = ConfigurationManager.AppSettings["SaleTime"];
            ///用一個字符串數組接受時間格式的值
            string[] saleTimeArray = saleTimes.Split(',');
            ///如果長度大於0 則取下標為 0 的時間
            if (saleTimeArray.Length > 0)
            {
                if (saleTimeArray[0] != "")
                {
                    saleTime1 = TimeSpan.Parse(saleTimeArray[0]);
                }
            }
            ///如果長度為1 則取下表為1的第二個執行時間
            if (saleTimeArray.Length > 1)
            {
                if (saleTimeArray[1] != "")
                {
                    saleTime2 = TimeSpan.Parse(saleTimeArray[1]);
                }
            }
            if ((now - preSuTime).Minutes > 5) preSuCount = 0;
            if ((now - saleTime1).Minutes > 5) saleCount = 0;
            if ((now - productTimes).Minutes > 5) productCount = 0;
            if (preSuCount == 0 && (now - preSuTime).Minutes == 0 && (now - preSuTime).Hours == 0)
            {
                thread = new Thread(CallNameExtend);
                thread.Start();
                preSuCount++;
            }

            if (saleCount == 0 && (((now - saleTime1).Minutes == 0 && (now - saleTime1).Hours == 0) || ((now - saleTime2).Minutes == 0 && (now - saleTime2).Hours == 0)))
            {
                thread2 = new Thread(CallSaleStatues);
                thread2.Start();
                saleCount++;
            }

            if (productCount == 0 && (now - productTimes).Minutes == 0 && (now - productTimes).Hours == 0)
            {
                thread3 = new Thread(ProductStock);
                thread3.Start();
                productCount++;
            }

            //if (saleCount == 0 && (now - saleTime).Minutes == 0 && (now - saleTime).Hours == 0)
            //{
            //    thread2 = new Thread(CallSaleStatues);
            //    thread2.Start();
            //    saleCount++;
            //}
        }

        protected override void OnStop()
        {
            servicePaused = true;
            time.Stop();
        }

        protected override void OnPause()
        {
            servicePaused = true;
            time.Stop();
        }

        protected override void OnContinue()
        {
            servicePaused = false;
            time.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.CanPauseAndContinue = true;
            this.ServiceName = "gigadeService";
        }

        /// <summary>
        /// 調用存儲過程的方法
        /// </summary>
        public void CallNameExtend()
        {
            try
            {
                string PreSuStrUrl = ConfigurationManager.AppSettings["PreSuStrUrl"];
                HttpPost(PreSuStrUrl);
            }
            finally
            {
                thread.Abort();
            }
        }

        public void CallSaleStatues()
        {
            try
            {
                string SaleStrUrl = ConfigurationManager.AppSettings["SaleStrUrl"];
                HttpPost(SaleStrUrl);
            }
            finally
            {
                thread2.Abort();
            }
        }

        public void ProductStock()
        {
            try
            {
                string productTimes = ConfigurationManager.AppSettings["ProductStockUrl"];
                HttpPost(productTimes);
            }
            finally
            {
                thread3.Abort();
            }
        }
        /// <summary>
        /// 發起的post請求
        /// </summary>
        /// <param name="Url">請求的路徑</param>
        /// <returns></returns>
        private string HttpPost(string Url)
        {
            string retString = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            Stream myRequestStream = request.GetRequestStream();
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            retString = myStreamReader.ReadToEnd();
            response.Close();
            response.Dispose();

            myStreamReader.Close();
            myStreamReader.Dispose();

            myResponseStream.Close();
            myResponseStream.Dispose();
            return retString;
        }
    }
}
