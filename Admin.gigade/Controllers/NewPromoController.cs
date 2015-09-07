using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using gigadeExcel.Comment;

namespace Admin.gigade.Controllers
{
    public class NewPromoController : Controller
    {
        //
        // GET: /NewPromo/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private INewPromoQuestionImplMgr _INewPromoQuestionMgr;
        private INewPromoCarnetImplMgr _INewPromoCarnetMgr;
        private INewPromoPresentImplMgr _INewPromoPresentMgr;
        private INewPromoRecordImplMgr _INewPromoRecordMgr;
        //上傳圖片
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.10:2121"
        string NewPromoPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.NewPromoPath);//圖片保存路徑

        //end 上傳圖片
        #region view
        /// <summary>
        /// 問卷送禮
        /// </summary>
        /// <returns></returns>
        public ActionResult NewPromoQuestionnaireList()
        {
            return View();
        }
        /// <summary>
        /// 通關密語
        /// </summary>
        /// <returns></returns>
        public ActionResult NewPromoCarnetList()
        {
            return View();
        }
        /// <summary>
        /// 獎品
        /// </summary>
        /// <returns></returns>
        public ActionResult NewPromoPresent()
        {
            //ViewBag.event_id = Request.Params["event_id"];
            return View();
        }
        #endregion

        #region 問卷送禮
        public HttpResponseBase PromoQuestionnaireList()
        {
            string jsonStr = string.Empty;

            try
            {
                NewPromoQuestionQuery query = new NewPromoQuestionQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["SearchTime"]))
                {
                    query.searchtype = int.Parse(Request.Params["SearchTime"]);
                    query.end = DateTime.Now;
                }
                int totalCount = 0;
                _INewPromoQuestionMgr = new NewPromoQuestionMgr(mySqlConnectionString);

