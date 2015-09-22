using BLL.gigade.Mgr;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class EdmNewController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private static EdmContentNewMgr _edmContentNewMgr;

        //
        // GET: /EdmNew/
        #region view
        public ActionResult Index()
        {
            return View();
        }
        //電子報
        public ActionResult EdmContentNew()
        {
            return View();
        }
        #endregion

        #region 電子報類型

        #region 電子報類型列表頁

        #endregion

        #region 電子報類型新增編輯

        #endregion

        #endregion

        #region 電子報範本

        #region  電子報範本列表頁

        #endregion

        #region  電子報範本新增編輯

        #endregion


        #endregion

        #region 電子報
       
        #region 電子報列表
        public HttpResponseBase GetECNList()
        {
            string json = string.Empty;
            try
            {
                EdmContentNew query = new EdmContentNew();
                List<EdmContentNew> store = new List<EdmContentNew>();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                store = _edmContentNewMgr.GetECNList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 電子報新增編輯
        public HttpResponseBase SaveEdmContentNew()
        {
            string json = string.Empty;
            EdmContentNew query = new EdmContentNew();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    query.content_id = Convert.ToInt32(Request.Params["content_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["sender_id"]))
                {
                    query.sender_id = Convert.ToInt32(Request.Params["sender_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["inportance"]))
                {
                    query.importance = Convert.ToInt32(Request.Params["inportance"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["subject"]))
                {
                    query.subject = Request.Params["subject"];
                }
                if (!string.IsNullOrEmpty(Request.Params["template_id"]))
                {
                    query.template_id = Convert.ToInt32(Request.Params["template_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["template_data"]))
                {
                    query.template_data = Request.Params["template_data"];
                }
                 json=_edmContentNewMgr.SaveEdmContentNew(query);
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
        #endregion

        #region store
        public HttpResponseBase GetEdmGroupNewStore()
        {
            string json = string.Empty;
            try
            {
                List<EdmGroupNew> store = new List<EdmGroupNew>();
                _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                store = _edmContentNewMgr.GetEdmGroupNewStore();
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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



        #endregion

        #endregion

        #region 擋信名單管理

        #endregion

    }
}
