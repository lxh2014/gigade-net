using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vendor.Controllers
{
    public class SendProductController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        static string specPath = ConfigurationManager.AppSettings["specPath"];
        static string imgServerPath = ConfigurationManager.AppSettings["imgServerPath"];
        static string ProduceGroupCsvPath = ConfigurationManager.AppSettings["ProduceGroupCsv"];
        IOrderSlaveImplMgr _IOrderSlaveMgr;
        IOrderDetailImplMgr _OrderDetailMgr;
        private IParametersrcImplMgr _ptersrc;
        IOrderDeliverImplMgr _OrderDeliverMgr;
        IVendorImplMgr _VendorMgr;
        IOrderImplMgr _OrderMgr;
        IOrderSlaveImplMgr _OrderSlaveMgr;
        IZipImplMgr _zipMgr;
        #region 公共參數表
        public int PRODUCT_FREIGHT_NORMAL = 1;//常溫
        public int PRODUCT_FREIGHT_LOW = 2;//冷凍
        public int PRODUCT_FREIGHT_NO_NORMAL = 3;//常溫免運
        public int PRODUCT_FREIGHT_NO_LOW = 4;//冷凍免運
        public int PRODUCT_FREIGHT_FREEZINF = 5;//冷藏
        public int PRODUCT_FREIGHT_NO_FREEZINF = 6;//冷藏免運
        #endregion


        #region View
        /// <summary>
        /// 供應商調度出貨視圖
        /// </summary>
        /// <returns></returns>
        public ActionResult AllOrderWaitDeliver()
        {
            //int[] num = {2,92 };
            //int u_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            //bool result=false;
            //for (int i = 0; i < num.Length; i++)
            //{
            //    if (num[i] == u_id)
            //    {
            //        result = true;
            //    }
            //}
            //if (!result)
            //{ 
            //    if(DateTime.Now >= DateTime.Parse("2013-09-16 00:00:00") && DateTime.Now<=  DateTime.Parse("2013-09-22 00:00:00"))
            //    {
            //        ViewBag.infomation = 1;
            //    }
            //}

            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            int vendor_id = Convert.ToInt32(vendor.vendor_id);
            return View();
        }
        /// <summary>
        /// 寄倉商品出貨列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Leavewithproduct()
        {
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            int vendor_id = Convert.ToInt32(vendor.vendor_id);
            return View();
        }
        /// <summary>
        /// 批次檢貨明細列印
        /// </summary>
        /// <returns></returns>
        public ActionResult PickingPrintView()
        {
            if (!string.IsNullOrEmpty(Request.Params["rowIDs"]))
            {
                string rowIDs = Request.Params["rowIDs"];
            }

            ViewBag.rowIDs = Request.Params["rowIDs"];

            return View();
        }
        /// <summary>
        /// 供應商後台出貨
        /// </summary>
        /// <returns></returns>

        public ActionResult AllOrderDeliverView()
        {
            if (!string.IsNullOrEmpty(Request.Params["times"]))
            {
                string times = Request.Params["times"];
            }
            if (!string.IsNullOrEmpty(Request.Params["rowIDs"]))
            {
                string rowIDs = Request.Params["rowIDs"];
            }
            ViewBag.times = Request.Params["times"];
            ViewBag.rowIDs = Request.Params["rowIDs"];
            return View();
        }
        /// <summary>
        /// 供應商自行出貨
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderVendorWaitDeliver()
        {
            return View();
        }
        /// <summary>
        /// 供應商自行出貨：點擊出貨時的詳細信息
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderToShippingDetails()
        {
            return View();
        }

        public ActionResult OrderVendorProduces()
        {
            ViewBag.ProduceGroupCsvPath = ProduceGroupCsvPath;
            return View();
        }
        #endregion

        #region 供應商自出商品
        /// <summary>
        /// 自出商品列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase VendorWaitDeliverList()
        {
            string jsonStr = String.Empty;
            StringBuilder sb = new StringBuilder();
            _IOrderSlaveMgr = new OrderSlaveMgr(mySqlConnectionString);

            try
            {
                List<OrderSlaveQuery> store = new List<OrderSlaveQuery>();
                OrderSlaveQuery query = new OrderSlaveQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                query.vendor_id = ((BLL.gigade.Model.Vendor)Session["vendor"]).vendor_id;//Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id);
                int totalCount = 0;
                store = _IOrderSlaveMgr.GetVendorWaitDeliver(query, sb.ToString(), out totalCount);//查询出供應商出貨單Vendor delivery
                int[] map = { 4, 1, 2, 3 };
                Dictionary<int, int[]> delivery_store_arrival_period = new Dictionary<int, int[]>();//參考路径:vendor.gigade100.com/order/order_wait_deliver.php第100行
                delivery_store_arrival_period.Add(1, map);
                delivery_store_arrival_period.Add(10, map);
                delivery_store_arrival_period.Add(16, map);
                delivery_store_arrival_period.Add(17, map);
                #region 到貨時段//0,不限時1,12:00以前
                List<Parametersrc> paramentTime = new List<Parametersrc>();//到貨時段
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                paramentTime = _ptersrc.GetAllKindType("Estimated_Arrival_Period");//物流業者
                DataTable _dtDelivery = new DataTable();
                _dtDelivery.Columns.Add("parametercode", typeof(String));
                _dtDelivery.Columns.Add("parameterName", typeof(String));
                _dtDelivery.Columns.Add("remark", typeof(String));
                foreach (var item in paramentTime)
                {
                    DataRow dr = _dtDelivery.NewRow();
                    dr[0] = item.ParameterCode;
                    dr[1] = item.parameterName;
                    dr[2] = item.remark;
                    _dtDelivery.Rows.Add(dr);
                }
                #endregion
                #region 台灣地區的參數
                DataTable _dtZip = new DataTable();
                _zipMgr = new ZipMgr(mySqlConnectionString);
                _dtZip = _zipMgr.ZipTable(null, null);
                #endregion

                foreach (var item in store)//為什麼是待出貨能，因為查詢的條件就是狀態為待出貨的
                {

                    item.status = "待出貨";
                    item.delivery = "";
                    item.pay_time = item.order_date_pay.ToString("yyyy/MM/dd HH:mm:ss");
                    if (delivery_store_arrival_period.Keys.Contains(item.delivery_store))
                    {
                        DataRow[] rows = _dtDelivery.Select("ParameterCode='" + item.estimated_arrival_period + "'");
                        foreach (DataRow row in rows)//篩選出的最多只有一條數據，
                        {
                            item.delivery = "";
                            if (!string.IsNullOrEmpty(row["ParameterCode"].ToString()))
                            {
                                item.delivery = row["ParameterName"].ToString();//---送貨時段
                            }
                        }
                    }
                    DataRow[] ziprows = _dtZip.Select("zipcode='" + item.delivery_zip + "'");
                    foreach (var ziprow in ziprows)
                    {
                        if (!string.IsNullOrEmpty(ziprow["zipcode"].ToString()))
                        {
                            item.delivery_address = item.delivery_zip + "  " + ziprow["middle"].ToString() + ziprow["small"].ToString() + item.delivery_address;
                        }
                    }
                }

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 供應商自行出貨信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeliveryInformation()
        {
            string jsonStr = String.Empty;
            StringBuilder sb = new StringBuilder();

            _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
            try
            {
                List<OrderDetailQuery> store = new List<OrderDetailQuery>();

                OrderDetailQuery query = new OrderDetailQuery();
                uint slave_id = 0;
                if (!string.IsNullOrEmpty(Request.Params["sid"]))
                {
                    slave_id = uint.Parse(Request.Params["sid"]);
                }
                query.Slave_Id = slave_id;

                query.Vendor_Id = ((BLL.gigade.Model.Vendor)Session["vendor"]).vendor_id;



                store = _OrderDetailMgr.DeliveryInformation(query, sb.ToString());//查询出供應商出貨單Vendor delivery

                #region 貨運單屬性
                List<Parametersrc> paramentFreight = new List<Parametersrc>();
                DataTable _dtProductFreightSet = new DataTable();
                _ptersrc = new ParameterMgr(mySqlConnectionString);

                paramentFreight = _ptersrc.GetAllKindType("product_freight");//物流業者

                _dtProductFreightSet.Columns.Add("parametercode", typeof(String));
                _dtProductFreightSet.Columns.Add("parameterName", typeof(String));
                _dtProductFreightSet.Columns.Add("remark", typeof(String));
                foreach (var item in paramentFreight)
                {
                    DataRow dr = _dtProductFreightSet.NewRow();
                    dr[0] = item.ParameterCode;
                    dr[1] = item.parameterName;
                    dr[2] = item.remark;
                    _dtProductFreightSet.Rows.Add(dr);
                }
                #endregion

                List<OrderDetailQuery> stores = new List<OrderDetailQuery>();
                foreach (var item in store)
                {
                    if (item.Combined_Mode >= 1 && item.Product_Mode == 1)
                        continue;
                    if (item.item_mode == 2)
                    {
                        item.Buy_Num = item.Buy_Num * item.parent_num;
                        // item.Sub_Total = item.Single_Money * item.parent_num;
                    }
                    //else 
                    //{
                    //    item.Sub_Total = item.Single_Money * item.Buy_Num;
                    //}
                    DataRow[] rows = _dtProductFreightSet.Select("ParameterCode='" + item.Product_Freight_Set + "'");
                    item.Product_Freight_Set_Str = item.Product_Freight_Set.ToString();
                    foreach (DataRow row in rows)//篩選出的最多只有一條數據，
                    {
                        if (!string.IsNullOrEmpty(row["ParameterCode"].ToString()))
                        {
                            item.Product_Freight_Set_Str = row["ParameterName"].ToString();//---送貨時段
                        }
                    }
                    if (!string.IsNullOrEmpty(item.Product_Spec_Name))
                    {
                        item.Product_Name = item.Product_Spec_Name;
                    }
                    stores.Add(item);

                }

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 供應商自行出貨確認
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SelfConfirmShipment()
        {
            string jsonStr = String.Empty;
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            BLL.gigade.Model.OrderSlaveMaster master = new OrderSlaveMaster();
            OrderDeliverQuery query = new OrderDeliverQuery();
            uint Slave_Id = 0;//要出貨的訂單；
            string Select_Did = "";//選中要出貨的出貨單號
            DateTime sendProTime = DateTime.MinValue;//出貨時間
            string delivery_note = "";//出貨單備註
            uint DeliverStores = 0;//物流單號
            string delivery_Code = "";//物流業者
            string IP = "";
            string Description = "";
            #region 獲取條件
            if (!string.IsNullOrEmpty(Request.Params["Slave_Id"]))
            {
                Slave_Id = uint.Parse(Request.Params["Slave_Id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["Select_Did"]))
            {
                Select_Did = Request.Params["Select_Did"];
            }
            if (!string.IsNullOrEmpty(Request.Params["sendProTime"]))
            {
                sendProTime = DateTime.Parse(Request.Params["sendProTime"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["delivery_note"]))
            {
                delivery_note = Request.Params["delivery_note"];
            }
            if (!string.IsNullOrEmpty(Request.Params["DeliverStores"]))
            {
                DeliverStores = uint.Parse(Request.Params["DeliverStores"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["delivery_Code"]))
            {
                delivery_Code = Request.Params["delivery_Code"];
            }
            #endregion
            string did = Select_Did.TrimEnd(',');//去掉最後一個分割符號
            string[] Sid_Row = did.Split(',');//拆分
            List<OrderDetailQuery> store = new List<OrderDetailQuery>();
            OrderDetailQuery querys = new OrderDetailQuery();
            querys.Slave_Id = Slave_Id;
            querys.Vendor_Id = vendor.vendor_id;
            _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
            store = _OrderDetailMgr.DeliveryInformation(querys, null);//查询出供應商出貨單Vendor delivery
            int reslut = 0;
            if (Sid_Row.Length != store.Count)
            {
                #region 轉換數據
                DataTable All_Did = new DataTable();
                All_Did.Columns.Add("detail_id", typeof(String));
                All_Did.Columns.Add("product_name", typeof(String));
                All_Did.Columns.Add("product_freight_set", typeof(String));
                All_Did.Columns.Add("product_spec_name", typeof(String));
                All_Did.Columns.Add("single_money", typeof(String));
                All_Did.Columns.Add("parent_name", typeof(String));
                All_Did.Columns.Add("parent_num", typeof(String));
                All_Did.Columns.Add("combined_mode", typeof(String));
                All_Did.Columns.Add("item_mode", typeof(String));
                All_Did.Columns.Add("buy_num", typeof(String));
                foreach (var item in store)
                {
                    DataRow dr = All_Did.NewRow();
                    dr[0] = item.Detail_Id;
                    dr[1] = item.Product_Name;
                    dr[2] = item.Product_Freight_Set;
                    dr[3] = item.Product_Spec_Name;
                    dr[4] = item.Single_Money;
                    dr[5] = item.parent_name;
                    dr[6] = item.parent_num;
                    dr[7] = item.Combined_Mode;
                    dr[8] = item.item_mode;
                    dr[9] = item.Buy_Num;
                    All_Did.Rows.Add(dr);
                }
                #endregion
                //
                //調用宋東亞
                _OrderDeliverMgr = new OrderDeliverMgr(mySqlConnectionString);
                _OrderDeliverMgr.DismantleSlave(int.Parse(querys.Slave_Id.ToString()), Select_Did, All_Did);
            }

            master.creator = vendor.vendor_id;
            System.Net.IPAddress[] addlist = Dns.GetHostByName(Dns.GetHostName()).AddressList;
            if (addlist.Length > 0)
            {
                IP = addlist[0].ToString();
            }

            try
            {
                OrderDeliver deliver = new OrderDeliver();
                deliver.slave_id = Slave_Id;
                deliver.deliver_store = DeliverStores;
                deliver.deliver_code = delivery_Code;
                deliver.deliver_note = delivery_note;
                deliver.deliver_time = uint.Parse(CommonFunction.GetPHPTime(sendProTime.ToString("yyyy-MM-dd")).ToString());
                deliver.deliver_ipfrom = IP;
                DataTable _dtSms = new DataTable();
                _dtSms.Columns.Add("sms_id", typeof(String));
                DataRow row = _dtSms.NewRow();//需發簡訊通知的item
                row[0] = 113552;
                _dtSms.Rows.Add(row);
                row = _dtSms.NewRow();
                row[0] = 113550;
                _dtSms.Rows.Add(row);
                Description = "Writer : vendor(" + vendor.vendor_id + ") " + vendor.vendor_name_simple;
                _OrderMgr = new OrderMgr(mySqlConnectionString);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                _OrderMgr.SelfThingsMethod(_dtSms, deliver, Description);//出貨成功！

                jsonStr = "{success:true}";//返回json數據
                //   jsonStr = "{success:true,msg:\"" + 1 + "\"}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 供應商調度出貨
        /// <summary>
        /// 供應商後台:訂單管理>供應商調度出貨
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetDeliverList()
        {
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            uint vendor_id = vendor.vendor_id;
            string json = String.Empty;
            StringBuilder sb = new StringBuilder();
            List<OrderSlaveQuery> store = new List<OrderSlaveQuery>();
            OrderSlaveQuery query = new OrderSlaveQuery();

            string jsonStr = String.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Search"]))
                {
                    sb.AppendFormat(" AND ( om.order_id = '{0}')", Request.Params["Search"]);
                }
                if (!string.IsNullOrEmpty(vendor_id.ToString()))
                {
                    query.vendor_id = vendor_id;
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                _IOrderSlaveMgr = new OrderSlaveMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _IOrderSlaveMgr.GetAllOrderWait(query, sb.ToString(), out totalCount);//查询出供應商出貨單
                for (int i = 0; i < store.Count; i++)
                {
                    store[i].pay_time = store[i].order_date_pay.ToString("yyyy/MM/dd HH:mm:ss");//轉單日期
                    store[i].status = "待出貨";
                    store[i].code = store[i].order_date_pay.AddDays(1).ToString("yyyyMMdd");//批次編號  (參照php:/vendor.gigade100.com/order/all_order_deliver.php第67行)
                    store[i].order_createdate = store[i].code;
                    if (i > 0)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (store[j].code == store[i].code)
                            {
                                store[i].code = "—";
                                break;
                            }
                        }
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 供應商後台:訂單管理>供應商調度出貨>要出的貨物信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AllOrderDeliver()
        {
            uint Normal_Subtotal = 0;//常溫商品總額--
            uint Hypothermia_Subtotal = 0;//低溫商品總額---
            uint All_Normal_Subtotal = 0;//當日常溫運費總額
            uint All_Hypothermia_Subtotal = 0;//當日低溫運費總額
            uint Order_Freight_Normal = 0;//常溫運費
            uint Order_Freight_Low = 0;//低溫運費
            uint All_Order_Freight_Normal = 0;//當日常溫運費--
            uint All_Order_Freight_Low = 0;//當日低溫運費--

            string json = string.Empty;
            try
            {
                string detail_id = "";
                if (!string.IsNullOrEmpty(Request.Params["rowIDs"]))
                {
                    detail_id = Request.Params["rowIDs"];
                }
                detail_id = detail_id.TrimEnd(',');//去掉最後一個分割符號
                string[] Sid_Row = detail_id.Split(',');//拆分
                string Sid = Sid_Row[Sid_Row.Length - 1];//獲取最後一個計算出出貨時間
                //通過sid去order_slave查詢order_id;
                //通過order_id查詢在order_master中order_date_pay。
                OrderSlaveQuery query = new OrderSlaveQuery();
                query.Slave_Id = uint.Parse(Sid);
                _IOrderSlaveMgr = new OrderSlaveMgr(mySqlConnectionString);
                DateTime time = _IOrderSlaveMgr.GetOrderDatePay(query).order_date_pay;
                long endtime = CommonFunction.GetPHPTime(time.ToString("yyyy/MM/dd 23:59:59"));
                uint vendor_id = (Session["vendor"] as BLL.gigade.Model.Vendor).vendor_id;
                //(計算這些數據參照自vendor.gigade100.com/order/all_order_deliver.php 第209~266行)
                #region 計算供應商當天的常溫和冷凍的總運費
                List<OrderDetailQuery> query1 = new List<OrderDetailQuery>();
                _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                query1 = _OrderDetailMgr.GetOrderDetailToSum(vendor_id, endtime);

                foreach (var item in query1)
                {
                    if (item.item_mode == 1)
                        continue;
                    if (item.Product_Freight_Set == PRODUCT_FREIGHT_NORMAL || item.Product_Freight_Set == PRODUCT_FREIGHT_NO_NORMAL)//常溫，常溫免運
                    {
                        if (item.item_mode == 2)//組合商品
                        {
                            All_Normal_Subtotal += item.Single_Price * item.parent_num;
                        }
                        else
                        {
                            All_Normal_Subtotal += item.Single_Price * item.Buy_Num;
                        }
                    }
                    else if (item.Product_Freight_Set == PRODUCT_FREIGHT_LOW || item.Product_Freight_Set == PRODUCT_FREIGHT_NO_LOW) //冷凍，冷凍免運
                    {
                        if (item.item_mode == 2)//組合商品
                        {
                            All_Hypothermia_Subtotal += item.Single_Price * item.parent_num;
                        }
                        else
                        {
                            All_Hypothermia_Subtotal += item.Single_Price * item.Buy_Num;
                        }
                    }
                }
                #endregion

                #region 計算運費


                OrderDetailQuery oddquery = new OrderDetailQuery();
                oddquery.Vendor_Id = vendor_id;
                _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                int total = 0;

                string sq = string.Format(" AND os.slave_id in ({0}) ORDER BY od.slave_id ASC ,od. combined_mode ASC , od.item_mode ASC ", detail_id);
                List<OrderDetailQuery> query2 = new List<OrderDetailQuery>();
                oddquery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                oddquery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "10");//用於分頁的變量
                query1 = _OrderDetailMgr.AllOrderDeliver(oddquery, out total, sq);

                //uint Normal_Subtotal = 0;//常溫商品總額--
                //uint Hypothermia_Subtotal = 0;//低溫商品總額---
                //uint All_Normal_Subtotal = 0;//當日常溫運費總額
                //uint All_Hypothermia_Subtotal = 0;//當日低溫運費總額
                //uint Order_Freight_Normal = 0;//常溫運費
                //uint Order_Freight_Low = 0;//低溫運費
                //uint All_Order_Freight_Normal = 0;//當日常溫運費--
                //uint All_Order_Freight_Low = 0;//當日低溫運費--


                foreach (var item in query1)
                {
                    if (item.item_mode == 1)
                        continue;
                    //商品總額計算(當下出貨金額與運費)
                    if (item.Product_Freight_Set == PRODUCT_FREIGHT_NORMAL || item.Product_Freight_Set == PRODUCT_FREIGHT_NO_NORMAL)//常溫，常溫免運
                    {
                        if (item.item_mode == 2)
                        {

                            Normal_Subtotal += item.Single_Price * item.parent_num;
                            item.Buy_Num = item.Buy_Num * item.parent_num;
                            item.Single_Price = item.Single_Price * item.parent_num;

                            //Normal_Subtotal += item.Single_Price * item.parent_num;//---
                            //item.Buy_Num = item.Buy_Num * item.parent_num;//-------
                            //item.Single_Money = item.Single_Price * item.Buy_Num;//---------
                        }
                        else
                        {
                            Normal_Subtotal += item.Single_Price * item.Buy_Num;
                            item.Single_Money = item.Single_Price * item.Buy_Num;
                        }
                    }
                    else if (item.Product_Freight_Set == PRODUCT_FREIGHT_LOW || item.Product_Freight_Set == PRODUCT_FREIGHT_NO_LOW)//低溫，低溫免運
                    {
                        if (item.item_mode == 2)
                        {
                            Hypothermia_Subtotal += item.Single_Price * item.parent_num;
                            item.Buy_Num = item.Buy_Num * item.parent_num;
                            item.Single_Price = item.Single_Price * item.parent_num;
                            //Hypothermia_Subtotal += item.Single_Price * item.parent_num;//--------
                            //item.Buy_Num = item.Buy_Num * item.parent_num;//------------
                            //item.Single_Money = item.Single_Price * item.Buy_Num;//---------
                        }
                        else
                        {
                            Hypothermia_Subtotal += item.Single_Price * item.Buy_Num;
                            item.Single_Money = item.Single_Price * item.Buy_Num;
                        }
                    }
                    query2.Add(item);
                }
                _VendorMgr = new VendorMgr(mySqlConnectionString);
                BLL.gigade.Model.Vendor vendor = new BLL.gigade.Model.Vendor();
                vendor.vendor_id = vendor_id;
                vendor = _VendorMgr.GetSingle(vendor);
                //當下運費計算
                if (Normal_Subtotal != 0)
                {
                    if (vendor.freight_normal_limit > Normal_Subtotal)
                    {
                        Order_Freight_Normal = vendor.freight_normal_money;
                    }
                }
                if (Hypothermia_Subtotal != 0)
                {
                    if (vendor.freight_low_limit > Hypothermia_Subtotal && Hypothermia_Subtotal > 0)
                    {
                        Order_Freight_Low = vendor.freight_low_money;
                    }
                }
                //批次單總額運費
                if (All_Normal_Subtotal != 0)
                {
                    if (vendor.freight_normal_limit > All_Normal_Subtotal)
                    {
                        All_Order_Freight_Normal = vendor.freight_normal_money;
                    }
                }
                if (All_Hypothermia_Subtotal != 0)
                {
                    if (vendor.freight_low_limit > All_Order_Freight_Low)
                    {
                        All_Order_Freight_Low = vendor.freight_low_money;
                    }
                }

                #endregion
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,Normal_Subtotal:" + Normal_Subtotal + ",Hypothermia_Subtotal:" + Hypothermia_Subtotal + ",Order_Freight_Normal:" + Order_Freight_Normal + ",Order_Freight_Low:" + Order_Freight_Low;
                json += ",All_Order_Freight_Normal:" + All_Order_Freight_Normal + ",All_Order_Freight_Low:" + All_Order_Freight_Low + ",data:" + JsonConvert.SerializeObject(query2, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        /// <summary>
        /// 調度出貨：確認出貨
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DispatchConfirmShipment()
        {
            string jsonStr = String.Empty;
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            BLL.gigade.Model.OrderSlaveMaster master = new OrderSlaveMaster();
            string rowIDs = "";//出貨單號
            string sendProTime = "";//出貨時間
            string delivery_note = "";//出貨單備註
            string DeliverStores = "";//物流單號
            string delivery_Code = "";//物流業者
            string IP = "";
            string Description = "";
            #region 條件
            if (!string.IsNullOrEmpty(Request.Params["rowIDs"]))
            {
                rowIDs = Request.Params["rowIDs"];
            }
            if (!string.IsNullOrEmpty(Request.Params["sendProTime"]))
            {
                sendProTime = Request.Params["sendProTime"];
                master.deliver_time = uint.Parse(CommonFunction.GetPHPTime(sendProTime).ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["delivery_note"]))
            {
                delivery_note = Request.Params["delivery_note"];
            }
            if (!string.IsNullOrEmpty(Request.Params["DeliverStores"]))
            {
                DeliverStores = Request.Params["DeliverStores"];
            }
            if (!string.IsNullOrEmpty(Request.Params["delivery_Code"]))
            {
                delivery_Code = Request.Params["delivery_Code"];
            }
            if (!string.IsNullOrEmpty(Request.Params["normal_Subtotal"]))
            {
                master.normal_subtotal = uint.Parse(Request.Params["normal_Subtotal"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["hypothermia_Subtotal"]))
            {
                master.hypothermia_subtotal = uint.Parse(Request.Params["hypothermia_Subtotal"]);
            }
            master.creator = vendor.vendor_id;
            //if (!string.IsNullOrEmpty(Request.Params["freight_Normal"]))
            //{
            //    master.fre = Request.Params["freight_Normal"];
            //}
            //if (!string.IsNullOrEmpty(Request.Params["hypothermia_Subtotal"]))
            //{
            //    hypothermia_Subtotal = Request.Params["hypothermia_Subtotal"];
            //}
            //if (!string.IsNullOrEmpty(Request.Params["freight_Low"]))
            //{
            //    freight_Low = Request.Params["freight_Low"];
            //}
            if (!string.IsNullOrEmpty(Request.Params["order_Freight_Normal"]))
            {
                master.order_freight_normal = uint.Parse(Request.Params["order_Freight_Normal"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["Order_Freight_Low"]))
            {
                master.order_freight_low = uint.Parse(Request.Params["Order_Freight_Low"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["code"]))
            {
                master.code_num = Request.Params["code"];
            }

            System.Net.IPAddress[] addlist = Dns.GetHostByName(Dns.GetHostName()).AddressList;
            if (addlist.Length > 0)
            {
                IP = addlist[0].ToString();
            }
            rowIDs = rowIDs.TrimEnd(',');//去掉最後一個分割符號
            string[] Sid_Row = rowIDs.Split(',');//拆分
            #endregion
            OrderDeliver order = new OrderDeliver();
            order.deliver_code = delivery_Code;
            order.deliver_store = uint.Parse(DeliverStores);
            order.deliver_note = delivery_note;
            order.deliver_ipfrom = IP;
            try
            {
                order.deliver_time = uint.Parse(CommonFunction.GetPHPTime(sendProTime).ToString());
                Description = "Writer : vendor(" + vendor.vendor_id + ") " + vendor.vendor_name_simple + "批次出貨";
                _OrderMgr = new OrderMgr(mySqlConnectionString);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                _OrderMgr.ThingsMethod(Sid_Row, order, master, Description);//出貨成功！
                // jsonStr = "{success:true}";//返回json數據
                jsonStr = "{success:true,msg:\"" + 1 + "\"}";
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 供應商後台:訂單管理>供應商調度出貨>批次檢貨明細列印
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase PickingPrintList()
        {
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            uint vendor_id = vendor.vendor_id;
            string json = String.Empty;
            StringBuilder sb = new StringBuilder();
            List<OrderDetailQuery> store = new List<OrderDetailQuery>();
            OrderDetailQuery query = new OrderDetailQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["rowIDs"]))
                {
                    string detail_id = Request.Params["rowIDs"];
                    detail_id = detail_id.TrimEnd(',');
                    sb.AppendFormat(" AND os.slave_id in ({0})", detail_id);
                }
                query.Vendor_Id = vendor_id;

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _OrderDetailMgr.VendorPickPrint(query, out totalCount, sb.ToString());//查询出供應商出貨單
                #region 為了監視數據，方便後期維護
                //DataTable dtH = new DataTable();
                //dtH.Columns.Add("item_id", typeof(String));
                //dtH.Columns.Add("product_name", typeof(string));
                //dtH.Columns.Add("order_id", typeof(String));
                //dtH.Columns.Add("order_name", typeof(String));
                //dtH.Columns.Add("delivery_name", typeof(String));
                //dtH.Columns.Add("buy_num", typeof(String));
                //dtH.Columns.Add("single_price", typeof(String));
                //dtH.Columns.Add("item_code", typeof(String));
                //dtH.Columns.Add("note_order", typeof(String));
                //foreach (var item in store)
                //{
                //     DataRow dr = dtH.NewRow();
                //        dr[0] = item.Item_Id;
                //        dr[1] = item.Product_Name;
                //        dr[2] = item.Order_Id;
                //        dr[3] = item.Order_Name;
                //        dr[4] =item.Delivery_Name;
                //        dr[5] = item.Buy_Num;
                //        dr[6] =item.Single_Price;
                //        dr[7] = item.Item_Code;
                //        dr[8] =item.Note_Order;
                //        dtH.Rows.Add(dr);
                //}
                #endregion
                Dictionary<string, List<OrderDetailQuery>> MergeDate = new Dictionary<string, List<OrderDetailQuery>>();//在這裡合併數據  
                //(合併數據,計算這些數據參照自vendor.gigade100.com/order/all_order_deliver_detail_print.php 第59~108行)
                #region 測試一次
                foreach (var item in store)//原數據循環
                {
                    if (item.Combined_Mode >= 1 && item.item_mode == 1)//提取组合商品，计算价格和数量
                        continue;

                    string item_id = item.Item_Id.ToString();
                    if (item.item_mode == 2)//子商品
                    {
                        item.Buy_Num = item.Buy_Num * item.parent_num;
                        item.Single_Price = item.Single_Price * item.parent_num;
                    }
                    else //母商品
                    {
                        item.Single_Price = item.Single_Price * item.Buy_Num;
                    }
                    if (!MergeDate.Keys.Contains(item_id))//不存在商品編號的添加
                    {
                        List<OrderDetailQuery> s = new List<OrderDetailQuery>();
                        MergeDate.Add(item_id, s);
                    }
                    MergeDate[item_id].Add(item);//通過商品編號把相同商品房在同一個集合里
                }
                StringBuilder html_table = new StringBuilder();//匯出html頁面
                html_table.AppendFormat(@"<div>&nbsp;<table style='border:1px solid; text-align:center'>");//550//滚动条overflow:auto;
                html_table.AppendFormat("<tr style='background-color:Lime'><td style='height:50px;border:1px solid;'>商品編號</td><td style='border:1px solid; '>商品名稱</td><td style='border:1px solid;'>數量</td>");
                html_table.AppendFormat("<td style='border:1px solid;'>廠商自訂編號</td><td style='border:1px solid;'>付款單編號</td><td style='border:1px solid;'>訂購人</td>");
                html_table.AppendFormat(" <td style='border:1px solid;'>收件人</td><td style='border:1px solid;'>數量</td><td style='border:1px solid;'>總價</td><td style='border:1px solid;'>備註</td></tr>");
                //   int subtotal = 1;
                foreach (var item in MergeDate.Keys)//這個循環商品編號，然後再循環同一種編號下面的集合
                {
                    uint SumNo = 0;//計算總數量
                    foreach (var items in MergeDate[item])//循環同一種商品編號下面的集合
                    {
                        SumNo += items.Buy_Num;
                    }
                    int r = 0;
                    foreach (var items in MergeDate[item])//循環同一種商品編號下面的集合
                    {
                        int row = MergeDate[item].Count;//合併的列
                        //if (subtotal % 28 == 0)
                        //{
                        //    html_table.AppendFormat("<tr style='page-break-after:always;'>");//
                        //}
                        //else
                        //{
                        html_table.AppendFormat("<tr>");
                        //}
                        if (r == 0)//合併數據
                        {
                            html_table.AppendFormat("<td rowspan='{0}' style='border:1px solid;'>{1}</td>", row, items.Item_Id);//商品編號
                            items.Product_Name = items.Product_Name.Replace("\n", "");
                            html_table.AppendFormat("<td rowspan='{0}' style='border:1px solid;'>{1}</td>", row, items.Product_Name);//商品名稱
                            html_table.AppendFormat("<td rowspan='{0}' style='border:1px solid;'>{1}</td>", row, SumNo);//數量
                            html_table.AppendFormat("<td rowspan='{0}' style='border:1px solid;'>{1}</td>", row, items.Item_Code);//廠商自訂編號
                        }
                        html_table.AppendFormat("<td style='border:1px solid;'>{0}</td>", items.Order_Id);//付款單編號
                        html_table.AppendFormat("<td style='border:1px solid;'>{0}</td>", items.Order_Name);//訂購人
                        html_table.AppendFormat("<td style='border:1px solid;'>{0}</td>", items.Delivery_Name);//收件人
                        html_table.AppendFormat("<td style='border:1px solid;'>{0}</td>", items.Buy_Num);//數量
                        html_table.AppendFormat("<td style='border:1px solid;'>{0}</td>", items.Single_Price);//總價
                        items.Note_Order = items.Note_Order.Replace("\n", "");
                        html_table.AppendFormat("<td style='border:1px solid;'>{0}</td>", items.Note_Order);//備註
                        //subtotal += 1;
                        html_table.AppendFormat("</tr>");
                        r++;
                    }
                }
                #endregion
                html_table.AppendFormat("</table></div>");

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                string n = html_table.ToString();
                json = "{success:true,msg:\"" + html_table.ToString() + "\"}";
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

        #region 寄倉商品出貨列表
        public HttpResponseBase GetLeaveproductList()
        {
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            uint vendor_id = vendor.vendor_id;
            StringBuilder sb = new StringBuilder();
            OrderDetailQuery query = new OrderDetailQuery();
            string jsonStr = String.Empty;
            try
            {
                if (Convert.ToInt32(Request.Params["serch_type"]) == 1)//等於1表示選擇了商品名稱
                {
                    if (!string.IsNullOrEmpty(Request.Params["searchcomment"].Trim()))
                    {
                        sb.AppendFormat(" AND od.product_name LIKE '%{0}%'", Request.Params["searchcomment"]);
                    }
                }
                if (Convert.ToInt32(Request.Params["serch_time_type"]) == 1)//等於1表示選擇了出貨日期
                {
                    if (!string.IsNullOrEmpty(Request.Params["time_start"].Trim()) && !string.IsNullOrEmpty(Request.Params["time_end"].Trim()))
                    {
                        sb.AppendFormat("AND os.slave_date_delivery >= '{0}' AND os.slave_date_delivery <='{1}' ", CommonFunction.GetPHPTime(Request.Params["time_start"]), CommonFunction.GetPHPTime(Request.Params["time_end"]));
                    }
                }
                //query.Search_Type = Convert.ToInt32(Request.Params["serch_type"]);
                //if (!string.IsNullOrEmpty(Request.Params["searchcomment"].Trim()))
                //{
                //    query.SearchComment = Request.Params["searchcomment"].Trim();
                //}
                //query.Serch_Time_Type = Convert.ToInt32(Request.Params["serch_time_type"]);
                if (!string.IsNullOrEmpty(vendor_id.ToString()))
                {
                    query.Vendor_Id = vendor_id;
                }
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                DataTable dt = new DataTable();
                _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                int totalCount = 0;
                dt = _OrderDetailMgr.GetLeaveproductList(query, out totalCount, sb.ToString());//查询出供應商出貨單
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter) + "}";//返回json數據
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(jsonStr.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 物流業者
        //[OutputCache(Duration = 3600, VaryByParam = "paraType", Location = System.Web.UI.OutputCacheLocation.Client)]
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
                json = "[]";
            }
            return json;
        }
        #endregion

        #region 出貨時間
        public string SendProTime()
        {
            string json = string.Empty;
            List<string> _dt = new List<string>();
            List<string> _time = new List<string>();
            string _newDt = string.Empty; ;
            DateTime _dtNow = DateTime.Now;
            string week = string.Empty;
            for (int i = -3; i <= 3; i++)
            {
                _newDt = _dtNow.AddDays(i).ToString("yyyy/MM/dd");
                week = TransferWeek(_dtNow.AddDays(i).DayOfWeek.ToString());
                _dt.Add(_newDt + "( " + week + ")");
                _time.Add(_newDt);
            }
            //   json = "{success:true,items:[{\"day\":\"" + _dt[0] +" \",\"week\":\"" + 1 + "\"}]}";
            StringBuilder stb = new StringBuilder();
            stb.Append("{success:true,items:[");
            for (int i = 0; i < _dt.Count; i++)
            {
                stb.Append("{");
                stb.Append(string.Format("\"day\":\"{0}\",\"time\":\"{1}\"", _dt[i], _time[i]));
                stb.Append("}");
            }
            stb.Append("]}");
            return stb.ToString().Replace("}{", "},{");

        }
        public string TransferWeek(string week)
        {
            string transfer = string.Empty;
            switch (week)
            {
                case "Monday":
                    transfer = "一";
                    break;
                case "Tuesday":
                    transfer = "二";
                    break;
                case "Wednesday":
                    transfer = "三";
                    break;
                case "Thursday":
                    transfer = "四";
                    break;
                case "Friday":
                    transfer = "五";
                    break;
                case "Saturday":
                    transfer = "六";
                    break;
                case "Sunday":
                    transfer = "日";
                    break;
            }
            return transfer;
        }
        #endregion

        #region 商品/訂單查詢
        #region 列表
        public HttpResponseBase OrderVendorProducesList()
        {
            OrderDetailQuery query = new OrderDetailQuery();
            List<OrderDetailQuery> store = new List<OrderDetailQuery>();
            string json = string.Empty;
            int totalCount = 0;
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            try
            {
                query.Vendor_Id = vendor.vendor_id;
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["select_type"]))
                {
                    query.select_type = Convert.ToInt32(Request.Params["select_type"]);
                    if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                    {
                        query.select_con = Request.Params["search_con"];
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.date = Convert.ToInt32(Request.Params["date"]);
                    if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                    {
                        query.start_time = Convert.ToDateTime(Request.Params["start_time"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                    {
                        query.end_time = Convert.ToDateTime(Request.Params["end_time"]);
                    }
                }
                query.radiostatus = Convert.ToInt32(Request.Params["radiostatus"]);
                query.promodel = Convert.ToInt32(Request.Params["proModel"]);
                _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                store = _OrderDetailMgr.OrderVendorProducesList(query, out totalCount);
                foreach (var item in store)
                {
                    if (item.slave_date_delivery != 0)
                    {
                        item.s_slave_date_delivery = CommonFunction.DateTimeToString(CommonFunction.GetNetTime(item.slave_date_delivery));
                    }
                    else
                    {
                        //還沒有出貨不顯示出貨時間
                        item.s_slave_date_delivery = "-";
                    }
                    if (item.item_mode == 2)
                    {
                        item.Buy_Num = item.Buy_Num * item.parent_num;
                    }
                    if (item.spec_image != "")
                    {
                        item.spec_image = imgServerPath + specPath + item.spec_image.Substring(0, 2) + "/" + item.spec_image.Substring(2, 2) + "/" + item.spec_image;
                    }
                }
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
                json = "{success:false,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 匯出CSV
        public HttpResponseBase ProduceGroupCsv()
        {
            string json = string.Empty;
            OrderDetailQuery query = new OrderDetailQuery();
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            try
            {
                query.Vendor_Id = vendor.vendor_id;
                if (!string.IsNullOrEmpty(Request.Params["select_type"]))
                {
                    query.select_type = Convert.ToInt32(Request.Params["select_type"]);
                    if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                    {
                        query.select_con = Request.Params["search_con"];
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.date = Convert.ToInt32(Request.Params["date"]);
                    if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                    {
                        query.start_time = Convert.ToDateTime(Request.Params["start_time"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                    {
                        query.end_time = Convert.ToDateTime(Request.Params["end_time"]);
                    }
                }
                query.radiostatus = Convert.ToInt32(Request.Params["radiostatus"]);
                query.promodel = Convert.ToInt32(Request.Params["proModel"]);
                _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                DataTable _dt = _OrderDetailMgr.ProduceGroupCsv(query);
                string[] columnName = { "商品細項編號", "商品名稱", "數量" };
                string fileName = "ovp_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                string newFileName = Server.MapPath(ProduceGroupCsvPath) + fileName;
                string title = "撿貨單" + query.start_time.ToString("yyyy/MM/dd") + "至" + query.end_time.ToString("yyyy/MM/dd");
                CsvHelper.TitleCsv(_dt, newFileName, title, columnName, true);
                json = "{success:'true',filename:'" + fileName + "'}";
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

        #region 匯出EXCEL
        public HttpResponseBase ProduceGroupExcel()
        {
            string json = string.Empty;
            OrderDetailQuery query = new OrderDetailQuery();
            BLL.gigade.Model.Vendor vendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            try
            {
                query.Vendor_Id = vendor.vendor_id;
                if (!string.IsNullOrEmpty(Request.Params["select_type"]))
                {
                    query.select_type = Convert.ToInt32(Request.Params["select_type"]);
                    if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                    {
                        query.select_con = Request.Params["search_con"];
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.date = Convert.ToInt32(Request.Params["date"]);
                    if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                    {
                        query.start_time = Convert.ToDateTime(Request.Params["start_time"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                    {
                        query.end_time = Convert.ToDateTime(Request.Params["end_time"]);
                    }
                }
                query.radiostatus = Convert.ToInt32(Request.Params["radiostatus"]);
                query.promodel = Convert.ToInt32(Request.Params["proModel"]);
                _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                DataTable _dt = _OrderDetailMgr.ProduceGroupExcel(query);
                string[] ColumnName = { "付款單號", "供應商", "商品名稱", "規格", "數量", "訂購姓名", "狀態", "出貨日期", "備註" };
                string headerText = "報表";
                string fileName = "報表.xls";
                string newFileName = Server.MapPath(ProduceGroupCsvPath) + fileName;
                #region 轉到新錶

                DataTable _newDt = new DataTable();
                for (int i = 0; i < ColumnName.Length; i++)
                {

                    _newDt.Columns.Add(ColumnName[i], typeof(string));
                }
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow _newRow = _newDt.NewRow();
                    for (int j = 0; j < _dt.Columns.Count; j++)
                    {


                        if (j == 7)
                        {
                            _newRow[j] = _dt.Rows[i][j];
                            if (_newRow[j].ToString() == "0")
                            {
                                _newRow[j] = "-";
                            }
                            else
                            {
                                _newRow[j] = CommonFunction.GetNetTime(Convert.ToInt32(_dt.Rows[i][j])).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                        else if (j == 4)
                        {
                            if (_dt.Rows[i]["item_mode"].ToString() == "2")
                            {
                                _newRow[j] = Convert.ToInt32(_dt.Rows[i]["buy_num"]) * Convert.ToInt32(_dt.Rows[i]["parent_num"]);
                            }
                            else
                            {
                                _newRow[j] = _dt.Rows[i]["buy_num"];
                            }
                        }
                        else
                        {
                            if (j == 0 || j == 1 || j == 2 || j == 3 || j == 5 || j == 6 || j == 8)
                            {
                                _newRow[j] = _dt.Rows[i][j];
                            }
                        }

                    }
                    _newDt.Rows.Add(_newRow);
                }
                #endregion
                ExcelHelperXhf.ExportDTtoExcel(_newDt, headerText, newFileName);
                json = "{success:'true',filename:'" + fileName + "'}";
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

        public HttpResponseBase GetOrderDeliverDetail()
        {
            string json = string.Empty;
            string order_id = Request.Params["oid"];
            string p_mode = Request.Params["p_mode"];
            BLL.gigade.Model.Vendor modelVendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            //OrderSlave
            OrderSlaveQuery orderSlaveQuery = new OrderSlaveQuery()
            {
                order_id = Convert.ToUInt32(order_id),
                vendor_id = modelVendor.vendor_id,
                Slave_Status = 2,//待出貨
                IsPage = false
            };
            OrderSlaveQuery orderSlave = new OrderSlaveQuery();
            DataTable dtOrderSlave = new DataTable();
            _OrderSlaveMgr = new OrderSlaveMgr(mySqlConnectionString);
            int totalCount_OrderSlave = 0;

            //OrderDetail
            OrderDetailQuery orderDetailQuery = new OrderDetailQuery();
            //orderDetailQuery.Order_Id = uint.Parse(order_id);
            List<OrderDetailQuery> list = new List<OrderDetailQuery>();
            _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
            int totalCount_OrderDetail = 0;

            try
            {
                dtOrderSlave = _OrderSlaveMgr.GetList(orderSlaveQuery, out totalCount_OrderSlave);
                if (dtOrderSlave.Rows.Count > 0)
                {
                    //orderSlave
                    orderSlave.order_id = Convert.ToUInt32(dtOrderSlave.Rows[0]["order_id"]);
                    orderSlave.order_createdate = dtOrderSlave.Rows[0]["order_createdate"].ToString();
                    orderSlave.pay_time = dtOrderSlave.Rows[0]["order_date_pay"].ToString();
                    orderSlave.order_name = dtOrderSlave.Rows[0]["order_name"].ToString();
                    orderSlave.delivery_name = dtOrderSlave.Rows[0]["delivery_name"].ToString();
                    orderSlave.estimated_arrival_period = Convert.ToInt32(dtOrderSlave.Rows[0]["estimated_arrival_period"]);
                    orderSlave.note_order = dtOrderSlave.Rows[0]["note_order"].ToString();
                    orderSlave.dispatch = modelVendor.dispatch;
                    orderSlave.delivery_zip = Convert.ToUInt32(dtOrderSlave.Rows[0]["delivery_zip"]);
                    orderSlave.delivery_address = dtOrderSlave.Rows[0]["delivery_address"].ToString();
                    orderSlave.delivery_mobile = dtOrderSlave.Rows[0]["delivery_mobile"].ToString();

                    //供查詢列表使用
                    orderDetailQuery.Slave_Id = Convert.ToUInt32(dtOrderSlave.Rows[0]["slave_id"]);
                }

                list = _OrderDetailMgr.GetOrderDetailList(orderDetailQuery, out totalCount_OrderDetail);
                List<OrderDetailQuery> stroes = new List<OrderDetailQuery>();
                stroes = getList(list);
                string html = GetOrderDeliverDetailHtml(orderSlave, stroes);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                //json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
                json = "{success:true,msg:\"" + html + "\"}";
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

        public HttpResponseBase GetOrderDeliverDetailList()
        {
            StringBuilder addsql = new StringBuilder();
            string json = string.Empty;
            string startTime = Request.Params["start_time"];
            string endTime = Request.Params["end_time"];

            if (!string.IsNullOrEmpty(startTime))
            {
                addsql.AppendFormat(" and om.order_date_pay>='{0}' ", CommonFunction.GetPHPTime(DateTime.Parse(startTime).ToString("yyyy/MM/dd 00:00:00")));
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                addsql.AppendFormat(" and om.order_date_pay<='{0}' ", CommonFunction.GetPHPTime(DateTime.Parse(endTime).ToString("yyyy/MM/dd 23:59:59")));
            }
            addsql.Append(" 	AND od.detail_status = 2	AND product_mode = 1 ");

            BLL.gigade.Model.Vendor modelVendor = Session["vendor"] as BLL.gigade.Model.Vendor;
            StringBuilder sbHtml = new StringBuilder();
            
            try
            {
                //OrderSlave
                OrderSlaveQuery orderSlaveQuery = new OrderSlaveQuery()
                {
                    vendor_id = modelVendor.vendor_id,
                    Slave_Status = 2,//待出貨
                    IsPage = false
                };
                OrderSlaveQuery orderSlave = new OrderSlaveQuery();
                DataTable dtOrderSlave = new DataTable();
                _OrderSlaveMgr = new OrderSlaveMgr(mySqlConnectionString);

                //OrderDetail
                OrderDetailQuery orderDetailQuery = new OrderDetailQuery();

                List<OrderDetailQuery> list = new List<OrderDetailQuery>();
                _OrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                int totalCount_OrderDetail = 0;
                Dictionary<OrderDetailQuery, List<OrderDetailQuery>> product_freight_set_mapping = new Dictionary<OrderDetailQuery, List<OrderDetailQuery>>();

                //根據時間搜索order_id
                dtOrderSlave = _OrderSlaveMgr.GetListPrint(orderSlaveQuery, addsql.ToString());
                if (dtOrderSlave.Rows.Count > 0)
                {
                    //循環order_id
                    foreach (DataRow item in dtOrderSlave.Rows)
                    {
                        //orderSlave
                        orderSlave.order_id = Convert.ToUInt32(item["order_id"]);
                        orderSlave.order_createdate = Convert.ToDateTime(item["order_createdate"]).ToString("yyyy-MM-dd HH:mm:ss");
                        orderSlave.pay_time = Convert.ToDateTime(item["order_date_pay"]).ToString("yyyy-MM-dd HH:mm:ss");
                        orderSlave.order_name = item["order_name"].ToString();
                        orderSlave.delivery_name = item["delivery_name"].ToString();
                        orderSlave.estimated_arrival_period = Convert.ToInt32(item["estimated_arrival_period"]);
                        orderSlave.note_order = item["note_order"].ToString();
                        orderSlave.dispatch = 0;
                        orderSlave.delivery_zip = Convert.ToUInt32(item["delivery_zip"]);
                        orderSlave.delivery_address = item["delivery_address"].ToString();
                        orderSlave.delivery_mobile = item["delivery_mobile"].ToString();

                        //供查詢列表使用
                        orderDetailQuery.Slave_Id = Convert.ToUInt32(item["slave_id"]);
                        list = _OrderDetailMgr.GetOrderDetailList(orderDetailQuery, out totalCount_OrderDetail);
                        sbHtml.Append(GetOrderDeliverDetailHtml(orderSlave, list));
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                //json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
                json = "{success:true,msg:\"" + sbHtml.ToString() + "\"}";
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

        public List<OrderDetailQuery> getList(List<OrderDetailQuery> list)
        {
           
             List<OrderDetailQuery> one_product = new List<OrderDetailQuery>();		//單一商品
             List<OrderDetailQuery> combination = new List<OrderDetailQuery>();		//組合商品
            List<OrderDetailQuery> combination_head = new List<OrderDetailQuery>();		//組合品名
            List<OrderDetailQuery> combination_tail = new List<OrderDetailQuery>();		//子商品名
        
        
            foreach (var item in list)
            {
                if (item.Combined_Mode > 1)
                {
                    if (item.item_mode == 1)
                        combination_head.Add(item);
                    else
                        combination_tail.Add(item);
                }
                else
                {
                    one_product.Add(item);
                }
            }
          
            foreach (var items in combination_head)
            {
                combination.Add(items);
                foreach (var item in combination_tail)
                {
                    if (items.Parent_Id == item.Parent_Id && items.pack_id == item.pack_id)
                    {
                        combination.Add(item);
                    }
                }
            }
       
            foreach (var item in combination)
            {
                one_product.Add(item);
            }
            return one_product;
        }
        //public ActionResult OrderDeliverDetail()
        //{
        //    string order_id = Request.Params["order_id"];
        //    OrderSlaveMasterQuery orderSlaveMasterQuery = new OrderSlaveMasterQuery();
        //    //orderSlaveMasterQuery=_OrderDetailMgr.

        //    OrderDetailQuery query = new OrderDetailQuery();
        //    query.Order_Id = uint.Parse(order_id);
        //    List<OrderDetailQuery> listOrderDetail = new List<OrderDetailQuery>();
        //    int totalCount = 0;
        //    listOrderDetail = _OrderDetailMgr.GetOrderDetailList(query, out totalCount);

        //    ViewBag.orderSlave = null;
        //    ViewBag.listOrderDetail = listOrderDetail;
        //    return View();
        //}

        /// <summary>
        /// 組織要打印的出貨單html
        /// </summary>
        /// <returns>html字符串</returns>
        private string GetOrderDeliverDetailHtml(OrderSlaveQuery model, List<OrderDetailQuery> list)
        {
            StringBuilder sbHtml = new StringBuilder();
            //增加樣式
            sbHtml.Append("<style>");
            sbHtml.Append(".pagenum {text-align: center; font: 400 12px/25px Verdana, '新細明體'; color: #686D73; margin: 15px 0; }");
            sbHtml.Append(".pagenum a { color: #686D73; text-decoration: none; }");
            sbHtml.Append(".pagenum a:hover { color: #686D73; text-decoration: underline; }");
            sbHtml.Append(".pagenum span.select { color: #FC6B00; font-weight: 700; }");
            sbHtml.Append(".black12 { font-family: Arial, Helvetica, sans-serif; font-size: 12px; line-height: 20px; color: #000000; text-decoration: none; }");
            sbHtml.Append(".line { border-top: dotted 1px #1d7da2; height: 0px; margin: 15px 0; clear: both; font-size: 0px; line-height: 0px; }");
            sbHtml.Append(".arial { font-family: Arial; font-size: 14px; }");
            sbHtml.Append("#main { margin: 20px; border: 1px solid #a8a8a8; width: 510px; }");
            sbHtml.Append("#main h1 { font-size: 16px; line-height: 30px; color: #909090; padding: 5px 20px 0 20px; margin: 10px 0; font-family: '微軟正黑體'; }");
            sbHtml.Append("#main h2 { font-size: 16px; line-height: 30px; color: #909090; padding: 0 20px; font-family: '微軟正黑體'; font-weight: normal;margin: 0; }");
            sbHtml.Append("#main h3 { font-size: 16px; line-height: 30px; color: #000000; padding: 0 20px; font-family: '微軟正黑體'; font-weight: normal; margin: 15px 60px 0 60px; }");
            sbHtml.Append("#main h4 { font-size: 20px; line-height: 35px; color: #000000; padding: 0 20px; font-family: '微軟正黑體'; font-weight: normal; margin: 0 20px 20px 60px; }");
            sbHtml.Append("</style>");
            //html內容
            sbHtml.Append("<div><img src='/Content/img/add_01.gif' width='550' height='106' alt='' /></div>");
            sbHtml.Append("<table width='700' border='0' cellpadding='2' cellspacing='0' bordercolor='#666666'>");
            sbHtml.Append(" <tr>");
            sbHtml.Append("     <td valign='top'>");
            sbHtml.Append("         <table width='100%' border='0' cellspacing='0' cellpadding='3'>");
            sbHtml.Append("             <tr>");
            sbHtml.Append("                 <td height='3'></td>");
            sbHtml.Append("             </tr>");
            sbHtml.Append("         </table>");
            sbHtml.Append("         <p class='pagenum'></p>");
            sbHtml.Append("         <table width='100%' border='0' align='right' cellpadding='0' cellspacing='0'>");
            sbHtml.Append("             <tr>");
            sbHtml.Append("                 <td width='13' height='17' background='/Content/img/01.gif'></td>");
            sbHtml.Append("                 <td background='/Content/img/02.gif'></td>");
            sbHtml.Append("                 <td width='17' height='17' background='/Content/img/06.gif'></td>");
            sbHtml.Append("             </tr>");
            sbHtml.Append("             <tr>");
            sbHtml.Append("                 <td background='/Content/img/03.gif'>&nbsp;</td>");
            sbHtml.Append("                 <td>");
            sbHtml.Append("                     <table width='100%' border='0' align='center' cellpadding='5' cellspacing='0'>");
            sbHtml.Append("                         <tr>");
            sbHtml.Append("                             <td>");
            sbHtml.Append("                                 <div align='left' class='black12'>");
            sbHtml.Append("                                     <div align='center'>吉甲地在地好物市集網站訂購明細</div>");
            sbHtml.Append("                                 </div>");
            sbHtml.Append("                             </td>");
            sbHtml.Append("                         </tr>");
            sbHtml.Append("                         <tr>");
            sbHtml.Append("                             <td>");
            sbHtml.Append("                                 <table width='100%' border='0' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("                                     <tr background='/Content/img/05.gif'>");
            //sbHtml.Append("                                         <td align='left'>付款單號： {{$order_id}}</td>");
            sbHtml.AppendFormat("                                         <td align='left'>付款單號： {0}</td>", model.order_id);
            sbHtml.Append("                                         <td></td>");
            sbHtml.Append("                                         <td></td>");
            sbHtml.Append("                                     </tr>");
            sbHtml.Append("                                     <tr background='/Content/img/05.gif'>");
            //sbHtml.Append("                                         <td align='left'>訂購日期：{{$a_order_master.order_createdate}}</td>");
            sbHtml.AppendFormat("                                         <td align='left'>訂購日期：{0}</td>", model.order_createdate);
            sbHtml.Append("                                         <td></td>");
            //sbHtml.Append("                                         <td align='left'>轉單日期：{{$a_order_master.order_date_pay}}</td>");
            sbHtml.AppendFormat("                                         <td align='left'>轉單日期：{0}</td>", model.pay_time);
            sbHtml.Append("                                     </tr>");
            sbHtml.Append("                                     <tr background='/Content/img/05.gif'>");
            //sbHtml.Append("                                         <td align='left'>訂購人：{{$a_order_master.order_name}}</td>");
            sbHtml.AppendFormat("                                         <td align='left'>訂購人：{0}</td>", model.order_name);
            sbHtml.Append("                                         <td></td>");
            //sbHtml.Append("                                         <td align='left'>收貨人：{{$a_order_master.delivery_name}}</td>");
            sbHtml.AppendFormat("                                         <td align='left'>收貨人：{0}</td>", model.delivery_name);
            sbHtml.Append("                                     </tr>");
            sbHtml.Append("                                     <tr background='/Content/img/05.gif'>");
            //sbHtml.Append("                                         <td align='left'>到貨時段：{{$a_order_master.estimated_arrival_period}}</td>");
            string strEstimatedArrivalPeriod = string.Empty;
            if (model.estimated_arrival_period == 0)
            {
                strEstimatedArrivalPeriod = "不限時";
            }
            else if (model.estimated_arrival_period == 1)
            {
                strEstimatedArrivalPeriod = "12:00以前";
            }
            else if (model.estimated_arrival_period == 2)
            {
                strEstimatedArrivalPeriod = "12:00-17:00";
            }
            else if (model.estimated_arrival_period == 3)
            {
                strEstimatedArrivalPeriod = "17:00-20:00";
            }
            sbHtml.AppendFormat("                                         <td align='left'>到貨時段：{0}</td>", strEstimatedArrivalPeriod);
            sbHtml.Append("                                         <td></td>");
            sbHtml.Append("                                         <td></td>");
            sbHtml.Append("                                     </tr>");
            sbHtml.Append("                                     <tr background='/Content/img/05.gif'>");
            //sbHtml.Append("                                         <td align='left'>備註：{{$a_order_master.note_order}}</td>");
            sbHtml.AppendFormat("                                         <td align='left'>備註：{0}</td>", model.note_order);
            sbHtml.Append("                                         <td></td>");
            sbHtml.Append("                                         <td></td>");
            sbHtml.Append("                                     </tr>");
            sbHtml.Append("                                 </table>");
            sbHtml.Append("                                 <div class='line'></div>");
            sbHtml.Append("                                 <table width='100%' border='1'  cellpadding='0' cellspacing='1' class='disc_table'>");
            sbHtml.Append("                                     <tr align='center'>");
            sbHtml.Append("                                         <th>商品編號</th>");
            sbHtml.Append("                                         <th>廠商自訂編號</th>");
            sbHtml.Append("                                         <th>售價</th>");
            sbHtml.Append("                                         <th>商品名稱</th>");
            sbHtml.Append("                                         <th>數量</th>");
            sbHtml.Append("                                     </tr>");
            //{{if !empty($a_order_detail) }}
            //  {{foreach key=key item=value from=$a_order_detail}}
            //      <tr align="center">
            //          <td>{{if ($value.combined_mode >= 1 and $value.item_mode == 1)}}{{$value.parent_id}}{{else}}{{$value.item_id}}{{/if}}</td>
            //          <td>{{$value.item_code}}</td>
            //          <td>{{if $value.item_mode != 1}}{{$value.single_money}}{{/if}}</td>
            //          <td  align="left">{{if ($value.combined_mode >= 1 and $value.item_mode == 2)}}&nbsp;&nbsp;&nbsp;&nbsp;＊{{$value.product_name}}{{$value.product_spec_name}}{{else}}{{$value.product_name}}{{$value.product_spec_name}}{{/if}}<!--{{if !empty($value.detail_note)}}<br /><span style="color:#f0f;">&nbsp;&nbsp;{{$value.detail_note}}</span>{{/if}}--></td>
            //          <td>{{$value.buy_num|number_format}}</td>                               
            //      </tr>
            //  {{/foreach}}
            //{{/if}}
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    sbHtml.Append("                                     <tr align='center'>");
                    sbHtml.Append("                                         <td>");
                    if (item.Combined_Mode >= 1 && item.item_mode == 1)
                    {
                        sbHtml.Append(item.Parent_Id);
                    }
                    else
                    {
                        sbHtml.Append(item.Item_Id);
                    }
                    sbHtml.Append("                                         </td>");
                    sbHtml.AppendFormat("                                         <td>{0}</td>", item.Item_Code);
                    sbHtml.Append("                                         <td>");
                    if (item.item_mode != 1)
                    {
                        sbHtml.Append(item.Single_Money);
                    }
                    sbHtml.Append("                                         </td>");
                    sbHtml.Append("                                         <td  align='left'>");
                    if (item.Combined_Mode >= 1 && item.item_mode == 2)
                    {
                        sbHtml.AppendFormat("&nbsp;&nbsp;&nbsp;&nbsp;＊{0}{1}", item.Product_Name, item.Product_Spec_Name);
                    }
                    else
                    {
                        sbHtml.AppendFormat("{0}{1}", item.Product_Name, item.Product_Spec_Name);
                    }
                    sbHtml.Append("                                         </td>");
                    sbHtml.AppendFormat("                                         <td>{0}</td>", item.Buy_Num);
                    sbHtml.Append("                                     </tr>");
                }
            }
            sbHtml.Append("                                 </table>");
            sbHtml.Append("                             </td>");
            sbHtml.Append("                         </tr>");
            sbHtml.Append("                     </table>");
            sbHtml.Append("                 </td>");
            sbHtml.Append("                 <td background='/Content/img/07.gif'>&nbsp;</td>");
            sbHtml.Append("             </tr>");
            sbHtml.Append("             <tr>");
            sbHtml.Append("                 <td background='/Content/img/04.gif'></td>");
            sbHtml.Append("                 <td background='/Content/img/05.gif'>&nbsp;&nbsp;</td>");
            sbHtml.Append("                 <td background='/Content/img/08.gif'></td>");
            sbHtml.Append("             </tr>");
            sbHtml.Append("         </table>");
            sbHtml.Append("     </td>");
            sbHtml.Append(" </tr>");
            sbHtml.Append("</table>");
            sbHtml.Append("<hr />");
            //sbHtml.Append("{{if $dispatch == 0}}");
            if (model.dispatch == 0)//是否是急件
            {
                sbHtml.Append("<div align='center'>");
                sbHtml.Append(" <table width='550' border='0' cellpadding='0' cellspacing='0'>");
                sbHtml.Append("     <tr>");
                sbHtml.Append("         <td><img src='/Content/img/add_01.gif' width='550' height='106' alt='' /></td>");
                sbHtml.Append("     </tr>");
                sbHtml.Append("     <tr>");
                sbHtml.Append("         <td>");
                sbHtml.Append("             <div id='main'>");
                sbHtml.Append("                 <h1>寄件人</h1>");
                sbHtml.Append("                 <h2>吉甲地在地好物市集 <br>");
                sbHtml.Append("                 台北市南港區八德路四段768巷7號6樓之1 <br>");
                sbHtml.Append("                 02-2783-4997</h2>");
                sbHtml.Append("                 <h3>收件人</h3>");
                //sbHtml.Append("                 <h4>{{$a_order_master.delivery_zip}}{{$a_order_master.delivery_address}}<br>");
                sbHtml.AppendFormat("                 <h4>{0}{1}<br>", model.delivery_zip, model.delivery_address);
                //sbHtml.Append("                 {{$a_order_master.delivery_name}} 收 <br>");
                sbHtml.AppendFormat("                 {0} 收 <br>", model.delivery_name);
                //sbHtml.Append("                 {{$a_order_master.delivery_mobile}}</h4>");
                sbHtml.AppendFormat("                 {0}</h4>", model.delivery_mobile);
                sbHtml.Append("                 <div id='order'></div>");
                sbHtml.Append("             </div>");
                sbHtml.Append("         </td>");
                sbHtml.Append("     </tr>");
                sbHtml.Append("     <tr>");
                sbHtml.Append("         <td>");
                sbHtml.Append("             <p>‧拆閱清點後，若您發現商品有異常，請保持商品原狀與通知客服人員<span class='arial'>02-2783-4997</span></p>");
                sbHtml.Append("             <p>‧若您訂購生鮮商品，請務必立即冷藏保存，以免產生變質。</p>");
                sbHtml.Append("         </td>");
                sbHtml.Append("     </tr>");
                sbHtml.Append(" </table>");
                sbHtml.Append("</div>");
                //sbHtml.Append("{{/if}}");
            }
            //sbHtml.Append("<div align='center'><input type='button' name='Submit1' class='cssbutton' value='列印' onClick='javascript:print();' /></div>");
            //sbHtml.Append("<script type='text/JavaScript'>");
            //sbHtml.Append(" function printList(){");
            //sbHtml.Append("     window.print();");
            ////window.close();
            //sbHtml.Append(" }");
            //sbHtml.Append("</script>");

            return sbHtml.ToString();
        }
    }
}