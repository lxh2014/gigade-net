using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using System.Configuration;
using BLL.gigade.Mgr;
using Newtonsoft.Json;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class ParameterController : Controller
    {
        //
        // GET: /Prameter/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];

        private IParametersrcImplMgr _paraMgr;

        [CustomHandleError]
        [OutputCache(Duration = 3600, VaryByParam = "paraType", Location = System.Web.UI.OutputCacheLocation.Client)]
        public string QueryPara()
        {
            _paraMgr = new ParameterMgr(connectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    json = _paraMgr.Query(Request.QueryString["paraType"].ToString());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            return json;
        }

        /// <summary>
        /// 獲取品類分類一級分類
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetProCage1()
        {
            string resultStr = "{success:false}";
            try
            {
                _paraMgr = new ParameterMgr(connectionString);
                resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_cate" }).Where(p => p.TopValue == "0").ToList()) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 獲取品類分類二級分類
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client, VaryByParam = "topValue")]
        public HttpResponseBase GetProCage2()
        {
            string resultStr = "{success:false}";
            try
            {
                string topValue = Request.Params["topValue"] ?? "";
                _paraMgr = new ParameterMgr(connectionString);
                resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_cate", TopValue = topValue })) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }

        public ActionResult QueryParaByXml(string paraType)
        {
            string strXml = "../XML/ParameterSrc.xml";
            _paraMgr = new ParameterMgr(Server.MapPath(strXml), ParaSourceType.XML);
            var list = _paraMgr.QueryUsed(new Parametersrc { ParameterType = paraType });
            return Json(list);
        }

        //根據 ParameterType 和 TopValue 查找相關數據 add by zhuoqin0830w  2015/06/26
        public JsonResult QueryParaByXmlTop(string paraType, string topValue)
        {
            JsonResult result = null;
            try
            {
                string strXml = "../XML/ParameterSrc.xml";
                _paraMgr = new ParameterMgr(Server.MapPath(strXml), ParaSourceType.XML);
                result = Json(_paraMgr.QueryUsed(new Parametersrc { ParameterType = paraType }).Where(x => x.TopValue == topValue).ToList());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }

        //add by zhuoqin0830w  2015/04/09  根據 TopValue 獲取 部門（數據庫）
        public JsonResult GetDeptByTopValue()
        {
            Parametersrc p = new Parametersrc();
            JsonResult result = null;
            try
            {
                _paraMgr = new ParameterMgr(connectionString);
                p.ParameterType = "dep";
                if (!string.IsNullOrEmpty(Request.Form["TopValue"]))
                { p.TopValue = Request.Form["TopValue"]; }
                result = Json(_paraMgr.QueryForTopValue(p));
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }
    }
}