using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Dao;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{ 
    public class PromoShareController : Controller
    {
        //
        // GET: /PromoShare/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private PromoShareMasterMgr PshareMgr;
        private PromoShareConditionMgr PshareConMgr;
        private string[] condition = { "device", "product_id", "after_user_time", "after_first_buy_time", "consume_money", "buy_count", "vendor_tip", "bonus", "voucher", "freight", "isrepeat", "multiple", "by_url", "picture", "group_id", "payType", "site_id" };//此處數組中元素與js中保持一致
        //上傳圖片
        string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值

        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        //string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.promoPath);//圖片保存路徑

        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        private ISerialImplMgr _seriMgr = new SerialMgr(mySqlConnectionString);

        //end 上傳圖片
        public ActionResult Index()
        {
            return View();
        }
        public HttpResponseBase PromoShareList()
        {
            PromoShareMasterQuery query = new PromoShareMasterQuery();
            DataTable _dt = new DataTable();
            string json = string.Empty;
            int  totalCount = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["ddlstatus"]))
                {
                query.promo_active = Convert.ToInt32(Request.Params["ddlstatus"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["promo_name_list"]))
                { 
                 query.promo_name=Request.Params["promo_name_list"];
                }
                PshareMgr = new PromoShareMasterMgr(mySqlConnectionString);
                _dt = PshareMgr.GetList(query, out totalCount);
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase PromoShareConditionList()
        {
            PromoShareConditionQuery query = new PromoShareConditionQuery();
            List<PromoShareConditionQuery> store = new List<PromoShareConditionQuery>();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["promo_id"]))
                {
                    query.promo_id = Convert.ToInt32(Request.Params["promo_id"]);
                }
                PshareConMgr = new PromoShareConditionMgr(mySqlConnectionString);
                store = PshareConMgr.GetList(query);
                if (store.Count>0)
                {
                    for (int i = 0; i < store.Count; i++)
                    {
                        //處理數據使"條件名稱"與"對應值"顯示對應描述
                        //store[i].
                    }   
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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

        #region 新增/編輯
        #region 新增第一步
        public HttpResponseBase InsertIntoPromoShareMaster()
        {
            PromoShareMasterQuery query = new PromoShareMasterQuery();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["promo_name"]))
                {
                    query.promo_name = Request.Params["promo_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["promo_desc"]))
                {
                    query.promo_desc = Request.Params["promo_desc"];
                }
                if (!string.IsNullOrEmpty(Request.Params["promo_start"]))
                {
                    query.promo_start = Convert.ToDateTime(Request.Params["promo_start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["promo_end"]))
                {
                    query.promo_end = Convert.ToDateTime(Request.Params["promo_end"]);
                }
                PshareMgr = new PromoShareMasterMgr(mySqlConnectionString);
                int PromoId = PshareMgr.Add(query);
                string strPromoId ="SH";
                if (PromoId > 0)
                {
                    if (PromoId.ToString().Length < 6)
                    {
                        for (int i = 0; i < 6 - PromoId.ToString().Length; i++)
                        {
                            strPromoId += "0";
                        }
                    }
                    strPromoId += PromoId.ToString();
                    query = new PromoShareMasterQuery();
                    query.promo_event_id = strPromoId;
                    query.promo_id = PromoId;
                    query.eventId = true;
                    if (PshareMgr.Update(query) > 0)
                    {
                        json = "{success:'true',strPromoId:'" + strPromoId + "',PromoId:'" + PromoId + "'}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
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
        #region 新增第二步
        public void InsertIntoPromoShareCon(PromoShareCondition query)
        {
            List<PromoShareCondition> list = new List<PromoShareCondition>();
            PshareConMgr = new PromoShareConditionMgr(mySqlConnectionString);
            string json = string.Empty;
             DataTable _dt=new DataTable();
            try
            {
                for (int i = 0; i < condition.Length; i++)
                {
                    if (!string.IsNullOrEmpty(Request.Params[condition[i]]))
                    {
                        query = new PromoShareConditionQuery();
                        query.promo_id = Convert.ToInt32(Request.Params["promo_id"]);
                        query.condition_name = condition[i];
                        query.condition_value = Request.Params[ condition[i]];
                        if (query.condition_name == "picture")
                        {
                            #region 上傳圖片
                            string ErrorMsg = string.Empty;
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
                            string localPromoPath = imgLocalPath + promoPath;//圖片存儲地址
                            FileManagement fileLoad = new FileManagement();
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
                                    HashEncrypt hash = new HashEncrypt();
                                    NewFileName = hash.Md5Encrypt(fileName, "32");
                                    string ServerPath = string.Empty;
                                    FTP f_cf = new FTP();
                                    f_cf.MakeMultiDirectory(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'), ftpuser, ftppwd);
                                    fileName = NewFileName + fileExtention;
                                    NewFileName = localPromoPath + NewFileName + fileExtention;//絕對路徑
                                    ServerPath = Server.MapPath(imgLocalServerPath + promoPath);

                                    //上傳之前刪除已有的圖片
                                    if (query.promo_id != 0)
                                    {
                                        _dt = PshareConMgr.Get(condition, query);
                                        if (_dt.Rows[0]["picture"].ToString() != "")
                                        {
                                            string oldFileName = _dt.Rows[0]["picture"].ToString();
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
                                        Resource.CoreMessage = new CoreResource("Product");
                                        bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                        if (result)
                                        {
                                            query.condition_value = fileName;
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
                                query.condition_value = _dt.Rows[0]["picture"].ToString();
                            }
                            #endregion
                        }
                        list.Add(query);
                    }
                }
             
                PshareConMgr.AddSql(list);
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
        #endregion

        #region  編輯 第一步 已棄用
        //public HttpResponseBase EditPromoShareMaster()
        //{
        //    PromoShareMaster query = new PromoShareMaster();
        //    string json = string.Empty;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["Epromo_id"]))
        //        {
        //            query.promo_id = Convert.ToInt32(Request.Params["Epromo_id"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["Epromo_name"]))
        //        {
        //            query.promo_name = Request.Params["Epromo_name"];
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["Epromo_desc"]))
        //        {
        //            query.promo_desc = Request.Params["Epromo_desc"];
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["Epromo_start"]))
        //        {
        //            query.promo_start = Convert.ToDateTime(Request.Params["Epromo_start"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["Epromo_end"]))
        //        {
        //            query.promo_end = Convert.ToDateTime(Request.Params["Epromo_end"]);
        //        }
        //        PshareMgr = new PromoShareMasterMgr(mySqlConnectionString);
        //        if (PshareMgr.Update(query) > 0)
        //        {
        //            json = "{success:true}";
        //        }
        //        else
        //        {
        //            json = "{success:false}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        #endregion

        #region 編輯
        public HttpResponseBase updatePromoShareCon()
        {
            PromoShareMaster psmQuery = new PromoShareMaster();
              PromoShareCondition pscQuery = new PromoShareCondition();
            PshareMgr = new PromoShareMasterMgr(mySqlConnectionString);
            PshareConMgr = new PromoShareConditionMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
            if (!string.IsNullOrEmpty(Request.Params["promo_name"]))
            {
                psmQuery.promo_name = Request.Params["promo_name"];
            }
            if (!string.IsNullOrEmpty(Request.Params["promo_desc"]))
            {
                psmQuery.promo_desc = Request.Params["promo_desc"];
            }
            if (!string.IsNullOrEmpty(Request.Params["promo_start"]))
            {
                psmQuery.promo_start = Convert.ToDateTime(Request.Params["promo_start"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["promo_end"]))
            {
                psmQuery.promo_end = Convert.ToDateTime(Request.Params["promo_end"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["promo_id"]))
            {
                psmQuery.promo_id = Convert.ToInt32(Request.Params["promo_id"]);
            }

            //判斷如果condition表內無此編號的數據則insert，如果有此編號的數據則update
            if (PshareMgr.PromoCon(psmQuery) == 0)
            {//執行新增
                InsertIntoPromoShareCon(pscQuery);//插入promo_share_condition；新增入第二個面板
            }
            else
            {//執行編輯
                //編輯第一個面板
                PshareMgr.Update(psmQuery);
                //1編輯第二個面板 promo_share_condition, 
                //2將promo_share_condition表中對應promo_id數據刪除 
                //3然後再次insert
                PshareConMgr.Delete(psmQuery.promo_id);
                InsertIntoPromoShareCon(pscQuery);
                //PshareConMgr.Update(pscQuery);
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }


        #endregion

        #endregion

        public JsonResult UpdateActivePromoShareMaster()
        {
            try
            {
                int row_id = 0;
                if (!string.IsNullOrEmpty(Request.Params["row_id"].ToString()))
                {
                    row_id = Convert.ToInt32(Request.Params["row_id"].ToString());
                }
                int activeValue = Convert.ToInt32(Request.Params["active"] ?? "0");
                PshareMgr = new PromoShareMasterMgr(mySqlConnectionString);
                PromoShareMaster model = new PromoShareMaster();
                model.promo_id = row_id;
                model.promo_active = activeValue;
                if (PshareMgr.UpdateActivePromoShareMaster(model) > 0)
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


        public HttpResponseBase GetPromoShareConditionCount()
        {
            PromoShareCondition query = new PromoShareCondition();
            string json = string.Empty;
            int count = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["promo_id"]))
                {
                    query.promo_id = Convert.ToInt32(Request.Params["promo_id"]);
                }
                PshareConMgr = new PromoShareConditionMgr(mySqlConnectionString);
                count = PshareConMgr.GetPromoShareConditionCount(query);
               
                json = "{success:true,Count:" + count + "}";
            }

            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,Count:0}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }


        public JsonResult DeletePromoShareMessage()
        {
            try
            {
                string str = Request.Params["rid"];//獲取類型
                str = str.Substring(0, str.LastIndexOf(","));
                PshareMgr = new PromoShareMasterMgr(mySqlConnectionString);
                if (PshareMgr.DeletePromoShareMessage(str) > 0)
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

        public HttpResponseBase GetEditData()
        {
           // List<PromoShareCondition> store = new List<PromoShareCondition>();
            PromoShareCondition query = new PromoShareCondition();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["PromoId"]))
                {
                    query.promo_id = Convert.ToInt32(Request.Params["PromoId"]);
                }
                PshareConMgr = new PromoShareConditionMgr(mySqlConnectionString);
                DataTable _dt=PshareConMgr.Get(condition, query);
            //    _dt.Columns.Add("promo_event_id",typeof (string));
             //   string strPromoId = "SH";
                //if (_dt.Rows[0]["promo_id"].ToString().Length < 6)
                //{
                //    for (int i = 0; i <6-_dt.Rows[0]["promo_id"].ToString().Length; i++)
                //    {
                //        strPromoId += "0";
                //    }
                //}
                //strPromoId += _dt.Rows[0]["promo_id"].ToString();
                //_dt.Rows[0]["promo_event_id"] = strPromoId;
                json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";
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

    }
}
