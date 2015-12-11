using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

namespace Admin.gigade.Controllers
{
    public class VoteController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private VoteArticleMgr _votearticle;
        private VoteEventMgr veMgr;
        private IManageUserImplMgr _muMgr;
        private VoteMessageMgr _voteMsg;
        private VoteDetailMgr voteDetailMgr;
        private IProductImplMgr _productMgr;
        private IUsersImplMgr _uMgr;

        //上傳圖片
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.10:2121"
        string PaperPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.paperPath);//圖片保存路徑
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        //
        // GET: /Vote/
        #region view
        public ActionResult VoteEvent()
        {
            return View();
        }
        public ActionResult VoteArticle()
        {
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            return View();
        }
        public ActionResult VoteDetail()
        {
            return View();
        }
        /// <summary>
        /// 留言表
        /// </summary>
        /// <returns></returns>
        public ActionResult VoteMessage()
        {
            return View();
        }
        #endregion

        #region 文章管理
        #region 文章管理列表頁
        [CustomHandleError]
        public HttpResponseBase GetVoteArticleList()
        {
            List<VoteArticleQuery> store = new List<VoteArticleQuery>();
            string json = string.Empty;
            int totalCount = 0;
            try
            {
                _muMgr = new ManageUserMgr(mySqlConnectionString);
                ManageUserQuery muq = new ManageUserQuery();
                muq.IsPage = false;
                List<ManageUserQuery> mustore = _muMgr.GetNameMail(muq, out totalCount);

                VoteArticleQuery query = new VoteArticleQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.event_id = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["selcontent"]))
                {
                    query.article_title = Request.Params["selcontent"];
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.date = Convert.ToInt32(Request.Params["date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["time_start"]))
                {
                    query.time_start = Convert.ToDateTime(Request.Params["time_start"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                {
                    query.time_end = Convert.ToDateTime(Request.Params["time_end"]).ToString("yyyy-MM-dd HH:mm:ss");
                }
                _votearticle = new VoteArticleMgr(mySqlConnectionString);

                store = _votearticle.GetAll(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                foreach (var item in store)
                {
                    item.kendo_editor = Server.HtmlDecode(Server.HtmlDecode(item.article_content));
                    if (item.article_banner != "")
                    {
                        item.article_banner = imgServerPath + PaperPath + item.article_banner;
                    }
                    int indexc = mustore.FindIndex((ManageUserQuery e) => e.user_id == uint.Parse(item.create_user.ToString()));
                    if (indexc != -1)
                    {
                        item.creat_name = mustore[indexc].user_name;
                    }
                    else
                    {
                        item.creat_name = string.Empty;
                    }
                    int indexu = mustore.FindIndex((ManageUserQuery e) => e.user_id == uint.Parse(item.update_user.ToString()));
                    if (indexu != -1)
                    {
                        item.upd_name = mustore[indexu].user_name;
                    }
                    else
                    {
                        item.upd_name = string.Empty;
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
        #region 文章管理新增/編輯
        [CustomHandleError]
        public HttpResponseBase SaveVoteArticle()
        {
            string json = string.Empty;
            #region 圖片用
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
            string localBannerPath = imgLocalPath + PaperPath;//圖片存儲地址

            FileManagement fileLoad = new FileManagement();
            #endregion
            try
            {
                _votearticle = new VoteArticleMgr(mySqlConnectionString);
                List<VoteArticleQuery> store = new List<VoteArticleQuery>();
                VoteArticleQuery query = new VoteArticleQuery();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {//如果是編輯獲取該id數據
                    int totalCount = 0;
                    query.IsPage = false;
                    query.article_id = int.Parse(Request.Params["id"]);
                    store = _votearticle.GetAll(query, out totalCount);
                }
                //product_id
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["product_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Convert.ToInt32(Request.Params["event_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["article_title"]))
                {
                    query.article_title = Request.Params["article_title"];
                }
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    query.user_id = int.Parse(Request.Params["user_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["article_content"]))
                {
                    query.article_content = Request.Params["article_content"];
                }
                if (!string.IsNullOrEmpty(Request.Params["article_sort"]))
                {
                    query.article_sort =Convert.ToInt32(Request.Params["article_sort"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["prod_link"]))
                {
                    query.prod_link = Request.Params["prod_link"];
                }
                if (!string.IsNullOrEmpty(Request.Params["vote_count"]))
                {
                    query.vote_count = int.Parse(Request.Params["vote_count"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["article_start_time"]))
                {
                    query.article_start_time =Convert.ToDateTime(Request.Params["article_start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["article_end_time"]))
                {
                    query.article_end_time = Convert.ToDateTime(Request.Params["article_end_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["article_show_start_time"]))
                {
                    query.article_show_start_time = Convert.ToDateTime(Request.Params["article_show_start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["article_show_end_time"]))
                {
                    query.article_show_end_time = Convert.ToDateTime(Request.Params["article_show_end_time"]);
                }
                #region 上傳圖片
                string oldImg = string.Empty;
                foreach (var item in store)
                {
                    oldImg = item.article_banner;
                }
                if (!string.IsNullOrEmpty(Request.Params["id"]) && Request.Params["article_banner"] == oldImg)
                {
                    query.article_banner = oldImg;
                }
                else
                {
                    string ServerPath = string.Empty;
                    try
                    {
                        ServerPath = Server.MapPath(imgLocalServerPath + PaperPath);
                        if (Request.Files["article_banner"] != null && Request.Files["article_banner"].ContentLength > 0)
                        {
                            HttpPostedFileBase file = Request.Files["article_banner"];
                            string fileName = string.Empty;//當前文件名
                            string fileExtention = string.Empty;//當前文件的擴展名
                            //獲取圖片名稱
                            fileName = Path.GetFileName(file.FileName);
                            //獲得後綴名
                            fileExtention = Path.GetExtension(file.FileName);
                            //獲得不帶後綴名的文件名
                            fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string NewFileName = string.Empty;
                            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                            NewFileName = hash.Md5Encrypt(fileName, "32");
                            //判斷目錄是否存在，不存在則創建
                            FTP f_cf = new FTP();
                            f_cf.MakeMultiDirectory(localBannerPath.Substring(0, localBannerPath.Length - PaperPath.Length + 1), PaperPath.Substring(1, PaperPath.Length - 2).Split('/'), ftpuser, ftppwd);
                            fileName = NewFileName + fileExtention;
                            NewFileName = localBannerPath + NewFileName + fileExtention;//絕對路徑
                            string ErrorMsg = string.Empty;
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                query.article_banner = fileName;
                                //上傳新圖片成功后，再刪除舊的圖片
                                CommonFunction.DeletePicFile(ServerPath + oldImg);//刪除本地圖片
                                FTP ftp = new FTP(localBannerPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldImg))
                                {
                                    FTP ftps = new FTP(localBannerPath + oldImg, ftpuser, ftppwd);
                                    ftps.DeleteFile(localBannerPath + oldImg);//刪除ftp:71.159上的舊圖片
                                }
                            }

                        }
                        else
                        {
                            //上傳之前刪除已有的圖片
                            CommonFunction.DeletePicFile(ServerPath + oldImg);//刪除本地圖片
                            FTP ftp = new FTP(localBannerPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldImg))
                            {
                                FTP ftps = new FTP(localBannerPath + oldImg, ftpuser, ftppwd);
                                ftps.DeleteFile(localBannerPath + oldImg);//刪除ftp:71.159上的舊圖片
                            }
                            query.article_banner = "";
                        }
                    }
                    catch (Exception ex)
                    {
                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                        json = "{success:false,msg:'圖片上傳失敗!'}";
                    }
                }
                #endregion
                query.article_status = 0;//默認啟用
                query.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.update_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.create_time = DateTime.Now;
                query.update_time = DateTime.Now;

                if (query.article_id > 0)
                {//編輯
                    if (_votearticle.Update(query) > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false,msg:'修改失敗!'}";
                    }
                }
                else
                {//新增
                    if (_votearticle.Save(query) > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false,msg:'新增失敗!'}";
                    }
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
        #region 文章管理更改狀態
        [CustomHandleError]
        public HttpResponseBase UpdateStatsVoteArticle()
        {
            List<VoteArticleQuery> store = new List<VoteArticleQuery>();
            string json = string.Empty;
            try
            {
                _votearticle = new VoteArticleMgr(mySqlConnectionString);
                VoteArticleQuery query = new VoteArticleQuery();

                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.article_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.article_status = Convert.ToInt32(Request.Params["status"]);
                }
                query.update_user = (Session["caller"] as Caller).user_id;
                query.update_time = DateTime.Now;

                if (_votearticle.UpdateStatus(query) > 0)
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
        #region 獲取活動數據
        public HttpResponseBase GetEventId()
        {
            List<VoteEventQuery> store = new List<VoteEventQuery>();
            string json = string.Empty;
            try
            {
                VoteEventQuery query = new VoteEventQuery();
                veMgr = new VoteEventMgr(mySqlConnectionString);
                //query.event_status = 1;
                store = veMgr.GetVoteEventDownList(query);
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
        #region 文章管理下拉框數據
        //GetArticle

        #endregion

        #region 獲取商品名稱
        public HttpResponseBase GetProductName()
        {
            List<Product> store = new List<Product>();
            string json = "{success:true,msg:'0'}";
            try
            {
                Product query = new Product();
                _productMgr = new ProductMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.Product_Id = Convert.ToUInt32(Request.Params["id"]);
                }
                if (query.Product_Id > 0)
                {
                    store = _productMgr.GetProductName(query);
                    foreach (var item in store)
                    {
                        json = "{success:true,msg:'" + item.Product_Name + "'}";//返回json數據
                    }
                }
                else
                {
                    json = "{success:true,msg:'0'}";//返回json數據
                }
                //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 判斷商品是否組合商品
        public HttpResponseBase GetProductType()
        {
            int results = 0;
            string json = "{success:true,msg:'0'}";
            try
            {
                Product query = new Product();
                _productMgr = new ProductMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.Product_Id = Convert.ToUInt32(Request.Params["id"]);
                }
                if (query.Product_Id > 0)
                {
                    results = _productMgr.GetProductType(query);
                    json = "{success:true,msg:'" + results + "'}";//返回json數據
                }
                else
                {
                    json = "{success:true,msg:'0'}";//返回json數據
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

        #region 獲取用戶名稱
        public HttpResponseBase GetUserName()
        {
            List<Users> store = new List<Users>();
            string json = "{success:true,msg:'0'}";
            try
            {
                Users query = new Users();
                _uMgr = new UsersMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.user_id = Convert.ToUInt32(Request.Params["id"]);
                }
                if (query.user_id > 0)
                {
                    store = _uMgr.GetUser(query);
                    foreach (var item in store)
                    {
                        json = "{success:true,msg:'" + item.user_name + "'}";//返回json數據
                    }
                }
                else
                {
                    json = "{success:true,msg:'0'}";//返回json數據
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

        #region 判斷添加的文章名稱是否重複
        public HttpResponseBase SelectByArticleName()
        {
            string json = "";
            try
            {
                VoteArticleQuery query = new VoteArticleQuery();
                 if (!string.IsNullOrEmpty(Request.Params["id"]))
                 {
                    query.article_id = Convert.ToInt32(Request.Params["id"]);
                 }
                 if (!string.IsNullOrEmpty(Request.Params["article_title"]))
                 {
                     query.article_title = Request.Params["article_title"];
                 }
                 query.article_title = query.article_title.Trim();
                 _votearticle = new VoteArticleMgr(mySqlConnectionString);
                 if (_votearticle.SelectByArticleName(query) <= 0)
                 {
                     json = "{success:true,msg:'0'}";//返回json數據
                 }
                 else 
                 {
                     json = "{success:true,msg:'1'}";//返回json數據
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
        #region SelMaxSort
        public HttpResponseBase SelMaxSort()
        {
            string json = string.Empty;
            VoteArticleQuery query = new VoteArticleQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Convert.ToInt32(Request.Params["event_id"]);
                }
                _votearticle = new VoteArticleMgr(mySqlConnectionString);
                 int sort=_votearticle.SelMaxSort(query);
                 json = "{success:'true',sort:'" + sort + "'}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:'false',sort:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #endregion

        #region 活動管理
        /// <summary>
        /// 活動列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVoteEventList()
        {
            List<VoteEventQuery> store = new List<VoteEventQuery>();
            string json = string.Empty;
            try
            {
                VoteEventQuery query = new VoteEventQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                veMgr = new VoteEventMgr(mySqlConnectionString);
                _muMgr = new ManageUserMgr(mySqlConnectionString);
                ManageUserQuery muq = new ManageUserQuery();
                muq.IsPage = false;
                List<ManageUserQuery> mustore = _muMgr.GetNameMail(muq, out totalCount);
                if (!string.IsNullOrEmpty(Request.Params["search_content"]))
                {
                    int value = 0;
                    if (int.TryParse(Request.Params["search_content"], out value))
                    {
                        query.event_id = value;
                    }
                    else
                    {
                        query.event_name = Request.Params["search_content"];
                    }
                }
                store = veMgr.GetVoteEventList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                foreach (var item in store)
                {
                    if (item.event_banner != "")
                    {
                        item.event_banner = imgServerPath + PaperPath + item.event_banner;
                    }
                    int indexc = mustore.FindIndex((ManageUserQuery e) => e.user_id == uint.Parse(item.create_user.ToString()));
                    if (indexc != -1)
                    {
                        item.cuser = mustore[indexc].user_name;
                    }
                    else
                    {
                        item.cuser = string.Empty;
                    }
                    int indexu = mustore.FindIndex((ManageUserQuery e) => e.user_id == uint.Parse(item.update_user.ToString()));
                    if (indexu != -1)
                    {
                        item.uuser = mustore[indexu].user_name;
                    }
                    else
                    {
                        item.uuser = string.Empty;
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
        /// <summary>
        /// 新增或者修改
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveEvent()
        {
            string json = string.Empty;
            VoteEvent vevent = new VoteEvent();
            veMgr = new VoteEventMgr(mySqlConnectionString);
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
            string localBannerPath = imgLocalPath + PaperPath;//圖片存儲地址

            FileManagement fileLoad = new FileManagement();
            try
            {

                List<VoteEventQuery> store = new List<VoteEventQuery>();
                VoteEventQuery veq = new VoteEventQuery();
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    int totalCount = 0;
                    veq.IsPage = false;
                    veq.event_id = int.Parse(Request.Params["event_id"]);
                    store = veMgr.GetVoteEventList(veq, out totalCount);
                }
                vevent.event_name = Request.Params["event_name"];
                vevent.event_desc = Request.Params["event_desc"];
                if (!string.IsNullOrEmpty(Request.Params["event_start"]))
                {
                    vevent.event_start = DateTime.Parse(Request.Params["event_start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_end"]))
                {
                    vevent.event_end = DateTime.Parse(Request.Params["event_end"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["word_length"]))
                {
                    vevent.word_length = int.Parse(Request.Params["word_length"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["vote_everyone_limit"]))
                {
                    vevent.vote_everyone_limit = int.Parse(Request.Params["vote_everyone_limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["vote_everyday_limit"]))
                {
                    vevent.vote_everyday_limit = int.Parse(Request.Params["vote_everyday_limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["number_limit"]))
                {
                    vevent.number_limit = int.Parse(Request.Params["number_limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["is_repeat"]))
                {
                    vevent.is_repeat = int.Parse(Request.Params["is_repeat"]);
                }
                vevent.present_event_id = Request.Params["present_event_id"];
                vevent.event_status = 0;
                vevent.update_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                vevent.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                #region 上傳圖片
                string oldImg = string.Empty;
                foreach (var item in store)
                {
                    oldImg = item.event_banner;
                }
                if (!string.IsNullOrEmpty(Request.Params["event_id"]) && Request.Params["event_banner"] == oldImg)
                {
                    vevent.event_banner = oldImg;
                }
                else
                {
                    string ServerPath = string.Empty;
                    try
                    {
                        ServerPath = Server.MapPath(imgLocalServerPath + PaperPath);
                        if (Request.Files["event_banner"] != null && Request.Files["event_banner"].ContentLength > 0)
                        {
                            HttpPostedFileBase file = Request.Files["event_banner"];
                            string fileName = string.Empty;//當前文件名
                            string fileExtention = string.Empty;//當前文件的擴展名
                            //獲取圖片名稱
                            fileName = Path.GetFileName(file.FileName);
                            //獲得後綴名
                            fileExtention = Path.GetExtension(file.FileName);
                            //獲得不帶後綴名的文件名
                            fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string NewFileName = string.Empty;
                            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                            NewFileName = hash.Md5Encrypt(fileName, "32");
                            //判斷目錄是否存在，不存在則創建
                            FTP f_cf = new FTP();
                            f_cf.MakeMultiDirectory(localBannerPath.Substring(0, localBannerPath.Length - PaperPath.Length + 1), PaperPath.Substring(1, PaperPath.Length - 2).Split('/'), ftpuser, ftppwd);
                            fileName = NewFileName + fileExtention;
                            NewFileName = localBannerPath + NewFileName + fileExtention;//絕對路徑
                            string ErrorMsg = string.Empty;
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                vevent.event_banner = fileName;
                                //上傳新圖片成功后，再刪除舊的圖片
                                CommonFunction.DeletePicFile(ServerPath + oldImg);//刪除本地圖片
                                FTP ftp = new FTP(localBannerPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldImg))
                                {
                                    FTP ftps = new FTP(localBannerPath + oldImg, ftpuser, ftppwd);
                                    ftps.DeleteFile(localBannerPath + oldImg);//刪除ftp:71.159上的舊圖片
                                }
                            }

                        }
                        else
                        {
                            //上傳之前刪除已有的圖片
                            CommonFunction.DeletePicFile(ServerPath + oldImg);//刪除本地圖片
                            FTP ftp = new FTP(localBannerPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldImg))
                            {
                                FTP ftps = new FTP(localBannerPath + oldImg, ftpuser, ftppwd);
                                ftps.DeleteFile(localBannerPath + oldImg);//刪除ftp:71.159上的舊圖片
                            }
                            vevent.event_banner = "";
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
                }
                #endregion
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    vevent.event_id = int.Parse(Request.Params["event_id"]);
                    //rgModel.group_ipfrom = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
                    if (veMgr.Update(vevent) > 0)
                    {
                        json = "{success:true,msg:'修改成功!'}";
                    }
                    else
                    {
                        json = "{success:true,msg:'修改失敗!'}";
                    }
                }
                else
                {
                    vevent.create_time = DateTime.Now;
                    vevent.update_time = vevent.create_time;
                    if (veMgr.Save(vevent) > 0)
                    {
                        json = "{success:true,msg:'保存成功!'}";
                    }
                    else
                    {
                        json = "{success:true,msg:'修改失敗!'}";
                    }
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
        /// <summary>
        /// 更改狀態 啟用或者禁用
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateEventState()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            int activeValue = Convert.ToInt32(Request.Params["active"]);
            veMgr = new VoteEventMgr(mySqlConnectionString);
            VoteEvent vevent = new VoteEvent();
            vevent.event_id = id;
            vevent.event_status = activeValue;
            vevent.update_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            //System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            //if (addlist.Length > 0)
            //{
            //    p.ipfrom = addlist[0].ToString();
            //}
            if (veMgr.UpdateState(vevent) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }
        /// <summary>
        /// 添加活動時，活動標題不能重複 
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SelectByEventName()
        {
            string json = "";
            try
            {
                VoteEventQuery query = new VoteEventQuery();
                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Convert.ToInt32(Request.Params["event_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                query.event_name = query.event_name.Trim();
                veMgr = new VoteEventMgr(mySqlConnectionString);
                if (veMgr.SelectByEventName(query) <= 0)
                {
                    json = "{success:true,msg:'0'}";//返回json數據
                }
                else
                {
                    json = "{success:true,msg:'1'}";//返回json數據
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

        #region 留言管理
        /// <summary>
        /// 留言管理列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVoteMessageList()
        {
            List<VoteMessageQuery> store = new List<VoteMessageQuery>();
            string json = string.Empty;
            int tranInt = 0;
            try
            {
                VoteMessageQuery query = new VoteMessageQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                _voteMsg = new VoteMessageMgr(mySqlConnectionString);
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["message"]))
                {
                    query.message_content = Request.Params["message"];
                    //if (int.TryParse(Request.Params["message"], out tranInt))
                    //{
                    //    query.message_id = int.Parse(Request.Params["message"].ToString());
                    //}
                    //else
                    //{
                    //    query.message_content = Request.Params["message"];
                    //}
                }
                if (!string.IsNullOrEmpty(Request.Params["article"]))
                {
                    query.article_id = int.Parse(Request.Params["article"].ToString());
                    //if (int.TryParse(Request.Params["article"], out tranInt))
                    //{
                    //    query.article_id = int.Parse(Request.Params["article"].ToString());
                    //}
                    //else
                    //{
                    //    query.article_title = Request.Params["article"];
                    //}
                }
                query.message_status = -1;
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.message_status = int.Parse(Request.Params["status"].ToString());
                }

                store = _voteMsg.GetVoteMessageList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
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
        /// <summary>
        /// 保存留言管理
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveVoteMessage()
        {
            string json = string.Empty;
            try
            {
                VoteMessageQuery query = new VoteMessageQuery();
                if (!string.IsNullOrEmpty(Request.Params["message_id"]))
                {
                    query.message_id = Convert.ToInt32(Request.Params["message_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["article_id"]))
                {
                    query.article_id = Convert.ToInt32(Request.Params["article_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["message_content"]))
                {
                    query.message_content = Request.Params["message_content"];
                }
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;

                query.ip = addlist[0].ToString();
                query.create_user = (Session["caller"] as Caller).user_id;
                query.update_user = (Session["caller"] as Caller).user_id;
                query.create_time = DateTime.Now;
                query.update_time = query.create_time;
                _voteMsg = new VoteMessageMgr(mySqlConnectionString);
                int result = 0;
                if (query.message_id != 0)//編輯
                {
                    result = _voteMsg.UpdateVoteMessage(query);
                }
                else //新增
                {
                    result = _voteMsg.AddVoteMessage(query);
                }

                if (result > 0)
                {
                    json = "{\"success\":\"true\",\"msg\":\"保存成功!\"}";
                }
                else
                {
                    json = "{\"success\":\"false\",\"msg\":\"保存失敗!\"}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{\"success\":\"false\",\"msg\":\"參數出錯!\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 刪除留言列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteVoteMessage()
        {
            VoteMessageQuery query = new VoteMessageQuery();
            string json = string.Empty;
            try
            {
                string Row_id = "";
                if (!string.IsNullOrEmpty(Request.Params["rowId"]))
                {
                    Row_id = Request.Params["rowId"];
                    query.message_id_in = Row_id.TrimEnd(',');
                }

                _voteMsg = new VoteMessageMgr(mySqlConnectionString);

                int result = _voteMsg.DelVoteMessage(query);
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

        public JsonResult UpdateVoteMessageStatus()
        {
            try
            {
                VoteMessageQuery query = new VoteMessageQuery();
                if (!string.IsNullOrEmpty(Request.Params["message_id"].ToString()))
                {
                    query.message_id = Convert.ToInt32(Request.Params["message_id"].ToString());
                }
                query.message_status = Convert.ToInt32(Request.Params["message_status"] ?? "0");
                _voteMsg = new VoteMessageMgr(mySqlConnectionString);

                query.update_user = (Session["caller"] as Caller).user_id;
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;

                query.ip = addlist[0].ToString();
                query.update_time = DateTime.Now;
                if (_voteMsg.UpVoteMessageStatus(query) > 0)
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
        #region 新增留言時的下拉框


        public HttpResponseBase GetArticle()
        {
            List<VoteArticleQuery> store = new List<VoteArticleQuery>();
            string json = string.Empty;
            try
            {
                _votearticle = new VoteArticleMgr(mySqlConnectionString);

                store = _votearticle.GetArticle();
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

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

        #region 投票管理
        /// <summary>
        /// 獲取投票管理列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase VoteDetailList()
        {
            string json = string.Empty;
            int totalCount = 0;
            List<VoteDetailQuery> list = new List<VoteDetailQuery>();
            voteDetailMgr = new VoteDetailMgr(mySqlConnectionString);
            VoteDetailQuery query = new VoteDetailQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");

            string article_id = Request.Params["article_id"];
            string searchContent = Request.Params["searchContent"];
            string start_time = Request.Params["time_start"];
            string end_time = Request.Params["time_end"];
            string vote_status = Request.Params["vote_status"];
            if (!string.IsNullOrEmpty(article_id) && Convert.ToInt32(article_id) != 0)
            {
                query.article_id = Convert.ToInt32(article_id);
            }
            if (!string.IsNullOrEmpty(searchContent))
            {
                query.searchContent = searchContent;
            }
            if (!string.IsNullOrEmpty(start_time))
            {
                query.start_time =Convert.ToDateTime(Convert.ToDateTime(start_time).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (!string.IsNullOrEmpty(end_time))
            {
                query.end_time = Convert.ToDateTime(Convert.ToDateTime(end_time).ToString("yyyy-MM-dd HH:mm:ss"));
            }
            query.vote_status = -1;
            if (!string.IsNullOrEmpty(vote_status))
            {
                query.vote_status = Convert.ToInt32(vote_status);
            }
            if (!string.IsNullOrEmpty(Request.Params["relation_id"]))//待回覆
            {
                query.vote_id = Convert.ToInt32(Request.Params["relation_id"]);
            }
            try
            {
                list = voteDetailMgr.GetList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                if (Convert.ToBoolean(Request.Params["isSecret"]))
                {
                    foreach (var item in list)
                    {
                        if (!string.IsNullOrEmpty(item.user_name))
                        {
                            item.user_name = item.user_name.Substring(0, 1) + "**";
                        }
                    }
                }
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        /// <summary>
        /// 保存投票管理
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveVoteDetail()
        {
            string json = string.Empty;
            string vote_id = Request.Params["vote_id"];
            string article_id = Request.Params["article_id"];
            string user_id = Request.Params["user_id"];
            VoteDetailQuery query = new VoteDetailQuery();
            if (!string.IsNullOrEmpty(vote_id))
            {
                query.vote_id = Convert.ToInt32(vote_id);
            }
            if (!string.IsNullOrEmpty(article_id))
            {
                query.article_id = Convert.ToInt32(article_id);
            }
            if (!string.IsNullOrEmpty(user_id))
            {
                query.user_id = Convert.ToInt32(user_id);
            }
            
           
            System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            query.ip = addlist[0].ToString();
            query.create_user = (Session["caller"] as Caller).user_id;
            query.update_user = (Session["caller"] as Caller).user_id;
            DateTime currentTime = DateTime.Now;
            query.create_time = currentTime;
            query.update_time = currentTime;
            voteDetailMgr = new VoteDetailMgr(mySqlConnectionString);
            int result = 0;
            try
            {
                if (query.vote_id != 0)//編輯
                {
                    result = voteDetailMgr.Update(query);
                }
                else //新增
                {
                    query.vote_status = 0;
                    result = voteDetailMgr.Add(query);
                }

                if (result > 0)
                {
                    json = "{\"success\":\"true\",\"msg\":\"保存成功!\"}";
                }
                else
                {
                    json = "{\"success\":\"false\",\"msg\":\"保存失敗!\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{\"success\":\"false\",\"msg\":\"參數出錯!\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public void VoteDetailExportExcel()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            VoteDetailQuery query = new VoteDetailQuery();
            try
            {            
                if (Request.Params["article_id"]!="null")
                {
                    query.article_id = Convert.ToInt32(Request.Params["article_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["searchContent"]))
                {
                    query.searchContent = Request.Params["searchContent"];
                }
                if (!string.IsNullOrEmpty(Request.Params["time_start"]))
                {
                    query.start_time = Convert.ToDateTime(Request.Params["time_start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                {
                    query.end_time = Convert.ToDateTime(Request.Params["time_end"]);
                }
                //query.vote_status = -1;
                if (Request.Params["vote_status"] != "null")
                {
                    query.vote_status = Convert.ToInt32(Request.Params["vote_status"]);
                }

                //活動編號，會員編號，email，姓名，投那一支商品，投票日
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("編號", typeof(String));
                dtHZ.Columns.Add("活動編號", typeof(String));
                dtHZ.Columns.Add("文章標題", typeof(String));
                dtHZ.Columns.Add("會員編號", typeof(String));
               // dtHZ.Columns.Add("Email", typeof(String));
                dtHZ.Columns.Add("姓名", typeof(String));
                dtHZ.Columns.Add("投票商品", typeof(String));
                dtHZ.Columns.Add("投票日", typeof(String));
                dtHZ.Columns.Add("是否啟用", typeof(String));

                voteDetailMgr = new VoteDetailMgr(mySqlConnectionString);
                List<VoteDetailQuery> list = new List<VoteDetailQuery>();
                //cuQuery.search_type = Convert.ToInt32(Request.Params["search_type"]);
                //cuQuery.searchcontent = Request.Params["searchcontent"];
                //cuQuery.date_type = Convert.ToInt32(Request.Params["date_type"]);
                //cuQuery.datestart = Convert.ToDateTime(Request.Params["dateStart"]);//建立時間
                //cuQuery.dateend = Convert.ToDateTime(Request.Params["dateEnd"]);
                //cuQuery.question_type = Convert.ToUInt32(Request.Params["qusetion_type"]);
                //if (Convert.ToBoolean(Request.Params["radio2"]) == true)//待回覆
                //{
                //    cuQuery.question_status = 3;
                //}
                //else if (Convert.ToBoolean(Request.Params["radio3"]) == true)//已回覆
                //{
                //    cuQuery.question_status = 4;
                //}
                //else if (Convert.ToBoolean(Request.Params["radio4"]) == true)
                //{
                //    cuQuery.question_status = 2;
                //}
                query.IsPage = false;
                int totalCount = 0;
                _dt = voteDetailMgr.GetDtVoteDetail(query, out totalCount);
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = _dt.Rows[i]["vote_id"];
                    dr[1] = "";
                    if (!string.IsNullOrEmpty(_dt.Rows[i]["event_id"].ToString()))
                    {
                        dr[1] = "【" + _dt.Rows[i]["event_id"].ToString() + "】" + _dt.Rows[i]["event_name"].ToString();
                    }
                    dr[2] = "";
                    if (!string.IsNullOrEmpty(_dt.Rows[i]["article_id"].ToString()))
                    {
                        dr[2] = "【" + _dt.Rows[i]["article_id"].ToString() + "】" + _dt.Rows[i]["article_title"].ToString();
                    }
                   
                    dr[3] = _dt.Rows[i]["user_id"];
                   // dr[4] = _dt.Rows[i]["user_email"];
                    dr[4] = _dt.Rows[i]["user_name"];
                    dr[5] = "";
                    if (!string.IsNullOrEmpty(_dt.Rows[i]["product_id"].ToString()))
                    {
                        dr[5] ="【"+_dt.Rows[i]["product_id"].ToString()+"】" + _dt.Rows[i]["product_name"].ToString();
                    }
                    dr[6] = _dt.Rows[i]["create_time"];
                    dr[7] = Convert.ToInt32(_dt.Rows[i]["vote_status"]) == 0 ? "否" : "是";
                    dtHZ.Rows.Add(dr);
                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = DateTime.Now.ToString("投票信息_yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "投票信息_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
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
        }

        public JsonResult UpdateVoteDetaiStatus()
        {
            try
            {
                VoteDetailQuery query = new VoteDetailQuery();
                if (!string.IsNullOrEmpty(Request.Params["vote_id"].ToString()))
                {
                    query.vote_id = Convert.ToInt32(Request.Params["vote_id"].ToString());
                }
                query.vote_status = Convert.ToInt32(Request.Params["vote_status"] ?? "0");
                voteDetailMgr = new VoteDetailMgr(mySqlConnectionString);

                query.update_user = (Session["caller"] as Caller).user_id;
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;

                query.ip = addlist[0].ToString();
                query.update_time = DateTime.Now;
                if (voteDetailMgr.UpdateVoteDetaiStatus(query) > 0)
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


      
    }
}