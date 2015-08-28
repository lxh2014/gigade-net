using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using BLL.gigade.Common;
using System.Collections.Specialized;
using System.IO;
using BLL.gigade.Model.Query;
using System.Data;


namespace Admin.gigade.Controllers
{
    public class WebsiteController : Controller
    {
        //
        // GET: /Website/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//關於導入的xml文件的限制
        private ISerialImplMgr _ISerImplMgr;
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        //string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址/promotion/dev/
        string promoPath = "/ad/a/";//圖片地址
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址ftp://192.168.71.10:2121/img
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.10:8765"

        private IBannerSiteImplMgr _bsMgr;
        private IBannerContentImplMgr _bcMgr;
        private IBannerCategoryImplMgr _bcateMgr;
        private BannerNewsSiteMgr _bnsMgr;
        private BannerNewsContentMgr bncMgr;
        #region View視圖
        public ActionResult BannerImageSiteList()
        {
            return View();
        }
        public ActionResult BannerImageList()
        {
            ViewBag.sid = Request.Params["sid"];
            List<BannerSite> store = new List<BannerSite>();
            BannerSite bs = new BannerSite();
            _bsMgr = new BannerSiteMgr(mySqlConnectionString);
            if (!string.IsNullOrEmpty(Request.Params["sid"]))
            {
                bs.banner_site_id = uint.Parse(Request.Params["sid"]);
            }
            store = _bsMgr.GetBannerSiteName(bs);
            if (store.Count > 0)
            {
                ViewBag.sname = store[0].banner_site_name;
            }
            else
            {
                ViewBag.sname = "";
            }
            if (!string.IsNullOrEmpty(Request.Params["history"]))
            {
                ViewBag.history = Request.Params["history"];
            }
            else
            {
                ViewBag.history = "0";
            }
            return View();
        }
        public ActionResult BannerCategoryList()
        {
            return View();
        }
        public ActionResult BannerNewsSite()
        {
            return View();
        }
        public ActionResult BannerNewsList()
        {
            ViewBag.sid = Request.Params["sid"];
            List<BannerNewsSite> store = new List<BannerNewsSite>();
            BannerNewsSite bns = new BannerNewsSite();
            _bnsMgr = new BannerNewsSiteMgr(mySqlConnectionString);
            if (!string.IsNullOrEmpty(Request.Params["sid"]))
            {
                bns.news_site_id = uint.Parse(Request.Params["sid"]);
            }
            store = _bnsMgr.GetBannerNewsSiteName(bns);
            if (store.Count > 0)
            {
                ViewBag.sname = store[0].news_site_name;
            }
            else
            {
                ViewBag.sname = "";
            }
            if (!string.IsNullOrEmpty(Request.Params["history"]))
            {
                ViewBag.history = Request.Params["history"];
            }
            else
            {
                ViewBag.history = "0";
            }
            return View();
        }
        #endregion

