using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using System.Collections;
using BLL.gigade.Model.Custom;
using BLL.gigade.Dao;
using BLL.gigade.Common;
using System.Configuration;
using System.IO;

namespace Admin.gigade.Controllers
{
    public class EventPromoController : Controller
    {
        //
        // GET: /EventPromo/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IEventpromoUserConditionImplMgr ipuConditionMgr;
        private IEventPromoAmountGiftImplMgr iepaGiftMgr;
        private IEventPromoAmountFareImplMgr iepaFareMgr;
        private IEventPromoGiftImplMgr iePromoGiftMgr;
        private IEventPromoAmountDiscountImplMgr iepaDiscountMgr;
        private IEventPromoDiscountImplMgr iepDiscountMgr;
        private EventPromoAdditionalPriceMgr epapMgr;
        private EventPromoAdditionalPriceProductMgr epappMgr;
        private EventPromoAdditionalPriceGroupMgr epapgMgr;
        private PromotionBannerMgr _promotionBannerMgr;
        private PromotionBannerRelationMgr _promotionBannerRelationMgr;

        static string excelPath_export = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制

        static string excelPath = ConfigurationManager.AppSettings["ImportCompareExcel"];//關於導入的excel文件的限制
        private InvoiceMasterRecordMgr _imrMgr;

