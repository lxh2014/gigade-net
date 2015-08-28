using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Admin.gigade.CustomError;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BLL.gigade.Common;
using System.IO;
using System.Data;
using gigadeExcel.Comment;
using System.Text.RegularExpressions;

namespace Admin.gigade.Controllers
{
    public class ChinatrustController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private EventChinaTrustBagMgr _eventChinaTrustBag;
        private EventChinatrustMgr _eventChinatrust;
        private EventChinaTrustBagMapMgr _ChinatrustBMMgr;
        string link = ConfigurationManager.AppSettings["link"];
        //上傳圖片
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = "/Content/img/nopic_50.jpg";
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址ftp://192.168.71.10:2121/img
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.10:8765"
        string PaperPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.paperPath);//圖片保存路徑/paper/
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
        //
        // GET: /Chinatrust/
        #region View
        public ActionResult Chinatrust()
        {
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            return View();
        }
        public ActionResult ChinatrustBag()
        {
            if (!string.IsNullOrEmpty(Request.Params["eventId"]))
            {
                ViewBag.eventId = Request.Params["eventId"];
              //  ViewBag.eventId = "PB000001";
            }
            else
            {
                ViewBag.eventId = "";
            }
            return View();
        }
        public ActionResult ChinatrustBagMap()
        {
            if (!string.IsNullOrEmpty(Request.Params["bag_id"]))
            {
                ViewBag.bag_id = Request.Params["bag_id"];
            }
         
            return View();
        }
        #endregion

        #region 中信一點一元
        #region 中信一點一元列表頁
        public HttpResponseBase GetChinatrustList()
        {
            string jsonStr = string.Empty;
           // int tranInt = 0;
            try
            {
                List<EventChinatrustQuery> store = new List<EventChinatrustQuery>();
                EventChinatrustQuery query = new EventChinatrustQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                query.event_active = -1;
                if (int.Parse(Request.Params["event_active"]) != -1)
                {
                    query.event_active = int.Parse(Request.Params["event_active"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_search"]))
                {
                    //if (int.TryParse(Request.Params["event_search"], out tranInt))
                    //{
                    //    query.event_id = int.Parse(Request.Params["event_search"].ToString());
                    //}
                    //else
                    //{
                    query.event_id_name = Request.Params["event_search"];
                    //}
                }
                if (!string.IsNullOrEmpty(Request.Params["TimeStart"]))
                {
                    query.event_start_time = DateTime.Parse(Request.Params["TimeStart"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["TimeEnd"]))
                {
                    query.event_end_time = DateTime.Parse(Request.Params["TimeEnd"]);
                }
                int totalCount = 0;
                _eventChinatrust = new EventChinatrustMgr(mySqlConnectionString);
                store = _eventChinatrust.GetEventChinatrustList(query, out totalCount);
                for (int i = 0; i < store.Count; i++)
                {
                    store[i].event_desc = Server.HtmlDecode(Server.HtmlDecode(store[i].event_desc));
                    if (store[i].event_banner != "")
                    {
                        store[i].s_event_banner = imgServerPath + PaperPath + store[i].event_banner;//"http://192.168.71.10:8765"+/paper/
                    }
                    else
                    {
                        store[i].s_event_banner = defaultImg;
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 中信一點一元新增編輯
        public HttpResponseBase SaveChinatrust()
        {
            string json = string.Empty;
            try
            {
                _eventChinatrust = new EventChinatrustMgr(mySqlConnectionString);
                FileManagement fileLoad = new FileManagement();
                EventChinatrustQuery query = new EventChinatrustQuery();
                EventChinatrustQuery oldModel = new EventChinatrustQuery();
                List<EventChinatrustQuery> store = new List<EventChinatrustQuery>();
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {//如果是編輯獲取該id數據
                    int totalCount = 0;
                    query.IsPage = false;
                    query.event_active = -1;
                    query.row_id = int.Parse(Request.Params["row_id"]);
                    store = _eventChinatrust.GetEventChinatrustList(query, out totalCount);
                }
                #region 需要更改的屬性
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["row_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_desc"]))
                {
                    query.event_desc = Request.Params["event_desc"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_start_time"]))
                {
                    query.event_start_time = DateTime.Parse(Request.Params["event_start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_end_time"]))
                {
                    query.event_end_time = DateTime.Parse(Request.Params["event_end_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_register_time"]))
                { 
                    query.user_register_time=DateTime.Parse(Request.Params["user_register_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_banner"]))
                {
                    query.event_banner = Request.Params["event_banner"];
                }
                #endregion
                query.event_type = "EC";
                #region 上傳圖片
                string path = Server.MapPath(xmlPath);
                SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
                SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_Min_Element");
                SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
                SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
                //擴展名、最小值、最大值
                string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
                string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
                string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
                string localPromoPath = imgLocalPath + PaperPath;//圖片存儲地址

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string fileName = string.Empty;//當前文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();
                        string NewFileName = string.Empty;
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localPromoPath.Substring(0, localPromoPath.Length - PaperPath.Length + 1), PaperPath.Substring(1, PaperPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        fileName = NewFileName + fileExtention;
                        NewFileName = localPromoPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + PaperPath);
                        string ErrorMsg = string.Empty;
                        //上傳之前刪除已有的圖片
                        if (query.row_id != 0)
                        {
                            oldModel = store.FirstOrDefault();
                            if (oldModel.event_banner != "")
                            {
                                string oldFileName = oldModel.event_banner;
                                CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldFileName))
                                {
                                    FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                                    ftps.DeleteFile(localPromoPath + oldFileName);
                                }
                            }
                        }
                        try
                        {
                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)
                            {
                                query.event_banner = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                        }
                        if (!string.IsNullOrEmpty(ErrorMsg))
                        {
                            string jsonStr = string.Empty;
                            json = "{success:true,msg:\"" + ErrorMsg + "\"}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                    }

                }
                #endregion
                if (query.row_id == 0)
                {
                    query.event_create_time = DateTime.Now;
                    query.event_update_time = query.event_create_time;
                    query.event_create_user = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                    query.event_update_user = query.event_create_user;
                    if (_eventChinatrust.AddEventChinatrust(query) > 0)
                    {
                        json = "{success:true }";
                    }
                    else
                    {
                        json = "{success:false,msg:'新增失敗!'}";
                    }
                }
                else
                {
                    query.event_update_time = DateTime.Now;
                    query.event_update_user = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                    if (_eventChinatrust.UpdateEventChinatrust(query) > 0)
                    {
                        json = "{success:true }";
                    }
                    else
                    {
                        json = "{success:false,msg:'修改失敗!'}";
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 中信一點一元更改狀態
        public JsonResult UpdateActiveQuestion()
        {
            try
            {
                EventChinatrustQuery query = new EventChinatrustQuery();
                if (!string.IsNullOrEmpty(Request.Params["row_id"].ToString()))
                {
                    query.row_id = Convert.ToInt32(Request.Params["row_id"].ToString());
                }
                query.event_active = Convert.ToInt32(Request.Params["event_active"] ?? "0");
                _eventChinatrust = new EventChinatrustMgr(mySqlConnectionString);
                query.event_update_user = (Session["caller"] as Caller).user_id;
                query.event_update_time = DateTime.Now;
                if (_eventChinatrust.UpEventChinatrustStatus(query) > 0)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false" });
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }
        }
        #endregion
        #region combobox  store數據
        public HttpResponseBase GetChinaTrustStore()
        {
            string json=string.Empty;
            List<EventChinatrustQuery> store = new List<EventChinatrustQuery>();
            try
            {
                _eventChinatrust=new EventChinatrustMgr (mySqlConnectionString);
                store = _eventChinatrust.GetChinaTrustStore();
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
            }
            catch(Exception  ex)
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
        #endregion
        #region 區域包
        #region 區域包列表頁
        public HttpResponseBase EventChinaTrustBagList()
        {
            EventChinaTrustBagQuery query = new EventChinaTrustBagQuery();
            List<EventChinaTrustBagQuery> store = new List<EventChinaTrustBagQuery>();
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                 query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                 query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                 if (!string.IsNullOrEmpty(Request.Params["eventId"]))
                 {
                     query.event_id = Request.Params["eventId"];
                 }

                 if (!string.IsNullOrEmpty(Request.Params["bag_status"]))
                 {
                     query.bag_active =Convert.ToInt32(Request.Params["bag_status"]);
                 }
                 if (!string.IsNullOrEmpty(Request.Params["bag_search_name"]))
                 {
                     query.bag_name = Request.Params["bag_search_name"];
                 }
                 if (!string.IsNullOrEmpty(Request.Params["date"]))
                 {
                     query.date =Convert.ToInt32(Request.Params["date"]);
                 }
                 if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                 {
                     query.start_time = Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00");
                 }
                 if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                 {
                     query.end_time = Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"); 
                 }
                _eventChinaTrustBag = new EventChinaTrustBagMgr(mySqlConnectionString);
                 store=_eventChinaTrustBag.EventChinaTrustBagList(query, out totalCount);
                foreach (var item in store)
                {
                    item.start_time = CommonFunction.DateTimeToString(item.bag_start_time);
                    item.end_time = CommonFunction.DateTimeToString(item.bag_end_time);
                    item.show_start_time = CommonFunction.DateTimeToString(item.bag_show_start_time);
                    item.show_end_time = CommonFunction.DateTimeToString(item.bag_show_end_time);
                    item.create_time= CommonFunction.DateTimeToString(item.bag_create_time);
                    item.update_time = CommonFunction.DateTimeToString(item.bag_update_time);
                    if (item.bag_banner != "")
                    {
                    item.s_bag_banner=imgServerPath + PaperPath + item.bag_banner;
                    }
                    else
                    {
                        item.s_bag_banner = defaultImg;
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
            }
            catch(Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 區域包新增/編輯
        public HttpResponseBase EventChinaTrustBagSave()
        {
            string json = string.Empty;
            EventChinaTrustBagQuery query = new EventChinaTrustBagQuery();
            EventChinaTrustBagQuery oldModel = new EventChinaTrustBagQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Request.Params["event_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["bag_name"]))
                {
                    query.bag_name = Request.Params["bag_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["bag_desc"]))
                {
                    query.bag_desc = Request.Params["bag_desc"];
                }
                if (!string.IsNullOrEmpty(Request.Params["bag_banner"]))
                {
                    query.bag_banner = Request.Params["bag_banner"];
                }
                if (!string.IsNullOrEmpty(Request.Params["bag_start_time"]))
                {
                    query.bag_start_time =Convert.ToDateTime( Request.Params["bag_start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["bag_end_time"]))
                {
                    query.bag_end_time = Convert.ToDateTime(Request.Params["bag_end_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["bag_show_start_time"]))
                {
                    query.bag_show_start_time =  Convert.ToDateTime(Request.Params["bag_show_start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["bag_show_end_time"]))
                {
                    query.bag_show_end_time =Convert.ToDateTime(Request.Params["bag_show_end_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["product_number"]))
                {
                    query.product_number =Convert.ToInt32(Request.Params["product_number"]);
                }
                if (string.IsNullOrEmpty(Request.Params["bag_id"]))//新增
                {
                    query.bag_create_user=(Session["caller"] as Caller).user_id;
                    query.bag_update_user = (Session["caller"] as Caller).user_id;
                    query.bag_create_time = DateTime.Now;
                    query.bag_update_time = query.bag_create_time;
                }
                else//編輯
                {
                    query.bag_id = Convert.ToInt32(Request.Params["bag_id"]);
                    query.bag_update_time = DateTime.Now;
                    query.bag_update_user = (Session["caller"] as Caller).user_id;
                }
                _eventChinaTrustBag = new EventChinaTrustBagMgr(mySqlConnectionString);
                #region  上傳圖片
                string path = Server.MapPath(xmlPath);
                SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
                SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_Min_Element");
                SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
                SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
                string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
                string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
                string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
                string localPromoPath = imgLocalPath + PaperPath;//圖片存儲地址
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string fileName = string.Empty;//當前文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    FileManagement fileLoad = new FileManagement();
                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();
                        string NewFileName = string.Empty;
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localPromoPath.Substring(0, localPromoPath.Length - PaperPath.Length + 1), PaperPath.Substring(1, PaperPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        fileName = NewFileName + fileExtention;
                        NewFileName = localPromoPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + PaperPath);
                        string ErrorMsg = string.Empty;
                        if (query.bag_id != 0)
                        {
                            oldModel = _eventChinaTrustBag.GetSinggleData(query);
                            if (oldModel.bag_banner != "")
                                {
                                    string oldFileName = oldModel.bag_banner;
                                    CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除
                                    FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                                    List<string> tem = ftp.GetFileList();
                                    if (tem.Contains(oldFileName))
                                    {
                                        FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                                        ftps.DeleteFile(localPromoPath + oldFileName);
                                    }
                                }
                        }

                        try
                        {
                            Resource.CoreMessage = new CoreResource("Product");
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)
                            {
                                query.bag_banner = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                        }
                        if (!string.IsNullOrEmpty(ErrorMsg))
                        {
                            string jsonStr = string.Empty;
                            json = "{success:true,msg:\"" + ErrorMsg + "\"}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                        }
                    }
                }
                else
                {
                    query.bag_banner = oldModel.bag_banner;
                }
                #endregion
          
                if (_eventChinaTrustBag.EventChinaTrustBagSave(query) > 0)
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
        #region 區域包更改狀態
        public JsonResult UpdateActive()
        {

            {
                try
                {
                    EventChinaTrustBagQuery query = new EventChinaTrustBagQuery();
                    if (!string.IsNullOrEmpty(Request.Params["bag_id"].ToString()))
                    {
                        query.bag_id = Convert.ToInt32(Request.Params["bag_id"].ToString());
                    }
                    query.bag_active = Convert.ToInt32(Request.Params["bag_active"] ?? "0");
                    _eventChinaTrustBag = new EventChinaTrustBagMgr(mySqlConnectionString);



                    query.bag_update_user = (Session["caller"] as Caller).user_id;
                    query.bag_update_time = DateTime.Now;
                    if (_eventChinaTrustBag.EventChinaTrustBagStatus(query) > 0)
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    return Json(new { success = "false" });
                }

            }
        }
        #endregion
        #region 獲取bag數據
        public HttpResponseBase GetBag()
        {
            List<EventChinaTrustBag> store = new List<EventChinaTrustBag>();
            string json = string.Empty;
            try
            {
                EventChinaTrustBagQuery query = new EventChinaTrustBagQuery();
                _eventChinaTrustBag = new EventChinaTrustBagMgr(mySqlConnectionString);
                //query.event_status = 1;
                store = _eventChinaTrustBag.GetBag(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 中信區域包商品
        #region 中信區域包商品列表頁
        [CustomHandleError]
        public HttpResponseBase GetChinaTrustBagMapList()
        {
            List<EventChinaTrustBagMapQuery> store = new List<EventChinaTrustBagMapQuery>();
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                _ChinatrustBMMgr = new EventChinaTrustBagMapMgr(mySqlConnectionString);

                EventChinaTrustBagMapQuery query = new EventChinaTrustBagMapQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                //if (!string.IsNullOrEmpty(Request.Params["bag_id"]))
                //{
                //    query.bag_id = Convert.ToInt32(Request.Params["bag_id"]);
                //}
                if (!string.IsNullOrEmpty(Request.Params["bagid"]))
                {
                    query.bag_id = Convert.ToInt32(Request.Params["bagid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                {
                    query.search_con = Convert.ToInt32(Request.Params["search_con"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["con"]))
                {
                    query.con = Request.Params["con"];
                }
                store = _ChinatrustBMMgr.GetChinaTrustBagMapList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                foreach (var item in store)
                {
                    item.link = link;
                    if (item.product_forbid_banner != "")
                    {
                        item.forbid_banner = imgServerPath + PaperPath + item.product_forbid_banner;
                    }
                    else
                    {
                        item.forbid_banner = defaultImg;
                    }
                    if (item.product_active_banner != "")
                    {
                        item.active_banner = imgServerPath + PaperPath + item.product_active_banner;
                    }
                    else
                    {
                        item.active_banner = defaultImg;
                    }
                }
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 中信商品新增/編輯
        public HttpResponseBase SaveChinaTrustBagMap()
        {
            string json = string.Empty;
            bool pic = true;
            DataTable dt = new DataTable();
            try
            {
                _ChinatrustBMMgr = new EventChinaTrustBagMapMgr(mySqlConnectionString);
                EventChinaTrustBagMapQuery store = new EventChinaTrustBagMapQuery();
                EventChinaTrustBagMapQuery query = new EventChinaTrustBagMapQuery();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {//如果是編輯獲取該id數據
                    int totalCount = 0;
                    query.IsPage = false;
                    query.map_id = int.Parse(Request.Params["id"]);
                    store = _ChinatrustBMMgr.GetChinaTrustBagMapList(query, out totalCount).FirstOrDefault();
                    query = store;
                }
                //product_id
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["product_id"]);
                }
                else
                {
                    query.product_id = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["bag_id"]))
                {
                    query.bag_id = Convert.ToInt32(Request.Params["bag_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["linkurl"]))
                {
                    query.linkurl = Request.Params["linkurl"];
                }
                if (!string.IsNullOrEmpty(Request.Params["product_forbid_banner"]))
                {
                    query.product_forbid_banner = Request.Params["product_forbid_banner"];
                }
                if (!string.IsNullOrEmpty(Request.Params["product_active_banner"]))
                {
                    query.product_active_banner = Request.Params["product_active_banner"];
                }
                if (!string.IsNullOrEmpty(Request.Params["ad_product_id"]))
                {
                    string p_id = Request.Params["ad_product_id"];
                    RegexOptions options = RegexOptions.None;
                    Regex regex = new Regex(@"[ ]{2,}", options);
                    p_id = regex.Replace(p_id, @" ");
                    p_id = p_id.Replace(" ", ",");
                    for (int i = p_id.Length - 1; i > 0; i--)
                    {
                        if (p_id.Substring(p_id.Length - 1, 1) == ",")
                        {
                            p_id = p_id.Substring(0, p_id.Length - 1);
                        }
                    }
                    string[] pr_id = p_id.Split(',').Distinct().ToArray(); ;
                    string pro_id = "";
                    for (int i = 0; i < pr_id.Length; i++)
                    {
                        if (_ChinatrustBMMgr.IsProductId(pr_id[i].ToString()))
                        {
                            pro_id += pr_id[i].ToString() + ",";
                        }
                    }
                    if (pro_id.Length > 0)
                    {
                        query.ad_product_id = pro_id.Substring(0, pro_id.Length - 1);
                    }
                    else
                    {
                        query.ad_product_id = null;
                    }
                }
                else
                {
                    query.ad_product_id = null;
                }
                if (!string.IsNullOrEmpty(Request.Params["product_desc"]))
                {
                    query.product_desc = Request.Params["product_desc"];
                }
                else
                {
                    query.product_desc = null;
                }
                #region 上傳圖片
                try
                {
                    string path = Server.MapPath(xmlPath);
                    SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                    SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
                    SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
                    SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                    SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
                    SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
                    //擴展名、最小值、最大值
                    string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
                    string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
                    string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
                    string localPromoPath = imgLocalPath + PaperPath;//圖片存儲地址
                    Random rand = new Random();
                    FileManagement fileLoad = new FileManagement();
                    for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                    {
                        //int newRand = rand.Next(1000, 9999);
                        HttpPostedFileBase file = Request.Files[iFile];
                        string fileName = string.Empty;//當前文件名
                        string fileExtention = string.Empty;//當前文件的擴展名
                        bool result = false;
                        string NewFileName = string.Empty;
                        string ServerPath = string.Empty;
                        fileName = Path.GetFileName(file.FileName);
                        string newRand = hash.Md5Encrypt(fileLoad.NewFileName(fileName), "32");
                        string ErrorMsg = string.Empty;
                        if (iFile == 0)
                        {
                            if (string.IsNullOrEmpty(file.FileName))
                            {
                                query.product_forbid_banner = store.product_forbid_banner;
                            }
                            else
                            {
                                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                NewFileName = newRand + fileExtention;
                                //判斷目錄是否存在，不存在則創建
                                string[] mapPath = new string[1];
                                mapPath[0] = PaperPath.Substring(1, PaperPath.Length - 1);
                                string s = localPromoPath.Substring(0, localPromoPath.Length - PaperPath.Length + 1);
                                CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - PaperPath.Length + 1), mapPath);
                                //  returnName += PaperPath + NewFileName;
                                fileName = NewFileName;
                                NewFileName = localPromoPath + NewFileName;//絕對路徑
                                ServerPath = Server.MapPath(imgLocalServerPath + PaperPath);
                                string oldFileName = "";
                                oldFileName = store.product_forbid_banner;
                                if (!string.IsNullOrEmpty(oldFileName))
                                {
                                    DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                }
                                try
                                {   //上傳
                                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                    if (result)//上傳成功
                                    {
                                        query.product_forbid_banner = fileName;
                                    }
                                    else
                                    {
                                        pic = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    query.product_forbid_banner = store.product_forbid_banner;
                                    query.product_active_banner = store.product_active_banner;
                                }
                            }
                        }
                        if (iFile == 1)
                        {
                            if (string.IsNullOrEmpty(file.FileName))
                            {
                                query.product_active_banner = store.product_active_banner;
                            }
                            else
                            {
                                fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                NewFileName = newRand + fileExtention;
                                //判斷目錄是否存在，不存在則創建
                                string[] mapPath = new string[1];
                                mapPath[0] = PaperPath.Substring(1, PaperPath.Length - 1);
                                string s = localPromoPath.Substring(0, localPromoPath.Length - PaperPath.Length + 1);
                                CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - PaperPath.Length + 1), mapPath);
                                //  returnName += PaperPath + NewFileName;
                                fileName = NewFileName;

                                NewFileName = localPromoPath + NewFileName;//絕對路徑
                                ServerPath = Server.MapPath(imgLocalServerPath + PaperPath);
                                string oldFileName = "";
                                oldFileName = store.product_active_banner;
                                if (!string.IsNullOrEmpty(oldFileName))
                                {
                                    DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                }
                                try
                                {
                                    //上傳
                                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                    if (result)//上傳成功
                                    {
                                        query.product_active_banner = fileName;
                                    }
                                    else
                                    {
                                        pic = false;
                                    }
                                }
                                catch (Exception)
                                {
                                    query.product_forbid_banner = store.product_forbid_banner;
                                    query.product_active_banner = store.product_active_banner;
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    query.product_forbid_banner = store.product_forbid_banner;
                    query.product_active_banner = store.product_active_banner;
                }
                #endregion             
              
                if (pic)
                {
                    if (query.map_id > 0)
                    {//編輯
                        if (!string.IsNullOrEmpty(Request.Params["map_sort"]))
                        {
                            query.map_sort = Convert.ToInt32(Request.Params["map_sort"]);
                        }
                        if (_ChinatrustBMMgr.Update(query) > 0)
                        {
                            json = "{success:true,msg:'修改成功!'}";
                        }
                        else
                        {
                            json = "{success:false,msg:'修改失敗!'}";
                        }
                    }
                    else
                    {//新增
                        dt = _ChinatrustBMMgr.GetMapSort(query);
                        if (dt.Rows.Count > 0)
                        {
                            query.map_sort = int.Parse(dt.Rows[0]["map_sort"].ToString()) + 1;
                        }
                        else
                        {
                            query.map_sort = 0;
                        }
                        query.map_active = 0;//默認不啟用
                        if (_ChinatrustBMMgr.Save(query) > 0)
                        {
                            json = "{success:true,msg:'新增成功!'}";
                        }
                        else
                        {
                            json = "{success:false,msg:'新增失敗!'}";
                        }
                    }
                }
                else
                {
                    json = "{success:false,msg:'圖片上傳失敗!'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'操作失敗!'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 中信商品更改狀態
        public HttpResponseBase UpdateStats()
        {
            List<EventChinaTrustBagMapQuery> store = new List<EventChinaTrustBagMapQuery>();
            string json = string.Empty;
            try
            {
                _ChinatrustBMMgr = new EventChinaTrustBagMapMgr(mySqlConnectionString);
                EventChinaTrustBagMapQuery query = new EventChinaTrustBagMapQuery();

                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.map_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.map_active = Convert.ToInt32(Request.Params["status"]);
                }

                if (_ChinatrustBMMgr.UpdateStatus(query) > 0)
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
            return this.Response;
        }


        #endregion
        #region 創建ftp文件夾
        public void CreateFolder(string path, string[] Mappath)
        {
            FTP ftp = null;
            try
            {
                string fullPath = path;
                foreach (string s in Mappath)
                {
                    ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                    fullPath += s;

                    if (!ftp.DirectoryExist(s.Replace("/", "")))
                    {
                        ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                        ftp.MakeDirectory();
                    }

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
        #region 刪除本地上傳的圖片
        public void DeletePicFile(string imageName)
        {
            if (System.IO.File.Exists(imageName))
            {
                System.IO.File.Delete(imageName);
            }
        }
        #endregion
        #region 獲取link前綴
        public HttpResponseBase GetLink()
        {
            string json = string.Empty;
            if (link.Length > 0)
            {
                json = "{success:true,msg:'" + link + "'}";
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
