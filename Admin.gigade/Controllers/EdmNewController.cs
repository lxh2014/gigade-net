using BLL.gigade.Mgr;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Admin.gigade.Controllers
{
    public class EdmNewController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private static EdmContentNewMgr _edmContentNewMgr;
        public EdmGroupNewMgr edmgroupmgr;
        public EdmTemplateMgr edmtemplatemgr;        //
        // GET: /EdmNew/
        #region view
        //電子報類型
        public ActionResult Index()
        {
            return View();
        }
        //電子報範本
        public ActionResult EdmTemplate()
        {
            return View();
        }
        //電子報
        public ActionResult EdmContentNew()
        {
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            return View();
        }
        #endregion

        #region 電子報類型

        #region 電子報類型列表頁
        public HttpResponseBase GetEdmGroupNewList()// add by yachao1120j 2015-9-21
        {
            string json = string.Empty;
            int totalcount = 0;
            try
            {
                EdmGroupNewQuery query = new EdmGroupNewQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                edmgroupmgr = new EdmGroupNewMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["group_name_list"]))
                {
                    query.group_name = Request.Params["group_name_list"];
                }
                List<EdmGroupNewQuery> list = edmgroupmgr.GetEdmGroupNewList(query, out totalcount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";
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
            return Response;
 
        }
        #endregion

        #region 狀態啟用 
        public HttpResponseBase UpdateStats()
        {
            string json = string.Empty;
            try
            {
                edmgroupmgr = new EdmGroupNewMgr (mySqlConnectionString);
                EdmGroupNewQuery query = new EdmGroupNewQuery();

                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.enabled = Convert.ToInt32(Request.Params["active"]);
                }
                json = edmgroupmgr.UpdateStatus(query);
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

        #region 電子報類型新增編輯
        public HttpResponseBase SaveEdmGroupNewAdd() //add by yachao1120j 2015-9-22
        {
            string json = string.Empty;
            try
            {
                EdmGroupNewQuery query = new EdmGroupNewQuery();
                edmgroupmgr = new EdmGroupNewMgr(mySqlConnectionString);
               
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_name"]))
                {
                    query.group_name = Request.Params["group_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["is_member_edm"]))
                {
                    query.is_member_edm = Convert.ToInt32(Request.Params["is_member_edm"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["trial_url"]))
                {
                    query.trial_url=Request.Params["trial_url"];
                }
                if (!string.IsNullOrEmpty(Request.Params["sort_order"]))
                {
                    query.sort_order = Convert.ToInt32(Request.Params["sort_order"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["description"]))
                {
                    query.description = Request.Params["description"];
                }
                int _dt = edmgroupmgr.SaveEdmGroupNewAdd(query);

                if (_dt > 0)
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false}";
                }
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
            return Response;
 
        }

        #endregion

        #endregion

        #region 電子報範本

        #region  電子報範本列表頁

        public HttpResponseBase GetEdmTemplateList() //add by yachao1120j 2015-9-22
        {
            string json = string.Empty;
            int totalcount = 0;
            try
            {
                EdmTemplateQuery query = new EdmTemplateQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                edmtemplatemgr = new EdmTemplateMgr(mySqlConnectionString);
                List<EdmTemplateQuery> list = edmtemplatemgr.GetEdmTemplateList(query, out totalcount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";
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
            return Response;
 
        }

        //EdmTemplate 中的狀態啟用
        public HttpResponseBase UpdateStats_ET()
        {
            string json = string.Empty;
            try
            {
                edmtemplatemgr = new EdmTemplateMgr(mySqlConnectionString);
                EdmTemplateQuery query = new EdmTemplateQuery();

                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.template_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.enabled = Convert.ToInt32(Request.Params["active"]);
                }
                json = edmtemplatemgr.UpdateStats_ET(query);
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

        #region  電子報範本新增編輯

        public HttpResponseBase SaveEdmTemplateAdd() //add by yachao1120j 2015-9-22
        {
            string json = string.Empty;
            try
            {
                EdmTemplateQuery query = new EdmTemplateQuery();
                edmtemplatemgr = new EdmTemplateMgr(mySqlConnectionString);

                if (!string.IsNullOrEmpty(Request.Params["template_id"]))
                {
                    query.template_id = Convert.ToInt32(Request.Params["template_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["template_name"]))
                {
                    query.template_name = Request.Params["template_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["edit_url"]))
                {
                    query.edit_url = Request.Params["edit_url"];
                }
                if (!string.IsNullOrEmpty(Request.Params["content_url"]))
                {
                    query.content_url = Request.Params["content_url"];
                }
                query.template_updatedate = System.DateTime.Now;
                query.template_update_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.template_create_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;


                int _dt = edmtemplatemgr.SaveEdmTemplateAdd(query);

                if (_dt > 0)
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false}";
                }
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
            return Response;

        }

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
                foreach (var item in store)
                {
                    item.template_data = Server.HtmlDecode(Server.HtmlDecode(item.template_data));
                }
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
                if (!string.IsNullOrEmpty(Request.Params["importance"]))
                {
                    query.importance = Convert.ToInt32(Request.Params["importance"]);
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
                query.content_create_userid = (Session["caller"] as Caller).user_id;
                query.content_update_userid= (Session["caller"] as Caller).user_id;
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

        public HttpResponseBase GetEdmTemplateStore()
        {
            string json = string.Empty;
            try
            {
                List<EdmTemplate> store = new List<EdmTemplate>();
                _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                store = _edmContentNewMgr.GetEdmTemplateStore();
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

        public HttpResponseBase GetMailSenderStore()
        {
            string json = string.Empty;
            try
            {
                List<MailSender> store = new List<MailSender>();
                _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                store = _edmContentNewMgr.GetMailSenderStore();
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
