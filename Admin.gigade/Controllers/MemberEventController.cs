using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Converters;
using BLL.gigade.Common;

namespace Admin.gigade.Controllers
{
    public class MemberEventController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private MemberLevelMgr _memberLevel;
        private MemberEventMgr _memberEvent;
        //
        // GET: /MemberEvent/

        //上傳圖片
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp上傳圖片地址http://192.168.71.10:2121
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//顯示圖片路徑http://192.168.71.10:8765
        string MemberEventPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.MemberEventPath);//圖片保存路徑
        #region 视图
        /// <summary>
        /// 會員活動頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberEvent()
        {
            return View();
        }
        //會員等級
        public ActionResult MemberLevel()
        {
            return View();
        }
        #endregion

        #region 會員級別
        #region 會員級別列表頁
        public HttpResponseBase MemberLevelList()
        {
            MemberLevelQuery query = new MemberLevelQuery();
            List<MemberLevelQuery> store = new List<MemberLevelQuery>();
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["code_name"]))
                {
                    query.code_name = Request.Params["code_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["mem_status"]))
                {
                    query.ml_status = Convert.ToSByte(Request.Params["mem_status"]);
                }
                _memberLevel = new MemberLevelMgr(mySqlConnectionString);
                store = _memberLevel.MemberLevelList(query, out totalCount);
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 更改狀態
        public JsonResult UpdateActive()
        {
            try
            {
                MemberLevelQuery query = new MemberLevelQuery();
                if (!string.IsNullOrEmpty(Request.Params["rowID"].ToString()))
                {
                    query.rowID = Convert.ToInt32(Request.Params["rowID"].ToString());
                }
                query.ml_status = Convert.ToSByte(Request.Params["ml_status"] ?? "0");
                query.m_date = DateTime.Now;
                query.m_user = (Session["caller"] as Caller).user_id;
                _memberLevel = new MemberLevelMgr(mySqlConnectionString);
                if (_memberLevel.UpdateActive(query) > 0)
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
        #region 新增/編輯
        public HttpResponseBase SaveMemberLevel()
        {
            string json = string.Empty;
            try
            {
                MemberLevelQuery query = new MemberLevelQuery();
                _memberLevel = new MemberLevelMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["ml_name"]))
                {
                    query.ml_name = Request.Params["ml_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["ml_code"]))
                {
                    query.ml_code = Request.Params["ml_code"];
                }
                if (!string.IsNullOrEmpty(Request.Params["ml_minimal_amount"]))
                {
                    query.ml_minimal_amount = Convert.ToInt32(Request.Params["ml_minimal_amount"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ml_max_amount"]))
                {
                    query.ml_max_amount = Convert.ToInt32(Request.Params["ml_max_amount"]);
                }
                
                if (!string.IsNullOrEmpty(Request.Params["ml_month_seniority"]))
                {
                    query.ml_month_seniority = Convert.ToInt32(Request.Params["ml_month_seniority"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ml_last_purchase"]))
                {
                    query.ml_last_purchase = Convert.ToInt32(Request.Params["ml_last_purchase"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ml_minpurchase_times"]))
                {
                    query.ml_minpurchase_times = Convert.ToInt32(Request.Params["ml_minpurchase_times"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ml_birthday_voucher"]))
                {
                    query.ml_birthday_voucher = Convert.ToInt32(Request.Params["ml_birthday_voucher"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ml_shipping_voucher"]))
                {
                    query.ml_shipping_voucher = Convert.ToInt32(Request.Params["ml_shipping_voucher"]);
                }
                
                if (!string.IsNullOrEmpty(Request.Params["ml_seq"]))
                {
                    query.ml_seq = Convert.ToInt32(Request.Params["ml_seq"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))//編輯
                {
                    query.rowID = Convert.ToInt32(Request.Params["rowID"]);
                    query.m_user = (Session["caller"] as Caller).user_id;
                    query.m_date = DateTime.Now;
                    if (!string.IsNullOrEmpty(Request.Params["old_ml_seq"]))
                    {
                        query.old_ml_seq = Convert.ToInt32(Request.Params["old_ml_seq"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["old_ml_code"]))
                    {
                        query.old_ml_code = (Request.Params["old_ml_code"]);
                    }
                    if (query.ml_seq != query.old_ml_seq && _memberLevel.DistinctSeq(query))//新代碼與老代碼不一樣並且重複
                    {
                        json = "{success:'true',msg:'0'}";//排序重複
                    }
                    else if (query.ml_code != query.old_ml_code && _memberLevel.DistinctCode(query))
                    {
                        json = "{success:'true',msg:'2'}";//級別代碼重複
                    }
                    else
                    {
                        if (_memberLevel.SaveMemberLevel(query) > 0)
                        {
                            json = "{success:'true',mag:'1'}";//保存成功
                        }
                        else
                        {
                            json = "{success:'false'}";//保存失敗
                        }
                    }
                }
                else//新增
                {
                    query.k_user = (Session["caller"] as Caller).user_id;
                    query.k_date = DateTime.Now;
                    query.k_user = (Session["caller"] as Caller).user_id;
                    query.m_date = query.k_date;
                    if (_memberLevel.DistinctSeq(query))
                    {
                        json = "{success:'true',msg:'0'}";//排序重複
                    }
                    else if (_memberLevel.DistinctCode(query))
                    {
                        json = "{success:'true',msg:'2'}";//級別代碼重複
                    }
                    else
                    {
                        if (_memberLevel.SaveMemberLevel(query) > 0)
                        {
                            json = "{success:'true',mag:'1'}";//保存成功
                        }
                        else
                        {
                            json = "{success:'false'}";//保存失敗
                        }
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
        #region 級別代碼不允許重複
        public HttpResponseBase DistinctCode()
        {
            string json = string.Empty;
            try
            {
                MemberLevelQuery query = new MemberLevelQuery();
                if (!string.IsNullOrEmpty(Request.Params["ml_code"]))
                {
                    query.ml_code = Request.Params["ml_code"];
                    _memberLevel = new MemberLevelMgr(mySqlConnectionString);
                    if (_memberLevel.DistinctCode(query))
                    {
                        json = "{success:false}";//代碼重複
                    }
                    else
                    {
                        json = "{success:true}";//代碼可用
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
        #region 級別排序不允許重複
        public HttpResponseBase DistinctSeq()
        {
            string json = string.Empty;
            try
            {
                MemberLevelQuery query = new MemberLevelQuery();
                if (!string.IsNullOrEmpty(Request.Params["ml_seq"]))
                {
                    query.ml_seq = Convert.ToInt32(Request.Params["ml_seq"]);
                    _memberLevel = new MemberLevelMgr(mySqlConnectionString);
                    if (_memberLevel.DistinctSeq(query))
                    {
                        json = "{success:false}";//排序重複
                    }
                    else
                    {
                        json = "{success:true}";//排序可用
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
        #region MaxMLSeq
        public HttpResponseBase MaxMLSeq()
        {
            string json = string.Empty;
            try
            {
                _memberLevel = new MemberLevelMgr(mySqlConnectionString);
                int ml_seq = _memberLevel.MaxMLSeq();
                json = "{success:'true',ml_seq:'" + ml_seq + "'}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:'false',ml_seq:'" + 1 + "'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #endregion

        #region 會員活動
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetMemberEventList()
        {
            MemberEventQuery query = new MemberEventQuery();
            List<MemberEventQuery> stores = new List<MemberEventQuery>();
            string json = string.Empty;
            try
            {

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _memberEvent = new MemberEventMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["search_content"]))
                {
                    query.ml_name = Request.Params["search_content"];
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.timestart = Convert.ToDateTime(Request.Params["timestart"].ToString()).ToString("yyyy-MM-dd 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.timeend = Convert.ToDateTime(Request.Params["timeend"].ToString()).ToString("yyyy-MM-dd 23:59:59");
                }
                int totalCount = 0;
                stores = _memberEvent.Query(query, out totalCount);
                foreach (var item in stores)
                {
                    item.s_me_banner_link = imgServerPath + MemberEventPath + item.me_big_banner;
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        /// <summary>
        /// 會員活動新增編輯
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveMemberEvent()
        {
            string json = string.Empty;
            try
            {
                MemberEventQuery query = new MemberEventQuery();
                _memberEvent = new MemberEventMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                {
                    query.rowID = int.Parse(Request.Params["rowID"]);
                }
                query.me_name = Request.Params["me_name"];
                query.me_desc = Request.Params["me_desc"];
                query.event_id = Request.Params["event_id"];

                if (!string.IsNullOrEmpty(Request.Params["me_startdate"]))
                {
                    query.me_startdate = DateTime.Parse(Request.Params["me_startdate"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["me_enddate"]))
                {
                    query.me_enddate = DateTime.Parse(Request.Params["me_enddate"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["me_birthday"]))
                {
                    //query.me_birthday = int.Parse(Request.Params["me_birthday"]);
                    if (Request.Params["me_birthday"] == "1")
                    {
                        query.me_birthday = 1;
                    }
                    else
                    {
                        query.me_birthday = 0;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["me_bonus_onetime"]))
                {
                    if (Request.Params["me_bonus_onetime"] == "1")
                    {
                        query.me_bonus_onetime = 1;
                    }
                    else
                    {
                        query.me_bonus_onetime = 0;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["me_banner_link"]))
                {
                    query.me_banner_link = Request.Params["me_banner_link"];
                }
                if (!string.IsNullOrEmpty(Request.Params["code"]))
                {
                    query.ml_code = Request.Params["code"].TrimEnd(',');
                }
                else
                {
                    query.ml_code = "0";
                }
                if (!string.IsNullOrEmpty(Request.Params["et_id"]))
                {
                    query.et_id = int.Parse(Request.Params["et_id"]);
                }
                //query.et_name = Request.Params["et_name"];
                switch (Request.Params["et_name"])
                {
                    case "1":
                        query.et_name = "DD";
                        query.et_date_parameter = "";
                        break;
                    case "2":
                        query.et_name = "WW";
                        if (!string.IsNullOrEmpty(Request.Params["week"]))
                        {
                            query.et_date_parameter = Request.Params["week"].TrimEnd(',');
                        }
                        break;
                    case "3":
                        query.et_name = "MM";
                        if (!string.IsNullOrEmpty(Request.Params["month"]))
                        {
                            query.et_date_parameter = Request.Params["month"].TrimEnd(',');
                        }
                        break;
                }

                #region 處理每天的開始結束時間
                query.et_starttime = Request.Params["et_starttime"];
                query.et_endtime = Request.Params["et_endtime"];

                #endregion
                query.k_user = (Session["caller"] as Caller).user_id;
                query.m_user = (Session["caller"] as Caller).user_id;
                query.k_date = DateTime.Now;
                query.m_date = query.k_date;
                string event_json=_memberEvent.IsGetEventID(query.event_id);
                if (event_json.IndexOf("true") > 0)
                {
                    #region 判斷數據不能重複
                    if (_memberEvent.IsRepeat(query))//不重複
                    {

                        #region 上傳圖片


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
                        string localMemberEventPath = imgLocalPath + MemberEventPath;//圖片存儲地址
                        FileManagement fileLoad = new FileManagement();
                        int totalCount = 0;
                        MemberEventQuery oldQuery = _memberEvent.Query(new MemberEventQuery { rowID = query.rowID }, out totalCount).FirstOrDefault();
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
                                string[] dirPath = new string[2];
                                //dirPath[0] = NewFileName.Substring(0, 2)+"/";
                                //dirPath[1] = NewFileName.Substring(2, 2)+"/";
                                string ServerPath = string.Empty;
                                FTP f_cf = new FTP();
                                CreateFolder(localMemberEventPath, dirPath);
                                fileName = NewFileName + fileExtention;
                                NewFileName = localMemberEventPath + NewFileName + fileExtention;//絕對路徑
                                ServerPath = Server.MapPath(imgLocalServerPath + MemberEventPath);
                                string ErrorMsg = string.Empty;
                                //上傳之前刪除已有的圖片
                                if (query.rowID != 0)
                                {

                                    if (oldQuery.me_big_banner != "")
                                    {
                                        string oldFileName = oldQuery.me_big_banner;
                                        CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                        FTP ftp = new FTP(localMemberEventPath, ftpuser, ftppwd);
                                        List<string> tem = ftp.GetFileList();
                                        if (tem.Contains(oldFileName))
                                        {
                                            FTP ftps = new FTP(localMemberEventPath + oldFileName, ftpuser, ftppwd);
                                            ftps.DeleteFile(localMemberEventPath + oldFileName);
                                        }
                                    }
                                }
                                try
                                {
                                    Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                                    bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                    if (result)
                                    {
                                        query.me_big_banner = fileName;
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
                            else
                            {
                                query.me_big_banner = oldQuery.me_big_banner;
                            }
                        }
                        #endregion
                        if (_memberEvent.MemberEventSave(query) > 0)
                        {
                            //json = "{success:true}";
                            if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                            {
                                json = "{success:true,msg:'修改成功！'}";
                            }
                            else
                            {
                                json = "{success:true,msg:'新增成功！'}";
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                            {
                                json = "{success:false,msg:'修改失敗！'}";
                            }
                            else
                            {
                                json = "{success:false,msg:'新增失敗！'}";
                            }
                        }

                    }
                    else
                    {
                        json = "{success:false,msg:'數據重複！'}";
                    }
                    #endregion
                }
                else
                {
                    json = "{success:false,msg:'促銷編號錯誤或促銷活動未啟用'}";
                }
                




            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'異常！'}";
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
            int id = 0;
            int activeValue = 0;
            _memberEvent = new MemberEventMgr(mySqlConnectionString);
            if (!string.IsNullOrEmpty(Request.Params["id"]))
            {
                id = int.Parse(Request.Params["id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["active"]))
            {
                activeValue = int.Parse(Request.Params["active"]);
            }
            MemberEventQuery meq = new MemberEventQuery();
            meq.rowID = id;
            meq.me_status = activeValue;
            meq.m_date = DateTime.Now;
            meq.m_user = (Session["caller"] as Caller).user_id;
            if (_memberEvent.UpdateState(meq) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }
        /// <summary>
        /// 獲得會員等級數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetMemberLevelDownList()
        {
            string json = string.Empty;
            MemberLevel me = new MemberLevel();
            try
            {
                _memberEvent = new MemberEventMgr(mySqlConnectionString);
                me.ml_status = 1;
                DataTable dt = _memberEvent.GetMemberLevelList(me);
                json = "{success:true,data:" + JsonConvert.SerializeObject(dt, Formatting.Indented) + "}";//返回json數據
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
        public HttpResponseBase IsGetEventID()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    string event_id = Request.Params["event_id"];
                    _memberEvent = new MemberEventMgr(mySqlConnectionString);
                    json=   _memberEvent.IsGetEventID(event_id);
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
        public void CreateFolder(string path, string[] Mappath)
        {
            FTP ftp = null;
            try
            {
                string fullPath = path.Substring(0, path.Length - 1);
                string nodeDir = fullPath.Substring(fullPath.LastIndexOf("/") + 1);
                //創建跟目錄
                ftp = new FTP(fullPath.Substring(0, fullPath.LastIndexOf("/") + 1), ftpuser, ftppwd);
                if (!ftp.DirectoryExist(nodeDir))
                {
                    ftp = new FTP(fullPath, ftpuser, ftppwd);
                    ftp.MakeDirectory();
                }
                foreach (string s in Mappath)
                {
                    ftp = new FTP(fullPath.Substring(0, fullPath.Length), ftpuser, ftppwd);
                    fullPath += "/" + s;

                    if (!ftp.DirectoryExist(s.Replace("/", "")))
                    {
                        ftp = new FTP(fullPath.Substring(0, fullPath.Length), ftpuser, ftppwd);
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
    }
}
