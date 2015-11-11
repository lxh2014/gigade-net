using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using System.Configuration;
using System.Text;
using Admin.gigade.CustomError;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using BLL.gigade.Common;
using Newtonsoft.Json;


namespace Admin.gigade.Controllers
{
    [HandleError]
    public class OpenController : Controller
    {
        //
        // GET: /GetExpectTime/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        IScheduleRelationImplMgr _srMgr;

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 獲得最近出貨時間
        /// </summary>
        /// <param name="v">關聯表的主鍵</param>
        /// <param name="relationType">關聯表的表名稱</param>
        /// <returns>error:MinValue success:CriterionTime</returns>
        public ActionResult ExpectTime(string t, int v = 0)
        {
            DateTime dtNow = DateTime.Now;///當前時間
            int isSuccess = 1;//1:success 0: fail
            string Msg = "";                        
            DateTime date = DateTime.MinValue;
            try
            {
                _srMgr = new ScheduleRelationMgr(connectionString);
                date = _srMgr.GetRecentlyTime(v, t);
                if (date == DateTime.MinValue) { isSuccess = 0; Msg = "該商品沒有預計出貨時間或出貨時間超出合理範圍"; }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                isSuccess = 0;
                Msg = "ExpectTime Exception!";
            }
            DateTime functionEnd = DateTime.Now;
            TimeSpan ts = functionEnd - dtNow;
            double second = ts.TotalMilliseconds;
            return Json(new { data = date.ToString("yyyy/MM/dd"), success =  isSuccess, errMsg = Msg,des ="最近出貨時間", execTime = dtNow.ToString("yyyy/MM/dd"), elapsed = second }, JsonRequestBehavior.AllowGet);
        }

        ///返回貨物運達日期
        private DateTime restDay(DateTime dt, DateTime isAddDay, int days,bool schedule)
        {
            //long num_date = CommonFunction.GetPHPTime(dt.ToString());
            //string dtFlagStr =isAddDay.GetDateTimeFormats()[0] + " 15:00";
            //long  dtFlag = CommonFunction.GetPHPTime(dtFlagStr);///獲得當天15：00的時間蹉
            //long isAddDayInt = CommonFunction.GetPHPTime(isAddDay.ToString("yyyy-MM-dd HH:mm:ss"));
            //days = (isAddDayInt - dtFlag) > 0 ? days + 1 : days;  //判斷時間是否大於15點,大於時,運達天數加1
            days = IsAddDayFun(dt, isAddDay, days, schedule);
            ICalendarImplMgr _cdMgr = new CalendarMgr(connectionString);
            List<Calendar> calendar_list = _cdMgr.GetCalendarInfo(new Calendar { EndDateStr = CommonFunction.GetPHPTime(dt.ToString()).ToString() }); ///獲取行事歷控件中休息時間的集合
            DateTime sourceDt = dt;///定義一個時間用來保存下單時間
            return VerifyTime(dt, days, calendar_list, dt);
        }


        public DateTime VerifyTime(DateTime dt, int days, List<Calendar> calendar_list,DateTime sourceDateTime)
        {
            /// 遞歸調用計算運達天數 add by wwei0216w 2015/5/26
             long num_date = CommonFunction.GetPHPTime(dt.ToString());
            List<Calendar> result_list = new List<Calendar>();
            result_list = calendar_list.FindAll(m => (num_date >= Convert.ToInt64(m.StartDateStr)) && (num_date <= Convert.ToInt64(m.EndDateStr)));
            if (days != 0 && result_list.Count > 0)
            {
                if (dt == sourceDateTime)///下單的當天,無論是否處於休息時間段,均不算在時間內,所以該種情況按照正常邏輯計算,運達天數要減一
                {
                    days--;
                }
                dt = dt.AddDays(1);
                dt = VerifyTime(dt, days, calendar_list, sourceDateTime);
                return dt;
            }
            else if (days != 0 && result_list.Count == 0)
            {
                dt = dt.AddDays(1);
                days--;
                dt = VerifyTime(dt, days, calendar_list, sourceDateTime);
                return dt;
            }
            else if (days == 0 && (result_list.Count > 0))
            {
                dt = dt.AddDays(1);
                dt = VerifyTime(dt, days, calendar_list, sourceDateTime);
                return dt;
            }
            else
            {
                return dt;
            }
        }


