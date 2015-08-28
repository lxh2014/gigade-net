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
                if(pid!=0)
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

    }
}
