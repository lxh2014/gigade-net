using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Model.Query;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using System.IO;
using System.Configuration;
using System.Data;
using System.Text;
using System.Collections;
using LitJson;
using System.Globalization;
using BLL.gigade.Dao;

namespace Admin.gigade.Controllers
{
    public class ElementController : Controller
    {
        //
        // GET: /Element/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IElementDetailImplMgr _detailMgr;
        private IPageAreaImplMgr _pageAreaMgr;
        private ISitePageImplMgr _sitePageMg;
        private IProductImplMgr _productMgr;
        private IParametersrcImplMgr _parameterMgr;
        private ISiteImplMgr _siteMgr;
        private IElementMapImplMgr _elementMapMgr;
        private IElementDetailImplMgr _elementDetailMgr;
        private IAreaPactetImplMgr _areaPacketMgr;



        private IProductCategoryImplMgr prodCateMgr;
        //上傳圖片
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.10:2121"
        string ElementPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.elementPath);//圖片保存路徑
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        //end 上傳圖片

        #region View
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AreaIndex()
        {
            return View();
        }
        /// <summary>
        /// 站臺列表視圖
        /// </summary>
        /// <returns></returns>
        public ActionResult SiteIndex()
        {
            return View();
        }
        /// <summary>
        /// 廣告詳情視圖
        /// </summary>
        /// <returns></returns>
        public ActionResult DetailIndex(string packet_id)
        {
            ViewBag.packetId = Convert.ToInt32(Request.Params["packet_id"]); //獲取編輯時傳遞過來的id
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            return View();
        }
        public ActionResult SitePageIndex()
        {
            return View();
        }
        /// <summary>
        /// 元素關係設定列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ElementMapIndex()
        {
            return View();
        }
        /// <summary>
        /// 區域包
        /// </summary>
        /// <returns></returns>
        public ActionResult AreaPacket()
        {
            return View();
        }
        #endregion