        //public ActionResult ArriveTime(uint itemId,string dateTime)
        //{
        //    //add by wwei0216w 2015/5/25
        //    int days = 0;
        //    int Deliver_Days = 0;
        //    int isSuccess = 1;
        //    bool isSchedule = true;
        //    DateTime implementTime = DateTime.Now;
        //    DateTime date = DateTime.MinValue;
        //    string msg = "";
        //    DateTime dtTime = DateTime.Now;
        //    if (!string.IsNullOrEmpty(dateTime))
        //    {
        //        if (!DateTime.TryParse(dateTime, out dtTime))
        //        {
        //            isSuccess = 0;
        //            msg = "Time Exception!";
        //            return Json(new { data = date.ToString("yyyy-MM-dd"), success = isSuccess, errMsg = msg, des = "貨物運達時間", execTime = implementTime.ToString("yyyy/MM/dd"), elapsed = 0 }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    try
        //    {
        //        _srMgr = new ScheduleRelationMgr(connectionString);
        //        ProductItemCustom pi = new ProductItemCustom();
        //        IProductItemImplMgr _productItemMgr = new ProductItemMgr(connectionString);
        //        IOrderDetailImplMgr _orderDetailMgr = new OrderDetailMgr(connectionString);
        //        IProductImplMgr _pMgr = new ProductMgr(connectionString);
        //        pi = _productItemMgr.GetProductArriveDay(new ProductItem { Item_Id = itemId},"item");
        //        Deliver_Days = _pMgr.GetDefaultArriveDays(new Product { Product_Id = pi.Product_Id });///獲取出貨時間

        //        if(pi==null || pi.Product_Id==0)
        //        {
        //            isSuccess = 0;
        //            msg = "不存在該itemId";
        //            return Json(new { data = date.ToString("yyyy-MM-dd"), success = isSuccess, errMsg = msg, des = "貨物運達時間", execTime = implementTime.ToString("yyyy/MM/dd"), elapsed = 0 }, JsonRequestBehavior.AllowGet);
        //        }
        //        days = pi.Arrive_Days + Deliver_Days ;///計算運達天數
        //        date = _srMgr.GetRecentlyTime(Convert.ToInt32(pi.Product_Id), "product");///調用GetRecentlyTime()方法 獲得最近出貨時間
        //        DateTime payDay = dtTime == null ? implementTime : (DateTime)dtTime;///將時間賦值
        //        //edit by wwei0216w 2015/6/2 添加item_stock > 0 的判斷,如果 item_stock > 0 則不適用排程的時間,啟用當前時間作為基礎時間

        //        long tsDate = CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")) - (CommonFunction.GetPHPTime(date.ToString("yyyy/MM/dd hh:mm:ss"))+10800);
        //        ///獲得的最近出貨時間默認為12:00 加上10800 代碼再加3小時 代表 15：00 作為一天是否有效的標準判斷
        //        if (date == DateTime.MinValue || date == null || tsDate > 0)///如果得到的最近出貨天數  為最小時間   或者   小于當前時間
        //        {
        //            isSchedule = false;
        //            date = payDay;///則按下單時間開始計算到貨日期
        //        }


        //        date = restDay(date, payDay, days,isSchedule);///計算具體到貨日期
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        isSuccess = 0;
        //        msg = ex.Message;
        //    }
        //    DateTime functionEndTime = DateTime.Now;
        //    TimeSpan ts = functionEndTime - implementTime;//獲得方法開始和結束的時間
        //    double second = ts.TotalMilliseconds;//方法執行的時間
        //    return Json(new { data = date.ToString("yyyy/MM/dd"), success = isSuccess, errMsg = msg, des = "貨物運達時間", execTime = implementTime.ToString("yyyy/MM/dd"), elapsed = second }, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ArriveTime(string itemId, string dateTime)
        {
            int msgFlag = 0;
            int isSuccess = 0;
            string msg = "";
            DateTime implementTime = DateTime.Now;
            DateTime date = GetArriveTimeCombo(dateTime, out msgFlag, itemId);///計算得到最晚商品的到達時間
            switch (msgFlag)
            { 
                case -1:
                    isSuccess = 0;
                    msg = "異常!請輸入正確的時間變量";
                    break;
                case -2:
                    isSuccess = 0;
                    msg = "異常!請參考系統錯誤日誌";
                    break;
                case -3:
                    isSuccess = 0;
                    msg = "異常!傳輸入的itemIds至少要有一個值";
                    break;
                case -4:
                    isSuccess = 0;
                    msg = "異常!請確認傳入itemIds的有效性!";
                    break;
                case 1:
                    isSuccess=1;
                    msg = "執行成功";
                    break;
                default:
                    isSuccess = 0;
                    msg = "異常!msgFlag處於初始化狀態!";
                    break;
            }
            DateTime functionEndTime = DateTime.Now;
            TimeSpan ts = functionEndTime - implementTime;//獲得方法開始和結束的時間
            double second = ts.TotalMilliseconds;//方法執行的時間
            return Json(new { data = date.ToString("yyyy/MM/dd"), success = isSuccess, errMsg = msg, des = "貨物運達時間", execTime = implementTime.ToString("yyyy/MM/dd"), elapsed = second }, JsonRequestBehavior.AllowGet);
        }

