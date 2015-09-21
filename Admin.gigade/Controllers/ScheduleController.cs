using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class ScheduleController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        // GET: /Schedule/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 遍歷Schedule表；
        /// </summary>
        /// <returns></returns>
        //public HttpResponseBase WindowsService()
        //{
        //    List<EmsGoalQuery> store = new List<EmsGoalQuery>();
        //    EmsGoalQuery query = new EmsGoalQuery();
        //    int totalCount = 0;
        //    string json = string.Empty;
        //    _IEmsMgr = new EmsMgr(mySqlConnectionString);
        //    query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
        //    query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["departgoal"]))
        //        {
        //            query.department_code = Request.Params["departgoal"].ToString();
        //        }
        //        //if (!string.IsNullOrEmpty(Request.Params["searchDategoal"]))
        //        //{
        //        //    query.searchdate = Convert.ToInt32(Request.Params["searchDategoal"].ToString());
        //        //}
        //        if (!string.IsNullOrEmpty(Request.Params["dategoal"]))
        //        {
        //            query.date = Convert.ToDateTime(Request.Params["dategoal"].ToString());
        //        }
        //        store = _IEmsMgr.GetEmsGoalList(query, out totalCount);
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,totalCount:0,data:[]}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}

    }
}
