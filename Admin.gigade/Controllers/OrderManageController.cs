using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Model;
using System.Configuration;
using System.Text;
using System.Data;
using System.Net;
using BLL.gigade;
using System.IO;
using gigadeExcel.Comment;

namespace Admin.gigade.Controllers
{
    public class OrderManageController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IHappyGoImplMgr _hgmgr;
        private ITabShowImplMgr _tbshow;
        private IVendorImplMgr _vendorImp;
        private ITabShowImplMgr _tabshow;
        private IParametersrcImplMgr _ptersrc;
        private IOrderDetailImplMgr _orderDetailMgr;
        private IOrderMasterImplMgr _orderMasterMgr;
        private IOrderUserReduceImplMgr _IOrderUserMgr;
        private IOrderCancelMasterImplMgr _orderCancelMgr;
        private IOrderCancelMsgImplMgr _orderCancelMsgMgr;
        private OrderReturnMasterMgr _orderReturnMaster;
        private IOrderExpectDeliverImplMgr _orderExpectDeliverMgr;
        private OrderBrandProducesMgr _Iorderbrandproduces;
        private OrderVendorProducesMgr _orderVendorProducesMgr;
        private ZipMgr zMgr;
        private VendorMgr _vendorMgr;
        private OrderMasterMgr _ordermaster;
        private OrderReturnStatusMgr _orderReturnStatus;
        private OrderMoneyReturnMgr _orderMoneyReturnMgr;
        static string excelPath = ConfigurationManager.AppSettings["ImportOrderExcel"];//關於導入的excel文件的限制
        static string HgReturnUrl = ConfigurationManager.AppSettings["HG_RETURN_URL"];//退货单返回hg点数url
        static string HgMerchandID = ConfigurationManager.AppSettings["HG_MERCHANDID"];//退货单返回hg点数url
        static string HgTeminalID = ConfigurationManager.AppSettings["HG_TERMINALID"];//退货单返回hg点数url
        static string HgCategoty = ConfigurationManager.AppSettings["HG_CATEGORY"];//退货单返回hg点数url
        static string HgWallet = ConfigurationManager.AppSettings["HG_WALLET"];//退货单返回hg点数url
        string vendorServerPath = ConfigurationManager.AppSettings["vendorServerPath"];//鏈接至供應商後台的地址
        //
        // GET: /OrderManage/
        #region View視圖
        public ActionResult Index()
        {
            ViewBag.OrderId = Request.QueryString["Order_Id"] ?? "";//獲取付款單號
            return View();
        }
        /// <summary>
        /// 物流單
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverIndex()
        {
            return View();
        }
        /// <summary>
        /// 品牌訂單查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult BrandProductIndex()
        {
            return View();
        }
        /// <summary>
        /// 取消單
        /// </summary>
        /// <returns></returns>
        public ActionResult CancelMasterIndex()
        {
            return View();
        }
        /// <summary>
        /// 供應商訂單查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult VendorIndex()
        {
            return View();
        }
        /// <summary>
        /// 營養類別
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoryIndex()
        {
            return View();
        }
        /// <summary>
        /// 取消訂單通知
        /// </summary>
        /// <returns></returns>
        public ActionResult CancelMsgIndex()
        {
            return View();
        }
        /// <summary>
        /// 預購單
        /// </summary>
        /// <returns></returns>
        public ActionResult ExpectIndex()
        {
            return View();
        }
        /// <summary>
        /// 品牌營養額統計
        /// </summary>
        /// <returns></returns>
        public ActionResult RevenueIndex()
        {
            return View();
        }
        /// <summary>
        /// 暫存退貨單
        /// </summary>
        /// <returns></returns>
        public ActionResult ReturnMasterTempIndex()
        {
            return View();
        }
        /// <summary>
        /// 退貨單
        /// </summary>
        /// <returns></returns>
        public ActionResult ReturnMasterIndex()
        {
            return View();
        }
        /// <summary>
        /// 訂單查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderIndex()
        {
            return View();
        }
        /// <summary>
        /// 大陸訂單查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderSearchChina()
        {
            return View();
        }
        /// <summary>
        /// 促銷減免查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderUserReduce()
        {
            return View();
        }
        /// <summary>
        /// 訂單內容
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderDetialList()
        {
            //vendorServerPath
            ViewBag.VendorServerPath = vendorServerPath;
            ViewBag.OrderId = Request.QueryString["Order_Id"] ?? "";//獲取付款單號
            #region 控制List頁面字段顯示與否
            OrderMasterQuery query = new OrderMasterQuery();
            _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
            query.Order_Id = Convert.ToUInt32(Request.QueryString["Order_Id"] ?? "");
            string result = _orderMasterMgr.VerifyData(query.Order_Id);

            string[] preValue = result.Split(';');
            ViewBag.site = preValue[0];
            ViewBag.sinopa = preValue[1];
            ViewBag.channel = preValue[2];
            ViewBag.hncb = preValue[3];
            ViewBag.companyWrite = preValue[4].ToString();
            ViewBag.importTime = CommonFunction.GetNetTime(CommonFunction.GetPHPTime(preValue[5].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
            ViewBag.channelOderId = preValue[6];
            ViewBag.retrieve_mode = preValue[7];
            ViewBag.delivery_same = preValue[8];
            ViewBag.delivery_name = preValue[9];
            ViewBag.c_delivery_gender = preValue[10];
            #endregion
            return View();
        }
        #region 訂單查詢 訂單詳情操作記錄

        /// <summary>
        /// 狀態列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Status()
        {
            return View();
        }
        /// <summary>
        /// 物流出貨狀態
        /// </summary>
        /// <returns></returns>
        public ActionResult Logistics()
        {
            return View();
        }
        /// <summary>
        /// 出貨單
        /// </summary>
        /// <returns></returns>
        public ActionResult Deliver()
        {
            return View();
        }

        /// <summary>
        /// 新出貨單
        /// </summary>
        /// <returns></returns>
        public ActionResult NewDeliver()
        {
            return View();
        }
        /// <summary>
        /// 取消單
        /// </summary>
        /// <returns></returns>
        public ActionResult Cancel()
        {
            return View();
        }

        /// <summary>
        /// 退貨單
        /// </summary>
        /// <returns></returns>
        public ActionResult Return()
        {
            return View();
        }
        /// <summary>
        /// 退款單
        /// </summary>
        /// <returns></returns>
        public ActionResult Money()
        {
            return View();
        }
        /// <summary>
        /// 問題域回覆
        /// </summary>
        /// <returns></returns>
        public ActionResult Question()
        {
            return View();
        }
        /// <summary>
        /// 取消訂單問題
        /// </summary>
        /// <returns></returns>
        public ActionResult CancelMsg()
        {
            return View();
        }
        /// <summary>
        /// 聯合信用卡
        /// </summary>
        /// <returns></returns>
        public ActionResult NCCC()
        {
            return View();
        }
        /// <summary>
        /// 訂單頁面之支付寶頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult Alipay()
        {
            return View();
        }
        /// <summary>
        /// 訂單頁面之銀聯頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult UnionPay()
        {
            return View();
        }
        /// <summary>
        /// happyoGo
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderHg()
        {
            return View();
        }
        /// <summary>
        /// 購物金扣除記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderBonus()
        {
            return View();
        }
        /// <summary>
        /// 發票記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderInvoice()
        {
            return View();
        }
        /// <summary>
        /// 中国信托
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderPaymentCt()
        {
            return View();
        }
        /// <summary>
        /// 華南匯款資料
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderHncb()
        {
            return View();
        }
        /// <summary>
        /// Hitrust-網際威信
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderHitrust()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreditCardMsg()
        {
            return View();
        
        }

        public ActionResult OrderReturnList()
        {
            ViewBag.return_id = Request.QueryString["return_id"] ?? "";//獲取退款單號
            return View();
        }
        /// <summary>
        /// 訂單內容之退貨單
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderReturnMasterView()
        {
            return View();

        }
        #endregion
        #region 訂單內容
        #region 訂單內容頁面的數據顯示(單一商品和父商品) GetOrderDetailList()
        public HttpResponseBase GetOrderDetailList()
        {
            List<OrderDetailQuery> stores = new List<OrderDetailQuery>();
            string json = string.Empty;
            try
            {
                OrderDetailQuery query = new OrderDetailQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                //付款單號
                query.Order_Id = Convert.ToUInt32(Request.Params["OrderId"].ToString());
                query.isChildItem = 0;
                _orderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _orderDetailMgr.GetOrderDetailList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                //獲取供應商後臺登陸的vendorMd5加密
                _vendorImp = new VendorMgr(mySqlConnectionString);
                foreach (OrderDetailQuery item in stores)
                {
                    string str = _vendorImp.GetLoginId(Convert.ToInt32(item.Vendor_Id));
                    if (str != "")
                    {
                        HashEncrypt hmd5 = new HashEncrypt();
                        string mdlogin_id = hmd5.Md5Encrypt(str, "MD5");
                        item.VendorMd5 = hmd5.Md5Encrypt(mdlogin_id + str, "MD5");
                    }                 
                    if (item.item_mode == 2)
                    {
                        item.subtotal = (item.Single_Money * item.parent_num)-uint.Parse(item.Deduct_Happygo_Money.ToString())-item.Deduct_Welfare-item.Deduct_Bonus;
                    }
                    else
                    {
                        item.subtotal = (item.Single_Money * item.Buy_Num) -uint.Parse( item.Deduct_Happygo_Money.ToString()) - item.Deduct_Welfare - item.Deduct_Bonus;
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
        #region 獲取付款單中的子商品訂單信息 + HttpResponseBase GetOrderChildList()
        public HttpResponseBase GetOrderChildList()
        {
            List<OrderDetailQuery> stores = new List<OrderDetailQuery>();
            string json = string.Empty;
            try
            {
                OrderDetailQuery query = new OrderDetailQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "5");//用於分頁的變量
                //付款單號
                query.Order_Id = Convert.ToUInt32(Request.Params["OrderId"].ToString());
                query.Parent_Id = Convert.ToInt32(Request.Params["ParentId"].ToString());
                query.pack_id = Convert.ToUInt32(Request.Params["PackId"].ToString());
                query.isChildItem = 1;
                _orderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _orderDetailMgr.GetOrderDetailList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                foreach (var item in stores)
                {//訂單內容頁面查詢的是自商品信息需要*parent_num
                    item.Buy_Num = item.Buy_Num * item.parent_num;
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
        #region 訂單內容之支付寶+HttpResponseBase GetAlipayList()
        /// <summary>
        /// 支付寶
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetAlipayList()
        {
            string json = string.Empty;

            OrderPaymentAlipay query = new OrderPaymentAlipay();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.ordernumber = Request.Params["Order_Id"];
            }
            try
            {
                List<OrderPaymentAlipay> store = new List<OrderPaymentAlipay>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetAlipayList(query, out  totalCount);
                //for (int i = 0; i < store.Count; i++)
                //{
                //    string time = store[i].timepaid;
                //    DateTime timepaid = DateTime.Parse(time);
                //    store[i].timepaid = timepaid.ToString("yyyy-mm-dd hh:mm:ss");
                //}
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之物流出貨狀態+HttpResponseBase GetLogistics()
        public HttpResponseBase GetLogistics()
        {
            string json = string.Empty;

            LogisticsDetailQuery query = new LogisticsDetailQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.order_id = Request.Params["Order_Id"];
            }
            try
            {
                List<LogisticsDetailQuery> store = new List<LogisticsDetailQuery>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetLogistics(query, out  totalCount);
               
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
        #region 訂單內容之銀聯+HttpResponseBase GetUnionPayList()
        /// <summary>
        /// 銀聯UnionPayList
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetUnionPayList()
        {
            string json = string.Empty;

            OrderPaymentUnionPay query = new OrderPaymentUnionPay();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.order_id = uint.Parse(Request.Params["Order_Id"]);
            }

            try
            {
                List<OrderPaymentUnionPay> store = new List<OrderPaymentUnionPay>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetUnionPayList(query, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之購物金扣除記錄+HttpResponseBase GetOrderBonus()
        /// <summary>
        /// 購物金扣除記錄
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetOrderBonus()
        {
            string json = string.Empty;

            UsersDeductBonus query = new UsersDeductBonus();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.order_id = uint.Parse(Request.Params["Order_Id"]);
            }

            try
            {
                List<UsersDeductBonus> store = new List<UsersDeductBonus>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetUserDeductBonus(query, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region MyRegion+訂單內容之發票記錄  +GetInvoiceMasterList()
        /// <summary>
        /// 發票記錄
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetInvoiceMasterList()
        {
            string json = string.Empty;

            InvoiceMasterRecordQuery query = new InvoiceMasterRecordQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.order_id = uint.Parse(Request.Params["Order_Id"]);
            }
            try
            {
                List<InvoiceMasterRecordQuery> store = new List<InvoiceMasterRecordQuery>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetInvoiceMasterRecord(query, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之中國信託+GetOrderPaymentCtList()
        /// <summary>
        /// 中國信託
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetOrderPaymentCtList()
        {
            string json = string.Empty;

            OrderPaymentCt query = new OrderPaymentCt();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.lidm = int.Parse(Request.Params["Order_Id"]);
            }
            try
            {
                List<OrderPaymentCt> store = new List<OrderPaymentCt>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetOrderPaymentCtList(query, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之華南匯款資料+ GetOrderHncbList()
        /// <summary>
        /// 華南匯款資料
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetOrderHncbList()
        {
            string json = string.Empty;

            OrderPaymentHncbQuery query = new OrderPaymentHncbQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.order_id = uint.Parse(Request.Params["Order_Id"].ToString());
            }
            try
            {
                List<OrderPaymentHncbQuery> store = new List<OrderPaymentHncbQuery>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.QueryOrderHncb(query, out  totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之Hitrust-網際威信+HttpResponseBase GetOrderHitrustList()
        /// <summary>
        /// Hitrust-網際威信
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetOrderHitrustList()
        {
            string json = string.Empty;

            OrderPaymentHitrustQuery query = new OrderPaymentHitrustQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.order_id = Request.Params["Order_Id"];
            }
            try
            {
                List<OrderPaymentHitrustQuery> store = new List<OrderPaymentHitrustQuery>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetOderHitrust(query, out  totalCount);
                foreach (var item in store)
                {
                    item.card_number ="'"+item.pan+"','"+item.bankname+"'";
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之happyGo點數 交易記錄 +HttpResponseBase GetHgDeductList()
        /// <summary>
        /// happyGo點數 交易記錄
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetHgDeductList()
        {
            string json = string.Empty;
            uint order_id = 0;

            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                order_id = uint.Parse(Request.Params["Order_Id"]);
            }

            try
            {
                _hgmgr = new HappyGoMgr(mySqlConnectionString);
                List<HgDeduct> hgstore = _hgmgr.GetHGDeductList(order_id);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(hgstore, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之happyGo點數 累點記錄 +HttpResponseBase GetHgAccumulateList()
        /// <summary>
        /// happyGo點數 累點記錄 
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetHgAccumulateList()
        {
            string json = string.Empty;
            uint order_id = 0;
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                order_id = uint.Parse(Request.Params["Order_Id"]);
            }

            try
            {
                _hgmgr = new HappyGoMgr(mySqlConnectionString);
                List<HgAccumulate> hgstore = _hgmgr.GetHGAccumulateList(order_id);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(hgstore, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之happyGo點數 累點取回記錄 +HttpResponseBase GetHgAccumulateRefundList()
        /// <summary>
        /// happyGo點數 累點取回記錄
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetHgAccumulateRefundList()
        {
            string json = string.Empty;
            uint order_id = 0;
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                order_id = uint.Parse(Request.Params["Order_Id"]);
            }

            try
            {
                _hgmgr = new HappyGoMgr(mySqlConnectionString);
                List<HgAccumulateRefund> hgstore = _hgmgr.GetHGAccumulateRefundList(order_id);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(hgstore, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之happyGo點數 抵點歸還記錄 +HttpResponseBase GetHgDeductRefundList()
        /// <summary>
        /// happyGo點數 抵點歸還記錄
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetHgDeductRefundList()
        {
            string json = string.Empty;
            uint order_id = 0;

            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                order_id = uint.Parse(Request.Params["Order_Id"]);
            }

            try
            {
                _hgmgr = new HappyGoMgr(mySqlConnectionString);
                List<HgDeductRefund> hgstore = _hgmgr.GetHgDeductRefundList(order_id);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(hgstore, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單內容之happyGo點數 抵點歸還記錄(即時) +HttpResponseBase GetHgDeductReverseList()
        /// <summary>
        /// happyGo點數 抵點歸還記錄(即時)
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetHgDeductReverseList()
        {
            string json = string.Empty;
            uint order_id = 0;

            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                order_id = uint.Parse(Request.Params["Order_Id"]);
            }

            try
            {
                _hgmgr = new HappyGoMgr(mySqlConnectionString);
                List<HgDeductReverse> hgstore = _hgmgr.GetHgDeductReverseList(order_id);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(hgstore, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 新出貨單+HttpResponseBase GetNewDeliver()
        public HttpResponseBase GetNewDeliver()
        {
            List<DeliverMasterQuery> stores = new List<DeliverMasterQuery>();
            string json = string.Empty;
            try
            {
                DeliverMasterQuery query = new DeliverMasterQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetNewDeliver(query, out totalCount);
                foreach (var item in stores)
                {
                    if (item.delivery_date.ToString("yyyy-MM-dd HH:mm:ss") == "0001-01-01 00:00:00")
                    {
                        item.delivery_date_str = "";
                    }
                    else
                    {
                        item.delivery_date_str = item.delivery_date.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();

                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        #region 取消單+HttpResponseBase GetNewDeliver()
        public HttpResponseBase GetCancel()
        {
            List<OrderCancelMasterQuery> stores = new List<OrderCancelMasterQuery>();
            string json = string.Empty;
            try
            {
                OrderCancelMasterQuery query = new OrderCancelMasterQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToUInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetCancel(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();

                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        #region 退貨單内容GridView+HttpResponseBase GetReturn()
        public HttpResponseBase GetReturn()
        {
            List<OrderReturnMasterQuery> stores = new List<OrderReturnMasterQuery>();
            string json = string.Empty;
            try
            {
                OrderReturnMasterQuery query = new OrderReturnMasterQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToUInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetReturn(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();

                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        #region 退款單+HttpResponseBase GetMoney()
        public HttpResponseBase GetMoney()
        {
            List<OrderMoneyReturnQuery> stores = new List<OrderMoneyReturnQuery>();
            string json = string.Empty;
            try
            {
                OrderMoneyReturnQuery query = new OrderMoneyReturnQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToUInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetMoney(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();

                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        public HttpResponseBase SaveCSNote()
        {
            OrderMoneyReturnQuery query = new OrderMoneyReturnQuery();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["money_id"]))
                {
                    query.money_id = Convert.ToUInt32(Request.Params["money_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["cs_note"]))
                {
                    query.cs_note = Request.Params["cs_note"].Replace("\n","  "); 
                }
                _orderMoneyReturnMgr = new OrderMoneyReturnMgr(mySqlConnectionString);
                json = _orderMoneyReturnMgr.SaveCSNote(query);
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
        #region 問題與回覆+HttpResponseBase GetQuestion()
        public HttpResponseBase GetQuestion()
        {
            List<OrderQuestionQuery> stores = new List<OrderQuestionQuery>();
            string json = string.Empty;
            try
            {
                OrderQuestionQuery query = new OrderQuestionQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToUInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetQuestion(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();

                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        #region 取消單問題+HttpResponseBase GetCancelMsg()
        public HttpResponseBase GetCancelMsg()
        {
            List<OrderCancelMsgQuery> stores = new List<OrderCancelMsgQuery>();
            string json = string.Empty;
            try
            {
                OrderCancelMsgQuery query = new OrderCancelMsgQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToUInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetCancelMsg(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();

                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        #region 聯合信用卡+HttpResponseBase GetNCCC()
        public HttpResponseBase GetNCCC()
        {
            List<OrderPaymentNcccQuery> stores = new List<OrderPaymentNcccQuery>();
            string json = string.Empty;
            try
            {
                OrderPaymentNcccQuery query = new OrderPaymentNcccQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToUInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetNCCC(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();

                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        #region 訂單退貨
        public HttpResponseBase OrderReturn()
        {
            List<OrderReturnMasterQuery> stores = new List<OrderReturnMasterQuery>();
            OrderReturnMasterQuery query = new OrderReturnMasterQuery();
            _orderReturnMaster=new OrderReturnMasterMgr(mySqlConnectionString);
            string json = string.Empty;
            //uint id;
            try
            {
                //if (uint.TryParse(Request.Params["order_id"].ToString(), out id))
                //{
                //    query.order_id = id;
                //}
                if (!string.IsNullOrEmpty(Request.Params["detail_id"]))
                {
                    query.detailId = Request.Params["detail_id"].ToString();
                }
                if (query.detailId.Length > 0)
                {
                    query.detailId = query.detailId.Substring(0, query.detailId.Length - 1);
                }
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                query.return_ipfrom = addlist[0].ToString();
                //_tabshow = new TabShowMgr(mySqlConnectionString);
                //int totalCount = 0;
                //stores = _tabshow.GetNCCC(query, out totalCount);
                //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = _orderReturnMaster.OrderReturn(query);// "{success:true}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:flase,msg:1}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 訂單內容之退貨單
        #region 訂單內容之退貨單上+GetOrderReturnContentQueryUp
        public HttpResponseBase GetOrderReturnContentQueryUp()
        {
            string json = string.Empty;

            OrderReturnContentQuery query = new OrderReturnContentQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.orc_order_id = int.Parse(Request.Params["Order_Id"]);
            }
            try
            {
                List<OrderReturnContentQuery> store = new List<OrderReturnContentQuery>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetOrderReturnContentQueryUp(query, out  totalCount);

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
        #region 訂單內容之退貨單下+ GetOrderReturnDown()
        public HttpResponseBase GetOrderReturnDown()
        {
            string json = string.Empty;

            OrderReturnMasterQuery query = new OrderReturnMasterQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            if (!string.IsNullOrEmpty(Request.Params["Order_Id"]))
            {
                query.detailId = Request.Params["Order_Id"];
            }
            try
            {
                List<OrderReturnMasterQuery> store = new List<OrderReturnMasterQuery>();
                _tbshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _tbshow.GetReturnMasterDown(query, out  totalCount);
               
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
        #endregion
        //OrderWaitClick
        #region 轉等待付款
        public HttpResponseBase OrderWaitClick()
        {
            List<OrderReturnMasterQuery> stores = new List<OrderReturnMasterQuery>();
            OrderReturnMasterQuery query = new OrderReturnMasterQuery();
            _orderReturnMaster = new OrderReturnMasterMgr(mySqlConnectionString);
            string json = string.Empty;
            //uint id;
            try
            {
                //if (uint.TryParse(Request.Params["order_id"].ToString(), out id))
                //{
                //    query.order_id = id;
                //}
                if (!string.IsNullOrEmpty(Request.Params["detail_id"]))
                {
                    query.detailId = Request.Params["detail_id"].ToString();
                }
                if (query.detailId.Length > 0)
                {
                    query.detailId = query.detailId.Substring(0, query.detailId.Length - 1);
                }
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                query.return_ipfrom = addlist[0].ToString();
                //_tabshow = new TabShowMgr(mySqlConnectionString);
                //int totalCount = 0;
                //stores = _tabshow.GetNCCC(query, out totalCount);
                //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = _orderReturnMaster.OrderReturn(query);// "{success:true}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:flase,msg:1}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion


        #endregion

        #region 物流單
        public HttpResponseBase GetOrderDeliverInfo()
        {
            List<OrderDeliverQuery> stores = new List<OrderDeliverQuery>();
            string json = string.Empty;
            try
            {
                OrderDeliverQuery query = new OrderDeliverQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                IOrderDeliverImplMgr _IorderDeliver = new OrderDeliverMgr(mySqlConnectionString);
                query.serchs = Request.Params["serchs"].ToString();//查詢條件
                if (Request.Params["orderid"] == "" || Request.Params["orderid"] == null)//查詢內容
                {
                    query.order_id = 0;
                }
                else
                {
                    query.search = Request.Params["orderid"];
                }
                query.seldate = Convert.ToInt32(Request.Params["seldate"] ?? "0");//日期條件
                if (Request.Params["selven"] == "" || Request.Params["selven"] == null)//供應商
                {
                    query.selven = 0;
                }
                else
                {
                    query.selven = Convert.ToInt32(Request.Params["selven"] ?? "0");
                }
                DateTime dt;
                if (DateTime.TryParse(Request.Params["deliverstart"].ToString(), out dt))//開始時間
                {
                    query.deliverstart = Convert.ToInt32(CommonFunction.GetPHPTime(dt.ToString("yyyy-MM-dd 00:00:00")));
                }
                if (DateTime.TryParse(Request.Params["deliverend"].ToString(), out dt))//結束時間
                {
                    query.deliverend = Convert.ToInt32(CommonFunction.GetPHPTime(dt.ToString("yyyy-MM-dd 23:59:59")));
                }
                int totalCount = 0;
                stores = _IorderDeliver.GetOrderDeliverList(query, out totalCount);
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
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
        #region 取消單
        #region 取消單列表
        [CustomHandleError]
        public HttpResponseBase GetOrderCancelMasterList()
        {
            List<OrderCancelMaster> stores = new List<OrderCancelMaster>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                OrderCancelMaster query = new OrderCancelMaster();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//
                _orderCancelMgr = new OrderCancelMasterMgr(mySqlConnectionString);
                stores = _orderCancelMgr.GetOrderCancelMasterList(query, out totalCount);
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
        #region 修改取消單
        public HttpResponseBase ModifyOrderCancelMasterList()
        {
            List<OrderCancelMaster> stores = new List<OrderCancelMaster>();
            string json = string.Empty;
            try
            {
                OrderCancelMaster query = new OrderCancelMaster();
                query.order_id = Convert.ToUInt32(Request.Form["order_id"]);
                query.cancel_id = Convert.ToUInt32(Request.Form["cancel_id"]);
                query.cancel_status = Convert.ToUInt32(Request.Form["cancel_status"]);
                query.cancel_note = Request.Params["cancel_note"];
                query.bank_note = Request.Params["bank_note"];
                query.cancel_createdate = Convert.ToDateTime(Request.Params["cancel_createdate"]);
                query.cancel_updatedate = Convert.ToDateTime(Request.Params["cancel_updatedate"]);
                System.Net.IPAddress[] ips = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                if (ips.Length > 0)
                {
                    query.cancel_ipfrom = ips[0].ToString();
                }
                _orderCancelMgr = new OrderCancelMasterMgr(mySqlConnectionString);
                int result = _orderCancelMgr.Update(query);
                string msg="";
                switch (result)
                {
                    case 1:
                        msg="bonus not enough!";
                        break;
                    case 2:
                        msg = "bonus type error!";
                        break;
                    case 3:
                        msg = "取得身分證字號失敗!";
                        break;
                    case 4:
                        msg = "扣除HappyGo點數失敗!";
                        break;
                    case 5:
                        msg = "bonus type error !";
                        break;
                    case 100:
                         msg = "";//操作成功!
                        break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(msg))
                {
                    json = "{success:true}";//返回json數據
                }
                else 
                {
                    json = "{success:true,msg:\"" + msg + "\"}";
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
        #endregion
        #endregion
        #region 品牌訂單查詢
        #region 品牌訂單查詢列表
        public HttpResponseBase GetOrderBrandProduces()
        {
            List<OrderBrandProducesQuery> stores = new List<OrderBrandProducesQuery>();
            string json = string.Empty;
            DataTable dt = new DataTable();
            DataTable dt_status = new DataTable();
            try
            {
                StringBuilder sb = new StringBuilder();
                OrderBrandProducesQuery query = new OrderBrandProducesQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                #region 查詢條件
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
                {
                    query.selecttype = Request.Params["selecttype"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
                {
                    query.searchcon = Request.Params["searchcon"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["date_type"]))
                {
                    query.date_type = Request.Params["date_type"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                {
                    query.dateOne = DateTime.Parse(Request.Params["dateOne"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                {
                    query.dateTwo = DateTime.Parse(Request.Params["dateTwo"]).AddDays(1);
                }
                if (!string.IsNullOrEmpty(Request.Params["slave_status"]) && Request.Params["slave_status"] != "null")
                {
                    query.slave = Request.Params["slave_status"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["order_payment"]))
                {
                    query.order_payment = uint.Parse(Request.Params["order_payment"].ToString());
                }
                #endregion
                _Iorderbrandproduces = new OrderBrandProducesMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _Iorderbrandproduces.GetOrderBrandProduces(query, out totalCount, sb.ToString());
                dt = GetTP("payment");
                dt_status = GetTP("order_status");
                foreach (var item in stores)
                {
                    if (item.item_mode == 2)
                    {//如果是子商品數量*該組合的量
                        double price = double.Parse(item.single_money.ToString()) / item.buy_num;
                        item.single_money = uint.Parse(Math.Round(price, 0).ToString());
                        item.buy_num = item.buy_num * item.parent_num;
                    }
                    if (item.order_payment > 0)
                    {//帶出付款方式的參數
                        DataRow[] dr = dt.Select("ParameterCode = '" + item.order_payment.ToString() + "'");
                        DataTable _newdt = dt.Clone();
                        foreach (DataRow i in dr)
                        {
                            _newdt.Rows.Add(i.ItemArray);
                        }
                        item.payments = _newdt.Rows[0]["ParameterName"].ToString();
                    }
                    if (item.slave_status >= 0)
                    {//帶出付款方式的參數
                        DataRow[] dr = dt_status.Select("ParameterCode = '" + item.slave_status.ToString() + "'");
                        DataTable _newdt = dt_status.Clone();
                        foreach (DataRow i in dr)
                        {
                            _newdt.Rows.Add(i.ItemArray);
                        }
                        item.states = _newdt.Rows[0]["remark"].ToString();
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 品牌訂單匯出excel
        public void OrderBrandProducesExport()
        {
            OrderBrandProducesQuery query = new OrderBrandProducesQuery();
            StringBuilder sb = new StringBuilder();
            DateTime dtime = new DateTime();
            uint uid;
            #region 查詢條件
            if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
            {
                query.selecttype = Request.Params["selecttype"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
            {
                query.searchcon = Request.Params["searchcon"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["date_type"]))
            {
                query.date_type = Request.Params["date_type"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
            {
                query.dateOne = DateTime.Parse(Request.Params["dateOne"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
            {
                query.dateTwo = DateTime.Parse(Request.Params["dateTwo"]).AddDays(1);
            }
            if (!string.IsNullOrEmpty(Request.Params["slave_status"]) && Request.Params["slave_status"] != "null")
            {
                query.slave = Request.Params["slave_status"].ToString();
            }
            if (uint.TryParse(Request.Params["order_payment"],out uid))
            {
                query.order_payment = uid;
            }
            #endregion
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            DataTable dtHZ = new DataTable();
            #region 表頭
            dtHZ.Columns.Add("付款單號", typeof(String));
            dtHZ.Columns.Add("供應商", typeof(String));
            dtHZ.Columns.Add("品牌名稱", typeof(String));
            dtHZ.Columns.Add("商品名稱", typeof(String));
            dtHZ.Columns.Add("規格", typeof(String));
            dtHZ.Columns.Add("付款方式", typeof(String));
            dtHZ.Columns.Add("進貨價", typeof(String));
            dtHZ.Columns.Add("實際售價", typeof(String));
            dtHZ.Columns.Add("數量", typeof(String));
            dtHZ.Columns.Add("折扣(購物金+抵用卷+折扣金額)", typeof(String));
            dtHZ.Columns.Add("小計", typeof(String));
            dtHZ.Columns.Add("促銷", typeof(String));
            dtHZ.Columns.Add("訂購姓名", typeof(String));
            dtHZ.Columns.Add("狀態", typeof(String));
            dtHZ.Columns.Add("收件人", typeof(String));
            dtHZ.Columns.Add("性別", typeof(String));
            dtHZ.Columns.Add("郵遞區號", typeof(String));
            //dtHZ.Columns.Add("地址", typeof(String));
            //dtHZ.Columns.Add("收貨人手機", typeof(String));
            //dtHZ.Columns.Add("收貨人電話", typeof(String));
            dtHZ.Columns.Add("訂單日期", typeof(String));
            dtHZ.Columns.Add("出貨日期", typeof(String));
            dtHZ.Columns.Add("付款時間", typeof(String));
            dtHZ.Columns.Add("備註", typeof(String));
            dtHZ.Columns.Add("管理員備註", typeof(String));
            dtHZ.Columns.Add("貨運模式", typeof(String));
            dtHZ.Columns.Add("註冊時間", typeof(String));
            dtHZ.Columns.Add("出生年月", typeof(String));
            // dtHZ.Columns.Add("電子信箱", typeof(String));
            #endregion
            try
            {
                List<OrderBrandProducesQuery> stores = new List<OrderBrandProducesQuery>();
                _Iorderbrandproduces = new OrderBrandProducesMgr(mySqlConnectionString);
                stores = _Iorderbrandproduces.OrderBrandProducesExport(query, sb.ToString());
                #region 填充data
                foreach (var item in stores)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr["付款單號"] = item.order_id;
                    dr["供應商"] = item.vendor_name_simple;
                    dr["品牌名稱"] = item.brand_name;
                    dr["商品名稱"] = item.product_name;
                    dr["規格"] = item.product_spec_name;
                    dr["付款方式"] = item.payments;
                    if (item.event_cost == 0)
                    {
                        dr["進貨價"] = item.single_cost;
                    }
                    else
                    {
                        dr["進貨價"] = item.event_cost;
                    }
                    if (item.item_mode == 2)
                    {
                        double price = double.Parse(item.single_money.ToString()) / item.buy_num;
                        dr["實際售價"] = uint.Parse(Math.Round(price, 0).ToString());
                        dr["數量"] = item.buy_num * item.parent_num;
                        dr["小計"] = item.single_money * item.parent_num - item.deduct_bonus - item.deduct_happygo_money - item.deduct_welfare;
                     
                        //dr["實際售價"] = item.single_money / item.buy_num;
                        

                        //dr["數量"] = item.buy_num*item.parent_num;

                    }else{
                        dr["實際售價"] = item.single_money;

                        dr["數量"] = item.buy_num;
                        dr["小計"] = item.buy_num * item.single_money - item.deduct_bonus - item.deduct_happygo_money - item.deduct_welfare;
                    }
                    //dr["數量"] = item.buy_num;
                    dr["折扣(購物金+抵用卷+折扣金額)"] = item.deduct_bonus + item.deduct_happygo_money + item.deduct_welfare;
                   // dr["小計"] = item.buy_num * item.single_money - item.deduct_bonus - item.deduct_happygo_money - item.deduct_welfare;
                    if (item.event_cost != 0 && item.single_cost != item.single_money)
                    {
                        dr["促銷"] = "是";
                    }
                    else
                    {
                        dr["促銷"] = "-";
                    }

                    dr["訂購姓名"] = item.order_name;
                    dr["狀態"] = item.states;
                    dr["收件人"] = item.delivery_name;
                    dr["性別"] = item.delivery_genders;
                    dr["郵遞區號"] = item.delivery_zips;
                    dr["訂單日期"] = item.order_createdates;
                    dtime = DateTime.Parse("1970/01/01 08:00:00");
                    if (item.slave_date_deliverys == dtime)
                    {
                        dr["出貨日期"] = "-";
                    }
                    else
                    {
                        dr["出貨日期"] = item.slave_date_deliverys;
                    }
                    if (item.order_date_pays == dtime)
                    {
                        dr["付款時間"] = "未付款";
                    }
                    else
                    {
                        dr["付款時間"] = item.order_date_pays;
                    }
                    dr["備註"] = item.note_order;
                    dr["管理員備註"] = item.note_admin;
                    #region  貨運模式
                    switch (item.product_freight_set.ToString())
                    {
                        case "1":
                            dr["貨運模式"] = "常溫";
                            break;
                        case "2":
                            dr["貨運模式"] = "冷凍";
                            break;
                        case "3":
                            dr["貨運模式"] = "常溫";
                            break;
                        case "4":
                            dr["貨運模式"] = "冷凍";
                            break;
                        case "5":
                            dr["貨運模式"] = "冷藏";
                            break;
                        case "6":
                            dr["貨運模式"] = "冷藏";
                            break;
                    }
                    #endregion
                    dr["註冊時間"] = item.user_reg_dates;
                    dr["出生年月"] = item.user_birthday;
                    //  dr["電子信箱"] = item.user_email;
                    dtHZ.Rows.Add(dr);
                }
                #endregion
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = "品牌訂單匯出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
            }
        }
        #endregion
        #endregion
        #region 供應商訂單查詢
        public HttpResponseBase GetOrderVendorProduces()
        {
            List<OrderVendorProducesQuery> stores = new List<OrderVendorProducesQuery>();
            string json = string.Empty;
            DataTable dt_payment = new DataTable();
            DataTable dt_status = new DataTable();
            try
            {
                StringBuilder sb = new StringBuilder();
                OrderVendorProducesQuery query = new OrderVendorProducesQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                uint v_id = 0;
                #region 供應商查詢條件
                if (uint.TryParse(Request.Params["Vendor_Id"].ToString(), out v_id))
                {
                    query.Item_Vendor_Id = v_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
                {
                    query.selecttype = Request.Params["selecttype"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
                {
                    query.searchcon = Request.Params["searchcon"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["datetype"]))
                {
                    query.date_type = Request.Params["datetype"].ToString();
                }
                DateTime dtime;
                if (DateTime.TryParse(Request.Params["dateStart"].ToString(), out dtime))
                {
                    query.dateStart = dtime;
                }
                if (DateTime.TryParse(Request.Params["dateEnd"].ToString(), out dtime))
                {
                    query.dateEnd = dtime.AddDays(1);
                }
                if (!string.IsNullOrEmpty(Request.Params["order_status"]))
                {//狀態
                    query.slave = Request.Params["order_status"];
                }
                if (uint.TryParse(Request.Params["order_payment"], out v_id))
                {//付款方式
                    query.order_payment = v_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["product_freight_set"]))
                {
                    query.product_freight_set_in = Request.Params["product_freight_set"];
                }
                if (!string.IsNullOrEmpty(Request.Params["product_manage"]))
                {//供應商管理者查詢條件
                    query.product_manage = Request.Params["product_manage"];
                }
                #endregion
                _orderVendorProducesMgr = new OrderVendorProducesMgr(mySqlConnectionString);
                DataTable dt = _orderVendorProducesMgr.GetProductItem();
                int totalCount = 0;
                stores = _orderVendorProducesMgr.GetOrderVendorProduces(query, out totalCount);
                dt_payment = GetTP("payment");
                dt_status = GetTP("order_status");
                for (int i = 0; i < stores.Count; i++)
                {
                    DataRow[] drs = dt.Select("item_id=" + stores[i].Item_Id);
                    if (drs.Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(drs[0]["spec_image"].ToString()))
                        {
                            stores[i].pic_patch = "http://a.gimg.tw/product_spec/280x280/" + drs[0]["spec_image"].ToString().Substring(0, 2) + "/";
                            stores[i].pic_patch += drs[0]["spec_image"].ToString().Substring(2, 2) + "/" + drs[0]["spec_image"].ToString();
                        }
                    }
                    else
                    {
                        stores[i].pic_patch = "";
                    }
                    if (stores[i].item_mode == 2)
                    {//如果是子商品數量*該組合的量

                        double price = double.Parse(stores[i].Single_Money.ToString()) / stores[i].Buy_Num;
                        stores[i].Single_Money = uint.Parse(Math.Round(price, 0).ToString());
                        stores[i].Buy_Num = stores[i].Buy_Num * stores[i].parent_num;
                    }
                    if (stores[i].order_payment > 0)
                    {//帶出付款方式的參數
                        DataRow[] dr = dt_payment.Select("ParameterCode = '" + stores[i].order_payment.ToString() + "'");
                        DataTable _newdt = dt_payment.Clone();
                        foreach (DataRow r in dr)
                        {
                            _newdt.Rows.Add(r.ItemArray);
                        }
                        stores[i].payment = _newdt.Rows[0]["ParameterName"].ToString();
                    }
                    if (stores[i].slave_status >= 0)
                    {//帶出付款方式的參數
                        DataRow[] dr = dt_status.Select("ParameterCode = '" + stores[i].slave_status.ToString() + "'");
                        DataTable _newdt = dt_status.Clone();
                        foreach (DataRow r in dr)
                        {
                            _newdt.Rows.Add(r.ItemArray);
                        }
                        stores[i].slave = _newdt.Rows[0]["remark"].ToString();
                    }

                }
                //foreach (var item in stores)
                //{
                    
                //    DataRow[] drs = dt.Select("item_id=" + item.Item_Id);
                //    if (drs.Count() > 0)
                //    {
                //        if (!string.IsNullOrEmpty(drs[0]["spec_image"].ToString()))
                //        {
                //            item.pic_patch = "http://a.gimg.tw/product_spec/280x280/" + drs[0]["spec_image"].ToString().Substring(0, 2) + "/";
                //            item.pic_patch += drs[0]["spec_image"].ToString().Substring(2, 2) + "/" + drs[0]["spec_image"].ToString();
                //        }
                //    }
                //    else
                //    {
                //        item.pic_patch = "";
                //    }

                //    if (item.item_mode == 2)
                //    {//如果是子商品數量*該組合的量
                        
                //        double price = double.Parse(item.Single_Money.ToString()) / item.Buy_Num;
                //        item.Single_Money  = uint.Parse(Math.Round(price, 0).ToString());
                //        item.Buy_Num = item.Buy_Num * item.parent_num;
                //    }
                //    if (item.order_payment > 0)
                //    {//帶出付款方式的參數
                //        DataRow[] dr = dt_payment.Select("ParameterCode = '" + item.order_payment.ToString() + "'");
                //        DataTable _newdt = dt_payment.Clone();
                //        foreach (DataRow i in dr)
                //        {
                //            _newdt.Rows.Add(i.ItemArray);
                //        }
                //        item.payment =  _newdt.Rows[0]["ParameterName"].ToString();
                //    }
                //    if (item.slave_status >= 0)
                //    {//帶出付款方式的參數
                //        DataRow[] dr = dt_status.Select("ParameterCode = '" + item.slave_status.ToString() + "'");
                //        DataTable _newdt = dt_status.Clone();
                //        foreach (DataRow i in dr)
                //        {
                //            _newdt.Rows.Add(i.ItemArray);
                //        }
                //        item.slave = _newdt.Rows[0]["remark"].ToString();
                //    }
                //}
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 供應商訂單匯出Excel
        public void ExportCSV()
        {
            string newCSVName = string.Empty;
            OrderVendorProducesQuery query = new OrderVendorProducesQuery();
            DataTable dt = new DataTable();
            try
            {
                #region 供應商查詢條件
                uint v_id = 0;
                if (uint.TryParse(Request.Params["Vendor_Id"].ToString(), out v_id))
                {
                    query.Item_Vendor_Id = v_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
                {
                    query.selecttype = Request.Params["selecttype"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
                {
                    query.searchcon = Request.Params["searchcon"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["datetype"]))
                {
                    query.date_type = Request.Params["datetype"].ToString();
                }
                DateTime dtime;
                if (DateTime.TryParse(Request.Params["dateStart"].ToString(), out dtime))
                {
                    query.dateStart = dtime;
                }
                if (DateTime.TryParse(Request.Params["dateEnd"].ToString(), out dtime))
                {
                    query.dateEnd = dtime.AddDays(1);
                }
                if (!string.IsNullOrEmpty(Request.Params["order_status"]) && Request.Params["order_status"] != "null")
                {//狀態
                    query.slave = Request.Params["order_status"].ToString();
                }
                if (uint.TryParse(Request.Params["order_payment"], out v_id))
                {//付款方式
                    query.order_payment = v_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["product_freight_set"]) && Request.Params["product_freight_set"] != "null")
                {
                    query.product_freight_set_in = Request.Params["product_freight_set"];
                }
                
               
                if (!string.IsNullOrEmpty(Request.Params["product_manage"]))
                {//供應商管理者查詢條件
                    query.product_manage = Request.Params["product_manage"];
                }
                #endregion
                _orderVendorProducesMgr = new OrderVendorProducesMgr(mySqlConnectionString);
                DataTable dta = new DataTable();
                dta = _orderVendorProducesMgr.ExportCsv(query);
                #region 表頭
                dt.Columns.Add("付款單號", typeof(String));
                dt.Columns.Add("供應商", typeof(String));
                dt.Columns.Add("倉別", typeof(String));
                dt.Columns.Add("品牌名稱", typeof(String));
                dt.Columns.Add("商品名稱", typeof(String));
                dt.Columns.Add("購物編號", typeof(String));
                dt.Columns.Add("規格", typeof(String));
                dt.Columns.Add("付款方式", typeof(String));
                dt.Columns.Add("商品類型", typeof(String));
                dt.Columns.Add("進貨價", typeof(String));
                dt.Columns.Add("實際售價", typeof(String));
                dt.Columns.Add("數量", typeof(String));
                dt.Columns.Add("使用購物金", typeof(String));
                dt.Columns.Add("使用抵用券", typeof(String));
                dt.Columns.Add("使用HG", typeof(String));
                dt.Columns.Add("小計", typeof(String));
                dt.Columns.Add("促銷", typeof(String));
                dt.Columns.Add("訂購姓名", typeof(String));
                dt.Columns.Add("狀態", typeof(String));
                dt.Columns.Add("收件人", typeof(String));
                dt.Columns.Add("性別", typeof(String));
                dt.Columns.Add("郵遞區號", typeof(String));
                //dt.Columns.Add("地址", typeof(String));
                //dt.Columns.Add("收貨人手機", typeof(String));
                //dt.Columns.Add("收貨人電話", typeof(String));
                dt.Columns.Add("購物單狀態", typeof(String));
                dt.Columns.Add("訂單日期", typeof(String));
                dt.Columns.Add("出貨日期", typeof(String));
                dt.Columns.Add("可出貨日期", typeof(String));
                dt.Columns.Add("付款日期", typeof(String));
                dt.Columns.Add("發票日期", typeof(String));
                dt.Columns.Add("貨款日期", typeof(String));
                dt.Columns.Add("備註", typeof(String));
                dt.Columns.Add("管理人員備註", typeof(String));
                dt.Columns.Add("貨運模式", typeof(String));
                dt.Columns.Add("註冊時間", typeof(String));
                dt.Columns.Add("出生年月", typeof(String));
                dt.Columns.Add("管理者", typeof(String));
                //dt.Columns.Add("電子信箱", typeof(String));
                dt.Columns.Add("宅配代碼", typeof(String));
                dt.Columns.Add("宅配時間", typeof(String));
                dt.Columns.Add("商品編號", typeof(String));
                dt.Columns.Add("假日可出貨", typeof(String));
                #endregion
                foreach (DataRow dr in dta.Rows)
                {
                    #region 數據插入
                    DataRow newRow = dt.NewRow();
                    newRow[0] = dr["order_id"].ToString();
                    newRow[1] = dr["vendor_name_simple"].ToString();
                    switch (dr["product_mode"].ToString())
                    {//出貨方式
                        case "1":
                            newRow[2] = "供應商自行出貨";
                            break;
                        case "2":
                            newRow[2] = "寄倉";
                            break;
                        case "3":
                            newRow[2] = "調度";
                            break;
                        default:
                            newRow[2] = "其他";
                            break;
                    }
                    newRow[3] = dr["brand_name"].ToString();
                    newRow[4] = dr["product_name"].ToString();
                    newRow[5] = dr["detail_id"].ToString();
                    newRow[6] = dr["product_spec_name"].ToString();
                    newRow[7] = dr["order_payment"].ToString();
                    switch (dr["item_mode"].ToString())
                    {//商品類型
                        case "1":
                            newRow[8] = "父商品";
                            break;
                        case "2":
                            newRow[8] = "子商品";
                            break;
                        default:
                            newRow[8] = "單一商品";
                            break;
                    }
                    //if (dr["item_mode"].ToString() == "1")
                    //{
                    //    newRow[9] = "";
                    //}
                    //else
                    //{
                        newRow[9] = dr["single_cost"];
                    //}
                    if (dr["event_cost"].ToString() != "0")
                    {
                        newRow[9] = dr["event_cost"];
                    }
                    newRow[12] = Convert.ToInt32(dr["deduct_bonus"].ToString() == "" ? "0" : dr["deduct_bonus"].ToString());
                    newRow[13] = dr["deduct_welfare"].ToString();
                    newRow[14] = dr["deduct_happygo_money"].ToString();
                    int zk = Convert.ToInt32(dr["deduct_bonus"].ToString() == "" ? "0" : dr["deduct_bonus"].ToString()) + Convert.ToInt32(dr["deduct_welfare"].ToString() == "" ? "0" : dr["deduct_welfare"].ToString()) + Convert.ToInt32(dr["deduct_happygo_money"].ToString() == "" ? "0" : dr["deduct_happygo_money"].ToString());
                    if (int.Parse(dr["item_mode"].ToString()) == 2)//single_money
                    {
                        double price = double.Parse(dr["single_money"].ToString()) / int.Parse(dr["buy_num"].ToString());

                        newRow[10] = uint.Parse(Math.Round(price, 0).ToString());//實際售價
                        newRow[11] = int.Parse(dr["buy_num"].ToString()) * int.Parse(dr["parent_num"].ToString());//購買數量
                        newRow[15] = int.Parse(dr["single_money"].ToString()) * int.Parse(dr["parent_num"].ToString())-zk;//小計
                    }
                    else 
                    {
                        newRow[10] = dr["single_money"].ToString();
                        newRow[11] = dr["buy_num"].ToString();
                        newRow[15] = int.Parse(dr["single_money"].ToString()) * int.Parse(dr["buy_num"].ToString())-zk;
                    }
                    //newRow[10] = dr["single_money"].ToString();
                    //newRow[11] = dr["buy_num"].ToString();
                    //newRow[15] = dr["subtotal"].ToString();
                    switch (dr["event"].ToString())
                    {//促銷
                        case "0":
                            newRow[16] = "-";
                            break;
                        default:
                            newRow[16] = "是";
                            break;
                    }
                    newRow[17] = dr["order_name"].ToString();
                    newRow[18] = dr["slave_status"].ToString();
                    newRow[19] = dr["delivery_name"].ToString();
                    switch (dr["delivery_gender"].ToString())
                    {//男 or 女
                        case "0":
                            newRow[20] = "女";
                            break;
                        case "1":
                            newRow[20] = "男";
                            break;
                        default:
                            break;
                    }
                    newRow[21] = dr["delivery_zip"].ToString();
                    newRow[22] = dr["detail_status"].ToString();
                    newRow[23] = dr["order_createdate"].ToString();
                    newRow[24] = dr["slave_date_delivery"].ToString();
                    newRow[25] = dr["order_date_pay"].ToString();
                    newRow[26] = dr["money_collect_date"].ToString();
                    newRow[27] = dr["invoice_date"].ToString();
                    newRow[28] = dr["slave_date_close"].ToString();
                    newRow[29] = dr["note_order"].ToString();
                    newRow[30] = dr["note_admin"].ToString();
                    switch (dr["product_freight_set"].ToString())
                    {//運送方式
                        case "1":
                            newRow[31] = "常溫";
                            break;
                        case "3":
                            newRow[31] = "常溫";
                            break;
                        case "2":
                            newRow[31] = "冷凍";
                            break;
                        case "4":
                            newRow[31] = "冷凍";
                            break;
                        case "5":
                            newRow[31] = "冷藏";
                            break;
                        case "6":
                            newRow[31] = "冷藏";
                            break;
                    }
                    newRow[32] = dr["user_reg_date"].ToString();
                    newRow[33] = dr["user_birthday"].ToString();
                    newRow[34] = dr["user_username"].ToString();
                    if (dr["delivery_store"].ToString() == "1" || dr["delivery_store"].ToString() == "10")
                    {
                        switch (dr["estimated_arrival_period"].ToString())
                        {
                            case "0":
                                newRow[35] = 4;
                                break;
                            case "1":
                                newRow[35] = 1;
                                break;
                            case "2":
                                newRow[35] = 2;
                                break;
                            case "3":
                                newRow[35] = 3;
                                break;
                        }
                    }
                    switch (dr["estimated_arrival_period"].ToString())
                    {//收貨時間
                        case "0":
                            newRow[36] = "不限時";
                            break;
                        case "1":
                            newRow[36] = "12:00以前";
                            break;
                        case "2":
                            newRow[36] = "12:00-17:00";
                            break;
                        case "3":
                            newRow[36] = "17:00-20:00";
                            break;
                        default:
                            break;
                    }
                    newRow[37] = dr["item_id"].ToString();
                    switch (dr["holiday_deliver"].ToString())
                    {//假日可出貨
                        case "1":
                            newRow[38] = "是";
                            break;
                        default:
                            newRow[38] = "否";
                            break;
                    }
                    dt.Rows.Add(newRow);
                    #endregion
                }
                if (dt.Rows.Count > 0)
                {
                    string fileName = "供應商訂單匯出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dt, "");
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
            }
        }
        #endregion
        #region 返回供應商列表
        public HttpResponseBase GetVendor()
        {
            _vendorMgr = new VendorMgr(mySqlConnectionString);
            List<Vendor> stores = new List<Vendor>();
            string json = string.Empty;
            try
            {
                stores = _vendorMgr.VendorQueryList(new Vendor());

                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據

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
        #endregion
        #region 類別營養額
        public HttpResponseBase GetCategoryList()
        {
            List<CategoryQuery> stores = new List<CategoryQuery>();
            string json = string.Empty;
            int sum = 0;
            try
            {
                CategoryQuery query = new CategoryQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (string.IsNullOrEmpty(Request.Params["serchs"]))
                {
                    query.serchs = 0;
                }
                else
                {
                    query.serchs = Convert.ToInt32(Request.Params["serchs"]);
                }

                query.seldate = Convert.ToInt32(Request.Params["seldate"] ?? "0");
                query.brand_status = Convert.ToInt32(Request.Params["brand_status"] ?? "0");
                query.starttime = Convert.ToInt32(CommonFunction.GetPHPTime(Request.Params["starttime"] ?? "0"));
                query.endtime = Convert.ToInt32(CommonFunction.GetPHPTime(Request.Params["endtime"] ?? "0"));

                ICategoryImplMgr _IcategoryMgr = new CategoryMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _IcategoryMgr.GetCategoryList(query, out totalCount);
                //sum = Int32.Parse(_IcategoryMgr.GetSum(query));
                foreach (var item in stores)
                {   //因為delivertime類型是int！
                    sum += int.Parse(item.amo);
                }
                foreach (var item in stores)
                {   //因為delivertime類型是int！
                    item.amo =  int.Parse(item.amo).ToString("###,###");
                    item.sum = sum.ToString("###,###");
                }
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
        public HttpResponseBase GetCategory()
        {
            ICategoryImplMgr _IcategoryMgr = new CategoryMgr(mySqlConnectionString);
            List<CategoryQuery> stores = new List<CategoryQuery>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                stores = _IcategoryMgr.GetCategory();
                foreach (var item in stores)
                {
                    item.category_name = item.category_id + "-" + item.category_name;
                }
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
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
        #region 取消訂單通知
        #region 獲取取消訂單通知列表
        public HttpResponseBase GetOrderCancelMsgList()
        {
            List<OrderCancelMsgQuery> stores = new List<OrderCancelMsgQuery>();
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                OrderCancelMsgQuery query = new OrderCancelMsgQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                _orderCancelMsgMgr = new OrderCancelMsgMgr(mySqlConnectionString);
                stores = _orderCancelMsgMgr.Query(query, out totalCount);
                foreach (var item in stores)
                {
                    item.sorder_payment = Payment(item.order_payment.ToString());
                    item.scancel_type = Order_Cancel_Reason(item.cancel_type.ToString());
                    item.sorder_status = Status(item.order_status.ToString());
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                //listUser是准备转换的对象
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
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
        #region 對用戶提出的問題進行回覆
        [CustomHandleError]
        public HttpResponseBase Reply()
        {
            string json = string.Empty;
            string response_content = string.Empty;
            OrderCancelResponse ocr = new OrderCancelResponse();
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            int i = 0;
            string cancel_id = string.Empty; ;
            if (!string.IsNullOrEmpty(Request.Params["cancel_id"]))
            {
                cancel_id = Request.Params["cancel_id"];
                ocr.cancel_id = uint.Parse(cancel_id);
            }
            string question_email = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["question_email"]))
            {
                question_email = Request.Params["question_email"];
            }
            //獲取登錄用戶的user_email
            string user_email = (Session["caller"] as Caller).user_email;
            //獲取登錄用戶的user_id
            string user_id = (Session["caller"] as Caller).user_id.ToString();
            string res;
            try
            {
                res = Request.Params["response"].ToString().Trim();
                res = res.Replace("\n", "");
                if (res.Length>0)
                {
                    response_content = Request.Params["response"] + " ";
                    ocr.user_id = uint.Parse(user_id);
                    ocr.response_content = response_content;
                    System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                    if (addlist.Length > 0)
                    {
                        ocr.response_ipfrom = addlist[0].ToString();
                    }
                    _orderCancelMsgMgr = new OrderCancelMsgMgr(mySqlConnectionString);
                    i = _orderCancelMsgMgr.Reply(ocr);
                    bool issend = false;
                    if (i == 2)
                    {
                        MailHelper mail = new MailHelper();
                        string mail1 = _ptersrc.Getmail("TestMail");
                        if (mail1 == "gigade@gimg.com.tw")
                        {//測試使用上正式機后可變更為gigademail則是讀取會員mail
                            mail1 = question_email;
                        }
                        issend = mail.SendMailAction(mail1, "取消訂單通知信", response_content);
                    }
                    json = "{success:true,msg:\"" + " 發送郵件成功！" + "\"}";
                }
                else
                {
                    json = "{success:false,msg:\"" + "请填写回覆内容！" + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = ex.InnerException.Message;
                json = ex.InnerException.Source;
                json = "{success:false,msg:\"" + " 發送郵件失敗！" + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion

        /*取消訂單原因 */
        #region 訂單狀態列表
        /// <summary>
        /// 取消訂單原因
        /// </summary>
        /// <param name="value">數字</param>
        /// <returns></returns>
        public static string Order_Cancel_Reason(string value)
        {
            string result = string.Empty;
            switch (value)
            {
                case "1":
                    result = "買錯了";
                    break;
                case "2":
                    result = "不適合";
                    break;
                case "3":
                    result = "重複選購";
                    break;
                case "4":
                    result = "商品規格不符";
                    break;
                case "5":
                    result = "等待時間太久";
                    break;
                case "6":
                    result = "改買其他商品";
                    break;
                case "7":
                    result = "運費金額太高";
                    break;
                case "8":
                    result = "接收時間無法配合";
                    break;
                case "9":
                    result = "改用其他付款方式";
                    break;
                case "10":
                    result = "價格較貴";
                    break;
                case "11":
                    result = "其他";
                    break;
                default:
                    result = "";
                    break;

            }
            return result;

        }
        #endregion
        /*訂單狀態 */
        #region 訂單狀態列表
        /// <summary>
        /// 訂單狀態
        /// </summary>
        /// <param name="value">數字</param>
        /// <returns></returns>
        public static string Status(string value)
        {
            string result = string.Empty;
            switch (value)
            {
                case "99":
                    result = " 訂單歸檔";
                    break;
                case "92":
                    result = " 訂單換貨";
                    break;
                case "91":
                    result = " 訂單退貨";
                    break;
                case "90":
                    result = " 訂單取消";
                    break;
                case "89":
                    result = " 單一商品取消";
                    break;
                case "20":
                    result = " 訂單異常";
                    break;
                case "10":
                    result = " 等待取消";
                    break;
                case "9":
                    result = " 待取貨";
                    break;
                case "8":
                    result = " 已分配";
                    break;
                case "7":
                    result = " 已進倉";
                    break;
                case "6":
                    result = " 進倉中";
                    break;
                case "5":
                    result = " 處理中";
                    break;
                case "4":
                    result = " 已出貨";
                    break;
                case "3":
                    result = " 出貨中";
                    break;
                case "2":
                    result = " 待出貨";
                    break;
                case "1":
                    result = " 付款失敗";
                    break;
                case "0":
                    result = " 等待付款";
                    break;
                default:
                    result = "";
                    break;

            }
            return result;

        }
        #endregion
        /*支付方式 */
        #region
        /// <summary>
        /// 支付方式
        /// </summary>
        /// <param name="value">數字</param>
        /// <returns></returns>
        public static string Payment(string value)
        {
            string result = string.Empty;
            switch (value)
            {
                case "1":
                    result = "信用卡";
                    break;
                case "2":
                    result = "ATM";
                    break;
                case "3":
                    result = "藍新";
                    break;
                case "4":
                    result = "支付寶";
                    break;
                case "5":
                    result = "銀聯";
                    break;
                case "6":
                    result = "傳真刷卡";
                    break;
                case "7":
                    result = "延遲付款";
                    break;
                case "8":
                    result = "黑貓貨到付款";
                    break;
                case "9":
                    result = "現金";
                    break;
                case "10":
                    result = "中國信託信用卡";
                    break;
                case "11":
                    result = "中國信託信用卡紅利折抵(100%)";
                    break;
                case "12":
                    result = "中國信託信用卡紅利折抵(10%)";
                    break;
                case "13":
                    result = "信用卡";
                    break;
                case "14":
                    result = "中國信託信用卡紅利折抵(20%)";
                    break;
                case "15":
                    result = "外站每月請款";
                    break;
                case "16":
                    result = "台新銀行紅利折抵(50%)";
                    break;
                case "17":
                    result = "其他銀行紅利折抵";
                    break;
                default:
                    result = value;
                    break;


            }
            return result;
        }
        #endregion
        #endregion
        #region 預購單
        #region 獲取預購單信息
        public HttpResponseBase GetOrderExpectList()
        {
            List<OrderExpectDeliverQuery> stores = new List<OrderExpectDeliverQuery>();
            string json = string.Empty;
            try
            {
                OrderExpectDeliverQuery query = new OrderExpectDeliverQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                #region 日期條件
                if (!string.IsNullOrEmpty(Request.Params["seledate"]))
                {
                    if (int.Parse(Request.Params["seledate"]) == 1)
                    {
                        if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                        {
                            query.date_one = Convert.ToDateTime(Request.Params["dateOne"].ToString());
                        }
                        if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                        {
                            query.date_two = Convert.ToDateTime(Request.Params["dateTwo"].ToString());
                        }
                    }
                }
                #endregion
                #region 訂單狀態
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.query_status = Convert.ToInt32(Request.Params["status"].ToString());
                }
                #endregion
                _orderExpectDeliverMgr = new OrderExpectDeliverMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _orderExpectDeliverMgr.GetOrderExpectList(query, out totalCount);
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

        #region 預購單出貨+HttpResponseBase OrderExpectModify()
        public HttpResponseBase OrderExpectModify()
        {
            string json = "{success:false}";
            try
            {
                _orderExpectDeliverMgr = new OrderExpectDeliverMgr(mySqlConnectionString);
                OrderExpectDeliverQuery query = new OrderExpectDeliverQuery();
                uint isTryUint = 0;
                query.expect_id = Convert.ToUInt32(Request.Params["expect_id"]);


                if (uint.TryParse(Request.Params["e_status"].ToString(), out isTryUint))
                {
                    query.status = Convert.ToUInt32(Request.Params["e_status"]);
                }
                else
                {
                    query.status = 0;
                }
                if (uint.TryParse(Request.Params["deliver_id"].ToString(), out isTryUint))
                {
                    query.store = uint.Parse(Request.Params["deliver_id"]);
                }
                else
                {
                    query.store = 99;
                }
                if (!string.IsNullOrEmpty(Request.Params["code"]))
                {
                    query.code = Request.Params["code"].ToString();
                }
                else
                {
                    query.code = "";
                }
                if (!string.IsNullOrEmpty(Request.Params["stime"]))
                {
                    query.time = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Parse(Request.Params["stime"]).ToString("yyyy-MM-dd 00:00:00")));
                }
                else
                {
                    query.time = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.MinValue.ToString()));
                }
                if (!string.IsNullOrEmpty(Request.Params["note"]))
                {
                    query.note = Request.Params["note"];
                }
                else
                {
                    query.note = "";
                }
                query.updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                if (_orderExpectDeliverMgr.OrderExpectModify(query) > 0)
                {
                    json = "{success:true}";//返回json數據
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
        #region 預購單匯出
        public void ExportToExcel()
        {
            string json = string.Empty;

            OrderExpectDeliverQuery query = new OrderExpectDeliverQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["seledate"]))
                {
                    if (int.Parse(Request.Params["seledate"]) == 1)
                    {
                        if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                        {
                            query.date_one = Convert.ToDateTime(Request.Params["dateOne"].ToString());
                        }
                        if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                        {
                            query.date_two = Convert.ToDateTime(Request.Params["dateTwo"].ToString());
                        }

                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.query_status = Convert.ToInt32(Request.Params["status"].ToString());
                }
                List<OrderExpectDeliverQuery> stores = new List<OrderExpectDeliverQuery>();
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("商品編號", typeof(String));
                dtHZ.Columns.Add("商品名稱", typeof(String));
                dtHZ.Columns.Add("訂購人", typeof(String));
                dtHZ.Columns.Add("收貨人", typeof(String));
                dtHZ.Columns.Add("收貨人手機", typeof(String));
                dtHZ.Columns.Add("收貨人住址", typeof(String));
                dtHZ.Columns.Add("數量", typeof(String));
                dtHZ.Columns.Add("金額", typeof(String));
                dtHZ.Columns.Add("購物金", typeof(String));
                dtHZ.Columns.Add("小計", typeof(String));
                dtHZ.Columns.Add("預購單狀態", typeof(String));
                _orderExpectDeliverMgr = new OrderExpectDeliverMgr(mySqlConnectionString);
                stores = _orderExpectDeliverMgr.GetModel(query);
                foreach (var item in stores)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = item.item_id;
                    dr[1] = item.product_name;
                    dr[2] = item.order_name;
                    dr[3] = item.delivery_name;
                    dr[4] = item.delivery_mobile;
                    dr[5] = item.zip;
                    dr[6] = item.buy_num;
                    dr[7] = item.single_money;
                    dr[8] = item.deduct_bonus;
                    dr[9] = item.sum;
                    switch (item.status.ToString())
                    {
                        case "0": dr[10] = "未出貨";
                            break;
                        case "1": dr[10] = "已出貨";
                            break;
                        case "2": dr[10] = "異常";
                            break;
                    }
                    dtHZ.Rows.Add(dr);
                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = "預購單_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
                json = "{success:false,data:[]}";
            }
        }
        #endregion
        #endregion
        #region 品牌营业额统计
        private ChannelMgr _channel;/*賣場*/
        private VendorBrandMgr _vbrand;/*品牌*/
        static DataTable dt;
        #region 品牌营业额列表
        public HttpResponseBase GetOrderRevenueList()
        {
            string json = string.Empty;
            string brandserch = "是否单个查询";
            StringBuilder sb = new StringBuilder();//查詢訂單的條件
            StringBuilder Sbandlist = new StringBuilder();//查詢賣場和管別的條件
            OrderBrandProducesQuery oq = new OrderBrandProducesQuery();
            uint id; int cid;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Brand_Id"]))/*品牌編號*/
                {
                    oq.bid = Request.Params["Brand_Id"];
                    if (oq.bid.Substring(0, 1) != "0")
                    {
                        sb.AppendFormat("and p.brand_id in ({0})", oq.bid);
                        Sbandlist.AppendFormat("and vb.brand_id in ( {0})", oq.bid);
                    }
                }
                if (uint.TryParse(Request.Params["product_manage"], out id))
                {//獲取管理人
                    oq.product_manage = id;
                    if (id > 0)
                    {
                        sb.AppendFormat(" AND v.product_manage='{0}'", id);
                    }
                }
                if (int.TryParse(Request.Params["channel"], out cid))
                {//獲取管理人
                    oq.channel = cid;
                    if (cid > 0)
                    {
                        sb.AppendFormat(" AND om.channel='{0}'", cid);
                    }
                }
                if (uint.TryParse(Request.Params["slave_status"], out id))
                {//獲取 訂單狀態
                    oq.slave_status = id;
                    switch (id)
                    {
                        case 0:
                            sb.AppendFormat(" and os.slave_status in(0,2,4,99,5,6)");
                            break;
                        default:
                            sb.AppendFormat(" and os.slave_status={0}", id);
                            break;
                    }
                }
                if (uint.TryParse(Request.Params["order_payment"], out id))
                {//獲取 付款方式
                    oq.order_payment = id;
                    if (id > 0)
                    {
                        sb.AppendFormat(" and om.order_payment={0}", id);
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
                {
                    oq.selecttype = Request.Params["selecttype"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
                {
                    oq.searchcon = Request.Params["searchcon"].ToString();
                    if (string.IsNullOrEmpty(oq.searchcon) && oq.searchcon != "null")
                    {//查詢內容不為空就執行模糊查詢
                        switch (oq.selecttype)
                        {
                            case "1":
                                sb.AppendFormat(" AND od.product_name LIKE  '%{0}%'", oq.searchcon);
                                break;
                            case "2"://會員編號
                                sb.AppendFormat(" AND om.user_id LIKE '%{0}%'", oq.searchcon);
                                break;
                            case "3":
                                sb.AppendFormat(" om.order_name LIKE   '%{0}%'", oq.searchcon);
                                break;
                            default:
                                break;
                        }
                    }
                }
                string sqlap = sb.ToString();
                #region 日期條件
                DateTime dtime;
                if (DateTime.TryParse(Request.Params["dateOne"], out dtime))
                {
                    oq.dateOne = DateTime.Parse(dtime.ToString("yyyy-MM-dd 00:00:00"));
                    if (oq.dateOne > DateTime.MinValue)
                    {
                        sb.AppendFormat(" AND om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(oq.dateOne.ToString()));
                    }
                }
                if (DateTime.TryParse(Request.Params["dateTwo"], out dtime))
                {
                    oq.dateTwo = DateTime.Parse(dtime.ToString("yyyy-MM-dd 23:59:59"));
                    if (oq.dateTwo > DateTime.MinValue)
                    {
                        sb.AppendFormat(" AND om.order_createdate <= '{0}' ", CommonFunction.GetPHPTime(oq.dateTwo.ToString()));
                    }
                }
                #endregion
                int a = 0;
                _vbrand = new VendorBrandMgr(mySqlConnectionString);
                _Iorderbrandproduces = new OrderBrandProducesMgr(mySqlConnectionString);
                dt = _vbrand.GetBandList(Sbandlist.ToString());//查詢品牌列表
                if (string.IsNullOrEmpty(brandserch))//查询所有
                {
                    DataTable ds = _Iorderbrandproduces.GetOrderVendorRevenuebyday(sb.ToString(), brandserch);//查詢品牌列表中的營業額，之查詢每天的
                    #region 没有的品牌添加上
                    foreach (DataRow item in ds.Rows)
                    {
                        DataRow[] rows = dt.Select("brand_id='" + item["brand_id"] + "' and vendor_id='" + item["vendor_id"] + "'");
                        if (rows.Count() == 0)
                        {
                            DataRow drr = dt.NewRow();
                            drr[0] = item["brand_id"];
                            drr[1] = item["brand_name"];
                            drr[2] = item["vendor_id"];
                            drr[3] = item["vendor_name_simple"];
                            dt.Rows.Add(drr);

                        }
                    }

                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{
                    //    DataRow[] rows = ds.Select("brand_id='" + dt.Rows[i]["brand_id"] + "'");
                    //    foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，加入某個品牌的每日營業額，沒有就為初始值
                    //    {
                    //        if (string.IsNullOrEmpty(row["brand_id"].ToString()))
                    //        {
                    //            DataRow drr = dt.NewRow();
                    //            drr[0] = dt.Rows[i]["brand_id"];
                    //            drr[1] = dt.Rows[i]["brand_name"];
                    //            drr[2] = dt.Rows[i]["vendor_id"];
                    //            drr[3] = dt.Rows[i]["vendor_name_simple"];
                    //            dt.Rows.Add(drr);
                    //        }
                    //    }
                    //}
                    #endregion
                }
                string q = a.ToString();
                //去掉注释，把sbliint改成sb
                //DateTime starttime = DateTime.Parse(dateOne);//查詢的時間範圍~~開始時間
                //DateTime endtime = DateTime.Parse(dateTwo);//查詢的時間範圍~~結束時間
                TimeSpan s = oq.dateTwo - oq.dateOne;//查詢的開始日期和結束日期相差的天數
                int day = s.Days;

                #region 測試方法
                _Iorderbrandproduces = new OrderBrandProducesMgr(mySqlConnectionString);
                DataColumn addcolms;//循環添加列每月的統計
                DataRow addrow = dt.NewRow();//添加一行每日小計
                addrow[3] = "每日小計";
                int m = 0;//記錄增加了多少列，來確定每月一記的具體位置
                int M_total = 0;//每月一記

                StringBuilder html_table = new StringBuilder();//匯出html頁面
                html_table.AppendFormat(@"<div style='overflow:auto;text-align:right;width:1650px;height:550px;'><table style='border:1px;'>");
                html_table.AppendFormat("<tr><td>品牌名稱</td><td>供應商名稱</td>");

                int startclome = 4;//設求每月之初的列的下標，初始為4/。
                int stopclome = 3;//每個月結束的列的下標，初始為3/。
                #region  循環每天

                for (int d = 0; d <= day; d++)//查詢每天
                {
                    int D_bandsum = 0; //所有品牌每日的營業額
                    string daytimes = oq.dateOne.AddDays(d).ToString("yyyy-MM-dd");//循環的每一天
                    addcolms = new DataColumn(daytimes, typeof(String));
                    dt.Columns.Add(addcolms);
                    html_table.AppendFormat("<td>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", daytimes);

                    stopclome++;//加一列
                    #region 每日一記
                    string daytimeone = DateTime.Parse(daytimes).ToString("yyyy-MM-dd 00:00:00");//把每天轉換成初始從零點開始
                    string daytimetwo = DateTime.Parse(daytimes).ToString("yyyy-MM-dd 23:59:59");//每天的結束時間
                    string orderby = string.Format(" AND om.order_createdate  >= '{0}' AND om.order_createdate <= '{1}'", CommonFunction.GetPHPTime(daytimeone), CommonFunction.GetPHPTime(daytimetwo));
                    DataTable ds = _Iorderbrandproduces.GetOrderVendorRevenuebyday(sqlap + orderby, brandserch);//查詢品牌列表中的營業額，之查詢每天的


                    for (int i = 0; i < dt.Rows.Count; i++)//循環品牌列表,把每天的營業額追加進這個表裡
                    {
                        DataRow[] rows = ds.Select("brand_id='" + dt.Rows[i]["brand_id"] + "' and vendor_id='" + dt.Rows[i]["vendor_id"] + "'");//篩選出一個品牌的這個時間段的營業額
                        dt.Rows[i][daytimes] = 0;//單個品牌每日的營業額先賦初始值為0
                        foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，加入某個品牌的每日營業額，沒有就為初始值
                        {
                            if (!string.IsNullOrEmpty(row["order_createdate"].ToString()))
                            {
                                string tb_createdate = CommonFunction.GetNetTime(long.Parse(rows[0]["order_createdate"].ToString())).ToString("yyyy-MM-dd");

                                dt.Rows[i][daytimes] = int.Parse(rows[0]["subtotal"].ToString());
                                D_bandsum = D_bandsum + int.Parse(rows[0]["subtotal"].ToString());
                            }
                        }
                    }
                    #endregion
                    #region 是否添加每月小計

                    addrow[4 + d + m] = D_bandsum;
                    M_total = M_total + D_bandsum;
                    if (oq.dateOne.Month != oq.dateTwo.Month || oq.dateOne.Year != oq.dateTwo.Year)//開始時間和結束時間不是在同一個月
                    {
                        DateTime days = DateTime.Parse(daytimes);
                        int t = DateTime.DaysInMonth(days.Year, days.Month);//一個月多少天
                        if (t == days.Day || d == day)//現在這個月的月底
                        {
                            addcolms = new DataColumn(days.Month + "月小計", typeof(String));//style='float:left;width:500px'
                            dt.Columns.Add(addcolms);
                            html_table.AppendFormat("<td ><font color='red'>&nbsp;&nbsp;&nbsp;&nbsp;{0}月小計&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</font></td>", days.Month);
                            m++;
                            foreach (DataRow item in dt.Rows)
                            {
                                item[stopclome + 1] = 0;//每月總計賦初始值
                                for (int i = startclome; i <= stopclome; i++)//每個品牌的每月總計
                                {
                                    item[stopclome + 1] = int.Parse(item[stopclome + 1].ToString()) + int.Parse(item[i].ToString());
                                }
                            }
                            addrow[stopclome + 1] = M_total;
                            startclome = stopclome + 2;
                            stopclome++;
                            M_total = 0;
                        }
                    }
                    #endregion
                }
                #endregion
                dt.Rows.InsertAt(addrow, 0);//添加一行總計
                #region  品牌總計
                addcolms = new DataColumn("品牌總計", typeof(String));
                html_table.AppendFormat("<td><font color='red'>品牌總計</font></td></tr>");
                dt.Columns.Add(addcolms);
                int S_total = 0;//所有品牌的總計
                DataTable dstable = _Iorderbrandproduces.GetOrderVendorRevenuebyday(sb.ToString(), brandserch);//查詢品牌列表中的營業額
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    int Aull_bandSum = 0;//每個商品的開始時間和結束時間的總計
                    DataRow[] rows = dstable.Select("brand_id='" + dt.Rows[i]["brand_id"] + "' and vendor_id='" + dt.Rows[i]["vendor_id"] + "'");//篩選出一個品牌的這個時間段的營業額
                    foreach (DataRow row in rows)
                    {
                        if (!string.IsNullOrEmpty(row["subtotal"].ToString()))
                        {
                            Aull_bandSum = Aull_bandSum + int.Parse(row["subtotal"].ToString());
                        }
                        S_total = S_total + Aull_bandSum;
                    }
                    dt.Rows[i]["品牌總計"] = Aull_bandSum;
                }
                dt.Rows[0]["品牌總計"] = S_total;
                #endregion
                #endregion
                #region  要查詢的列表,匯成一張table
                dt.Columns.RemoveAt(2);//移除多餘的列，供應商編號
                dt.Columns.RemoveAt(0);//移除多餘的列，品牌編號
                dt.Columns[0].ColumnName = "品牌名稱";
                dt.Columns[1].ColumnName = "供應商名稱";
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    html_table.AppendFormat("<tr>");
                    for (int y = 0; y < dt.Columns.Count; y++)
                    {
                        if (dt.Columns[y].ColumnName.ToString().IndexOf("月小計") > 0 || dt.Columns[y].ColumnName.ToString() == "品牌總計")
                        {
                            html_table.AppendFormat("<td style='text-align:right'>&nbsp;&nbsp;<font color='red'>{0}</font>&nbsp;&nbsp;</td>", dt.Rows[x][y]);
                        }
                        else
                        {
                            html_table.AppendFormat("<td style='text-align:right'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", dt.Rows[x][y]);
                        }

                        if (y == dt.Columns.Count - 1)
                        {
                            html_table.AppendFormat("</tr>");
                        }
                    }
                }
                html_table.AppendFormat(@"</table></div>");
                #endregion
                string n = html_table.ToString();
                json = "{success:true,msg:\"" + html_table.ToString() + "\"}";
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
        //加購物金欄位
        public HttpResponseBase GetOrderRevenue()
        {
            string json = string.Empty;
            string brandserch = "是否单个查询";
            StringBuilder sb = new StringBuilder();//查詢訂單的條件
            StringBuilder Sbandlist = new StringBuilder();//查詢賣場和管別的條件
            OrderBrandProducesQuery oq = new OrderBrandProducesQuery();
            uint id; int cid;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Brand_Id"]))/*品牌編號*/
                {
                    oq.bid = Request.Params["Brand_Id"];
                    if (oq.bid.Substring(0, 1) != "0")
                    {
                        sb.AppendFormat("and p.brand_id in ({0})", oq.bid);
                        Sbandlist.AppendFormat("and vb.brand_id in ( {0})", oq.bid);
                    }
                }
                if (uint.TryParse(Request.Params["product_manage"], out id))
                {//獲取管理人
                    oq.product_manage = id;
                    if (id > 0)
                    {
                        sb.AppendFormat(" AND v.product_manage='{0}'", id);
                    }
                }
                if (int.TryParse(Request.Params["channel"], out cid))
                {//獲取管理人
                    oq.channel = cid;
                    if (cid > 0)
                    {
                        sb.AppendFormat(" AND om.channel='{0}'", cid);
                    }
                }
                if (uint.TryParse(Request.Params["slave_status"], out id))
                {//獲取 訂單狀態
                    oq.slave_status = id;
                    switch (id)
                    {
                        case 0:
                            sb.AppendFormat(" and os.slave_status in(0,2,4,99,5,6)");
                            break;
                        default:
                            sb.AppendFormat(" and os.slave_status={0}", id);
                            break;
                    }
                }
                if (uint.TryParse(Request.Params["order_payment"], out id))
                {//獲取 付款方式
                    oq.order_payment = id;
                    if (id > 0)
                    {
                        sb.AppendFormat(" and om.order_payment={0}", id);
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
                {
                    oq.selecttype = Request.Params["selecttype"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
                {
                    oq.searchcon = Request.Params["searchcon"].ToString();
                    if (string.IsNullOrEmpty(oq.searchcon) && oq.searchcon != "null")
                    {//查詢內容不為空就執行模糊查詢
                        switch (oq.selecttype)
                        {
                            case "1":
                                sb.AppendFormat(" AND od.product_name LIKE  '%{0}%'", oq.searchcon);
                                break;
                            case "2"://會員編號
                                sb.AppendFormat(" AND om.user_id LIKE '%{0}%'", oq.searchcon);
                                break;
                            case "3":
                                sb.AppendFormat(" om.order_name LIKE   '%{0}%'", oq.searchcon);
                                break;
                            default:
                                break;
                        }
                    }
                }
                string sqlap = sb.ToString();
                #region 日期條件
                DateTime dtime;
                if (DateTime.TryParse(Request.Params["dateOne"], out dtime))
                {
                    oq.dateOne = DateTime.Parse(dtime.ToString("yyyy-MM-dd 00:00:00"));
                    if (oq.dateOne > DateTime.MinValue)
                    {
                        sb.AppendFormat(" AND om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(oq.dateOne.ToString()));
                    }
                }
                if (DateTime.TryParse(Request.Params["dateTwo"], out dtime))
                {
                    oq.dateTwo = DateTime.Parse(dtime.ToString("yyyy-MM-dd 23:59:59"));
                    if (oq.dateTwo > DateTime.MinValue)
                    {
                        sb.AppendFormat(" AND om.order_createdate <= '{0}' ", CommonFunction.GetPHPTime(oq.dateTwo.ToString()));
                    }
                }
                #endregion
                int a = 0;
                _vbrand = new VendorBrandMgr(mySqlConnectionString);
                _Iorderbrandproduces = new OrderBrandProducesMgr(mySqlConnectionString);
                dt = _vbrand.GetBandList(Sbandlist.ToString());//查詢品牌列表
                if (string.IsNullOrEmpty(brandserch))//查询所有
                {
                    DataTable ds = _Iorderbrandproduces.GetOrderVendorRevenuebyday(sb.ToString(), brandserch);//查詢品牌列表中的營業額，之查詢每天的
                    #region 没有的品牌添加上
                    foreach (DataRow item in ds.Rows)
                    {
                        DataRow[] rows = dt.Select("brand_id='" + item["brand_id"] + "' and vendor_id='" + item["vendor_id"] + "'");
                        if (rows.Count() == 0)
                        {
                            DataRow drr = dt.NewRow();
                            drr[0] = item["brand_id"];
                            drr[1] = item["brand_name"];
                            drr[2] = item["vendor_id"];
                            drr[3] = item["vendor_name_simple"];
                            dt.Rows.Add(drr);
                        }
                    }
                    #endregion
                }
                string q = a.ToString();
                TimeSpan s = oq.dateTwo - oq.dateOne;//查詢的開始日期和結束日期相差的天數
                int day = s.Days;

                #region 測試方法
                _Iorderbrandproduces = new OrderBrandProducesMgr(mySqlConnectionString);
                DataColumn addcolms;//循環添加列每月的統計
                DataRow addrow = dt.NewRow();//添加一行每日小計
                addrow[3] = "每日小計";
                int m = 0;//記錄增加了多少列，來確定每月一記的具體位置
                int M_total = 0;//每月一記

                StringBuilder html_table = new StringBuilder();//匯出html頁面
                html_table.AppendFormat(@"<div style='overflow:auto;text-align:right;width:1650px;height:550px;'><table style='border:1px;'>");
                html_table.AppendFormat("<tr><td>品牌名稱</td><td>供應商名稱</td>");

                int startclome = 4;//設求每月之初的列的下標，初始為4/。
                int stopclome = 3;//每個月結束的列的下標，初始為3/。
                #region  循環每天

                for (int d = 0; d <= day; d++)//查詢每天
                {
                    int D_bandsum = 0; //所有品牌每日的營業額
                    string daytimes = oq.dateOne.AddDays(d).ToString("yyyy-MM-dd");//循環的每一天
                    addcolms = new DataColumn(daytimes, typeof(String));
                    dt.Columns.Add(addcolms);
                    html_table.AppendFormat("<td>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", daytimes);

                    stopclome++;//加一列
                    #region 每日一記
                    string daytimeone = DateTime.Parse(daytimes).ToString("yyyy-MM-dd 00:00:00");//把每天轉換成初始從零點開始
                    string daytimetwo = DateTime.Parse(daytimes).ToString("yyyy-MM-dd 23:59:59");//每天的結束時間
                    string orderby = string.Format(" AND om.order_createdate  >= '{0}' AND om.order_createdate <= '{1}'", CommonFunction.GetPHPTime(daytimeone), CommonFunction.GetPHPTime(daytimetwo));
                    DataTable ds = _Iorderbrandproduces.GetOrderVendorRevenuebyday(sqlap + orderby, brandserch);//查詢品牌列表中的營業額，之查詢每天的


                    for (int i = 0; i < dt.Rows.Count; i++)//循環品牌列表,把每天的營業額追加進這個表裡
                    {
                        DataRow[] rows = ds.Select("brand_id='" + dt.Rows[i]["brand_id"] + "' and vendor_id='" + dt.Rows[i]["vendor_id"] + "'");//篩選出一個品牌的這個時間段的營業額
                        dt.Rows[i][daytimes] = 0;//單個品牌每日的營業額先賦初始值為0
                        foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，加入某個品牌的每日營業額，沒有就為初始值
                        {
                            if (!string.IsNullOrEmpty(row["order_createdate"].ToString()))
                            {
                                string tb_createdate = CommonFunction.GetNetTime(long.Parse(rows[0]["order_createdate"].ToString())).ToString("yyyy-MM-dd");

                                dt.Rows[i][daytimes] = int.Parse(rows[0]["subtotal"].ToString());
                                D_bandsum = D_bandsum + int.Parse(rows[0]["subtotal"].ToString());
                            }
                        }
                    }
                    #endregion
                    #region 是否添加每月小計

                    addrow[4 + d + m] = D_bandsum;
                    M_total = M_total + D_bandsum;
                    if (oq.dateOne.Month != oq.dateTwo.Month || oq.dateOne.Year != oq.dateTwo.Year)//開始時間和結束時間不是在同一個月
                    {
                        DateTime days = DateTime.Parse(daytimes);
                        int t = DateTime.DaysInMonth(days.Year, days.Month);//一個月多少天
                        if (t == days.Day || d == day)//現在這個月的月底
                        {
                            addcolms = new DataColumn(days.Month + "月小計", typeof(String));//style='float:left;width:500px'
                            dt.Columns.Add(addcolms);
                            html_table.AppendFormat("<td ><font color='red'>&nbsp;&nbsp;&nbsp;&nbsp;{0}月小計&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</font></td>", days.Month);
                            m++;
                            foreach (DataRow item in dt.Rows)
                            {
                                item[stopclome + 1] = 0;//每月總計賦初始值
                                for (int i = startclome; i <= stopclome; i++)//每個品牌的每月總計
                                {
                                    item[stopclome + 1] = int.Parse(item[stopclome + 1].ToString()) + int.Parse(item[i].ToString());
                                }
                            }
                            addrow[stopclome + 1] = M_total;
                            startclome = stopclome + 2;
                            stopclome++;
                            M_total = 0;
                        }
                    }
                    #endregion
                }
                #endregion
                dt.Rows.InsertAt(addrow, 0);//添加一行總計
                #region  品牌總計
                addcolms = new DataColumn("品牌總計", typeof(String));
                html_table.AppendFormat("<td><font color='red'>品牌總計</font></td></tr>");
                dt.Columns.Add(addcolms);
                int S_total = 0;//所有品牌的總計
                DataTable dstable = _Iorderbrandproduces.GetOrderVendorRevenuebyday(sb.ToString(), brandserch);//查詢品牌列表中的營業額
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    int Aull_bandSum = 0;//每個商品的開始時間和結束時間的總計
                    DataRow[] rows = dstable.Select("brand_id='" + dt.Rows[i]["brand_id"] + "' and vendor_id='" + dt.Rows[i]["vendor_id"] + "'");//篩選出一個品牌的這個時間段的營業額
                    foreach (DataRow row in rows)
                    {
                        if (!string.IsNullOrEmpty(row["subtotal"].ToString()))
                        {
                            Aull_bandSum = Aull_bandSum + int.Parse(row["subtotal"].ToString());
                        }
                        S_total = S_total + Aull_bandSum;
                    }
                    dt.Rows[i]["品牌總計"] = Aull_bandSum;
                }
                dt.Rows[0]["品牌總計"] = S_total;
                #endregion
                #endregion
                #region  要查詢的列表,匯成一張table
                dt.Columns.RemoveAt(2);//移除多餘的列，供應商編號
                dt.Columns.RemoveAt(0);//移除多餘的列，品牌編號
                dt.Columns[0].ColumnName = "品牌名稱";
                dt.Columns[1].ColumnName = "供應商名稱";
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    html_table.AppendFormat("<tr>");
                    for (int y = 0; y < dt.Columns.Count; y++)
                    {
                        if (dt.Columns[y].ColumnName.ToString().IndexOf("月小計") > 0 || dt.Columns[y].ColumnName.ToString() == "品牌總計")
                        {
                            html_table.AppendFormat("<td style='text-align:right'>&nbsp;&nbsp;<font color='red'>{0}</font>&nbsp;&nbsp;</td>", dt.Rows[x][y]);
                        }
                        else
                        {
                            html_table.AppendFormat("<td style='text-align:right'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", dt.Rows[x][y]);
                        }

                        if (y == dt.Columns.Count - 1)
                        {
                            html_table.AppendFormat("</tr>");
                        }
                    }
                }

                html_table.AppendFormat(@"</table></div>");
                #endregion
                string n = html_table.ToString();
                json = "{success:true,msg:\"" + html_table.ToString() + "\"}";
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
        #region 賣場列表
        public HttpResponseBase GetChannel()/*賣場列表*/
        {
            _channel = new ChannelMgr(mySqlConnectionString);
            List<Channel> stores = new List<Channel>();
            string json = string.Empty;
            try
            {
                stores = _channel.QueryList();
                //Channel channel = new Channel();
                //channel.channel_name_simple = "全部";
                //stores.Insert(0, channel);
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
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
        public HttpResponseBase GetVendorBand()/*品牌列表*/
        {
            VendorBrand vb = new VendorBrand();
            _vbrand = new VendorBrandMgr(mySqlConnectionString);
            List<VendorBrand> stores = new List<VendorBrand>();
            string json = string.Empty;
            try
            {
                DataTable dt = _vbrand.GetBandList("");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    vb = new VendorBrand();
                    vb.Brand_Id = uint.Parse(dt.Rows[i]["brand_id"].ToString());
                    vb.Brand_Name = dt.Rows[i]["brand_name"].ToString();
                    stores.Add(vb);
                }
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據

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
        #region 品牌營業額匯出excel
        public void Export()
        {
            string newExcelName = string.Empty;
            if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
            }
            try
            {
                string[] colname = new string[dt.Columns.Count];
                string filename = "order_vendor_revenue.csv";
                newExcelName = Server.MapPath(excelPath) + filename;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    colname[i] = dt.Columns[i].ColumnName;
                }
                if (System.IO.File.Exists(newExcelName))
                {
                    System.IO.File.Delete(newExcelName);
                }
                //CsvHelper.ExportDataTableToCsv(dt, newExcelName, colname, true);
                if (dt.Rows.Count > 0)
                {
                    string fileName = "品牌營業額_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dt, "品牌營業額_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
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
            }
        }

        #endregion
        #endregion

        #region 重寫品牌營業額統計

        public HttpResponseBase GetNewOrderRevenueList()
        {
            string json = string.Empty;
            try
            {
                OrderDetailQuery query = new OrderDetailQuery();
                #region 前置查詢條件
                if (!string.IsNullOrEmpty(Request.Params["Brand_Id"]))/*品牌編號*/
                {
                    query.Brand_Id_In = Request.Params["Brand_Id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["product_manage"]))//管理人員 
                {//獲取管理人
                    query.product_manage = int.Parse(Request.Params["product_manage"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                {
                    query.time_start = CommonFunction.GetPHPTime(DateTime.Parse(Request.Params["dateOne"]).ToString("yyyy/MM/dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                {
                    query.time_end = CommonFunction.GetPHPTime(DateTime.Parse(Request.Params["dateTwo"]).ToString("yyyy/MM/dd 23:59:59"));
                }
                long start = query.time_start;//開始時間
                long end = query.time_end;//結束時間
                if (!string.IsNullOrEmpty(Request.Params["Channel_Id"]))//賣場
                {
                    query.channel = int.Parse(Request.Params["Channel_Id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["slave_status"]))//訂單狀態
                {
                    query.Status = int.Parse(Request.Params["slave_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["order_payment"]))//付款方式
                {
                    query.order_payment = int.Parse(Request.Params["order_payment"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))//關鍵字查詢類型
                {
                    query.select_type = int.Parse(Request.Params["selecttype"]);
                    if (!string.IsNullOrEmpty(Request.Params["searchcon"]))//關鍵字查詢內容
                    {
                        query.select_con = Request.Params["searchcon"];
                    }
                }


                #endregion
                VendorBrandQuery Vendorbrandquery = new VendorBrandQuery();
                Dictionary<uint, VendorBrandQuery> brands = new Dictionary<uint, VendorBrandQuery>();///字典----儲存供應商編號，供應商名稱和錧別，錧別編號的信息
                Dictionary<uint, Dictionary<string, Dictionary<string, uint>>> brandDailyTotal = new Dictionary<uint, Dictionary<string, Dictionary<string, uint>>>();///字典，儲存錧別，每天的計算和每天的購物金
                ///////////////brand_id//////////daysum,dayduct/////日期///值
                List<VendorBrandQuery> aDB_Brand_Select = new List<VendorBrandQuery>();
                _vbrand = new VendorBrandMgr(mySqlConnectionString);
                aDB_Brand_Select = _vbrand.GetBandList(Vendorbrandquery);//把錧別和供應商信息保存到字典里
                for (int i = 0; i < aDB_Brand_Select.Count; i++)
                {
                    if (!brands.Keys.Contains(aDB_Brand_Select[i].Brand_Id))
                    {
                        brands.Add(aDB_Brand_Select[i].Brand_Id, aDB_Brand_Select[i]);
                    }
                }

                bool CrossMonth = CommonFunction.GetNetTime(start).Month == CommonFunction.GetNetTime(end).Month ? false : true;
                string timelong = "";
                while (start <= end)//時間格式化，用來保存每天的小計
                {
                    timelong += CommonFunction.GetNetTime(start).ToString("yyyy/MM/dd") + ",";
                    start += 86400;
                }
                timelong = timelong.Substring(0, timelong.LastIndexOf(","));
                string[] times = timelong.Split(',');
                Dictionary<string, uint> timetro = new Dictionary<string, uint>();
                for (int i = 0; i < times.Count(); i++)//字典加上日期時間
                {
                    timetro.Add(times[i], 0);
                }
                Dictionary<string, Dictionary<string, uint>> daysum_deduct = new Dictionary<string, Dictionary<string, uint>>();//保存每個商品每日的小計和購物金
                daysum_deduct.Add("daysum", timetro);//每日小計
                daysum_deduct.Add("daydeduct", timetro);//每日小計


                _orderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                List<OrderDetailQuery> ordertailList = new List<OrderDetailQuery>();
                ordertailList = _orderDetailMgr.GetOrderDetailList(query);//通過查詢條件獲取的數據
                //if (ordertailList.Count == 0)
                //{
                //    json = "{success:true,msg:\"" + "<div style='overflow:auto;text-align:center;width:1650px;height:550px;><p tyle='text-align:center'>~~暫無數據~~</p></div>" + "\"}";
                //}
                //else
                //{
                 string[] quanxuan = query.Brand_Id_In.Split(',');
                    foreach (uint item in brands.Keys)
                    {
                        if (!brandDailyTotal.Keys.Contains(item))
                        {
                            daysum_deduct = new Dictionary<string, Dictionary<string, uint>>();
                            timetro = new Dictionary<string, uint>();
                            for (int a = 0; a < times.Count(); a++)//字典加上日期時間
                            {
                                timetro.Add(times[a], 0);
                            }
                            daysum_deduct.Add("daysum", timetro);//每日小計
                            timetro = new Dictionary<string, uint>();

                            for (int a = 0; a < times.Count(); a++)//字典加上日期時間
                            {
                                timetro.Add(times[a], 0);
                            }
                            daysum_deduct.Add("daydeduct", timetro);//購物金
                            brandDailyTotal.Add(item, daysum_deduct);
                        }
                    }
                    for (int i = 0; i < ordertailList.Count; i++)
                    {
                        ordertailList[i].subtotal = ordertailList[i].Single_Money * ordertailList[i].Buy_Num;
                        ordertailList[i].cost = (ordertailList[i].Event_Cost != 0 && ordertailList[i].Single_Cost != ordertailList[i].Single_Money) ? ordertailList[i].Event_Cost : ordertailList[i].Single_Cost;
                        if (!brands.Keys.Contains(ordertailList[i].Brand_Id))
                        {
                            VendorBrandQuery brand = new VendorBrandQuery();
                            brand.Vendor_Id = ordertailList[i].Vendor_Id;
                            brand.Brand_Name = ordertailList[i].Brand_Name;
                            brand.Brand_Id = ordertailList[i].Brand_Id;
                            brand.vendor_name_simple = ordertailList[i].Vendor_Name_Simple;
                            brands.Add(ordertailList[i].Brand_Id, brand);
                        }
                        if (!brandDailyTotal.Keys.Contains(ordertailList[i].Brand_Id))
                        {
                            daysum_deduct = new Dictionary<string, Dictionary<string, uint>>();
                            timetro = new Dictionary<string, uint>();
                            for (int a = 0; a < times.Count(); a++)//字典加上日期時間
                            {
                                timetro.Add(times[a], 0);
                            }
                            daysum_deduct.Add("daysum", timetro);//每日小計
                            timetro = new Dictionary<string, uint>();

                            for (int a = 0; a < times.Count(); a++)//字典加上日期時間
                            {
                                timetro.Add(times[a], 0);
                            }
                            daysum_deduct.Add("daydeduct", timetro);//購物金

                            brandDailyTotal.Add(ordertailList[i].Brand_Id, daysum_deduct);
                        }
                        string time = CommonFunction.GetNetTime(ordertailList[i].Order_Createdate).ToString("yyyy/MM/dd");
                        brandDailyTotal[ordertailList[i].Brand_Id]["daysum"][time] += ordertailList[i].subtotal;//每個商品的小計
                        brandDailyTotal[ordertailList[i].Brand_Id]["daydeduct"][time] += ordertailList[i].Deduct_Bonus;//每個商品的購物金小計
                    }
                    Dictionary<string, uint> daysum_allbrand = new Dictionary<string, uint>();//所有品牌的每日小計
                    Dictionary<string, uint> deductsum_allbrand = new Dictionary<string, uint>();//所有品牌的每日購物金
                    foreach (uint key in brandDailyTotal.Keys)
                    {
                        foreach (string time in brandDailyTotal[key]["daysum"].Keys)//循環每個品牌的每日小計，計算到所有品牌的每日小計
                        {
                            if (!daysum_allbrand.Keys.Contains(time))
                            {
                                daysum_allbrand.Add(time, brandDailyTotal[key]["daysum"][time]);
                            }
                            else
                            {
                                daysum_allbrand[time] += brandDailyTotal[key]["daysum"][time];
                            }
                        }
                        foreach (string time in brandDailyTotal[key]["daydeduct"].Keys)//循環每個品牌的每日購物金小計，計算到所有品牌的每日小計
                        {
                            if (!deductsum_allbrand.Keys.Contains(time))
                            {
                                deductsum_allbrand.Add(time, brandDailyTotal[key]["daydeduct"][time]);
                            }
                            else
                            {
                                deductsum_allbrand[time] += brandDailyTotal[key]["daydeduct"][time];
                            }
                        }

                    }
                    StringBuilder html_table = new StringBuilder();//匯出html頁面
                    html_table.AppendFormat(@"<div style='overflow:auto;text-align:right;width:1590px;height:580px;'><table style='border:0px;'>");

                    html_table.AppendFormat("<thead style='text-align:center;border-bottom: 1px solid #ccc;color: #000;'><tr><td style='border-bottom: 1px solid #ccc;color: #000;line-height: 1.2em;white-space: nowrap;font-size: 12px; text-align: center; text-align: center;'>&nbsp;&nbsp;品牌名稱&nbsp;&nbsp;</td>");
                    html_table.AppendFormat("<td style='border-bottom: 1px solid #ccc;color: #000;line-height: 1.2em;white-space: nowrap;font-size: 12px; text-align: center; text-align: center;'>&nbsp;&nbsp;供應商名稱&nbsp;&nbsp;</td>");
                    string ym_last = "";
                    foreach (string it in timetro.Keys)
                    {
                        string ym = it.Substring(0, 7);
                        if (string.IsNullOrEmpty(ym_last))
                            ym_last = ym;
                        if (!ym_last.Equals(ym))
                        {
                            html_table.AppendFormat("<td style= 'color: #c00;border-bottom: 1px solid #ccc;line-height: 1.2em;white-space: nowrap;font-size: 12px; text-align: center;'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", ym_last.Split('/')[1] + "月小計");
                            ym_last = ym;
                        }
                        html_table.AppendFormat("<td style='color: #000;border-bottom: 1px solid #ccc; text-align: center;'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", it);
                    }
                    if (CrossMonth)
                    {
                        html_table.AppendFormat("<td style= 'color: #c00;border-bottom: 1px solid #ccc;line-height: 1.2em;white-space: nowrap;font-size: 12px; text-align: center;'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", ym_last.Split('/')[1] + "月小計");
                    }
                    html_table.AppendFormat("<td style= 'color: #c00;border-bottom: 1px solid #ccc;line-height: 1.2em;white-space: nowrap;font-size: 12px;  text-align: center;'>&nbsp;&nbsp;品牌總計&nbsp;&nbsp;</td></tr>");






                    html_table.AppendFormat("<tr><td colspan='2' style= 'text-align: right;border-bottom: 1px solid #ccc;color: #000;'>&nbsp;&nbsp;每日小計&nbsp;&nbsp;</td>");
                    ym_last = "";
                    uint sum_monthly = 0;
                    uint sum_year = 0;
                    foreach (string it in daysum_allbrand.Keys)
                    {
                        string ym = it.Substring(0, 7);
                        if (string.IsNullOrEmpty(ym_last))
                            ym_last = ym;
                        if (!ym_last.Equals(ym))
                        {
                            html_table.AppendFormat("<td style= 'border-bottom: 1px solid #ccc; text-align: right;color: #c00; '>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", sum_monthly);
                            ym_last = ym;
                            sum_monthly = 0;
                        }
                        sum_monthly += daysum_allbrand[it];
                        sum_year += daysum_allbrand[it];
                        html_table.AppendFormat("<td style='text-align:right;border-bottom: 1px solid #ccc; '>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", daysum_allbrand[it]);

                    }
                    if (CrossMonth)
                    {
                        html_table.AppendFormat("<td style= 'text-align:right;color: #c00;border-bottom: 1px solid #ccc; '>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", sum_monthly);
                    }
                    html_table.AppendFormat("<td style= 'text-align:right;color: #c00;border-bottom: 1px solid #ccc; '>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td></tr></thead>", sum_year);
                    html_table.AppendFormat("<tbody>");

                    foreach (uint it in brandDailyTotal.Keys)
                    {

                        ym_last = "";
                        sum_monthly = 0;
                        sum_year = 0;
                        if (brands.Keys.Contains(it))
                        {
                            html_table.AppendFormat("<tr><td style=' white-space: nowrap;text-align:left;line-height: 1.2em;font-size: 12px;'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td><td style='text-align:left; white-space: nowrap;line-height: 1.2em;font-size: 12px;'>&nbsp;&nbsp;{1}&nbsp;&nbsp;</td>", brands[it].Brand_Name, brands[it].vendor_name_simple);
                        }
                        else
                        {
                            html_table.AppendFormat("<tr><td>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td><td >&nbsp;&nbsp;{1}&nbsp;&nbsp;</td>", " ", " ");
                        }
                        foreach (string ite in brandDailyTotal[it]["daysum"].Keys)
                        {
                            string ym = ite.Substring(0, 7);
                            if (string.IsNullOrEmpty(ym_last))
                                ym_last = ym;
                            if (!ym_last.Equals(ym))
                            {
                                html_table.AppendFormat("<td style= 'text-align:right;color: #c00; white-space: nowrap;line-height: 1.2em;font-size: 12px;'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", sum_monthly);
                                ym_last = ym;
                                sum_monthly = 0;
                            }
                            sum_monthly += brandDailyTotal[it]["daysum"][ite];
                            sum_year += brandDailyTotal[it]["daysum"][ite];
                            html_table.AppendFormat("<td style='text-align:right;white-space: nowrap;line-height: 1.2em;font-size: 12px;'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", brandDailyTotal[it]["daysum"][ite]);
                        }
                        if (CrossMonth)
                        {
                            html_table.AppendFormat("<td style= 'text-align:right;color: #c00; white-space: nowrap;line-height: 1.2em;font-size: 12px;'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td>", sum_monthly);
                        }
                        html_table.AppendFormat("<td style= 'text-align:right;color: #c00; white-space: nowrap;line-height: 1.2em;font-size: 12px;'>&nbsp;&nbsp;{0}&nbsp;&nbsp;</td> </tr>", sum_year);



                    }
                    html_table.AppendFormat("</tbody></table>");
                    json = "{success:true,msg:\"" + html_table.ToString() + "\"}";
                }


           // }
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
        /// 品牌營業額統計匯出
        /// </summary>
        /// <returns></returns>
        public void GetNewOrderRevenueExprot()
        {
            string json = string.Empty;
            try
            {
                OrderDetailQuery query = new OrderDetailQuery();

                #region 前置查詢條件
                if (!string.IsNullOrEmpty(Request.Params["Brand_Id"]))/*品牌編號*/
                {
                    query.Brand_Id_In = Request.Params["Brand_Id"];

                }
                if (!string.IsNullOrEmpty(Request.Params["product_manage"]) && Request.Params["product_manage"] != "null")//管理人員 
                {//獲取管理人
                    query.product_manage = int.Parse(Request.Params["product_manage"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
                {
                    query.time_start = CommonFunction.GetPHPTime(DateTime.Parse(Request.Params["dateOne"]).ToString("yyyy/MM/dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                {
                    query.time_end = CommonFunction.GetPHPTime(DateTime.Parse(Request.Params["dateTwo"]).ToString("yyyy/MM/dd 23:59:59"));
                }
                long start = query.time_start;//開始時間
                long end = query.time_end;//結束時間
                if (!string.IsNullOrEmpty(Request.Params["Channel_Id"]) && Request.Params["Channel_Id"] != "null")//賣場
                {
                    query.channel = int.Parse(Request.Params["Channel_Id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["slave_status"]) && Request.Params["slave_status"]!="null")//訂單狀態
                {
                    query.Status = int.Parse(Request.Params["slave_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["order_payment"]) && Request.Params["order_payment"] != "null")//付款方式
                {
                    query.order_payment = int.Parse(Request.Params["order_payment"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))//關鍵字查詢類型
                {
                    query.select_type = int.Parse(Request.Params["selecttype"]);
                    if (!string.IsNullOrEmpty(Request.Params["searchcon"]))//關鍵字查詢內容
                    {
                        query.select_con = Request.Params["searchcon"];
                    }
                }


                #endregion
                VendorBrandQuery Vendorbrandquery = new VendorBrandQuery();
                Dictionary<uint, VendorBrandQuery> brands = new Dictionary<uint, VendorBrandQuery>();///字典----儲存供應商編號，供應商名稱和錧別，錧別編號的信息
                Dictionary<uint, Dictionary<string, Dictionary<string, uint>>> brandDailyTotal = new Dictionary<uint, Dictionary<string, Dictionary<string, uint>>>();///字典，儲存錧別，每天的計算和每天的購物金
                ///////////////brand_id//////////daysum,dayduct/////日期///值
                List<VendorBrandQuery> aDB_Brand_Select = new List<VendorBrandQuery>();
                _vbrand = new VendorBrandMgr(mySqlConnectionString);
                aDB_Brand_Select = _vbrand.GetBandList(Vendorbrandquery);//把錧別和供應商信息保存到字典里
                for (int i = 0; i < aDB_Brand_Select.Count; i++)
                {
                    if (!brands.Keys.Contains(aDB_Brand_Select[i].Brand_Id))
                    {
                        brands.Add(aDB_Brand_Select[i].Brand_Id, aDB_Brand_Select[i]);
                    }
                }

                bool CrossMonth = CommonFunction.GetNetTime(start).Month == CommonFunction.GetNetTime(end).Month ? false : true;
                string timelong = "";
                while (start <= end)//時間格式化，用來保存每天的小計
                {
                    timelong += CommonFunction.GetNetTime(start).ToString("yyyy/MM/dd") + ",";
                    start += 86400;
                }
                timelong = timelong.Substring(0, timelong.LastIndexOf(","));
                string[] times = timelong.Split(',');
                Dictionary<string, uint> timetro = new Dictionary<string, uint>();
                for (int i = 0; i < times.Count(); i++)//字典加上日期時間
                {
                    timetro.Add(times[i], 0);
                }
                Dictionary<string, Dictionary<string, uint>> daysum_deduct = new Dictionary<string, Dictionary<string, uint>>();//保存每個商品每日的小計和購物金
                daysum_deduct.Add("daysum", timetro);//每日小計
                daysum_deduct.Add("daydeduct", timetro);//每日小計


                _orderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                List<OrderDetailQuery> ordertailList = new List<OrderDetailQuery>();
                DataTable _dt = new DataTable();
                ordertailList = _orderDetailMgr.GetOrderDetailList(query);//通過查詢條件獲取的數據
                string[] quanxuan = query.Brand_Id_In.Split(',');
                foreach (uint item in brands.Keys)
                {
                    if (!brandDailyTotal.Keys.Contains(item))
                    {
                        daysum_deduct = new Dictionary<string, Dictionary<string, uint>>();
                        timetro = new Dictionary<string, uint>();
                        for (int a = 0; a < times.Count(); a++)//字典加上日期時間
                        {
                            timetro.Add(times[a], 0);
                        }
                        daysum_deduct.Add("daysum", timetro);//每日小計
                        timetro = new Dictionary<string, uint>();

                        for (int a = 0; a < times.Count(); a++)//字典加上日期時間
                        {
                            timetro.Add(times[a], 0);
                        }
                        daysum_deduct.Add("daydeduct", timetro);//購物金
                        brandDailyTotal.Add(item, daysum_deduct);
                    }
                }
                
                //if (ordertailList.Count > 0)
                //{
                    for (int i = 0; i < ordertailList.Count; i++)
                    {
                        ordertailList[i].subtotal = ordertailList[i].Single_Money * ordertailList[i].Buy_Num;
                        ordertailList[i].cost = (ordertailList[i].Event_Cost != 0 && ordertailList[i].Single_Cost != ordertailList[i].Single_Money) ? ordertailList[i].Event_Cost : ordertailList[i].Single_Cost;
                        if (!brands.Keys.Contains(ordertailList[i].Brand_Id))
                        {
                            VendorBrandQuery brand = new VendorBrandQuery();
                            brand.Vendor_Id = ordertailList[i].Vendor_Id;
                            brand.Brand_Name = ordertailList[i].Brand_Name;
                            brand.Brand_Id = ordertailList[i].Brand_Id;
                            brand.vendor_name_simple = ordertailList[i].Vendor_Name_Simple;
                            brands.Add(ordertailList[i].Brand_Id, brand);
                        }
                        if (!brandDailyTotal.Keys.Contains(ordertailList[i].Brand_Id))
                        {
                            daysum_deduct = new Dictionary<string, Dictionary<string, uint>>();
                            timetro = new Dictionary<string, uint>();
                            for (int a = 0; a < times.Count(); a++)//字典加上日期時間
                            {
                                timetro.Add(times[a], 0);
                            }
                            daysum_deduct.Add("daysum", timetro);//每日小計
                            timetro = new Dictionary<string, uint>();

                            for (int a = 0; a < times.Count(); a++)//字典加上日期時間
                            {
                                timetro.Add(times[a], 0);
                            }
                            daysum_deduct.Add("daydeduct", timetro);//購物金

                            brandDailyTotal.Add(ordertailList[i].Brand_Id, daysum_deduct);
                        }
                        string time = CommonFunction.GetNetTime(ordertailList[i].Order_Createdate).ToString("yyyy/MM/dd");
                        brandDailyTotal[ordertailList[i].Brand_Id]["daysum"][time] += ordertailList[i].subtotal;//每個商品的小計
                        brandDailyTotal[ordertailList[i].Brand_Id]["daydeduct"][time] += ordertailList[i].Deduct_Bonus;//每個商品的購物金小計
                    }
                    Dictionary<string, uint> daysum_allbrand = new Dictionary<string, uint>();//所有品牌的每日小計
                    Dictionary<string, uint> deductsum_allbrand = new Dictionary<string, uint>();//所有品牌的每日購物金
                    foreach (uint key in brandDailyTotal.Keys)
                    {
                        foreach (string time in brandDailyTotal[key]["daysum"].Keys)//循環每個品牌的每日小計，計算到所有品牌的每日小計
                        {
                            if (!daysum_allbrand.Keys.Contains(time))
                            {
                                daysum_allbrand.Add(time, brandDailyTotal[key]["daysum"][time]);
                            }
                            else
                            {
                                daysum_allbrand[time] += brandDailyTotal[key]["daysum"][time];
                            }
                        }
                        foreach (string time in brandDailyTotal[key]["daydeduct"].Keys)//循環每個品牌的每日購物金小計，計算到所有品牌的每日小計
                        {
                            if (!deductsum_allbrand.Keys.Contains(time))
                            {
                                deductsum_allbrand.Add(time, brandDailyTotal[key]["daydeduct"][time]);
                            }
                            else
                            {
                                deductsum_allbrand[time] += brandDailyTotal[key]["daydeduct"][time];
                            }
                        }

                    }

                    //
                    _dt.Columns.Add("品牌名稱", typeof(String));
                    _dt.Columns.Add("供應商名稱", typeof(String));


                    string ym_last = "";
                    foreach (string it in timetro.Keys)
                    {
                        string ym = it.Substring(0, 7);
                        if (string.IsNullOrEmpty(ym_last))
                            ym_last = ym;
                        if (!ym_last.Equals(ym))
                        {
                            _dt.Columns.Add(ym_last.Split('/')[1] + "月小計", typeof(String));
                            _dt.Columns.Add("購物金(" + ym_last.Split('/')[0] + "/" + ym_last.Split('/')[1] + "月)", typeof(String));
                            _dt.Columns.Add("扣除購物金(" + ym_last.Split('/')[0] + "/" + ym_last.Split('/')[1] + "月)", typeof(String));
                            ym_last = ym;
                        }
                        _dt.Columns.Add(it, typeof(String));
                        _dt.Columns.Add("購物金(" + it + ")", typeof(String));
                        _dt.Columns.Add("扣除購物金(" + it + ")", typeof(String));
                    }
                    if (CrossMonth)
                    {
                        _dt.Columns.Add(ym_last.Split('/')[1] + "月小計", typeof(String));
                        _dt.Columns.Add("購物金(" + ym_last.Split('/')[0] + "/" + ym_last.Split('/')[1] + "月)", typeof(String));
                        _dt.Columns.Add("扣除購物金(" + ym_last.Split('/')[0] + "/" + ym_last.Split('/')[1] + "月)", typeof(String));
                    }
                    _dt.Columns.Add("品牌總計", typeof(String));
                    _dt.Columns.Add(" 購物金總計", typeof(String));
                    _dt.Columns.Add(" 扣除購物金總計", typeof(String));

                    DataRow addrow = _dt.NewRow();//添加一行
                    addrow[0] = " ";
                    addrow[1] = "每日小計";
                    int r = 2;
                    ym_last = "";
                    uint sum_monthly = 0;//每月小計
                    uint deduct_monthly = 0;//每月購物金小計
                    uint sum_year = 0;//總計
                    uint deduct_year = 0;//購物金總計
                    foreach (string it in daysum_allbrand.Keys)
                    {
                        string ym = it.Substring(0, 7);
                        if (string.IsNullOrEmpty(ym_last))
                            ym_last = ym;
                        if (!ym_last.Equals(ym))
                        {
                            addrow[r] = sum_monthly;
                            r++;
                            addrow[r] = deduct_monthly;
                            r++;
                            addrow[r] = sum_monthly - deduct_monthly;
                            r++;
                            ym_last = ym;
                            sum_monthly = 0;
                            deduct_monthly = 0;
                        }
                        sum_monthly += daysum_allbrand[it];
                        deduct_monthly += deductsum_allbrand[it];
                        sum_year += daysum_allbrand[it];
                        deduct_year += deductsum_allbrand[it];
                        addrow[r] = daysum_allbrand[it];
                        r++;
                        addrow[r] = deductsum_allbrand[it];
                        r++;
                        addrow[r] = daysum_allbrand[it] - deductsum_allbrand[it];
                        r++;

                    }
                    if (CrossMonth)
                    {
                        addrow[r] = sum_monthly;
                        r++;
                        addrow[r] = deduct_monthly;
                        r++;
                        addrow[r] = sum_monthly - deduct_monthly;
                        r++;
                    }
                    addrow[r] = sum_year;
                    r++;
                    addrow[r] = deduct_year;
                    r++;
                    addrow[r] = sum_year - deduct_year;
                    _dt.Rows.Add(addrow);

                    foreach (uint it in brandDailyTotal.Keys)
                    {
                        ym_last = "";
                        sum_monthly = 0;
                        deduct_monthly = 0;
                        sum_year = 0;
                        deduct_year = 0;
                        r = 0;
                        addrow = _dt.NewRow();
                        if (brands.Keys.Contains(it))
                        {
                            addrow[r] = brands[it].Brand_Name;
                            r++;
                            addrow[r] = brands[it].vendor_name_simple;
                            r++;
                        }
                        else
                        {
                            addrow[r] = " ";
                            r++;
                            addrow[r] = " ";
                            r++;
                        }
                        foreach (string ite in brandDailyTotal[it]["daysum"].Keys)
                        {
                            string ym = ite.Substring(0, 7);
                            if (string.IsNullOrEmpty(ym_last))
                                ym_last = ym;
                            if (!ym_last.Equals(ym))
                            {
                                addrow[r] = sum_monthly;
                                r++;
                                addrow[r] = deduct_monthly;
                                r++;
                                addrow[r] = sum_monthly - deduct_monthly;
                                r++;
                                ym_last = ym;
                                sum_monthly = 0;
                                deduct_monthly = 0;
                            }
                            sum_monthly += brandDailyTotal[it]["daysum"][ite];
                            sum_year += brandDailyTotal[it]["daysum"][ite];
                            deduct_monthly += brandDailyTotal[it]["daydeduct"][ite];
                            deduct_year += brandDailyTotal[it]["daydeduct"][ite];
                            addrow[r] = brandDailyTotal[it]["daysum"][ite];
                            r++;
                            addrow[r] = brandDailyTotal[it]["daydeduct"][ite];
                            r++;
                            addrow[r] = brandDailyTotal[it]["daysum"][ite] - brandDailyTotal[it]["daydeduct"][ite];
                            r++;
                        }
                        if (CrossMonth)
                        {
                            addrow[r] = sum_monthly;
                            r++;
                            addrow[r] = deduct_monthly;
                            r++;
                            addrow[r] = sum_monthly - deduct_monthly;
                            r++;
                        }
                        addrow[r] = sum_year;
                        r++;
                        addrow[r] = deduct_year;
                        r++;
                        addrow[r] = sum_year - deduct_year;
                        _dt.Rows.Add(addrow);
                    }
               // }

                //if (_dt.Rows.Count > 0)
                //{
                    //string fileName = "品牌營業額匯出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    //MemoryStream ms = ExcelHelperXhf.ExportDT(_dt, "");
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    //Response.BinaryWrite(ms.ToArray());
                    string fileName = "品牌營業額匯出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                    //MemoryStream ms = ExcelHelperXhf.ExportDT(_dt, "");
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    //Response.BinaryWrite(ms.ToArray());
                    StringWriter sw = ExcelHelperXhf.SetCsvFromData(_dt, fileName);
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.ContentType = "application/ms-excel";
                    Response.ContentEncoding = Encoding.Default;
                    Response.Write(sw);
                    Response.End();
                //}
                //else
                //{
                //    Response.Write("匯出數據不存在");
                //}

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

        #region 暫存退貨單
        public HttpResponseBase GetReturnMasterList()
        {
            List<OrderReturnUserQuery> stores = new List<OrderReturnUserQuery>();
            string json = string.Empty;
            try
            {
                OrderReturnUserQuery query = new OrderReturnUserQuery();
                IReturnMasterImplMgr _Iretrunlistmgr = new ReturnMasterMgr(mySqlConnectionString);
                StringBuilder sb = new StringBuilder();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                #region 查詢條件
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
                {
                    query.selecttype = Request.Params["selecttype"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
                {
                    query.searchcon = Request.Params["searchcon"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["seldate"]))
                {
                    query.seldate = Request.Params["seldate"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.timestart = Int32.Parse(CommonFunction.GetPHPTime(Request.Params["timestart"]).ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
                {
                    query.timeend = Int32.Parse(CommonFunction.GetPHPTime(Request.Params["dateTwo"]).ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["temp_status"]))
                {
                    query.temp_status = uint.Parse(Request.Params["temp_status"].ToString());
                }
                #endregion
                int totalCount = 0;
                stores = _Iretrunlistmgr.GetOrderTempReturnList(query, out totalCount);
                foreach (var item in stores)
                {
                    item.user_return_createdates = CommonFunction.GetNetTime(item.user_return_createdate);
                    item.user_return_updatedates = CommonFunction.GetNetTime(item.user_return_updatedate);
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        /// <summary>
        /// 暫存退貨單未歸檔送出
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase UpdateReturnMaster()
        {
            OrderReturnUserQuery query = new OrderReturnUserQuery();
            IReturnMasterImplMgr _Iordertempretrunlistmgr = new ReturnMasterMgr(mySqlConnectionString);
            Serial serial = new Serial();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["user_return_id"]))
                {
                    query.user_return_id = Convert.ToUInt32(Request.Params["user_return_id"].ToString());
                }
                else
                {
                    query.user_return_id = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["detail_id"]))
                {
                    query.detail_id = Convert.ToUInt32(Request.Params["detail_id"].ToString());
                }
                else
                {
                    query.detail_id = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["return_reason"]))
                {
                    query.return_reason = Convert.ToUInt32(Request.Params["return_reason"].ToString());
                }
                else
                {
                    query.return_reason = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["temp_status"]))
                {
                    query.temp_status = Convert.ToUInt32(Request.Params["temp_status"].ToString());

                    if (query.temp_status == 1)
                    {
                        DataTable _dt = _Iordertempretrunlistmgr.GetOrderReturnCount(query);
                        if (int.Parse(_dt.Rows[0][0].ToString()) != 0)
                        {
                            string err = string.Empty;
                            err = "{success:true,msg:\"退貨單已存在，請洽諮訊部" + "\"}";

                            this.Response.Clear();
                            this.Response.Write(err);
                            this.Response.End();
                            return this.Response;
                        }
                        else
                        {
                            #region 當狀態要改為歸檔時，新增一條退貨單數據
                            ISerialImplMgr _serial = new SerialMgr(mySqlConnectionString);
                            serial = _serial.GetSerialById(45);
                            query.return_id = Convert.ToUInt32((serial.Serial_Value + 1).ToString());
                            if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                            {
                                query.order_id = uint.Parse(Request.Params["order_id"]);
                            }
                            else
                            {
                                query.order_id = 0;
                            }
                            if (!string.IsNullOrEmpty(Request.Params["item_vendor_id"]))
                            {
                                query.item_vendor_id = uint.Parse(Request.Params["item_vendor_id"]);
                            }
                            else
                            {
                                query.item_vendor_id = 0;
                            }
                            query.return_status = 0;
                            if (!string.IsNullOrEmpty(Request.Params["user_note"]))
                            {
                                query.user_note = Request.Params["user_note"];
                            }
                            else
                            {
                                query.user_note = "";
                            }
                            query.return_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                            if (!string.IsNullOrEmpty(Request.Params["return_zip"]))
                            {
                                query.return_zip = uint.Parse(Request.Params["return_zip"]);
                            }
                            else
                            {
                                query.return_zip = 0;
                            }
                            if (!string.IsNullOrEmpty(Request.Params["return_address"]))
                            {
                                query.return_address = Request.Params["return_address"];
                            }
                            else
                            {
                                query.return_address = "";
                            }

                            query.return_updatedate = 0;
                            System.Net.IPAddress[] addlist = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                            if (addlist.Length > 0)
                            {
                                query.return_ipfrom = addlist[0].ToString();
                            }
                            serial.Serial_Value = serial.Serial_Value + 1;/*所在操作表的列增加*/
                            _serial.Update(serial);/*修改所在的表的列對應的值*/
                            _Iordertempretrunlistmgr.InsertOrderReturnMaster(query);
                            _Iordertempretrunlistmgr.InsertOrderReturnDetail(query);
                            #endregion
                            #region 新增付款單狀態
                            OrderMasterStatus oms = new OrderMasterStatus();
                            _serial = new SerialMgr(mySqlConnectionString);
                            serial = _serial.GetSerialById(29);
                            oms.serial_id = uint.Parse((serial.Serial_Value + 1).ToString());
                            if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                            {
                                oms.order_id = query.order_id;

                            }
                            else
                            {
                                oms.order_id = 0;
                            }
                            oms.order_status = 91;
                            int totalCount = 0;
                            List<LogInLogeQuery> logins = new List<LogInLogeQuery>();
                            LogInLogeQuery login = new LogInLogeQuery();
                            ILogInLogeImplMgr _loginloge = new LogInLogeMgr(mySqlConnectionString);
                            logins = _loginloge.QueryList(login, out totalCount);
                            string Description = string.Empty;
                            Description = "Write:(" + login.user_id + ")" + login.user_username;
                            oms.status_description = Description;
                            System.Net.IPAddress[] addlists = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                            if (addlist.Length > 0)
                            {
                                oms.status_ipfrom = addlist[0].ToString();
                            }

                            oms.status_createdate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                            serial.Serial_Value = serial.Serial_Value + 1;
                            _serial.Update(serial);
                            _Iordertempretrunlistmgr.InsertOrderMasterStatus(oms);

                            #endregion
                            #region 修改付款單狀態

                            if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                            {
                                List<OrderReturnUserQuery> stores = new List<OrderReturnUserQuery>();
                                query.order_id = uint.Parse(Request.Params["order_id"]);
                                stores = _Iordertempretrunlistmgr.OrderMasterQuery(query);
                                if (query.order_status == 90)
                                {
                                    query.order_date_cancel = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                                }
                                else if (query.order_status == 99)
                                {
                                    query.order_date_close = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                                }
                                else
                                {
                                    query.order_status = 5;
                                    query.order_updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                                    System.Net.IPAddress[] addresslist = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                                    if (addlist.Length > 0)
                                    {
                                        query.order_ipfrom = addlist[0].ToString();
                                    }
                                }
                                _Iordertempretrunlistmgr.UpdateOrderMaster(query);
                            }
                            #endregion
                            #region 異動訂單明細商品狀態
                            if (!string.IsNullOrEmpty(Request.Params["detail_id"]))
                            {
                                query.detail_id = Convert.ToUInt32(Request.Params["detail_id"].ToString());
                                query.detail_status = 91;
                                _Iordertempretrunlistmgr.UpdateOrderDetailStatus(query);
                            }
                            #endregion
                            int invoice_id = 0;
                            string status = string.Empty;
                            OrderMaster odm = new OrderMaster();
                            odm.Order_Id = query.order_id;
                            List<OrderMaster> Odmaster = new List<OrderMaster>();
                            #region 是否開立過發票
                            if (_Iordertempretrunlistmgr.SelOrderMaster(query).Rows[0]["invoice_status"].ToString() == "0")
                            {
                                _Iordertempretrunlistmgr.Seltime(query);
                                #region 判斷時間  異動
                                if (_Iordertempretrunlistmgr.SelCon(query).Rows.Count > 0)
                                {
                                    if (_Iordertempretrunlistmgr.SelCon(query).Rows[0]["order_freight_normal"].ToString() == "1")
                                    {//抓取付款單
                                        Odmaster = _Iordertempretrunlistmgr.Selpay(odm);
                                        double free_tax, tax_amout, order_freight_normal_notax, order_freight_low_notax, order_freight_normal_tax, order_freight_low_tax;
                                        foreach (var item in Odmaster)
                                        {
                                            free_tax = Int32.Parse(item.Order_Amount.ToString()) / 1.5;
                                            tax_amout = item.Order_Amount - free_tax;
                                            order_freight_normal_notax = item.Order_Freight_Normal / 1.5;
                                            order_freight_low_notax = item.Order_Freight_Low / 1.5;
                                            order_freight_normal_tax = item.Order_Freight_Normal - order_freight_normal_notax;
                                            order_freight_low_tax = item.Order_Freight_Low - order_freight_low_notax;
                                        }
                                        //抓出明細
                                        status = "0,4";
                                        _Iordertempretrunlistmgr.Seldetail(query, status);
                                    }
                                    if (Int32.Parse(_Iordertempretrunlistmgr.SelCon(query).Rows[0][0].ToString()) != 1)
                                    {
                                        #region 運費判斷
                                        if (_Iordertempretrunlistmgr.SelOrderMaster(query).Rows[0]["order_freight_normal"].ToString() == "0")
                                        {
                                            if (_Iordertempretrunlistmgr.SelOrderMaster(query).Rows[0]["order_freight_low"].ToString() == "0")
                                            {
                                                invoice_id = Int32.Parse(_Iordertempretrunlistmgr.SelOrderMaster(query).Rows[0]["order_freight_low"].ToString());
                                                invoice_id = Int32.Parse(_Iordertempretrunlistmgr.SelInvoiceid(invoice_id).Rows[0][0].ToString());
                                            }
                                        }
                                        #endregion
                                        #region 寫入資料
                                        InvoiceMasterRecord m = new InvoiceMasterRecord();
                                        InvoiceSliveInfo n = new InvoiceSliveInfo();
                                        if (_Iordertempretrunlistmgr.Insertdb(m, n) > 0)
                                        {//寫入資料成功執行成功
                                            _Iordertempretrunlistmgr.Updinvoice(query);
                                            _Iordertempretrunlistmgr.Delinvoice(invoice_id);
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                                #region 過期 開立折讓單
                                invoice_id = Int32.Parse(_Iordertempretrunlistmgr.SelOrderMaster(query).Rows[0]["invoice_id"].ToString());
                                _Iordertempretrunlistmgr.Selmaster(invoice_id);
                                _Iordertempretrunlistmgr.Selslive(invoice_id);
                                status = "89,90,91";
                                _Iordertempretrunlistmgr.Seldetail(query, status);
                                //更新總數(開立折讓單)
                                InvoiceAllowanceRecord larm = new InvoiceAllowanceRecord();
                                _Iordertempretrunlistmgr.Updcount(larm);
                                _Iordertempretrunlistmgr.UpdMaster(invoice_id);
                                #endregion
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    query.temp_status = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["user_note"]))
                {
                    query.user_note = Request.Params["user_note"];

                }
                else
                {
                    query.user_note = "";
                }
                if (!string.IsNullOrEmpty(Request.Params["user_return_createdates"]))
                {
                    long times = CommonFunction.GetPHPTime(Request.Params["user_return_createdates"]);
                    query.user_return_createdate = Convert.ToUInt32(times.ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["slave_status"]))
                {
                    uint status = Convert.ToUInt32(Request.Params["slave_status"]);
                    if (status == 99)
                    {
                        string jsons = string.Empty;
                        jsons = "{success:true,msg:\"已歸檔，無法產生退貨單" + "\"}";

                        this.Response.Clear();
                        this.Response.Write(jsons);
                        this.Response.End();
                        return this.Response;
                    }
                }
                query.user_return_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                _Iordertempretrunlistmgr.UpdateTempStatus(query);
                json = "{success:true}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion
        #region  退货单
        /*獲取退貨單列表*/
        #region 獲取退貨單列表 + HttpResponseBase GetOrderReturnMasterList()
        /// <summary> 
        /// 獲取退貨單列表
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetOrderReturnMasterList()
        {
            string json = string.Empty;
            try
            {
                int totalCount = 0;
                OrderReturnMasterQuery query = new OrderReturnMasterQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//

                if (!string.IsNullOrEmpty(Request.Params["ven_type"]))
                {
                    query.ven_type = Convert.ToInt32(Request.Params["ven_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["search_content"]))
                {
                    query.content = Request.Params["search_content"];
                }
                if (!string.IsNullOrEmpty(Request.Params["date_type"]))
                {
                    query.date_type = Convert.ToInt32(Request.Params["date_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["time_start"]))
                {
                    query.time_start = Convert.ToDateTime(Request.Params["time_start"]).ToString("yyyy-MM-dd 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                {
                    query.time_end = Convert.ToDateTime(Request.Params["time_end"]).ToString("yyyy-MM-dd 23:59:59");
                }
                if (!string.IsNullOrEmpty(Request.Params["return_status"]))
                {
                    query.return_status = Convert.ToUInt32(Request.Params["return_status"]);
                }

                _orderReturnMaster = new OrderReturnMasterMgr(mySqlConnectionString);
                List<OrderReturnMasterQuery> stores = _orderReturnMaster.GetReturnMaster(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";
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

        #region 獲取區域地址  +string QueryCity() ; +string QueryZip()
        [CustomHandleError]
        public string QueryCity()
        {
            zMgr = new ZipMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                json = zMgr.QueryMiddle(Request.Form["topValue"] ?? "");
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

        [CustomHandleError]
        public string QueryZip()
        {
            zMgr = new ZipMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["topValue"]))
                {
                    json = zMgr.QuerySmall(Request.Form["topValue"].ToString(), Request.Form["topText"].ToString());
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
            return json;
        }
        #endregion

        #region 獲取區域地址  +string QueryCityZip()
        [CustomHandleError]
        public string QueryCityZip()
        {
            zMgr = new ZipMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["zipcode"]))
                {
                    json = "{success:true,data:" + JsonConvert.SerializeObject(zMgr.QueryCityAndZip(Request.Params["zipcode"])) + "}";//返回json數據
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
            return json;
        }


        #endregion

        #region 獲取該訂單的支付方式  +string GetPayment()
        [CustomHandleError]
        public string GetPayment()
        {
            _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
            _orderReturnMaster = new OrderReturnMasterMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    OrderMaster om = _orderMasterMgr.GetPaymentById(uint.Parse(Request.Params["order_id"]));
                    //獲取暫存單中atm機 銀行信息 地址信息

                    if (om.Order_Payment == 2)
                    {
                        OrderReturnUserQuery oruQuery = _orderReturnMaster.GetReturnDetailById(om.Order_Id);
                        json = "{success:true,bank_name:" + oruQuery.bank_name + ",bank_branch:" + oruQuery.bank_branch
                            + ",bank_account:" + oruQuery.bank_account + ",account_name:" + oruQuery.account_name + "}";
                    }
                    else
                    {
                        json = "{success:true,order_id:" + om.Order_Id + ",Order_Payment:" + om.Order_Payment + "}";
                    }
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
            return json;
        }


        #endregion

        /*修改退貨單*/
        #region 修改退貨單 + HttpResponseBase SaveOrderReturnMaster()
        /// <summary> 
        /// 修改退貨單
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase SaveOrderReturnMaster()
        {
            string json = string.Empty;
            try
            {
                _orderReturnMaster = new OrderReturnMasterMgr(mySqlConnectionString);
                OrderReturnMasterQuery query = new OrderReturnMasterQuery();
                string return_status = string.Empty;
                query.return_id = uint.Parse(Request.Params["return_id"]);
                query.order_id = uint.Parse(Request.Params["order_id"]);

                if (!string.IsNullOrEmpty(Request.Params["return_note"]))
                {
                    query.return_note = Request.Params["return_note"];
                    if (!string.IsNullOrEmpty(Request.Params["invoice_deal"]))
                    {
                        query.invoice_deal = uint.Parse(Request.Params["invoice_deal"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["return_deal"]))
                    {
                        query.package = uint.Parse(Request.Params["return_deal"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["return_zip"]))
                    {
                        query.return_zip = uint.Parse(Request.Params["return_zip"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["return_address"]))
                    {
                        query.return_address = Request.Params["return_address"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["deliver_code"]))
                    {
                        query.deliver_code = Request.Params["deliver_code"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["return_status"]))
                    {
                        query.return_status = uint.Parse(Request.Params["return_status"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_note"]))
                    {
                        query.bank_note = Request.Params["bank_note"];
                    }
                    query.return_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    query.return_ipfrom = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();

                    if (!string.IsNullOrEmpty(Request.Params["bank_name"]))
                    {
                        query.bank_name = Request.Params["bank_name"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_branch"]))
                    {
                        query.bank_branch = Request.Params["bank_branch"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["bank_account"]))
                    {
                        query.bank_account = Request.Params["bank_account"];

                    }
                    if (!string.IsNullOrEmpty(Request.Params["account_name"]))
                    {
                        query.account_name = Request.Params["account_name"];
                    }
                    query.user_id = (Session["caller"] as Caller).user_id.ToString();
                    query.user_username = (Session["caller"] as Caller).user_username.ToString();
                    query.HgReturnUrl = HgReturnUrl;
                    query.HgMerchandID = HgMerchandID;
                    query.HgTeminalID = HgTeminalID;
                    query.HgCategoty = HgCategoty;
                    query.HgWallet = HgWallet;
                    if (_orderReturnMaster.Save(query) > 0)
                    {
                        json = "{success:'true'}";
                    }
                    else
                    {
                        json = "{success:'false'}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:'false'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 取消退貨
        public HttpResponseBase CancelReturnPurchaes()
        {
            string json = string.Empty;
            try
            {
                OrderReturnMasterQuery query = new OrderReturnMasterQuery();
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    query.order_id = Convert.ToUInt32(Request.Params["order_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["return_id"]))
                {
                    query.return_id = Convert.ToUInt32(Request.Params["return_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["return_status"]))
                {
                    query.return_status = Convert.ToUInt32(Request.Params["return_status"]);
                }
                
                query.return_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
             _orderReturnMaster=new OrderReturnMasterMgr (mySqlConnectionString);
              json=   _orderReturnMaster.CancelReturnPurchaes(query);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:'false'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #endregion
        #region 訂單查詢
        #region 訂單查詢列表
        public HttpResponseBase GetOrderList()
        {
            List<OrderMasterQuery> stores = new List<OrderMasterQuery>();
            string json = string.Empty;
            try
            {
                StringBuilder sb = new StringBuilder();
                OrderMasterQuery query = new OrderMasterQuery();
                DataTable dt_payment = new DataTable();
                DataTable dt_status = new DataTable();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                string addSerch = string.Empty;//追加一張表的查詢
                int id; uint uid; DateTime dtime;
                #region 查詢條件
                if (int.TryParse(Request.Params["Vip_User_Group"].ToString(), out id))//會員群組
                {
                    query.group_id = id;
                }
                if (int.TryParse(Request.Params["invoice"].ToString() ,out id ))//過濾已開發票
                {
                    query.invoice=id;                    
                }
                if (uint.TryParse(Request.Params["channel"].ToString(),out uid))//賣場
                {
                    query.Channel = uid;
                }
                if (int.TryParse(Request.Params["order_pay"].ToString(), out id))//付款狀態
                {
                    query.pay_status = id;
                }
                if (!string.IsNullOrEmpty(Request.Params["selecttype"]))//查詢種類，訂單編號，訂購姓名
                {
                    query.selecttype = Request.Params["selecttype"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))//查詢內容
                {
                    query.searchcon = Request.Params["searchcon"].ToString();
                }                
                if (int.TryParse(Request.Params["timeone"].ToString(),out id))//時間類型
                {
                    query.dateType = id;
                }
                if (DateTime.TryParse(Request.Params["dateOne"].ToString(), out dtime))//開始時間
                {
                    query.datestart = dtime;
                }
                if (DateTime.TryParse(Request.Params["dateTwo"].ToString(), out dtime))
                {
                    query.dateend = dtime;
                }
                if (int.TryParse(Request.Params["page_status"].ToString(),out id))//付款單狀態
                {
                    query.orderStatus = Request.Params["page_status"];
                }
                if (uint.TryParse(Request.Params["order_payment"], out uid))//付款方式，AT
                {
                    query.Order_Payment = uid;                    
                }
                #endregion
                _ordermaster = new OrderMasterMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _ordermaster.getOrderSearch(query, sb.ToString(), out totalCount, addSerch);
                dt_payment = GetTP("payment");
                dt_status = GetTP("order_status");
                foreach (var item in stores)
                {
                    if (item.source_trace != 0)
                    {
                        item.redirect_name = "【" + item.source_trace + "】" + item.redirect_name;
                    }
                    string message = string.Empty;
                    //獲取時間
                    item.suser_reg_date = CommonFunction.GetNetTime(item.user_reg_date);
                    item.ordercreatedate = CommonFunction.GetNetTime(item.Order_Createdate);
                    if (item.Deduct_Bonus != 0)
                    {
                        message = "購物金使用:" + item.Deduct_Bonus;
                    }
                    if (item.Deduct_Welfare != 0)
                    {
                        message = "抵用卷使用使用:" + item.Deduct_Welfare;
                    }
                    if (item.Deduct_Happygo != 0)
                    {
                        message = "HG使用:" + item.Deduct_Happygo + "點/" + Math.Round(Convert.ToDecimal(item.Deduct_Happygo * item.Deduct_Happygo_Convert)) + "元";
                    }
                    item.order_pay_message = message;
                    if (item.Order_Payment >0)
                    {//帶出付款方式的參數
                        DataRow[] dr = dt_payment.Select("ParameterCode = '" + item.Order_Payment.ToString() + "'");
                        DataTable _newdt = dt_payment.Clone();
                        foreach (DataRow i in dr)
                        {
                            _newdt.Rows.Add(i.ItemArray);
                        }
                        item.payment = _newdt.Rows[0]["ParameterName"].ToString();
                    }
                    if (item.Order_Status >=0)
                    {//帶出付款方式的參數
                        DataRow[] dr = dt_status.Select("ParameterCode = '" + item.Order_Status.ToString() + "'");
                        DataTable _newdt = dt_status.Clone();
                        foreach (DataRow i in dr)
                        {
                            _newdt.Rows.Add(i.ItemArray);
                        }
                        item.orderStatus = _newdt.Rows[0]["remark"].ToString();
                    }
                    switch (item.Export_Flag)
	                    {
                        case 0:
                                item.export_flag_str = "-";
                                break;
                        case 1:
                                item.export_flag_str = "待拋轉";
                                break;
                        case 2:
                                item.export_flag_str = "異動";
                                break;
                        case 3:
                                item.export_flag_str = "整筆取消";
                                break;
		                    default:
                                item.export_flag_str = "已拋轉";
                                break;
	                    }
                   
                }
                #region 注釋 給訂單加上超鏈接/判斷購物金和抵用卷
                //for (int i = 0; i < stores.Count; i++)
                //{
                    //string message = string.Empty;
                    //DataRow[] rows = dt.Select("redirect_id=" + stores[i].Source_Trace);//篩選出訂單下的來源ID和url
                    //foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，加入某個品牌的每日營業額，沒有就為初始值
                    //{
                    //    if (!string.IsNullOrEmpty(row["redirect_id"].ToString()))
                    //    {
                    //        stores[i].ordercreatedate = CommonFunction.GetNetTime(stores[i].Order_Createdate);
                    //        stores[i].redirect_name = row["redirect_name"].ToString();
                    //        stores[i].redirect_url = row["redirect_url"].ToString();
                    //    }
                    //}
                    //stores[i].ordercreatedate = CommonFunction.GetNetTime(stores[i].Order_Createdate);         
                    //if (stores[i].Deduct_Bonus != 0)
                    //{
                    //    message = "購物金使用:" + stores[i].Deduct_Bonus;
                    //}
                    //if (stores[i].Deduct_Welfare != 0)
                    //{
                    //    message = "抵用卷使用使用:" + stores[i].Deduct_Welfare;
                    //}
                    //if (stores[i].Deduct_Happygo != 0)
                    //{
                    //    message = "HG使用:" + stores[i].Deduct_Happygo + "點/" + Math.Round(Convert.ToDecimal(stores[i].Deduct_Happygo * stores[i].Deduct_Happygo_Convert)) + "元";
                    //}
                    //stores[i].order_pay_message = message;
                //}
                #endregion
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter(); 
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 訂單查詢匯出功能
        public void OrderSerchExport()
        {
            
            List<OrderMasterQuery> stores = new List<OrderMasterQuery>();
            OrderMasterQuery query = new OrderMasterQuery();
            int id; uint uid; DateTime dtime;

            #region 查詢條件
            if (int.TryParse(Request.Params["Vip_User_Group"].ToString(), out id))//會員群組
            {
                query.group_id = id;
            }
            if (int.TryParse(Request.Params["invoice"].ToString(), out id))//過濾已開發票
            {
                query.invoice = id;
            }
            if (uint.TryParse(Request.Params["channel"].ToString(), out uid))//賣場
            {
                query.Channel = uid;
            }
            if (int.TryParse(Request.Params["order_pay"].ToString(), out id))//付款狀態
            {
                query.pay_status = id;
            }
            if (!string.IsNullOrEmpty(Request.Params["selecttype"]))//查詢種類，訂單編號，訂購姓名
            {
                query.selecttype = Request.Params["selecttype"].ToString();
            }
            if (!string.IsNullOrEmpty(Request.Params["searchcon"]))//查詢內容
            {
                query.searchcon = Request.Params["searchcon"].ToString();
            }
            if (int.TryParse(Request.Params["timeone"].ToString(), out id))//時間類型
            {
                query.dateType = id;
            }
            if (DateTime.TryParse(Request.Params["dateOne"].ToString(), out dtime))//開始時間
            {
                query.datestart = dtime;
            }
            if (DateTime.TryParse(Request.Params["dateTwo"].ToString(), out dtime))
            {
                query.dateend = dtime;
            }
            if (int.TryParse(Request.Params["page_status"].ToString(), out id))//付款單狀態
            {
                query.orderStatus = Request.Params["page_status"];
            }
            if (uint.TryParse(Request.Params["order_payment"], out uid))//付款方式，AT
                {
                query.Order_Payment = uid;
            }
            #endregion
            string json = string.Empty;
          
            DataTable dtHZ = new DataTable();
            DataTable dt_payment = new DataTable();
            DataTable dt_status = new DataTable();
            dt_payment = GetTP("payment");
            dt_status = GetTP("order_status");

            string newExcelName = string.Empty;
            dtHZ.Columns.Add("付款單號", typeof(String));
            dtHZ.Columns.Add("訂單人", typeof(String));
            dtHZ.Columns.Add("收貨人", typeof(String));
            dtHZ.Columns.Add("訂單應收金額", typeof(String));
            dtHZ.Columns.Add("付款方式", typeof(String));

            dtHZ.Columns.Add("付款單狀態", typeof(String));
            dtHZ.Columns.Add("付款單成立日期", typeof(String));
            dtHZ.Columns.Add("購物金抵用金額", typeof(String));
            dtHZ.Columns.Add("抵用券抵用金額", typeof(String));
            dtHZ.Columns.Add("HG抵用金額", typeof(String));

            dtHZ.Columns.Add("常溫運費", typeof(String));
            dtHZ.Columns.Add("低溫運費", typeof(String));
            dtHZ.Columns.Add("付款時間", typeof(String));
            dtHZ.Columns.Add("銀行紅利折抵", typeof(String));
            dtHZ.Columns.Add("取消金額", typeof(String));

            dtHZ.Columns.Add("退貨金額", typeof(String));
            dtHZ.Columns.Add("來源群組", typeof(String));
            dtHZ.Columns.Add("來源", typeof(String));
            dtHZ.Columns.Add("UTM_CODE", typeof(String));
            dtHZ.Columns.Add("UTM_NAME", typeof(String));

            dtHZ.Columns.Add("訂購人編號", typeof(String));
            //dtHZ.Columns.Add("訂購人手機", typeof(String));
            dtHZ.Columns.Add("虛擬帳號", typeof(String));
            dtHZ.Columns.Add("賣場", typeof(String));
            dtHZ.Columns.Add("外站訂單編號", typeof(String));

            dtHZ.Columns.Add("管理員備註", typeof(String));

            try
            {
                int totalCount = 0;
                _ordermaster = new OrderMasterMgr(mySqlConnectionString);
                stores = _ordermaster.Export(query, "", out totalCount);
                foreach (var item in stores)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = item.Order_Id;
                    dr[1] = item.Order_Name;
                    dr[2] = item.Delivery_Name;
                    dr[3] = item.Order_Amount;
                    dr[4] = item.Order_Payment;
                    dr[5] = item.Order_Status;
                    if (item.Order_Payment > 0)
                    {//帶出付款方式的參數
                        DataRow[] drs = dt_payment.Select("ParameterCode = '" + item.Order_Payment.ToString() + "'");
                        DataTable _newdt = dt_payment.Clone();
                        foreach (DataRow i in drs)
                        {
                            _newdt.Rows.Add(i.ItemArray);
                        }
                        dr[4] = _newdt.Rows[0]["ParameterName"].ToString();
                    }
                    if (item.Order_Status >= 0)
                    {//帶出付款方式的參數
                        DataRow[] drs = dt_status.Select("ParameterCode = '" + item.Order_Status.ToString() + "'");
                        DataTable _newdt = dt_status.Clone();
                        foreach (DataRow i in drs)
                        {
                            _newdt.Rows.Add(i.ItemArray);
                        }
                        dr[5] = _newdt.Rows[0]["remark"].ToString();
                    }
                    dr[6] = item.Order_Createdate == 0 ? "" : CommonFunction.GetNetTime(item.Order_Createdate).ToString();
                    dr[7] = item.Deduct_Bonus;
                    dr[8] = item.Deduct_Welfare;
                    dr[9] = item.Deduct_Happygo == 0 ? 0 : Math.Round(item.Deduct_Happygo*item.deduct_happygo_convert);
                    dr[10] = item.Order_Freight_Normal;
                    dr[11] = item.Order_Freight_Low;
                    dr[12] = item.Order_Date_Pay == 0 ? "未付款" : CommonFunction.GetNetTime(item.Order_Date_Pay).ToString();
                    dr[13] = item.Deduct_Card_Bonus;
                    dr[14] = item.Money_Cancel;
                    dr[15] = item.Money_Return;
                    dr[16] = item.group_name;
                    dr[17] = item.redirect_name;
                    dr[18] = item.utm_id;
                    dr[19] = item.utm_source;
                    dr[20] = item.User_Id;
                    //dr[21] = " "+ item.Order_Mobile;
                    dr[21] = " " + item.hncb_id;
                    dr[22] = item.channel_name_full;
                    dr[23] = item.Channel_Order_Id;
                    dr[24] = item.Note_Admin;
                    dtHZ.Rows.Add(dr);
                }
                if (stores.Count > 0)
                {
                    string fileName = "訂單查詢匯出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "");
                    Response.AddHeader("Content-Disposition", "attach-ment;filename=" + fileName);
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
                json = "false, ";
            }

        }
        #endregion
        #region 群組會員
        public HttpResponseBase GetVipUserGroup()
        {
            string json = string.Empty;
            List<VipUserGroup> store = new List<VipUserGroup>();
            VipUserGroup vu = new VipUserGroup();
            try
            {
                _IOrderUserMgr = new OrderUserReduceMgr(mySqlConnectionString);
                store = _IOrderUserMgr.GetVipUserGroupStore();
                vu.group_id = 0;
                vu.group_name = "所有群組會員";
                store.Insert(0, vu);
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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

        #endregion
        #region 促銷減免查詢
        #region 促銷減免查詢列表頁
        public HttpResponseBase GetOrderUserReduce()
        {
            string json = string.Empty;
            List<PromotionsAmountReduceMemberQuery> store = new List<PromotionsAmountReduceMemberQuery>();
            PromotionsAmountReduceMemberQuery query = new PromotionsAmountReduceMemberQuery();
            try
            {
                int totalCount = 0;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["select_type"]))//查詢條件
                {
                    query.select_type = Request.Params["select_type"];
                }
                if (!string.IsNullOrEmpty(Request.Params["search_con"]))//查詢內容
                {
                    query.search_con = Request.Params["search_con"];
                }
                if (!string.IsNullOrEmpty(Request.Params["reduce_id"]))
                {
                    query.reduce_id = Convert.ToInt32(Request.Params["reduce_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.search_date = Convert.ToInt32(Request.Params["date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Request.Params["start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time = Convert.ToDateTime(Request.Params["end_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["type"]))
                {
                    query.type = Convert.ToInt32(Request.Params["type"]);
                }
                _IOrderUserMgr = new OrderUserReduceMgr(mySqlConnectionString);
                store = _IOrderUserMgr.GetOrderUserReduce(query, out totalCount);
                foreach (var item in store)
                {

                    item.suser_reg_date = CommonFunction.GetNetTime(item.user_reg_date);
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 減免活動
        public HttpResponseBase GetPromotionsAmoutReduce()
        {
            string json = string.Empty;
            List<PromotionsAmountReduceMemberQuery> store = new List<PromotionsAmountReduceMemberQuery>();
            PromotionsAmountReduceMemberQuery PAM = new PromotionsAmountReduceMemberQuery();
            try
            {
                _IOrderUserMgr = new OrderUserReduceMgr(mySqlConnectionString);
                store = _IOrderUserMgr.GetReduceStore();
                PAM.id = 0;
                PAM.name = "所有減免活動";
                store.Insert(0, PAM);
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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
        #endregion
        #region 訂單詳細資料
        #region 訂單中詳細字段賦值
        public HttpResponseBase GetData()
        {
            uint orderId =0;
            if(uint.TryParse(Request.Params["order_id"],out orderId))
            {
                orderId = orderId;
            }
            OrderShowMasterQuery query = new OrderShowMasterQuery();
            string json = string.Empty;
            OrderShowMasterQuery store = new OrderShowMasterQuery();
            _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
            zMgr = new ZipMgr(mySqlConnectionString);
            try
            {
                store = _orderMasterMgr.GetData(orderId);
                store.OrderDatePay = CommonFunction.GetNetTime(store.order_date_pay);
                if (!string.IsNullOrEmpty(store.money_collect_date.ToString()) && store.money_collect_date != 0)
                {
                    store.MoneyCollectDate = CommonFunction.GetNetTime(store.money_collect_date);
                }
                store.NoteOrderModifyTime = CommonFunction.GetNetTime(store.note_order_modify_time);
                store.OrderCreateDate = CommonFunction.GetNetTime(store.order_createdate);
                store.OrderDateClose = CommonFunction.GetNetTime(store.order_date_close);
                double value = Convert.ToDouble(store.deduct_happygo * store.deduct_happygo_convert);
                store.Hg_Nt = Math.Round(value).ToString();
                //地址直接讀取
                store.order_address = zMgr.Getaddress(int.Parse(store.order_zip.ToString())) + store.order_address;
                store.delivery_address = zMgr.Getaddress(int.Parse(store.delivery_zip.ToString())) + store.delivery_address;
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
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 編輯客戶備註
        public HttpResponseBase SaveNoteOrder()
        {
            string json = string.Empty;
            OrderShowMasterQuery query = new OrderShowMasterQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    query.order_id = Convert.ToUInt32(Request.Params["order_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["note_order"]))
                {
                    query.note_order = Request.Params["note_order"];
                }
                query.user_id = query.user_id = uint.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                query.NoteOrderModifyTime = DateTime.Now;
                _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
                _orderMasterMgr.SaveNoteOrder(query);
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 新增狀態列表
        public HttpResponseBase SaveStatus()
        {
            string json = string.Empty;
            OrderShowMasterQuery query = new OrderShowMasterQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    query.order_id = Convert.ToUInt32(Request.Params["order_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["order_status"]))
                {
                    query.order_status = Convert.ToUInt32(Request.Params["order_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_username"]))
                {
                    query.manager_name = Request.Params["user_username"];
                }
                if (!string.IsNullOrEmpty(Request.Params["status_description"]))
                {//query.manager_name 
                    query.status_description = (Session["caller"] as Caller).user_username+ ":" + Request.Params["status_description"];
                }
                System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                if (addlist.Length > 0)
                {
                    query.status_ipfrom = addlist[0].ToString();
                }
                query.StatusCreateDate = DateTime.Now;
                _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
                _orderMasterMgr.SaveStatus(query);
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region  新增管理員備註
        public HttpResponseBase SaveNoteAdmin()
        {
            string json = string.Empty;
            OrderShowMasterQuery query = new OrderShowMasterQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    query.order_id = Convert.ToUInt32(Request.Params["order_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["note_admin"]))
                {
                    query.note_admin = Request.Params["note_admin"];
                }
                _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
                _orderMasterMgr.SaveNoteAdmin(query);
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
        #region 獲取用戶信息
        public HttpResponseBase GetUserInfo()
        {
            string json = string.Empty;
            UsersListQuery store = new UsersListQuery();
            _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
            try
            {
                uint user_id = Convert.ToUInt32(Request.Params["user_id"]);
                // uint user_id = 2341;
                store = _orderMasterMgr.GetUserInfo(user_id);
                store.suser_reg_date = CommonFunction.GetNetTime(store.user_reg_date);
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
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #endregion
        #region 訂單詳細記錄所有列表
        #region 狀態列表
        public HttpResponseBase GetStatus()
        {
            List<OrderMasterStatusQuery> stores = new List<OrderMasterStatusQuery>();
            string json = string.Empty;
            try
            {
                OrderMasterStatusQuery query = new OrderMasterStatusQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToUInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetStatus(query, out totalCount);
                ulong Temp_Count = 0;
                foreach (var item in stores)
                {
                    Temp_Count++;//參考自 admin.gigade100.com/order/ajax_order.php第46行
                    item.serial_id = Temp_Count;
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #region 出貨單
        public HttpResponseBase GetDeliver()
        {
            List<OrderDeliverQuery> stores = new List<OrderDeliverQuery>();
            string json = string.Empty;
            try
            {
                OrderDeliverQuery query = new OrderDeliverQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                query.order_id = Convert.ToUInt32(Request.Params["Order_Id"].ToString());
                _tabshow = new TabShowMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _tabshow.GetDeliver(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        #endregion

        #region 公共方法(取地址,參數)
        //獲取地址
        public HttpResponseBase GetZip()
        {
            string json = string.Empty;
            DataTable store = new DataTable();
            zMgr = new ZipMgr(mySqlConnectionString);
            try
            {
                store = zMgr.GetZip();
                json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";
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
        //取參數表數據
        public DataTable GetTP(string type)
        {
            string json = string.Empty;
            Parametersrc p = new Parametersrc();
            DataTable dt = new DataTable();
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            try
            {
                p.ParameterType = type;
                dt = _ptersrc.GetTP(p);
                if (dt.Rows.Count > 0)
                {
                    return dt;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return null;
            }
        }
        //付款單/訂單 狀態
        public HttpResponseBase GetPayMentType()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            string paraType = string.Empty;
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    paraType = Request.QueryString["paraType"].ToString();
                    stores = _ptersrc.GetElementType(paraType);
                    json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
                }
                else
                {
                    json = "{success:true,totalCount:0,data:[]}";
                }
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
        //付款方式
        public HttpResponseBase GetPayMentStatus()
        {
            List<Parametersrc> stores = new List<Parametersrc>();
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            string paraType = string.Empty;
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    paraType = Request.QueryString["paraType"].ToString();
                    stores = _ptersrc.PayforType(paraType);
                    json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
                }
                else
                {
                    json = "{success:true,totalCount:0,data:[]}";
                }
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
        #region 獲取下拉框參數
        [CustomHandleError]
        [OutputCache(Duration = 3600, VaryByParam = "paraType", Location = System.Web.UI.OutputCacheLocation.Client)]
        public string QueryPara()
        {
            _ptersrc = new ParameterMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    json = _ptersrc.Query(Request.QueryString["paraType"].ToString());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,totalCount:0,data:[]}";
            }
            return json;
        }
        #endregion
        //根據vendorid獲取供應商信息        
        public HttpResponseBase GetVendorDetail()
        {
            List<Vendor> stores = new List<Vendor>();
            Vendor q= new Vendor();
            _orderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["vid"]))
                {
                    q.vendor_id = uint.Parse(Request.Params["vid"].ToString());
                    stores = _orderDetailMgr.GetVendor(q);
                    json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
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
                json = "{success:false,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 退款狀態
        public HttpResponseBase CheckOrderId()
        {
            string json = string.Empty;
            try
            {
                OrderReturnStatusQuery query = new OrderReturnStatusQuery();
                if (!string.IsNullOrEmpty(Request.Params["return_id"]))
                {
                     query.return_id =   Convert.ToUInt32(Request.Params["return_id"]);
                    //如果ors_order_id為0則證明return_id不存在
                     _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                     query.ors_order_id = _orderReturnStatus.GetOrderIdByReturnId(query.return_id);
                    json = _orderReturnStatus.CheckOrderId(query);
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

        public HttpResponseBase SaveOrderStatus()
        {
            OrderReturnStatusQuery query = new OrderReturnStatusQuery();
            string json = string.Empty;
            try
            {
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["orc_name"]))
                {
                    query.orc_name = (Request.Params["orc_name"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_phone"]))
                {
                    query.orc_phone = (Request.Params["orc_phone"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_zipcode"]))
                {
                    query.orc_zipcode = (Request.Params["orc_zipcode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_address"]))
                {
                    query.orc_address = (Request.Params["orc_address"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_type"]))
                {
                    query.orc_type =Convert.ToInt32(Request.Params["orc_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_service_remark"]))
                {
                    query.orc_service_remark =  (Request.Params["orc_service_remark"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_remark"]))
                {
                    query.orc_remark = (Request.Params["orc_remark"]);
                    query.ors_remark = (Request.Params["orc_remark"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["return_id"]))
                {
                    query.return_id = Convert.ToUInt32(Request.Params["return_id"]);
                    query.orc_order_id = _orderReturnStatus.GetOrderIdByReturnId(query.return_id);
                    query.ors_order_id = query.orc_order_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["invoice_deal"]))
                {
                    query.invoice_deal =Convert.ToInt32(Request.Params["invoice_deal"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_send"]))
                {
                    query.orc_send = Convert.ToInt32(Request.Params["orc_send"]);
                }
                
                query.ors_createuser = (Session["caller"] as Caller).user_id;
             
                json = _orderReturnStatus.SaveOrderReturn(query);
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

        public HttpResponseBase InsertTransport()
        {
            OrderReturnStatusQuery query = new OrderReturnStatusQuery();
            string json = string.Empty;
            try
            {
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["return_id"]))
                {
                    query.return_id = Convert.ToUInt32(Request.Params["return_id"]);
                    query.ors_order_id = _orderReturnStatus.GetOrderIdByReturnId(query.return_id);
                    query.orc_order_id = query.ors_order_id;
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_deliver_code"]))
                {
                    query.orc_deliver_code = (Request.Params["orc_deliver_code"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_deliver_date"]))
                {
                    query.orc_deliver_date = Convert.ToDateTime(Request.Params["orc_deliver_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orc_deliver_time"]))
                {
                    query.orc_deliver_time = (Request.Params["orc_deliver_time"]);
                }
                query.ors_status = 1;
               query.ors_createuser = (Session["caller"] as Caller).user_id;
                json = _orderReturnStatus.InsertTransport(query);
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

        public HttpResponseBase CouldGridList()
        {
            string json = string.Empty;
            try
            {
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                OrderReturnStatusQuery query = new OrderReturnStatusQuery();
                List<OrderReturnStatusQuery> store = new List<OrderReturnStatusQuery>();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["return_id"]))
                {
                    query.return_id = Convert.ToUInt32(Request.Params["return_id"]);
                    query.ors_order_id = _orderReturnStatus.GetOrderIdByReturnId(query.return_id);
                }
         
                int totalCount = 0;
                store = _orderReturnStatus.CouldGridList(query, out totalCount);
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";//返回json數據
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

        //收到商品可退
        public HttpResponseBase CouldReturn()
        {
            string json = string.Empty;
            try
            {
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                OrderReturnStatusQuery query = new OrderReturnStatusQuery();
                if (!string.IsNullOrEmpty(Request.Params["return_id"]))
                {
                    query.return_id = Convert.ToUInt32(Request.Params["return_id"]);
                    query.ors_order_id = _orderReturnStatus.GetOrderIdByReturnId(query.return_id);
                    query.ors_status = 2;
                    query.ors_createuser = (Session["caller"] as Caller).user_id;

                    if (!string.IsNullOrEmpty(Request.Params["ormpackage"]))
                    {
                        query.ormpackage = Convert.ToInt32(Request.Params["ormpackage"]);
                    }
                    json = _orderReturnStatus.CouldReturn(query);
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
        //確認入庫
        public HttpResponseBase CouldWareHouse()
        {
            string json = string.Empty;
            try
            {
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                OrderReturnStatusQuery query = new OrderReturnStatusQuery();
                if (!string.IsNullOrEmpty(Request.Params["return_id"]))
                {
                    query.return_id = Convert.ToUInt32(Request.Params["return_id"]);
                    query.ors_order_id = _orderReturnStatus.GetOrderIdByReturnId(query.return_id);
                    query.ors_status = 3;
                    query.ors_createuser = (Session["caller"] as Caller).user_id;
                    json = _orderReturnStatus.PlaceOnFile(query);
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
        //確認退款單
        public HttpResponseBase CouldReturnMoney()
        {
            string json = string.Empty;
            try
            {
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                OrderReturnStatusQuery query = new OrderReturnStatusQuery();
                if (!string.IsNullOrEmpty(Request.Params["return_id"]))
                {
                    query.return_id = Convert.ToUInt32(Request.Params["return_id"]);
                    query.ors_order_id = _orderReturnStatus.GetOrderIdByReturnId(query.return_id);
                }
                if (!string.IsNullOrEmpty(Request.Params["bank_name"]))
                {
                    query.bank_name =(Request.Params["bank_name"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["bank_branch"]))
                {
                    query.bank_branch = (Request.Params["bank_branch"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["bank_account"]))
                {
                    query.bank_account = (Request.Params["bank_account"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["account_name"]))
                {
                    query.account_name = (Request.Params["account_name"]);
                }
                query.ors_status = 4;
                query.ors_createuser = (Session["caller"] as Caller).user_id;
               json = _orderReturnStatus.CouldReturnMoney(query);
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


        public HttpResponseBase GetOrcTypeStore()
        {
            string json = string.Empty;
            try
            {
                //List<OrderReturnStatusQuery> store = new List<OrderReturnStatusQuery>();
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                DataTable _dt = _orderReturnStatus.GetOrcTypeStore();
                json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";//返回json數據
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


        public HttpResponseBase GetInvoiceDealStore()
        {
            string json = string.Empty;
            try
            {
                //List<OrderReturnStatusQuery> store = new List<OrderReturnStatusQuery>();
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                DataTable _dt = _orderReturnStatus.GetInvoiceDealStore();
                json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";//返回json數據
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

        public HttpResponseBase GetPackageStore()
        {
            string json = string.Empty;
            try
            {
                //List<OrderReturnStatusQuery> store = new List<OrderReturnStatusQuery>();
                _orderReturnStatus = new OrderReturnStatusMgr(mySqlConnectionString);
                DataTable _dt = _orderReturnStatus.GetPackageStore();
                json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";//返回json數據
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

        #region 更改付款方式
        public HttpResponseBase ChangePayMent()
        {
            string json = string.Empty;
            OrderMasterQuery query = new OrderMasterQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["payment"]))
                {
                    query.payment = Request.Params["payment"];
                }

                if (!string.IsNullOrEmpty(Request.Params["delivery"]))
                {
                    query.delivery = Convert.ToInt32(Request.Params["delivery"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    query.Order_Id = Convert.ToUInt32(Request.Params["order_id"]);
                }
               System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
               query.Order_Ipfrom = addlist[0].ToString();
               query.username = (Session["caller"] as Caller).user_username;
                _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
             json=   _orderMasterMgr.ChangePayMent(query);
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
    }
}