        private DateTime GetArriveTimeCombo(string dateTime,out int msgFlag, string itemIds = "")
        {
            /*定義變量*/
            DateTime lastDateTime = DateTime.Now;///變量,用於保存最晚商品到達時間
            int numDays = 0;///用於保存最晚商品途中運輸天數,
            int diffDay = 0;///用於保存最晚商品天數差的變量
            int expendDay = 0;///用於保存最晚商品到達用戶手中總共的天數
            DateTime implementTime = DateTime.Now;///定義變量 用於表示排程開始時間
            DateTime date = DateTime.MinValue;///定義變量 用於表示排程是否執行成功
            DateTime payDay = DateTime.Now;///定義初始化下單時間
            int deliver_Days = 0;    
            bool isSchedule = true;///定義變量 用於判斷是否是按排程時間出貨
            string msg = "";///定義變量 用於表示錯誤的具體信息
            DateTime dtTime = DateTime.Now;
            int days = 0;///定義變量 用於表示貨物途中運輸所需要的天數
            int itemIdFlag = 0;///用於判斷循環中 是否所有的pi是否都為空,如果都為null則itemIdFlag =0;否則為1;

            /*
             實例化對象
             */
            _srMgr = new ScheduleRelationMgr(connectionString);
            ProductItemCustom pi = new ProductItemCustom();
            IProductItemImplMgr _productItemMgr = new ProductItemMgr(connectionString);
            IOrderDetailImplMgr _orderDetailMgr = new OrderDetailMgr(connectionString);
            IProductImplMgr _pMgr = new ProductMgr(connectionString);


            ///邏輯計算
            try
            {
                if (!string.IsNullOrEmpty(dateTime))///如果時間不正確,直接返回時間錯誤
                {
                    if (!DateTime.TryParse(dateTime, out dtTime))
                    {
                        msgFlag = -1;///錯誤變量 -1:TimeException!
                        return DateTime.MinValue;
                    }
                }

                payDay = dtTime == null ? implementTime : (DateTime)dtTime;///得到下單時間


                string[] itemIdsArray = itemIds.Split(',');
                if (itemIds == "" || itemIdsArray.Length==0)
                {
                    msgFlag = -3;
                    return DateTime.Now;
                }

                foreach (string i in itemIdsArray)
                {
                    //pi = _productItemMgr.GetProductArriveDay(new ProductItem { Product_Id = i }, "product");///獲得prdocut_item中的array_days
                   
                    if (i != null)
                    {
                        itemIdFlag = 1;
                        pi = _productItemMgr.GetProductArriveDay(new ProductItem { Item_Id = Convert.ToUInt32(i) }, "item");//得到product_item中的出貨天數
                        deliver_Days = _pMgr.GetDefaultArriveDays(new Product { Product_Id = pi.Product_Id });///獲取供應商所對應的出貨天數
                        days = pi.Arrive_Days + deliver_Days;///獲得貨物運輸途中所需要的天數
                        date = _srMgr.GetRecentlyTime(Convert.ToInt32(pi.Product_Id), "product");///調用GetRecentlyTime()方法 獲得最近出貨時間
                        long tsDate = CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")) - (CommonFunction.GetPHPTime(date.ToString("yyyy/MM/dd hh:mm:ss")) + 10800);
                        ///獲得的最近出貨時間默認為12:00 加上10800 代碼再加3小時 代表 15：00 作為一天是否有效的標準判斷
                        if (date == DateTime.MinValue || date == null || tsDate > 0)///如果得到的最近出貨天數  為最小時間   或者   小于當前時間
                        {
                            isSchedule = false;
                            date = payDay;///則按下單時間開始計算到貨日期
                        }
                        TimeSpan ts = date - payDay; ///預計出貨天數 - 下單時間(中間購買者要等的時間)
                        expendDay = ts.Days + days;///(總共用戶要等的天數)
                        if (expendDay > diffDay)
                        {
                            diffDay = expendDay;
                            lastDateTime = date;///最終確定的出貨時間
                            numDays = days;///運輸天數
                        }
                    }
                }
                if (itemIdFlag == 0)
                {
                    msgFlag = -4;
                    return DateTime.MinValue;
                }
                lastDateTime = restDay(lastDateTime, payDay, numDays, isSchedule);///計算具體到貨日期
                msgFlag = 1;///正確返回結果
                return lastDateTime;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                msg = ex.Message;
                msgFlag = -2;///錯誤變量 -2:其他錯誤異常!請參考系統錯誤日誌
                return DateTime.MinValue;
            }
        }


        private int IsAddDayFun(DateTime dt, DateTime isAddDay, int days, bool schedule)
        {
            long num_date = CommonFunction.GetPHPTime(dt.ToString());///獲得基本發貨時間的基數
            string dtFlagStr = isAddDay.GetDateTimeFormats()[0] + " 15:00";///計算出是否+1天的時間邊
            long dtFlag = CommonFunction.GetPHPTime(dtFlagStr);///獲得當天15：00的時間蹉
            long isAddDayInt = CommonFunction.GetPHPTime(isAddDay.ToString("yyyy-MM-dd HH:mm:ss"));///獲得下單時間的時間戳
            DateTime dateBase = dt.Date;///獲得預計出貨日的日期部份
            DateTime dateDay = isAddDay.Date;///獲得下單日期的日期部份
            if (dateBase != dateDay && schedule==true)
            {
                return days;
            }
            else
            {
                return days = (isAddDayInt - dtFlag) > 0 ? days + 1 : days;  //判斷時間是否大於15點,大於時,運達天數加1
            }
        }

    }
}
