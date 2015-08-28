#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：PromoAdditionalPriceController.cs 
 * 摘   要： 
 *      不同品加不同價等
 * 当前版本：v1.1 
 * 作   者： jialei0706j dongya0410j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/8/15
 *      v1.1修改人員：dongya0410j
 *      v1.1修改内容：代碼合併
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Common;
using System.IO;
using System.Configuration;
using BLL.gigade.Model.Custom;
using BLL.gigade.Dao;

namespace Admin.gigade.Controllers
{
    public class PromoAdditionalPriceController : Controller
    {
        //
        // GET: /PromoAdditionalPrice/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IProductCategoryImplMgr _produCateMgr = new ProductCategoryMgr(mySqlConnectionString);
        private IPromoAdditionalPriceMgr _promoadditionproceMgr;
        private IParametersrcImplMgr _ptersrc;
        // private IPromoAdditionalPriceMgr _proAddPrice;
        private IUserConditionImplMgr _ucMgr = new UserConditionMgr(mySqlConnectionString);
        //上傳圖片
        string promoPath = ConfigurationManager.AppSettings["promoPath"];//圖片地址
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.promoPath);//圖片保存路徑
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        //end 上傳圖片
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 相同品加相同價
        /// </summary>
        /// <returns></returns>
        public ActionResult SameFixed()
        {
            return View();
        }
        /// <summary>
        /// 不同品加相同價
        /// </summary>
        /// <returns></returns>
        public ActionResult DifferentFixed()
        {
            return View();
        }
        /// <summary>
        /// 不同品加不同價
        /// </summary>
        /// <returns></returns>
        public ActionResult DifferentNoFixed()
        {
            return View();
        }

        #region 不同商品固定價 同品加固定價 不同品加不同價格 列表頁 +HttpResponseBase PromoAdditionalPriceList()
        [CustomHandleError]
        public HttpResponseBase PromoAdditionalPriceList()
        {
            PromoAdditionalPriceQuery query = new PromoAdditionalPriceQuery();
            List<PromoAdditionalPriceQuery> store = new List<PromoAdditionalPriceQuery>();
            string json = string.Empty;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.expired = Convert.ToInt32(Request.Params["ddlSel"]);
                }

