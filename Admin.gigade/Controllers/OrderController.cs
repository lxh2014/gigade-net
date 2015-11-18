using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using System.Configuration;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Model.Custom;
using Newtonsoft.Json;
using BLL.gigade.Common;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;
using System.IO;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text.RegularExpressions;
namespace Admin.gigade.Controllers
{
    [HandleError]
    public class OrderController : Controller
    {
        //
        // GET: /Order/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        static string excelPath = ConfigurationManager.AppSettings["ImportOrderExcel"];
        static string pdfPath = ConfigurationManager.AppSettings["ImportOrderPDF"];
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        static string excelPath_export = ConfigurationManager.AppSettings["ImportUserIOExcel"];

        private IOrderImport orderImport;
        private IChannelImplMgr channelMgr;
        private IImportOrdersLogImplMgr importOrdersLogMgr;
        private ISiteConfigImplMgr siteConfigMgr;
        private IChannelShippingImplMgr channelShipMgr;
        private IProductComboImplMgr _prodCombMgr;
        private IProductItemMapImplMgr productItemMapMgr;
        private IProductItemImplMgr _proItemMgr;
        private IPriceMasterImplMgr _priceMasterMgr;
        private IOrderMasterImplMgr _OrderMasterMgr;
        private IBonusMasterImplMgr _bonusMasterMgr;
        private ZipMgr zMgr;
        private IProductCategoryImplMgr _productCategoryMgr;
        private IOrderDetailImplMgr _orderDetialMgr;
        [CustomHandleError]
        public ActionResult Index()
        {

            return View();
        }

        [CustomHandleError]
        public ActionResult OrderAdd()
        {
            return View();
        }

        //add by Jiajun 2014/10/23
        public ActionResult InteriorOrderAdd()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult OrderMasterIn()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult ArrorOrder()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public ActionResult OrderStatusModify()
        {
            return View();
        }

        public ActionResult OrderCategorySum()
        {
            return View();
        }

        public ActionResult OrderAmountDetial()
        {
            if (!string.IsNullOrEmpty(Request.Params["_parameters"]))
            {
                string s = Request.Params["_parameters"];
                string[] arr = s.Split('|');
                for (int i = 0; i < arr.Length; i++)
                { 
                    switch(i)
                    {
                        case 0:
                            ViewBag.category_id = arr[i];
                            break;
                        case 1:
                            ViewBag.category_name = arr[i];
                            break;
                        case 2:
                            ViewBag.category_amount = arr[i];
                            break;
                        case 3:
                            ViewBag.category_status = arr[i];
                            break;
                        case 4:
                            ViewBag.date_stauts = arr[i];
                            break;
                        case 5:
                            ViewBag.date_start = Convert.ToDateTime(arr[i]);
                            ViewBag.date_start = Convert.ToDateTime(ViewBag.date_start.ToString("yyyy-MM-dd 00:00:00"));
                            break;
                        case 6:
                            ViewBag.date_end = Convert.ToDateTime(arr[i]);
                            ViewBag.date_end = Convert.ToDateTime(ViewBag.date_end.ToString("yyyy-MM-dd  23:59:59"));
                            break;
                    }
                
                }
            }
            else
            {
                ViewBag.category_id = "5";
                ViewBag.category_status = "0";
                ViewBag.date_stauts = "0";
                ViewBag.date_start = CommonFunction.DateTimeToString(Convert.ToDateTime(DateTime.MinValue.ToString("yyyy-MM-dd 00:00:00")));
                ViewBag.date_end = CommonFunction.DateTimeToString(Convert.ToDateTime(DateTime.MinValue.ToString("yyyy-MM-dd 23:59:59")));
            }
           
            return View();
        }

