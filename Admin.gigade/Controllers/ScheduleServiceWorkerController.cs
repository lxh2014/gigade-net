using BLL.gigade.Mgr.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    //排程控制器 add by yafeng0715j20151021PM
    public class ScheduleServiceWorkerController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        public ActionResult Index()
        {
            return View();
        }
        #region 黑貓物流狀態抓取排程 add by yafeng0715j 20151019AM
        public bool DeliverStatus()
        {
            DateTime startTime = DateTime.Now;
            if (string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                return false;
            }
            try
            {
                DeliverStatusMgr dsMgr = new DeliverStatusMgr(mySqlConnectionString);
                dsMgr.Start(Request.Params["schedule_code"], startTime);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return true;
        }

        #endregion
    }
}
