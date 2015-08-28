using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Attributes;

namespace Admin.gigade.Controllers
{
    public class ProductSelectController : Controller
    {
        //
        // GET: /ProductSelect/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IProductImplMgr _prodMgr;
        private IPriceMasterTsImplMgr _priceMasterTsMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private IFunctionImplMgr _functionMgr;
        private IPriceUpdateApplyImplMgr _priceUpdateApplyMgr;
        private IPriceUpdateApplyHistoryImplMgr _priceUpdateApplyHistoryMgr;
        private ITableHistoryImplMgr _tableHistoryMgr;
        private IItemPriceTsImplMgr _itemPriceTsMgr;
        private IItemPriceImplMgr _itemPriceMgr;
        string imgServerPath = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server);
        string prod50Path = Unitle.GetImgGigade100ComPath(Unitle.ImgGigade100ComType.prod50Path);
        string defaultImg = Unitle.GetImgGigade100ComSitePath(Unitle.ImgPathType.server) + "/product/nopic_50.jpg";

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ModifyEventCost()
        {
            return View();
        }

        /// <summary>
        /// 批量增加站臺價格 add by jiajun 2014/08/21
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexForStation()
        {
            return View();
        }
        public ActionResult ModifyStationPrices()
        {
            return View();
        }

        #region 查詢站台商品列表
        //查詢站台商品列表
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProList(string priceMasterIds)
        {
            string json = string.Empty;
            try
            {
                _prodMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                List<QueryandVerifyCustom> pros = _prodMgr.QueryByProSite(priceMasterIds);
                totalCount = pros.Count;

                json = new JObject(
                    new JProperty("succes", true),
                    new JProperty("totalCount", totalCount),
                    new JProperty("item", new JArray(
                       from p in pros
                       select new JObject(
                           new JProperty("price_master_id", p.price_master_id),
                           new JProperty("product_image", p.product_image != "" ? (imgServerPath + prod50Path + GetDetailFolder(p.product_image) + p.product_image) : defaultImg),
                           new JProperty("product_id", p.product_id),
                           new JProperty("brand_name", p.brand_name),
                           new JProperty("product_name", p.product_name),
                           new JProperty("prod_sz", p.prod_sz),
                           new JProperty("combination", p.combination),
                           new JProperty("combination_id", p.combination_id),
                           new JProperty("price_type", p.price_type),
                           new JProperty("price_type_id", p.price_type_id),
                           new JProperty("product_status", p.product_status),
                           new JProperty("site_name", p.site_name),
                           new JProperty("user_level", p.user_level),
                           new JProperty("price", p.price),
                           new JProperty("cost", p.cost),
                           new JProperty("event_price", p.event_price),
                           new JProperty("event_cost", p.event_cost),
                           new JProperty("event_start", CommonFunction.GetNetTime(p.event_start).ToString("yyyy/MM/dd HH:mm:ss")),
                           new JProperty("event_end", CommonFunction.GetNetTime(p.event_end).ToString("yyyy/MM/dd HH:mm:ss")),
                           new JProperty("prepaid", p.prepaid)
                           )))
                    ).ToString();
                // json = "{succes:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(pros) + "}";
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

        #region 商品列表查詢For批量增加站臺價格
        /// <summary>
        /// add by jiajun 2014/08/21
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase QueryProListForAddSta()
        {
            string json = string.Empty;
            try
            {
                QueryVerifyCondition query = new QueryVerifyCondition();
                #region 查询条件填充

                query.IsPage = true;
                query.Start = int.Parse(Request.Form["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Form["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Form["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["brand_id"]))
                {
                    query.brand_id = uint.Parse(Request.Form["brand_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["comboProCate_hide"]))
                {
                    query.cate_id = Request.Form["comboProCate_hide"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Form["comboFrontCage_hide"]))
                {
                    query.category_id = uint.Parse(Request.Form["comboFrontCage_hide"]);
                    query.category_id = query.category_id == 2 ? 0 : query.category_id;
                }
                if (!string.IsNullOrEmpty(Request.Form["combination"]))
                {
                    query.combination = int.Parse(Request.Form["combination"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_status"]))
                {
                    query.product_status = int.Parse(Request.Form["product_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_freight_set"]))
                {
                    query.freight = uint.Parse(Request.Form["product_freight_set"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["product_mode"]))
                {
                    query.mode = uint.Parse(Request.Form["product_mode"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["tax_type"]))
                {
                    query.tax_type = uint.Parse(Request.Form["tax_type"]);
                }
                query.date_type = Request.Form["date_type"] ?? "";
                if (!string.IsNullOrEmpty(Request.Form["time_start"]))
                {
                    query.time_start = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Form["time_start"]).ToString("yyyy/MM/dd 00:00:00")).ToString();
                }
                if (!string.IsNullOrEmpty(Request.Form["time_end"]))
                {
                    query.time_end = CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Form["time_end"]).ToString("yyyy/MM/dd 23:59:59")).ToString();
                }
                query.name_number = Request.Form["key"] ?? "";
                //增加查詢值  按鈕選中  add by zhuoqin0830w  2015/02/10
                if (!string.IsNullOrEmpty(Request.Form["priceCondition"]))
                {
                    query.priceCondition = int.Parse(Request.Form["priceCondition"]);
                }
                //add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
                if (!string.IsNullOrEmpty(Request.Form["productPrepaid"]))
                {
                    query.Prepaid = int.Parse(Request.Form["productPrepaid"]);
                }
                #endregion
                _prodMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                List<QueryandVerifyCustom> pros = _prodMgr.QueryForStation(query, out totalCount);
                pros.ForEach(p => p.CanDo = p.product_id.ToString().Length > 4 ? "0" : "1");//edit by xiangwang 2014/09/11

                foreach (var item in pros)
                {
                    if (item.product_image != "")
                    {
                        item.product_image = imgServerPath + prod50Path + GetDetailFolder(item.product_image) + item.product_image;
                    }
                    else
                    {
                        item.product_image = defaultImg;
                    }
                    item.product_name = item.product_name.Replace(Product.L_KH + item.prod_sz + Product.R_KH, "");
                }
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

        #region 計算折扣
        [HttpPost]
        public JsonResult ArithmeticalDiscount(string priceMasterCustoms, string type)
        {
            var priceMasters = JsonConvert.DeserializeObject<List<PriceMasterCustom>>(priceMasterCustoms);
            foreach (var priceMaster in priceMasters)
            {
                //售價
                int price = priceMaster.price;
                //成本
                int cost = priceMaster.cost;
                //活動價
                int event_price = priceMaster.event_price;
                //活動成本
                int event_cost = priceMaster.event_cost;
                //售價折數
                int event_price_discount = priceMaster.event_price_discount;
                //成本折數
                int event_cost_discount = priceMaster.event_cost_discount;

                switch (type)
                {
                    case "price_discount":
                        priceMaster.price = CommonFunction.ArithmeticalDiscount(price, priceMaster.price_discount);
                        break;
                    case "cost_discount":
                        priceMaster.cost = CommonFunction.ArithmeticalDiscount(cost, priceMaster.cost_discount);
                        break;
                    case "event_price_discount":
                        priceMaster.event_price = CommonFunction.ArithmeticalDiscount(price, priceMaster.event_price_discount);
                        //判斷活動成本折數是否為0 如果為 0 則存入數據庫的數據為原始數據
                        if (event_cost_discount != 0)
                        {
                            priceMaster.event_cost = CommonFunction.ArithmeticalDiscount(priceMaster.event_price, priceMaster.event_cost_discount);
                        }
                        else
                        {
                            priceMaster.event_cost = event_cost;
                        }
                        break;
                    case "event_cost_discount":
                        priceMaster.event_cost = CommonFunction.ArithmeticalDiscount(event_price, priceMaster.event_cost_discount);
                        break;
                    default:
                        break;
                }
            }
            return Json(priceMasters.Select(p => new { p.product_id, p.price_master_id, p.price, p.event_price, p.cost, p.event_cost }));
        }
        #endregion

        #region 批次修改活動價（相關方法）

        #region 批次修改活動價
        /// <summary>
        /// 批次修改活動價
        /// </summary>
        /// <param name="priceMasters"></param>
        /// <param name="priceCondition">1為依折扣，2為依活動價</param>  // add by zhuoqin0830w  2015/02/10
        /// <param name="chkCost">0 為 活動成本 設定 原成本，1 為 活動成本 依 折數 計算</param>  //add by zhuoqin0830w  2015/04/02
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public string SavePriceMaster(string priceMasters, int priceCondition = 2, int chkCost = 0)
        {
            //獲取 前臺傳來的按鈕的值  以便于 存儲時進行判斷  // add by zhuoqin0830w  2015/02/10
            Resource.CoreMessage = new CoreResource("Product");
            string json = "{success:true}";
            bool result = false;
            List<PriceMasterCustom> pms = JsonConvert.DeserializeObject<List<PriceMasterCustom>>(priceMasters);

            _priceMasterTsMgr = new PriceMasterTsMgr("");
            _priceMasterMgr = new PriceMasterMgr(connectionString);
            List<PriceMaster> priceMasterList = _priceMasterMgr.Query(pms.Select(p => p.price_master_id).ToArray());

            //產生批號
            HistoryBatch batch = new HistoryBatch();
            _functionMgr = new FunctionMgr(connectionString);
            string function = "eventUpdate";
            Function fun = _functionMgr.QueryFunction(function, "/ProductSelect");
            int functionid = fun == null ? 0 : fun.RowId;
            batch.functionid = functionid;
            string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";
            batch.kuser = (Session["caller"] as Caller).user_email;
            try
            {
                foreach (var pm in pms)
                {
                    var item = priceMasterList.Find(m => m.price_master_id == pm.price_master_id);
                    item.event_price = pm.event_price;
                    item.event_cost = pm.event_cost;
                    item.event_start = pm.event_start;
                    item.event_end = pm.event_end;
                    item.price_status = pm.price_status = 2;//申請審核
                    batch.batchno = batchNo + pm.product_id;//批號

                    //價格修改 申請審核
                    PriceUpdateApply priceUpdateApply = new PriceUpdateApply { price_master_id = pm.price_master_id };
                    priceUpdateApply.apply_user = Convert.ToUInt32((Session["caller"] as Caller).user_id);

                    //價格審核記錄
                    PriceUpdateApplyHistory applyHistroy = new PriceUpdateApplyHistory();
                    applyHistroy.user_id = Convert.ToInt32(priceUpdateApply.apply_user);
                    applyHistroy.price_status = 1;
                    applyHistroy.type = 1;

                    _priceUpdateApplyMgr = new PriceUpdateApplyMgr(connectionString);
                    _priceUpdateApplyHistoryMgr = new PriceUpdateApplyHistoryMgr(connectionString);
                    _tableHistoryMgr = new TableHistoryMgr(connectionString);
                    int apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);
                    if (apply_id != -1)
                    {
                        ArrayList excuteSql = new ArrayList();
                        item.apply_id = pm.apply_id = (uint)apply_id;
                        applyHistroy.apply_id = apply_id;

                        excuteSql.Add(_priceMasterTsMgr.UpdateTs(item));
                        //excuteSql.Add(_priceMasterTsMgr.UpdateEventTs(pm));
                        excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));

                        if (pm.combination == 1)//單一商品
                        {
                            //添加  priceType  判斷按鈕 的值 是否是 price  // add by zhuoqin0830w  2015/02/10
                            //add by zhuoqin0830w  2015/04/02  判斷是否是  依原成本設定值 
                            result = SaveSingleProduct(pm, batch, apply_id, excuteSql, priceCondition, chkCost);
                        }
                        else if (pm.price_type == 2)//各自定價
                        {
                            //add by zhuoqin0830w  2015/04/02  判斷是否是  依原成本設定值
                            result = SaveComboProductSelf(pm, batch, apply_id, excuteSql, chkCost);
                        }
                        else//按比例拆分
                        {
                            result = SaveComboProductRatio(pm, batch, apply_id, excuteSql);
                        }
                    }
                }
                return json = "{success:" + result.ToString().ToLower() + "}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return json = "{success:false,msg:'" + Resources.Product.SAVE_FAIL + "'}";
            }
        }
        #endregion

        #region 批次修改活動價  依折扣修改 和 依活動價修改（單一商品）
        /// <summary>
        /// 單一商品
        /// </summary>
        /// <param name="priceMaster"></param>
        /// <param name="batch"></param>
        /// <param name="applyId"></param>
        /// <param name="excuteSql"></param>
        /// <param name="priceType">1為依折扣，2為依活動價</param>  // add by zhuoqin0830w  2015/02/10
        /// <param name="chkCost">0 為 活動成本 設定 原成本，1 為 活動成本 依 折數 計算</param>  //add by zhuoqin0830w  2015/04/02
        /// <returns></returns>
        public bool SaveSingleProduct(PriceMasterCustom priceMaster, HistoryBatch batch, int applyId, ArrayList excuteSql, int priceType, int chkCost)
        {
            bool result = false;
            if (_tableHistoryMgr.SaveHistory<PriceMaster>(priceMaster, batch, excuteSql))
            {
                _itemPriceTsMgr = new ItemPriceTsMgr("");
                _itemPriceMgr = new ItemPriceMgr(connectionString);
                List<ItemPriceCustom> itemPrices = _itemPriceMgr.Query(new ItemPrice() { price_master_id = priceMaster.price_master_id });

                DBTableInfo tableInfo = BLL.gigade.Common.CommonFunction.GetDBInfo<ItemPrice>();
                IParametersrcImplMgr parametersrcMgr = new ParameterMgr(connectionString);
                var paras = parametersrcMgr.QueryUsed(new Parametersrc { ParameterType = "ColumnHistory", ParameterCode = tableInfo.DBName, Used = 1 });
                List<string> cols = (from p in paras
                                     select p.parameterName.Trim().ToLower()).ToList();

                var columns = _tableHistoryMgr.Query_COL_Comment(tableInfo.DBName);
                string pkName = _tableHistoryMgr.Query_TB_PK(tableInfo.DBName);

                foreach (var item in itemPrices)
                {
                    item.apply_id = (uint)applyId;
                    if (priceType == 1)
                    {
                        item.event_money = (uint)CommonFunction.ArithmeticalDiscount((int)item.item_money, priceMaster.event_price_discount);// (uint)Math.Round(item.item_money * priceMaster.event_price_discount * 0.01, 0);
                        //add by zhuoqin0830w  2015/04/02  判斷是否是  依原成本設定值
                        if (chkCost == 1)
                        {
                            //eidt by zhuoqin0830w  更改商品活動成本驗算公式使活動價乘以折扣  2015/02/27
                            item.event_cost = (uint)CommonFunction.ArithmeticalDiscount((int)item.event_money, priceMaster.event_cost_discount);// (uint)Math.Round(item.item_cost * priceMaster.event_cost_discount * 0.01, 0);
                        }
                        else { item.event_cost = (uint)priceMaster.event_cost; }
                    }
                    else
                    {
                        item.event_money = (uint)priceMaster.event_price;
                        item.event_cost = (uint)priceMaster.event_cost;
                    }
                    excuteSql = new ArrayList();
                    excuteSql.Add(_itemPriceTsMgr.UpdateTs(item));
                    if (_tableHistoryMgr.SaveHistory<ItemPrice>(item, batch, excuteSql, cols, columns, pkName))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
        #endregion

        #region 批次修改活動價 組合商品按比例拆分
        /// <summary>
        /// 組合商品按比例拆分
        /// </summary>
        /// <param name="pMaster"></param>
        /// <returns></returns>
        public bool SaveComboProductRatio(PriceMaster priceMaster, HistoryBatch batch, int applyId, ArrayList excuteSql)
        {
            return _tableHistoryMgr.SaveHistory<PriceMaster>(priceMaster, batch, excuteSql);
        }
        #endregion

        #region 批次修改活動價 組合商品各自定價
        /// <summary>
        /// 組合商品各自定價
        /// </summary>
        /// <param name="pMaster"></param>
        /// <param name="batch"></param>
        /// <param name="applyHistroy"></param>
        /// <param name="excuteSql"></param>
        /// <param name="chkCost">0 為 活動成本 設定 原成本，1 為 活動成本 依 折數 計算</param>  //add by zhuoqin0830w  2015/04/02
        /// <returns></returns>
        public bool SaveComboProductSelf(PriceMasterCustom priceMaster, HistoryBatch batch, int applyId, ArrayList excuteSql, int chkCost)
        {
            List<PriceMaster> childProducts = _priceMasterMgr.QuerySelf(priceMaster).FindAll(p => p.product_id != p.child_id && p.child_id != 0);//查詢子商品
            foreach (var child in childProducts)
            {
                child.apply_id = (uint)applyId;
                child.event_price = CommonFunction.ArithmeticalDiscount(child.price, priceMaster.event_price_discount);// (int)Math.Round(child.price * priceMaster.event_price_discount * 0.01, 0);
                //add by zhuoqin0830w  2015/04/02  判斷是否是  依原成本設定值
                if (chkCost == 1)
                {
                    //eidt by zhuoqin0830w  更改商品活動成本驗算公式使活動價乘以折扣  2015/02/27
                    child.event_cost = CommonFunction.ArithmeticalDiscount(child.event_price, priceMaster.event_cost_discount);// (int)Math.Round(child.cost * priceMaster.event_cost_discount * 0.01, 0); 
                }
                else { child.event_cost = child.event_cost; }
                // excuteSql.Add(_priceMasterTsMgr.UpdateEventTs(child));UpdateTs
                excuteSql.Add(_priceMasterTsMgr.UpdateTs(child));
            }
            return _tableHistoryMgr.SaveHistory<PriceMaster>(priceMaster, batch, excuteSql);
        }
        #endregion

        #endregion

        #region 根據圖片名反推文件目錄
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
        #endregion

        #region 批量新增站臺價（相關方法）

        #region 批量新增站臺價
        /// <summary>
        /// 批量新增站臺價格
        /// </summary>
        /// <param name="chkCost">0 為 活動成本 設定 原成本，1 為 活動成本 依 折數 計算</param>  //add by zhuoqin0830w  2015/04/02
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase AddSitePriceByGroup(string priceMasters, string priceCondition, int chkCost = 0)
        {
            string priceType = "price";
            if (priceCondition == "1") { priceType = "discount"; }
            else if (priceCondition == "2") { priceType = "price"; }
            string error = "";
            string json = "{success:true}";
            List<PriceMasterCustom> list = JsonConvert.DeserializeObject<List<PriceMasterCustom>>(priceMasters);
            foreach (var item in list)
            {
                if (!PriceMaster.CheckProdName(item.product_name))
                {
                    json = "{success:false,msg:'" + Resources.Product.FORBIDDEN_CHARACTER + "'}";
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;
                }
                if (item.prod_sz != "")
                {
                    item.product_name = PriceMaster.Product_Name_FM("`LM`" + item.product_name + "`LM`" + item.prod_sz + "`LM`");
                }
            }
            string productIdss = "";
            uint[] productIds = (from p in list select p.product_id).ToArray();
            for (int i = 0; i < productIds.Length; i++)
            {
                productIdss += productIds[i].ToString() + ",";
            }
            productIdss = productIdss.Remove(productIdss.Length - 1, 1);
            _priceMasterMgr = new PriceMasterMgr(connectionString);
            List<PriceMaster> list_priceMaster = _priceMasterMgr.GetPriceMasterInfo(productIdss, Convert.ToInt32(list[0].site_id));

            //產生批號
            HistoryBatch batch = new HistoryBatch();
            _functionMgr = new FunctionMgr(connectionString);
            string function = "newSitePrice";
            Function fun = _functionMgr.QueryFunction(function, "/ProductSelect/IndexForStation");
            int functionid = fun == null ? 0 : fun.RowId;
            batch.functionid = functionid;
            string batchNo = CommonFunction.GetPHPTime().ToString() + "_" + (Session["caller"] as Caller).user_id + "_";
            batch.kuser = (Session["caller"] as Caller).user_email;
            try
            {
                int writer_id = (Session["caller"] as Caller).user_id;
                foreach (var pm in list)
                {
                    batch.batchno = batchNo + pm.product_id;//批號
                    List<MakePriceCustom> PriceStore = new List<MakePriceCustom>();
                    if (!string.IsNullOrEmpty(pm.product_id.ToString()))//如果商品編號不為空
                    {
                        // List<PriceMaster> pMList = new List<PriceMaster>();
                        _priceMasterMgr = new PriceMasterMgr(connectionString);
                        _priceMasterTsMgr = new PriceMasterTsMgr(connectionString);
                        //PriceMaster pMaster = new PriceMaster();
                        string result = string.Empty;
                        if (pm.combination != 0)
                        {
                            if (pm.price_type == 2)
                            {
                                //add by zhuoqin0830w  2015/04/02  判斷是否是  依原成本設定值
                                result = ProductByPriceEach(pm, batch, list_priceMaster, chkCost);
                            }
                            else
                            {
                                //add by zhuoqin0830w  2015/04/02  判斷是否是  依原成本設定值
                                result = AddItemProduct(pm, batch, priceType, list_priceMaster, chkCost);
                            }
                            if (result != "success")
                            {
                                error += result + ",";
                            }
                        }
                        else
                        {
                            error += "異常數據" + ",";
                        }
                    }
                }
                if (error != "")
                {
                    json = "{success:false,msg:'" + error.Remove(error.Length - 1, 1) + "'}";
                }
                // json = "{success:false,msg:'" + error.Remove(error.Length - 1, 1) + "'}";
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

        #region 批量新增站臺價  依折扣修改 和 依活動價修改
        /// <summary>
        /// 批量新增組合商品價格
        /// </summary>
        /// <param name="pmc"></param>
        /// <param name="batch"></param>
        /// <param name="typePrice"></param>
        /// <param name="list_priceMaster"></param>
        /// <param name="chkCost">0 為 活動成本 設定 原成本，1 為 活動成本 依 折數 計算</param>  //add by zhuoqin0830w  2015/04/02
        /// <returns></returns>
        public string AddItemProduct(PriceMasterCustom pmc, HistoryBatch batch, string typePrice, List<PriceMaster> list_priceMaster, int chkCost)
        {
            ProductTemp pTemp = new ProductTemp();
            //List<List<ItemPrice>> ItemPList = new List<List<ItemPrice>>();
            PriceMasterTemp pMasterTemp = new PriceMasterTemp();
            string json = "{success:true}";
            if (!string.IsNullOrEmpty(pmc.product_id.ToString()))
            {
                #region 正式表操作
                //插入price_master
                _priceMasterMgr = new PriceMasterMgr(connectionString);
                _priceMasterTsMgr = new PriceMasterTsMgr(connectionString);
                PriceMaster gigade = list_priceMaster.Where(m => m.product_id == pmc.product_id).FirstOrDefault();
                PriceMaster pMaster = new PriceMaster();
                pMaster.product_id = pmc.product_id;
                if (pmc.combination == 1)
                {
                    pMaster.child_id = 0;
                }
                else if (pmc.combination != 1 && pmc.combination != 0)
                {
                    pMaster.child_id = Convert.ToInt32(pmc.product_id);
                }

                pMaster.site_id = pmc.site_id;
                uint userId = 0;
                if (userId != 0)
                {
                    pMaster.user_id = userId;
                }
                if (pmc.user_level.ToString() != "")
                {
                    pMaster.user_level = pmc.user_level;
                }
                pMaster.product_name = pmc.product_name;
                pMaster.bonus_percent = gigade.bonus_percent;
                pMaster.cost = pmc.cost_at;
                pMaster.price = pmc.price_at;
                pMaster.max_price = pmc.max_price;
                pMaster.max_event_price = pmc.max_price;
                pMaster.default_bonus_percent = gigade.default_bonus_percent;
                if (typePrice == "discount")
                {
                    pMaster.event_price = CommonFunction.ArithmeticalDiscount(pmc.price, pmc.event_price_discount);//Convert.ToInt32(Convert.ToDouble(pmc.price) * (pmc.event_price_discount * 0.01));
                    //add by zhuoqin0830w  2015/04/02  判斷是否是  依原成本設定值
                    if (chkCost == 1)
                    {
                        //eidt by zhuoqin0830w  更改商品活動成本驗算公式使活動價乘以折扣  2015/02/27
                        pMaster.event_cost = CommonFunction.ArithmeticalDiscount(pmc.event_price, pmc.event_cost_discount);// Convert.ToInt32(Convert.ToDouble(pmc.cost) * (pmc.event_cost_discount * 0.01));
                    }
                    else { pMaster.event_cost = pmc.event_cost; }
                }
                else
                {
                    pMaster.event_price = pmc.event_price;
                    pMaster.event_cost = pmc.event_cost;
                }
                pMaster.same_price = gigade.same_price;
                pMaster.price_status = 2;//申請審核
                pMaster.accumulated_bonus = gigade.accumulated_bonus;

                #region 時間 活動時間
                if (pmc.event_start.ToString() != "")
                {
                    pMaster.event_start = pmc.event_start;
                }
                if (pmc.event_end.ToString() != "")
                {
                    pMaster.event_end = pmc.event_end;
                }
                #endregion

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
                #region 新增
                string msg = string.Empty;
                int status = 0;
                int priceMasterId = 0;
                IPriceMasterImplMgr i = new PriceMasterMgr(connectionString);
                List<ItemPrice> iprice = i.AddSingleProduct(pmc, typePrice);
                var result = _priceMasterMgr.Query(pMaster);
                if (result.Count > 0)
                {
                    priceMasterId = -1;
                    msg = pMaster.product_id.ToString();
                    return msg;
                }
                if (typePrice == "discount")
                {
                    pMaster.price = CommonFunction.ArithmeticalDiscount(pmc.price, pmc.price_discount);// Convert.ToInt32(Convert.ToDouble(pmc.price) * (pmc._discount * 0.01));
                    pMaster.cost = CommonFunction.ArithmeticalDiscount(pmc.cost, pmc.cost_discount);// Convert.ToInt32(Convert.ToDouble(pmc.cost) * (pmc._cost_discount * 0.01));
                    pMaster.event_price = CommonFunction.ArithmeticalDiscount(pmc.price, pmc.event_price_discount);// Convert.ToInt32(Convert.ToDouble(pmc.price) * (pmc.event_price_discount * 0.01));
                    //add by zhuoqin0830w  2015/04/02  判斷是否是  依原成本設定值
                    if (chkCost == 1)
                    {
                        pMaster.event_cost = CommonFunction.ArithmeticalDiscount(pmc.event_price, pmc.event_cost_discount);// Convert.ToInt32(Convert.ToDouble(pmc.cost) * (pmc.event_cost_discount * 0.01));
                    }
                    else { pMaster.event_cost = pmc.cost; }
                }

                priceMasterId = _priceMasterMgr.Save(pMaster, iprice, null, ref msg);
                if (priceMasterId != -1)
                {
                    priceUpdateApply.price_master_id = Convert.ToUInt32(priceMasterId); //價格更新申請記錄(設置更新記錄中,更新的價格id)
                    int apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);//價格更新申請記錄(新增或者更新價格后,將記錄下來插入該表)返回更新記錄的Id號
                    if (apply_id != -1)
                    {
                        pMaster = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();
                        pMaster.apply_id = Convert.ToUInt32(apply_id);
                        applyHistroy.apply_id = apply_id;

                        excuteSql.Add(_priceMasterMgr.Update(pMaster));
                        excuteSql.Add(_priceMasterTsMgr.UpdateTs(pMaster)); //edit by xiangwang0413w 2014/07/22 更新price_master_ts表後同時更新price_master_ts表，以便價格審核
                        excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));
                        _tableHistoryMgr = new TableHistoryMgr(connectionString);
                        if (_tableHistoryMgr.SaveHistory<PriceMaster>(pMaster, batch, excuteSql))
                        {
                            status = 1;
                        }
                        else { status = 2; }
                    }
                    else { status = 2; }
                }
                else { status = 3; }
                if (status == 1)
                {
                    return "success";
                }
                else if (status == 2)
                {
                    return json = "商品:" + pmc.product_name + Resources.Product.SAVE_FAIL + "'}";
                }
                else
                {
                    return json = msg;
                }
                #endregion

                #endregion
            }
            return "";
        }
        #endregion

        #region 批量新增站臺價  商品列表中各 自定價類型 商品的新增
        /// <summary>
        /// 商品列表中各 自定價類型 商品的新增
        /// <param name="productID">商品Id號</param>
        /// add by wangwei0216w 2014/8/28
        /// <param name="chkCost">0 為 活動成本 設定 原成本，1 為 活動成本 依 折數 計算</param>  //add by zhuoqin0830w  2015/04/02
        /// </summary>
        public string ProductByPriceEach(PriceMasterCustom pmc, HistoryBatch batch, List<PriceMaster> list_priceMaster, int chkCost)
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
            string msg = "";  //定義json字符串
            _priceMasterMgr = new PriceMasterMgr(connectionString);
            //定義priceMaster對象
            var priceStores = list_priceMaster.FindAll(m => m.product_id == pmc.product_id);  //獲取各自定價商品的各自信息

            priceStores.ForEach(m =>
            {
                m.site_id = pmc.site_id;
                m.user_level = pmc.user_level;
                m.user_id = pmc.user_id;

                int event_price = m.event_price;
                int event_cost = m.event_cost;
                //將p對象賦予pm
                m.price = CommonFunction.ArithmeticalDiscount(event_price, pmc.price_discount);// Convert.ToInt32(Convert.ToDouble(event_price) * (pmc._discount * 0.01));  //新的售價
                m.cost = CommonFunction.ArithmeticalDiscount(event_cost, pmc.cost_discount); //Convert.ToInt32(Convert.ToDouble(event_cost) * (pmc._cost_discount * 0.01));//新的成本
                m.event_price = CommonFunction.ArithmeticalDiscount(event_price, pmc.event_price_discount);// Convert.ToInt32(Convert.ToDouble(event_price) * (pmc.event_price_discount * 0.01));//活動售價
                //eidt by zhuoqin0830w  更改商品活動成本驗算公式使活動價乘以折扣  2015/02/27
                if (chkCost == 1)
                {
                    m.event_cost = CommonFunction.ArithmeticalDiscount(m.event_price, pmc.event_cost_discount);// Convert.ToInt32(Convert.ToDouble(event_cost) * (pmc.event_cost_discount * 0.01));//活動成本
                }
                else { m.event_cost = event_cost; }
                m.price_status = 2;                                                                        //設置狀態為申請審核
                if (pmc.event_start.ToString() != "")
                {
                    m.event_start = pmc.event_start;
                }
                if (pmc.event_end.ToString() != "")
                {
                    m.event_end = pmc.event_end;   //設置時間
                }

            });
            var parent = priceStores.Find(m => m.product_id == m.child_id);
            //為parent賦予值
            uint userId = 0;
            int priceMasterId = -1;
            var result = _priceMasterMgr.Query(new PriceMaster { product_id = parent.product_id, child_id = parent.child_id, user_id = userId, site_id = parent.site_id });
            if (result.Count > 0)
            {
                msg = parent.product_id.ToString();
                return msg;
            }

            priceMasterId = _priceMasterMgr.Save(parent, null, null, ref msg);
            var pm = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();
            int apply_id = -1;
            if (priceMasterId != -1)
            {
                priceUpdateApply.price_master_id = Convert.ToUInt32(priceMasterId);
                apply_id = _priceUpdateApplyMgr.Save(priceUpdateApply);
                if (apply_id == -1) return msg = parent.product_id.ToString();
                applyHistroy.apply_id = apply_id;
                pm.apply_id = (uint)apply_id;
                excuteSql.Add(_priceMasterMgr.Update(pm));
                excuteSql.Add(_priceMasterTsMgr.UpdateTs(pm)); //edit by xiangwang0413w 2014/07/22 更新price_master_ts表後同時更新price_master_ts表，以便價格審核
                excuteSql.Add(_priceUpdateApplyHistoryMgr.SaveSql(applyHistroy));
            }

            foreach (PriceMaster p in priceStores.FindAll(m => m.product_id != m.child_id))                 //遍歷集合
            {
                priceMasterId = _priceMasterMgr.Save(p, null, null, ref msg);
                if (priceMasterId == -1) return msg = parent.product_id.ToString();
                pm = _priceMasterMgr.Query(new PriceMaster { price_master_id = Convert.ToUInt32(priceMasterId) }).FirstOrDefault();
                pm.apply_id = (uint)apply_id;
                excuteSql.Add(_priceMasterMgr.Update(pm));
                excuteSql.Add(_priceMasterTsMgr.UpdateTs(pm)); //edit by xiangwang0413w 2014/07/22 更新price_master_ts表後同時更新price_master_ts表，以便價格審核
            }

            _tableHistoryMgr = new TableHistoryMgr(connectionString);
            if (_tableHistoryMgr.SaveHistory<PriceMaster>(parent, batch, excuteSql))
            {
                msg = "success";
            }
            else { msg = parent.product_id.ToString(); }
            return msg;
        }
        #endregion

        #endregion

        #region 根據商品ID查詢商品信息
        /// <summary>
        /// 根據商品ID查詢商品信息
        /// </summary>
        /// <param name="priceMasterIds">商品ID</param>
        /// <returns>對應商品Id的商品信息</returns>
        /// add by wangwei0216w 
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetInfoByProductID(string ProductIDs)
        {
            string json = string.Empty;
            try
            {
                _prodMgr = new ProductMgr(connectionString);
                int totalCount = 0;
                List<QueryandVerifyCustom> pros = _prodMgr.GetProductInfoByID(ProductIDs);
                totalCount = pros.Count;

                json = new JObject(
                    new JProperty("succes", true),
                    new JProperty("totalCount", totalCount),
                    new JProperty("item", new JArray(
                       from p in pros
                       select new JObject(
                           new JProperty("product_image", p.product_image != "" ? (imgServerPath + prod50Path + GetDetailFolder(p.product_image) + p.product_image) : defaultImg),
                           new JProperty("product_id", p.product_id),
                           new JProperty("brand_name", p.brand_name),
                           new JProperty("product_name", p.product_name.Replace(Product.L_KH + p.prod_sz + Product.R_KH, "")),
                           new JProperty("prod_sz", p.prod_sz),
                           new JProperty("combination", p.combination),
                           new JProperty("combination_id", p.combination_id),
                           new JProperty("price_type", p.price_type),
                           new JProperty("price_type_id", p.price_type_id),
                           new JProperty("product_status", p.product_status),
                           new JProperty("price", p.price),
                           new JProperty("cost", p.cost),
                           new JProperty("gigade_price", p.price),
                           new JProperty("gigade_cost", p.cost),
                           new JProperty("event_price", p.event_price),
                           new JProperty("event_cost", p.event_cost),
                           new JProperty("event_start", CommonFunction.GetNetTime(p.event_start).ToString("yyyy/MM/dd HH:mm:ss")),
                           new JProperty("event_end", CommonFunction.GetNetTime(p.event_end).ToString("yyyy/MM/dd HH:mm:ss")),
                           new JProperty("prepaid", p.prepaid)
                           )))
                    ).ToString();
                // json = "{succes:true,totalCount:" + totalCount + ",item:" + JsonConvert.SerializeObject(pros) + "}";
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
    }
}