        public HttpResponseBase GetBannerImageSiteList()
        {
            string json = string.Empty;
            List<BannerSite> store = new List<BannerSite>();
            BannerSite bs = new BannerSite();
            try
            {
                bs.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                bs.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _bsMgr = new BannerSiteMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _bsMgr.GetList(bs, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        public HttpResponseBase GetBannerImageList()
        {
            string json = string.Empty;
            List<BannerContent> store = new List<BannerContent>();
            BannerContent bc = new BannerContent();
            try
            {
                bc.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                bc.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (Request.Params["history"] == "1")
                {
                    bc.banner_status = 3;
                }
                if (!string.IsNullOrEmpty(Request.Params["sid"]))
                {
                    bc.banner_site_id = uint.Parse(Request.Params["sid"]);
                }
                _bcMgr = new BannerContentMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _bcMgr.GetList(bc, out totalCount);
                foreach (var item in store)
                {
                    if (item.banner_image.Length >= 4)
                    {
                        item.banner_image = imgServerPath + promoPath + item.banner_image.Substring(0, 2) + "/" + item.banner_image.Substring(2, 2) + "/" + item.banner_image;
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        public HttpResponseBase BannerImageEdit()
        {
            string json = string.Empty;
            BannerContent bc = new BannerContent();
            Serial serial = new Serial();
            _bcMgr = new BannerContentMgr(mySqlConnectionString);
            _ISerImplMgr = new SerialMgr(mySqlConnectionString);
            serial = _ISerImplMgr.GetSerialById(72);
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
            string localPromoPath = imgLocalServerPath + promoPath;//圖片存儲地址 /aimg.gigade100.com/ +/promotion/dev/ 

            string NewName = string.Empty;//當前文件名
            string fileExtention = string.Empty;//當前文件的擴展名
            string NewFileName = string.Empty;
            FileManagement fileLoad = new FileManagement();

            try
            {
                string oldImg = string.Empty;
                serial = _ISerImplMgr.GetSerialById(6);
                List<BannerContent> store = new List<BannerContent>();
                if (!string.IsNullOrEmpty(Request.Params["banner_content_id"]))
                {
                    int totalCount = 0;
                    bc.IsPage = false;
                    bc.banner_content_id = uint.Parse(Request.Params["banner_content_id"]);
                    if (Request.Params["history"] == "1")
                    {
                        bc.banner_status = 3;
                    }
                    store = _bcMgr.GetList(bc, out totalCount);
                    foreach (var item in store)
                    {
                        oldImg = item.banner_image;
                    }
                }
                bc = new BannerContent();
                bc.banner_title = Request.Params["banner_title"];
                bc.banner_link_url = Request.Params["banner_link_url"];
                if (!string.IsNullOrEmpty(Request.Params["banner_site_id"]))
                {
                    bc.banner_site_id = uint.Parse(Request.Params["banner_site_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["banner_link_mode"]))
                {
                    bc.banner_link_mode = int.Parse(Request.Params["banner_link_mode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["banner_sort"]))
                {
                    bc.banner_sort = uint.Parse(Request.Params["banner_sort"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["banner_statuses"]))
                {
                    bc.banner_status = uint.Parse(Request.Params["banner_statuses"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["banner_start"]))
                {
                    bc.banner_start = DateTime.Parse(DateTime.Parse(Request.Params["banner_start"]).ToString("yyyy-MM-dd") + " 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["banner_end"]))
                {
                    bc.banner_end = DateTime.Parse(DateTime.Parse(Request.Params["banner_end"]).ToString("yyyy-MM-dd") + " 23:59:59");
                }
                bc.banner_ipfrom = CommonFunction.GetClientIPNew();
                if (bc.banner_ipfrom == "::1")
                {
                    bc.banner_ipfrom = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[2].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["banner_content_id"]) && Request.Params["banner_image"] == oldImg)
                {
                    bc.banner_image = oldImg;
                }
                if (Request.Files["banner_image"] != null && Request.Files["banner_image"].ContentLength > 0)
                {
                    HttpPostedFileBase file = Request.Files["banner_image"];
                    if (file.ContentLength > int.Parse(minValue) * 1024 && file.ContentLength < int.Parse(maxValue) * 1024)
                    {
                        NewName = Path.GetFileName(file.FileName);
                        bool result = false;
                        string filename = NewName.Substring(0, NewName.LastIndexOf("."));
                        //獲得文件的後綴名
                        fileExtention = NewName.Substring(NewName.LastIndexOf(".")).ToLower();
                        //新的文件名是哈希字符串
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(filename, "32");

                        string firstFolder = NewFileName.Substring(0, 2) + "/";
                        string secondFolder = NewFileName.Substring(2, 2) + "/";
                        NewFileName = NewFileName + fileExtention;
                        NewName = NewFileName;
                        string ServerPath = string.Empty;
                        string localPromoDirectory = Server.MapPath(localPromoPath);
                        if (!System.IO.Directory.Exists(localPromoDirectory))
                        {
                            System.IO.Directory.CreateDirectory(localPromoDirectory);
                        }
                        FTP ftp = new FTP();
                        string directorys = promoPath + firstFolder + secondFolder;
                        ftp.MakeMultiDirectory(imgLocalPath+"/", directorys.Substring(1, directorys.Length - 2).Split('/'), ftpuser, ftppwd);
                        //NewFileName = localPromoPath + NewFileName;//絕對路徑
                        ServerPath = Server.MapPath(localPromoPath + firstFolder + secondFolder);
                        string ErrorMsg = string.Empty;
                        //上傳
                        result = fileLoad.UpLoadFile(file, ServerPath, imgLocalPath + directorys + NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                        if (!result)
                        {
                            json = "{success:false,msg:'圖片上傳失敗!'}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        bc.banner_image = NewName;
                        string oldImgPath = string.Empty;
                        string ftppath = string.Empty;
                        if (oldImg.Length >= 4)
                        {
                            oldImgPath = localPromoPath + oldImg.Substring(0, 2) + "/" + oldImg.Substring(2, 2) + "/" + oldImg;
                        }
                        if (System.IO.File.Exists(Server.MapPath(oldImgPath)))
                        {
                            ftppath=imgLocalPath + promoPath + oldImg.Substring(0, 2) + "/" + oldImg.Substring(2, 2) + "/";
                            System.IO.File.Delete(Server.MapPath(oldImgPath));
                            FTP ftp1 = new FTP(ftppath, ftpuser, ftppwd);
                            List<string> tem = ftp1.GetFileList();
                            if (tem.Contains(oldImg))
                            {
                                FTP ftps = new FTP(ftppath + oldImg, ftpuser, ftppwd);
                                ftps.DeleteFile(ftppath + oldImg);
                            }
                        }
                    }
                    else
                    {
                        json = "{success:false,msg:'上傳圖片不能超過" + maxValue + "K'}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                }
                
                #region 新增
                if (String.IsNullOrEmpty(Request.Params["banner_content_id"]))
                {
                    bc.banner_content_id = uint.Parse((serial.Serial_Value + 1).ToString());
                    if (_bcMgr.Add(bc) > 0)
                    {
                        serial.Serial_Value = serial.Serial_Value + 1;/*所在操作表的列增加*/
                        _ISerImplMgr.Update(serial);/*修改所在的表的列對應的值*/
                        json = "{success:true,msg:\"" + "新增成功！" + "\"}";
                    }

                }
                #endregion
                #region 編輯
                else
                {
                    bc.banner_content_id = uint.Parse(Request.Params["banner_content_id"]);
                    if (_bcMgr.Update(bc) > 0)
                    {
                        json = "{success:true,msg:\"" + "修改成功！" + "\"}";
                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                json = "{success:false,msg:\"" + "異常" + "\"}";
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
        public HttpResponseBase GetBannerCategoryList()
        {
            string json = string.Empty;
            List<BannerCategory> store = new List<BannerCategory>();
            BannerCategory bc = new BannerCategory();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["category_father_id"]))
                {
                    if (Request.Params["category_father_id"] == "0")
                    {
                        bc.category_father_id = 0;
                        bc.IsPage = false;
                    }
                    else
                    {
                        bc.category_father_id = int.Parse(Request.Params["category_father_id"]);
                    }
                }
                bc.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                bc.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _bcateMgr = new BannerCategoryMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _bcateMgr.GetBannerCategoryList(bc, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 刪除本地上傳的圖片
        /// <summary>
        /// 刪除本地上傳的圖片
        /// </summary>

        public void DeletePicFile(string imageName)
        {
            if (System.IO.File.Exists(imageName))
            {
                System.IO.File.Delete(imageName);
            }
        }
        #endregion
        #region 創建ftp文件夾
        /// <summary>
        /// 創建文件夾
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="Mappath">文件名</param>
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

        #region 文字廣告

        #region 文字廣告列表
        public HttpResponseBase GetBannerNewsSiteList()
        {
            string json = string.Empty;
            List<BannerNewsSiteQuery> store = new List<BannerNewsSiteQuery>();
            BannerNewsSiteQuery bs = new BannerNewsSiteQuery();
            try
            {
                bs.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                bs.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _bnsMgr = new BannerNewsSiteMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _bnsMgr.GetList(bs, out totalCount);
                foreach (var item in store)
                {
                    item.creattime = CommonFunction.GetNetTime(item.news_site_createdate);
                    item.updtime = CommonFunction.GetNetTime(item.news_site_updatedate);
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";

                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        ///內容和歷史跳轉鏈接頁面  文字廣告列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetBannerNewsList()
        {
            string json = string.Empty;
            BannerNewsContent query = new BannerNewsContent();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["sid"]))
                {
                    query.news_site_id = uint.Parse(Request.Params["sid"]);
                }
                if (Request.Params["history"] == "1")
                {
                    query.news_status = 3;
                }
                bncMgr = new BannerNewsContentMgr(mySqlConnectionString);
                int totalCount = 0;
                DataTable dt = bncMgr.GetBannerNewsContentList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 文字廣告新增
        public HttpResponseBase SaveBannerNewsContent()
        {
            string json = string.Empty;
            try
            {
                bncMgr = new BannerNewsContentMgr(mySqlConnectionString);
                BannerNewsContentQuery query = new BannerNewsContentQuery();
                if (!string.IsNullOrEmpty(Request.Params["news_id"]))
                {//如果是編輯獲取該id數據
                    query.news_id = uint.Parse(Request.Params["news_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["news_site_id"]))
                {
                    query.news_site_id = uint.Parse(Request.Params["news_site_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["news_title"]))
                {
                    query.news_title = Request.Params["news_title"];
                }
                if (!string.IsNullOrEmpty(Request.Params["news_link_url"]))
                {
                    query.news_link_url = Request.Params["news_link_url"];
                }
                if (!string.IsNullOrEmpty(Request.Params["news_link_mode"]))
                {
                    query.news_link_mode = Convert.ToInt32(Request.Params["news_link_mode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["news_sort"]))
                {
                    query.news_sort = Convert.ToUInt32(Request.Params["news_sort"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["news_status"]))
                {
                    query.news_status = Convert.ToUInt32(Request.Params["news_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["news_start"]))
                {
                    query.news_start = uint.Parse(CommonFunction.GetPHPTime(Request.Params["news_start"].Substring(0, 10) + " 00:00:00").ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["news_end"]))
                {
                    query.news_end = uint.Parse(CommonFunction.GetPHPTime(Request.Params["news_end"].Substring(0, 10) + " 23:59:59").ToString());
                }
                query.news_createdate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                query.news_updatedate = query.news_createdate;
                query.news_ipfrom = CommonFunction.GetClientIPNew();
                if (query.news_ipfrom == "::1")
                {
                    query.news_ipfrom = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[2].ToString();
                }
                if (query.news_id > 0)
                {//編輯
                    if (bncMgr.UpdateBannerNewsContent(query) > 0)
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
                    if (bncMgr.SaveBannerNewsContent(query) > 0)
                    {
                        json = "{success:true,msg:'新增成功!'}";
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
                json = "{success:false,msg:'操作失敗!'}";
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
