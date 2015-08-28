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
using System.Data;
using System.Text;
using System.Collections;
using LitJson;
using System.Globalization;


namespace Admin.gigade.Controllers
{
    public class WebContentTypeController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        //上傳圖片
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        string healthPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.healthPath);//圖片保存路徑


        //end 上傳圖片
        private IWebContentType1ImplMgr _wctMgr1 = new WebContentType1Mgr(mySqlConnectionString);
        private IWebContentType2ImplMgr _wctMgr2 = new WebContentType2Mgr(mySqlConnectionString);
        private IWebContentType3ImplMgr _wctMgr3 = new WebContentType3Mgr(mySqlConnectionString);
        private IWebContentType4ImplMgr _wctMgr4 = new WebContentType4Mgr(mySqlConnectionString);
        private IWebContentType5ImplMgr _wctMgr5 = new WebContentType5Mgr(mySqlConnectionString);
        private IWebContentType6ImplMgr _wctMgr6 = new WebContentType6Mgr(mySqlConnectionString);
        private IWebContentType7ImplMgr _wctMgr7 = new WebContentType7Mgr(mySqlConnectionString);
        private IWebContentType8ImplMgr _wctMgr8 = new WebContentType8Mgr(mySqlConnectionString);
        private IWebContentTypeSetupImplMgr _wctsuMgr = new WebContentTypeSetupMgr(mySqlConnectionString);
        private IProductImplMgr _proMgr = new ProductMgr(mySqlConnectionString);
        //private ParameterMgr paraMgr;
        private IProductCategorySetImplMgr pcsMgr = new ProductCategorySetMgr(mySqlConnectionString);
        FileManagement fileLoad = new FileManagement();
        private IVendorBrandSetImplMgr vbsMgr = new VendorBrandSetMgr(mySqlConnectionString);
        private IWebContentTypeSetupImplMgr wtiMgr = new WebContentTypeSetupMgr(mySqlConnectionString);

        //擴展名、最小值、最大值,圖片存儲地址
        string extention, minValue, maxValue, localHealthPath;
        // GET: /WebContentType/
        #region WebContentType1List(View)
        public ActionResult WebContentType1List()
        {
            return View();
        }
        public ActionResult WebContentType2List()
        {
            return View();
        }
        public ActionResult WebContentType3List()
        {
            return View();
        }
        public ActionResult WebContentType4List()
        {
            return View();
        }
        public ActionResult WebContentType5List()
        {
            return View();
        }
        public ActionResult WebContentType6List()
        {
            return View();
        }
        public ActionResult WebContentType7List()
        {
            return View();
        }
        public ActionResult WebContentType8List()
        {
            return View();
        }
        #endregion
        #region 獲取WebContentType1List列表
        /// <summary>
        /// WebContentType1List列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase WebContentTypelist1()
        {
            List<WebContentType1Query> stores = new List<WebContentType1Query>();
            string json = string.Empty;
            try
            {
                WebContentType1Query query = new WebContentType1Query();
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                int totalCount = 0;
                stores = _wctMgr1.QueryAll(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {
                    if (item.content_image != "")
                    {
                        item.content_image = imgServerPath + healthPath + item.content_image;
                    }
                    else
                    {
                        item.content_image = defaultImg;
                    }
                }

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
        #endregion
        #region 獲取WebContentType2List列表
        /// <summary>
        /// WebContentType2List列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase WebContentTypelist2()
        {
            List<WebContentType2Query> stores = new List<WebContentType2Query>();
            string json = string.Empty;
            try
            {
                WebContentType2Query wcType2 = new WebContentType2Query();
                wcType2.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    wcType2.Limit = Convert.ToInt32(Request["Limit"]);
                }
                int totalCount = 0;
                stores = _wctMgr2.QueryAll(wcType2, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {
                    if (item.content_image != "")
                    {
                        item.content_image = imgServerPath + healthPath + item.content_image;
                    }
                    else
                    {
                        item.content_image = defaultImg;
                    }
                }
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

        #endregion
        #region 獲取WebContentType3List列表
        /// <summary>
        /// WebContentType3List列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase WebContentTypelist3()
        {
            List<WebContentType3Query> stores = new List<WebContentType3Query>();
            string json = string.Empty;
            try
            {
                WebContentType3Query wcType3 = new WebContentType3Query();
                wcType3.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    wcType3.Limit = Convert.ToInt32(Request["Limit"]);
                }
                int totalCount = 0;
                stores = _wctMgr3.QueryAll(wcType3, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {
                    if (item.content_image != "")
                    {
                        item.content_image = imgServerPath + healthPath + item.content_image;
                    }
                    else
                    {
                        item.content_image = defaultImg;
                    }
                }
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

        #endregion
        #region 獲取WebContentType4List列表
        /// <summary>
        /// WebContentType1List列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase WebContentTypelist4()
        {
            List<WebContentType4Query> stores = new List<WebContentType4Query>();
            string json = string.Empty;
            try
            {
                WebContentType4Query wcType4 = new WebContentType4Query();
                wcType4.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    wcType4.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request["serchcontent"]))
                {
                    wcType4.serchwhere = Request["serchcontent"].Trim();
                }
                int totalCount = 0;
                stores = _wctMgr4.QueryAll(wcType4, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
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

        #endregion
        #region 獲取WebContentType5List列表
        /// <summary>
        /// WebContentType1List列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase WebContentTypelist5()
        {
            List<WebContentType5Query> stores = new List<WebContentType5Query>();
            string json = string.Empty;
            try
            {
                WebContentType5Query wcType5 = new WebContentType5Query();
                wcType5.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    wcType5.Limit = Convert.ToInt32(Request["Limit"]);
                }
                int totalCount = 0;
                stores = _wctMgr5.QueryAll(wcType5, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {
                    if (item.content_image != "")
                    {
                        item.content_image = imgServerPath + healthPath + item.content_image;
                    }
                    else
                    {
                        item.content_image = defaultImg;
                    }
                }
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

        #endregion
        #region 獲取WebContentType6List列表
        /// <summary>
        /// WebContentType1List列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase WebContentTypelist6()
        {
            List<WebContentType6Query> stores = new List<WebContentType6Query>();
            string json = string.Empty;
            try
            {
                WebContentType6Query wcType6 = new WebContentType6Query();
                wcType6.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    wcType6.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request["serchcontent"]))
                {
                    wcType6.serchwhere = Request["serchcontent"].Trim();
                }
                int totalCount = 0;
                stores = _wctMgr6.QueryAll(wcType6, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {
                    if (item.home_image != "")
                    {
                        item.home_image = imgServerPath + healthPath + item.home_image;
                    }
                    else
                    {
                        item.home_image = defaultImg;
                    }
                }
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

        #endregion
        #region 獲取WebContentType7List列表
        /// <summary>
        /// WebContentType1List列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase WebContentTypelist7()
        {
            List<WebContentType7Query> stores = new List<WebContentType7Query>();
            string json = string.Empty;
            try
            {
                WebContentType7Query query = new WebContentType7Query();
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request["serchcontent"]))
                {
                    query.serchwhere = Request["serchcontent"].Trim();
                }
                if (!string.IsNullOrEmpty(Request["serchchoose"]))
                {
                    query.serchchoose = Convert.ToInt32(Request["serchchoose"]);
                }
                int totalCount = 0;
                stores = _wctMgr7.QueryAll(query, out totalCount);


                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {
                    if (item.content_image != "")
                    {
                        item.content_image = imgServerPath + healthPath + item.content_image;
                    }
                    else
                    {
                        item.content_image = defaultImg;
                    }
                }
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


        #endregion
        #region 獲取WebContentType8List列表
        /// <summary>
        /// WebContentType1List列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase WebContentTypelist8()
        {
            List<WebContentType8Query> stores = new List<WebContentType8Query>();
            string json = string.Empty;
            try
            {
                WebContentType8Query wcType8 = new WebContentType8Query();
                wcType8.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    wcType8.Limit = Convert.ToInt32(Request["Limit"]);
                }
                int totalCount = 0;
                stores = _wctMgr8.QueryAll(wcType8, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {
                    if (item.home_image != "")
                    {
                        item.home_image = imgServerPath + healthPath + item.home_image;
                    }
                    else
                    {
                        item.home_image = defaultImg;
                    }
                }
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

        #endregion

        #region 獲取參數page數據
        /// <summary>
        /// 獲取參數t_parametersrc數據
        /// </summary>
        /// <returns>t_parametersrc數據</returns>
        public string GetPage()
        {
            string json = string.Empty;

            WebContentTypeSetup wts = new WebContentTypeSetup();
            wts.site_id = 7;
            wts.web_content_type = Request.Params["webcontenttype_page"];
            if (Request.Params["type"].ToString() == "page")
            {
                DataTable pagedt = _wctsuMgr.QueryPageStore(wts);
                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                for (int i = 0; i < pagedt.Rows.Count; i++)
                {
                    stb.Append("{");
                    stb.AppendFormat("\"page_id\":\"{0}\",\"page_name\":\"{1}\"", pagedt.Rows[i][0], pagedt.Rows[i][1]);
                    stb.Append("}");
                }
                stb.Append("]}");
                json = stb.ToString().Replace("}{", "},{");
            }
            else if (Request.Params["type"].ToString() == "area")
            {
                wts.page_id = Convert.ToInt32(Request.Params["pageid"]);
                DataTable areadt = _wctsuMgr.QueryAreaStore(wts);
                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                for (int i = 0; i < areadt.Rows.Count; i++)
                {
                    stb.Append("{");
                    stb.AppendFormat("\"area_id\":\"{0}\",\"area_name\":\"{1}\"", areadt.Rows[i][0], areadt.Rows[i][1]);
                    stb.Append("}");
                }
                stb.Append("]}");
                json = stb.ToString().Replace("}{", "},{");
            }

            return json;
        }

        #endregion
        #region 獲取link_url
        public JsonResult GetLinkUrl()
        {
            VendorBrandSet vbs = new VendorBrandSet();
            WebContentTypeSetup wts = new WebContentTypeSetup();
            wts.area_id = int.Parse(Request.Params["areaid"].ToString());
            wts.page_id = int.Parse(Request.Params["pageid"].ToString());
            wts.web_content_type = Request.Params["webcontenttype_page"].ToString();
            wts.site_id = 7;
            var linkUrl = "";
            if (wtiMgr.Query(wts) != null)
            {
                WebContentTypeSetup model = wtiMgr.Query(wts)[0];
                linkUrl = model.default_link_url.ToString();

                switch (model.web_content_type)
                {
                    case "web_content_type1":
                    case "web_content_type8":
                        break;
                    case "web_content_type2":
                        if (Request.Params["productid"].ToString() != "" && Request.Params["productid"].ToString() != "0")
                        {
                            var cid = _proMgr.QueryClassId(Convert.ToInt32(Request.Params["productid"].ToString()));
                            linkUrl += "?pid=" + Request.Params["productid"].ToString() + "&cid=" + cid;
                        }
                        break;
                    case "web_content_type3":
                    case "web_content_type5":
                        vbs.brand_id = uint.Parse(Request.Params["brandid"].ToString());
                        linkUrl += "&bid=" + Request.Params["brandid"].ToString();
                        break;
                    case "web_content_type4":
                        vbs.brand_id = uint.Parse(Request.Params["brandid"].ToString());
                        linkUrl += "?bid=" + Request.Params["brandid"].ToString();
                        List<VendorBrandSet> list = vbsMgr.GetClassId(vbs);
                        if (list.Count > 0)
                        {
                            linkUrl += "&cid=" + list[0].class_id.ToString() + "#readstory";
                        }
                        break;
                    case "web_content_type6":
                    case "web_content_type7":
                        break;
                    default:
                        break;


                }

            }
            return Json(new { success = "true", msg = linkUrl });
        }
        #endregion

        #region 根據web_content_type_setup來獲取page_id和area_id
        public WebContentTypeSetup QueryPageArea(string setupContentId)
        {
            WebContentTypeSetup wct = new WebContentTypeSetup();
            wct.content_id = Convert.ToInt32(setupContentId);
            return wtiMgr.Query(wct)[0];
        }
        #endregion

        #region 獲取brand_id和brand_name數據
        public string QueryBrand()
        {
            string json = string.Empty;
            try
            {
                DataTable brandDt = new DataTable();
                if (Request.Form["webcontenttype"] != null && Request.Form["webcontenttype"] == "web_content_type4")
                {

                    int content_id = Convert.ToInt32(Request.Form["content_id"]);
                    brandDt = pcsMgr.QueryBrand(Request.Form["webcontenttype"], content_id);
                }
                else
                {
                    brandDt = pcsMgr.QueryBrand("", 0);
                }

                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                //stb.Append("{");
                //stb.AppendFormat("\"brand_id\":\"{0}\",\"brand_name\":\"{1}\"", 0, "請選擇");
                //stb.Append("}");
                for (int i = 0; i < brandDt.Rows.Count; i++)
                {
                    if ((!String.IsNullOrEmpty(brandDt.Rows[i]["brand_id"].ToString())) || (!String.IsNullOrEmpty(brandDt.Rows[i]["brand_name"].ToString())))
                    {
                        stb.Append("{");
                        stb.Append(string.Format("\"brand_id\":\"{0}\",\"brand_name\":\"{1}\"", brandDt.Rows[i]["brand_id"], brandDt.Rows[i]["brand_name"]));
                        stb.Append("}");
                    }

                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");


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
        #region 獲取product_id和product_name數據
        public string QueryProduct()
        {
            string json = string.Empty;

            try
            {
                DataTable productDt = pcsMgr.QueryProduct("");
                StringBuilder stb = new StringBuilder();
                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                stb.Append("{");
                stb.AppendFormat("\"product_id\":\"{0}\",\"product_name\":\"{1}\"", 0, "請選擇");
                stb.Append("}");
                for (int i = 0; i < productDt.Rows.Count; i++)
                {
                    if ((!String.IsNullOrEmpty(productDt.Rows[i]["product_id"].ToString())) || (!String.IsNullOrEmpty(productDt.Rows[i]["product_name"].ToString())))
                    {
                        stb.Append("{");
                        stb.Append(string.Format("\"product_id\":\"{0}\",\"product_name\":\"{1}\"", productDt.Rows[i]["product_id"], productDt.Rows[i]["product_name"]));
                        stb.Append("}");
                    }
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
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

        #region 保存或者編輯WebContentType1
        public HttpResponseBase SaveWebContentType1()
        {
            string rowid = Request.Params["rowid"];
            WebContentType1 model = new WebContentType1();
            WebContentType1 oldModel = new WebContentType1();
            if (!String.IsNullOrEmpty(rowid))//如果不存在該id說明是添加頁面
            {
                model.content_id = Convert.ToInt32(rowid);
                oldModel = _wctMgr1.GetModel(model);
            }
            #region 獲取數據
            model.site_id = 7;

            try
            {
                model.page_id = Convert.ToInt32(Request.Params["page_id"].ToString());
            }
            catch
            {
                model.page_id = oldModel.page_id;
            }
            try
            {
                model.area_id = Convert.ToInt32(Request.Params["area_id"].ToString());
            }
            catch
            {
                model.area_id = oldModel.area_id;
            }

            try
            {
                model.content_title = Request.Params["content_title"].ToString();
            }
            catch
            {
                model.content_title = oldModel.content_title;
            }
            #region 上傳圖片
            try
            {
                ImagePathConfig();
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    string fileName = string.Empty;//當前文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    //獲取圖片名稱
                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                        string NewFileName = string.Empty;
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");

                        string ServerPath = string.Empty;
                        //判斷目錄是否存在，不存在則創建
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localHealthPath.Substring(0, localHealthPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        fileName = NewFileName + fileExtention;
                        NewFileName = localHealthPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
                        string ErrorMsg = string.Empty;

                        //上傳之前刪除已有的圖片
                        if (model.content_id != 0)
                        {
                            string oldFileName = oldModel.content_image;
                            CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                            FTP ftp = new FTP(localHealthPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftps = new FTP(localHealthPath + oldFileName, ftpuser, ftppwd);
                                ftps.DeleteFile(localHealthPath + oldFileName);//刪除ftp:71.159上的舊圖片
                            }
                        }
                        try
                        {
                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                            //上傳
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                model.content_image = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            model.content_image = oldModel.content_image;
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
                        model.content_image = oldModel.content_image;
                    }
                }
            }
            catch (Exception)
            {

                model.content_image = oldModel.content_image;
            }

            #endregion
            try
            {
                model.content_status = Convert.ToInt32(Request.Params["content_status"]);
            }
            catch
            {
                model.content_status = oldModel.content_status;
            }
            try
            {
                model.content_default = Convert.ToInt32(Request.Params["content_default"]);
            }
            catch
            {
                model.content_default = oldModel.content_default;
            }
            try
            {
                model.link_url = Request.Params["link_url"].ToString();
            }
            catch
            {
                model.link_url = oldModel.link_url;
            }
            //try
            //{
            //    model.link_page = Request.Params["link_page"].ToString();
            //}
            //catch
            //{
            //    model.link_page = oldModel.link_page;
            //}
            try
            {
                model.link_mode = Convert.ToInt32(Request.Params["link_mode"]);
            }
            catch
            {
                model.link_mode = oldModel.link_mode;
            }
            #endregion
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(rowid))//如果不存在該id說明是添加頁面
            {
                model.created_on = DateTime.Now;
                model.update_on = model.created_on;
                //這裡加上各種參數
                return InsertWebContentType1(model);//如果獲取不到則進行新增
            }
            else
            {
                model.created_on = oldModel.created_on;
                model.update_on = DateTime.Now;
                //這裡加上各種參數
                return updateWebContentType1(model);//如果可以獲取到rowid則進行修改
            }
        }

        protected HttpResponseBase InsertWebContentType1(WebContentType1 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr1.Add(model);
                json = "{success:true}";//返回json數據
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

        protected HttpResponseBase updateWebContentType1(WebContentType1 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr1.Update(model);
                json = "{success:true}";//返回json數據
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
        #region 保存或者編輯WebContentType2
        public HttpResponseBase SaveWebContentType2()
        {
            WebContentType2 model = new WebContentType2();
            WebContentType2 oldModel = new WebContentType2();
            int isTranInt = 0;
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.content_id = Convert.ToInt32(Request.Params["rowid"].ToString());
                oldModel = _wctMgr2.GetModel(model);
            }
            #region 獲取數據
            model.site_id = 7;
            try
            {
                model.page_id = Convert.ToInt32(Request.Params["page_id"].ToString());
            }
            catch
            {
                model.page_id = oldModel.page_id;
            }
            try
            {
                model.area_id = Convert.ToInt32(Request.Params["area_id"].ToString());
            }
            catch
            {
                model.area_id = oldModel.area_id;
            }

            model.content_title = Request.Params["content_title"].ToString();
            model.home_title = Request.Params["home_title"].ToString();
            model.home_text = Request.Params["home_text"].ToString();
            #region 圖片上傳
            try
            {
                ImagePathConfig();
                for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                {
                    HttpPostedFileBase file = Request.Files[iFile];
                    string fileName = string.Empty;//當前文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    //獲取圖片名稱
                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                        string NewFileName = string.Empty;
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        //判斷目錄是否存在，不存在則創建
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localHealthPath.Substring(0, localHealthPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        // CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'));
                        //  returnName += promoPath + NewFileName;
                        fileName = NewFileName + fileExtention;
                        NewFileName = localHealthPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
                        string ErrorMsg = string.Empty;
                        //上傳之前刪除已有的圖片
                        if (model.content_id != 0)
                        {
                            string oldFileName = oldModel.content_image;
                            CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                            FTP ftp = new FTP(localHealthPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftps = new FTP(localHealthPath + oldFileName, ftpuser, ftppwd);
                                ftps.DeleteFile(localHealthPath + oldFileName);//刪除ftp:71.159上的舊圖片
                            }
                        }
                        try
                        {
                            //上傳   
                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                model.content_image = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            model.content_image = oldModel.content_image;
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
                        model.content_image = oldModel.content_image;
                    }
                }
            }
            catch (Exception)
            {

                model.content_image = oldModel.content_image;
            }
            #endregion
            if (!string.IsNullOrEmpty(Request.Params["product_id"].ToString()))
            {
                if (int.TryParse(Request.Params["product_id"].ToString(), out isTranInt))
                {
                    model.product_id = Convert.ToInt32(Request.Params["product_id"]);
                }
                else
                {
                    model.product_id = oldModel.product_id;
                }
            }
            else
            {
                model.product_id = 0;
            }
            model.content_default = Convert.ToInt32(Request.Params["content_default"]);
            model.content_status = Convert.ToInt32(Request.Params["content_status"]);
            model.link_url = Request.Params["link_url"].ToString();
            if (!string.IsNullOrEmpty(Request.Params["start_time"].ToString()))
            {
                model.start_time = DateTime.Parse(Request.Params["start_time"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["end_time"].ToString()))
            {
                model.end_time = DateTime.Parse(Request.Params["end_time"].ToString());
            }
            //model.link_page = Request.Params["link_page"].ToString();
            model.link_mode = Convert.ToInt32(Request.Params["link_mode"]);
            #endregion
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["content_id"]))//如果不存在該id說明是添加頁面
            {
                model.created_on = DateTime.Now;
                model.update_on = model.created_on;//這裡加上各種參數                
                return InsertWebContentType2(model);//如果獲取不到則進行新增
            }
            else
            {
                model.update_on = DateTime.Now;//這裡加上各種參數                
                return updateWebContentType2(model);//如果可以獲取到rowid則進行修改
            }
        }
        protected HttpResponseBase InsertWebContentType2(WebContentType2 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr2.Add(model);
                json = "{success:true}";//返回json數據
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

        protected HttpResponseBase updateWebContentType2(WebContentType2 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr2.Update(model);
                json = "{success:true}";//返回json數據
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
        #region 保存或者編輯WebContentType3
        public HttpResponseBase SaveWebContentType3()
        {
            WebContentType3 model = new WebContentType3();
            WebContentType3 oldModel = new WebContentType3();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.content_id = Convert.ToInt32(Request.Params["rowid"].ToString());
                oldModel = _wctMgr3.GetModel(model);
            }
            #region 獲取數據
            model.site_id = 7;
            try
            {
                model.page_id = Convert.ToInt32(Request.Params["page_id"].ToString());
            }
            catch
            {
                model.page_id = oldModel.page_id;
            }
            try
            {
                model.area_id = Convert.ToInt32(Request.Params["area_id"].ToString());
            }
            catch
            {
                model.area_id = oldModel.area_id;
            }
            try
            {
                model.type_id = Convert.ToInt32(Request.Params["type_id"]);
            }
            catch
            {
                model.type_id = oldModel.type_id;
            }
            try
            {
                model.brand_id = Convert.ToInt32(Request.Params["brand_id"]);
            }
            catch
            {
                model.brand_id = oldModel.brand_id;
            }
            try
            {
                model.content_title = Request.Params["content_title"].ToString();
            }
            catch
            {
                model.content_title = oldModel.content_title;
            }
            #region 上傳圖片
            try
            {
                ImagePathConfig();
                for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                {
                    HttpPostedFileBase file = Request.Files[iFile];
                    string fileName = string.Empty;//當前文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    //獲取圖片名稱
                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {

                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();


                        string NewFileName = string.Empty;

                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");

                        string ServerPath = string.Empty;
                        //判斷目錄是否存在，不存在則創建
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localHealthPath.Substring(0, localHealthPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        fileName = NewFileName + fileExtention;
                        NewFileName = localHealthPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
                        string ErrorMsg = string.Empty;
                        //上傳之前刪除已有的圖片
                        if (model.content_id != 0)
                        {
                            string oldFileName = oldModel.content_image;
                            CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                            FTP ftp = new FTP(localHealthPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftps = new FTP(localHealthPath + oldFileName, ftpuser, ftppwd);
                                ftps.DeleteFile(localHealthPath + oldFileName);//刪除ftp:71.159上的舊圖片
                            }
                        }
                        try
                        {
                            //上傳
                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                model.content_image = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            model.content_image = oldModel.content_image;
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
                        model.content_image = oldModel.content_image;
                    }
                }
            }
            catch (Exception)
            {

                model.content_image = oldModel.content_image;
            }

            #endregion
            try
            {
                model.content_status = Convert.ToInt32(Request.Params["content_status"]);
            }
            catch
            {
                model.content_status = oldModel.content_status;
            }
            try
            {
                model.content_default = Convert.ToInt32(Request.Params["content_default"]);
            }
            catch
            {
                model.content_default = oldModel.content_default;
            }
            try
            {
                model.link_url = Request.Params["link_url"].ToString();
            }
            catch
            {
                model.link_url = oldModel.link_url;
            }
            //try
            //{
            //    model.link_page = Request.Params["link_page"].ToString();
            //}
            //catch
            //{
            //    model.link_page = oldModel.link_page;
            //}
            try
            {
                model.link_mode = Convert.ToInt32(Request.Params["link_mode"]);
            }
            catch
            {
                model.link_mode = oldModel.link_mode;
            }
            #endregion
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.created_on = DateTime.Now;
                model.update_on = model.created_on;
                //這裡加上各種參數
                return InsertWebContentType3(model);//如果獲取不到則進行新增
            }
            else
            {
                model.created_on = oldModel.created_on;
                model.update_on = DateTime.Now;
                //這裡加上各種參數
                return updateWebContentType3(model);//如果可以獲取到rowid則進行修改
            }
        }

        protected HttpResponseBase InsertWebContentType3(WebContentType3 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr3.Add(model);
                json = "{success:true}";//返回json數據
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

        protected HttpResponseBase updateWebContentType3(WebContentType3 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr3.Update(model);
                json = "{success:true}";//返回json數據
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
        #region 保存或者編輯WebContentType4
        public HttpResponseBase SaveWebContentType4()
        {
            WebContentType4 model = new WebContentType4();
            WebContentType4 oldModel = new WebContentType4();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.content_id = Convert.ToInt32(Request.Params["rowid"].ToString());
                oldModel = _wctMgr4.GetModel(model);
            }
            #region 獲取數據
            model.site_id = 7;
            try
            {
                model.page_id = Convert.ToInt32(Request.Params["page_id"].ToString());
            }
            catch
            {
                model.page_id = oldModel.page_id;
            }
            try
            {
                model.area_id = Convert.ToInt32(Request.Params["area_id"].ToString());
            }
            catch
            {
                model.area_id = oldModel.area_id;
            }
            try
            {
                model.type_id = Convert.ToInt32(Request.Params["type_id"]);
            }
            catch
            {
                model.type_id = oldModel.type_id;
            }
            #region //圖片上傳
            //try
            //{
            //    ImagePathConfig();
            //    for (int iFile = 0; iFile < Request.Files.Count; iFile++)
            //    {
            //        HttpPostedFileBase file = Request.Files[iFile];
            //        string fileName = string.Empty;//當前文件名
            //        string fileExtention = string.Empty;//當前文件的擴展名
            //        //獲取圖片名稱
            //        fileName = fileLoad.NewFileName(file.FileName);
            //        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
            //        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            //        string NewFileName = string.Empty;
            //        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
            //        NewFileName = hash.Md5Encrypt(fileName, "32");
            //        string ServerPath = string.Empty;
            //        //判斷目錄是否存在，不存在則創建
            //        FTP f_cf = new FTP();


            //        f_cf.MakeMultiDirectory(localPromoPath.Substring(0, localPromoPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);
            //        // CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'));

            //        //  returnName += promoPath + NewFileName;
            //        fileName = NewFileName + fileExtention;
            //        NewFileName = localPromoPath + NewFileName + fileExtention;//絕對路徑
            //        ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
            //        string ErrorMsg = string.Empty;
            //        //上傳之前刪除已有的圖片
            //        if (model.content_id != 0)
            //        {
            //            string oldFileName = oldModel.content_image;
            //            CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
            //            FTP ftp = new FTP(localPromoPath, ftpuser, ftppwd);
            //            List<string> tem = ftp.GetFileList();
            //            if (tem.Contains(oldFileName))
            //            {
            //                FTP ftps = new FTP(localPromoPath + oldFileName, ftpuser, ftppwd);
            //                ftps.DeleteFile(localPromoPath + oldFileName);//刪除ftp:71.159上的舊圖片

            //            }
            //        }
            //        try
            //        {//上傳                        
            //            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
            //            if (result)//上傳成功
            //            {
            //                  model.content_image = fileName;
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            model.content_image = oldModel.content_image;
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //    model.content_image = oldModel.content_image;
            //}
            #endregion
            try
            {
                model.brand_id = Convert.ToInt32(Request.Params["brand_id"].ToString());
            }
            catch
            {
                model.brand_id = oldModel.brand_id;
            }
            try
            {
                model.content_html = Request.Params["home_text"].ToString();
            }
            catch
            {
                model.content_html = oldModel.content_html;
            }
            try
            {
                model.content_default = Convert.ToInt32(Request.Params["content_default"]);
            }
            catch
            {
                model.content_default = oldModel.content_default;
            }
            try
            {
                model.content_status = Convert.ToInt32(Request.Params["content_status"]);
            }
            catch
            {
                model.content_status = oldModel.content_status;
            }
            try
            {
                model.link_url = Request.Params["link_url"].ToString();
            }
            catch
            {
                model.content_status = oldModel.content_status;
            }
            try
            {
                model.link_mode = Convert.ToInt32(Request.Params["link_mode"]);
            }
            catch
            {
                model.link_mode = oldModel.link_mode;
            }

            #endregion
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["content_id"]))//如果不存在該id說明是添加頁面
            {
                model.created_on = DateTime.Now;
                model.update_on = model.created_on;
                //這裡加上各種參數
                return InsertWebContentType4(model);//如果獲取不到則進行新增
            }
            else
            {
                model.update_on = DateTime.Now;//這裡加上各種參數                
                return updateWebContentType4(model);//如果可以獲取到rowid則進行修改
            }
        }

        protected HttpResponseBase InsertWebContentType4(WebContentType4 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr4.Insert(model);
                json = "{success:true}";//返回json數據
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

        protected HttpResponseBase updateWebContentType4(WebContentType4 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr4.Update(model);
                json = "{success:true}";//返回json數據
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
        #region 保存或者編輯WebContentType5
        public HttpResponseBase SaveWebContentType5()
        {
            WebContentType5 model = new WebContentType5();
            WebContentType5 oldModel = new WebContentType5();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.content_id = Convert.ToInt32(Request.Params["rowid"].ToString());
                oldModel = _wctMgr5.GetModel(model);
            }
            #region 獲取數據
            model.site_id = 7;
            try
            {
                model.page_id = Convert.ToInt32(Request.Params["page_id"].ToString());
            }
            catch
            {
                model.page_id = oldModel.page_id;
            }
            try
            {
                model.area_id = Convert.ToInt32(Request.Params["area_id"].ToString());
            }
            catch
            {
                model.area_id = oldModel.area_id;
            }
            try
            {
                model.brand_id = Convert.ToInt32(Request.Params["brand_id"]);
            }
            catch
            {
                model.brand_id = oldModel.brand_id;
            }

            try
            {
                model.content_title = Request.Params["content_title"].ToString();
            }
            catch
            {
                model.content_title = oldModel.content_title;
            }
            #region 上傳圖片
            try
            {
                ImagePathConfig();
                for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                {
                    HttpPostedFileBase file = Request.Files[iFile];
                    string fileName = string.Empty;//當前文件名

                    string fileExtention = string.Empty;//當前文件的擴展名
                    //獲取圖片名稱
                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                        string NewFileName = string.Empty;

                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");

                        string ServerPath = string.Empty;
                        //判斷目錄是否存在，不存在則創建
                        FTP f_cf = new FTP();

                        f_cf.MakeMultiDirectory(localHealthPath.Substring(0, localHealthPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);

                        fileName = NewFileName + fileExtention;
                        NewFileName = localHealthPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
                        string ErrorMsg = string.Empty;

                        //上傳之前刪除已有的圖片
                        if (model.content_id != 0)
                        {
                            string oldFileName = oldModel.content_image;
                            CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                            FTP ftp = new FTP(localHealthPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftps = new FTP(localHealthPath + oldFileName, ftpuser, ftppwd);
                                ftps.DeleteFile(localHealthPath + oldFileName);//刪除ftp:71.159上的舊圖片
                            }
                        }
                        try
                        {
                            //上傳
                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                model.content_image = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            model.content_image = oldModel.content_image;
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
                        model.content_image = oldModel.content_image;
                    }

                }

            }
            catch (Exception)
            {

                model.content_image = oldModel.content_image;
            }
            #endregion
            try
            {
                model.content_status = Convert.ToInt32(Request.Params["content_status"]);
            }
            catch
            {
                model.content_status = oldModel.content_status;
            }
            try
            {
                model.content_default = Convert.ToInt32(Request.Params["content_default"]);
            }
            catch
            {
                model.content_default = oldModel.content_default;
            }
            try
            {
                model.link_url = Request.Params["link_url"].ToString();
            }
            catch
            {
                model.link_url = oldModel.link_url;
            }
            try
            {
                model.link_mode = Convert.ToInt32(Request.Params["link_mode"]);
            }
            catch
            {
                model.link_mode = oldModel.link_mode;
            }
            #endregion
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.created_on = DateTime.Now;
                model.update_on = model.created_on;
                //這裡加上各種參數
                return InsertWebContentType5(model);//如果獲取不到則進行新增
            }
            else
            {
                model.created_on = oldModel.created_on;
                model.update_on = DateTime.Now;
                //這裡加上各種參數
                return updateWebContentType5(model);//如果可以獲取到rowid則進行修改
            }
        }

        protected HttpResponseBase InsertWebContentType5(WebContentType5 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr5.Insert(model);
                json = "{success:true}";//返回json數據
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

        protected HttpResponseBase updateWebContentType5(WebContentType5 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr5.Update(model);
                json = "{success:true}";//返回json數據
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
        #region 保存或者編輯WebContentType6
        public HttpResponseBase SaveWebContentType6()
        {
            WebContentType6 model = new WebContentType6();
            WebContentType6 oldModel = new WebContentType6();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.content_id = Convert.ToInt32(Request.Params["rowid"].ToString());
                oldModel = _wctMgr6.GetModel(model);
            }
            #region 獲取數據
            model.site_id = 7;
            try
            {
                model.page_id = Convert.ToInt32(Request.Params["page_id"].ToString());
            }
            catch
            {
                model.page_id = oldModel.page_id;
            }
            try
            {
                model.area_id = Convert.ToInt32(Request.Params["area_id"].ToString());
            }
            catch
            {
                model.area_id = oldModel.area_id;
            }
            #region 圖片上傳
            try
            {
                ImagePathConfig();
                for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                {
                    HttpPostedFileBase file = Request.Files[iFile];
                    string fileName = string.Empty;//當前文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    //獲取圖片名稱

                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                        string NewFileName = string.Empty;
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        //判斷目錄是否存在，不存在則創建
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localHealthPath.Substring(0, localHealthPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        // CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'));
                        //  returnName += promoPath + NewFileName;
                        fileName = NewFileName + fileExtention;
                        NewFileName = localHealthPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
                        string ErrorMsg = string.Empty;
                        //上傳之前刪除已有的圖片
                        if (model.content_id != 0)
                        {
                            string oldFileName = oldModel.home_image;
                            CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                            FTP ftp = new FTP(localHealthPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftps = new FTP(localHealthPath + oldFileName, ftpuser, ftppwd);
                                ftps.DeleteFile(localHealthPath + oldFileName);//刪除ftp:71.159上的舊圖片
                            }
                        }
                        try
                        {//上傳     
                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                model.home_image = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            model.home_image = oldModel.home_image;
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
                        model.home_image = oldModel.home_image;
                    }
                }
            }
            catch (Exception)
            {

                model.home_image = oldModel.home_image;
            }
            #endregion
            model.home_title = Request.Params["home_title"].ToString();
            model.content_title = Request.Params["content_title"].ToString();
            model.content_html = Request.Params["content_html"].ToString();
            //model.content_default = Convert.ToInt32(Request.Params["content_default"]);
            model.content_status = Convert.ToInt32(Request.Params["content_status"]);
            model.link_url = Request.Params["link_url"].ToString();
            model.link_mode = Convert.ToInt32(Request.Params["link_mode"]);
            model.keywords = Request.Params["keywords"].ToString();
            string[] teshu = { "<", ">", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "-", "+", "=", "?", ":", ";", "'", "`", "{", "}", "[", "]" };
            foreach (string item in teshu)
            {
                model.keywords = model.keywords.Replace(item, "").ToString();
            }
            model.keywords = model.keywords.Replace("，", ",").ToString();
            #endregion
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.created_on = DateTime.Now;
                model.update_on = model.created_on;
                //這裡加上各種參數
                return InsertWebContentType6(model);//如果獲取不到則進行新增
            }
            else
            {
                model.update_on = DateTime.Now;
                //這裡加上各種參數
                return updateWebContentType6(model);//如果可以獲取到rowid則進行修改
            }
        }

        protected HttpResponseBase InsertWebContentType6(WebContentType6 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr6.Insert(model);
                _wctMgr6.Update2(model);
                json = "{success:true}";//返回json數據
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

        protected HttpResponseBase updateWebContentType6(WebContentType6 model)
        {
            string json = string.Empty;
            try
            {
                if (!model.link_url.Contains('?'))
                {
                    model.link_url = model.link_url + "?content_id=" + model.content_id;
                }
                _wctMgr6.Update(model);

                json = "{success:true}";//返回json數據
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
        #region 保存或者編輯WebContentType7
        public HttpResponseBase SaveWebContentType7()
        {
            string rowid = Request.Params["rowid"];
            WebContentType7 model = new WebContentType7();
            WebContentType7 oldModel = new WebContentType7();
            if (!String.IsNullOrEmpty(rowid))//修改
            {
                model.content_id = Convert.ToInt32(rowid);
                oldModel = _wctMgr7.GetModel(model);
            }
            #region 獲取數據
            model.site_id = 7;
            try
            {
                model.page_id = Convert.ToInt32(Request.Params["page_id"].ToString());
            }
            catch
            {
                model.page_id = oldModel.page_id;
            }
            try
            {
                model.area_id = Convert.ToInt32(Request.Params["area_id"].ToString());
            }
            catch
            {
                model.area_id = oldModel.area_id;
            }
            //try
            //{
            //    model.type_id = Convert.ToInt32(Request.Params["type_id"]);
            //}
            //catch
            //{
            //    model.type_id = oldModel.type_id;
            //}
            #region 圖片上傳
            try
            {
                ImagePathConfig();
                //文件名,文件擴展名,新文件名,舊文件名
                string fileName = String.Empty, fileExtention = String.Empty, newFileName = String.Empty, oldFileName = String.Empty;
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                        newFileName = new BLL.gigade.Common.HashEncrypt().Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        //判斷目錄是否存在，不存在則創建
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localHealthPath.Substring(0, localHealthPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        fileName = newFileName + fileExtention;
                        newFileName = localHealthPath + newFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
                        string ErrorMsg = string.Empty;
                        //上傳之前刪除已有的圖片
                        if (model.content_id != 0)
                        {
                            oldFileName = oldModel.content_image;
                            CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                            FTP ftp = new FTP(localHealthPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftpDel = new FTP(localHealthPath + oldFileName, ftpuser, ftppwd);
                                ftpDel.DeleteFile(localHealthPath + oldFileName);//刪除ftp:71.159上的舊圖片
                            }
                        }
                        try
                        {
                            //上傳       
                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                            bool result = fileLoad.UpLoadFile(file, ServerPath, newFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                model.content_image = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            model.content_image = oldModel.content_image;
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
                        model.content_image = oldModel.content_image;
                    }
                }
            }
            catch (Exception)
            {
                model.content_image = oldModel.content_image;
            }
            #endregion
            model.home_title = Request.Params["home_title"].ToString();
            model.home_text = Request.Params["home_text"].ToString();
            model.content_title = Request.Params["content_title"].ToString();
            model.content_html = Request.Params["content_html"].ToString();
            model.content_default = Convert.ToInt32(Request.Params["content_default"]);
            model.content_status = Convert.ToInt32(Request.Params["content_status"]);
            model.link_url = Request.Params["link_url"].ToString();
            model.link_mode = Convert.ToInt32(Request.Params["link_mode"]);
            model.keywords = Request.Params["keywords"].ToString();
            model.sort = Convert.ToInt32(Request.Params["sort"]);
            if (string.IsNullOrEmpty(Request.Params["start_time"].ToString()))
            {
                model.start_time = DateTime.Parse(Request.Params["start_time"].ToString());
            }
            model.end_time = DateTime.Parse(Request.Params["end_time"].ToString());
            #endregion
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(rowid))//新增
            {
                model.created_on = DateTime.Now;
                model.update_on = model.created_on;
                return InsertWebContentType7(model);//如果獲取不到則進行新增
            }
            else
            {
                model.update_on = DateTime.Now;
                return updateWebContentType7(model);//如果可以獲取到rowid則進行修改
            }
        }

        protected HttpResponseBase InsertWebContentType7(WebContentType7 model)
        {
            string json = string.Empty;
            try
            {
                int content_id = _wctMgr7.Insert(model);
                //更新link_url
                WebContentType7 wct7Model = new WebContentType7();
                wct7Model.content_id = content_id;
                wct7Model = _wctMgr7.GetModel(wct7Model);
                wct7Model.link_url += "?content_id=" + content_id;
                _wctMgr7.Update(wct7Model);
                json = "{success:true}";//返回json數據
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

        protected HttpResponseBase updateWebContentType7(WebContentType7 model)
        {
            string json = string.Empty;
            try
            {
                if (!model.link_url.Contains("?"))
                {
                    model.link_url += "?content_id=" + model.content_id;
                }
                _wctMgr7.Update(model);
                json = "{success:true}";//返回json數據
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
        #region 保存或者編輯WebContentType8
        public HttpResponseBase SaveWebContentType8()
        {
            WebContentType8 model = new WebContentType8();
            WebContentType8 oldModel = new WebContentType8();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.content_id = Convert.ToInt32(Request.Params["rowid"]);
                oldModel = _wctMgr8.GetModel(model);
            }
            #region 獲取數據
            model.site_id = 7;
            try
            {
                model.page_id = Convert.ToInt32(Request.Params["page_id"]);
            }
            catch
            {
                model.page_id = oldModel.page_id;
            }
            try
            {
                model.area_id = Convert.ToInt32(Request.Params["area_id"]);
            }
            catch
            {
                model.area_id = oldModel.area_id;
            }
            #region 圖片上傳
            try
            {
                ImagePathConfig();
                for (int iFile = 0; iFile < Request.Files.Count; iFile++)
                {
                    HttpPostedFileBase file = Request.Files[iFile];
                    string fileName = string.Empty;//當前文件名
                    string fileExtention = string.Empty;//當前文件的擴展名
                    //獲取圖片名稱
                    fileName = fileLoad.NewFileName(file.FileName);
                    if (fileName != "")
                    {
                        fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                        fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                        string NewFileName = string.Empty;
                        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                        NewFileName = hash.Md5Encrypt(fileName, "32");
                        string ServerPath = string.Empty;
                        //判斷目錄是否存在，不存在則創建
                        FTP f_cf = new FTP();
                        f_cf.MakeMultiDirectory(localHealthPath.Substring(0, localHealthPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);
                        // CreateFolder(localPromoPath.Substring(0, localPromoPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'));
                        //  returnName += promoPath + NewFileName;
                        fileName = NewFileName + fileExtention;
                        NewFileName = localHealthPath + NewFileName + fileExtention;//絕對路徑
                        ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
                        string ErrorMsg = string.Empty;
                        //上傳之前刪除已有的圖片
                        if (model.content_id != 0)
                        {
                            string oldFileName = oldModel.home_image;
                            CommonFunction.DeletePicFile(ServerPath + oldFileName);//刪除本地圖片
                            FTP ftp = new FTP(localHealthPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftps = new FTP(localHealthPath + oldFileName, ftpuser, ftppwd);
                                ftps.DeleteFile(localHealthPath + oldFileName);//刪除ftp:71.159上的舊圖片
                            }
                        }
                        try
                        {//上傳    
                            Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件
                            bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                            if (result)//上傳成功
                            {
                                model.home_image = fileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                            logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                            logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                            log.Error(logMessage);
                            model.home_image = oldModel.home_image;
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
                        model.home_image = oldModel.home_image;
                    }
                }
            }
            catch (Exception)
            {

                model.home_image = oldModel.home_image;
            }
            #endregion
            model.home_title = Request.Params["home_title"];
            model.big_title = Request.Params["big_title"];
            model.small_title = Request.Params["small_title"];
            // model.content_default = Convert.ToInt32(Request.Params["content_default"]);
            model.content_status = Convert.ToInt32(Request.Params["content_status"]);
            model.link_url = Request.Params["link_url"];
            model.link_mode = Convert.ToInt32(Request.Params["link_mode"]);
            model.sort = Convert.ToInt32(Request.Params["sort"]);
            model.start_time = Convert.ToDateTime(Request.Params["start_time"]);
            model.end_time = Convert.ToDateTime(Request.Params["end_time"]);
            #endregion
            //判斷是否能夠獲取到rowid
            if (String.IsNullOrEmpty(Request.Params["rowid"]))//如果不存在該id說明是添加頁面
            {
                model.created_on = DateTime.Now;
                model.update_on = model.created_on;
                //這裡加上各種參數
                return InsertWebContentType8(model);//如果獲取不到則進行新增
            }
            else
            {
                model.update_on = DateTime.Now;
                //這裡加上各種參數
                return updateWebContentType8(model);//如果可以獲取到rowid則進行修改
            }
        }

        protected HttpResponseBase InsertWebContentType8(WebContentType8 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr8.Insert(model);
                json = "{success:true}";//返回json數據
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

        protected HttpResponseBase updateWebContentType8(WebContentType8 model)
        {
            string json = string.Empty;
            try
            {
                _wctMgr8.Update(model);
                json = "{success:true}";//返回json數據
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

        #region 獲取參數表中設置的限制值
        /// <summary>
        /// 獲取參數表中設置的限制值
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult GetDefaultLimit()
        {
            int statusListNum = 0; int limitNumStatus = 0;
            WebContentTypeSetup model = new WebContentTypeSetup();
            model.web_content_type = Request.Params["storeType"].ToString();
            model.site_id = Convert.ToInt32(Request.Params["site"].ToString());
            model.page_id = Convert.ToInt32(Request.Params["page"]);
            model.area_id = Convert.ToInt32(Request.Params["area"]);
            limitNumStatus = _wctsuMgr.Query(model)[0].content_status_num;

            if (limitNumStatus != 0 && !string.IsNullOrEmpty(limitNumStatus.ToString()))
            {

                switch (model.web_content_type)
                {
                    case "web_content_type1":
                        WebContentType1 model1 = new WebContentType1();
                        model1.page_id = model.page_id;
                        statusListNum = _wctMgr1.GetDefault(model1);//獲取列表中已啟用的數量
                        break;
                    case "web_content_type2":
                        WebContentType2 model2 = new WebContentType2();
                        model2.page_id = model.page_id;
                        statusListNum = _wctMgr2.GetDefault(model2);
                        break;
                    case "web_content_type3":
                        WebContentType3 model3 = new WebContentType3();
                        model3.page_id = model.page_id;
                        statusListNum = _wctMgr3.GetDefault(model3);
                        break;
                    case "web_content_type4":
                        WebContentType4 model4 = new WebContentType4();
                        model4.page_id = model.page_id;
                        statusListNum = _wctMgr4.GetDefault(model4);
                        break;
                    case "web_content_type5":
                        WebContentType5 model5 = new WebContentType5();
                        model5.page_id = model.page_id;
                        statusListNum = _wctMgr5.GetDefault(model5);
                        break;
                    case "web_content_type6":
                        WebContentType6 model6 = new WebContentType6();
                        model6.page_id = model.page_id;
                        statusListNum = _wctMgr6.GetDefault(model6);
                        break;
                    case "web_content_type7":
                        WebContentType7 model7 = new WebContentType7();
                        model7.page_id = model.page_id;
                        statusListNum = _wctMgr7.GetDefault(model7);
                        break;
                    case "web_content_type8":
                        WebContentType8 model8 = new WebContentType8();
                        model8.page_id = model.page_id;
                        statusListNum = _wctMgr8.GetDefault(model8);
                        break;
                    default:
                        break;

                }
            }
            return Json(new { success = "true", listNum = statusListNum, limitNum = limitNumStatus });
        }
        #endregion
        #region 獲取圖片配置
        /// <summary>
        /// 獲取圖片配置
        /// </summary>  
        private void ImagePathConfig()
        {
            string path = Server.MapPath(xmlPath);
            SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
            SiteConfig extention_config = _siteConfigMgr.GetConfigByName("PIC_Extention_Format");
            SiteConfig minValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MinValue");
            SiteConfig maxValue_config = _siteConfigMgr.GetConfigByName("PIC_Length_MaxValue");
            SiteConfig admin_userName = _siteConfigMgr.GetConfigByName("ADMIN_USERNAME");
            SiteConfig admin_passwd = _siteConfigMgr.GetConfigByName("ADMIN_PASSWD");
            //擴展名、最小值、最大值
            extention = extention_config.Value == "" ? extention_config.DefaultValue : extention_config.Value;
            minValue = minValue_config.Value == "" ? minValue_config.DefaultValue : minValue_config.Value;
            maxValue = maxValue_config.Value == "" ? maxValue_config.DefaultValue : maxValue_config.Value;
            localHealthPath = imgLocalPath + healthPath;//圖片存儲地址
        }
        #endregion
        #region 更改活動使用狀態
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string storeType = Request.Params["storeType"].ToString();
            switch (storeType)
            {
                case "type1":
                    WebContentType1 model1 = new WebContentType1();
                    model1.content_id = Convert.ToInt32(Request.Params["id"]);
                    model1 = _wctMgr1.GetModel(model1);
                    model1.content_status = Convert.ToInt32(Request.Params["active"]);
                    model1.content_default = 1;
                    if (_wctMgr1.Update(model1) > 0)
                    {
                        return Json(new { success = "true", msg = "" });
                    }
                    else
                    {
                        return Json(new { success = "false", msg = "" });
                    }

                case "type2":
                    WebContentType2 model2 = new WebContentType2();
                    model2.content_id = Convert.ToInt32(Request.Params["id"]);
                    model2 = _wctMgr2.GetModel(model2);
                    model2.content_status = Convert.ToInt32(Request.Params["active"]);
                    model2.content_default = 1;
                    if (_wctMgr2.Update(model2) > 0)
                    {
                        return Json(new { success = "true", msg = "" });
                    }
                    else
                    {
                        return Json(new { success = "false", msg = "" });
                    }

                case "type3":
                    WebContentType3 model3 = new WebContentType3();
                    model3.content_id = Convert.ToInt32(Request.Params["id"]);
                    model3 = _wctMgr3.GetModel(model3);
                    model3.content_status = Convert.ToInt32(Request.Params["active"]);
                    model3.content_default = 1;
                    if (_wctMgr3.Update(model3) > 0)
                    {
                        return Json(new { success = "true", msg = "" });
                    }
                    else
                    {
                        return Json(new { success = "false", msg = "" });
                    }

                case "type4":
                    WebContentType4 model4 = new WebContentType4();
                    model4.content_id = Convert.ToInt32(Request.Params["id"]);
                    model4 = _wctMgr4.GetModel(model4);
                    model4.content_status = Convert.ToInt32(Request.Params["active"]);
                    model4.content_default = 1;
                    if (_wctMgr4.Update(model4) > 0)
                    {
                        return Json(new { success = "true", msg = "" });
                    }
                    else
                    {
                        return Json(new { success = "false", msg = "" });
                    }

                case "type5":
                    WebContentType5 model5 = new WebContentType5();
                    model5.content_id = Convert.ToInt32(Request.Params["id"]);
                    model5 = _wctMgr5.GetModel(model5);
                    model5.content_status = Convert.ToInt32(Request.Params["active"]);
                    model5.content_default = 1;
                    if (_wctMgr5.Update(model5) > 0)
                    {
                        return Json(new { success = "true", msg = "" });
                    }
                    else
                    {
                        return Json(new { success = "false", msg = "" });
                    }

                case "type6":
                    WebContentType6 model6 = new WebContentType6();
                    model6.content_id = Convert.ToInt32(Request.Params["id"]);
                    model6 = _wctMgr6.GetModel(model6);
                    model6.content_status = Convert.ToInt32(Request.Params["active"]);
                    model6.content_default = 1;
                    if (_wctMgr6.Update(model6) > 0)
                    {
                        return Json(new { success = "true", msg = "" });
                    }
                    else
                    {
                        return Json(new { success = "false", msg = "" });
                    }

                case "type7":
                    WebContentType7 model7 = new WebContentType7();
                    model7.content_id = Convert.ToInt32(Request.Params["id"]);
                    model7 = _wctMgr7.GetModel(model7);
                    model7.content_status = Convert.ToInt32(Request.Params["active"]);
                    model7.content_default = 1;
                    if (_wctMgr7.Update(model7) > 0)
                    {
                        return Json(new { success = "true", msg = "" });
                    }
                    else
                    {
                        return Json(new { success = "false", msg = "" });
                    }

                case "type8":
                    WebContentType8 model8 = new WebContentType8();
                    model8.content_id = Convert.ToInt32(Request.Params["id"]);
                    model8 = _wctMgr8.GetModel(model8);
                    model8.content_status = Convert.ToInt32(Request.Params["active"]);
                    model8.content_default = 1;
                    if (_wctMgr8.Update(model8) > 0)
                    {
                        return Json(new { success = "true", msg = "" });
                    }
                    else
                    {
                        return Json(new { success = "false", msg = "" });
                    }
                default:
                    return Json(new { success = "false", msg = "" });

            }
        }
        #endregion
        public HttpResponseBase UploadHtmlEditorPicture()
        {
            String fileUrl = "";
            try
            {
                ImagePathConfig();
                HttpPostedFileBase file = Request.Files["imgFile"];
                if (file == null)
                {
                    showError("请选择文件。");
                }
                string fileName = string.Empty;//當前文件名
                string fileExtention = string.Empty;//當前文件的擴展名
                //獲取圖片名稱
                fileName = fileLoad.NewFileName(file.FileName);


                fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                fileExtention = file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower().ToString();

                string NewFileName = string.Empty;
                BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();
                NewFileName = hash.Md5Encrypt(fileName, "32");
                string ServerPath = string.Empty;
                //判斷目錄是否存在，不存在則創建
                FTP f_cf = new FTP();
                f_cf.MakeMultiDirectory(localHealthPath.Substring(0, localHealthPath.Length - healthPath.Length + 1), healthPath.Substring(1, healthPath.Length - 2).Split('/'), ftpuser, ftppwd);
                fileName = NewFileName + fileExtention;
                NewFileName = localHealthPath + NewFileName + fileExtention;//絕對路徑
                ServerPath = Server.MapPath(imgLocalServerPath + healthPath);
                string ErrorMsg = string.Empty;
                try
                {   //上傳                    
                    bool result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                    if (result)//上傳成功
                    {
                        fileUrl = imgServerPath + healthPath + fileName;
                    }
                }
                catch (Exception)
                {
                    showError("圖片上傳失敗。");
                }

            }
            catch (Exception)
            {
                showError("圖片上傳失敗。");
            }
            Hashtable hashResult = new Hashtable();
            hashResult["error"] = 0;
            hashResult["url"] = fileUrl;
            this.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
            this.Response.Write(JsonMapper.ToJson(hashResult));
            this.Response.End();
            return this.Response;
        }
        private void showError(string message)
        {
            Hashtable hash = new Hashtable();
            hash["error"] = 1;
            hash["message"] = message;
            this.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
            this.Response.Write(JsonMapper.ToJson(hash));
            this.Response.End();
        }
    }
}
