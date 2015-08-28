using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using BLL.gigade.Mgr.Impl;
namespace Admin.gigade.Controllers
{
    [HandleError]
    public class LogInLogeController : Controller
    {
        //
        // GET: /LogInLoge/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private ILogInLogeImplMgr logInLogeMgr;

        public ActionResult Index()
        {
            return View();
        }

        [CustomHandleError]
        public HttpResponseBase QueryLogIn()
        {
            string jsonStr = string.Empty;
            LogInLogeQuery logInLogeQuery = new LogInLogeQuery();

            try
            {
                logInLogeQuery.Start = Convert.ToInt32(Request.Form["start"] ?? "0");
                logInLogeQuery.Limit = Convert.ToInt32(Request.Form["limit"] ?? "20");
                logInLogeMgr = new LogInLogeMgr(connectionString);
                int totalCount;
                List<LogInLogeQuery> querys = logInLogeMgr.QueryList(logInLogeQuery,out totalCount);

                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(querys) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "[]";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

    }
}
