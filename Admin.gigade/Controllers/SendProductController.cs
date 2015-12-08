using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BLL.gigade.Common;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.collection;
using System.IO;
using System.Collections;
using gigadeExcel.Comment;
using BLL.gigade.Model.APIModels;
namespace Admin.gigade.Controllers
{
    /// <summary>
    /// 出貨管理控制器
    /// </summary>
    public class SendProductController : Controller
    {
        //
        // GET: /SendProduct/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        string vendorServerPath = ConfigurationManager.AppSettings["vendorServerPath"];
        IOrderSlaveImplMgr _IOrderSlaveMgr;
        IIplasImplMgr _IiplasMgr;
        IOrderPaymentNcccImplMgr _OrderPaymentNcccMgr;
        ISinopacDetailImplMgr _SinopacMgr;
        IDeliverMasterImplMgr _DeliverMsterMgr;
        ITicketImplMgr _tkMgr;
        IDeliverDetailImplMgr _DeliverDetailMgr;
        IOrderDetailImplMgr _IOrderDetailMgr;
        private IParametersrcImplMgr _ptersrc;
        private IVendorImplMgr _vendorImp;
        IOrderMasterImplMgr _orderMasterMgr;
        IOrderSlaveMasterImplMgr _orderSlaveMasterMgr;
        IChannelImplMgr _channelMgr;
        IZipImplMgr _zipMgr;
        IDeliverChangeLogImplMgr _DeliverChangeLogMgr;
        string filename = string.Empty;
        #region View
        /// <summary>
        /// 測試用,兩天內刪除
        /// </summary>
        /// <returns></returns>

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 待出貨訂單
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderWaitDeliver()
        {
            return View();
        }
        /// <summary>
        /// 出貨確認
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverVerify()
        {
            return View();
        }
        /// <summary>
        /// 檢視頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverView()
        {
            ViewBag.deliver_id = Request.Params["deliver_id"];
            return View();
        }
        /// <summary>
        /// 新批次出貨單
        /// </summary>
        /// <returns></returns>
        public ActionResult new_ticketsView()
        {
            return View();
        }
        /// <summary>
        /// 出货查询
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliversSearchList()
        {
            if (!string.IsNullOrEmpty(Request.Params["delivery_type"]))
            {
                ViewBag.delivery_type = Request.Params["delivery_type"];
            }
            if (!string.IsNullOrEmpty(Request.Params["search"]))
            {

                ViewBag.search = Request.Params["search"];
            }
            if (!string.IsNullOrEmpty(Request.Params["delivery_status"]))
            {

                ViewBag.delivery_status = Request.Params["delivery_status"];
            }

            return View();
        }
        /// <summary>
        /// 傳票明細頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult SubPoenaDetails()
        {
            ViewBag.ticket_id = Request.Params["ticket_id"];
            return View();
        }

        /// <summary>
        /// 每日出貨總表
        /// </summary>
        /// <returns>每日出貨總表視圖</returns>
        public ActionResult All_In_1()
        {
            ViewBag.time = Request.Params["time"];
            ViewBag.delay = Request.Params["delay"];
            return View();
        }
        /// <summary>
        /// 外站出貨當匯出
        /// </summary>
        /// <returns></returns>
        public ActionResult ChannelOrderList()
        {
            return View();
        }
        /// <summary>
        /// 批次上傳物流費
        /// </summary>
        /// <returns></returns>
        public ActionResult PiciAddwuliufei()
        {
            return View();
        }
        /// <summary>
        /// 廠商批次出貨單查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult Batch_List()
        {
            return View();
        }
        /// <summary>
        /// 可出貨--已經完成，產生批次裡面PHP寫的有問題，如今有了排程，這個功能可以不用了。
        /// </summary>
        /// <returns></returns>
        public ActionResult Deliveralbe()
        {
            return View();
        }

        /// <summary>
        /// 延遲出貨
        /// </summary>
        /// <returns></returns>
        public ActionResult DelayDelivers()
        {
            return View();
        }
        /// <summary>
        /// 出貨單統計
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverMasterList()
        {
            return View();
        }
        /// <summary>
        /// 出貨單期望到貨日
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliverExpectArrival()
        {
            return View();
        }
        /// <summary>
        /// 期望到貨日調整記錄
        /// </summary>
        /// <returns></returns>
        public ActionResult ExpectArrivalChangeLog()
        {
            return View();
        }
        #endregion

