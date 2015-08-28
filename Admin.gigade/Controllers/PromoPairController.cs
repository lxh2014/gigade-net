#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromoPairController.cs 
 * 摘   要： 
 *      點數累積
 * 当前版本：v1.1 
 * 作   者： jialei0706h
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：hongfei0416j
 *      v1.1修改内容：代碼合併、添加注釋
 */
#endregion
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
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
 
namespace Admin.gigade.Controllers
{
    public class PromoPairController : Controller
    {
        //
        // GET: /PromoPair/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPromoPairImplMgr _promopairMgr;
        //private IParametersrcImplMgr _parasrcMgr;
        private IProductCategoryImplMgr _produCateMgr = new ProductCategoryMgr(mySqlConnectionString);
        private IUserConditionImplMgr _ucMgr = new UserConditionMgr(mySqlConnectionString);
        //獲取webconfig裡面的存取路徑
        //上傳圖片
        string promoPath = ConfigurationManager.AppSettings["promoPairPath"];//圖片地址
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.promoPairPath);//圖片保存路徑
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com  phpWeb_host
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        string phpwebhost = ConfigurationManager.AppSettings["phpWeb_host"] == "" ? "www.gigade100.com" : ConfigurationManager.AppSettings["phpWeb_host"];
        //end 上傳圖片