        #region Gigade商品查詢
        #region 獲取組合商品下的子商品
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase getChildren()
        {
            string pid = Request.Params["pid"];
            string price_type = Request.Params["price_type"];

            string jsonStr = "{success:false}";

            _prodCombMgr = new ProductComboMgr(connectionString);
            List<ProductComboCustom> prodComList = _prodCombMgr.getChildren(new ProductComboCustom { Parent_Id = int.Parse(pid), price_type = int.Parse(price_type) });


            jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(prodComList) + "}";
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 內部訂單輸入組合商品>商品ID查詢
        /// <summary>
        /// 订单输入查询Giagde站台下的組合商品
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase OrderInfoQueryByGigade()
        {
            string jsonStr = "{success:false}";
            uint pid = 0;
            uint spec1 = 0;
            uint spec2 = 0;
            UInt32.TryParse(Request.Params["pid"] ?? "0", out pid);
            IProductImplMgr _proMgr = new ProductMgr(connectionString);
            ProductCombo pcombo = new ProductCombo();
            if (pid != 0)
            {
                long nowTime = BLL.gigade.Common.CommonFunction.GetPHPTime();//獲取當前時間  add by zhuoqin0830w  2015/11/09
                Product prod = _proMgr.Query(new Product { Product_Id = pid }).FirstOrDefault();
                if (prod != null)
                {
                    //判断商品是否上架
                    long ltime = BLL.gigade.Common.CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    bool upFlag = prod.Product_Status == 5 && ltime >= prod.Product_Start && ltime <= prod.Product_End;//判斷商品是否上架
                    if (Request.UrlReferrer.AbsolutePath == "/Order/InteriorOrderAdd" || upFlag)//edit by xiangwang0413w 內部訂單輸入不進行商品是否上架判斷
                    {
                        if (prod.Combination == 3)
                        {
                            #region 無規格任選
                            try
                            {
                                bool hasSpec = false;
                                //查詢子商品信息
                                _prodCombMgr = new ProductComboMgr(connectionString);
                                _proItemMgr = new ProductItemMgr(connectionString);
                                _priceMasterMgr = new PriceMasterMgr(connectionString);
                                List<ProductComboCustom> prodComList = _prodCombMgr.combQuery(new ProductComboCustom { Parent_Id = int.Parse(pid.ToString()) });
                                //組合商品信息
                                OrderComboAddCustom orderAdd = _proMgr.OrderQuery(new Product { Product_Id = pid }, 1, 0, 1); // add by wwei0216w 2015/2/10 將之前的0，1，1改成 1，0，1 因為參數不對查詢不出結果
                                orderAdd.childCount = prodComList.Count();
                                jsonStr = "{success:true";
                                double comboPrice = 0.0;     //組合商品的價格 ＝sum( 子商品價格 * 子商品必選數量(s_must_buy);
                                double comboCost = 0.0;  //組合商品的成本
                                double comboEventCost = 0.0;  //組合商品的活動成本  add by zhuoqin0830w  2015/11/09
                                int childSum = 0;      //必選商品的必購數量之和
                                int minStock = 0;
                                int stockIndex = 0;
                                List<OrderAddCustom> orderAddList = new List<OrderAddCustom>();   //存放必選商品集
                                //此循環是計算子商品的庫存，總價(供按比例拆分時計算子商品的價格使用)等其它信息
                                for (int i = 0, j = prodComList.Count(); i < j; i++)
                                {
                                    ProductComboCustom item = prodComList[i];
                                    uint c_product_id = uint.Parse(item.Child_Id.ToString());
                                    Product pResult = _proMgr.Query(new Product { Product_Id = c_product_id }).FirstOrDefault();
                                    //判斷子商品是否有規格
                                    if (pResult.Product_Spec > 0)
                                    {
                                        hasSpec = true;
                                        break;
                                    }
                                    //補貨中停止販售 1:是 0:否
                                    if (pResult.Shortage == 1)
                                    {
                                        jsonStr = "{success:false,msg:'" + Resources.OrderAdd.SHPRT_AGE + "'}";
                                        this.Response.Clear();
                                        this.Response.Write(jsonStr);
                                        this.Response.End();
                                        return this.Response;
                                    }
                                    OrderAddCustom oc = new OrderAddCustom();
                                    ProductItem pItemResult = _proItemMgr.Query(new ProductItem { Product_Id = c_product_id }).FirstOrDefault();
                                    if (pItemResult != null)
                                    {
                                        oc.Item_Id = pItemResult.Item_Id;
                                        oc.Item_Stock = pItemResult.Item_Stock;
                                        oc.s_must_buy = item.S_Must_Buy;
                                        oc.parent_id = int.Parse(pid.ToString());
                                        oc.ignore_stock = pResult.Ignore_Stock;
                                        //計算必先商品之最小庫存
                                        if (item.S_Must_Buy > 0)
                                        {
                                            oc.Product_Id = c_product_id;
                                            childSum += item.S_Must_Buy;
                                            if (stockIndex == 0)
                                            {
                                                minStock = int.Parse((pItemResult.Item_Stock / item.S_Must_Buy).ToString());
                                            }
                                            else
                                            {
                                                int curStock = int.Parse((pItemResult.Item_Stock / item.S_Must_Buy).ToString());
                                                if (curStock < minStock)
                                                {
                                                    minStock = curStock;
                                                }
                                            }
                                            stockIndex++;
                                        }
                                    }
                                    PriceMaster pm = null;
                                    if (prod.Price_type == 1)        //按比例拆分
                                    {
                                        pm = _priceMasterMgr.QueryPriceMaster(new PriceMaster
                                        {
                                            site_id = 1,
                                            child_id = 0,
                                            user_level = 1,
                                            user_id = 0,
                                            product_id = c_product_id
                                        });
                                    }
                                    else if (prod.Price_type == 2)      //各自定價
                                    {
                                        pm = _priceMasterMgr.QueryPriceMaster(new PriceMaster
                                        {
                                            site_id = 1,
                                            child_id = int.Parse(c_product_id.ToString()),
                                            user_level = 1,
                                            user_id = 0,
                                            product_id = pid
                                        });
                                    }
                                    if (pm != null)
                                    {
                                        //計算Item的價格
                                        ItemPriceCustom ipResult = new ItemPriceCustom();
                                        IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
                                        ipResult = iPMgr.Query(new ItemPrice { item_id = pItemResult.Item_Id, price_master_id = pm.price_master_id }).FirstOrDefault();

                                        oc.product_name = pm.product_name;
                                        oc.price_master_id = pm.price_master_id;
                                        oc.Item_Money = ipResult.item_money;
                                        oc.product_cost = ipResult.item_money;
                                        oc.original_price = int.Parse(ipResult.item_money.ToString());
                                        oc.Item_Cost = ipResult.item_cost;
                                        //計算必選商品之總價
                                        if (item.S_Must_Buy > 0)
                                        {
                                            comboPrice += int.Parse((ipResult.item_money * item.S_Must_Buy).ToString());
                                            comboCost += int.Parse((ipResult.item_cost * item.S_Must_Buy).ToString());
                                        }
                                        orderAddList.Add(oc);
                                    }
                                    else
                                    {
                                        jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_PRICE_NOT_EXIST + "'}";
                                        this.Response.Clear();
                                        this.Response.Write(jsonStr);
                                        this.Response.End();
                                        return this.Response;
                                    }
                                }
                                //將計算的最小庫存價賦值給組合商品
                                orderAdd.stock = minStock;
                                //各自定價時將組合商品的定價賦值給組合商品
                                if (prod.Price_type == 2)
                                {
                                    orderAdd.product_cost = int.Parse(comboPrice.ToString());
                                }
                                //如果子商品含有規格則提示
                                if (hasSpec)
                                {
                                    jsonStr = "{success:false,msg:'" + Resources.OrderAdd.COMBO_CHILD_NO_SPEC + "'}";
                                    this.Response.Clear();
                                    this.Response.Write(jsonStr);
                                    this.Response.End();
                                    return this.Response;
                                }
                                jsonStr += ",child:[";
                                string priceScales = "";//單一商品價格所占比例
                                string costScalses = ""; //單一商品成本所占比例
                                var rightList = orderAddList.Where(rec => rec.s_must_buy > 0).ToList();
                                var totalPrice = orderAdd.product_cost;  //組合商品的定價
                                var totalCost = orderAdd.cost;  //組合商品的成本
                                foreach (var item in rightList)
                                {
                                    //如果組合商品的價格類型為 按比例拆分 則需重新為必選子商品定價
                                    if (prod.Price_type == 1)
                                    {
                                        //if (comboCost != 0)
                                        if (comboPrice != 0)
                                        {
                                            //new logic  计算按比例拆分之比例
                                            double priceScale = double.Parse((item.Item_Money * item.s_must_buy / comboPrice).ToString());
                                            //double costScale = double.Parse((item.Item_Cost * item.s_must_buy / comboCost).ToString());
                                            priceScales += priceScale + ",";
                                            //costScalses += costScale + ",";
                                            int afterPrice = Convert.ToInt16(Math.Round(totalPrice * priceScale / item.s_must_buy));
                                            //int afterCost = Convert.ToInt16(Math.Round(totalCost * costScale / item.s_must_buy));
                                            comboPrice -= Convert.ToInt16(item.Item_Money * item.s_must_buy);
                                            //comboCost -= Convert.ToInt16(item.Item_Cost * item.s_must_buy);
                                            totalPrice -= afterPrice * item.s_must_buy;
                                            //totalCost -= afterCost * item.s_must_buy;
                                            item.product_cost = uint.Parse(afterPrice.ToString());
                                            //item.Item_Cost = uint.Parse(afterCost.ToString());
                                        }
                                        else
                                        {
                                            priceScales += "0";
                                            //costScalses += "0";
                                            item.product_cost = 0;
                                        }
                                    }
                                    jsonStr += JsonConvert.SerializeObject(item);
                                }
                                //jsonStr = jsonStr.Substring(0, jsonStr.Length - 1);
                                jsonStr = jsonStr.Replace("}{", "},{");
                                jsonStr += "],data:[";
                                //拼接組合商品   s_must_buy:當前任選中必選商品的記錄數
                                jsonStr += "{product_id:'" + orderAdd.product_id + "',product_name:'" + orderAdd.product_name + "',Item_Cost:'" + orderAdd.cost + "',product_cost:'" + orderAdd.product_cost + "',item_id:0";
                                jsonStr += ",child_scale:'" + priceScales + "',child_cost_scale:'" + costScalses + "',stock:'" + minStock + "',g_must_buy:'" + prodComList[0].G_Must_Buy + "',s_must_buy:" + rightList.Count() + "";
                                jsonStr += ",combination:" + orderAdd.child + ",buy_limit:" + prodComList[0].Buy_Limit + ",child:" + orderAdd.child + ",childCount:" + orderAdd.childCount + ",childSum:" + childSum + ",price_type:" + orderAdd.price_type + ",product_status_name:'" + (upFlag ? "上架" : "未上架") + "'}]}";
                            }
                            catch (Exception ex)
                            {
                                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                log.Error(logMessage);
                            }
                            #endregion
                        }
                        else if (prod.Combination == 2)
                        {
                            #region 固定組合
                            try
                            {
                                OrderComboAddCustom orderAdd = _proMgr.OrderQuery(new Product { Product_Id = pid }, 1, 0, 1);
                                jsonStr = "{success:true";
                                //查詢子商品信息
                                _prodCombMgr = new ProductComboMgr(connectionString);
                                List<ProductComboCustom> prodComList = _prodCombMgr.combQuery(new ProductComboCustom { Parent_Id = int.Parse(pid.ToString()) });
                                IProductItemImplMgr _proItemMgr = new ProductItemMgr(connectionString);
                                IPriceMasterImplMgr priceMgr = new PriceMasterMgr(connectionString);
                                foreach (var item in prodComList)
                                {
                                    Product pResult = _proMgr.Query(new Product { Product_Id = uint.Parse(item.Child_Id.ToString()) }).FirstOrDefault();
                                    //補貨中停止販售 1:是 0:否
                                    if (pResult.Shortage == 1)
                                    {
                                        jsonStr = "{success:false,msg:'" + Resources.OrderAdd.SHPRT_AGE + "'}";
                                        this.Response.Clear();
                                        this.Response.Write(jsonStr);
                                        this.Response.End();
                                        return this.Response;
                                    }
                                }
                                jsonStr += ",child:[";
                                double totalPrice = 0;
                                double totalCost = 0;
                                double totalEventCost = 0; //add by zhuoqin0830w  2015/11/09
                                string priceScales = "";//單一商品價格所占比例
                                string costScales = ""; //單一商品成本所占比例
                                string eventcostScales = "";//單一商品活動成本所占比例  add by zhuoqin0830w  2015/11/09
                                int minStock = 0;
                                if (prod.Price_type == 1)
                                {
                                    #region 按比例拆分
                                    //計算子商品的總價
                                    List<ProductItem> price_data_right_list = new List<ProductItem>();
                                    foreach (var item in prodComList)
                                    {
                                        bool findState = false;
                                        List<Product> pList = _proMgr.Query(new Product { Product_Id = uint.Parse(item.Child_Id.ToString()) });
                                        List<ProductItem> pItemList = _proItemMgr.Query(new ProductItem { Product_Id = uint.Parse(item.Child_Id.ToString()) });
                                        //在item_price中查找對應item_id的商品規格價格
                                        IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
                                        PriceMaster pM = priceMgr.QueryPriceMaster(new PriceMaster
                                        {
                                            product_id = uint.Parse(item.Child_Id.ToString()),
                                            user_id = 0,
                                            user_level = 1,
                                            site_id = 1,
                                            child_id = 0
                                        });
                                        List<ItemPriceCustom> ipList = new List<ItemPriceCustom>();

                                        if (pM != null)
                                        {
                                            //add by zhuoqin0830w  2015/11/06  添加判斷 活動時間是否過期  如果過期則在頁面上顯示的活動成本為 0 如果沒有過期則顯示活動成本
                                            if (nowTime > pM.event_end)
                                            {
                                                pM.event_cost = 0;
                                            }

                                            //遍歷此商品所有item_id,找出第一筆在item_price中的item_id返回，若無，則此組合商品不能加入訂單
                                            foreach (ProductItem items in pItemList)
                                            {
                                                ipList = iPMgr.Query(new ItemPrice
                                                {
                                                    item_id = uint.Parse(items.Item_Id.ToString()),
                                                    price_master_id = pM.price_master_id
                                                });
                                                if (ipList.Count == 1)
                                                {
                                                    if (pM.same_price == 1)
                                                    {
                                                        totalPrice += pM.price * item.S_Must_Buy;
                                                        totalCost += pM.cost * item.S_Must_Buy;
                                                        totalEventCost += pM.event_cost * item.S_Must_Buy;//add by zhuoqin0830w  2015/11/09
                                                    }
                                                    else
                                                    {
                                                        totalPrice += ipList[0].item_money * item.S_Must_Buy;
                                                        totalCost += ipList[0].item_cost * item.S_Must_Buy;
                                                        totalEventCost += ipList[0].event_cost * item.S_Must_Buy;//add by zhuoqin0830w  2015/11/09
                                                    }
                                                    price_data_right_list.Add(items);
                                                    findState = true;
                                                    break;
                                                }
                                            }
                                            if (!findState)
                                            {
                                                //在item_price中未查出對應價格
                                                this.Response.Clear();
                                                this.Response.Write("{success:false,msg:'" + Resources.OrderAdd.COMBO_CHILD_PRICE_NULL + "'}");
                                                this.Response.End();
                                                return this.Response;
                                            }
                                        }
                                    }
                                    int index = 0;
                                    int comboPrice = orderAdd.product_cost;   //組合商品的定價
                                    int comboCost = orderAdd.cost;
                                    int comboEventCost = 0;
                                    //add by zhuoqin0830w  2015/11/06  添加判斷 活動時間是否過期  如果過期則在頁面上顯示的活動成本為 0 如果沒有過期則顯示活動成本
                                    if (orderAdd != null)
                                    {
                                        if (nowTime > orderAdd.event_start && nowTime < orderAdd.event_end)
                                        {
                                            comboEventCost = orderAdd.event_cost;
                                        }
                                        else { orderAdd.event_cost = 0; }
                                    }
                                    for (int i = 0; i < prodComList.Count; i++)
                                    {
                                        index++;
                                        OrderAddCustom oc = new OrderAddCustom();
                                        oc.Product_Id = uint.Parse(prodComList[i].Child_Id.ToString());
                                        oc.parent_id = prodComList[i].Parent_Id;
                                        //讀取product_name
                                        IPriceMasterImplMgr pMgr = new PriceMasterMgr(connectionString);
                                        PriceMaster pm = pMgr.QueryPriceMaster(new PriceMaster
                                        {
                                            site_id = 1,
                                            child_id = 0,
                                            user_level = 1,
                                            user_id = 0,
                                            product_id = oc.Product_Id
                                        });
                                        if (pm != null)
                                        {
                                            oc.product_name = pm.product_name;
                                        }
                                        oc.s_must_buy = prodComList[i].S_Must_Buy;
                                        if (price_data_right_list.Count != 0)
                                        {
                                            if (index == 1)
                                            {
                                                //最小庫存為組合下子商品的最小庫存/該子商品必購數量（如果必購數量為0則按1計算）
                                                minStock = int.Parse((price_data_right_list[i].Item_Stock / (prodComList[i].S_Must_Buy == 0 ? 1 : prodComList[i].S_Must_Buy)).ToString());
                                            }
                                            Product pResult = _proMgr.Query(new Product { Product_Id = uint.Parse(prodComList[i].Child_Id.ToString()) }).FirstOrDefault();
                                            //有規格
                                            oc.Spec_Name_1 = price_data_right_list[i].Spec_Name_1;
                                            oc.Spec_Name_2 = price_data_right_list[i].Spec_Name_2;
                                            oc.Item_Stock = price_data_right_list[i].Item_Stock;
                                            oc.Item_Id = price_data_right_list[i].Item_Id;
                                            int curStock = int.Parse((oc.Item_Stock / (prodComList[i].S_Must_Buy == 0 ? 1 : prodComList[i].S_Must_Buy)).ToString());
                                            if (curStock < minStock)
                                            {
                                                minStock = curStock;
                                            }
                                            oc.Spec_Id_1 = price_data_right_list[i].Spec_Id_1;
                                            oc.Spec_Id_2 = price_data_right_list[i].Spec_Id_2;
                                            oc.ignore_stock = pResult.Ignore_Stock;
                                            //在item_price中查找對應item_id的商品規格價格
                                            IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
                                            PriceMaster pM = priceMgr.QueryPriceMaster(new PriceMaster
                                            {
                                                product_id = uint.Parse(price_data_right_list[i].Product_Id.ToString()),
                                                user_id = 0,
                                                user_level = 1,
                                                site_id = 1,
                                                child_id = 0
                                            });
                                            List<ItemPriceCustom> ipList = new List<ItemPriceCustom>();
                                            if (pM != null)
                                            {
                                                double singlePrice = 0;
                                                double singleCost = 0;
                                                double singleEventCost = 0;//add by zhuoqin0830w  2015/11/09

                                                if (pM.same_price == 1)
                                                {
                                                    oc.original_price = pM.price;
                                                    oc.Item_Cost = uint.Parse(pM.cost.ToString());
                                                    oc.Event_Item_Cost = uint.Parse(pm.event_cost.ToString());//add by zhuoqin0830w  2015/11/09
                                                    singlePrice = pM.price * oc.s_must_buy;
                                                    singleCost = pM.cost * oc.s_must_buy;
                                                    singleEventCost = pM.event_cost * oc.s_must_buy;//add by zhuoqin0830w  2015/11/09
                                                }
                                                else
                                                {
                                                    ipList = iPMgr.Query(new ItemPrice
                                                    {
                                                        item_id = uint.Parse(price_data_right_list[i].Item_Id.ToString()),
                                                        price_master_id = pM.price_master_id
                                                    });
                                                    if (ipList.Count == 1)
                                                    {
                                                        oc.original_price = int.Parse(ipList[0].item_money.ToString());
                                                        oc.Item_Cost = ipList[0].item_cost;
                                                        oc.Event_Item_Cost = ipList[0].event_cost;//add by zhuoqin0830w  2015/11/09
                                                        singlePrice = ipList[0].item_money * oc.s_must_buy;
                                                        singleCost = ipList[0].item_cost * oc.s_must_buy;
                                                        singleEventCost = ipList[0].event_cost * oc.s_must_buy;//add by zhuoqin0830w  2015/11/09
                                                    }
                                                }
                                                if (totalPrice == 0)
                                                {
                                                    priceScales += "0";
                                                    costScales += "0";
                                                    eventcostScales += "0";//add by zhuoqin0830w  2015/11/09
                                                    oc.product_cost = 0;
                                                }
                                                else
                                                {
                                                    //new logic 算比例
                                                    double priceScale = double.Parse((singlePrice / totalPrice).ToString());
                                                    double costScale = double.Parse((singleCost / totalCost).ToString());
                                                    //add by zhuoqin0830w  2015/11/09  添加活動成本的比例
                                                    double eventcostScale = 0;
                                                    if (totalEventCost != 0)
                                                    {
                                                        eventcostScale = double.Parse((singleEventCost / totalEventCost).ToString());
                                                    }
                                                    else
                                                    {
                                                        eventcostScale = priceScale;// 如果 活動成本的比例為0 則表示沒有活動成本則按照價格的比例進行計算
                                                    }

                                                    priceScales += priceScale + ",";
                                                    costScales += costScale + ",";
                                                    eventcostScales += eventcostScale + ",";//add by zhuoqin0830w  2015/11/09

                                                    var afterprice = Convert.ToInt16(Math.Round(comboPrice * priceScale / oc.s_must_buy));
                                                    var aftercost = Convert.ToInt16(Math.Round(comboCost * costScale / oc.s_must_buy));
                                                    var aftereventcost = Convert.ToInt16(Math.Round(comboEventCost * eventcostScale / oc.s_must_buy));//add by zhuoqin0830w  2015/11/09

                                                    comboPrice -= afterprice * oc.s_must_buy;
                                                    comboCost -= aftercost * oc.s_must_buy;
                                                    comboEventCost -= aftereventcost * oc.s_must_buy;//add by zhuoqin0830w  2015/11/09

                                                    totalPrice -= singlePrice;
                                                    totalCost -= singleCost;
                                                    totalEventCost -= singleEventCost;//add by zhuoqin0830w  2015/11/09

                                                    oc.product_cost = uint.Parse(afterprice.ToString());
                                                    oc.Item_Cost = uint.Parse(aftercost.ToString());
                                                    oc.Event_Item_Cost = uint.Parse(aftereventcost.ToString());//add by zhuoqin0830w  2015/11/09
                                                }
                                            }
                                        }
                                        jsonStr += JsonConvert.SerializeObject(oc) + ",";
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 各自定價
                                    orderAdd.product_cost = 0;
                                    foreach (var item in prodComList)
                                    {
                                        Product pResult = _proMgr.Query(new Product { Product_Id = uint.Parse(item.Child_Id.ToString()) }).FirstOrDefault();
                                        OrderAddCustom oc = new OrderAddCustom();
                                        oc.price_type = prod.Price_type;
                                        oc.parent_id = item.Parent_Id;
                                        oc.Product_Id = uint.Parse(item.Child_Id.ToString());
                                        List<ProductItem> pItemList = _proItemMgr.Query(new ProductItem { Product_Id = uint.Parse(item.Child_Id.ToString()) });
                                        if (prodComList.IndexOf(item) == 0)
                                        {
                                            minStock = int.Parse((pItemList[0].Item_Stock / (item.S_Must_Buy == 0 ? 1 : item.S_Must_Buy)).ToString()); //最小庫存為組合下子商品的最小庫存/該子商品必購數量（如果必購數量為0則按1計算）
                                        }
                                        PriceMaster pM = priceMgr.QueryPriceMaster(new PriceMaster
                                        {
                                            product_id = pid,
                                            user_id = 0,
                                            user_level = 1,
                                            site_id = 1,
                                            child_id = Int32.Parse(item.Child_Id) //add by wangwei02016w 2014/9/24 
                                        });
                                        ItemPriceCustom ip = new ItemPriceCustom();
                                        IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
                                        if (pM != null)
                                        {
                                            oc.product_name = pM.product_name;
                                            oc.price_master_id = pM.price_master_id;

                                            ip = iPMgr.Query(new ItemPrice
                                            {
                                                item_id = uint.Parse(pItemList[0].Item_Id.ToString()),
                                                price_master_id = pM.price_master_id
                                            }).First();

                                            oc.Spec_Name_1 = pItemList[0].Spec_Name_1;
                                            oc.Spec_Name_2 = pItemList[0].Spec_Name_2;
                                            oc.Item_Stock = pItemList[0].Item_Stock;
                                            oc.Spec_Id_1 = pItemList[0].Spec_Id_1;
                                            oc.Spec_Id_2 = pItemList[0].Spec_Id_2;
                                            oc.ignore_stock = pResult.Ignore_Stock;
                                            int curStock = int.Parse((pItemList[0].Item_Stock / (item.S_Must_Buy == 0 ? 1 : item.S_Must_Buy)).ToString());
                                            if (curStock < minStock)
                                            {
                                                minStock = curStock;
                                            }
                                            oc.Item_Id = pItemList[0].Item_Id;
                                            oc.product_cost = ip.item_money;
                                            oc.original_price = int.Parse(ip.item_money.ToString());
                                            orderAdd.product_cost += Convert.ToInt32(oc.product_cost * item.S_Must_Buy);
                                        }
                                        oc.s_must_buy = item.S_Must_Buy;
                                        jsonStr += JsonConvert.SerializeObject(oc) + ",";
                                    }
                                    #endregion
                                }
                                jsonStr = jsonStr.Substring(0, jsonStr.Length - 1) + "]";
                                jsonStr += ",data:[";
                                //拼接組合商品   cost為組合商品之成本,product_cost為組合商品之售價
                                jsonStr += "{product_id:'" + orderAdd.product_id + "',product_name:'" + orderAdd.product_name + "',Item_Cost:'" + orderAdd.cost + "',product_cost:'" + orderAdd.product_cost + "',item_id:0" + ",Event_Item_Cost:" + orderAdd.event_cost;
                                jsonStr += ",s_must_buy:'" + prodComList.Count + "',child_scale:'" + priceScales + "',child_cost_scale:'" + costScales + "',child_event_cost_scale:'" + eventcostScales + "',stock:'" + minStock + "',g_must_buy:'" + prodComList.Count + "',child:" + orderAdd.child + ",product_status_name:'" + (upFlag ? "上架" : "未上架") + "'}";
                                jsonStr += "]}";
                            }
                            catch (Exception ex)
                            {
                                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                log.Error(logMessage);
                            }
                            #endregion
                        }
                        else
                        {
                            #region 單一商品
                            // 判斷 輸入的商品是否是單一商品 並且是 內部訂單輸入  如果是 則彈出提示框  add by zhuoqin0830w 2015/07/10
                            if (Request.UrlReferrer.AbsolutePath == "/Order/InteriorOrderAdd")
                            {
                                if (prod.Combination == 1 && Convert.ToInt32(Request.Params["pid"].Length) != 6 && Request.Params["parent_id"] == "")
                                {
                                    this.Response.Clear();
                                    this.Response.Write("{success:false,msg:'單一商品必須輸入六碼編號！'}");
                                    this.Response.End();
                                    return this.Response;
                                }
                            }
                            IProductItemImplMgr _proItemMgr = new ProductItemMgr(connectionString);
                            Product p = new Product();
                            p.Product_Id = pid;
                            ProductItem pItem = new ProductItem();
                            pItem.Product_Id = pid;
                            UInt32.TryParse(Request.Params["spec1"] ?? "0", out spec1);
                            UInt32.TryParse(Request.Params["spec2"] ?? "0", out spec2);
                            pItem.Spec_Id_1 = spec1;
                            pItem.Spec_Id_2 = spec2;
                            try
                            {
                                List<OrderAddCustom> oalist = new List<OrderAddCustom>();
                                OrderAddCustom oc = new OrderAddCustom();
                                List<Product> pList = _proMgr.Query(p);
                                List<ProductItem> pItemList = _proItemMgr.Query(pItem);
                                if (pList.Count() <= 0 || pItemList.Count() <= 0)
                                {
                                    jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_NOT_EXIST + "'}";//商品不存在
                                    this.Response.Clear();
                                    this.Response.Write(jsonStr);
                                    this.Response.End();
                                    return this.Response;
                                }
                                //補貨中停止販售 1:是 0:否
                                if (pList[0].Shortage == 1)
                                {
                                    jsonStr = "{success:false,msg:'" + Resources.OrderAdd.SHPRT_AGE + "'}";//補貨中不能販賣
                                    this.Response.Clear();
                                    this.Response.Write(jsonStr);
                                    this.Response.End();
                                    return this.Response;
                                }
                                int parent_id = 0;
                                int price_type = 0;
                                uint c_combination = 0;
                                int c_buy_limit = 0;
                                //組合商品子商品查詢
                                if (!string.IsNullOrEmpty(Request.Params["parent_id"]))
                                {
                                    parent_id = int.Parse(Request.Params["parent_id"]);
                                    //查詢組合商品的價格類型
                                    Product parentResult = _proMgr.Query(new Product { Product_Id = uint.Parse(parent_id.ToString()) }).FirstOrDefault();
                                    if (parentResult != null)
                                    {
                                        price_type = parentResult.Price_type;
                                        c_combination = parentResult.Combination;
                                    }
                                    oc.price_type = price_type;
                                    //找出該商品在組合商品中的必購數量
                                    _prodCombMgr = new ProductComboMgr(connectionString);
                                    List<ProductComboCustom> prodComList = _prodCombMgr.combQuery(new ProductComboCustom { Parent_Id = parent_id, Child_Id = pid.ToString() });//add by wangwei02016w 2014/9/24 
                                    if (prodComList.Count == 1)
                                    {
                                        oc.s_must_buy = prodComList[0].S_Must_Buy;
                                        c_buy_limit = prodComList[0].Buy_Limit;
                                    }
                                }
                                //在item_price中查找對應item_id的商品規格價格
                                IPriceMasterImplMgr priceMgr = new PriceMasterMgr(connectionString);
                                IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
                                PriceMaster pM = null;
                                if (price_type == 2)        //各自定價時取價格
                                {
                                    pM = priceMgr.QueryPriceMaster(new PriceMaster
                                    {
                                        product_id = uint.Parse(parent_id.ToString()),
                                        user_id = 0,
                                        user_level = 1,
                                        site_id = 1,
                                        child_id = int.Parse(pList[0].Product_Id.ToString())
                                    });
                                }
                                else
                                {
                                    pM = priceMgr.QueryPriceMaster(new PriceMaster
                                    {
                                        product_id = pList[0].Product_Id,
                                        user_id = 0,
                                        user_level = 1,
                                        site_id = 1,
                                        child_id = 0
                                    });
                                }
                                if (pM != null)
                                {
                                    oc.price_master_id = pM.price_master_id;
                                    List<ItemPriceCustom> ipList = iPMgr.Query(new ItemPrice
                                    {
                                        item_id = uint.Parse(pItemList[0].Item_Id.ToString()),
                                        price_master_id = pM.price_master_id
                                    });

                                    oc.Product_Id = pList[0].Product_Id;
                                    oc.product_name = pList[0].Product_Name;
                                    oc.Spec_Id_1 = pItemList[0].Spec_Id_1;
                                    oc.Spec_Id_2 = pItemList[0].Spec_Id_2;
                                    oc.Item_Id = pItemList[0].Item_Id;
                                    oc.Item_Stock = pItemList[0].Item_Stock;
                                    if (ipList.Count() > 0)
                                    {
                                        //取成本
                                        if (pM.same_price == 1)
                                        {
                                            oc.Item_Cost = uint.Parse(pM.cost.ToString());
                                        }
                                        else
                                        {
                                            oc.Item_Cost = ipList[0].item_cost;
                                        }

                                        //add by zhuoqin0830w  2015/11/06  添加判斷 活動時間是否過期  如果過期則在頁面上顯示的活動成本為 0 如果沒有過期則顯示活動成本
                                        if (nowTime > pM.event_start && nowTime < pM.event_end)
                                        {
                                            oc.Event_Item_Cost = uint.Parse(pM.event_cost.ToString());
                                        }
                                        else { oc.Event_Item_Cost = 0; }

                                        if (nowTime > pItemList[0].Event_Product_Start && nowTime < pItemList[0].Event_Product_End)
                                        {
                                            if (pM.same_price == 1)
                                            {
                                                oc.Item_Money = uint.Parse(pM.event_price.ToString());
                                            }
                                            else
                                            {
                                                oc.Item_Money = ipList[0].event_money;
                                            }
                                        }
                                        else
                                        {
                                            if (pM.same_price == 1)
                                            {
                                                oc.Item_Money = uint.Parse(pM.price.ToString());
                                            }
                                            else
                                            {
                                                oc.Item_Money = ipList[0].item_money;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.Response.Clear();
                                        this.Response.Write("{success:false,msg:'" + Resources.OrderAdd.SINGLE_SPEC_PRICE_WRONG + "'}");
                                        this.Response.End();
                                        return this.Response;
                                    }
                                    IProductSpecImplMgr _specMgr = new BLL.gigade.Mgr.ProductSpecMgr(connectionString);
                                    List<ProductSpec> specList1 = _specMgr.query(Int32.Parse(pItemList[0].Product_Id.ToString()), "spec_id_1");
                                    List<ProductSpec> specList2 = _specMgr.query(Int32.Parse(pItemList[0].Product_Id.ToString()), "spec_id_2");
                                    oc.specList1 = specList1;
                                    oc.specList2 = specList2;
                                    oalist.Add(oc);
                                    StringBuilder stb = new StringBuilder();
                                    stb.Append("[");
                                    stb.Append("{product_id:" + oc.Product_Id + ",price_type:" + oc.price_type + ",child:" + c_combination + ",buy_limit:" + c_buy_limit + ",item_id:" + oc.Item_Id + ",product_name:'" + oc.product_name + "',");
                                    //添加 活動成本 金額顯示 ",Event_Item_Cost:" + pM.event_cost  zhuoqin0830w  2015/04/30
                                    stb.Append("product_cost:" + oc.Item_Money + ",Item_Cost:" + oc.Item_Cost + ",Event_Item_Cost:" + pM.event_cost + ",stock:" + oc.Item_Stock + ",s_must_buy:" + oc.s_must_buy + ",Spec_Name_1:'" + pItemList[0].Spec_Name_1 + "',Spec_Name_2:'" + pItemList[0].Spec_Name_2 + "',spec1:" + pItemList[0].Spec_Id_1 + ",spec2:" + pItemList[0].Spec_Id_2 + ",price_master_id:" + oc.price_master_id + ",ignore_stock:" + pList[0].Ignore_Stock + ",product_status_name:'" + (upFlag ? "上架" : "未上架") + "'}");
                                    stb.Append("]");
                                    jsonStr = "{success:true,data:" + stb.ToString() + "}";
                                }
                                else
                                {
                                    jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_PRICE_NOT_EXIST + "'}";
                                }
                            }
                            catch (Exception ex)
                            {
                                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                                log.Error(logMessage);
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_DOWN + "'}";
                    }
                }
                else
                {
                    jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_NOT_EXIST + "'}";
                }
            }
            else
            {
                jsonStr = "[{success:true,data:{product_id:''}}]";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 內部訂單輸入單一商品>商品細項查詢
        /// <summary>
        /// 使用商品細項查詢商品信息  add by zhuoqin0830w  2015/07/10 (內部訂單輸入單一商品>商品細項查詢)
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase OrderInfoQueryBySoleGigade()
        {
            string jsonStr = "{success:false}";
            uint itemID = 0;
            UInt32.TryParse(Request.Params["item_id"] ?? "0", out itemID);
            IProductItemImplMgr _productItemMgr = new ProductItemMgr(connectionString);
            IProductImplMgr _proMgr = new ProductMgr(connectionString);
            IPriceMasterImplMgr priceMgr = new PriceMasterMgr(connectionString);
            IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
            try
            {
                if (itemID != 0)
                {
                    //查詢出 product_item 中商品的價格
                    List<ProductItem> proItem = _productItemMgr.QueryPrice(new ProductItem { Item_Id = itemID });
                    if (proItem.Count != 0)
                    {
                        //查詢出 商品 信息
                        Product prod = _proMgr.Query(new Product { Product_Id = proItem[0].Product_Id }).FirstOrDefault();
                        if (prod != null)
                        {
                            //判断商品是否上架
                            long ltime = BLL.gigade.Common.CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            bool upFlag = prod.Product_Status == 5 && ltime >= prod.Product_Start && ltime <= prod.Product_End;//判斷商品是否上架
                            //判斷是否是內部訂單輸入
                            if (Request.UrlReferrer.AbsolutePath == "/Order/InteriorOrderAdd" || upFlag)
                            {
                                #region 單一商品
                                Product p = new Product();
                                p.Product_Id = proItem[0].Product_Id;
                                List<Product> pList = _proMgr.Query(p);
                                //判斷商品是否存在
                                if (pList.Count() <= 0)
                                {
                                    jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_NOT_EXIST + "'}";//商品不存在
                                    this.Response.Clear();
                                    this.Response.Write(jsonStr);
                                    this.Response.End();
                                    return this.Response;
                                }
                                //補貨中停止販售 1:是 0:否
                                if (pList[0].Shortage == 1)
                                {
                                    jsonStr = "{success:false,msg:'" + Resources.OrderAdd.SHPRT_AGE + "'}";//補貨中不能販賣
                                    this.Response.Clear();
                                    this.Response.Write(jsonStr);
                                    this.Response.End();
                                    return this.Response;
                                }

                                List<OrderAddCustom> oalist = new List<OrderAddCustom>();
                                OrderAddCustom oc = new OrderAddCustom();
                                int c_buy_limit = 0;
                                uint c_combination = 0;
                                PriceMaster pM = priceMgr.QueryPriceMaster(new PriceMaster
                                {
                                    product_id = pList[0].Product_Id,
                                    user_id = 0,
                                    user_level = 1,
                                    site_id = 1,
                                    child_id = 0
                                });
                                if (pM != null)
                                {
                                    oc.price_master_id = pM.price_master_id;
                                    List<ItemPriceCustom> ipList = iPMgr.Query(new ItemPrice
                                    {
                                        item_id = uint.Parse(proItem[0].Item_Id.ToString()),
                                        price_master_id = pM.price_master_id
                                    });

                                    oc.Product_Id = pList[0].Product_Id;
                                    oc.product_name = pList[0].Product_Name;
                                    oc.Spec_Id_1 = proItem[0].Spec_Id_1;
                                    oc.Spec_Id_2 = proItem[0].Spec_Id_2;
                                    oc.Item_Id = proItem[0].Item_Id;
                                    oc.Item_Stock = proItem[0].Item_Stock;
                                    long nowTime = BLL.gigade.Common.CommonFunction.GetPHPTime();
                                    if (ipList.Count() > 0)
                                    {
                                        if (pM.same_price == 1)
                                        {
                                            oc.Item_Cost = uint.Parse(pM.cost.ToString());
                                        }
                                        else
                                        {
                                            oc.Item_Cost = ipList[0].item_cost;
                                        }

                                        //add by zhuoqin0830w  2015/11/06  添加判斷 活動時間是否過期  如果過期則在頁面上顯示的活動成本為 0 如果沒有過期則顯示活動成本
                                        if (nowTime > pM.event_start && nowTime < pM.event_end)
                                        {
                                            oc.Event_Item_Cost = uint.Parse(pM.event_cost.ToString());
                                        }
                                        else { oc.Event_Item_Cost = 0; }

                                        if (nowTime > proItem[0].Event_Product_Start && nowTime < proItem[0].Event_Product_End)
                                        {
                                            if (pM.same_price == 1)
                                            {
                                                oc.Item_Money = uint.Parse(pM.event_price.ToString());
                                            }
                                            else
                                            {
                                                oc.Item_Money = ipList[0].event_money;
                                            }
                                        }
                                        else
                                        {
                                            if (pM.same_price == 1)
                                            {
                                                oc.Item_Money = uint.Parse(pM.price.ToString());
                                            }
                                            else
                                            {
                                                oc.Item_Money = ipList[0].item_money;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.Response.Clear();
                                        this.Response.Write("{success:false,msg:'" + Resources.OrderAdd.SINGLE_SPEC_PRICE_WRONG + "'}");
                                        this.Response.End();
                                        return this.Response;
                                    }
                                    oalist.Add(oc);
                                    StringBuilder stb = new StringBuilder();
                                    stb.Append("[");
                                    stb.Append("{product_id:" + oc.Product_Id + ",price_type:" + oc.price_type + ",child:" + c_combination + ",buy_limit:" + c_buy_limit + ",item_id:" + oc.Item_Id + ",product_name:'" + oc.product_name + "',");
                                    stb.Append("product_cost:" + oc.Item_Money + ",Item_Cost:" + oc.Item_Cost + ",Event_Item_Cost:" + oc.Event_Item_Cost + ",stock:" + oc.Item_Stock + ",s_must_buy:" + oc.s_must_buy + ",Spec_Name_1:'" + proItem[0].Spec_Name_1 + "',Spec_Name_2:'" + proItem[0].Spec_Name_2 + "',spec1:" + proItem[0].Spec_Id_1 + ",spec2:" + proItem[0].Spec_Id_2 + ",price_master_id:" + oc.price_master_id + ",ignore_stock:" + pList[0].Ignore_Stock + ",product_status_name:'" + (upFlag ? "上架" : "未上架") + "'}");
                                    stb.Append("]");
                                    jsonStr = "{success:true,data:" + stb.ToString() + "}";
                                }
                                else
                                {
                                    jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_PRICE_NOT_EXIST + "'}";
                                }
                                #endregion
                            }
                            else
                            {
                                jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_DOWN + "'}";
                            }
                        }
                        else
                        {
                            jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_NOT_EXIST + "'}";
                        }
                    }
                    else
                    {
                        jsonStr = "{success:false,msg:'" + Resources.OrderAdd.PRODUCT_NOT_EXIST + "'}";
                    }
                }
                else
                {
                    jsonStr = "[{success:true,data:{item_id:''}}]";
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
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #endregion

        #region 合作外站商品查詢
        /// <summary>
        /// 合作外站商品查詢
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase OrderInfoQueryByCooperator()
        {
            string jsonStr = "{success:false}";
            string pid = Request.Params["pid"] ?? "";
            uint channelId = 0;
            UInt32.TryParse(Request.Params["channelId"] ?? "0", out channelId);
            if (!string.IsNullOrEmpty(pid))
            {
                IProductItemMapImplMgr _pMapMgr = new ProductItemMapMgr(connectionString);
                IProductItemImplMgr _proItemMgr = new ProductItemMgr(connectionString);
                try
                {
                    ProductItemMap pMap = new ProductItemMap();

                    pMap.channel_detail_id = pid;
                    pMap.channel_id = channelId;
                    List<ProductItemMap> pMapList = _pMapMgr.QueryAll(pMap);
                    IPriceMasterImplMgr priceMgr = new PriceMasterMgr(connectionString);
                    IProductItemImplMgr _itemMgr = new ProductItemMgr(connectionString);
                    PriceMaster priceMap = priceMgr.QueryPriceMaster(new PriceMaster { price_master_id = pMapList.FirstOrDefault().price_master_id });
                    if (pMapList.Count() == 0)
                    {
                        jsonStr = "{success:false}";
                        this.Response.Clear();
                        this.Response.Write(jsonStr);
                        this.Response.End();
                        return this.Response;
                    }
                    uint product_id = pMapList[0].product_id;
                    if (product_id == 0)
                    {
                        product_id = _itemMgr.Query(new ProductItem { Item_Id = uint.Parse(pMapList[0].item_id.ToString()) }).FirstOrDefault().Product_Id;
                    }

                    IProductImplMgr _proMgr = new ProductMgr(connectionString);
                    Product prod = _proMgr.Query(new Product { Product_Id = product_id }).FirstOrDefault();

                    //判断商品是否上架
                    // long ltime = BLL.gigade.Common.CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    if (prod.Combination != 1 && prod.Combination != 0)
                    {
                        OrderComboAddCustom orderAdd = _proMgr.OrderQuery(new Product { Product_Id = product_id }, priceMap.user_level, priceMap.user_id, priceMap.site_id);
                        orderAdd.product_id = 0;
                        orderAdd.out_product_id = pMapList[0].channel_detail_id;
                        orderAdd.product_name = pMapList[0].product_name;
                        orderAdd.product_cost = pMapList[0].product_price;
                        jsonStr = "{success:true";
                        string price_scale = "", cost_scale = "";
                        //查詢子商品信息
                        _prodCombMgr = new ProductComboMgr(connectionString);
                        List<ProductComboCustom> prodComList = _prodCombMgr.combQuery(new ProductComboCustom { Parent_Id = int.Parse(product_id.ToString()) });

                        IPriceMasterImplMgr pMgr = new PriceMasterMgr(connectionString);
                        PriceMaster query = new PriceMaster { user_id = priceMap.user_id, user_level = priceMap.user_level, site_id = priceMap.site_id };

                        jsonStr += ",child:[";
                        double childTotalPrice = 0;
                        double childTotalCost = 0;
                        string[] itemList = pMapList[0].group_item_id.ToString().Split(',');
                        foreach (var item in prodComList)
                        {
                            #region 計算子商品總價

                            for (int i = 0; i < itemList.Length; i++)
                            {
                                //在item_price中查找該itemid對應的價格
                                IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);

                                if (prod.Price_type == 1)
                                {
                                    query.product_id = uint.Parse(item.Child_Id.ToString());
                                    query.child_id = 0;
                                }
                                else if (prod.Price_type == 2)
                                {
                                    query.product_id = prod.Product_Id;
                                    query.child_id = int.Parse(item.Child_Id.ToString());
                                }


                                PriceMaster pM = priceMgr.QueryPriceMaster(query);
                                List<ItemPriceCustom> ipList = new List<ItemPriceCustom>();

                                //查詢子商品詳細信息（規格，庫存）
                                ProductItem pItem = new ProductItem();
                                pItem.Item_Id = uint.Parse(itemList[i]);
                                pItem.Product_Id = uint.Parse(item.Child_Id.ToString());
                                List<ProductItem> pItemList = _proItemMgr.Query(pItem);

                                if (pM != null && pItemList.Count > 0)
                                {
                                    //查找外站商品必購數量
                                    IProductMapSetImplMgr msMgr = new ProductMapSetMgr(connectionString);
                                    List<ProductMapSet> mSetList = msMgr.Query(new ProductMapSet { map_rid = uint.Parse(pMapList[0].rid.ToString()), item_id = uint.Parse(itemList[i]) });
                                    if (pM.same_price == 1)
                                    {
                                        childTotalPrice += int.Parse((pM.price * mSetList[0].set_num).ToString());
                                        childTotalCost += int.Parse((pM.cost * mSetList[0].set_num).ToString());
                                    }
                                    else
                                    {
                                        ipList = iPMgr.Query(new ItemPrice
                                        {
                                            item_id = uint.Parse(itemList[i]),
                                            price_master_id = pM.price_master_id
                                        });
                                        if (ipList.Count != 0)
                                        {
                                            if (mSetList.Count == 1)
                                            {
                                                childTotalPrice += int.Parse((ipList[0].item_money * mSetList[0].set_num).ToString());
                                                childTotalCost += int.Parse((ipList[0].item_cost * mSetList[0].set_num).ToString());
                                            }
                                            else
                                            {
                                                //在item_price中未查出對應價格
                                                this.Response.Clear();
                                                this.Response.Write("{success:false,msg:'" + Resources.OrderAdd.COMBO_CHILD_PRICE_NULL + "'}");
                                                this.Response.End();
                                                return this.Response;
                                            }
                                        }
                                    }
                                }


                            }
                            #endregion
                        }

                        double beforePrice = 0;
                        double beforeCost = 0;
                        int minStock = 0;
                        int index = 0;
                        double comboPrice = orderAdd.product_cost;
                        int curIndex = 0;
                        foreach (var item in prodComList)
                        {
                            curIndex++;
                            OrderAddCustom oc = new OrderAddCustom();
                            List<Product> pList = _proMgr.Query(new Product { Product_Id = uint.Parse(item.Child_Id.ToString()) });
                            oc.price_type = prod.Price_type;
                            oc.Product_Id = uint.Parse(item.Child_Id.ToString());
                            oc.ignore_stock = pList[0].Ignore_Stock;
                            oc.newparent_id = pMapList[0].channel_detail_id;
                            //補貨中是否還能販賣
                            if (pList.FirstOrDefault().Shortage == 1)
                            {
                                this.Response.Clear();
                                this.Response.Write("{success:false,msg:'" + Resources.OrderAdd.SHPRT_AGE + "'}");
                                this.Response.End();
                                return this.Response;
                            }
                            //讀取product_name
                            if (prod.Price_type == 1)
                            {
                                query.product_id = oc.Product_Id;
                                query.child_id = 0;
                            }
                            else if (prod.Price_type == 2)
                            {
                                query.product_id = prod.Product_Id;
                                query.child_id = Convert.ToInt32(oc.Product_Id);
                            }
                            PriceMaster pm = pMgr.QueryPriceMaster(query);
                            if (pm != null)
                            {
                                oc.product_name = pm.product_name;
                            }


                            for (int i = 0; i < itemList.Length; i++)
                            {
                                //查詢子商品詳細信息（規格，庫存）
                                ProductItem pItem = new ProductItem();
                                pItem.Item_Id = uint.Parse(itemList[i]);
                                pItem.Product_Id = uint.Parse(item.Child_Id.ToString());
                                oc.Item_Id = pItem.Item_Id;
                                List<ProductItem> pItemList = _proItemMgr.Query(pItem);
                                if (pItemList.Count() != 0)
                                {
                                    index++;

                                    //在item_price中查找該itemid對應的價格
                                    IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
                                    List<ItemPriceCustom> ipList = new List<ItemPriceCustom>();
                                    if (pm != null)
                                    {
                                        oc.price_master_id = pm.price_master_id;
                                        //查找外站商品必購數量
                                        IProductMapSetImplMgr msMgr = new ProductMapSetMgr(connectionString);
                                        List<ProductMapSet> mSetList = msMgr.Query(new ProductMapSet { map_rid = uint.Parse(pMapList[0].rid.ToString()), item_id = uint.Parse(itemList[i]) });
                                        if (mSetList.Count != 0)
                                        {
                                            oc.s_must_buy = int.Parse(mSetList[0].set_num.ToString());
                                        }
                                        if (prod.Price_type == 1)
                                        {
                                            #region 按比例拆分


                                            if (i == itemList.Length - 1 && curIndex == prodComList.Count)
                                            {
                                                oc.product_cost = uint.Parse(CommonFunction.Math4Cut5Plus((orderAdd.product_cost - beforePrice) / oc.s_must_buy).ToString());
                                                oc.Item_Cost = uint.Parse(CommonFunction.Math4Cut5Plus((orderAdd.cost - beforeCost) / oc.s_must_buy).ToString());

                                            }
                                            else
                                            {
                                                double price = 0.0;
                                                double cost = 0.0;
                                                string price_res = "0";
                                                string cost_res = "0";
                                                double price_result = 0.0;
                                                double cost_result = 0.0;

                                                //规格同价
                                                if (pm.same_price == 1)
                                                {
                                                    if (childTotalPrice != 0)
                                                    {
                                                        double singlePrice = double.Parse((pm.price * oc.s_must_buy).ToString());
                                                        double totalPrice = childTotalPrice;
                                                        double c_scale = singlePrice / totalPrice;
                                                        price = orderAdd.product_cost * c_scale / oc.s_must_buy;
                                                        price_result = CommonFunction.Math4Cut5Plus(price);
                                                        price_res = c_scale.ToString();
                                                    }
                                                    beforePrice += price_result;
                                                    oc.product_cost = uint.Parse(price_result.ToString());
                                                    price_scale += price_res + ",";

                                                    if (childTotalCost != 0)
                                                    {
                                                        double singleCost = double.Parse((pm.cost * oc.s_must_buy).ToString());
                                                        double totalCost = childTotalCost;
                                                        double c_scale = singleCost / totalCost;
                                                        cost = orderAdd.cost * c_scale / oc.s_must_buy;
                                                        cost_result = CommonFunction.Math4Cut5Plus(cost);
                                                        cost_res = c_scale.ToString();
                                                    }
                                                    beforeCost += cost_result;
                                                    oc.Item_Cost = uint.Parse(cost_result.ToString());
                                                    cost_scale += cost_res + ",";


                                                }
                                                else
                                                {
                                                    ipList = iPMgr.Query(new ItemPrice
                                                    {
                                                        item_id = uint.Parse(itemList[i]),
                                                        price_master_id = pm.price_master_id
                                                    });
                                                    if (ipList.Count != 0)
                                                    {
                                                        if (childTotalPrice != 0)
                                                        {
                                                            double singlePrice = double.Parse((ipList[0].item_money * oc.s_must_buy).ToString());
                                                            double totalPrice = childTotalPrice;
                                                            double c_scale = singlePrice / totalPrice;
                                                            price = orderAdd.product_cost * c_scale / oc.s_must_buy;
                                                            price_result = CommonFunction.Math4Cut5Plus(price);
                                                            price_res = c_scale.ToString();
                                                        }
                                                        beforePrice += price_result;
                                                        oc.product_cost = uint.Parse(price_result.ToString());
                                                        price_scale += price_res + ",";

                                                        if (childTotalCost != 0)
                                                        {
                                                            double singleCost = double.Parse((ipList[0].item_cost * oc.s_must_buy).ToString());
                                                            double totalCost = childTotalCost;
                                                            double c_scale = singleCost / totalCost;
                                                            cost = orderAdd.cost * c_scale / oc.s_must_buy;
                                                            cost_result = CommonFunction.Math4Cut5Plus(cost);
                                                            cost_res = c_scale.ToString();
                                                        }
                                                        beforeCost += cost_result;
                                                        oc.Item_Cost = uint.Parse(cost_result.ToString());
                                                        cost_scale += cost_res + ",";
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        else if (prod.Price_type == 2)
                                        {
                                            #region 各自定價

                                            ipList = iPMgr.Query(new ItemPrice { item_id = uint.Parse(itemList[i]), price_master_id = pm.price_master_id });
                                            if (ipList.FirstOrDefault() != null)
                                            {
                                                oc.product_cost = ipList.FirstOrDefault().item_money;
                                            }
                                            #endregion
                                        }
                                    }


                                    if (index == 1)
                                    {
                                        minStock = oc.s_must_buy > 0 ? Convert.ToInt32(pItemList[0].Item_Stock / oc.s_must_buy) : pItemList[0].Item_Stock;
                                    }
                                    oc.Item_Stock = pItemList[0].Item_Stock;
                                    int tmp_stock = oc.s_must_buy > 0 ? Convert.ToInt32(oc.Item_Stock / oc.s_must_buy) : oc.Item_Stock;
                                    if (tmp_stock < minStock)
                                    {
                                        minStock = tmp_stock;
                                    }
                                    //根據spec_id找出spec_name
                                    IProductSpecImplMgr _specMgr = new BLL.gigade.Mgr.ProductSpecMgr(connectionString);
                                    ProductSpec ps1 = _specMgr.query(int.Parse(pItemList[0].Spec_Id_1.ToString()));
                                    if (ps1 != null)
                                    {
                                        oc.Spec_Name_1 = ps1.spec_name;
                                        oc.Spec_Id_1 = ps1.spec_id;
                                    }
                                    ProductSpec ps2 = _specMgr.query(int.Parse(pItemList[0].Spec_Id_2.ToString()));
                                    if (ps2 != null)
                                    {
                                        oc.Spec_Name_2 = ps2.spec_name;
                                        oc.Spec_Id_2 = ps2.spec_id;
                                    }
                                    jsonStr += "{";
                                    jsonStr += "'product_id':'" + oc.Product_Id + "','product_name':'" + oc.product_name + "','item_id':'" + oc.Item_Id + "'";
                                    jsonStr += ",'spec1':'" + oc.Spec_Id_1 + "','spec2':'" + oc.Spec_Id_2 + "','spec1_show':'" + oc.Spec_Name_1 + "',s_must_buy:'" + oc.s_must_buy + "'";
                                    jsonStr += ",'spec2_show':'" + oc.Spec_Name_2 + "','product_cost':'" + oc.product_cost + "',Item_Cost:'" + oc.Item_Cost + "','stock':'" + oc.Item_Stock + "',price_master_id:" + oc.price_master_id;
                                    jsonStr += ",price_type:" + oc.price_type + ",ignore_stock:" + oc.ignore_stock;
                                    if (oc.parent_id != 0)
                                    {
                                        jsonStr += ",parent_id:'" + oc.parent_id + "'";
                                    }
                                    else
                                    {
                                        jsonStr += ",parent_id:'" + oc.newparent_id + "'";
                                    }
                                    jsonStr += "},";
                                }
                            }
                        }
                        if (price_scale != "")
                        {
                            price_scale = price_scale.Substring(0, price_scale.Length - 1);
                        }
                        jsonStr = jsonStr.Substring(0, jsonStr.Length - 1) + "]";
                        jsonStr += ",data:[";
                        jsonStr += "{product_id:'" + orderAdd.out_product_id + "',item_id:0,product_name:'" + orderAdd.product_name + "',product_cost:'" + orderAdd.product_cost + "'";
                        jsonStr += ",s_must_buy:'" + pMapList[0].group_item_id.ToString().Split(',').Length + "',Item_Cost:'" + orderAdd.cost + "',child_cost_scale:'" + cost_scale + "',child_scale:'" + price_scale + "',stock:'" + minStock + "',g_must_buy:'" + pMapList[0].group_item_id.ToString().Split(',').Length + "'}]}";


                    }
                    else
                    {
                        if (prod.Combination == 0)
                        {
                            jsonStr = "{success:false,msg:\"" + Resources.OrderAdd.PRODUCT_NOT_USE_IN_ORDER + "\"}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr);
                            this.Response.End();
                            return this.Response;
                        }
                        else
                        {
                            //單一商品
                            ProductItem pItem = new ProductItem();
                            pItem.Item_Id = UInt32.Parse(pMapList[0].item_id.ToString());


                            List<ProductItem> pItemList = _proItemMgr.Query(pItem);
                            List<Product> pList = _proMgr.Query(new Product { Product_Id = uint.Parse(pItemList[0].Product_Id.ToString()) });
                            //補貨時還能販賣
                            if (pList[0].Shortage == 1)
                            {
                                this.Response.Clear();
                                this.Response.Write("{success:false,msg:'" + Resources.OrderAdd.SHPRT_AGE + "'}");
                                this.Response.End();
                                return this.Response;
                            }
                            if (pItemList.Count() == 0)
                            {
                                jsonStr = "{success:false}";
                                this.Response.Clear();
                                this.Response.Write(jsonStr);
                                this.Response.End();
                                return this.Response;
                            }


                            List<OrderAddCustom> oalist = new List<OrderAddCustom>();

                            OrderAddCustom oc = new OrderAddCustom();

                            oc.Spec_Id_1 = pItemList[0].Spec_Id_1;
                            oc.Spec_Id_2 = pItemList[0].Spec_Id_2;
                            oc.Item_Stock = pItemList[0].Item_Stock;
                            oc.ignore_stock = pList[0].Ignore_Stock;

                            IProductSpecImplMgr _specMgr = new BLL.gigade.Mgr.ProductSpecMgr(connectionString);
                            ProductSpec spec1 = _specMgr.query(Convert.ToInt32(pItemList[0].Spec_Id_1));
                            ProductSpec spec2 = _specMgr.query(Convert.ToInt32(pItemList[0].Spec_Id_2));
                            if (spec1 != null)
                            {
                                oc.Spec_Name_1 = spec1.spec_name;
                            }
                            if (spec2 != null)
                            {
                                oc.Spec_Name_2 = spec2.spec_name;
                            }

                            oalist.Add(oc);

                            StringBuilder stb = new StringBuilder();

                            stb.Append("[");
                            stb.Append("{product_id:'" + pMapList[0].channel_detail_id + "',item_id:" + pMapList[0].item_id + ",product_name:'" + HttpUtility.HtmlEncode(pMapList[0].product_name) + "',spec1:'" + oc.Spec_Id_1.ToString() + "',");
                            stb.Append("spec2:'" + oc.Spec_Id_2.ToString() + "',spec1_show:'" + oc.Spec_Name_1 + "',spec2_show:'" + oc.Spec_Name_2 + "',product_cost:" + pMapList[0].product_price + ",");
                            stb.Append("stock:" + oc.Item_Stock + ",price_master_id:" + oc.price_master_id + ",ignore_stock:" + oc.ignore_stock + "}");
                            stb.Append("]");

                            jsonStr = "{success:true,data:" + stb.ToString() + "}";
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
            else
            {
                jsonStr = "{success:true,data:{product_id:''}}";
            }

            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取規格一
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase getSpec1()
        {
            string s = Request.Params["pid"];

            string jsonStr = "{success:false}";

            IProductSpecImplMgr _specMgr = new BLL.gigade.Mgr.ProductSpecMgr(connectionString);
            List<ProductSpec> specList1 = _specMgr.query(Int32.Parse(s), "spec_id_1");



            jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(specList1) + "}";
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取規格二
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase getSpec2()
        {
            string s = Request.Params["pid"];

            string jsonStr = "{success:false}";

            IProductSpecImplMgr _specMgr = new BLL.gigade.Mgr.ProductSpecMgr(connectionString);
            List<ProductSpec> specList2 = _specMgr.query(Int32.Parse(s), "spec_id_2");


            jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(specList2) + "}";
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 獲取支付方式
        [HttpPost]
        [CustomHandleError]
        public string GetPayment()
        {
            ParameterMgr paraMgr = new ParameterMgr(connectionString);
            return paraMgr.Query("payment", 1);
        }
        #endregion

        #region 保存訂單
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase OrderSave()
        {
            int orderType = Request.UrlReferrer.AbsolutePath == "/Order/InteriorOrderAdd" ? 2 : 1;//訂單類型,:1為普通訂單輸入,2為內部訂單輸入
            string jsonStr = "{success:false,msg:'" + Resources.OrderAdd.ORDER_ADD_FAIL + "'}";
            try
            {
                Caller _caller = (Session["caller"] as Caller);

                uint normalFright = 0, lowFright = 0, receipt_to = 0, productTotalPrice = 0, combChannelId = 0, combPayMent = 0, combStoreMode = 0, combOrderStatus = 0, storeType = 0, retrieve_mode = 0;
                string combOrderDate = "", combLatestDeliverDate = "", txtAdminNote = "", txtCartNote = "", txtTradeNumber = "", txtOrderId = "", txtDeliverNumber = "";
                uint.TryParse(Request.Params["retrieve_mode"] ?? "0", out retrieve_mode);
                uint.TryParse(Request.Params["receipt_to"] ?? "0", out receipt_to);
                uint.TryParse(Request.Params["normalFright"] ?? "0", out normalFright);
                uint.TryParse(Request.Params["lowFright"] ?? "0", out lowFright);
                uint.TryParse(Request.Params["comboPrice"] ?? "0", out productTotalPrice);
                uint.TryParse(Request.Params["combChannelId"] ?? "0", out combChannelId);
                uint.TryParse(Request.Params["combPayMent"] ?? "0", out combPayMent);//付款方式
                uint.TryParse(Request.Params["combStoreMode"] ?? "0", out combStoreMode);
                uint.TryParse(Request.Params["combOrderStatus"] ?? "0", out combOrderStatus);//訂單狀態
                uint.TryParse(Request.Params["storeType"] ?? "0", out storeType);

                combOrderDate = Request.Params["combOrderDate"];
                //txtOrderDateHour = Request.Params["txtOrderDateHour"];
                //txtOrderDateMinute = Request.Params["txtOrderDateMinute"];
                combLatestDeliverDate = Request.Params["combLatestDeliverDate"];
                txtAdminNote = Request.Params["txtareAdminNote"];
                txtCartNote = Request.Params["txtareCartNote"];
                txtTradeNumber = Request.Params["txtTradeNumber"];
                txtOrderId = Request.Params["txtOrderId"];
                txtDeliverNumber = Request.Params["txtDeliverNumber"];
                //combOrderDate += " " + txtOrderDateHour + ":" + txtOrderDateMinute;

                if (!string.IsNullOrEmpty(txtAdminNote))
                {
                    txtAdminNote = Resources.OrderAdd.NOTE + "：" + txtAdminNote;
                }

                #region 訂購人信息
                string b_txtName = "", b_txtMobil = "", b_txtPhoneHead = "", b_txtPhoneContent = "", b_txtAddress = "";
                uint b_combZip = 0;
                b_txtName = Request.Params["b_txtName"];
                b_txtMobil = Request.Params["b_txtMobel"];
                b_txtPhoneHead = Request.Params["b_txtPhoneHead"];
                b_txtPhoneContent = Request.Params["b_txtPhoneContent"];
                b_txtAddress = Request.Params["b_txtAddress"];
                uint.TryParse(Request.Params["b_combZip"] ?? "0", out b_combZip);
                #endregion

                #region 收件人信息
                string r_txtCNFullName = "", r_txtActionPhone = "", r_txtContactPhoneHead = "", r_txtContactPhoneContent = "", r_txtContactAddress = "";
                uint r_combZip = 0;
                r_txtCNFullName = Request.Params["r_txtCNFullName"];
                r_txtActionPhone = Request.Params["r_txtActionPhone"];
                r_txtContactPhoneHead = Request.Params["r_txtContactPhoneHead"];
                r_txtContactPhoneContent = Request.Params["r_txtContactPhoneContent"];
                r_txtContactAddress = Request.Params["r_txtContactAddress"];
                uint.TryParse(Request.Params["r_combZip"] ?? "0", out r_combZip);
                #endregion

                string dataStr = Request.Params["gridData"];

                OrderAddCustom odc = new OrderAddCustom();
                JavaScriptSerializer jss = new JavaScriptSerializer();

                List<OrderAddCustom> errorOrder = new List<OrderAddCustom>();
                List<CooperatorOrderCustom> coopErrorList = new List<CooperatorOrderCustom>();
                //添加總和 抵用金 和 購物金  add by zhuoqin0830w
                uint productTotal = 0, deduct_bonusTotal = 0, deduct_welfareTotal = 0, acc_bonusTotal = 0;

                OrderImportMgr orderImportMgr = new OrderImportMgr(connectionString, 0);
                Resource.CoreMessage = new CoreResource("OrderImport");
                List<OrderSlave> slaves = new List<OrderSlave>();
                List<ChannelOrder> channelList = new List<ChannelOrder>();
                //add by zhuoqin0830w  2015/02/26  公關單與報廢單功能  獲取前臺傳來的單據類型
                string billtype = orderType == 1 ? "" : Request.Form["BillType"];
                string dep = orderType == 1 ? "" : Request.Form["dep"];
                //獲取前臺傳來 的 Cart_Delivery 和 Site_Id  add by zhuoqin0830w  2015/07/03
                uint Cart_Delivery = uint.Parse(Request.Params["Cart_Delivery"]);
                uint Site_Id = orderType == 1 ? 0 : uint.Parse(Request.Form["Site_Id"]);

                switch (storeType)
                {
                    case 1:
                        #region 合作外站新增訂單
                        List<CooperatorOrderCustom> odcList1 = jss.Deserialize<List<CooperatorOrderCustom>>(dataStr);
                        IPriceMasterImplMgr _priceMgr = new PriceMasterMgr(connectionString);
                        var parentList1 = from rec in odcList1 where rec.parent_id == "0" && rec.Item_Id == 0 select rec;//rec.price_type!=2:各自定價的價格是從表中讀取的，所以到後臺不需要重新計算價格
                        var singleList1 = from rec in odcList1 where rec.Item_Id != 0 && rec.parent_id == "0" select rec;

                        foreach (var item in parentList1)
                        {
                            productTotal += uint.Parse((item.product_cost * item.buynum).ToString());

                            productItemMapMgr = new ProductItemMapMgr(connectionString);
                            ProductItemMap pMap = productItemMapMgr.QueryAll(new ProductItemMap { channel_id = uint.Parse(combChannelId.ToString()), channel_detail_id = item.coop_product_id }).FirstOrDefault();

                            PriceMaster pMaster = _priceMgr.QueryPriceMaster(new PriceMaster
                            {
                                product_id = uint.Parse(pMap.product_id.ToString()),
                                //user_id = 0,
                                //user_level = 1,
                                //site_id = 1,
                                price_master_id = pMap.price_master_id,
                                child_id = int.Parse(pMap.product_id.ToString())
                            });
                            odcList1.Find(rec => rec.Product_Id == item.Product_Id && rec.group_id == item.group_id).price_master_id = pMaster.price_master_id;

                            ////查找該父商品下的子商品
                            var childList = from rec in odcList1 where rec.parent_id == item.coop_product_id select rec;
                            foreach (var child in childList)
                            {
                                IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
                                PriceMaster query = new PriceMaster { user_id = pMaster.user_id, user_level = pMaster.user_level, site_id = pMaster.site_id };
                                if (item.price_type == 1)
                                {
                                    query.product_id = uint.Parse(child.coop_product_id);
                                    query.child_id = 0;
                                }
                                else if (item.price_type == 2)
                                {
                                    query.product_id = uint.Parse(pMap.product_id.ToString());
                                    query.child_id = int.Parse(child.coop_product_id);
                                }
                                PriceMaster pM = _priceMgr.QueryPriceMaster(query);
                                List<ItemPriceCustom> ipList = new List<ItemPriceCustom>();
                                if (pM != null)
                                {
                                    odcList1.Find(rec => rec.coop_product_id == child.coop_product_id && rec.group_id == child.group_id).price_master_id = pM.price_master_id;
                                    ipList = iPMgr.Query(new ItemPrice
                                    {
                                        item_id = child.Item_Id,
                                        price_master_id = pM.price_master_id
                                    });
                                    if ((item.price_type == 1 && pM.same_price == 1) || (item.price_type == 2 && pMaster.same_price == 1))
                                    {
                                        odcList1.Find(rec => rec.coop_product_id == child.coop_product_id && rec.group_id == child.group_id).Item_Money = uint.Parse(pM.price.ToString());
                                        //odcList1.Find(rec => rec.coop_product_id == child.coop_product_id && rec.group_id == child.group_id).Item_Cost = uint.Parse(pM.cost.ToString());
                                        odcList1.Find(rec => rec.coop_product_id == child.coop_product_id && rec.group_id == child.group_id).Event_Item_Cost = uint.Parse(pM.event_cost.ToString());
                                    }
                                    else
                                    {
                                        odcList1.Find(rec => rec.coop_product_id == child.coop_product_id && rec.group_id == child.group_id).Item_Money = ipList[0].item_money;
                                        //odcList1.Find(rec => rec.coop_product_id == child.coop_product_id && rec.group_id == child.group_id).Item_Cost = ipList[0].item_cost;
                                        odcList1.Find(rec => rec.coop_product_id == child.coop_product_id && rec.group_id == child.group_id).Event_Item_Cost = ipList[0].event_cost;
                                    }
                                }

                            }
                        }
                        //單一商品
                        #region Channel_Order 資料
                        ChannelOrder chOrd;
                        foreach (CooperatorOrderCustom coop in odcList1)
                        {
                            ChannelOrder existChl = channelList.Where(m => m.Channel_Detail_Id == coop.coop_product_id).FirstOrDefault();
                            if (existChl != null)
                            {
                                continue;
                            }
                            chOrd = new ChannelOrder();
                            chOrd.Channel_Detail_Id = coop.coop_product_id;
                            chOrd.Channel_Id = int.Parse(combChannelId.ToString());
                            chOrd.Createtime = DateTime.Now;
                            if (!string.IsNullOrEmpty(combOrderDate))
                            {
                                chOrd.Ordertime = Convert.ToDateTime(combOrderDate);
                            }
                            chOrd.Order_Id = txtTradeNumber;
                            if (!string.IsNullOrEmpty(combLatestDeliverDate))
                            {
                                chOrd.Latest_Deliver_Date = Convert.ToDateTime(combLatestDeliverDate);
                            }
                            chOrd.Dispatch_Seq = txtDeliverNumber;
                            channelList.Add(chOrd);
                        }
                        #endregion

                        slaves = orderImportMgr.FillSlaveCooperator(odcList1, coopErrorList, combChannelId, combOrderStatus, txtOrderId);

                        foreach (var item in singleList1)
                        {
                            productTotal += uint.Parse((item.product_cost * item.buynum).ToString());
                        }
                        #endregion
                        break;
                    case 2:     //Gigade新增訂單
                        List<OrderAddCustom> odcList2 = jss.Deserialize<List<OrderAddCustom>>(dataStr);
                        //將前臺傳來的 Site_Id 的值傳入到後臺 并插入數據庫  add by zhuoqin0830w  2015/07/03
                        odcList2.ForEach(m =>
                        {
                            m.Site_Id = Site_Id;
                        });

                        if (orderType == 1)
                        {
                        #region 重新計算組合商品價格
                        IPriceMasterImplMgr priceMgr = new PriceMasterMgr(connectionString);

                        var parentList = from rec in odcList2 where rec.parent_id == 0 && rec.Item_Id == 0 && rec.price_type != 2 select rec;  //rec.price_type!=2:各自定價的價格是從表中讀取的，所以到後臺不需要重新計算價格

                        var singleList = from rec in odcList2 where rec.Item_Id != 0 && rec.parent_id == 0 select rec;

                        var priceSelfList = from rec in odcList2 where rec.Item_Id == 0 && rec.parent_id == 0 && rec.price_type == 2 select rec;

                        //單一商品計算價格
                        foreach (var item in singleList)
                        {
                            //使 總價  減去 抵用金 和 購物金  edit by zhuoqin0830w  2015/05/14
                            productTotal += uint.Parse((item.product_cost * item.buynum - item.deduct_bonus - item.deduct_welfare).ToString());
                            // 計算 購物金 和 抵用金  的 總和  add by zhuoqin0830w  2015/05/14
                            deduct_bonusTotal += uint.Parse(item.deduct_bonus.ToString());
                            deduct_welfareTotal += uint.Parse(item.deduct_welfare.ToString());
                            PriceMaster pM = priceMgr.QueryPriceMaster(new PriceMaster
                            {
                                product_id = uint.Parse(item.Product_Id.ToString()),
                                user_id = 0,
                                user_level = 1,
                                site_id = 1,
                                child_id = 0
                            });
                            if (pM != null)
                            {
                                odcList2.Find(rec => rec.Product_Id == item.Product_Id).price_master_id = pM.price_master_id;
                            }
                        }

                        //組合商品各自定價計算價格
                        foreach (var item in priceSelfList)
                        {
                            //使 總價  減去 抵用金 和 購物金  edit by zhuoqin0830w  2015/05/14
                            productTotal += uint.Parse((item.product_cost * item.buynum - item.deduct_bonus - item.deduct_welfare).ToString());

                            // 計算 購物金 和 抵用金  的 總和  add by zhuoqin0830w  2015/05/14
                            deduct_bonusTotal += uint.Parse(item.deduct_bonus.ToString());
                            deduct_welfareTotal += uint.Parse(item.deduct_welfare.ToString());

                            PriceMaster pM = priceMgr.QueryPriceMaster(new PriceMaster { product_id = item.Product_Id, user_id = 0, user_level = 1, site_id = 1, child_id = Convert.ToInt32(item.Product_Id) });
                            if (pM != null)
                            {
                                odcList2.Find(rec => rec.Product_Id == item.Product_Id && rec.group_id == item.group_id).price_master_id = pM.price_master_id;
                            }
                            //找出當前父商品的子商品
                            var childList = from rec in odcList2 where rec.parent_id == item.Product_Id && rec.group_id == item.group_id select rec;
                            foreach (var child in childList)
                            {
                                pM = priceMgr.QueryPriceMaster(new PriceMaster { product_id = item.Product_Id, user_id = 0, user_level = 1, site_id = 1, child_id = Convert.ToInt32(child.Product_Id) });
                                if (pM != null)
                                {
                                    IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);
                                    ItemPrice price = iPMgr.Query(new ItemPrice { item_id = child.Item_Id, price_master_id = pM.price_master_id }).FirstOrDefault();
                                    if (price != null)
                                    {
                                        odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Item_Cost = price.item_cost;       //成本
                                        odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Event_Item_Cost = price.event_cost;//活動成本
                                        odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Item_Money = price.item_money;//售價
                                    }
                                }
                            }
                        }

                        //組合商品按比例拆分計算價格
                        foreach (var item in parentList)
                        {
                            var TotalPrice = 0.0;
                            var TotalCost = 0.0;
                            //使 總價  減去 抵用金 和 購物金  edit by zhuoqin0830w  2015/05/14
                            productTotal += uint.Parse((item.product_cost * item.buynum - item.deduct_bonus - item.deduct_welfare).ToString());

                            // 計算 購物金 和 抵用金  的 總和  add by zhuoqin0830w  2015/05/14
                            deduct_bonusTotal += uint.Parse(item.deduct_bonus.ToString());
                            deduct_welfareTotal += uint.Parse(item.deduct_welfare.ToString());

                            PriceMaster pMaster = priceMgr.QueryPriceMaster(new PriceMaster
                            {
                                product_id = uint.Parse(item.Product_Id.ToString()),
                                user_id = 0,
                                user_level = 1,
                                site_id = 1,
                                child_id = int.Parse(item.Product_Id.ToString())
                            });
                            odcList2.Find(rec => rec.Product_Id == item.Product_Id && rec.group_id == item.group_id).price_master_id = pMaster.price_master_id;

                            //找出當前父商品的子商品
                            var childList = from rec in odcList2 where rec.parent_id == item.Product_Id && rec.group_id == item.group_id select rec;

                            var parentPrice = item.product_cost;
                            var parentCost = pMaster != null ? pMaster.cost : 0;

                            //子商品總價
                            foreach (var child in childList)
                            {
                                //價格
                                IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);

                                PriceMaster pM = priceMgr.QueryPriceMaster(new PriceMaster
                                {
                                    product_id = uint.Parse(child.Product_Id.ToString()),
                                    user_id = 0,
                                    user_level = 1,
                                    site_id = 1,
                                    child_id = 0
                                });
                                List<ItemPriceCustom> ipList = new List<ItemPriceCustom>();
                                if (pM != null)
                                {
                                    //必購數量
                                    _prodCombMgr = new ProductComboMgr(connectionString);
                                    List<ProductComboCustom> prodComList = _prodCombMgr.combQuery(new ProductComboCustom
                                    {
                                        Parent_Id = int.Parse(item.Product_Id.ToString()),
                                        Child_Id = child.Product_Id.ToString() //add by wangwei02016w 2014/9/24 
                                    });
                                    var s_must_buy = prodComList[0].S_Must_Buy == 0 ? child.s_must_buy : prodComList[0].S_Must_Buy;

                                    if (pM.same_price == 1)//同價
                                    {
                                        TotalPrice += pM.price * s_must_buy;
                                        TotalCost += pM.cost * s_must_buy;
                                    }
                                    else
                                    {
                                        ipList = iPMgr.Query(new ItemPrice
                                        {
                                            item_id = child.Item_Id,
                                            price_master_id = pM.price_master_id
                                        });

                                        if (ipList.Count != 0)
                                        {
                                            TotalPrice += ipList[0].item_money * s_must_buy;
                                            TotalCost += ipList[0].item_cost * s_must_buy;
                                        }
                                    }
                                }
                            }

                            //子商品按比例拆分后的價格
                            foreach (var child in childList)
                            {
                                _prodCombMgr = new ProductComboMgr(connectionString);
                                List<ProductComboCustom> prodComList = _prodCombMgr.combQuery(new ProductComboCustom
                                {
                                    Parent_Id = int.Parse(item.Product_Id.ToString()),
                                    Child_Id = child.Product_Id.ToString() //add by wangwei02016w 2014/9/24 
                                });
                                var s_must_buy = prodComList[0].S_Must_Buy == 0 ? child.s_must_buy : prodComList[0].S_Must_Buy;

                                IItemPriceImplMgr iPMgr = new ItemPriceMgr(connectionString);

                                PriceMaster pM = priceMgr.QueryPriceMaster(new PriceMaster
                                {
                                    product_id = uint.Parse(child.Product_Id.ToString()),
                                    user_id = 0,
                                    user_level = 1,
                                    site_id = 1,
                                    child_id = 0
                                });
                                List<ItemPriceCustom> ipList = new List<ItemPriceCustom>();
                                if (pM != null)
                                {
                                    odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).price_master_id = pM.price_master_id;
                                    if (pM.same_price == 1)
                                    {
                                        odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Item_Cost = uint.Parse(pM.cost.ToString());       //成本
                                        odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Event_Item_Cost = uint.Parse(pM.event_cost.ToString());//活動成本

                                        var afterPrice = 0;
                                        var afterCost = 0;
                                        if (TotalPrice <= 0)
                                        {
                                            afterPrice = 0;
                                        }
                                        else
                                        {
                                                afterPrice = Convert.ToInt16(Math.Round(parentPrice * (Convert.ToDouble(pM.price * s_must_buy) / TotalPrice / s_must_buy)));
                                        }
                                        if (TotalCost <= 0)
                                        {
                                            afterCost = 0;
                                        }
                                        else
                                        {
                                                afterCost = Convert.ToInt16(Math.Round(parentCost * (Convert.ToDouble(pM.cost * s_must_buy) / TotalCost / s_must_buy)));
                                        }

                                        //var singleTotal = afterPrice * s_must_buy;
                                        var singleTotal = afterPrice;
                                        parentPrice -= uint.Parse(singleTotal.ToString());
                                        TotalPrice -= Convert.ToDouble(pM.price) * s_must_buy;
                                        odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).product_cost = uint.Parse(afterPrice.ToString());

                                        //var singleCostTotal = afterCost * s_must_buy;
                                        var singleCostTotal = afterCost;
                                        parentCost -= singleCostTotal;
                                        TotalCost -= Convert.ToDouble(pM.cost) * s_must_buy;
                                        odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Item_Cost = uint.Parse(afterCost.ToString());
                                    }
                                    else
                                    {
                                        ipList = iPMgr.Query(new ItemPrice { item_id = child.Item_Id, price_master_id = pM.price_master_id });
                                        if (ipList.Count != 0)
                                        {
                                            odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Item_Cost = ipList[0].item_cost;       //成本
                                            odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Event_Item_Cost = ipList[0].event_cost;//活動成本

                                            //var afterPrice = CommonFunction.Math4Cut5Plus(parentPrice * ipList[0].item_money * s_must_buy / TotalPrice / s_must_buy);
                                            //beforePrice += afterPrice * s_must_buy;

                                            //new logic
                                            var afterPrice = 0;
                                            var afterCost = 0;
                                            if (TotalPrice <= 0)
                                            {
                                                afterPrice = 0;
                                            }
                                            else
                                            {
                                                    afterPrice = Convert.ToInt16(Math.Round(parentPrice * (Convert.ToDouble(ipList[0].item_money * s_must_buy) / TotalPrice / s_must_buy)));

                                            }

                                            if (TotalCost <= 0)
                                            {
                                                afterCost = 0;
                                            }
                                            else
                                            {
                                                    afterCost = Convert.ToInt16(Math.Round(parentCost * (Convert.ToDouble(ipList[0].item_cost * s_must_buy) / TotalCost / s_must_buy)));
                                            }

                                            //var singleTotal = afterPrice * s_must_buy;
                                            var singleTotal = afterPrice;
                                            parentPrice -= uint.Parse(singleTotal.ToString());
                                            TotalPrice -= Convert.ToDouble(ipList[0].item_money) * s_must_buy;
                                            // beforePrice += singleTotal;

                                            //var singleCostTotal = afterCost * s_must_buy;
                                            var singleCostTotal = afterCost;
                                            parentCost -= singleCostTotal;
                                            TotalCost -= Convert.ToDouble(ipList[0].item_cost) * s_must_buy;
                                            odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).product_cost = uint.Parse(afterPrice.ToString());
                                            odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).Item_Cost = uint.Parse(afterCost.ToString());
                                            odcList2.Find(rec => rec.Product_Id == child.Product_Id && rec.group_id == item.group_id).s_must_buy = s_must_buy;
                                        }
                                    }
                                }

                            }
                        }
                        #endregion
                        }
                        else
                        {
                            foreach (var item in odcList2.FindAll(m => (m.parent_id == 0 && m.Item_Id == 0) || (m.Item_Id != 0 && m.parent_id == 0)))
                            {
                                //使 總價  減去 抵用金 和 購物金  edit by zhuoqin0830w  2015/05/14
                                productTotal += uint.Parse((item.product_cost * item.buynum - item.deduct_bonus - item.deduct_welfare).ToString());
                                // 計算 購物金 和 抵用金  的 總和  add by zhuoqin0830w  2015/05/14
                                deduct_bonusTotal += uint.Parse(item.deduct_bonus.ToString());
                                deduct_welfareTotal += uint.Parse(item.deduct_welfare.ToString());

                                acc_bonusTotal += uint.Parse(item.accumulated_bonus.ToString());
                            }
                        }

                        slaves = orderImportMgr.FillSlave(odcList2, errorOrder, combOrderStatus, txtOrderId);
                        break;
                    default:
                        break;
                }

                if (slaves != null && errorOrder.Count() == 0 && coopErrorList.Count() == 0)
                {
                    channelMgr = new ChannelMgr(connectionString);

                    #region OrderMaster 信息
                    #region receipt_to 發票開立方式
                    switch (receipt_to)
                    {
                        case 1: receipt_to = 2; break;
                        case 2: receipt_to = 0; break;
                        case 3: receipt_to = 2; break;
                    }
                    #endregion

                    #region retrieve_mode 取貨方式
                    channelShipMgr = new ChannelShippingMgr(connectionString);
                    ChannelShipping chip = new ChannelShipping();
                    chip.channel_id = int.Parse(combChannelId.ToString());
                    chip.shipping_carrior = int.Parse(combStoreMode.ToString());
                    retrieve_mode = uint.Parse(channelShipMgr.Query(chip)[0].retrieve_mode.ToString());
                    #endregion

                    OrderMaster ordermaster = new OrderMaster();
                    //ordermaster.Order_Id = Convert.ToUInt32(orderImportMgr.NextOrderId());
                    ordermaster.Channel_Order_Id = txtTradeNumber;
                    ordermaster.Invoice_Status = receipt_to;
                    ordermaster.Order_Freight_Normal = normalFright;
                    ordermaster.Order_Freight_Low = lowFright;
                    ordermaster.Delivery_Store = combStoreMode;
                    ordermaster.Channel = combChannelId;
                    ordermaster.Order_Status = combOrderStatus;
                    //orderType=2為内部订单输入 edit by xiangwang0413w 2014/10/29
                    ordermaster.User_Id = (orderType == 1) ? (uint)channelMgr.GetUserIdByChannelId((int)combChannelId) : uint.Parse(Request.Params["userId"]);//订单人的user_id
                    ordermaster.Order_Product_Subtotal = productTotal;
                    ordermaster.Order_Amount = productTotal + normalFright + lowFright;
                    ordermaster.Order_Payment = combPayMent;
                    //向 order_master 裱中 添加 從前臺獲取的 Cart_Delivery  edit by zhuoqin0830w  2015/07/03
                    ordermaster.Cart_Id = Cart_Delivery;
                    ordermaster.Note_Admin = string.Format(Resources.OrderAdd.NOTE_CONTENT, _caller.user_username, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), txtAdminNote);
                    ordermaster.Note_Order = txtCartNote;
                    ordermaster.Order_Date_Pay = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime());
                    ordermaster.Order_Createdate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime(combOrderDate));
                    ordermaster.Import_Time = DateTime.Now;
                    ordermaster.Retrieve_Mode = retrieve_mode;
                    //向 order_master 裱中 添加 抵用金 和 購物金  add by zhuoqin0830w  2015/05/14
                    ordermaster.Deduct_Welfare = deduct_welfareTotal;
                    ordermaster.Deduct_Bonus = deduct_bonusTotal;
                    //向 order_master 表中 添加 返還購物金  add by zhuoqin0830w  2015/09/01
                    ordermaster.Accumulated_Bonus = (int)acc_bonusTotal;
                    #endregion

                    #region 出貨時間
                    if (orderType == 2)//內部訂單輸入
                    {
                        ordermaster.Holiday_Deliver = Convert.ToInt32(Request.Form["HolidayDeliver"]);
                        ordermaster.Estimated_Arrival_Period = Convert.ToInt32(Request.Form["EstimatedArrivalPeriod"]);
                    }
                    #endregion

                    #region 訂購人信息
                    ordermaster.Order_Name = b_txtName;
                    ordermaster.Order_Mobile = b_txtMobil;
                    b_txtPhoneHead = !string.IsNullOrEmpty(b_txtPhoneHead) ? b_txtPhoneHead + "-" : b_txtPhoneHead;
                    ordermaster.Order_Phone = b_txtPhoneHead + b_txtPhoneContent;
                    ordermaster.Order_Zip = b_combZip;
                    ordermaster.Order_Address = b_txtAddress;
                    ordermaster.Order_Gender = Request.Params["ServiceSex"] == "1" ? true : false; //add by wwei0216w 2015/1/21 訂購人性別
                    #endregion

                    #region 收件人信息
                    ordermaster.Delivery_Name = r_txtCNFullName;
                    ordermaster.Delivery_Mobile = r_txtActionPhone;
                    ordermaster.Delivery_Phone = r_txtContactPhoneHead + "-" + r_txtContactPhoneContent;
                    ordermaster.Delivery_Zip = r_combZip;
                    ordermaster.Delivery_Address = r_txtContactAddress;
                    ordermaster.Delivery_Gender = Request.Params["AddresseeSex"] == "1" ? true : false; //add by wwei0216w 收件人性別
                    #endregion

                    #region add by zhuoqin0830w  2015/02/26  公關單與報廢單功能   判斷單據類型是否為空值 如果不為空則執行添加功能
                    OrderMasterPattern op = null;
                    if (!string.IsNullOrEmpty(billtype))
                    {
                        op = new OrderMasterPattern { Pattern = int.Parse(billtype), Dep = int.Parse(dep) };
                    }
                    #endregion

                    #region add by zhuoqin0830w 2015/08/24  在內部訂單輸入時 同時將 輸入的 購物金 和 返還購物金 以及 抵用券 添加至 bonus_master 和 bonus_record 表中
                    BonusMaster bm = null; BonusRecord brBonus = null; BonusRecord brWelfare = null; 
                    //判斷返還購物金 是否 大於0 如果大於 0 表示需要在 bonus_master 新增一筆數據 
                    if (acc_bonusTotal > 0)
                    {
                        bm = new BonusMaster
                        {
                            user_id = ordermaster.User_Id,
                            type_id = 30,// 默認為 商品回饋購物金
                            master_writer = "商品回饋購物金",
                            master_total = acc_bonusTotal,
                            master_balance = (int)acc_bonusTotal,
                            master_start = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime()),
                            master_end = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime(DateTime.Now.AddDays(90).ToString())),//默認過期時間是當前時間+90天
                            master_createdate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime()),
                            master_updatedate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime()),
                            bonus_type = 1//默認為  購物金
                        };
                    }
                    //判斷購物金是否大於 0 如果是 則表示 需要在 bonus_record 增加一筆數據 
                    //並且需要 在 bonus_master 中 按照 過期日期 減去 相應的 參數
                    if (deduct_bonusTotal > 0)
                    {
                        brBonus = new BonusRecord
                        {
                            user_id = ordermaster.User_Id,
                            record_use = deduct_bonusTotal,
                            record_createdate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime()),
                            record_updatedate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime())
                        };
                        //判斷購物金是否小於或等於數據庫中的購物金
                        _bonusMasterMgr = new BonusMasterMgr(connectionString);
                        int userBonusTotal = _bonusMasterMgr.GetSumBouns(brBonus);
                        if (deduct_bonusTotal > userBonusTotal)
                        {
                            jsonStr = "{success:false,msg:'" + Resources.OrderAdd.BONUS_PRICE_NULL + "'}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr);
                            this.Response.End();
                            return this.Response;
                        }
                    }
                    //判斷 抵用卷 金額 是否 大於 0 
                    if (deduct_welfareTotal > 0)
                    {
                        brWelfare = new BonusRecord
                        {
                            user_id = ordermaster.User_Id,
                            record_use = deduct_welfareTotal,
                            record_createdate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime()),
                            record_updatedate = Convert.ToUInt32(BLL.gigade.Common.CommonFunction.GetPHPTime())
                        };
                        //判斷購物金是否小於或等於數據庫中的購物金
                        _bonusMasterMgr = new BonusMasterMgr(connectionString);
                        int userWelfateTotal = _bonusMasterMgr.GetSumWelfare(brWelfare);
                        if (deduct_welfareTotal > userWelfateTotal)
                        {
                            jsonStr = "{success:false,msg:'" + Resources.OrderAdd.WELFARE_PRICE_NULL + "'}";
                            this.Response.Clear();
                            this.Response.Write(jsonStr);
                            this.Response.End();
                            return this.Response;
                        }
                    }
                    #endregion

                    #region 保存至數據庫
                    bool result = orderImportMgr.Save2DB(ordermaster, slaves, channelList, null, op, bm, brBonus, brWelfare);
                    #endregion

                    if (result)
                    {
                        jsonStr = "{success:true,msg:'" + Resources.OrderAdd.ORDER_ADD_SUCCESS + "'}";
                    }
                }
                else
                {
                    StringBuilder stb = new StringBuilder();
                    if (storeType == 1)
                    {
                        foreach (CooperatorOrderCustom item in coopErrorList)
                        {
                            stb.Append(item.coop_product_id + "\\n");
                        }
                        stb.Append("\\n" + Resources.OrderAdd.EMPTY_NULL_FAIL);
                    }
                    else if (storeType == 2)
                    {
                        foreach (OrderAddCustom item in errorOrder)
                        {
                            stb.Append(item.Product_Id + item.Spec_Name_1 + item.Spec_Name_2 + "\\n");
                        }
                        stb.Append("\\n" + Resources.OrderAdd.EMPTY_NULL_FAIL);
                    }
                    jsonStr = "{success:false,msg:'" + stb.ToString() + "'}";
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
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 匯入訂單
        [CustomHandleError]
        public ActionResult OrderImport()
        {
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            if (!System.IO.Directory.Exists(Server.MapPath(pdfPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(pdfPath));
            }

            siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            SiteConfig excellConfig = siteConfigMgr.GetConfigByName("Excel_Extension_Filter");
            SiteConfig pdfConfig = siteConfigMgr.GetConfigByName("PDF_Extension_Filter");
            ViewBag.ExcelEx = string.IsNullOrEmpty(excellConfig.Value) ? excellConfig.DefaultValue : excellConfig.Value;
            ViewBag.PDFEx = string.IsNullOrEmpty(pdfConfig.Value) ? pdfConfig.DefaultValue : pdfConfig.Value;
            return View();
        }
        #endregion

        #region 得到外站

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase GetChannel()
        {
            string json = string.Empty;
            try
            {
                channelMgr = new ChannelMgr(connectionString);
                json = JsonConvert.SerializeObject(channelMgr.QueryList(1));

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

        #region 得到物流模式
        [HttpPost]
        [CustomHandleError]
        public string GetStore()
        {
            string storeType = Request.Form["storeType"] ?? "0";
            string channelId = Request.Form["channel_id"] ?? "0";
            string resultStr = "";
            ParameterMgr paraMgr = new ParameterMgr(connectionString);
            ChannelShippingMgr shipMgr = new ChannelShippingMgr(connectionString);

            //switch (storeType)
            //{
            //    case "1": resultStr = shipMgr.QueryCarry(channelId); break;
            //    case "2": resultStr = paraMgr.Query("Deliver_Store", 1); break;
            //    default:
            //        break;
            //}

            resultStr = shipMgr.QueryCarry(channelId);

            return resultStr;
        }
        #endregion

        #region 得到超商店家
        [HttpPost]
        [CustomHandleError]
        public JsonResult GetSuperStore()
        {
            IDeliveryStoreImplMgr _deliveryMgr = new DeliveryStoreMgr(connectionString);
            JsonResult result = null;
            int storeId = 0;
            int.TryParse(Request.Params["storeId"] ?? "0", out storeId);

            DeliveryStore store = new DeliveryStore();
            store.delivery_store_id = storeId;
            store.IsPage = false;

            int count = 0;

            try
            {
                if (storeId != 0)
                {
                    result = Json(_deliveryMgr.Query(store, out count));
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }
        #endregion

        #region 保存導入訂單的文件

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase SaveImportFile()
        {
            Session["import"] = null;
            string json = "{success:false}";
            siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            try
            {
                if (Request.Files["channelFile"] != null && Request.Files["channelFile"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["channelFile"];

                    FileManagement fileManagement = new FileManagement();
                    string newExcelName = fileManagement.NewFileName(excelFile.FileName);
                    string oldExcelName = excelFile.FileName.Split('\\').LastOrDefault();

                    #region Excel文件限制

                    SiteConfig excellConfig = siteConfigMgr.GetConfigByName("Excel_Extension_Filter");
                    string excelEx = string.IsNullOrEmpty(excellConfig.Value) ? excellConfig.DefaultValue : excellConfig.Value;

                    SiteConfig minConfig = siteConfigMgr.GetConfigByName("Excel_Length_Min");
                    SiteConfig maxConfig = siteConfigMgr.GetConfigByName("Excel_Length_Max");
                    int excelMin = Convert.ToInt32(DataCheck.IsNumeric(minConfig.Value) ? minConfig.Value : minConfig.DefaultValue);
                    int excelMax = Convert.ToInt32(DataCheck.IsNumeric(maxConfig.Value) ? maxConfig.Value : maxConfig.DefaultValue);
                    #endregion
                    Resource.CoreMessage = new CoreResource("Product");
                    if (fileManagement.UpLoadFile(excelFile, Server.MapPath(excelPath) + "/" + newExcelName, excelEx, excelMax, excelMin))//保存導入訂單excel文件
                    {
                        if (!string.IsNullOrEmpty(Request.Form["importType"]) && Request.Form["importType"] == "2")
                        {
                            if (Request.Files["dispatch"] != null && Request.Files["dispatch"].ContentLength > 0)
                            {
                                HttpPostedFileBase dispatchFile = Request.Files["dispatch"];

                                string newPDFName = fileManagement.NewFileName(dispatchFile.FileName);

                                #region PDF文件限制

                                SiteConfig pdfConfig = siteConfigMgr.GetConfigByName("PDF_Extension_Filter");
                                string pdfEx = string.IsNullOrEmpty(pdfConfig.Value) ? pdfConfig.DefaultValue : pdfConfig.Value;

                                minConfig = siteConfigMgr.GetConfigByName("PDF_Length_Min");
                                maxConfig = siteConfigMgr.GetConfigByName("PDF_Length_Max");
                                int pdfMin = Convert.ToInt32(DataCheck.IsNumeric(minConfig.Value) ? minConfig.Value : minConfig.DefaultValue);
                                int pdfMax = Convert.ToInt32(DataCheck.IsNumeric(maxConfig.Value) ? maxConfig.Value : maxConfig.DefaultValue);
                                #endregion

                                if (fileManagement.UpLoadFile(dispatchFile, Server.MapPath(pdfPath) + "/" + newPDFName, pdfEx, pdfMax, pdfMin))//保存選擇 ‘超商取貨’ 時需上傳的取貨單文件
                                {
                                    json = "{success:true,msg:\"" + newExcelName + "|" + oldExcelName + "|" + newPDFName + "\"}";
                                }
                            }
                        }
                        else
                        {
                            json = "{success:true,msg:\"" + newExcelName + "|" + oldExcelName + "\"}";
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
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 讀取導入文件數據

        [CustomHandleError]
        [HttpPost]
        public JsonResult ReadExcelFile()
        {
            List<OrdersImport> orders = new List<OrdersImport>();
            Session["import"] = null;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Files"]) && !string.IsNullOrEmpty(Request.Form["ImportType"]) && !string.IsNullOrEmpty(Request.Form["Channel"]))
                {
                    string improtType = Request.Form["ImportType"];
                    int channel = Convert.ToInt32(Request.Form["Channel"]);
                    string excelFile = Server.MapPath(excelPath) + Request.Form["Files"].Split('|')[0];

                    Resource.CoreMessage = new CoreResource("OrderImport");
                    orderImport = OrderImportFactory.InitOrderImport(channel);

                    siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
                    SiteConfig config = siteConfigMgr.GetConfigByName("Template_OrderImport_Path");
                    if (orderImport != null && config != null)
                    {
                        string template = Server.MapPath(string.IsNullOrEmpty(config.Value) ? config.DefaultValue : config.Value);
                        if (System.IO.File.Exists(template))
                        {
                            orders = orderImport.ReadExcel2Page(excelFile, template, orderImport.CurChannel.model_in);
                            Session["import"] = orders;
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
            }
            return Json(orders);
        }
        #endregion

        #region 導入選中數據

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase Import()
        {
            List<OrdersImport> all = (List<OrdersImport>)Session["import"] ?? null;
            int successCount = 0;
            int totalCount = 0;
            string json = string.Empty;
            try
            {
                if (all != null)
                {
                    if (!string.IsNullOrEmpty(Request.Form["Orders"]) && !string.IsNullOrEmpty(Request.Form["Files"]) && !string.IsNullOrEmpty(Request.Form["ImportType"]) && !string.IsNullOrEmpty(Request.Form["Channel"]))
                    {
                        string orders = Request.Form["Orders"];
                        int channel = Convert.ToInt32(Request.Form["Channel"]);
                        string improtType = Request.Form["ImportType"];
                        int site_id = int.Parse(Request.Form["site"]);//add xw
                        string files = Request.Form["Files"];

                        Resource.CoreMessage = new CoreResource("OrderImport");
                        orderImport = OrderImportFactory.InitOrderImport(channel);
                        if (orderImport != null)
                        {
                            string[] names = files.Split('|');
                            string execlFile = files.LastIndexOf("|") != -1 ? Server.MapPath(excelPath) + names[0] : Server.MapPath(excelPath) + files;
                            string pdfFile = files.LastIndexOf("|") != -1 && names.Length == 3 ? names[2] : "";

                            //修改 Excel 表裏面的地址  add by zhuoqin0830w  2015/04/17
                            for (int i = 0; i < all.Count; i++)
                            {
                                zMgr = new ZipMgr(connectionString);
                                if (!string.IsNullOrEmpty(all[i].agpesadrzip))
                                {
                                    string small = zMgr.QueryCityAndZip(all[i].agpesadrzip).small;
                                    int index = all[i].agpesadr.IndexOf(small);
                                    if (index != -1)
                                    {
                                        string newAddress = all[i].agpesadr.Substring(index + small.Length);
                                        all[i].agpesadr = newAddress;
                                    }
                                }
                            }

                            successCount = orderImport.Import2DB(all, pdfFile, improtType, orders, site_id, ref totalCount);
                            if (successCount > 0)
                            {
                                ImportOrdersLog log = new ImportOrdersLog { Channel_Id = channel, TCount = totalCount, Success_Count = successCount };
                                log.File_Name = names[1];
                                log.Import_Date = DateTime.Now;
                                log.Exec_Name = (Session["caller"] as Caller).user_username;

                                importOrdersLogMgr = new ImportOrdersLogMgr(connectionString);
                                importOrdersLogMgr.Save(log);
                            }
                        }
                    }
                    all.FindAll(m => m.IsSel).ForEach(m => m.IsSel = false);
                    Session["import"] = all;
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            json = "{Total:" + totalCount + ",SucccessCount:" + successCount + ",Orders:" + JsonConvert.SerializeObject(all) + "}";
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 訂單匯入 範本下載

        public HttpResponseBase OrderTemplate()
        {
            siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            SiteConfig config = siteConfigMgr.GetConfigByName("Template_OrderImport_Path");
            if (config != null)
            {
                string template = Server.MapPath(string.IsNullOrEmpty(config.Value) ? config.DefaultValue : config.Value);
                if (System.IO.File.Exists(template))
                {
                    this.Response.Clear();
                    this.Response.ContentType = "application/ms-excel";
                    this.Response.AppendHeader("Content-Disposition", "attachment;filename=t_orders.xlsx");
                    this.Response.WriteFile(template);
                    this.Response.End();
                }
            }
            return this.Response;
        }

        #endregion

        #region 得到text框提示信息
        //add by wangwei0216w 2014/10/27
        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase GetInfoByTest(string condition)
        {
            string json = string.Empty;
            try
            {
                UsersMgr um;
                if (!string.IsNullOrEmpty(condition))
                {
                    um = new UsersMgr(connectionString);
                    json = "{items:" + JsonConvert.SerializeObject(um.GetUserInfoByTest(condition)) + "}";
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

        #region 匯入會計入帳時間+HttpResponseBase OrderMasterImport()
        /// <summary>
        /// 匯入會計入帳時間
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase OrderMasterImport()
        {
            string json = string.Empty;//json字符串
            //string shipment = Request.Params["shipment"].ToString();
            //int total = 0;
            try
            {
                if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)//判斷文件是否為空
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];//獲取文件流
                    FileManagement fileManagement = new FileManagement();//實例化 FileManagement
                    string fileLastName = excelFile.FileName;
                    string newExcelName = Server.MapPath(excelPath) + "會計賬款實收時間" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名
                    excelFile.SaveAs(newExcelName);//上傳文件
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    dt = helper.SheetData();
                    List<OrderAccountCollection> oacli = new List<OrderAccountCollection>();
                    string errorStr = string.Empty;
                    Int64[] orderArr = new Int64[dt.Rows.Count];
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        OrderAccountCollection model = new OrderAccountCollection();
                        uint order_id = 0;
                        if (!string.IsNullOrEmpty(dt.Rows[j][0].ToString()))
                        {
                            if (uint.TryParse(dt.Rows[j][0].ToString(), out  order_id))
                            {
                                model.order_id = order_id;
                            }
                            else
                            {
                                errorStr += (j + 2) + ",";
                                continue;
                            }
                        }
                        else
                        {
                            errorStr += (j + 2) + ",";
                            continue;
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[j][1].ToString()))
                        {
                            DateTime st;
                            if (DateTime.TryParse(dt.Rows[j][1].ToString(), out st))
                            {
                                model.account_collection_time = st;
                            }
                            else
                            {
                                string strtime = Regex.Replace(dt.Rows[j][1].ToString().Trim(), "/(\\s+)|(，)|(-)|(,)|(.)/g", "/");
                                if (DateTime.TryParse(strtime, out st))
                                {
                                    model.account_collection_time = st;
                                }
                                else
                                {
                                    string[] str = dt.Rows[j][1].ToString().Split('/');
                                    int year = 0;
                                    if (str.Length == 3)
                                    {
                                        if (str[2].Length == 2)
                                        {
                                            year = Convert.ToInt32("20" + str[2]);
                                        }
                                        else
                                        {
                                            year = Convert.ToInt32(str[2]);
                                        }
                                        int month = Convert.ToInt32(str[0]);
                                        int day = Convert.ToInt32(str[1]);
                                        if (DateTime.TryParse(year + "/" + month + "/" + day, out st))
                                        {
                                            model.account_collection_time = st;
                                        }
                                        else
                                        {
                                            errorStr += (j + 2) + ",";
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        errorStr += (j + 2) + ",";
                                        continue;
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(dt.Rows[j][2].ToString()))
                            {
                                int account_collection_money = 0;
                                if (int.TryParse(dt.Rows[j][2].ToString(), out account_collection_money))
                                {
                                    model.account_collection_money = account_collection_money;
                                }
                                else
                                {
                                    errorStr += (j + 2) + ",";
                                    continue;
                                }
                            }
                            else
                            {
                                errorStr += (j + 2) + ",";
                                continue;
                            }
                            if (!string.IsNullOrEmpty(dt.Rows[j][3].ToString()))
                            {
                                int poundage = 0;
                                if (int.TryParse(dt.Rows[j][3].ToString(), out poundage))
                                {
                                    model.poundage = poundage;
                                }
                                else
                                {
                                    errorStr += (j + 2) + ",";
                                    continue;
                                }
                            }
                            else
                            {
                                errorStr += (j + 2) + ",";
                                continue;
                            }
                        }

                        if (!string.IsNullOrEmpty(dt.Rows[j][4].ToString()))
                        {
                            DateTime streturn;
                            if (DateTime.TryParse(dt.Rows[j][4].ToString(), out streturn))
                            {
                                model.return_collection_time = streturn;
                            }
                            else
                            {
                                string strtimeR = Regex.Replace(dt.Rows[j][4].ToString().Trim(), "/(\\s+)|(，)|(-)|(,)|(.)/g", "/");
                                if (DateTime.TryParse(strtimeR.ToString(), out streturn))
                                {
                                    model.return_collection_time = streturn;
                                }
                                else
                                {
                                    string[] strR = dt.Rows[j][4].ToString().Split('/');
                                    int yearR = 0;
                                    if (strR.Length == 3)
                                    {
                                        if (strR[2].Length == 2)
                                        {
                                            yearR = Convert.ToInt32("20" + strR[2]);
                                        }
                                        else
                                        {
                                            yearR = Convert.ToInt32(strR[2]);
                                        }
                                        int monthR = Convert.ToInt32(strR[0]);
                                        int dayR = Convert.ToInt32(strR[1]);
                                        if (DateTime.TryParse(yearR + "/" + monthR + "/" + dayR, out streturn))
                                        {
                                            model.return_collection_time = streturn;
                                        }
                                        else
                                        {
                                            errorStr += (j + 2) + ",";
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        errorStr += (j + 2) + ",";
                                        continue;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(dt.Rows[j][5].ToString()))
                            {
                                int return_collection_money = 0;
                                if (int.TryParse(dt.Rows[j][5].ToString(), out return_collection_money))
                                {
                                    model.return_collection_money = return_collection_money;
                                }
                                else
                                {
                                    errorStr += (j + 2) + ",";
                                    continue;
                                }
                            }
                            else
                            {
                                errorStr += (j + 2) + ",";
                                continue;
                            }
                            if (!string.IsNullOrEmpty(dt.Rows[j][6].ToString()))
                            {
                                int return_poundage = 0;
                                if (int.TryParse(dt.Rows[j][6].ToString(), out return_poundage))
                                {
                                    model.return_poundage = return_poundage;
                                }
                                else
                                {
                                    errorStr += (j + 2) + ",";
                                    continue;
                                }
                            }
                            else
                            {
                                errorStr += (j + 2) + ",";
                                continue;
                            }
                        }
                        if (!string.IsNullOrEmpty(dt.Rows[j][7].ToString()))
                        {
                            DateTime st;
                            if (DateTime.TryParse(dt.Rows[j][7].ToString(), out st))
                            {
                                model.invoice_date_manual = st;
                            }
                            else
                            {
                                string strtime = Regex.Replace(dt.Rows[j][1].ToString().Trim(), "/(\\s+)|(，)|(-)|(,)|(.)/g", "/");
                                if (DateTime.TryParse(strtime, out st))
                                {
                                    model.invoice_date_manual = st;
                                }
                                else
                                {
                                    string[] str = dt.Rows[j][7].ToString().Split('/');
                                    int year = 0;
                                    if (str.Length == 3)
                                    {
                                        if (str[2].Length == 2)
                                        {
                                            year = Convert.ToInt32("20" + str[2]);
                                        }
                                        else
                                        {
                                            year = Convert.ToInt32(str[2]);
                                        }
                                        int month = Convert.ToInt32(str[0]);
                                        int day = Convert.ToInt32(str[1]);
                                        if (DateTime.TryParse(year + "/" + month + "/" + day, out st))
                                        {
                                            model.invoice_date_manual = st;
                                        }
                                        else
                                        {
                                            errorStr += (j + 2) + ",";
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        errorStr += (j + 2) + ",";
                                        continue;
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(dt.Rows[j][8].ToString()))
                            {
                                int invoice_sale_manual = 0;
                                if (int.TryParse(dt.Rows[j][8].ToString(), out invoice_sale_manual))
                                {
                                    model.invoice_sale_manual = invoice_sale_manual;
                                }
                                else
                                {
                                    errorStr += (j + 2) + ",";
                                    continue;
                                }
                            }
                            else
                            {
                                errorStr += (j + 2) + ",";
                                continue;
                            }
                            if (!string.IsNullOrEmpty(dt.Rows[j][9].ToString()))
                            {
                                int invoice_tax_manual = 0;
                                if (int.TryParse(dt.Rows[j][9].ToString(), out invoice_tax_manual))
                                {
                                    model.invoice_tax_manual = invoice_tax_manual;
                                }
                                else
                                {
                                    errorStr += (j + 2) + ",";
                                    continue;
                                }
                            }
                            else
                            {
                                errorStr += (j + 2) + ",";
                                continue;
                            }
                        }
                        model.remark = dt.Rows[j][10].ToString();
                        if (model != null && !(model.account_collection_time == model.return_collection_time && model.account_collection_time == model.invoice_date_manual && model.return_collection_time == DateTime.MinValue))
                        {
                            if (!orderArr.Contains(model.order_id))
                            {
                                orderArr[j] = order_id;
                                oacli.Add(model);
                            }
                            else
                            {
                                errorStr += (j + 2) + ",";
                            }
                        }
                    }
                    int rowsnum = oacli.Count;
                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        errorStr = errorStr.Remove(errorStr.Length - 1);
                    }
                    if (rowsnum > 0)//判斷是否是這個表
                    {
                        _OrderMasterMgr = new OrderMasterMgr(connectionString);

                        int i = _OrderMasterMgr.OrderMasterImport(oacli);
                        if (i > 0)
                        {
                            if (i == 99999)
                            {
                                if (!string.IsNullOrEmpty(errorStr))
                                {
                                    json = "{success:true,msg:\"" + "無數據可匯入!另文件第" + errorStr + "行數據異常\"}";
                                }
                                else
                                {
                                    json = "{success:true,msg:\"" + "無數據可匯入!\"}";
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(errorStr))
                                {
                                    json = "{success:true,msg:\"" + "匯入成功!另文件第" + errorStr + "行數據異常\"}";
                                }
                                else
                                {
                                    json = "{success:true,msg:\"" + "匯入成功!\"}";
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(errorStr))
                            {
                                json = "{success:true,msg:\"" + "操作失敗!另文件第" + errorStr + "行數據異常\"}";
                            }
                            else
                            {
                                json = "{success:true,msg:\"" + "操作失敗!\"}";
                            }
                        }
                    }
                    else
                    {
                        json = "{success:true,total:0,msg:\"" + "此表內沒有數據或數據有誤,請檢查后再次匯入!" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                }
                else//當直接點擊時會產生,
                {
                    json = "{success:true,msg:\"" + "操作失敗！" + "\"}";
                    this.Response.Clear();
                    this.Response.Write(json);
                    this.Response.End();
                    return this.Response;
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:\"" + "操作失敗！" + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 匯入模板
        /// </summary>
        public void OrderMasterImportMuBan()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            OrderMasterQuery query = new OrderMasterQuery();
            try
            {
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("付款單號", typeof(String));
                dtHZ.Columns.Add("銀行入帳日期", typeof(String));
                dtHZ.Columns.Add("入帳金額", typeof(String));
                dtHZ.Columns.Add("手續費", typeof(String));
                dtHZ.Columns.Add("退貨入帳日期", typeof(String));
                dtHZ.Columns.Add("退貨入帳金額", typeof(String));
                dtHZ.Columns.Add("退貨入帳手續費", typeof(String));
                dtHZ.Columns.Add("手開發票日期", typeof(String));
                dtHZ.Columns.Add("手開發票銷售額", typeof(String));
                dtHZ.Columns.Add("手開發票稅額", typeof(String));
                dtHZ.Columns.Add("備註", typeof(String));
                DataRow dr = dtHZ.NewRow();
                dr[0] = "";
                dr[1] = "";
                dr[2] = "";
                dr[3] = "";
                dr[4] = "";
                dr[5] = "";
                dr[6] = "";
                dr[7] = "";
                dr[8] = "";
                dr[9] = "";
                dr[10] = "";
                dtHZ.Rows.Add(dr);
                string fileName = DateTime.Now.ToString("會計入帳_yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());


            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }
        }
        #endregion

        #region 異常訂單匯出列表頁+HttpResponseBase ArrorOrderList()
        public HttpResponseBase ArrorOrderList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            OrderMasterQuery query = new OrderMasterQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.first_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["timestart"]));
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.last_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["timeend"]));
                }

                int totalCount = 0;
                _OrderMasterMgr = new OrderMasterMgr(connectionString);
                _dt = _OrderMasterMgr.ArrorOrderList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 匯出異常訂單Excel信息+void ExportArrorOrderExcel()
        public void ExportArrorOrderExcel()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            OrderMasterQuery query = new OrderMasterQuery();
            try
            {
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("訂單編號", typeof(String));
                dtHZ.Columns.Add("訂單狀態", typeof(String));
                //  dtHZ.Columns.Add("備註", typeof(String));
                dtHZ.Columns.Add("細項編號", typeof(String));
                dtHZ.Columns.Add("父商品編號", typeof(String));
                dtHZ.Columns.Add("組合包編號", typeof(String));
                dtHZ.Columns.Add("組合方式", typeof(String));
                dtHZ.Columns.Add("細項類型", typeof(String));
                dtHZ.Columns.Add("數量", typeof(String));
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.first_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["timestart"]));
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.last_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["timeend"]));
                }
                _OrderMasterMgr = new OrderMasterMgr(connectionString);
                query.IsPage = false;
                int totalCount = 0;
                _dt = _OrderMasterMgr.ArrorOrderList(query, out totalCount);
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr["訂單編號"] = _dt.Rows[i]["order_id"];
                    dr["訂單狀態"] = _dt.Rows[i]["remark"];
                    dr["細項編號"] = _dt.Rows[i]["detail_id"];
                    dr["父商品編號"] = _dt.Rows[i]["parent_id"];
                    dr["組合包編號"] = _dt.Rows[i]["pack_id"];
                    dr["組合方式"] = _dt.Rows[i]["modeName"];
                    switch (_dt.Rows[i]["item_mode"].ToString())
                    {
                        case "0":
                            dr["細項類型"] = "一般商品";
                            break;

                        case "1":
                            dr["細項類型"] = "父商品";
                            break;

                        case "2":
                            dr["細項類型"] = "子商品";
                            break;

                    }

                    dr["數量"] = _dt.Rows[i]["cout"];

                    dtHZ.Rows.Add(dr);
                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = DateTime.Now.ToString("異常訂單表_yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }
        }
        #endregion

        #region 會計入帳匯出列表頁+HttpResponseBase OrderMasterExportList()
        public HttpResponseBase OrderMasterExportList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            OrderMasterQuery query = new OrderMasterQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            try
            {
                query.dateType = Convert.ToInt32(Request.Params["dateType"]);
                if (Request.Params["dateType"].ToString() == "1" || Request.Params["dateType"].ToString() == "4")
                {
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeStart"]))
                    {
                        query.order_date_pay_startTime = Convert.ToDateTime(Request.Params["orderTimeStart"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeEnd"]))
                    {
                        query.order_date_pay_endTime = Convert.ToDateTime(Request.Params["orderTimeEnd"]);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeStart"]))
                    {
                        query.first_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["orderTimeStart"]).ToString("yyyy-MM-dd 00:00:00")));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeEnd"]))
                    {
                        query.last_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["orderTimeEnd"]).ToString("yyyy-MM-dd 23:59:59")));
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    query.Order_Id = Convert.ToUInt32(Request.Params["order_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["show_type"]))
                {
                    query.show_type = Convert.ToInt32(Request.Params["show_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["invoice_type"]))
                {
                    query.invoice_type = Convert.ToInt32(Request.Params["invoice_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["payment"]))
                {
                    query.Order_Payment = Convert.ToUInt32(Request.Params["payment"]);
                }
                int totalCount = 0;
                _OrderMasterMgr = new OrderMasterMgr(connectionString);
                _dt = _OrderMasterMgr.OrderMasterExportList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy/MM/dd";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        /// <summary>
        /// 會計手續費、入帳金額等匯總
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase OrderMasterHuiZong()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            OrderMasterQuery query = new OrderMasterQuery();
            try
            {
                query.dateType = Convert.ToInt32(Request.Params["dateType"]);
                if (Request.Params["dateType"].ToString() == "1" || Request.Params["dateType"].ToString() == "4")
                {
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeStart"]))
                    {
                        query.order_date_pay_startTime = Convert.ToDateTime(Request.Params["orderTimeStart"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeEnd"]))
                    {
                        query.order_date_pay_endTime = Convert.ToDateTime(Request.Params["orderTimeEnd"]);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeStart"]))
                    {
                        query.first_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["orderTimeStart"]).ToString("yyyy-MM-dd 00:00:00")));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeEnd"]))
                    {
                        query.last_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["orderTimeEnd"]).ToString("yyyy-MM-dd 23:59:59")));
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    query.Order_Id = Convert.ToUInt32(Request.Params["order_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["show_type"]))
                {
                    query.show_type = Convert.ToInt32(Request.Params["show_type"]);
                } if (!string.IsNullOrEmpty(Request.Params["invoice_type"]))
                {
                    query.invoice_type = Convert.ToInt32(Request.Params["invoice_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["payment"]))
                {
                    query.Order_Payment = Convert.ToUInt32(Request.Params["payment"]);
                }
                _OrderMasterMgr = new OrderMasterMgr(connectionString);
                _dt = _OrderMasterMgr.OrderMasterHuiZong(query);
                int ZPoundage = 0;
                int AccountMoney = 0;
                int FreeTax = 0;
                int SalesAmount = 0;
                if (!string.IsNullOrEmpty(_dt.Rows[0]["ZPoundage"].ToString()))
                {
                    ZPoundage = Convert.ToInt32(_dt.Rows[0]["ZPoundage"].ToString());
                }
                if (!string.IsNullOrEmpty(_dt.Rows[0]["AccountCollectionMoney"].ToString()))
                {
                    AccountMoney = Convert.ToInt32(_dt.Rows[0]["AccountCollectionMoney"].ToString());
                }
                if (!string.IsNullOrEmpty(_dt.Rows[0]["FreeTax"].ToString()))
                {
                    FreeTax = Convert.ToInt32(_dt.Rows[0]["FreeTax"].ToString());
                }
                if (!string.IsNullOrEmpty(_dt.Rows[0]["SalesAmount"].ToString()))
                {
                    SalesAmount = Convert.ToInt32(_dt.Rows[0]["SalesAmount"].ToString());
                }
                json = "{success:true,'msg':'" + "手續費：" + GetString(ZPoundage.ToString()) + "','AccountMoney':'" + "入帳金額:" + GetString(AccountMoney.ToString()) + "','ZMoney':'" + "入帳總額：" + GetString((ZPoundage + AccountMoney).ToString()) + "','FreeTax':'" + "發票稅額：" + GetString(SalesAmount.ToString()) + "','SalesAmount':'" + "發票銷售額：" + GetString(FreeTax.ToString()) + "','ZTax':'" + "發票總額：" + GetString((FreeTax + SalesAmount).ToString()).ToString() + "'}";//返回json數據
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
        public string GetString(string name)
        {
            string results = Convert.ToDouble(name).ToString("N");
            if (results.IndexOf('.') > 0)
            {
                return results.Substring(0, results.LastIndexOf('.'));
            }
            else
            {
                return results;
            }
        }

        #endregion

        public HttpResponseBase OrderMasterChangeStatusFromPayToDel()
        {
            string json = json = "{success:true,data:''}"; ;
            OrderModifyModel orderModifyModel = new OrderModifyModel();

            
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    orderModifyModel.order_id = Convert.ToInt32(Request.Params["order_id"]);
                    
                }
                if (!string.IsNullOrEmpty(Request.Params["deduct_card_bonus"]))
                {
                    orderModifyModel.deduct_card_bonus = Convert.ToInt32(Request.Params["deduct_card_bonus"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["isBilling_checked"]))
                {
                    orderModifyModel.isBilling_checked = Convert.ToBoolean(Request.Params["isBilling_checked"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["isHGBonus"]))
                {
                    orderModifyModel.isHGBonus = Convert.ToBoolean(Request.Params["isHGBonus"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["isCash_record_bonus"]))
                {
                    orderModifyModel.isCash_record_bonus = Convert.ToBoolean(Request.Params["isCash_record_bonus"]);
                }
                OrderMasterMgr orderMasterMgr = new OrderMasterMgr(connectionString);
                orderModifyModel.ip_from = CommonFunction.GetIP4Address(Request.UserHostAddress);
                //如果不可以轉單，則返回不可以
                if (!orderMasterMgr.ModifyOrderMsaterForDeliver(orderModifyModel))
                {
                    json = "{success:false,data:'此訂單不可轉出貨單'}";
                }
                else
                {
                    try
                    {
                        orderMasterMgr.ModifyOrderMsaterForDeliver(orderModifyModel);
                        json = "{success:true,data:'恭喜！轉單成功'}";
                    }
                    catch (Exception ex)
                    {

                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                        json = "{success:false,data:'轉單失敗！'}";
                    }
                    
                }
            }
            catch (Exception ex)
            {
                
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:'系統故障！'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }

        #region 會計入帳時間匯出Excel信息+void OrderMasterExport()
        public void OrderMasterExport()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            OrderMasterQuery query = new OrderMasterQuery();
            try
            {
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("付款單號", typeof(String));
                dtHZ.Columns.Add("訂購人", typeof(String));
                //dtHZ.Columns.Add("收貨人", typeof(String));
                dtHZ.Columns.Add("訂單應收金額", typeof(int)); //A
                dtHZ.Columns.Add("紅利折抵金額", typeof(String));
                dtHZ.Columns.Add("付款方式", typeof(String));
                dtHZ.Columns.Add("請款狀態", typeof(String));//C!=NULL  已經款 C==null 未請款 A=I商品取消
                dtHZ.Columns.Add("付款單成立日期", typeof(String));
                dtHZ.Columns.Add("銀行入帳日期", typeof(String));
                dtHZ.Columns.Add("手續費", typeof(String));//B
                dtHZ.Columns.Add("入帳金額", typeof(String));//C
                dtHZ.Columns.Add("退貨入帳日期", typeof(String));
                dtHZ.Columns.Add("退貨入帳手續費", typeof(String));//N
                dtHZ.Columns.Add("退貨入帳金額", typeof(String));//M
                dtHZ.Columns.Add("入帳總額", typeof(String));//D=B+C+M+N
                dtHZ.Columns.Add("入帳金額差異", typeof(String));//E=A-D
                dtHZ.Columns.Add("開立發票日期", typeof(String));
                dtHZ.Columns.Add("發票銷售額", typeof(String));
                dtHZ.Columns.Add("發票稅額", typeof(String));
                dtHZ.Columns.Add("手開發票日期", typeof(String));
                dtHZ.Columns.Add("手開發票銷售額", typeof(String));
                dtHZ.Columns.Add("手開發票稅額", typeof(String));
                dtHZ.Columns.Add("發票總額", typeof(String));
                dtHZ.Columns.Add("商品取消金額", typeof(String));
                dtHZ.Columns.Add("發票金額差異", typeof(String));
                dtHZ.Columns.Add("備註", typeof(String));
                query.dateType = Convert.ToInt32(Request.Params["dateType"]);
                if (Request.Params["dateType"].ToString() == "1" || Request.Params["dateType"].ToString() == "4")
                {
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeStart"]))
                    {
                        query.order_date_pay_startTime = Convert.ToDateTime(Request.Params["orderTimeStart"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeEnd"]))
                    {
                        query.order_date_pay_endTime = Convert.ToDateTime(Request.Params["orderTimeEnd"]);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeStart"]))
                    {
                        query.first_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["orderTimeStart"]).ToString("yyyy-MM-dd 00:00:00")));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["orderTimeEnd"]))
                    {
                        query.last_time = Convert.ToUInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["orderTimeEnd"]).ToString("yyyy-MM-dd 23:59:59")));
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    query.Order_Id = Convert.ToUInt32(Request.Params["order_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["show_type"]))
                {
                    query.show_type = Convert.ToInt32(Request.Params["show_type"]);
                } if (!string.IsNullOrEmpty(Request.Params["invoice_type"]))
                {
                    query.invoice_type = Convert.ToInt32(Request.Params["invoice_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["payment"]))
                {
                    query.Order_Payment = Convert.ToUInt32(Request.Params["payment"]);
                }
                _OrderMasterMgr = new OrderMasterMgr(connectionString);
                _dt = _OrderMasterMgr.OrderMasterExport(query);
                if (_dt.Rows.Count > 0)
                {
                    if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                    }
                    for (int i = 0; i < _dt.Rows.Count; i++)
                    {
                        DataRow dr = dtHZ.NewRow();
                        dr["付款單號"] = _dt.Rows[i]["order_id"];
                        dr["訂購人"] = _dt.Rows[i]["order_name"];
                        dr["訂單應收金額"] = Convert.ToInt32(_dt.Rows[i]["order_amount"]);//A
                        dr["紅利折抵金額"] = _dt.Rows[i]["deduct_card_bonus"];
                        dr["付款方式"] = _dt.Rows[i]["parameterName"];
                        if (!string.IsNullOrEmpty(_dt.Rows[i]["account_collection_money"].ToString()))
                        {
                            dr["請款狀態"] = "已請款";
                        }
                        else
                        {
                            dr["請款狀態"] = "未請款";
                        }

                        dr["付款單成立日期"] = _dt.Rows[i]["ordercreatedate"];
                        if (!string.IsNullOrEmpty(_dt.Rows[i]["account_collection_time"].ToString()))
                        {
                            dr["銀行入帳日期"] = Convert.ToDateTime(_dt.Rows[i]["account_collection_time"]).ToString("yyyy/MM/dd");
                        }
                        dr["手續費"] = _dt.Rows[i]["poundage"];//B
                        dr["入帳金額"] = _dt.Rows[i]["account_collection_money"]; //C
                        if (!string.IsNullOrEmpty(_dt.Rows[i]["return_collection_time"].ToString()))
                        {
                            dr["退貨入帳日期"] = Convert.ToDateTime(_dt.Rows[i]["return_collection_time"]).ToString("yyyy/MM/dd");
                        }
                        dr["退貨入帳手續費"] = _dt.Rows[i]["return_poundage"];
                        dr["退貨入帳金額"] = _dt.Rows[i]["return_collection_money"];
                        dr["入帳總額"] = _dt.Rows[i]["oacamount"];
                        if (!string.IsNullOrEmpty(dr["訂單應收金額"].ToString()) && !string.IsNullOrEmpty(dr["入帳總額"].ToString()))
                        {
                            dr["入帳金額差異"] = Convert.ToInt32(dr["訂單應收金額"].ToString()) - Convert.ToInt32(dr["入帳總額"].ToString());//E=A-D
                        }
                        if (!string.IsNullOrEmpty(_dt.Rows[i]["invoicedate"].ToString()))
                        {
                            dr["開立發票日期"] = _dt.Rows[i]["invoicedate"].ToString();
                        }
                        dr["發票銷售額"] = _dt.Rows[i]["free_tax"];//F
                        dr["發票稅額"] = _dt.Rows[i]["tax_amount"];//G
                        if (!string.IsNullOrEmpty(_dt.Rows[i]["invoice_date_manual"].ToString()))
                        {
                            dr["手開發票日期"] = Convert.ToDateTime(_dt.Rows[i]["invoice_date_manual"]).ToString("yyyy/MM/dd");
                        }
                        dr["手開發票銷售額"] = _dt.Rows[i]["invoice_sale_manual"];//F
                        dr["手開發票稅額"] = _dt.Rows[i]["invoice_tax_manual"];//G
                        dr["發票總額"] = _dt.Rows[i]["imramount"];//F+G
                        //if (!string.IsNullOrEmpty(dr["發票銷售額"].ToString()) && !string.IsNullOrEmpty(dr["發票稅額"].ToString()))
                        //{
                        //    dr["發票總額"] = Convert.ToInt32(dr["發票銷售額"]) + Convert.ToInt32(dr["發票稅額"]);//F+G
                        //}
                        //dr["發票總額"] = Convert.ToInt32(_dt.Rows[i]["sales_amount"]) + Convert.ToInt32(_dt.Rows[i]["free_tax"]);
                        if (Convert.ToInt32(_dt.Rows[i]["money_cancel"]) != 0 && Convert.ToInt32(_dt.Rows[i]["money_return"]) == 0)
                        {
                            dr["商品取消金額"] = _dt.Rows[i]["money_cancel"];//I
                        }
                        if (Convert.ToInt32(_dt.Rows[i]["money_cancel"]) == 0 && Convert.ToInt32(_dt.Rows[i]["money_return"]) != 0)
                        {
                            dr["商品取消金額"] = _dt.Rows[i]["money_return"];
                        }
                        if (dr["訂單應收金額"].ToString() == dr["商品取消金額"].ToString())
                        {
                            dr["請款狀態"] = "商品取消";
                        }
                        dr["發票金額差異"] = _dt.Rows[i]["invoice_diff"];//入帳總額-發票總額
                        //if (!string.IsNullOrEmpty(dr["入帳總額"].ToString()) && !string.IsNullOrEmpty(dr["發票總額"].ToString()))
                        //{
                        //    dr["發票金額差異"] = Convert.ToInt32(dr["入帳總額"].ToString()) - Convert.ToInt32(dr["發票總額"].ToString());//J=E-H
                        //}

                        dr["備註"] = _dt.Rows[i]["remark"]; ;
                        dtHZ.Rows.Add(dr);
                    }
                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = DateTime.Now.ToString("會計入帳時間匯出_yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Write("匯出數據不存在");
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,totalCount:0,data:[]}";
            }
        }
        #endregion

        #region 類別營業額 
        #region 類別選擇store
        public HttpResponseBase GetProductCategoryStore()
        {
            string json = string.Empty;
            try
            {
                _productCategoryMgr = new ProductCategoryMgr(connectionString);
                DataTable store = _productCategoryMgr.GetProductCategoryStore();
                if (store != null)
                {
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd";
                    json = "{success:true" + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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
        #region 列表頁store
        public HttpResponseBase GetCategorySummaryList()
        {
            string json = string.Empty;
            int sumAmount = 0;
            int totalCount = 0;
            try
            {
                _orderDetialMgr = new OrderDetailMgr(connectionString);
                OrderDetailQuery query = new OrderDetailQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["chooseCategory"]))
                {
                    query.category_id = Convert.ToUInt32(Request.Params["chooseCategory"]);                
                }
                if (!string.IsNullOrEmpty(Request.Params["receiptStatus"]))
                {
                    query.category_status = Convert.ToInt32(Request.Params["receiptStatus"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["dateCon"]))
                {
                    query.date_stauts = Convert.ToInt32(Request.Params["dateCon"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_start"]))
                {
                    query.date_start = Convert.ToDateTime(Request.Params["date_start"]);
                    query.date_start = Convert.ToDateTime(query.date_start.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["date_end"]))
                {
                    query.date_end = Convert.ToDateTime(Request.Params["date_end"]);
                    query.date_end = Convert.ToDateTime(query.date_end.ToString("yyyy-MM-dd 23:59:59"));
                }
                List<OrderDetailQuery> store = _orderDetialMgr.GetCategorySummaryList(query,out totalCount, out sumAmount);
                if (store != null && store.Count > 0)
                {
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd";
                    json = "{success:true,totalCount:" + totalCount + ",sumAmount:" + sumAmount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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
        #region 類別訂單明細
        public HttpResponseBase GetAmountDetial()
        {
            string json = string.Empty;
            try
            {
                _orderDetialMgr = new OrderDetailMgr(connectionString);
                OrderDetailQuery query = new OrderDetailQuery();
                int totalCount = 0;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["category_id"]))
                {
                    query.category_id = Convert.ToUInt32(Request.Params["category_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["category_status"]))
                {
                    query.category_status = Convert.ToInt32(Request.Params["category_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_stauts"]))
                {
                    query.date_stauts = Convert.ToInt32(Request.Params["date_stauts"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_start"]))
                {
                    query.date_start = Convert.ToDateTime(Request.Params["date_start"]);
                    query.date_start = Convert.ToDateTime(query.date_start.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["date_end"]))
                {
                    query.date_end = Convert.ToDateTime(Request.Params["date_end"]);
                    query.date_end = Convert.ToDateTime(query.date_end.ToString("yyyy-MM-dd 23:59:59"));
                }
                DataTable store = _orderDetialMgr.GetAmountDetial(query, out totalCount);
                if (store != null && store.Rows.Count > 0)
                {
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd";
                    json = "{success:true,totalCount:" + totalCount +  ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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
        #region 訂單明細匯出
        public HttpResponseBase OrderDetialExport()
        {
            string json = string.Empty;
            try
            {
                _OrderMasterMgr = new OrderMasterMgr (connectionString);
                OrderDetailQuery query = new OrderDetailQuery();                
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                DataTable store = new DataTable();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["category_id"]))
                {
                    query.category_id = Convert.ToUInt32(Request.Params["category_id"]);
                }           
                if (!string.IsNullOrEmpty(Request.Params["category_status"]))
                {
                    query.category_status = Convert.ToInt32(Request.Params["category_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_stauts"]))
                {
                    query.date_stauts = Convert.ToInt32(Request.Params["date_stauts"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_start"]))
                {
                    query.date_start = Convert.ToDateTime(Request.Params["date_start"]);
                    query.date_start = Convert.ToDateTime(query.date_start.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["date_end"]))
                {
                    query.date_end = Convert.ToDateTime(Request.Params["date_end"]);
                    query.date_end = Convert.ToDateTime(query.date_end.ToString("yyyy-MM-dd 23:59:59"));
                }
                query.IsPage = false;
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("會員姓名", typeof(String));
                dtHZ.Columns.Add("購買時間", typeof(String));
                dtHZ.Columns.Add("付款單號", typeof(int));
                dtHZ.Columns.Add("付款方式", typeof(String));
                dtHZ.Columns.Add("購買金額", typeof(int));
                dtHZ.Columns.Add("付款狀態", typeof(String));
                dtHZ.Columns.Add("發票號碼", typeof(String));
                dtHZ.Columns.Add("發票金額", typeof(int));
                dtHZ.Columns.Add("發票開立日期", typeof(String));
                dtHZ.Columns.Add("商品細項編號", typeof(int));
                dtHZ.Columns.Add("訂單狀態", typeof(String));
                dtHZ.Columns.Add("供應商", typeof(String));
                dtHZ.Columns.Add("供應商編碼", typeof(String));
                dtHZ.Columns.Add("品名", typeof(String));
                dtHZ.Columns.Add("數量", typeof(int));
                dtHZ.Columns.Add("購買單價", typeof(int));
                dtHZ.Columns.Add("折抵購物金", typeof(int));
                dtHZ.Columns.Add("抵用券", typeof(int));
                dtHZ.Columns.Add("總價", typeof(int));
                dtHZ.Columns.Add("成本單價", typeof(int));
                dtHZ.Columns.Add("寄倉費", typeof(int));
                dtHZ.Columns.Add("成本總額", typeof(int));
                dtHZ.Columns.Add("出貨單歸檔期", typeof(String));
                dtHZ.Columns.Add("負責PM", typeof(String));
                dtHZ.Columns.Add("來源ID", typeof(String));
                dtHZ.Columns.Add("來源名稱", typeof(String));
                dtHZ.Columns.Add("出貨方式", typeof(String));

                store = _OrderMasterMgr.OrderDetialExportInfo(query);
                foreach (DataRow dr_v in store.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = dr_v["user_name"].ToString();
                    if (!string.IsNullOrEmpty(dr_v["order_createdate"].ToString()))
                    {
                        DateTime order_createdate = Convert.ToDateTime(dr_v["order_createdate"].ToString());
                        dr[1] = CommonFunction.DateTimeToString(order_createdate);
                    }
                    if (!string.IsNullOrEmpty(dr_v["order_id"].ToString()))
                    {
                        dr[2] = Convert.ToInt32(dr_v["order_id"].ToString());
                    }
                    else {
                        dr[2] = 0;
                    }
                    dr[3] = dr_v["order_payment"].ToString();
                    if (!string.IsNullOrEmpty(dr_v["order_amount"].ToString()))
                    {
                        dr[4] = Convert.ToInt32(dr_v["order_amount"].ToString());
                    }
                    else
                    {
                        dr[4] = 0;
                    }
                    dr[5] = dr_v["order_status"].ToString();
                    dr[6] = dr_v["invoice_number"].ToString() == "" ? "" : dr_v["invoice_number"].ToString();                    
                    if (!string.IsNullOrEmpty(dr_v["total_amount"].ToString()))
                    {
                        dr[7] =Convert.ToInt32( dr_v["total_amount"].ToString());
                    }
                    else
                    {
                        dr[7] = 0;
                    }                   
                    if (!string.IsNullOrEmpty(dr_v["invoice_date"].ToString()))
                    {
                        DateTime invoice_date = Convert.ToDateTime(dr_v["invoice_date"].ToString());
                        dr[8] = CommonFunction.DateTimeToString(invoice_date);
                    }
                    else
                    {
                        dr[8] = "";
                    }
                    if (!string.IsNullOrEmpty(dr_v["item_id"].ToString()))
                    {
                        dr[9] =Convert.ToInt32( dr_v["item_id"].ToString());
                    }
                    else
                    {
                        dr[9] = 0;
                    }
                    dr[10] = dr_v["slave_status"].ToString();
                    dr[11] = dr_v["vendor_name_simple"].ToString();
                    dr[12] = dr_v["vendor_code"].ToString();
                    dr[13] = dr_v["product_name"].ToString();
                    if (!string.IsNullOrEmpty(dr_v["buy_num"].ToString()))
                    {
                        dr[14] =Convert.ToInt32( dr_v["buy_num"].ToString());
                    }
                    else
                    {
                        dr[14] = 0;
                    }

                    if (!string.IsNullOrEmpty(dr_v["single_money"].ToString()))
                    {
                        dr[15] = Convert.ToInt32(dr_v["single_money"].ToString());
                    }
                    else
                    {
                        dr[15] = 0;
                    }
                    if (!string.IsNullOrEmpty(dr_v["deduct_bonus"].ToString()))
                    {
                        dr[16] =Convert.ToInt32( dr_v["deduct_bonus"].ToString());
                    }
                    else
                    {
                        dr[16] = 0;
                    }
                    if (!string.IsNullOrEmpty(dr_v["deduct_welfare"].ToString()))
                    {
                        dr[17] =Convert.ToInt32( dr_v["deduct_welfare"].ToString());
                    }
                    else
                    {
                        dr[17] = 0;
                    }
                    if (!string.IsNullOrEmpty(dr_v["od.single_money*buy_num"].ToString()))
                    {
                        dr[18] = Convert.ToInt32(dr_v["od.single_money*buy_num"].ToString()) - Convert.ToInt32(dr[16]) - Convert.ToInt32(dr[17]);
                    }
                    else
                    {
                        dr[18] = 0;
                    }
                    if (!string.IsNullOrEmpty(dr_v["single_cost"].ToString()))
                    {
                        dr[19] =Convert.ToInt32( dr_v["single_cost"].ToString());
                    }
                    else
                    {
                        dr[19] = 0;
                    }
                    if (!string.IsNullOrEmpty(dr_v["bag_check_money"].ToString()))
                    {
                        dr[20] =Convert.ToInt32( dr_v["bag_check_money"].ToString());
                    }
                    else
                    {
                        dr[20] = 0;
                    }
                    if (!string.IsNullOrEmpty(dr_v["od.single_cost*od.buy_num"].ToString()))
                    {
                        dr[21] = Convert.ToInt32(dr_v["od.single_cost*od.buy_num"].ToString());
                    }
                    else
                    {
                        dr[21] = 0;
                    }
                    if (!string.IsNullOrEmpty(dr_v["slave_date_close"].ToString()))
                    {
                        DateTime slave_date_close = Convert.ToDateTime(dr_v["slave_date_close"].ToString());
                        dr[22] = slave_date_close == Convert.ToDateTime("1/1/1970 8:00:00 AM") ? "未歸檔" : CommonFunction.DateTimeToString(slave_date_close);
                    }
                    dr[23] = dr_v["pm"].ToString();
                    dr[24] = dr_v["ID"].ToString() == "0" ? "" : dr_v["ID"].ToString();                 
                    dr[25] = dr_v["group_name"].ToString();
                    dr[26] = dr_v["product_mode"].ToString();
                    dtHZ.Rows.Add(dr);
                }
                string[] colname = new string[dtHZ.Columns.Count];
                string filename = "訂單明細"+ DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                newExcelName = Server.MapPath(excelPath_export) + filename;
                for (int i = 0; i < dtHZ.Columns.Count; i++)
                {
                    colname[i] = dtHZ.Columns[i].ColumnName;
                }

                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }
                ExcelHelperXhf.ExportDTtoExcel(dtHZ, "", newExcelName);
                json = "{success:true,ExcelName:\'" + filename + "\'}";
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

        #region 類別訂單明細匯出
        public HttpResponseBase CategoryDetialExport()
        {
            string json = string.Empty;
            try
            {
                _orderDetialMgr = new  OrderDetailMgr(connectionString);
                OrderDetailQuery query = new OrderDetailQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                DataTable store = new DataTable();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["category_id"]))
                {
                    query.category_id = Convert.ToUInt32(Request.Params["category_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["category_name"]))
                {
                    query.category_name = Request.Params["category_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["category_status"]))
                {
                    query.category_status = Convert.ToInt32(Request.Params["category_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_stauts"]))
                {
                    query.date_stauts = Convert.ToInt32(Request.Params["date_stauts"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_start"]))
                {
                    query.date_start = Convert.ToDateTime(Request.Params["date_start"]);
                    query.date_start = Convert.ToDateTime(query.date_start.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["date_end"]))
                {
                    query.date_end = Convert.ToDateTime(Request.Params["date_end"]);
                    query.date_end = Convert.ToDateTime(query.date_end.ToString("yyyy-MM-dd 23:59:59"));
                }
                query.IsPage = false;
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("購買金額", typeof(int));
                dtHZ.Columns.Add("付款狀態", typeof(String));
                dtHZ.Columns.Add("商品細項編號", typeof(int));
                dtHZ.Columns.Add("訂單狀態", typeof(String));
                dtHZ.Columns.Add("供應商", typeof(String));
                dtHZ.Columns.Add("供應商編碼", typeof(String));
                dtHZ.Columns.Add("品名", typeof(String));
                dtHZ.Columns.Add("數量", typeof(int));
                dtHZ.Columns.Add("購買單價", typeof(int));
                dtHZ.Columns.Add("總價", typeof(int));
                store = _orderDetialMgr.CategoryDetialExportInfo(query);
                foreach (DataRow dr_v in store.Rows)
                {
                    DataRow dr = dtHZ.NewRow();
                    if (!string.IsNullOrEmpty(dr_v["order_amount"].ToString()))
                    {
                        dr[0] = Convert.ToInt32(dr_v["order_amount"].ToString());
                    }
                    else
                    {
                        dr[0] = 0;
                    }
                    dr[1] = dr_v["order_status"].ToString();
                    if (!string.IsNullOrEmpty(dr_v["item_id"].ToString()))
                    {
                        dr[2] = Convert.ToInt32(dr_v["item_id"].ToString());
                    }
                    else
                    {
                        dr[2] = 0;
                    }
                    dr[3] = dr_v["slave_status"].ToString();
                    dr[4] = dr_v["vendor_name_simple"].ToString();
                    dr[5] = dr_v["vendor_code"].ToString();
                    dr[6] = dr_v["product_name"].ToString();
                    if (!string.IsNullOrEmpty(dr_v["buy_num"].ToString()))
                    {
                        dr[7] = Convert.ToInt32(dr_v["buy_num"].ToString());
                    }
                    else
                    {
                        dr[7] = 0;
                    }

                    if (!string.IsNullOrEmpty(dr_v["single_money"].ToString()))
                    {
                        dr[8] = Convert.ToInt32(dr_v["single_money"].ToString());
                    }
                    else
                    {
                        dr[8] = 0;
                    }
                    if (!string.IsNullOrEmpty(dr_v["amount"].ToString()))
                    {
                        dr[9] = Convert.ToInt32(dr_v["amount"].ToString());
                    }
                    else
                    {
                        dr[9] = 0;
                    }                   
                    dtHZ.Rows.Add(dr);
                }
                string[] colname = new string[dtHZ.Columns.Count];
                string filename = query.category_name + "-類別訂單明細" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                newExcelName = Server.MapPath(excelPath_export) + filename;
                for (int i = 0; i < dtHZ.Columns.Count; i++)
                {
                    colname[i] = dtHZ.Columns[i].ColumnName;
                }

                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }
                ExcelHelperXhf.ExportDTtoExcel(dtHZ, "", newExcelName);
                json = "{success:true,ExcelName:\'" + filename + "\'}";
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

    }
}