                DataTable _dt = _INewPromoQuestionMgr.GetPromoQuestionList(query, out totalCount);
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (_dt.Rows[i]["promo_image"].ToString() != "")
                    {
                        _dt.Rows[i]["s_promo_image"] = imgServerPath + NewPromoPath + _dt.Rows[i]["promo_image"];
                    }
                    else
                    {
                        _dt.Rows[i]["s_promo_image"] = defaultImg;
                    }
                }
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
        /// 分步保存第二步
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveQuestion()
        {
            string json = string.Empty;
            try
            {
                _INewPromoQuestionMgr = new NewPromoQuestionMgr(mySqlConnectionString);
                NewPromoQuestionQuery query = new NewPromoQuestionQuery();
                NewPromoQuestionQuery oldModel = new NewPromoQuestionQuery();
                #region 需要更改的屬性
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["row_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Request.Params["event_id"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["present_event_id"]))
                {
                    query.present_event_id = Request.Params["present_event_id"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_desc"]))
                {
                    query.event_desc = Request.Params["event_desc"];
                }
                if (!string.IsNullOrEmpty(Request.Params["start"]))
                {
                    query.start = DateTime.Parse(Request.Params["start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end"]))
                {
                    query.end = DateTime.Parse(Request.Params["end"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = int.Parse(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["new_user"]))
                {
                    query.new_user = int.Parse(Request.Params["new_user"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["new_user_date"]))
                {
                    query.new_user_date = DateTime.Parse(Request.Params["new_user_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["count_by"]))
                {
                    query.count_by = int.Parse(Request.Params["count_by"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["count"]))
                {
                    query.count = int.Parse(Request.Params["count"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["device"]))
                {
                    query.device = Request.Params["device"];
                }
                if (!string.IsNullOrEmpty(Request.Params["link_url"]))
                {
                    query.link_url = Request.Params["link_url"];
                }
                if (!string.IsNullOrEmpty(Request.Params["promo_image"]))
                {
                    query.promo_image = Request.Params["promo_image"];
                }
                if (!string.IsNullOrEmpty(Request.Params["active_now"]))
                {
                    query.active_now = int.Parse(Request.Params["active_now"]);
                }


                #endregion


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
                string localPromoPath = imgLocalPath + NewPromoPath;//圖片存儲地址
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
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localPromoPath.Substring(0, localPromoPath.Length - NewPromoPath.Length + 1), NewPromoPath.Substring(1, NewPromoPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        fileName = NewFileName + fileExtention;
                        NewFileName = localPromoPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + NewPromoPath);
                        string ErrorMsg = string.Empty;
                        //上傳之前刪除已有的圖片
                        if (query.row_id != 0)
                        {
                            oldModel = _INewPromoQuestionMgr.GetPromoQuestionList(new NewPromoQuestionQuery { row_id = query.row_id }).FirstOrDefault();
                            if (oldModel.promo_image != "")
                            {
                                string oldFileName = oldModel.promo_image;
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
                                query.promo_image = fileName;
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

                    query.created = DateTime.Now;
                    query.modified = query.created;
                    query.kuser = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                    query.muser = query.kuser;
                    if (_INewPromoQuestionMgr.InsertNewPromoQuestion(query) > 0)
                    {
                        json = "{success:true }";
                    }
                    else
                    {
                        json = "{success:false }";
                    }
                }
                else
                {
                    query.modified = DateTime.Now;
                    query.muser = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                    if (_INewPromoQuestionMgr.UpdateNewPromoQuestion(query) > 0)
                    {
                        json = "{success:true }";
                    }
                    else
                    {
                        json = "{success:false }";
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


        /// <summary>
        /// 刪除問卷送禮
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteQuestion()
        {
            NewPromoQuestionQuery query = new NewPromoQuestionQuery();
            string json = string.Empty;
            try
            {
                string Row_id = "";
                if (!string.IsNullOrEmpty(Request.Params["rowId"]))
                {
                    Row_id = Request.Params["rowId"];
                    Row_id = Row_id.TrimEnd(',');
                }

                _INewPromoQuestionMgr = new NewPromoQuestionMgr(mySqlConnectionString);

                int result = _INewPromoQuestionMgr.DeleteQuestion(Row_id);
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

        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActiveQuestion()
        {
            try
            {
                int row_id = 0;
                if (!string.IsNullOrEmpty(Request.Params["row_id"].ToString()))
                {
                    row_id = Convert.ToInt32(Request.Params["row_id"].ToString());
                }
                int activeValue = Convert.ToInt32(Request.Params["active"] ?? "0");
                _INewPromoQuestionMgr = new NewPromoQuestionMgr(mySqlConnectionString);
                NewPromoQuestionQuery model = new NewPromoQuestionQuery();
                model.row_id = row_id;
                model.active = activeValue;
                model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                model.modified = DateTime.Now;
                if (_INewPromoQuestionMgr.UpdateActive(model) > 0)
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


        #region 通關密語
        #region 列表頁+HttpResponseBase GetNewPromoCarnetList
        public HttpResponseBase GetNewPromoCarnetList()
        {
            string json = string.Empty;
            List<NewPromoCarnetQuery> store = new List<NewPromoCarnetQuery>();
            NewPromoCarnetQuery query = new NewPromoCarnetQuery();
            NewPromoPresentQuery queryPresent = new NewPromoPresentQuery();
            queryPresent.IsPage = false;
            int totalCount = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

                if (!string.IsNullOrEmpty(Request.Params["condition"]))
                {
                    query.condition = Convert.ToInt32(Request.Params["condition"]);
                }
                _INewPromoCarnetMgr = new NewPromoCarnetMgr(mySqlConnectionString);
                DataTable _dt = _INewPromoCarnetMgr.NewPromoCarnetList(query, out  totalCount);
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (_dt.Rows[i]["s_promo_image"].ToString() != "")
                    {
                        _dt.Rows[i]["s_promo_image"] = imgServerPath + NewPromoPath + _dt.Rows[i]["promo_image"];
                    }
                    else
                    {
                        _dt.Rows[i]["s_promo_image"] = defaultImg;
                    }
                    if (_dt.Rows[i]["group_name"].ToString() == "")
                    {
                        _dt.Rows[i]["group_name"] = "不分";
                    }

                }
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 新增/編輯
        public HttpResponseBase InsertNewPromoCarnet()
        {
            NewPromoCarnetQuery query = new NewPromoCarnetQuery();
            NewPromoCarnetQuery oldModel = new NewPromoCarnetQuery();
            _INewPromoCarnetMgr = new NewPromoCarnetMgr(mySqlConnectionString);
            string json = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["row_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["present_event_id"]))
                {
                    query.present_event_id = Request.Params["present_event_id"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["new_user"]))
                {
                    query.new_user = Convert.ToInt32(Request.Params["new_user"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["new_user_date"]))
                {
                    query.new_user_date = Convert.ToDateTime(Request.Params["new_user_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["count_by"]))
                {
                    query.count_by = Convert.ToInt32(Request.Params["count_by"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["count"]))
                {
                    query.count = Convert.ToInt32(Request.Params["count"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["device"]))
                {
                    query.device = (Request.Params["device"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["message_mode"]))
                {
                    query.message_mode = Convert.ToInt32(Request.Params["message_mode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["message_content"]))
                {
                    query.message_content = (Request.Params["message_content"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["link_url"]))
                {
                    query.link_url = (Request.Params["link_url"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["promo_image"]))
                {
                    query.promo_image = Request.Params["promo_image"];
                }
                if (!string.IsNullOrEmpty(Request.Params["active_now"]))
                {
                    query.active_now = Convert.ToInt32(Request.Params["active_now"]);
                }

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
                string localPromoPath = imgLocalPath + NewPromoPath;//圖片存儲地址
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
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localPromoPath.Substring(0, localPromoPath.Length - NewPromoPath.Length + 1), NewPromoPath.Substring(1, NewPromoPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        fileName = NewFileName + fileExtention;
                        NewFileName = localPromoPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + NewPromoPath);

                        //上傳之前刪除已有的圖片
                        if (query.row_id != 0)
                        {
                            oldModel = _INewPromoCarnetMgr.GetModel(query);
                            if (oldModel.promo_image != "")
                            {
                                string oldFileName = oldModel.promo_image;
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
                                query.promo_image = fileName;
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
                else
                {
                    query.promo_image = oldModel.promo_image;
                }
                #endregion

                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_desc"]))
                {
                    query.event_desc = Request.Params["event_desc"];
                }
                if (!string.IsNullOrEmpty(Request.Params["start"]))
                {
                    query.start = Convert.ToDateTime(Request.Params["start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end"]))
                {
                    query.end = Convert.ToDateTime(Request.Params["end"]);
                }

                if (query.row_id == 0)
                {
                    query.kuser = (Session["caller"] as Caller).user_id;
                    query.muser = query.kuser;
                    query.created = DateTime.Now;
                    query.modified = query.created;
                    query.row_id = _INewPromoCarnetMgr.GetNewPromoCarnetMaxId();

                    query.event_id = BLL.gigade.Common.CommonFunction.GetEventId("F2", query.row_id.ToString());
                    if (_INewPromoCarnetMgr.InsertNewPromoCarnet(query) > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                else
                {
                    query.muser = (Session["caller"] as Caller).user_id;
                    query.modified = DateTime.Now;
                    if (_INewPromoCarnetMgr.UpdateNewPromoCarnet(query))
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

        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActiveCarnet()
        {
            try
            {
                NewPromoCarnetQuery query = new NewPromoCarnetQuery();
                query.row_id = Convert.ToInt32(Request.Params["row_id"].ToString());
                query.active = Convert.ToInt32(Request.Params["active"] ?? "0");
                _INewPromoCarnetMgr = new NewPromoCarnetMgr(mySqlConnectionString);
                query.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.modified = DateTime.Now;
                if (_INewPromoCarnetMgr.UpdateActive(query))
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


        public HttpResponseBase DeleteCarnet()
        {
            string jsonStr = String.Empty;
            _INewPromoCarnetMgr = new NewPromoCarnetMgr(mySqlConnectionString);

            if (!String.IsNullOrEmpty(Request.Params["rowId"]))
            {
                try
                {
                    if (_INewPromoCarnetMgr.DeleteNewPromoCarnet(Request.Params["rowId"].ToString()) > 0)
                    {
                        jsonStr = "{success:true}";
                    }
                    else
                    {
                        jsonStr = "{success:false}";
                    }
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    jsonStr = "{success:false}";
                }
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 根據event_id獲取到該event_id下面的信息
        public void ExportNewPromoRecordList()
        {
            string json = string.Empty;

            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            DataTable dtHZ = new DataTable();

            string newExcelName = string.Empty;
            dtHZ.Columns.Add("會員編號", typeof(String));
            dtHZ.Columns.Add("姓名", typeof(String));
           // dtHZ.Columns.Add("信箱", typeof(String));
            dtHZ.Columns.Add("註冊日期", typeof(String));
            // dtHZ.Columns.Add("電話", typeof(String));
            // dtHZ.Columns.Add("地址", typeof(String));
            dtHZ.Columns.Add("留言內容", typeof(String));
            try
            {
                int totalCount = 0;
                NewPromoRecordQuery newQuery = new NewPromoRecordQuery();
                newQuery.event_id = Request.Params["event_id"];
                List<NewPromoRecordQuery> store = new List<NewPromoRecordQuery>();
                _INewPromoRecordMgr = new NewPromoRecordMgr(mySqlConnectionString);
                store = _INewPromoRecordMgr.NewPromoRecordList(newQuery, out  totalCount);
                if (store.Count > 0)
                {
                    foreach (var item in store)
                    {
                        DataRow dr = dtHZ.NewRow();
                        dr[0] = item.user_id;
                        dr[1] = item.user_name;
                       // dr[2] = item.user_mail;

                        dr[2] = item.user_reg_date;
                        // dr[4] = item.user_tel;

                        // dr[5] = item.user_address;
                        dr[3] = item.message;
                        dtHZ.Rows.Add(dr);
                    }
                    if (System.IO.File.Exists(newExcelName))
                    {
                        System.IO.File.Delete(newExcelName);
                    }

                    string fileName = "匯出活動名單表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "匯出活動名單表_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Clear();
                    this.Response.Write("無數據存在<br/>");
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
        }
        #endregion

        #region 贈品部分
        #region 獲取到禮品頁面信息
        public HttpResponseBase GetNewPromoPresentList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            NewPromoPresentQuery query = new NewPromoPresentQuery();
            int totalCount = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["entId"]))
                {
                    query.event_id = Request.Params["entId"].ToString();
                }
                //query.event_id = Request.Params["entId"];
                _INewPromoPresentMgr = new NewPromoPresentMgr(mySqlConnectionString);
                _dt = _INewPromoPresentMgr.NewPromoPresentList(query, out  totalCount);
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 根據product_id獲取到product_name
        public HttpResponseBase GetProductnameById()
        {
            string jsonStr = String.Empty;
            string msg = string.Empty;
            int tranInt = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"].ToString()) && int.TryParse(Request.Params["id"], out tranInt))
                {
                    int id = Convert.ToInt32(Request.Params["id"]);
                    _INewPromoPresentMgr = new NewPromoPresentMgr(mySqlConnectionString);
                    msg = _INewPromoPresentMgr.GetProductnameById(id);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        jsonStr = "{success:true,msg:'" + msg + "'}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:0}";//返回json數據
                    }
                }
                else
                {
                    jsonStr = "{success:false,msg:0}";//返回json數據
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 添加/編輯 贈品獎勵
        public HttpResponseBase InestNewPromoPresent()
        {
            string jsonStr = String.Empty;
            try
            {
                if (string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    NewPromoPresentQuery newPresentQuery = new NewPromoPresentQuery();
                    newPresentQuery.created = DateTime.Now;
                    newPresentQuery.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    newPresentQuery.modified = newPresentQuery.created;
                    newPresentQuery.muser = newPresentQuery.kuser;
                    _INewPromoPresentMgr = new NewPromoPresentMgr(mySqlConnectionString);
                    newPresentQuery.start = Convert.ToDateTime(Request.Params["valid_start"]);
                    newPresentQuery.end = Convert.ToDateTime(Request.Params["valid_end"]);
                    newPresentQuery.bonus_expire_day = Convert.ToInt32(Request.Params["bonus_expire_day"]);
                    if (!string.IsNullOrEmpty(Request.Params["use_span_day"]))
                    {
                        newPresentQuery.use_span_day = Convert.ToInt32(Request.Params["use_span_day"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                    {
                        newPresentQuery.group_id = int.Parse(Request.Params["group_id"]);
                    }
                    //newPresentQuery.event_id = Request.Params["this_event_id"];
                    if (Convert.ToBoolean(Request.Params["state1"]) == true)
                    {
                        newPresentQuery.bonus_expire_day = 0;
                        newPresentQuery.gift_type = 1;
                        newPresentQuery.gift_id = Convert.ToInt32(Request.Params["gift_id"]);
                        newPresentQuery.freight_price = Convert.ToInt32(Request.Params["freight_price"]);
                        newPresentQuery.gift_amount = Convert.ToInt32(Request.Params["gift_amount"]);
                    }
                    else if (Convert.ToBoolean(Request.Params["state2"]) == true)
                    {
                        newPresentQuery.welfare_mulriple = Convert.ToDouble(Request.Params["welfare_mulriple"]);
                        newPresentQuery.gift_type = 2;
                        newPresentQuery.deduct_welfare = Convert.ToInt32(Request.Params["deduct_welfare"]);
                        newPresentQuery.ticket_name = Request.Params["ticket_name"].ToString();
                    }
                    else if (Convert.ToBoolean(Request.Params["state3"]) == true)
                    {
                        newPresentQuery.gift_type = 3;
                        newPresentQuery.deduct_welfare = Convert.ToInt32(Request.Params["deduct_welfare"]);
                        newPresentQuery.ticket_name = Request.Params["ticket_name"].ToString();
                    }
                    newPresentQuery.row_id = _INewPromoPresentMgr.GetNewPromoPresentMaxId();

                    newPresentQuery.event_id = BLL.gigade.Common.CommonFunction.GetEventId("PB", newPresentQuery.row_id.ToString());

                    if (_INewPromoPresentMgr.InsertNewPromoPresent(newPresentQuery) > 0)
                    {
                        jsonStr = "{success:true,event_id:\"" + newPresentQuery.event_id + "\"}";//返回json數據
                    }
                    else
                    {
                        jsonStr = "{success:false}";//返回json數據
                    }
                }
                else
                {
                    NewPromoPresentQuery newPresentQuery = new NewPromoPresentQuery();
                    _INewPromoPresentMgr = new NewPromoPresentMgr(mySqlConnectionString);
                    newPresentQuery.row_id = Convert.ToInt32(Request.Params["row_id"]);
                    newPresentQuery.start = Convert.ToDateTime(CommonFunction.DateTimeToString(Convert.ToDateTime(Request.Params["valid_start"])));
                    newPresentQuery.end = Convert.ToDateTime(CommonFunction.DateTimeToString(Convert.ToDateTime(Request.Params["valid_end"])));
                    newPresentQuery.bonus_expire_day = Convert.ToInt32(Request.Params["bonus_expire_day"]);
                    newPresentQuery.event_id = Request.Params["this_event_id"];
                    newPresentQuery.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    newPresentQuery.modified = DateTime.Now;
                    if (!string.IsNullOrEmpty(Request.Params["use_span_day"]))
                    {
                        newPresentQuery.use_span_day = Convert.ToInt32(Request.Params["use_span_day"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                    {
                        newPresentQuery.group_id = int.Parse(Request.Params["group_id"]);
                    }
                    if (Convert.ToBoolean(Request.Params["state1"]) == true)
                    {
                        newPresentQuery.gift_type = 1;
                        newPresentQuery.gift_id = Convert.ToInt32(Request.Params["gift_id"]);
                        newPresentQuery.freight_price = Convert.ToInt32(Request.Params["freight_price"]);
                        newPresentQuery.gift_amount = Convert.ToInt32(Request.Params["gift_amount"]);
                    }
                    else if (Convert.ToBoolean(Request.Params["state2"]) == true)
                    {
                        newPresentQuery.welfare_mulriple = Convert.ToDouble(Request.Params["welfare_mulriple"]);
                        newPresentQuery.gift_type = 2;
                        newPresentQuery.deduct_welfare = Convert.ToInt32(Request.Params["deduct_welfare"]);
                        newPresentQuery.ticket_name = Request.Params["ticket_name"].ToString();
                    }
                    else if (Convert.ToBoolean(Request.Params["state3"]) == true)
                    {
                        newPresentQuery.gift_type = 3;
                        newPresentQuery.deduct_welfare = Convert.ToInt32(Request.Params["deduct_welfare"]);
                        newPresentQuery.ticket_name = Request.Params["ticket_name"].ToString();
                    }
                    if (_INewPromoPresentMgr.UpdateNewPromoPresent(newPresentQuery) > 0)
                    {
                        jsonStr = "{success:true,event_id:0}";
                    }
                    else
                    {
                        jsonStr = "{success:false}";//返回json數據
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;

        }
        #endregion

        #region 獲取有效贈品數量
        public HttpResponseBase GetNewPromoPresent()
        {
            string jsonStr = String.Empty;
            _INewPromoPresentMgr = new NewPromoPresentMgr(mySqlConnectionString);
            if (!String.IsNullOrEmpty(Request.Params["present_event_id"]))
            {
                try
                {
                    int giftNum = _INewPromoPresentMgr.GetNewPromoPresent(Request.Params["present_event_id"].ToString());
                    jsonStr = "{success:true,count:" + giftNum + "}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    jsonStr = "{success:false,count:" + 0 + "}";
                }
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 更改贈品表狀態
        public JsonResult UpdateNewPromoPresentActive()
        {
            try
            {
                int currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int muser =0;
                int activeValue = Convert.ToInt32(Request.Params["active"]);
                if (!string.IsNullOrEmpty(Request.Params["muser"]))
                {
                    muser = int.Parse(Request.Params["muser"].ToString());
                }
                if (currentUser == muser && activeValue == 1)
                {
                    return Json(new { success = "stop" });
                }
                _INewPromoPresentMgr = new NewPromoPresentMgr(mySqlConnectionString);
                NewPromoPresentQuery modelPresent = new NewPromoPresentQuery();
                modelPresent.status = Convert.ToInt32(Request.Params["active"]);
                modelPresent.row_id = Convert.ToInt32(Request.Params["id"]);
                modelPresent.muser = currentUser;
                modelPresent.modified = DateTime.Now;
                if (_INewPromoPresentMgr.UpdateActive(modelPresent) > 0)
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

        #region 刪除獎品數據
        public HttpResponseBase DeleteNewPromoPresent()
        {
            string jsonStr = String.Empty;
            _INewPromoPresentMgr = new NewPromoPresentMgr(mySqlConnectionString);
            NewPromoPresentQuery query = new NewPromoPresentQuery();
            if (!String.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowID"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            query.row_id = Convert.ToInt32(rid);
                            _INewPromoPresentMgr.DeleteNewPromoPresent(query);
                        }
                    }
                    jsonStr = "{success:true}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    jsonStr = "{success:false}";
                }
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #endregion
    }
}
