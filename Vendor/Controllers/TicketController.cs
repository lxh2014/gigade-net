using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using System.Configuration;
using Vendor.CustomHandleError;

namespace Vendor.Controllers
{
    public class TicketController : Controller
    {
        //
        // GET: /Ticket/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private ITicketDetailImplMgr _ITicketDetail;
        private ITicketMasterImplMgr _ITicketMaster;
        private ICourseImplMgr _courseMgr;
        private IVendorImplMgr _IVendorMgr;
        private IParametersrcImplMgr _ptersrc;
        private IVendorImplMgr _vendorMgr;

        #region 上傳web
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.10:2121"
        string NewPromoPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.NewPromoPath);//圖片保存路徑
        string archives = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.archives);
        #endregion
        #region View
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 課程訂購單明細
        /// </summary>
        /// <returns></returns>
        public ActionResult TicketDetail()
        {
            string master_id = Request.Params["ticket_master_id"] ?? "";
            ViewBag.master_id = master_id;
            return View();
        }
        public ActionResult TicketMaster()
        {
            string course_id = Request.Params["course_id"] ?? "";
            ViewBag.course_id = course_id;
            return View();
        }
        public ActionResult TicketCount()
        {
            return View();
        }
        /// <summary>
        /// 檔案web上傳
        /// </summary>
        /// <returns></returns>
        public ActionResult Archives()
        {
            return View();
        }
        #endregion
        //#region 課程訂單列表頁
        //public HttpResponseBase GetTicketMasterList()
        //{
        //    string json = string.Empty;
        //    int tranInt = 0;
        //    try
        //    {
        //        TicketMasterQuery query = new TicketMasterQuery();
        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
        //        int totalCount = 0;
        //        _ITicketMaster = new TicketMasterMgr(mySqlConnectionString);
        //        if (!string.IsNullOrEmpty(Request.Params["order_status"]))
        //        {
        //            query.order_status = int.Parse(Request.Params["order_status"]);
        //        }
        //        else
        //        {
        //            query.order_status = -1;
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["order_payment"]))
        //        {
        //            query.order_payment = int.Parse(Request.Params["order_payment"]);
        //        }
        //        else
        //        {
        //            query.order_payment = -1;
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["order_id"]))
        //        {
        //            query.ticket_master_id = int.Parse(Request.Params["order_id"]);
        //        }
        //        query.order_name = Request.Params["order_name"];
        //        if (!string.IsNullOrEmpty(Request.Params["ticket_start"]))
        //        {
        //            query.order_start = (int)CommonFunction.GetPHPTime(Request.Params["ticket_start"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ticket_end"]))
        //        {
        //            query.order_end = (int)CommonFunction.GetPHPTime(Request.Params["ticket_end"].Substring(0, 10) + " 23:59:59");
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["bill_check"]))
        //        {
        //            query.billing_checked = int.Parse(Request.Params["bill_check"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["course_search"]))
        //        {
        //            if (int.TryParse(Request.Params["course_search"], out tranInt))
        //            {
        //                query.course_id = tranInt;
        //            }
        //            else
        //            {
        //                query.course_name = Request.Params["course_search"];
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["course_start"]))
        //        {
        //            query.start_date = DateTime.Parse(Request.Params["course_start"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["course_end"]))
        //        {
        //            query.end_date = DateTime.Parse(Request.Params["course_end"].Substring(0, 10) + " 23:59:59");
        //        }
        //        DataTable dtTicketMaster = _ITicketMaster.GetTicketMasterList(query, out totalCount);

        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dtTicketMaster, Formatting.Indented, timeConverter) + "}";//返回json數據
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,msg:0}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //public HttpResponseBase UpdateTicketMaster()
        //{
        //    string json = string.Empty;
        //    try
        //    {
        //        TicketMaster tm = new TicketMaster();
        //        _ITicketMaster = new TicketMasterMgr(mySqlConnectionString);
        //        if (Request.Params["ticket_master_id"] != "0")
        //        {
        //            tm.ticket_master_id = int.Parse(Request.Params["ticket_master_id"]);
        //        }
        //        tm.delivery_name = Request.Params["delivery_name"];
        //        tm.delivery_mobile = Request.Params["delivery_mobile"];
        //        tm.delivery_phone = Request.Params["delivery_phone"];
        //        tm.delivery_address = Request.Params["delivery_address"];
        //        if (!string.IsNullOrEmpty(Request.Params["delivery_zip"]))
        //        {
        //            tm.delivery_zip = int.Parse(Request.Params["delivery_zip"]);
        //        }
        //        tm.order_name = Request.Params["order_name"];
        //        tm.order_mobile = Request.Params["order_mobile"];
        //        tm.order_phone = Request.Params["order_phone"];
        //        tm.order_address = Request.Params["order_address"];
        //        if (!string.IsNullOrEmpty(Request.Params["order_zip"]))
        //        {
        //            tm.order_zip = int.Parse(Request.Params["order_zip"]);
        //        }
        //        int i = _ITicketMaster.Update(tm);
        //        if (i > 0)
        //        {
        //            json = "{success:true}";//返回json數據
        //        }
        //        else
        //        {
        //            json = "{success:false}";//返回json數據
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,msg:0}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion

        #region 課程訂購單明細表
        /// <summary>
        /// 訂單詳情頁面列表
        /// </summary>
        /// <returns></returns>

        public HttpResponseBase GetTicketDetailList()
        {
            string jsonStr = string.Empty;
            try
            {
                TicketDetailQuery query = new TicketDetailQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                query.flag = -1;
                if (!string.IsNullOrEmpty(Request.Params["Search"]))
                {
                    query.flag = int.Parse(Request.Params["Search"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["MasterID"]))
                {
                    query.MDID = int.Parse(Request.Params["MasterID"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["TimeStart"]))
                {
                    query.TimeStart = DateTime.Parse(Request.Params["TimeStart"]).ToString("yyyy/MM/dd 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["TimeEnd"]))
                {
                    query.TimeEnd = DateTime.Parse(Request.Params["TimeEnd"]).ToString("yyyy/MM/dd 23:59:59");
                }

                long a = CommonFunction.GetPHPTime("2014-02-01 00:00:00");
                long s = CommonFunction.GetPHPTime("2014-02-28 23:29:59");
                int totalCount = 0;
                _ITicketDetail = new TicketDetailMgr(mySqlConnectionString);

                DataTable _dt = _ITicketDetail.GetTicketDetailTable(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        /// <summary>
        /// 訂單詳情頁面列表核銷功能
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UpdateTicketStatus()
        {

            //更改course_ticket表中的flag欄位
            string json = string.Empty;
            try
            {
                string Row_id = "";
                if (!string.IsNullOrEmpty(Request.Params["rowId"]))
                {
                    Row_id = Request.Params["rowId"];
                    Row_id = Row_id.TrimEnd(',');
                    //query.map_id_in = Row_id;
                }
                _ITicketDetail = new TicketDetailMgr(mySqlConnectionString);
                int result = _ITicketDetail.UpdateTicketStatus(Row_id);
                if (result > 0)
                {
                    json = "{success:true,msg:\"" + result + "\"}";
                }
                else
                {
                    json = "{success:false,msg:\"" + result + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion
       
        #region 課程統計
        public HttpResponseBase GetCourseCountList()
        {
            CourseQuery query = new CourseQuery();
            string json = string.Empty;
            int isTranInt = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["select_vendor"]))
                {
                    query.Vendor_Id = int.Parse(Request.Params["select_vendor"]);
                }
               
                if (!string.IsNullOrEmpty(Request.Params["select_content"]))
                {
                    if (int.TryParse(Request.Params["select_content"].Trim(), out isTranInt))
                    {
                        query.Course_Id = Convert.ToInt32(Request.Params["select_content"].Trim());
                    }
                    else
                    {
                        query.Course_Name = (Request.Params["select_content"]).Trim();
                    }
                }
               
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.Start_Date = Convert.ToDateTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.End_Date =  Convert.ToDateTime(Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"));
                }
                int totalCount = 0;
                _ITicketMaster = new TicketMasterMgr(mySqlConnectionString);
                DataTable _dt = _ITicketMaster.GetCourseCountList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";
            }
            catch (Exception ex)
            {
                Vendor.Log4NetCustom.LogMessage logMessage = new Vendor.Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion
      

        #region 上傳檔案到web
        public HttpResponseBase GetUploadArchives()
        {
            string json = string.Empty;

            try
            {
                #region 上傳

                string ErrorMsg = string.Empty;
                string path = Server.MapPath(xmlPath);
                SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
                SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
                SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
                //擴展名、最小值、最大值
                string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
                string fileName = string.Empty;//當前文件名

                _ITicketDetail = new TicketDetailMgr(mySqlConnectionString);
                FileManagement fileLoad = new FileManagement();
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];

                    string fileExtention = string.Empty;//當前文件的擴展名
                    string oldFileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
                    fileName = fileLoad.NewFileName(file.FileName);

                    if (fileName != "")
                    {
                        string filepathday = fileName.Substring(0, fileName.LastIndexOf("."));//每天建立一個文件夾保存村的文件;

                        fileName = oldFileName + "_" + filepathday;//上傳文檔為以前的名字+年月日時分秒.後綴名
                        filepathday = filepathday.Substring(0, 8);
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();
                        // string NewFileName = string.Empty;

                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        // NewFileName = hash.Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        FTP f_cf = new FTP();
                        archives = archives + filepathday + "/";//創建多層路徑
                        string localPromoPath = imgLocalPath + archives;//圖片存儲地址
                        f_cf.MakeMultiDirectory(localPromoPath.Substring(0, localPromoPath.Length - archives.Length + 1), archives.Substring(1, archives.Length - 2).Split('/'), ftpuser, ftppwd);
                        // fileName = NewFileName + fileExtention;
                        fileName = localPromoPath + fileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + archives);

                        Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                        bool result = _ITicketDetail.UpLoadFile(file, ServerPath, fileName, extention, (int.MaxValue / 1024) - 1, 0, ref ErrorMsg, ftpuser, ftppwd);
                        if (result)
                        {
                            json = "{\"success\":\"true\"}";
                        }
                        else
                        {
                            json = "{\"success\":\"false\",\"msg\":\"上傳失败\"}";

                        }

                    }

                }
                #endregion

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{\"success\":\"false\",\"msg\":\"參數出錯\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取所有的供應商名稱 +HttpResponseBase GetVendor()
        /// <summary>
        /// 獲取所有的供應商名稱
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVendor()
        {
            _vendorMgr = new VendorMgr(mySqlConnectionString);
            List<BLL.gigade.Model.Vendor> stores = new List<BLL.gigade.Model.Vendor>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                stores = _vendorMgr.VendorQueryAll(new BLL.gigade.Model.Vendor());
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
            }
            catch (Exception ex)
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
        #region 付款單狀態+HttpResponseBase GetPayMentType()
        public HttpResponseBase GetPayMentType()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            _ptersrc = new ParameterMgr(mySqlConnectionString);

            string json = string.Empty;
            try
            {
                stores = _ptersrc.PayforType("order_status");
                //for (int i = 0; i <stores.Count; i++)
                //{
                //    pc = new Parametersrc();
                //    pc.ParameterCode = stores[i].ParameterCode;
                //    pc.remark = stores[i].ParameterCode;
                //    stores.Add(pc);

                //}
                //pc = new Parametersrc();
                //pc.remark = "所有付款單方式";
                //stores.Insert(9999, pc);
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據

            }
            catch (Exception ex)
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
        #region 付款方式下拉列表+string QueryPara()
        [CustomHandleError]
        [OutputCache(Duration = 3600, VaryByParam = "paraType", Location = System.Web.UI.OutputCacheLocation.Client)]
        public string QueryPara()
        {
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    json = _ptersrc.Query(Request.QueryString["paraType"].ToString());
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
        #endregion

    }
}
