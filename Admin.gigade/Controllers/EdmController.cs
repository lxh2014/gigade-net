using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Admin.gigade.Controllers
{
    public class EdmController : Controller
    {
        //
        // GET: /Edm/
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IEdmContentImplMgr _IEdmContentMgr;
        private EdmContentMgr _edmContentMgr;
        private IEdmGroupEmailImpIMgr _IEdmGroupEmailMgr;
        private IEpaperContentImplMgr _epaperMgr;
        private EdmGroupMgr _edmGroup;
        private EdmSendMgr _edmSendMgr;
        private EdmEmailMgr _edmEmailMgr;
        private TFunctionMgr _tFunctonMgr;
        static string excelPath_export = ConfigurationManager.AppSettings["ImportUserIOExcel"];
        private VipUserMgr _vipuserMgr;
        static string excelPath = ConfigurationManager.AppSettings["ImportCompareExcel"];//關於導入的excel文件的限制
        private EdmTestMgr _etestMgr;
        HttpWebRequest httpReq;
        HttpWebResponse httpResp;
        string strBuff = "";
        char[] cbuffer = new char[256];
        int byteRead = 0;

        //string filename = @"c:\log.txt";
        #region View
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 電子報列表
        /// </summary>
        /// <returns></returns>
        public ActionResult EdmContentList()
        {
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];

            return View();
        }
        /// <summary>
        /// 新增電子報
        /// </summary>
        /// <returns></returns>
        public ActionResult EdmContentAdd()
        {
            ViewBag.EdmStore = Request.Params["EdmStore"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            return View();
        }
        /// <summary>
        /// 取消電子報
        /// </summary>
        /// <returns></returns>
        public ActionResult EdmCancel()
        {

            return View();
        }
        /// <summary>
        /// 測試名單
        /// </summary>
        /// <returns></returns>
        public ActionResult EdmTest()
        {
            return View();
        }
        /// <summary>
        /// 電子報群組名單
        /// </summary>
        /// <returns></returns>
        public ActionResult EdmGroupEmail()
        {
            if (!string.IsNullOrEmpty(Request.Params["group_id"]))
            {
                ViewBag.group_id = Request.Params["group_id"];
            }
            else
            {
                ViewBag.group_id = 1;
            }
            return View();
        }

        public ActionResult EdmGroupList()
        {
            return View();
        }

        /// <summary>
        /// 黑名單管理
        /// </summary>
        /// <returns></returns>
        public ActionResult BlackList()
        {
            return View();
        }

        /// <summary>
        /// 電子報統計報表
        /// </summary>
        /// <returns></returns>
        public ActionResult EdmStatisticsList()
        {
            if (!string.IsNullOrEmpty(Request.Params["cid"]))
            {
                ViewBag.cid = Request.Params["cid"];
            }
            else
            {
                ViewBag.cid = "1200";
            }
            return View();
        }
        /// <summary>
        /// 發信名單統計
        /// </summary>
        /// <returns></returns>
        public ActionResult EdmStatisticsSend()
        {
            if (!string.IsNullOrEmpty(Request.Params["cid"]))
            {
                ViewBag.cid = Request.Params["cid"];
            }
            else
            {
                ViewBag.cid = "1200";
            }
            return View();
        }
        /// <summary>
        /// 名單及送記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult SendRecordList()
        {
            if (!string.IsNullOrEmpty(Request.Params["eid"]))
            {
                ViewBag.eid = Request.Params["eid"];
            }
            return View();
        }
        /// <summary>
        /// 電子報人員名單列表
        /// </summary>
        /// <returns></returns>
        public ActionResult EdmPersonList()
        {
            return View();
        }
        /// <summary>
        /// 人員詳情名單
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonList()
        {
            if (!string.IsNullOrEmpty(Request.Params["email_id"]))
            {
                ViewBag.email_id = Request.Params["email_id"];
            }
            else
            {
                ViewBag.email_id = "3";
            }
            return View();
        }
        #endregion

        #region 電子報列表
        public HttpResponseBase GetEdmContentList()
        {
            string json = string.Empty;
            List<EdmContentQuery> store = new List<EdmContentQuery>();
            EdmContentQuery query = new EdmContentQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["searchStatus"]))
                {
                    query.searchStatus = Request.Params["searchStatus"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["search_text"]))
                {
                    query.search_text = Request.Params["search_text"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["tstart"]))
                {
                    query.s_content_start = Convert.ToDateTime(Request.Params["tstart"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["tend"]))
                {
                    query.s_content_end = Convert.ToDateTime(Request.Params["tend"]);
                }
                _IEdmContentMgr = new EdmContentMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _IEdmContentMgr.GetEdmContentList(query, out totalCount);
                foreach (var item in store)
                {
                    item.content_send_count = item.content_send_success + item.content_send_failed;
                    item.content_body = Server.HtmlDecode(Server.HtmlDecode(item.content_body));
                    item.s_content_start = CommonFunction.GetNetTime(item.content_start);
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

        #region 刪除電子報
        public HttpResponseBase DeleteEdm()
        {
            string json = string.Empty;
            _IEdmContentMgr = new EdmContentMgr(mySqlConnectionString);
            if (!string.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    foreach (string item in Request.Params["rowID"].Split('|'))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _IEdmContentMgr.DeleteEdm(Convert.ToInt32(item));
                        }
                    }
                    json = "{success:true}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:false}";
                }
            }
            else
            {
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 新增編輯電子報信息+HttpResponseBase EdmContentSave()
        /// <summary>
        /// 新增編輯電子報信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase EdmContentSave()
        {
            string json = string.Empty;
            string jsonStr = string.Empty;
            EdmContentQuery cq = new EdmContentQuery();
            _IEdmContentMgr = new EdmContentMgr(mySqlConnectionString);
            try
            {

                if (string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    //cq.content_id = Convert.ToUInt32(Request.Params["content_id"].ToString());
                    if (!string.IsNullOrEmpty(Request.Params["content_title"]))
                    {
                        cq.content_title = Request.Params["content_title"].ToString().Replace("\\", "\\\\");
                    }
                    if (!string.IsNullOrEmpty(Request.Params["kendoEditor"]))
                    {
                        cq.content_body = Request.Params["kendoEditor"].ToString().Replace("\\", "\\\\");
                    }
                    if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                    {
                        DateTime dt = Convert.ToDateTime(Request.Params["startdate"]);
                        cq.content_start = Convert.ToUInt32(CommonFunction.GetPHPTime(dt.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["content_from_name"]))
                    {
                        cq.content_from_name = Request.Params["content_from_name"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["content_from_email"]))
                    {
                        cq.content_from_email = Request.Params["content_from_email"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["content_reply_email"]))
                    {
                        cq.content_reply_email = Request.Params["content_reply_email"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                    {
                        cq.group_id = Convert.ToUInt32(Request.Params["group_id"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Request.Params["edm_dis"]))
                    {
                        cq.content_priority = Convert.ToUInt32(Request.Params["edm_dis"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_id"]))
                    {
                        int s = 0;
                        if (Int32.TryParse(Request.Params["epaper_id"], out s))
                        {
                            cq.info_epaper_id = s;
                        }

                    }
                    cq.content_email_id = 0;
                    cq.content_range = 300;
                    cq.content_single_count = 100;
                    cq.content_click = 0;
                    cq.content_person = 0;
                    cq.content_status = 1;
                    int i = _IEdmContentMgr.EdmContentSave(cq);
                    if (i > 0)
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    cq.content_id = Convert.ToUInt32(Request.Params["content_id"].ToString());
                    EdmContentQuery oldQuery = _IEdmContentMgr.GetEdmContentById(cq);
                    if (!string.IsNullOrEmpty(Request.Params["content_title"]))
                    {
                        cq.content_title = Request.Params["content_title"].ToString().Replace("\\", "\\\\"); ;
                    }
                    else
                    {
                        cq.content_title = oldQuery.content_title;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["kendoEditor"]))
                    {
                        cq.content_body = Request.Params["kendoEditor"].ToString().Replace("\\", "\\\\"); ;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                    {
                        DateTime dt = Convert.ToDateTime(Request.Params["startdate"]);
                        cq.content_start = Convert.ToUInt32(CommonFunction.GetPHPTime(dt.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                    else
                    {
                        cq.content_start = oldQuery.content_start;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["content_from_name"]))
                    {
                        cq.content_from_name = Request.Params["content_from_name"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["content_from_email"]))
                    {
                        cq.content_from_email = Request.Params["content_from_email"].ToString();
                    }
                    else
                    {
                        cq.content_from_email = oldQuery.content_from_email;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["content_reply_email"]))
                    {
                        cq.content_reply_email = Request.Params["content_reply_email"].ToString();
                    }
                    else
                    {
                        cq.content_reply_email = oldQuery.content_reply_email;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                    {
                        cq.group_id = Convert.ToUInt32(Request.Params["group_id"].ToString());
                    }
                    else
                    {
                        cq.group_id = oldQuery.group_id;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["edm_dis"]))
                    {
                        cq.content_priority = Convert.ToUInt32(Request.Params["edm_dis"].ToString());
                    }
                    else
                    {
                        cq.content_priority = oldQuery.content_priority;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_id"]))
                    {
                        int i = 0;
                        if (Int32.TryParse(Request.Params["epaper_id"], out i))
                        {
                            cq.info_epaper_id = i;
                        }

                    }
                    int j = _IEdmContentMgr.EdmContentSave(cq);
                    if (j > 0)
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
                json = "{success:true,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase EditStatus()
        {
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig Mail_From = _siteConfigMgr.GetConfigByName("Mail_From");
            SiteConfig Mail_Host = _siteConfigMgr.GetConfigByName("Mail_Host");
            SiteConfig Mail_Port = _siteConfigMgr.GetConfigByName("Mail_Port");
            SiteConfig Mail_UserName = _siteConfigMgr.GetConfigByName("Mail_UserName");
            SiteConfig Mail_UserPasswd = _siteConfigMgr.GetConfigByName("Mail_UserPasswd");
            string EmailFrom = Mail_From.Value;//發件人郵箱
            string SmtpHost = Mail_Host.Value;//smtp服务器
            string SmtpPort = Mail_Port.Value;//smtp服务器端口
            string EmailUserName = Mail_UserName.Value;//郵箱登陸名
            string EmailPassWord = Mail_UserPasswd.Value;//郵箱登陸密碼

            string json = string.Empty;
            EdmContentQuery query = new EdmContentQuery();
            int i = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    query.content_id = Convert.ToUInt32(Request.Params["content_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.content_status = Convert.ToUInt32(Request.Params["status"]);
                }
                _IEdmContentMgr = new EdmContentMgr(mySqlConnectionString);
                _edmContentMgr = new EdmContentMgr(mySqlConnectionString);
                ////////////
                string FromName = string.Empty;
                string EmailTile = string.Empty;
                string strTemp = string.Empty;
                string userEmail = string.Empty;
                string userName = string.Empty;
                DataTable _emaildt = _edmContentMgr.GetTestEmailById(query.content_id);
                if (_emaildt.Rows.Count > 0)
                {
                    EmailFrom = _emaildt.Rows[0]["content_from_email"].ToString();
                    FromName = _emaildt.Rows[0]["content_from_name"].ToString();

                    EmailTile = _emaildt.Rows[0]["content_title"].ToString();
                    strTemp = _emaildt.Rows[0]["content_body"].ToString();
                }
                if (query.content_status == 1)
                {
                    //發送郵件
                    DataTable _dt = _edmContentMgr.GetAllTestEmail();
                    for (int index = 0; index < _dt.Rows.Count; index++)
                    {
                        userEmail = _dt.Rows[index]["email_address"].ToString();
                        userName = _dt.Rows[index]["test_username"].ToString();

                        bool result = sendmail(EmailFrom, FromName, userEmail, userName, EmailTile, strTemp, "", SmtpHost, Convert.ToInt32(SmtpPort), EmailUserName, EmailPassWord);
                        if (result)
                            json = "{success:true}";

                    }
                }
                if (query.content_status == 3)
                {
                    //修改狀態                  
                    i = _IEdmContentMgr.EditStatus(query);
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
                json = "{success:true,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase LoadEpaperContent()
        {
            string json = string.Empty;
            EpaperContentQuery query = new EpaperContentQuery();
            EpaperContentQuery model = new EpaperContentQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["content_id"]))
                {
                    _IEdmContentMgr = new EdmContentMgr(mySqlConnectionString);
                    query.epaper_id = Convert.ToUInt32(Request.Params["content_id"]);
                    _epaperMgr = new EpaperContentMgr(mySqlConnectionString);
                    model = _epaperMgr.GetEpaperContentById(query);
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    if (model != null)
                    {
                        model.epaper_id = query.epaper_id;
                        switch (model.epaper_status)
                        {
                            case 0:
                                json = "{success:false" + ",data:" + JsonConvert.SerializeObject(model, Formatting.Indented, timeConverter) + ",msg:0}";
                                break;
                            case 1:
                                json = "{success:true" + ",data:" + JsonConvert.SerializeObject(model, Formatting.Indented, timeConverter) + "}";
                                break;
                            case 2:
                                json = "{success:false" + ",data:" + JsonConvert.SerializeObject(model, Formatting.Indented, timeConverter) + ",msg:1}";
                                break;
                            case 3:
                                json = "{success:false" + ",data:" + JsonConvert.SerializeObject(model, Formatting.Indented, timeConverter) + ",msg:2}";
                                break;
                        }
                    }
                    else
                    {
                        json = "{success:false,msg:3}";
                    }
                }
                else
                {
                    json = "{success:false,data:[]}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        public HttpResponseBase GetWebContent()
        {
            string json = string.Empty;
            string jsonStr = string.Empty;
            EdmContentQuery cq = new EdmContentQuery();
            IAreaPactetImplMgr _iareaPacketMgr = new AreaPacketMgr(mySqlConnectionString);
            try
            {

                if (!string.IsNullOrEmpty(Request.Params["webtext"]))
                {
                    jsonStr = Request.Params["webtext"].ToString();
                    Uri httpURL = new Uri(jsonStr);
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
                }


                //int i = 0;
                //if (i > 0)
                //{
                //   json = "{success:true}";
                //}
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase GetEpaperContent()
        {
            string json = string.Empty;
            List<EpaperContent> store = new List<EpaperContent>();
            EpaperContent query = new EpaperContent();
            try
            {
                _epaperMgr = new EpaperContentMgr(mySqlConnectionString);
                store = _epaperMgr.GetEpaperContentLimit();
                EpaperContent zero = new EpaperContent();
                zero.epaper_title = "請選擇...";
                store.Insert(0, zero);
                foreach (var item in store)
                {
                    item.epaper_content = Server.HtmlDecode(item.epaper_content);
                }
                json = "{success:true" + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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
        public HttpResponseBase GetEpaperContentById()
        {
            string json = string.Empty;
            EdmContentQuery store = new EdmContentQuery();
            EdmContentQuery query = new EdmContentQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    _IEdmContentMgr = new EdmContentMgr(mySqlConnectionString);
                    store.content_id = Convert.ToUInt32(Request.Params["rowid"]);
                    _epaperMgr = new EpaperContentMgr(mySqlConnectionString);
                    store = _IEdmContentMgr.GetEdmContentById(store);
                    store.content_body = Server.HtmlDecode(store.content_body);
                    store.s_content_start = CommonFunction.GetNetTime(store.content_start);
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    json = "{success:true" + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
                }
                else
                {
                    json = "{success:false,data:[]}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #region 獲取收件者名單+HttpResponseBase GetEdmGroup()
        /// <summary>
        /// 獲取收件者名單
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetEdmGroup()
        {
            string json = string.Empty;
            List<EdmContentQuery> store = new List<EdmContentQuery>();
            EdmContentQuery query = new EdmContentQuery();
            try
            {
                _IEdmContentMgr = new EdmContentMgr(mySqlConnectionString);
                store = _IEdmContentMgr.GetEdmGroup();
                json = "{success:true" + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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

        #region 取消電子報
        /// <summary>
        /// 取消電子報
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase CancelEdm()
        {
            string json = "{success:false}";
            uint update_id = 0;
            uint vid = 0;
            try
            {
                string mail = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["mail"]))
                {
                    mail = Request.Params["mail"].ToString().Trim();
                }
                _IEdmContentMgr = new EdmContentMgr(mySqlConnectionString);
                update_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                int i = _IEdmContentMgr.CancelEdm(mail, update_id, out vid);
                if (i > 0)
                {
                    json = "{success:true}";
                }
                else if (i == -1)
                {
                    json = "{success:false,msg:\'0\'}"; //郵箱不存在，不可取消電子報
                }
                else if (i == -2)
                {
                    json = "{success:false,msg:\'1\'}";//郵箱沒有對應的用戶
                }
                else if (i == -3)
                {
                    //DataTable dt = new DataTable();
                    //dt.Columns.Add("vid");
                    //DataRow dr = new DataRow();
                    //dr[0] = vid.ToString();
                    //dt.Rows.Add(dr);
                    json = "{success:false,msg:\'2\',vid:\'" + vid + "\'}";//郵箱已加入黑名單且狀態為解除
                }
                else if (i == -4)
                {
                    json = "{success:false,msg:\'3\'}";//郵箱已加入黑名單且狀態為鎖定
                }
                else if (i == -5)
                {
                    json = "{success:false,msg:\'4\'}";//郵箱無法取消電子報
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 測試名單
        #region 測試名單列表+ HttpResponseBase GetEdmTest
        public HttpResponseBase GetEdmTestList()
        {
            string json = string.Empty;
            int totalCount = 0;
            DataTable store = new DataTable();
            try
            {
                _etestMgr = new EdmTestMgr(mySqlConnectionString);
                EdmTestQuery query = new EdmTestQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["selectType"]))
                {
                    query.selectType = Request.Params["selectType"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                {
                    query.search_con = Request.Params["search_con"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["dateCon"]))
                {
                    query.dateCon = Request.Params["dateCon"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["date_start"]))
                {
                    query.date_start = (uint)CommonFunction.GetPHPTime(Request.Params["date_start"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["date_end"]))
                {
                    query.date_end = (uint)CommonFunction.GetPHPTime(Request.Params["date_end"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["activeStatus"]))
                {
                    query.test_status = Convert.ToInt32(Request.Params["activeStatus"].ToString());
                }

                if (!string.IsNullOrEmpty(Request.Params["email_id"]))
                {
                    query.email_id = Convert.ToUInt32(Request.Params["email_id"]);
                }
                store = _etestMgr.GetEdmTestList(query, out totalCount);
                for (int i = 0; i < store.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(Request.Params["isSecret"]))
                    {
                        DataRow dr = store.Rows[i];
                        if (!string.IsNullOrEmpty(dr["test_username"].ToString()))
                        {
                            dr["test_username"] = dr["test_username"].ToString().Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(dr["test_username"].ToString()))
                        {
                            dr["email_address"] = dr["email_address"].ToString().Split('@')[0] + "@***";
                        }
                    }
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 添加/修改測試名單 + HttpResponseBase EdmTestAddorEdit
        public HttpResponseBase EdmTestAddorEdit()
        {
            string json = string.Empty;
            bool i = false;
            int msg = 0;
            try
            {
                _etestMgr = new EdmTestMgr(mySqlConnectionString);
                //新增
                if (string.IsNullOrEmpty(Request.Params["email_id"]))
                {
                    EdmTestQuery query = new EdmTestQuery();
                    if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                    {
                        query.email_address = Request.Params["email_address"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["test_username"]))
                    {
                        query.test_username = Request.Params["test_username"].Replace("\\", "\\\\");
                    }
                    if (!string.IsNullOrEmpty(Request.Params["test_status"]))
                    {
                        query.test_status = Convert.ToInt32(Request.Params["test_status"]);
                    }
                    query.test_createdate = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    query.test_updatedate = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    i = _etestMgr.AddEdmTest(query, out msg);
                    if (i)
                    {
                        json = "{success:true}";
                    }
                    if (msg == 1)
                    {
                        json = "{success:false,msg:1}";
                    }
                }
                else
                {
                    EdmTestQuery oldQuery = new EdmTestQuery();
                    if (!string.IsNullOrEmpty(Request.Params["email_id"]))
                    {
                        oldQuery.email_id = Convert.ToUInt32(Request.Params["email_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                    {
                        oldQuery.email_address = Request.Params["email_address"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["test_username"]))
                    {
                        oldQuery.test_username = Request.Params["test_username"].Replace("\\", "\\\\");
                    }
                    if (!string.IsNullOrEmpty(Request.Params["test_status"]))
                    {
                        oldQuery.test_status = Convert.ToInt32(Request.Params["test_status"]);
                    }
                    oldQuery.test_updatedate = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    i = _etestMgr.EditEdmTest(oldQuery, out msg);
                    if (i)
                    {
                        json = "{success:true}";
                    }
                    if (msg == 1)
                    {
                        json = "{success:false,msg:0}";
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
        #region 刪除測試名單 + HttpResponseBase EdmTestAddorEdit
        public HttpResponseBase DeleteEdmTest()
        {
            string json = string.Empty;
            try
            {
                _etestMgr = new EdmTestMgr(mySqlConnectionString);
                EdmTestQuery query = new EdmTestQuery();
                if (!string.IsNullOrEmpty(Request.Params["email_id"]))
                {
                    foreach (string item in Request.Params["email_id"].Split('|'))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _etestMgr.DeleteEdmTest(Convert.ToInt32(item));
                        }
                    }
                    json = "{success:true}";
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

        #region 黑名單管理
        public HttpResponseBase GetBlackList()
        {
            string json = string.Empty;
            int totalCount = 0;
            _vipuserMgr = new VipUserMgr(mySqlConnectionString);
            try
            {
                DataTable store = new DataTable();
                VipUserQuery query = new VipUserQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["searchState"]))
                {
                    query.search_state = Convert.ToInt32(Request.Params["searchState"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search_text"]))
                {
                    query.serchtype = Request.Params["search_text"].ToString().Replace("\\", "\\\\").Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["start_date"]))
                {
                    query.start = Convert.ToDateTime(Request.Params["start_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_date"]))
                {
                    query.end = Convert.ToDateTime(Request.Params["end_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["source"]))
                {
                    query.source = Convert.ToUInt32(Request.Params["source"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["uid"]))
                {
                    query.User_Id = Convert.ToUInt32(Request.Params["uid"]);
                }
                store = _vipuserMgr.GetBlackList(query, out totalCount);

                foreach (DataRow item in store.Rows)
                {
                    item["user_email"] = item["user_email"].ToString().Split('@')[0] + "@***";
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 更改狀態 啟用或者禁用
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateState()
        {
            uint id = 0;
            uint activeValue = 0;
            _vipuserMgr = new VipUserMgr(mySqlConnectionString);
            VipUserQuery query = new VipUserQuery();
            if (!string.IsNullOrEmpty(Request.Params["id"]))
            {
                id = Convert.ToUInt32(Request.Params["id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["active"]))
            {
                activeValue = Convert.ToUInt32(Request.Params["active"]);
            }
            query.v_id = id;

            if (!string.IsNullOrEmpty(Request.Params["uid"]))
            {
                query.user_id = Convert.ToUInt32(Request.Params["uid"]);
            }
            query.status = activeValue;
            query.update_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
            if (_vipuserMgr.UpdateState(query) > 0)
            {
                return Json(new { success = "true", msg = "1" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }

        }
        /// <summary>
        /// 新增黑名單
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UpdateBlackList()
        {
            string json = string.Empty;
            _vipuserMgr = new VipUserMgr(mySqlConnectionString);
            try
            {
                string mail = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["mail"]))
                {
                    mail = Request.Params["mail"].ToString();
                }
                int result = _vipuserMgr.UpdateBlackList(mail);
                if (result > 0)
                {
                    json = "{success:true}";
                }
                else if (result == -1)
                {
                    json = "{success:false,msg:0}";
                }
                else if (result == -2)
                {
                    json = "{success:false,msg:1}";
                }
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase ExportFile()
        {
            string json = string.Empty;
            int totalCount = 0;
            _vipuserMgr = new VipUserMgr(mySqlConnectionString);
            try
            {
                DataTable store = new DataTable();
                VipUserQuery query = new VipUserQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["searchState"]))
                {
                    query.search_state = Convert.ToInt32(Request.Params["searchState"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search_text"]))
                {
                    query.serchtype = Request.Params["search_text"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["start_date"]))
                {
                    query.start = Convert.ToDateTime(Request.Params["start_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_date"]))
                {
                    query.end = Convert.ToDateTime(Request.Params["end_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["source"]))
                {
                    query.source = Convert.ToUInt32(Request.Params["source"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["uid"]))
                {
                    query.User_Id = Convert.ToUInt32(Request.Params["uid"]);
                }
                query.IsPage = false;
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("黑名單編號", typeof(int));
                dtHZ.Columns.Add("會員郵箱", typeof(String));
                dtHZ.Columns.Add("添加來源", typeof(String));
                dtHZ.Columns.Add("狀態", typeof(String));
                dtHZ.Columns.Add("會員編號", typeof(int));
                dtHZ.Columns.Add("創建者", typeof(String));
                dtHZ.Columns.Add("列入黑名單日期", typeof(String));
                dtHZ.Columns.Add("修改者", typeof(String));
                dtHZ.Columns.Add("修改時間", typeof(String));
                store = _vipuserMgr.GetBlackList(query, out totalCount);
                foreach (DataRow dr_v in store.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = dr_v["v_id"].ToString();
                    dr[1] = dr_v["user_email"].ToString();
                    dr[2] = dr_v["source"].ToString() == "2" ? "客服" : "用戶";
                    dr[3] = dr_v["status"].ToString() == "0" ? "解除" : "鎖定";
                    dr[4] = dr_v["user_id"].ToString();
                    dr[5] = dr_v["createUsername"].ToString();
                    dr[6] = dr_v["create"].ToString();
                    dr[7] = dr_v["updateUsername"].ToString();
                    dr[8] = dr_v["updatedate"].ToString();
                    dtHZ.Rows.Add(dr);
                }
                string[] colname = new string[dtHZ.Columns.Count];
                string filename = "BlackList" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                newExcelName = Server.MapPath(excelPath_export) + filename;
                for (int i = 0; i < dtHZ.Columns.Count; i++)
                {
                    colname[i] = dtHZ.Columns[i].ColumnName;
                }

                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }
                ExcelHelperXhf.ExportDTtoExcel(dtHZ, "", newExcelName);
                json = "{success:true,ExcelName:\'" + filename + "\'}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 名單列表
        public HttpResponseBase GetEdmGroupList()
        {
            List<EdmGroup> store = new List<EdmGroup>();
            EdmGroup query = new EdmGroup();
            int totalCount = 0;
            string json = string.Empty;

            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            if (!string.IsNullOrEmpty(Request.Params["selectType"]))
            {
                query.selectType = Request.Params["selectType"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["search_con"]))
            {
                query.search_con = Request.Params["search_con"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["dateType"]))
            {
                query.dateCondition = Convert.ToInt32(Request.Params["dateType"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["timestart"]))
            {
                query.start = (uint)CommonFunction.GetPHPTime(Request.Params["timestart"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["timeend"]))
            {
                query.end = (uint)CommonFunction.GetPHPTime(Request.Params["timeend"].ToString());
            }
            try
            {
                _edmGroup = new EdmGroupMgr(mySqlConnectionString);
                store = _edmGroup.GetEdmGroupList(query, out totalCount);
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

        public HttpResponseBase SaveEdmGroup()
        {
            EdmGroup query = new EdmGroup();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_name"]))
                {
                    query.group_name = (Request.Params["group_name"]);
                }
                _edmGroup = new EdmGroupMgr(mySqlConnectionString);
                json = _edmGroup.SaveEdmGroup(query);
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

        public HttpResponseBase DeleteEdmGroup()
        {
            string json = string.Empty;
            EdmGroup query = null;
            _edmGroup = new EdmGroupMgr(mySqlConnectionString);
            List<EdmGroup> list = new List<EdmGroup>();
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string rowIDs = Request.Form["rowID"];
                    if (rowIDs.IndexOf("|") != -1)
                    {
                        foreach (string id in rowIDs.Split('|'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                query = new EdmGroup();
                                query.group_id = Convert.ToUInt32(id);
                                list.Add(query);
                            }
                        }
                    }
                    json = _edmGroup.DeleteEdmGroup(list);
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

        public HttpResponseBase Export()
        {
            string json = string.Empty;
            try
            {
                EdmGroup query = new EdmGroup();
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                }
                _edmGroup = new EdmGroupMgr(mySqlConnectionString);
                DataTable _dt = _edmGroup.Export(query);
                string fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "." + "csv";
                DataTable _newdt = new DataTable();
                DataRow dr;
                _newdt.Columns.Add("電子信箱", typeof(string));
                _newdt.Columns.Add("訂閱狀態", typeof(string));
                _newdt.Columns.Add("姓名", typeof(string));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    dr = _newdt.NewRow();
                    _newdt.Rows.Add(dr);
                    _newdt.Rows[i]["電子信箱"] = _dt.Rows[i]["email_address"];
                    _newdt.Rows[i]["姓名"] = _dt.Rows[i]["email_name"];
                    uint email_status = Convert.ToUInt32(_dt.Rows[i]["email_status"].ToString());
                    if (email_status == 1)
                    {
                        _newdt.Rows[i]["訂閱狀態"] = "已訂閱";
                    }
                    else
                    {
                        _newdt.Rows[i]["訂閱狀態"] = "未訂閱";
                    }
                }
                StringWriter sw = ExcelHelperXhf.SetCsvFromData(_newdt, fileName);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
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

        public HttpResponseBase Import()
        {
            string json = string.Empty;
            try
            {
                EdmGroupEmailQuery query = new EdmGroupEmailQuery();
                if (!string.IsNullOrEmpty(Request.Params["ImportCsv"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                    HttpPostedFileBase file = Request.Files["ImportCsv"];
                    FileManagement fileManagement = new FileManagement();
                    string fileName = fileManagement.NewFileName(file.FileName);
                    string newFileName = Server.MapPath(excelPath_export) + fileName;
                    file.SaveAs(newFileName);
                    DataTable _dt = CsvHelper.ReadCsvToDataTable_CN(newFileName, true);
                    _edmGroup = new EdmGroupMgr(mySqlConnectionString);
                    json = _edmGroup.Import(_dt, query);
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

        public HttpResponseBase DownTemplate()
        {
            string json = string.Empty;
            try
            {
                string fileName = "名單管理匯入模板" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "." + "csv";

                DataTable _newdt = new DataTable();
                _newdt.Columns.Add("電子信箱", typeof(string));
                _newdt.Columns.Add("訂閱狀態", typeof(string));
                _newdt.Columns.Add("姓名", typeof(string));
                DataRow dr = _newdt.NewRow();
                dr["電子信箱"] = "example@gimg.tw";
                dr["訂閱狀態"] = "1";
                dr["姓名"] = "example";
                _newdt.Rows.Add(dr);

                StringWriter sw = ExcelHelperXhf.SetCsvFromData(_newdt, fileName);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
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


        #region 電子報群組
        #region 電子報群組名單列表
        public HttpResponseBase GetEdmGroupEmailList()
        {
            string json = string.Empty;
            _IEdmGroupEmailMgr = new EdmGroupEmailMgr(mySqlConnectionString);
            List<EdmGroupEmailQuery> store = new List<EdmGroupEmailQuery>();
            EdmGroupEmailQuery query = new EdmGroupEmailQuery();

            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["selectType"]))
                {
                    query.selectType = Request.Params["selectType"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                {
                    query.search_con = Request.Params["search_con"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["email_status"]))
                {
                    query.email_status = Convert.ToUInt32(Request.Params["email_status"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["email_id"]))
                {
                    query.email_id = Convert.ToUInt32(Request.Params["email_id"]);
                }
                int totalCount = 0;
                store = _IEdmGroupEmailMgr.GetEdmGroupEmailList(query, out totalCount);
                foreach (EdmGroupEmailQuery egeq in store)
                {
                    if (Convert.ToBoolean(Request.Params["isSecret"]))
                    {
                        if (!string.IsNullOrEmpty(egeq.email_name))
                        {
                            egeq.email_name = egeq.email_name.Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(egeq.email_address))
                        {
                            egeq.email_address = egeq.email_address.Split('@')[0] + "@***";
                        }
                    }
                    egeq.email_createdate_tostring = CommonFunction.GetNetTime(egeq.email_createdate);
                    egeq.email_updatedate_tostring = CommonFunction.GetNetTime(egeq.email_updatedate);
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
        #region 群組信息獲取
        public HttpResponseBase Load()
        {
            string json = string.Empty;
            _IEdmGroupEmailMgr = new EdmGroupEmailMgr(mySqlConnectionString);
            EdmGroupQuery store = new EdmGroupQuery();
            EdmGroupQuery query = new EdmGroupQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                }
                store = _IEdmGroupEmailMgr.Load(query);
                if (store != null)
                {
                    json = "{success:true,group_id:" + store.group_id + ",group_name:'" + store.group_name + "',group_count:'" + store.group_total_email + "'}";
                }
                else
                {
                    json = "{success:false,msg:1}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 電子報群組刪除
        public HttpResponseBase DelEdmGroupEmail()
        {
            string json = string.Empty;
            _IEdmGroupEmailMgr = new EdmGroupEmailMgr(mySqlConnectionString);
            EdmGroupEmailQuery query = new EdmGroupEmailQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["email_ids"]))
                {
                    query.email_ids = Request.Params["email_ids"].ToString();
                }
                query.email_ids = query.email_ids.Substring(0, query.email_ids.Length - 1);
                int res = _IEdmGroupEmailMgr.DeleteEdmGroupEmail(query);
                if (res > 0)
                {
                    json = "{success:true,msg:1}";
                    int num = _IEdmGroupEmailMgr.UpdateCount(Convert.ToInt32(query.group_id));
                    if (num <= 0)
                    {
                        json = "{success:true,msg:0}";
                    }
                }
                else
                {
                    json = "{success:true,msg:0}";
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
        #region 電子報群組新增/編輯
        public HttpResponseBase EdmGroupEmailEdit()
        {
            string json = string.Empty;
            _IEdmGroupEmailMgr = new EdmGroupEmailMgr(mySqlConnectionString);
            EdmGroupEmailQuery groupEmailQuery = new EdmGroupEmailQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    groupEmailQuery.group_id = Convert.ToUInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["email_id"]))
                {
                    groupEmailQuery.email_id = Convert.ToUInt32(Request.Params["email_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                {
                    groupEmailQuery.email_address = Request.Params["email_address"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["email_name"]))
                {
                    groupEmailQuery.email_name = Request.Params["email_name"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["email_status"]))
                {
                    groupEmailQuery.email_status = Convert.ToUInt32(Request.Params["email_status"]);
                }
                json = _IEdmGroupEmailMgr.EdmGroupEmailEdit(groupEmailQuery);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion
        #endregion

        #region 發信名單統計
        public HttpResponseBase GetStatisticsEdmSend()
        {
            string json = string.Empty;
            List<EdmSendQuery> store = new List<EdmSendQuery>();
            EdmSendQuery query = new EdmSendQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

                if (!string.IsNullOrEmpty(Request.Params["cid"]))
                {
                    query.content_id = Convert.ToUInt32(Request.Params["cid"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["email_name"]))
                {
                    query.email_name = Request.Params["email_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.date = Convert.ToInt32(Request.Params["date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time = Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59");
                }
                _edmSendMgr = new EdmSendMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _edmSendMgr.GetStatisticsEdmSend(query, out totalCount);
                foreach (var items in store)
                {
                    items.email_name = items.email_name.ToString().Substring(0, 1) + "**";
                }
                //計算圖表width
                int max_open = _edmSendMgr.GetMaxOpen(query);
                double nTemp_Image_Rate = 1;
                int nMax_Image_Width = 250;
                if (max_open > 0)
                {
                    nTemp_Image_Rate = (max_open > nMax_Image_Width) ? Math.Round((double)nMax_Image_Width / max_open, 2) : 1;
                }

                foreach (var item in store)
                {
                    item.s_send_status = item.send_status == 1 ? "成功" : "失敗";
                    if (item.send_datetime != 0)
                    {
                        item.s_send_datetime = CommonFunction.GetNetTime(item.send_datetime).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (item.open_first != 0)
                    {
                        item.s_open_first = CommonFunction.GetNetTime(item.open_first).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (item.open_last != 0)
                    {
                        item.s_open_last = CommonFunction.GetNetTime(item.open_last).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    item.image_width = (uint)Math.Round(item.open_total * nTemp_Image_Rate, 0);

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

        public HttpResponseBase EdmSendExportCSV()
        {
            string json = string.Empty;
            try
            {
                EdmSendQuery query = new EdmSendQuery();
                if (!string.IsNullOrEmpty(Request.Params["cid"]))
                {
                    query.content_id = Convert.ToUInt32(Request.Params["cid"].ToString());
                }
                _edmSendMgr = new EdmSendMgr(mySqlConnectionString);
                DataTable _dt = _edmSendMgr.EdmSendExportCSV(query);
                string fileName = "edm_status_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "." + "csv";
                string newFileName = Server.MapPath(excelPath_export + fileName);  //"電子信箱"
                string[] colName = { "發信狀態", "郵件編號", "姓名", "開信次數", "寄信時間", "首次開信時間", "最近開信時間" };
                DataTable _newdt = new DataTable();
                foreach (string item in colName)
                {
                    _newdt.Columns.Add(item, typeof(string));
                }

                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = _newdt.NewRow();//SELECT	es.email_id,es.send_status,es.send_datetime,es.open_first,es.open_last,es.open_total,ee.email_name
                    dr[0] = _dt.Rows[i]["send_status"].ToString() == "1" ? "Success" : "Fail";
                    dr[1] = _dt.Rows[i]["email_id"];
                    dr[2] = _dt.Rows[i]["email_name"];
                    //dr[3] = _dt.Rows[i]["email_address"];  //不導出email_address
                    dr[3] = _dt.Rows[i]["open_total"];
                    if (Convert.ToUInt32(_dt.Rows[i]["send_datetime"]) != 0)
                    {
                        dr[4] = CommonFunction.GetNetTime(Convert.ToUInt32(_dt.Rows[i]["send_datetime"])).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (Convert.ToUInt32(_dt.Rows[i]["open_first"]) != 0)
                    {
                        dr[5] = CommonFunction.GetNetTime(Convert.ToUInt32(_dt.Rows[i]["open_first"])).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (Convert.ToUInt32(_dt.Rows[i]["open_last"]) != 0)
                    {
                        dr[6] = CommonFunction.GetNetTime(Convert.ToUInt32(_dt.Rows[i]["open_last"])).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    if (Request["st"].ToString() == "1" && Convert.ToUInt32(_dt.Rows[i]["open_first"]) != 0) //開信名單下載
                    {
                        _newdt.Rows.Add(dr);
                    }
                    if (Request["st"].ToString() == "0" && Convert.ToUInt32(_dt.Rows[i]["open_first"]) == 0)//未開信名單下載
                    {
                        _newdt.Rows.Add(dr);
                    }

                }
                CsvHelper.ExportDataTableToCsv(_newdt, newFileName, colName, true);
                json = "{success:true,fileName:\'" + fileName + "\'}";
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

        public HttpResponseBase EdmSendLoad()
        {
            string json = string.Empty;
            EdmSendQuery store = new EdmSendQuery();
            EdmSendQuery query = new EdmSendQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["cid"]))
                {
                    query.content_id = Convert.ToUInt32(Request.Params["cid"].ToString());
                }
                _edmSendMgr = new EdmSendMgr(mySqlConnectionString);
                store = _edmSendMgr.EdmSendLoad(query);

                int nMax_Image_Width = 250; //最大圖片寬度

                if (store != null)
                {
                    store.content_send = store.content_send_success + store.content_send_failed;
                    if (store.content_send > 0)
                    {
                        store.content_openRate = Math.Round((double)store.content_person / store.content_send * 100, 2);
                        store.content_imagewidth_send = nMax_Image_Width;
                        store.content_imagewidth_success = (int)Math.Round((double)store.content_send_success / store.content_send * nMax_Image_Width, 0);
                        store.content_imagewidth_failed = (int)Math.Round((double)store.content_send_failed / store.content_send * nMax_Image_Width, 0);
                    }
                    if (store.content_person > 0)
                    {
                        store.content_averageClick = Math.Round((double)store.content_click / store.content_person, 1);
                    }
                    store.content_start_s = CommonFunction.GetNetTime(store.content_start).ToString("yyyy-MM-dd HH:mm:ss");
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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

        public HttpResponseBase GetStatisticsEdmList()
        {
            string json = string.Empty;
            List<EdmListQuery> store = new List<EdmListQuery>();
            EdmListQuery query = new EdmListQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

                if (!string.IsNullOrEmpty(Request.Params["cid"]))
                {
                    query.content_id = Convert.ToUInt32(Request.Params["cid"].ToString());
                }
                _edmSendMgr = new EdmSendMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _edmSendMgr.GetStatisticsEdmList(query, out totalCount);
                //計算圖表width和sum_total_click、sum_total_click
                int sum_total_click = 0;
                int sum_total_person = 0;
                int max_open = _edmSendMgr.GetMaxClick(query, out sum_total_click, out sum_total_person);
                double nTemp_Image_Rate = 1.00;
                int nMax_Image_Width = 250;
                if (max_open > 0)
                {
                    nTemp_Image_Rate = (max_open > nMax_Image_Width) ? Math.Round((double)nMax_Image_Width / max_open, 2) : 1;
                }

                foreach (var item in store)
                {
                    string temp_statistics_id = item.statistics_id.ToString();
                    string year = temp_statistics_id.Substring(0, 4);
                    string month = temp_statistics_id.Substring(4, 2);
                    string day = temp_statistics_id.Substring(6, 2);
                    DateTime d1 = Convert.ToDateTime(year + "-" + month + "-" + day + " 00:00:00");
                    d1.DayOfWeek.ToString();
                    switch (d1.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            item.week = "[日]";
                            break;
                        case DayOfWeek.Monday:
                            item.week = "[一]";
                            break;
                        case DayOfWeek.Tuesday:
                            item.week = "[二]";
                            break;
                        case DayOfWeek.Wednesday:
                            item.week = "[三]";
                            break;
                        case DayOfWeek.Thursday:
                            item.week = "[四]";
                            break;
                        case DayOfWeek.Friday:
                            item.week = "[五]";
                            break;
                        case DayOfWeek.Saturday:
                            item.week = "[六]";
                            break;
                        default:
                            break;
                    }
                    item.date = year + "-" + month + "-" + day;
                    if (sum_total_click > 0)
                    {
                        item.clickRate = Math.Round((double)item.total_click / sum_total_click * 100, 2);
                    }
                    if (sum_total_person > 0)
                    {
                        item.personRate = Math.Round((double)item.total_person / sum_total_person * 100, 2);
                    }

                    item.image_width = (uint)Math.Round(item.total_click * nTemp_Image_Rate, 0);

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

        public HttpResponseBase GetSendRecordList()
        {
            string json = string.Empty;
            _edmSendMgr = new EdmSendMgr(mySqlConnectionString);
            EdmSendQuery query = new EdmSendQuery();
            int totalCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["eid"]))
                {
                    query.email_id = Convert.ToUInt32(Request.Params["eid"]);
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["content_title"]))
                {
                    query.content_title = Request.Params["content_title"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.date = Convert.ToInt32(Request.Params["date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time = Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59");
                }
                DataTable store = _edmSendMgr.GetSendRecordList(query, out totalCount);
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

        public HttpResponseBase GetEmailByID()
        {
            string json = string.Empty;
            uint eid = 0;
            string email_name = string.Empty;
            string email_address = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["eid"]))
                {
                    eid = Convert.ToUInt32(Request.Params["eid"].ToString());
                }
                _edmEmailMgr = new EdmEmailMgr(mySqlConnectionString);
                _edmEmailMgr.GetEmailByID(eid, out email_name, out email_address);
                //json = "{success:true}";
                json = "{success:true,email_name:'" + email_name + "',email_address:'" + email_address + "'}";
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

        #region 電子報人員名單列表
        public HttpResponseBase GetEdmPersonList()
        {
            string json = string.Empty;
            EdmEmailQuery query = new EdmEmailQuery();
            DataTable store = new DataTable();
            int totalCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {
                    query.email_name = Request.Params["name"].ToString().Replace("\\", "\\\\");
                }
                if (!string.IsNullOrEmpty(Request.Params["email"]))
                {
                    query.email_address = Request.Params["email"].ToString().Replace("\\", "\\\\");
                }
                if (!string.IsNullOrEmpty(Request.Params["search_id"]))
                {
                    query.email_id = Convert.ToUInt32(Request.Params["search_id"].ToString());
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                _edmEmailMgr = new EdmEmailMgr(mySqlConnectionString);
                store = _edmEmailMgr.GetEdmPersonList(query, out totalCount);
                for (int i = 0; i < store.Rows.Count; i++)
                {
                    DataRow dr = store.Rows[i];
                    if (!string.IsNullOrEmpty(dr["email_name"].ToString()))
                    {
                        dr["email_name"] = dr["email_name"].ToString().Substring(0, 1) + "**";
                    }
                    dr["email_address"] = dr["email_address"].ToString().Split('@')[0] + "@***";
                }
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store) + "}";
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
        public HttpResponseBase GetPersonList()
        {
            string json = string.Empty;
            EdmEmailQuery query = new EdmEmailQuery();
            DataTable store = new DataTable();
            int totalCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["gname"]))
                {
                    query.group_name = Request.Params["gname"].ToString().Replace("\\", "\\\\");
                }
                if (!string.IsNullOrEmpty(Request.Params["gid"]))
                {
                    query.group_id = Convert.ToInt16(Request.Params["gid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["email_id"]))
                {
                    query.email_id = Convert.ToUInt32(Request.Params["email_id"]);
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                _edmEmailMgr = new EdmEmailMgr(mySqlConnectionString);
                store = _edmEmailMgr.GetPersonList(query, out totalCount);
                for (int i = 0; i < store.Rows.Count; i++)
                {
                    DataRow dr = store.Rows[i];
                    if (!string.IsNullOrEmpty(dr["email_name"].ToString()))
                    {
                        dr["email_name"] = dr["email_name"].ToString().Substring(0, 1) + "**";
                    }
                    dr["email_address"] = dr["email_address"].ToString().Split('@')[0] + "@***";
                }
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store) + "}";
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


        #region C#发送邮件函数
        /// <summary>
        /// C#发送邮件函数
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="fromer">发送人</param>
        /// <param name="sto">接受者邮箱</param>
        /// <param name="toer">收件人</param>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容</param>
        /// <param name="file">附件地址</param>
        /// <param name="SMTPHost">smtp服务器</param>
        /// <param name="SMTPuser">邮箱</param>
        /// <param name="SMTPpass">密码</param>
        /// <returns></returns>
        public bool sendmail(string sfrom, string sfromer, string sto, string stoer, string sSubject, string sBody, string sfile, string sSMTPHost, int sSMTPPort, string sSMTPuser, string sSMTPpass)
        {
            ////设置from和to地址
            MailAddress from = new MailAddress(sfrom, sfromer);
            MailAddress to = new MailAddress(sto, stoer);

            ////创建一个MailMessage对象
            MailMessage oMail = new MailMessage(from, to);
            //// 添加附件
            if (sfile != "")
            {
                oMail.Attachments.Add(new Attachment(sfile));
            }
            ////邮件标题
            oMail.Subject = sSubject;
            ////邮件内容
            //oMail.Body = sBody;
            sBody = Server.HtmlDecode(sBody);
            AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(sBody, null, "text/html");
            oMail.AlternateViews.Add(htmlBody);
            ////邮件格式
            oMail.IsBodyHtml = true;
            ////邮件采用的编码
            oMail.BodyEncoding = System.Text.Encoding.UTF8;

            ////设置邮件的优先级为高
            oMail.Priority = MailPriority.Normal;
            ////发送邮件
            SmtpClient client = new SmtpClient();
            ////client.UseDefaultCredentials = false;
            client.Host = sSMTPHost;
            client.Port = sSMTPPort;
            client.Credentials = new NetworkCredential(sSMTPuser, sSMTPpass);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(oMail);
                return true;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return false;
            }
            finally
            {
                ////释放资源
                oMail.Dispose();
            }

        }
        #endregion

        public HttpResponseBase GetfunctionCodeID()
        {
            string json = string.Empty;
            DataTable store = new DataTable();
            int rowid = 0;
            try
            {
                TFunction query = new TFunction();
                if (!string.IsNullOrEmpty(Request.Params["functionCode"]))
                {
                    query.functionCode = Request.Params["functionCode"];
                }
                if (!string.IsNullOrEmpty(Request.Params["functionName"]))
                {
                    query.functionName = Request.Params["functionName"];
                }
                _tFunctonMgr = new TFunctionMgr(mySqlConnectionString);
                store = _tFunctonMgr.GetModel(query);
                if (store != null && store.Rows.Count > 0)
                {
                    rowid = Convert.ToInt32(store.Rows[0][0]);
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,rowid:" + rowid + "}";
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
