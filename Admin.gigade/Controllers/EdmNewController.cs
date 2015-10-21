﻿using BLL.gigade.Mgr;
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
using System.Net;
using System.IO;
using System.Text;
using System.Data;
using System.Net.Mail;
using BLL.gigade.Common;
namespace Admin.gigade.Controllers
{
    public class EdmNewController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private static EdmContentNewMgr _edmContentNewMgr;
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
        public EdmGroupNewMgr edmgroupmgr;
        public EdmTemplateMgr edmtemplatemgr;        //
        public EmailBlockListMgr _emailBlockListMgr;
        private EmailGroupMgr _emailGroupMgr;
      
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
        //擋信名單
        public ActionResult EmailBlockList()
        {
            return View();
        }

        public ActionResult EmailGroup()
        {
            return View();
        }

        public ActionResult EdmContentNewReport()
        {
            int content_id = Convert.ToInt32(Request.Params["content_id"]);
            ViewBag.contentId = content_id;
            return View();
        }

        public ActionResult EdmSendListCountView()
        {
            int content_id = Convert.ToInt32(Request.Params["content_id"]);
            ViewBag.contentId = content_id;
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
                // timeConverter.DateTimeFormat = "yyyy-MM-dd";
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
                edmgroupmgr = new EdmGroupNewMgr(mySqlConnectionString);
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
                    query.trial_url = Request.Params["trial_url"];
                }
                if (!string.IsNullOrEmpty(Request.Params["sort_order"]))
                {
                    query.sort_order = Convert.ToInt32(Request.Params["sort_order"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["description"]))
                {
                    query.description = Request.Params["description"];
                }

                query.group_create_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.group_update_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
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
                // timeConverter.DateTimeFormat = "yyyy-MM-dd ";
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
                //if (!string.IsNullOrEmpty(Request.Params["template_createdate"]))
                //{
                //    query.template_createdate = Convert.ToDateTime(Request.Params["template_createdate"]);
                //}
                //query.template_updatedate = System.DateTime.Now;
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
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
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
                query.content_update_userid = (Session["caller"] as Caller).user_id;
                json = _edmContentNewMgr.SaveEdmContentNew(query);
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

        #region edit_url
        public HttpResponseBase GetEditUrlData()
        {
            string json = string.Empty;
            try
            {
                #region 獲取edit_url
                if (!string.IsNullOrEmpty(Request.Params["edit_url"]))
                {
                    #region 獲取網頁內容方法
                    string url = Request.Params["edit_url"].ToString();
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.Timeout = 5000;
                    httpRequest.Method = "GET";
                    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                    json = sr.ReadToEnd();
                    #endregion
                }
                #endregion

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "獲取網頁出現異常！";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion


        #region ContentUrl
        public HttpResponseBase GetContentUrl()
        {
            string json = string.Empty;
            string template_data = string.Empty;
            string contentJson = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["content_url"]))
                {
                    #region 獲取網頁內容方法
                    string url = Request.Params["content_url"].ToString();
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpRequest.Timeout = 5000;
                    httpRequest.Method = "GET";
                    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    StreamReader sr = new StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                    contentJson = sr.ReadToEnd();
                    #endregion
                }
                if (!string.IsNullOrEmpty(Request.Params["template_data"]))
                {
                    template_data = Request.Params["template_data"];
                }
                json = contentJson + " " + template_data;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "獲取網頁出現異常！";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 發送電子報 （測試發送/正式發送）
        public HttpResponseBase SendEdm()
        {
            string json = string.Empty;
            EdmSendLog eslQuery = new EdmSendLog();
            MailRequest mQuery = new MailRequest();
            try
            {
                _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    eslQuery.content_id = mQuery.content_id = Convert.ToInt32(Request.Params["content_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    mQuery.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["sender_email"]))
                {
                    mQuery.sender_address = Request.Params["sender_email"];
                }
                if (!string.IsNullOrEmpty(Request.Params["sender_name"]))
                {
                    mQuery.sender_name = Request.Params["sender_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["subject"]))
                {
                    mQuery.subject = Request.Params["subject"];
                }
                if (!string.IsNullOrEmpty(Request.Params["body"]))
                {
                    mQuery.body = Request.Params["body"];
                }

                eslQuery.create_userid = (Session["caller"] as Caller).user_id;
                mQuery.user_id = eslQuery.create_userid;
                if (!string.IsNullOrEmpty(Request.Params["testSend"]))
                {
                    if (Request.Params["testSend"] == "true")//測試發送，只發送給自己
                    {
                        eslQuery.test_send_end = true;
                        #region 字段賦值
                        eslQuery.test_send = 1;
                        eslQuery.receiver_count = 1;
                        eslQuery.schedule_date = DateTime.Now;
                        eslQuery.expire_date = eslQuery.schedule_date.AddHours(1);
                        eslQuery.createdate = eslQuery.schedule_date;

                        mQuery.receiver_address = (Session["caller"] as Caller).user_email;
                        mQuery.receiver_name = (Session["caller"] as Caller).user_username;
                        mQuery.schedule_date = eslQuery.schedule_date;
                        mQuery.valid_until_date = eslQuery.expire_date;
                        mQuery.retry_count = 0;
                        mQuery.next_send = eslQuery.schedule_date;
                        mQuery.max_retry = 1;

                        #endregion
                        MailHelper mail = new MailHelper();
                        mail.SendMailAction((Session["caller"] as Caller).user_email, mQuery.subject, mQuery.body + "   ");
                        {
                            json = _edmContentNewMgr.MailAndRequest(eslQuery, mQuery);
                        }
                    }
                    else//正式發送，寫入排程所用表
                    {
                        eslQuery.test_send_end = false;
                        eslQuery.test_send = 0;
                        //eslQuery.receiver_count=""; 經計算後寫入
                        if (!string.IsNullOrEmpty(Request.Params["schedule_date"]))
                        {
                            eslQuery.schedule_date = Convert.ToDateTime(Request.Params["schedule_date"]);
                            mQuery.schedule_date = eslQuery.schedule_date;
                        }

                        if (!string.IsNullOrEmpty(Request.Params["email_group_id"]))
                        {
                            eslQuery.email_group_id = Convert.ToInt32(Request.Params["email_group_id"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["expire_date"]))
                        {
                            eslQuery.expire_date = Convert.ToDateTime(Request.Params["expire_date"]);
                            mQuery.valid_until_date = eslQuery.expire_date;
                        }
                        eslQuery.createdate = DateTime.Now;
                        if (!string.IsNullOrEmpty(Request.Params["elcm_id"]))
                        {
                            eslQuery.elcm_id = Convert.ToInt32(Request.Params["elcm_id"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["extra_send"]))
                        {
                            mQuery.extra_send = Request.Params["extra_send"];
                        }
                        if (!string.IsNullOrEmpty(Request.Params["extra_no_send"]))
                        {
                            mQuery.extra_no_send = Request.Params["extra_no_send"];
                        }
                        if (!string.IsNullOrEmpty(Request.Params["is_outer"]))
                        {
                            if (Request.Params["is_outer"] == "true")
                            {
                                mQuery.is_outer = true;
                            }
                            else
                            {
                                mQuery.is_outer = false;
                            }
                        }
                        json = _edmContentNewMgr.MailAndRequest(eslQuery, mQuery);
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
        #endregion

        #endregion

        #region 擋信名單管理
        public HttpResponseBase GetEmailBlockList()
        {
            string json = string.Empty;
            try
            {
                _emailBlockListMgr = new EmailBlockListMgr(mySqlConnectionString);
                DataTable store = new DataTable();
                EmailBlockListQuery query = new EmailBlockListQuery();
                if (!string.IsNullOrEmpty(Request.Params["email"]))
                {
                    query.email_address = Request.Params["email"];
                }
                store = _emailBlockListMgr.GetEmailBlockList(query);
                if (store != null)
                {
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd";
                    json = "{success:true" + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase AddorEdit()
        {
            string json = string.Empty;
            _emailBlockListMgr = new EmailBlockListMgr(mySqlConnectionString);
            try
            {
                EmailBlockListQuery query = new EmailBlockListQuery();
                query.block_update_userid = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                if (!string.IsNullOrEmpty(Request.Params["reason"]))
                {
                    query.block_reason = Request.Params["reason"].ToString().Replace("\\", "\\\\"); ;
                    query.block_create_userid = query.block_update_userid;
                    if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                    {
                        query.email_address = Request.Params["email_address"].ToString().Replace("\\", "\\\\"); ;
                    }
                    int i = _emailBlockListMgr.Add(query);
                    if (i > 0)
                    {
                        json = "{success:true}";
                    }
                    else if (i == -1)
                    {
                        json = "{success:false,msg:\'0\'}"; //郵箱已添加過
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["block_reason"]))
                    {
                        query.block_reason = Request.Params["block_reason"].ToString().Replace("\\", "\\\\"); ;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                    {
                        query.email_address = Request.Params["email_address"].ToString();
                    }
                    int i = _emailBlockListMgr.Update(query);
                    if (i > 0)
                    {
                        json = "{success:true}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase UnBlock()
        {
            string json = string.Empty;
            bool result = false;
            try
            {
                _emailBlockListMgr = new EmailBlockListMgr(mySqlConnectionString);
                EmailBlockLogQuery query = new EmailBlockLogQuery();
                if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                {
                    query.email_address = Request.Params["email_address"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["unblock_reason"]))
                {
                    query.unblock_reason = Request.Params["unblock_reason"].ToString().Replace("\\", "\\\\"); ;
                }
                query.unblock_create_userid = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                result = _emailBlockListMgr.UnBlock(query);
                if (result)
                {
                    json = "{success:true}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }




        #endregion

        #region 信箱名單管理
        #region 列表頁
        public HttpResponseBase EmailGroupList()
        {
            string json = string.Empty;

            try
            {
                EmailGroup query = new EmailGroup();
                List<EmailGroup> store = new List<EmailGroup>();
                if (!string.IsNullOrEmpty(Request.Params["group_name"]))
                {
                    query.group_name = Request.Params["group_name"];
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                _emailGroupMgr = new EmailGroupMgr(mySqlConnectionString);
                store = _emailGroupMgr.EmailGroupList(query, out totalCount);
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

        #region 新增/編輯
        public HttpResponseBase SaveEmailGroup()
        {
            string json = string.Empty;
            EmailGroup query = new EmailGroup();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_name"]))
                {
                    query.group_name =Request.Params["group_name"];
                }
                query.group_create_userid = (Session["caller"] as Caller).user_id;
                query.group_update_userid = (Session["caller"] as Caller).user_id;
                _emailGroupMgr = new EmailGroupMgr(mySqlConnectionString);
                if (_emailGroupMgr.SaveEmailGroup(query))
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 匯入
        public HttpResponseBase ImportExcel()
        {
            string json = string.Empty;
            string excelPath = "../Template/EmailGroup/";
            try
            {
                if (Request.Files.Count > 0)
                {
                    int group_id = Convert.ToInt32(Request.Params["group_id"]);
                    HttpPostedFileBase excelFile = Request.Files["ImportExcel"];
                    FileManagement fileManagement = new FileManagement();
                    string newExcelName = Server.MapPath(excelPath) + "email_group" + fileManagement.NewFileName(excelFile.FileName);
                    excelFile.SaveAs(newExcelName);
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    DataTable _dt = helper.ExcelToTableForXLSX();
                    
                    _emailGroupMgr = new EmailGroupMgr(mySqlConnectionString);
                    if (_emailGroupMgr.ImportEmailList(_dt, group_id))
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
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
        #endregion

        #region 匯出
        public void ExportExcel()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                   _emailGroupMgr = new EmailGroupMgr(mySqlConnectionString);
                  DataTable _dt=  _emailGroupMgr.Export(Convert.ToInt32(Request.Params["group_id"]));
                  DataTable _newDt = new DataTable();
                  _newDt.Columns.Add("群組代碼", typeof(string));
                  _newDt.Columns.Add("電子信箱地址", typeof(string));
                  _newDt.Columns.Add("收件人名稱", typeof(string));
                  for (int i = 0; i < _dt.Rows.Count; i++)
                  {
                      DataRow newRow = _newDt.NewRow();
                      newRow[0] = _dt.Rows[i]["group_id"];
                      newRow[1] = _dt.Rows[i]["email_address"];
                      newRow[2] = _dt.Rows[i]["name"];
                      _newDt.Rows.Add(newRow);
                  }
                  string fileName = "email_group" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                  MemoryStream ms = ExcelHelperXhf.ExportDT(_newDt, "");
                  Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                  Response.BinaryWrite(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion

        #region 模版

        public void ExportTemplateExcel()
        {
            try
            {
                DataTable _dt = new DataTable();
                _dt.Columns.Add("電子信箱地址", typeof(string));
                _dt.Columns.Add("收件人名稱", typeof(string));
                DataRow newRow = _dt.NewRow();
                _dt.Rows.Add();
                string fileName = "email_group_template" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(_dt, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }

        #endregion

        #region store
        public HttpResponseBase EmailGroupStore()
        {
            string json = string.Empty;
            try
            {
                EmailGroup query = new BLL.gigade.Model.EmailGroup();
                query.group_id = 0;
                query.group_name = "無";
                List<EmailGroup> store = new List<EmailGroup>();
                _emailGroupMgr = new EmailGroupMgr(mySqlConnectionString);
                store = _emailGroupMgr.EmailGroupStore();
                store.Insert(0, query);
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

        #region 報表

        #region form數據加載
        public HttpResponseBase Load()
        {
            string json = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["content_id"]))
            {
                int openAveragePrecent = 0;
                int openAverageCount = 0;
                int content_id = Convert.ToInt32(Request.Params["content_id"]);
                List<EdmContentNew> store = new List<BLL.gigade.Model.EdmContentNew>();
                EdmContentNew query = new BLL.gigade.Model.EdmContentNew();
                EdmContentNew newQuery = new BLL.gigade.Model.EdmContentNew();
                query.content_id = content_id;
                int count = 0;
                //電子報主旨和發送時間
                newQuery = _edmContentNewMgr.GetECNList(query, out count).FirstOrDefault();
                string date = newQuery.date.ToString("yyyy-MM-dd HH:mm:ss");
                string subject = newQuery.subject;
                //發信成功人數
                int successCount = _edmContentNewMgr.GetSendMailSCount(content_id);
                //發信失敗人數
                int failCount = _edmContentNewMgr.GetSendMailFCount(content_id);
                //總開信人數
                int totalPersonCount = _edmContentNewMgr.GetSendMailCount(content_id);
                //開總信次數
                int totalCount = _edmContentNewMgr.GetSendCount(content_id);
                //開信率
                if (successCount == 0)
                {
                    openAveragePrecent = 0;
                }
                else
                {
                    openAveragePrecent = totalPersonCount / successCount;
                }
                //平均開信次數
                if (totalPersonCount == 0)
                {
                    openAverageCount = 0;
                }
                else
                {
                    openAverageCount = totalCount / totalPersonCount;
                }
                json = "{success:true,successCount:'" + successCount + "',failCount:'" + failCount + "',totalPersonCount:'" + totalPersonCount + "',totalCount:'" + totalCount + "',openAveragePrecent:'" + openAveragePrecent + "',openAverageCount:'" + openAverageCount + "',subject:'" + subject + "',date:'" + date + "'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 開信名單下載
        public void ImportKXMD()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                    DataTable _dt = _edmContentNewMgr.KXMD(Convert.ToInt32(Request.Params["content_id"]));
                    //DataTable _newDt = new DataTable();
                    //_newDt.Columns.Add("群組代碼", typeof(string));
                    //_newDt.Columns.Add("電子信箱地址", typeof(string));
                    //_newDt.Columns.Add("收件人名稱", typeof(string));
                    //for (int i = 0; i < _dt.Rows.Count; i++)
                    //{
                    //    DataRow newRow = _newDt.NewRow();
                    //    newRow[0] = _dt.Rows[i]["group_id"];
                    //    newRow[1] = _dt.Rows[i]["email_address"];
                    //    newRow[2] = _dt.Rows[i]["name"];
                    //    _newDt.Rows.Add(newRow);
                    //}
                    string fileName = "開信名單" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(_dt, "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion

        #region 未開信名單下載
        public void ImportWKXMD()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                    DataTable _dt = _edmContentNewMgr.WKXMD(Convert.ToInt32(Request.Params["content_id"]));
                    //DataTable _newDt = new DataTable();
                    //_newDt.Columns.Add("群組代碼", typeof(string));
                    //_newDt.Columns.Add("電子信箱地址", typeof(string));
                    //_newDt.Columns.Add("收件人名稱", typeof(string));
                    //for (int i = 0; i < _dt.Rows.Count; i++)
                    //{
                    //    DataRow newRow = _newDt.NewRow();
                    //    newRow[0] = _dt.Rows[i]["group_id"];
                    //    newRow[1] = _dt.Rows[i]["email_address"]; 
                    //    newRow[2] = _dt.Rows[i]["name"];
                    //    _newDt.Rows.Add(newRow);
                    //}
                    string fileName = "未開信名單" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(_dt, "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        #endregion

        #region 發信名單統計
        public HttpResponseBase EdmSendListCount()
        {
            string json = string.Empty;
            try
            {
                EdmTrace query = new EdmTrace();
                if (!string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    query.content_id = Convert.ToInt32(Request.Params["content_id"]);
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                DataTable _dt = _edmContentNewMgr.FXMD(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";
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

        #region 電子報統計報表
         public HttpResponseBase EdmContentNewReportList()
        {
            string json = string.Empty;
            try
            {
                EdmTrace query = new EdmTrace();
                if (!string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    query.content_id = Convert.ToInt32(Request.Params["content_id"]);
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                _edmContentNewMgr = new EdmContentNewMgr(mySqlConnectionString);
                DataTable _dt = _edmContentNewMgr.EdmContentNewReportList(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";
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
        #endregion
    }
}