        #region 供應商出貨單+HttpResponseBase OrderWaitDeliverList()
        /// <summary>
        /// 供應商出貨單
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase OrderWaitDeliverList()
        {
            string jsonStr = String.Empty;
            StringBuilder sb = new StringBuilder();
            HashEncrypt hmd5 = new HashEncrypt();
            _IOrderSlaveMgr = new OrderSlaveMgr(mySqlConnectionString);
            _OrderPaymentNcccMgr = new OrderPaymentNcccMgr(mySqlConnectionString);
            _SinopacMgr = new SinopacDetailMgr(mySqlConnectionString);
            _vendorImp = new VendorMgr(mySqlConnectionString);
            try
            {
                #region 前提查詢條件
                if (!string.IsNullOrEmpty(Request.Params["search_type"]))//查詢條件
                {
                    int search_type = int.Parse(Request.Params["search_type"]);
                    string searchcontent = "";
                    if (!string.IsNullOrEmpty(Request.Params["searchcontent"]))
                    {
                        searchcontent = Request.Params["searchcontent"];
                    }
                    switch (search_type)
                    {
                        case 1:
                            sb.AppendFormat(" and om.order_id LIKE '%{0}%' ", searchcontent);
                            break;
                        case 2:
                            sb.AppendFormat(" AND om.order_name LIKE '%{0}%' ", searchcontent);
                            break;
                        case 3:
                            sb.AppendFormat(" AND u.user_email LIKE '%{0}%' ", searchcontent);
                            break;
                        case 4:
                            sb.AppendFormat(" AND om.delivery_name LIKE '%{0}%' ", searchcontent);
                            break;
                        case 5:
                            sb.AppendFormat(" AND om.source_trace LIKE '%{0}%' ", searchcontent);
                            break;
                        default:
                            sb.Append(" ");
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["search_vendor"]))//供應商條件
                {
                    if (int.Parse(Request.Params["search_vendor"]) != 0)
                    {
                        sb.AppendFormat("  AND os.vendor_id = '{0}' ", Request.Params["search_vendor"]);
                    }
                }
                #endregion
                List<OrderSlaveQuery> store = new List<OrderSlaveQuery>();
                OrderSlaveQuery query = new OrderSlaveQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量

                int totalCount = 0;
                store = _IOrderSlaveMgr.GetOrderWaitDeliver(query, sb.ToString(), out totalCount);//查询出供應商出貨單

                int vendor_id = 0;
                string loginIdStr = string.Empty;
                string mdlogin_id = string.Empty;

                StringBuilder Order_id_store = new StringBuilder();
                foreach (var item in store)
                {
                    Order_id_store.AppendFormat("{0},", item.order_id);
                }
                string dtSql = "and order_id in(" + Order_id_store.ToString().TrimEnd(',') + ") ";//去查詢DataTable
                DataTable _dtNccc = _OrderPaymentNcccMgr.OrderPaymentNccc(null, dtSql);
                DataTable _dtSino = _SinopacMgr.GetSinopacDetai(null, dtSql);
                // String Sql = "123"; 
                foreach (var item in store)
                {
                    //組裝用於登錄到供應商後臺的金鑰
                    vendor_id = Convert.ToInt32(item.vendor_id);
                    loginIdStr = _vendorImp.GetLoginId(vendor_id);
                    mdlogin_id = hmd5.Md5Encrypt(loginIdStr, "MD5");
                    //http://localhost:32088/?vendor_id=2&key=f32bef1b57de24330d8bf900cee4ba5e
                    item.key = vendorServerPath + "?vendor_id=" + item.vendor_id + "&key=" + hmd5.Md5Encrypt(mdlogin_id + loginIdStr, "MD5");

                    item.status = "待出貨";
                    if (item.order_payment == 1)//付款方式为ATM
                    {
                        DataRow[] rows = _dtSino.Select("order_id='" + item.order_id + "'");
                        if (rows.Count() != 0)
                        {
                            item.pay_time = DateTime.Parse(rows[0]["pay_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        else { item.pay_time = ""; }
                    }
                    else
                    {
                        DataRow[] rows = _dtNccc.Select("order_id='" + item.order_id + "'");
                        if (rows.Count() != 0)
                        {
                            item.pay_time = DateTime.Parse(rows[0]["pay_time"].ToString()).ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        else { item.pay_time = ""; }
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
        /// 匯出供應商出貨單
        /// </summary>
        public void OrderWaitDeliverExport()
        {
            string json = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 前提查詢條件

                string type = Request.Params["search_type"];
                if (!string.IsNullOrEmpty(type.Trim()) && !type.Equals("null"))//查詢條件 || 
                {

                    int search_type = int.Parse(Request.Params["search_type"]);
                    string searchcontent = "";
                    if (!string.IsNullOrEmpty(Request.Params["search_type"]))
                    {
                        searchcontent = Request.Params["searchcontent"];
                    }
                    switch (search_type)
                    {
                        case 1:
                            sb.AppendFormat(" and om.order_id LIKE '%{0}%' ", searchcontent);
                            break;
                        case 2:
                            sb.AppendFormat(" AND om.order_name LIKE '%{0}%' ", searchcontent);
                            break;
                        case 3:
                            sb.AppendFormat(" AND u.user_email LIKE '%{0}%' ", searchcontent);
                            break;
                        case 4:
                            sb.AppendFormat(" AND om.delivery_name LIKE '%{0}%' ", searchcontent);
                            break;
                        case 5:
                            sb.AppendFormat(" AND om.source_trace LIKE '%{0}%' ", searchcontent);
                            break;
                        default:
                            sb.Append(" ");
                            break;
                    }
                }
                type = Request.Params["search_vendor"];
                if (!string.IsNullOrEmpty(Request.Params["search_vendor"]) && !type.Equals("null"))//供應商條件
                {
                    if (int.Parse(Request.Params["search_vendor"]) != 0)
                    {
                        sb.AppendFormat("  AND os.vendor_id = '{0}' ", Request.Params["search_vendor"]);
                    }
                }
                #endregion
                string fileName = "order_deliver" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                List<OrderSlaveQuery> store = new List<OrderSlaveQuery>();
                DataTable _dtZip = new DataTable();
                _zipMgr = new ZipMgr(mySqlConnectionString);
                _dtZip = _zipMgr.ZipTable(null, null);
                OrderSlaveQuery query = new OrderSlaveQuery();
                int totalCount = 0;
                _IOrderSlaveMgr = new OrderSlaveMgr(mySqlConnectionString);
                query.IsPage = false;
                store = _IOrderSlaveMgr.GetOrderWaitDeliver(query, sb.ToString(), out totalCount);//匯出出供應商出貨單
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("付款單號", typeof(String));
                dtHZ.Columns.Add("轉單時間", typeof(String));
                dtHZ.Columns.Add("供應商", typeof(String));
                dtHZ.Columns.Add("收貨人", typeof(String));
                // dtHZ.Columns.Add("收貨人手機", typeof(String));
                //dtHZ.Columns.Add("收貨人地址", typeof(String));
                dtHZ.Columns.Add("備註", typeof(String));

                for (int i = 0; i < store.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = store[i].order_id;
                    dr[1] = store[i].order_date_pay;
                    dr[2] = store[i].vendor_name_simple;
                    dr[3] = store[i].delivery_name;
                    //dr[4] = store[i].delivery_mobile;
                    //DataRow[] rows = _dtZip.Select("zipcode='" + store[i].delivery_zip + "'");
                    //StringBuilder sbaddress = new StringBuilder();
                    //if (rows.Count() != 0)
                    //{
                    //    dr[5] = sbaddress.Append(rows[0]["middle"].ToString() + rows[0]["small"].ToString());
                    //}
                    //sbaddress.Append(store[i].delivery_address);
                    //string address = sbaddress.ToString();
                    //address = address.Replace(',', '，');
                    //address = address.Replace("\n", "");
                    //dr[5] = address;
                    store[i].note_order = store[i].note_order.Replace(',', '，');
                    store[i].note_order = store[i].note_order.Replace("\n", "");
                    dr[4] = store[i].note_order;
                    dtHZ.Rows.Add(dr);
                }

                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                string newName = string.Empty;
                newName = Server.MapPath(excelPath) + fileName;

                if (System.IO.File.Exists(newName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newName, FileAttributes.Normal);
                    System.IO.File.Delete(newName);
                }
                StringWriter sw = ExcelHelperXhf.SetCsvFromData(dtHZ, fileName);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();


                //StringWriter sw = ExcelHelperXhf.SetCsvFromData(dtHZ, fileName);
                //Response.Clear();
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                //Response.ContentType = "application/ms-excel";
                //Response.ContentEncoding = Encoding.Default;
                //Response.Write(sw);
                //Response.End();

                //MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "order_deliver" + "_" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                //Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:[]}";
            }
            //this.Response.Clear();
            //this.Response.Write(jsonStr.ToString());
            //this.Response.End();
            //return this.Response;
        }

        #endregion

        #region 出貨查詢列表+HttpResponseBase DeliversList()
        public HttpResponseBase DeliversList()
        {
            string jsonStr = String.Empty;
            StringBuilder sb = new StringBuilder();
            List<DeliverMasterQuery> store = new List<DeliverMasterQuery>();
            DeliverMasterQuery query = new DeliverMasterQuery();
            #region 现在参数表没有的数据，先加进来
            DataTable _dtDeliverCatStatus = new DataTable();
            DataTable _dtLogisticsType = new DataTable();
            #region _dtDeliverCatStatus


            _dtDeliverCatStatus.Columns.Add("ParameterCode", typeof(String));
            _dtDeliverCatStatus.Columns.Add("remark", typeof(String));
            DataRow dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 1;
            dr[1] = "順利送達";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 2;
            dr[1] = "轉運中";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 3;
            dr[1] = "配送中";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 4;
            dr[1] = "配送中(當配下車) (當配上車)";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 5;
            dr[1] = "取件中";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 6;
            dr[1] = "已集貨";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 7;
            dr[1] = "取消取件";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 8;
            dr[1] = "未順利取件，請洽客服中心";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 9;
            dr[1] = "暫置營業所保管中(請聯絡黑貓宅急便)";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 10;
            dr[1] = "調查處理中";
            _dtDeliverCatStatus.Rows.Add(dr);
            dr = _dtDeliverCatStatus.NewRow();
            dr[0] = 11;
            dr[1] = "不在家.公司行號休息";
            _dtDeliverCatStatus.Rows.Add(dr);
            #endregion
            #region _dtLogisticsType
            _dtLogisticsType.Columns.Add("ParameterCode", typeof(String));
            _dtLogisticsType.Columns.Add("remark", typeof(String));
            dr = _dtLogisticsType.NewRow();
            dr[0] = 1;
            dr[1] = "大物流中心";
            _dtLogisticsType.Rows.Add(dr);
            dr = _dtLogisticsType.NewRow();
            dr[0] = 2;
            dr[1] = "配送中";
            _dtLogisticsType.Rows.Add(dr);
            dr = _dtLogisticsType.NewRow();
            dr[0] = 3;
            dr[1] = "大物流退貨";
            _dtLogisticsType.Rows.Add(dr);
            dr = _dtLogisticsType.NewRow();
            dr[0] = 4;
            dr[1] = "進店作業";
            _dtLogisticsType.Rows.Add(dr);
            dr = _dtLogisticsType.NewRow();
            dr[0] = 5;
            dr[1] = "進店退貨";
            dr = _dtLogisticsType.NewRow();
            dr = _dtLogisticsType.NewRow();
            dr[0] = 6;
            dr[1] = "取貨完成";
            _dtLogisticsType.Rows.Add(dr);
            #endregion

            #endregion

            #region 查詢條件
            if (!string.IsNullOrEmpty(Request.Params["type"]))//出貨類別
            {
                query.types = Request.Params["type"];
            }
            if (!string.IsNullOrEmpty(Request.Params["delivery_status"]) && int.Parse(Request.Params["delivery_status"]) != -1)//出貨狀態
            {
                query.status = Request.Params["delivery_status"];
            }
            if (!string.IsNullOrEmpty(Request.Params["export_id"]) && int.Parse(Request.Params["export_id"]) != 0) //出貨方式
            {
                query.vendor_id = uint.Parse(Request.Params["export_id"]);
            }
            query.delivery_store = 0;
            if (!string.IsNullOrEmpty(Request.Params["delivery_store"]) && int.Parse(Request.Params["delivery_store"]) != 0)//物流商
            {
                query.delivery_store = uint.Parse(Request.Params["delivery_store"]);
            }
            query.warehouse_status = -1;
            if (!string.IsNullOrEmpty(Request.Params["warehouse_statu"]) && int.Parse(Request.Params["warehouse_statu"]) != -1) //調度狀態
            {
                query.warehouse_status = int.Parse(Request.Params["warehouse_statu"]);
            }
            query.priority = -1;
            if (!string.IsNullOrEmpty(Request.Params["priority"]) && int.Parse(Request.Params["priority"]) != -1)//出貨篩選
            {
                query.priority = int.Parse(Request.Params["priority"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["datequery"]) && int.Parse(Request.Params["datequery"]) != 0)//日期條件
            {
                if (!string.IsNullOrEmpty(Request.Params["time_start"]))//出貨日期
                {
                    string s = Request.Params["time_start"];
                    query.time_start = DateTime.Parse(Request.Params["time_start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))//出貨日期
                {
                    query.time_end = DateTime.Parse(Request.Params["time_end"]);
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["search"]))//搜索
            {
                query.vendor_name_simple = Request.Params["search"];
            }
            #endregion
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _DeliverMsterMgr.GetdeliverList(query, out totalCount);
                #region 獲取物流業者列表，訂單狀態列表。然後根據判斷來<數據來自參數表>
                List<Parametersrc> Shipment = new List<Parametersrc>();
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                Shipment = _ptersrc.GetAllKindType("Deliver_Store");//物流業者
                DataTable _dtShipment = new DataTable();
                _dtShipment.Columns.Add("parametercode", typeof(String));
                _dtShipment.Columns.Add("parameterName", typeof(String));
                _dtShipment.Columns.Add("remark", typeof(String));
                foreach (var item in Shipment)
                {
                    dr = _dtShipment.NewRow();//--------------------------------------------------DataRow dr=...
                    dr[0] = item.ParameterCode;
                    dr[1] = item.parameterName;
                    dr[2] = item.remark;
                    _dtShipment.Rows.Add(dr);
                }
                Shipment = _ptersrc.GetAllKindType("order_status");//訂單狀態
                DataTable _dtOrderStatus = new DataTable();
                _dtOrderStatus.Columns.Add("parametercode", typeof(String));
                _dtOrderStatus.Columns.Add("parameterName", typeof(String));
                _dtOrderStatus.Columns.Add("remark", typeof(String));
                foreach (var item in Shipment)
                {
                    dr = _dtOrderStatus.NewRow();//------------------------------------------dr
                    dr[0] = item.ParameterCode;
                    dr[1] = item.parameterName;
                    dr[2] = item.remark;
                    _dtOrderStatus.Rows.Add(dr);
                }
                #endregion
                foreach (var item in store)
                {


                    //(計算數據，參考自admin/View/Delivers/index.ctp 第152~163行)
                    #region 計算逾期天數：(出貨時間-付款單成立日期+1)-4，出貨時間=空值，以當日計算。
                    DateTime Shipmenttime = DateTime.Now;//出貨日期

                    //if (!string.IsNullOrEmpty(item.delivery_date.ToString()))
                    //{
                    //    Shipmenttime = item.delivery_date;
                    //}

                    if (!item.delivery_date.ToString("yyyy-MM-dd").Equals("0001-01-01"))
                    {
                        Shipmenttime = item.delivery_date;
                    }
                    TimeSpan s = DateTime.Parse(Shipmenttime.ToString("yyyy-MM-dd 23:59:59")) - DateTime.Parse(item.order_createtime.ToString("yyyy-MM-dd 23:59:59"));//時間跨度,因為捨棄的不一樣，計算的也不一樣
                    item.overdue_day = s.Days - 3;
                    #endregion
                    #region 把訂單狀態和物流業者提取賦值
                    DataRow[] rows = _dtShipment.Select("ParameterCode='" + item.delivery_store + "'");
                    foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，把物流商的名稱傳遞過去，如果沒有，把物流商編號傳遞過去
                    {
                        item.ShipmentName = item.delivery_store.ToString();
                        if (!string.IsNullOrEmpty(row["ParameterCode"].ToString()))
                        {
                            item.ShipmentName = row["ParameterName"].ToString();
                        }
                    }
                    if (item.type == 1 || item.type == 2)//訂單狀態
                    {
                        rows = _dtOrderStatus.Select("ParameterCode='" + item.order_status + "'");
                        foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，把物流商的名稱傳遞過去，如果沒有，把物流商編號傳遞過去
                        {
                            item.states = item.order_status.ToString();
                            if (!string.IsNullOrEmpty(row["ParameterCode"].ToString()))
                            {
                                item.states = row["remark"].ToString();
                            }
                        }
                    }
                    #endregion
                    #region 物流状态

                    item.LogisticsStatus = "";
                    if (item.logisticsType != 0)
                    {
                        if (item.delivery_store == 1 || item.delivery_store == 10)
                        {
                            rows = _dtDeliverCatStatus.Select("ParameterCode='" + item.logisticsType + "'");
                            foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，把物流商的名稱傳遞過去，如果沒有，把物流商編號傳遞過去
                            {
                                if (!string.IsNullOrEmpty(row["ParameterCode"].ToString()))
                                {
                                    item.LogisticsStatus = row["remark"].ToString();
                                }
                            }
                        }
                        else
                        {
                            rows = _dtLogisticsType.Select("ParameterCode='" + item.logisticsType + "'");
                            foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，把物流商的名稱傳遞過去，如果沒有，把物流商編號傳遞過去
                            {
                                if (!string.IsNullOrEmpty(row["ParameterCode"].ToString()))
                                {
                                    item.LogisticsStatus = row["remark"].ToString();
                                }
                            }
                        }
                    }
                    #endregion
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss ";
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

        public void DeliversExport()
        {
            string json = string.Empty;
            StringBuilder sb = new StringBuilder();
            string jsonStr = String.Empty;


            try
            {
                List<DeliverMasterQuery> store = new List<DeliverMasterQuery>();
                DeliverMasterQuery query = new DeliverMasterQuery();
                #region 查詢條件
                if (!string.IsNullOrEmpty(Request.Params["type"]))//出貨類別
                {
                    query.types = Request.Params["type"];
                }
                if (!string.IsNullOrEmpty(Request.Params["delivery_status"]) && int.Parse(Request.Params["delivery_status"]) != -1)//出貨狀態
                {
                    query.status = Request.Params["delivery_status"];
                }
                if (!string.IsNullOrEmpty(Request.Params["export_id"]) && int.Parse(Request.Params["export_id"]) != 0) //出貨方式
                {
                    query.vendor_id = uint.Parse(Request.Params["export_id"]);
                }
                query.delivery_store = 0;
                if (!string.IsNullOrEmpty(Request.Params["delivery_store"]) && int.Parse(Request.Params["delivery_store"]) != 0)//物流商
                {
                    query.delivery_store = uint.Parse(Request.Params["delivery_store"]);
                }
                query.warehouse_status = -1;
                if (!string.IsNullOrEmpty(Request.Params["warehouse_statu"]) && int.Parse(Request.Params["warehouse_statu"]) != -1) //調度狀態
                {
                    query.warehouse_status = int.Parse(Request.Params["warehouse_statu"]);
                }
                query.priority = -1;
                if (!string.IsNullOrEmpty(Request.Params["priority"]) && int.Parse(Request.Params["priority"]) != -1)//出貨篩選
                {
                    query.priority = int.Parse(Request.Params["priority"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["datequery"]) && int.Parse(Request.Params["datequery"]) != 0)//日期條件
                {
                    if (!string.IsNullOrEmpty(Request.Params["time_start"]))//出貨日期
                    {

                        query.time_start = DateTime.Parse(Request.Params["time_start"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["time_end"]))//出貨日期
                    {
                        query.time_end = DateTime.Parse(Request.Params["time_end"]);
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["search"]))//搜索
                {
                    query.vendor_name_simple = Request.Params["search"];
                }
                #endregion
                _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);

                string fileName = "delivers" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                store = _DeliverMsterMgr.GetdeliverListCSV(query);
                #region 獲取物流業者列表，訂單狀態列表。然後根據判斷來<數據來自參數表>
                List<Parametersrc> Shipment = new List<Parametersrc>();
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                Shipment = _ptersrc.GetAllKindType("Deliver_Store");//物流業者
                DataTable _dtShipment = new DataTable();
                _dtShipment.Columns.Add("parametercode", typeof(String));
                _dtShipment.Columns.Add("parameterName", typeof(String));
                _dtShipment.Columns.Add("remark", typeof(String));
                foreach (var item in Shipment)
                {
                    DataRow dr = _dtShipment.NewRow();
                    dr[0] = item.ParameterCode;
                    dr[1] = item.parameterName;
                    dr[2] = item.remark;
                    _dtShipment.Rows.Add(dr);
                }
                Shipment = _ptersrc.GetAllKindType("order_status");//訂單狀態
                DataTable _dtOrderStatus = new DataTable();
                _dtOrderStatus.Columns.Add("parametercode", typeof(String));
                _dtOrderStatus.Columns.Add("parameterName", typeof(String));
                _dtOrderStatus.Columns.Add("remark", typeof(String));
                foreach (var item in Shipment)
                {
                    DataRow dr = _dtOrderStatus.NewRow();
                    dr[0] = item.ParameterCode;
                    dr[1] = item.parameterName;
                    dr[2] = item.remark;
                    _dtOrderStatus.Rows.Add(dr);
                }
                #endregion

                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("出貨時間", typeof(String));
                dtHZ.Columns.Add("到貨時間", typeof(String));
                dtHZ.Columns.Add("訂單編號", typeof(String));
                dtHZ.Columns.Add("訂單狀態", typeof(String));
                dtHZ.Columns.Add("收貨人", typeof(String));
                dtHZ.Columns.Add("出貨編號", typeof(String));
                dtHZ.Columns.Add("出貨單狀態", typeof(String));
                dtHZ.Columns.Add("出貨廠商", typeof(String));
                dtHZ.Columns.Add("運送方式", typeof(String));
                dtHZ.Columns.Add("預計出貨日期", typeof(String));
                dtHZ.Columns.Add("預計到貨日期", typeof(String));
                dtHZ.Columns.Add("預計到貨時段", typeof(String));
                dtHZ.Columns.Add("物流業者", typeof(String));
                dtHZ.Columns.Add("調度", typeof(String));
                dtHZ.Columns.Add("物流單號", typeof(String));
                dtHZ.Columns.Add("物流費", typeof(String));
                dtHZ.Columns.Add("可出貨時間", typeof(String));
                dtHZ.Columns.Add("付款時間", typeof(String));


                foreach (var item in store)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = processingTime(item.delivery_date);
                    dr[1] = processingTime(item.arrival_date);
                    dr[2] = item.order_id;

                    #region 把訂單狀態和物流業者提取賦值
                    DataRow[] rows = _dtShipment.Select("ParameterCode='" + item.delivery_store + "'");
                    foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，把物流商的名稱傳遞過去，如果沒有，把物流商編號傳遞過去
                    {
                        item.ShipmentName = item.delivery_store.ToString();
                        if (!string.IsNullOrEmpty(row["ParameterCode"].ToString()))
                        {
                            //  item.ShipmentName = row["ParameterName"].ToString();
                            dr[12] = row["ParameterName"].ToString();//---物流業者
                        }
                    }
                    if (item.type == 1 || item.type == 2)//訂單狀態
                    {
                        rows = _dtOrderStatus.Select("ParameterCode='" + item.order_status + "'");
                        foreach (DataRow row in rows)//篩選出的最多只有一條數據，如果有，把物流商的名稱傳遞過去，如果沒有，把物流商編號傳遞過去
                        {
                            dr[3] = item.order_status.ToString();
                            if (!string.IsNullOrEmpty(row["ParameterCode"].ToString()))
                            {
                                dr[3] = row["remark"].ToString();
                            }
                        }
                    }
                    #endregion

                    dr[4] = item.delivery_name;
                    dr[5] = item.deliver_id;
                    #region 出貨單狀態
                    uint stat = item.delivery_status;//出貨單狀態
                    switch (stat)
                    {
                        case 0:
                            dr[6] = "待出貨";
                            break;
                        case 1:
                            dr[6] = "可出貨";
                            break;
                        case 2:
                            dr[6] = "出貨中";
                            break;
                        case 3:
                            dr[6] = "已出貨";
                            break;
                        case 4:
                            dr[6] = "已到貨";
                            break;
                        case 5:
                            dr[6] = "未到貨";
                            break;
                        case 6:
                            dr[6] = "取消出貨";
                            break;
                        case 7:
                            dr[6] = "待取貨";
                            break;
                        default:
                            dr[6] = "意外數據錯誤";
                            break;
                    }
                    #endregion

                    dr[7] = item.vendor_name_simple;
                    #region 運送方式
                    uint freight_set = item.freight_set;
                    switch (freight_set)
                    {
                        case 1:
                            dr[8] = "常溫";
                            break;
                        case 2:
                            dr[8] = "冷凍";
                            break;
                        case 3:
                            dr[8] = "常溫免運";
                            break;
                        case 4:
                            dr[8] = "冷凍免運";
                            break;
                        case 5:
                            dr[8] = "冷藏";
                            break;
                        case 6:
                            dr[8] = "冷藏免運";
                            break;
                        default:
                            dr[8] = freight_set;
                            break;
                    }
                    #endregion

                    dr[9] = processingTime(item.estimated_delivery_date);
                    dr[10] = processingTime(item.estimated_arrival_date);
                    #region 預計到貨時段
                    switch (item.estimated_arrival_period)
                    {
                        case 0:
                            dr[11] = "不限時";
                            break;
                        case 1:
                            dr[11] = "12:00以前";
                            break;
                        case 2:
                            dr[11] = "12:00-17:00";
                            break;
                        case 3:
                            dr[11] = "17:00-20:00";
                            break;
                        default:
                            dr[11] = item.estimated_arrival_period;
                            break;
                    }
                    #endregion
                    //  dr[11] = item.estimated_arrival_period;//--預計到貨時段
                    //dr[12] = item.delivery_date;
                    dr[13] = "";//--調度
                    if (item.warehouse_status != 0)
                    {
                        dr[13] = "調度";
                    }
                    dr[14] = item.delivery_code;//--物流單號
                    dr[15] = item.delivery_freight_cost;//--物流費
                    dr[16] = processingTime(item.order_pay_date);//--可出貨時間
                    dr[17] = processingTime(item.money_pay_date);//--付款時間

                    dtHZ.Rows.Add(dr);
                }

                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                string newName = string.Empty;
                newName = Server.MapPath(excelPath) + fileName;

                if (System.IO.File.Exists(newName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newName, FileAttributes.Normal);
                    System.IO.File.Delete(newName);
                }
                StringWriter sw = ExcelHelperXhf.SetCsvFromData(dtHZ, fileName);
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
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
        /// <summary>
        /// 處理時間
        /// </summary>
        /// <returns></returns>
        public string processingTime(DateTime times)
        {
            string result;
            if (times.ToString("yyyy-MM-dd").Equals("0001-01-01"))
            {
                result = "N/A";
            }
            else
            {
                result = times.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return result;
        }

        #endregion

        #region 新批次出貨查詢
        public HttpResponseBase Getnewticketslist()
        {
            string jsonStr = String.Empty;
            StringBuilder sb = new StringBuilder();
            List<TicketQuery> store = new List<TicketQuery>();
            TicketQuery tqQuery = new TicketQuery();
            try
            {
                tqQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                tqQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                _tkMgr = new TicketMgr(mySqlConnectionString);
                int totalCount = 0;
                string condition = string.Empty;

                int deliver_type = Convert.ToInt32(Request.Params["deliver_type"]);//批次出貨類別
                int vendorcondition = 0;
                if (!string.IsNullOrEmpty(Request.Params["vendorcondition"]))
                {
                    vendorcondition = int.Parse(Request.Params["vendorcondition"]);//出貨廠商
                }
                int shipment = 0;
                if (!string.IsNullOrEmpty(Request.Params["shipment"]))
                {
                    shipment = int.Parse(Request.Params["shipment"]);//物流商
                }
                int gongyinshang = 0;
                if (!string.IsNullOrEmpty(Request.Params["gongyinshang"]))
                {
                    gongyinshang = int.Parse(Request.Params["gongyinshang"]);
                }
                int scheduling = int.Parse(Request.Params["scheduling"]);//調度狀態
                int screen = int.Parse(Request.Params["screen"]);//批次出貨狀態
                int lytype = int.Parse(Request.Params["lytype"]);//列印處理狀態
                int ystype = int.Parse(Request.Params["ystype"]);//運送方式
                string search = Request.Params["search"].Trim();//搜索內容
                if (deliver_type == 1) //批次出貨類別
                {
                    condition = condition + " and Ticket.type = 1  ";
                }
                else
                {
                    condition = condition + " and Ticket.type = 2  ";
                }
                #region 出貨廠商
                if (vendorcondition == 0)//出貨廠商 所有 對
                {
                    condition = condition + "";
                }
                else
                {
                    condition = condition + " and Export.vendor_id=" + vendorcondition;
                }
                #endregion

                #region 物流商或者是供應商
                if (shipment == 0)//物流商
                {
                    condition = condition + "";
                }
                else
                {
                    condition = condition + " and Ticket.delivery_store=" + shipment;
                }
                if (gongyinshang == 0)//物流商
                {
                    condition = condition + "";
                }
                else
                {
                    condition = condition + " and Ticket.delivery_store=" + shipment;
                }
                #endregion

                if (scheduling == 0)//調度狀態
                {
                    condition = condition + " and warehouse_status=0 ";
                }
                else if (scheduling == 1)
                {
                    condition = condition + " and warehouse_status=1 ";
                }
                if (screen == -1)//批次出貨狀態
                {
                    condition = condition + "";
                }
                else if (screen == 0)
                {
                    condition = condition + " and ticket_status=0 ";
                }
                else if (screen == 1)
                {
                    condition = condition + " and ticket_status=1 ";
                }
                if (lytype == 0)//列印處理狀態 現在php後台好像沒有作用
                {
                    condition = condition + " ";
                }
                else
                {
                    condition = condition + " ";
                }
                if (ystype == 1)//運送方式
                {
                    condition = condition + " and freight_set=1 ";
                }
                else if (ystype == 2)
                {
                    condition = condition + " and freight_set=2 ";
                }
                else if (ystype == 5)
                {
                    condition = condition + " and freight_set=5 ";
                }
                if (!string.IsNullOrEmpty(search))//搜索條件
                {
                    condition = condition + string.Format(" AND ((Ticket.ticket_id like '%{0}%') OR (Export.vendor_name_simple like '%{0}%'))", search);
                }
                store = _tkMgr.GetTicketList(tqQuery, out totalCount, condition);
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

        public void GetPDF()
        {
            string filename = string.Empty;
            string jsonStr = String.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ticket_id"]))
                {
                    filename = "picking_detail" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
                    string newPDFName = Server.MapPath(excelPath) + filename;
                    if (System.IO.File.Exists(newPDFName))
                    {
                        //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                        System.IO.File.SetAttributes(newPDFName, FileAttributes.Normal);
                        System.IO.File.Delete(newPDFName);
                    }
                    TicketQuery query = new TicketQuery();
                    string temp = Request.Params["ticket_id"].ToString();
                    temp = temp.Remove(temp.LastIndexOf(','));
                    query.ticketIds = temp;
                    _tkMgr = new TicketMgr(mySqlConnectionString);
                    List<TicketQuery> store = _tkMgr.GetPickingDetail(query);
                    List<TicketQuery> FMode = new List<TicketQuery>();
                    List<TicketQuery> CMode = new List<TicketQuery>();
                    foreach (var item in store)
                    {
                        if (item.combined_mode > 1)
                        {

                            if (item.item_mode == 1)
                            {
                                FMode.Add(item);
                            }
                            else
                            {
                                CMode.Add(item);
                            }
                        }
                        else
                        {
                            CMode.Add(item);
                        }

                    }
                    foreach (var fitem in FMode)
                    {
                        foreach (var citem in CMode)
                        {
                            if (fitem.parent_id == citem.parent_id && fitem.pack_id == citem.pack_id)
                            {
                                citem.buy_num = citem.buy_num * citem.parent_num;
                            }
                        }
                    }
                    string[] strId = query.ticketIds.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string str = strId[strId.Length - 1].ToString();
                    int de = 8 - str.Length;
                    for (int z = 0; z < de; z++)
                    {
                        str = str.Insert(0, "0");
                    }
                    str = str.Insert(0, "P");
                    BarCode.Code128 _Code = new BarCode.Code128();
                    _Code.ValueFont = new System.Drawing.Font("宋体", 20);
                    System.Drawing.Bitmap imgTemp = _Code.GetCodeImage(str, BarCode.Code128.Encode.Code128A);
                    imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "BarCode.gif", System.Drawing.Imaging.ImageFormat.Gif);
                    BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 14, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
                    iTextSharp.text.Font font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.GetStyleValue("font-size:xx-small"), new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    iTextSharp.text.Font bigFont = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    Document document = new Document(PageSize.A4.Rotate(), (float)5, (float)5, (float)5, (float)0.5);

                    PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.OpenOrCreate));

                    document.Open();
                    Phrase ph = new Phrase("\n ", font);
                    iTextSharp.text.Image IMG = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/BarCode.gif"));
                    IMG.ScaleToFit(100, 30);
                    Chunk ck = new Chunk(IMG, 580, -30); //图片可设置 偏移
                    ph.Add(ck);
                    document.Add(ph);
                    Phrase p1 = new Phrase("\n \n", font);
                    document.Add(p1);
                    PdfPTable table = new PdfPTable(7);
                    //table.BorderWidth = 0;
                    //table.Cellpadding = 3;
                    //table.Cellspacing = 3;
                    table.SpacingBefore = 5;
                    table.SetWidths(new int[] { 20, 30, 20, 40, 10, 15, 15 });
                    PdfPCell cell = new PdfPCell(new Phrase("付款單號", font));
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthBottom = 0.5f;
                    cell.BorderWidthRight = 0;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("付款日期", font));
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthBottom = 0.5f;
                    cell.BorderWidthRight = 0;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("供應商", font));
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthBottom = 0.5f;
                    cell.BorderWidthRight = 0;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("商品名稱", font));
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthBottom = 0.5f;
                    cell.BorderWidthRight = 0;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("數量", font));
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthBottom = 0.5f;
                    cell.BorderWidthRight = 0;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("收件人", font));
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthBottom = 0.5f;
                    cell.BorderWidthRight = 0;
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("訂購人", font));
                    cell.BorderWidthLeft = 0;
                    cell.BorderWidthBottom = 0.5f;
                    cell.BorderWidthRight = 0;
                    table.AddCell(cell);
                    for (int i = 0; i < CMode.Count; i++)
                    {
                        if (i == 0)
                        {
                            cell = new PdfPCell(new Phrase(CMode[i].order_id.ToString(), font));
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(CommonFunction.GetNetTime(CMode[i].order_date_pay).ToString("yyyy-MM-dd HH:mm:ss"), font));
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(CMode[i].vendor_name_simple.ToString(), font));
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(CMode[i].product_name.ToString() + CMode[i].spec_name.ToString() + CMode[i].spec_name1.ToString(), font));
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(CMode[i].buy_num.ToString(), font));
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(CMode[i].delivery_name.ToString(), font));
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            table.AddCell(cell);
                            cell = new PdfPCell(new Phrase(CMode[i].order_name.ToString(), font));
                            cell.BorderWidthLeft = 0;
                            cell.BorderWidthRight = 0;
                            cell.BorderWidthTop = 0;
                            cell.BorderWidthBottom = 0;
                            table.AddCell(cell);
                        }
                        else
                        {
                            if (CMode[i].order_id == CMode[i - 1].order_id)
                            {
                                cell = new PdfPCell(new Phrase(CMode[i].order_id.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CommonFunction.GetNetTime(CMode[i].order_date_pay).ToString("yyyy-MM-dd HH:mm:ss"), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].vendor_name_simple.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].product_name.ToString() + CMode[i].spec_name.ToString() + CMode[i].spec_name1.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].buy_num.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].delivery_name.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].order_name.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                            }
                            else
                            {
                                cell = new PdfPCell(new Phrase(CMode[i].order_id.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CommonFunction.GetNetTime(CMode[i].order_date_pay).ToString("yyyy-MM-dd HH:mm:ss"), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].vendor_name_simple.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].product_name.ToString() + CMode[i].spec_name.ToString() + CMode[i].spec_name1.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].buy_num.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].delivery_name.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                                cell = new PdfPCell(new Phrase(CMode[i].order_name.ToString(), font));
                                cell.BorderWidthLeft = 0;
                                cell.BorderWidthRight = 0;
                                cell.BorderWidthTop = 0.5f;
                                cell.BorderWidthBottom = 0;
                                table.AddCell(cell);
                            }
                        }
                    }
                    document.Add(table);
                    document.Close();
                    Response.Clear();
                    Response.Charset = "gb2312";
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    // Response.AddHeader("Content-Disposition", "attach-ment;filename=" + System.Web.HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8) + ".pdf ");
                    Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename);
                    Response.WriteFile(newPDFName);
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                jsonStr = "{success:false,result:0}";
            }
        }
        #region 已注釋出貨單
        //public HttpResponseBase GetDeliversPDF()
        //{
        //    string filename = string.Empty;
        //    string jsonStr = String.Empty;
        //    try
        //    {
        //    TicketQuery query = new TicketQuery();
        //        string str = Request.Params["ticket_id"].ToString();
        //        str = str.Remove(str.LastIndexOf(','));
        //        query.ticketIds = str;
        //    query.type = 1;
        //    _tkMgr = new TicketMgr(mySqlConnectionString);
        //    List<TicketQuery> list = _tkMgr.GetTicketDetail(query);
        //    BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        //    iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 14, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
        //    iTextSharp.text.Font font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
        //    iTextSharp.text.Font bigFont = new iTextSharp.text.Font(bfChinese, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑
        //        int temp = 0;
        //        if (int.TryParse(str, out temp))
        //        {
        //            filename = "order_delivers_T" + str.ToString().PadLeft(8, '0') + ".pdf";
        //        }
        //        else
        //        {
        //            filename = "order_delivers_all.pdf";
        //        }

        //    Rectangle pageSize = new Rectangle(PageSize.A4.Rotate());
        //        Document document = new Document(pageSize, (float)0.5, (float)5, (float)0.5, (float)0.5);
        //    string newPDFName = Server.MapPath(excelPath) + filename;
        //    PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.OpenOrCreate));

        //    foreach (var item in list)
        //    {
        //        item.orderCreatedate = CommonFunction.GetNetTime(item.order_createdate).ToString("yyyy-MM-dd HH:mm:ss");
        //        item.moneyCollectDate = CommonFunction.GetNetTime(item.money_collect_date).ToString("yyyy-MM-dd HH:mm:ss");
        //        item.holidayDeliver = item.holiday_deliver == 1 ? "可" : "不可";
        //        if (item.estimated_arrival_period == 0)
        //        {
        //            item.arrival_period_name = "不限時";
        //        }
        //        else if (item.estimated_arrival_period == 1)
        //        {
        //            item.arrival_period_name = "12:00以前";
        //        }
        //        else if (item.estimated_arrival_period == 2)
        //        {
        //            item.arrival_period_name = "12:00-17:00";
        //        }
        //        else if (item.estimated_arrival_period == 3)
        //        {
        //            item.arrival_period_name = "17:00-20:00";
        //        }
        //        if (item.freight_set == 1)
        //        {
        //            item.freight_set_name = "常溫";
        //        }
        //        else if (item.freight_set == 2)
        //        {
        //            item.freight_set_name = "冷凍";
        //        }
        //        else if (item.freight_set == 3)
        //        {
        //            item.freight_set_name = "常溫免運";
        //        }
        //        else if (item.freight_set == 4)
        //        {
        //            item.freight_set_name = "冷凍免運";
        //        }
        //        else if (item.freight_set == 5)
        //        {
        //            item.freight_set_name = "冷藏";
        //        }
        //        else if (item.freight_set == 6)
        //        {
        //            item.freight_set_name = "冷藏免運";
        //        }
        //        if (item.delivery_store == 12)
        //        {
        //            item.deliveryStore = "*自取(取貨地址:台北市南港區八德路4段768巷7號6樓之1，取貨時間週一~週五，AM9:00~PM6:00)";
        //        }
        //        else if (item.delivery_store == 13)
        //        {
        //            item.deliveryStore = "*自取(取貨地址:新北市板橋區三民路二段33號21樓，取貨時間週一~週五，AM9:00~PM6:00)";
        //        }
        //        else if (item.delivery_store == 14)
        //        {
        //            item.deliveryStore = "*自取(取貨地址:新北市永和區成功路一段80號20樓，取貨時間週一~週五，AM9:00~PM6:00)";
        //        }
        //    }
        //    for (int m = 0; m < list.Count; m++)
        //    {
        //        _tkMgr = new TicketMgr(mySqlConnectionString);
        //        list[m].type = 1;
        //        DataTable li = _tkMgr.GetOrderDelivers(list[m]);
        //        string strId = list[m].deliver_id.ToString();
        //        int de = 8 - strId.Length;
        //        for (int z = 0; z < de; z++)
        //        {
        //            strId = strId.Insert(0, "0");
        //        }
        //        strId = strId.Insert(0, "D");
        //        document.Open();
        //        Phrase ph = new Phrase("\n \n", font);
        //        BarCode.Code128 _Code = new BarCode.Code128();
        //        _Code.ValueFont = new System.Drawing.Font("宋体", 20);
        //        System.Drawing.Bitmap imgTemp = _Code.GetCodeImage(strId, BarCode.Code128.Encode.Code128A);
        //        imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
        //        iTextSharp.text.Image IMG = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
        //            IMG.ScaleToFit(134, 60);
        //        Chunk ck = new Chunk(IMG, 0, 0); //图片可设置 偏移
        //        ph.Add(ck);
        //        //document.Add(ck);
        //            //            if (orderdeliver.Rows[0]["channel"].ToString() != "1")
        //            //            {
        //            //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["channel_name_simple"].ToString(), 80, 700, 0);
        //            //            }
        //            //            if (orderdeliver.Rows[0]["retrieve_mode"].ToString() == "1")
        //            //            {
        //            //                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "7-11取貨", 200, 700, 0);
        //            //            }

        //        PdfPTable tab = new PdfPTable(2);
        //        //tab.Border = 0;
        //        //tab.BorderWidth = 1;
        //        //tab.BorderColor = new iTextSharp.text.BaseColor(0, 0, 255);
        //        //tab.Cellpadding = 2;
        //        //tab.Cellspacing = 2;
        //        //tab.SetWidths(new int[] { 5, 30, 20, 5,10, 30 });
        //        PdfPCell cel = new PdfPCell();
        //            cel = new PdfPCell(new Phrase("吉甲地市集出貨明細\n", bigFont));
        //        cel.HorizontalAlignment = 1;
        //        cel.Border = 0;
        //        tab.AddCell(cel);
        //        cel = new PdfPCell(new Phrase(list[m].freight_set_name + "\n" + list[m].arrival_period_name + "\n", font));
        //        //cel.Add(new Phrase(list[m].arrival_period_name + "\n", font));
        //        cel.AddElement(ph);
        //        cel.Border = 0;
        //        cel.HorizontalAlignment = 1;
        //            if (li.Rows[0]["channel"].ToString() != "1")
        //            {

        //                Paragraph channel_name_simple = new Paragraph(new Chunk(li.Rows[0]["channel_name_simple"].ToString() + "\n", font));
        //                cel.AddElement(channel_name_simple);
        //            }
        //            if (li.Rows[0]["retrieve_mode"].ToString() == "1")
        //            {
        //                Paragraph retrieve_mode = new Paragraph(new Chunk("7-11取貨\n", font));
        //                cel.AddElement(retrieve_mode);
        //            }
        //        tab.AddCell(cel);
        //        document.Add(tab);

        //        Paragraph p1 = new Paragraph(new Chunk("\n", FontFactory.GetFont(FontFactory.HELVETICA, 3)));
        //        document.Add(p1);
        //        PdfPTable tabShow = new PdfPTable(4);
        //        tabShow.SetWidths(new int[] { 15, 30, 15, 40 });
        //        iTextSharp.text.pdf.PdfPCell cellShow = new PdfPCell();
        //        cellShow = new PdfPCell(new Phrase("訂購人：", font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingRight = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].order_name, font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        //cel.HorizontalAlignment = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("收件人：", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].delivery_name, font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("付款單號：", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].order_id.ToString(), font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("收件地址：", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].zip_name.ToString(), font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("訂購時間：", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].orderCreatedate, font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("聯絡電話：", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].delivery_mobile, font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("付款時間：", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].moneyCollectDate, font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("假日可收貨：", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].holidayDeliver.ToString(), font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("出貨備註：", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].note_order, font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        cellShow.Colspan = 3;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase("訂單明細:", font));
        //        cellShow.Border = 0;
        //        tabShow.AddCell(cellShow);
        //        cellShow = new PdfPCell(new Phrase(list[m].deliveryStore, font));
        //        cellShow.Border = 0;
        //        cellShow.PaddingLeft = 0;
        //        cellShow.Colspan = 3;
        //        tabShow.AddCell(cellShow);
        //            if (li.Rows[0]["receivable"].ToString() != "0")
        //            {
        //                cellShow = new PdfPCell(new Phrase("應付金額:", font));
        //                cellShow.Border = 0;
        //                tabShow.AddCell(cellShow);
        //                cellShow = new PdfPCell(new Phrase(li.Rows[0]["receivable"].ToString(), font));
        //                cellShow.Border = 0;
        //                cellShow.PaddingLeft = 0;
        //                cellShow.Colspan = 3;
        //                tabShow.AddCell(cellShow);
        //            }
        //        document.Add(tabShow);
        //        PdfPTable table = new PdfPTable(7);
        //        table.SpacingBefore = 5;
        //            table.SetWidths(new int[] { 10, 50, 10, 5, 8, 10, 8 });
        //        PdfPCell cell = new PdfPCell(new Phrase("商品編號", font));
        //        cell.BorderWidth = (float)0.1;
        //        table.AddCell(cell);
        //        cell = new PdfPCell(new Phrase("商品名稱", font));
        //        cell.BorderWidth = (float)0.1;
        //        cell.HorizontalAlignment = 1;
        //        table.AddCell(cell);
        //        cell = new PdfPCell(new Phrase("托運單屬性", font));
        //        cell.BorderWidth = (float)0.1;
        //        cell.HorizontalAlignment = 1;
        //        table.AddCell(cell);
        //        cell = new PdfPCell(new Phrase("數量", font));
        //        cell.BorderWidth = (float)0.1;
        //        cell.HorizontalAlignment = 1;
        //        table.AddCell(cell);
        //        cell = new PdfPCell(new Phrase("本次出貨", font));
        //        cell.BorderWidth = (float)0.1;
        //        cell.HorizontalAlignment = 1;
        //        table.AddCell(cell);
        //        cell = new PdfPCell(new Phrase("預計出貨日", font));
        //        cell.BorderWidth = (float)0.1;
        //        cell.HorizontalAlignment = 1;
        //        table.AddCell(cell);
        //        cell = new PdfPCell(new Phrase("供應商自出", font));
        //        cell.BorderWidth = (float)0.1;
        //        cell.HorizontalAlignment = 1;
        //        table.AddCell(cell);
        //        for (int i = 0; i < li.Rows.Count; i++)
        //        {
        //                string item_id = string.Empty;
        //                if (li.Rows[i]["item_mode"].ToString() == "1")
        //                {
        //                    item_id = li.Rows[i]["parent_id"].ToString();
        //                }
        //                else
        //                {
        //                    item_id = li.Rows[i]["item_id"].ToString();
        //                }
        //                cell = new PdfPCell(new Phrase(item_id.ToString(), font));
        //            cell.BorderWidth = (float)0.1;
        //            table.AddCell(cell);
        //                string datacontent = ((li.Rows[i]["product_mode"].ToString() == "2" && li.Rows[i]["item_mode"].ToString() == "1") ? "*" : " ") + li.Rows[i]["brand_name"].ToString() + "-" + li.Rows[i]["product_name"].ToString() + li.Rows[i]["product_spec_name"].ToString();
        //                if (li.Rows[i]["combined_mode"].ToString() != "0" && li.Rows[i]["item_mode"].ToString() == "2")
        //                {
        //                    datacontent = "  " + datacontent;
        //                }
        //                cell = new PdfPCell(new Phrase(datacontent.ToString(), font));
        //            cell.HorizontalAlignment = 1;
        //            cell.BorderWidth = (float)0.1;
        //            table.AddCell(cell);
        //                string freight_set_name = string.Empty;
        //                if (Convert.ToInt32(li.Rows[i]["freight_set"]) == 1)
        //                {
        //                    freight_set_name = "常溫";
        //                }
        //                else if (Convert.ToInt32(li.Rows[i]["freight_set"]) == 2)
        //                {
        //                    freight_set_name = "冷凍";
        //                }
        //                else if (Convert.ToInt32(li.Rows[i]["freight_set"]) == 3)
        //                {
        //                    freight_set_name = "常溫免運";
        //                }
        //                else if (Convert.ToInt32(li.Rows[i]["freight_set"]) == 4)
        //                {
        //                    freight_set_name = "冷凍免運";
        //                }
        //                else if (Convert.ToInt32(li.Rows[i]["freight_set"]) == 5)
        //                {
        //                    freight_set_name = "冷藏";
        //                }
        //                else if (Convert.ToInt32(li.Rows[i]["freight_set"]) == 6)
        //                {
        //                    freight_set_name = "冷藏免運";
        //                }
        //                cell = new PdfPCell(new Phrase(freight_set_name, font));
        //            cell.HorizontalAlignment = 1;
        //            cell.BorderWidth = (float)0.1;
        //            table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(li.Rows[i]["item_mode"].ToString() != "1" ? li.Rows[i]["buy_num"].ToString() : "", font));
        //            cell.HorizontalAlignment = 1;
        //            cell.BorderWidth = (float)0.1;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("", font));
        //            cell.HorizontalAlignment = 1;
        //            cell.BorderWidth = (float)0.1;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("", font));
        //            cell.HorizontalAlignment = 1;
        //            cell.BorderWidth = (float)0.1;
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("", font));
        //            cell.HorizontalAlignment = 1;
        //            cell.BorderWidth = (float)0.1;
        //            table.AddCell(cell);
        //        }

        //        cell = new PdfPCell(new Phrase("備註：", font));
        //        cell.BorderWidth = (float)0.1;
        //        cell.MinimumHeight = 50;
        //        cell.Colspan = 7;
        //        //cell.BorderWidthBottom = 0;
        //        //cell.BorderWidthRight = 0;
        //        //cell.BorderWidthLeft = 0;
        //        table.AddCell(cell);

        //            Paragraph jine = new Paragraph(new Chunk("                  應收金額：", font));
        //        document.Add(table);
        //        Paragraph bz0 = new Paragraph(new Chunk("                    吉甲地市集網路平台購物發票說明:\n", font));
        //        document.Add(bz0);
        //        Paragraph bz1 = new Paragraph(new Chunk("                    若您訂購時未選擇開立三聯式發票，平台一律開發電子發票。\n", font));
        //        document.Add(bz1);
        //        Paragraph bz2 = new Paragraph(new Chunk("                    發票將於該筆訂單商品完全出貨之後第10天開立並以E-mail通知您。\n", font));
        //        document.Add(bz2);
        //        Paragraph bz4 = new Paragraph(new Chunk("                    如需紙本發票請來信客服中心，會計部門將會依需求將電子發票印出並以平信郵寄約2~7個工作天送達。\n", font));
        //        document.Add(bz4);
        //        Paragraph bz6 = new Paragraph(new Chunk("                    託管發票將會在單月26日進行兌獎作業後，系統將會發信通知中獎發票持有人，\n", font));
        //        document.Add(bz6);
        //        Paragraph bz7 = new Paragraph(new Chunk("                    且為保障您的權益，我們將在七個工作天內，以掛號方式把中獎發票寄給您。\n", font));
        //        document.Add(bz7);
        //        Paragraph bz8 = new Paragraph(new Chunk("                    祝您購物愉快！\n", bigFont));
        //        document.Add(bz8);
        //        document.NewPage();
        //    }
        //    document.Close();
        //        jsonStr = "{success:'true',filename:'" + filename + "'}";
        //    }
        //    catch (Exception ex)
        //    {

        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        jsonStr = "{success:false,result:0}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(jsonStr.ToString());
        //    this.Response.End();
        //    return this.Response;


        //}
        #endregion

        #region 出貨單+void GetDeliversPDF()
        /// <summary>
        /// 出貨單
        /// </summary>
        public void GetDeliversPDF()
        {
            TicketQuery query = new TicketQuery();
            string strTemp = Request.Params["ticket_id"].ToString();
            strTemp = strTemp.Remove(strTemp.LastIndexOf(','));
            query.ticketIds = strTemp;
            query.type = 1;
            _tkMgr = new TicketMgr(mySqlConnectionString);
            List<TicketQuery> list = _tkMgr.GetTicketDetail(query);
            Dictionary<string, string> dicproduct_freight_set = new Dictionary<string, string> { { "1", "1" }, { "2", "2" }, { "3", "1" }, { "4", "2" }, { "5", "5" }, { "6", "5" } };
            BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfChinese, 12, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
            int tTemp = 0;
            if (int.TryParse(strTemp, out tTemp))
            {
                filename = "order_details_D" + strTemp.PadLeft(8, '0') + ".pdf";
            }
            else
            {
                filename = "order_details_all.pdf";
            }
            Document document = new Document(PageSize.A4, (float)5, (float)5, (float)20, (float)0.5);
            string newPDFName = Server.MapPath(excelPath) + filename;
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
            document.Open();
            //cb.SetTextMatrix(document.Left, document.Bottom - 15);
            if (list.Count > 0)
            {
                foreach (var m in list)
                {
                    string deliver_id = m.deliver_id.ToString();
                    _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                    DataTable orderdeliver = _DeliverDetailMgr.GetOrderDelivers(deliver_id);
                    PdfContentByte cb = writer.DirectContent;
                    cb.BeginText();
                    Phrase ph = new Phrase("\n \n", font);
                    BarCode.Code128 _Code = new BarCode.Code128();
                    _Code.ValueFont = new System.Drawing.Font("宋体", 20);
                    System.Drawing.Bitmap imgTemp = _Code.GetCodeImage("D" + deliver_id.PadLeft(8, '0'), BarCode.Code128.Encode.Code128A);
                    imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                    iTextSharp.text.Image IMG = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                    IMG.ScaleToFit(200, 30);
                    IMG.SetAbsolutePosition(345, 740);
                    //IMG.
                    //Chunk ck = new Chunk(IMG, 345, -40); //图片可设置 偏移
                    //ph.Add(ck);
                    //document.Add(ph);
                    cb.SetFontAndSize(bfChinese, 20);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "吉甲地市集出貨明細", 30, 750, 0);
                    if (orderdeliver.Rows[0]["priority"].ToString() == "1")
                    {
                        PdfPTable ot = new PdfPTable(1);
                        ot.SetTotalWidth(new float[] { 190 });
                        PdfPCell c = new PdfPCell(new Phrase("", font));
                        c.FixedHeight = 30;
                        c.BorderWidthBottom = 0.5f;
                        c.BorderWidthLeft = 0.5f;
                        c.BorderWidthRight = 0.5f;
                        c.BorderWidthTop = 0.5f;
                        ot.AddCell(c);
                        ot.WriteSelectedRows(0, -1, 29, 770, cb);
                    }
                    cb.AddImage(IMG);
                    if (orderdeliver.Rows.Count > 0)
                    {

                        if (orderdeliver.Rows[0]["channel"].ToString() != "1")
                        {
                            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["channel_name_simple"].ToString(), 80, 700, 0);
                        }
                        if (orderdeliver.Rows[0]["retrieve_mode"].ToString() == "1")
                        {
                            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "7-11取貨", 200, 700, 0);
                        }
                        cb.SetFontAndSize(bfChinese, 10);
                        string freight_set = string.Empty;
                        switch (orderdeliver.Rows[0]["freight_set"].ToString().Trim())
                        {
                            case "1":
                                freight_set = "常溫";
                                break;
                            case "2":
                                freight_set = "冷凍";
                                break;
                            case "5":
                                freight_set = "冷藏";
                                break;
                        }
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, freight_set, 345, 785, 0);
                        string estimated_arrival_period = string.Empty;
                        if (orderdeliver.Rows[0]["estimated_arrival_period"].ToString() != "0")
                        {
                            switch (orderdeliver.Rows[0]["estimated_arrival_period"].ToString().Trim())
                            {
                                case "0":
                                    estimated_arrival_period = "不限時";
                                    break;
                                case "1":
                                    estimated_arrival_period = "12:00以前";
                                    break;
                                case "2":
                                    estimated_arrival_period = "12:00-17:00";
                                    break;
                                case "3":
                                    estimated_arrival_period = "17:00-20:00";
                                    break;
                            }

                            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, estimated_arrival_period, 345, 773, 0);
                        }

                    }
                    cb.SetFontAndSize(bfChinese, 10);
                    //cb.SetTextMatrix(150,20);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "訂購人：", 10, 680, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "收件人：", 200, 680, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "付款單號：", 10, 660, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "收件地址：", 200, 660, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "訂購時間：", 10, 640, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "聯絡電話：", 200, 640, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "付款時間：", 10, 620, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "假日可收貨：", 200, 620, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "出貨備註：", 10, 600, 0);
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "訂單明細：", 10, 580, 0);
                    if (orderdeliver.Rows.Count > 0)
                    {
                        if (orderdeliver.Rows[0]["receivable"].ToString() != "0")
                        {
                            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "應收金額：" + orderdeliver.Rows[0]["receivable"].ToString(), 200, 580, 0);
                        }
                    }
                    string address = string.Empty;
                    string deliver_note = string.Empty;
                    if (orderdeliver.Rows.Count > 0)
                    {
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["order_name"].ToString(), 65, 680, 0);
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["delivery_name"].ToString(), 250, 680, 0);
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["order_id"].ToString(), 65, 660, 0);
                        address += CommonFunction.ZipAddress(orderdeliver.Rows[0]["delivery_zip"].ToString()) + orderdeliver.Rows[0]["delivery_address"].ToString();

                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, address, 250, 660, 0);
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["order_createdate"].ToString() != "0" ? CommonFunction.GetNetTime(long.Parse(orderdeliver.Rows[0]["order_createdate"].ToString())).ToString("yyyy-MM-dd HH:mm:ss") : "", 65, 640, 0);
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["delivery_mobile"].ToString(), 250, 640, 0);
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["money_collect_date"].ToString() != "0" ? CommonFunction.GetNetTime(long.Parse(orderdeliver.Rows[0]["money_collect_date"].ToString())).ToString("yyyy-MM-dd HH:mm:ss") : "", 65, 620, 0);
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["holiday_deliver"].ToString() == "1" ? "可" : "不可", 260, 620, 0);
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["note_order"].ToString(), 65, 600, 0);
                        if (orderdeliver.Rows[0]["delivery_store"].ToString() == "12")
                        {
                            deliver_note = "*自取(取貨地址:台北市南港區八德路4段768巷7號6樓之1，取貨時間週一~週五，AM9:00~PM6:00)";
                        }
                        else if (orderdeliver.Rows[0]["delivery_store"].ToString() == "13")
                        {
                            deliver_note = "*自取(取貨地址:新北市板橋區三民路二段33號21樓，取貨時間週一~週五，AM9:00~PM6:00)";
                        }
                        else if (orderdeliver.Rows[0]["delivery_store"].ToString() == "14")
                        {
                            deliver_note = "*自取(取貨地址:新北市永和區成功路一段80號20樓，取貨時間週一~週五，AM9:00~PM6:00)";
                        }
                        cb.SetFontAndSize(bfChinese, 8);
                        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliver_note, 65, 580, 0);
                    }
                    cb.EndText();
                    PdfPTable ptable = new PdfPTable(7);
                    ptable.SetTotalWidth(new float[] { 50, 280, 50, 50, 50, 50, 50 });
                    ptable.WidthPercentage = 98;
                    PdfPCell cell;
                    font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    cell = new PdfPCell(new Phrase("商品編號", font));
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("商品名稱", font));
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("托運單屬性", font));
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("數量", font));
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("本次出貨", font));
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("預計出貨日", font));
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(8);
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("供應商自出", font));
                    cell.DisableBorderSide(2);
                    ptable.AddCell(cell);
                    PdfPCell td;
                    string lastdeliverid = "0";
                    ArrayList normal = new ArrayList();
                    ArrayList low = new ArrayList();
                    ArrayList lowstore = new ArrayList();
                    DataRow[] sinceorder = new DataRow[] { };

                    DataRow[] singleproduct = new DataRow[] { };//單一商品
                    DataRow[] fatherproduct = new DataRow[] { };//組合商品中的父商品
                    DataRow[] sonproduct = new DataRow[] { };//組合商品中的子商品
                    ArrayList combine = new ArrayList();
                    List<DataRow[]> orderdelivers = new List<DataRow[]>();

                    sinceorder = orderdeliver.Select("dtype=2 and combined_mode<=1 ", "item_id asc");//自出商品
                    singleproduct = orderdeliver.Select("dtype <>2 and combined_mode<=1  ", "item_id asc");//單一商品
                    if (singleproduct.Count() > 0)
                    {
                        orderdelivers.Add(singleproduct);
                    }
                    fatherproduct = orderdeliver.Select(" combined_mode>1 and item_mode=1", "item_id asc");//組合商品中父商品是否存在
                    foreach (var item in fatherproduct)
                    {
                        combine.Add(item);
                        sonproduct = orderdeliver.Select(" combined_mode>1 and item_mode<>1 and parent_id=" + item["parent_id"] + " and pack_id=" + item["pack_id"], "item_id asc");//對應組合商品中的子商品
                        foreach (var son in sonproduct)
                        {
                            son["buy_num"] = (int.Parse(son["buy_num"].ToString()) * int.Parse(son["parent_num"].ToString())).ToString();
                            combine.Add(son);
                        }

                    }
                    if (combine.Count > 0)
                    {
                        orderdelivers.Add((DataRow[])combine.ToArray(typeof(DataRow)));
                    }
                    //區分常溫、冷凍、冷藏
                    foreach (var item in orderdelivers)
                    {
                        foreach (var row in item)
                        {
                            string s = row["product_freight_set"].ToString();
                            switch (row["product_freight_set"].ToString())
                            {
                                case "1":
                                case "3":
                                    normal.Add(row);//常溫
                                    break;
                                case "2":
                                case "4":
                                    low.Add(row);//冷凍
                                    break;
                                case "5":
                                case "6":
                                    lowstore.Add(row);//冷藏
                                    break;
                                default:
                                    break;


                            }
                        }
                    }

                    orderdelivers = new List<DataRow[]>();
                    if (normal.Count > 0)
                    {
                        orderdelivers.Add((DataRow[])normal.ToArray(typeof(DataRow)));
                    }
                    if (low.Count > 0)
                    {
                        orderdelivers.Add((DataRow[])low.ToArray(typeof(DataRow)));
                    }
                    if (lowstore.Count > 0)
                    {
                        orderdelivers.Add((DataRow[])lowstore.ToArray(typeof(DataRow)));
                    }
                    if (sinceorder.Count() > 0)
                    {
                        orderdelivers.Add(sinceorder);
                    }
                    int j = 0;
                    foreach (var item in orderdelivers)
                    {
                        j++;
                        for (int i = 0; i < item.Count(); i++)
                        {
                            if (item[i]["ddeliver_id"].ToString() != lastdeliverid || i == 0)
                            {
                                lastdeliverid = item[i]["ddeliver_id"].ToString();//以一個出貨單號為界限
                                if (lastdeliverid != "0" || i == 0)
                                {
                                    td = new PdfPCell();
                                    td.Colspan = 7;
                                    td.DisableBorderSide(2);
                                    td.DisableBorderSide(4);
                                    td.DisableBorderSide(8);
                                    //td.BorderWidthTop = 0.2f;
                                    ptable.AddCell(td);
                                }
                            }
                            string item_id = string.Empty;
                            if (item[i]["item_mode"].ToString() == "1")
                            {
                                item_id = item[i]["parent_id"].ToString();
                            }
                            else
                            {
                                item_id = item[i]["item_id"].ToString();
                            }
                            font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                            td = new PdfPCell(new Phrase(item_id, font));
                            td.DisableBorderSide(1);
                            td.DisableBorderSide(2);
                            td.DisableBorderSide(8);
                            ptable.AddCell(td);
                            string datacontent = ((item[i]["product_mode"].ToString() == "2" && item[i]["item_mode"].ToString() == "1") ? "*" : " ") + item[i]["brand_name"].ToString() + "-" + item[i]["product_name"].ToString() + item[i]["product_spec_name"].ToString();
                            if (item[i]["combined_mode"].ToString() != "0" && item[i]["item_mode"].ToString() == "2")
                            {
                                datacontent = "  " + datacontent;
                            }
                            td = new PdfPCell(new Phrase(datacontent, font));
                            td.DisableBorderSide(1);
                            td.DisableBorderSide(2);
                            td.DisableBorderSide(8);
                            ptable.AddCell(td);
                            string value = string.Empty;
                            string freight_set = string.Empty;
                            if (dicproduct_freight_set.TryGetValue(item[i]["product_freight_set"].ToString(), out value))
                            {

                            }
                            switch (value)
                            {
                                case "1":
                                    freight_set = "常温";
                                    break;
                                case "2":
                                    freight_set = "冷冻";
                                    break;
                                case "5":
                                    freight_set = "冷藏";
                                    break;
                            }
                            td = new PdfPCell(new Phrase(freight_set, font));
                            td.DisableBorderSide(1);
                            td.DisableBorderSide(2);
                            td.DisableBorderSide(8);
                            ptable.AddCell(td);
                            font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                            td = new PdfPCell(new Phrase(item[i]["item_mode"].ToString() != "1" ? item[i]["buy_num"].ToString() : "", font));
                            td.DisableBorderSide(1);
                            td.DisableBorderSide(2);
                            td.DisableBorderSide(8);
                            ptable.AddCell(td);
                            td = new PdfPCell(new Phrase("", font));
                            td.DisableBorderSide(1);
                            td.DisableBorderSide(2);
                            td.DisableBorderSide(8);
                            ptable.AddCell(td);
                            td = new PdfPCell(new Phrase("", font));
                            td.DisableBorderSide(1);
                            td.DisableBorderSide(2);
                            td.DisableBorderSide(8);
                            ptable.AddCell(td);
                            Image image = Image.GetInstance(Server.MapPath("../Content/img/icons/mark.png"));
                            image.ScalePercent(5, 5);
                            if (item[i]["dtype"].ToString() == "2")
                            {
                                td = new PdfPCell(image, false);
                            }
                            else
                            {
                                td = new PdfPCell();
                            }
                            td.HorizontalAlignment = Element.ALIGN_CENTER;
                            td.VerticalAlignment = Element.ALIGN_MIDDLE;
                            td.DisableBorderSide(1);
                            td.DisableBorderSide(2);
                            ptable.AddCell(td);
                        }


                    }
                    string note_order = orderdeliver.Rows.Count.ToString() != "0" ? orderdeliver.Rows[0]["note_order"].ToString() : "";
                    cell = new PdfPCell(new Phrase("備註:" + note_order, font));
                    cell.Colspan = 7;
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.BorderColor = new iTextSharp.text.BaseColor(0, 0, 0);
                    ptable.AddCell(cell);
                    PdfPTable nulltable = new PdfPTable(2);
                    nulltable.SetWidths(new int[] { 20, 20 });
                    nulltable.DefaultCell.DisableBorderSide(1);
                    nulltable.DefaultCell.DisableBorderSide(2);
                    nulltable.DefaultCell.DisableBorderSide(4);
                    nulltable.DefaultCell.DisableBorderSide(8);
                    nulltable.AddCell("");
                    nulltable.AddCell("");
                    nulltable.SpacingAfter = 250;
                    document.Add(nulltable);
                    ptable.SpacingAfter = 50;
                    document.Add(ptable);
                    font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    document.Add(new Phrase("吉甲地市集網路平台購物發票說明:\n", font));
                    font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    document.Add(new Phrase("若您訂購時未選擇開立三聯式發票，平台一律開立電子發票。\n", font));
                    document.Add(new Phrase("發票將於該筆訂單商品完全出貨之後第10天開立並以E-Mail通知您。\n", font));
                    document.Add(new Phrase("如需紙本發票請來信客服中心，會計部門將會依需求將電子發票印出並以平信郵寄約2~7個工作天內送達。\n", font));
                    document.Add(new Phrase("託管發票將會在單月26日進行對獎作業後，系統將會發信通知中獎發票持有人，\n", font));
                    document.Add(new Phrase("且為保障您的權益，我們將在七個工作天內，以掛號方式把中獎發票寄給您。\n", font));
                    font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    document.Add(new Phrase("祝您購物愉快!", font));
                    document.NewPage();
                }
            }
            else
            {
                PdfPTable nulltable = new PdfPTable(2);
                nulltable.SetWidths(new int[] { 20, 20 });
                nulltable.DefaultCell.DisableBorderSide(1);
                nulltable.DefaultCell.DisableBorderSide(2);
                nulltable.DefaultCell.DisableBorderSide(4);
                nulltable.DefaultCell.DisableBorderSide(8);
                nulltable.AddCell("");
                nulltable.AddCell("");
                nulltable.SpacingAfter = 250;
                document.Add(nulltable);
            }

            document.Close();
            Response.Clear();
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            // Response.AddHeader("Content-Disposition", "attach-ment;filename=" + System.Web.HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8) + ".pdf ");
            Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename);
            Response.WriteFile(newPDFName);
        }
        #endregion

        public void GetWaybillsXls()
        {
            if (!string.IsNullOrEmpty(Request.Params["ticket_id"]))
            {
                string strTemp = Request.Params["ticket_id"].ToString();
                strTemp = strTemp.Remove(strTemp.LastIndexOf(','));

                string newPDFName = string.Empty;
                int iTemp = 0;
                if (int.TryParse(strTemp, out iTemp))
                {
                    newPDFName = "waybills_T" + strTemp.PadLeft(8, '0').ToString() + ".xls";
                }
                else
                {
                    string ss = strTemp.Substring(0, strTemp.IndexOf(','));
                    newPDFName = "waybills_T" + ss.PadLeft(8, '0').ToString() + ".xls";
                }

                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                DataTable dt = _DeliverDetailMgr.GetWayBills(null, strTemp);
                DataTable dtTemp = new DataTable();
                dtTemp.Columns.Add("訂單編號", typeof(String));
                dtTemp.Columns.Add("收件人姓名", typeof(String));
                dtTemp.Columns.Add("收件人電話", typeof(String));
                dtTemp.Columns.Add("收件人手機", typeof(String));
                dtTemp.Columns.Add("收件人地址", typeof(String));
                dtTemp.Columns.Add("出貨日期\nYYYYMMDD", typeof(String));
                dtTemp.Columns.Add("預定配達日期\n YYYYMMDD", typeof(String));
                dtTemp.Columns.Add("預定配達時段\n (1:中午前~2:12-17時~3:17-20時~4:不指定~5:20-21時)", typeof(String));
                dtTemp.Columns.Add("品名", typeof(String));
                dtTemp.Columns.Add("代收貨款", typeof(String));
                dtTemp.Columns.Add("溫層\n (1:常溫~2:冷藏~3:冷凍)", typeof(String));
                dtTemp.Columns.Add("距離\n  (0:同縣市~1:外縣市~2:離島)", typeof(String));
                dtTemp.Columns.Add("規格\n  (1:60cm~2:90cm~3:120cm~4:150cm)", typeof(String));
                dtTemp.Columns.Add("易碎 \n  (Y~N)", typeof(String));
                dtTemp.Columns.Add("精密 \n  (Y~N)", typeof(String));
                dtTemp.Columns.Add("備註", typeof(String));
                dtTemp.Columns.Add("假日可出貨", typeof(String));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow dr = dtTemp.NewRow();
                    dr[0] = dt.Rows[i]["order_id"];
                    dr[1] = dt.Rows[i]["delivery_name"];
                    dr[2] = dt.Rows[i]["delivery_mobile"];

                    dr[3] = dt.Rows[i]["delivery_phone"];
                    dr[4] = CommonFunction.ZipAddress(dt.Rows[i]["delivery_zip"].ToString()) + dt.Rows[i]["delivery_address"].ToString();

                    dr[5] = DateTime.Now.ToString("yyyy-MM-dd");

                    dr[7] = dt.Rows[i]["estimated_arrival_period"];
                    if (dt.Rows[i]["estimated_arrival_period"].ToString() == "2")
                    {
                        dr[8] = "冷凍食品";
                    }

                    dr[9] = dt.Rows[i]["receivable"];
                    dr[10] = dt.Rows[i]["freight_set"];
                    dr[15] = dt.Rows[i]["note_order"];
                    dr[16] = dt.Rows[i]["holiday_deliver"].ToString() == "1" ? "是" : "否";
                    dtTemp.Rows.Add(dr);
                }

                MemoryStream ms = ExcelHelperXhf.ExportDT(dtTemp, "");
                //Response.Clear();
                //Response.Charset = "gb2312";
                //Response.ContentEncoding = System.Text.Encoding.UTF8;
                // Response.AddHeader("Content-Disposition", "attach-ment;filename=" + System.Web.HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8) + ".pdf ");
                Response.AddHeader("Content-Disposition", "attach-ment;filename=" + newPDFName);
                Response.BinaryWrite(ms.ToArray());
            }
        }
        #endregion

        #region 供應商下拉列表
        public HttpResponseBase GetVendorName()
        {
            _IiplasMgr = new IplasMgr(mySqlConnectionString);
            List<Vendor> stores = new List<Vendor>();
            string json = string.Empty;
            try
            {
                Vendor query = new Vendor();
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["type"]))
                {
                    sql = "";
                }
                else
                {
                    sql = " and assist = 1 ";
                }
                stores = _IiplasMgr.VendorQueryAll(query, sql);
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

        #region 出貨確認
        /// <summary>
        /// 出貨確認列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeliverVerifyList()
        {
            DeliverMaster dm = new DeliverMaster();
            _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);
            List<DeliverMasterQuery> stores = new List<DeliverMasterQuery>();
            string json = string.Empty;
            try
            {
                dm.deliver_id = uint.Parse(Request.Params["deliver_id"]);
                dm.order_id = int.Parse(Request.Params["order_id"]);
                int totalCount = 0;
                stores = _DeliverMsterMgr.DeliverVerifyList(dm, out totalCount);
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
        /// <summary>
        /// 驗證訂單編號和出貨單號的對應
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase JudgeOrdid()
        {
            string json = String.Empty;
            DeliverMaster dm = new DeliverMaster();
            List<DeliverMasterQuery> list = new List<DeliverMasterQuery>();
            _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);
            int msg = 0;
            try
            {
                string deliver_id = Request.Params["deliver_id"];
                string D = deliver_id.Substring(0, 1);
                if (D == "D")
                {
                    deliver_id = deliver_id.Substring(1, deliver_id.Length - 1);
                    uint deliver; int ord;
                    if (uint.TryParse(deliver_id, out deliver))
                    {
                        dm.deliver_id = uint.Parse(deliver_id);
                    }
                    if (int.TryParse(Request.Params["order_id"], out ord))
                    {
                        dm.order_id = int.Parse(Request.Params["order_id"]);
                    }
                    if (dm.deliver_id != 0 && dm.order_id != 0)
                    {
                        list = _DeliverMsterMgr.JudgeOrdid(dm);
                    }
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            if (item.delivery_status >= 3)
                            {
                                msg = 2;
                            }
                        }
                        json = "{success:true,msg:" + msg + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented) + "}";//返回json數據  
                    }
                    else
                    {
                        dm.deliver_id = 0;
                        list = _DeliverMsterMgr.JudgeOrdid(dm);
                        if (list.Count > 0)
                        {
                            msg = 5;
                            json = "{success:true,msg:" + msg + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented) + "}";//返回json數據  
                        }
                        else
                        {
                            json = "{success:true,msg:1}";
                        }
                    }
                }
                else
                {
                    json = "{success:true,msg:3}";
                }
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 出貨查詢中的檢視頁面
        #region 頁面上的方法
        /// <summary>
        /// 獲取grid的數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetDeliverDetail()
        {
            string json = String.Empty;
            StringBuilder sb = new StringBuilder();
            List<DeliverDetailQuery> store = new List<DeliverDetailQuery>();
            List<DeliverDetailQuery> cancel = new List<DeliverDetailQuery>();//取消狀態列表
            List<DeliverDetailQuery> newstore = new List<DeliverDetailQuery>();//去掉組合商品中的父商品
            DeliverDetailQuery query = new DeliverDetailQuery();
            try
            {

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))
                {
                    query.deliver_id = uint.Parse(Request.Params["deliver_id"]);
                }
                store = _DeliverDetailMgr.GetDeliverDetail(query);
                uint[] cancelstatus = { 5, 89, 90, 91, 92 };//取消
                IList cstatus = (IList)cancelstatus;
                foreach (var item in store)
                {
                    if (item.item_mode != 1)
                    {
                        newstore.Add(item);
                        if (cstatus.Contains(item.detail_status))
                        {
                            cancel.Add(item);
                        }

                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,normaldata:" + JsonConvert.SerializeObject(newstore, Formatting.Indented, timeConverter) + ",canceldata:" + JsonConvert.SerializeObject(cancel, Formatting.Indented, timeConverter) + "}";//返回json數據

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
        /// <summary>
        /// 獲取出貨信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetDeliverMaster()
        {
            string json = String.Empty;
            List<DeliverMasterQuery> store = new List<DeliverMasterQuery>();
            DeliverMasterQuery query = new DeliverMasterQuery();
            try
            {
                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))
                {
                    query.deliver_id = uint.Parse(Request.Params["deliver_id"]);
                }
                store = _DeliverDetailMgr.GetDeliverMaster(query);
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
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 改自出 改寄倉 改調度
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase ProductMode()
        {
            string json = string.Empty;
            try
            {
                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                string deliver_id = Request.Params["deliver_id"];
                string detail_id = Request.Params["detail_id"];
                string product_mode = Request.Params["product_mode"];
                string new_deliver_id = _DeliverDetailMgr.ProductMode(deliver_id, detail_id, product_mode);
                if (!string.IsNullOrEmpty(new_deliver_id))
                {
                    json = "{success:true,msg:'" + new_deliver_id + "'}";
                }

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
        /// 未到貨
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase LackDelivery()
        {
            string json = string.Empty;
            try
            {
                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                string deliver_id = Request.Params["deliver_id"];
                string detail_id = Request.Params["detail_id"];
                bool success = _DeliverDetailMgr.NoDelivery(deliver_id, detail_id);
                if (success == true)
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
        /// <summary>
        /// 拆分細項
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SplitDetail()
        {
            string json = string.Empty;
            try
            {
                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                string deliver_id = Request.Params["deliver_id"];
                string detail_id = Request.Params["detail_id"];
                bool success = _DeliverDetailMgr.SplitDetail(deliver_id, detail_id);
                if (success)
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
        /// <summary>
        /// 下一次出貨
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase Split()
        {
            string json = string.Empty;
            try
            {
                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                string deliver_id = Request.Params["deliver_id"];
                string detail_ids = Request.Params["detail_ids"];
                string[] detailids = detail_ids.Split(',');
                string newdeliverid = _DeliverDetailMgr.Split(deliver_id, detailids);
                if (!string.IsNullOrEmpty(newdeliverid))
                {
                    json = "{success:true,msg:" + newdeliverid + "}";
                }
                else
                {
                    json = "{success:false:msg:}";
                }


            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false:msg:}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        /// <summary>
        /// 修改物流單號 出貨日期 出貨信息
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeliverDetailEdit()
        {
            string json = string.Empty;
            try
            {
                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                string deliver_id = Request.Params["deliver_id"];
                string delivery_store = string.Empty;
                string delivery_code = string.Empty;
                string delivery_date = string.Empty;
                string sms_date = string.Empty;
                bool success = false;
                if (!string.IsNullOrEmpty(Request.Params["delivery_store"]) && !string.IsNullOrEmpty(Request.Params["delivery_code"]))
                {
                    delivery_store = Request.Params["delivery_store"];
                    delivery_code = Request.Params["delivery_code"];
                    delivery_date = Request.Params["delivery_date"];
                    success = _DeliverDetailMgr.DeliveryCode(deliver_id, delivery_store, delivery_code, delivery_date, "0");
                    if (success)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                else if (!string.IsNullOrEmpty(Request.Params["sms_date"]))
                {
                    Sms sms = new Sms();
                    sms.memo = deliver_id;
                    string smsid = _DeliverDetailMgr.GetSmsId(sms);
                    if (!string.IsNullOrEmpty(smsid))
                    {
                        int i = _DeliverDetailMgr.UpSmsTime(deliver_id, sms_date, smsid);
                        if (i > 0)
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
                        json = "{success:false}";
                    }
                }
                else
                {
                    DeliverMaster dm = new DeliverMaster();
                    int type = int.Parse(Request.Params["type"]);
                    dm.deliver_id = uint.Parse(deliver_id);
                    if (type == 1)
                    {
                        if (!string.IsNullOrEmpty(Request.Params["estimated_arrival_date"]))
                        {
                            dm.estimated_arrival_date = DateTime.Parse(Request.Params["estimated_arrival_date"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["estimated_delivery_date"]))
                        {
                            dm.estimated_delivery_date = DateTime.Parse(Request.Params["estimated_arrival_date"]);
                        }
                        dm.estimated_arrival_period = int.Parse(Request.Params["estimated_arrival_period"]);
                        //dm.estimated_delivery_date = DateTime.Parse(Request.Params["estimated_delivery_date"]);

                    }
                    if (type == 2)
                    {
                        dm.delivery_name = Request.Params["delivery_name"];
                        dm.delivery_mobile = Request.Params["delivery_mobile"];
                        dm.delivery_phone = Request.Params["delivery_phone"];
                        dm.delivery_zip = uint.Parse(Request.Params["delivery_zip"]);
                        dm.delivery_address = Request.Params["delivery_address"];
                    }
                    int j = _DeliverDetailMgr.DeliverMasterEdit(dm, type);
                    if (j == 1)
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

        #region 匯出PDF文件
        /// <summary>
        /// 订单出货明细
        /// </summary>
        public void GetOrderDetailsPDF()
        {
            string deliver_id = Request.Params["deliver_id"];
            _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
            DataTable orderdeliver = _DeliverDetailMgr.GetOrderDelivers(deliver_id);
            Dictionary<string, string> dicproduct_freight_set = new Dictionary<string, string> { { "1", "1" }, { "2", "2" }, { "3", "1" }, { "4", "2" }, { "5", "5" }, { "6", "5" } };
            BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfChinese, 12, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑

            string filename = "order_details_D" + deliver_id.PadLeft(8, '0') + ".pdf";
            Document document = new Document(PageSize.A4, (float)5, (float)5, (float)20, (float)0.5);
            string newPDFName = Server.MapPath(excelPath) + filename;
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
            document.Open();

            PdfContentByte cb = writer.DirectContent;

            if (orderdeliver.Rows.Count > 0)
            {
                #region 生成條形碼
                BarCode.Code128 _Code = new BarCode.Code128();
                _Code.ValueFont = new System.Drawing.Font("宋体", 20);
                System.Drawing.Bitmap imgTemp = _Code.GetCodeImage("D" + orderdeliver.Rows[0]["deliver_id"].ToString().PadLeft(8, '0'), BarCode.Code128.Encode.Code128A);
                imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                iTextSharp.text.Image IMG = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                IMG.ScaleToFit(200, 30);
                IMG.SetAbsolutePosition(345, 740);
                #endregion

                cb.BeginText();
                cb.SetFontAndSize(bfChinese, 20);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "吉甲地市集出貨明細", 30, 750, 0);
                //首購
                if (orderdeliver.Rows[0]["priority"].ToString() == "1")
                {
                    PdfPTable ot = new PdfPTable(1);
                    ot.SetTotalWidth(new float[] { 190 });
                    PdfPCell c = new PdfPCell(new Phrase("", font));
                    c.FixedHeight = 30;
                    c.BorderWidthBottom = 0.5f;
                    c.BorderWidthLeft = 0.5f;
                    c.BorderWidthRight = 0.5f;
                    c.BorderWidthTop = 0.5f;
                    ot.AddCell(c);
                    ot.WriteSelectedRows(0, -1, 29, 770, cb);
                }
                cb.AddImage(IMG);
                if (orderdeliver.Rows[0]["channel"].ToString() != "1")
                {
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["channel_name_simple"].ToString(), 80, 700, 0);
                }
                if (orderdeliver.Rows[0]["retrieve_mode"].ToString() == "1")
                {
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "7-11取貨", 200, 700, 0);
                }
                cb.SetFontAndSize(bfChinese, 10);
                string freight_set = string.Empty;
                switch (orderdeliver.Rows[0]["freight_set"].ToString().Trim())
                {
                    case "1":
                        freight_set = "常溫";
                        break;
                    case "2":
                        freight_set = "冷凍";
                        break;
                    case "5":
                        freight_set = "冷藏";
                        break;
                }
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, freight_set, 345, 785, 0);
                string estimated_arrival_period = string.Empty;
                if (orderdeliver.Rows[0]["estimated_arrival_period"].ToString() != "0")
                {
                    switch (orderdeliver.Rows[0]["estimated_arrival_period"].ToString().Trim())
                    {
                        case "0":
                            estimated_arrival_period = "不限時";
                            break;
                        case "1":
                            estimated_arrival_period = "12:00以前";
                            break;
                        case "2":
                            estimated_arrival_period = "12:00-17:00";
                            break;
                        case "3":
                            estimated_arrival_period = "17:00-20:00";
                            break;
                    }

                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, estimated_arrival_period, 345, 773, 0);
                }

            }
            cb.SetFontAndSize(bfChinese, 10);
            //cb.SetTextMatrix(150,20);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "訂購人：", 10, 680, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "收件人：", 200, 680, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "付款單號：", 10, 660, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "收件地址：", 200, 660, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "訂購時間：", 10, 640, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "聯絡電話：", 200, 640, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "付款時間：", 10, 620, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "假日可收貨：", 200, 620, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "出貨備註：", 10, 600, 0);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "訂單明細：", 10, 580, 0);
            if (orderdeliver.Rows.Count > 0 && orderdeliver.Rows[0]["receivable"].ToString() != "0")
            {
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "應收金額：" + orderdeliver.Rows[0]["receivable"].ToString(), 200, 580, 0);
            }

            string address = string.Empty;
            string deliver_note = string.Empty;
            if (orderdeliver.Rows.Count > 0)
            {
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["order_name"].ToString(), 65, 680, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["delivery_name"].ToString(), 250, 680, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["order_id"].ToString(), 65, 660, 0);
                address += CommonFunction.ZipAddress(orderdeliver.Rows[0]["delivery_zip"].ToString()) + orderdeliver.Rows[0]["delivery_address"].ToString();

                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, address, 250, 660, 0);
                string order_createdate = orderdeliver.Rows[0]["order_createdate"].ToString() != "0" ? CommonFunction.GetNetTime(long.Parse(orderdeliver.Rows[0]["order_createdate"].ToString())).ToString("yyyy-MM-dd HH:mm:ss") : "";
                string money_collect_date = orderdeliver.Rows[0]["money_collect_date"].ToString() != "0" ? CommonFunction.GetNetTime(long.Parse(orderdeliver.Rows[0]["money_collect_date"].ToString())).ToString("yyyy-MM-dd HH:mm:ss") : "";
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, order_createdate, 65, 640, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["delivery_mobile"].ToString(), 250, 640, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, money_collect_date, 65, 620, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["holiday_deliver"].ToString() == "1" ? "可" : "不可", 260, 620, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, orderdeliver.Rows[0]["note_order"].ToString().Trim(), 65, 600, 0);
                if (orderdeliver.Rows[0]["delivery_store"].ToString() == "12")
                {
                    deliver_note = "*自取(取貨地址:台北市南港區八德路4段768巷7號6樓之1，取貨時間週一~週五，AM9:00~PM6:00)";
                }
                else if (orderdeliver.Rows[0]["delivery_store"].ToString() == "13")
                {
                    deliver_note = "*自取(取貨地址:新北市板橋區三民路二段33號21樓，取貨時間週一~週五，AM9:00~PM6:00)";
                }
                else if (orderdeliver.Rows[0]["delivery_store"].ToString() == "14")
                {
                    deliver_note = "*自取(取貨地址:新北市永和區成功路一段80號20樓，取貨時間週一~週五，AM9:00~PM6:00)";
                }
                cb.SetFontAndSize(bfChinese, 8);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliver_note, 65, 580, 0);
            }
            cb.EndText();
            PdfPTable ptable = new PdfPTable(7);
            ptable.WidthPercentage = 98;
            ptable.SetTotalWidth(new float[] { 50, 280, 50, 50, 50, 50, 50 });
            PdfPCell cell;
            font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
            cell = new PdfPCell(new Phrase("商品編號", font));
            cell.DisableBorderSide(2);
            cell.DisableBorderSide(8);
            ptable.AddCell(cell);
            cell = new PdfPCell(new Phrase("商品名稱", font));
            cell.DisableBorderSide(2);
            cell.DisableBorderSide(8);
            ptable.AddCell(cell);
            cell = new PdfPCell(new Phrase("托運單屬性", font));
            cell.DisableBorderSide(2);
            cell.DisableBorderSide(8);
            ptable.AddCell(cell);
            cell = new PdfPCell(new Phrase("數量", font));
            cell.DisableBorderSide(2);
            cell.DisableBorderSide(8);
            ptable.AddCell(cell);
            cell = new PdfPCell(new Phrase("本次出貨", font));
            cell.DisableBorderSide(2);
            cell.DisableBorderSide(8);
            ptable.AddCell(cell);
            cell = new PdfPCell(new Phrase("預計出貨日", font));
            cell.DisableBorderSide(2);
            cell.DisableBorderSide(8);
            ptable.AddCell(cell);
            cell = new PdfPCell(new Phrase("供應商自出", font));
            cell.DisableBorderSide(2);
            ptable.AddCell(cell);
            PdfPCell td;
            string lastdeliverid = "0";
            ArrayList normal = new ArrayList();
            ArrayList low = new ArrayList();
            ArrayList lowstore = new ArrayList();
            DataRow[] sinceorder = new DataRow[] { };

            DataRow[] singleproduct = new DataRow[] { };//單一商品
            DataRow[] fatherproduct = new DataRow[] { };//組合商品中的父商品
            DataRow[] sonproduct = new DataRow[] { };//組合商品中的子商品
            ArrayList combine = new ArrayList();
            List<DataRow[]> orderdelivers = new List<DataRow[]>();

            sinceorder = orderdeliver.Select("dtype=2 and combined_mode<=1 ", "item_id asc");//自出商品
            singleproduct = orderdeliver.Select("dtype <>2 and combined_mode<=1  ", "item_id asc");//單一商品
            if (singleproduct.Count() > 0)
            {
                orderdelivers.Add(singleproduct);
            }
            fatherproduct = orderdeliver.Select(" combined_mode>1 and item_mode=1", "item_id asc");//組合商品中父商品是否存在
            foreach (var item in fatherproduct)
            {
                combine.Add(item);
                sonproduct = orderdeliver.Select(" combined_mode>1 and item_mode<>1 and parent_id=" + item["parent_id"] + " and pack_id=" + item["pack_id"], "item_id asc");//對應組合商品中的子商品
                foreach (var son in sonproduct)
                {
                    son["buy_num"] = (int.Parse(son["buy_num"].ToString()) * int.Parse(son["parent_num"].ToString())).ToString();
                    combine.Add(son);
                }
            }
            if (combine.Count > 0)
            {
                orderdelivers.Add((DataRow[])combine.ToArray(typeof(DataRow)));
            }
            //區分常溫、冷凍、冷藏
            foreach (var item in orderdelivers)
            {
                foreach (var row in item)
                {
                    string s = row["product_freight_set"].ToString();
                    switch (row["product_freight_set"].ToString())
                    {
                        case "1":
                        case "3":
                            normal.Add(row);//常溫
                            break;
                        case "2":
                        case "4":
                            low.Add(row);//冷凍
                            break;
                        case "5":
                        case "6":
                            lowstore.Add(row);//冷藏
                            break;
                        default:
                            break;
                    }
                }
            }

            orderdelivers = new List<DataRow[]>();
            if (normal.Count > 0)
            {
                orderdelivers.Add((DataRow[])normal.ToArray(typeof(DataRow)));
            }
            if (low.Count > 0)
            {
                orderdelivers.Add((DataRow[])low.ToArray(typeof(DataRow)));
            }
            if (lowstore.Count > 0)
            {
                orderdelivers.Add((DataRow[])lowstore.ToArray(typeof(DataRow)));
            }
            if (sinceorder.Count() > 0)
            {
                orderdelivers.Add(sinceorder);
            }
            int j = 0;
            foreach (var item in orderdelivers)
            {
                j++;
                for (int i = 0; i < item.Count(); i++)
                {
                    if (item[i]["ddeliver_id"].ToString() != lastdeliverid || i == 0)
                    {
                        lastdeliverid = item[i]["ddeliver_id"].ToString();//以一個出貨單號為界限
                        if (lastdeliverid != "0" || i == 0)
                        {
                            td = new PdfPCell();
                            td.Colspan = 7;
                            td.DisableBorderSide(2);
                            td.DisableBorderSide(4);
                            td.DisableBorderSide(8);
                            //td.BorderWidthTop = 0.2f;
                            ptable.AddCell(td);
                        }
                    }
                    string item_id = string.Empty;
                    if (item[i]["item_mode"].ToString() == "1")
                    {
                        item_id = item[i]["parent_id"].ToString();
                    }
                    else
                    {
                        item_id = item[i]["item_id"].ToString();
                    }
                    font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    td = new PdfPCell(new Phrase(item_id, font));
                    td.DisableBorderSide(1);
                    td.DisableBorderSide(2);
                    td.DisableBorderSide(8);
                    //td.BorderWidthLeft = 0.2f;
                    ptable.AddCell(td);
                    string datacontent = ((item[i]["product_mode"].ToString() == "2" && item[i]["item_mode"].ToString() != "1") ? "*" : " ") + item[i]["brand_name"].ToString() + "-" + item[i]["product_name"].ToString() + item[i]["product_spec_name"].ToString();
                    if (item[i]["combined_mode"].ToString() != "0" && item[i]["item_mode"].ToString() == "2")
                    {
                        datacontent = "  " + datacontent;
                    }
                    td = new PdfPCell(new Phrase(datacontent, font));
                    td.DisableBorderSide(1);
                    td.DisableBorderSide(2);
                    td.DisableBorderSide(8);
                    ptable.AddCell(td);
                    string value = string.Empty;
                    string freight_set = string.Empty;
                    if (dicproduct_freight_set.TryGetValue(item[i]["product_freight_set"].ToString(), out value))
                    {

                    }
                    switch (value)
                    {
                        case "1":
                            freight_set = "常溫";
                            break;
                        case "2":
                            freight_set = "冷凍";
                            break;
                        case "5":
                            freight_set = "冷藏";
                            break;
                    }
                    td = new PdfPCell(new Phrase(freight_set, font));
                    td.DisableBorderSide(1);
                    td.DisableBorderSide(2);
                    td.DisableBorderSide(8);
                    //td.BorderWidthLeft = 0.2f;
                    ptable.AddCell(td);
                    font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    td = new PdfPCell(new Phrase(item[i]["item_mode"].ToString() != "1" ? item[i]["buy_num"].ToString() : "", font));
                    td.DisableBorderSide(1);
                    td.DisableBorderSide(2);
                    td.DisableBorderSide(8);
                    // td.BorderWidthLeft = 0.2f;
                    ptable.AddCell(td);
                    td = new PdfPCell();
                    td.DisableBorderSide(1);
                    td.DisableBorderSide(2);
                    td.DisableBorderSide(8);
                    //td.BorderWidthLeft = 0.2f;
                    ptable.AddCell(td);
                    td = new PdfPCell();
                    td.DisableBorderSide(1);
                    td.DisableBorderSide(2);
                    td.DisableBorderSide(8);
                    //td.BorderWidthLeft = 0.2f;
                    ptable.AddCell(td);

                    Image image = Image.GetInstance(Server.MapPath("../Content/img/icons/mark.png"));
                    image.ScalePercent(5, 5);
                    if (item[i]["dtype"].ToString() == "2")
                    {
                        td = new PdfPCell(image, false);
                    }
                    else
                    {
                        td = new PdfPCell();
                    }
                    td.HorizontalAlignment = Element.ALIGN_CENTER;
                    td.VerticalAlignment = Element.ALIGN_MIDDLE;
                    td.DisableBorderSide(1);
                    td.DisableBorderSide(2);
                    ptable.AddCell(td);
                }
            }
            string note_order = orderdeliver.Rows.Count.ToString() != "0" ? orderdeliver.Rows[0]["note_order"].ToString().Trim() : "";
            cell = new PdfPCell(new Phrase(new Chunk("備註:" + note_order, font)));
            cell.Colspan = 7;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            ptable.AddCell(cell);
            PdfPTable nulltable = new PdfPTable(2);
            nulltable.SetWidths(new int[] { 20, 20 });
            nulltable.DefaultCell.DisableBorderSide(1);
            nulltable.DefaultCell.DisableBorderSide(2);
            nulltable.DefaultCell.DisableBorderSide(4);
            nulltable.DefaultCell.DisableBorderSide(8);
            nulltable.AddCell("");
            nulltable.AddCell("");
            nulltable.SpacingAfter = 250;
            document.Add(nulltable);
            ptable.SpacingAfter = 50;
            document.Add(ptable);

            font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
            document.Add(new Phrase("吉甲地市集網路平台購物發票說明:\n", font));
            font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
            document.Add(new Phrase("若您訂購時未選擇開立三聯式發票，平台一律開立電子發票。\n", font));
            document.Add(new Phrase("發票將於該筆訂單商品完全出貨之後第10天開立並以E-Mail通知您。\n", font));
            document.Add(new Phrase("如需紙本發票請來信客服中心，會計部門將會依需求將電子發票印出並以平信郵寄約2~7個工作天內送達。\n", font));
            document.Add(new Phrase("託管發票將會在單月26日進行對獎作業後，系統將會發信通知中獎發票持有人，\n", font));
            document.Add(new Phrase("且為保障您的權益，我們將在七個工作天內，以掛號方式把中獎發票寄給您。\n", font));
            font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑
            document.Add(new Phrase("祝您購物愉快!", font));
            document.Close();
            writer.Resume();

            Response.Clear();
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename);
            Response.WriteFile(newPDFName);
        }
        /// <summary>
        /// 出貨明細
        /// </summary>
        public void GetDeliverDetailsPDF()
        {
            string deliver_id = Request.Params["deliver_id"];
            _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
            DataTable deliverdetail = _DeliverDetailMgr.GetOrderDelivers(deliver_id, 1);
            BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfChinese, 12, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
            string filename = "deliver_details_D" + deliver_id.PadLeft(8, '0') + ".pdf";
            Document document = new Document(PageSize.A4, (float)5, (float)5, (float)20, (float)0.5);
            string newPDFName = Server.MapPath(excelPath) + filename;
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            cb.BeginText();
            DataRow[] singleproduct = new DataRow[] { };//單一商品
            DataRow[] fatherproduct = new DataRow[] { };//組合商品中的父商品
            DataRow[] sonproduct = new DataRow[] { };//組合商品中的子商品
            //DataRow[] normal;
            //DataRow[] low;
            //DataRow[] lowstore;
            //List<DataRow> deliverdetails = new List<DataRow>();
            ArrayList combine = new ArrayList();

            singleproduct = deliverdetail.Select(" combined_mode<=1  and ddeliver_id=" + deliver_id, "item_id asc");//單一商品
            fatherproduct = deliverdetail.Select(" combined_mode>1 and item_mode=1 and ddeliver_id=" + deliver_id, "item_id asc");//組合商品中父商品是否存在
            foreach (var item in fatherproduct)
            {
                combine.Add(item);
                sonproduct = deliverdetail.Select(" combined_mode>1  and item_mode<>1 and parent_id=" + item["parent_id"] + " and pack_id=" + item["pack_id"], "item_id asc");//對應組合商品中的子商品
                foreach (var son in sonproduct)
                {
                    son["buy_num"] = (int.Parse(son["buy_num"].ToString()) * int.Parse(son["parent_num"].ToString())).ToString();
                    combine.Add(son);
                }

            }

            List<DataRow[]> deliverdetails = new List<DataRow[]>();
            //normal = deliverdetail.Select("product_freight_set in(1,3)  and ddeliver_id=" + deliver_id, "item_id asc");//常溫
            //low = deliverdetail.Select("product_freight_set in(2,4)  and ddeliver_id=" + deliver_id, "item_id asc");//冷凍
            //lowstore = deliverdetail.Select("product_freight_set in(5,6)  and ddeliver_id=" + deliver_id, "item_id asc");//冷藏
            if (singleproduct.Count() > 0)
            {
                deliverdetails.Add(singleproduct);
            }
            if (combine.Count > 0)
            {
                deliverdetails.Add((DataRow[])combine.ToArray(typeof(DataRow)));
            }
            if (deliverdetail.Rows.Count > 0)
            {

                cb.SetFontAndSize(bfChinese, 20);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "吉甲地台灣好市集出貨明細", 30, 750, 0);
                string freight_set = string.Empty;

                switch (deliverdetail.Rows[0]["freight_set"].ToString().Trim())
                {
                    case "1":
                        freight_set = "常溫";
                        break;
                    case "2":
                        freight_set = "冷凍";
                        break;
                    case "5":
                        freight_set = "冷藏";
                        break;
                }
                cb.SetFontAndSize(bfChinese, 12);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, freight_set, 350, 780, 0);
                if (deliverdetail.Rows[0]["estimated_arrival_period"].ToString() != "0")
                {
                    string estimated_arrival_period = string.Empty;
                    switch (deliverdetail.Rows[0]["estimated_arrival_period"].ToString().Trim())
                    {
                        case "0":
                            estimated_arrival_period = "不限時";
                            break;
                        case "1":
                            estimated_arrival_period = "12:00以前";
                            break;
                        case "2":
                            estimated_arrival_period = "12:00-17:00";
                            break;
                        case "3":
                            estimated_arrival_period = "17:00-20:00";
                            break;
                    }
                    cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, estimated_arrival_period, 350, 765, 0);
                }

                Phrase ph = new Phrase();
                BarCode.Code128 _Code = new BarCode.Code128();
                _Code.ValueFont = new System.Drawing.Font("宋体", 20);
                System.Drawing.Bitmap imgTemp1 = _Code.GetCodeImage("D" + deliverdetail.Rows[0]["deliver_id"].ToString().PadLeft(8, '0'), BarCode.Code128.Encode.Code128A);
                imgTemp1.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                iTextSharp.text.Image IMG1 = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                IMG1.ScaleToFit(200, 30);
                Chunk ck = new Chunk(IMG1, 345, -100); //图片可设置 偏移
                ph.Add(ck);
                document.Add(ph);
                cb.SetFontAndSize(bfChinese, 10);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "付款單號：", 10, 680, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "訂購時間：", 10, 660, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "转单日期：", 200, 660, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "订购人：", 10, 640, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "收货人：", 200, 640, 0);
                string address = string.Empty;


                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliverdetail.Rows[0]["order_id"].ToString(), 80, 680, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliverdetail.Rows[0]["order_createdate"].ToString() != "0" ? CommonFunction.GetNetTime(long.Parse(deliverdetail.Rows[0]["order_createdate"].ToString())).ToString("yyyy-MM-dd HH:mm:ss") : "", 80, 660, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliverdetail.Rows[0]["money_collect_date"].ToString() != "0" ? CommonFunction.GetNetTime(long.Parse(deliverdetail.Rows[0]["money_collect_date"].ToString())).ToString("yyyy-MM-dd HH:mm:ss") : "", 250, 660, 0);
                //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliverdetail.Rows[0]["holiday_deliver"].ToString() == "1" ? "可" : "不可", 250, 620, 0);
                //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliverdetail.Rows[0]["note_order"].ToString(), 80, 600, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliverdetail.Rows[0]["order_name"].ToString(), 80, 640, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, deliverdetail.Rows[0]["delivery_name"].ToString(), 250, 640, 0);
                if (deliverdetail.Rows[0]["type"].ToString() != "3")
                {
                    PdfPTable ptable = new PdfPTable(4);
                    ptable.SetTotalWidth(new float[] { 100, 320, 70, 70 });
                    PdfPCell cell = new PdfPCell();
                    cell.BorderWidth = 0.1f;
                    cell.BorderColor = new BaseColor(0, 0, 0);
                    cell = new PdfPCell(new Phrase("商品編號", font));
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("商品名稱", font));
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("數量", font));
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("售价", font));
                    ptable.AddCell(cell);
                    PdfPCell td = new PdfPCell();
                    td.BorderWidth = 0.1f;
                    int j = 0;
                    foreach (var item in deliverdetails)
                    {
                        j++;
                        for (int i = 0; i < item.Count(); i++)
                        {
                            string item_id = string.Empty;
                            if (item[i]["item_mode"].ToString() == "1")
                            {
                                item_id = item[i]["parent_id"].ToString();
                            }
                            else
                            {
                                item_id = item[i]["item_id"].ToString();
                            }
                            cell = new PdfPCell(new Phrase(item_id, font));
                            ptable.AddCell(cell);
                            string datacontent = ((item[i]["item_mode"].ToString() == "2") ? " *" : "") + item[i]["product_name"].ToString() + item[i]["product_spec_name"].ToString();
                            cell = new PdfPCell(new Phrase(item[i]["brand_name"].ToString() + "-" + datacontent, font));
                            ptable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(item[i]["buy_num"].ToString(), font));
                            ptable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(item[i]["item_mode"].ToString() == "1" ? "" : item[i]["single_money"].ToString(), font));
                            ptable.AddCell(cell);
                        }
                        if (deliverdetails.Count > 1 && j != deliverdetails.Count)
                        {
                            td = new PdfPCell();
                            td.Colspan = 4;
                            td.BorderWidthTop = 0.2f;
                            td.DisableBorderSide(2);
                            ptable.AddCell(td);
                        }
                    }
                    ptable.WriteSelectedRows(0, -1, 10, 620, writer.DirectContent);
                }
                else
                {
                    PdfPTable ptable = new PdfPTable(4);
                    // ptable.WidthPercentage = 90;
                    // ptable.TotalWidth = ptable.WidthPercentage;
                    ptable.SetTotalWidth(new float[] { 100, 350, 70, 70 });
                    PdfPCell cell = new PdfPCell();
                    cell.BorderWidth = 0.1f;
                    cell = new PdfPCell(new Phrase("產品細項編號", font));
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("產品名稱", font));
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("規格", font));
                    ptable.AddCell(cell);
                    cell = new PdfPCell(new Phrase("數量", font));
                    ptable.AddCell(cell);
                    int j = 0;
                    foreach (var item in deliverdetails)
                    {
                        j++;
                        for (int i = 0; i < item.Count(); i++)
                        {
                            cell = new PdfPCell(new Phrase(item[i]["item_id"].ToString(), font));
                            ptable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(item[i]["brand_name"].ToString() + "-" + item[i]["product_name"].ToString(), font));
                            ptable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(item[i]["product_spec_name"].ToString(), font));
                            ptable.AddCell(cell);
                            cell = new PdfPCell(new Phrase(item[i]["buy_num"].ToString(), font));
                            ptable.AddCell(cell);
                        }
                        if (deliverdetails.Count > 1 && j != deliverdetails.Count)
                        {
                            cell = new PdfPCell();
                            cell.Colspan = 4;
                            cell.BorderWidthTop = 0.2f;
                            cell.DisableBorderSide(2);
                            ptable.AddCell(cell);
                        }
                    }
                    ptable.WriteSelectedRows(0, -1, 10, 620, writer.DirectContent);

                }

                cb.EndText();
                document.Close();
                writer.Resume();

                Response.Clear();
                Response.Charset = "gb2312";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                // Response.AddHeader("Content-Disposition", "attach-ment;filename=" + System.Web.HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8) + ".pdf ");
                Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename);
                Response.WriteFile(newPDFName);
            }

        }
        /// <summary>
        /// 貨運單  delivery_store=42 即 到店取貨 才須匯出貨運單
        /// </summary>
        public void GetShopbillsPDF()
        {
            string deliver_id = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))
            {
                deliver_id = Request.Params["deliver_id"];
            }
            string ticket_id = string.Empty;
            string fticket_id = string.Empty; ;
            if (!string.IsNullOrEmpty(Request.Params["ticket_id"]))
            {
                ticket_id = Request.Params["ticket_id"];
                ticket_id = ticket_id.Remove(ticket_id.LastIndexOf(','));
                string[] ticket_ids = ticket_id.Split(',');
                if (ticket_ids.Length > 0)
                {
                    fticket_id = ticket_ids[0];
                }
                else
                {
                    fticket_id = ticket_id;
                }

            }

            int i = 0;//用來計算表格數量，來分頁
            _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
            DataTable bills = _DeliverDetailMgr.GetWayBills(deliver_id, ticket_id);
            BaseFont bf = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bf, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
            string filename = string.Empty;
            if (string.IsNullOrEmpty(ticket_id))
            {
                filename = "shopbills_T" + deliver_id.PadLeft(8, '0') + ".pdf";
            }
            else
            {
                filename = "shopbills_T" + fticket_id.PadLeft(8, '0') + ".pdf";
            }
            Document document = new Document(PageSize.A4, (float)5, (float)5, (float)20, (float)0.5);
            string newPDFName = Server.MapPath(excelPath) + filename;
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
            document.Open();
            #region 條形碼
            foreach (DataRow dr in bills.Rows)
            {
                #region 條形碼要顯示的內容
                string code1 = string.Empty;
                string csvuser = "228";
                string deliver_stno = dr["deliver_stno"].ToString();
                string delivercode = "0".PadLeft(11 - dr["deliver_id"].ToString().Length, '0');
                if (deliver_stno.Length >= 1)
                {
                    switch (deliver_stno.Substring(0, 1))
                    {
                        case "F":
                            code1 = "1" + csvuser + "00";
                            break;
                        case "K":
                            code1 = "3" + csvuser + "00";
                            break;
                        case "L":
                            code1 = "2" + csvuser + "00";
                            break;
                        default:
                            break;
                    }
                }
                code1 += delivercode + dr["deliver_id"].ToString();
                int checkcode = 0;
                for (int j = 0; j < code1.Length; j++)
                {
                    checkcode += int.Parse(code1.Substring(j, 1));
                }
                checkcode = checkcode % 43;
                string[] checkcodemap = {"0","1","2","3","4","5","6","7","8","9","A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S",
			     "T","U","V","W","X","Y","Z","-",".","SP","$","/","+","%" };
                code1 += checkcodemap.GetValue(checkcode).ToString();

                string code2 = string.Empty;
                code2 = csvuser + delivercode.Substring(0, 3) + "963";
                string code3 = string.Empty;
                string nreceivable = string.Empty;
                string ndeliver_id = string.Empty;
                ndeliver_id = (delivercode + dr["deliver_id"].ToString()).Substring(3, 8);
                nreceivable = "0".PadLeft(5 - dr["receivable"].ToString().Length, '0') + dr["receivable"].ToString();
                if (dr["order_payment"].ToString() == "20")
                {
                    code3 = ndeliver_id + "1" + nreceivable;
                }
                else
                {
                    code3 = ndeliver_id + "3" + nreceivable;
                }
                int basenum = 0;//奇數
                int evennum = 0;//偶數
                for (int k = 0; k < code2.Length; k++)
                {
                    if ((k + 1) % 2 == 1)
                    {
                        basenum += int.Parse(code2.Substring(k, 1));
                    }
                    else
                    {
                        evennum += int.Parse(code2.Substring(k, 1));
                    }
                }
                for (int l = 0; l < code3.Length; l++)
                {
                    if ((l + 1) % 2 == 1)
                    {
                        basenum += int.Parse(code3.Substring(l, 1));
                    }
                    else
                    {
                        evennum += int.Parse(code3.Substring(l, 1));
                    }
                }
                basenum = basenum % 11;
                evennum = evennum % 11;
                if (basenum == 10)
                {
                    basenum = 1;
                }
                if (evennum == 10)
                {
                    evennum = 9;
                }
                else if (evennum == 0)
                {
                    evennum = 8;
                }
                code3 += basenum.ToString() + evennum.ToString();
                #endregion
                BarCode.Code128 _Code = new BarCode.Code128();
                _Code.ValueFont = new System.Drawing.Font("宋体", 20);
                System.Drawing.Bitmap imgTemp1 = _Code.GetCodeImage((!string.IsNullOrEmpty(dr["dcrono"].ToString())) ? dr["dcrono"].ToString() : "0", BarCode.Code128.Encode.Code128A);
                imgTemp1.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                iTextSharp.text.Image IMG1 = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                IMG1.ScaleToFit(200, 30);

                Chunk ck1 = new Chunk(IMG1, 0, 0); //图片可设置 偏移


                _Code = new BarCode.Code128();
                //如果條形碼顯示的內容過長的話，一定要注意設置的字體大小，字體過大，內容不會顯示
                _Code.ValueFont = new System.Drawing.Font("宋体", 18);
                System.Drawing.Bitmap imgTemp2 = _Code.GetCodeImage(code1, BarCode.Code128.Encode.Code128A);
                imgTemp2.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                iTextSharp.text.Image IMG2 = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                IMG2.ScaleToFit(200, 30);
                Chunk ck2 = new Chunk(IMG2, 0, 0); //图片可设置 偏移

                _Code = new BarCode.Code128();
                _Code.ValueFont = new System.Drawing.Font("宋体", 20);
                System.Drawing.Bitmap imgTemp3 = _Code.GetCodeImage(code2, BarCode.Code128.Encode.Code128A);
                imgTemp3.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                iTextSharp.text.Image IMG3 = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                IMG3.ScaleToFit(200, 30);
                Chunk ck3 = new Chunk(IMG3, 0, 0); //图片可设置 偏移

                _Code = new BarCode.Code128();
                _Code.ValueFont = new System.Drawing.Font("宋体", 18);
                System.Drawing.Bitmap imgTemp4 = _Code.GetCodeImage(code3, BarCode.Code128.Encode.Code128A);
                imgTemp4.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                iTextSharp.text.Image IMG4 = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                IMG4.ScaleToFit(200, 30);
                Chunk ck4 = new Chunk(IMG4, 0, 0); //图片可设置 偏移


            #endregion
                if (i % 3 == 0 && i != 0)
                {
                    document.NewPage();
                }
                string stnm_1 = string.Empty;
                string stnm_2 = string.Empty;
                string stnm = dr["stnm"].ToString();
                if (dr["deliver_stno"].ToString().Length >= 1)
                {
                    switch (dr["deliver_stno"].ToString().Substring(0, 1))
                    {
                        case "F":
                            if (stnm.Length * 3 >= 6)
                            {
                                stnm_1 = stnm.Substring(0, 6 / 3);
                            }
                            else
                            {
                                stnm_1 = stnm.Substring(0, stnm.Length);
                            }
                            if (stnm.Length * 3 >= 40)
                            {
                                stnm_2 = stnm.Substring(6 / 3, 40 / 3);
                            }
                            else
                            {
                                if (stnm.Length * 3 >= 6)
                                {
                                    stnm_2 = stnm.Substring(6 / 3, stnm.Length - 6 / 3);
                                }
                            }
                            break;
                        case "K":
                            if (stnm.Length * 3 >= 4)
                            {
                                stnm_1 = stnm.Substring(0, 4 / 3);
                            }
                            else
                            {
                                stnm_1 = stnm.Substring(0, stnm.Length);
                            }
                            if (stnm.Length * 3 >= 40)
                            {
                                stnm_2 = stnm.Substring(2 / 3, 40 / 3);
                            }
                            else
                            {
                                if (stnm.Length * 3 > 2)
                                {
                                    stnm_2 = stnm.Substring(2 / 3, stnm.Length - 2 / 3);
                                }
                            }
                            break;
                        case "L":
                            if (stnm.Length * 3 >= 10)
                            {
                                stnm_1 = stnm.Substring(0, 10 / 3);
                            }
                            else
                            {
                                stnm_1 = stnm.Substring(0, stnm.Length);
                            }
                            if (stnm.Length * 3 >= 40)
                            {
                                stnm_2 = stnm.Substring(9 / 3, 40 / 3);
                            }
                            else
                            {
                                if (stnm.Length * 3 >= 9)
                                {
                                    stnm_2 = stnm.Substring(9 / 3, stnm.Length - 9 / 3);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                PdfPTable totaltable = new PdfPTable(3);
                totaltable.WidthPercentage = 100;
                totaltable.SetWidths(new int[] { 40, 2, 58 });
                PdfPTable table = new PdfPTable(3);
                #region 左邊框
                table.SetWidths(new int[] { 25, 15, 60 });
                table.DefaultCell.DisableBorderSide(1);
                table.DefaultCell.DisableBorderSide(2);
                table.DefaultCell.DisableBorderSide(4);
                table.DefaultCell.DisableBorderSide(8);
                PdfPCell cell;
                font = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                cell = new PdfPCell(new PdfPCell(new Phrase(dr["dcrono"].ToString(), font)));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("   提貨人:" + dr["delivery_name"].ToString(), font));
                cell.Colspan = 2;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(stnm_1 + "\n" + stnm_2, font));
                cell.Colspan = 2;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(ck1));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(ck2));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                cell.Colspan = 3;
                table.AddCell(cell);
                font = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                cell = new PdfPCell(new Phrase("廠商出貨編號：" + dr["deliver_id"].ToString(), font));
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("金額：" + dr["receivable"].ToString() + "元", font));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("吉甲地在地好物 www.gigade100.com", font));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("客服專線：(02) 2783-4997", font));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("若需退貨請消費者聯繫上述電子商務網站\n\n\n\n\n\n", font));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                font = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                cell = new PdfPCell(new PdfPCell(new Phrase("D10", font)));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Colspan = 2;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                #endregion
                totaltable.AddCell(table);
                cell = new PdfPCell();
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(8);
                totaltable.AddCell(cell);
                #region 右邊
                table = new PdfPTable(6);
                table.SetWidths(new int[] { 20, 20, 37, 10, 1, 12 });
                table.DefaultCell.DisableBorderSide(1);
                table.DefaultCell.DisableBorderSide(2);
                table.DefaultCell.DisableBorderSide(4);
                table.DefaultCell.DisableBorderSide(8);
                table.DefaultCell.UseAscender = true;
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                //table.AddCell(new Phrase(dr["order_payment"].ToString() == "20" ? "取貨付款" : "取貨不付款", font));
                cell = new PdfPCell(new PdfPCell(new Phrase(dr["order_payment"].ToString() == "20" ? " 取貨付款" : " 取貨不付款", font)));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 3;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new PdfPCell(new Phrase("D10", font)));
                //cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase());
                if (dr["order_payment"].ToString() == "20")
                {
                    cell.Rowspan = 6;
                }
                else
                {
                    cell.Rowspan = 7;
                }
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                string delivery_name = string.Empty;
                for (int k = 0; k < dr["delivery_name"].ToString().Length; k++)
                {
                    delivery_name += dr["delivery_name"].ToString()[k] + "\n";
                }
                cell = new PdfPCell(new Phrase("吉\n甲\n地\n\n" + delivery_name, font));
                if (dr["order_payment"].ToString() == "20")
                {
                    cell.Rowspan = 6;
                }
                else
                {
                    cell.Rowspan = 7;
                }
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                //cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(ck3));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                cell.Colspan = 5;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(ck4));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                cell.Colspan = 5;
                table.AddCell(cell);

                if (dr["order_payment"].ToString() == "20")
                {
                    font = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    Phrase p = new Phrase("＊應付金額：" + dr["receivable"].ToString() + "元＊\n\n", font);
                    font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    p.Add(new Phrase("消費者簽名：____________________\n\n\n\n\n", font));
                    cell = new PdfPCell(p);
                    cell.Colspan = 5;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    table.AddCell(cell);

                }
                else
                {
                    Phrase p = new Phrase();
                    font = new iTextSharp.text.Font(bf, 14, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    p.Add(new Phrase(" ＊憑身分證件正本領貨＊\n\n", font));
                    font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                    p.Add(new Phrase(" 消費者簽名：____________________\n\n", font));
                    p.Add(new Phrase(" 消費者身分證末四碼\n", font));
                    cell = new PdfPCell(p);
                    cell.Colspan = 5;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    table.AddCell(cell);

                    PdfPTable stable = new PdfPTable(4);
                    stable.SetTotalWidth(new float[] { 20, 20, 20, 20 });
                    stable.DefaultCell.FixedHeight = 10;
                    stable.AddCell(new Phrase());
                    stable.AddCell(new Phrase());
                    stable.AddCell(new Phrase());
                    stable.AddCell(new Phrase());
                    //p.Add(new Phrase("請核對證件\n並  簽  名", font));
                    cell = new PdfPCell(stable);
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    table.AddCell(cell);
                    cell = new PdfPCell(new Phrase("請核對證件\n並  簽  名\n", font));
                    //cell.AddElement(new Phrase("請核對證件\n並  簽  名", font));
                    cell.Colspan = 4;
                    cell.DisableBorderSide(1);
                    cell.DisableBorderSide(2);
                    cell.DisableBorderSide(4);
                    cell.DisableBorderSide(8);
                    cell.UseAscender = true;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);

                }
                font = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑
                cell = new PdfPCell(new Phrase("\n門市結帳人員簽名：____________________", font));
                cell.Colspan = 5;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                table.AddCell(cell);
                string estimated_delivery_date = (!string.IsNullOrEmpty(dr["estimated_delivery_date"].ToString())) ? DateTime.Parse(dr["estimated_delivery_date"].ToString()).ToString("yyyy/MM/dd") : "";

                cell = new PdfPCell(new PdfPCell(new Phrase(dr["deliver_stno"].ToString(), font)));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(" " + estimated_delivery_date, font));
                cell.Colspan = 2;
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(2);
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(8);
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell);
                cell = new PdfPCell(new PdfPCell(new Phrase(dr["dcrono"].ToString(), font)));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);
                table.AddCell("");
                //cell = new PdfPCell(new Phrase("11111111111", font));
                //cell.Colspan = 5;
                //table.AddCell(cell);

                totaltable.AddCell(table);
                totaltable.SpacingAfter = 25f;
                #endregion
                document.Add(totaltable);
            }


            document.Close();

            Response.Clear();
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            // Response.AddHeader("Content-Disposition", "attach-ment;filename=" + System.Web.HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8) + ".pdf ");
            Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename);
            Response.WriteFile(newPDFName);


        }
        /// <summary>
        /// 貨運單  delivery_store=17 即 吉甲地車隊貨到付款 才須匯出貨運單
        /// </summary>
        public void GetCarWaybillsPDF()
        {
            _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
            string deliver_id = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))
            {
                deliver_id = Request.Params["deliver_id"];
            }
            string ticketid = string.Empty;
            if (!string.IsNullOrEmpty(Request.Params["ticket_id"]))
            {
                ticketid = Request.Params["ticket_id"];
                ticketid = ticketid.Remove(ticketid.LastIndexOf(','));
            }
            DataTable bills = _DeliverDetailMgr.GetWayBills(deliver_id, ticketid);
            string estimated_delivery_date = string.Empty;
            string sestimated_arrival_period = string.Empty;
            string estimated_arrival_period = string.Empty;
            string infor = string.Empty;
            string receivable = string.Empty;
            string order_id = string.Empty;
            int i = 0;
            Document document = new Document(PageSize.A4, (float)5, (float)5, (float)5, (float)0.5);
            int iTemp = 0;
            string filename = string.Empty;
            if (int.TryParse(ticketid, out iTemp))
            {
                filename = "carwaybills_T" + ticketid.PadLeft(8, '0') + ".pdf";
            }
            else
            {
                filename = "carwaybills_T" + ticketid.Substring(0, ticketid.IndexOf(',')).ToString().PadLeft(8, '0') + ".pdf";
            }

            string newPDFName = Server.MapPath(excelPath) + filename;
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
            document.Open();
            BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  

            foreach (DataRow dr in bills.Rows)
            {
                estimated_delivery_date = !string.IsNullOrEmpty(dr["estimated_delivery_date"].ToString()) ? DateTime.Parse(dr["estimated_delivery_date"].ToString()).ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");
                estimated_arrival_period = dr["estimated_arrival_period"].ToString();
                switch (dr["estimated_arrival_period"].ToString())
                {
                    case "0":
                        sestimated_arrival_period = "不限時";
                        break;
                    case "1":
                        sestimated_arrival_period = "12:00以前";
                        break;
                    case "2":
                        sestimated_arrival_period = "12:00-17:00";
                        break;
                    case "3":
                        sestimated_arrival_period = "17:00-20:00";
                        break;
                    default:
                        break;

                }
                //infor = dr["delivery_name"].ToString() + "\n\n" + CommonFunction.ZipAddress(dr["delivery_zip"].ToString()) + "\n" + dr["delivery_address"].ToString() + "\n\n\n" + dr["delivery_mobile"].ToString();
                receivable = dr["receivable"].ToString() != "0" ? dr["receivable"].ToString() : "不收款";
                order_id = dr["order_id"].ToString();
                font = new iTextSharp.text.Font(bfChinese, 16, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑 
                Chunk c1 = new Chunk(dr["delivery_name"].ToString() + "\n\n", font);
                font = new iTextSharp.text.Font(bfChinese, 8, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑 
                Chunk c2 = new Chunk(CommonFunction.ZipAddress(dr["delivery_zip"].ToString()) + "\n\n" + dr["delivery_address"].ToString() + "\n\n\n", font);
                font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑 
                Chunk c3 = new Chunk(dr["delivery_mobile"].ToString(), font);
                Phrase pinfor = new Phrase();
                pinfor.Add(c1);
                pinfor.Add(c2);
                pinfor.Add(c3);
                BarCode.Code128 _Code = new BarCode.Code128();
                _Code.ValueFont = new System.Drawing.Font("宋体", 20);
                System.Drawing.Bitmap imgTemp = _Code.GetCodeImage(order_id, BarCode.Code128.Encode.Code128A);
                imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                iTextSharp.text.Image IMG = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                IMG.ScaleToFit(200, 40);
                Chunk orderidck = new Chunk(IMG, 0, 0); //图片可设置 偏移


                imgTemp = _Code.GetCodeImage("D" + dr["deliver_id"].ToString().PadLeft(8, '0'), BarCode.Code128.Encode.Code128A);
                imgTemp.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
                IMG = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
                IMG.ScaleToFit(200, 40);
                Chunk deliveridck = new Chunk(IMG, 0, 0); //图片可设置 偏移



                //PdfContentByte cb = writer.DirectContent;
                // cb.BeginText();
                if (i % 2 == 0 && i != 0)
                {
                    document.NewPage();
                }
                PdfPTable totaltable = new PdfPTable(3);
                totaltable.WidthPercentage = 100;
                totaltable.SetWidths(new int[] { 45, 2, 53 });
                totaltable.DefaultCell.DisableBorderSide(1);
                totaltable.DefaultCell.DisableBorderSide(2);
                totaltable.DefaultCell.DisableBorderSide(4);
                totaltable.DefaultCell.DisableBorderSide(8);
                PdfPCell cell;
                #region 左邊框
                PdfPTable table = new PdfPTable(4);
                table.SetTotalWidth(new float[] { 60, 75, 10, 100 });
                table.DefaultCell.UseAscender = true;
                table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell = new PdfPCell(new Phrase("出貨日", font));
                cell.FixedHeight = 17f;
                table.AddCell(cell);
                table.AddCell(new Phrase("預定配送日", font));

                cell = new PdfPCell(new Phrase("指定時段", font));
                cell.Colspan = 2;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(DateTime.Now.ToString("yyyyMMdd"), font));
                cell.FixedHeight = 17f;
                table.AddCell(cell);
                //table.AddCell(new Phrase(DateTime.Now.ToString("yyyyMMdd"), font));
                table.AddCell(new Phrase(estimated_delivery_date, font));
                table.AddCell(new Phrase(estimated_arrival_period, font));
                table.AddCell(new Phrase(sestimated_arrival_period, font));
                table.AddCell(new Phrase("收\n件\n人", font));
                cell = new PdfPCell(pinfor);
                cell.Colspan = 3;
                table.AddCell(cell);
                table.AddCell(new Phrase("寄件人", font));
                font = new iTextSharp.text.Font(bfChinese, 7, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                cell = new PdfPCell(new Phrase("台北市南港區八德路四段768巷5號4F之一 \n\n 吉甲地好市集股份有限公司", font));
                cell.UseAscender = true;
                cell.HorizontalAlignment = 3;
                cell.Colspan = 3;
                table.AddCell(cell);
                font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                table.AddCell(new Phrase("訂單編號", font));
                cell = new PdfPCell(new Phrase(orderidck));
                cell.UseAscender = true;
                cell.HorizontalAlignment = 1;
                cell.Colspan = 3;
                table.AddCell(cell);
                font = new iTextSharp.text.Font(bfChinese, 14, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                cell = new PdfPCell(new Phrase("吉\n甲\n地", font));
                cell.UseDescender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.DisableBorderSide(4);
                cell.DisableBorderSide(2);
                table.AddCell(cell);
                PdfPTable stable = new PdfPTable(2);
                stable.DefaultCell.UseAscender = true;
                stable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                stable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                stable.AddCell(new Phrase("代收貨款", font));
                stable.AddCell(new Phrase(receivable, font));
                stable.AddCell(new Phrase("出貨單號", font));
                cell = new PdfPCell(new Phrase(deliveridck));
                float h = cell.Height;
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                stable.AddCell(cell);
                cell = new PdfPCell(stable);
                cell.Colspan = 3;
                table.AddCell(cell);
                // table.WriteSelectedRows(0, -1, 10, 820, writer.DirectContent);
                #endregion
                #region
                totaltable.AddCell(table);
                totaltable.AddCell(" ");
                #endregion
                #region 右邊框
                font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                table = new PdfPTable(6);
                table.SetTotalWidth(new float[] { 60, 70, 40, 65, 40, 50 });
                table.DefaultCell.UseAscender = true;
                table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell = new PdfPCell();
                cell.DisableBorderSide(1);
                cell.DisableBorderSide(4);
                table.AddCell(cell);
                stable = new PdfPTable(1);
                stable.DefaultCell.UseAscender = true;
                stable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                stable.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                //stable.AddCell(new Phrase("包裹查詢號碼", font));
                cell = new PdfPCell(new Phrase("包裹查詢號碼", font));
                cell.FixedHeight = 17f;
                stable.AddCell(cell);
                //stable.AddCell(new Phrase("D" + dr["deliver_id"].ToString().PadLeft(8, '0'), font));
                cell = new PdfPCell(new Phrase("D" + dr["deliver_id"].ToString().PadLeft(8, '0'), font));
                cell.FixedHeight = 17f;
                stable.AddCell(cell);
                cell = new PdfPCell(stable);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(deliveridck));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table.AddCell(cell);
                table.AddCell(new Phrase("備註", font));
                string name = string.Empty;
                for (int k = 0; k < dr["delivery_name"].ToString().Length; k++)
                {
                    name += dr["delivery_name"].ToString()[k] + "\n";
                }
                font = new iTextSharp.text.Font(bfChinese, 14, iTextSharp.text.Font.BOLD, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                cell = new PdfPCell(new Phrase("吉\n甲\n地\n\n" + name, font));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Rowspan = 6;
                table.AddCell(cell);
                font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                table.AddCell(new Phrase("收\n件\n人", font));
                cell = new PdfPCell(pinfor);
                //cell.UseAscender = true;
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                table.AddCell(cell);
                cell = new PdfPCell();
                cell.Rowspan = 5;
                table.AddCell(cell);
                table.AddCell(new Phrase("寄件人", font));
                font = new iTextSharp.text.Font(bfChinese, 7, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                cell = new PdfPCell(new Phrase("台北市南港區八德路四段768巷5號4F之一 \n\n 吉甲地好市集股份有限公司", font));
                //cell.UseAscender = true;
                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Colspan = 3;
                table.AddCell(cell);
                font = new iTextSharp.text.Font(bfChinese, 10, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
                table.AddCell(new Phrase("訂單編號", font));
                cell = new PdfPCell(new Phrase(orderidck));
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 3;
                table.AddCell(cell);
                table.AddCell(new Phrase("指定時段", font));
                table.AddCell(new Phrase(sestimated_arrival_period, font));
                cell = new PdfPCell(new Phrase("\n\n收件人簽名\n\n", font));
                cell.Rowspan = 2;
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                // cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                PdfPCell ncell = new PdfPCell();
                //ncell.DisableBorderSide(1);
                //ncell.DisableBorderSide(2);
                ncell.Rowspan = 2;
                table.AddCell(ncell);
                //table.AddCell(new Phrase("收件人簽名", font));
                cell = new PdfPCell(new Phrase("代收貨款", font));
                cell.FixedHeight = h;
                cell.UseAscender = true;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell);
                table.AddCell(new Phrase(receivable, font));
                // table.WriteSelectedRows(0, -1, 280, 820, writer.DirectContent);
                //table.AddCell(ncell);
                //table.AddCell(ncell);
                //table.AddCell(ncell);
                //table.AddCell(ncell);
                #endregion
                totaltable.AddCell(table);
                // cb.EndText();
                totaltable.SpacingAfter = 75f;
                document.Add(totaltable);
                i++;
            }
            document.Close();


            Response.Clear();
            Response.Charset = "gb2312";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            // Response.AddHeader("Content-Disposition", "attach-ment;filename=" + System.Web.HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8) + ".pdf ");
            Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename);
            Response.WriteFile(newPDFName);

        }
        #endregion

        #endregion

        #region 外站出貨檔匯出
        /// <summary>
        /// 獲取賣場列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetChannel()
        {
            string json = String.Empty;
            List<Channel> store = new List<Channel>();

            try
            {
                Channel channel = new Channel();
                _channelMgr = new ChannelMgr(mySqlConnectionString);
                store = _channelMgr.QueryList(0);
                channel.channel_id = 0;
                channel.channel_name_simple = "全部";
                store.Insert(0, channel);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        public HttpResponseBase GetChannelOrderList()
        {
            string json = String.Empty;
            List<DeliverMasterQuery> store = new List<DeliverMasterQuery>();

            try
            {
                DeliverMasterQuery query = new DeliverMasterQuery();
                _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                int totalCount = 0;
                string condition = Request.Params["condition"];
                string content = Request.Params["content"];
                string deliverystore = Request.Params["delivery_store"];
                string channel = Request.Params["channel"];
                string ddstatus = Request.Params["delivery_status"];
                string retrieve_mode = Request.Params["retrieve_mode"];
                string datequery = Request.Params["datequery"];
                string starttime = Request.Params["time_start"];
                string endtime = Request.Params["time_end"];
                if (condition != "0")
                {
                    if (!string.IsNullOrEmpty(content))
                    {
                        switch (condition)
                        {
                            case "1":
                                query.od_order_id = uint.Parse(content);
                                break;
                            case "2":
                                query.channel_order_id = content;
                                break;
                            case "3":
                                query.sub_order_id = content;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (deliverystore != "0" && !string.IsNullOrEmpty(deliverystore))
                {
                    query.delivery_store = uint.Parse(deliverystore);
                }
                if (channel != "0")
                {
                    query.channel = int.Parse(channel);
                }
                if (ddstatus != "-1")
                {
                    query.dd_status = int.Parse(ddstatus);
                }
                if (retrieve_mode != "-1" && !string.IsNullOrEmpty(retrieve_mode))
                {
                    query.retrieve_mode = int.Parse(retrieve_mode);
                }
                if (datequery != "0")
                {
                    if (!string.IsNullOrEmpty(starttime))
                    {
                        starttime = DateTime.Parse(starttime).ToString("yyyy-MM-dd HH:mm:ss");
                        query.sqlwhere = " and dm.delivery_date >='" + starttime + "'";
                    }
                    if (!string.IsNullOrEmpty(endtime))
                    {
                        endtime = DateTime.Parse(endtime).ToString("yyyy-MM-dd") + " 23:59:59";
                        query.sqlwhere += " and dm.delivery_date <='" + endtime + "'";
                    }
                }
                object ob = _DeliverDetailMgr.GetChannelOrderList(query, out totalCount);
                if (ob.GetType() == typeof(List<DeliverMasterQuery>))
                {
                    store = (List<DeliverMasterQuery>)ob;
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
                json = "{success:false,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        ///  匯出yahoo出貨當
        /// </summary>
        public void OutChannelOrderCSV()
        {
            DeliverMasterQuery query = new DeliverMasterQuery();
            _DeliverDetailMgr = new DeliverDetailMgr(mySqlConnectionString);
            try
            {
                int totalCount = 0;
                string condition = Request.Params["condition"];
                string content = Request.Params["content"];
                string deliverystore = Request.Params["delivery_store"];
                string channel = Request.Params["channel"];
                string ddstatus = Request.Params["delivery_status"];
                string retrieve_mode = Request.Params["retrieve_mode"];
                string datequery = Request.Params["datequery"];
                string starttime = Request.Params["time_start"];
                string endtime = Request.Params["time_end"];
                query.IsPage = false;
                if (condition != "0")
                {
                    if (!string.IsNullOrEmpty(content))
                    {
                        switch (condition)
                        {
                            case "1":
                                query.od_order_id = uint.Parse(content);
                                break;
                            case "2":
                                query.channel_order_id = content;
                                break;
                            case "3":
                                query.sub_order_id = content;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (deliverystore != "0" && !string.IsNullOrEmpty(deliverystore))
                {
                    query.delivery_store = uint.Parse(deliverystore);
                }
                if (channel != "0" && !string.IsNullOrEmpty(channel))
                {
                    query.channel = int.Parse(channel);
                }
                if (ddstatus != "-1")
                {
                    query.dd_status = int.Parse(ddstatus);
                }
                if (retrieve_mode != "-1" && !string.IsNullOrEmpty(retrieve_mode))
                {
                    query.retrieve_mode = int.Parse(retrieve_mode);
                }
                if (datequery != "0")
                {
                    if (!string.IsNullOrEmpty(starttime))
                    {
                        starttime = DateTime.Parse(starttime).ToString("yyyy-MM-dd HH:mm:ss");
                        query.sqlwhere = " and dm.delivery_date >='" + starttime + "'";
                    }
                    if (!string.IsNullOrEmpty(endtime))
                    {
                        endtime = DateTime.Parse(endtime).ToString("yyyy-MM-dd") + " 23:59:59";
                        query.sqlwhere += " and dm.delivery_date <='" + endtime + "'";
                    }
                }
                object ob = _DeliverDetailMgr.GetChannelOrderList(query, out totalCount, 1);
                DataTable channelorder = new DataTable();
                if (ob.GetType() == typeof(DataTable))
                {
                    channelorder = (DataTable)ob;
                }
                //DataTable newchannel = channelorder.DefaultView.ToTable(false, new string[] { "sub_order_id", "delivery_code" });
                // newchannel.Columns["delivery_store"].Expression = " IIF (delivery_store <> 11,11,11)";
                DataColumn dcol = new DataColumn("delivery_store", typeof(String));
                dcol.DefaultValue = 11;
                channelorder.Columns.Add(dcol);
                channelorder.Columns["delivery_store"].SetOrdinal(1);
                //dcol.Expression = " IIF (delivery_store <> 11,11,11)";
                DataColumn col = new DataColumn("ok", typeof(String));
                col.DefaultValue = "OK";
                channelorder.Columns.Add(col);
                string fileName = string.Empty;
                string newName = string.Empty;
                fileName = "channel_order_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                newName = Server.MapPath(excelPath) + fileName;

                if (System.IO.File.Exists(newName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newName, FileAttributes.Normal);
                    System.IO.File.Delete(newName);
                }
                //StringWriter sw = ExcelHelperXhf.SetCsvFromData(channelorder, fileName);
                // CsvHelper.ExportDataTableToCsv(channelorder,newName,null,false);
                StringWriter sw = new StringWriter();
                foreach (DataRow dr in channelorder.Rows)
                {
                    for (int i = 0; i < channelorder.Columns.Count; i++)
                    {
                        sw.Write(dr[i].ToString());
                        if (i != channelorder.Columns.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.WriteLine("");
                }
                sw.Close();

                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.ContentType = "application/ms-excel";
                Response.ContentEncoding = Encoding.Default;
                Response.Write(sw);
                Response.End();
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

        #region 傳票明細+HttpResponseBase SubPoenaDetailList()
        /// <summary>
        /// 出貨管理:檢索>傳票明細
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SubPoenaDetailList()
        {
            int ticket_id = Convert.ToInt32(Request.Params["ticket_id"]);
            string jsonStr = String.Empty;
            List<OrderDetailQuery> store = new List<OrderDetailQuery>();
            OrderDetailQuery query = new OrderDetailQuery();
            List<OrderDetailQuery> na = new List<OrderDetailQuery>();
            try
            {
                query.Ticket_Id = ticket_id;
                _IOrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _IOrderDetailMgr.SubPoenaDetail(query);
                #region MyRegion


                // List<OrderDetailQuery> one_product = new List<OrderDetailQuery>();		//單一商品
                // List<OrderDetailQuery> combination = new List<OrderDetailQuery>();			//組合商品
                // List<OrderDetailQuery> combination_head = new List<OrderDetailQuery>();		//組合品名
                // List<OrderDetailQuery> combination_tail = new List<OrderDetailQuery>();		//子商品名
                // List<OrderDetailQuery> new_order_detail = new List<OrderDetailQuery>(); ;    //新商品資料
                // List<OrderDetailQuery> since_order = new List<OrderDetailQuery>();	   //自出商品
                // Dictionary<int, string> freight_set_map = new Dictionary<int, string>();
                // freight_set_map.Add(1, "常溫");
                // freight_set_map.Add(2, "冷凍");
                // freight_set_map.Add(5, "冷藏");
                // Dictionary<int, string> product_freight_set_mapping = new Dictionary<int, string>();
                // product_freight_set_mapping.Add(1, "1");
                // product_freight_set_mapping.Add(2, "2");
                // product_freight_set_mapping.Add(3, "1");
                // product_freight_set_mapping.Add(4, "2");
                // product_freight_set_mapping.Add(5, "5");
                // product_freight_set_mapping.Add(6, "5");

                // foreach (var item in store)
                // {
                //     if (item.Combined_Mode > 1)
                //     {
                //         if (item.item_mode == 1)
                //             combination_head.Add(item);
                //         else
                //             combination_tail.Add(item);
                //     }
                //     else
                //     {
                //         uint freight = item.Product_Freight_Set;
                //         item.Product_Freight_Set =uint.Parse(product_freight_set_mapping[int.Parse(item.Product_Freight_Set.ToString())]);
                //         one_product.Add(item);
                //         item.Product_Freight_Set = freight;
                //     }
                // }
                // foreach (var item in combination_head)
                // {
                //     uint freight = item.Product_Freight_Set;
                //     item.Product_Freight_Set = uint.Parse(product_freight_set_mapping[int.Parse(item.Product_Freight_Set.ToString())]);
                //     combination.Add(item);
                //     item.Product_Freight_Set = freight;
                //     foreach (var items in combination_tail)
                //     {
                //         if (item.Parent_Id == items.Parent_Id && item.pack_id == items.pack_id)
                //         {
                //             items.Buy_Num = items.Buy_Num * items.parent_num;
                //             combination.Add(items);
                //         }
                //     }
                // }
                #endregion

                //(合併數據來自www/Model/Deliver.Php,第1391~1463行)
                #region 測試方法

                List<Parametersrc> Shipment = new List<Parametersrc>();
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                Shipment = _ptersrc.GetAllKindType("order_status");//訂單狀態
                Dictionary<string, string> _dtOrderStatus = new Dictionary<string, string>();
                foreach (var item in Shipment)
                {
                    _dtOrderStatus.Add(item.ParameterCode, item.remark);
                }
                Dictionary<int, string> freight_set_map = new Dictionary<int, string>();
                freight_set_map.Add(1, "常溫");
                freight_set_map.Add(2, "冷凍");
                freight_set_map.Add(5, "冷藏");
                Dictionary<int, string> product_freight_set_mapping = new Dictionary<int, string>();
                product_freight_set_mapping.Add(1, "1");
                product_freight_set_mapping.Add(2, "2");
                product_freight_set_mapping.Add(3, "1");
                product_freight_set_mapping.Add(4, "2");
                product_freight_set_mapping.Add(5, "5");
                product_freight_set_mapping.Add(6, "5");
                Dictionary<string, List<OrderDetailQuery>> one_product = new Dictionary<string, List<OrderDetailQuery>>();		//單一商品
                Dictionary<string, List<OrderDetailQuery>> combination = new Dictionary<string, List<OrderDetailQuery>>();		//組合商品
                List<OrderDetailQuery> combination_head = new List<OrderDetailQuery>();		//組合品名
                List<OrderDetailQuery> combination_tail = new List<OrderDetailQuery>();		//子商品名
                List<OrderDetailQuery> new_order_detail = new List<OrderDetailQuery>(); ;    //新商品資料
                List<OrderDetailQuery> since_order = new List<OrderDetailQuery>();	   //自出商品
                since_order.AddRange(new_order_detail);
                int w = since_order.Count;

                string frest;
                #region 把所有的商品，把單一和組合拆分開
                //把所有的商品，把單一和組合區分開，然後把組合的商品運送方式區分開（常溫冷凍冷藏）
                foreach (var item in store)
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

                        frest = "";
                        frest = product_freight_set_mapping[int.Parse(item.Product_Freight_Set.ToString())];
                        if (!one_product.Keys.Contains(frest))
                        {
                            List<OrderDetailQuery> s = new List<OrderDetailQuery>();
                            one_product.Add(frest, s);
                        }
                        one_product[frest].Add(item);
                        //one_product.Add(frest, item);
                    }
                }
                #endregion

                #region 把組合商品中拆分開子商品和父商品
                //把組合商品拆分開，并計算子商品的數量，把子商品按照運送方式再次拆開
                foreach (var item in combination_head)
                {

                    frest = "";
                    frest = product_freight_set_mapping[int.Parse(item.Product_Freight_Set.ToString())];
                    if (!combination.Keys.Contains(frest))
                    {
                        List<OrderDetailQuery> s = new List<OrderDetailQuery>();
                        combination.Add(frest, s);
                    }
                    //combination.Add(frest, item);
                    combination[frest].Add(item);
                    foreach (var items in combination_tail)
                    {
                        if (item.Parent_Id == items.Parent_Id && item.pack_id == items.pack_id)
                        {
                            items.Buy_Num = items.Buy_Num * items.parent_num;
                            //combination.Add(frest, item);
                            combination[frest].Add(items);
                        }
                    }
                }
                #endregion

                #region 把單一商品，組合商品，子商品 根據運送方式在此組合在一起

                foreach (var item in freight_set_map)
                {
                    ////List<OrderDetailQuery> s = new List<OrderDetailQuery>();
                    ////if (combination.ContainsKey(item.Key.ToString()))
                    ////{
                    ////    s.AddRange(combination[item.Key.ToString()]);
                    ////}
                    ////if (s.Count > 0)
                    ////{
                    ////    new_order_detail.AddRange(combination[item.Key.ToString()]);
                    ////}
                    ////else
                    ////{
                    ////    if (one_product.ContainsKey(item.Key.ToString()))
                    ////    {
                    ////        new_order_detail.AddRange(one_product[item.Key.ToString()]);
                    ////    }
                    ////}
                    if (one_product.ContainsKey(item.Key.ToString()))
                    {
                        new_order_detail.AddRange(one_product[item.Key.ToString()]);
                    }
                    if (combination.ContainsKey(item.Key.ToString()))
                    {
                        //new_order_detail.Add(combination[item.Key.ToString()]);
                        new_order_detail.AddRange(combination[item.Key.ToString()]);
                    }
                    ///--------------------------------------------------
                    //string a = item.Key.ToString();
                    //if (combination[item.Key.ToString()].Count > 0)
                    //{
                    //    new_order_detail.AddRange(combination[item.Key.ToString()]);
                    //}
                    //else if (one_product[item.Key.ToString()].Count > 0)
                    //{
                    //    //new_order_detail.Add(combination[item.Key.ToString()]);
                    //    new_order_detail.AddRange(one_product[item.Key.ToString()]);
                    //}
                }
                #endregion

                #region 這個根據什麼條件篩選出來


                for (int i = 0; i < new_order_detail.Count; i++)
                {
                    new_order_detail[i].Product_Freight_Set_Str = _dtOrderStatus[(new_order_detail[i].Detail_Status).ToString()];
                    if (new_order_detail[i].Combined_Mode >= 1 && new_order_detail[i].item_mode == 1)
                    {
                        continue;
                    }
                    else
                    {
                        na.Add(new_order_detail[i]);
                    }
                }
                #endregion

                #endregion

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(na, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        /// 傳票明細:未出貨
        /// </summary>
        /// <returns></returns>
        public JsonResult NoDelivery()////Admin/Controller/DeliversController/第301行 no_delivery
        {
            string json = string.Empty;
            OrderDetailQuery query = new OrderDetailQuery();
            if (!string.IsNullOrEmpty(Request.Params["Detail_id"]))
            {
                query.Detail_Id = uint.Parse(Request.Params["Detail_id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["Deliver_id"]))
            {
                query.Deliver_Id = uint.Parse(Request.Params["Deliver_id"]);
            }

            _IOrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
            if (_IOrderDetailMgr.no_delivery(query))
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }

        }
        /// <summary>
        /// 拆分細項
        /// </summary>
        /// <returns></returns>
        public JsonResult split_detail()//Admin/Controller/DeliversController/第325行 split_detail
        {

            string json = string.Empty;
            OrderDetailQuery query = new OrderDetailQuery();
            if (!string.IsNullOrEmpty(Request.Params["Detail_Id"]))
            {
                query.Detail_Id = uint.Parse(Request.Params["Detail_Id"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["Deliver_Id"]))
            {
                query.Deliver_Id = uint.Parse(Request.Params["Deliver_Id"]);
            }

            _IOrderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
            if (_IOrderDetailMgr.split_detail(query) == 1)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }

        }
        #endregion

        #region 每日出貨總表
        /// <summary>
        /// 每日出貨總表
        /// </summary>
        /// <returns>每日出貨數據列表</returns>
        public HttpResponseBase EveryDayShipmentList()
        {
            string json = string.Empty;
            List<OrderMasterQuery> listOrderMaster = new List<OrderMasterQuery>();
            List<OrderMasterQuery> newlistOrderMaster = new List<OrderMasterQuery>();
            OrderMasterQuery query = new OrderMasterQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["orderDatePayStartTime"]))
                {
                    query.order_date_pay_startTime = Convert.ToDateTime(Request.Params["orderDatePayStartTime"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orderDatePayEndTime"]))
                {
                    query.order_date_pay_endTime = Convert.ToDateTime(Request.Params["orderDatePayEndTime"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["delays"]))
                {
                    query.delay = int.Parse(Request.Params["delays"]);
                }
                int totalCount = 0;
                listOrderMaster = _orderMasterMgr.GetShipmentList(query, out totalCount);
                for (int i = 0; i < listOrderMaster.Count; i++)
                {
                    if (listOrderMaster[i].combined_mode >= 1 && listOrderMaster[i].item_mode == 1) //將組合商品篩選掉
                    {
                        //listOrderMaster.RemoveAt(i);
                    }
                    else
                    {
                        newlistOrderMaster.Add(listOrderMaster[i]);
                    }
                }

                for (int i = 0; i < newlistOrderMaster.Count; i++)
                {
                    newlistOrderMaster[i].product_name = newlistOrderMaster[i].product_name + newlistOrderMaster[i].product_spec_name;
                    for (int j = 0; j < i; j++)
                    {

                        if (newlistOrderMaster.Count > 0)
                        {
                            if (newlistOrderMaster[j].OrderId == newlistOrderMaster[i].OrderId)
                            {
                                newlistOrderMaster[i].OrderId = 0;
                                if (newlistOrderMaster[j].MoneyCollectDate == newlistOrderMaster[i].MoneyCollectDate)
                                {
                                    newlistOrderMaster[i].MoneyCollectDate = DateTime.MinValue;
                                }
                                if (newlistOrderMaster[j].OrderDatePay == newlistOrderMaster[i].OrderDatePay)
                                {
                                    newlistOrderMaster[i].OrderDatePay = DateTime.MinValue;
                                }
                                if (newlistOrderMaster[j].freight_set == newlistOrderMaster[i].freight_set)
                                {
                                    newlistOrderMaster[i].freight_set = 0;
                                }
                                if (newlistOrderMaster[j].order_name == newlistOrderMaster[i].order_name)
                                {
                                    newlistOrderMaster[i].order_name = "";
                                }
                                if (newlistOrderMaster[j].delivery_name == newlistOrderMaster[i].delivery_name)
                                {
                                    newlistOrderMaster[i].delivery_name = "";
                                }
                                if (newlistOrderMaster[j].type == newlistOrderMaster[i].type)
                                {
                                    newlistOrderMaster[i].type = 0;
                                }
                                if (newlistOrderMaster[j].deliver_id == newlistOrderMaster[i].deliver_id)
                                {
                                    newlistOrderMaster[i].deliver_id = 0;
                                }
                                if (newlistOrderMaster[j].vendor_name_simple == newlistOrderMaster[i].vendor_name_simple)
                                {
                                    newlistOrderMaster[i].vendor_name_simple = "";
                                }
                                if (newlistOrderMaster[j].Order_Status == newlistOrderMaster[i].Order_Status)
                                {
                                    newlistOrderMaster[i].Order_Status = 999;
                                }
                                if (newlistOrderMaster[j].delivery_status == newlistOrderMaster[i].delivery_status)
                                {
                                    newlistOrderMaster[j].delivery_status = 999;
                                }
                                if (newlistOrderMaster[j].note_order == newlistOrderMaster[i].note_order)
                                {
                                    newlistOrderMaster[i].note_order = "";
                                }

                            }

                        }
                    }
                }
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(newlistOrderMaster, Formatting.Indented, timeConverter) + "}";
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 批次匯入物流費+HttpResponseBase HuiruPiciAddwuliufei()
        public HttpResponseBase HuiruPiciAddwuliufei()
        {
            string json = string.Empty;//json字符串
            string shipment = Request.Params["shipment"].ToString();
            //int total = 0;
            try
            {
                if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)//判斷文件是否為空
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];//獲取文件流
                    FileManagement fileManagement = new FileManagement();//實例化 FileManagement
                    string fileLastName = excelFile.FileName;
                    string newExcelName = Server.MapPath(excelPath) + "PiCiAddWuliufei" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名
                    excelFile.SaveAs(newExcelName);//上傳文件
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    dt = helper.SheetData();
                    DataRow[] dr = dt.Select(); //定义一个DataRow数组,读取ds里面所有行
                    int rowsnum = dt.Rows.Count;
                    if (rowsnum > 0)//判斷是否是這個表
                    {
                        DeliverMasterQuery dmQuery = new DeliverMasterQuery();
                        StringBuilder str = new StringBuilder();
                        DataTable dtMaster = new DataTable();
                        DataTable ExcelDt = new DataTable();
                        string filenameExcel = string.Empty;
                        ExcelDt.Columns.Add("訂單編號", typeof(String));
                        ExcelDt.Columns.Add("物流單號", typeof(String));
                        ExcelDt.Columns.Add("物流費", typeof(String));
                        ExcelDt.Columns.Add("應收帳款", typeof(String));
                        ExcelDt.Columns.Add("異常信息", typeof(String));
                        int i = 0; int j = 0; int x = 0; string y = string.Empty;
                        int successcount = 0;
                        int failcount = 0;
                        string create_dtim = CommonFunction.DateTimeToString(DateTime.Now);       //創建時間
                        int create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                        _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);
                        #region 循環excel表中的數據 并判斷是否滿足條件和失敗的個數
                        for (int z = 0; z < dr.Length; z++)
                        {
                            x = Convert.ToInt32(dr[z][0].ToString());//訂單編號
                            y = dr[z][1].ToString();//托運單號
                            i = Convert.ToInt32(dr[z][2].ToString());//運送金額
                            j = Convert.ToInt32(dr[z][3].ToString());//代收金額--也就是購物應該付款總金額
                            dmQuery.delivery_code = y;//托運單號

                            dtMaster = _DeliverMsterMgr.GetMessageByDeliveryCode(dmQuery);
                            string errorstring = string.Empty;
                            bool behavior = true;
                            if (dtMaster.Rows.Count <= 0)
                            {
                                errorstring = "物流單號不存在";
                                behavior = false;
                            }
                            else if (Convert.ToInt32(dtMaster.Rows[0]["delivery_store"]) != Convert.ToInt32(shipment))//如果物流方式不對應
                            {
                                errorstring = "物流廠商不相符,此物流單對應物流為:" + dtMaster.Rows[0]["parameterName"].ToString();
                                behavior = false;
                            }
                            //else if (y != dtMaster.Rows[0]["delivery_code"].ToString())
                            //{
                            //    errorstring = "物流單號不相符";
                            //    behavior = false;
                            //}
                            else if (dtMaster.Rows[0]["order_id"].ToString() != x.ToString())
                            {
                                errorstring = "定單編號不相符,此物流單對應的定單編號為:" + dtMaster.Rows[0]["order_id"].ToString();
                                behavior = false;
                            }
                            else if (dtMaster.Rows.Count > 1)
                            {
                                errorstring = "物流單號重複";
                                behavior = false;
                            }
                            //10表示黑貓貨到付款     order_amount購物應付總金額(加運費,扣除扺用紅利等金額)
                            else if (Convert.ToInt32(dtMaster.Rows[0]["delivery_store"]) == 10 && j != Convert.ToInt32(dtMaster.Rows[0]["order_amount"]))
                            {
                                errorstring = "應收帳款金額不符";
                                behavior = false;
                            }
                            //4表示已出貨 9 表示待取貨
                            else if (Convert.ToInt32(dtMaster.Rows[0]["order_status"]) != 4 && Convert.ToInt32(dtMaster.Rows[0]["order_status"]) != 9)
                            {
                                _ptersrc = new ParameterMgr(mySqlConnectionString);
                                int types = Convert.ToInt32(dtMaster.Rows[0]["order_status"]);
                                string endresult = _ptersrc.GetOrderStatus(types);
                                errorstring = "出貨狀態異常：出貨狀態為" + endresult;
                                behavior = false;
                            }
                            if (behavior == true)//如果數據不存在這些問題   如果failcount大於0就表示匯入信息有不正確的
                            {
                                successcount = successcount + 1;
                                //DataRow Execldr = ExcelDt.NewRow();
                                //Execldr[0] = x;
                                //Execldr[1] = y;
                                //Execldr[2] = i;
                                //Execldr[3] = dtMaster.Rows[0]["order_amount"];
                                //Execldr[4] = "數據正常";
                                //ExcelDt.Rows.Add(Execldr);
                                str.AppendFormat(" set sql_safe_updates = 0;update deliver_master set delivery_freight_cost='{0}',creator='{1}',modified='{2}' where delivery_code='{3}' ;set sql_safe_updates = 1;", i, create_user, create_dtim, y);
                            }
                            else
                            {
                                failcount = failcount + 1;
                                DataRow Execldr = ExcelDt.NewRow();
                                Execldr[0] = x;
                                Execldr[1] = y;
                                Execldr[2] = i;
                                Execldr[3] = j;//dtMaster.Rows[0]["order_amount"];
                                Execldr[4] = errorstring;
                                ExcelDt.Rows.Add(Execldr);
                                //此次上傳資料有異請下載差異檔
                            }
                        }
                        #endregion

                        #region 判斷失敗個數 成功個數 當失敗個數大於0時 直接匯出excel
                        if (failcount > 0)//存在失敗的情況 直接匯出數據
                        {
                            string fileName = DateTime.Now.ToString("匯出批次上傳物流費不規則數據_yyyyMMddHHmm") + ".xls";
                            MemoryStream ms = ExcelHelperXhf.ExportDT(ExcelDt, "匯出批次上傳物流費不規則數據");
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                            Response.BinaryWrite(ms.ToArray());
                            return this.Response;
                        }
                        else if (failcount == 0 && successcount > 0)
                        {
                            if (_DeliverMsterMgr.Updatedeliveryfreightcost(str) > 0)
                            {
                                json = "{success:true,total:" + successcount + ",msg:\"" + "匯入成功" + "\"}";
                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }
                            else
                            {
                                json = "{success:false}";
                                this.Response.Clear();
                                this.Response.Write(json);
                                this.Response.End();
                                return this.Response;
                            }
                        }
                        else
                        {
                            json = "{success:true,msg:\"" + "此表內沒有數據或數據有誤,請檢查后再次匯入!" + "\"}";
                            this.Response.Clear();
                            this.Response.Write(json);
                            this.Response.End();
                            return this.Response;
                        }
                        #endregion
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
                    json = "{success:true,msg:\"" + "請匯入批次上傳物流費表" + "\"}";
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
                json = "{success:false,msg:\"" + ex.ToString() + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        #endregion

        #region 廠商批次出貨單查詢

        public HttpResponseBase GetBatchList()
        {
            string jsonStr = String.Empty;
            StringBuilder sb = new StringBuilder();
            int isTranUint = 0;
            try
            {
                _orderSlaveMasterMgr = new OrderSlaveMasterMgr(mySqlConnectionString);

                OrderSlaveMasterQuery query = new OrderSlaveMasterQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["searchType"]))
                {
                    if (Request.Params["searchType"].ToString() == "1")
                    {
                        query.code_num = Request.Params["searchContent"].ToString();
                    }
                    else if (Request.Params["searchType"].ToString() == "2")
                    {
                        query.vendor_name_simple = Request.Params["searchContent"].ToString();
                    }
                }
                if (int.TryParse(Request.Params["status"], out isTranUint))
                {
                    if (Request.Params["status"].ToString() != "-1")
                    {
                        query.is_check = 1;//在條件中加入on_chack搜索
                        query.on_check = Convert.ToUInt32(Request.Params["status"]);
                    }
                    else
                    {
                        query.is_check = -1;
                    }

                }
                if (int.TryParse(Request.Params["dateType"], out isTranUint))
                {
                    query.date_type = Convert.ToUInt32(Request.Params["dateType"]);
                    query.date_start = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["dateStart"]));
                    query.date_end = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["dateEnd"]));

                }
                int totalCount = 0;
                List<OrderSlaveMasterQuery> store = _orderSlaveMasterMgr.GetBatchList(query, out totalCount);


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

        #region 批次出貨單明細
        public HttpResponseBase GetBatchDetailList()
        {
            string jsonStr = String.Empty;
            StringBuilder sb = new StringBuilder();
            uint isTranUint = 0;
            try
            {
                _orderSlaveMasterMgr = new OrderSlaveMasterMgr(mySqlConnectionString);
                List<OrderSlaveMasterQuery> detailStore = new List<OrderSlaveMasterQuery>();
                OrderSlaveMasterQuery query = new OrderSlaveMasterQuery();
                if (uint.TryParse(Request.Params["slave_master_id"], out isTranUint))
                {
                    string slaveStr = string.Empty;
                    query.slave_master_id = Convert.ToUInt32(Request.Params["slave_master_id"]);
                    List<OrderSlaveMasterQuery> store = _orderSlaveMasterMgr.GetSlaveByMasterId(query);
                    foreach (var item in store)
                    {
                        slaveStr += item.slave_id + ",";
                    }
                    slaveStr = slaveStr.Substring(0, slaveStr.Length - 1);
                    detailStore = _orderSlaveMasterMgr.GetDetailBySlave(slaveStr);
                    int j = 0;
                    for (int i = 1; i < detailStore.Count; i++)
                    {

                        j = i - 1;
                        while (detailStore[j].order_id == 0 && j > 0)
                        {
                            j = j - 1;
                        }
                        if (detailStore[i].order_id == detailStore[j].order_id)
                        {
                            detailStore[i].order_id = 0;
                        }
                        if (detailStore[i].slave_id == detailStore[j].slave_id)
                        {
                            detailStore[i].slave_id = 0;
                        }
                    }
                }

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(detailStore, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        //#region 到貨確認//到貨確認功能棄用
        //public HttpResponseBase BatchSendProd()
        //{
        //    string jsonStr = String.Empty;
        //    StringBuilder sb = new StringBuilder();
        //    string smIDs = string.Empty;

        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["slaveMasterIds"]))
        //        {
        //            smIDs = Request.Params["slaveMasterIds"].ToString();
        //            _orderSlaveMasterMgr = new OrderSlaveMasterMgr(mySqlConnectionString);
        //            //獲取當前用戶和ip
        //            System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;

        //            string userIP = addlist[0].ToString();

        //            string userName = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_username;
        //            if (_orderSlaveMasterMgr.BatchSendProd(smIDs, userName, userIP))
        //            {
        //                jsonStr = "{success:true,msg:1}";//成功
        //            }
        //            else
        //            {
        //                jsonStr = "{success:false,msg:0}";
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        jsonStr = "{success:false,msg:0}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(jsonStr.ToString());
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion


        #endregion

        #region 當匯出pdf之後,改變表中的狀態
        public HttpResponseBase UpdateTicketStatus()
        {
            string json = string.Empty;
            try
            {
                TicketQuery tdquery = new TicketQuery();
                tdquery.type_id = Convert.ToInt32(Request.Params["type"]);
                string temp = Request.Params["tickets"].ToString();
                tdquery.ticket_idto_str = temp.Remove(temp.LastIndexOf(','));
                _tkMgr = new TicketMgr(mySqlConnectionString);
                if (_tkMgr.UpdateTicketStatus(tdquery) > 0)
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

        #region 可出貨
        public HttpResponseBase GetDeliveralbelList()
        {
            string jsonStr = string.Empty;
            try
            {

                DeliverMasterQuery query = new DeliverMasterQuery();
                List<DeliverMasterQuery> store = new List<DeliverMasterQuery>();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                query.type = 1;
                query.export_id = 2;
                query.freight_set = 1;
                if (!string.IsNullOrEmpty(Request.Params["Search"]))
                {
                    query.freight_set = uint.Parse(Request.Params["Search"]);
                }
                query.delivery_status = 0;
                if (query.type == 1)
                    query.delivery_status = 1;
                int totalCount = 0;
                _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);

                store = _DeliverMsterMgr.GetTicketDetailList(query, out totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";// HH:mm:ss
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
        #endregion

        #region 延遲出貨
        public HttpResponseBase GetDelayDeliverList()
        {
            string jsonStr = string.Empty;
            try
            {
                DeliverMasterQuery query = new DeliverMasterQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);
                int totalCount = 0;
                DataTable _dt = _DeliverMsterMgr.GetDelayDeliverList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";// HH:mm:ss
                jsonStr = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 出貨單統計
        //add by yafeng0715j 2015-08-20 PM
        public HttpResponseBase GetDeliverMasterList()
        {
            DateTime dt = DateTime.Now.AddDays(-1);
            DateTime dtStart = Convert.ToDateTime(dt.ToString("yyyy-MM-dd 00:00:00"));
            DateTime dtEnd = Convert.ToDateTime(dt.ToString("yyyy-MM-dd 23:59:59"));
            string jsonStr = string.Empty;
            try
            {
                DeliverMasterQuery query = new DeliverMasterQuery();
                DateTime date;
                if (DateTime.TryParse(Request.Params["time_start"], out date))
                {
                    query.time_start = Convert.ToDateTime(date.ToString("yyyy-MM-dd 00:00:00"));
                }
                else
                {
                    query.time_start = dtStart;
                }
                if (DateTime.TryParse(Request.Params["time_end"], out date))
                {
                    query.time_end = Convert.ToDateTime(date.ToString("yyyy-MM-dd 23:59:59"));
                }
                else
                {
                    query.time_end = dtEnd;
                }
                _ptersrc = new ParameterMgr(mySqlConnectionString);
                List<Parametersrc> parametersrcList = _ptersrc.ReturnParametersrcList();

                int sum2 = 0;
                int sum92 = 0;
                DataTable table = new DataTable();
                table.Columns.Add("物流商", typeof(string));
                table.Columns.Add("統倉包裹數", typeof(string));
                table.Columns.Add("冷凍倉包裹數", typeof(string));
                _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);

                for (int i = 0; i < parametersrcList.Count; i++)
                {

                    Parametersrc par = parametersrcList[i];
                    query.delivery_store = uint.Parse(par.ParameterCode);
                    query.export_id = 2;
                    int sum1 = 0;
                    sum1 = _DeliverMsterMgr.GetDeliverMasterCount(query);
                    sum2 += sum1;
                    query.export_id = 92;
                    int sum = _DeliverMsterMgr.GetDeliverMasterCount(query);
                    sum92 += sum;
                    DataRow dr = table.NewRow();
                    dr[0] = parametersrcList[i].parameterName;
                    dr[1] = sum1;
                    dr[2] = sum;
                    if (sum1 != 0 || sum != 0)
                    {
                        table.Rows.Add(dr);
                    }
                    if (i == (parametersrcList.Count - 1))
                    {
                        dr[0] = "總計";
                        dr[1] = sum2;
                        dr[2] = sum92;
                        table.Rows.Add(dr);
                    }
                }
                jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(table) + "}";//返回json數據
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

        #region 出貨單期望到貨日
        /// <summary>
        /// 出貨單期望到貨日
        /// </summary>
        /// <returns></returns>
        // by zhaozhi0623j add at 20151110
        public HttpResponseBase GetDeliverExpectArrivalList()
        {
            string json = string.Empty;
            DeliverMasterQuery dmQuery = new DeliverMasterQuery();
            _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);
            List<DeliverMasterQuery> dmList = new List<DeliverMasterQuery>();
            try
            {
                dmQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                dmQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量

                #region 查詢條件

                if (Request.Params["productMode"] != "-1")//type為0時，表示全部
                {
                    dmQuery.type = Convert.ToUInt32(Request.Params["productMode"]);
                }
                if (Request.Params["freightType"] != "-1")//freight_set為0時，表示全部
                {
                    dmQuery.freight_set = Convert.ToUInt32(Request.Params["freightType"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["deliveryStatus"]))//delivery_status為10000時，表示全部狀態
                {
                    dmQuery.delivery_status = Convert.ToUInt32(Request.Params["deliveryStatus"]);
                }


                if (!string.IsNullOrEmpty(Request.Params["deliverId"]))
                {
                    dmQuery.deliver_id = Convert.ToUInt32(Request.Params["deliverId"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["orderId"]))
                {
                    dmQuery.order_id = Convert.ToInt32(Request.Params["orderId"]);
                }
                string vendorId_ro_name = Request.Params["vendorId_ro_name"];
                if (!string.IsNullOrEmpty(vendorId_ro_name))
                {
                    if (Regex.IsMatch(vendorId_ro_name, @"^[0-9]*$"))
                    {
                        dmQuery.vendor_id = Convert.ToUInt32(vendorId_ro_name);
                    }
                    else
                    {
                        dmQuery.vendor_name_full = vendorId_ro_name;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["time_start"]))
                {             
                    dmQuery.time_start = Convert.ToDateTime(Convert.ToDateTime(Request.Params["time_start"]).ToString("yyyy-MM-dd "));
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                {
                    dmQuery.time_end = Convert.ToDateTime(Convert.ToDateTime(Request.Params["time_end"]).ToString("yyyy-MM-dd "));
                } 
                #endregion

                int totalCount = 0;
                dmList = _DeliverMsterMgr.GetDeliverExpectArriveList(dmQuery,out totalCount);
                //foreach (var item in dmList)
                //{
                //    if(item.type==)
                    
                //}

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dmList, Formatting.Indented, timeConverter) + "}";
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

        public HttpResponseBase SaveDeliverExportArrivalInfo()
        {
            string json = string.Empty;
            DeliverChangeLog dCL = new DeliverChangeLog();
            DeliverMasterQuery dmQuery = new DeliverMasterQuery();
            _DeliverChangeLogMgr = new DeliverChangeLogMgr(mySqlConnectionString);
            _DeliverMsterMgr = new DeliverMasterMgr(mySqlConnectionString);
            try
            {
                                             
                dCL.deliver_id = Convert.ToInt32(Request.Params["deliver_id"]);
                string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//XML的設置
                string path = Server.MapPath(xmlPath);
                SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                string APIServer = _siteConfigMgr.GetConfigByName("APIServer").Value;
                bool isCanModidy = _DeliverChangeLogMgr.isCanModifyExpertArriveDate(APIServer,dCL.deliver_id);
                
                dCL.dcl_create_datetime = DateTime.Now;
                dCL.dcl_ipfrom = CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                dCL.dcl_create_muser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                dCL.dcl_create_user = 0;
                dCL.dcl_create_type = 2;
                dmQuery.deliver_id = Convert.ToUInt32(Request.Params["deliver_id"]); 
    
                if (!string.IsNullOrEmpty(Request.Params["dcl_note"]))
                {
                    dCL.dcl_note = Request.Params["dcl_note"]; 
                }
                if (!string.IsNullOrEmpty(Request.Params["expect_arrive_date"]))
                {
                    dCL.expect_arrive_date = Convert.ToDateTime(Request.Params["expect_arrive_date"]);
                    dmQuery.expect_arrive_date = Convert.ToDateTime(Request.Params["expect_arrive_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["expect_arrive_period"]))
                {
                    dCL.expect_arrive_period = Convert.ToInt32(Request.Params["expect_arrive_period"]);
                    dmQuery.expect_arrive_period = Convert.ToInt32(Request.Params["expect_arrive_period"]);
                }
                ModifyExpertArriveDateViewModel expertArriveDateViewModel = new ModifyExpertArriveDateViewModel();
               
                //更新deliver_mater表的 期望到貨日期、時段
                int result1 = _DeliverMsterMgr.UpdateExpectArrive(dmQuery);
                //向deliver_change_log表插入數據
                int result2 = _DeliverChangeLogMgr.insertDeliverChangeLog(dCL);

                if (result1 > 0)
                {
                    if (result2 > 0)
                    {
                        json = "{success:true,msg:'保存成功'}";//
                    }
                    else
                    {
                        json = "{success:true,msg:'deliver_mster表數據更新成功,<br/>deliver_change_log表數據添加失敗'}";
                    }
                }
                else
                {
                    if (result2 > 0)
                    {
                        json = "{success:true,msg: 'deliver_mster表數據更新失敗,<br/>deliver_change_log數據添加成功'}";
                    }
                    else
                    {
                        json = "{success:true,msg:'deliver_mster表數據更新失敗,<br/>deliver_change_log表數據添加失敗'}";
                    }
                }                
            }
            catch(Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'保存失敗'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 期望到貨日調整記錄
        /// <summary>
        /// 期望到货日调整记录
        /// </summary>
        /// <returns></returns>
        // by zhaozhi0623j add at 20151112
        public HttpResponseBase GetDeliveryChangeLogList()
        {
            string json = string.Empty;
            DeliverChangeLogQuery dclQuery = new DeliverChangeLogQuery();
            _DeliverChangeLogMgr = new DeliverChangeLogMgr(mySqlConnectionString);
            List<DeliverChangeLogQuery> dclList = new List<DeliverChangeLogQuery>();
            try
            {
                dclQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                dclQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量

                #region 查詢條件

                if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))//出貨單單號
                {
                    dclQuery.deliver_id = Convert.ToInt32(Request.Params["deliver_id"]);
                }
                if (Request.Params["dcl_create_type"] != "-1")//創建類型1:前台創建 2:後台創建
                {
                    dclQuery.dcl_create_type = Convert.ToInt32(Request.Params["dcl_create_type"]);
                }               
                if (!string.IsNullOrEmpty(Request.Params["userName_ro_email"]))//出貨單記錄調整人員
                {
                    if (Request.Params["query_type"] == "1")
                    {
                        dclQuery.dcl_user_name = Request.Params["userName_ro_email"];
                    }
                    if (Request.Params["query_type"] == "2")
                    {
                        dclQuery.dcl_user_email = Request.Params["userName_ro_email"];
                    }                  
                }

                if (!string.IsNullOrEmpty(Request.Params["time_start"]))//dcl_create_datetime
                {
                    dclQuery.time_start = Convert.ToDateTime(Convert.ToDateTime(Request.Params["time_start"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["time_end"]))
                {
                    dclQuery.time_end = Convert.ToDateTime(Convert.ToDateTime(Request.Params["time_end"]).ToString("yyyy-MM-dd 23:59:59"));
                }
                #endregion

                int totalCount = 0;
                dclList = _DeliverChangeLogMgr.GetDeliverChangeLogList(dclQuery, out totalCount);
                //foreach (var item in dclList)
                //{
                //  
                //}

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式                   
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dclList, Formatting.Indented, timeConverter) + "}";
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