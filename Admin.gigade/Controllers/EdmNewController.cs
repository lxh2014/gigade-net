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
using System.Net;
using System.IO;
using System.Text;
using System.Data;namespace Admin.gigade.Controllers
{
    public class EdmNewController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private static EdmContentNewMgr _edmContentNewMgr;
        public EdmGroupNewMgr edmgroupmgr;
        public EdmTemplateMgr edmtemplatemgr;        //
        public EmailBlockListMgr _emailBlockListMgr;        HttpWebRequest httpReq;
        HttpWebResponse httpResp;
        string strBuff = "";
        char[] cbuffer = new char[256];
        int byteRead = 0;        // GET: /EdmNew/
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

        #region edit_url
        public HttpResponseBase GetEditUrlData()
        {
            string json = string.Empty;
            try
            {
                #region 獲取edit_url
                string url = "http://net2005/DocSys/DocLogin/MainMenu.aspx";
                Uri httpURL = new Uri(url);
                //HttpWebRequest类继承于WebRequest，并没有自己的构造函数，需通过WebRequest的Creat方法 建立，并进行强制的类型转换
                httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
                //通过HttpWebRequest的GetResponse()方法建立HttpWebResponse,强制类型转换

                httpResp = (HttpWebResponse)httpReq.GetResponse();
                //GetResponseStream()方法获取HTTP响应的数据流,并尝试取得URL中所指定的网页内容
                //若成功取得网页的内容，则以System.IO.Stream形式返回，若失败则产生ProtoclViolationException错 误。在此正确的做法应将以下的代码放到一个try块中处理。这里简单处理
                Stream respStream = httpResp.GetResponseStream();
                //返回的内容是Stream形式的，所以可以利用StreamReader类获取GetResponseStream的内容，并以

                //StreamReader类的Read方法依次读取网页源程序代码每一行的内容，直至行尾（读取的编码格式：UTF8）
                StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);
                byteRead = respStreamReader.Read(cbuffer, 0, 256);
                while (byteRead != 0)
                {
                    string strResp = new string(cbuffer, 0, byteRead);
                    strBuff = strBuff + strResp;
                    byteRead = respStreamReader.Read(cbuffer, 0, 256);
                }
                respStream.Close();
                json = strBuff;
                #endregion
                #region 獲取content_url

                #endregion
                #region 將content_url中的佔位符替換為edit_url中的內容

                #endregion


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

    }
}
