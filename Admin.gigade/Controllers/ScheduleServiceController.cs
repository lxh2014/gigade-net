using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;

namespace Admin.gigade.Controllers
{
    public class ScheduleServiceController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private ScheduleServiceMgr _secheduleServiceMgr;
        // GET: /Schedule/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 獲取需要執行的排程列表；
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetExeScheduleServiceList()
        {
            //獲取需要執行的排程

            List<ScheduleServiceQuery> store = new List<ScheduleServiceQuery>();
            ScheduleServiceQuery query = new ScheduleServiceQuery();
            query.state = 1;

            _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
            
            try
            {

                store = _secheduleServiceMgr.GetExeScheduleServiceList(query);
                
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            //this.Response.Clear();
            //this.Response.Write();
            //this.Response.End();
            return this.Response;
        }

    }
}
