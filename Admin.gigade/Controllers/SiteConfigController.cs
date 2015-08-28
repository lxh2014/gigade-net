using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using Admin.gigade.CustomError;
using BLL.gigade.Model;
using BLL.gigade.Common;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class SiteConfigController : Controller
    {
        //
        // GET: /SiteConfig/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        private ISiteConfigImplMgr siteConfigMgr;

        public ActionResult Index()
        {
            return View();
        }

        [CustomHandleError]
        [HttpPost]
        public JsonResult QueryConfig()
        {
            List<SiteConfig> configs = new List<SiteConfig>();
            try
            {
                string path = Server.MapPath(xmlPath);
                if (System.IO.File.Exists(path))
                {
                    siteConfigMgr = new SiteConfigMgr(path);
                    configs = siteConfigMgr.Query();
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json(configs);
        }

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase UpdateConfig()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Name"]))
                {
                    SiteConfig newConfig = new SiteConfig { Name = Request.Form["Name"], Value = Request.Form["Value"] ?? "" };
                    string path = Server.MapPath(xmlPath);
                    if (System.IO.File.Exists(path))
                    {
                        siteConfigMgr = new SiteConfigMgr(path);
                        if (siteConfigMgr.UpdateNode(newConfig))
                        {
                            json = "{success:true}";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
    }
}
