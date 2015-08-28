using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using System.Text.RegularExpressions;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Common;
using System.Configuration;
using System.IO;
using System.Data;

using BLL.gigade.Model.Custom;
using System.Collections;
namespace Admin.gigade.Controllers
{
    public class ProductCategoryController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        //上傳圖片
        string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        static string excelPath = ConfigurationManager.AppSettings["ImportCategoryCSV"];//關於導入的excel文件的限制

        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        //string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.promoPath);//圖片保存路徑

        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        //end上傳圖片
        private IProductCategoryImplMgr prodCateMgr;
        private ICategoryImplMgr _proCategoryImplMgr;
        private IProductCategoryBannerImplMgr _productCategoryBannerImplMgr;
        private IProductCategoryBannerImplMgr _IProductCategoryBannerMgr;
        private ProductCategoryBrandMgr _cateBrandMgr;
        private IParametersrcImplMgr _parameterMgr;
        //
        // GET: /ProductCategory/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 專區類別商品設置
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductCategorySet()
        {
            return View();
        }
        /// <summary>
        /// 類別品牌設置
        /// </summary>
        /// <returns></returns>
        public ActionResult CateBrand()
        {
            return View();
        }

        #region 類別列表頁+HttpResponseBase GetProductCategoryList()
        /// <summary>
        /// 類別列表頁
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetProductCategoryList()
        {
            List<CategoryQuery> store = new List<CategoryQuery>();
            string json = string.Empty;
            try
            {
                CategoryQuery query = new CategoryQuery();
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["father_id"]))
                {
                    query.category_father_id = Convert.ToUInt32(Request.Params["father_id"]);
                }
                _proCategoryImplMgr = new CategoryMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _proCategoryImplMgr.GetProductCategoryList(query, out totalCount);
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 新增編輯類別信息+HttpResponseBase ProductCategorySave()
        /// <summary>
        /// 新增編輯類別信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase ProductCategorySave()
        {
            string json = string.Empty;
            string jsonStr = string.Empty;
            CategoryQuery cq = new CategoryQuery();
            IAreaPactetImplMgr _iareaPacketMgr = new AreaPacketMgr(mySqlConnectionString);
            try
            {
                if (string.IsNullOrEmpty(Request.Params["category_id"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["comboFrontCage"]))
                    {
                        cq.category_father_id = Convert.ToUInt32(Request.Params["comboFrontCage"].ToString());
                    }

                    if (!string.IsNullOrEmpty(Request.Params["category_name"]))
                    {
                        cq.category_name = Request.Params["category_name"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Params["category_sort"]))
                    {
                        cq.category_sort = Convert.ToUInt32(Request.Params["category_sort"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Request.Params["category_display"]))
                    {
                        cq.category_display = Convert.ToUInt32(Request.Params["category_display"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["categorylinkmode"]))
                    {
                        cq.category_link_mode = Convert.ToUInt32(Request.Params["categorylinkmode"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Request.Params["category_link_url"]))
                    {
                        cq.category_link_url = Request.Params["category_link_url"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["photo"]))
                    {
                        #region 上傳圖片

                        try
                        {
                            if (Request.Files.Count != 0)
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

                                string localPromoPath = imgLocalPath + promoPath;//圖片存儲地址
                                //生成隨機數，用於圖片的重命名
                                Random rand = new Random();
                                int newRand = rand.Next(1000, 9999);



                                //獲取上傳的圖片
                                HttpPostedFileBase file = Request.Files[0];
                                string fileName = string.Empty;//當前文件名
                                string fileExtention = string.Empty;//當前文件的擴展名

                                fileName = Path.GetFileName(file.FileName);
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    bool result = false;
                                    string NewFileName = string.Empty;

                                    fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                    //NewFileName = pacdModel.event_id + newRand + fileExtention;//圖片重命名為event_id+4位隨機數+擴展名
                                    NewFileName = newRand + fileExtention;
                                    string ServerPath = string.Empty;
                                    //判斷目錄是否存在，不存在則創建
                                    CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));

                                    //  returnName += promoPath + NewFileName;
                                    fileName = NewFileName;
                                    NewFileName = localPromoPath + NewFileName;//絕對路徑
                                    ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                                    string ErrorMsg = string.Empty;

                                    //上傳之前刪除已有的圖片
                                    //string oldFileName = olderpcmodel.banner_image;
                                    FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                                    List<string> tem = ftp.GetFileList();
                                    //if (tem.Contains(oldFileName))
                                    //{
                                    //    //FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                                    //    //ftps.DeleteFile(localPromoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                    //    //DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                    //}
                                    try
                                    {
                                        //上傳
                                        FileManagement fileLoad = new FileManagement();
                                        result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                        if (result)//上傳成功
                                        {
                                            cq.banner_image = fileName;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name + "->fileLoad.UpLoadFile()", ex.Source, ex.Message);
                                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                        log.Error(logMessage);
                                        jsonStr = "{success:false,msg:'圖片上傳失敗'}";

                                        //pacdModel.banner_image = olderpcmodel.banner_image;
                                    }
                                }
                                else
                                {
                                    //pacdModel.banner_image = olderpcmodel.banner_image;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name + "->圖片上傳失敗！", ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            jsonStr = "{success:false,msg:'圖片上傳失敗'}";

                            //pacdModel.banner_image = olderpcmodel.banner_image;
                        }


                        #endregion
                    }
                    if (!string.IsNullOrEmpty(Request.Params["banner_status"]))
                    {
                        cq.banner_status = Convert.ToUInt32(Request.Params["banner_status"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Request.Params["banner_link_mode"]))
                    {
                        cq.banner_link_mode = Convert.ToUInt32(Request.Params["banner_link_mode"].ToString());
                    }
                    if (!string.IsNullOrEmpty(Request.Params["banner_link_url"]))
                    {
                        cq.banner_link_url = Request.Params["banner_link_url"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                    {
                        cq.banner_show_start = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["startdate"].ToString()));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                    {
                        cq.banner_show_end = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["enddate"].ToString()));
                    }
                    cq.category_ipfrom = CommonFunction.GetClientIP();
                    cq.category_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    cq.category_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    cq.status = 1;
                    _proCategoryImplMgr = new CategoryMgr(mySqlConnectionString);

                    int i = _proCategoryImplMgr.ProductCategorySave(cq);
                    if (i > 0)
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    _proCategoryImplMgr = new CategoryMgr(mySqlConnectionString);
                    cq.category_id = Convert.ToUInt32(Request.Params["category_id"]);
                    CategoryQuery oldCq = _proCategoryImplMgr.GetProductCategoryById(cq);
                    if (!string.IsNullOrEmpty(Request.Params["comboFrontCage"]))
                    {
                        cq.category_father_id = Convert.ToUInt32(Request.Params["comboFrontCage"].ToString());
                    }
                    else
                    {
                        cq.category_father_id = oldCq.category_father_id;
                    }

                    if (!string.IsNullOrEmpty(Request.Params["category_name"]))
                    {
                        cq.category_name = Request.Params["category_name"].ToString();
                    }
                    else
                    {
                        cq.category_name = oldCq.category_name;
                    }

                    if (!string.IsNullOrEmpty(Request.Params["category_sort"]))
                    {
                        cq.category_sort = Convert.ToUInt32(Request.Params["category_sort"].ToString());
                    }
                    else
                    {
                        cq.category_sort = oldCq.category_sort;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["category_display"]))
                    {
                        cq.category_display = Convert.ToUInt32(Request.Params["category_display"]);
                    }
                    else
                    {
                        cq.category_display = oldCq.category_display;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["categorylinkmode"]))
                    {
                        cq.category_link_mode = Convert.ToUInt32(Request.Params["categorylinkmode"].ToString());
                    }
                    else
                    {
                        cq.category_link_mode = oldCq.category_link_mode;
                    }

                    cq.category_link_url = Request.Params["category_link_url"].ToString();

                    if (!string.IsNullOrEmpty(Request.Params["photo"]))
                    {
                        #region 上傳圖片

                        try
                        {
                            if (Request.Files.Count != 0)
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

                                string localPromoPath = imgLocalPath + promoPath;//圖片存儲地址
                                //生成隨機數，用於圖片的重命名
                                Random rand = new Random();
                                int newRand = rand.Next(1000, 9999);



                                //獲取上傳的圖片
                                HttpPostedFileBase file = Request.Files[0];
                                string fileName = string.Empty;//當前文件名
                                string fileExtention = string.Empty;//當前文件的擴展名

                                fileName = Path.GetFileName(file.FileName);
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    bool result = false;
                                    string NewFileName = string.Empty;

                                    fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                                    //NewFileName = pacdModel.event_id + newRand + fileExtention;//圖片重命名為event_id+4位隨機數+擴展名
                                    NewFileName = newRand + fileExtention;
                                    string ServerPath = string.Empty;
                                    //判斷目錄是否存在，不存在則創建
                                    CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));

                                    //  returnName += promoPath + NewFileName;
                                    fileName = NewFileName;
                                    NewFileName = localPromoPath + NewFileName;//絕對路徑
                                    ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                                    string ErrorMsg = string.Empty;

                                    //上傳之前刪除已有的圖片
                                    string oldFileName = oldCq.banner_image;
                                    FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                                    List<string> tem = ftp.GetFileList();
                                    if (tem.Contains(oldFileName))
                                    {
                                        //FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                                        //ftps.DeleteFile(localPromoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                        //DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                    }
                                    try
                                    {
                                        //上傳
                                        FileManagement fileLoad = new FileManagement();
                                        result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                        if (result)//上傳成功
                                        {
                                            cq.banner_image = fileName;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name + "->fileLoad.UpLoadFile()", ex.Source, ex.Message);
                                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                        log.Error(logMessage);
                                        jsonStr = "{success:false,msg:'圖片上傳失敗'}";

                                        cq.banner_image = oldCq.banner_image;
                                    }
                                }
                                else
                                {
                                    cq.banner_image = oldCq.banner_image;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name + "->圖片上傳失敗！", ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            jsonStr = "{success:false,msg:'圖片上傳失敗'}";

                            cq.banner_image = oldCq.banner_image;
                        }


                        #endregion
                    }
                    else
                    {
                        cq.banner_image = oldCq.banner_image;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["banner_status"]))
                    {
                        cq.banner_status = Convert.ToUInt32(Request.Params["banner_status"].ToString());
                    }
                    else
                    {
                        cq.banner_status = oldCq.banner_status;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["banner_link_mode"]))
                    {
                        cq.banner_link_mode = Convert.ToUInt32(Request.Params["banner_link_mode"].ToString());
                    }
                    else
                    {
                        cq.banner_link_mode = oldCq.banner_link_mode;
                    }

                    cq.banner_link_url = Request.Params["banner_link_url"].ToString();


                    cq.banner_show_start = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["startdate"].ToString()));

                    cq.banner_show_end = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["enddate"].ToString()));

                    cq.category_ipfrom = CommonFunction.GetClientIP();
                    cq.category_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    int j = _proCategoryImplMgr.ProductCategorySave(cq);
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
        #endregion

        #region 獲取父類別編號 + HttpResponseBase GetFatherId()
        /// <summary>
        /// 獲取父類別編號
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult GetFatherId()
        {
            string jsonStr = string.Empty;
            try
            {
                CategoryQuery model = new CategoryQuery();

                if (!string.IsNullOrEmpty(Request.Params["fid"]))
                {
                    model.category_id = Convert.ToUInt32(Request.Params["fid"]);
                }
                _proCategoryImplMgr = new CategoryMgr(mySqlConnectionString);
                CategoryQuery cq = _proCategoryImplMgr.GetProductCategoryById(model);
                return Json(new { success = "true", result = cq.category_father_id.ToString() });
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

        #region 更改活動使用狀態 + HttpResponseBase UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string jsonStr = string.Empty;
            try
            {
                CategoryQuery model = new CategoryQuery();
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    model.status = Convert.ToInt32(Request.Params["active"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    model.category_id = Convert.ToUInt32(Request.Params["id"]);
                }
                _proCategoryImplMgr = new CategoryMgr(mySqlConnectionString);
                if (_proCategoryImplMgr.UpdateActive(model) > 0)
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

        #region 創建ftp文件夾 + void CreateFolder(string path, string[] Mappath)
        /// <summary>
        /// 創建文件夾
        /// </summary>
        /// <param name="path">路徑</param>
        /// <param name="Mappath">文件名數組</param>
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
                        // ftp = new FTP(fullPath.Substring(0, fullPath.Length - 1), ftpuser, ftppwd);
                        ftp = new FTP(fullPath, ftpuser, ftppwd);
                        ftp.MakeDirectory();
                    }
                    fullPath += "/";
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


        #region 類別信息匯出csv + HttpResponseBase ProductCategoryCSV()
        /// <summary>
        /// 匯出csv
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase ProductCategoryCSV()
        {
            string newCsvName = string.Empty;
            string json = string.Empty;
            DataTable dt = new DataTable();
            CategoryQuery query = new CategoryQuery();
            if (!string.IsNullOrEmpty(Request.Params["Root"]))
            {
                query.category_father_id = Convert.ToUInt32(Request.Params["Root"]);
            }

            List<ProductCategory> categoryList = new List<ProductCategory>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            // _procateMgr = new ProductCategoryMgr(connectionString);
            _proCategoryImplMgr = new CategoryMgr(mySqlConnectionString);
            dt.Columns.Add(new DataColumn("", typeof(string)));
            DataRow dr = dt.NewRow();
            dr[0] = query.category_father_id.ToString() + "   " + _proCategoryImplMgr.GetProductCategoryById(new CategoryQuery { category_id = query.category_father_id }).category_name;//根
            dt.Rows.Add(dr);
            //  categoryList = _procateMgr.GetProductCate(new ProductCategory { });//數據源
            categoryList = _proCategoryImplMgr.GetProductCategoryCSV(query);


            cateList = getCate(categoryList, query.category_father_id.ToString());//得到第二層分支
            // cateList={1,2,3,4};

            //調試resultlist是否為空
            GetCategoryList(categoryList, ref cateList, ref dt, 1);

            try
            {

                string filename = "ProductCategory_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                newCsvName = Server.MapPath(excelPath) + filename;

                if (System.IO.File.Exists(newCsvName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newCsvName, FileAttributes.Normal);
                    System.IO.File.Delete(newCsvName);
                }
                string[] colname = { };

                CsvHelper.ExportDataTableToCsv(dt, newCsvName, colname, false);
                json = "true," + filename + "," + excelPath;

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "false, ";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }


        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetCategoryList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist, ref DataTable dt, int newCol)
        {

            for (int i = 0; i < catelist.Count; i++)// cateList={1,2,3,4};
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, catelist[i].id.ToString());
                catelist[i].children = childList;
                if (childList.Count() > 0)
                {
                    if (i == 0)
                    {
                        if (newCol >= dt.Columns.Count)
                        {
                            dt.Columns.Add(new DataColumn());//dt[0][1]
                        }
                        dt.Rows[dt.Rows.Count - 1][newCol] = catelist[i].id.ToString() + "  " + catelist[i].text.ToString();
                        GetCategoryList(categoryList, ref childList, ref dt, newCol + 1);
                    }
                    else
                    {
                        DataRow drNew = dt.NewRow();
                        drNew[newCol] = catelist[i].id.ToString() + "  " + catelist[i].text.ToString();
                        dt.Rows.Add(drNew);//dt[2][1]
                        GetCategoryList(categoryList, ref childList, ref dt, newCol + 1);
                    }
                }
                else//沒有子節點時一根枝杈已走完
                {
                    if (i == 0)
                    {
                        if (newCol >= dt.Columns.Count)
                        {
                            dt.Columns.Add(new DataColumn());//dt[0][1]
                        }
                        dt.Rows[dt.Rows.Count - 1][newCol] = catelist[i].id.ToString() + "  " + catelist[i].text.ToString();
                    }
                    else
                    {
                        DataRow dr = dt.NewRow();
                        dr[newCol] = catelist[i].id.ToString() + "  " + catelist[i].text.ToString();//dt[0][1]
                        dt.Rows.Add(dr);
                    }
                }
            }
        }

        public List<ProductCategoryCustom> getCate(List<ProductCategory> categoryList, string fatherId)
        {
            var cateList = (from c in categoryList
                            where c.category_father_id.ToString() == fatherId
                            select new
                            {
                                id = c.category_id,
                                text = c.category_name
                            }).ToList().ConvertAll<ProductCategoryCustom>(m => new ProductCategoryCustom
                            {
                                id = m.id.ToString(),
                                text = m.text
                            });
            return cateList;
        }


        #endregion

        #region 左邊樹
        public List<ProductCategoryBannerCustom> ProductCategoryBannerCate(List<ProductCategoryBannerQuery> categoryList, uint fatherId)
        {
            var cateList = (from c in categoryList
                            where c.category_father_id == fatherId
                            select new
                            {
                                id = c.category_id,
                                text = c.category_name
                            }).ToList().ConvertAll<ProductCategoryBannerCustom>(m => new ProductCategoryBannerCustom
                            {
                                id = m.id.ToString(),
                                text = m.text
                            });
            return cateList;
        }

        #region 得到所有前台分類
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase GetProductCategoryBanner(string id = "true")
        {
            List<ProductCategoryBannerQuery> categoryList = new List<ProductCategoryBannerQuery>();
            List<ProductCategoryBannerCustom> cateList = new List<ProductCategoryBannerCustom>();
            string resultStr = "";
            try
            {
                _productCategoryBannerImplMgr = new ProductCategoryBannerMgr(mySqlConnectionString);
                ProductCategoryBannerQuery model = new ProductCategoryBannerQuery();
                categoryList = _productCategoryBannerImplMgr.QueryAll(model);
                cateList = ProductCategoryBannerCate(categoryList, 2);
                List<ProductCategorySet> resultList = new List<ProductCategorySet>();

                GetCategoryList(categoryList, ref cateList, resultList);
                resultStr = JsonConvert.SerializeObject(cateList);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 遞歸得到分類節點
        /// <summary>
        /// 遞歸得到分類節點
        /// </summary>
        /// <param name="catelist">父節點</param>
        public void GetCategoryList(List<ProductCategoryBannerQuery> categoryList, ref List<ProductCategoryBannerCustom> catelist, List<ProductCategorySet> resultList)
        {
            foreach (ProductCategoryBannerCustom item in catelist)
            {
                List<ProductCategoryBannerCustom> childList = ProductCategoryBannerCate(categoryList, uint.Parse(item.id.ToString()));
                item.children = childList;

                ProductCategorySet resultTemp = resultList.Where(m => m.Category_Id.ToString() == item.id).FirstOrDefault();
                if (resultTemp != null)
                {
                    item.Checked = true;
                }

                if (childList.Count() > 0)
                {
                    GetCategoryList(categoryList, ref childList, resultList);
                }
            }

        }
        #endregion
        #endregion

        #region        product_category_banner列表頁 +GetProCateBanList()
        public HttpResponseBase GetProCateBanList()
        {
            string json = string.Empty;
            List<ProductCategoryBannerQuery> store = new List<ProductCategoryBannerQuery>();
            ProductCategoryBannerQuery query = new ProductCategoryBannerQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");
                if (!string.IsNullOrEmpty(Request.Params["banner_id"]))
                {
                    query.banner_cateid = Convert.ToUInt32(Request.Params["banner_id"]);
                }
                int totalCount = 0;
                _IProductCategoryBannerMgr = new ProductCategoryBannerMgr(mySqlConnectionString);
                store = _IProductCategoryBannerMgr.GetProCateBanList(query, out totalCount);
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
        #endregion

        #region 新增編輯類別設定信息+HttpResponseBase CategoryBannerSetSave()
        /// <summary>
        /// 新增編輯類別設定信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase CategoryBannerSetSave()
        {
            ProductCategoryBannerQuery query = new ProductCategoryBannerQuery();
            _IProductCategoryBannerMgr = new ProductCategoryBannerMgr(mySqlConnectionString);
            string json = string.Empty;
            string insertValues = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["banner_cateid"]))
                {
                    query.banner_cateid = Convert.ToUInt32(Request.Params["banner_cateid"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["insertValues"]))
                {
                    insertValues = Request.Params["insertValues"].ToString();
                    insertValues = Server.HtmlDecode(insertValues);
                    string[] values = insertValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    query.createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    query.updatedate = query.createdate;
                    query.create_ipfrom = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();

                    if (_IProductCategoryBannerMgr.Save(values, query))
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
                    bool b = _IProductCategoryBannerMgr.DeleteByCateId(query);
                    if (b)
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region  專區類別刪除
        public HttpResponseBase DeleteProCateBan()
        {
            string json = string.Empty;
            _IProductCategoryBannerMgr = new ProductCategoryBannerMgr(mySqlConnectionString);
            if (!string.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    foreach (string item in Request.Params["rowID"].Split('|'))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _IProductCategoryBannerMgr.DeleteProCateBan(Convert.ToInt32(item));
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

        #region 修改狀態
        public JsonResult UpdateState()
        {
            string json = string.Empty;
            ProductCategoryBannerQuery query = new ProductCategoryBannerQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.status = Convert.ToInt32(Request.Params["active"]);
                }
                _IProductCategoryBannerMgr = new ProductCategoryBannerMgr(mySqlConnectionString);
                if (_IProductCategoryBannerMgr.UpdateState(query) > 0)
                {
                    return Json(new { success = "true", msg = "" });
                }
                else
                {
                    return Json(new { success = "false", msg = "" });
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion

        #region 遞歸查詢子cateID
        /// <summary>
        /// 遞歸查詢子ID 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowid"></param>
        public void GetAllCategory_id(List<ProductCategory> category, uint rowid, ref string id)
        {
            List<ProductCategory> query = category.FindAll(p => p.category_father_id == rowid).ToList();
            if (query.Count != 0)
            {
                foreach (var que in query)
                {
                    id += "," + que.category_id.ToString();
                    GetAllCategory_id(category, que.category_id, ref id);
                }
            }
            if (id.IndexOf(rowid.ToString()) < 0)
            {
                id += "," + rowid.ToString();
            }
            if (id.Substring(0, 1) == ",")
            {
                id = id.Remove(0, 1);
            }
        }
        #endregion

        #region 重寫獲取專區類別 新館類別   GetProductByCategorySet() GetFatherCategory_id


        #region 根據banner_id獲取相對相應的cate_id +HttpResponseBase GetProductByCategorySet()
        public HttpResponseBase GetProductByCategorySet()
        {
            string resultStr = "{success:false,msg:0,item:[]}";
            try
            {
                _productCategoryBannerImplMgr = new ProductCategoryBannerMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["banner_cateid"].ToString()))
                {
                    uint isTryUint = 0;
                    if (uint.TryParse(Request.Params["banner_cateid"].ToString(), out isTryUint))
                    {
                        uint banner_cateid = Convert.ToUInt32(Request.Params["banner_cateid"].ToString());

                        //獲取新館類別
                        uint fatherid = _productCategoryBannerImplMgr.GetXGCate().FirstOrDefault().category_id;//獲取754
                        prodCateMgr = new ProductCategoryMgr(mySqlConnectionString);//實例化對象mgr
                        List<ProductCategory> category = prodCateMgr.QueryAll(new ProductCategory { category_display = 0 });//獲取所有的類別 包括隱藏和顯示的
                        string cateStr = string.Empty;//設定對象保存新館所有類別
                        GetAllCategory_id(category, fatherid, ref cateStr);//獲取所有新館類別

                        if (!string.IsNullOrEmpty(cateStr))//專區類別處理數據
                        {
                            DataTable dt = new DataTable();
                            List<ProductCategory> query = category.FindAll(p => p.category_id == banner_cateid).ToList();//判斷是否是類別專區 
                            if (query.Count != 0) //新館專區類別設定
                            {
                                dt = _productCategoryBannerImplMgr.isSaleProd(cateStr, banner_cateid);//獲取所有類別
                            }
                            else//不是專區的類別數據  優惠專區999998  優惠專區頁999997 優惠專區頁999996 產地直送999995
                            {
                                dt = _productCategoryBannerImplMgr.GetXGCateByBanner(cateStr, banner_cateid);
                            }

                            string cateStrs = string.Empty;//存儲所有新館類別樹狀結構數據

                            //根據新館子元素找父級元素 新館樹狀結構
                            if (dt.Rows.Count != 0)
                            {
                                bool isTrue = false;
                                string cateStrTree = string.Empty;

                                foreach (DataRow row in dt.Rows)
                                {
                                    if (Convert.ToUInt32(Convert.ToUInt32(row["category_id"])) != banner_cateid)
                                    {
                                        GetFatherCategory_id(category, ref isTrue, fatherid, Convert.ToUInt32(row["category_id"]), ref cateStrTree);
                                        if (isTrue && cateStrTree != null)
                                        {
                                            cateStrs += Convert.ToUInt32(row["category_id"]);
                                            cateStrs += ",";
                                        }
                                    }
                                }
                                cateStrs += cateStrTree;

                                if (!string.IsNullOrEmpty(cateStrs))
                                {
                                    cateStrs = cateStrs.Substring(0, cateStrs.Length - 1);
                                    List<ProductCategory> li = _productCategoryBannerImplMgr.GetProductCategoryInfo(cateStrs);
                                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                                    //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                                    resultStr = "{success:true,item:" + JsonConvert.SerializeObject(li, Formatting.Indented, timeConverter) + "}";//返回json數據

                                }
                            }
                            else
                            {
                                resultStr = "{success:true,totalCount:0,item:[]}";
                            }
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
                resultStr = "{success:false,msg:0,item:[]}";
            }


            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 遞歸驗證該category_id的父節點是否等於fatherId
        /// <summary>
        /// 遞歸驗證該category_id的父節點是否等於fatherId
        /// isTrue默認為false，方法結束判斷isTrue是否為true
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowid"></param>
        public void GetFatherCategory_id(List<ProductCategory> category, ref bool isTrue, uint fatherId, uint cate_id, ref string cateStr)
        {
            ProductCategory query = category.FindAll(p => p.category_id == cate_id).ToList().FirstOrDefault();//找到該cate_id

            if (query.category_father_id != 0 && query.category_father_id >= fatherId)//判斷其父元素id不為0
            {
                if (query.category_father_id == fatherId)
                {
                    isTrue = true;
                    return;
                }
                else
                {
                    if (!cateStr.Contains(query.category_father_id.ToString()))//若不存在該字符則保存
                    {
                        cateStr += query.category_father_id + ",";
                        isTrue = false;
                        GetFatherCategory_id(category, ref isTrue, fatherId, query.category_father_id, ref cateStr);
                        return;
                    }
                    else
                    {
                        isTrue = true;
                        return;
                    }
                }
            }
            else
            {
                isTrue = false;
                return;
            }
        }
        #endregion
        #endregion



        #region  類別品牌 +HttpResponseBase GetCateBrandList()
        [CustomHandleError]
        public HttpResponseBase GetCateBrandList()
        {
            List<ProductCategoryBrandQuery> store = new List<ProductCategoryBrandQuery>();
            _cateBrandMgr = new ProductCategoryBrandMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                ProductCategoryBrandQuery query = new ProductCategoryBrandQuery();
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["searchCate"]))
                {
                    uint cate_id = 0;
                    if (uint.TryParse(Request.Params["searchCate"].ToString(), out cate_id))
                    {
                        query.category_id = cate_id;
                    }
                    else
                    {
                        query.category_name = Request.Params["searchCate"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["searchBrand"]))
                {
                    uint brand_id = 0;
                    if (uint.TryParse(Request.Params["searchBrand"].ToString(), out brand_id))
                    {
                        query.brand_id = brand_id;
                    }
                    else
                    {
                        query.brand_name = Request.Params["searchBrand"].ToString();
                    }
                } if (!string.IsNullOrEmpty(Request.Params["banner_id"]))
                {
                    query.banner_cate_id = Convert.ToInt32(Request.Params["banner_id"]);
                }
                _proCategoryImplMgr = new CategoryMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _cateBrandMgr.GetCateBrandList(query, out totalCount);
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        [CustomHandleError]
        public HttpResponseBase GetBannerCateBrand()
        {
            string json = "{success:true}";
            _cateBrandMgr = new ProductCategoryBrandMgr(mySqlConnectionString);
            _productCategoryBannerImplMgr = new ProductCategoryBannerMgr(mySqlConnectionString);
            _parameterMgr = new ParameterMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["banner_cateid"].ToString()))
                {
                    uint isTryUint = 0;
                    if (uint.TryParse(Request.Params["banner_cateid"].ToString(), out isTryUint))
                    {
                        int banner_cateid = Convert.ToInt32(Request.Params["banner_cateid"].ToString());

                        //獲取新館類別
                        uint XGCateId = _productCategoryBannerImplMgr.GetXGCate().FirstOrDefault().category_id;//獲取754
                        prodCateMgr = new ProductCategoryMgr(mySqlConnectionString);//實例化對象mgr
                        List<ProductCategory> category = prodCateMgr.QueryAll(new ProductCategory { category_display = 1 });//獲取所有的類別 包括隱藏和顯示的
                        List<ProductCategory> cateXGlist = new List<ProductCategory>();//設定對象保存新館所有類別
                        GetAllCategory_idList(category, XGCateId, ref cateXGlist);//獲取所有新館類別
                        List<Parametersrc> pali = _parameterMgr.GetElementType("banner_cate");

                        List<ProductCategoryBrand> XGCateBrandResult = new List<ProductCategoryBrand>();
                        if (pali.Count != 0)
                        {
                            if (cateXGlist != null)
                            {


                                List<ProductCategory> query = category.FindAll(p => p.category_id == banner_cateid).ToList();//判斷是否是類別專區 
                                List<ProductCategoryBrand> ProList = new List<ProductCategoryBrand>();

                                if (query.Count != 0) //新館專區類別設定
                                {
                                    ProList = _cateBrandMgr.GetSaledProduct(XGCateId, banner_cateid);//獲取所有有效商品 以及對應的品牌和館別
                                }
                                else
                                {
                                    ProList = _cateBrandMgr.GetProductByCondi(XGCateId, banner_cateid);
                                }
                                //ProList = _cateBrandMgr.GetSaledProduct(XGCateId);//獲取所有有效商品

                                foreach (ProductCategoryBrand itemcate in ProList)
                                {
                                    ProductCategory querySingle = cateXGlist.Find(p => p.category_id == itemcate.category_id);
                                    if (querySingle != null)
                                    {
                                        itemcate.category_name = querySingle.category_name;
                                        XGCateBrandResult.Add(itemcate);
                                        GetFatherCateBrand(cateXGlist, XGCateId, itemcate, ref  XGCateBrandResult);
                                    }
                                }
                                json = "{success:true,item:" + JsonConvert.SerializeObject(XGCateBrandResult, Formatting.Indented) + "}";//返回json數據
                            }
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
                json = "{success:false,data:[]}";
            }


            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        [CustomHandleError]
        public HttpResponseBase SaveCateBrand()
        {
            string json = "{success:false}";
            _cateBrandMgr = new ProductCategoryBrandMgr(mySqlConnectionString);
            string insertValues = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["banner_cateid"]))
                {
                    int banner_cate_id = Convert.ToInt32(Request.Params["banner_cateid"].ToString());
                    if (!string.IsNullOrEmpty(Request.Params["insertValues"]))
                    {
                        insertValues = Request.Params["insertValues"].ToString();
                        insertValues = Server.HtmlDecode(insertValues);
                        string[] values = insertValues.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        if (_cateBrandMgr.InsertCateBrand(values, banner_cate_id))
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
                        bool b = _cateBrandMgr.DeleteCateBrand(banner_cate_id);
                        if (b)
                        {
                            json = "{success:true}";
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
        #region 遞歸驗證該category_id的父節點是否等於fatherId
        /// <summary>
        /// 遞歸驗證該category_id的父節點是否等於fatherId
        /// isTrue默認為false，方法結束判斷isTrue是否為true
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowid"></param>
        public void GetFatherCateBrand(List<ProductCategory> category, uint fatherId, ProductCategoryBrand model, ref List<ProductCategoryBrand> XGCateBrandResult)
        {
            ProductCategory query = category.FindAll(p => p.category_id == model.category_id).ToList().FirstOrDefault();//找到該cate_id

            if (query != null)
            {
                if (query.category_father_id == fatherId || query.category_id == fatherId)
                {
                    XGCateBrandResult.Remove(model);
                    return;
                }
                else
                {
                    model.category_father_id = query.category_father_id;
                    model.category_father_name = category.Find(p => p.category_id == model.category_father_id).category_name;
                    ProductCategoryBrand modelItem = new ProductCategoryBrand();
                    modelItem.brand_id = model.brand_id;
                    modelItem.category_id = query.category_father_id;
                    modelItem.category_name = category.Find(p => p.category_id == modelItem.category_id).category_name;
                    modelItem.depth = 2;
                    if (XGCateBrandResult.FindAll(p => p.brand_id == modelItem.brand_id && p.category_id == modelItem.category_id).ToList().Count == 0)//若不存在該字符則保存
                    {
                        XGCateBrandResult.Add(modelItem);
                    }
                    GetFatherCateBrand(category, fatherId, modelItem, ref XGCateBrandResult);
                    model.depth = modelItem.depth + 1;
                    return;
                }
            }
            else
            {
                return;
            }
        }
        #endregion

        #region 遞歸查詢子cateID
        /// <summary>
        /// 遞歸查詢子ID 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowid"></param>
        public void GetAllCategory_idList(List<ProductCategory> category, uint rowid, ref List<ProductCategory> cate)
        {
            List<ProductCategory> query = category.FindAll(p => p.category_father_id == rowid).ToList();
            if (query.Count != 0)
            {
                foreach (var que in query)
                {
                    cate.Add(que);
                    GetAllCategory_idList(category, que.category_id, ref cate);
                }
            }
        }
        #endregion
    }
}