        #region 頁面區域管理
        #region PageAreaList 列表頁+HttpResponseBase PageAreaList()
        public HttpResponseBase PageAreaList()
        {
            List<PageAreaQuery> stores = new List<PageAreaQuery>();

            string json = string.Empty;
            try
            {
                PageAreaQuery query = new PageAreaQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request["serchcontent"]))
                {
                    query.serchcontent = Request["serchcontent"];
                }
                if (!string.IsNullOrEmpty(Request["search_type"]))
                {
                    query.element_type = Convert.ToInt32(Request["search_type"]);
                }
                _pageAreaMgr = new PageAreaMgr(mySqlConnectionString);
                int totalCount = 0;
                query.create_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                stores = _pageAreaMgr.QueryAll(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";
                //返回json數據
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

        #region pagearea表添加編輯+HttpResponseBase AreaSave
        public HttpResponseBase AreaSave()
        {
            string json = string.Empty;
            try
            {
                PageAreaQuery ba = new PageAreaQuery();
                _pageAreaMgr = new PageAreaMgr(mySqlConnectionString);
                if (string.IsNullOrEmpty(Request.Params["id"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["name"]))
                    {
                        ba.area_name = Request.Params["name"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["element_type"]))
                    {
                        ba.element_type = Convert.ToInt32(Request.Params["element_type"]);
                    }

                    if (!string.IsNullOrEmpty(Request.Params["area_desc"]))
                    {
                        ba.area_desc = Request.Params["area_desc"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["show_number"]))
                    {
                        ba.show_number = Convert.ToInt32(Request.Params["show_number"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["area_element_id"]))
                    {
                        ba.area_element_id = Request.Params["area_element_id"].ToString();
                    }
                    ba.create_userid = (Session["caller"] as Caller).user_id;
                    ba.update_userid = (Session["caller"] as Caller).user_id;
                    ba.area_updatedate = DateTime.Now;
                    ba.area_createdate = DateTime.Now;
                    IPageAreaImplMgr _ibamgr = new PageAreaMgr(mySqlConnectionString);
                    int i = _ibamgr.AreaSave(ba);
                    if (i > 0)
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    PageAreaQuery oldBa = _pageAreaMgr.GetBannerByAreaId(Convert.ToInt32(Request.Params["id"]));
                    ba.area_id = Convert.ToInt32(Request.Params["id"]);

                    ba.area_name = Request.Params["name"].ToString();

                    if (!string.IsNullOrEmpty(Request.Params["element_type"]))
                    {
                        ba.element_type = Convert.ToInt32(Request.Params["element_type"]);
                    }

                    ba.area_desc = Request.Params["area_desc"].ToString();


                    ba.show_number = Convert.ToInt32(Request.Params["show_number"]);


                    ba.area_element_id = Request.Params["area_element_id"].ToString();

                    ba.update_userid = (Session["caller"] as Caller).user_id;
                    ba.area_updatedate = DateTime.Now;
                    ba.area_status = oldBa.area_status;
                    int j = _pageAreaMgr.Update(ba);
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

        #region 更新表Page_area的狀態 +JsonResult UpdatePageAreaActive
        public JsonResult UpdatePageAreaActive()
        {
            string jsonStr = string.Empty;
            try
            {
                _pageAreaMgr = new PageAreaMgr(mySqlConnectionString);
                int id = Convert.ToInt32(Request.Params["id"]);
                PageArea model = new PageArea();
                model.area_id = id;
                model = _pageAreaMgr.GetModel(model);
                int statusValue = Convert.ToInt32(Request.Params["active"]);

                model.area_id = id;
                model.area_status = statusValue;
                model.update_userid = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                model.area_updatedate = DateTime.Now;

                if (_pageAreaMgr.UpPageAreaStatus(model) > 0)
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
        #endregion

        #region Site站臺頁面管理

        #region 獲取site列表+HttpResponseBase SiteList
        /// <summary>
        /// 獲取Site詳情表頁
        /// </summary>
        /// <returns>json數組列表頁</returns>
        public HttpResponseBase SiteList()
        {
            List<SiteQuery> stores = new List<SiteQuery>();

            string json = string.Empty;
            try
            {
                SiteQuery query = new SiteQuery();
                if (!string.IsNullOrEmpty(Request.Params["serchcontent"]))
                {
                    query.site_name = Request.Params["serchcontent"];
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.create_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                _siteMgr = new SiteMgr(mySqlConnectionString);
                int totalCount = 0;

                stores = _siteMgr.QuerryAll(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";
                //返回json數據

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

        #region 保存站臺信息+HttpResponseBase SaveSiteInfo
        /// <summary>
        /// 保存站臺信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveSiteInfo()
        {
            string json = string.Empty;
            SiteModel query = new SiteModel();

            query.site_name = Request.Params["site_name"];
            query.domain = Request.Params["domain"];
            query.cart_delivery = uint.Parse(Request.Params["cart_delivery"].ToString());
            query.max_user = int.Parse(Request.Params["max_user"].ToString());
            //query.online_user = int.Parse(Request.Params["online_user"].ToString());
            query.page_location = Request.Params["page_location"];
            query.site_status = 0;
            query.update_userid = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());

            try
            {
                if (!string.IsNullOrEmpty(Request.Params["site_id"]))//修改
                {
                    query.site_id = uint.Parse(Request.Params["site_id"].ToString());
                    query.site_updatedate = DateTime.Now;
                    _siteMgr = new SiteMgr(mySqlConnectionString);
                    _siteMgr.UpSite(query);
                }
                else//新增
                {
                    query.site_createdate = DateTime.Now;
                    query.site_updatedate = query.site_createdate;
                    query.create_userid = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    _siteMgr = new SiteMgr(mySqlConnectionString);
                    _siteMgr.InsertSite(query);
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,msg:\"" + "" + "\"}";
                //返回json數據

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + ex.Message + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 修改站臺狀態+JsonResult UpdateSiteState
        /// <summary>
        ///修改站臺狀態
        /// </summary>
        /// <returns></returns>
        public JsonResult UpdateSiteState()
        {
            uint id = Convert.ToUInt32(Request.Params["id"]);
            int activeValue = Convert.ToInt32(Request.Params["active"]);

            SiteModel site = new SiteModel();
            site.site_id = id;
            site.site_status = activeValue;
            site.site_updatedate = DateTime.Now;
            site.update_userid = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
            _siteMgr = new SiteMgr(mySqlConnectionString);
            if (_siteMgr.UpSiteStatus(site) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion
        #endregion

        #region 元素詳細管理

        #region 獲取element詳情表頁 + HttpResponseBase GetDetailList()
        /// <summary>
        /// 獲取element詳情表頁
        /// </summary>
        /// <returns>json數組列表頁</returns>
        [CustomHandleError]
        public HttpResponseBase GetDetailList()
        {
            string json = string.Empty;
            try
            {
                List<ElementDetailQuery> detailStore = new List<ElementDetailQuery>();

                ElementDetailQuery query = new ElementDetailQuery();

                #region 獲取query對象數據
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

                if (!string.IsNullOrEmpty(Request.Params["product_status"]))
                {
                    query.product_status = uint.Parse(Request.Params["product_status"]);
                    query.element_type = 3;
                }
                if (!string.IsNullOrEmpty(Request.Params["packet_id"]))
                {
                    query.packet_id = int.Parse(Request.Params["packet_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["serchcontent"]))
                {
                    query.key = Request.Params["serchcontent"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["searchCate"]))
                {
                    query.searchcate = Request.Params["searchCate"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["search_type"]))
                {
                    query.element_type = int.Parse(Request.Params["search_type"]);
                }

                #endregion

                _detailMgr = new ElementDetailMgr(mySqlConnectionString);
                query.create_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int totalCount = 0;
                detailStore = _detailMgr.QueryAll(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                foreach (var item in detailStore)
                {
                    if (item.element_type == 1)
                    {
                        if (item.element_content != "")
                        {
                            item.element_content = imgServerPath + ElementPath + item.element_content;
                        }
                        if (item.element_img_big != "")
                        {
                            item.element_img_big = imgServerPath + ElementPath + item.element_img_big;
                        }
                        else
                        {
                            item.element_img_big = defaultImg;
                        }
                    }
                    if (item.element_type == 2)
                    {
                        if (item.element_content != "")
                        {
                            item.kendo_editor = Server.HtmlDecode(Server.HtmlDecode(item.element_content));
                        }
                    }
                    if (item.element_type == 3)
                    {
                        if (item.element_img_big != "")
                        {
                            item.element_img_big = imgServerPath + ElementPath + item.element_img_big;
                        }
                        else
                        {
                            item.element_img_big = defaultImg;
                        }
                    }
                    if (item.category_name != "")
                    {
                        item.category_name = Server.HtmlDecode(Server.HtmlDecode(item.category_name));
                      
                    }

                }

                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(detailStore, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 保存ElementDetaiil +HttpResponseBase SaveElementDetaiil()
        public HttpResponseBase SaveElementDetaiil()
        {
            string resultJson = "{success:false}";
            ElementDetail model = new ElementDetail();
            ElementDetail oldModel = new ElementDetail();
            _detailMgr = new ElementDetailMgr(mySqlConnectionString);
            #region 獲取圖片信息
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
            string localBannerPath = imgLocalPath + ElementPath;//圖片存儲地址

            #endregion
            if (!String.IsNullOrEmpty(Request.Params["element_id"]))//如果不存在該id說明是添加頁面
            {
                model.element_id = Convert.ToInt32(Request.Params["element_id"].ToString());
                oldModel = _detailMgr.GetModel(model);
            }
            #region 獲取數據
            int isTranInt = 0;
            //if (Int32.TryParse(Request.Params["element_area_id"].ToString(), out isTranInt))
            //{
            //    model.element_area_id = Convert.ToInt32(Request.Params["element_area_id"].ToString());
            //}
            //else
            //{
            //    model.element_area_id = oldModel.element_area_id;
            //}
            if (!string.IsNullOrEmpty(Request.Params["element_name"].ToString()))
            {
                model.element_name = Request.Params["element_name"].ToString();
            }
            else
            {
                model.element_name = oldModel.element_name;
            }
            if (!string.IsNullOrEmpty(Request.Params["packet_id"]))
            {
                model.packet_id = Convert.ToInt32(Request.Params["packet_id"]);
            }
            if (Int32.TryParse(Request.Params["element_type"].ToString(), out isTranInt))
            {
                model.element_type = Convert.ToInt32(Request.Params["element_type"]);
            }
            if (model.element_type == 1)
            {
                #region 上傳圖片
                try
                {
                    FileManagement fileLoad = new FileManagement();

                    //if (Request.Files.Count > 0)//單個圖片上傳
                    for (int iFile = 0; iFile < Request.Files.Count; iFile++)//多個上傳圖片
                    {
                        HttpPostedFileBase file = Request.Files[iFile];//單個Request.Files[0]
                        string fileName = string.Empty;//當前文件名
                        string fileExtention = string.Empty;//當前文件的擴展名
                        //獲取圖片名稱
                        fileName = fileLoad.NewFileName(file.FileName);
                        if (iFile == 0 && fileName == oldModel.element_content)
                        {
                            fileName = "";
                        }
                        if(iFile == 1 && string.IsNullOrEmpty(Request.Params["element_img_big"].ToString()) )
                        {
                            fileName = Request.Params["element_img_big"].ToString();
                        }
                        if (!String.IsNullOrEmpty(fileName))
                        {
                            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                            fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();
                            string NewFileName = string.Empty;
                            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                            NewFileName = hash.Md5Encrypt(fileName, "32");
                            string ServerPath = string.Empty;
                            //判斷目錄是否存在，不存在則創建
                            FTP f_cf = new FTP();
                            f_cf.MakeMultiDirectory(localBannerPath.Substring(0, localBannerPath.Length - ElementPath.Length + 1), ElementPath.Substring(1, ElementPath.Length - 2).Split('/'), ftpuser, ftppwd);

                            fileName = NewFileName + fileExtention;
                            NewFileName = localBannerPath + NewFileName + fileExtention;//絕對路徑
                            ServerPath = Server.MapPath(imgLocalServerPath + ElementPath);
                            string ErrorMsg = string.Empty;

                            //上傳之前刪除已有的圖片
                            if (model.element_id != 0)
                            {
                                string oldFileName = oldModel.element_content;
                                if (iFile == 1)
                                {
                                    oldFileName = oldModel.element_img_big;
                                }
                                CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                FTP ftp = new FTP(localBannerPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldFileName))
                                {
                                    FTP ftps = new FTP(localBannerPath + oldFileName, ftpuser, ftppwd);
                                    ftps.DeleteFile(localBannerPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                }
                            }
                            try
                            {
                                //上傳
                                Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                                bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                if (result)//上傳成功
                                {
                                    if (iFile == 0)
                                    {
                                        model.element_content = fileName;
                                    }
                                    else 
                                    {
                                        model.element_img_big = fileName; 
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                log.Error(logMessage);
                                model.element_content = oldModel.element_content;
                            }
                            if (!string.IsNullOrEmpty(ErrorMsg))
                            {
                                string json = string.Empty;
                                json = "{success:true,msg:\"" + ErrorMsg + "\"}";
                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }
                        }
                        else
                        {
                            if (iFile == 0)
                            {
                                model.element_content = oldModel.element_content;
                            }
                            else 
                            {
                                if (Request.Params["element_img_big"].ToString() == "")
                                {//編輯時如果傳過來空值則直接刪除
                                    model.element_img_big = "";
                                }
                                else
                                {
                                    model.element_img_big = oldModel.element_img_big;
                                }
                                //model.element_img_big = oldModel.element_img_big;
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
                    model.element_content = oldModel.element_content;
                }
                #endregion
            }
            else if (model.element_type == 2)
            {
                if (!string.IsNullOrEmpty(Request.Params["element_content"].ToString()))
                {
                    model.element_content = Request.Params["element_content"].ToString();
                }
                else
                {
                    model.element_content = oldModel.element_content;
                }
            }
            else if (model.element_type == 3)
            {
                if (int.TryParse(Request.Params["element_product_id"].ToString(), out isTranInt))
                {
                    model.element_content = Request.Params["element_product_id"].ToString();
                }
                else
                {
                    model.element_content = oldModel.element_content;
                }
                #region 上傳圖片
                try
                {
                    FileManagement fileLoad = new FileManagement();
                    //if (Request.Files.Count > 0)//單個圖片上傳
                    for (int iFile = 1; iFile < Request.Files.Count; iFile++)//多個上傳圖片
                    {
                        HttpPostedFileBase file = Request.Files[iFile];//單個Request.Files[0]
                        string fileName = string.Empty;//當前文件名
                        string fileExtention = string.Empty;//當前文件的擴展名
                        //獲取圖片名稱
                        fileName = fileLoad.NewFileName(file.FileName);
                        if (!String.IsNullOrEmpty(fileName) && !String.IsNullOrEmpty(Request.Params["element_img_big"].ToString()))
                        {//可獲取文件,名稱不為空則變更圖片
                            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                            fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();
                            string NewFileName = string.Empty;
                            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                            NewFileName = hash.Md5Encrypt(fileName, "32");
                            string ServerPath = string.Empty;
                            //判斷目錄是否存在，不存在則創建
                            FTP f_cf = new FTP();
                            f_cf.MakeMultiDirectory(localBannerPath.Substring(0, localBannerPath.Length - ElementPath.Length + 1), ElementPath.Substring(1, ElementPath.Length - 2).Split('/'), ftpuser, ftppwd);

                            fileName = NewFileName + fileExtention;
                            NewFileName = localBannerPath + NewFileName + fileExtention;//絕對路徑
                            ServerPath = Server.MapPath(imgLocalServerPath + ElementPath);
                            string ErrorMsg = string.Empty;

                            //上傳之前刪除已有的圖片
                            if (model.element_id != 0)
                            {
                                string oldFileName = oldModel.element_img_big;
                                CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                                FTP ftp = new FTP(localBannerPath, ftpuser, ftppwd);
                                List<string> tem = ftp.GetFileList();
                                if (tem.Contains(oldFileName))
                                {
                                    FTP ftps = new FTP(localBannerPath + oldFileName, ftpuser, ftppwd);
                                    ftps.DeleteFile(localBannerPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                }
                            }
                            try
                            {
                                //上傳
                                Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                                bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                if (result)//上傳成功
                                {
                                    model.element_img_big = fileName;                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                log.Error(logMessage);
                                model.element_img_big = oldModel.element_img_big;
                            }
                            if (!string.IsNullOrEmpty(ErrorMsg))
                            {
                                string json = string.Empty;
                                json = "{success:true,msg:\"" + ErrorMsg + "\"}";
                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }
                        }
                        else
                        {
                            if (Request.Params["element_img_big"].ToString() == "")
                            {//編輯時如果傳過來空值則直接刪除
                                model.element_img_big = "";
                            }
                            else
                            {
                                model.element_img_big = oldModel.element_img_big;
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
                    model.element_img_big = oldModel.element_img_big;
                }
                #endregion
            }
            if (int.TryParse(Request.Params["category_id_s"].ToString(), out isTranInt))
            {
                model.category_id = uint.Parse(Request.Params["category_id_s"].ToString());
            }
            else
            {
                model.category_id = 0;
            }

            model.category_name = Server.HtmlDecode(Request.Params["category_name_s"].ToString());


            if (!string.IsNullOrEmpty(Request.Params["element_link_url"].ToString()))
            {
                model.element_link_url = Request.Params["element_link_url"].ToString();
            }
            //else
            //{
            //    model.element_link_url = oldModel.element_link_url;
            //}
            if (int.TryParse(Request.Params["element_link_mode"].ToString(), out isTranInt))
            {
                model.element_link_mode = Convert.ToInt32(Request.Params["element_link_mode"]);
            }
            else
            {
                model.element_link_mode = oldModel.element_link_mode;
            }
            if (!string.IsNullOrEmpty(Request.Params["element_sort"].ToString()))
            {
                model.element_sort = Convert.ToInt32(Request.Params["element_sort"].ToString());
            }
            //else
            //{
            //    model.element_sort = oldModel.element_sort;
            //}

            if (!string.IsNullOrEmpty(Request.Params["element_start"].ToString()))
            {
                model.element_start = Convert.ToDateTime(Request.Params["element_start"].ToString());
            }
            else
            {
                model.element_start = oldModel.element_start;
            }
            if (!string.IsNullOrEmpty(Request.Params["element_end"].ToString()))
            {
                model.element_end = Convert.ToDateTime(Request.Params["element_end"].ToString());
            }
            else
            {
                model.element_end = oldModel.element_end;
            }

            model.element_remark = (Request.Params["element_remark"].ToString());

            model.element_status = 0;//默認為不啟用
            #endregion

            try
            {
                //判斷是否能夠獲取到rowid
                if (String.IsNullOrEmpty(Request.Params["element_id"]))//如果不存在該id說明是添加頁面
                {
                    model.element_createdate = DateTime.Now;
                    model.element_updatedate = model.element_createdate;
                    model.create_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    model.update_userid = model.create_userid;
                    //這裡加上各種參數

                    if (_detailMgr.Save(model) > 0)
                    {
                        resultJson = "{success:true}";//返回json數據
                    }
                }
                else
                {
                    //model.element_createdate = oldModel.element_createdate;
                    model.element_updatedate = DateTime.Now;
                    model.update_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    //    model.create_userid = oldModel.create_userid;
                    //這裡加上各種參數
                    if (_detailMgr.Update(model) > 0)//如果可以獲取到rowid則進行修改
                    {
                        resultJson = "{success:true}";//返回json數據
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                resultJson = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(resultJson);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 更改活動使用狀態 + HttpResponseBase UpdateDetailStatus()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateDetailStatus()
        {
            string jsonStr = string.Empty;
            try
            {
                _detailMgr = new ElementDetailMgr(mySqlConnectionString);
                int id = Convert.ToInt32(Request.Params["id"]);
                int statusValue = Convert.ToInt32(Request.Params["status"]);
                ElementDetailQuery model = new ElementDetailQuery();
                model.element_id = id;
                model.element_status = statusValue;
                model.update_userid = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                model.element_updatedate = DateTime.Now;

                if (_detailMgr.UpdateStatus(model) > 0)
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

        #region 元素包
        public HttpResponseBase GetPacket()
        {
            string json = string.Empty;
            List<AreaPacket> store = new List<AreaPacket>();
            try
            {
                _areaPacketMgr = new AreaPacketMgr(mySqlConnectionString);

                if (!string.IsNullOrEmpty(Request.Params["ele_type"]))
                {
                    store = _areaPacketMgr.GetPacket(int.Parse(Request.Params["ele_type"].ToString()));
                }
                else
                {
                    store = _areaPacketMgr.GetPacket(0);
                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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
        public HttpResponseBase GetId()
        {
            string json = string.Empty;
            //if (!string.IsNullOrEmpty(Request.Params["PacketId"]))
            //{
            _areaPacketMgr = new AreaPacketMgr(mySqlConnectionString);
            int packtId = Convert.ToInt32(Request.Params["PacketId"].ToString());
            try
            {
                if (_areaPacketMgr.SelectCount(packtId))
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
            //  }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 根據信息包的id獲取其element_type
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetElementTypeInPacket()
        {
            string json = string.Empty;
            int isTryInt = 0;
            int totalCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["PacketId"].ToString()))
                {
                    if (int.TryParse(Request.Params["PacketId"].ToString(), out isTryInt))
                    {
                        _areaPacketMgr = new AreaPacketMgr(mySqlConnectionString);
                        AreaPacket store = _areaPacketMgr.QueryAll(new AreaPacket { packet_id = Convert.ToInt32(Request.Params["PacketId"].ToString()) }, out totalCount).FirstOrDefault();
                        json = "{success:true,elementType:" + store.element_type + "}";
                    }
                    else
                    {
                        json = "{success:false,elementType:-1}";
                    }

                }
                else
                {
                    json = "{success:false,elementType:-1}";
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
        public HttpResponseBase GetProductInfo()
        {
            string resultJson = "{success:false}";
            _productMgr = new ProductMgr(mySqlConnectionString);

            int isTranInt = 0;
            if (int.TryParse(Request.Params["Product_id"].ToString(), out isTranInt))
            {
                List<QueryandVerifyCustom> pros = _productMgr.GetProductInfoByID(Request.Params["Product_id"].ToString());
                if (pros.Count > 0)
                {
                    int packet_id = Convert.ToInt32(Request.Params["Packet_id"].ToString());
                    _elementDetailMgr = new ElementDetailMgr(mySqlConnectionString);
                    List<ElementDetailQuery> store = _elementDetailMgr.QueryPacketProd(new ElementDetail { packet_id = packet_id, element_content = isTranInt.ToString() });
                    if (store.Count != 0)
                    {
                        bool repeat = true;
                        if (!string.IsNullOrEmpty(Request.Params["Element_id"].ToString()))
                        {
                            int element_id = Convert.ToInt32(Request.Params["Element_id"].ToString());
                            foreach (var item in store)
                            {
                                if (item.element_id == element_id)
                                {
                                    repeat = false;
                                    resultJson = "{success:true,msg:0}";
                                    break;
                                }
                            }
                            if (repeat)
                            {
                                resultJson = "{success:true,msg:1}";
                            }
                        }
                        else
                        {
                            resultJson = "{success:true,msg:1}";
                        }
                    }
                    else
                    {
                        resultJson = "{success:true,msg:0}";
                    }
                }
            }
            this.Response.Clear();
            this.Response.Write(resultJson);
            this.Response.End();
            return this.Response;
        }

        #region 刪除
        public HttpResponseBase DeleteElementDetail()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["rowIDs"]))
                {
                    _elementDetailMgr = new ElementDetailMgr(mySqlConnectionString);
                    string rowIDs = Request.Params["rowIDs"].Trim('|');
                    string[] newRowID = rowIDs.Split('|');
                    if (_elementDetailMgr.DeleteElementDetail(newRowID))
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
        #region 獲取元素詳情列表中類別中的所有商品，包含父節點及其下面的子節點
        public HttpResponseBase GetProductByCategorySet()
        {
            string resultStr = "{success:false}";
            string str = string.Empty;
            try
            {
                _productMgr = new ProductMgr(mySqlConnectionString);
                ProductQuery query = new ProductQuery();
                query.isjoincate = true;
                query.IsPage = true;
                query.Start = int.Parse(Request.Form["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"].ToString());
                }
                else
                {
                    query.Limit = 500;
                }
                uint isTryUint = 0;
                if (uint.TryParse(Request.Params["status"].ToString(), out isTryUint))
                {
                    query.Product_Status = Convert.ToUInt32(Request.Params["status"].ToString());
                }
                if (uint.TryParse(Request.Params["brand_id"].ToString(), out isTryUint))
                {
                    query.Brand_Id = Convert.ToUInt32(Request.Params["brand_id"].ToString());
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["class_id"]))
                    {
                        VendorBrandSetMgr vbsMgr = new VendorBrandSetMgr(mySqlConnectionString);
                        VendorBrandSet vbs = new VendorBrandSet();
                        vbs.class_id = Convert.ToUInt32(Request.Form["class_id"]);
                        List<VendorBrandSet> vbsList = vbsMgr.Query(vbs);
                        foreach (VendorBrandSet item in vbsList)
                        {
                            query.brandArry += item.brand_id;
                            query.brandArry += ",";
                        }
                        query.brandArry = query.brandArry.Substring(0, query.brandArry.Length - 1);
                    }

                }
                if (!string.IsNullOrEmpty(Request.Params["keyCode"].ToString()))
                {
                    if (uint.TryParse(Request.Params["keyCode"].ToString(), out isTryUint))
                    {
                        query.Product_Id = Convert.ToUInt32(Request.Params["keyCode"].ToString());
                    }
                    else
                    {
                        query.Product_Name = Request.Params["keyCode"].ToString();
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["category_id"].ToString()))
                {
                    if (uint.TryParse(Request.Params["category_id"].ToString(), out isTryUint))
                    {
                        //判斷是否是父節點，若是則獲取所有的category_id
                        prodCateMgr = new ProductCategoryMgr(mySqlConnectionString);
                        List<ProductCategory> category = prodCateMgr.QueryAll(new ProductCategory());
                        GetAllCategory_id(category, Convert.ToUInt32(Request.Params["category_id"].ToString()), ref str);
                        query.categoryArry = str;
                    }
                }
                int totalCount = 0;
                List<ProductDetailsCustom> prods = _productMgr.GetAllProList(query, out totalCount);
                resultStr = "{succes:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(prods) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                resultStr = "{succes:false,totalCount:0,item:[]}";
            }


            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        public HttpResponseBase Getpic()
        {//判斷該文件是否超過限制
            string Json = "";
            FileManagement fileLoad = new FileManagement();

            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_Min_Element");
            SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
            //擴展名、最小值、最大值
            string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
            string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
            //if (Request.Files.Count > 0)//單個圖片上傳
            for (int iFile = 0; iFile < Request.Files.Count; iFile++)//多個上傳圖片
            {
                HttpPostedFileBase file = Request.Files[iFile];//單個Request.Files[0]
                int fileSize = file.ContentLength;
                if (fileSize > int.Parse(minValue) && fileSize < int.Parse(maxValue))
                {
                    Json = "{success:true}";
                }
                else
                {
                    Json = "{success:false}";
                }
            }
            this.Response.Clear();
            this.Response.Write(Json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 站台頁面管理
        #region 頁面列表頁 +HttpResponseBase SitePageList()
        public HttpResponseBase SitePageList()
        {
            List<SitePageQuery> store = new List<SitePageQuery>();
            string json = string.Empty;
            SitePageQuery query = new SitePageQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                //if (!string.IsNullOrEmpty(Request.Params["site"]))
                //{
                //    query.site_name = Request.Params["site"];
                //}
                //else
                //{
                //    query.site_id = 0;
                //}
                if (!string.IsNullOrEmpty(Request.Params["pagename"]))
                {
                    query.page_name = Request.Params["pagename"];
                }
                else
                {
                    query.page_name = string.Empty;
                }

                _sitePageMg = new SitePageMgr(mySqlConnectionString);
                int totalCount = 0;
                query.create_userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                store = _sitePageMg.GetSitePageList(query, out totalCount);
                foreach (var item in store)
                {

                    if (item.page_html.Length >= 15)
                    {
                        item.page_shortHtml = item.page_html.Substring(0, 15) + "...";
                    }
                    else
                    {
                        item.page_shortHtml = item.page_html;
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
                json = "{success:false,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region  更改狀態 + UpdateSitePageStatus
        public JsonResult UpdateSitePageStatus()
        {
            string json = string.Empty;
            try
            {
                SitePageQuery query = new SitePageQuery();
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.page_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.page_status = Convert.ToInt32(Request.Params["active"]);
                }
                query.page_updatedate = DateTime.Now;
                query.update_userid = (Session["caller"] as Caller).user_id;
                _sitePageMg = new SitePageMgr(mySqlConnectionString);
                if (_sitePageMg.UpdateStatus(query) > 0)
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
        #region 頁面新增和修改+HttpResponseBase SaveSitePage()
        public HttpResponseBase SaveSitePage()
        {
            string json = string.Empty;
            SitePageQuery query = new SitePageQuery();
            _sitePageMg = new SitePageMgr(mySqlConnectionString);
            try
            {
                if (string.IsNullOrEmpty(Request.Params["page_id"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["page_name"]))
                    {
                        query.page_name = Request.Params["page_name"];
                    }
                    else
                    {
                        query.page_name = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["page_url"]))
                    {
                        query.page_url = Request.Params["page_url"];
                    }
                    else
                    {
                        query.page_url = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["page_html"]))
                    {
                        query.page_html = Request.Params["page_html"];

                    }
                    else
                    {
                        query.page_html = string.Empty;
                    }

                    query.page_desc = Request.Params["page_desc"];

                    query.page_status = 0;
                    query.page_createdate = DateTime.Now;
                    query.page_updatedate = DateTime.Now;
                    query.create_userid = (Session["caller"] as Caller).user_id;
                    query.update_userid = (Session["caller"] as Caller).user_id;
                    _sitePageMg.Save(query);
                    json = "{success:true}";
                }
                else
                {
                    query.page_id = Convert.ToInt32(Request.Params["page_id"]);
                    query.page_name = Request.Params["page_name"];
                    query.page_url = Request.Params["page_url"];
                    query.page_html = Request.Params["page_html"];
                    query.page_desc = Request.Params["page_desc"];
                    query.page_status = 0;
                    query.page_updatedate = DateTime.Now;
                    query.update_userid = (Session["caller"] as Caller).user_id;
                    _sitePageMg.Update(query);
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

        #region 獲得網頁下拉框 +HttpResponseBase GetPage()
        public HttpResponseBase GetPage()
        {
            List<SitePage> store = new List<SitePage>();
            string json = string.Empty;
            try
            {
                SitePage bp = new SitePage();

                _sitePageMg = new SitePageMgr(mySqlConnectionString);
                store = _sitePageMg.GetPage(bp);

                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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

        #region 獲得區域 下拉框+string GetArea()
        public HttpResponseBase GetArea()
        {
            string json = string.Empty;
            try
            {
                _pageAreaMgr = new PageAreaMgr(mySqlConnectionString);

                List<PageArea> areastore = _pageAreaMgr.GetArea();
                json = "{success:true,data:" + JsonConvert.SerializeObject(areastore, Formatting.Indented) + "}";
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

        #region 元素下拉框
        public HttpResponseBase GetElement()
        {
            string json = string.Empty;
            try
            {
                _elementDetailMgr = new ElementDetailMgr(mySqlConnectionString);

                List<ElementDetail> elementstore = _elementDetailMgr.QueryElementDetail();
                json = "{success:true,data:" + JsonConvert.SerializeObject(elementstore, Formatting.Indented) + "}";
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


        #region 獲取所有站點+ HttpResponseBase GetSite
        public HttpResponseBase GetSite()
        {
            List<Site> store = new List<Site>();
            string json = string.Empty;
            try
            {
                _siteMgr = new SiteMgr(mySqlConnectionString);
                Site site = new Site();
                site.Create_Userid = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                store = _siteMgr.GetSite(site);

                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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

        #region 元素關係列表頁
        public HttpResponseBase ElementMapList()
        {
            List<ElementMapQuery> store = new List<ElementMapQuery>();
            string json = string.Empty;
            ElementMapQuery query = new ElementMapQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["siteName"]))
                {
                    query.site_name = Request.Params["siteName"];
                }
                if (!string.IsNullOrEmpty(Request.Params["searchType"]))
                {
                    query.element_type = Convert.ToInt32(Request.Params["searchType"]);
                }
                _elementMapMgr = new ElementMapMgr(mySqlConnectionString);
                int totalCount = 0;

                store = _elementMapMgr.GetElementMapList(query, out totalCount);
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

        #region 元素關係保存+SaveElementMap()
        /// <summary>
        /// 元素關係保存
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveElementMap()
        {
            string json = string.Empty;
            try
            {
                ElementMapQuery emQuery = new ElementMapQuery();
                if (!string.IsNullOrEmpty(Request.Params["map_id"]))//修改
                {
                    emQuery.map_id = Convert.ToInt32(Request.Params["map_id"]);
                    if (!string.IsNullOrEmpty(Request.Params["site_id"]))
                    {
                        emQuery.site_id = Convert.ToInt32(Request.Params["site_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["page_id"]))
                    {
                        emQuery.page_id = Convert.ToInt32(Request.Params["page_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["area_id"]))
                    {
                        emQuery.area_id = Convert.ToInt32(Request.Params["area_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["packet_id"]))
                    {
                        emQuery.packet_id = Convert.ToInt32(Request.Params["packet_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["sort"]))
                    {
                        emQuery.sort = Convert.ToInt32(Request.Params["sort"]);
                    }
                    emQuery.update_date = DateTime.Now;
                    emQuery.update_user_id = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    _elementMapMgr = new ElementMapMgr(mySqlConnectionString);
                    if (_elementMapMgr.SelectElementMap(emQuery))
                    {
                        _elementMapMgr.upElementMap(emQuery);
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }

                }
                else //新增
                {
                    if (!string.IsNullOrEmpty(Request.Params["site_id"]))
                    {
                        emQuery.site_id = Convert.ToInt32(Request.Params["site_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["page_id"]))
                    {
                        emQuery.page_id = Convert.ToInt32(Request.Params["page_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["area_id"]))
                    {
                        emQuery.area_id = Convert.ToInt32(Request.Params["area_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["packet_id"]))
                    {
                        emQuery.packet_id = Convert.ToInt32(Request.Params["packet_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["sort"]))
                    {
                        emQuery.sort = Convert.ToInt32(Request.Params["sort"]);
                    }
                    emQuery.create_date = DateTime.Now;
                    emQuery.update_date = emQuery.create_date;
                    emQuery.create_user_id = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    emQuery.update_user_id = emQuery.create_user_id;
                    _elementMapMgr = new ElementMapMgr(mySqlConnectionString);
                    if (_elementMapMgr.SelectElementMap(emQuery))
                    {
                        _elementMapMgr.AddElementMap(emQuery);
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

        #region 獲取ElementType名稱+HttpResponseBase GetElementType()
        /// <summary>
        /// 獲取ElementType名稱
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetElementType()
        {
            List<Parametersrc> store = new List<Parametersrc>();
            string json = string.Empty;
            try
            {
                _parameterMgr = new ParameterMgr(mySqlConnectionString);
                store = _parameterMgr.GetElementType("element_type");

                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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

        #region 獲取區域數量
        public HttpResponseBase GetAreaCount()
        {
            string json = string.Empty;
            try
            {
                int areaId = Convert.ToInt32(Request.Params["AreaId"].ToString());
                _elementMapMgr = new ElementMapMgr(mySqlConnectionString);
                if (_elementMapMgr.GetAreaCount(areaId))
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

        #region 區域包頁面管理
        #region 區域包列表+HttpResponseBase AreaPacketList()
        /// <summary>
        /// 區域包列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AreaPacketList()
        {
            List<AreaPacket> stores = new List<AreaPacket>();

            string json = string.Empty;
            try
            {
                AreaPacket query = new AreaPacket();

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["serchcontent"]))
                {
                    query.packet_name = Request.Params["serchcontent"].ToString();
                    query.packet_desc = Request.Params["serchcontent"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["serchtype"]))
                {
                    query.element_type = Convert.ToInt32(Request.Params["serchtype"].ToString());

                }
                _areaPacketMgr = new AreaPacketMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _areaPacketMgr.QueryAll(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";
                //返回json數據

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

        #region 新增區域包信息+HttpResponseBase AreaPactetSave()
        /// <summary>
        /// 新增編輯區域包信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AreaPactetSave()
        {
            string json = string.Empty;
            AreaPacket ap = new AreaPacket();
            IAreaPactetImplMgr _iareaPacketMgr = new AreaPacketMgr(mySqlConnectionString);
            try
            {

                if (string.IsNullOrEmpty(Request.Params["id"]))
                {
                    if (!string.IsNullOrEmpty(Request.Params["name"]))
                    {
                        ap.packet_name = Request.Params["name"].ToString();
                    }

                    if (!string.IsNullOrEmpty(Request.Params["element_type"]))
                    {
                        ap.element_type = Convert.ToInt32(Request.Params["element_type"]);
                    }

                    if (!string.IsNullOrEmpty(Request.Params["packet_desc"]))
                    {
                        ap.packet_desc = Request.Params["packet_desc"].ToString();
                    }
                    if (!string.IsNullOrEmpty(Request.Params["show_number"]))
                    {
                        ap.show_number = Convert.ToInt32(Request.Params["show_number"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["packet_sort"]))
                    {
                        ap.packet_sort = Convert.ToInt32(Request.Params["packet_sort"].ToString());
                    }
                    ap.create_userid = (Session["caller"] as Caller).user_id;
                    ap.update_userid = (Session["caller"] as Caller).user_id;
                    ap.packet_createdate = DateTime.Now;
                    ap.packet_updatedate = DateTime.Now;
                    int i = _iareaPacketMgr.AreaPacketSave(ap);
                    if (i > 0)
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    AreaPacket areaPacketTemp = new AreaPacket();
                    areaPacketTemp.packet_id = Convert.ToInt32(Request.Params["id"]);
                    AreaPacket oldap = _iareaPacketMgr.GetModelById(areaPacketTemp);
                    ap.packet_id = Convert.ToInt32(Request.Params["id"]);

                    ap.packet_name = Request.Params["name"].ToString();

                    if (!string.IsNullOrEmpty(Request.Params["element_type"]))
                    {
                        ap.element_type = Convert.ToInt32(Request.Params["element_type"]);
                    }
                    else
                    {
                        ap.element_type = oldap.element_type;
                    }

                    ap.packet_desc = Request.Params["packet_desc"].ToString();

                    ap.show_number = Convert.ToInt32(Request.Params["show_number"]);


                    ap.packet_sort = Convert.ToInt32(Request.Params["packet_sort"].ToString());

                    ap.update_userid = (Session["caller"] as Caller).user_id;
                    ap.packet_updatedate = DateTime.Now;
                    ap.packet_status = oldap.packet_status;
                    int j = _iareaPacketMgr.AreaPacketSave(ap);
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

        #region 更新表area_packet的狀態 +JsonResult UpdateAreaPacketActive
        public JsonResult UpdateAreaPacketActive()
        {
            string jsonStr = string.Empty;
            try
            {
                _areaPacketMgr = new AreaPacketMgr(mySqlConnectionString);
                int id = Convert.ToInt32(Request.Params["id"]);
                AreaPacket model = new AreaPacket();
                model.packet_id = id;
                model = _areaPacketMgr.GetModelById(model);
                int statusValue = Convert.ToInt32(Request.Params["active"]);

                model.packet_id = id;
                model.packet_status = statusValue;
                model.update_userid = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                model.packet_updatedate = DateTime.Now;

                if (_areaPacketMgr.UpAreaPacketStatus(model) > 0)
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

        #endregion


        #region 遞歸查詢子cateID
        /// <summary>
        /// 遞歸查詢子ID //edit by hjiajun1211w 2014/08/08 父商品查詢
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
    }

}
