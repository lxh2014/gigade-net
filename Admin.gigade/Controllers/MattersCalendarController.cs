using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class MattersCalendarController : Controller
    {
        //
        // GET: /MattersCalendar/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        ICalendarImplMgr _calendarMgr = new CalendarMgr(mySqlConnectionString);
        //ICalendarImplMgr _calendarMgr = new CalendarMgr("Server=127.0.0.1;database=test;user=root;pwd=198929;charset='utf8'");

        public ActionResult Extensible()
        {
            return View();
        }

        public HttpResponseBase Query()
        {
            string json = "";
            List<Calendar> list = _calendarMgr.Query();
            json = "{evts:" + JsonConvert.SerializeObject(list) + "}";
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public ActionResult OperateEvent()
        {

            string star = Convert.ToDateTime(CommonFunction.GetNetTime(Convert.ToInt64(Request["startTime"]))).ToString("yyyy-MM-dd") + " 00:00:00";
            string end = Convert.ToDateTime(CommonFunction.GetNetTime(Convert.ToInt64(Request["endTime"]))).ToString("yyyy-MM-dd") + " 00:00:00";
            DateTime dtStar = Convert.ToDateTime(star);
            DateTime dtEnd = Convert.ToDateTime(end).AddHours(24).AddSeconds(-1);
            Calendar c = new Calendar();
            string type = Request["paraType"];
            c.StartDateStr = CommonFunction.GetPHPTime(dtStar.ToString()).ToString();
            c.EndDateStr = CommonFunction.GetPHPTime(dtEnd.ToString()).ToString();
            c.id = Convert.ToUInt32(Request["id"]);
            c.Title = Request["title"];
            c.Notes = Request["notes"];
            c.CalendarId = Convert.ToInt32(Request["calendarId"]);

            ///開始時間和結束時間都不是當天零點的時間,將開始和結束時間轉換為當天零點時間
            switch(type)
            {
                case "eventadd":
                    return Json(_calendarMgr.Save(c));
                case "delete":
                    return Json(_calendarMgr.Delete(c));
                case "eventupdate":
                    return Json(_calendarMgr.Update(c));
                default:
                    return Json(false);
            }
        }

    }
}
