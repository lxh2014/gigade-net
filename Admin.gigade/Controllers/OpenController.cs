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
        private DateTime restDay(DateTime dt, DateTime isAddDay, int days)
        {
            long num_date = CommonFunction.GetPHPTime(dt.ToString());
            string dtFlagStr =isAddDay.GetDateTimeFormats()[0] + " 15:00";
            long  dtFlag = CommonFunction.GetPHPTime(dtFlagStr);///獲得當天15：00的時間蹉
            long isAddDayInt = CommonFunction.GetPHPTime(isAddDay.ToString("yyyy-MM-dd HH:mm:ss"));
            days = (isAddDayInt - dtFlag) > 0 ? days + 1 : days;  //判斷時間是否大於15點,大於時,運達天數加1
            ICalendarImplMgr _cdMgr = new CalendarMgr(connectionString);
            List<Calendar> calendar_list = _cdMgr.GetCalendarInfo(new Calendar { EndDateStr = num_date.ToString() }); ///獲取行事歷控件中休息時間的集合
            return VerifyTime(dt, days, calendar_list);
        }


        public DateTime VerifyTime(DateTime dt, int days, List<Calendar> calendar_list)
        {
            /// 遞歸調用計算運達天數 add by wwei0216w 2015/5/26
             long num_date = CommonFunction.GetPHPTime(dt.ToString());
            List<Calendar> result_list = new List<Calendar>();
            result_list = calendar_list.FindAll(m => (num_date >= Convert.ToInt64(m.StartDateStr)) && (num_date <= Convert.ToInt64(m.EndDateStr)));
            if (days != 0 && result_list.Count > 0)
            {
                dt = dt.AddDays(1);
                dt = VerifyTime(dt, days, calendar_list);
                return dt;
            }
            else if (days != 0 && result_list.Count == 0)
            {
                dt = dt.AddDays(1);
                days--;
                dt = VerifyTime(dt, days, calendar_list);
                return dt;
            }
            else if (days == 0 && (result_list.Count > 0))
            {
                dt = dt.AddDays(1);
                dt = VerifyTime(dt, days, calendar_list);
                return dt;
            }
            else
            {
                return dt;
            }
        }

        public ActionResult ArriveTime(uint itemId,string dateTime)
        {
            //add by wwei0216w 2015/5/25
            int days = 0;
            int Deliver_Days = 0;
            int isSuccess = 1;
            DateTime implementTime = DateTime.Now;
            DateTime date = DateTime.MinValue;
            string msg = "";
            DateTime dtTime = DateTime.Now;
            if (!string.IsNullOrEmpty(dateTime))
            {
                if (!DateTime.TryParse(dateTime, out dtTime))
                {
                    isSuccess = 0;
                    msg = "Time Exception!";
                    return Json(new { data = date.ToString("yyyy-MM-dd"), success = isSuccess, errMsg = msg, des = "貨物運達時間", execTime = implementTime.ToString("yyyy/MM/dd"), elapsed = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                _srMgr = new ScheduleRelationMgr(connectionString);
                ProductItemCustom pi = new ProductItemCustom();
                IProductItemImplMgr _productItemMgr = new ProductItemMgr(connectionString);
                IOrderDetailImplMgr _orderDetailMgr = new OrderDetailMgr(connectionString);
                IProductImplMgr _pMgr = new ProductMgr(connectionString);
                //OrderDetailCustom od = _orderDetailMgr.GetArriveDay(detailId).FirstOrDefault();///獲得訂單中關於天數的信息(供應商出貨天數,運達天數....)
                //if(od.item_mode==0) //如果是單一商品(既item_mode=0,parent_id = 0)則根據item_id來獲取計算運達天數的信息
                //{
                pi = _productItemMgr.GetProductArriveDay(new ProductItem { Item_Id = itemId},"item");
                Deliver_Days = _pMgr.GetDefaultArriveDays(new Product { Product_Id = pi.Product_Id });///獲取出貨時間
                //}
                //else if (od.item_mode != 0)//如果是組合商品(既item_mode !=0,有parent_id的值)則更具parent_id來計算運達天數
                //{
                    //isSuccess = 0;
                    //msg = "組合商品不再計算範圍之內";
                //}
                if(pi==null || pi.Product_Id==0)
                {
                    isSuccess = 0;
                    msg = "不存在該itemId";
                    return Json(new { data = date.ToString("yyyy-MM-dd"), success = isSuccess, errMsg = msg, des = "貨物運達時間", execTime = implementTime.ToString("yyyy/MM/dd"), elapsed = 0 }, JsonRequestBehavior.AllowGet);
                }
                days = pi.Arrive_Days + Deliver_Days ;///計算運達天數
                date = _srMgr.GetRecentlyTime(Convert.ToInt32(pi.Product_Id), "product");///調用GetRecentlyTime()方法 獲得最近出貨時間

                //edit by wwei0216w 2015/6/2 添加item_stock > 0 的判斷,如果 item_stock > 0 則不適用排程的時間,啟用當前時間作為基礎時間
                if (date == DateTime.MinValue || date < DateTime.Now || date == null)///如果得到的最近出貨天數  為最小時間   或者   小于當前時間
                {
                    date = DateTime.Now;
                }

                DateTime isAddDay = dtTime == null ? implementTime : (DateTime)dtTime;///將時間賦值
                date = restDay(date,isAddDay, days);///計算具體到貨日期
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                isSuccess = 0;
                msg = ex.Message;

            }
            DateTime functionEndTime = DateTime.Now;
            TimeSpan ts = functionEndTime - implementTime;//獲得方法開始和結束的時間
            double second = ts.TotalMilliseconds;//方法執行的時間
            return Json(new { data = date.ToString("yyyy/MM/dd"), success = isSuccess, errMsg = msg, des = "貨物運達時間", execTime = implementTime.ToString("yyyy/MM/dd"), elapsed = second }, JsonRequestBehavior.AllowGet);
        }
    }
}