                query.event_type = Request.Params["event_type"];
                _promoadditionproceMgr = new PromoAdditionalPriceMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _promoadditionproceMgr.Query(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                List<Parametersrc> ProductFreightStore = new List<Parametersrc>();
                List<Parametersrc> DeviceStore = new List<Parametersrc>();
                List<Parametersrc> EventTypeStore = new List<Parametersrc>();
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                ProductFreightStore = _ptersrc.GetElementType("product_freight");//---deliver_type
                DeviceStore = _ptersrc.GetElementType("device");//--device
                EventTypeStore = _ptersrc.GetElementType("event_type");//--event_type

                foreach (var item in store)
                {
                    item.event_id = CommonFunction.GetEventId(item.event_type, item.id.ToString());
                    for (int i = 0; i < ProductFreightStore.Count; i++)
                    {
                        if (int.Parse(ProductFreightStore[i].ParameterCode) == item.deliver_type)
                        {
                            item.deliver_name = ProductFreightStore[i].parameterName;
                        }
                    }
                    for (int i = 0; i < DeviceStore.Count; i++)
                    {
                        if (DeviceStore[i].ParameterCode == item.device)
                        {
                            item.device_name = DeviceStore[i].parameterName;
                        }
                    }
                    //for (int i = 0; i < EventTypeStore.Count; i++)
                    //{
                    //    if (int.Parse(EventTypeStore[i].ParameterCode) == item.event_type)
                    //    {
                    //        item.device_name = EventTypeStore[i].parameterName;
                    //    }
                    //}

                    if (item.banner_image != "")
                    {
                        item.banner_image = imgServerPath + promoPath + item.banner_image;
                    }
                    else
                    {
                        item.banner_image = defaultImg;
                    }
                }
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
        //public HttpResponseBase PromoAdditionalPriceList()
        //{
        //    PromoAdditionalPriceQuery query = new PromoAdditionalPriceQuery();
        //    List<PromoAdditionalPriceQuery> store = new List<PromoAdditionalPriceQuery>();
        //    string json = string.Empty;
        //    try
        //    {
        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
        //        if (!string.IsNullOrEmpty(Request.Params["limit"]))
        //        {
        //            query.Limit = Convert.ToInt32(Request.Params["limit"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
        //        {
        //            query.expired = Convert.ToInt32(Request.Params["ddlSel"]);
        //        }

        //        query.event_type = Request.Params["event_type"];
        //        _promoadditionproceMgr = new PromoAdditionalPriceMgr(mySqlConnectionString);
        //        int totalCount = 0;
        //        store = _promoadditionproceMgr.Query(query, out totalCount);
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        //        foreach (var item in store)
        //        {
        //            if (item.banner_image != "")
        //            {
        //                item.banner_image = imgServerPath + promoPath + item.banner_image;
        //            }
        //            else
        //            {
        //                item.banner_image = defaultImg;
        //            }
        //        }
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,totalCount:0,data:[]}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格  刪除+HttpResponseBase PromoAdditionalPriceDelete()
        public HttpResponseBase PromoAdditionalPriceDelete()
        {
            PromoAdditionalPrice pap = new PromoAdditionalPrice();
            string jsonStr = String.Empty;
            _promoadditionproceMgr = new PromoAdditionalPriceMgr(mySqlConnectionString);
            if (!String.IsNullOrEmpty(Request.Params["rowID"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowID"].ToString().Split('|'))
                    {
                        if (rid != null)
                        {
                            int id = 0;

                            try
                            {
                                id = Convert.ToInt32(rid);
                            }
                            catch (Exception)
                            {

                            }

                            pap = _promoadditionproceMgr.GetModel(id);
                            var eventid = CommonFunction.GetEventId(pap.event_type, pap.id.ToString());

                            if (!string.IsNullOrEmpty(rid))
                            {
                                if (_promoadditionproceMgr.Delete(Convert.ToInt32(rid), eventid) > 0)
                                {
                                    jsonStr = "{success:true}";
                                }
                                else
                                {
                                    jsonStr = "{success:false}";
                                }
                            }
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

        #region 不同商品固定價 同品加固定價 不同品加不同價格 更新狀態 +JsonResult UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            _promoadditionproceMgr = new PromoAdditionalPriceMgr(mySqlConnectionString);
            int id = Convert.ToInt32(Request.Params["id"]);
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
            PromoAdditionalPriceQuery model = _promoadditionproceMgr.Select(id);
            if (model.url_by == 1)
            {
                model.category_link_url = _produCateMgr.GetModelById(Convert.ToUInt32(model.category_id)).category_link_url;
            }
            model.active = Convert.ToBoolean(activeValue);
            model.id = id;
            //model.event_id = GetEventId(model.event_type, model.id.ToString());
            model.muser =currentUser;
            model.modified = DateTime.Now;
            if (_promoadditionproceMgr.ChangeActive(model) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }
        #endregion

        #region 不同商品固定價 同品加固定價 不同品加不同價格  第一步 + SaveOne()
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
                PromoAdditionalPrice model = new PromoAdditionalPrice();
                _promoadditionproceMgr = new PromoAdditionalPriceMgr(mySqlConnectionString);
                if (!String.IsNullOrEmpty(Request.Params["rowid"]))
                {
                }
                else
                {
                    model.event_name = Request.Params["name"].ToString();
                    model.event_desc = Request.Params["desc"].ToString();
                    model.event_type = Request.Params["event"].ToString();
                    model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.created = DateTime.Now;
                    model.muser = model.kuser;
                    model.modified = model.created;
                    model.id = _promoadditionproceMgr.InsertFirst(model); // _promopairMgr.Save(model);
                    model = _promoadditionproceMgr.GetModel(model.id);  //_promopairMgr.Select(query);
                    string event_id = GetEventId(model.event_type, model.id.ToString());
                    if (model.id > 0)
                    {
                        jsonStr = "{success:true,\"id\":\"" + model.id + "\",\"cateID\":\"" + model.category_id + "\",\"event_id\":\"" + event_id + "\",\"cateOne\":\"" + model.left_category_id + "\",\"cateTwo\":\"" + model.right_category_id + "\" }";
                    }
                    else
                    {
                        jsonStr = "{success:false}";
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

        #region 不同商品固定價 同品加固定價 不同品加不同價格  第二步 + SaveTwo
        [CustomHandleError]
        public HttpResponseBase SaveTwo()
        {
            _promoadditionproceMgr = new PromoAdditionalPriceMgr(mySqlConnectionString);
            string jsonStr = String.Empty;
            try
            {
                PromoAdditionalPrice model = new PromoAdditionalPrice();
                PromoAdditionalPrice oldermodel = new PromoAdditionalPrice();

                PromoAdditionalPriceQuery PPQuery = new PromoAdditionalPriceQuery();
                PromoAdditionalPriceQuery oldPPQuery = new PromoAdditionalPriceQuery();
                //DateTime dtdt = Convert.ToDateTime(times);
                model.id = Convert.ToInt32(Request.Params["rowid"].ToString());
                model.category_id = PPQuery.category_id = Convert.ToInt32(Request.Params["categoryid"].ToString());
                model.deliver_type = Convert.ToInt32(Request.Params["deliver_id"].ToString());
                model.device = Request.Params["device_id"].ToString();
                model.starts = Convert.ToDateTime(Request.Params["start_date"].ToString());
                model.end = Convert.ToDateTime(Request.Params["end_date"].ToString());
                model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.modified = DateTime.Now;
                model.fixed_price = Convert.ToInt32(Request.Params["fixed_price"].ToString());
                model.buy_limit = Convert.ToInt32(Request.Params["buy_limit"].ToString());
                model.event_type = Request.Params["event_type"];
                model.event_desc = Request.Params["event_desc"];
                PPQuery.category_ipfrom = Request.UserHostAddress;
                PPQuery.event_id = GetEventId(model.event_type, model.id.ToString());
                if (!string.IsNullOrEmpty(Request.Params["side"].ToString()))
                {
                    model.website = Request.Params["side"].ToString();
                }
                else
                    model.website = oldermodel.website;

                try//存連接地址
                {
                    PPQuery.category_link_url = Request.Params["banner_url"].ToString();
                    if (PPQuery.category_link_url == "" || PPQuery.category_link_url == null)
                    {
                        model.url_by = 0;
                        PPQuery.category_link_url = oldPPQuery.category_link_url;
                        //"http://www.gigade100.com/combo_promotion.php?event_id=" + GetEventId(model.event_type, model.id.ToString());
                    }
                    else
                    {
                        model.url_by = 1;
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
                                string oldFileName = oldPPQuery.banner_image;
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
                                    PPQuery.banner_image = oldPPQuery.banner_image;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            PPQuery.banner_image = oldPPQuery.banner_image;
                        }
                        #endregion
                    }
                }
                catch (Exception)
                {
                    PPQuery.category_link_url = oldPPQuery.category_link_url;
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
                if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
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
                model.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                model.created = DateTime.Now;
                model.muser = model.kuser;
                model.modified = model.created;
                PPQuery.created = DateTime.Now;
                PPQuery.event_id = GetEventId(model.event_type, model.id.ToString());
                model.category_id = Convert.ToInt32(_promoadditionproceMgr.GetModel(model.id).category_id);
                //20140925 jialei add discount
                if (model.event_type == "A3")
                {
                    model.discount = Convert.ToInt32(Request.Params["discount"]);
                }
                else
                {
                    model.discount = 0;
                }
                if (_promoadditionproceMgr.CategoryID(model).ToString() == "true")
                {
                    _promoadditionproceMgr.InsertSecond(model, PPQuery);
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

        #region 把低於商品加購價的促銷商品刪除掉
        public HttpResponseBase DeletLessThen()
        {
            uint producateid = Convert.ToUInt32(Request.Params["producateid"]);
            //uint key_id = 0;
            string product_name = string.Empty;
            int types = 0;
            if (!string.IsNullOrEmpty(Request.Params["types"]))//變動3
            {
                types = int.Parse(Request.Params["types"]);
            }
            string site = "0";//站台改成多個站台的site_id字符串 edit by shuangshuang0420j 20140925 13:40
            if (!string.IsNullOrEmpty(Request.Params["websiteid"]))
            {
                site = Request.Params["websiteid"];
            }

            ProductCategorySetMgr _categorySetMgr = new ProductCategorySetMgr(mySqlConnectionString);
            List<ProductCategorySet> categoryList = _categorySetMgr.QueryMsg(new ProductCategorySetQuery { Category_Id = producateid });

            ProductMgr _prodMgr = new ProductMgr(mySqlConnectionString);
            int totalCount = 0;

            PromotionsMaintainDao pmDao = new PromotionsMaintainDao(mySqlConnectionString);
            string pricemaster_in = "";
            foreach (ProductCategorySet pcs in categoryList)
            {
                QueryVerifyCondition query = new QueryVerifyCondition();
                query.name_number = pcs.Product_Id.ToString();
                query.site_ids = site;
                if (!string.IsNullOrEmpty(product_name))
                {
                    query.product_name = product_name;
                }
                List<QueryandVerifyCustom> tempPros = pmDao.QueryByProSite(query, out totalCount, 0);
                if (1 <= tempPros.Count)
                {
                    pricemaster_in += tempPros[0].price_master_id + ",";
                }

            }
            pricemaster_in = pricemaster_in.TrimEnd(',');
            string jsonStr = String.Empty;
            try
            {
                PromoAdditionalPriceQuery model = new PromoAdditionalPriceQuery();

                _promoadditionproceMgr = new PromoAdditionalPriceMgr(mySqlConnectionString);
                model.id = Convert.ToInt32(Request.Params["rowid"].ToString());
                model.fixed_price = Convert.ToInt32(Request.Params["fixed_price"].ToString());
                model.price_master_in = pricemaster_in;
                if (string.IsNullOrEmpty(pricemaster_in))
                {
                    jsonStr = "{success:true,\"delcount\":\"" + 0 + "\" }";
                }
                else
                {
                    int result = _promoadditionproceMgr.DeletLessThen(model, types);
                    if (result > 0)
                    {
                        jsonStr = "{success:true,\"delcount\":\"" + 1 + "\" }";
                    }
                    else if (result == 0)
                    {
                        jsonStr = "{success:true,\"delcount\":\"" + 0 + "\" }";
                    }
                    else
                    {
                        jsonStr = "{success:false}";
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

        #region 不同商品固定價 同品加固定價 不同品加不同價格 編輯 +HttpResponseBase PromoAdditionalPriceEdit()
        public HttpResponseBase PromoAdditionalPriceEdit()
        {
            string jsonStr = String.Empty;
            _promoadditionproceMgr = new PromoAdditionalPriceMgr(mySqlConnectionString);
            PromoAdditionalPrice model = new PromoAdditionalPrice();
            PromoAdditionalPrice oldermodel = new PromoAdditionalPrice();
            PromoAdditionalPriceQuery PPQuery = new PromoAdditionalPriceQuery();
            ProductCategory olderpcmodel = new ProductCategory();
            PromoAdditionalPriceQuery oldPPQuery = new PromoAdditionalPriceQuery();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    model.id = Convert.ToInt32(Request.Params["rowid"].ToString());
                    oldermodel = _promoadditionproceMgr.GetModel(model.id);
                    model.category_id = oldermodel.category_id;
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
                        model.condition_id = 0;
                    }
                    if (Request.Params["condition_id"].ToString() != "" && Request.Params["condition_id"].ToString() != "0")
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
                    model.website = Request.Params["side"].ToString();
                    PPQuery.event_id = GetEventId(model.event_type, model.id.ToString());
                    try
                    {
                        model.fixed_price = Convert.ToInt32(Request.Params["fixed_price"].ToString());
                    }
                    catch (Exception)
                    {
                        model.fixed_price = oldermodel.fixed_price;
                    }
                    try
                    {
                        model.buy_limit = Convert.ToInt32(Request.Params["buy_limit"].ToString());
                    }
                    catch (Exception)
                    {
                        model.fixed_price = oldermodel.fixed_price;
                    }
                    model.starts = Convert.ToDateTime(Request.Params["starts"].ToString());
                    //string s = Request.Params["starts"];
                    //model.starts = DateTime.Parse(Request.Params["starts"]);
                    model.end = Convert.ToDateTime(Request.Params["end"].ToString());
                    model.muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    model.modified = DateTime.Now;
                    //20140925 add 折扣
                    if (model.event_type == "A3")
                    {
                        model.discount = Int32.Parse(Request.Params["discount"].ToString());
                    }
                    else
                    {
                        model.discount = 0;
                    }
                    PPQuery.category_ipfrom = Request.UserHostAddress;
                    PPQuery.event_id = GetEventId(model.event_type, model.id.ToString());
                    try//存連接地址
                    {
                        PPQuery.category_link_url = Request.Params["banner_url"].ToString();
                        if (PPQuery.category_link_url == "" || PPQuery.category_link_url == null)
                        {
                            model.url_by = 0;
                            //刪除上傳的圖片
                            string oldFileName = olderpcmodel.banner_image;
                            FTP ftp = new FTP(imgLocalPath + promoPath, ftpuser, ftppwd);
                            List<string> tem = ftp.GetFileList();
                            if (tem.Contains(oldFileName))
                            {
                                FTP ftps = new FTP(imgLocalPath + promoPath + oldFileName, ftpuser, ftppwd);
                                ftps.DeleteFile(imgLocalPath + promoPath + oldFileName);//刪除ftp:71.159上的舊圖片
                                DeletePicFile(Server.MapPath(imgLocalServerPath + promoPath) + oldFileName);//刪除本地圖片
                            }
                            PPQuery.category_link_url = "";
                            PPQuery.banner_image = "";
                        }
                        else
                        {
                            model.url_by = 1;
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
                        }
                    }
                    catch (Exception)
                    {
                        PPQuery.category_link_url = oldPPQuery.category_link_url;
                    }
                    model.active = false;
                    if (_promoadditionproceMgr.CategoryID(model).ToString() == "true")
                    {

                        _promoadditionproceMgr.Update(model, PPQuery);
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

        public string GetEventId(string type, string id)
        {
            string sResult = type;
            if (id.Length < 6)
            {
                for (int i = 0; i < 6 - id.Length; i++)
                {
                    sResult += "0";
                }

            }
            sResult += id;
            return sResult;
        }
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