        string vendorPath = ConfigurationManager.AppSettings["vendorPath"];
        string vendorOriginalPath = "/brand_story/a/";
        //string vendor400Path = "/brand_story/400x400/";   //在前台如果各种尺寸的图档没有的时候，前台会自动产生！！！
        string brandPath = "/brand_master/a/";
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//圖片的限制條件，格式，最大、小值
        string ftpuser = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftpuser);//ftp用戶名
        string ftppwd = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.ftppwd);//ftp密碼
        string imgLocalPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.local);//ftp地址
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);//"http://192.168.71.159:8080"
        string specPath = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.vendorPath);//圖片保存路徑
        private string imgLocalServerPath = ConfigurationManager.AppSettings["imgLocalServerPath"];//aimg.gigade100.com 
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";
        BLL.gigade.Common.HashEncrypt hash = new BLL.gigade.Common.HashEncrypt();   //用於添密
        #region 視圖
        public ActionResult UserCondition()
        {
            return View();
        }
        /// <summary>
        /// 促銷送禮
        /// </summary>
        /// <returns></returns>
        public ActionResult EventPromoAmountGift()
        {
            return View();
        }
        /// <summary>
        /// 促銷免運
        /// </summary>
        /// <returns></returns>
        public ActionResult EventPromoAmountFare()
        {
            return View();
        }
        /// <summary>
        /// 促銷折扣
        /// </summary>
        /// <returns></returns>
        public ActionResult EventPromoAmountDiscount()
        {
            return View();
        }
        /// <summary>
        /// 促銷加價購
        /// </summary>
        /// <returns></returns>
        public ActionResult EventPromoAdditionalPrice()
        {
            return View();
        }

        public ActionResult EventPromoImageList()
        {
            return View();
        }

        #endregion

        #region 會員條件設定
        #region 會員條件設定列表頁
        public HttpResponseBase GetUserConditionList()
        {
            string json = string.Empty;
            int totalCount = 0;
            EventPromoUserConditionQuery query = new EventPromoUserConditionQuery();
            DataTable _dt = new DataTable();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["text"]))
                {
                    query.condition_name = Request.Params["text"].Trim();
                }
                ipuConditionMgr = new EventpromoUserConditionMgr(mySqlConnectionString);
                _dt = ipuConditionMgr.GetList(query, out totalCount);
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
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 會員條件設定新增/編輯
        public HttpResponseBase UserConditonAddOrEdit()
        {
            string json = "{success:false}";
            try
            {
                EventPromoUserConditionQuery query = new EventPromoUserConditionQuery();
                if (!string.IsNullOrEmpty(Request.Params["condition_ids"]))
                {
                    query.condition_id = Convert.ToInt32(Request.Params["condition_ids"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["condition_name"]))
                {
                    query.condition_name = Request.Params["condition_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["level_id"]))
                {
                    query.level_id = Request.Params["level_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["first_buy_time"]))
                {
                    query.first_buy_time = Convert.ToInt32(BLL.gigade.Common.CommonFunction.GetPHPTime(Request.Params["first_buy_time"]));
                }
                if (!string.IsNullOrEmpty(Request.Params["reg_start"]))
                {
                    query.reg_start = Convert.ToInt32(BLL.gigade.Common.CommonFunction.GetPHPTime(Request.Params["reg_start"]));
                }
                if (!string.IsNullOrEmpty(Request.Params["reg_end"]))
                {
                    query.reg_end = Convert.ToInt32(BLL.gigade.Common.CommonFunction.GetPHPTime(Request.Params["reg_end"]));
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_times_min"]))
                {
                    query.buy_times_min = Convert.ToInt32(Request.Params["buy_times_min"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_times_max"]))
                {
                    query.buy_times_max = Convert.ToInt32(Request.Params["buy_times_max"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_amount_min"]))
                {
                    query.buy_amount_min = Convert.ToInt32(Request.Params["buy_amount_min"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["buy_amount_max"]))
                {
                    query.buy_amount_max = Convert.ToInt32(Request.Params["buy_amount_max"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }

                ipuConditionMgr = new EventpromoUserConditionMgr(mySqlConnectionString);
                if (query.condition_id > 0)//表示是編輯
                {
                    query.modify_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    query.modify_time = DateTime.Now;
                    int result = ipuConditionMgr.AddOrUpdate(query);
                    if (result > 0)
                    {
                        json = "{success:true}";
                    }

                }
                else
                {
                    query.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    query.create_time = DateTime.Now;
                    query.modify_user = query.create_user;
                    query.modify_time = query.create_time;
                    int result = ipuConditionMgr.AddOrUpdate(query);
                    if (result > 0)
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
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 會員條件設定刪除
        public HttpResponseBase DeleteUserCondition()
        {
            string json = string.Empty;
            try
            {
                EventPromoUserConditionQuery query = new EventPromoUserConditionQuery();
                if (!string.IsNullOrEmpty(Request.Params["ids"]))
                {
                    string condition_id = Request.Params["ids"];
                    query.condition_id_tostring = condition_id.Substring(0, condition_id.LastIndexOf(','));
                }
                ipuConditionMgr = new EventpromoUserConditionMgr(mySqlConnectionString);
                int result = ipuConditionMgr.Delete(query);
                if (result > 0)
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
                json = "{success:false,msg:\"" + "" + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 抓取會員條件
        public HttpResponseBase GetEventCondiUser()
        {
            string json = "{success:false,data:[]}";
            try
            {
                ipuConditionMgr = new EventpromoUserConditionMgr(mySqlConnectionString);
                json = ipuConditionMgr.GetEventCondi(new EventPromoUserConditionQuery { });
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
        #endregion

        #region 抓取館別
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public HttpResponseBase QueryShopClass()
        {
            string json = "{success:false,data:[]}";
            try
            {

                IShopClassImplMgr _shopClassMgr = new ShopClassMgr(mySqlConnectionString);
                List<ShopClass> stores = _shopClassMgr.QueryStore();
                ShopClass Dmodel = new ShopClass();
                Dmodel.class_name = "不分";
                stores.Insert(0, Dmodel);
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
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

        #region 抓取購物車
        [CustomHandleError]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Client)]
        public string QueryShopCart()
        {
            string json = "{success:false,data:[]}";
            try
            {
                ShoppingSetupCartMgr _sscMgr = new ShoppingSetupCartMgr(mySqlConnectionString);
                json = _sscMgr.QueryShopCart();
                return json;

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return json;
            }

        }
        #endregion

        #region 根據condition_type獲得商品設定信息
        public JsonResult GetCondiType()
        {
            try
            {
                int condiType = Convert.ToInt32(Request.Params["condiType"].ToString());
                string event_id = Request.Params["event_id"].ToString();

                iepaGiftMgr = new EventPromoAmountGiftMgr(mySqlConnectionString);

                string contentStr = iepaGiftMgr.GetCondiType(condiType, event_id);
                return Json(new { success = "true", conStr = contentStr });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false", conStr = "" });
            }

        }



        #endregion

        #region 根據商品編號獲取商品名稱
        public HttpResponseBase GetProdName()
        {
            string json = string.Empty;
            try
            {
                ProductMgr _prodMgr = new ProductMgr(mySqlConnectionString);
                int isTranUint = 0;
                int pid = 0;
                if (!string.IsNullOrEmpty(Request.Params["product_id"]) && int.TryParse(Request.Params["product_id"], out isTranUint))
                {
                    pid = Convert.ToInt32(Request.Params["product_id"]);
                }
                string prodName = string.Empty;
                if (pid != 0)
                {
                    prodName = _prodMgr.GetNameForID(pid, 0, 0, 1);
                }
                if (!string.IsNullOrEmpty(prodName))
                {
                    json = "{success:true,\"prod_name\":\"" + prodName + "\"}";//返回json數據
                }
                else
                {
                    json = "{success:false,\"prod_name\":\"\"}";//返回json數據
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,\"prod_name\":\"\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

        #region 促銷送禮
        public HttpResponseBase GetPromoAmountGift()
        {
            string jsonStr = "{success:false}";

            try
            {
                EventPromoAmountGiftQuery query = new EventPromoAmountGiftQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Request.Params["event_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                int totalCount = 0;

                iepaGiftMgr = new EventPromoAmountGiftMgr(mySqlConnectionString);
                List<EventPromoAmountGiftQuery> _list = iepaGiftMgr.GetList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(_list, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        public ActionResult GetPromoGiftDetail(string event_id = "")
        {
            try
            {
                iePromoGiftMgr = new EventPromoGiftMgr(mySqlConnectionString);
                List<EventPromoGiftQuery> stores = iePromoGiftMgr.GetList(event_id);
                IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                return Content(JsonConvert.SerializeObject(stores, Formatting.None, iso));

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }

        public ActionResult SavePromoAmountGift()
        {
            try
            {
                string promoAmountGiftStr = Request.Params["promoAmountGiftStr"];
                string promoGiftDetailStr = Request.Params["promoGiftDetailStr"];
                string condiType = Request.Params["condiType"];
                EventPromoAmountGift promoAmountGiftModel = JsonConvert.DeserializeObject<EventPromoAmountGift>(promoAmountGiftStr);
                List<EventPromoGiftQuery> promoDetails = JsonConvert.DeserializeObject<List<EventPromoGiftQuery>>(promoGiftDetailStr);
                iepaGiftMgr = new EventPromoAmountGiftMgr(mySqlConnectionString);
                return Json(new { success = iepaGiftMgr.SavePromoAmountGift(promoAmountGiftModel, promoDetails, condiType) });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = false });
            }

        }

        public JsonResult UpdateActive()
        {
            try
            {
                int currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int activeValue = Convert.ToInt32(Request.Params["active"]);
                if (Convert.ToInt32(Request.Params["mo_user"]) == currentUser && activeValue == 1)
                {
                    return Json(new { success = "stop" });
                }
                else
                {


                    string event_id = Request.Params["event_id"].ToString();
                    EventPromoAmountGift model = new EventPromoAmountGift();
                    model.modify_user = currentUser;
                    model.modify_time = DateTime.Now;
                    model.event_id = event_id;
                    model.event_status = activeValue;
                    model.condition_type = Convert.ToInt32(Request.Params["type"]);
                    iepaGiftMgr = new EventPromoAmountGiftMgr(mySqlConnectionString);

                    if (iepaGiftMgr.UpdateActive(model))
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
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

        #region 獲得品牌 +string QueryBrand()

        [CustomHandleError]
        public string QueryBrand()
        {
            string json = string.Empty;
            try
            {
                uint cid = 0;
                int isShowGrade = 0;
                VendorBrandMgr vbMgr = new VendorBrandMgr(mySqlConnectionString);

                if (!string.IsNullOrEmpty(Request.Params["class_id"]))
                {
                    cid = Convert.ToUInt32(Request.Params["class_id"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["isShowGrade"]))
                {
                    isShowGrade = Convert.ToInt32(Request.Params["isShowGrade"]);
                }
                json = vbMgr.QueryClassBrand(new VendorBrand { }, cid, isShowGrade);//參1：品牌條件 參2：館別條件 參3：失格供應商下是否顯示

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
        #endregion

        #region 商品列表查詢 +HttpResponseBase QueryProList()

        //[HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProList()
        {
            string json = string.Empty;
            string _classid = String.Empty;
            PromotionsMaintainDao pmDao = new PromotionsMaintainDao(mySqlConnectionString);
            try
            {
                QueryVerifyCondition query = new QueryVerifyCondition();
                #region 查询条件填充
                query.IsPage = true;
                query.Start = int.Parse(Request.Params["start"] ?? "0");
                query.Limit = int.Parse(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["key"]))//變動1支持商品編號的批次查詢
                {
                    query.product_name = Request.Params["key"].Replace('，', ',').Replace('|', ',').Replace(' ', ','); //在這裡product_id用,分割拼接的字符串用product_name 存放
                }
                query.combination = 1;//只顯示單一商品
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))//支持品牌查詢
                {
                    query.brand_id = uint.Parse(Request.Params["brand_id"]);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["class_id"]))//支持館別查詢
                    {
                        VendorBrandDao _vendorBrandDao = new VendorBrandDao(mySqlConnectionString);
                        List<VendorBrand> results = _vendorBrandDao.GetClassBrandList(new VendorBrand { }, Convert.ToUInt32(Request.Params["class_id"]), 1);
                        foreach (VendorBrand item in results)
                        {
                            query.brand_ids += item.Brand_Id + ",";
                        }
                        query.brand_ids = query.brand_ids.TrimEnd(',');
                    }
                }
                #endregion
                int totalCount = 0;
                List<QueryandVerifyCustom> pros = pmDao.GetProList(query, out totalCount);
                json = "{succes:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(pros) + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{succes:true,totalCount:0,item:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 促銷折扣
        public HttpResponseBase GetPromoAmountDiscount()
        {
            string jsonStr = "{success:false}";
            try
            {
                EventPromoAmountDiscountQuery query = new EventPromoAmountDiscountQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Request.Params["event_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                int totalCount = 0;

                iepaDiscountMgr = new EventPromoAmountDiscountMgr(mySqlConnectionString);
                List<EventPromoAmountDiscountQuery> _list = iepaDiscountMgr.GetList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(_list, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        public ActionResult SavePromoAmountDiscount()
        {
            try
            {
                string promoAmountDiscountStr = Request.Params["promoAmountDiscountStr"];
                string promoDiscountDetailStr = Request.Params["promoDiscountDetailStr"];
                string condiType = Request.Params["condiType"];
                EventPromoAmountDiscount promoAmountGiftModel = JsonConvert.DeserializeObject<EventPromoAmountDiscount>(promoAmountDiscountStr);
                List<EventPromoDiscount> promoDetails = JsonConvert.DeserializeObject<List<EventPromoDiscount>>(promoDiscountDetailStr);
                iepaDiscountMgr = new EventPromoAmountDiscountMgr(mySqlConnectionString);
                return Json(new { success = iepaDiscountMgr.SavePromoAmountDiscount(promoAmountGiftModel, promoDetails, condiType) });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = false });
            }

        }
        public ActionResult GetPromoDiscountDetail()
        {
            try
            {
                string event_id = Request.Params["event_id"];
                iepDiscountMgr = new EventPromoDiscountMgr(mySqlConnectionString);
                List<EventPromoDiscount> stores = iepDiscountMgr.GetList(event_id);
                IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                return Content(JsonConvert.SerializeObject(stores, Formatting.None, iso));

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }

        public JsonResult UpdateActiveDiscount()
        {
            try
            {
                int currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int activeValue = Convert.ToInt32(Request.Params["active"]);
                if (Convert.ToInt32(Request.Params["mo_user"]) == currentUser && activeValue == 1)
                {
                    return Json(new { success = "stop" });
                }
                else
                {


                    string event_id = Request.Params["event_id"].ToString();
                    EventPromoAmountDiscount model = new EventPromoAmountDiscount();
                    model.modify_user = currentUser;
                    model.modify_time = DateTime.Now;
                    model.event_id = event_id;
                    model.event_status = activeValue;
                    model.condition_type = Convert.ToInt32(Request.Params["type"]);
                    iepaDiscountMgr = new EventPromoAmountDiscountMgr(mySqlConnectionString);

                    if (iepaDiscountMgr.UpdateActive(model))
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
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

        #region 促銷免運
        public HttpResponseBase GetPromoAmountFare()
        {
            string jsonStr = "{success:false}";

            try
            {
                EventPromoAmountFare query = new EventPromoAmountFare();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Request.Params["event_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                int totalCount = 0;

                iepaFareMgr = new EventPromoAmountFareMgr(mySqlConnectionString);
                List<EventPromoAmountFare> _list = iepaFareMgr.GetList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(_list, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,error:\"" + BLL.gigade.Common.CommonFunction.MySqlException(ex) + "\"}";

            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }



        public ActionResult SavePromoAmountFare()
        {
            try
            {
                string promoAmountFareStr = Request.Params["promoAmountFareStr"];
                string condiType = Request.Params["condiType"];
                EventPromoAmountFare promoAmountFareModel = JsonConvert.DeserializeObject<EventPromoAmountFare>(promoAmountFareStr);
                iepaFareMgr = new EventPromoAmountFareMgr(mySqlConnectionString);
                return Json(new { success = iepaFareMgr.SavePromoAmountFare(promoAmountFareModel, condiType) });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = false, error = BLL.gigade.Common.CommonFunction.MySqlException(ex) });
            }

        }

        public JsonResult UpdateActiveFare()
        {
            try
            {
                int currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int activeValue = Convert.ToInt32(Request.Params["active"]);
                if (Convert.ToInt32(Request.Params["mo_user"]) == currentUser && activeValue == 1)
                {
                    return Json(new { success = "stop" });
                }
                else
                {


                    string event_id = Request.Params["event_id"].ToString();
                    EventPromoAmountFare model = new EventPromoAmountFare();
                    model.modify_user = currentUser;
                    model.modify_time = DateTime.Now;
                    model.event_id = event_id;
                    model.event_status = activeValue;
                    model.condition_type = Convert.ToInt32(Request.Params["type"]);
                    iepaFareMgr = new EventPromoAmountFareMgr(mySqlConnectionString);

                    if (iepaFareMgr.UpdateActive(model))
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
                }
            }
            catch (Exception ex)
            {
                //加入資料庫異常捕捉

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false", error = BLL.gigade.Common.CommonFunction.MySqlException(ex) });
            }

        }
        #endregion

        #region 促銷加價購

        #region 加價購活動
        public ActionResult SaveEventPromoAdditionalPrice()
        {
            try
            {
                string eventPromoAdditionalPriceStr = Request.Params["eventPromoAdditionalPriceStr"];
                string eventPromoAdditionalPriceProductStr = Request.Params["eventPromoAdditionalPriceProductStr"];
                string condiType = Request.Params["condiType"];
                EventPromoAdditionalPrice eventPromoAdditionalPriceModel = JsonConvert.DeserializeObject<EventPromoAdditionalPrice>(eventPromoAdditionalPriceStr);
                List<EventPromoAdditionalPriceProduct> promoDetails = JsonConvert.DeserializeObject<List<EventPromoAdditionalPriceProduct>>(eventPromoAdditionalPriceProductStr);
                epapMgr = new EventPromoAdditionalPriceMgr(mySqlConnectionString);
                return Json(new { success = epapMgr.SaveEventPromoAdditionalPrice(eventPromoAdditionalPriceModel, promoDetails, condiType) });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = false });
            }
        }

        public HttpResponseBase GetEventPromoAdditionalPrice()
        {
            string jsonStr = "{success:false}";
            try
            {
                EventPromoAdditionalPriceQuery query = new EventPromoAdditionalPriceQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["event_id"]))
                {
                    query.event_id = Request.Params["event_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["event_name"]))
                {
                    query.event_name = Request.Params["event_name"];
                }
                int totalCount = 0;

                epapMgr = new EventPromoAdditionalPriceMgr(mySqlConnectionString);
                List<EventPromoAdditionalPriceQuery> _list = epapMgr.GetList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(_list, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 商品細項
        public HttpResponseBase GetEventPromoAdditionalPriceProduct()
        {
            string jsonStr = "{success:false}";
            try
            {
                EventPromoAdditionalPriceProductQuery query = new EventPromoAdditionalPriceProductQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = int.Parse(Request.Params["group_id"]);
                }
                int totalCount = 0;
                epappMgr = new EventPromoAdditionalPriceProductMgr(mySqlConnectionString);
                List<EventPromoAdditionalPriceProductQuery> _list = epappMgr.GetList(query);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(_list, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 商品群組
        public HttpResponseBase GetProductGroupCbo()
        {

            string jsonStr = "{success:false,data:[]}";
            try
            {
                EventPromoAdditionalPriceGroup model = new EventPromoAdditionalPriceGroup();
                epapgMgr = new EventPromoAdditionalPriceGroupMgr(mySqlConnectionString);
                List<EventPromoAdditionalPriceGroupQuery> _list = epapgMgr.GetList(model);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(_list, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase SaveEventPromoAdditionalPriceGroup()
        {

            string jsonStr = "{success:false}";
            try
            {
                EventPromoAdditionalPriceGroup model = new EventPromoAdditionalPriceGroup();


                if (!string.IsNullOrEmpty(Request.Params["group_name"]))
                {
                    model.group_name = Request.Params["group_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    model.group_id = int.Parse(Request.Params["group_id"]);
                }

                model.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                model.create_date = DateTime.Now;
                model.modify_user = model.create_user;
                model.modify_time = model.create_date;
                model.group_status = 1;

                epapgMgr = new EventPromoAdditionalPriceGroupMgr(mySqlConnectionString);
                if (model.group_id == 0)
                {
                    if (epapgMgr.InsertModel(model) > 0)
                    {
                        jsonStr = "{success:true}";
                    }
                }
                else
                {
                    if (epapgMgr.UpdateModel(model) > 0)
                    {
                        jsonStr = "{success:true}";
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
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase DelEventPromoAdditionalPriceGroup()
        {
            string jsonStr = "{success:false}";
            try
            {
                EventPromoAdditionalPriceGroupQuery query = new EventPromoAdditionalPriceGroupQuery();
                if (!string.IsNullOrEmpty(Request.Params["group_ids"]))
                {
                    query.group_ids = Request.Params["group_ids"].TrimEnd(',');
                }
                epapgMgr = new EventPromoAdditionalPriceGroupMgr(mySqlConnectionString);
                if (epapgMgr.DeleteModel(query) > 0)
                {
                    jsonStr = "{success:true}";
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
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase GetGroupName()
        {
            string jsonStr = "{success:false}";
            try
            {
                EventPromoAdditionalPriceGroup model = new EventPromoAdditionalPriceGroup();
                if (!string.IsNullOrEmpty(Request.Params["group_name"]))
                {
                    model.group_name = Request.Params["group_name"];
                }
                epapgMgr = new EventPromoAdditionalPriceGroupMgr(mySqlConnectionString);
                if (epapgMgr.GetList(model).Count > 0)
                {
                    jsonStr = "{success:true}";
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
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 更改狀態
        public JsonResult UpdateActiveAdditionalPrice()
        {
            try
            {
                int currentUser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int activeValue = Convert.ToInt32(Request.Params["active"]);
                if (Convert.ToInt32(Request.Params["mo_user"]) == currentUser && activeValue == 1)
                {
                    return Json(new { success = "stop" });
                }
                else
                {
                    string event_id = Request.Params["event_id"].ToString();
                    EventPromoAdditionalPrice model = new EventPromoAdditionalPrice();
                    model.modify_user = currentUser;
                    model.modify_time = DateTime.Now;
                    model.event_id = event_id;
                    model.event_status = activeValue;
                    model.condition_type = Convert.ToInt32(Request.Params["type"]);
                    epapMgr = new EventPromoAdditionalPriceMgr(mySqlConnectionString);

                    if (epapMgr.UpdateActive(model))
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
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

        #region 促銷圖片
        public HttpResponseBase AllowMultiOrNot()
        {
            string json = string.Empty;
            PromotionBannerQuery query = new PromotionBannerQuery();
            _promotionBannerMgr = new PromotionBannerMgr(mySqlConnectionString);
            try
            {
                int id = 0;               
                if (!string.IsNullOrEmpty(Request.Params["change"]))
                {
                    if (Request.Params["change"] == "true")
                    {
                        query.changeMode = 1;
                    }
                }
                int result = _promotionBannerMgr.AllowMultiOrNot(query, out id);
                if (id == 0)
                {
                    json = "{success:true,msg:\"" + result + "\"}";
                }
                else
                {
                    if (query.changeMode == 0)
                    {
                        json = "{success:true,msg:\"" + result + "\"}";
                    }
                    else
                    {
                        json = "{success:false,msg:\"" + result + "\",id:\"" + id + "\"}";
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

        public HttpResponseBase GetPromotionBannerList()
        {
            string json = string.Empty;
            int totalCount = 0;
            PromotionBannerQuery query = new PromotionBannerQuery();
            List<PromotionBannerQuery> store = new List<PromotionBannerQuery>();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["dateCon"]))
                {
                    query.dateCon = Convert.ToInt32(Request.Params["dateCon"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_start"]))
                {
                    query.date_start =Convert.ToDateTime( Request.Params["date_start"]);
                    query.date_start = Convert.ToDateTime(query.date_start.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["date_end"]))
                {
                    query.date_end =Convert.ToDateTime( Request.Params["date_end"]);
                    query.date_end = Convert.ToDateTime(query.date_end.ToString("yyyy-MM-dd 23:59:59"));
                }
                if (!string.IsNullOrEmpty(Request.Params["activeStatus"]))
                {
                    query.pb_status = Convert.ToInt32(Request.Params["activeStatus"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["showStatus"]))
                {
                    query.showStatus = Convert.ToInt32(Request.Params["showStatus"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_name"]))
                {
                    query.brand_name = Request.Params["brand_name"].Trim(); ;
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    query.singleBrand_id = Convert.ToInt32(Request.Params["brand_id"]);
                }
                _promotionBannerMgr = new PromotionBannerMgr(mySqlConnectionString);
                store = _promotionBannerMgr.GetPromotionBannerList(query, out totalCount);
                foreach (var item in store)
                {
                    if (!string.IsNullOrEmpty(item.pb_image))
                    {
                        string folder5 = item.pb_image.Substring(0, 2) + "/"; //圖片名前兩碼
                        string folder6 = item.pb_image.Substring(2, 2) + "/"; //圖片名第三四碼
                        item.pb_image = imgServerPath + brandPath + folder5 + folder6 + item.pb_image;
                    }
                    else
                    {
                        item.pb_image = defaultImg;
                    }
                    if (!string.IsNullOrEmpty(item.pb_image_link))
                    {
                        item.pb_image_link = item.pb_image_link.Replace("''", "'");
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
                json = "{success:false}";
            }


            this.Response.Clear();
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        public JsonResult UpdateStatus()
        {
            string json = string.Empty;
            int brand_id;
            PromotionBannerQuery query = new PromotionBannerQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.pb_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.pb_status = Convert.ToInt32(Request.Params["status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["multi"]))
                {
                    query.multi =  Convert.ToInt32(Request.Params["multi"]);
                }
                query.pb_muser = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                _promotionBannerMgr = new PromotionBannerMgr(mySqlConnectionString);
                int i = _promotionBannerMgr.UpdateStatus(query,out brand_id);
                if (i > 0)
                {
                    return Json(new { success = "true", error = "" });
                }
                else if (i == -1)
                {
                    return Json(new { success = "false", error = "-1" }); //促銷圖片已過期，不可修改
                }
                else if (i == -2)
                {
                    return Json(new { success = "false", error = "-2", id = brand_id }); //品牌編號已存在促銷圖，不可重複添加
                }
                else
                {
                    return Json(new { success = "false", error = "0" });
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
        public HttpResponseBase AddorEdit()
        {
            string json = string.Empty;
            string errorInfo = string.Empty;
            int brand_id = 0;
            bool result = false;
            try
            {
                _promotionBannerMgr = new PromotionBannerMgr(mySqlConnectionString);
                int i = 0;
                PromotionBannerQuery query = new PromotionBannerQuery();
                PromotionBannerQuery oldquery = new PromotionBannerQuery();
                if (!string.IsNullOrEmpty(Request.Params["pb_id"]))
                {
                    oldquery.pb_id = Convert.ToInt32(Request.Params["pb_id"]);
                    List<PromotionBannerQuery> oldModel = _promotionBannerMgr.GetModelById(oldquery.pb_id);
                    if (oldModel != null)
                    {
                        query.pb_image = oldModel[0].pb_image;
                    }
                }
                #region 上傳圖片
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
                string localPromoPath = imgLocalPath + brandPath;//圖片存儲地址
                FileManagement fileLoad = new FileManagement();
                for (int j = 0; j < Request.Files.Count; j++)
                {
                    string fileName = string.Empty;//當前文件名
                    HttpPostedFileBase file = Request.Files[j];
                    fileName = Path.GetFileName(file.FileName);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        continue;
                    }

                    fileLoad = new FileManagement();
                    string oldFileName = string.Empty;  //舊文件名
                    string fileExtention = string.Empty;//當前文件的擴展名                 
                    string NewFileName = string.Empty;
                    string ServerPath = string.Empty;
                    string newRand = string.Empty;
                    string ErrorMsg = string.Empty;

                    newRand = hash.Md5Encrypt(fileLoad.NewFileName(fileName) + DateTime.Now.ToString(), "32");
                    fileExtention = fileName.Substring(fileName.LastIndexOf(".")).ToLower();
                    NewFileName = newRand + fileExtention;

                    string folder1 = NewFileName.Substring(0, 2) + "/"; //圖片名前兩碼
                    string folder2 = NewFileName.Substring(2, 2) + "/"; //圖片名第三四碼

                    FTP f_cf = new FTP();
                    localPromoPath = imgLocalPath + brandPath + folder1 + folder2;  //圖片存儲地址
                    string s = localPromoPath.Substring(0, localPromoPath.Length - (brandPath + folder1 + folder2).Length + 1);
                    f_cf.MakeMultiDirectory(s, (brandPath + folder1 + folder2).Substring(1, (brandPath + folder1 + folder2).Length - 2).Split('/'), ftpuser, ftppwd);
                    ServerPath = Server.MapPath(imgLocalServerPath + brandPath + folder1 + folder2);
                    fileName = NewFileName;
                    NewFileName = localPromoPath + NewFileName;//絕對路徑
                    Resource.CoreMessage = new CoreResource("Product");//尋找product.resx中的資源文件

                    //上傳
                    result = fileLoad.UpLoadFile(file, ServerPath, NewFileName, extention, int.Parse(maxValue), int.Parse(minValue), ref ErrorMsg, ftpuser, ftppwd);
                    if (result)//上傳成功
                    {
                        query.pb_image = fileName;
                    }
                    else
                    {
                        errorInfo += ErrorMsg;
                    }
                }
                #endregion               
                if (!string.IsNullOrEmpty(Request.Params["image_link"]))
                {
                    query.pb_image_link = Request.Params["image_link"];
                }
                if (!string.IsNullOrEmpty(Request.Params["begin_time"]))
                {
                    query.pb_startdate = Convert.ToDateTime(Request.Params["begin_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.pb_enddate = Convert.ToDateTime(Request.Params["end_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["vb_ids"]))
                {
                    query.brandIDS = Request.Params["vb_ids"].Substring(0, Request.Params["vb_ids"].LastIndexOf(','));
                }
                if (!string.IsNullOrEmpty(Request.Params["multi"]))
                {
                    query.multi = Convert.ToInt32(Request.Params["multi"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["pb_id"]))
                {
                    //編輯
                    query.pb_id = Convert.ToInt32(Request.Params["pb_id"]);
                    query.pb_muser = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                    result = _promotionBannerMgr.UpdateImageInfo(query, out brand_id);
                    if (result && string.IsNullOrEmpty(errorInfo))
                    {
                        json = "{success:true}"; 
                    }
                    else if (result == false && brand_id != 0)
                    {
                        json = "{success:false,msg:\"" + brand_id + "\"}";//brand_id重複
                    }

                    else if (result && !string.IsNullOrEmpty(errorInfo))
                    {
                        json = "{success:true,msg:\"數據保存成功<br/>但圖片保存失敗 <br/>" + errorInfo + "\"}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                else
                {
                    //新增
                    query.pb_kuser = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                    query.pb_muser = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                    result = _promotionBannerMgr.AddImageInfo(query, out brand_id);
                    if (result && string.IsNullOrEmpty(errorInfo))
                    {
                        json = "{success:true}";
                    }
                    else if (result == false && brand_id != 0)
                    {
                        json = "{success:false,msg:\"" + brand_id + "\"}";//brand_id重複
                    }
                    else if (result && !string.IsNullOrEmpty(errorInfo))
                    {
                        json = "{success:true,msg:\"數據保存成功<br/>但圖片保存失敗 <br/>" + errorInfo + "\"}";
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        public HttpResponseBase GetRelationList()
        {
            string json = string.Empty;
            PromotionBannerRelationQuery query = new PromotionBannerRelationQuery();
            List<PromotionBannerRelationQuery> store = new List<PromotionBannerRelationQuery>();
            try
            {
                _promotionBannerRelationMgr = new PromotionBannerRelationMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["pb_id"]))
                {
                    query.pb_id = Convert.ToInt32(Request.Params["pb_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.brand_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["name"]))
                {
                    query.brand_name = Request.Params["name"].Trim();
                }
                store = _promotionBannerRelationMgr.GetRelationList(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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
        public HttpResponseBase DeleteImage()
        {
            string json = string.Empty;
            _promotionBannerMgr = new PromotionBannerMgr(mySqlConnectionString);
            PromotionBannerQuery query = new PromotionBannerQuery();            
             try
             {
                 if (!string.IsNullOrEmpty(Request.Params["ids"]))
                 {
                     foreach (string item in Request.Params["ids"].Split('|'))
                     {
                         if (!string.IsNullOrEmpty(item))
                         {
                             query.pb_id = Convert.ToInt32(item);
                             bool result = _promotionBannerMgr.DeleteImage(query);
                             if (result)
                             {
                                 continue;
                             }
                             else
                             {
                                 json = "{success:false}";
                                 break;
                             }
                         }
                     }
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }     
        public HttpResponseBase IsModifiable()
        {
            string json = string.Empty;
            _promotionBannerMgr = new PromotionBannerMgr(mySqlConnectionString);
            PromotionBannerQuery query = new PromotionBannerQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["pb_id"]))
                {
                    query.pb_id = Convert.ToInt32(Request.Params["pb_id"]);
                }
                bool i = _promotionBannerMgr.IsModifiable(query);
                if (i)
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false,msg:\"-1\"}";
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
        public HttpResponseBase GetBrandName()
        {
            string json = string.Empty;
            _promotionBannerMgr = new PromotionBannerMgr(mySqlConnectionString);
            PromotionBannerQuery query = new PromotionBannerQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["pb_id"]))
                {
                    query.pb_id = Convert.ToInt32(Request.Params["pb_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_id"]))
                {
                    query.singleBrand_id = Convert.ToInt32(Request.Params["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                {
                    query.date_start = Convert.ToDateTime(Request.Params["startdate"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                {
                    query.date_end = Convert.ToDateTime(Request.Params["enddate"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["multi"]))
                {
                    query.multi = Convert.ToInt32(Request.Params["multi"]);
                }
                if (query.singleBrand_id == 0)
                {
                    json = "{success:false,msg:\"-1\"}";
                }
                else
                {
                    string name = _promotionBannerMgr.GetBrandName(query);
                    if (name == "-1")
                    {
                        json = "{success:false,msg:\"-1\"}";
                    }
                    else if (name == "-2")
                    {
                        json = "{success:false,msg:\"-2\"}";
                    }
                    else
                    {
                        json = "{success:true,msg:\""+ name +" \"}";
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
        #endregion

    }
}
