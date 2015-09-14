using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using System.Configuration;
using Newtonsoft.Json;
using Admin.gigade.CustomError;
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Collections;
using System.Web.Security;
using System.Reflection;
using System.Text.RegularExpressions;
//using BLL.gigade.Model.Temp;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class ProductComboController : Controller
    {
        //
        // GET: /ProductCombo/

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IProductImplMgr _productMgr;
        private IProductTempImplMgr _productTempMgr;
        private IPriceMasterTempImplMgr _pMasterTempMgr;
        private IProductCategoryImplMgr _procateMgr;
        private IProductCategorySetImplMgr _categorySetMgr;
        private IProductCategorySetTempImplMgr _categoryTempSetMgr;
        private IProductSpecImplMgr _specMgr;
        private IProductSpecTempImplMgr _specTempMgr;
        private IProductItemImplMgr _productItemMgr;
        private IProductPictureTempImplMgr _pPicTempMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private IPriceMasterTsImplMgr _priceMasterTsMgr;
        private IItemPriceImplMgr _itemPriceMgr;
        private IItemPriceTsImplMgr _itemPriceTsMgr;
        private IProductTagImplMgr _productTagMgr;
        private IProductTagSetImplMgr _productTagSetMgr;
        private IProductTagSetTempImplMgr _productTagSetTempMgr;
        private IProductNoticeImplMgr _productNoticeMgr;
        private IProductNoticeSetImplMgr _productNoticeSetMgr;
        private IProductNoticeSetTempImplMgr _productNoticeSetTempMgr;
        private IProductPictureImplMgr _productPicMgr;
        private ITableHistoryImplMgr _tableHistoryMgr;
        private IUsersImplMgr _usersMgr;
        private VendorBrandMgr vbMgr;
        private ISiteImplMgr _siteMgr;
        private ParameterMgr paraMgr;
        private ISiteConfigImplMgr siteConfigMgr;
        private int COMBO_TYPE = 2;
        private string resultStr = string.Empty;
        private IProductComboImplMgr _combMgr;
        private IProductComboTempImplMgr _combTempMgr;
        private IPriceUpdateApplyImplMgr _priceUpdateApplyMgr;
        private IPriceUpdateApplyHistoryImplMgr _priceUpdateApplyHistoryMgr;
        private IFunctionImplMgr _functionMgr;
        private IVendorBrandImplMgr _vendorBrandMgr;
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];

        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);

        string prodPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prodPath);
        string prod50Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod50Path);
        string prod150Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod150Path);
        string prod280Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod280Path);

        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.specPath);
        string spec100Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.spec100Path);
        string spec280Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.spec280Path);


        string descPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descPath);
        string desc400Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.desc400Path);

        string descMobilePath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descMobilePath);
        string desc400MobilePath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.desc400MobilePath);

        string tagPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_tagPath);

        string prodMobile640 = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod_mobile640); //add by wwei0216w 2015/4/1 添加原因:手機圖片要放在640*640路徑下

        //string noticePath = ConfigurationManager.AppSettings["prod_noticePath"];
        //string notice400Path = ConfigurationManager.AppSettings["prod_notice400Path"];
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];

        //private int defaultImgLength = 5;
        private int imgNameIdx = 7;    //按‘/’分割第 n 个为图片名称

        #region View

        public ActionResult Index(string id)
        {
            ViewBag.ProductId = id;
            ViewBag.OldProductId = Request.QueryString["product_id"] ?? "";
            ViewBag.hfAuth = Request.Cookies[FormsAuthentication.FormsCookieName] == null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value;
            ViewBag.hfAspSessID = Session.SessionID;
            return View();
        }
        public ActionResult productStock()
        {
            return View();
        }

        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public ActionResult baseInfo()
        {
            Parametersrc para = new Parametersrc();
            string[] paraList = new string[2] { "", "" };
            //加載運費方式
            para.ParameterType = "product_freight";
            paraList[0] = QueryParameter(para);
            //加載出貨方式
            para.ParameterType = "product_mode";
            paraList[1] = QueryParameter(para);
            return View(paraList);
        }

        /// <summary>
        /// 添加物流模式設定
        /// </summary>
        /// <returns></returns>
        public ActionResult transportSet()
        {
            return View();
        }

        public ActionResult Price()
        {
            return View();
        }


        public ActionResult Spec()
        {
            return View();
        }

        public ActionResult Fortune()
        {
            return View();
        }

        public ActionResult Category()
        {
            return View();
        }

        /// <summary>
        /// 新類別 2014/12/25
        /// </summary>
        /// <returns></returns>
        public ActionResult NewCategory()
        {
            return View();
        }


        public ActionResult Description()
        {
            return View();
        }

        public ActionResult ProductPic()
        {

            return View();
        }
        public ActionResult upLoad()
        {
            return View();
        }

        #endregion


        #region Action

        #region  圖檔

        public ActionResult upLoadImg()
        {
            HttpPostedFileBase file = Request.Files["Filedata"];
            int type = Request["appOrexplain"] == null ? 0 : Convert.ToInt32(Request["appOrexplain"]);
            string nameType = Request.Params["nameType"];// 將 nametype 提前 使其判斷傳入的圖片是否為商品主圖 edit by zhuoqin0830w 2015/01/29
            int prodCheck = file.FileName.LastIndexOf("prod_");// 將 proCheck 提前 使其判斷批量上傳的圖片中是否存在商品主圖   edit by zhuoqin0830w 2015/01/30
            if (prodCheck == 0 && type == 0)
            {
                type = 3;
            }
            int mobileCheck = file.FileName.LastIndexOf("mobile_");
            if (mobileCheck == 0 && type == 0)
            {
                type = 4;
            }
            string path = Server.MapPath(xmlPath);
            siteConfigMgr = new SiteConfigMgr(path);
            ViewBag.moreFileOneTime = false;
            SiteConfig extention_config = siteConfigMgr.GetConfigByName("PIC_Extention_Format");
            SiteConfig minValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
            SiteConfig maxValue_config = null;
            //判斷 批量上傳 或 單個上傳 的圖片是否為 商品主圖 或 手機商品圖  edit by zhuoqin0830w 2015/03/24
            switch (nameType)
            {
                //如果  nameType == prod  則表示 是 單個上傳 商品主圖
                case "prod":
                    maxValue_config = siteConfigMgr.GetConfigByName("PIC_280_Length_Max");
                    break;
                //如果  nameType == mobile  則表示 是 單個上傳 手機商品圖 
                case "mobile":
                    maxValue_config = siteConfigMgr.GetConfigByName("PIC_640_Length_Max");
                    break;
                //如果  nameType == null  則表示 是 批量上傳
                case null:
                    //如果  prodCheck == 0  則表示 是 單個上傳 商品主圖 
                    if (prodCheck == 0)
                    {
                        maxValue_config = siteConfigMgr.GetConfigByName("PIC_280_Length_Max");
                    }
                    //如果  mobileCheck == 0  則表示 是 單個上傳 手機商品圖 
                    else if (mobileCheck == 0)
                    {
                        maxValue_config = siteConfigMgr.GetConfigByName("PIC_640_Length_Max");
                    }
                    else
                    {
                        maxValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                    }
                    break;
                default:
                    maxValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
                    break;
            }

            SiteConfig admin_userName = siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
            SiteConfig admin_passwd = siteConfigMgr.GetConfigByName("ADMIN_PASSWD");

            string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
            string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
            string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;

            if ((nameType == "mobile" && type == 4) || (mobileCheck == 0 && type == 4)) //add by wwei0216w 2015/4/1 添加原因:手機圖片要放在640*640路徑下
            {
                prodPath = prodMobile640;
            }

            string localProdPath = imgLocalPath + prodPath;
            //string localProd50Path = imgLocalPath + prod50Path;
            //string localProd150Path = imgLocalPath + prod150Path;
            //string localProd280Path = imgLocalPath + prod280Path;

            string localSpecPath = imgLocalPath + specPath;
            //string localSpec100Path = imgLocalPath + spec100Path;
            //string localSpec280Path = imgLocalPath + spec280Path;

            string[] Mappath = new string[2];

            FileManagement fileLoad = new FileManagement();

            string fileName = string.Empty;
            string fileExtention = string.Empty;
            ViewBag.spec_id = -1;
            if (nameType != null)
            {
                fileName = nameType + fileLoad.NewFileName(file.FileName);
                fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower();

            }
            else
            {
                #region 批次上傳圖片操作
                //此處由批次上傳進入.
                //判斷文件名格式是否正確
                ViewBag.moreFileOneTime = true;
                //int prodCheck = file.FileName.LastIndexOf("prod_");
                //int specCheck = file.FileName.LastIndexOf("spec_");
                int descCheck = file.FileName.LastIndexOf("desc_");
                int appCheck = file.FileName.LastIndexOf("app_");
                string errorMsg = "ERROR/";
                if (prodCheck == -1 && descCheck == -1 && appCheck == -1 && mobileCheck == -1)
                {
                    errorMsg += "[" + file.FileName + "] ";
                    errorMsg += Resources.Product.FILE_NAME_ERROR;
                    ViewBag.fileName = errorMsg;
                    return View("~/Views/Product/upLoad.cshtml");
                }
                else
                {
                    nameType = file.FileName.Split('_')[0];

                    if (nameType == "app")
                    {
                        type = 2;
                    }
                    else if (nameType == "desc")
                    {
                        type = 1;
                    }
                    fileName = nameType + fileLoad.NewFileName(file.FileName);
                    fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                    fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.'));
                }
                #endregion
            }
            SetPath(type);//設定圖片路徑
            string localDescPath = imgLocalPath + descPath;
            //string localDesc400Path = imgLocalPath + desc400Path;
            string returnName = imgServerPath;

            bool result = false;
            string NewFileName = string.Empty;


            BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
            NewFileName = hash.Md5Encrypt(fileName, "32");

            string firstFolder = NewFileName.Substring(0, 2) + "/";
            string secondFolder = NewFileName.Substring(2, 2) + "/";
            string ServerPath = string.Empty;

            if (nameType == "desc" || nameType == "app")
            {

                Mappath[0] = firstFolder;
                Mappath[1] = secondFolder;

                CreateFolder(localDescPath, Mappath);
                //CreateFolder(localDesc400Path, Mappath);

                localDescPath += firstFolder + secondFolder;

                //localDesc400Path += firstFolder + secondFolder;
                descPath += firstFolder + secondFolder;

                //localDesc400Path += NewFileName + fileExtention;
                returnName += descPath + NewFileName + fileExtention;


                NewFileName = localDescPath + NewFileName + fileExtention;

                ServerPath = Server.MapPath(imgLocalServerPath + descPath);
            }
            else
            {
                Mappath[0] = firstFolder;
                Mappath[1] = secondFolder;
                //Data:2014/06/26
                //author：Castle
                //在前台如果各种尺寸的图档没有的时候，前台会自动产生！！！
                CreateFolder(localProdPath, Mappath);

                localProdPath += firstFolder + secondFolder;

                prodPath += firstFolder + secondFolder;

                returnName += prodPath + NewFileName + fileExtention;
                NewFileName = localProdPath + NewFileName + fileExtention;
                ServerPath = Server.MapPath(imgLocalServerPath + prodPath);
            }
            string ErrorMsg = string.Empty;
            Resource.CoreMessage = new CoreResource("Product");
            try
            {
                //上傳圖片
                result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                //上傳對應大小圖片
                //压缩图片至其它规格
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            if (string.IsNullOrEmpty(ErrorMsg))
            {
                ViewBag.fileName = returnName;
                ViewBag.Type = type;

                //獲取文件長度 add by zhuoqin0830w 2015/01/29
                string[] strFile = file.FileName.Split('_');
                //判斷文件名的長度是否大於 1 如果大於 1 則再次進行判斷是否為數字 如果不是則進行強制轉換  
                int image_sort = 0;
                int.TryParse(strFile.Length > 1 ? strFile[1] : "0", out image_sort);
                ViewBag.image_sort = image_sort;
            }
            else
            {
                // 判斷 批量上傳中 是否存在 商品圖 或 手機商品圖  edit by zhuoqin0830w 2015/03/24
                if (prodCheck == 0 || mobileCheck == 0)
                { ViewBag.fileName = "ERROR/" + "[" + file.FileName + "] " + ErrorMsg; }
                else { ViewBag.fileName = "ERROR/" + "[" + file.FileName + "] " + ErrorMsg; }

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = "ERROR/" + ErrorMsg;
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            return View("~/Views/Product/upLoad.cshtml");
        }

        //[HttpPost]
        //public ActionResult upLoadImg()
        //{
        //    int type = Request["appOrexplain"] == null ? 0 : Convert.ToInt32(Request["appOrexplain"]);
        //    string path = Server.MapPath(xmlPath);
        //    siteConfigMgr = new SiteConfigMgr(path);
        //    ViewBag.moreFileOneTime = false;
        //    SiteConfig extention_config = siteConfigMgr.GetConfigByName("PIC_Extention_Format");
        //    SiteConfig minValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
        //    SiteConfig maxValue_config = siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
        //    SiteConfig admin_userName = siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
        //    SiteConfig admin_passwd = siteConfigMgr.GetConfigByName("ADMIN_PASSWD");

        //    string extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
        //    string minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
        //    string maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;

        //    string localProdPath = imgLocalPath + prodPath;
        //    //string localProd50Path = imgLocalPath + prod50Path;
        //    //string localProd150Path = imgLocalPath + prod150Path;
        //    //string localProd280Path = imgLocalPath + prod280Path;

        //    string localSpecPath = imgLocalPath + specPath;
        //    //string localSpec100Path = imgLocalPath + spec100Path;
        //    //string localSpec280Path = imgLocalPath + spec280Path;

        //    string[] Mappath = new string[2];

        //    FileManagement fileLoad = new FileManagement();

        //    HttpPostedFileBase file = Request.Files["Filedata"];
        //    string nameType = Request.Params["nameType"];
        //    string fileName = string.Empty;
        //    string fileExtention = string.Empty;
        //    ViewBag.spec_id = -1;
        //    if (nameType != null)
        //    {
        //        fileName = nameType + fileLoad.NewFileName(file.FileName);
        //        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
        //        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower();

        //    }
        //    else
        //    {
        //        #region 批次上傳圖片操作
        //        //此處由批次上傳進入.
        //        //判斷文件名格式是否正確
        //        ViewBag.moreFileOneTime = true;
        //        int prodCheck = file.FileName.LastIndexOf("prod_");
        //        int descCheck = file.FileName.LastIndexOf("desc_");
        //        int appCheck = file.FileName.LastIndexOf("app_");

        //        string errorMsg = "ERROR/";
        //        if (prodCheck == -1 && descCheck == -1 && appCheck == -1)
        //        {
        //            errorMsg += "[" + file.FileName + "] ";
        //            errorMsg += Resources.Product.FILE_NAME_ERROR;
        //            ViewBag.fileName = errorMsg;
        //            return View("~/Views/Product/upLoad.cshtml");
        //        }
        //        else
        //        {

        //            nameType = file.FileName.Split('_')[0];
        //            if (nameType == "app")
        //            {
        //                type = 2;
        //            }
        //            else if (nameType == "desc")
        //            {
        //                type = 1;
        //            }
        //            fileName = nameType + fileLoad.NewFileName(file.FileName);
        //            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
        //            fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.'));
        //        }
        //        #endregion
        //    }

        //    SetPath(type);//設定圖片路徑
        //    string localDescPath = imgLocalPath + descPath;
        //    //string localDesc400Path = imgLocalPath + desc400Path;

        //    string returnName = imgServerPath;


        //    bool result = false;
        //    string NewFileName = string.Empty;


        //    BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
        //    NewFileName = hash.Md5Encrypt(fileName, "32");

        //    string firstFolder = NewFileName.Substring(0, 2) + "/";
        //    string secondFolder = NewFileName.Substring(2, 2) + "/";
        //    string ServerPath = string.Empty;

        //    if (nameType == "spec")
        //    {
        //        Mappath[0] = firstFolder;
        //        Mappath[1] = secondFolder;

        //        CreateFolder(localSpecPath, Mappath);
        //        //CreateFolder(localSpec100Path, Mappath);
        //        //CreateFolder(localSpec280Path, Mappath);

        //        localSpecPath += firstFolder + secondFolder;
        //        //localSpec100Path += firstFolder + secondFolder;
        //        //localSpec280Path += firstFolder + secondFolder;
        //        specPath += firstFolder + secondFolder;

        //        returnName += specPath + NewFileName + fileExtention;
        //        //localSpec100Path += NewFileName + fileExtention;
        //        //localSpec280Path += NewFileName + fileExtention;
        //        NewFileName = localSpecPath + NewFileName + fileExtention;
        //        ServerPath = Server.MapPath(imgLocalServerPath + specPath);

        //    }
        //    else if (nameType == "desc" || nameType == "app")
        //    {
        //        Mappath[0] = firstFolder;
        //        Mappath[1] = secondFolder;

        //        CreateFolder(localDescPath, Mappath);
        //        //CreateFolder(localDesc400Path, Mappath);

        //        localDescPath += firstFolder + secondFolder;
        //        //localDesc400Path += firstFolder + secondFolder;
        //        descPath += firstFolder + secondFolder;

        //        //localDesc400Path += NewFileName + fileExtention;
        //        returnName += descPath + NewFileName + fileExtention;
        //        NewFileName = localDescPath + NewFileName + fileExtention;
        //        ServerPath = Server.MapPath(imgLocalServerPath + descPath);
        //    }
        //    else
        //    {
        //        Mappath[0] = firstFolder;
        //        Mappath[1] = secondFolder;

        //        CreateFolder(localProdPath, Mappath);
        //        //CreateFolder(localProd50Path, Mappath);
        //        //CreateFolder(localProd150Path, Mappath);
        //        //CreateFolder(localProd280Path, Mappath);

        //        localProdPath += firstFolder + secondFolder;
        //        //localProd50Path += firstFolder + secondFolder;
        //        //localProd150Path += firstFolder + secondFolder;
        //        //localProd280Path += firstFolder + secondFolder;
        //        prodPath += firstFolder + secondFolder;

        //        //localProd50Path += NewFileName + fileExtention;
        //        //localProd150Path += NewFileName + fileExtention;
        //        //localProd280Path += NewFileName + fileExtention;
        //        returnName += prodPath + NewFileName + fileExtention;
        //        NewFileName = localProdPath + NewFileName + fileExtention;
        //        ServerPath = Server.MapPath(imgLocalServerPath + prodPath);
        //    }
        //    string ErrorMsg = string.Empty;
        //    Resource.CoreMessage = new CoreResource("Product");
        //    try
        //    {
        //        //

        //        //上傳圖片
        //        result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
        //        //上傳對應大小圖片
        //        //if (result)
        //        //{
        //        //    FTP ftp = null;
        //        //    if (nameType == "spec")
        //        //    {
        //        //        ImageClass iC = new ImageClass(NewFileName);
        //        //        iC.getReducedImage(100, 100, localSpec100Path);
        //        //        ImageClass iC100 = new ImageClass(NewFileName);
        //        //        iC100.getReducedImage(280, 280, localSpec280Path);
        //        //    }
        //        //    else if (nameType == "desc"||nameType == "app")
        //        //    {
        //        //        ImageClass iC = new ImageClass(NewFileName);
        //        //        iC.getReducedImage(400, 400, localDesc400Path);
        //        //    }
        //        //    else
        //        //    {
        //        //        if (!Directory.Exists(Server.MapPath(imgLocalServerPath + prod50Path + firstFolder + secondFolder)))
        //        //            Directory.CreateDirectory(Server.MapPath(imgLocalServerPath + prod50Path + firstFolder + secondFolder));
        //        //        ImageClass iC = new ImageClass(Server.MapPath(imgLocalServerPath + prodPath + NewFileName.Substring(NewFileName.LastIndexOf("/"))));
        //        //        iC.getReducedImage(50, 50, Server.MapPath(imgLocalServerPath + prod50Path + firstFolder + secondFolder + NewFileName.Substring(NewFileName.LastIndexOf("/"))));
        //        //        ftp = new FTP(localProd50Path, ftpuser, ftppwd);
        //        //        ftp.UploadFile(Server.MapPath(imgLocalServerPath + prod50Path + firstFolder + secondFolder + NewFileName.Substring(NewFileName.LastIndexOf("/"))));

        //        //        if (!Directory.Exists(Server.MapPath(imgLocalServerPath + prod150Path + firstFolder + secondFolder)))
        //        //            Directory.CreateDirectory(Server.MapPath(imgLocalServerPath + prod150Path + firstFolder + secondFolder));
        //        //        ImageClass iC150 = new ImageClass(Server.MapPath(imgLocalServerPath + prodPath + NewFileName.Substring(NewFileName.LastIndexOf("/"))));
        //        //        iC150.getReducedImage(150, 150, Server.MapPath(imgLocalServerPath + prod150Path + firstFolder + secondFolder + NewFileName.Substring(NewFileName.LastIndexOf("/"))));
        //        //        ftp = new FTP(localProd150Path, ftpuser, ftppwd);
        //        //        ftp.UploadFile(Server.MapPath(imgLocalServerPath + prod150Path + firstFolder + secondFolder + NewFileName.Substring(NewFileName.LastIndexOf("/"))));

        //        //        if (!Directory.Exists(Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder)))
        //        //            Directory.CreateDirectory(Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder));
        //        //        ImageClass iC280 = new ImageClass(Server.MapPath(imgLocalServerPath + prodPath + NewFileName.Substring(NewFileName.LastIndexOf("/"))));
        //        //        iC280.getReducedImage(280, 280, Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder + NewFileName.Substring(NewFileName.LastIndexOf("/"))));
        //        //        ftp = new FTP(localProd280Path, ftpuser, ftppwd);
        //        //        ftp.UploadFile(Server.MapPath(imgLocalServerPath + prod280Path + firstFolder + secondFolder + NewFileName.Substring(NewFileName.LastIndexOf("/"))));
        //        //    }
        //        //}

        //    if (string.IsNullOrEmpty(ErrorMsg))
        //    {
        //        ViewBag.fileName = returnName;
        //        ViewBag.Type = type;
        //    }
        //    else
        //    {
        //        ViewBag.fileName = "ERROR/" + ErrorMsg;
        //    }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //    }

        //    return View("~/Views/Product/upLoad.cshtml");
        //}


        //判斷類型
        public void SetPath(int type)
        {
            switch (type)
            {
                case 1:
                    descPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descPath);
                    break;
                case 2:
                    descPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.descMobilePath);
                    break;
                default:
                    break;
            }

        }

        [HttpPost]
        public string QueryExplainPic()
        {
            int apporexplain = Convert.ToInt32(Request["apporexplain"]);
            string json = string.Empty;
            SetPath(apporexplain);
            if (string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                //查找臨時表記錄
                _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
                int writer_Id = (Session["caller"] as Caller).user_id;
                ProductPictureTemp temp = new ProductPictureTemp { writer_Id = writer_Id, combo_type = COMBO_TYPE };
                if (!string.IsNullOrEmpty(Request.Params["OldProductId"]))
                {
                    temp.product_id = Request.Params["OldProductId"];
                }
                List<ProductPictureTemp> picList = _pPicTempMgr.Query(temp, apporexplain);

                foreach (var item in picList)
                {
                    if (item.image_filename != "")
                    {
                        if (item.pic_type == 2) ///edti by wwei0216w 當是手機圖片時,將查找的物理路徑改變為手機的路徑
                        {
                            descPath = "/product_picture/mobile/";
                        }
                        item.image_filename = imgServerPath + descPath + GetDetailFolder(item.image_filename) + item.image_filename;
                    }
                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(picList) + "}";
                json = json.Replace("image_filename", "img");
            }
            else
            {
                //查詢正式表
                _productPicMgr = new ProductPictureMgr(connectionString);

                int product_id = int.Parse(Request.Params["product_id"]);
                List<ProductPicture> pList = _productPicMgr.Query(product_id, apporexplain);
                foreach (var item in pList)
                {
                    if (item.image_filename != "")
                    {
                        if (item.pic_type == 2) ///edti by wwei0216w 當是手機圖片時,將查找的物理路徑改變為手機的路徑
                        {
                            descPath = "/product_picture/mobile/";
                        }
                        item.image_filename = imgServerPath + descPath + GetDetailFolder(item.image_filename) + item.image_filename;

                    }
                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(pList) + "}";
                json = json.Replace("image_filename", "img");
            }


            return json;
        }

        /// <summary>
        /// 根據圖片名反推文件目錄
        /// </summary>
        /// <param name="picName">文件名</param>
        /// <returns>返回文件目錄</returns>
        public string GetDetailFolder(string picName)
        {
            string firthFolder = picName.Substring(0, 2) + "/";
            string secondFolder = picName.Substring(2, 2) + "/";

            return firthFolder + secondFolder;
        }

        [HttpPost]
        public HttpResponseBase DeletePic()
        {
            string json = "{success:true,msg:\"" + Resources.Product.DELETE_SUCCESS + "\"}";
            string deleteType = Request.Params["type"];
            int apporexplain = Convert.ToInt32(Request["apporexplain"]);//判斷是從前臺的APP傳來還是從explain傳來
            ProductSpecTemp psTemp = new ProductSpecTemp();
            ProductSpec pSpec = new ProductSpec();
            List<ProductSpecTemp> psList = new List<ProductSpecTemp>();

            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _specMgr = new ProductSpecMgr(connectionString);


            string[] record = Request.Params["rec"].Split(',');
            string fileName = record[0].Split('/')[imgNameIdx];

            SetPath(apporexplain);
            if (deleteType == "desc")
            {
                string imageName = imgLocalPath + descPath + GetDetailFolder(fileName) + fileName;
                string imageName400 = imgLocalPath + desc400Path + GetDetailFolder(fileName) + fileName;
                DeletePicFile(imageName);
                DeletePicFile(imageName400);
            }

            if (string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                psTemp.Writer_Id = (Session["caller"] as Caller).user_id;

                try
                {
                    _specTempMgr.Update(psList, "image");
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:true,msg:\"" + Resources.Product.DELETE_SPEC_FAIL + "\"}";
                }
            }
            else
            {
                pSpec.product_id = uint.Parse(Request.Params["product_id"]);

                try
                {
                    _specMgr.UpdateSingle(pSpec);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:true,msg:\"" + Resources.Product.DELETE_SPEC_FAIL + "\"}";
                }
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        [HttpPost]
        public HttpResponseBase productPictrueTempSave()
        {

            string json = "{success:true}";
            ProductTemp pTemp = new ProductTemp();
            _productTempMgr = new ProductTempMgr(connectionString);
            _specTempMgr = new ProductSpecTempMgr(connectionString);

            _productPicMgr = new ProductPictureMgr(connectionString);

            _specMgr = new ProductSpecMgr(connectionString);



            if (string.IsNullOrEmpty(Request.Params["product_id"]))
            {

                if (!string.IsNullOrEmpty(Request.Params["image_InsertValue"])) pTemp.Product_Image = Request.Params["image_InsertValue"];
                if (!string.IsNullOrEmpty(Request.Params["image_MobileValue"])) pTemp.Mobile_Image = Request.Params["image_MobileValue"];//如果手機說明圖有值,將值賦予Moibile_Image edit by wwei0216w 2015/3/18 
                if (!string.IsNullOrEmpty(Request.Params["productMedia"])) pTemp.product_media = Request.Params["productMedia"];
                if (!string.IsNullOrEmpty(Request.Params["specify_Product_alt"])) pTemp.Product_alt = Request.Params["specify_Product_alt"];//add by wwei0216w 2015/4/9
                pTemp.Writer_Id = (Session["caller"] as Caller).user_id;
                pTemp.Combo_Type = COMBO_TYPE;
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    pTemp.Product_Id = Request.Form["OldProductId"];
                }
                ProductSpecTemp pSpec = new ProductSpecTemp();
                List<ProductSpecTemp> pSpecList = new List<ProductSpecTemp>();
                if (!string.IsNullOrEmpty(Request.Params["spec_InsertValue"]))
                {
                    string[] Values = Request.Form["spec_InsertValue"].ToString().Split(';');
                    for (int i = 0; i < Values.Length - 1; i++)
                    {
                        pSpec = new ProductSpecTemp();
                        pSpec.Writer_Id = (Session["caller"] as Caller).user_id;
                        pSpec.product_id = pTemp.Product_Id;
                        string[] perValue = Values[i].Split(',');
                        if (!string.IsNullOrEmpty(perValue[0])) { pSpec.spec_image = perValue[0]; };
                        if (!string.IsNullOrEmpty(perValue[1])) { pSpec.spec_id = uint.Parse(perValue[1]); };
                        if (!string.IsNullOrEmpty(perValue[2])) { pSpec.spec_sort = uint.Parse(perValue[2]); };
                        if (!string.IsNullOrEmpty(perValue[3])) { pSpec.spec_status = uint.Parse(perValue[3]); };
                        pSpecList.Add(pSpec);
                    }
                }

                List<ProductPictureTemp> picList = new List<ProductPictureTemp>();
                _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
                ProductPictureTemp pPic = new ProductPictureTemp();
                if (!string.IsNullOrEmpty(Request.Params["picture_InsertValue"]))
                {
                    string[] Values = Request.Form["picture_InsertValue"].ToString().Split(';');
                    for (int i = 0; i < Values.Length - 1; i++)
                    {
                        pPic = new ProductPictureTemp();
                        string[] perValue = Values[i].Split(',');
                        pPic.combo_type = COMBO_TYPE;
                        pPic.writer_Id = (Session["caller"] as Caller).user_id;
                        pPic.product_id = pTemp.Product_Id;
                        if (!string.IsNullOrEmpty(perValue[0])) { pPic.image_filename = perValue[0]; };
                        if (!string.IsNullOrEmpty(perValue[1])) { pPic.image_sort = uint.Parse(perValue[1]); };
                        if (!string.IsNullOrEmpty(perValue[2])) { pPic.image_state = uint.Parse(perValue[2]); };
                        picList.Add(pPic);
                    }
                }

                //關於手機APP的代碼部份
                List<ProductPictureTemp> picAppList = new List<ProductPictureTemp>();
                //IProductPictureTempImplAppMgr ppt = new ProductPictureAppTempImplMgr(connectionString);
                ProductPictureTemp pa = new ProductPictureTemp();
                if (!string.IsNullOrEmpty(Request.Params["mobilePic_InsertValue"]))
                {
                    string[] Values = Request.Form["mobilePic_InsertValue"].ToString().Split(';');
                    for (int i = 0; i < Values.Length - 1; i++)
                    {
                        pa = new ProductPictureTemp();
                        string[] AppValue = Values[i].Split(',');
                        pa.combo_type = COMBO_TYPE;
                        pa.writer_Id = (Session["caller"] as Caller).user_id;
                        pa.product_id = pTemp.Product_Id;
                        if (!string.IsNullOrEmpty(AppValue[0])) { pa.image_filename = AppValue[0]; };
                        if (!string.IsNullOrEmpty(AppValue[1])) { pa.image_sort = uint.Parse(AppValue[1]); };
                        if (!string.IsNullOrEmpty(AppValue[2])) { pa.image_state = uint.Parse(AppValue[2]); };
                        picAppList.Add(pa);
                    }
                }

                try
                {
                    int type = 1;//1：商品說明圖,2：手機App說明圖

                    int writer_id = (Session["caller"] as Caller).user_id;
                    //保存至productTemp
                    if (pTemp.Product_Image != "" || pTemp.product_media != "" || pTemp.Mobile_Image != "" || pTemp.Product_alt != "")
                    {
                        _productTempMgr.ProductTempUpdate(pTemp, "pic");
                    }
                    //保存規格圖
                    _specTempMgr.Update(pSpecList, "image");
                    //保存說明圖
                    string oldProductId = "0";
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        oldProductId = Request.Form["OldProductId"];
                    }
                    _pPicTempMgr.Save(picList, new ProductPictureTemp() { writer_Id = writer_id, combo_type = COMBO_TYPE, product_id = oldProductId }, type);// edit by wangwei0216w 註釋掉_pPicTempMgr.Save 以解決複製后不能讀取圖片路勁到數據庫
                    type = 2;

                    _pPicTempMgr.Save(picAppList, new ProductPictureTemp() { writer_Id = writer_id, combo_type = COMBO_TYPE, product_id = oldProductId }, type);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:false,msg:\"" + Resources.Product.EDIT_FAIL + "\"}";
                }
            }
            else//更新正式表
            {
                Product p = new Product();
                uint productId = uint.Parse(Request.Params["product_id"]);
                if (!string.IsNullOrEmpty(Request.Params["image_InsertValue"])) p.Product_Image = Request.Params["image_InsertValue"];//如果商品說明圖有值,將值賦予Product_Image
                if (!string.IsNullOrEmpty(Request.Params["image_MobileValue"])) p.Mobile_Image = Request.Params["image_MobileValue"];//如果手機說明圖有值,將值賦予Moibile_Image edit by wwei0216w 2015/3/18 
                if (!string.IsNullOrEmpty(Request.Params["productMedia"])) p.product_media = Request.Params["productMedia"];
                if (!string.IsNullOrEmpty(Request.Params["specify_Product_alt"])) p.Product_alt = Request.Params["specify_Product_alt"];//add by wwei0216w 2015/4/9
                p.Product_Id = productId;
                _productMgr = new ProductMgr(connectionString);

                ProductSpec pSpec = new ProductSpec();
                List<ProductSpec> pSpecList = new List<ProductSpec>();
                if (!string.IsNullOrEmpty(Request.Params["spec_InsertValue"]))
                {
                    string[] Values = Request.Form["spec_InsertValue"].ToString().Split(';');
                    for (int i = 0; i < Values.Length - 1; i++)
                    {
                        pSpec = new ProductSpec();
                        pSpec.product_id = productId;
                        string[] perValue = Values[i].Split(',');
                        if (!string.IsNullOrEmpty(perValue[0])) { pSpec.spec_image = perValue[0]; };
                        if (!string.IsNullOrEmpty(perValue[1])) { pSpec.spec_id = uint.Parse(perValue[1]); };
                        if (!string.IsNullOrEmpty(perValue[2])) { pSpec.spec_sort = uint.Parse(perValue[2]); };
                        if (!string.IsNullOrEmpty(perValue[3])) { pSpec.spec_status = uint.Parse(perValue[3]); };
                        pSpecList.Add(pSpec);
                    }
                }

                List<ProductPicture> picList = new List<ProductPicture>();
                _productPicMgr = new ProductPictureMgr(connectionString);
                ProductPicture pPic = new ProductPicture();

                if (!string.IsNullOrEmpty(Request.Params["picture_InsertValue"]))
                {
                    string[] Values = Request.Form["picture_InsertValue"].ToString().Split(';');
                    for (int i = 0; i < Values.Length - 1; i++)
                    {
                        pPic = new ProductPicture();
                        string[] perValue = Values[i].Split(',');
                        pPic.product_id = int.Parse(Request.Params["product_id"]);
                        if (!string.IsNullOrEmpty(perValue[0])) { pPic.image_filename = perValue[0]; };
                        if (!string.IsNullOrEmpty(perValue[1])) { pPic.image_sort = uint.Parse(perValue[1]); };
                        if (!string.IsNullOrEmpty(perValue[2])) { pPic.image_state = uint.Parse(perValue[2]); };
                        picList.Add(pPic);
                    }
                }

                //手機app圖檔
                List<ProductPicture> appList = new List<ProductPicture>();
                _productPicMgr = new ProductPictureMgr(connectionString);
                ProductPicture apppPic = new ProductPicture();

                if (!string.IsNullOrEmpty(Request.Params["mobilePic_InsertValue"]))
                {
                    string[] Values = Request.Form["mobilePic_InsertValue"].ToString().Split(';');
                    for (int i = 0; i < Values.Length - 1; i++)
                    {
                        apppPic = new ProductPicture();
                        string[] perValue = Values[i].Split(',');
                        apppPic.product_id = int.Parse(Request.Params["product_id"]);
                        if (!string.IsNullOrEmpty(perValue[0])) { apppPic.image_filename = perValue[0]; };
                        if (!string.IsNullOrEmpty(perValue[1])) { apppPic.image_sort = uint.Parse(perValue[1]); };
                        if (!string.IsNullOrEmpty(perValue[2])) { apppPic.image_state = uint.Parse(perValue[2]); };
                        appList.Add(apppPic);
                    }
                }

                try
                {
                    _productMgr.Update_Product_Spec_Picture(p, pSpecList, picList, appList);
                    json = "{success:true,msg:\"" + Resources.Product.SAVE_SUCCESS + "\"}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:true,msg:\"" + Resources.Product.SAVE_FAIL + "\"}";
                }
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

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

        public void DeletePicFile(string imageName)
        {
            if (System.IO.File.Exists(imageName))
            {
                System.IO.File.Delete(imageName);
            }
        }

        [HttpPost]
        public string QuerySpecPic()
        {
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _specMgr = new ProductSpecMgr(connectionString);

            string json = string.Empty;
            if (string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                //查找臨時表記錄
                ProductSpecTemp psTemp = new ProductSpecTemp();
                psTemp.Writer_Id = (Session["caller"] as Caller).user_id;
                psTemp.spec_type = 1;
                //string str = "{success:true,items:"+JsonConvert.+"}";
                List<ProductSpecTemp> results = _specTempMgr.Query(psTemp); //JsonConvert.SerializeObject();
                foreach (var item in results)
                {
                    if (item.spec_image != "")
                    {
                        item.spec_image = imgServerPath + specPath + GetDetailFolder(item.spec_image) + item.spec_image;
                    }

                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(results) + "}";
                json = json.Replace("spec_image", "img");
            }
            else
            {
                //查找正式表
                ProductSpec pSpec = new ProductSpec();
                pSpec.product_id = uint.Parse(Request.Params["product_id"]);
                pSpec.spec_type = 1;
                List<ProductSpec> spList = _specMgr.Query(pSpec);
                foreach (var item in spList)
                {
                    if (item.spec_image != "")
                    {
                        item.spec_image = imgServerPath + specPath + GetDetailFolder(item.spec_image) + item.spec_image;
                    }

                }
                json = "{success:true,items:" + JsonConvert.SerializeObject(spList) + "}";
                json = json.Replace("spec_image", "img");

            }
            return json;
        }

        public void DeletePicOnServer(bool prod, bool spec, bool desc, string product_id, int type = 1)
        {
            int writerId = (Session["caller"] as Caller).user_id;
            _productTempMgr = new ProductTempMgr(connectionString);
            _specTempMgr = new ProductSpecTempMgr(connectionString);
            _pPicTempMgr = new ProductPictureTempImplMgr(connectionString);
            ArrayList ImgList = new ArrayList();
            ProductSpecTemp pSpec = new ProductSpecTemp();
            pSpec.Writer_Id = writerId;
            pSpec.spec_type = 1;
            //刪除對應的圖片
            //商品圖
            if (prod)
            {
                ProductTemp query = new ProductTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE, Product_Id = product_id.ToString() };
                string fileName = _productTempMgr.GetProTemp(query).Product_Image;
                if (!string.IsNullOrEmpty(fileName))
                {
                    ImgList.Add(imgLocalPath + prodPath + GetDetailFolder(fileName) + fileName);
                    ImgList.Add(imgLocalPath + prod50Path + GetDetailFolder(fileName) + fileName);
                    ImgList.Add(imgLocalPath + prod150Path + GetDetailFolder(fileName) + fileName);
                    ImgList.Add(imgLocalPath + prod280Path + GetDetailFolder(fileName) + fileName);
                }

            }
            //規格圖
            if (spec)
            {
                List<ProductSpecTemp> pSList = _specTempMgr.Query(pSpec);
                foreach (var item in pSList)
                {
                    if (item.spec_image != "")
                    {
                        ImgList.Add(imgLocalPath + specPath + GetDetailFolder(item.spec_image) + item.spec_image);
                        ImgList.Add(imgLocalPath + spec100Path + GetDetailFolder(item.spec_image) + item.spec_image);
                        ImgList.Add(imgLocalPath + spec280Path + GetDetailFolder(item.spec_image) + item.spec_image);
                    }
                }
            }
            //商品說明圖
            if (desc)
            {
                ProductPictureTemp query = new ProductPictureTemp { writer_Id = writerId, combo_type = COMBO_TYPE, product_id = product_id };
                List<ProductPictureTemp> pPList = _pPicTempMgr.Query(query);
                SetPath(type);
                foreach (var item in pPList)
                {
                    ImgList.Add(imgLocalPath + descPath + GetDetailFolder(item.image_filename) + item.image_filename);
                    ImgList.Add(imgLocalPath + desc400Path + GetDetailFolder(item.image_filename) + item.image_filename);
                }
            }

            foreach (string item in ImgList)
            {
                //刪除服務器上對應的圖片
                if (System.IO.File.Exists(item))
                {
                    System.IO.File.Delete(item);
                }
            }
        }



        #endregion

        #region  商品基本信息

        [HttpPost]
        public HttpResponseBase SaveBaseInfo()
        {
            string json = "{success:true}";
            int transportDays = -1;///初始化
            uint product_mode = 0;
            uint brand_id = 0;
            try
            {
                string prod_name = (Request.Form["prod_name"] ?? "").Trim();
                string prod_sz = (Request.Form["prod_sz"] ?? "").Trim();
                if (!Product.CheckProdName(prod_name) || !Product.CheckProdName(prod_sz))
                {
                    json = "{success:false,msg:'" + Resources.Product.FORBIDDEN_CHARACTER + "'}";
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;
                }
                ProductTemp pTemp = new ProductTemp();
                _productTempMgr = new ProductTempMgr(connectionString);
                _productMgr = new ProductMgr(connectionString);
                Caller _caller = (Session["caller"] as Caller);
                Product p = new Product();
             

                ///add by wwei0216w 2015/8/24
                ///根據product_mode查找供應商對應的自出,寄倉,調度欄位,如果為0則不予保存
                 brand_id = uint.Parse(Request.Form["brand_id"]?? "0");
                 product_mode = uint.Parse(Request.Form["product_mode"]??"0");///獲得product_mode
                 string msg = "寄倉";
                IVendorImplMgr _vendorMgr = new VendorMgr(connectionString);
                List<Vendor> vendorList = _vendorMgr.GetArrayDaysInfo(brand_id);
                if (vendorList.Count > 0)
                {
                    switch (product_mode)
                    {
                        case 1:
                            transportDays = vendorList.FirstOrDefault<Vendor>().self_send_days;
                            msg = "自出";
                            break;
                        case 2:
                            transportDays = vendorList.FirstOrDefault<Vendor>().stuff_ware_days;
                            msg = "寄倉";
                            break;
                        case 3:
                            msg = "調度";
                            transportDays = vendorList.FirstOrDefault<Vendor>().dispatch_days;
                            break;
                        default:
                            break;
                    }
                }

                if (transportDays == 0)
                {
                    json = "{success:false,msg:'" + msg + Resources.Product.TRANSPORT_DAYS + "'}";
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;
                }

                //查詢product表。
                if (Request.Params["product_id"] != "")
                {
                    p.Product_Id = uint.Parse(Request.Params["product_id"]);
                    p = _productMgr.Query(p)[0];
                }


                uint product_sort = 0;
                string product_vendor_code = "";
                uint product_start = 0;
                uint product_end = 0;
                uint expect_time = 0;
                uint product_freight_set = 0;
                int tax_type = 0;
                uint combination = 0;
                string expect_msg = string.Empty;
                int show_in_deliver = 0;
                int process_type = 0;
                int product_type = 0;
                uint recommedde_jundge = 0;
                uint recommedde_expend_day = 0;
                string recommededcheckall = string.Empty;
                int purchase_in_advance = 0;
                uint purchase_in_advance_start = 0;
                uint purchase_in_advance_end = 0;
                //庫存
                if (!string.IsNullOrEmpty(Request.Params["ig_sh_InsertValue"]))
                {
                    string[] Value = Request.Params["ig_sh_InsertValue"].Split(',');
                    pTemp.Ignore_Stock = int.Parse(Value[0]);
                    pTemp.Shortage = int.Parse(Value[1]);
                    pTemp.stock_alarm = int.Parse(Value[2]);

                    if (Request.Params["product_id"] != "")
                    {
                        p.Ignore_Stock = int.Parse(Value[0]);
                        p.Shortage = int.Parse(Value[1]);
                        p.stock_alarm = int.Parse(Value[2]);
                    }
                }
                else
                {
                    //brand_id = uint.Parse(Request.Form["brand_id"]);
                    product_sort = uint.Parse(Request.Form["product_sort"]);
                    product_vendor_code = Request.Form["product_vendor_code"];
                    product_start = uint.Parse(CommonFunction.GetPHPTime(Request.Form["product_start"]).ToString());
                    product_end = uint.Parse(CommonFunction.GetPHPTime(Request.Form["product_end"]).ToString());
                    expect_time = uint.Parse(CommonFunction.GetPHPTime(Request.Form["expect_time"]).ToString());
                    product_freight_set = uint.Parse(Request.Form["product_freight_set"]);
                    tax_type = int.Parse(Request.Form["tax_type"]);
                    combination = uint.Parse(Request.Form["combination"]);
                    //product_mode = uint.Parse(Request.Params["product_mode"]);
                    expect_msg = Request.Form["expect_msg"] ?? "";
                    //商品新增欄位 add by  xiangwang0413w 2014/09/15
                    show_in_deliver = int.Parse(Request.Form["show_in_deliver"]);
                    process_type = int.Parse(Request.Form["process_type"]);
                    product_type = int.Parse(Request.Form["product_type"]);
                    //add by dongya 2015/08/26 
                    recommedde_jundge = uint.Parse(Request.Form["recommedde_jundge"]);//是否選擇了推薦商品屬性 1 表示推薦
                    recommedde_expend_day = 0;
                    if (recommedde_jundge == 1)
                    {
                        if (!string.IsNullOrEmpty(Request.Params["recommededcheckall"]))
                        {
                            recommededcheckall = Request.Params["recommededcheckall"].ToString().TrimEnd(',');//選擇的所有的月數
                        }
                        recommedde_expend_day = uint.Parse(Request.Form["recommedde_expend_day"]);
                    }
                    //add by dongya 2015/09/02 
                    purchase_in_advance = Convert.ToInt32(Request.Form["purchase_in_advance"]);
                    purchase_in_advance_start = uint.Parse(Request.Form["purchase_in_advance_start"]);
                    purchase_in_advance_end = uint.Parse(Request.Form["purchase_in_advance_end"]);
                }

             

                if (string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    pTemp.Brand_Id = brand_id;
                    pTemp.Prod_Name = prod_name;
                    pTemp.Prod_Sz = prod_sz;
                    pTemp.Product_Name = pTemp.GetProductName();

                    pTemp.Product_Sort = product_sort;
                    pTemp.Product_Vendor_Code = product_vendor_code;
                    pTemp.Product_Start = product_start;
                    pTemp.Product_End = product_end;
                    pTemp.Expect_Time = expect_time;
                    pTemp.Product_Freight_Set = product_freight_set;
                    pTemp.Product_Mode = product_mode;
                    pTemp.Tax_Type = tax_type;
                    pTemp.Combination = combination;
                    pTemp.expect_msg = expect_msg;
                    pTemp.Combo_Type = COMBO_TYPE;
                    pTemp.Create_Channel = 1;// 1:後台管理者(manage_user) edit by xiagnwang0413w 2014/08/09
                    //商品新增欄位 add by  xiangwang0413w 2014/09/15
                    pTemp.Show_In_Deliver = show_in_deliver;
                    pTemp.Process_Type = process_type;
                    pTemp.Product_Type = product_type;

                    //add by zhuoqin0830w  增加新的欄位  2015/03/17
                    pTemp.Deliver_Days = 3;
                    pTemp.Min_Purchase_Amount = 1;
                    pTemp.Safe_Stock_Amount = 1;
                    pTemp.Extra_Days = 0;
                    //add by dongya
                    pTemp.recommedde_jundge = recommedde_jundge;//推薦商品 1表示推薦 0表示不推薦
                    pTemp.months = recommededcheckall;//以1,3,這樣的形式顯示
                    pTemp.expend_day = recommedde_expend_day;
                    //add by dongya 2015/09/02 
                    pTemp.purchase_in_advance = purchase_in_advance;
                    pTemp.purchase_in_advance_start = purchase_in_advance_start;
                    pTemp.purchase_in_advance_end = purchase_in_advance_end;

                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        pTemp.Product_Id = Request.Form["OldProductId"];
                    }

                    //查找臨時表是否存在數據，存在：更新，不存在插入
                    pTemp.Writer_Id = _caller.user_id;
                    pTemp.Product_Status = 0;
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        pTemp.Product_Id = Request.Form["OldProductId"];
                    }

                   

                    ProductTemp query = new ProductTemp { Writer_Id = pTemp.Writer_Id, Combo_Type = COMBO_TYPE, Product_Id = pTemp.Product_Id };
                    ProductTemp pTempList = _productTempMgr.GetProTemp(query);
                    if (pTempList == null)
                    {
                        //插入
                        int result = 0;
                        result = _productTempMgr.baseInfoSave(pTemp);
                        if (result >0)
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

                        //更新
                        if (!string.IsNullOrEmpty(Request.Params["ig_sh_InsertValue"]))
                        {
                            _productTempMgr.ProductTempUpdate(pTemp, "stock");
                        }
                        else
                        {
                            if (pTemp.Product_Mode != 2)
                            {
                                pTemp.Bag_Check_Money = 0;
                            }
                            else
                            {
                                pTemp.Bag_Check_Money = pTempList.Bag_Check_Money;
                            }
                            _productTempMgr.baseInfoUpdate(pTemp);
                        }

                    }
                }
                else
                {
                   
                    if (string.IsNullOrEmpty(Request.Params["ig_sh_InsertValue"]))
                    {
                        p.Brand_Id = brand_id;
                        p.Prod_Name = prod_name;
                        p.Prod_Sz = prod_sz;
                        p.Product_Name = p.GetProductName();

                        p.Product_Sort = product_sort;
                        p.Product_Vendor_Code = product_vendor_code;
                        p.Product_Start = product_start;
                        p.Product_End = product_end;
                        p.Expect_Time = expect_time;
                        p.Product_Freight_Set = product_freight_set;
                        p.Product_Mode = product_mode;
                        p.Tax_Type = tax_type;
                        p.expect_msg = expect_msg;//預留商品信息
                        p.Combination = combination;
                        //商品新增欄位 add by  xiangwang0413w 2014/09/15
                        p.Show_In_Deliver = show_in_deliver;
                        p.Process_Type = process_type;
                        p.Product_Type = product_type;

                        //add by zhuoqin0830w  增加新的欄位  2015/03/17
                        p.Deliver_Days = 3;
                        p.Min_Purchase_Amount = 1;
                        p.Safe_Stock_Amount = 1;
                        p.Extra_Days = 0;
                        p.off_grade = int.Parse(Request.Form["off-grade"]);
                        //add by dongya
                        p.recommedde_jundge = recommedde_jundge;//推薦商品 1表示推薦 0表示不推薦
                        p.months = recommededcheckall;//以1,3,這樣的形式顯示
                        p.expend_day = recommedde_expend_day;
                        //add by dongya 0410j
                        p.purchase_in_advance = purchase_in_advance;
                        p.purchase_in_advance_start = purchase_in_advance_start;
                        p.purchase_in_advance_end = purchase_in_advance_end;
                        //更新正式表
                        p.Product_Id = uint.Parse(Request.Params["product_id"]);
                        if (p.Product_Mode != 2)
                        {
                            p.Bag_Check_Money = 0;
                        }

                        #region ScheduleRelation
                        int scheduleId = int.Parse(Request.Form["schedule_id"]);
                        IScheduleRelationImplMgr _srMgr = new ScheduleRelationMgr(connectionString);
                        _srMgr.Save(new ScheduleRelation { relation_table = "product", relation_id = (int)p.Product_Id, schedule_id = scheduleId });
                        #endregion

                    }
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    _productMgr = new ProductMgr(connectionString);
                    _categorySetMgr = new ProductCategorySetMgr(connectionString);
                    ArrayList aList = new ArrayList();
                    aList.Add(_productMgr.Update(p));
                    aList.Add(_categorySetMgr.UpdateBrandId(new ProductCategorySet { Product_Id = p.Product_Id, Brand_Id = p.Brand_Id })); //add by wwei0216w 2015/2/24 品牌名稱變更后,product_category_set表所對應的品牌名稱也需要更新

                    _functionMgr = new FunctionMgr(connectionString);
                    string function = Request.Params["function"] ?? "";
                    Function fun = _functionMgr.QueryFunction(function, "/ProductCombo");
                    int functionid = fun == null ? 0 : fun.RowId;
                    HistoryBatch batch = new HistoryBatch { functionid = functionid };
                    batch.batchno = Request.Params["batch"] ?? "";
                    batch.kuser = (Session["caller"] as Caller).user_email;

                    if (_tableHistoryMgr.SaveHistory<Product>(p, batch, aList))
                    {
                        #region add by zhuoqin0830w  2015/06/25  判斷修改的商品是否是失格商品  1為失格 0為正常
                        if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                        {
                            _productMgr.UpdateOff_Grade(p.Product_Id, p.off_grade);
                        }
                        #endregion
                        //add by wwei0216 2015/1/9 刪除不符合條件的物品匹配模式
                        if (!p.CheckdStoreFreight())
                        {
                            IProductDeliverySetImplMgr _productDeliverySetMgr = new ProductDeliverySetMgr(connectionString);
                            _productDeliverySetMgr.Delete(
                                new ProductDeliverySet { Freight_big_area = 1, Freight_type = 12 }, p.Product_Id);
                        }

                        #region 推薦商品屬性插入/修改recommended_product_attribute表中做記錄 add by dongya 2015/09/30 ----目前只針對單一商品
                        RecommendedProductAttributeMgr rProductAttributeMgr = new RecommendedProductAttributeMgr(connectionString);
                        RecommendedProductAttribute rPA = new RecommendedProductAttribute();
                        rPA.product_id = Convert.ToUInt32(p.Product_Id);
                        rPA.time_start = 0;
                        rPA.time_end = 0;
                        rPA.expend_day = recommedde_expend_day;
                        rPA.months = recommededcheckall;
                        rPA.combo_type = 2;//組合商品
                        //首先判斷表中是否對該product_id設置為推薦
                        int productId = Convert.ToInt32(rPA.product_id);
                        if (rProductAttributeMgr.GetMsgByProductId(productId) > 0)//如果大於0,表示推薦表中存在數據
                        {
                            if (recommedde_jundge == 1)//==1表示推薦 
                            {
                                rProductAttributeMgr.Update(rPA);
                            }
                            else if (recommedde_jundge == 0)//==0表示不推薦 
                            {
                                rProductAttributeMgr.Delete(productId);
                            }
                        }
                        else
                        {
                            if (recommedde_jundge == 1)//==1表示推薦 
                            {
                                rProductAttributeMgr.Save(rPA);
                            }
                        }
                        #endregion
                        json = "{success:true,msg:'" + Resources.Product.SAVE_SUCCESS + "'}";
                    }
                    else
                    {
                        json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
            }

            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public string QueryParameter(Parametersrc p)
        {
            string json = string.Empty;
            try
            {
                paraMgr = new ParameterMgr(connectionString);
                json = paraMgr.QueryBindData(p);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return json;

        }

        [HttpPost]
        [CustomHandleError]
        public string QueryBrand()
        {
            VendorBrand vb = new VendorBrand();
            vbMgr = new VendorBrandMgr(connectionString);
            vb.Brand_Status = 1;
            string json = vbMgr.QueryBrand(vb);
            return json;
        }
        #endregion

        #region 查詢商品信息

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProduct()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    uint product_id = 0;
                    if (uint.TryParse(Request.Form["ProductId"], out product_id))
                    {
                        _productMgr = new ProductMgr(connectionString);
                        Product product = _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                        if (product.Product_alt == "")
                        {
                            product.Product_alt = product.Product_Name;
                        }//add by wwei0216w 2015/4/15 //如果商品說明為空則將商品名稱賦予product_alt
                        if (product != null)
                        {
                            if (!string.IsNullOrEmpty(product.Product_Image))
                            {
                                product.Product_Image = imgServerPath + prodPath + GetDetailFolder(product.Product_Image) + product.Product_Image;
                            }
                            else
                            {
                                product.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                            }
                            if (!string.IsNullOrEmpty(product.Mobile_Image))//edit by wwei0216w 2015/3/18 添加關於手機說明圖的操作
                            {
                                prodPath = prodMobile640; //add by wwei0216w 2015/4/1 添加原因:手機圖片要放在640*640路徑下
                                product.Mobile_Image = imgServerPath + prodPath + GetDetailFolder(product.Mobile_Image) + product.Mobile_Image;
                            }
                            else
                            {
                                product.Mobile_Image = imgServerPath + "/product/nopic_150.jpg";
                            }
                        }
                        json = "{success:true,data:" + JsonConvert.SerializeObject(product) + "}";
                    }
                    else
                    {
                        json = "[]";
                    }
                }
                else
                {
                    _productTempMgr = new ProductTempMgr(connectionString);
                    int writerId = (Session["caller"] as Caller).user_id;
                    ProductTemp query = new ProductTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Product_Id = Request.Form["OldProductId"];
                    }
                    ProductTemp proTemp = _productTempMgr.GetProTemp(query);
                    if (proTemp.Product_alt == "")
                    {
                        proTemp.Product_alt = proTemp.Product_Name;
                    }//add by wwei0216w 2015/4/15 //如果商品說明為空則將商品名稱賦予product_alt
                    if (proTemp != null)
                    {
                        if (proTemp.Product_Image != "")
                        {
                            proTemp.Product_Image = imgServerPath + prodPath + GetDetailFolder(proTemp.Product_Image) + proTemp.Product_Image;
                        }
                        else
                        {
                            proTemp.Product_Image = imgServerPath + "/product/nopic_150.jpg";
                        }
                        if (proTemp.Mobile_Image != "")
                        {
                            proTemp.Mobile_Image = imgServerPath + prodPath + GetDetailFolder(proTemp.Mobile_Image) + proTemp.Mobile_Image;
                        }
                        else
                        {
                            proTemp.Mobile_Image = imgServerPath + "/product/nopic_150.jpg";
                        }
                    }
                    json = "{success:true,data:" + JsonConvert.SerializeObject(proTemp) + "}";
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 規格

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase combSpecQuery()
        {
            resultStr = "{success:false}";
            try
            {
                int pileId = 0;
                int.TryParse(Request.Params["pileId"] ?? "0", out pileId);
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    int parentId = int.Parse(Request.Params["ProductId"]);
                    _combMgr = new ProductComboMgr(connectionString);
                    resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_combMgr.combQuery(new ProductComboCustom { Parent_Id = parentId, Pile_Id = pileId })) + "}";
                }
                else
                {
                    Caller _caller = (Session["caller"] as Caller);
                    _combTempMgr = new ProductComboTempMgr(connectionString);
                    ProductComboCustom query = new ProductComboCustom { Writer_Id = _caller.user_id, Pile_Id = pileId };
                    //複製商品時
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Parent_Id = int.Parse(Request.Form["OldProductId"]);
                    }
                    resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_combTempMgr.combQuery(query)) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }

        /// <summary>
        /// 組合商品各自定價修改站臺價格
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetUpdatePrice()
        {
            resultStr = "{success:false}";
            try
            {
                int pileId = 0;
                int.TryParse(Request.Params["pileId"] ?? "0", out pileId);
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    _combMgr = new ProductComboMgr(connectionString);
                    ProductComboCustom query = new ProductComboCustom { Parent_Id = int.Parse(Request.Params["ProductId"]), Pile_Id = pileId };
                    query.user_id = int.Parse(Request.Params["user_id"]);
                    query.user_level = int.Parse(Request.Params["user_level"]);
                    query.site_id = int.Parse(Request.Params["site_id"]);
                    if (string.IsNullOrEmpty(Request.Form["is_same"]))
                    {
                        resultStr = "{success:true,same:" + JsonConvert.SerializeObject(_combMgr.sameSpecQuery(query)) + ",different:" + JsonConvert.SerializeObject(_combMgr.differentSpecQuery(query)) + "}";
                    }
                    else if (Request.Form["is_same"] == "1")
                    {
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_combMgr.sameSpecQuery(query)) + "}";
                    }
                    else if (Request.Form["is_same"] == "0")
                    {
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(_combMgr.differentSpecQuery(query)) + "}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                resultStr = "{success:true,same:[],different:[]}";
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetMakePrice()
        {
            resultStr = "{success:false}";
            try
            {
                int pileId = 0;
                int.TryParse(Request.Params["pileId"] ?? "0", out pileId);
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    _combMgr = new ProductComboMgr(connectionString);
                    ProductComboCustom query = new ProductComboCustom { Parent_Id = int.Parse(Request.Params["ProductId"]), Pile_Id = pileId };
                    //複製商品時
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Parent_Id = int.Parse(Request.Form["OldProductId"]);
                    }
                    resultStr = "{success:true,same:" + JsonConvert.SerializeObject(_combMgr.combNoPriceQuery(query)) + ",different:" + JsonConvert.SerializeObject(_combMgr.differentNoPriceSpecQuery(query)) + "}";
                }
                else
                {
                    Caller _caller = (Session["caller"] as Caller);
                    _combTempMgr = new ProductComboTempMgr(connectionString);
                    ProductComboCustom query = new ProductComboCustom { Writer_Id = _caller.user_id, Pile_Id = pileId };
                    //複製商品時
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Parent_Id = int.Parse(Request.Form["OldProductId"]);
                    }
                    resultStr = "{success:true,same:" + JsonConvert.SerializeObject(_combTempMgr.comboPriceQuery(query)) + ",different:" + JsonConvert.SerializeObject(_combTempMgr.differentSpecQuery(query)) + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }


        [HttpPost]
        [CustomHandleError]
        public string groupNameQuery()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    List<ProductCombo> resultList = null;
                    _combMgr = new ProductComboMgr(connectionString);
                    resultList = _combMgr.groupNumQuery(new ProductCombo { Parent_Id = int.Parse(Request.Params["ProductId"]) });
                    json = JsonConvert.SerializeObject(resultList);

                }
                else
                {
                    List<ProductComboTemp> resultList = null;
                    Caller _caller = (Session["caller"] as Caller);
                    _combTempMgr = new ProductComboTempMgr(connectionString);
                    ProductComboTemp query = new ProductComboTemp { Writer_Id = _caller.user_id };
                    //複製商品時
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Parent_Id = Request.Form["OldProductId"];
                    }
                    resultList = _combTempMgr.groupNumQuery(query);
                    json = JsonConvert.SerializeObject(resultList);
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            return json;
        }


        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase combSpecSave()
        {
            resultStr = "{success:false}";
            try
            {
                Caller _caller = (Session["caller"] as Caller);
                string result = Request.Params["resultStr"];
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    #region 正式表
                    //組合商品之規格目前不支持修改
                    #endregion
                }
                else
                {
                    #region 臨時表
                    _combTempMgr = new ProductComboTempMgr(connectionString);
                    List<ProductComboTemp> comList = JsonConvert.DeserializeObject<List<ProductComboTemp>>(result);
                    string parent_id = "0";
                    //複製商品時
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        parent_id = Request.Form["OldProductId"];
                    }
                    foreach (ProductComboTemp item in comList)
                    {
                        item.Writer_Id = _caller.user_id;
                        item.Parent_Id = parent_id;
                    }

                    if (_combTempMgr.Save(comList))
                    {
                        resultStr = "{success:true}";
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }

        /// <summary>
        /// 組合規格發生變化時刪除相關數據（product_combo_temp,price_master_temp,item_price_temp...）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase combSpecTempDelete()
        {
            resultStr = "{success:false}";
            try
            {
                Caller _caller = (Session["caller"] as Caller);
                _combTempMgr = new ProductComboTempMgr(connectionString);
                ProductComboTemp delete = new ProductComboTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                //複製商品時
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    delete.Parent_Id = Request.Form["OldProductId"];
                }
                _combTempMgr.Delete(delete);
                resultStr = "{success:true}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }

        #endregion

        #region  價格
        public void CreateList(List<MakePriceCustom> PriceStore, PriceMaster pMaster, PriceMasterTemp pMasterTemp, string same_price, List<List<ItemPrice>> ItemPList, List<PriceMasterTemp> pMasterListT,
            List<PriceMaster> pMasterList, List<ItemPrice> update)
        {
            _productItemMgr = new ProductItemMgr(connectionString);

            if (PriceStore.Count > 0)
            {
                //先遍歷群組
                var piles = PriceStore.GroupBy(rec => rec.Pile_Id).ToList();
                piles.ForEach(rec =>
                {
                    if (same_price == "1")
                    {
                        //循環grid的每一行
                        rec.ToList().ForEach(row =>
                        {
                            //生成price_master
                            if (pMasterTemp != null)
                            {
                                PriceMasterTemp pmT = pMasterTemp.Clone() as PriceMasterTemp;
                                pmT.product_name = row.Product_Name;
                                pmT.child_id = row.Child_Id.ToString();
                                pmT.price = int.Parse(row.item_money.ToString());
                                pmT.max_price = int.Parse(row.item_money.ToString());   //edit by xinglu0624w reason 子商品需要给 max_price 值
                                pmT.max_event_price = 0;
                                pmT.cost = int.Parse(row.item_cost.ToString());
                                pmT.event_price = int.Parse(row.event_money.ToString());
                                pmT.event_cost = int.Parse(row.event_cost.ToString());
                                pmT.valid_start = pMasterTemp.valid_start;
                                pmT.valid_end = pMasterTemp.valid_end;
                                pMasterListT.Add(pmT);
                            }
                            else
                            {
                                PriceMaster priceMa = pMaster.Clone() as PriceMaster;
                                priceMa.product_name = row.Product_Name;
                                priceMa.child_id = int.Parse(row.Child_Id.ToString());
                                priceMa.price = int.Parse(row.item_money.ToString());
                                priceMa.max_price = int.Parse(row.item_money.ToString());
                                priceMa.max_event_price = 0;
                                priceMa.cost = int.Parse(row.item_cost.ToString());
                                priceMa.event_price = int.Parse(row.event_money.ToString());
                                priceMa.event_cost = int.Parse(row.event_cost.ToString());
                                priceMa.price_master_id = row.price_master_id;
                                pMasterList.Add(priceMa);
                            }
                            if (row.price_master_id == 0)//新增
                            {

                                //生成item_price
                                List<ProductItem> piList = _productItemMgr.Query(new ProductItem { Product_Id = row.Child_Id });
                                List<ItemPrice> iList = new List<ItemPrice>();
                                piList.ForEach(item =>
                                {
                                    ItemPrice iP = new ItemPrice();
                                    iP.item_id = item.Item_Id;
                                    iP.item_money = row.item_money;
                                    iP.item_cost = row.item_cost;
                                    iP.event_money = row.event_money;
                                    iP.event_cost = row.event_cost;
                                    iList.Add(iP);
                                });
                                ItemPList.Add(iList);
                            }
                            else //更新
                            {
                                ItemPrice ip = new ItemPrice();
                                ip.item_price_id = row.item_price_id;
                                ip.price_master_id = row.price_master_id;
                                ip.item_money = row.item_money;
                                ip.item_cost = row.item_cost;
                                ip.event_money = row.event_money;
                                ip.event_cost = row.event_cost;
                                update.Add(ip);
                            }
                        });
                    }
                    else   //處理各自定價-規格不同價
                    {

                        var children = rec.GroupBy(c => c.Child_Id).ToList();
                        children.ForEach(c =>
                        {
                            List<ItemPrice> differentPrice = new List<ItemPrice>();
                            //生成price_mater
                            var rowData = (from ci in c where ci.Child_Id == c.Key select new { ci.Product_Name, ci.price_master_id }).ToList().First();
                            if (pMasterTemp != null)
                            {
                                PriceMasterTemp pmT = pMasterTemp.Clone() as PriceMasterTemp;
                                pmT.product_name = rowData.Product_Name;
                                pmT.child_id = c.Key.ToString();
                                pmT.price = int.Parse(c.FirstOrDefault().item_money.ToString());
                                pmT.max_price = int.Parse(c.FirstOrDefault().item_money.ToString());
                                //edit by hufeng0813w 2014/06/16 Reason:規格不同價時子商品也需要cost,event_cost,event_money
                                pmT.max_event_price = int.Parse(c.FirstOrDefault().event_money.ToString());
                                pmT.cost = int.Parse(c.FirstOrDefault().item_cost.ToString());
                                pmT.event_price = int.Parse(c.FirstOrDefault().event_money.ToString());
                                pmT.event_cost = int.Parse(c.FirstOrDefault().event_cost.ToString());
                                //edit by hufeng0813w 2014/06/16
                                pMasterListT.Add(pmT);
                            }
                            else
                            {
                                PriceMaster priceMa = pMaster.Clone() as PriceMaster;
                                priceMa.price_master_id = uint.Parse(rowData.price_master_id.ToString());
                                priceMa.product_name = rowData.Product_Name;
                                priceMa.child_id = int.Parse(c.Key.ToString());
                                priceMa.price = int.Parse(c.FirstOrDefault().item_money.ToString());
                                priceMa.max_price = int.Parse(c.FirstOrDefault().item_money.ToString());
                                //edit by hufeng0813w 2014/06/16 Reason:規格不同價時子商品也需要cost,event_cost,event_money
                                priceMa.max_event_price = int.Parse(c.FirstOrDefault().event_money.ToString());
                                priceMa.cost = int.Parse(c.FirstOrDefault().item_cost.ToString());
                                priceMa.event_price = int.Parse(c.FirstOrDefault().event_money.ToString());
                                priceMa.event_cost = int.Parse(c.FirstOrDefault().event_cost.ToString());
                                //edit by hufeng0813w 2014/06/16
                                pMasterList.Add(priceMa);
                            }
                            c.ToList().ForEach(row =>
                            {
                                //生成item_price
                                ItemPrice iP = new ItemPrice();
                                iP.item_id = row.item_id;
                                iP.item_money = row.item_money;
                                iP.item_cost = row.item_cost;
                                iP.event_money = row.event_money;
                                iP.event_cost = row.event_cost;
                                iP.item_price_id = row.item_price_id;
                                iP.price_master_id = row.price_master_id;
                                differentPrice.Add(iP);
                                update.Add(iP);
                            });
                            ItemPList.Add(differentPrice);
                        });

                    }
                });

            }
            //為保證ItemPList.Count=pMasterListT.Count
            ItemPList.Add(null);

        }

        [HttpPost]
        [CustomHandleError]
        public string ComboPriceSave()
        {
            string json = "{success:true}";
            if (!PriceMaster.CheckProdName(Request.Form["product_name"]))
            {
                return "{success:false,msg:'" + Resources.Product.FORBIDDEN_CHARACTER + "'}";
            }
            ProductTemp pTemp = new ProductTemp();
            List<PriceMasterTemp> pMasterListT = new List<PriceMasterTemp>();
            List<PriceMaster> pMasterList = new List<PriceMaster>();

            List<List<ItemPrice>> ItemPList = new List<List<ItemPrice>>();
            PriceMasterTemp pMasterTemp = new PriceMasterTemp();


            if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
            {
                pTemp.Product_Id = Request.Form["OldProductId"];
                pMasterTemp.product_id = Request.Form["OldProductId"];
            }
            string paramValue = Request.Params["paramValue"];
            #region 參數
            int writer_id = (Session["caller"] as Caller).user_id;
            string product_name = PriceMaster.Product_Name_FM(Request.Params["product_name"]);
            string price_type = Request.Params["price_type"];
            string product_price_list = Request.Params["product_price_list"];
            string default_bonus_percent = Request.Params["default_bonus_percent"];
            string price = Request.Params["price"];
            string cost = Request.Params["cost"];
            string max_price = Request.Params["max_price"];
            string bonus_percent = Request.Params["bonus_percent"];
            string event_price = Request.Params["event_price"];
            string event_cost = Request.Params["event_cost"];
            string max_event_price = Request.Params["max_event_price"];
            string event_start = Request.Params["event_start"];
            string event_end = Request.Params["event_end"];
            string site_id = Request.Params["site_id"];
            string user_level = Request.Params["user_level"];
            string user_mail = Request.Params["user_mail"];
            string bag_check_money = Request.Params["bag_check_money"] == "" ? "0" : Request.Params["bag_check_money"];
            string accumulated_bonus = Request.Params["accumulated_bonus"];
            string bonus_percent_start = Request.Params["bonus_percent_start"];
            string bonus_percent_end = Request.Params["bonus_percent_end"];
            string same_price = Request.Params["same_price"];
            string show_listprice = Request.Params["show_listprice"];
            string valid_start = Request.Params["valid_start"];
            string valid_end = Request.Params["valid_end"];

            #endregion
            List<MakePriceCustom> PriceStore = new List<MakePriceCustom>();
            if (price_type == "2")//各自定價
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                string priceStr = Request.Params["priceStr"];
                PriceStore = jss.Deserialize<List<MakePriceCustom>>(priceStr);
            }

            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                #region 正式表操作
                List<PriceMaster> pMList = new List<PriceMaster>();

                //插入price_master
                _priceMasterMgr = new PriceMasterMgr(connectionString);
                _priceMasterTsMgr = new PriceMasterTsMgr(connectionString);
                PriceMaster pMaster = new PriceMaster();

                //查詢price_master
                if (!string.IsNullOrEmpty(Request.Params["price_master_id"]))
                {
                    pMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = uint.Parse(Request.Params["price_master_id"]) }).FirstOrDefault();
                }

                pMaster.product_id = uint.Parse(Request.Params["product_id"]);
                pMaster.child_id = int.Parse(Request.Params["product_id"]);
                pMaster.site_id = uint.Parse(site_id);


                uint userId = 0;
                if (!string.IsNullOrEmpty(Request.Form["user_mail"]))
                {
                    _usersMgr = new UsersMgr(connectionString);
                    System.Data.DataTable dt_User = _usersMgr.Query(Request.Form["user_mail"]);
                    if (dt_User != null && dt_User.Rows.Count > 0)
                    {
                        userId = Convert.ToUInt32(dt_User.Rows[0]["user_id"]);
                    }
                }
                if (userId != 0)
                {
                    pMaster.user_id = userId;
                }
                if (user_level != "")
                {
                    pMaster.user_level = uint.Parse(user_level);
                }
                pMaster.product_name = product_name;
                pMaster.bonus_percent = float.Parse(bonus_percent);
                pMaster.cost = int.Parse(cost);
                pMaster.price = int.Parse(price);
                pMaster.max_price = int.Parse(max_price);
                pMaster.max_event_price = int.Parse(max_event_price);
                pMaster.default_bonus_percent = float.Parse(default_bonus_percent);
                pMaster.event_price = int.Parse(event_price);
                pMaster.event_cost = int.Parse(event_cost);
                pMaster.same_price = int.Parse(same_price);
                pMaster.price_status = 2;//申請審核
                pMaster.accumulated_bonus = uint.Parse(accumulated_bonus);

                #region 時間 活動時間
                if (event_start != "")
                {
                    pMaster.event_start = Convert.ToUInt32(CommonFunction.GetPHPTime(event_start));
                }
                if (event_end != "")
                {
                    pMaster.event_end = Convert.ToUInt32(CommonFunction.GetPHPTime(event_end));
                }
                if (bonus_percent_start != "")
                {
                    pMaster.bonus_percent_start = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_percent_start));
                }
                if (bonus_percent_end != "")
                {
                    pMaster.bonus_percent_end = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_percent_end));
                }
                if (!string.IsNullOrEmpty(valid_start))
                {
                    pMaster.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(valid_start));
                }
                if (!string.IsNullOrEmpty(valid_end))
                {
                    pMaster.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(valid_end));
                }


                #endregion

                _functionMgr = new FunctionMgr(connectionString);
                string function = Request.Params["function"] ?? "";
                Function fun = _functionMgr.QueryFunction(function, "/ProductCombo");
                int functionid = fun == null ? 0 : fun.RowId;
                HistoryBatch batch = new HistoryBatch { functionid = functionid };
                batch.batchno = Request.Params["batch"] ?? "";
                batch.kuser = (Session["caller"] as Caller).user_email;

                List<ItemPrice> update = new List<ItemPrice>();
                if (price_type == "2")  //各自定價
                {
                    CreateList(PriceStore, pMaster, null, same_price, ItemPList, pMasterListT, pMasterList, update);
                }
                pMasterList.Add(pMaster);

                try
                {
                    //價格修改 申請審核
                    PriceUpdateApply priceUpdateApply = new PriceUpdateApply();
                    priceUpdateApply.apply_user = Convert.ToUInt32((Session["caller"] as Caller).user_id);

                    //價格審核記錄
                    PriceUpdateApplyHistory applyHistroy = new PriceUpdateApplyHistory();
                    applyHistroy.user_id = Convert.ToInt32(priceUpdateApply.apply_user);
                    applyHistroy.price_status = 1;
                    //applyHistroy.type = 3;
                    applyHistroy.type = 1;//edit by wwei0216w 所作操作為 1:申請審核的操作 

                    _priceUpdateApplyMgr = new PriceUpdateApplyMgr(connectionString);
                    _priceUpdateApplyHistoryMgr = new PriceUpdateApplyHistoryMgr(connectionString);
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);

                    ArrayList excuteSql = new ArrayList();
                    if (!string.IsNullOrEmpty(Request.Params["price_master_id"]))
                    {
                        #region 修改

                        priceUpdateApply.price_master_id = pMaster.price_master_id;
                        PriceMaster priceMster = _priceMasterMgr.QueryPMaster(new PriceMaster()
                        {
                            site_id = uint.Parse(site_id),
                            user_level = uint.Parse(user_level == "" ? "0" : user_level),
                            user_id = userId,
                            product_id = uint.Parse(Request.Params["product_id"]),
                            price_master_id = uint.Parse(Request.Params["price_master_id"])
                        });

                        //更新price_master
                        if (priceMster != null)
                        {
                            json = "{success:true,msg:'" + Resources.Product.SITE_EXIST + "'}";
                        }
                        else
                        {
                            int apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);
                            if (apply_id != -1)
                            {
                                bool flag = false;
                                foreach (var item in pMasterList)
                                {
                                    item.apply_id = (uint)apply_id;
                                    excuteSql = new ArrayList();
                                    if (item == pMaster)
                                    {
                                        pMaster.apply_id = Convert.ToUInt32(apply_id);
                                        applyHistroy.apply_id = apply_id;
                                        excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));
                                    }
                                    //excuteSql.Add(_priceMasterMgr.Update(item));
                                    excuteSql.Add(_priceMasterTsMgr.UpdateTs(item));//edit by xiangwang0413w 2014/07/16 將數據更新到pirce_master_ts表
                                    flag = _tableHistoryMgr.SaveHistory<PriceMaster>(item, batch, excuteSql);
                                }
                                if (flag)
                                {
                                    //更新item_price
                                    //_itemPriceMgr = new ItemPriceMgr("");
                                    _itemPriceTsMgr = new ItemPriceTsMgr(connectionString);
                                    foreach (var iPrice in update)
                                    {
                                        iPrice.apply_id = (uint)apply_id;
                                        excuteSql = new ArrayList();
                                        //excuteSql.Add(_itemPriceMgr.Update(iPrice));
                                        excuteSql.Add(_itemPriceTsMgr.UpdateTs(iPrice));//edit by xiangwang0413w 2014/07/17 將數據更新到pirce_master_ts表
                                        if (!_tableHistoryMgr.SaveHistory<ItemPrice>(iPrice, batch, excuteSql))
                                        {
                                            json = "{success:true,msg:'" + Resources.Product.EDIT_FAIL + "'}";
                                        }
                                    }
                                    json = "{success:true,msg:'" + Resources.Product.EDIT_SUCCESS + "'}";
                                }
                                else
                                {
                                    json = "{success:true,msg:'" + Resources.Product.EDIT_FAIL + "'}";
                                }
                            }
                            else
                            {
                                json = "{success:true,msg:'" + Resources.Product.EDIT_FAIL + "'}";
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 新增

                        string msg = string.Empty;

                        int status = 0;

                        List<ItemPrice> iprice = (ItemPList == null || ItemPList.Count == 0) ? null : ItemPList[pMasterList.IndexOf(pMaster)];
                        int priceMasterId = _priceMasterMgr.Save(pMaster, iprice, null, ref msg);
                        if (priceMasterId != -1)
                        {
                            priceUpdateApply.price_master_id = Convert.ToUInt32(priceMasterId);
                            int apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);

                            if (apply_id != -1)
                            {
                                pMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();
                                pMaster.apply_id = Convert.ToUInt32(apply_id);
                                excuteSql.Add(_priceMasterMgr.Update(pMaster));
                                excuteSql.Add(_priceMasterTsMgr.UpdateTs(pMaster)); //edit by xiangwang0413w 2014/07/22 更新price_master_ts表後同時更新price_master_ts表，以便價格審核

                                foreach (var item in pMasterList.FindAll(m => m.product_id != m.child_id))
                                {
                                    iprice = (ItemPList == null || ItemPList.Count == 0) ? null : ItemPList[pMasterList.IndexOf(item)];
                                    priceMasterId = _priceMasterMgr.Save(item, iprice, null, ref msg);
                                    pMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();

                                    if (priceMasterId != -1)
                                    {
                                        priceUpdateApply.price_master_id = Convert.ToUInt32(priceMasterId);
                                        pMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();
                                        pMaster.apply_id = Convert.ToUInt32(apply_id);
                                        excuteSql.Add(_priceMasterMgr.Update(pMaster));
                                        excuteSql.Add(_priceMasterTsMgr.UpdateTs(pMaster)); //edit by xiangwang0413w 2014/07/22 更新price_master_ts表後同時更新price_master_ts表，以便價格審核
                                    }
                                    else { status = 2; }
                                }
                            }
                            else { status = 3; }
                        }
                        else { status = 3; }

                        excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));
                        _tableHistoryMgr = new TableHistoryMgr(connectionString);
                        if (_tableHistoryMgr.SaveHistory<PriceMaster>(pMaster, batch, excuteSql))
                        {
                            status = 1;
                        }
                        else { status = 2; }

                        //foreach (var item in pMasterList)
                        //{
                        //    List<ItemPrice> iprice = (ItemPList == null || ItemPList.Count == 0) ? null : ItemPList[pMasterList.IndexOf(item)];
                        //    int priceMasterId = _priceMasterMgr.Save(item, iprice, null, ref msg);
                        //    if (apply_id != -1)
                        //    {
                        //        pMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();
                        //    }

                        //    if (priceMasterId != -1)
                        //    {
                        //        if (item != pMaster) { status = 1; continue; }
                        //        priceUpdateApply.price_master_id = Convert.ToUInt32(priceMasterId);
                        //        int apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);
                        //        if (apply_id != -1)
                        //        {
                        //            pMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();
                        //            pMaster.apply_id = Convert.ToUInt32(apply_id);
                        //            applyHistroy.apply_id = apply_id;

                        //            excuteSql.Add(_priceMasterMgr.Update(pMaster));
                        //            excuteSql.Add(_priceMasterTsMgr.UpdateTs(pMaster)); //edit by xiangwang0413w 2014/07/22 更新price_master_ts表後同時更新price_master_ts表，以便價格審核
                        //            excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));
                        //            _tableHistoryMgr = new TableHistoryMgr(connectionString);
                        //            if (_tableHistoryMgr.SaveHistory<PriceMaster>(pMaster, batch, excuteSql))
                        //            {
                        //                status = 1;
                        //            }
                        //            else { status = 2; }
                        //        }
                        //        else { status = 2; }
                        //    }
                        //    else { status = 3; }
                        //}

                        if (status == 1)
                        {
                            json = "{success:true,msg:'" + Resources.Product.ADD_SUCCESS + "'}";
                        }
                        else if (status == 2)
                        {
                            json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                        }
                        else
                        {
                            json = "{success:false,msg:'" + msg + "'}";
                        }

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                }
                #endregion
            }
            else
            {
                #region 新增至臨時表
                try
                {
                    _productTempMgr = new ProductTempMgr(connectionString);
                    _pMasterTempMgr = new PriceMasterTempMgr(connectionString);

                    //product_temp
                    pTemp.Product_Price_List = uint.Parse(product_price_list);
                    pTemp.Writer_Id = writer_id;
                    pTemp.Combo_Type = COMBO_TYPE;
                    pTemp.Price_type = int.Parse(price_type);
                    pTemp.Bag_Check_Money = uint.Parse(bag_check_money);
                    pTemp.show_listprice = uint.Parse(show_listprice);
                    pTemp.Bonus_Percent = float.Parse(bonus_percent);
                    pTemp.Default_Bonus_Percent = float.Parse(default_bonus_percent);

                    //Price_Master
                    pMasterTemp.product_name = product_name; ;
                    pMasterTemp.default_bonus_percent = float.Parse(default_bonus_percent);
                    pMasterTemp.writer_Id = writer_id;
                    pMasterTemp.combo_type = COMBO_TYPE;
                    //默認站臺1:吉甲地,(按統一價格比例拆分)會員等級1：普通會員
                    pMasterTemp.site_id = 1;
                    pMasterTemp.user_level = 1;
                    pMasterTemp.same_price = int.Parse(same_price);
                    pMasterTemp.accumulated_bonus = uint.Parse(accumulated_bonus);
                    pMasterTemp.bonus_percent = float.Parse(bonus_percent);
                    pMasterTemp.price = int.Parse(price);
                    pMasterTemp.cost = int.Parse(cost);
                    pMasterTemp.max_price = int.Parse(max_price);
                    pMasterTemp.max_event_price = int.Parse(max_event_price);
                    pMasterTemp.event_price = int.Parse(event_price);
                    pMasterTemp.event_cost = int.Parse(event_cost);
                    #region 時間 活動時間
                    if (event_start != "")
                    {
                        pMasterTemp.event_start = Convert.ToUInt32(CommonFunction.GetPHPTime(event_start));
                    }
                    if (event_end != "")
                    {
                        pMasterTemp.event_end = Convert.ToUInt32(CommonFunction.GetPHPTime(event_end));
                    }
                    if (bonus_percent_start != "")
                    {
                        pMasterTemp.bonus_percent_start = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_percent_start));
                        pTemp.Bonus_Percent_Start = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_percent_start));
                    }
                    if (bonus_percent_end != "")
                    {
                        pMasterTemp.bonus_percent_end = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_percent_end));
                        pTemp.Bonus_Percent_End = Convert.ToUInt32(CommonFunction.GetPHPTime(bonus_percent_end));
                    }
                    if (!string.IsNullOrEmpty(valid_start))
                    {
                        pMasterTemp.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(valid_start));
                    }
                    if (!string.IsNullOrEmpty(valid_end))
                    {
                        pMasterTemp.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(valid_end));
                    }

                    #endregion

                    pMasterTemp.price_status = 1;

                    _productItemMgr = new ProductItemMgr(connectionString);

                    string oldProductId = "0";
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        oldProductId = Request.Form["OldProductId"];
                    }

                    List<ItemPrice> update = new List<ItemPrice>();

                    if (price_type == "2")
                    {
                        _combTempMgr = new ProductComboTempMgr(connectionString);
                        List<ProductComboCustom> combResultList = _combTempMgr.priceComboQuery(new ProductComboCustom { Writer_Id = writer_id, Parent_Id = int.Parse(oldProductId.ToString()) });
                        bool match = true;
                        if (combResultList.Count() > 0)
                        {
                            int countBySearchPile = combResultList.GroupBy(m => m.Pile_Id).Count();      //庫中原有價格數據
                            int countByStorePile = PriceStore.GroupBy(m => m.Pile_Id).Count();           //頁面store數據
                            //判斷群組數量是否相同
                            if (countBySearchPile == countByStorePile)
                            {
                                for (int i = 1; i <= countBySearchPile; i++)
                                {
                                    //組合類型為固定或任選時pile_id為0;
                                    var tempSearch = combResultList.Where(m => m.Pile_Id == (combResultList[0].Pile_Id == 0 ? 0 : i)).ToList().GroupBy(m => m.Child_Id).ToList();
                                    var tempPrice = PriceStore.Where(m => m.Pile_Id == (combResultList[0].Pile_Id == 0 ? 0 : i)).ToList().GroupBy(m => m.Child_Id).ToList();
                                    //判斷當前組中子商品的數量是否相同
                                    if (tempSearch.Count() == tempPrice.Count())
                                    {
                                        foreach (var item in tempPrice)
                                        {
                                            if (tempSearch.Where(m => m.Key == item.Key.ToString()).ToList().Count() <= 0)//edit 2014/09/24
                                            {
                                                match = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        match = false;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                match = false;
                            }
                        }
                        else
                        {
                            match = false;
                        }

                        if (!match)
                        {
                            _combTempMgr = new ProductComboTempMgr(connectionString);

                            _combTempMgr.comboPriceDelete(new ProductComboTemp { Writer_Id = writer_id, Combo_Type = COMBO_TYPE, Parent_Id = oldProductId });

                            PriceStore.ForEach(rec => rec.price_master_id = 0);

                        }

                        CreateList(PriceStore, null, pMasterTemp, same_price, ItemPList, pMasterListT, pMasterList, update);
                    }

                    pMasterListT.Add(pMasterTemp);

                    //如果原價格為複製價格，則需刪除原複製價格並將child_id設為oldProductId
                    if (oldProductId != "0")
                    {
                        pMasterListT.ForEach(m =>
                        {
                            if (m.child_id == "0")
                            {
                                m.child_id = oldProductId;
                            }
                        });
                    }

                    //查詢                 

                    PriceMasterProductCustom queryReust = _pMasterTempMgr.Query(new PriceMasterTemp() { writer_Id = writer_id, child_id = oldProductId, product_id = oldProductId, combo_type = COMBO_TYPE });
                    //檢查價格類型是否有變動，如果有則刪除原有價格數據
                    if (queryReust != null)
                    {
                        if (!price_type.Equals(queryReust.price_type.ToString()))
                        {
                            _combTempMgr = new ProductComboTempMgr(connectionString);
                            _combTempMgr.comboPriceDelete(new ProductComboTemp { Writer_Id = writer_id, Combo_Type = COMBO_TYPE, Parent_Id = oldProductId.ToString() });
                            queryReust = null;
                        }
                    }

                    if (queryReust == null)//插入
                    {
                        if (price_type == "1")
                        {
                            _pMasterTempMgr.Save(pMasterListT, null, null);
                        }
                        else
                        {
                            _pMasterTempMgr.Save(pMasterListT, ItemPList, null);
                        }
                    }
                    else//更新
                    {
                        if (price_type == "1")
                        {
                            _pMasterTempMgr.Update(pMasterListT, null);
                        }
                        else
                        {
                            _pMasterTempMgr.Update(pMasterListT, update);
                        }
                    }
                    _productTempMgr.PriceBonusInfoSave(pTemp);

                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                }
                #endregion
            }
            return json;
        }

        #region 站臺價格狀態更改

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase UpdatePriceStatus()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["priceMasterId"]))
                {
                    uint priceMasterId = uint.Parse(Request.Form["priceMasterId"]);
                    _priceMasterMgr = new PriceMasterMgr(connectionString);
                    PriceMaster priceMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = priceMasterId }).FirstOrDefault();
                    if (priceMaster != null)
                    {
                        string status = Request.Form["updateStatus"];
                        if (status == "up")//上架
                        {
                            priceMaster.price_status = 1;

                        }
                        else if (status == "down")//下架
                        {
                            priceMaster.price_status = 4;
                        }


                        _functionMgr = new FunctionMgr(connectionString);
                        string function = Request.Params["function"] ?? "";
                        Function fun = _functionMgr.QueryFunction(function, "/ProductCombo");
                        int functionid = fun == null ? 0 : fun.RowId;
                        HistoryBatch batch = new HistoryBatch { functionid = functionid };
                        batch.batchno = Request.Params["batch"] ?? "";
                        batch.kuser = (Session["caller"] as Caller).user_email;

                        ArrayList sqls = new ArrayList();
                        sqls.Add(_priceMasterMgr.Update(priceMaster));
                        _tableHistoryMgr = new TableHistoryMgr(connectionString);
                        bool result = _tableHistoryMgr.SaveHistory<PriceMaster>(priceMaster, batch, sqls);
                        json = "{success:" + result.ToString().ToLower() + "}";
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

        //查詢價格
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryPriceMasterProduct()
        {
            string oldProductId = "0";
            if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
            {
                oldProductId = Request.Form["OldProductId"];
            }
            string json = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    int writer_id = (Session["caller"] as Caller).user_id;
                    _pMasterTempMgr = new PriceMasterTempMgr(connectionString);
                    PriceMasterProductCustom pmpCus = _pMasterTempMgr.Query(new PriceMasterTemp() { writer_Id = writer_id, combo_type = COMBO_TYPE, product_id = oldProductId, child_id = oldProductId }); //edit by wangwei 2014/9/26
                    json = "{success:true,data:" + JsonConvert.SerializeObject(pmpCus) + "}";
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }


        /// <summary>
        /// 價格類型發生變化時刪除原有價格數據
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase comboPriceTempDelete()
        {
            resultStr = "{success:false}";
            try
            {
                Caller _caller = (Session["caller"] as Caller);
                _combTempMgr = new ProductComboTempMgr(connectionString);
                ProductComboTemp delete = new ProductComboTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                //複製商品時
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    delete.Parent_Id = Request.Form["OldProductId"];
                }
                _combTempMgr.comboPriceDelete(delete);
                resultStr = "{success:true}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }

            Response.Clear();
            Response.Write(resultStr);
            Response.End();
            return this.Response;
        }

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase IsSameChildVendor()
        {
            string result = string.Empty;
            try
            {
                List<ProductComboCustom> combos;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    int parentId = int.Parse(Request.Params["ProductId"]);
                    _combMgr = new ProductComboMgr(connectionString);
                    combos = _combMgr.combQuery(new ProductComboCustom { Parent_Id = parentId });
                }
                else
                {
                    Caller _caller = (Session["caller"] as Caller);
                    _combTempMgr = new ProductComboTempMgr(connectionString);
                    ProductComboCustom query = new ProductComboCustom { Writer_Id = _caller.user_id };
                    //複製商品時
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Parent_Id = int.Parse(Request.Form["OldProductId"]);
                    }
                    combos = _combTempMgr.combQuery(query);
                }
                List<uint> brandIds = combos.GroupBy(m => m.brand_id).Select(m => m.Key).ToList();
                if (brandIds.Count == 1)
                {
                    result = "{success:true,same:true}";
                }
                else
                {
                    _vendorBrandMgr = new VendorBrandMgr(connectionString);
                    List<uint> vendorIds = new List<uint>();
                    VendorBrand vendorBrand;
                    foreach (var id in brandIds)
                    {
                        vendorBrand = _vendorBrandMgr.GetProductBrand(new VendorBrand { Brand_Id = id });
                        if (vendorBrand != null && !vendorIds.Contains(vendorBrand.Vendor_Id))
                        {
                            vendorIds.Add(vendorBrand.Vendor_Id);
                        }
                    }
                    result = "{success:true,same:" + (vendorIds.Count == 1).ToString().ToLower() + "}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                result = "{success:true,same:false}";
            }
            this.Response.Clear();
            this.Response.Write(result);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 查询商品站台信息

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetPriceMaster()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    uint productId = uint.Parse(Request.Params["ProductId"]);

                    _priceMasterMgr = new PriceMasterMgr(connectionString);
                    List<PriceMasterCustom> proSiteCustom = _priceMasterMgr.Query(new PriceMaster { product_id = productId, child_id = Convert.ToInt32(productId) });
                    StringBuilder strJson = new StringBuilder("[");
                    if (proSiteCustom != null)
                    {
                        _itemPriceMgr = new ItemPriceMgr(connectionString);
                        ItemPrice query = new ItemPrice { IsPage = true, Limit = 1 };
                        foreach (var item in proSiteCustom)
                        {
                            strJson.Append("{");
                            strJson.AppendFormat("price_master_id:{0},product_id:{1},site_id:{2},site_name:\"{3}\"", item.price_master_id, item.product_id, item.site_id, item.site_name);
                            strJson.AppendFormat(",product_name:\"{0}\",product_name_format:\"{1}\",bonus_percent:{2},default_bonus_percent:{3}", item.product_name, PriceMaster.Product_Name_Op(item.product_name), item.bonus_percent, item.default_bonus_percent);
                            strJson.AppendFormat(",user_level_name:\"{0}\",user_email:\"{1}\",user_level:{2}", item.user_level_name, item.user_email, item.user_level);
                            strJson.AppendFormat(",event_start:\"{0}\",user_id:{1}", item.event_start, item.user_id);
                            strJson.AppendFormat(",event_end:\"{0}\"", item.event_end);
                            strJson.AppendFormat(",cost:\"{0}\"", item.cost);
                            strJson.AppendFormat(",event_cost:\"{0}\"", item.event_cost);
                            strJson.AppendFormat(",price_status:\"{0}\"", item.price_status);
                            //if (item.same_price == 1)//edit by hufeng0813w 2014/06/16 Reason:所有單一商品規格不同價時 也去master表中的價格和活動價格
                            //{
                            strJson.AppendFormat(",price:\"{0}\"", item.price);
                            strJson.AppendFormat(",event_price:\"{0}\"", item.event_price);
                            //}
                            strJson.AppendFormat(",status:\"{0}\",accumulated_bonus:{1},bonus_percent_start:\"{2}\",bonus_percent_end:\"{3}\",same_price:{4},valid_start:\"{5}\",valid_end:\"{6}\"", item.status, item.accumulated_bonus, item.bonus_percent_start, item.bonus_percent_end, item.same_price, item.valid_start, item.valid_end);
                            strJson.Append("}");
                        }
                    }
                    strJson.Append("]");
                    json = strJson.ToString().Replace("}{", "},{");
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 類別
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetSelectedCage()
        {
            string resultStr = "{success:false}";

            try
            {
                string strCateId = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    #region 從正式表獲取
                    uint pid = uint.Parse(Request.Params["ProductId"]);
                    _productMgr = new ProductMgr(connectionString);
                    Product result = _productMgr.Query(new Product { Product_Id = pid }).FirstOrDefault();
                    if (result != null)
                    {
                        strCateId = result.Cate_Id;
                    }
                    #endregion
                }
                else
                {
                    Caller _caller = (Session["caller"] as Caller);
                    _productTempMgr = new ProductTempMgr(connectionString);
                    ProductTemp query = new ProductTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Product_Id = Request.Form["OldProductId"];
                    }

                    ProductTemp tempResult = _productTempMgr.GetProTemp(query);
                    if (tempResult != null)
                    {
                        strCateId = tempResult.Cate_Id;
                    }
                }

                paraMgr = new ParameterMgr(connectionString);
                Parametersrc cate2Result = paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_cate", ParameterCode = strCateId }).FirstOrDefault();
                if (cate2Result != null)
                {
                    Parametersrc cate1Result = paraMgr.QueryUsed(new Parametersrc { ParameterType = "product_cate", ParameterCode = cate2Result.TopValue.ToString() }).FirstOrDefault();
                    if (cate1Result != null)
                    {
                        StringBuilder stb = new StringBuilder("{");
                        stb.AppendFormat("cate1Name:\"{0}\",cate1Value:\"{1}\",cate1Rowid:\"{2}\",cate1TopValue:\"{3}\"", cate1Result.parameterName, cate1Result.ParameterCode, cate1Result.Rowid, cate1Result.TopValue);
                        stb.AppendFormat(",cate2Name:\"{0}\",cate2Value:\"{1}\",cate2Rowid:\"{2}\",cate2TopValue:\"{3}\"", cate2Result.parameterName, cate2Result.ParameterCode, cate2Result.Rowid, cate2Result.TopValue);
                        stb.Append("}");
                        resultStr = "{success:true,data:" + stb.ToString() + "}";
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


            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }

        #region 保存
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase tempCategoryAdd()
        {

            Caller _caller = (Session["caller"] as Caller);
            string resultStr = "{success:true}";
            string tempStr = Request.Params["result"];
            string cate_id = Request.Params["cate_id"];
            string deStr = Request["oldresult"] == null ? "0" : Request["oldresult"];

            List<ProductCategorySetTemp> saveTempList = new List<ProductCategorySetTemp>();
            try
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                List<ProductCategorySetCustom> cateCustomList = js.Deserialize<List<ProductCategorySetCustom>>(tempStr);
                if (string.IsNullOrEmpty(tempStr))
                {
                    cateCustomList = new List<ProductCategorySetCustom>();
                }


                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    #region 修改正式表
                    uint pid = uint.Parse(Request.Params["ProductId"]);
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    _categorySetMgr = new ProductCategorySetMgr(connectionString);
                    _productMgr = new ProductMgr(connectionString);

                    Product pro = new Product();
                    pro = _productMgr.Query(new Product { Product_Id = pid }).FirstOrDefault();
                    pro.Cate_Id = cate_id;

                    _functionMgr = new FunctionMgr(connectionString);
                    string function = Request.Params["function"] ?? "";
                    Function fun = _functionMgr.QueryFunction(function, "/ProductCombo");
                    int functionid = fun == null ? 0 : fun.RowId;
                    HistoryBatch batch = new HistoryBatch { functionid = functionid };
                    batch.batchno = Request.Params["batch"] ?? "";
                    batch.kuser = (Session["caller"] as Caller).user_email;

                    ArrayList sqls = new ArrayList();
                    sqls.Add(_categorySetMgr.Delete(new ProductCategorySet { Product_Id = pid }));
                    sqls.Add(_productMgr.Update(pro));
                    foreach (ProductCategorySetCustom item in cateCustomList)
                    {
                        item.Product_Id = pid;
                        item.Brand_Id = pro.Brand_Id;
                        sqls.Add(_categorySetMgr.Save(item));
                    }

                    if (!_tableHistoryMgr.SaveHistory<ProductCategorySetCustom>(cateCustomList, batch, sqls))
                    {
                        resultStr = "{success:false}";
                    }
                    #endregion
                }
                else
                {
                    #region 修改臨時表

                    string product_id = "0";
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        product_id = Request.Form["OldProductId"];
                    }
                    _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);
                    _productTempMgr = new ProductTempMgr(connectionString);
                    if (string.IsNullOrEmpty(tempStr))
                    {
                        bool result = _categoryTempSetMgr.Delete(new ProductCategorySetTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE, Product_Id = product_id }, deStr);
                    }
                    else
                    {
                        ProductCategorySetTemp saveTemp;
                        ProductTemp query = new ProductTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE, Product_Id = product_id.ToString() };
                        ProductTemp proTemp = _productTempMgr.GetProTemp(query);
                        foreach (ProductCategorySetCustom item in cateCustomList)
                        {
                            saveTemp = new ProductCategorySetTemp();
                            saveTemp.Writer_Id = _caller.user_id;
                            saveTemp.Product_Id = product_id;
                            saveTemp.Category_Id = item.Category_Id;
                            saveTemp.Brand_Id = proTemp.Brand_Id;
                            saveTemp.Combo_Type = COMBO_TYPE;
                            saveTempList.Add(saveTemp);
                        }

                        if (!_categoryTempSetMgr.Save(saveTempList))
                        {
                            resultStr = "{success:false}";
                        }
                    }

                    if (!_productTempMgr.CategoryInfoUpdate(new ProductTemp { Writer_Id = _caller.user_id, Cate_Id = cate_id, Combo_Type = COMBO_TYPE, Product_Id = product_id.ToString() }))
                    {
                        resultStr = "{success:false}";
                    }

                    #endregion
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                resultStr = "{success:false}";
            }

            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }

        #endregion

        public List<ProductCategoryCustom> getCate(List<ProductCategory> categoryList, uint fatherId)
        {
            var cateList = (from c in categoryList
                            where c.category_father_id == fatherId
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



        #region 得到前台分類
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetCatagory()
        {
            List<ProductCategory> categoryList = new List<ProductCategory>();
            List<ProductCategoryCustom> cateList = new List<ProductCategoryCustom>();
            int categoryType = 2; //用來區別查詢所有節點還是查詢新館的節點 //edit by wwei0216w 2014/12/25
            //從後臺獲取查詢所有節點還是查詢新館的節點
            categoryType = Request["categoryType"] == null ? 2 : Convert.ToInt32(Request["categoryType"]);

            string resultStr = "";
            try
            {
                _procateMgr = new ProductCategoryMgr(connectionString);
                categoryList = _procateMgr.QueryAll(new ProductCategory { });
                cateList = getCate(categoryList, 0);
                List<ProductCategorySet> resultList = new List<ProductCategorySet>();
                if (!string.IsNullOrEmpty(Request.Params["ProductId"]))
                {
                    _categorySetMgr = new ProductCategorySetMgr(connectionString);
                    resultList = _categorySetMgr.Query(new ProductCategorySet { Product_Id = uint.Parse(Request.Params["ProductId"]) });

                }
                else
                {
                    Caller _caller = (Session["caller"] as Caller);
                    ProductCategorySetTemp query = new ProductCategorySetTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                    _categoryTempSetMgr = new ProductCategorySetTempMgr(connectionString);
                    if (!string.IsNullOrEmpty(Request.Params["OldProductId"]))//edit by xiaohui 2014/09/24
                    {
                        query.Product_Id = Request.Params["OldProductId"];
                    }
                    resultList = (from c in _categoryTempSetMgr.Query(query)
                                  select new
                                  {
                                      Id = c.Id,
                                      Product_Id = c.Product_Id,
                                      Category_Id = c.Category_Id,
                                      Brand_Id = c.Brand_Id
                                  }).ToList().ConvertAll<ProductCategorySet>(m => new ProductCategorySet()
                                  {
                                      Id = m.Id,
                                      Product_Id = uint.Parse(m.Product_Id),
                                      Category_Id = m.Category_Id,
                                      Brand_Id = m.Brand_Id
                                  });
                }


                GetCategoryList(categoryList, ref cateList, resultList);

                switch (categoryType)
                {
                    case 0:
                        resultStr = JsonConvert.SerializeObject(cateList);
                        break;
                    case 1: //等於754      
                        cateList.FindAll(m => m.id == "2")[0].children.RemoveAll(m => m.id != "754");//移除新館節點
                        resultStr = JsonConvert.SerializeObject(cateList);
                        break;
                    case 2:
                        cateList.FindAll(m => m.id == "2")[0].children.RemoveAll(m => m.id == "754");//移除新館節點
                        resultStr = JsonConvert.SerializeObject(cateList);
                        break;
                }
                resultStr = resultStr.Replace("Checked", "checked");

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
        public void GetCategoryList(List<ProductCategory> categoryList, ref List<ProductCategoryCustom> catelist, List<ProductCategorySet> resultList)
        {
            foreach (ProductCategoryCustom item in catelist)
            {
                List<ProductCategoryCustom> childList = getCate(categoryList, uint.Parse(item.id.ToString()));
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

        #region 商品描述

        #region 商品標籤

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProTag()
        {
            string json = string.Empty;
            try
            {
                StringBuilder strJson = new StringBuilder();
                _productTagMgr = new ProductTagMgr(connectionString);
                List<ProductTag> tags = _productTagMgr.Query(new ProductTag { tag_status = 1 });
                if (tags != null)
                {
                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        uint productId = uint.Parse(Request.Form["ProductId"]);
                        _productTagSetMgr = new ProductTagSetMgr(connectionString);
                        List<ProductTagSet> tagSets = _productTagSetMgr.Query(new ProductTagSet { product_id = productId });
                        foreach (var item in tags)
                        {
                            strJson.AppendFormat("<input type='checkbox' id='tag_{0}' name='tags' value='{0}' ", item.tag_id);
                            if (tagSets.Exists(m => m.tag_id == item.tag_id))
                            {
                                strJson.Append("checked='true'");
                            }
                            strJson.AppendFormat("/><label for='tag_{0}'><img  src='{1}' style='margin-right:5px' /></label>", item.tag_id, imgServerPath + tagPath + GetDetailFolder(item.tag_filename) + item.tag_filename);
                        }
                    }
                    else
                    {
                        int writerId = (Session["caller"] as Caller).user_id;
                        _productTagSetTempMgr = new ProductTagSetTempMgr(connectionString);
                        ProductTagSetTemp query = new ProductTagSetTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE };
                        if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                        {
                            query.product_id = Request.Form["OldProductId"];
                        }
                        List<ProductTagSetTemp> tagSetTemps = _productTagSetTempMgr.Query(query);
                        foreach (var item in tags)
                        {
                            strJson.AppendFormat("<input type='checkbox' id='tag_{0}' name='tags' value='{0}' ", item.tag_id);
                            if (tagSetTemps.Exists(m => m.tag_id == item.tag_id))
                            {
                                strJson.Append("checked='true'");
                            }
                            strJson.AppendFormat("/><label for='tag_{0}'><img src='{1}' style='margin-right:5px' /></label>", item.tag_id, imgServerPath + tagPath + GetDetailFolder(item.tag_filename) + item.tag_filename);
                        }
                    }
                    json = strJson.ToString();
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

        #region 商品公告

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetProNotice()
        {
            string json = string.Empty;
            try
            {
                StringBuilder strJson = new StringBuilder();
                _productNoticeMgr = new ProductNoticeMgr(connectionString);
                List<ProductNotice> notices = _productNoticeMgr.Query(new ProductNotice { notice_status = 1 });
                if (notices != null)
                {
                    if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                    {
                        uint productId = uint.Parse(Request.Form["ProductId"]);
                        _productNoticeSetMgr = new ProductNoticeSetMgr(connectionString);
                        List<ProductNoticeSet> noticeSets = _productNoticeSetMgr.Query(new ProductNoticeSet { product_id = productId });
                        foreach (var item in notices)
                        {
                            strJson.AppendFormat("<input type='checkbox' id='notice_{0}' name='notices' value='{0}' ", item.notice_id);
                            if (noticeSets.Exists(m => m.notice_id == item.notice_id))
                            {
                                strJson.Append("checked='true'");
                            }
                            strJson.AppendFormat("/><label for='notice_{0}'>{1}</label>", item.notice_id, item.notice_name);
                        }
                    }
                    else
                    {
                        int writerId = (Session["caller"] as Caller).user_id;
                        _productNoticeSetTempMgr = new ProductNoticeSetTempMgr(connectionString);
                        _productTempMgr = new ProductTempMgr(connectionString);
                        ProductNoticeSetTemp query = new ProductNoticeSetTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE };
                        if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                        {
                            query.product_id = Request.Form["OldProductId"];
                        }
                        List<ProductNoticeSetTemp> noticeSetTemps = _productNoticeSetTempMgr.Query(query);
                        ProductTemp proTemp = _productTempMgr.GetProTemp(new ProductTemp { Writer_Id = writerId, Combo_Type = COMBO_TYPE });
                        bool check = (proTemp != null && !string.IsNullOrEmpty(proTemp.Page_Content_2)) ? false : true;
                        foreach (var item in notices)
                        {
                            strJson.AppendFormat("<input type='checkbox' id='notice_{0}' name='notices' value='{0}' ", item.notice_id);
                            if (check || noticeSetTemps.Exists(m => m.notice_id == item.notice_id))
                            {
                                strJson.Append("checked='true'");
                            }
                            strJson.AppendFormat("/><label for='notice_{0}'>{1}</label>", item.notice_id, item.notice_name);
                        }
                    }
                    json = strJson.ToString();
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

        #region 保存商品描述 標籤 公告

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase SaveDescription()
        {
            string json = string.Empty;
            try
            {
                string tags = Request.Form["Tags"] ?? "";
                string notices = Request.Form["Notice"] ?? "";
                JavaScriptSerializer jsSer = new JavaScriptSerializer();

                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    uint product_id = uint.Parse(Request.Form["ProductId"]);
                    _productMgr = new ProductMgr(connectionString);
                    Product product = _productMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();
                    if (product != null)
                    {
                        product.Page_Content_1 = Request.Form["page_content_1"] ?? "";
                        product.Page_Content_2 = Request.Form["page_content_2"] ?? "";
                        product.Page_Content_3 = Request.Form["page_content_3"] ?? "";
                        product.Product_Keywords = Request.Form["product_keywords"] ?? "";
                        if (!string.IsNullOrEmpty(Request.Form["product_buy_limit"]))
                        {
                            product.Product_Buy_Limit = uint.Parse(Request.Form["product_buy_limit"]);
                        }
                        ArrayList sqls = new ArrayList();
                        sqls.Add(_productMgr.Update(product));
                        //TAG
                        List<ProductTagSet> tagSets = jsSer.Deserialize<List<ProductTagSet>>(tags);
                        tagSets.ForEach(m => m.product_id = product_id);
                        _productTagSetMgr = new ProductTagSetMgr("");
                        sqls.Add(_productTagSetMgr.Delete(new ProductTagSet { product_id = product_id }));
                        tagSets.ForEach(m => sqls.Add(_productTagSetMgr.Save(m)));
                        //NOTICE
                        List<ProductNoticeSet> noticeSets = jsSer.Deserialize<List<ProductNoticeSet>>(notices);
                        noticeSets.ForEach(m => m.product_id = product_id);
                        _productNoticeSetMgr = new ProductNoticeSetMgr("");
                        sqls.Add(_productNoticeSetMgr.Delete(new ProductNoticeSet { product_id = product_id }));
                        noticeSets.ForEach(m => sqls.Add(_productNoticeSetMgr.Save(m)));

                        _functionMgr = new FunctionMgr(connectionString);
                        string function = Request.Params["function"] ?? "";
                        Function fun = _functionMgr.QueryFunction(function, "/ProductCombo");
                        int functionid = fun == null ? 0 : fun.RowId;
                        HistoryBatch batch = new HistoryBatch { functionid = functionid };
                        batch.batchno = Request.Params["batch"] ?? "";
                        batch.kuser = (Session["caller"] as Caller).user_email;

                        _tableHistoryMgr = new TableHistoryMgr(connectionString);
                        if (_tableHistoryMgr.SaveHistory<Product>(product, batch, sqls))
                        {
                            json = "{success:true}";
                        }
                        else
                        {
                            json = "{success:false}";
                        }
                    }
                }
                else
                {
                    int writer_id = (Session["caller"] as Caller).user_id;
                    string product_id = "0";
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        product_id = Request.Form["OldProductId"];
                    }
                    ProductTemp proTemp = new ProductTemp();
                    proTemp.Page_Content_1 = Request.Form["page_content_1"] ?? "";
                    proTemp.Page_Content_2 = Request.Form["page_content_2"] ?? "";
                    proTemp.Page_Content_3 = Request.Form["page_content_3"] ?? "";
                    proTemp.Product_Keywords = Request.Form["product_keywords"] ?? "";
                    if (!string.IsNullOrEmpty(Request.Form["product_buy_limit"]))
                    {
                        proTemp.Product_Buy_Limit = uint.Parse(Request.Form["product_buy_limit"]);
                    }
                    proTemp.Writer_Id = writer_id;
                    proTemp.Combo_Type = COMBO_TYPE;
                    proTemp.Product_Id = product_id.ToString();
                    List<ProductTagSetTemp> tagTemps = jsSer.Deserialize<List<ProductTagSetTemp>>(tags);
                    foreach (ProductTagSetTemp item in tagTemps)
                    {
                        item.Writer_Id = writer_id;
                        item.Combo_Type = COMBO_TYPE;
                        item.product_id = product_id;
                    }
                    List<ProductNoticeSetTemp> noticeTemps = jsSer.Deserialize<List<ProductNoticeSetTemp>>(notices);
                    foreach (ProductNoticeSetTemp item in noticeTemps)
                    {
                        item.Writer_Id = writer_id;
                        item.Combo_Type = COMBO_TYPE;
                        item.product_id = product_id;
                    }

                    _productTempMgr = new ProductTempMgr(connectionString);
                    if (_productTempMgr.DescriptionInfoSave(proTemp, tagTemps, noticeTemps))
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

        #endregion

        #region 庫存

        [HttpPost]
        public string QueryItemStock()
        {
            string json = string.Empty;

            _productItemMgr = new ProductItemMgr(connectionString);

            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                try
                {
                    List<StockDataCustom> stockCustomList = _productItemMgr.QueryItemStock(int.Parse(Request.Params["product_id"]), int.Parse(Request.Params["pile_id"]));
                    json = "{items:" + JsonConvert.SerializeObject(stockCustomList) + "}";

                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    json = "[]";
                }


            }

            return json;
        }

        #endregion

        #region 抽獎

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase fortuneSave()
        {
            string resultStr = "{success:true}";
            Caller _caller = (Session["caller"] as Caller);

            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    #region 保存正式表

                    _productMgr = new ProductMgr(connectionString);
                    Product pro = _productMgr.Query(new Product { Product_Id = uint.Parse(Request.Form["ProductId"]) }).FirstOrDefault();
                    pro.Fortune_Quota = uint.Parse(Request.Params["Fortune_Quota"]);
                    pro.Fortune_Freight = uint.Parse(Request.Params["Fortune_Freight"]);


                    if (!_productMgr.ExecUpdate(pro))
                    {
                        resultStr = "{success:false}";
                    }
                    #endregion

                }
                else
                {
                    #region 保存臨時表
                    ProductTemp pTemp = new ProductTemp();
                    pTemp.Fortune_Quota = uint.Parse(Request.Params["Fortune_Quota"]);
                    pTemp.Fortune_Freight = uint.Parse(Request.Params["Fortune_Freight"]);
                    pTemp.Writer_Id = _caller.user_id;
                    pTemp.Combo_Type = COMBO_TYPE;
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        pTemp.Product_Id = Request.Form["OldProductId"];
                    }

                    _productTempMgr = new ProductTempMgr(connectionString);
                    bool saveFortuneTempResult = _productTempMgr.FortuneInfoSave(pTemp);
                    if (!saveFortuneTempResult)
                    {
                        resultStr = "{success:false}";
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                resultStr = "{success:false}";
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

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase fortuneQuery()
        {
            string resultStr = "{success:false}";
            Caller _caller = (Session["caller"] as Caller);
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["ProductId"]))
                {
                    //product
                    _productMgr = new ProductMgr(connectionString);
                    Product pro = null;
                    pro = _productMgr.Query(new Product { Product_Id = uint.Parse(Request.Params["ProductId"]) }).FirstOrDefault();
                    if (pro != null)
                    {
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(pro) + "}";
                    }

                }
                else
                {
                    //product_temp
                    _productTempMgr = new ProductTempMgr(connectionString);
                    ProductTemp result = null;
                    ProductTemp query = new ProductTemp { Writer_Id = _caller.user_id, Combo_Type = COMBO_TYPE };
                    if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                    {
                        query.Product_Id = Request.Form["OldProductId"];
                    }
                    result = _productTempMgr.GetProTemp(query);


                    if (result != null)
                    {
                        resultStr = "{success:true,data:" + JsonConvert.SerializeObject(result) + "}";
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


            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 臨時表數據匯入正式表

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase Temp2Pro()
        {
            string json = string.Empty;
            try
            {
                int writerId = (Session["caller"] as Caller).user_id;
                _productMgr = new ProductMgr(connectionString);
                string product_id = "0";
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    product_id = Request.Form["OldProductId"];
                }
                int productId = _productMgr.TempMove2Pro(writerId, COMBO_TYPE, product_id);
                if (productId != -1)
                {
                    json = "{success:true,productId:" + productId + ",msg:'" + Resources.Product.SAVE_SUCCESS + "'}";
                }
                else
                {
                    json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 查询站臺

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetSite()
        {
            string json = string.Empty;
            try
            {
                _siteMgr = new SiteMgr(connectionString);
                json = JsonConvert.SerializeObject(_siteMgr.Query(new Site()));
                // json = "[{\"Site_Id\":\"1\",\"Site_Name\":\"吉甲地\"},{\"Site_Id\":\"2\",\"Site_Name\":\"天貓\"}]";


            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "[]";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 查詢組合商品下單一商品價格信息
        public HttpResponseBase QuerySingleProPrice()
        {
            string json = string.Empty;
            _priceMasterMgr = new PriceMasterMgr(connectionString);
            if (!string.IsNullOrEmpty(Request.Params["product_id"]))
            {
                try
                {
                    if (int.Parse(Request.Params["Price_type"]) == 2)
                    {
                        var singleList = new List<SelfSingleProductPrice>();
                        singleList = _priceMasterMgr.SingleProductPriceQuery(uint.Parse(Request.Params["product_id"]));
                        json = "{data:" + JsonConvert.SerializeObject(singleList) + "}";
                    }
                    else
                    {
                        var singleList = new List<SingleProductPrice>();
                        singleList = _priceMasterMgr.SingleProductPriceQuery(uint.Parse(Request.Params["product_id"]), int.Parse(Request.Params["pile_id"]));
                        json = "{data:" + JsonConvert.SerializeObject(singleList) + "}";
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
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 刪除臨時數據

        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase DeleteTempPro()
        {
            string json = string.Empty;
            try
            {
                int writerId = (Session["caller"] as Caller).user_id;
                string product_id = "0";
                if (!string.IsNullOrEmpty(Request.Form["OldProductId"]))
                {
                    product_id = Request.Form["OldProductId"];
                }
                _productTempMgr = new ProductTempMgr(connectionString);
                //刪除服務器上對應的圖片
                DeletePicOnServer(true, true, true, product_id, 1);//刪除商品說明臨時表
                DeletePicOnServer(true, true, true, product_id, 2);//刪除APP臨時表
                if (_productTempMgr.DeleteTemp(writerId, COMBO_TYPE, product_id))
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
        #endregion*/

    }
}