        public ActionResult Index()
        {
            return View();
        }
        #region 紅配綠列表頁 +HttpResponseBase Promolist()
        [CustomHandleError]
        public HttpResponseBase Promolist()
        {
            List<PromoPairQuery> store = new List<PromoPairQuery>();
            string json = string.Empty;
            try
            {
                PromoPairQuery query = new PromoPairQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                _promopairMgr = new PromoPairMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _promopairMgr.Query(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                foreach (var item in store)
                {
                    if (item.banner_image != "")
                    {
                        item.banner_image = imgServerPath + promoPath + item.banner_image;
                    }
                    else
                    {
                        item.banner_image = defaultImg;
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

        #region 紅配綠第一步 + SaveOne()
        /// <summary>
        /// 紅配綠Insert頁面
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase SaveOne()
        {
            string jsonStr = String.Empty;
            try
            {
                PromoPair model = new PromoPair();
                _promopairMgr = new PromoPairMgr(mySqlConnectionString);
                model.event_type = "P1";

                model.event_name = Request.Params["name"].ToString();
                model.event_desc = Request.Params["desc"].ToString();
                model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.created = DateTime.Now;
                model.muser = model.kuser;
                model.modified = model.created;
                model.id = _promopairMgr.Save(model); // _promopairMgr.Save(model);
                System.Data.DataTable dt = _promopairMgr.Select(model);  //_promopairMgr.Select(query);
                if (dt.Rows.Count > 0)
                {
                    jsonStr = "{success:true,id:" + dt.Rows[0]["id"] + ",cateID:" + dt.Rows[0]["category_id"] + ",cate_red:" + dt.Rows[0]["cate_red"] + ",cate_green:" + dt.Rows[0]["cate_green"] + "}";
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
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 紅配綠第二步 + SaveTwo
        [CustomHandleError]
        public HttpResponseBase SaveTwo()
        {
            _promopairMgr = new PromoPairMgr(mySqlConnectionString);
            string jsonStr = String.Empty;
            try
            {
                PromoPair model = new PromoPair();
                PromoPair oldermodel = new PromoPair();
                PromoPairQuery PPQuery = new PromoPairQuery();
                PromoPairQuery oldPPQuery = new PromoPairQuery();
                model.id = Convert.ToInt32(Request.Params["rowid"].ToString());
                oldermodel = _promopairMgr.GetModelById(model.id);
                model.category_id = Convert.ToInt32(Request.Params["categoryid"].ToString());
                model.deliver_type = Convert.ToInt32(Request.Params["deliver_id"].ToString());
                model.device = Request.Params["device_id"].ToString();
                model.starts = Convert.ToDateTime(Request.Params["start_date"].ToString());
                model.end = Convert.ToDateTime(Request.Params["end_date"].ToString());
                model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.modified = DateTime.Now;
                model.event_type = "P1";
                PPQuery.event_id = CommonFunction.GetEventId(model.event_type, model.id.ToString());
                PPQuery.category_ipfrom = Request.UserHostAddress;
                if (!string.IsNullOrEmpty(Request.Params["side"].ToString()))//修改時傳的值為siteName
                {
                    Regex reg = new Regex("^([0-9]+,)*[0-9]+$");
                    if (reg.IsMatch(Request.Params["side"].ToString()))
                    {
                        model.website = Request.Params["side"].ToString();// 將站台改為多選 edit by shuangshuang0420j 20140925 10:08
                    }
                    else
                    {
                        model.website = oldermodel.website;
                    }
                }
                //if (Request.Params["side"].ToString() != "" && Request.Params["side"].ToString() != null)
                //{
                //    model.website = Request.Params["side"].ToString();
                //}
                //else
                //    model.website = oldermodel.website;

                Random rand = new Random();
                int newRand = rand.Next(1000, 9999);
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

                    string localPromoPath = imgLocalPath + promoPath;//圖片存儲地址


                    FileManagement fileLoad = new FileManagement();

                    for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                    {
                        HttpPostedFileBase file = Request.Files[iFile];
                        string fileName = string.Empty;//當前文件名

                        string fileExtention = string.Empty;//當前文件的擴展名

                        fileName = Path.GetFileName(file.FileName);

                        // string returnName = imgServerPath;
                        bool result = false;
                        string NewFileName = string.Empty;

                        fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                        NewFileName = PPQuery.event_id + newRand + fileExtention;

                        string ServerPath = string.Empty;
                        //判斷目錄是否存在，不存在則創建
                        //string[] mapPath = new string[1];
                        //mapPath[0] = promoPath.Substring(1, promoPath.Length - 2);
                        //string s = localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1);
                        CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));

                        //  returnName += promoPath + NewFileName;
                        fileName = NewFileName;
                        NewFileName = localPromoPath + NewFileName;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                        string ErrorMsg = string.Empty;


                        //上傳之前刪除已有的圖片
                        string oldFileName = oldPPQuery.banner_image;
                        FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                        List<string> tem = ftp.GetFileList();
                        //if (tem.Contains(oldFileName))
                        //{
                        //FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                        //ftps.DeleteFile(localPromoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                        //DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                        //}

                        try
                        {
                            //上傳
                            result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                PPQuery.banner_image = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            PPQuery.banner_image = PPQuery.event_id + newRand + ".jpg";
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message + "圖片上傳失敗");
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    PPQuery.banner_image = PPQuery.event_id + newRand + ".jpg";
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }

                #endregion

                #region  注釋上傳圖片
                //string nowtime = DateTime.Now.ToString("hhmm");
                //try
                //{
                //    string saveFoler = Server.MapPath("~/aimg.gigade100.com/active/");
                //    string savePath, fileName;
                //    for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                //    {
                //        HttpPostedFileBase postedFile = Request.Files[iFile];
                //        fileName = Path.GetFileName(postedFile.FileName);
                //        if (fileName != "")
                //        {
                //            string fileType = fileName.Substring(fileName.LastIndexOf("."));
                //            string newName = GetEventId(model.event_type, model.id.ToString()) + nowtime + fileType;
                //            savePath = saveFoler + newName;                            
                //            if (System.IO.File.Exists(savePath))
                //            {//检查是否在服务器上已经存在用户上传的同名文件 
                //                System.IO.File.Delete(savePath);
                //            }
                //            if (fileType.ToLower() == ".jpg" || fileType.ToLower() == ".png" || fileType.ToLower() == ".gif" || fileType.ToLower() == ".jpeg")
                //            {
                //                if (postedFile.ContentLength <= 300 * 1024)
                //                {
                //                    postedFile.SaveAs(savePath);
                //                    PPQuery.banner_image = newName;
                //                }
                //                else
                //                {
                //                    jsonStr = "{success:false,msg:1}";
                //                    this.Response.Clear();
                //                    this.Response.Write(jsonStr.ToString());
                //                    this.Response.End();
                //                    return this.Response;
                //                }
                //            }
                //            else
                //            {
                //                jsonStr = "{success:false,msg:2}";
                //                this.Response.Clear();
                //                this.Response.Write(jsonStr.ToString());
                //                this.Response.End();
                //                return this.Response;
                //            }
                //        }
                //        else
                //        {
                //            PPQuery.banner_image = oldPPQuery.banner_image;
                //        }
                //    }
                //}
                //catch (Exception)
                //{
                //    PPQuery.banner_image = oldPPQuery.banner_image;
                //}
                #endregion
                try//存連接地址
                {
                    PPQuery.category_link_url = phpwebhost + "/pair/red_green_match.php?event_id=" + CommonFunction.GetEventId(model.event_type, model.id.ToString());
                }
                catch (Exception ex)
                {
                    PPQuery.category_link_url = oldPPQuery.category_link_url;
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                }
                #region 會員群組會員管理
                if (Request.Params["group_id"].ToString() != "")
                {
                    try//group_id
                    {
                        model.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());
                    }
                    catch (Exception)
                    {
                        model.group_id = oldermodel.group_id;
                    }
                    model.condition_id = 0;
                }
                else if (Request.Params["condition_id"].ToString() != "")
                {
                    try//condition_id
                    {
                        model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                    }
                    catch (Exception ex)
                    {
                        model.condition_id = oldermodel.condition_id;
                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                    }
                    model.group_id = 0;
                }
                #endregion
                #region 紅+綠
                if (Request.Params["price"].ToString() != "")
                {
                    try//price
                    {
                        model.price = Convert.ToInt32(Request.Params["price"].ToString());
                    }
                    catch (Exception)
                    {
                        model.price = oldermodel.group_id;
                    }
                    model.discount = 0;
                }
                else if (Request.Params["discount"].ToString() != "")
                {
                    try//discount
                    {
                        model.discount = Convert.ToInt32(Request.Params["discount"].ToString());
                    }
                    catch (Exception)
                    {
                        model.discount = oldermodel.condition_id;
                    }
                    model.price = 0;
                }
                #endregion
                model.kuser = oldermodel.kuser;
                model.created = oldermodel.created;
                PPQuery.created = DateTime.Now;
                PPQuery.event_id = CommonFunction.GetEventId(model.event_type, model.id.ToString());
                model.category_id = Convert.ToInt32(_promopairMgr.SelCategoryID(model).Rows[0][0].ToString());
                if (_promopairMgr.CategoryID(model).ToString() == "true")
                {
                    _promopairMgr.SaveTwo(model, PPQuery);
                    jsonStr = "{success:true}";
                }
                else
                {
                    jsonStr = "{success:false,msg:3}";
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

        #region 編輯 + HttpResponseBase PromoPairEdit()
        public HttpResponseBase PromoPairEdit()
        {
            string jsonStr = String.Empty;
            _promopairMgr = new PromoPairMgr(mySqlConnectionString);
            PromoPair model = new PromoPair();
            PromoPair oldermodel = new PromoPair();
            PromoPairQuery PPQuery = new PromoPairQuery();
            ProductCategory olderpcmodel = new ProductCategory();
            PromoPairQuery oldPPQuery = new PromoPairQuery();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    model.id = Convert.ToInt32(Request.Params["rowid"].ToString());
                    model.category_id = Convert.ToInt32(Request.Params["categoryid"].ToString());
                    oldermodel = _promopairMgr.GetModelById(model.id);
                    olderpcmodel = _produCateMgr.GetModelById(Convert.ToUInt32(model.category_id));
                    model.event_name = Request.Params["event_name"].ToString();
                    model.event_desc = Request.Params["event_desc"].ToString();
                    model.event_type = oldermodel.event_type;
                    #region 會員群組 會員條件
                    if (Request.Params["group_id"].ToString() != "")
                    {
                        try//group_id
                        {
                            model.group_id = Convert.ToInt32(Request.Params["group_id"].ToString());
                        }
                        catch (Exception)
                        {
                            model.group_id = oldermodel.group_id;
                        }

                        if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                        {
                            UserCondition uc = new UserCondition();
                            uc.condition_id = Convert.ToInt32(Request.Params["condition_id"]);
                            if (_ucMgr.Delete(uc) > 0)
                            {
                                jsonStr = "{success:true}";

                            }
                            else
                            {
                                jsonStr = "{success:false,msg:'user_condition刪除出錯！'}";
                                this.Response.Clear();
                                this.Response.Write(jsonStr.ToString());
                                this.Response.End();
                                return this.Response;
                            }
                        }
                        model.condition_id = 0;
                    }
                    else if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
                    {
                        try//condition_id
                        {
                            model.condition_id = Convert.ToInt32(Request.Params["condition_id"].ToString());
                        }
                        catch (Exception)
                        {
                            model.condition_id = oldermodel.condition_id;
                        }
                        model.group_id = 0;
                    }
                    #endregion
                    model.deliver_type = Convert.ToInt32(Request.Params["deliver_id"].ToString());
                    model.device = Request.Params["device_id"].ToString();
                    //model.website = Request.Params["side"].ToString();
                    if (!string.IsNullOrEmpty(Request.Params["side"].ToString()))//修改時傳的值為siteName
                    {
                        Regex reg = new Regex("^([0-9]+,)*[0-9]+$");
                        if (reg.IsMatch(Request.Params["side"].ToString()))
                        {
                            model.website = Request.Params["side"].ToString();// 將站台改為多選 edit by shuangshuang0420j 20140925 10:08
                        }
                        else
                        {
                            model.website = oldermodel.website;
                        }
                    }
                    #region 紅+綠
                    if (Request.Params["price"].ToString() != "")
                    {
                        try//price
                        {
                            model.price = Convert.ToInt32(Request.Params["price"].ToString());
                        }
                        catch (Exception)
                        {
                            model.price = oldermodel.group_id;
                        }
                        model.discount = 0;
                    }
                    else if (Request.Params["discount"].ToString() != "")
                    {
                        try//discount
                        {
                            model.discount = Convert.ToInt32(Request.Params["discount"].ToString());
                        }
                        catch (Exception)
                        {
                            model.discount = oldermodel.condition_id;
                        }
                        model.price = 0;
                    }
                    #endregion
                    model.starts = Convert.ToDateTime(Request.Params["starts"].ToString());
                    model.end = Convert.ToDateTime(Request.Params["end"].ToString());
                    model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.modified = DateTime.Now;
                    PPQuery.event_id = CommonFunction.GetEventId(model.event_type, model.id.ToString());
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

                        string localPromoPath = imgLocalPath + promoPath;//圖片存儲地址

                        Random rand = new Random();
                        int newRand = rand.Next(1000, 9999);

                        FileManagement fileLoad = new FileManagement();

                        for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                        {
                            HttpPostedFileBase file = Request.Files[iFile];
                            string fileName = string.Empty;//當前文件名
                            string fileExtention = string.Empty;//當前文件的擴展名
                            fileName = Path.GetFileName(file.FileName);
                            // string returnName = imgServerPath;
                            bool result = false;
                            string NewFileName = string.Empty;

                            fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                            NewFileName = PPQuery.event_id + newRand + fileExtention;

                            string ServerPath = string.Empty;
                            //判斷目錄是否存在，不存在則創建
                            //string[] mapPath = new string[1];
                            //mapPath[0] = promoPath.Substring(1, promoPath.Length - 2);
                            //string s = localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1);
                            CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - promoPath.Length + 1), promoPath.Substring(1, promoPath.Length - 2).Split('/'));

                            //  returnName += promoPath + NewFileName;
                            fileName = NewFileName;
                            NewFileName = localPromoPath + NewFileName;//絕對路徑
                            ServerPath = Server.MapPath(imgLocalServerPath + promoPath);
                            string ErrorMsg = string.Empty;


                            //上傳之前刪除已有的圖片
                            string oldFileName = olderpcmodel.banner_image;
                            FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
                                ftps.DeleteFile(localPromoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                            }
                            try
                            {
                                //上傳
                                result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                                if (result)//上傳成功
                                {
                                    PPQuery.banner_image = fileName;
                                }
                            }
                            catch (Exception)
                            {
                                PPQuery.banner_image = olderpcmodel.banner_image;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        PPQuery.banner_image = olderpcmodel.banner_image;
                    }
                    #endregion

                    #region  注釋上傳圖片
                    ////string nowtime = DateTime.Now.ToString("hhmm");
                    //Random rand = new Random();
                    //int nowtime = rand.Next(1000, 9999);
                    //try
                    //{
                    //    string saveFoler = Server.MapPath("~/aimg.gigade100.com/active/");
                    //    string savePath, fileName, oldsavePath;
                    //    for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                    //    {
                    //        HttpPostedFileBase postedFile = Request.Files[iFile];
                    //        fileName = Path.GetFileName(postedFile.FileName);
                    //        if (fileName != "")
                    //        {
                    //            string fileType = fileName.Substring(fileName.LastIndexOf("."));
                    //            string newName = GetEventId(oldermodel.event_type, model.id.ToString()) + nowtime + fileType;
                    //            oldsavePath = olderpcmodel.banner_image;
                    //            oldsavePath = saveFoler + oldsavePath;
                    //            savePath = saveFoler + newName;                                 
                    //            if (System.IO.File.Exists(oldsavePath))
                    //            {//检查是否在服务器上已经存在用户上传的同名文件
                    //                System.IO.File.Delete(oldsavePath);
                    //            }
                    //            if (fileType.ToLower() == ".jpg" || fileType.ToLower() == ".png" || fileType.ToLower() == ".gif" || fileType.ToLower() == ".jpeg")
                    //            {
                    //                if (postedFile.ContentLength <= 300 * 1024)
                    //                {
                    //                    postedFile.SaveAs(savePath);
                    //                    PPQuery.banner_image = newName;
                    //                }
                    //                else
                    //                {
                    //                    jsonStr = "{success:false,msg:1}";
                    //                    this.Response.Clear();
                    //                    this.Response.Write(jsonStr.ToString());
                    //                    this.Response.End();
                    //                    return this.Response;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                jsonStr = "{success:false,msg:2}";
                    //                this.Response.Clear();
                    //                this.Response.Write(jsonStr.ToString());
                    //                this.Response.End();
                    //                return this.Response;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            PPQuery.banner_image = olderpcmodel.banner_image;
                    //        }
                    //    }
                    //}
                    //catch (Exception)
                    //{
                    //    PPQuery.banner_image = olderpcmodel.banner_image;
                    //}
                    #endregion
                    try//存連接地址 id是否也添加時間
                    {
                        PPQuery.category_link_url = phpwebhost + "/pair/red_green_match.php?event_id=" + PPQuery.event_id;
                    }
                    catch (Exception)
                    {
                        PPQuery.category_link_url = oldPPQuery.category_link_url;
                    }
                    //foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    //{
                    //    if (!string.IsNullOrEmpty(rid))
                    //    {
                    //        query.id = Convert.ToInt32(rid);
                    //        _promopairMgr.Update(query);
                    //    }
                    //}                    
                    if (_promopairMgr.CategoryID(model).ToString() == "true")
                    {
                        _promopairMgr.Update(model, PPQuery);
                        jsonStr = "{success:true}";
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:3}";
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

        #region 刪除 +HttpResponseBase PromoPairDelete()
        public HttpResponseBase PromoPairDelete()
        {
            string jsonStr = String.Empty;
            _promopairMgr = new PromoPairMgr(mySqlConnectionString);
            PromoPair query = new PromoPair();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            query.id = Convert.ToInt32(rid);
                            _promopairMgr.Delete(query);
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



        #region 更改活動使用狀態 +JsonResult UpdateActive()
        /// <summary>
        /// 
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
            string muser = string.Empty;
            int activeValue = Convert.ToInt32(Request.Params["active"]);
            if (!string.IsNullOrEmpty(Request.Params["muser"]))
            {
                muser = (Request.Params["muser"]);
            }
            if (currentUser == muser && activeValue == 1)
            {
                return Json(new { success = "stop" });
            }
            _promopairMgr = new PromoPairMgr(mySqlConnectionString);
            int id = Convert.ToInt32(Request.Params["id"]);
            PromoPairQuery model = _promopairMgr.Select(id);
            model.category_link_url = _produCateMgr.GetModelById(Convert.ToUInt32(model.category_id)).category_link_url;
            model.active = Convert.ToBoolean(activeValue);
            model.event_id = CommonFunction.GetEventId(model.event_type, model.id.ToString());
            model.muser = currentUser;
            model.modified = DateTime.Now;
            if (_promopairMgr.UpdateActive(model) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion

        #region 刪除本地上傳的圖片 +void DeletePicFile(string imageName)
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

        #region 創建ftp文件夾 +void CreateFolder(string path, string[] Mappath)
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
    }
}
