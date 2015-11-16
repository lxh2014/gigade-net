using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
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
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
using System.Text.RegularExpressions;
using BLL.gigade.Model.Custom;

namespace Admin.gigade.Controllers
{//會計模塊
    public class AccountantController : Controller
    {
        //
        // GET: /Accountant/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        //   private IInvoiceAllowanceRecordlMgr _invoiceAllow;
        private IOrderMasterImplMgr _orderMasterMgr;
        private IOrderSlaveImplMgr _orderSlaveMgr;

        //  private IOrderPaymentCtImplMgr _iopcMgr;
        private InvoiceMasterRecordMgr imrMgr;
        private IOrderDetailImplMgr _orderDetailMgr;
        //  private IOrderMoneyReturnImplMgr _IOrderMoneyReturnMgr;
        private IManageUserImplMgr _muMgr;
        private ISinopacDetailImplMgr _spcdil;
        private IConfigImplMgr _configMgr;
        private IVendorAccountMonthImplMgr _IVAMMgr;
        private static DataTable DTExcel = new DataTable();
        private static DataTable DTExcel1 = new DataTable();
        private static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private IupcMgr _iupc;

        #region 視圖
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InvoiceAllowanceRecord()
        {//折讓發票列表頁
            return View();
        }
        public ActionResult OrderBillingCheckIndex()
        {//現金,外站,貨到付款對賬列表
            return View();
        }
        public ActionResult OrderBillingCheckList()
        {//泛用對賬
            return View();
        }
        public ActionResult OrderBillingCheckList1()
        {//中信紅利對賬
            return View();
        }
        /// <summary>
        /// 強制開立發票
        /// </summary>
        /// <returns>強制開立發票視圖</returns>
        public ActionResult OpenInvoices()
        {
            return View();
        }
        /// <summary>
        /// 開立發票列表
        /// </summary>
        /// <returns></returns>
        public ActionResult InvoiceList()
        {
            return View();
        }
        public ActionResult OrderMoneyReturn()
        {//退款單
            return View();
        }
        public ActionResult VendorAccountMonthList()
        {//供應商業績報表
            return View();
        }
        public ActionResult VendorAccountDetail()
        {//供應商業績報表明細
            ViewBag.dateone = Request.QueryString["dateOne"];
            ViewBag.datetwo = Request.QueryString["dateTwo"];
            ViewBag.vendorid = Request.QueryString["vendor_id"];
            ViewBag.vendorcode = Request.QueryString["vendor_code"];
            ViewBag.vendorname = Request.QueryString["vendor_name_simple"];
            return View();
        }
        #endregion

        //#region 折讓發票列表
        //#region 列表頁
        //public HttpResponseBase InvoiceAllowanceRecordList()
        //{//折讓發票列表頁  
        //    string json = string.Empty;
        //    InvoiceAllowanceRecordQuery query = new InvoiceAllowanceRecordQuery();
        //    DataTable dtIAIList;
        //    DataTable dtIARList = new DataTable();
        //    DataTable Newdt = new DataTable();
        //    _invoiceAllow = new InvoiceAllowanceRecordMgr(mySqlConnectionString);
        //    int totalCount = 0;
        //    try
        //    {
        //        #region 查詢條件

        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
        //        if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
        //        {//查詢條件
        //            query.seach_tj = Convert.ToInt32(Request.Params["ddlSel"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["selcontent"]))
        //        {//查詢條件內容
        //            query.content = Convert.ToInt32(Request.Params["selcontent"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddtSel"]))
        //        {//日期條件
        //            query.ddtSel = Convert.ToInt32(Request.Params["ddtSel"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["time_start"]))
        //        {//開始時間
        //            query.startdate = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["time_start"].ToString()));
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["time_end"]))
        //        {//結束時間
        //            query.enddate = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["time_end"].ToString()));
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddlstatus"]))
        //        {//開立狀態
        //            query.ddlstatus = Convert.ToInt32(Request.Params["ddlstatus"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddlinvoice"]))
        //        {//發票類型
        //            query.ddlinvoice = Convert.ToInt32(Request.Params["ddlinvoice"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddlqh"]))
        //        {//是否簽回
        //            query.ddlqh = Convert.ToInt32(Request.Params["ddlqh"]);
        //        }
        //        #endregion
        //        dtIARList = _invoiceAllow.GetInvoiceAllowanceRecordList(query, out dtIAIList, out totalCount);

        //        #region 數據轉換,dt賦值
        //        dtIAIList.Columns.Add("nobonus_money", typeof(string));
        //        dtIAIList.Columns.Add("allowance_amount_tmp", typeof(string));
        //        dtIAIList.Columns.Add("tax", typeof(string));
        //        #region 合成的新數據dt列
        //        Newdt.Columns.Add("allowance_id", typeof(string));
        //        Newdt.Columns.Add("order_id", typeof(string));
        //        Newdt.Columns.Add("invoice_number", typeof(string));
        //        Newdt.Columns.Add("invoice_date", typeof(string));
        //        Newdt.Columns.Add("allowance_date", typeof(string));
        //        Newdt.Columns.Add("allownace_total", typeof(string));
        //        Newdt.Columns.Add("allowance_tax", typeof(string));
        //        Newdt.Columns.Add("allowance_amount", typeof(string));
        //        Newdt.Columns.Add("buyer_type", typeof(string));
        //        Newdt.Columns.Add("buyertype", typeof(int));
        //        Newdt.Columns.Add("buyer_name", typeof(string));
        //        Newdt.Columns.Add("company_invoice", typeof(string));
        //        Newdt.Columns.Add("company_title", typeof(string));
        //        Newdt.Columns.Add("allowance_return", typeof(string));
        //        Newdt.Columns.Add("allowance_return_date", typeof(string));
        //        Newdt.Columns.Add("invoice_status", typeof(string));
        //        Newdt.Columns.Add("invoicestatus", typeof(int));
        //        Newdt.Columns.Add("allowancereturn", typeof(int));
        //        Newdt.Columns.Add("invoice_id", typeof(int));
        //        #endregion
        //        for (int i = 0; i < dtIARList.Rows.Count; i++)
        //        {
        //            DataRow newdatarow = Newdt.NewRow();
        //            newdatarow["invoice_id"] = Convert.ToInt32(dtIARList.Rows[i]["invoice_id"]);
        //            newdatarow["invoicestatus"] = Convert.ToInt32(dtIARList.Rows[i]["invoice_status"]);
        //            switch (dtIARList.Rows[i]["invoice_status"].ToString())
        //            {
        //                case "1":
        //                    newdatarow["invoice_status"] = "存入資料庫";
        //                    break;
        //                case "2":
        //                    newdatarow["invoice_status"] = "上傳至發票機";
        //                    break;
        //                case "3":
        //                    newdatarow["invoice_status"] = "上傳至財政部";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            newdatarow["buyertype"] = Convert.ToInt32(dtIARList.Rows[i]["buyer_type"]);
        //            switch (dtIARList.Rows[i]["buyer_type"].ToString())
        //            {
        //                case "0":
        //                    newdatarow["buyer_type"] = "個人";
        //                    break;
        //                case "1":
        //                    newdatarow["buyer_type"] = "公司行號";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            newdatarow["allowancereturn"] = Convert.ToInt32(dtIARList.Rows[i]["allowance_return"]);
        //            switch (dtIARList.Rows[i]["allowance_return"].ToString())
        //            {
        //                case "1":
        //                    newdatarow["allowance_return"] = "簽回";
        //                    break;
        //                case "0":
        //                    newdatarow["allowance_return"] = "未簽回";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            if (dtIARList.Rows[i]["invoice_date"].ToString() != "0" && !string.IsNullOrEmpty(dtIARList.Rows[i]["invoice_date"].ToString()))
        //            {
        //                newdatarow["invoice_date"] = CommonFunction.GetNetTime(uint.Parse(dtIARList.Rows[i]["invoice_date"].ToString())).ToString("yyyy-MM-dd hh:mm:ss");
        //            }
        //            if (dtIARList.Rows[i]["allowance_date"].ToString() != "0" && !string.IsNullOrEmpty(dtIARList.Rows[i]["allowance_date"].ToString()))
        //            {
        //                newdatarow["allowance_date"] = CommonFunction.GetNetTime(uint.Parse(dtIARList.Rows[i]["allowance_date"].ToString())).ToString("yyyy-MM-dd hh:mm:ss");
        //            }
        //            if (dtIARList.Rows[i]["allowance_return_date"].ToString() != "0" && !string.IsNullOrEmpty(dtIARList.Rows[i]["allowance_return_date"].ToString()))
        //            {
        //                newdatarow["allowance_return_date"] = CommonFunction.GetNetTime(uint.Parse(dtIARList.Rows[i]["allowance_return_date"].ToString())).ToString("yyyy-MM-dd hh:mm:ss");
        //            }
        //            for (int j = 0; j < dtIAIList.Rows.Count; i++)
        //            {
        //                if (dtIAIList.Rows[j]["allowance_id"].ToString() == dtIARList.Rows[i]["allowance_id"].ToString())
        //                {
        //                    dtIAIList.Rows[j]["nobonus_money"] = (Convert.ToInt32(dtIAIList.Rows[j]["nobonus_money"]) + Convert.ToInt32(dtIARList.Rows[i]["nobonus_money"])).ToString();
        //                    dtIAIList.Rows[j]["allowance_amount_tmp"] = (Convert.ToInt32(dtIAIList.Rows[j]["nobonus_money"]) / (1 + 0.05)).ToString();
        //                    dtIAIList.Rows[j]["tax"] = (Convert.ToInt32(dtIAIList.Rows[j]["nobonus_money"]) - Convert.ToInt32(dtIAIList.Rows[j]["allowance_amount_tmp"])).ToString();
        //                }
        //                newdatarow["allownace_total"] = Convert.ToInt32(dtIARList.Rows[i]["allownace_total"]) + Convert.ToInt32(dtIAIList.Rows[j]["nobonus_money"]);
        //                newdatarow["allowance_amount"] = Convert.ToInt32(dtIARList.Rows[i]["allowance_amount"]) + Convert.ToInt32(dtIAIList.Rows[j]["allowance_amount_tmp"]);
        //                newdatarow["allowance_tax"] = Convert.ToInt32(dtIARList.Rows[i]["allowance_tax"]) + Convert.ToInt32(dtIAIList.Rows[j]["tax"]);
        //            }
        //            newdatarow["allowance_id"] = dtIARList.Rows[i]["allowance_id"].ToString();
        //            newdatarow["order_id"] = dtIARList.Rows[i]["order_id"].ToString();
        //            newdatarow["invoice_number"] = dtIARList.Rows[i]["invoice_number"].ToString();
        //            newdatarow["company_title"] = dtIARList.Rows[i]["company_title"].ToString();
        //            newdatarow["company_invoice"] = dtIARList.Rows[i]["company_invoice"].ToString();
        //            newdatarow["buyer_name"] = dtIARList.Rows[i]["buyer_name"].ToString();
        //            Newdt.Rows.Add(newdatarow);
        //        }
        //        #endregion

        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        //listUser是准备转换的对象
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(Newdt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        //#endregion

        //#region 編輯簽回記錄
        //public HttpResponseBase Edit()
        //{
        //    string jsonStr = String.Empty;
        //    InvoiceAllowanceRecordQuery query = new InvoiceAllowanceRecordQuery();
        //    try
        //    {
        //        _invoiceAllow = new InvoiceAllowanceRecordMgr(mySqlConnectionString);
        //        if (!string.IsNullOrEmpty(Request.Params["allowance_id"]))
        //        {//id
        //            query.allowance_id = Convert.ToUInt32(Request.Params["allowance_id"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["allowance_return"]))
        //        {//狀態
        //            query.allowance_return = Convert.ToUInt32(Request.Params["allowance_return"]);
        //        }
        //        _invoiceAllow.Edit(query);
        //        jsonStr = "{success:true}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        jsonStr = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(jsonStr.ToString());
        //    this.Response.End();
        //    return this.Response;
        //}

        //#endregion

        //#region 匯出excel
        //public void InvoiceAllowanceRecordExportToCSV()
        //{
        //    string json = string.Empty;
        //    InvoiceAllowanceRecordQuery query = new InvoiceAllowanceRecordQuery();
        //    DataTable dtIARList = new DataTable();
        //    DataTable dtIAIList;
        //    DataTable dtCsv = new DataTable();
        //    string newExcelName = string.Empty;
        //    dtCsv.Columns.Add("流水號", typeof(String));
        //    dtCsv.Columns.Add("付款單號", typeof(String));
        //    dtCsv.Columns.Add("發票號碼", typeof(String));
        //    dtCsv.Columns.Add("開立時間", typeof(String));
        //    dtCsv.Columns.Add("開立折讓時間", typeof(String));
        //    dtCsv.Columns.Add("免稅金額", typeof(String));
        //    dtCsv.Columns.Add("營業稅", typeof(String));
        //    dtCsv.Columns.Add("應稅金額", typeof(String));
        //    dtCsv.Columns.Add("購買者形態", typeof(String));
        //    dtCsv.Columns.Add("購買人", typeof(String));
        //    dtCsv.Columns.Add("統一編號", typeof(String));
        //    dtCsv.Columns.Add("公司名稱", typeof(String));
        //    dtCsv.Columns.Add("簽回狀態", typeof(String));
        //    dtCsv.Columns.Add("簽回時間", typeof(String));
        //    dtCsv.Columns.Add("狀態", typeof(String));
        //    try
        //    {
        //        _invoiceAllow = new InvoiceAllowanceRecordMgr(mySqlConnectionString);
        //        if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
        //        {//查詢條件
        //            query.seach_tj = Convert.ToInt32(Request.Params["ddlSel"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["selcontent"]))
        //        {//查詢條件內容
        //            query.content = Convert.ToInt32(Request.Params["selcontent"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddtSel"]))
        //        {//日期條件
        //            query.ddtSel = Convert.ToInt32(Request.Params["ddtSel"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["time_start"]))
        //        {//開始時間
        //            query.startdate = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["time_start"].ToString()));
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["time_end"]))
        //        {//結束時間
        //            query.enddate = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["time_end"].ToString()));
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddlstatus"]))
        //        {//開立狀態
        //            query.ddlstatus = Convert.ToInt32(Request.Params["ddlstatus"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddlinvoice"]))
        //        {//發票類型
        //            query.ddlinvoice = Convert.ToInt32(Request.Params["ddlinvoice"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["ddlqh"]))
        //        {//是否簽回
        //            query.ddlqh = Convert.ToInt32(Request.Params["ddlqh"]);
        //        }
        //        dtIARList = _invoiceAllow.InvoiceAllowanceRecordToCSV(query, out dtIAIList);
        //        DataTable Newdt = new DataTable();
        //        #region 數據轉換,dt賦值
        //        dtIAIList.Columns.Add("nobonus_money", typeof(string));
        //        dtIAIList.Columns.Add("allowance_amount_tmp", typeof(string));
        //        dtIAIList.Columns.Add("tax", typeof(string));
        //        #region 合成的新數據dt列
        //        Newdt.Columns.Add("allowance_id", typeof(string));
        //        Newdt.Columns.Add("order_id", typeof(string));
        //        Newdt.Columns.Add("invoice_number", typeof(string));
        //        Newdt.Columns.Add("invoice_date", typeof(string));
        //        Newdt.Columns.Add("allowance_date", typeof(string));
        //        Newdt.Columns.Add("allownace_total", typeof(string));
        //        Newdt.Columns.Add("allowance_tax", typeof(string));
        //        Newdt.Columns.Add("allowance_amount", typeof(string));
        //        Newdt.Columns.Add("buyer_type", typeof(string));
        //        Newdt.Columns.Add("buyer_name", typeof(string));
        //        Newdt.Columns.Add("company_invoice", typeof(string));
        //        Newdt.Columns.Add("company_title", typeof(string));
        //        Newdt.Columns.Add("allowance_return", typeof(string));
        //        Newdt.Columns.Add("allowance_return_date", typeof(string));
        //        Newdt.Columns.Add("invoice_status", typeof(string));
        //        #endregion
        //        for (int i = 0; i < dtIARList.Rows.Count; i++)
        //        {
        //            DataRow newdatarow = Newdt.NewRow();
        //            switch (dtIARList.Rows[i]["invoice_status"].ToString())
        //            {
        //                case "1":
        //                    newdatarow["invoice_status"] = "存入資料庫";
        //                    break;
        //                case "2":
        //                    newdatarow["invoice_status"] = "上傳至發票機";
        //                    break;
        //                case "3":
        //                    newdatarow["invoice_status"] = "上傳至財政部";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            switch (dtIARList.Rows[i]["buyer_type"].ToString())
        //            {
        //                case "0":
        //                    newdatarow["buyer_type"] = "個人";
        //                    break;
        //                case "1":
        //                    newdatarow["buyer_type"] = "公司行號";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            switch (dtIARList.Rows[i]["allowance_return"].ToString())
        //            {
        //                case "0":
        //                    newdatarow["allowance_return"] = "簽回";
        //                    break;
        //                case "1":
        //                    newdatarow["allowance_return"] = "未簽回";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            if (dtIARList.Rows[i]["invoice_date"].ToString() != "0" && !string.IsNullOrEmpty(dtIARList.Rows[i]["invoice_date"].ToString()))
        //            {
        //                newdatarow["invoice_date"] = CommonFunction.GetNetTime(uint.Parse(dtIARList.Rows[i]["invoice_date"].ToString())).ToString();
        //            }
        //            if (dtIARList.Rows[i]["allowance_date"].ToString() != "0" && !string.IsNullOrEmpty(dtIARList.Rows[i]["allowance_date"].ToString()))
        //            {
        //                newdatarow["allowance_date"] = CommonFunction.GetNetTime(uint.Parse(dtIARList.Rows[i]["allowance_date"].ToString())).ToString();
        //            }
        //            if (dtIARList.Rows[i]["allowance_return_date"].ToString() != "0" && !string.IsNullOrEmpty(dtIARList.Rows[i]["allowance_return_date"].ToString()))
        //            {
        //                newdatarow["allowance_return_date"] = CommonFunction.GetNetTime(uint.Parse(dtIARList.Rows[i]["allowance_return_date"].ToString())).ToString();
        //            }
        //            for (int j = 0; j < dtIAIList.Rows.Count; i++)
        //            {
        //                if (dtIAIList.Rows[j]["allowance_id"].ToString() == dtIARList.Rows[i]["allowance_id"].ToString())
        //                {
        //                    dtIAIList.Rows[j]["nobonus_money"] = (Convert.ToInt32(dtIAIList.Rows[j]["nobonus_money"]) + Convert.ToInt32(dtIARList.Rows[i]["nobonus_money"])).ToString();
        //                    dtIAIList.Rows[j]["allowance_amount_tmp"] = (Convert.ToInt32(dtIAIList.Rows[j]["nobonus_money"]) / (1 + 0.05)).ToString();
        //                    dtIAIList.Rows[j]["tax"] = (Convert.ToInt32(dtIAIList.Rows[j]["nobonus_money"]) - Convert.ToInt32(dtIAIList.Rows[j]["allowance_amount_tmp"])).ToString();
        //                }
        //                newdatarow["allownace_total"] = Convert.ToInt32(dtIARList.Rows[i]["allownace_total"]) + Convert.ToInt32(dtIAIList.Rows[j]["nobonus_money"]);
        //                newdatarow["allowance_amount"] = Convert.ToInt32(dtIARList.Rows[i]["allowance_amount"]) + Convert.ToInt32(dtIAIList.Rows[j]["allowance_amount_tmp"]);
        //                newdatarow["allowance_tax"] = Convert.ToInt32(dtIARList.Rows[i]["allowance_tax"]) + Convert.ToInt32(dtIAIList.Rows[j]["tax"]);
        //            }
        //            newdatarow["allowance_id"] = dtIARList.Rows[i]["allowance_id"].ToString();
        //            newdatarow["order_id"] = dtIARList.Rows[i]["order_id"].ToString();
        //            newdatarow["buyer_name"] = dtIARList.Rows[i]["buyer_name"].ToString();
        //            newdatarow["invoice_number"] = dtIARList.Rows[i]["invoice_number"].ToString();
        //            newdatarow["company_title"] = dtIARList.Rows[i]["company_title"].ToString();
        //            newdatarow["company_invoice"] = dtIARList.Rows[i]["company_invoice"].ToString();
        //            Newdt.Rows.Add(newdatarow);
        //        }
        //        #endregion

        //        #region 遍歷整合後的數據到exceldatatable
        //        for (int i = 0; i < Newdt.Rows.Count; i++)
        //        {
        //            DataRow newrow = dtCsv.NewRow();
        //            newrow[0] = Newdt.Rows[i]["allowance_id"];
        //            newrow[1] = Newdt.Rows[i]["order_id"];
        //            newrow[2] = Newdt.Rows[i]["invoice_number"];
        //            newrow[3] = Newdt.Rows[i]["invoice_date"];
        //            newrow[4] = Newdt.Rows[i]["allowance_date"];
        //            newrow[5] = Newdt.Rows[i]["allownace_total"];
        //            newrow[6] = Newdt.Rows[i]["allowance_tax"];
        //            newrow[7] = Newdt.Rows[i]["allowance_amount"];
        //            newrow[8] = Newdt.Rows[i]["buyer_type"];
        //            newrow[9] = Newdt.Rows[i]["buyer_name"];
        //            newrow[10] = Newdt.Rows[i]["company_invoice"];
        //            newrow[11] = Newdt.Rows[i]["company_title"];
        //            newrow[12] = Newdt.Rows[i]["allowance_return"];
        //            newrow[13] = Newdt.Rows[i]["allowance_return_date"];
        //            newrow[14] = Newdt.Rows[i]["invoice_status"];
        //            dtCsv.Rows.Add(newrow);
        //        }
        //        #endregion
        //        string filename = "invoice_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
        //        newExcelName = Server.MapPath(excelPath) + filename;
        //        if (System.IO.File.Exists(newExcelName))
        //        {
        //            //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
        //            System.IO.File.SetAttributes(newExcelName, FileAttributes.Normal);
        //            System.IO.File.Delete(newExcelName);
        //        }
        //        StringWriter sw = ExcelHelperXhf.SetCsvFromData(dtCsv, newExcelName);
        //        Response.Clear();
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + "invoice__" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
        //        Response.ContentType = "application/ms-excel";
        //        Response.ContentEncoding = Encoding.Default;
        //        Response.Write(sw);
        //        Response.End();

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "false";
        //    }
        //}

        //#endregion

        //#region 折讓發票明細
        //public HttpResponseBase GetInvoiceDetail()
        //{
        //    string jsonStr = String.Empty;
        //    InvoiceAllowanceRecordQuery query = new InvoiceAllowanceRecordQuery();
        //    DataTable dtInvoiceDetail = new DataTable();
        //    try
        //    {
        //        _invoiceAllow = new InvoiceAllowanceRecordMgr(mySqlConnectionString);
        //        if (!string.IsNullOrEmpty(Request.Params["allowance_id"]))
        //        {//id
        //            query.allowance_id = Convert.ToUInt32(Request.Params["allowance_id"]);
        //        }
        //        dtInvoiceDetail = _invoiceAllow.GetInvoiceAllowanceInfoDeatil(query.allowance_id.ToString());
        //        jsonStr = "{success:true,data:" + JsonConvert.SerializeObject(dtInvoiceDetail, Formatting.Indented) + "}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        jsonStr = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(jsonStr.ToString());
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion

        //#endregion

        //#region 現金,外站,貨到付款對賬列表

        //#region  泛用對賬列表 + HttpResponseBase GetOBCFanYongList()
        //public HttpResponseBase GetOBCFanYongList()
        //{
        //    string json = string.Empty;
        //    OrderMasterQuery query = new OrderMasterQuery();
        //    List<OrderMasterQuery> omq = new List<OrderMasterQuery>();
        //    _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
        //    int totalCount = 0;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
        //        {
        //            int id = int.Parse(Request.Params["selecttype"]);
        //            if (id == 2)
        //            {
        //                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
        //                {
        //                    query.Order_Id = uint.Parse(Request.Params["searchcon"]);
        //                }
        //            }
        //            else
        //            {
        //                query.Order_Id = 0;
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["timeone"]))
        //        {
        //            int res = int.Parse(Request.Params["timeone"]);
        //            if (res == 2)
        //            {
        //                if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
        //                {
        //                    query.OrderCreateDate = DateTime.Parse(Request.Params["dateOne"]);
        //                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //                    query.Order_Createdate = (uint)(query.OrderCreateDate - startTime).TotalSeconds;
        //                }
        //                if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
        //                {
        //                    DateTime dt = DateTime.Parse(Request.Params["dateTwo"]);
        //                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //                    query.Order_Updatedate = (uint)(dt - startTime).TotalSeconds;
        //                }
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["billing_type"]))
        //        {
        //            query.billing_check = int.Parse(Request.Params["billing_type"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["payment"]))
        //        {
        //            query.Order_Payment = uint.Parse(Request.Params["payment"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["channel"]))
        //        {
        //            query.Channel = uint.Parse(Request.Params["channel"]);
        //        }
        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
        //        omq = _orderMasterMgr.GetOBCList(query, out totalCount);
        //        for (int i = 0; i < omq.Count; i++)
        //        {
        //            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //            long ITime = long.Parse(omq[i].Order_Createdate + "0000000");
        //            TimeSpan toNow = new TimeSpan(ITime);
        //            omq[i].OrderCreateDate = dtStart.Add(toNow);
        //            //omq[i].OrderCreateDate= omq[i].Order_Createdate;
        //        }
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(omq, Formatting.Indented, timeConverter) + "}";
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
        //#endregion

        //#region 賣場store + HttpResponseBase GetOrderBillingChannelList()
        //public HttpResponseBase GetOrderBillingChannelList()
        //{
        //    string json = string.Empty;
        //    Channel query = new Channel();
        //    _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
        //    List<Channel> channels = new List<Channel>();
        //    try
        //    {
        //        channels = _orderMasterMgr.GetChannelList(query);
        //        Channel query1 = new Channel();
        //        query1.channel_name_simple = "全部";
        //        channels.Insert(0, query1);
        //        json = "{success:true,data:" + JsonConvert.SerializeObject(channels, Formatting.Indented) + "}";
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
        //#endregion

        //#region 付款方式store + HttpResponseBase OrderBillingPaymentList()
        //public HttpResponseBase OrderBillingPaymentList()
        //{
        //    string json = string.Empty;
        //    //ChannelModel query = new OrderMasterQuery();
        //    OrderMasterQuery query = new OrderMasterQuery();
        //    _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
        //    Parametersrc par = new Parametersrc();
        //    List<Parametersrc> _dt = new List<Parametersrc>();
        //    try
        //    {
        //        _dt = _orderMasterMgr.GetPaymentList(query);
        //        par.parameterName = "所有方式";
        //        par.ParameterCode = "0";
        //        //par.ParameterCode = "0";
        //        _dt.Insert(0, par);
        //        json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";
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
        //#endregion

        //#region 中信紅利對賬list + HttpResponseBase GetOBCHongLiList()
        //public HttpResponseBase GetOBCHongLiList()
        //{
        //    string json = string.Empty;
        //    OrderPaymentCt query = new OrderPaymentCt();
        //    List<OrderPaymentCt> opc = new List<OrderPaymentCt>();
        //    _iopcMgr = new OrderPaymentCtMgr(mySqlConnectionString);
        //    int totalCount = 0;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["selectcheck"]))
        //        {
        //            query.check = uint.Parse(Request.Params["selectcheck"]);
        //        }
        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
        //        opc = _iopcMgr.GetOPCList(query, out totalCount);
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(opc, Formatting.Indented) + "}";
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
        //#endregion

        //#region 中信對賬總結 + HttpResponseBase OrderPaymentCtinto()
        //public HttpResponseBase OrderPaymentCtinto()
        //{
        //    string json = string.Empty;
        //    DTExcel.Clear();
        //    DTExcel.Columns.Clear();
        //    string newName = string.Empty;
        //    DTExcel.Columns.Add("付款單號", typeof(String));
        //    DTExcel.Columns.Add("不能匯入的原因", typeof(String));
        //    int count = 0;//總匯入數
        //    int errorcount = 0;
        //    int bucunzaicount = 0;
        //    int defaultcount = 0;
        //    StringBuilder strsql = new StringBuilder();
        //    try
        //    {
        //        if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)
        //        {
        //            HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];
        //            //FileManagement fileManagement = new FileManagement();
        //            newName = Server.MapPath(excelPath) + excelFile.FileName;
        //            excelFile.SaveAs(newName);
        //            DataTable dt = new DataTable();
        //            dt = CsvHelper.ReadCsvToDataTable(newName, true);
        //            _iopcMgr = new OrderPaymentCtMgr(mySqlConnectionString);
        //            OrderPaymentCt opc = new OrderPaymentCt();
        //            if (dt.Rows.Count > 0)
        //            {
        //                count = dt.Rows.Count;
        //                foreach (DataRow dr in dt.Rows)
        //                {
        //                    try
        //                    {
        //                        int a = Convert.ToInt32(dr[0]);//付款單號
        //                        opc.lidm = a;
        //                        int b = Convert.ToInt32(dr[1]);//付款單金額
        //                        int c = Convert.ToInt32(dr[2]);//付款單號
        //                        int d = Convert.ToInt32(dr[3]);//付款單金額
        //                        DataTable _lidmlist = new DataTable();
        //                        _lidmlist = _iopcMgr.CheckLidm(opc);
        //                        if (_lidmlist.Rows.Count > 0)//大於1表示該數據庫內存在該值
        //                        {
        //                            if (int.Parse(_lidmlist.Rows[0]["originalamt"].ToString()) != b)
        //                            {
        //                                DataRow drtwo = DTExcel.NewRow();
        //                                drtwo[0] = dr[0].ToString();
        //                                drtwo[1] = "該行數據金額錯誤!";
        //                                DTExcel.Rows.Add(drtwo);
        //                                defaultcount++;

        //                            }
        //                            else if (int.Parse(_lidmlist.Rows[0]["offsetamt"].ToString()) != c)
        //                            {
        //                                DataRow drtwo = DTExcel.NewRow();
        //                                drtwo[0] = dr[0].ToString();
        //                                drtwo[1] = "該行數據金額錯誤!";
        //                                DTExcel.Rows.Add(drtwo);
        //                                defaultcount++;
        //                            }
        //                            else if (int.Parse(_lidmlist.Rows[0]["utilizedpoint"].ToString()) != d)
        //                            {
        //                                DataRow drtwo = DTExcel.NewRow();
        //                                drtwo[0] = dr[0].ToString();
        //                                drtwo[1] = "該行數據金額錯誤!";
        //                                DTExcel.Rows.Add(drtwo);
        //                                defaultcount++;

        //                            }

        //                        }
        //                        else//當數據不存在時進行添加數據
        //                        {
        //                            DataRow drtwo = DTExcel.NewRow();
        //                            drtwo[0] = dr[0].ToString();
        //                            drtwo[1] = "數據庫中不存在該條數據";
        //                            DTExcel.Rows.Add(drtwo);
        //                            bucunzaicount++;

        //                        }

        //                    }
        //                    catch
        //                    {
        //                        DataRow drtwo = DTExcel.NewRow();
        //                        drtwo[0] = dr[0].ToString();
        //                        drtwo[1] = dr[1].ToString();
        //                        drtwo[2] = "數據異常";
        //                        DTExcel.Rows.Add(drtwo);
        //                        errorcount++;

        //                    }
        //                }
        //                json = "{success:true,total:'" + count + "',error:'" + errorcount + "',repeat:'" + bucunzaicount + "',defaultcount:'" + defaultcount + "'}";
        //            }
        //            else
        //            {
        //                json = "{success:true,total:" + 0 + ",error:" + 0 + ",repeat:" + 0 + ",defaultcount:" + 0 + "}";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        newName = string.Empty;
        //        DTExcel.Clear();
        //        DTExcel.Columns.Clear();
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,data:" + "" + "}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json.ToString());
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion

        //#region 匯入Excel + HttpResponseBase ReportOrderBillingExcel()
        //public HttpResponseBase ReportOrderBillingExcel()
        //{
        //    string json = string.Empty;//json字符串
        //    OrderMasterQuery query = new OrderMasterQuery();
        //    _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
        //    int successcount = 0;
        //    int failcount = 0;
        //    int totalCount = 0;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["mcdate"]))
        //        {
        //            DateTime dt = DateTime.Parse(Request.Params["mcdate"].ToString());
        //            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //            query.Money_Collect_Date = (int)(dt - startTime).TotalSeconds;
        //        }
        //        else
        //        {
        //            DateTime dt = DateTime.Now;
        //            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //            query.Money_Collect_Date = (int)(dt - startTime).TotalSeconds;
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["isOrderchecked"]))
        //        {
        //            int res = int.Parse(Request.Params["isOrderchecked"].ToString());
        //            if (res == 1)
        //            {
        //                query.billing_check = -1;
        //            }
        //            else
        //            {
        //                query.billing_check = 0;
        //            }
        //        }
        //        if (Request.Files["ImportFileMsg1"] != null && Request.Files["ImportFileMsg1"].ContentLength > 0)//判斷文件是否為空
        //        {
        //            HttpPostedFileBase excelFile = Request.Files["ImportFileMsg1"];//獲取文件流
        //            FileManagement fileManagement = new FileManagement();//實例化 FileManagement
        //            string fileLastName = excelFile.FileName;
        //            string newExcelName = Server.MapPath(excelPath) + "OrderBilling錯誤數據" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名
        //            excelFile.SaveAs(newExcelName);//上傳文件
        //            DataTable dt = new DataTable();
        //            string file = newExcelName.Substring(newExcelName.LastIndexOf(".") + 1);
        //            DataTable _dt = new DataTable();
        //            if (file == "xls" || file == "xlsx")
        //            {
        //                NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
        //                dt = helper.SheetData();
        //            }
        //            if (file == "csv")
        //            {
        //                _dt = CsvHelper.ReadCsvToDataTable(newExcelName, true);
        //                dt = _dt;
        //            }
        //            DataRow[] dr = dt.Select(); //定义一个DataRow数组,读取ds里面所有行
        //            int rowsnum = dt.Rows.Count;
        //            if (rowsnum > 0)//判斷是否是這個表
        //            {
        //                DeliverMasterQuery dmQuery = new DeliverMasterQuery();
        //                StringBuilder str = new StringBuilder();
        //                DataTable dtMaster = new DataTable();
        //                string filenameExcel = string.Empty;
        //                DTExcel1.Clear();
        //                DTExcel1.Columns.Clear();
        //                DTExcel1 = new DataTable();
        //                DTExcel1.Columns.Add("付款單號", typeof(String));
        //                DTExcel1.Columns.Add("錯誤信息", typeof(String));
        //                DTExcel1.Columns.Add("應收", typeof(String));
        //                DTExcel1.Columns.Add("實收", typeof(String));
        //                int estart = 0;
        //                int eend = 0;
        //                string create_dtim = CommonFunction.DateTimeToString(DateTime.Now);       //創建時間
        //                int create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
        //                //#region 循環excel表中的數據 并判斷是否滿足條件和失敗的個數
        //                for (int z = 0; z < dr.Length; z++)
        //                {
        //                    //string orderId = dr[z + 1][4].ToString();
        //                    //string date = dr[z + 1][0].ToString();
        //                    //string orderAmount = dr[z + 1][5].ToString();
        //                    if (dr[z][0].ToString() == "出貨日期" && dr[z][4].ToString() == "訂單號碼" && dr[z][5].ToString() == "應收金額")
        //                    {
        //                        estart = z;
        //                    }
        //                    if (dr[z][0].ToString() == "筆數：" && dr[z][4].ToString() == "合計：")
        //                    {
        //                        eend = z;
        //                    }
        //                    //if (eend > 0)
        //                    //{
        //                    //    break;
        //                    //}
        //                    if (estart < eend && estart > 0)
        //                    {
        //                        for (int j = estart + 1; j < eend; j++)
        //                        {
        //                            if (!string.IsNullOrEmpty(dr[j][4].ToString()))
        //                            {
        //                                query.OrderId = uint.Parse(dr[j][4].ToString());
        //                                query.Order_Amount = uint.Parse(dr[j][5].ToString());
        //                                query.Money_Cancel = uint.Parse(dr[j][6].ToString());
        //                                int result = _orderMasterMgr.CheckedImport(query).Rows.Count;
        //                                if (result <= 0)
        //                                {
        //                                    DataRow drtwo = DTExcel1.NewRow();
        //                                    drtwo[0] = dr[j][4].ToString();
        //                                    drtwo[1] = "該行數據金額錯誤!";
        //                                    drtwo[2] = dr[j][5].ToString();
        //                                    drtwo[3] = dr[j][5].ToString();
        //                                    DTExcel1.Rows.Add(drtwo);
        //                                    failcount++;
        //                                }
        //                                else
        //                                {
        //                                    query.Billing_Checked = true;
        //                                    _orderMasterMgr.UpdateOrderBilling(query);
        //                                    successcount++;
        //                                }
        //                                totalCount++;
        //                            }
        //                        }
        //                        eend = 0;
        //                        estart = 0;
        //                    }
        //                }
        //                DataRow drnew = DTExcel1.NewRow();
        //                drnew[0] = "成功" + successcount + "條數據!";
        //                drnew[1] = "失敗" + failcount + "條數據!";
        //                drnew[2] = "共處理" + totalCount + "條數據!";
        //                drnew[3] = "";
        //                DTExcel1.Rows.Add(drnew);
        //                //string fileName = "泛用匯入對賬錯誤數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
        //                //string newFileName = Server.MapPath(excelPath) + fileName;
        //                //string[] columnName = { "付款單號", "錯誤信息", "應收", "實收"};
        //                //CsvHelper.ExportDataTableToCsv(DTExcel1, newFileName,columnName, true);
        //                json = "{success:true,total:'" + totalCount + "',error:'" + failcount + "',success:'" + successcount + "'}";
        //            }
        //            else
        //            {
        //                json = "{success:true,total:0,error:'',success:''}";
        //            }
        //        }
        //        else
        //        {
        //            json = "{success:true,total:'-1',error:'',success:''}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,data:" + "" + "}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json.ToString());
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion

        //#region 泛用對賬匯出信息
        //public void UpdownmessageofFanyong()
        //{
        //    string json = string.Empty;
        //    try
        //    {
        //        StringWriter sw = ExcelHelperXhf.SetCsvFromData(DTExcel1, "泛用匯入對賬錯誤數據.csv");
        //        Response.Clear();
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + "泛用匯入對賬錯誤數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
        //        Response.ContentType = "application/ms-excel";
        //        Response.ContentEncoding = Encoding.Default;
        //        Response.Write(sw);
        //        Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,data:" + "" + "}";
        //    }
        //}
        //#endregion

        //#region 中信對賬匯出信息
        //public void Updownmessage()
        //{
        //    string json = string.Empty;
        //    try
        //    {
        //        StringWriter sw = ExcelHelperXhf.SetCsvFromData(DTExcel, "目前不符合的數據.csv");
        //        Response.Clear();
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + "目前不符合的數據_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
        //        Response.ContentType = "application/ms-excel";
        //        Response.ContentEncoding = Encoding.Default;
        //        Response.Write(sw);
        //        Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,data:" + "" + "}";
        //    }
        //}
        //#endregion

        //#region 匯出Excel信息
        //public void ReportManagementExcelList()
        //{
        //    string json = string.Empty;
        //    OrderMasterQuery query = new OrderMasterQuery();
        //    List<OrderMasterQuery> omq = new List<OrderMasterQuery>();
        //    _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
        //    DataTable _dt = new DataTable();
        //    DataTable dtHZ = new DataTable();
        //    try
        //    {
        //        string newExcelName = string.Empty;
        //        dtHZ.Columns.Add("付款單號", typeof(String));
        //        dtHZ.Columns.Add("付款方式", typeof(String));
        //        dtHZ.Columns.Add("付款金額", typeof(String));
        //        dtHZ.Columns.Add("取消金額", typeof(String));
        //        dtHZ.Columns.Add("退貨金額", typeof(String));
        //        dtHZ.Columns.Add("應收金額", typeof(String));
        //        dtHZ.Columns.Add("對賬狀態", typeof(String));
        //        dtHZ.Columns.Add("訂購時間", typeof(String));
        //        dtHZ.Columns.Add("確認對賬日期", typeof(String));
        //        if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
        //        {
        //            int id = int.Parse(Request.Params["selecttype"]);
        //            if (id == 2)
        //            {
        //                if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
        //                {
        //                    query.Order_Id = uint.Parse(Request.Params["searchcon"]);
        //                }
        //            }
        //            else
        //            {
        //                query.Order_Id = 0;
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["timeone"]))
        //        {
        //            int res = int.Parse(Request.Params["timeone"]);
        //            if (res == 2)
        //            {
        //                if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
        //                {
        //                    query.OrderCreateDate = DateTime.Parse(Request.Params["dateOne"]);
        //                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //                    query.Order_Createdate = (uint)(query.OrderCreateDate - startTime).TotalSeconds;
        //                }
        //                if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
        //                {
        //                    DateTime dt = DateTime.Parse(Request.Params["dateTwo"]);
        //                    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //                    query.Order_Updatedate = (uint)(dt - startTime).TotalSeconds;
        //                }
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["billing_type"]))
        //        {
        //            query.billing_check = int.Parse(Request.Params["billing_type"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["payment"]))
        //        {
        //            query.Order_Payment = uint.Parse(Request.Params["payment"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["channel"]))
        //        {
        //            query.Channel = uint.Parse(Request.Params["channel"]);
        //        }
        //        _dt = _orderMasterMgr.ReportOrderBillingExcel(query);



        //        if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
        //        {
        //            System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
        //        }
        //        for (int i = 0; i < _dt.Rows.Count; i++)
        //        {
        //            DataRow dr = dtHZ.NewRow();
        //            dr[0] = _dt.Rows[i]["order_id"];
        //            dr[1] = _dt.Rows[i]["order_pay_message"];
        //            dr[2] = _dt.Rows[i]["Order_Amount"];
        //            dr[3] = _dt.Rows[i]["Money_Cancel"];
        //            dr[4] = _dt.Rows[i]["Money_Return"];
        //            dr[5] = (int.Parse(_dt.Rows[i]["Order_Amount"].ToString()) - (int.Parse(_dt.Rows[i]["Money_Cancel"].ToString()) + int.Parse(_dt.Rows[i]["Money_Return"].ToString()))).ToString();
        //            if ((bool)_dt.Rows[i]["billing_checked"])
        //            {
        //                dr[6] = "是";
        //            }
        //            else
        //            {
        //                dr[6] = "否";
        //            }
        //            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //            long ITime = long.Parse(_dt.Rows[i]["order_createdate"] + "0000000");
        //            TimeSpan toNow = new TimeSpan(ITime);
        //            long ITime1 = long.Parse(_dt.Rows[i]["money_collect_date"] + "0000000");
        //            TimeSpan toNow1 = new TimeSpan(ITime1);
        //            dr[7] = dtStart.Add(toNow).ToString();
        //            dr[8] = dtStart.Add(toNow1).ToString();
        //            dtHZ.Rows.Add(dr);
        //        }
        //        if (dtHZ.Rows.Count > 0)
        //        {
        //            string fileName = "現金,外站,貨到付款對賬列表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
        //            MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "現金,外站,貨到付款對賬列表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")");
        //            Response.AddHeader("Content-Disposition", "attachment; filename='" + fileName + "'");
        //            Response.BinaryWrite(ms.ToArray());
        //        }
        //        else
        //        {
        //            Response.Write("匯出數據不存在");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //    }
        //}
        //#endregion

        //#region 修改確認對賬狀態
        //public HttpResponseBase UpdateOrderBillingCheck()
        //{
        //    string json = string.Empty;
        //    OrderMasterQuery query = new OrderMasterQuery();
        //    _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["orderid"]))
        //        {
        //            query.Order_Id = uint.Parse(Request.Params["orderid"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["mcdate"]))
        //        {
        //            DateTime dt = DateTime.Parse(Request.Params["mcdate"]);
        //            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //            query.Money_Collect_Date = (int)(dt - startTime).TotalSeconds;
        //        }
        //        else
        //        {
        //            DateTime dt = DateTime.Now;
        //            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //            query.Money_Collect_Date = (int)(dt - startTime).TotalSeconds;
        //        }
        //        query.Billing_Checked = true;
        //        _orderMasterMgr.UpdateOrderBilling(query);
        //        json = "{success:true}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion

        //#region 批次對賬 + HttpResponseBase BatchOBC()
        //public HttpResponseBase BatchOBC()
        //{
        //    string json = string.Empty;
        //    OrderMasterQuery query = new OrderMasterQuery();
        //    _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
        //    string id = string.Empty;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["loc_id"]))
        //        {
        //            id = Request.Params["loc_id"].ToString();
        //        }
        //        string[] ids = id.Split(',');
        //        for (int i = 0; i < ids.Length - 1; i++)
        //        {
        //            query.OrderId = uint.Parse(ids[i].ToString());
        //            DateTime dt = DateTime.Now;
        //            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //            query.Money_Collect_Date = (int)(dt - startTime).TotalSeconds;
        //            query.Billing_Checked = true;
        //            _orderMasterMgr.UpdateOrderBilling(query);
        //        }
        //        json = "{success:true}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion
        //#endregion

        //#region 開立發票列表
        /////*獲取發票列表*/
        //#region 獲取發票列表 + HttpResponseBase GetInvoiceList()
        ///// <summary>
        ///// 獲取發票列表
        ///// </summary>
        ///// <returns></returns>
        //public HttpResponseBase GetInvoiceList()
        //{
        //    List<InvoiceMasterRecordQuery> stores = new List<InvoiceMasterRecordQuery>();
        //    DataTable dt = new DataTable();
        //    //_zipMgr = new ZipMgr(mySqlConnectionString);
        //    string json = string.Empty;
        //    try
        //    {
        //        int totalCount = 0;
        //        InvoiceMasterRecordQuery query = new InvoiceMasterRecordQuery();
        //        StringBuilder sqlwhere = new StringBuilder();
        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
        //        //查詢條件
        //        if (!string.IsNullOrEmpty(Request.Params["ven_type"]) && !string.IsNullOrEmpty(Request.Params["search_content"]))
        //        {
        //            string search_content = Request.Params["search_content"].Trim();
        //            string value = Request.Params["ven_type"];
        //            switch (value)
        //            {
        //                case "1":
        //                    sqlwhere.AppendFormat(@" and imr.order_id='{0}' ", search_content);
        //                    break;
        //                case "2":
        //                    sqlwhere.AppendFormat(@" and imr.company_title like '%{0}%' ", search_content);
        //                    break;
        //                case "3":
        //                    sqlwhere.AppendFormat(@" and imr.company_invoice ='{0}'", search_content);
        //                    break;
        //                case "4":
        //                    sqlwhere.AppendFormat(@" and imr.invoice_number='{0}'", search_content);
        //                    break;
        //                case "5":
        //                    sqlwhere.AppendFormat(@" and imr.buyer_name like '%{0}%' ", search_content);
        //                    break;
        //                default:
        //                    break;
        //            }

        //        }
        //        //日期條件
        //        if (!string.IsNullOrEmpty(Request.Params["date_type"]) && !string.IsNullOrEmpty(Request.Params["time_start"]) && !string.IsNullOrEmpty(Request.Params["time_end"]))
        //        {
        //            uint time_start = (uint)CommonFunction.GetPHPTime(Request.Params["time_start"]);
        //            uint time_end = (uint)CommonFunction.GetPHPTime(Request.Params["time_end"].Substring(0, 10) + " 23:59:59");
        //            string value = Request.Params["date_type"];
        //            switch (value)
        //            {
        //                case "1":
        //                    sqlwhere.AppendFormat(@"  and imr.invoice_date >={0} and invoice_date <={1}", time_start, time_end);
        //                    break;
        //                case "2":
        //                    sqlwhere.AppendFormat(@"  and imr.print_post_createdate >={0}  and print_post_createdate <={1}", time_start, time_end);
        //                    break;
        //                case "3":
        //                    sqlwhere.AppendFormat(@"  and imr.status_createdate >={0} and imr.status_createdate <={1}", time_start, time_end);
        //                    break;
        //            }

        //        }
        //        //開立狀態
        //        if (!string.IsNullOrEmpty(Request.Params["invoice_status"]) && Request.Params["invoice_status"] != "-1")
        //        {
        //            sqlwhere.AppendFormat(@" and imr.invoice_status ={0}", Request.Params["invoice_status"]);
        //        }
        //        //發票類型
        //        if (!string.IsNullOrEmpty(Request.Params["invoice_type"]) && Request.Params["invoice_type"] != "-1")
        //        {
        //            sqlwhere.AppendFormat(@" and imr.buyer_type={0} ", Request.Params["invoice_type"]);
        //        }
        //        //是否寄出發票
        //        if (!string.IsNullOrEmpty(Request.Params["is_send"]) && Request.Params["is_send"] != "-1")
        //        {
        //            string value = Request.Params["is_send"];
        //            sqlwhere.AppendFormat(@" and ( om.paper_invoice = 1 or imr.buyer_type= 1 ) ");
        //            switch (value)
        //            {
        //                case "0":
        //                    sqlwhere.AppendFormat(@" and imr.print_post_createdate =  0 ");
        //                    break;
        //                case "1":
        //                    sqlwhere.AppendFormat(@" and imr.print_post_createdate !=0 ");
        //                    break;
        //            }

        //        }
        //        //發票屬性
        //        if (!string.IsNullOrEmpty(Request.Params["invoice_attribute"]) && Request.Params["invoice_attribute"] != "-1")
        //        {
        //            string value = Request.Params["invoice_attribute"];
        //            if (value == "1" || value == "4")
        //            {
        //                sqlwhere.AppendFormat(@" and imr.invoice_attribute IN (1,4) ");
        //            }
        //            else
        //            {
        //                sqlwhere.AppendFormat(@" and  imr.invoice_attribute ={0} ", value);
        //            }
        //        }
        //        query.sqlwhere = sqlwhere.ToString();
        //        imrMgr = new InvoiceMasterRecordMgr(mySqlConnectionString);
        //        dt = imrMgr.GetInvoiceList(query, out totalCount);
        //        for (int i = 0; i < dt.Rows.Count; i++)
        //        {
        //            dt.Rows[i]["aorder_address"] = CommonFunction.ZipAddress(dt.Rows[i]["order_zip"].ToString()) + dt.Rows[i]["order_address"].ToString();
        //        }
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        //listUser是准备转换的对象
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter) + "}";//返回json數據
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:true,totalCount:0,data:[]}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        ///// <summary>
        ///// 修改發票的地址信息
        ///// </summary>
        ///// <returns></returns>
        //public HttpResponseBase SaveAddress()
        //{
        //    /********************************************************/
        //    int n = 0;
        //    string json = string.Empty;
        //    try
        //    {
        //        InvoiceMasterRecord imr = new InvoiceMasterRecord();
        //        if (!string.IsNullOrEmpty(Request.Params["invoice_id"]))
        //        {
        //            imr.invoice_id = Convert.ToUInt32(Request.Params["invoice_id"]);
        //        }
        //        imr.company_title = Request.Params["company_title"];
        //        imr.company_invoice = Request.Params["company_invoice"];
        //        if (!string.IsNullOrEmpty(Request.Params["order_zip"]))
        //        {
        //            imr.order_zip = uint.Parse(Request.Params["order_zip"]);
        //        }
        //        imr.order_address = Request.Params["order_address"];

        //        imrMgr = new InvoiceMasterRecordMgr(mySqlConnectionString);
        //        int i = imrMgr.Update(imr);
        //        if (i > 0)
        //        {
        //            json = "{success:true}";//返回json數據
        //        }
        //        else
        //        {
        //            json = "{success:false}";//返回json數據
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion
        //#region 獲取地址下拉列表
        //public HttpResponseBase GetZipAddress()
        //{
        //    List<Zip> store = new List<Zip>();
        //    string json = string.Empty;
        //    try
        //    {
        //        imrMgr = new InvoiceMasterRecordMgr(mySqlConnectionString);
        //        Zip zip = new Zip();
        //        store = imrMgr.GetZipAddress(zip);
        //        zip = new Zip();
        //        zip.zipcode = "";
        //        zip.middle = "請選擇";
        //        store.Insert(0, zip);
        //        json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,data:[]}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}

        //#endregion
        //#region 匯出文檔
        //public void InvoiceExportToCSV()
        //{
        //    string json = string.Empty;
        //    InvoiceMasterRecordQuery imrq = new InvoiceMasterRecordQuery();
        //    int totalCount = 0;
        //    DataTable dt = new DataTable();
        //    DataTable dtCsv = new DataTable();
        //    string newExcelName = string.Empty;
        //    //dtHZ.Columns.Add("流水號", typeof(String));
        //    //dtHZ.Columns.Add("付款單號", typeof(String));
        //    //dtHZ.Columns.Add("付款時間", typeof(String));
        //    //dtHZ.Columns.Add("付款方式", typeof(String));
        //    //dtHZ.Columns.Add("虛擬帳號", typeof(String));
        //    //dtHZ.Columns.Add("發票狀態", typeof(String));
        //    //dtHZ.Columns.Add("發票號碼", typeof(String));
        //    //dtHZ.Columns.Add("發票號碼2", typeof(String));
        //    //dtHZ.Columns.Add("開立日期", typeof(String));
        //    //dtHZ.Columns.Add("免稅額", typeof(String));
        //    //dtHZ.Columns.Add("營業稅", typeof(String));
        //    //dtHZ.Columns.Add("應稅額", typeof(String));
        //    //dtHZ.Columns.Add("購買者屬性", typeof(String));
        //    //dtHZ.Columns.Add("姓名", typeof(String));
        //    //dtHZ.Columns.Add("統一編號", typeof(String));
        //    //dtHZ.Columns.Add("公司名稱", typeof(String));
        //    //dtHZ.Columns.Add("郵政區號", typeof(String));
        //    //dtHZ.Columns.Add("寄送地址", typeof(String));
        //    //dtHZ.Columns.Add("列印日期", typeof(String));
        //    //dtHZ.Columns.Add("產生時間", typeof(String));
        //    //dtHZ.Columns.Add("備註" , typeof(String));
        //    try
        //    {
        //        imrMgr = new InvoiceMasterRecordMgr(mySqlConnectionString);
        //        imrq.IsPage = false;
        //        dt = imrMgr.GetInvoiceList(imrq, out totalCount);
        //        dtCsv = dt.DefaultView.ToTable(false, new string[] { "invoice_id", "order_id","order_date_pay","order_payment","sinopac_id","invoice_status",
        //            "invoice_number","invoice_number2","invoice_date","free_tax","tax_amount","sales_amount",
        //            "buyer_type","buyer_name","company_invoice","company_title","order_zip","order_address",
        //            "print_post_createdate","status_createdate","invoice_note" });
        //        for (int i = 0; i < dtCsv.Rows.Count; i++)
        //        {
        //            switch (dtCsv.Rows[i]["invoice_status"].ToString())
        //            {
        //                case "1":
        //                    dtCsv.Rows[i]["invoice_status"] = "存入資料庫";
        //                    break;
        //                case "2":
        //                    dtCsv.Rows[i]["invoice_status"] = "上傳至發票機";
        //                    break;
        //                case "3":
        //                    dtCsv.Rows[i]["invoice_status"] = "上傳至財政部";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            switch (dtCsv.Rows[i]["buyer_type"].ToString())
        //            {
        //                case "0":
        //                    dtCsv.Rows[i]["buyer_type"] = "個人";
        //                    break;
        //                case "1":
        //                    dtCsv.Rows[i]["buyer_type"] = "公司行號";
        //                    break;
        //                default:
        //                    break;
        //            }
        //            dtCsv.Rows[i]["order_zip"] = CommonFunction.ZipAddress(dtCsv.Rows[i]["order_zip"].ToString()); ;

        //        }
        //        string[] colname ={"流水號", "付款單號", "付款時間","付款方式","虛擬帳號","發票狀態",
        //                           "發票號碼","發票號碼2", "開立日期", "免稅額", "營業稅", "應稅額", 
        //                           "購買者屬性", "姓名", "統一編號", "公司名稱", "郵遞區號", "寄送地址", "列印日期", "產生時間", "備註"};
        //        string filename = "invoice_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
        //        newExcelName = Server.MapPath(excelPath) + filename;
        //        if (System.IO.File.Exists(newExcelName))
        //        {
        //            //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
        //            System.IO.File.SetAttributes(newExcelName, FileAttributes.Normal);
        //            System.IO.File.Delete(newExcelName);
        //        }
        //        StringWriter sw = ExcelHelperXhf.SetCsvFromData(dtCsv, filename);
        //        Response.Clear();
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(filename));
        //        Response.ContentType = "application/ms-excel";
        //        Response.ContentEncoding = Encoding.Default;
        //        Response.Write(sw);
        //        Response.End();

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "false";
        //    }
        //}
        //public void ExportToTXT()
        //{
        //    InvoiceMasterRecordQuery query = new InvoiceMasterRecordQuery();
        //    string json = string.Empty;
        //    StringWriter sw = new StringWriter();
        //    if (!string.IsNullOrEmpty(Request.Params["time_start"]) && !string.IsNullOrEmpty(Request.Params["time_end"]))
        //    {
        //        uint time_start = (uint)CommonFunction.GetPHPTime(Request.Params["time_start"]);
        //        uint time_end = (uint)CommonFunction.GetPHPTime(Request.Params["time_end"].Substring(0, 10) + " 23:59:59");
        //        query.sqlwhere = " and invoice_date>=" + time_start + " and invoice_date <= " + time_end + "";
        //    }
        //    string month = string.Empty;
        //    string year = string.Empty;
        //    string day = string.Empty;
        //    string test_num = string.Empty;
        //    int temp_num = 0;
        //    string stemp_num = string.Empty;
        //    string format = string.Empty;
        //    string status_mark = string.Empty;
        //    string company_invoice = string.Empty;
        //    string tax_amount = "0";
        //    string order_amount = string.Empty;
        //    const string TAX_NUMBER = "120309443";
        //    const string INVOICE_MS_IDENTIFIER = "25137186";
        //    string line_end = "";
        //    line_end = line_end.PadLeft(9, ' ');
        //    try
        //    {
        //        imrMgr = new InvoiceMasterRecordMgr(mySqlConnectionString);
        //        DataTable dt = imrMgr.GetInvoice(query);
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            if (string.IsNullOrEmpty(test_num))
        //            {

        //                test_num = Regex.Replace(dr["invoice_number"].ToString(), "/[^\\d]/", " ");
        //            }
        //            if (test_num != Regex.Replace(dr["invoice_number"].ToString(), "/[^\\d]/", " "))
        //            {
        //                test_num = test_num + "無連號";
        //            }
        //            if (!string.IsNullOrEmpty(dr["invoice_date"].ToString()))
        //            {
        //                year = CommonFunction.GetNetTime(Convert.ToUInt32(dr["invoice_date"].ToString()) - CommonFunction.GetPHPTime("1911")).ToString("yy");
        //                month = CommonFunction.GetNetTime((uint)dr["invoice_date"]).ToString("MM");
        //                day = CommonFunction.GetNetTime((uint)dr["invoice_date"]).ToString("dd");
        //            }
        //            if (dr["invoice_attribute"].ToString() != "2")
        //            {
        //                temp_num++;
        //                stemp_num = stemp_num.PadLeft(7, '0');
        //                if (dr["company_invoice"].ToString() == "")
        //                {
        //                    format = "31";
        //                    status_mark = dr["tax_type"].ToString();
        //                    company_invoice = "";
        //                    tax_amount = "0";

        //                    company_invoice = company_invoice.PadLeft(8, ' ');
        //                    tax_amount = tax_amount.PadLeft(10, '0');
        //                    order_amount = dr["total_amount"].ToString().PadLeft(12, '0');
        //                }
        //                else
        //                {
        //                    format = "31";
        //                    status_mark = dr["tax_type"].ToString();
        //                    company_invoice = dr["company_invoice"].ToString();
        //                    tax_amount = dr["tax_amount"].ToString().PadLeft(10, '0');
        //                    order_amount = dr["free_tax"].ToString().PadLeft(12, '0');
        //                }
        //                sw.Write(format + "," + TAX_NUMBER + "," + stemp_num + "," + year + "," + month + "," + company_invoice + "," + INVOICE_MS_IDENTIFIER + "," + dr["invoice_number"]);
        //                sw.Write(order_amount + "," + status_mark + "," + tax_amount + "," + line_end);
        //            }
        //            else
        //            {
        //                temp_num++;
        //                stemp_num = temp_num.ToString().PadLeft(7, '0');
        //                tax_amount = "0".PadLeft(10, '0');
        //                order_amount = "0".PadLeft(12, '0'); ;
        //                if (dr["company_invoice"].ToString() == "")
        //                {
        //                    format = "31";//2聯式作廢折讓
        //                    status_mark = "F";//狀態
        //                    company_invoice = "";//個人
        //                    company_invoice = company_invoice.PadLeft(8, ' ');
        //                }
        //                else
        //                {
        //                    format = "31";//3聯式作廢折讓
        //                    status_mark = "F";//狀態
        //                    company_invoice = dr["company_invoice"].ToString();//公司統編
        //                }
        //                sw.Write(format + "," + TAX_NUMBER + "," + stemp_num + "," + year + "," + month + "," + company_invoice + "," + INVOICE_MS_IDENTIFIER + "," + dr["invoice_number"]);
        //                sw.Write(order_amount + "," + status_mark + "," + tax_amount + "," + line_end);
        //            }
        //        }
        //        string filename = "25137186.txt";
        //        Response.Clear();
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(filename));
        //        Response.ContentType = "application/ms-excel";
        //        Response.ContentEncoding = Encoding.Default;
        //        Response.Write(sw);
        //        Response.End();

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        // json = "false,totalCount:0,data:[]";
        //    }

        //}
        ///// <summary>
        ///// 會計報表
        ///// </summary>
        //public void SellReport()
        //{
        //    string json = string.Empty;
        //    StringWriter sw = new StringWriter();
        //    string newFileName = string.Empty;
        //    InvoiceMasterRecordQuery imrq = new InvoiceMasterRecordQuery();
        //    imrMgr = new InvoiceMasterRecordMgr(mySqlConnectionString);
        //    _muMgr = new ManageUserMgr(mySqlConnectionString);
        //    if (!string.IsNullOrEmpty(Request.Params["time_start"]) && !string.IsNullOrEmpty(Request.Params["time_end"]))
        //    {
        //        uint time_start = (uint)CommonFunction.GetPHPTime(Request.Params["time_start"]);
        //        uint time_end = (uint)CommonFunction.GetPHPTime(Request.Params["time_end"].Substring(0, 10) + " 23:59:59");
        //        imrq.sqlwhere += "  and imr.invoice_date >= " + time_start + " and invoice_date <=" + time_end;
        //        imrq.sqlwhere += " and order_date_pay <> 0 ";
        //        //開立狀態
        //        if (!string.IsNullOrEmpty(Request.Params["invoice_attribute"]) && Request.Params["invoice_attribute"] != "-1")
        //        {
        //            imrq.sqlwhere += string.Format(" and  imr.invoice_attribute ={0} ", Request.Params["invoice_attribute"]);
        //        }
        //    }
        //    DataTable dtHZ = new DataTable();
        //    dtHZ.Columns.Add("會員姓名", typeof(String));
        //    dtHZ.Columns.Add("購買時間", typeof(String));
        //    dtHZ.Columns.Add("付款單號", typeof(String));
        //    dtHZ.Columns.Add("付款方式", typeof(String));
        //    dtHZ.Columns.Add("購買金額", typeof(String));
        //    dtHZ.Columns.Add("付款狀態", typeof(String));
        //    dtHZ.Columns.Add("發票號碼", typeof(String));
        //    dtHZ.Columns.Add("發票稅別", typeof(String));
        //    dtHZ.Columns.Add("銷售金額", typeof(String));
        //    dtHZ.Columns.Add("稅額", typeof(String));
        //    dtHZ.Columns.Add("發票金額", typeof(String));
        //    dtHZ.Columns.Add("發票開立日期", typeof(String));
        //    dtHZ.Columns.Add("商品細項編號", typeof(String));
        //    dtHZ.Columns.Add("訂單狀態", typeof(String));
        //    dtHZ.Columns.Add("供應商", typeof(String));
        //    dtHZ.Columns.Add("供應商編碼", typeof(String));
        //    dtHZ.Columns.Add("品名", typeof(String));
        //    dtHZ.Columns.Add("數量", typeof(String));
        //    dtHZ.Columns.Add("購買單價", typeof(String));
        //    dtHZ.Columns.Add("折抵購物金", typeof(String));
        //    dtHZ.Columns.Add("抵用券", typeof(String));
        //    dtHZ.Columns.Add("HappyGo抵用", typeof(String));
        //    dtHZ.Columns.Add("總價", typeof(String));
        //    dtHZ.Columns.Add("成本單價", typeof(String));
        //    dtHZ.Columns.Add("寄倉費", typeof(String));
        //    dtHZ.Columns.Add("成本總額", typeof(String));
        //    dtHZ.Columns.Add("出貨單歸檔期", typeof(String));
        //    dtHZ.Columns.Add("負責PM", typeof(String));
        //    dtHZ.Columns.Add("發票狀態", typeof(String));
        //    dtHZ.Columns.Add("出貨方式", typeof(String));

        //    ManageUserQuery muq = new ManageUserQuery();
        //    muq.IsPage = false;
        //    int totalCount = 0;
        //    DataTable dtOrderMaster = imrMgr.GetOrderMaster(imrq);
        //    DataTable dtOrderDetail = imrMgr.GetOrderDetail(imrq.sqlwhere);
        //    DataTable orderstatus = imrMgr.GetParametersrc("order_status", "remark");
        //    DataTable payment = imrMgr.GetParametersrc("payment", "parameterName");
        //    List<ManageUserQuery> store = _muMgr.GetNameMail(muq, out totalCount);
        //    try
        //    {
        //        string offsetamt = string.Empty;
        //        foreach (DataRow dr in dtOrderMaster.Rows)
        //        {
        //            if (!string.IsNullOrEmpty(dr["order_id"].ToString()))
        //            {
        //                string order_freight_normal = string.Empty;
        //                string order_freight_low = string.Empty;
        //                imrq.order_id = uint.Parse(dr["order_id"].ToString());
        //                //根據訂單查詢發票信息
        //                imrq = new InvoiceMasterRecordQuery();
        //                imrq.order_id = uint.Parse(dr["order_id"].ToString());
        //                DataTable dtInvoice = imrMgr.GetInvoice(imrq);
        //                bool flag1 = false;
        //                bool flag3 = false;
        //                #region 根據訂單號查詢訂單明細表的信息

        //                #region 處理 deduct_happygo
        //                DataRow[] drows = dtOrderDetail.Select("order_id=" + dr["order_id"].ToString());
        //                DataTable drs = new DataTable();
        //                if (drows.Count() > 0)
        //                {
        //                    drs = drows.CopyToDataTable<DataRow>();
        //                }
        //                if (dr["bonus_type"].ToString() == "3" && dr["deduct_happygo"].ToString() != "0")
        //                {
        //                    int deduct_happygo = 0;
        //                    int total_money = 0;
        //                    int hg_nt = 0;
        //                    int sub_total = 0;
        //                    if (drows.Count() > 0)
        //                    {
        //                        deduct_happygo = int.Parse(drs.Compute("sum(deduct_happygo)", "true").ToString());
        //                        total_money = int.Parse(drs.Compute("sum(single_money*buy_num)", "true").ToString());
        //                    }
        //                    //sub_total = int.Parse(drs.Compute("sum(single_money*buy_num)", "true").ToString());
        //                    if (deduct_happygo != 0)
        //                    {
        //                        if (!string.IsNullOrEmpty(dr["deduct_happygo_convert"].ToString()))
        //                        {
        //                            hg_nt = int.Parse((deduct_happygo * float.Parse(dr["deduct_happygo_convert"].ToString())).ToString("0"));
        //                        }
        //                        foreach (DataRow drr in drs.Rows)
        //                        {
        //                            if (!string.IsNullOrEmpty(drr["single_money"].ToString()) && !string.IsNullOrEmpty(drr["buy_num"].ToString()))
        //                            {
        //                                sub_total = int.Parse(drr["single_money"].ToString()) * int.Parse(drr["buy_num"].ToString());
        //                            }
        //                            if (sub_total != 0)
        //                            {
        //                                drr["deduct_happygo"] = int.Parse((hg_nt * sub_total / total_money).ToString("0"));
        //                                if (!string.IsNullOrEmpty(drr["deduct_happygo"].ToString()))
        //                                {
        //                                    hg_nt -= int.Parse(drr["deduct_happygo"].ToString());
        //                                }
        //                                total_money -= sub_total;
        //                            }
        //                        }
        //                    }
        //                }
        //                #endregion

        //                int indexmax = dtOrderDetail.Rows.Count;
        //                int i = 0;
        //                foreach (DataRow drr in drs.Rows)
        //                {
        //                    i++;
        //                    #region 處理order_detail表的數據
        //                    DataRow drNew = dtHZ.NewRow();
        //                    drNew["會員姓名"] = dr["order_name"].ToString();
        //                    if (!string.IsNullOrEmpty(dr["order_createdate"].ToString()))
        //                    {
        //                        drNew["購買時間"] = (CommonFunction.GetNetTime(Convert.ToInt32(dr["order_createdate"]))).ToString("yyyy-MM-dd HH:mm:ss");
        //                    }
        //                    drNew["付款單號"] = dr["order_id"].ToString();
        //                    if (!string.IsNullOrEmpty(dr["order_payment"].ToString()))
        //                    {
        //                        DataRow[] drop = orderstatus.Select("parameterCode=" + dr["order_payment"].ToString());
        //                        if (drop.Count() > 0)
        //                        {
        //                            drNew["付款方式"] = drop[0][1];
        //                        }
        //                    }
        //                    drNew["購買金額"] = dr["order_amount"].ToString();

        //                    if (!string.IsNullOrEmpty(dr["order_status"].ToString()))
        //                    {
        //                        DataRow[] dros = orderstatus.Select("parameterCode=" + dr["order_status"].ToString());
        //                        if (dros.Count() > 0)
        //                        {
        //                            drNew["付款狀態"] = dros[0][1];
        //                        }
        //                    }
        //                    DataRow[] rs = dtInvoice.Select("tax_type=" + drr["tax_type"].ToString());
        //                    if (rs.Count() > 0)
        //                    {
        //                        drNew[6] = rs[0]["invoice_number"].ToString();
        //                        drNew[11] = rs[0]["invoice_date"].ToString();
        //                    }
        //                    else
        //                    {
        //                        drNew[6] = "無發票";
        //                        drNew[11] = "無發票";
        //                    }
        //                    if ((drr["tax_type"].ToString() == "1") ? flag1 == false : flag3 == false)
        //                    {
        //                        drNew[8] = (rs.Count() > 0) ? (drr["tax_type"].ToString() == "1") ? rs[0]["free_tax"].ToString() : rs[0]["total_amount"].ToString() : "無發票";
        //                        drNew[9] = (rs.Count() > 0) ? (drr["tax_type"].ToString() == "1") ? rs[0]["tax_amount"].ToString() : "0" : "無發票";
        //                        drNew[10] = (rs.Count() > 0) ? rs[0]["total_amount"].ToString() : "無發票";
        //                    }
        //                    else
        //                    {
        //                        drNew[8] = "";
        //                        drNew[9] = "";
        //                        drNew[10] = "";
        //                    }
        //                    if (drr["tax_type"].ToString() == "1")
        //                    {
        //                        flag1 = true;
        //                    }
        //                    else
        //                    {
        //                        flag3 = true;
        //                    }
        //                    drNew[12] = drr["item_id"].ToString();
        //                    //drNew[13] = drr["detail_status"].ToString();
        //                    if (!string.IsNullOrEmpty(drr["detail_status"].ToString()))
        //                    {
        //                        DataRow[] drds = orderstatus.Select("parameterCode=" + drr["detail_status"].ToString());
        //                        if (drds.Count() > 0)
        //                        {
        //                            drNew[13] = drds[0][1];
        //                        }
        //                        // drNew["訂單狀態"] = orderstatus.Select("parameterCode=" + drr["detail_status"].ToString())[0][1];
        //                    }
        //                    drNew[14] = drr["vendor_name_simple"].ToString();
        //                    drNew[15] = drr["vendor_code"].ToString();
        //                    drNew[16] = drr["product_name"].ToString();
        //                    if (drr["item_mode"].ToString() == "0")
        //                    {
        //                        drNew[17] = drr["buy_num"].ToString();
        //                    }
        //                    else
        //                    {
        //                        if (!string.IsNullOrEmpty(drr["buy_num"].ToString()) && !string.IsNullOrEmpty(drr["parent_num"].ToString()))
        //                        {
        //                            drNew[17] = int.Parse(drr["buy_num"].ToString()) * int.Parse(drr["parent_num"].ToString());
        //                        }
        //                    }
        //                    drNew[18] = drr["single_money"].ToString();
        //                    drNew[19] = drr["deduct_bonus"].ToString();
        //                    drNew[20] = drr["deduct_welfare"].ToString();
        //                    drNew[21] = drr["deduct_happygo"].ToString();
        //                    if (!string.IsNullOrEmpty(drr["buy_num"].ToString()) && !string.IsNullOrEmpty(drr["single_money"].ToString()))
        //                    {
        //                        if (!string.IsNullOrEmpty(drr["deduct_bonus"].ToString()) && !string.IsNullOrEmpty(drr["deduct_welfare"].ToString()) && !string.IsNullOrEmpty(drr["deduct_happygo"].ToString()))
        //                        {
        //                            drNew[22] = int.Parse(drr["single_money"].ToString()) * int.Parse(drr["buy_num"].ToString()) - (int.Parse(drr["deduct_bonus"].ToString()) + int.Parse(drr["deduct_welfare"].ToString()) + int.Parse(drr["deduct_happygo"].ToString()));
        //                        }
        //                    }
        //                    string cost = "0";
        //                    if (drr["event_cost"].ToString() != "0")
        //                    {
        //                        drNew[23] = drr["event_cost"].ToString();
        //                        cost = drr["event_cost"].ToString();
        //                    }
        //                    else
        //                    {
        //                        drNew[23] = drr["single_cost"].ToString();
        //                        cost = drr["single_cost"].ToString();
        //                    }
        //                    drNew[24] = drr["bag_check_money"].ToString();
        //                    if (!string.IsNullOrEmpty(drr["buy_num"].ToString()) && !string.IsNullOrEmpty(drr["bag_check_money"].ToString()))
        //                    {
        //                        drNew[25] = int.Parse(drr["buy_num"].ToString()) * int.Parse(cost) - (int.Parse(drr["bag_check_money"].ToString()) * int.Parse(drr["buy_num"].ToString()));
        //                    }
        //                    //drNew[26] = drr["slave_date_close"].ToString();
        //                    if (drr["slave_date_close"].ToString() == "0")
        //                    {
        //                        drNew["出貨單歸檔期"] = "未歸檔";
        //                    }
        //                    else
        //                    {
        //                        drNew["出貨單歸檔期"] = CommonFunction.GetNetTime(Convert.ToInt32(drr["slave_date_close"])).ToString("yyyy-MM-dd HH:mm:ss");
        //                    }
        //                    if (drr["product_manage"].ToString() != "0")
        //                    {
        //                        drNew["負責PM"] = store[store.FindIndex((ManageUserQuery e) => e.user_id == uint.Parse(drr["product_manage"].ToString()))].user_name;
        //                    }
        //                    else
        //                    {
        //                        drNew["負責PM"] = "未設定";
        //                    }
        //                    if (rs.Count() > 0)
        //                    {
        //                        //drNew[28] = drr[""].ToString();
        //                        string value = rs[0]["invoice_attribute"].ToString();
        //                        switch (value)
        //                        {
        //                            case "1":
        //                                drNew["發票狀態"] = "開理發票";
        //                                break;
        //                            case "2":
        //                                drNew["發票狀態"] = "作廢發票";
        //                                break;
        //                            case "3":
        //                                drNew["發票狀態"] = "折讓發票";
        //                                break;
        //                            case "4":
        //                                drNew["發票狀態"] = "金額異動";
        //                                break;
        //                            case "5":
        //                                drNew["發票狀態"] = "二聯轉三聯";
        //                                break;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        drNew[28] = "無發票";
        //                    }
        //                    switch (drr["product_mode"].ToString())
        //                    {
        //                        case "1":
        //                            drNew[29] = "供應商自出";
        //                            break;
        //                        case "2":
        //                            drNew[29] = "寄倉";
        //                            break;
        //                        case "3":
        //                            drNew[29] = "調度";
        //                            break;
        //                    }
        //                    dtHZ.Rows.Add(drNew);
        //                    #endregion
        //                    #region 循環至最後一行時添加一行總結
        //                    if (i == indexmax)
        //                    {
        //                        DataRow drSummary = dtHZ.NewRow();
        //                        drSummary[0] = drNew[0];
        //                        drSummary[1] = drNew[1];
        //                        drSummary[2] = drNew[2];
        //                        drSummary[3] = drNew[3];
        //                        drSummary[4] = drNew[4];
        //                        drSummary[5] = drNew[5];
        //                        DataRow[] rows = dtInvoice.Select("tax_type=1");
        //                        string taxtype = "1";
        //                        if (rows.Count() > 0)
        //                        {
        //                            drSummary[6] = rows[0]["invoice_number"].ToString();
        //                        }
        //                        else
        //                        {
        //                            drSummary[6] = "無發票";
        //                        }
        //                        drSummary[7] = "應稅";
        //                        drSummary[8] = (rows.Count() > 0) ? (taxtype == "1") ? rows[0]["free_tax"].ToString() : rows[0]["total_amount"].ToString() : "無發票";
        //                        drSummary[9] = (rows.Count() > 0) ? (taxtype == "1") ? rows[0]["tax_amount"].ToString() : "0" : "無發票";
        //                        drSummary[10] = (rows.Count() > 0) ? rows[0]["total_amount"].ToString() : "無發票";
        //                        drSummary[11] = (rows.Count() > 0) ? rows[0]["invoice_date"].ToString() : "無發票";
        //                        drSummary[26] = drNew[26];
        //                        if (dr["order_freight_normal"].ToString() != "0")
        //                        {
        //                            drSummary[12] = "G00001";
        //                            drSummary[16] = "常溫運費";
        //                            drSummary[17] = "1";
        //                            drSummary[18] = dr["order_freight_normal"].ToString();

        //                            drSummary[23] = dr["order_freight_normal"].ToString();

        //                        }
        //                        if (dr["order_freight_low"].ToString() != "0")
        //                        {
        //                            drSummary[12] = "G00002";
        //                            drSummary[16] = "低溫運費";
        //                            drSummary[17] = "1";
        //                            drSummary[18] = dr["order_freight_low"].ToString();

        //                            drSummary[23] = dr["order_freight_low"].ToString();
        //                        }
        //                        if (dr["offsetamt"].ToString() != "0")
        //                        {
        //                            drSummary[12] = "G00003";
        //                            drSummary[16] = "中信折抵";
        //                            drSummary[17] = "1";
        //                            drSummary[18] = "-" + dr["offsetamt"].ToString();
        //                            drSummary[6] = "";
        //                            drSummary[7] = "";
        //                            drSummary[8] = "";
        //                            drSummary[9] = "";
        //                            drSummary[10] = "";
        //                            drSummary[11] = "";
        //                            drSummary[23] = "-" + dr["offsetamt"].ToString();

        //                        }
        //                        dtHZ.ImportRow(drSummary);
        //                    }
        //                    #endregion

        //                }
        //                #endregion


        //            }

        //        }
        //        MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "");
        //        Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", "會計報表.xls"));
        //        Response.ContentType = "application/ms-excel";
        //        Response.BinaryWrite(ms.ToArray());
        //        ms.Close();
        //        ms.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "false";
        //    }
        //}
        //#region 列印 匯出PDF文檔
        ///// <summary>
        ///// 列印
        ///// </summary>
        //public void ColumnPrint()
        //{
        //    /********************************************************/
        //    InvoiceMasterRecordQuery query = new InvoiceMasterRecordQuery();
        //    //_zipMgr = new ZipMgr(mySqlConnectionString);
        //    string json = string.Empty;
        //    DataTable dt = new DataTable();
        //    StringBuilder sqlwhere = new StringBuilder();
        //    try
        //    {
        //        imrMgr = new InvoiceMasterRecordMgr(mySqlConnectionString);
        //        //查詢條件
        //        if (!string.IsNullOrEmpty(Request.Params["ven_type"]) && !string.IsNullOrEmpty(Request.Params["search_content"]))
        //        {
        //            string search_content = Request.Params["search_content"].Trim();
        //            string value = Request.Params["ven_type"];
        //            switch (value)
        //            {
        //                case "1":
        //                    sqlwhere.AppendFormat(@" and imr.order_id='{0}' ", search_content);
        //                    break;
        //                case "2":
        //                    sqlwhere.AppendFormat(@" and imr.company_title like '%{0}%' ", search_content);
        //                    break;
        //                case "3":
        //                    sqlwhere.AppendFormat(@" and imr.company_invoice ='{0}'", search_content);
        //                    break;
        //                case "4":
        //                    sqlwhere.AppendFormat(@" and imr.invoice_number='{0}'", search_content);
        //                    break;
        //                case "5":
        //                    sqlwhere.AppendFormat(@" and imr.buyer_name like '%{0}%' ", search_content);
        //                    break;
        //                case "6":
        //                    sqlwhere.AppendFormat(@" and imr.invoice_id = '{0}' ", search_content);
        //                    break;
        //                default:
        //                    break;
        //            }

        //        }
        //        //日期條件
        //        if (!string.IsNullOrEmpty(Request.Params["date_type"]) && !string.IsNullOrEmpty(Request.Params["time_start"]) && !string.IsNullOrEmpty(Request.Params["time_end"]))
        //        {
        //            uint time_start = (uint)CommonFunction.GetPHPTime(Request.Params["time_start"]);
        //            uint time_end = (uint)CommonFunction.GetPHPTime(Request.Params["time_end"].Substring(0, 10) + " 23:59:59");
        //            string value = Request.Params["date_type"];
        //            switch (value)
        //            {
        //                case "1":
        //                    sqlwhere.AppendFormat(@"  and imr.invoice_date >={0} and invoice_date <={1}", time_start, time_end);
        //                    break;
        //                case "2":
        //                    sqlwhere.AppendFormat(@"  and imr.print_post_createdate >={0}  and print_post_createdate <={1}", time_start, time_end);
        //                    break;
        //                case "3":
        //                    sqlwhere.AppendFormat(@"  and imr.status_createdate >={0} and imr.status_createdate <={1}", time_start, time_end);
        //                    break;
        //            }

        //        }
        //        //開立狀態
        //        if (!string.IsNullOrEmpty(Request.Params["invoice_status"]) && Request.Params["invoice_status"] != "-1")
        //        {
        //            sqlwhere.AppendFormat(@" and imr.invoice_status ={0}", Request.Params["invoice_status"]);
        //        }
        //        //發票類型
        //        if (!string.IsNullOrEmpty(Request.Params["invoice_type"]) && Request.Params["invoice_type"] != "-1")
        //        {
        //            sqlwhere.AppendFormat(@" and imr.buyer_type={0} ", Request.Params["invoice_type"]);
        //        }
        //        //是否寄出發票
        //        if (!string.IsNullOrEmpty(Request.Params["is_send"]) && Request.Params["is_send"] != "-1")
        //        {
        //            string value = Request.Params["is_send"];
        //            sqlwhere.AppendFormat(@" and ( om.paper_invoice = 1 or imr.buyer_type= 1 ) ");
        //            switch (value)
        //            {
        //                case "0":
        //                    sqlwhere.AppendFormat(@" and imr.print_post_createdate =  0 ");
        //                    break;
        //                case "1":
        //                    sqlwhere.AppendFormat(@" and imr.print_post_createdate !=0 ");
        //                    break;
        //            }

        //        }
        //        query.sqlwhere = sqlwhere.ToString();
        //        Dictionary<DataRow, DataRow[]> dic = imrMgr.InvoicePrint(query);
        //        string buyer_name = string.Empty;
        //        string buyer_type = string.Empty;
        //        string order_zip = string.Empty;
        //        string order_address = string.Empty;
        //        string invoice_number = string.Empty;
        //        string invoice_id = string.Empty;
        //        string total_amount = string.Empty;
        //        string invoice_date = string.Empty;
        //        string company_title = string.Empty;
        //        string company_invoice = string.Empty;
        //        string sales_amount = string.Empty;
        //        string tax_type = string.Empty;
        //        string tax_amount = string.Empty;
        //        string filename = "456.pdf";
        //        string newPDFName = Server.MapPath(excelPath) + filename;

        //        if (System.IO.File.Exists(newPDFName))
        //        {
        //            //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
        //            System.IO.File.SetAttributes(newPDFName, FileAttributes.Normal);
        //            System.IO.File.Delete(newPDFName);
        //        }
        //        Document document = new Document(PageSize.A4, (float)5, (float)5, (float)0.5, (float)0.5);
        //        PdfWriter.GetInstance(document, new FileStream(newPDFName, FileMode.Create));
        //        //生成的PDF文件名为test1.pdf  
        //        document.Open();
        //        BaseFont bfChinese = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        //        iTextSharp.text.Font fontChinese = new iTextSharp.text.Font(bfChinese, 14, iTextSharp.text.Font.UNDERLINE, iTextSharp.text.BaseColor.RED);
        //        iTextSharp.text.Font font = new iTextSharp.text.Font(bfChinese, 12, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(0, 0, 0));//黑  
        //        PdfPTable table = new PdfPTable(6);

        //        //table.DefaultCell.AutoFillEmptyCells = true;
        //        table.DefaultCell.BorderWidth = 0;
        //        table.DefaultCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //        foreach (var item in dic)
        //        {
        //            #region  條形碼
        //            BarCode.Code128 _Code = new BarCode.Code128();
        //            _Code.ValueFont = new System.Drawing.Font("宋体", 18);
        //            System.Drawing.Bitmap imgTemp1 = _Code.GetCodeImage(item.Key["invoice_number"].ToString(), BarCode.Code128.Encode.Code128A);
        //            imgTemp1.Save(System.AppDomain.CurrentDomain.BaseDirectory + "\\ImportUserIOExcel\\" + "Code.gif", System.Drawing.Imaging.ImageFormat.Gif);
        //            iTextSharp.text.Image IMG1 = iTextSharp.text.Image.GetInstance(Server.MapPath("../ImportUserIOExcel/Code.gif"));
        //            IMG1.ScaleToFit(200, 30);
        //            //Chunk ck1 = new Chunk(IMG1, 0, 0); //图片可设置 偏移
        //            #endregion
        //            PdfPCell cell;
        //            buyer_name = item.Key["buyer_name"].ToString();
        //            buyer_type = item.Key["buyer_type"].ToString();
        //            order_zip = item.Key["order_zip"].ToString();
        //            order_address = item.Key["order_address"].ToString();
        //            invoice_number = item.Key["invoice_number"].ToString();
        //            invoice_id = item.Key["invoice_id"].ToString();
        //            total_amount = item.Key["total_amount"].ToString();
        //            invoice_date = item.Key["invoice_date"].ToString();
        //            company_title = item.Key["company_title"].ToString();
        //            company_invoice = item.Key["company_invoice"].ToString();
        //            sales_amount = item.Key["sales_amount"].ToString();
        //            tax_type = item.Key["tax_type"].ToString();
        //            tax_amount = item.Key["tax_amount"].ToString();

        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(buyer_name, font));
        //            cell.Colspan = 6;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell();
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(order_zip + order_address, font));
        //            cell.Colspan = 6;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(IMG1);
        //            cell.Colspan = 6;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(buyer_name, font));
        //            cell.Colspan = 2;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(buyer_name, font));
        //            cell.Colspan = 2;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(invoice_number, font));
        //            cell.Colspan = 2;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.BorderColor = new BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(item.Key["order_id"].ToString(), font));
        //            cell.Colspan = 6;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);

        //            foreach (var row in item.Value)
        //            {
        //                cell = new PdfPCell(new Phrase(row["item_id"].ToString(), font));
        //                cell.Colspan = 1;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(row["product_name"].ToString(), font));
        //                cell.Colspan = 2;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(row["buy_num"].ToString(), font));
        //                cell.Colspan = 1;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(row["single_money"].ToString(), font));
        //                cell.Colspan = 1;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(row["subtotal"].ToString(), font));
        //                cell.Colspan = 1;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //            }
        //            string year = string.Empty;
        //            int y = 0;
        //            string month = string.Empty;
        //            string day = string.Empty;
        //            if (!string.IsNullOrEmpty(invoice_date))
        //            {
        //                year = CommonFunction.GetNetTime(Convert.ToUInt32(invoice_date)).ToString("yyyy");
        //                y = Convert.ToInt32(year);
        //                y -= 1911;
        //                year = y.ToString();
        //                month = CommonFunction.GetNetTime(Convert.ToUInt32(invoice_date)).ToString("MM");
        //                day = CommonFunction.GetNetTime(Convert.ToUInt32(invoice_date)).ToString("dd");
        //            }
        //            #region 空白 用來控制樣式
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(" "));
        //            cell.Colspan = 6;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            #endregion
        //            cell = new PdfPCell(new Phrase(total_amount, font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(total_amount, font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(year + "  " + month + "  " + day, font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(year + "  " + month + "  " + day, font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(invoice_number, font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(invoice_number, font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            if (buyer_type == "1")
        //            {
        //                cell = new PdfPCell(new Phrase(company_title, font));
        //                cell.Colspan = 3;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(company_title, font));
        //                cell.Colspan = 3;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(company_invoice, font));
        //                cell.Colspan = 3;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(company_invoice, font));
        //                cell.Colspan = 3;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //            }
        //            else
        //            {
        //                cell = new PdfPCell(new Phrase(buyer_name, font));
        //                cell.Colspan = 3;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);
        //                cell = new PdfPCell(new Phrase(buyer_name, font));
        //                cell.Colspan = 3;
        //                cell.UseAscender = true;
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //                table.AddCell(cell);

        //            }
        //            cell = new PdfPCell(new Phrase(check_invoice_num(invoice_number), font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase(check_invoice_num(invoice_number), font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);

        //            cell = new PdfPCell(new Phrase("銷售如附件" + "              " + total_amount, font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);
        //            cell = new PdfPCell(new Phrase("銷售如附件" + "              " + total_amount, font));
        //            cell.Colspan = 3;
        //            cell.UseAscender = true;
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
        //            table.AddCell(cell);

        //        }
        //        document.Add(table);
        //        document.Close();
        //        Response.Clear();
        //        Response.Charset = "gb2312";
        //        Response.ContentEncoding = System.Text.Encoding.UTF8;
        //        // Response.AddHeader("Content-Disposition", "attach-ment;filename=" + System.Web.HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8) + ".pdf ");
        //        Response.AddHeader("Content-Disposition", "attach-ment;filename=" + filename);
        //        Response.WriteFile(newPDFName);
        //        // Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        //json = "false,totalCount:0,data:[]";
        //    }

        //}
        //public string check_invoice_num(string invoice_number)
        //{
        //    int value1 = 33;
        //    int value2 = 99;
        //    int valueA = 0;
        //    int valueB = 0;
        //    int count = 0;
        //    int temp = 0;
        //    int valueE = 0;
        //    if (invoice_number.Length >= 10)
        //    {
        //        if (!string.IsNullOrEmpty(invoice_number.Substring(9, 1)))
        //        {
        //            valueA = Convert.ToInt32(invoice_number.Substring(9, 1)) * value1;
        //            if (Convert.ToInt32(invoice_number.Substring(9, 1)) <= 3)
        //            {
        //                valueA = 999;
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(invoice_number.Substring(2, 7)))
        //        {
        //            valueB = Convert.ToInt32(invoice_number.Substring(2, 7));

        //            for (int i = 0; i < valueB.ToString().Length; i++)
        //            {
        //                count += Convert.ToInt32(valueB.ToString().Substring(i, 1));
        //            }
        //        }
        //        if (!string.IsNullOrEmpty(valueA.ToString() + count.ToString()))
        //        {
        //            temp = Convert.ToInt32(valueA.ToString() + count.ToString());
        //        }
        //        if (temp.ToString().Length > 4)
        //        {
        //            int valueC = Convert.ToInt32(temp.ToString().Substring(temp.ToString().Length - 4, 4));
        //            int valueD = valueC * value2;
        //            if (valueD.ToString().Length > 3)
        //            {
        //                valueE = Convert.ToInt32(valueD.ToString().Substring(valueD.ToString().Length - 3, 3));
        //            }
        //        }

        //    }
        //    if (valueE.ToString().Length < 3)
        //    {
        //        return "";
        //    }
        //    else
        //    {
        //        return valueE.ToString();
        //    }
        //}

        //#endregion
        //#endregion
        //#endregion

        //#region 退款單
        //#region 列表頁
        //public HttpResponseBase OrderMoneyReturnList()
        //{
        //    string json = string.Empty;
        //    List<OrderMoneyReturnQuery> store = new List<OrderMoneyReturnQuery>();
        //    try
        //    {
        //        OrderMoneyReturnQuery query = new OrderMoneyReturnQuery();
        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
        //        if (!string.IsNullOrEmpty(Request.Params["searchStore"]))
        //        {
        //            query.SearchStore = Request.Params["searchStore"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["searchContents"]))
        //        {
        //            query.searchContents = Request.Params["searchContents"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["start_date"]))
        //        {
        //            query.start_date = Convert.ToDateTime(Convert.ToDateTime(Request.Params["start_date"]).ToString("yyyy-MM-dd 00:00:00"));
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["end_date"]))
        //        {
        //            query.end_date = Convert.ToDateTime(Convert.ToDateTime(Request.Params["end_date"]).ToString("yyyy-MM-dd 23:59:59"));
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["file_type"]))
        //        {
        //            query.states = Request.Params["file_type"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["date"]))
        //        {
        //            query.date = (Request.Params["date"].ToString());
        //        }
        //        _IOrderMoneyReturnMgr = new OrderMoneyReturnMgr(mySqlConnectionString);
        //        int totalCount = 0;
        //        store = _IOrderMoneyReturnMgr.OrderMoneyReturnList(query, out totalCount);
        //        foreach (var item in store)
        //        {
        //            item.createdate = CommonFunction.GetNetTime(item.money_createdate);
        //            item.updatedate = CommonFunction.GetNetTime(item.money_updatedate);
        //        }
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:true,totalCount:0,data:[]}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;

        //}
        //#endregion
        //#region 編輯
        //public HttpResponseBase SaveOMReturn()
        //{
        //    OrderMoneyReturnQuery query = new OrderMoneyReturnQuery();
        //    string json = string.Empty;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["money_id"]))
        //        {
        //            query.money_id = Convert.ToUInt32(Request.Params["money_id"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["money_status"]))
        //        {
        //            query.money_status = Convert.ToUInt32(Request.Params["money_status"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["money_note"]))
        //        {
        //            query.money_note = Request.Params["money_note"];
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["bank_note"]))
        //        {
        //            query.bank_note = Request.Params["bank_note"];
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["bank_name"]))
        //        {
        //            query.bank_name = Request.Params["bank_name"];
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["bank_branch"]))
        //        {
        //            query.bank_branch = Request.Params["bank_branch"];
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["bank_account"]))
        //        {
        //            query.bank_account = Request.Params["bank_account"];
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["account_name"]))
        //        {
        //            query.account_name = Request.Params["account_name"];
        //        }

        //        _IOrderMoneyReturnMgr = new OrderMoneyReturnMgr(mySqlConnectionString);
        //        if (_IOrderMoneyReturnMgr.SaveOMReturn(query) > 0)
        //        {
        //            json = "{success:'true'}";
        //        }
        //        else
        //        {
        //            json = "{success:'false'}";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:'false'}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion
        //#region 批次歸檔
        //public HttpResponseBase ModifyStatus()
        //{
        //    string json = string.Empty;
        //    OrderMoneyReturnQuery query = new OrderMoneyReturnQuery();
        //    List<OrderMoneyReturnQuery> store = new List<OrderMoneyReturnQuery>();
        //    try
        //    {
        //        _IOrderMoneyReturnMgr = new OrderMoneyReturnMgr(mySqlConnectionString);
        //        if (!string.IsNullOrEmpty(Request.Params["rowID"]))
        //        {
        //            foreach (string rid in Request.Params["rowID"].ToString().Split('|'))
        //            {
        //                if (!string.IsNullOrEmpty(rid))
        //                {
        //                    query = new OrderMoneyReturnQuery();
        //                    query.money_id = Convert.ToUInt32(rid);
        //                    query.money_note = "批次歸檔";
        //                    store.Add(query);
        //                }
        //            }
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion
        //#region 匯出ATM
        //public HttpResponseBase ExportATM()
        //{
        //    string json = string.Empty;
        //    OrderMoneyReturnQuery query = new OrderMoneyReturnQuery();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["searchStore"]))
        //        {
        //            query.SearchStore = Request.Params["searchStore"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["searchContents"]))
        //        {
        //            query.searchContents = Request.Params["searchContents"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["start_date"]))
        //        {
        //            query.start_date = Convert.ToDateTime(Request.Params["start_date"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["end_date"]))
        //        {
        //            query.end_date = Convert.ToDateTime(Request.Params["end_date"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["file_type"]))
        //        {
        //            query.states = Request.Params["file_type"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["date"]))
        //        {
        //            query.date = (Request.Params["date"].ToString());
        //        }
        //        if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
        //        {
        //            System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
        //        }
        //        string fileName = string.Empty;
        //        string[] ColumnName = { "購物單號", "退款單號", "退款金額", "銀行名稱", "銀行代碼", "銀行帳號", "銀行戶名", "類型" };
        //        fileName = "money_export_atm" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        //        string newExcelName = Server.MapPath(excelPath) + fileName;//處理文件名，獲取新的文件名

        //        _IOrderMoneyReturnMgr = new OrderMoneyReturnMgr(mySqlConnectionString);
        //        DataTable _dt = _IOrderMoneyReturnMgr.ExportATM(query);
        //        DataTable _newDt = new DataTable();
        //        for (int i = 0; i < ColumnName.Length; i++)
        //        {
        //            _newDt.Columns.Add(ColumnName[i], typeof(string));
        //        }
        //        for (int i = 0; i < _dt.Rows.Count; i++)
        //        {
        //            DataRow dr = _newDt.NewRow();
        //            for (int j = 0; j < _dt.Columns.Count; j++)
        //            {
        //                if (j == 7)
        //                {
        //                    dr[7] = _dt.Rows[i][7];
        //                    if (dr[7].ToString() == "2")
        //                    {
        //                        dr[7] = "ATM";
        //                    }
        //                }
        //                else
        //                {
        //                    dr[j] = _dt.Rows[i][j];
        //                }
        //            }
        //            _newDt.Rows.Add(dr);
        //        }
        //        CsvHelper.ExportDataTableToCsv(_newDt, newExcelName, ColumnName, true);
        //        json = "{success:'true',filename:'" + fileName + "'}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:'false'}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion
        //#region 匯出信用卡
        //public HttpResponseBase ExportCard()
        //{
        //    string json = string.Empty;
        //    OrderMoneyReturnQuery query = new OrderMoneyReturnQuery();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(Request.Params["searchStore"]))
        //        {
        //            query.SearchStore = Request.Params["searchStore"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["searchContents"]))
        //        {
        //            query.searchContents = Request.Params["searchContents"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["start_date"]))
        //        {
        //            query.start_date = Convert.ToDateTime(Request.Params["start_date"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["end_date"]))
        //        {
        //            query.end_date = Convert.ToDateTime(Request.Params["end_date"]);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["file_type"]))
        //        {
        //            query.states = Request.Params["file_type"].ToString();
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["date"]))
        //        {
        //            query.date = (Request.Params["date"].ToString());
        //        }
        //        if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
        //        {
        //            System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
        //        }
        //        string fileName = string.Empty;
        //        string[] ColumnName = { "購物單號", "退款單號", "退款金額", "付款單總金額", "會員姓名", "類型", "狀態" };
        //        _IOrderMoneyReturnMgr = new OrderMoneyReturnMgr(mySqlConnectionString);
        //        fileName = "money_export_card" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        //        string newExcelName = Server.MapPath(excelPath) + fileName;//處理文件名，獲取新的文件名
        //        _IOrderMoneyReturnMgr = new OrderMoneyReturnMgr(mySqlConnectionString);
        //        DataTable _dt = _IOrderMoneyReturnMgr.ExportCARD(query);
        //        DataTable _newDt = new DataTable();
        //        for (int i = 0; i < ColumnName.Length; i++)
        //        {
        //            _newDt.Columns.Add(ColumnName[i], typeof(string));
        //        }
        //        for (int i = 0; i < _dt.Rows.Count; i++)
        //        {
        //            DataRow dr = _newDt.NewRow();
        //            for (int j = 0; j < _dt.Columns.Count; j++)
        //            {
        //                #region  money_type為1時,改為信用卡
        //                if (j == 5)
        //                {
        //                    dr[5] = _dt.Rows[i][5];
        //                    if (dr[5].ToString() == "1")
        //                    {
        //                        dr[5] = "信用卡";
        //                    }
        //                }

        //                #endregion
        //                #region money_status為1時,歸檔,為0時,未歸檔
        //                else if (j == 6)
        //                {
        //                    dr[6] = _dt.Rows[i][6];
        //                    if (dr[6].ToString() == "1")
        //                    {
        //                        dr[6] = "歸檔";
        //                    }
        //                    if (dr[6].ToString() == "0")
        //                    {
        //                        dr[6] = "未歸檔";
        //                    }
        //                    //else
        //                    //{
        //                    //    dr[6] = "未歸檔";
        //                    //}
        //                }
        //                else
        //                {
        //                    dr[j] = _dt.Rows[i][j];
        //                }
        //                #endregion

        //            }
        //            _newDt.Rows.Add(dr);
        //        }
        //        CsvHelper.ExportDataTableToCsv(_newDt, newExcelName, ColumnName, true);
        //        json = "{success:'true',filename:'" + fileName + "'}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:'false'}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json.ToString());
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion
        //#endregion

        #region 供應商業績報表
        #region 供應商業績報表列表
        /// <summary>
        /// 供應商業績報表列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetVendorAccountMonthList()
        {
            VendorAccountMonthQuery query = new VendorAccountMonthQuery();
            List<VendorAccountMonthQuery> stores = new List<VendorAccountMonthQuery>();
            string json = string.Empty;
            try
            {

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                string type = Request.Params["dateType"].ToString();
                string con = Request.Params["dateCon"].ToString();
                if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(con))
                {
                    query.type = Convert.ToInt32(type);
                    query.keyworks = con;
                }
                query.account_year = Convert.ToUInt32(Request.Params["dateOne"]);
                query.account_month = Convert.ToUInt32(Request.Params["dateTwo"]);

                _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _IVAMMgr.GetVendorAccountMonthList(query, out totalCount);
                //foreach (var item in stores)
                //{
                //    item.screatedate = CommonFunction.GetNetTime(item.createdate);

                //}
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

        #region 供應商對帳總表+void VendorAccountCountExport()
        /// <summary>
        /// 供應商對帳總表
        /// </summary>
        public void VendorAccountCountExport()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            VendorAccountMonthQuery VAMQuery = new VendorAccountMonthQuery();
            VendorQuery vendorQuery = new VendorQuery();
            try
            {
                string type = Request.Params["dateType"].ToString();//根據統一編號/電子信箱/供應商名稱
                string con = Request.Params["dateCon"].ToString();//查詢內容
                if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(con))
                {
                    VAMQuery.type = Convert.ToInt32(type);
                    VAMQuery.keyworks = con;
                }
                //List<string> list = GetTime(uint.Parse(Request.Params["dateone"]), uint.Parse(Request.Params["datetwo"]));
                //List<string> list = GetTime(2014, 2);
                _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
                // Dictionary<int, DataTable> tempDT = new Dictionary<int, DataTable>();
                vendorQuery.vendor_id = Convert.ToUInt32(Request.Params["vendorid"]);
                VAMQuery.vendor_id = Convert.ToUInt32(Request.Params["vendorid"]);
                VAMQuery.account_year = Convert.ToUInt32(Request.Params["dateOne"]);
                VAMQuery.account_month = Convert.ToUInt32(Request.Params["dateTwo"]);
                //tempDT.Add(0, tempTemp);
                _dt = _IVAMMgr.VendorAccountCountExport(VAMQuery);// 查詢訂單筆數
                DataTable tableTemp = _IVAMMgr.VendorAccountInfoExport(VAMQuery);// 供應商總表部分信息
                Dictionary<string, double> tempDT = new Dictionary<string, double>();
                for (int i = 0; i < tableTemp.Rows.Count; i++)
                {
                    double creditcard_Money = 0;
                    int orderpPayment = Convert.ToInt32(tableTemp.Rows[i]["order_payment"]);
                    if (orderpPayment == 1)//支付方式為聯合信用卡的
                    {
                        creditcard_Money = Math.Round(Convert.ToInt32(tableTemp.Rows[i]["deduction"]) * 0.02);
                    }
                    else if (orderpPayment == 11 || orderpPayment == 12 || orderpPayment == 14 || orderpPayment == 10)//支付方式分別為中國信託信用卡紅利折抵、10%、20%、中國信託信用卡
                    {
                        creditcard_Money = Math.Round(Convert.ToInt32(tableTemp.Rows[i]["deduction"]) * 0.025);
                    }
                    else if (orderpPayment == 13)
                    {
                        creditcard_Money = Math.Round(Convert.ToInt32(tableTemp.Rows[i]["deduction"]) * 0.018);
                    }
                    if (!tempDT.ContainsKey(tableTemp.Rows[i]["vendor_id"].ToString()))
                    {
                        tempDT.Add(tableTemp.Rows[i]["vendor_id"].ToString(), creditcard_Money);
                    }
                    else
                    {
                        tempDT[tableTemp.Rows[i]["vendor_id"].ToString()] = creditcard_Money + Convert.ToDouble(tempDT[tableTemp.Rows[i]["vendor_id"].ToString()]);
                    }

                }
                dtHZ.Columns.Add("供應商編號");
                dtHZ.Columns.Add("供應商編碼");
                dtHZ.Columns.Add("ERP 廠商編號");
                dtHZ.Columns.Add("供應商名稱");
                dtHZ.Columns.Add("供應商簡稱");
                dtHZ.Columns.Add("供應商統編");
                dtHZ.Columns.Add("出貨模式");
                dtHZ.Columns.Add("寄倉模式");
                dtHZ.Columns.Add("商品總售價");
                dtHZ.Columns.Add("商品折扣總價");
                dtHZ.Columns.Add("商品淨售價");
                dtHZ.Columns.Add("商品總成本");
                dtHZ.Columns.Add("一期刷卡費");
                dtHZ.Columns.Add("廠商常溫運費");
                dtHZ.Columns.Add("廠商調度常溫運費");
                dtHZ.Columns.Add("廠商自出低溫運費");
                dtHZ.Columns.Add("廠商調度低溫運費");
                dtHZ.Columns.Add("廠商常溫逆物流");
                dtHZ.Columns.Add("廠商低溫逆物流");
                dtHZ.Columns.Add("廠商業績獎金");
                dtHZ.Columns.Add("寄倉費用");
                dtHZ.Columns.Add("應付廠商款");
                dtHZ.Columns.Add("刷卡費用");
                dtHZ.Columns.Add("廠商成本小計");
                dtHZ.Columns.Add("淨毛利(淨售價-成本小計)");
                dtHZ.Columns.Add("淨毛利率(毛利/淨售價)");
                dtHZ.Columns.Add("總毛利(商品總售價-成本小計)");
                dtHZ.Columns.Add("總毛利率(毛利/總售價)");
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    if (Convert.ToInt32(_dt.Rows[i]["m_product_cost"]) != 0)
                    {
                        DataRow dr = dtHZ.NewRow();
                        dr[0] = _dt.Rows[i]["vendor_id"];
                        dr[1] = _dt.Rows[i]["vendor_code"];
                        dr[2] = _dt.Rows[i]["erp_id"];
                        dr[3] = _dt.Rows[i]["vendor_name_full"];
                        dr[4] = _dt.Rows[i]["vendor_name_simple"];
                        dr[5] = _dt.Rows[i]["vendor_invoice"];
                        if (Convert.ToInt32(_dt.Rows[i]["dispatch"]) == 1)
                        {
                            dr[6] = "調度倉";
                        }
                        else
                        {
                            dr[6] = "自行出貨";
                        }
                        if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 1)
                        {
                            dr[7] = "非寄倉";
                        }
                        else if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 2)
                        {
                            dr[7] = "寄倉";
                        }
                        dr[8] = _dt.Rows[i]["m_product_money"].ToString();
                        dr[9] = (Convert.ToInt32(_dt.Rows[i]["m_product_money"]) - Convert.ToInt32(_dt.Rows[i]["m_all_deduction"])).ToString();
                        dr[10] = _dt.Rows[i]["m_all_deduction"];
                        dr[11] = _dt.Rows[i]["m_product_cost"];
                        //dr[12] = "-" + _dt.Rows[i]["m_money_creditcard_1"];
                        if (Convert.ToInt32(_dt.Rows[i]["m_money_creditcard_1"].ToString()) != 0)
                        {
                            dr[12] = "-" + _dt.Rows[i]["m_money_creditcard_1"].ToString();
                        }
                        else
                        {
                            dr[12] = 0;
                        }
                        dr[13] = _dt.Rows[i]["m_freight_delivery_normal"];
                        dr[14] = _dt.Rows[i]["m_dispatch_freight_delivery_normal"];
                        dr[15] = _dt.Rows[i]["m_freight_delivery_low"];
                        dr[16] = _dt.Rows[i]["m_dispatch_freight_delivery_low"];
                        if (_dt.Rows[i]["m_freight_return_normal"].ToString() == "0")
                        {
                            dr[17] = _dt.Rows[i]["m_freight_return_normal"];
                        }
                        else
                        {
                            dr[17] = "-" + _dt.Rows[i]["m_freight_return_normal"];
                        }
                        if (_dt.Rows[i]["m_freight_return_low"].ToString() == "0")
                        {
                            dr[18] = _dt.Rows[i]["m_freight_return_low"];
                        }
                        else
                        {
                            dr[18] = "-" + _dt.Rows[i]["m_freight_return_low"];
                        }
                        dr[19] = "?";
                        if (_dt.Rows[i]["m_bag_check_money"].ToString() == "0")
                        {
                            dr[20] = _dt.Rows[i]["m_bag_check_money"];
                        }
                        else
                        {
                            dr[20] = "-" + _dt.Rows[i]["m_bag_check_money"];
                        }
                        int m_product_cost = Convert.ToInt32(_dt.Rows[i]["m_product_cost"]) - (Convert.ToInt32(_dt.Rows[i]["m_money_creditcard_1"]) + Convert.ToInt32(_dt.Rows[i]["m_freight_return_normal"]) + Convert.ToInt32(_dt.Rows[i]["m_freight_return_low"]));
                        m_product_cost = m_product_cost - Convert.ToInt32(_dt.Rows[i]["m_bag_check_money"]);
                        m_product_cost = m_product_cost + (Convert.ToInt32(_dt.Rows[i]["m_freight_delivery_normal"]) - Convert.ToInt32(_dt.Rows[i]["m_freight_delivery_low"]));
                        dr[21] = m_product_cost;
                        double temp1 = 0;
                        if (tempDT.ContainsKey(_dt.Rows[i]["vendor_id"].ToString()))
                        {
                            temp1 = tempDT[_dt.Rows[i]["vendor_id"].ToString()];
                        }
                        dr[22] = temp1.ToString();
                        dr[23] = Convert.ToInt32(_dt.Rows[i]["m_product_cost"]) + temp1;
                        dr[24] = Convert.ToInt32(_dt.Rows[i]["m_all_deduction"]) - Convert.ToInt32(_dt.Rows[i]["m_product_cost"]);
                        if (Convert.ToInt32(_dt.Rows[i]["m_all_deduction"]) != 0)
                        {
                            dr[25] = (Convert.ToInt32(_dt.Rows[i]["m_all_deduction"]) - Convert.ToInt32(_dt.Rows[i]["m_product_cost"])) / Convert.ToDecimal(_dt.Rows[i]["m_all_deduction"]);
                        }
                        else
                        {
                            dr[25] = "";
                        }
                        dr[26] = Convert.ToInt32(_dt.Rows[i]["m_product_money"]) - Convert.ToInt32(_dt.Rows[i]["m_product_cost"]);
                        if (Convert.ToDecimal(_dt.Rows[i]["m_product_money"]) != 0)
                        {
                            dr[27] = (Convert.ToInt32(_dt.Rows[i]["m_product_money"]) - Convert.ToInt32(_dt.Rows[i]["m_product_cost"])) / Convert.ToDecimal(_dt.Rows[i]["m_product_money"]);
                        }
                        else
                        {
                            dr[27] = "";
                        }
                        dtHZ.Rows.Add(dr);
                    }

                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = "供應商對帳總表" + VAMQuery.account_year + "-" + VAMQuery.account_month + ".xls";
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

        #region 查詢供應商業績明細
        public HttpResponseBase GetVendorAccountMonthDetail()
        {

            VendorAccountDetailQuery query = new VendorAccountDetailQuery();
            List<VendorAccountDetailQuery> stores = new List<VendorAccountDetailQuery>();
            List<string> list = GetTime(uint.Parse(Request.Params["dateone"]), uint.Parse(Request.Params["datetwo"]));
            query.vendor_id = uint.Parse(Request.Params["vendorid"]);
            query.search_start_time = list[0];
            query.search_end_time = list[1];
            string json = string.Empty;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量

                _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
                _iupc = new IupcMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _IVAMMgr.GetVendorAccountMonthDetailList(query, out totalCount);
                foreach (var item in stores)
                {
                    item.order_createdates = CommonFunction.GetNetTime(item.order_createdate);
                    item.account_dates = CommonFunction.GetNetTime(item.account_date);
                    item.slave_date_deliverys = CommonFunction.GetNetTime(item.slave_date_delivery);
                    if (item.item_mode == 2)
                    {
                        item.buy_num *= item.parent_num;
                    }
                    item.search_start_time = query.search_start_time;
                    item.search_end_time = query.search_end_time;
                    item.upc_id = _iupc.Getupc(item.item_id.ToString(), "1");
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
        #endregion


        //#region 業績報表明細Excel信息+void ExportVendorAccountMonthDetail()
        //public void ExportVendorAccountMonthDetail()
        //{
        //    string json = string.Empty;
        //    DataTable _dt = new DataTable();
        //    DataTable dtHZ = new DataTable();
        //    VendorAccountDetailQuery query = new VendorAccountDetailQuery();
        //    VendorAccountMonthQuery VAMQuery = new VendorAccountMonthQuery();
        //    VendorQuery vendorQuery = new VendorQuery();
        //    try
        //    {
        //        List<string> list = GetTime(uint.Parse(Request.Params["dateone"]), uint.Parse(Request.Params["datetwo"]));
        //        _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
        //        Dictionary<int, DataTable> tempDT = new Dictionary<int, DataTable>();
        //        vendorQuery.vendor_id = Convert.ToUInt32(Request.Params["vendorid"]);
        //        VAMQuery.vendor_id = Convert.ToUInt32(Request.Params["vendorid"]);
        //        VAMQuery.account_year = Convert.ToUInt32(Request.Params["dateOne"]);
        //        VAMQuery.account_month = Convert.ToUInt32(Request.Params["dateTwo"]);
        //        query.vendor_id = Convert.ToUInt32(Request.Params["vendorid"]);
        //        query.search_start_time = list[0];
        //        query.search_end_time = list[1];
        //        int tempFreightDelivery_Normal = 0;
        //        int tempFreightDelivery_Low = 0;
        //        //調度倉運費
        //        DataTable dt = _IVAMMgr.GetFreightMoney(query, out tempFreightDelivery_Normal, out tempFreightDelivery_Low);
        //        // 查供應商總帳
        //        DataTable tempTemp = _IVAMMgr.GetVendorAccountMonthZongZhang(VAMQuery);
        //        DataTable dtPiCi = _IVAMMgr.BatchOrderDetail(query);
        //        tempDT.Add(0, tempTemp);
        //        //供應商信息
        //        vendorQuery = _IVAMMgr.GetVendorInfoByCon(vendorQuery);
        //        //結帳總金額扣除批次出貨運費
        //        int M_Account_Amount = Convert.ToInt32(tempTemp.Rows[0]["m_account_amount"]);
        //        if (!string.IsNullOrEmpty(tempTemp.Rows[0]["m_bag_check_money"].ToString()))
        //        {
        //            M_Account_Amount = M_Account_Amount - Convert.ToInt32(tempTemp.Rows[0]["m_bag_check_money"]);
        //        }
        //        if (tempFreightDelivery_Normal != 0)
        //        {
        //            M_Account_Amount += tempFreightDelivery_Normal;
        //        }
        //        if (tempFreightDelivery_Low != 0)
        //        {
        //            M_Account_Amount += tempFreightDelivery_Low;
        //        }
        //        string dispatch = string.Empty;
        //        if (vendorQuery.dispatch == 1)
        //        {
        //            dispatch = "調度倉";
        //        }
        //        else
        //        {
        //            dispatch = "非調度倉";
        //        }

        //        #region 匯出樣式
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        DataRow dr1 = dtHZ.NewRow();
        //        DataRow dr2 = dtHZ.NewRow();
        //        DataRow dr3 = dtHZ.NewRow();
        //        DataRow dr4 = dtHZ.NewRow();
        //        DataRow dr5 = dtHZ.NewRow();
        //        DataRow dr6 = dtHZ.NewRow();
        //        DataRow dr7 = dtHZ.NewRow();
        //        DataRow dr8 = dtHZ.NewRow();
        //        DataRow dr9 = dtHZ.NewRow();
        //        string strTemp = string.Empty;
        //        strTemp = "吉甲地市集" + Request.Params["dateOne"].ToString() + "年" + Request.Params["dateTwo"].ToString() + "月銷售報表";
        //        dr1[0] = strTemp.ToString();
        //        dr2[0] = "供應商編號：" + vendorQuery.vendor_code;
        //        dr3[0] = "供應商名稱:" + vendorQuery.vendor_name_simple;
        //        dr4[0] = "報表輸出時間:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //        dr5[0] = "結帳金額:" + M_Account_Amount;
        //        dr6[0] = "廠商業績獎金：" + 0 + "   " + "寄倉：" + tempTemp.Rows[0]["m_bag_check_money"].ToString() + "   " + "    " + "調度常溫運費：" + tempFreightDelivery_Normal + "   " + "調度低溫運費：" + tempFreightDelivery_Low;
        //        //dr6[2] = "寄倉：" + tempTemp.Rows[0]["m_bag_check_money"].ToString();
        //        //dr6[3] = "調度常溫運費：" + tempFreightDelivery_Normal;
        //        //dr6[5] = "調度低溫運費：" + tempFreightDelivery_Low;
        //        dr7[0] = "廠商帳款總計：" + (M_Account_Amount - 0);
        //        dr8[0] = "出貨模式：" + dispatch;
        //        dr9[0] = "歸檔日期";
        //        dr9[1] = "訂單日期";
        //        dr9[2] = "出貨日期";
        //        dr9[3] = "付款單編號";
        //        dr9[4] = "訂單編號";
        //        dr9[5] = "商品售價";
        //        dr9[6] = "商品成本";
        //        dr9[7] = "付款方式";
        //        dr9[8] = "一期刷卡費";
        //        dr9[9] = "常溫運費";
        //        dr9[10] = "低溫運費";
        //        dr9[11] = "常溫逆物流";
        //        dr9[12] = "低溫逆物流";
        //        dr9[13] = "結帳金額";
        //        dr9[14] = "商品模式";
        //        dr9[15] = "運費條件";
        //        dr9[16] = "組合方式";
        //        dr9[17] = "商品名稱";
        //        dr9[18] = "數量";
        //        dr9[19] = "子商品數量";
        //        dr9[20] = "成本";
        //        dr9[21] = "成本小計";
        //        dr9[22] = "活動價成本";
        //        dr9[23] = "滿額滿件折扣";
        //        dr9[24] = "售價";
        //        dr9[25] = "售價小計";
        //        dr9[26] = "退貨日期";
        //        dr9[27] = "商品狀態";
        //        dr9[28] = "父親商品編號";
        //        dr9[29] = "寄倉費";
        //        dr9[30] = "稅別";
        //        dr9[31] = "管理員備註";
        //        dtHZ.Rows.Add(dr1);
        //        dtHZ.Rows.Add(dr2);
        //        dtHZ.Rows.Add(dr3);
        //        dtHZ.Rows.Add(dr4);
        //        dtHZ.Rows.Add(dr5);
        //        dtHZ.Rows.Add(dr6);
        //        dtHZ.Rows.Add(dr7);
        //        dtHZ.Rows.Add(dr8);
        //        dtHZ.Rows.Add(dr9);
        //        #endregion
        //        _dt = _IVAMMgr.VendorAccountDetailExport(query);
        //        int product_money = 0;
        //        int product_cost = 0;
        //        int order_payment = 0;
        //        int money_creditcard_1 = 0;
        //        int freight_delivery_normal = 0;
        //        int freight_delivery_low = 0;
        //        int freight_return_normal = 0;
        //        int freight_return_low = 0;
        //        int account_amount = 0;
        //        int freeTax = 0;
        //        int taxAmount = 0;
        //        int mianx = 0;
        //        int yingx = 0;
        //        int yings = 0;
        //        string strOrderIds = string.Empty;
        //        for (int i = 0; i < _dt.Rows.Count; i++)
        //        {
        //            DataRow dr = dtHZ.NewRow();
        //            if (i > 0)
        //            {

        //                if (Convert.ToInt32(_dt.Rows[i]["order_id"]) != Convert.ToInt32(_dt.Rows[i - 1]["order_id"]))
        //                {
        //                    strOrderIds += _dt.Rows[i]["order_id"].ToString() + ",";
        //                    dr[0] = _dt.Rows[i]["account_date"].ToString();
        //                    dr[1] = _dt.Rows[i]["order_createdate"].ToString();
        //                    dr[2] = _dt.Rows[i]["slave_date_delivery"].ToString();
        //                    dr[3] = _dt.Rows[i]["order_id"].ToString();
        //                    dr[4] = _dt.Rows[i]["slave_id"].ToString();
        //                    dr[5] = _dt.Rows[i]["product_money"].ToString();
        //                    dr[6] = _dt.Rows[i]["product_cost"].ToString();
        //                    dr[7] = _dt.Rows[i]["parameterName"].ToString();
        //                    if (Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString()) != 0)
        //                    {
        //                        dr[8] = "-" + _dt.Rows[i]["money_creditcard_1"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[8] = 0;
        //                    }
        //                    dr[9] = _dt.Rows[i]["freight_delivery_normal"].ToString();
        //                    dr[10] = _dt.Rows[i]["freight_delivery_low"].ToString();
        //                    if (_dt.Rows[i]["freight_return_normal"].ToString() == "0")
        //                    {
        //                        dr[11] = _dt.Rows[i]["freight_return_normal"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[11] = "-" + _dt.Rows[i]["freight_return_normal"].ToString();

        //                    }
        //                    if (_dt.Rows[i]["freight_return_low"].ToString() == "0")
        //                    {
        //                        dr[12] = _dt.Rows[i]["freight_return_low"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[12] = "-" + _dt.Rows[i]["freight_return_low"].ToString();
        //                    }
        //                    dr[13] = _dt.Rows[i]["account_amount"].ToString();
        //                    if (!string.IsNullOrEmpty(_dt.Rows[i]["free_tax"].ToString()))
        //                    {
        //                        freeTax += Convert.ToInt32(_dt.Rows[i]["free_tax"].ToString());
        //                    }

        //                    if (!string.IsNullOrEmpty(_dt.Rows[i]["tax_amount"].ToString()))
        //                    {
        //                        taxAmount += Convert.ToInt32(_dt.Rows[i]["tax_amount"].ToString());
        //                    }
        //                    product_money += Convert.ToInt32(_dt.Rows[i]["product_money"].ToString());
        //                    product_cost += Convert.ToInt32(_dt.Rows[i]["product_cost"].ToString());
        //                    //order_payment += Convert.ToInt32(_dt.Rows[i]["order_payment"].ToString());原來php程式是所有的付款方式相加，因為是字符串所有加總都是0
        //                    money_creditcard_1 += Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString());
        //                    freight_delivery_normal += Convert.ToInt32(_dt.Rows[i]["freight_delivery_normal"].ToString());
        //                    freight_delivery_low += Convert.ToInt32(_dt.Rows[i]["freight_delivery_low"].ToString());
        //                    freight_return_normal += Convert.ToInt32(_dt.Rows[i]["freight_return_normal"].ToString());
        //                    freight_return_low += Convert.ToInt32(_dt.Rows[i]["freight_return_low"].ToString());
        //                    account_amount += Convert.ToInt32(_dt.Rows[i]["account_amount"].ToString());

        //                }

        //            }
        //            else
        //            {
        //                strOrderIds += _dt.Rows[i]["order_id"].ToString() + ",";
        //                dr[0] = _dt.Rows[i]["account_date"].ToString();
        //                dr[1] = _dt.Rows[i]["order_createdate"].ToString();
        //                dr[2] = _dt.Rows[i]["slave_date_delivery"].ToString();
        //                dr[3] = _dt.Rows[i]["order_id"].ToString();
        //                dr[4] = _dt.Rows[i]["slave_id"].ToString();
        //                dr[5] = _dt.Rows[i]["product_money"].ToString();
        //                dr[6] = _dt.Rows[i]["product_cost"].ToString();
        //                dr[7] = _dt.Rows[i]["parameterName"].ToString();
        //                if (Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString()) != 0)
        //                {
        //                    dr[8] = "-" + _dt.Rows[i]["money_creditcard_1"].ToString();
        //                }
        //                else
        //                {
        //                    dr[8] = 0;
        //                }
        //                dr[9] = _dt.Rows[i]["freight_delivery_normal"].ToString();
        //                dr[10] = _dt.Rows[i]["freight_delivery_low"].ToString();
        //                if (_dt.Rows[i]["freight_return_normal"].ToString() == "0")
        //                {
        //                    dr[11] = _dt.Rows[i]["freight_return_normal"].ToString();
        //                }
        //                else
        //                {
        //                    dr[11] = "-" + _dt.Rows[i]["freight_return_normal"].ToString();
        //                }
        //                if (_dt.Rows[i]["freight_return_low"].ToString() == "0")
        //                {
        //                    dr[12] = _dt.Rows[i]["freight_return_low"].ToString();
        //                }
        //                else
        //                {
        //                    dr[12] = "-" + _dt.Rows[i]["freight_return_low"].ToString();
        //                }
        //                dr[13] = _dt.Rows[i]["account_amount"].ToString();
        //                if (!string.IsNullOrEmpty(_dt.Rows[i]["free_tax"].ToString()))
        //                {
        //                    freeTax += Convert.ToInt32(_dt.Rows[i]["free_tax"].ToString());
        //                }

        //                if (!string.IsNullOrEmpty(_dt.Rows[i]["tax_amount"].ToString()))
        //                {
        //                    taxAmount += Convert.ToInt32(_dt.Rows[i]["tax_amount"].ToString());
        //                }
        //                product_money += Convert.ToInt32(_dt.Rows[i]["product_money"].ToString());
        //                product_cost += Convert.ToInt32(_dt.Rows[i]["product_cost"].ToString());
        //                //order_payment += Convert.ToInt32(_dt.Rows[i]["order_payment"].ToString());
        //                money_creditcard_1 += Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString());
        //                freight_delivery_normal += Convert.ToInt32(_dt.Rows[i]["freight_delivery_normal"].ToString());
        //                freight_delivery_low += Convert.ToInt32(_dt.Rows[i]["freight_delivery_low"].ToString());
        //                freight_return_normal += Convert.ToInt32(_dt.Rows[i]["freight_return_normal"].ToString());
        //                freight_return_low += Convert.ToInt32(_dt.Rows[i]["freight_return_low"].ToString());
        //                account_amount += Convert.ToInt32(_dt.Rows[i]["account_amount"].ToString());
        //            }
        //            if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 1)
        //            {
        //                dr[14] = "自出";
        //            }
        //            else if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 2)
        //            {
        //                dr[14] = "寄倉";
        //            }
        //            else if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 3)
        //            {
        //                dr[14] = "調度倉";
        //            }
        //            string single_cost_subtotal = string.Empty;//成本小計
        //            if (Convert.ToInt32(_dt.Rows[i]["detail_status"]) != 4)//已出貨
        //            {
        //                _dt.Rows[i]["single_money"] = 0;
        //                _dt.Rows[i]["single_cost"] = 0;
        //                single_cost_subtotal = "0";
        //            }
        //            else
        //            {
        //                single_cost_subtotal = (Convert.ToInt32(_dt.Rows[i]["single_cost"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //            }
        //            dr[15] = _dt.Rows[i]["product_freight"];//運送方式
        //            if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 0)
        //            {
        //                dr[16] = "單一";
        //            }
        //            else if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 1)
        //            {
        //                dr[16] = "父";
        //            }
        //            else if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 2)
        //            {
        //                dr[16] = "子";
        //            }
        //            dr[17] = _dt.Rows[i]["product_name"].ToString() + _dt.Rows[i]["product_spec_name"].ToString();
        //            if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 2)
        //            {
        //                dr[18] = "";
        //                dr[19] = _dt.Rows[i]["buy_num"].ToString();
        //                dr[20] = "";
        //                dr[21] = "";
        //                dr[22] = "";
        //                dr[23] = "";
        //                dr[24] = "";
        //                dr[25] = "";
        //            }
        //            else
        //            {
        //                dr[18] = _dt.Rows[i]["buy_num"].ToString();
        //                dr[19] = "";
        //                dr[20] = _dt.Rows[i]["single_cost"].ToString();
        //                dr[21] = single_cost_subtotal;
        //                dr[22] = Convert.ToInt32(_dt.Rows[i]["event_cost"].ToString()) == 0 ? "-" : Convert.ToInt32(_dt.Rows[i]["event_cost"].ToString()).ToString();
        //                dr[23] = _dt.Rows[i]["deduct_account"].ToString();
        //                dr[24] = _dt.Rows[i]["single_money"].ToString();
        //                dr[25] = (Convert.ToInt32(_dt.Rows[i]["single_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //            }
        //            dr[26] = "";
        //            dr[27] = _dt.Rows[i]["order_status_name"].ToString();
        //            dr[28] = _dt.Rows[i]["parent_id"].ToString();
        //            if (Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString()) == 0)
        //            {
        //                dr[29] = (Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //            }
        //            else
        //            {
        //                dr[29] = "-" + (Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //            }
        //            if (Convert.ToInt32(_dt.Rows[i]["tax_type"].ToString()) == 1)
        //            {
        //                dr[30] = "應稅";

        //            }
        //            if (Convert.ToInt32(_dt.Rows[i]["tax_type"].ToString()) == 3)
        //            {
        //                if (_dt.Rows[i]["freight_return_normal"].ToString() == "0" && _dt.Rows[i]["freight_return_low"].ToString() == "0")
        //                {
        //                    dr[30] = "應稅";
        //                }
        //                else
        //                {
        //                    dr[30] = "免稅";
        //                }
        //            }
        //            dr[31] = _dt.Rows[i]["note_admin"].ToString();
        //            dtHZ.Rows.Add(dr);
        //        }
        //        VendorAccountDetailQuery taxQuery = new VendorAccountDetailQuery();
        //        taxQuery.orderIds = strOrderIds.Remove(strOrderIds.LastIndexOf(','));
        //        DataTable dtTax = _IVAMMgr.GetTaxMoney(taxQuery);
        //        for (int i = 0; i < dtTax.Rows.Count; i++)
        //        {
        //            if (Convert.ToInt32(dtTax.Rows[i]["tax_type"]) == 1)
        //            {
        //                yingx += Convert.ToInt32(dtTax.Rows[i]["free_tax"]);
        //                yings += Convert.ToInt32(dtTax.Rows[i]["tax_amount"]);
        //            }
        //            if (Convert.ToInt32(dtTax.Rows[i]["tax_type"]) == 3)
        //            {
        //                mianx += Convert.ToInt32(dtTax.Rows[i]["free_tax"]);
        //            }
        //        }
        //        DataRow dre = dtHZ.NewRow();
        //        dre[0] = "總計";
        //        dre[5] = product_money;
        //        dre[6] = product_cost;
        //        dre[7] = order_payment;
        //        dre[8] = money_creditcard_1;
        //        dre[9] = freight_delivery_normal;
        //        dre[10] = freight_delivery_low;
        //        if (freight_return_normal != 0)
        //        {
        //            dre[11] = "-" + freight_return_normal;
        //        }
        //        else
        //        {
        //            dre[11] = freight_return_normal;
        //        }
        //        if (freight_return_low == 0)
        //        {
        //            dre[12] = freight_return_low;
        //        }
        //        else
        //        {
        //            dre[12] = "-" + freight_return_low;
        //        }
        //        dre[13] = account_amount;
        //        dtHZ.Rows.Add(dre);
        //        DataRow drTt = dtHZ.NewRow();
        //        dtHZ.Rows.Add(drTt);
        //        DataRow drTv = dtHZ.NewRow();
        //        dtHZ.Rows.Add(drTv);
        //        DataRow drT1 = dtHZ.NewRow();
        //        drT1[0] = "※廠商款每月20日結帳，逾20日收到的發票或收據其款項將計入下期支付。";
        //        dtHZ.Rows.Add(drT1);
        //        DataRow drT2 = dtHZ.NewRow();
        //        drT2[0] = "※發票抬頭：吉甲地好市集股份有限公司，統編：25137186。";
        //        dtHZ.Rows.Add(drT2);
        //        DataRow drShuiBei = dtHZ.NewRow();
        //        drShuiBei[13] = "稅別";
        //        drShuiBei[14] = "免稅";
        //        drShuiBei[15] = "應稅";
        //        drShuiBei[16] = "統計";
        //        dtHZ.Rows.Add(drShuiBei);
        //        DataRow DrXS = dtHZ.NewRow();
        //        DrXS[13] = "銷售額";
        //        DrXS[14] = mianx;
        //        DrXS[15] = yingx;
        //        DrXS[16] = "/";
        //        dtHZ.Rows.Add(DrXS);
        //        DataRow DrS = dtHZ.NewRow();
        //        DrS[13] = "稅額";
        //        DrS[14] = 0;
        //        DrS[15] = yings;
        //        DrS[16] = "/";
        //        dtHZ.Rows.Add(DrS);
        //        DataRow DrZong = dtHZ.NewRow();
        //        DrZong[13] = "發票金額";
        //        DrZong[14] = mianx;
        //        DrZong[15] = yings + yingx;
        //        DrZong[16] = mianx + yings + yingx;
        //        dtHZ.Rows.Add(DrZong);
        //        List<DataTable> Elist = new List<DataTable>();
        //        List<string> NameList = new List<string>();
        //        List<bool> comName = new List<bool>();
        //        comName.Add(false);
        //        Elist.Add(dtHZ);
        //        NameList.Add("對賬報表");
        //        DataTable dtYF = new DataTable();
        //        if (dt.Rows.Count > 0)
        //        {
        //            dtYF.Columns.Add("批次出貨單號");
        //            dtYF.Columns.Add("常溫商品總額");
        //            dtYF.Columns.Add("低溫商品總額");
        //            dtYF.Columns.Add("批次出貨明細");
        //            dtYF.Columns.Add("廠商出貨單編號");
        //            dtYF.Columns.Add("出貨時間");
        //            dtYF.Columns.Add("付款單號");
        //            for (int s = 0; s < dtPiCi.Rows.Count; s++)
        //            {
        //                DataRow yfDr = dtYF.NewRow();
        //                yfDr[0] = dtPiCi.Rows[s]["code_num"];
        //                yfDr[1] = dtPiCi.Rows[s]["normal_subtotal"];
        //                yfDr[2] = dtPiCi.Rows[s]["hypothermia_subtotal"];
        //                yfDr[3] = dtPiCi.Rows[s]["code_num"];
        //                yfDr[4] = dtPiCi.Rows[s]["slave_id"];
        //                yfDr[5] = dtPiCi.Rows[s]["deliver_time"];
        //                yfDr[6] = dtPiCi.Rows[s]["order_id"];
        //                dtYF.Rows.Add(yfDr);
        //            }
        //            DataRow tempdr1 = dtYF.NewRow();
        //            tempdr1[0] = "常溫運費補貼:" + tempFreightDelivery_Normal;
        //            dtYF.Rows.Add(tempdr1);
        //            DataRow tempdr2 = dtYF.NewRow();
        //            tempdr2[0] = "低溫運費補貼:" + tempFreightDelivery_Low;
        //            dtYF.Rows.Add(tempdr2);
        //            comName.Add(true);
        //            Elist.Add(dtYF);
        //            NameList.Add("調度倉運費");
        //        }
        //        if (dtHZ.Rows.Count > 0)
        //        {
        //            string fileName = vendorQuery.vendor_id + "-" + vendorQuery.vendor_code + "-供應商對帳報表" + vendorQuery.vendor_name_full + VAMQuery.account_year + "-" + VAMQuery.account_month + ".xls";
        //            MemoryStream ms = ExcelHelperXhf.ExportDTNoColumns(Elist, NameList, comName);
        //            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
        //            Response.BinaryWrite(ms.ToArray());
        //        }
        //        else
        //        {
        //            Response.Write("匯出數據不存在");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,totalCount:0,data:[]}";
        //    }
        //}
        //#endregion

        //#region 總表明細+void ExportVendorAccountMonthAll()
        ///// <summary>
        ///// 總表明細
        ///// </summary>
        //public void ExportVendorAccountMonthAll()
        //{
        //    string json = string.Empty;
        //    DataTable _dt = new DataTable();
        //    DataTable dtHZ = new DataTable();
        //    VendorAccountDetailQuery query = new VendorAccountDetailQuery();
        //    VendorAccountMonthQuery VAMQuery = new VendorAccountMonthQuery();
        //    VendorQuery vendorQuery = new VendorQuery();
        //    List<DataTable> Elist = new List<DataTable>();
        //    List<string> NameList = new List<string>();
        //    List<bool> comName = new List<bool>();
        //    try
        //    {
        //        List<string> list = GetTime(uint.Parse(Request.Params["dateone"]), uint.Parse(Request.Params["datetwo"]));
        //        _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
        //        VAMQuery.account_year = Convert.ToUInt32(Request.Params["dateOne"]);
        //        VAMQuery.account_month = Convert.ToUInt32(Request.Params["dateTwo"]);
        //        query.search_start_time = list[0];
        //        query.search_end_time = list[1];
        //        int tempFreightDelivery_Normal = 0;
        //        int tempFreightDelivery_Low = 0;
        //        #region 定義列
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        dtHZ.Columns.Add("", typeof(String));
        //        #endregion
        //        //供應商信息
        //        DataTable vendorDt = _IVAMMgr.GetVendorAccountMonthInfo(VAMQuery);
        //        DataTable dtPiCi = _IVAMMgr.BatchOrderDetail(query);
        //        for (int m = 0; m < vendorDt.Rows.Count; m++)
        //        {
        //            vendorQuery.vendor_id = Convert.ToUInt32(vendorDt.Rows[m]["vendor_id"]);
        //            VAMQuery.vendor_id = Convert.ToUInt32(vendorDt.Rows[m]["vendor_id"]);
        //            query.vendor_id = Convert.ToUInt32(vendorDt.Rows[m]["vendor_id"]);
        //            //調度倉運費
        //            DataTable dt = _IVAMMgr.GetFreightMoney(query, out tempFreightDelivery_Normal, out tempFreightDelivery_Low);
        //            // 查供應商總帳
        //            DataTable tempTemp = _IVAMMgr.GetVendorAccountMonthZongZhang(VAMQuery);

        //            //tempDT.Add(0, tempTemp);
        //            //供應商信息
        //            vendorQuery = _IVAMMgr.GetVendorInfoByCon(vendorQuery);
        //            //結帳總金額扣除批次出貨運費
        //            int M_Account_Amount = 0;
        //            if (!string.IsNullOrEmpty(tempTemp.Rows[0]["m_account_amount"].ToString()))
        //            {
        //                M_Account_Amount = Convert.ToInt32(tempTemp.Rows[0]["m_account_amount"]);
        //                if (!string.IsNullOrEmpty(tempTemp.Rows[0]["m_bag_check_money"].ToString()))
        //                {
        //                    M_Account_Amount = M_Account_Amount - Convert.ToInt32(tempTemp.Rows[0]["m_bag_check_money"]);
        //                }
        //                if (tempFreightDelivery_Normal != 0)
        //                {
        //                    M_Account_Amount += tempFreightDelivery_Normal;
        //                }
        //                if (tempFreightDelivery_Low != 0)
        //                {
        //                    M_Account_Amount += tempFreightDelivery_Low;
        //                }
        //            }

        //            string dispatch = string.Empty;
        //            if (vendorQuery.dispatch == 1)
        //            {
        //                dispatch = "調度倉";
        //            }
        //            else
        //            {
        //                dispatch = "非調度倉";
        //            }

        //            #region 匯出樣式
        //            DataRow dr1 = dtHZ.NewRow();
        //            DataRow dr2 = dtHZ.NewRow();
        //            DataRow dr3 = dtHZ.NewRow();
        //            DataRow dr4 = dtHZ.NewRow();
        //            DataRow dr5 = dtHZ.NewRow();
        //            DataRow dr6 = dtHZ.NewRow();
        //            DataRow dr7 = dtHZ.NewRow();
        //            DataRow dr8 = dtHZ.NewRow();
        //            DataRow dr9 = dtHZ.NewRow();
        //            dr1[0] = "吉甲地市集" + Request.Params["dateOne"].ToString() + "年" + Request.Params["dateTwo"].ToString() + "月銷售報表";
        //            //dr1[0] = "吉甲地市集" + 2014 + "年" + 2 + "月銷售報表";
        //            dr2[0] = "供應商編號：" + vendorQuery.vendor_code;
        //            dr3[0] = "供應商名稱:" + vendorQuery.vendor_name_simple;
        //            dr4[0] = "報表輸出時間:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            dr5[0] = "結帳金額:" + M_Account_Amount;
        //            dr6[0] = "廠商業績獎金：" + 0 + "   " + "寄倉：" + tempTemp.Rows[0]["m_bag_check_money"].ToString() + "   " + "    " + "調度常溫運費：" + tempFreightDelivery_Normal + "   " + "調度低溫運費：" + tempFreightDelivery_Low; ;
        //            //dr6[2] = "寄倉：" + tempTemp.Rows[0]["m_bag_check_money"].ToString();
        //            //dr6[3] = "調度常溫運費：" + tempFreightDelivery_Normal;
        //            //dr6[5] = "調度低溫運費：" + tempFreightDelivery_Low;
        //            dr7[0] = "廠商帳款總計：" + (M_Account_Amount - 0);
        //            dr8[0] = "出貨模式：" + dispatch;
        //            dr9[0] = "歸檔日期";
        //            dr9[1] = "訂單日期";
        //            dr9[2] = "出貨日期";
        //            dr9[3] = "付款單編號";
        //            dr9[4] = "訂單編號";
        //            dr9[5] = "商品售價";
        //            dr9[6] = "商品成本";
        //            dr9[7] = "付款方式";
        //            dr9[8] = "一期刷卡費";
        //            dr9[9] = "常溫運費";
        //            dr9[10] = "低溫運費";
        //            dr9[11] = "常溫逆物流";
        //            dr9[12] = "低溫逆物流";
        //            dr9[13] = "結帳金額";
        //            dr9[14] = "商品模式";
        //            dr9[15] = "運費條件";
        //            dr9[16] = "組合方式";
        //            dr9[17] = "商品名稱";
        //            dr9[18] = "數量";
        //            dr9[19] = "子商品數量";
        //            dr9[20] = "成本";
        //            dr9[21] = "成本小計";
        //            dr9[22] = "活動價成本";
        //            dr9[23] = "滿額滿件折扣";
        //            dr9[24] = "售價";
        //            dr9[25] = "售價小計";
        //            dr9[26] = "退貨日期";
        //            dr9[27] = "商品狀態";
        //            dr9[28] = "寄倉費用";
        //            dr9[29] = "稅別";
        //            dr9[30] = "管理員備註";
        //            dtHZ.Rows.Add(dr1);
        //            dtHZ.Rows.Add(dr2);
        //            dtHZ.Rows.Add(dr3);
        //            dtHZ.Rows.Add(dr4);
        //            dtHZ.Rows.Add(dr5);
        //            dtHZ.Rows.Add(dr6);
        //            dtHZ.Rows.Add(dr7);
        //            dtHZ.Rows.Add(dr8);
        //            dtHZ.Rows.Add(dr9);
        //            #endregion
        //            _dt = _IVAMMgr.VendorAccountDetailExport(query);
        //            int product_money = 0;
        //            int product_cost = 0;
        //            int order_payment = 0;
        //            int money_creditcard_1 = 0;
        //            int freight_delivery_normal = 0;
        //            int freight_delivery_low = 0;
        //            int freight_return_normal = 0;
        //            int freight_return_low = 0;
        //            int account_amount = 0;
        //            int mianx = 0;
        //            int yingx = 0;
        //            int yings = 0;
        //            string strOrderIds = string.Empty;
        //            for (int i = 0; i < _dt.Rows.Count; i++)
        //            {
        //                DataRow dr = dtHZ.NewRow();
        //                if (i > 0)
        //                {
        //                    if (Convert.ToInt32(_dt.Rows[i]["order_id"]) != Convert.ToInt32(_dt.Rows[i - 1]["order_id"]))
        //                    {
        //                        strOrderIds += _dt.Rows[i]["order_id"].ToString() + ",";
        //                        dr[0] = _dt.Rows[i]["account_date"].ToString();
        //                        dr[1] = _dt.Rows[i]["order_createdate"].ToString();
        //                        dr[2] = _dt.Rows[i]["slave_date_delivery"].ToString();
        //                        dr[3] = _dt.Rows[i]["order_id"].ToString();
        //                        dr[4] = _dt.Rows[i]["slave_id"].ToString();
        //                        dr[5] = _dt.Rows[i]["product_money"].ToString();
        //                        dr[6] = _dt.Rows[i]["product_cost"].ToString();
        //                        dr[7] = _dt.Rows[i]["parameterName"].ToString();
        //                        if (Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString()) != 0)
        //                        {
        //                            dr[8] = "-" + _dt.Rows[i]["money_creditcard_1"].ToString();
        //                        }
        //                        else
        //                        {
        //                            dr[8] = 0;
        //                        }
        //                        dr[9] = _dt.Rows[i]["freight_delivery_normal"].ToString();
        //                        dr[10] = _dt.Rows[i]["freight_delivery_low"].ToString();
        //                        if (_dt.Rows[i]["freight_return_normal"].ToString() == "0")
        //                        {
        //                            dr[11] = _dt.Rows[i]["freight_return_normal"].ToString();
        //                        }
        //                        else
        //                        {
        //                            dr[11] = "-" + _dt.Rows[i]["freight_return_normal"].ToString();
        //                        }
        //                        if (_dt.Rows[i]["freight_return_low"].ToString() == "0")
        //                        {
        //                            dr[12] = _dt.Rows[i]["freight_return_low"].ToString();
        //                        }
        //                        else
        //                        {
        //                            dr[12] = "-" + _dt.Rows[i]["freight_return_low"].ToString();
        //                        }
        //                        dr[13] = _dt.Rows[i]["account_amount"].ToString();
        //                        product_money += Convert.ToInt32(_dt.Rows[i]["product_money"].ToString());
        //                        product_cost += Convert.ToInt32(_dt.Rows[i]["product_cost"].ToString());
        //                        order_payment += Convert.ToInt32(_dt.Rows[i]["order_payment"].ToString());
        //                        money_creditcard_1 += Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString());
        //                        freight_delivery_normal += Convert.ToInt32(_dt.Rows[i]["freight_delivery_normal"].ToString());
        //                        freight_delivery_low += Convert.ToInt32(_dt.Rows[i]["freight_delivery_low"].ToString());
        //                        freight_return_normal += Convert.ToInt32(_dt.Rows[i]["freight_return_normal"].ToString());
        //                        freight_return_low += Convert.ToInt32(_dt.Rows[i]["freight_return_low"].ToString());
        //                        account_amount += Convert.ToInt32(_dt.Rows[i]["account_amount"].ToString());
        //                    }
        //                }
        //                else
        //                {
        //                    strOrderIds += _dt.Rows[i]["order_id"].ToString() + ",";
        //                    dr[0] = _dt.Rows[i]["account_date"].ToString();
        //                    dr[1] = _dt.Rows[i]["order_createdate"].ToString();
        //                    dr[2] = _dt.Rows[i]["slave_date_delivery"].ToString();
        //                    dr[3] = _dt.Rows[i]["order_id"].ToString();
        //                    dr[4] = _dt.Rows[i]["slave_id"].ToString();
        //                    dr[5] = _dt.Rows[i]["product_money"].ToString();
        //                    dr[6] = _dt.Rows[i]["product_cost"].ToString();
        //                    dr[7] = _dt.Rows[i]["parameterName"].ToString();
        //                    if (Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString()) != 0)
        //                    {
        //                        dr[8] = "-" + _dt.Rows[i]["money_creditcard_1"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[8] = 0;
        //                    }
        //                    dr[9] = _dt.Rows[i]["freight_delivery_normal"].ToString();
        //                    dr[10] = _dt.Rows[i]["freight_delivery_low"].ToString();
        //                    if (_dt.Rows[i]["freight_return_normal"].ToString() == "0")
        //                    {
        //                        dr[11] = _dt.Rows[i]["freight_return_normal"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[11] = "-" + _dt.Rows[i]["freight_return_normal"].ToString();
        //                    }
        //                    if (_dt.Rows[i]["freight_return_low"].ToString() == "0")
        //                    {
        //                        dr[12] = _dt.Rows[i]["freight_return_low"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[12] = "-" + _dt.Rows[i]["freight_return_low"].ToString();
        //                    }
        //                    dr[13] = _dt.Rows[i]["account_amount"].ToString();
        //                    product_money += Convert.ToInt32(_dt.Rows[i]["product_money"].ToString());
        //                    product_cost += Convert.ToInt32(_dt.Rows[i]["product_cost"].ToString());
        //                    order_payment += Convert.ToInt32(_dt.Rows[i]["order_payment"].ToString());
        //                    money_creditcard_1 += Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString());
        //                    freight_delivery_normal += Convert.ToInt32(_dt.Rows[i]["freight_delivery_normal"].ToString());
        //                    freight_delivery_low += Convert.ToInt32(_dt.Rows[i]["freight_delivery_low"].ToString());
        //                    freight_return_normal += Convert.ToInt32(_dt.Rows[i]["freight_return_normal"].ToString());
        //                    freight_return_low += Convert.ToInt32(_dt.Rows[i]["freight_return_low"].ToString());
        //                    account_amount += Convert.ToInt32(_dt.Rows[i]["account_amount"].ToString());
        //                }

        //                if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 1)
        //                {
        //                    dr[14] = "自出";
        //                }
        //                else if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 2)
        //                {
        //                    dr[14] = "寄倉";
        //                }
        //                else if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 3)
        //                {
        //                    dr[14] = "調度倉";
        //                }
        //                string single_cost_subtotal = string.Empty;//成本小計
        //                if (Convert.ToInt32(_dt.Rows[i]["detail_status"]) != 4)//已出貨
        //                {
        //                    _dt.Rows[i]["single_money"] = 0;
        //                    _dt.Rows[i]["single_cost"] = 0;
        //                    single_cost_subtotal = "0";
        //                }
        //                else
        //                {
        //                    single_cost_subtotal = (Convert.ToInt32(_dt.Rows[i]["single_cost"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();

        //                }
        //                dr[15] = _dt.Rows[i]["product_freight"];
        //                if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 0)
        //                {
        //                    dr[16] = "單一";
        //                }
        //                if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 1)
        //                {
        //                    dr[16] = "父";
        //                }
        //                if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 2)
        //                {
        //                    dr[16] = "子";
        //                }
        //                dr[17] = _dt.Rows[i]["product_name"].ToString() + _dt.Rows[i]["product_spec_name"].ToString();
        //                if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 2)
        //                {
        //                    dr[18] = "";
        //                    dr[19] = dt.Rows[i]["buy_num"].ToString();
        //                    dr[20] = "";
        //                    dr[21] = "";
        //                    dr[22] = "";
        //                    dr[23] = "";
        //                    dr[24] = "";
        //                    dr[25] = "";
        //                }
        //                else
        //                {
        //                    dr[18] = _dt.Rows[i]["buy_num"].ToString();
        //                    dr[19] = "";
        //                    dr[20] = _dt.Rows[i]["single_cost"].ToString();
        //                    dr[21] = single_cost_subtotal;
        //                    dr[22] = Convert.ToInt32(_dt.Rows[i]["event_cost"].ToString()) == 0 ? "-" : Convert.ToInt32(_dt.Rows[i]["event_cost"].ToString()).ToString();
        //                    dr[23] = _dt.Rows[i]["deduct_account"].ToString();
        //                    dr[24] = _dt.Rows[i]["single_money"].ToString();
        //                    dr[25] = (Convert.ToInt32(_dt.Rows[i]["single_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //                }
        //                dr[26] = "";
        //                dr[27] = _dt.Rows[i]["order_status_name"].ToString();
        //                if (Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString()) == 0)
        //                {
        //                    dr[28] = (Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //                }
        //                else
        //                {
        //                    dr[28] = "-" + (Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //                }
        //                if (Convert.ToInt32(_dt.Rows[i]["tax_type"].ToString()) == 1)
        //                {
        //                    dr[29] = "應稅";
        //                }
        //                if (Convert.ToInt32(_dt.Rows[i]["tax_type"].ToString()) == 3)
        //                {
        //                    if (_dt.Rows[i]["freight_return_normal"].ToString() != "0" && _dt.Rows[i]["freight_return_low"].ToString() != "0")
        //                    {
        //                        dr[29] = "應稅";
        //                    }
        //                    else
        //                    {
        //                        dr[29] = "免稅";
        //                    }
        //                }
        //                dr[30] = _dt.Rows[i]["note_admin"].ToString();
        //                dtHZ.Rows.Add(dr);
        //            }

        //            //DataRow dre = dtHZ.NewRow();
        //            //dre[0] = "總計";
        //            //dre[5] = product_money;
        //            //dre[6] = product_cost;
        //            //dre[7] = order_payment;
        //            //dre[8] = money_creditcard_1;
        //            //dre[9] = freight_delivery_normal;
        //            //dre[10] = freight_delivery_low;
        //            //if (freight_return_normal == 0)
        //            //{
        //            //    dre[11] = freight_return_normal;
        //            //}
        //            //else
        //            //{
        //            //    dre[11] = "-" + freight_return_normal;
        //            //}
        //            //if (freight_return_low == 0)
        //            //{
        //            //    dre[12] = freight_return_low;
        //            //}
        //            //else
        //            //{
        //            //    dre[12] = "-" + freight_return_low;
        //            //}
        //            //dre[13] = account_amount;
        //            //dtHZ.Rows.Add(dre);
        //            VendorAccountDetailQuery taxQuery = new VendorAccountDetailQuery();
        //            if (!string.IsNullOrEmpty(strOrderIds))
        //            {
        //                taxQuery.orderIds = strOrderIds.Remove(strOrderIds.LastIndexOf(','));
        //                DataTable dtTax = _IVAMMgr.GetTaxMoney(taxQuery);
        //                for (int i = 0; i < dtTax.Rows.Count; i++)
        //                {
        //                    if (Convert.ToInt32(dtTax.Rows[i]["tax_type"]) == 1)
        //                    {
        //                        yingx += Convert.ToInt32(dtTax.Rows[i]["free_tax"]);
        //                        yings += Convert.ToInt32(dtTax.Rows[i]["tax_amount"]);
        //                    }
        //                    if (Convert.ToInt32(dtTax.Rows[i]["tax_type"]) == 3)
        //                    {
        //                        mianx += Convert.ToInt32(dtTax.Rows[i]["free_tax"]);
        //                    }
        //                }
        //            }
        //            DataRow drTt = dtHZ.NewRow();
        //            dtHZ.Rows.Add(drTt);
        //            DataRow drTv = dtHZ.NewRow();
        //            dtHZ.Rows.Add(drTv);
        //            //if (m == vendorDt.Rows.Count - 1)
        //            //{
        //            //    DataRow drTt = dtHZ.NewRow();
        //            //    dtHZ.Rows.Add(drTt);
        //            //    DataRow drTv = dtHZ.NewRow();
        //            //    dtHZ.Rows.Add(drTv);
        //            //    DataRow drT1 = dtHZ.NewRow();
        //            //    drT1[0] = "※廠商款每月20日結帳，逾20日收到的發票或收據其款項將計入下期支付。";
        //            //    dtHZ.Rows.Add(drT1);
        //            //    DataRow drT2 = dtHZ.NewRow();
        //            //    drT2[0] = "※發票抬頭：吉甲地好市集股份有限公司，統編：25137186。";
        //            //    dtHZ.Rows.Add(drT2);
        //            //    DataRow drShuiBei = dtHZ.NewRow();
        //            //    drShuiBei[13] = "稅別";
        //            //    drShuiBei[14] = "免稅";
        //            //    drShuiBei[15] = "應稅";
        //            //    drShuiBei[16] = "統計";
        //            //    dtHZ.Rows.Add(drShuiBei);
        //            //    DataRow DrXS = dtHZ.NewRow();
        //            //    DrXS[13] = "銷售額";
        //            //    DrXS[14] = mianx;
        //            //    DrXS[15] = yingx;
        //            //    DrXS[16] = "/";
        //            //    dtHZ.Rows.Add(DrXS);
        //            //    DataRow DrS = dtHZ.NewRow();
        //            //    DrS[13] = "稅額";
        //            //    DrS[14] = 0;
        //            //    DrS[15] = yings;
        //            //    DrS[16] = "/";
        //            //    dtHZ.Rows.Add(DrS);
        //            //    DataRow DrZong = dtHZ.NewRow();
        //            //    DrZong[13] = "發票金額";
        //            //    DrZong[14] = mianx;
        //            //    DrZong[15] = yings + yingx;
        //            //    DrZong[16] = mianx + yings + yingx;
        //            //    dtHZ.Rows.Add(DrZong);
        //            //}
        //            //else
        //            //{
        //            //    DataRow drShuiBei = dtHZ.NewRow();
        //            //    drShuiBei[13] = "稅別";
        //            //    drShuiBei[14] = "免稅";
        //            //    drShuiBei[15] = "應稅";
        //            //    drShuiBei[16] = "統計";
        //            //    dtHZ.Rows.Add(drShuiBei);
        //            //    DataRow DrXS = dtHZ.NewRow();
        //            //    DrXS[13] = "銷售額";
        //            //    DrXS[14] = mianx;
        //            //    DrXS[15] = yingx;
        //            //    DrXS[16] = "/";
        //            //    dtHZ.Rows.Add(DrXS);
        //            //    DataRow DrS = dtHZ.NewRow();
        //            //    DrS[13] = "稅額";
        //            //    DrS[14] = 0;
        //            //    DrS[15] = yings;
        //            //    DrS[16] = "/";
        //            //    dtHZ.Rows.Add(DrS);
        //            //    DataRow DrZong = dtHZ.NewRow();
        //            //    DrZong[13] = "發票金額";
        //            //    DrZong[14] = mianx;
        //            //    DrZong[15] = yings + yingx;
        //            //    DrZong[16] = mianx + yings + yingx;
        //            //    dtHZ.Rows.Add(DrZong);
        //            //}
        //        }

        //        comName.Add(false);
        //        Elist.Add(dtHZ);
        //        NameList.Add("應付金額與商品明細");
        //        DataTable dtYF = new DataTable();
        //        if (dtPiCi.Rows.Count > 0)
        //        {
        //            dtYF.Columns.Add("批次出貨單號");
        //            dtYF.Columns.Add("常溫商品總額");
        //            dtYF.Columns.Add("低溫商品總額");
        //            dtYF.Columns.Add("批次出貨明細");
        //            dtYF.Columns.Add("廠商出貨單編號");
        //            dtYF.Columns.Add("出貨時間");
        //            dtYF.Columns.Add("付款單號");
        //            for (int s = 0; s < dtPiCi.Rows.Count; s++)
        //            {
        //                DataRow yfDr = dtYF.NewRow();
        //                yfDr[0] = dtPiCi.Rows[s]["code_num"];
        //                yfDr[1] = dtPiCi.Rows[s]["normal_subtotal"];
        //                yfDr[2] = dtPiCi.Rows[s]["hypothermia_subtotal"];
        //                yfDr[3] = dtPiCi.Rows[s]["code_num"];
        //                yfDr[4] = dtPiCi.Rows[s]["slave_id"];
        //                yfDr[5] = dtPiCi.Rows[s]["deliver_time"];
        //                yfDr[6] = dtPiCi.Rows[s]["order_id"];
        //                dtYF.Rows.Add(yfDr);
        //            }
        //            DataRow tempdr1 = dtYF.NewRow();
        //            tempdr1[0] = "常溫運費補貼:" + tempFreightDelivery_Normal;
        //            dtYF.Rows.Add(tempdr1);
        //            DataRow tempdr2 = dtYF.NewRow();
        //            tempdr2[0] = "低溫運費補貼:" + tempFreightDelivery_Normal;
        //            dtYF.Rows.Add(tempdr2);
        //            comName.Add(true);
        //            Elist.Add(dtYF);
        //            NameList.Add("調度倉運費");
        //        }
        //        if (dtHZ.Rows.Count > 0)
        //        {
        //            string fileName = "供應商對賬報表-" + VAMQuery.account_year + "-" + VAMQuery.account_month + ".xls";
        //            MemoryStream ms = ExcelHelperXhf.ExportDTNoColumns(Elist, NameList, comName);
        //            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
        //            Response.BinaryWrite(ms.ToArray());
        //        }
        //        else
        //        {
        //            Response.Write("匯出數據不存在");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false,totalCount:0,data:[]}";
        //        Response.Write("不存在");
        //    }
        //}
        //#endregion

        //#region 批次匯出各供應商的業績明細 +AllExportVendorAccountMonthDetail()
        ///// <summary>
        ///// 批次匯出各供應商的業績明細
        ///// </summary>
        //public HttpResponseBase AllExportVendorAccountMonthDetail()
        //{
        //    string json = string.Empty;
        //    List<string> strPath = new List<string>();
        //    VendorQuery vendorQuery = new VendorQuery();
        //    try
        //    {
        //        List<string> list = GetTime(uint.Parse(Request.Params["dateone"]), uint.Parse(Request.Params["datetwo"]));
        //        _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
        //        //Dictionary<int, DataTable> tempDT = new Dictionary<int, DataTable>();
        //        VendorAccountMonthQuery VAMQueryTemp = new VendorAccountMonthQuery();

        //        VAMQueryTemp.account_year = Convert.ToUInt32(Request.Params["dateOne"]);
        //        VAMQueryTemp.account_month = Convert.ToUInt32(Request.Params["dateTwo"]);
        //        DataTable vendorDt = _IVAMMgr.GetVendorAccountMonthInfo(VAMQueryTemp);
        //        for (int l = 0; l < vendorDt.Rows.Count; l++)
        //        {
        //            DataTable _dt = new DataTable();
        //            DataTable dtHZ = new DataTable();
        //            VendorAccountDetailQuery query = new VendorAccountDetailQuery();
        //            VendorAccountMonthQuery VAMQuery = new VendorAccountMonthQuery();
        //            int tempFreightDelivery_Normal = 0;
        //            int tempFreightDelivery_Low = 0;
        //            vendorQuery.vendor_id = Convert.ToUInt32(vendorDt.Rows[l]["vendor_id"]);
        //            VAMQuery.vendor_id = Convert.ToUInt32(vendorDt.Rows[l]["vendor_id"]);
        //            VAMQuery.account_year = VAMQueryTemp.account_year;
        //            VAMQuery.account_month = VAMQueryTemp.account_month;
        //            query.vendor_id = Convert.ToUInt32(vendorDt.Rows[l]["vendor_id"]);
        //            query.search_start_time = list[0];
        //            query.search_end_time = list[1];
        //            //調度倉運費
        //            DataTable dt = _IVAMMgr.GetFreightMoney(query, out tempFreightDelivery_Normal, out tempFreightDelivery_Low);
        //            // 查供應商總帳
        //            DataTable tempTemp = _IVAMMgr.GetVendorAccountMonthZongZhang(VAMQuery);

        //            DataTable dtPiCi = _IVAMMgr.BatchOrderDetail(query);
        //            //tempDT.Add(0, tempTemp);
        //            //供應商信息
        //            vendorQuery = _IVAMMgr.GetVendorInfoByCon(vendorQuery);
        //            //結帳總金額扣除批次出貨運費
        //            int M_Account_Amount = Convert.ToInt32(tempTemp.Rows[0]["m_account_amount"]);
        //            if (!string.IsNullOrEmpty(tempTemp.Rows[0]["m_bag_check_money"].ToString()))
        //            {
        //                M_Account_Amount = M_Account_Amount - Convert.ToInt32(tempTemp.Rows[0]["m_bag_check_money"]);
        //            }
        //            if (tempFreightDelivery_Normal != 0)
        //            {
        //                M_Account_Amount += tempFreightDelivery_Normal;
        //            }
        //            if (tempFreightDelivery_Low != 0)
        //            {
        //                M_Account_Amount += tempFreightDelivery_Low;
        //            }
        //            string dispatch = string.Empty;
        //            if (vendorQuery.dispatch == 1)
        //            {
        //                dispatch = "調度倉";
        //            }
        //            else
        //            {
        //                dispatch = "非調度倉";
        //            }

        //            #region 匯出樣式
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            dtHZ.Columns.Add("", typeof(String));
        //            DataRow dr1 = dtHZ.NewRow();
        //            DataRow dr2 = dtHZ.NewRow();
        //            DataRow dr3 = dtHZ.NewRow();
        //            DataRow dr4 = dtHZ.NewRow();
        //            DataRow dr5 = dtHZ.NewRow();
        //            DataRow dr6 = dtHZ.NewRow();
        //            DataRow dr7 = dtHZ.NewRow();
        //            DataRow dr8 = dtHZ.NewRow();
        //            DataRow dr9 = dtHZ.NewRow();
        //            dr1[0] = "吉甲地市集" + Request.Params["dateOne"].ToString() + "年" + Request.Params["dateTwo"].ToString() + "月銷售報表";
        //            //dr1[0] = "吉甲地市集" + 2014 + "年" + 2 + "月銷售報表";
        //            dr2[0] = "供應商編號：" + vendorQuery.vendor_code;
        //            dr3[0] = "供應商名稱:" + vendorQuery.vendor_name_simple;
        //            dr4[0] = "報表輸出時間:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            dr5[0] = "結帳金額:" + M_Account_Amount;
        //            dr6[0] = "廠商業績獎金：" + 0 + "   " + "寄倉：" + tempTemp.Rows[0]["m_bag_check_money"].ToString() + "   " + "    " + "調度常溫運費：" + tempFreightDelivery_Normal + "   " + "調度低溫運費：" + tempFreightDelivery_Low; ;
        //            //dr6[2] = "寄倉：" + tempTemp.Rows[0]["m_bag_check_money"].ToString();
        //            //dr6[3] = "調度常溫運費：" + tempFreightDelivery_Normal;
        //            //dr6[5] = "調度低溫運費：" + tempFreightDelivery_Low;
        //            dr7[0] = "廠商帳款總計：" + (M_Account_Amount - 0);
        //            dr8[0] = "出貨模式：" + dispatch;
        //            dr9[0] = "歸檔日期";
        //            dr9[1] = "訂單日期";
        //            dr9[2] = "出貨日期";
        //            dr9[3] = "付款單編號";
        //            dr9[4] = "訂單編號";
        //            dr9[5] = "商品售價";
        //            dr9[6] = "商品成本";
        //            dr9[7] = "付款方式";
        //            dr9[8] = "一期刷卡費";
        //            dr9[9] = "常溫運費";
        //            dr9[10] = "低溫運費";
        //            dr9[11] = "常溫逆物流";
        //            dr9[12] = "低溫逆物流";
        //            dr9[13] = "結帳金額";
        //            dr9[14] = "商品模式";
        //            dr9[15] = "運費條件";
        //            dr9[16] = "組合方式";
        //            dr9[17] = "商品名稱";
        //            dr9[18] = "數量";
        //            dr9[19] = "子商品數量";
        //            dr9[20] = "成本";
        //            dr9[21] = "成本小計";
        //            dr9[22] = "活動價成本";
        //            dr9[23] = "滿額滿件折扣";
        //            dr9[24] = "售價";
        //            dr9[25] = "售價小計";
        //            dr9[26] = "退貨日期";
        //            dr9[27] = "商品狀態";
        //            dr9[28] = "父親商品編號";
        //            dr9[29] = "寄倉費";
        //            dr9[30] = "稅別";
        //            dr9[31] = "管理員備註";
        //            dtHZ.Rows.Add(dr1);
        //            dtHZ.Rows.Add(dr2);
        //            dtHZ.Rows.Add(dr3);
        //            dtHZ.Rows.Add(dr4);
        //            dtHZ.Rows.Add(dr5);
        //            dtHZ.Rows.Add(dr6);
        //            dtHZ.Rows.Add(dr7);
        //            dtHZ.Rows.Add(dr8);
        //            dtHZ.Rows.Add(dr9);
        //            #endregion
        //            _dt = _IVAMMgr.VendorAccountDetailExport(query);
        //            int product_money = 0;
        //            int product_cost = 0;
        //            int order_payment = 0;
        //            int money_creditcard_1 = 0;
        //            int freight_delivery_normal = 0;
        //            int freight_delivery_low = 0;
        //            int freight_return_normal = 0;
        //            int freight_return_low = 0;
        //            int account_amount = 0;
        //            //int freeTax = 0;
        //            //int taxAmount = 0;
        //            int mianx = 0;
        //            int yingx = 0;
        //            int yings = 0;
        //            string strOrderIds = string.Empty;
        //            for (int i = 0; i < _dt.Rows.Count; i++)
        //            {
        //                DataRow dr = dtHZ.NewRow();
        //                if (i > 0)
        //                {
        //                    if (Convert.ToInt32(_dt.Rows[i]["order_id"]) != Convert.ToInt32(_dt.Rows[i - 1]["order_id"]))
        //                    {
        //                        strOrderIds += _dt.Rows[i]["order_id"].ToString() + ",";
        //                        dr[0] = _dt.Rows[i]["account_date"].ToString();
        //                        dr[1] = _dt.Rows[i]["order_createdate"].ToString();
        //                        dr[2] = _dt.Rows[i]["slave_date_delivery"].ToString();
        //                        dr[3] = _dt.Rows[i]["order_id"].ToString();
        //                        dr[4] = _dt.Rows[i]["slave_id"].ToString();
        //                        dr[5] = _dt.Rows[i]["product_money"].ToString();
        //                        dr[6] = _dt.Rows[i]["product_cost"].ToString();
        //                        dr[7] = _dt.Rows[i]["parameterName"].ToString();
        //                        if (Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString()) != 0)
        //                        {
        //                            dr[8] = "-" + _dt.Rows[i]["money_creditcard_1"].ToString();
        //                        }
        //                        else
        //                        {
        //                            dr[8] = 0;
        //                        }
        //                        dr[9] = _dt.Rows[i]["freight_delivery_normal"].ToString();
        //                        dr[10] = _dt.Rows[i]["freight_delivery_low"].ToString();
        //                        if (_dt.Rows[i]["freight_return_normal"].ToString() == "0")
        //                        {
        //                            dr[11] = _dt.Rows[i]["freight_return_normal"].ToString();
        //                        }
        //                        else
        //                        {
        //                            dr[11] = "-" + _dt.Rows[i]["freight_return_normal"].ToString();
        //                        }
        //                        if (_dt.Rows[i]["freight_return_low"].ToString() == "0")
        //                        {
        //                            dr[12] = _dt.Rows[i]["freight_return_low"].ToString();
        //                        }
        //                        else
        //                        {
        //                            dr[12] = "-" + _dt.Rows[i]["freight_return_low"].ToString();
        //                        }
        //                        dr[13] = _dt.Rows[i]["account_amount"].ToString();
        //                        product_money += Convert.ToInt32(_dt.Rows[i]["product_money"].ToString());
        //                        product_cost += Convert.ToInt32(_dt.Rows[i]["product_cost"].ToString());
        //                        //order_payment += Convert.ToInt32(_dt.Rows[i]["order_payment"].ToString());
        //                        money_creditcard_1 += Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString());
        //                        freight_delivery_normal += Convert.ToInt32(_dt.Rows[i]["freight_delivery_normal"].ToString());
        //                        freight_delivery_low += Convert.ToInt32(_dt.Rows[i]["freight_delivery_low"].ToString());
        //                        freight_return_normal += Convert.ToInt32(_dt.Rows[i]["freight_return_normal"].ToString());
        //                        freight_return_low += Convert.ToInt32(_dt.Rows[i]["freight_return_low"].ToString());
        //                        account_amount += Convert.ToInt32(_dt.Rows[i]["account_amount"].ToString());
        //                    }

        //                }
        //                else
        //                {
        //                    strOrderIds += _dt.Rows[i]["order_id"].ToString() + ",";
        //                    dr[0] = _dt.Rows[i]["account_date"].ToString();
        //                    dr[1] = _dt.Rows[i]["order_createdate"].ToString();
        //                    dr[2] = _dt.Rows[i]["slave_date_delivery"].ToString();
        //                    dr[3] = _dt.Rows[i]["order_id"].ToString();
        //                    dr[4] = _dt.Rows[i]["slave_id"].ToString();
        //                    dr[5] = _dt.Rows[i]["product_money"].ToString();
        //                    dr[6] = _dt.Rows[i]["product_cost"].ToString();
        //                    dr[7] = _dt.Rows[i]["parameterName"].ToString();
        //                    if (Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString()) != 0)
        //                    {
        //                        dr[8] = "-" + _dt.Rows[i]["money_creditcard_1"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[8] = 0;
        //                    }
        //                    dr[9] = _dt.Rows[i]["freight_delivery_normal"].ToString();
        //                    dr[10] = _dt.Rows[i]["freight_delivery_low"].ToString();
        //                    if (_dt.Rows[i]["freight_return_normal"].ToString() == "0")
        //                    {
        //                        dr[11] = _dt.Rows[i]["freight_return_normal"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[11] = "-" + _dt.Rows[i]["freight_return_normal"].ToString();

        //                    }
        //                    if (_dt.Rows[i]["freight_return_low"].ToString() == "0")
        //                    {
        //                        dr[12] = _dt.Rows[i]["freight_return_low"].ToString();
        //                    }
        //                    else
        //                    {
        //                        dr[12] = "-" + _dt.Rows[i]["freight_return_low"].ToString();
        //                    }
        //                    dr[13] = _dt.Rows[i]["account_amount"].ToString();
        //                    product_money += Convert.ToInt32(_dt.Rows[i]["product_money"].ToString());
        //                    product_cost += Convert.ToInt32(_dt.Rows[i]["product_cost"].ToString());
        //                    //order_payment += Convert.ToInt32(_dt.Rows[i]["order_payment"].ToString());
        //                    money_creditcard_1 += Convert.ToInt32(_dt.Rows[i]["money_creditcard_1"].ToString());
        //                    freight_delivery_normal += Convert.ToInt32(_dt.Rows[i]["freight_delivery_normal"].ToString());
        //                    freight_delivery_low += Convert.ToInt32(_dt.Rows[i]["freight_delivery_low"].ToString());
        //                    freight_return_normal += Convert.ToInt32(_dt.Rows[i]["freight_return_normal"].ToString());
        //                    freight_return_low += Convert.ToInt32(_dt.Rows[i]["freight_return_low"].ToString());
        //                    account_amount += Convert.ToInt32(_dt.Rows[i]["account_amount"].ToString());
        //                }

        //                if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 1)
        //                {
        //                    dr[14] = "自出";
        //                }
        //                else if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 2)
        //                {
        //                    dr[14] = "寄倉";
        //                }
        //                else if (Convert.ToInt32(_dt.Rows[i]["product_mode"]) == 3)
        //                {
        //                    dr[14] = "調度倉";
        //                }
        //                string single_cost_subtotal = string.Empty;//成本小計
        //                if (Convert.ToInt32(_dt.Rows[i]["detail_status"]) != 4)//已出貨
        //                {
        //                    _dt.Rows[i]["single_money"] = 0;
        //                    _dt.Rows[i]["single_cost"] = 0;
        //                    single_cost_subtotal = "0";
        //                }
        //                else
        //                {
        //                    single_cost_subtotal = (Convert.ToInt32(_dt.Rows[i]["single_cost"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //                }
        //                dr[15] = _dt.Rows[i]["product_freight"];//運送方式
        //                if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 0)
        //                {
        //                    dr[16] = "單一";
        //                }
        //                else if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 1)
        //                {
        //                    dr[16] = "父";
        //                }
        //                else if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 2)
        //                {
        //                    dr[16] = "子";
        //                }
        //                dr[17] = _dt.Rows[i]["product_name"].ToString() + _dt.Rows[i]["product_spec_name"].ToString();
        //                if (Convert.ToInt32(_dt.Rows[i]["item_mode"]) == 2)
        //                {
        //                    dr[18] = "";
        //                    dr[19] = _dt.Rows[i]["buy_num"].ToString();
        //                    dr[20] = "";
        //                    dr[21] = "";
        //                    dr[22] = "";
        //                    dr[23] = "";
        //                    dr[24] = "";
        //                    dr[25] = "";
        //                }
        //                else
        //                {
        //                    dr[18] = _dt.Rows[i]["buy_num"].ToString();
        //                    dr[19] = "";
        //                    dr[20] = _dt.Rows[i]["single_cost"].ToString();
        //                    dr[21] = single_cost_subtotal;
        //                    dr[22] = Convert.ToInt32(_dt.Rows[i]["event_cost"].ToString()) == 0 ? "-" : Convert.ToInt32(_dt.Rows[i]["event_cost"].ToString()).ToString();
        //                    dr[23] = _dt.Rows[i]["deduct_account"].ToString();
        //                    dr[24] = _dt.Rows[i]["single_money"].ToString();
        //                    dr[25] = (Convert.ToInt32(_dt.Rows[i]["single_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //                }
        //                dr[26] = "";
        //                dr[27] = _dt.Rows[i]["order_status_name"].ToString();
        //                dr[28] = _dt.Rows[i]["parent_id"].ToString();
        //                if ((Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString() == "0")
        //                {
        //                    dr[29] = (Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //                }
        //                else
        //                {
        //                    dr[29] = "-" + (Convert.ToInt32(_dt.Rows[i]["od_bag_check_money"].ToString()) * Convert.ToInt32(_dt.Rows[i]["buy_num"].ToString())).ToString();
        //                }
        //                if (Convert.ToInt32(_dt.Rows[i]["tax_type"].ToString()) == 1)
        //                {

        //                    dr[30] = "應稅";
        //                }
        //                if (Convert.ToInt32(_dt.Rows[i]["tax_type"].ToString()) == 3)
        //                {
        //                    if (_dt.Rows[i]["freight_return_normal"].ToString() != "0" && _dt.Rows[i]["freight_return_low"].ToString() != "0")
        //                    {
        //                        dr[30] = "應稅";
        //                    }
        //                    else
        //                    {
        //                        dr[30] = "免稅";
        //                    }
        //                }
        //                dr[31] = _dt.Rows[i]["note_admin"].ToString();
        //                dtHZ.Rows.Add(dr);
        //            }
        //            VendorAccountDetailQuery taxQuery = new VendorAccountDetailQuery();
        //            if (!string.IsNullOrEmpty(strOrderIds))
        //            {
        //                taxQuery.orderIds = strOrderIds.Remove(strOrderIds.LastIndexOf(','));
        //                DataTable dtTax = _IVAMMgr.GetTaxMoney(taxQuery);
        //                for (int i = 0; i < dtTax.Rows.Count; i++)
        //                {
        //                    if (Convert.ToInt32(dtTax.Rows[i]["tax_type"]) == 1)
        //                    {
        //                        yingx += Convert.ToInt32(dtTax.Rows[i]["free_tax"]);
        //                        yings += Convert.ToInt32(dtTax.Rows[i]["tax_amount"]);
        //                    }
        //                    if (Convert.ToInt32(dtTax.Rows[i]["tax_type"]) == 3)
        //                    {
        //                        mianx += Convert.ToInt32(dtTax.Rows[i]["free_tax"]);
        //                    }
        //                }
        //            }

        //            DataRow dre = dtHZ.NewRow();
        //            dre[0] = "總計";
        //            dre[5] = product_money;
        //            dre[6] = product_cost;
        //            dre[7] = order_payment;
        //            dre[8] = money_creditcard_1;
        //            dre[9] = freight_delivery_normal;
        //            dre[10] = freight_delivery_low;
        //            if (freight_return_normal == 0)
        //            {
        //                dre[11] = freight_return_normal;
        //            }
        //            else
        //            {
        //                dre[11] = "-" + freight_return_normal;
        //            }
        //            if (freight_return_low == 0)
        //            {
        //                dre[12] = freight_return_low;
        //            }
        //            else
        //            {
        //                dre[12] = "-" + freight_return_low;
        //            }
        //            dre[13] = account_amount;
        //            dtHZ.Rows.Add(dre);
        //            DataRow drTt = dtHZ.NewRow();
        //            dtHZ.Rows.Add(drTt);
        //            DataRow drTv = dtHZ.NewRow();
        //            dtHZ.Rows.Add(drTv);
        //            DataRow drT1 = dtHZ.NewRow();
        //            drT1[0] = "※廠商款每月20日結帳，逾20日收到的發票或收據其款項將計入下期支付。";
        //            dtHZ.Rows.Add(drT1);
        //            DataRow drT2 = dtHZ.NewRow();
        //            drT2[0] = "※發票抬頭：吉甲地好市集股份有限公司，統編：25137186。";
        //            dtHZ.Rows.Add(drT2);
        //            DataRow drShuiBei = dtHZ.NewRow();
        //            drShuiBei[13] = "稅別";
        //            drShuiBei[14] = "免稅";
        //            drShuiBei[15] = "應稅";
        //            drShuiBei[16] = "統計";
        //            dtHZ.Rows.Add(drShuiBei);
        //            DataRow DrXS = dtHZ.NewRow();
        //            DrXS[13] = "銷售額";
        //            DrXS[14] = mianx;
        //            DrXS[15] = yingx;
        //            DrXS[16] = "/";
        //            dtHZ.Rows.Add(DrXS);
        //            DataRow DrS = dtHZ.NewRow();
        //            DrS[13] = "稅額";
        //            DrS[14] = 0;
        //            DrS[15] = yings;
        //            DrS[16] = "/";
        //            dtHZ.Rows.Add(DrS);
        //            DataRow DrZong = dtHZ.NewRow();
        //            DrZong[13] = "發票金額";
        //            DrZong[14] = mianx;
        //            DrZong[15] = yings + yingx;
        //            DrZong[16] = mianx + yings + yingx;
        //            dtHZ.Rows.Add(DrZong);
        //            List<DataTable> Elist = new List<DataTable>();
        //            List<string> NameList = new List<string>();
        //            List<bool> comName = new List<bool>();
        //            comName.Add(false);
        //            Elist.Add(dtHZ);
        //            NameList.Add("對賬報表");
        //            DataTable dtYF = new DataTable();
        //            if (dt.Rows.Count > 0)
        //            {
        //                dtYF.Columns.Add("批次出貨單號");
        //                dtYF.Columns.Add("常溫商品總額");
        //                dtYF.Columns.Add("低溫商品總額");
        //                dtYF.Columns.Add("批次出貨明細");
        //                dtYF.Columns.Add("廠商出貨單編號");
        //                dtYF.Columns.Add("出貨時間");
        //                dtYF.Columns.Add("付款單號");
        //                for (int s = 0; s < dtPiCi.Rows.Count; s++)
        //                {
        //                    DataRow yfDr = dtYF.NewRow();
        //                    yfDr[0] = dtPiCi.Rows[s]["code_num"];
        //                    yfDr[1] = dtPiCi.Rows[s]["normal_subtotal"];
        //                    yfDr[2] = dtPiCi.Rows[s]["hypothermia_subtotal"];
        //                    yfDr[3] = dtPiCi.Rows[s]["code_num"];
        //                    yfDr[4] = dtPiCi.Rows[s]["slave_id"];
        //                    yfDr[5] = dtPiCi.Rows[s]["deliver_time"];
        //                    yfDr[6] = dtPiCi.Rows[s]["order_id"];
        //                    dtYF.Rows.Add(yfDr);
        //                }
        //                DataRow tempdr1 = dtYF.NewRow();
        //                tempdr1[0] = "常溫運費補貼:" + tempFreightDelivery_Normal;
        //                dtYF.Rows.Add(tempdr1);
        //                DataRow tempdr2 = dtYF.NewRow();
        //                tempdr2[0] = "低溫運費補貼:" + tempFreightDelivery_Normal;
        //                dtYF.Rows.Add(tempdr2);
        //                comName.Add(true);
        //                Elist.Add(dtYF);
        //                NameList.Add("調度倉運費");
        //            }
        //            if (_dt.Rows.Count > 0)
        //            {
        //                string fileName = vendorQuery.vendor_id + "-" + vendorQuery.vendor_code + "-供應商對帳報表" + vendorQuery.vendor_name_full + VAMQuery.account_year + "-" + VAMQuery.account_month + ".xls";
        //                MemoryStream ms = ExcelHelperXhf.ExportDTNoColumns(Elist, NameList, comName);
        //                //MemoryStream m = new MemoryStream();

        //                FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/" + fileName), FileMode.OpenOrCreate);
        //                BinaryWriter w = new BinaryWriter(fs);
        //                w.Write(ms.ToArray());
        //                fs.Close();
        //                ms.Close();
        //                strPath.Add(Server.MapPath("../ImportUserIOExcel/" + fileName));
        //            }
        //        }
        //        string strZipPath = Server.MapPath("../ImportUserIOExcel/供應商對賬報表.zip");
        //        string strZipTopDirectoryPath = Server.MapPath("../ImportUserIOExcel/");
        //        int intZipLevel = 6;
        //        string strPassword = "";
        //        //string[] filesOrDirectoriesPaths = new string[] { @"D:\ConsultSystem\mgr\user.txt" };
        //        // Zip(strZipPath, strZipTopDirectoryPath, intZipLevel, strPassword, filesOrDirectoriesPaths);
        //        SharpZipLibHelp szlh = new SharpZipLibHelp();
        //        szlh.Zip(strZipPath, strZipTopDirectoryPath, intZipLevel, strPassword, strPath);
        //        json = "{success:'true'}";
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:false}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion

        public HttpResponseBase VendorName()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            VendorQuery query = new VendorQuery();
            try
            {

                _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["vendor_id"]))
                {
                    query.vendor_id = Convert.ToUInt32(Request.Params["vendor_id"]);
                }
                VendorQuery vendorQuery = _IVAMMgr.GetVendorInfoByCon(query);

                json = "{success:true,'msg':'" + vendorQuery.vendor_name_simple + "'}";//返回json數據
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

        #region 把從列表頁獲取的年，月轉化成php時間
        public List<string> GetTime(uint year, uint month)
        {
            int day = 0;
            int flag;
            string startTime = string.Empty;
            string endTime = string.Empty;
            List<string> list = new List<string>();
            if (year % 4 == 0 && year % 100 != 0 || year % 400 == 0)
            {
                flag = 1;

            }
            else
            {
                flag = 0;

            }
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12: day = 31; break;

                case 4:
                case 6:
                case 9:
                case 11: day = 30; break;
                case 2: day = 28 + flag; break;
            }
            //string s = DateTime.Now.ToString("yyyy-MM-dd ");

            startTime = string.Format(year + "-" + month + "-" + 1 + " 00:00:00");
            startTime = CommonFunction.GetPHPTime(startTime.ToString()).ToString();
            endTime = string.Format(year + "-" + month + "-" + day + " 23:59:59");
            endTime = CommonFunction.GetPHPTime(endTime.ToString()).ToString();
            list.Add(startTime);
            list.Add(endTime);
            return list;


        }
        #endregion

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

        //#region 獲取定單詳細信息
        ///// <summary>
        ///// 獲取定單詳細信息
        ///// </summary>
        ///// <returns></returns>
        //public HttpResponseBase GetOrderDetail()
        //{
        //    string json = string.Empty;
        //    List<OrderDetailQuery> listOrderDetail = new List<OrderDetailQuery>();
        //    OrderDetailQuery query = new OrderDetailQuery();
        //    try
        //    {
        //        _orderDetailMgr = new OrderDetailMgr(mySqlConnectionString);
        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
        //        //if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
        //        //{//查詢條件
        //        //    query.seach_tj = Convert.ToInt32(Request.Params["ddlSel"]);
        //        //}
        //        //if (!string.IsNullOrEmpty(Request.Params["selcontent"]))
        //        //{//查詢條件內容
        //        //    query.content = Convert.ToInt32(Request.Params["selcontent"]);
        //        //}
        //        //if (!string.IsNullOrEmpty(Request.Params["ddtSel"]))
        //        //{//日期條件
        //        //    query.ddtSel = Convert.ToInt32(Request.Params["ddtSel"]);
        //        //}
        //        //if (!string.IsNullOrEmpty(Request.Params["time_start"]))
        //        //{//開始時間
        //        //    query.startdate = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["time_start"].ToString()));
        //        //}
        //        //if (!string.IsNullOrEmpty(Request.Params["time_end"]))
        //        //{//結束時間
        //        //    query.enddate = Convert.ToUInt32(CommonFunction.GetPHPTime(Request.Params["time_end"].ToString()));
        //        //}
        //        //if (!string.IsNullOrEmpty(Request.Params["ddlstatus"]))
        //        //{//開立狀態
        //        //    query.ddlstatus = Convert.ToInt32(Request.Params["ddlstatus"]);
        //        //}
        //        //if (!string.IsNullOrEmpty(Request.Params["ddlinvoice"]))
        //        //{//發票類型
        //        //    query.ddlinvoice = Convert.ToInt32(Request.Params["ddlinvoice"]);
        //        //}
        //        //if (!string.IsNullOrEmpty(Request.Params["ddlqh"]))
        //        //{//是否簽回
        //        //    query.ddlqh = Convert.ToInt32(Request.Params["ddlqh"]);
        //        //}
        //        int totalCount = 0;
        //        //listOrderDetail = _invoiceAllow.QueryAll(query, out totalCount);
        //        listOrderDetail = _orderDetailMgr.GetOrderDetailList(query, out totalCount);
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        //listUser是准备转换的对象
        //        json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(listOrderDetail, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        //#endregion

        //#region 強制開立發票
        ///// <summary>
        ///// 強制開立發票
        ///// </summary>
        ///// <returns></returns>
        //public HttpResponseBase OpenInvoice()
        //{
        //    string json = string.Empty;
        //    string order_id = Request.Params["order_id"];
        //    OrderMaster orderMaster = new OrderMaster();
        //    _orderMasterMgr = new OrderMasterMgr(mySqlConnectionString);
        //    _orderSlaveMgr = new OrderSlaveMgr(mySqlConnectionString);
        //    imrMgr = new InvoiceMasterRecordMgr(mySqlConnectionString);
        //    int totalCount = 0;
        //    try
        //    {
        //        orderMaster = _orderMasterMgr.GetOrderMasterByOrderId(Convert.ToInt32(order_id));
        //        if (orderMaster != null && orderMaster.Invoice_Status == 0)
        //        {
        //            OrderSlaveQuery query = new OrderSlaveQuery();
        //            query.order_id = Convert.ToUInt32(order_id);
        //            query.slave_status_in = "0,2";
        //            DataTable dtOrderSlave = _orderSlaveMgr.GetList(query, out totalCount);
        //            if (dtOrderSlave.Rows.Count == 0)
        //            {
        //                //OrderMaster om = new OrderMaster()
        //                //{

        //                //};
        //                _orderMasterMgr.UpdateOrderToOpenInvoice(Convert.ToInt32(order_id));//開發票前先更新定單狀態
        //                bool flag = imrMgr.ModifyOrderInvoice(Convert.ToInt32(order_id), 1, 1, "create");
        //            }
        //            else
        //            {

        //            }
        //        }
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
        //#endregion

        //#region 銀行匯款資料
        //#region 銀行匯款資料view
        //public ActionResult BankReportIndex()
        //{//銀行匯款資料列表
        //    return View();
        //}
        //public ActionResult YongFengBankReportList()
        //{//永業銀行匯款資料列表
        //    return View();
        //}
        //public ActionResult HuaNanBankReportList()
        //{//華南銀行匯款資料列表
        //    return View();
        //}
        //#endregion

        //#region 永豐資料列表
        //public HttpResponseBase SinopacDetailOneList()
        //{
        //    List<SinopacDetailQuery> stores = new List<SinopacDetailQuery>();

        //    string json = string.Empty;
        //    try
        //    {
        //        SinopacDetailQuery query = new SinopacDetailQuery();

        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量

        //        string selecttype = string.Empty;//查詢條件
        //        #region 查詢條件
        //        if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
        //        {
        //            selecttype = Request.Params["selecttype"].ToString();
        //        }
        //        if (selecttype == "1")
        //        {
        //            if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
        //            {
        //                query.order_id = int.Parse(Request.Params["searchcon"].ToString());
        //            }
        //        }
        //        else if (selecttype == "2")
        //        {
        //            if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
        //            {
        //                query.sinopac_id = Request.Params["searchcon"].ToString();
        //            }
        //        }
        //        #endregion

        //        #region 日期條件一
        //        if (!string.IsNullOrEmpty(Request.Params["timeone"]))
        //        {
        //            query.timeconditionone = int.Parse(Request.Params["timeone"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
        //        {
        //            query.dateconditiononestart = DateTime.Parse(Request.Params["dateOne"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
        //        {
        //            query.dateconditiononeend = DateTime.Parse(Request.Params["dateTwo"].ToString());
        //        }
        //        #endregion

        //        #region 日期條件二
        //        if (!string.IsNullOrEmpty(Request.Params["timetwo"]))
        //        {
        //            query.timeconditiontwo = int.Parse(Request.Params["timetwo"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateThree"]))
        //        {
        //            query.dateconditiontwostart = DateTime.Parse(Request.Params["dateThree"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateFour"]))
        //        {
        //            query.dateconditiontwoend = DateTime.Parse(Request.Params["dateFour"].ToString());
        //        }
        //        #endregion

        //        #region 核對狀態
        //        if (!string.IsNullOrEmpty(Request.Params["error_type"]))
        //        {
        //            query.error = int.Parse(Request.Params["error_type"].ToString());
        //        }
        //        #endregion

        //        #region 群組
        //        if (!string.IsNullOrEmpty(Request.Params["yes_type"]))
        //        {
        //            query.grouptype = int.Parse(Request.Params["yes_type"].ToString());
        //        }
        //        #endregion
        //        _spcdil = new SinopacDetailMgr(mySqlConnectionString);
        //        int totalCount = 0;
        //        stores = _spcdil.GetSinopacDetailInfo(query, out totalCount);
        //        foreach (var item in stores)
        //        {
        //            //獲取時間
        //            item.ocreatedate = CommonFunction.GetNetTime(item.order_createdate);
        //            item.endate = CommonFunction.GetNetTime(item.entday);
        //            item.txdate = CommonFunction.GetNetTime(item.txday);
        //            item.sinopacdate = CommonFunction.GetNetTime(item.sinopac_createdate);
        //        }
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        //listUser是准备转换的对象
        //        json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:true,totalCount:0,data:[]}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion

        //#region 華南銀行列表查詢
        //public HttpResponseBase SinopacDetailTwoList()
        //{
        //    List<OrderPaymentHncbQuery> stores = new List<OrderPaymentHncbQuery>();

        //    string json = string.Empty;
        //    try
        //    {
        //        OrderPaymentHncbQuery query = new OrderPaymentHncbQuery();

        //        query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
        //        query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量

        //        string selecttype = string.Empty;//查詢條件

        //        #region 查詢條件
        //        if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
        //        {
        //            selecttype = Request.Params["selecttype"].ToString();
        //        }
        //        if (selecttype == "1")
        //        {
        //            if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
        //            {
        //                query.order_id = uint.Parse(Request.Params["searchcon"].ToString());
        //            }
        //        }
        //        else if (selecttype == "2")
        //        {
        //            if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
        //            {
        //                query.hncb_id = Request.Params["searchcon"].ToString();
        //            }
        //        }
        //        #endregion

        //        #region 日期條件一
        //        if (!string.IsNullOrEmpty(Request.Params["timeone"]))
        //        {
        //            query.timeconditionOne = int.Parse(Request.Params["timeone"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
        //        {
        //            query.dateconditionone = DateTime.Parse(Request.Params["dateOne"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
        //        {
        //            query.dateconditiontwo = DateTime.Parse(Request.Params["dateTwo"].ToString());
        //        }
        //        #endregion

        //        #region 沖帳狀態
        //        if (!string.IsNullOrEmpty(Request.Params["error_type"]))
        //        {
        //            query.error = uint.Parse(Request.Params["error_type"].ToString());
        //        }
        //        #endregion
        //        _spcdil = new SinopacDetailMgr(mySqlConnectionString);
        //        int totalCount = 0;
        //        stores = _spcdil.GetSinopacDetailTwoInfo(query, out totalCount);
        //        foreach (var item in stores)
        //        {
        //            //獲取時間
        //            item.ocreatedate = CommonFunction.GetNetTime(item.createdate);
        //            item.endate = CommonFunction.GetNetTime(item.entday);
        //            item.txdate = CommonFunction.GetNetTime(item.txtday);

        //        }
        //        IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
        //        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
        //        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        //        //listUser是准备转换的对象
        //        json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //        json = "{success:true,totalCount:0,data:[]}";
        //    }
        //    this.Response.Clear();
        //    this.Response.Write(json);
        //    this.Response.End();
        //    return this.Response;
        //}
        //#endregion

        //#region 永豐銀行匯出
        //public void ExportExcel()
        //{
        //    List<SinopacDetailQuery> stores = new List<SinopacDetailQuery>();
        //    SinopacDetailQuery query = new SinopacDetailQuery();
        //    string json = string.Empty;
        //    DataTable dtHZ = new DataTable();

        //    string newExcelName = string.Empty;
        //    dtHZ.Columns.Add("付款單號", typeof(String));
        //    dtHZ.Columns.Add("虛擬帳號", typeof(String));
        //    dtHZ.Columns.Add("付款單金額", typeof(String));
        //    dtHZ.Columns.Add("繳費金額", typeof(String));
        //    dtHZ.Columns.Add("訂購日期", typeof(String));
        //    dtHZ.Columns.Add("匯款日期", typeof(String));
        //    dtHZ.Columns.Add("入帳日期", typeof(String));
        //    dtHZ.Columns.Add("沖帳日期", typeof(String));
        //    try
        //    {
        //        string selecttype = string.Empty;//查詢條件
        //        #region 查詢條件
        //        if (!string.IsNullOrEmpty(Request.Params["selecttype"]))
        //        {
        //            selecttype = Request.Params["selecttype"].ToString();
        //        }
        //        if (selecttype == "1")
        //        {
        //            if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
        //            {
        //                query.order_id = int.Parse(Request.Params["searchcon"].ToString());
        //            }
        //        }
        //        else if (selecttype == "2")
        //        {
        //            if (!string.IsNullOrEmpty(Request.Params["searchcon"]))
        //            {
        //                query.sinopac_id = Request.Params["searchcon"].ToString();
        //            }
        //        }
        //        #endregion

        //        #region 日期條件一
        //        if (!string.IsNullOrEmpty(Request.Params["timeone"]))
        //        {
        //            query.timeconditionone = int.Parse(Request.Params["timeone"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateOne"]))
        //        {
        //            query.dateconditiononestart = DateTime.Parse(Request.Params["dateOne"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateTwo"]))
        //        {
        //            query.dateconditiononeend = DateTime.Parse(Request.Params["dateTwo"].ToString());
        //        }
        //        #endregion

        //        #region 日期條件二
        //        if (!string.IsNullOrEmpty(Request.Params["timetwo"]))
        //        {
        //            query.timeconditiontwo = int.Parse(Request.Params["timetwo"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateThree"]))
        //        {
        //            query.dateconditiontwostart = DateTime.Parse(Request.Params["dateThree"].ToString());
        //        }
        //        if (!string.IsNullOrEmpty(Request.Params["dateFour"]))
        //        {
        //            query.dateconditiontwoend = DateTime.Parse(Request.Params["dateFour"].ToString());
        //        }
        //        #endregion

        //        #region 核對狀態
        //        if (!string.IsNullOrEmpty(Request.Params["error_type"]))
        //        {
        //            query.error = int.Parse(Request.Params["error_type"].ToString());
        //        }
        //        #endregion

        //        #region 群組
        //        if (!string.IsNullOrEmpty(Request.Params["yes_type"]))
        //        {
        //            query.grouptype = int.Parse(Request.Params["yes_type"].ToString());
        //        }
        //        #endregion

        //        _spcdil = new SinopacDetailMgr(mySqlConnectionString);
        //        stores = _spcdil.GetSinopacDetailInfo(query);
        //        foreach (var item in stores)
        //        {
        //            DataRow dr = dtHZ.NewRow();
        //            dr[0] = item.order_id.ToString();
        //            dr[1] = item.sinopac_id.ToString();
        //            dr[2] = item.order_amount;
        //            dr[3] = item.pay_amount;

        //            dr[4] = CommonFunction.GetNetTime(item.order_createdate).ToString("yyyy-MM-dd HH:mm:ss:f");
        //            dr[5] = CommonFunction.GetNetTime(item.entday).ToString("yyyy-MM-dd HH:mm:ss:f");

        //            dr[6] = CommonFunction.GetNetTime(item.txday).ToString("yyyy-MM-dd HH:mm:ss:f");
        //            dr[7] = CommonFunction.GetNetTime(item.sinopac_createdate).ToString("yyyy-MM-dd HH:mm:ss:f");

        //            dtHZ.Rows.Add(dr);

        //        }
        //        string[] colname = new string[dtHZ.Columns.Count];
        //        string fileName = "sinpac_list_" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".csv";
        //        newExcelName = Server.MapPath(excelPath) + fileName;
        //        for (int i = 0; i < dtHZ.Columns.Count; i++)
        //        {
        //            colname[i] = dtHZ.Columns[i].ColumnName;
        //        }

        //        if (System.IO.File.Exists(newExcelName))
        //        {
        //            System.IO.File.Delete(newExcelName);
        //        }
        //        StringWriter sw = ExcelHelperXhf.SetCsvFromData(dtHZ, "sinpac_export_.csv");
        //        Response.Clear();
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + "sinpac_export_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv");
        //        Response.ContentType = "application/ms-excel";
        //        Response.ContentEncoding = Encoding.Default;
        //        Response.Write(sw);
        //        Response.End();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
        //        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
        //        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //        log.Error(logMessage);
        //    }
        //}
        //#endregion

        //#endregion


        #region 獲取同一表頭 +DataTable GetTitle(DataTable dtHZ, VendorQuery vendorQuery, DataTable tempTemp, int tempFreightDelivery_Normal, int tempFreightDelivery_Low)
        public DataTable GetTitle(DataTable dtHZ, VendorQuery vendorQuery, DataTable tempTemp, int tempFreightDelivery_Normal, int tempFreightDelivery_Low)
        {
            //結帳總金額扣除批次出貨運費
            int M_Account_Amount = Convert.ToInt32(tempTemp.Rows[0]["m_account_amount"]);

            int M_Account_Amount_total = M_Account_Amount;
            int jicang = 0;
            if (!string.IsNullOrEmpty(tempTemp.Rows[0]["m_bag_check_money"].ToString()))
            {
                if (int.TryParse(tempTemp.Rows[0]["m_bag_check_money"].ToString(), out jicang))
                {
                    M_Account_Amount_total = M_Account_Amount_total - jicang;
                }
            }
            if (tempFreightDelivery_Normal != 0)
            {
                M_Account_Amount_total += tempFreightDelivery_Normal;
            }
            if (tempFreightDelivery_Low != 0)
            {
                M_Account_Amount_total += tempFreightDelivery_Low;
            }
            string dispatch = string.Empty;
            if (vendorQuery.dispatch == 1)
            {
                dispatch = "調度倉";
            }
            else
            {
                dispatch = "非調度倉";
            }

            #region 匯出樣式

            DataRow dr1 = dtHZ.NewRow();
            DataRow dr2 = dtHZ.NewRow();
            DataRow dr3 = dtHZ.NewRow();
            DataRow dr4 = dtHZ.NewRow();
            DataRow dr5 = dtHZ.NewRow();
            DataRow dr6 = dtHZ.NewRow();
            DataRow dr7 = dtHZ.NewRow();
            DataRow dr8 = dtHZ.NewRow();
            DataRow dr9 = dtHZ.NewRow();
            string strTemp = string.Empty;
            strTemp = "吉甲地市集" + Request.Params["dateOne"].ToString() + "年" + Request.Params["dateTwo"].ToString() + "月銷售報表";
            dr1[0] = strTemp.ToString();
            dr2[0] = "供應商編號：" + vendorQuery.vendor_code;
            dr3[0] = "供應商名稱:" + vendorQuery.vendor_name_simple;
            dr4[0] = "報表輸出時間:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            dr5[0] = "結帳金額:" + M_Account_Amount;
            dr6[0] = "廠商業績獎金：" + 0;

            dr6[2] = "寄倉：-" + jicang;
            dr6[3] = "調度常溫運費：" + tempFreightDelivery_Normal;
            dr6[5] = "調度低溫運費：" + tempFreightDelivery_Low;
            dr7[0] = "廠商帳款總計：" + (M_Account_Amount_total + 0);//總計金額應該是結帳金額+獎金-寄倉費+調度常溫運費+調度低溫運費
            dr8[0] = "出貨模式：" + dispatch;
            dr9[0] = "歸檔日期";
            dr9[1] = "訂單日期";
            dr9[2] = "出貨日期";
            dr9[3] = "付款單編號";
            dr9[4] = "訂單編號";
            dr9[5] = "商品售價";
            dr9[6] = "商品成本";
            dr9[7] = "付款方式";
            dr9[8] = "一期刷卡費";
            dr9[9] = "常溫運費";
            dr9[10] = "低溫運費";
            dr9[11] = "常溫逆物流";
            dr9[12] = "低溫逆物流";
            dr9[13] = "結帳金額";
            dr9[14] = "商品模式";
            dr9[15] = "運費條件";
            dr9[16] = "組合方式";
            dr9[17] = "商品名稱";
            dr9[18] = "數量";
            dr9[19] = "子商品數量";
            dr9[20] = "成本";
            dr9[21] = "成本小計";
            dr9[22] = "活動價成本";
            dr9[23] = "滿額滿件折扣";
            dr9[24] = "售價";
            dr9[25] = "售價小計";
            dr9[26] = "退貨日期";
            dr9[27] = "商品狀態";
            dr9[28] = "父親商品編號";
            dr9[29] = "寄倉費";
            dr9[30] = "稅別";
            dr9[31] = "商品細項編號";
            dr9[32] = "國際編碼";
            dr9[33] = "管理員備註";
            dtHZ.Rows.Add(dr1);
            dtHZ.Rows.Add(dr2);
            dtHZ.Rows.Add(dr3);
            dtHZ.Rows.Add(dr4);
            dtHZ.Rows.Add(dr5);
            dtHZ.Rows.Add(dr6);
            dtHZ.Rows.Add(dr7);
            dtHZ.Rows.Add(dr8);
            dtHZ.Rows.Add(dr9);
            #endregion
            return dtHZ;
        }

        #endregion

        #region 獲取供應商明細數據 +DataTable GetData(DataTable dtHZ, List<VendorAccountCustom> liStore, int type)
        public DataTable GetData(DataTable dtHZ, List<VendorAccountCustom> liStore, int type, DataTable tempTemp, int tempFreightDelivery_Normal, int tempFreightDelivery_Low)
        {
            uint product_money = 0;
            uint product_cost = 0;
            uint order_payment = 0;
            uint money_creditcard_1 = 0;
            uint freight_delivery_normal = 0;
            uint freight_delivery_low = 0;
            uint freight_return_normal = 0;
            uint freight_return_low = 0;
            int account_amount = 0;
            int fritotal = 0;
            int yingTotal = 0;
            int mianx = 0;
            //DateTime mindate = DateTime.MinValue;
            //DateTime maxdate = DateTime.MinValue;
            _configMgr = new ConfigMgr(mySqlConnectionString);
            DataTable dt = _configMgr.GetConfig(new ConfigQuery { config_name = "is_not_billing_checked" });
            string[] paymentarr = null;
            if (dt.Rows.Count != 0)
            {
                paymentarr = dt.Rows[0]["config_value"].ToString().Split(',').ToArray();
            }
            for (int i = 0; i < liStore.Count; i++)
            {
                DataRow dr = dtHZ.NewRow();
                int tt = 0;
                VendorAccountCustom item = liStore[i];
                if ((i > 0 && (item.slave_id != liStore[i - 1].slave_id || item.order_id != liStore[i - 1].order_id)) || i == 0)
                {
                    dr[0] = item.accountdate.ToString("yyyy/MM/dd");
                    dr[1] = item.ordercreatedate.ToString("yyyy/MM/dd");
                    dr[2] = item.slavedate_delivery.ToString("yyyy/MM/dd");
                    dr[3] = item.order_id;
                    dr[4] = item.slave_id;
                    dr[5] = item.product_money;
                    dr[6] = item.product_cost;
                    dr[7] = item.paymentname;
                    if (item.money_creditcard_1 != 0)
                    {
                        dr[8] = "-" + item.money_creditcard_1;
                    }
                    else
                    {
                        dr[8] = 0;
                    }
                    dr[9] = item.freight_delivery_normal;
                    dr[10] = item.freight_delivery_low;
                    if (item.freight_return_normal != 0)
                    {
                        dr[11] = "-" + item.freight_return_normal;
                    }
                    else
                    {
                        dr[11] = 0;
                    }
                    if (item.freight_return_low != 0)
                    {
                        dr[12] = "-" + item.freight_return_low;
                    }
                    else
                    {
                        dr[12] = 0;
                    }
                    dr[13] = item.account_amount;
                    product_money += item.product_money;
                    product_cost += item.product_cost;
                    //order_payment += item.Order_Payment;
                    money_creditcard_1 += item.money_creditcard_1;
                    freight_delivery_normal += item.freight_delivery_normal;
                    freight_delivery_low += item.freight_delivery_low;
                    freight_return_normal += item.freight_return_normal;
                    freight_return_low += item.freight_return_low;
                    account_amount += item.account_amount;
                }
                if (item.Product_Mode == 1)
                {
                    item.ProductMode = "自出";
                }
                else if (item.Product_Mode == 2)
                {
                    item.ProductMode = "寄倉";
                }
                else if (item.Product_Mode == 3)
                {
                    item.ProductMode = "調度倉";
                }
                if (item.Detail_Status != 4)
                {
                    item.Single_Money = 0;
                    item.Single_Cost = 0;
                    item.single_cost_subtotal = 0;
                }
                else
                {
                    item.single_cost_subtotal = item.Single_Cost * item.Buy_Num;
                }
                if (item.item_mode == 0)
                {
                    item.itemmode = "單一";
                }
                else if (item.item_mode == 1)
                {
                    item.itemmode = "父";
                }
                else if (item.item_mode == 2)
                {
                    item.itemmode = "子";
                }
                dr[14] = item.ProductMode;
                dr[15] = item.product_freight;
                dr[16] = item.itemmode;
                dr[17] = item.Product_Name + item.Product_Spec_Name;
                if (item.item_mode == 2)
                {
                    dr[19] = item.Buy_Num * item.parent_num;
                }
                else
                {
                    dr[18] = item.Buy_Num;
                    dr[20] = item.Single_Cost;
                    dr[21] = item.single_cost_subtotal;
                    dr[22] = item.Event_Cost == 0 ? "-" : item.Event_Cost.ToString();
                    dr[23] = item.Deduct_Account;
                    dr[24] = item.Single_Money;
                    dr[25] = item.Single_Money * item.Buy_Num;
                    if (item.tax_type == 3)
                    {
                        if (item.Event_Cost == 0)
                        {

                            tt = Convert.ToInt32(item.Single_Cost * item.Buy_Num);
                        }
                        else
                        {
                            tt = Convert.ToInt32(item.Event_Cost * item.Buy_Num);
                        }
                        mianx += tt;
                    }
                }
                dr[26] = "";
                dr[27] = item.order_status_name;
                dr[28] = item.parent_id;

                if (item.od_bag_check_money * item.Buy_Num == 0)
                {
                    dr[29] = 0;
                }
                else
                {
                    dr[29] = "-" + (item.od_bag_check_money * item.Buy_Num);
                }

                //dr[29] = item.od_bag_check_money * item.Buy_Num == 0 ? 0 : '-' + (item.od_bag_check_money * item.Buy_Num);

                if (item.tax_type == 1)
                {
                    item.taxtype = "應稅";

                }
                else if (item.tax_type == 3)
                {
                    item.taxtype = "免稅";
                }

                dr[30] = item.taxtype;
                dr[31] = item.Item_Id;
                dr[32] = " " + item.upc_id;
                dr[33] = item.Note_Admin;
                dtHZ.Rows.Add(dr);
            }
            if (type == 1 || type == 3)
            {
                DataRow dre = dtHZ.NewRow();
                dre[0] = "總計";
                dre[5] = product_money;
                dre[6] = product_cost;
                dre[7] = order_payment;
                dre[8] = "-" + money_creditcard_1;
                dre[9] = freight_delivery_normal;
                dre[10] = freight_delivery_low;
                if (freight_return_normal != 0)
                {
                    dre[11] = "-" + freight_return_normal;
                }
                else
                {
                    dre[11] = freight_return_normal;
                }
                if (freight_return_low == 0)
                {
                    dre[12] = 0;
                }
                else
                {
                    dre[12] = "-" + freight_return_low;
                }
                dre[13] = account_amount;
                dtHZ.Rows.Add(dre);
                DataRow drJiC = dtHZ.NewRow();
                drJiC[12] = "寄倉費";
                drJiC[13] = "-" + Convert.ToInt32(tempTemp.Rows[0]["m_bag_check_money"].ToString());
                if (drJiC[13].ToString() != "-0")
                {
                    dtHZ.Rows.Add(drJiC);
                }
                DataRow drFre = dtHZ.NewRow();
                drFre[12] = "運費";
                drFre[13] = tempFreightDelivery_Low + tempFreightDelivery_Normal;
                if (drFre[13].ToString() != "0")
                {
                    dtHZ.Rows.Add(drFre);
                }
                fritotal = tempFreightDelivery_Low + tempFreightDelivery_Normal - (Convert.ToInt32(tempTemp.Rows[0]["m_bag_check_money"].ToString())) + 0;
                if (drJiC[13].ToString() != "-0" || drFre[13].ToString() != "0")
                {
                    DataRow drFreTotal = dtHZ.NewRow();
                    drFreTotal[13] = account_amount + fritotal;
                    dtHZ.Rows.Add(drFreTotal);
                }
                yingTotal = account_amount + fritotal - mianx;
                DataRow drT1 = dtHZ.NewRow();
                int year = Convert.ToInt32(Request.Params["dateOne"].ToString());
                int month = Convert.ToInt32(Request.Params["dateTwo"].ToString());
                DateTime dtime = new DateTime(year, month, 1);

                drT1[0] = "※" + Request.Params["dateTwo"].ToString() + "月對帳表出貨日期：" + dtime.AddDays(-11).ToShortDateString() + "～" + dtime.AddMonths(1).AddDays(-12).ToShortDateString() + "(到店取貨:" + dtime.AddDays(-16).ToShortDateString() + "～" + dtime.AddMonths(1).AddDays(-17).ToShortDateString() + ")";
                dtHZ.Rows.Add(drT1);
                DataRow drT2 = dtHZ.NewRow();
                drT2[0] = "※發票抬頭：吉甲地好市集股份有限公司，統編：25137186。";
                dtHZ.Rows.Add(drT2);
                DataRow drT3 = dtHZ.NewRow();
                drT3[0] = "※正本發票或發據請寄至:115台北市南港區八德路四段768巷9號3樓之1(會計部收)，02-2783-3183。";
                dtHZ.Rows.Add(drT3);
                DataRow drT7 = dtHZ.NewRow();
                drT7[0] = "※" + Request.Params["dateTwo"].ToString() + "月對帳表(" + dtime.AddMonths(1).Month + "月對帳時程)：";
                dtHZ.Rows.Add(drT7);
                DataRow drT4 = dtHZ.NewRow();
                drT4[0] = "1.對帳表提供:吉甲地提供對帳表，月初前3工作日";
                dtHZ.Rows.Add(drT4);
                DataRow drT5 = dtHZ.NewRow();
                drT5[0] = "2.對帳:廠商對帳OK後正本請款發票/收據寄達至吉甲地(每月10前,遇假日順延一日)";
                dtHZ.Rows.Add(drT5);
                DataRow drT6 = dtHZ.NewRow();
                drT6[0] = "3.付款:吉甲地廠商款付款，每月最後一工作日";
                dtHZ.Rows.Add(drT6);
                DataRow drT8 = dtHZ.NewRow();
                drT8[13] = "(稅別金額如下，提供開立發票參考,如金額有誤,請跟吉甲地連絡,如為開立收據,可毋需理會)";
                dtHZ.Rows.Add(drT8);
                DataRow drShuiBei = dtHZ.NewRow();
                drShuiBei[13] = "稅別";
                drShuiBei[14] = "免稅";
                drShuiBei[15] = "應稅";
                drShuiBei[16] = "統計";
                dtHZ.Rows.Add(drShuiBei);
                DataRow DrXS = dtHZ.NewRow();
                DrXS[13] = "銷售額";
                if (yingTotal < 0)
                {
                    mianx = mianx + yingTotal;
                    yingTotal = 0;
                }
                DrXS[14] = mianx;
                DrXS[15] = Math.Round(yingTotal / 1.05);
                DrXS[16] = "/";
                dtHZ.Rows.Add(DrXS);
                DataRow DrS = dtHZ.NewRow();
                DrS[13] = "稅額";
                DrS[14] = 0;
                DrS[15] = yingTotal - Math.Round(yingTotal / 1.05);
                DrS[16] = "/";
                dtHZ.Rows.Add(DrS);
                DataRow DrZong = dtHZ.NewRow();
                DrZong[13] = "發票金額";
                DrZong[14] = mianx;
                DrZong[15] = yingTotal;
                DrZong[16] = mianx + yingTotal;
                dtHZ.Rows.Add(DrZong);
            }
            else if (type == 2)
            {
                DataRow drTt = dtHZ.NewRow();
                dtHZ.Rows.Add(drTt);
                DataRow drTt1 = dtHZ.NewRow();
                dtHZ.Rows.Add(drTt1);
            }
            return dtHZ;
        }
        #endregion

        #region 獲取供應商對賬明細 + DataTable GetDZ(DataTable dt, DataTable dtPiCi, int tempFreightDelivery_Normal, int tempFreightDelivery_Low)
        public DataTable GetDZ(DataTable dt, DataTable dtPiCi, int tempFreightDelivery_Normal, int tempFreightDelivery_Low)
        {
            DataTable dtYF = new DataTable();
            for (int i = 1; i <= 7; i++)
            {
                dtYF.Columns.Add("", typeof(String));
            }
            DataRow drTtt = dtYF.NewRow();
            //dtYF.Columns.Add("批次出貨單號");
            //dtYF.Columns.Add("常溫商品總額");
            //dtYF.Columns.Add("低溫商品總額");
            //dtYF.Columns.Add("批次出貨明細");
            //dtYF.Columns.Add("廠商出貨單編號");
            //dtYF.Columns.Add("出貨時間");
            //dtYF.Columns.Add("付款單號");
            drTtt[0] = "批次出貨單號";
            drTtt[1] = "常溫商品總額";
            drTtt[2] = "低溫商品總額";
            drTtt[3] = "批次出貨明細";
            drTtt[4] = "廠商出貨單編號";
            drTtt[5] = "出貨時間";
            drTtt[6] = "付款單號";

            dtYF.Rows.Add(drTtt);
            if (dt.Rows.Count > 0)
            {

                for (int s = 0; s < dtPiCi.Rows.Count; s++)
                {
                    DataRow yfDr = dtYF.NewRow();
                    yfDr[0] = dtPiCi.Rows[s]["code_num"];
                    yfDr[1] = dtPiCi.Rows[s]["normal_subtotal"];
                    yfDr[2] = dtPiCi.Rows[s]["hypothermia_subtotal"];
                    yfDr[3] = dtPiCi.Rows[s]["code_num"];
                    yfDr[4] = dtPiCi.Rows[s]["slave_id"];
                    yfDr[5] = dtPiCi.Rows[s]["deliver_time"];
                    yfDr[6] = dtPiCi.Rows[s]["order_id"];
                    dtYF.Rows.Add(yfDr);
                }
                DataRow tempdr1 = dtYF.NewRow();
                tempdr1[0] = "常溫運費補貼:" + tempFreightDelivery_Normal;
                dtYF.Rows.Add(tempdr1);
                DataRow tempdr2 = dtYF.NewRow();
                tempdr2[0] = "低溫運費補貼:" + tempFreightDelivery_Low;
                dtYF.Rows.Add(tempdr2);
            }
            return dtYF;
        }
        #endregion

        #region 單個供應商報表詳情+void ExportVendorAccountMonthDetail()
        public void ExportVendorAccountMonthDetail()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            VendorAccountDetailQuery query = new VendorAccountDetailQuery();
            VendorAccountMonthQuery VAMQuery = new VendorAccountMonthQuery();
            VendorQuery vendorQuery = new VendorQuery();
            try
            {
                List<string> list = GetTime(uint.Parse(Request.Params["dateone"]), uint.Parse(Request.Params["datetwo"]));
                _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
                vendorQuery.vendor_id = Convert.ToUInt32(Request.Params["vendorid"]);
                VAMQuery.vendor_id = Convert.ToUInt32(Request.Params["vendorid"]);
                VAMQuery.account_year = Convert.ToUInt32(Request.Params["dateOne"]);
                VAMQuery.account_month = Convert.ToUInt32(Request.Params["dateTwo"]);
                query.vendor_id = Convert.ToUInt32(Request.Params["vendorid"]);
                query.search_start_time = list[0];
                query.search_end_time = list[1];
                int tempFreightDelivery_Normal = 0;
                int tempFreightDelivery_Low = 0;
                for (int i = 1; i <= 34; i++)
                {
                    dtHZ.Columns.Add("", typeof(String));
                }
                //調度倉運費
                DataTable dt = _IVAMMgr.GetFreightMoney(query, out tempFreightDelivery_Normal, out tempFreightDelivery_Low);
                // 查供應商總帳
                DataTable tempTemp = _IVAMMgr.GetVendorAccountMonthZongZhang(VAMQuery);
                DataTable dtPiCi = _IVAMMgr.BatchOrderDetail(query);
                //供應商信息
                vendorQuery = _IVAMMgr.GetVendorInfoByCon(vendorQuery);
                dtHZ = GetTitle(dtHZ, vendorQuery, tempTemp, tempFreightDelivery_Normal, tempFreightDelivery_Low);
                List<VendorAccountCustom> liStore = _IVAMMgr.VendorAccountDetailExport(query);
                dtHZ = GetData(dtHZ, liStore, 1, tempTemp, tempFreightDelivery_Normal, tempFreightDelivery_Low);//1：單獨 2：全部 3：批次

                List<DataTable> Elist = new List<DataTable>();
                List<string> NameList = new List<string>();
                List<bool> comName = new List<bool>();
                comName.Add(false);
                Elist.Add(dtHZ);
                NameList.Add("對賬報表");
                DataTable dtYF = GetDZ(dt, dtPiCi, tempFreightDelivery_Normal, tempFreightDelivery_Low);
                comName.Add(true);
                Elist.Add(dtYF);
                NameList.Add("調度倉運費");
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = vendorQuery.vendor_id + "-" + vendorQuery.vendor_code + "-供應商對帳報表" + vendorQuery.vendor_name_full + VAMQuery.account_year + "-" + VAMQuery.account_month + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDTNoColumns(Elist, NameList, comName);
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

        #region 總表明細+void ExportVendorAccountMonthAll()
        /// <summary>
        /// 總表明細
        /// </summary>
        public void ExportVendorAccountMonthAll()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            VendorAccountDetailQuery query = new VendorAccountDetailQuery();
            VendorAccountMonthQuery VAMQuery = new VendorAccountMonthQuery();
            VendorQuery vendorQuery = new VendorQuery();
            List<DataTable> Elist = new List<DataTable>();
            List<string> NameList = new List<string>();
            List<bool> comName = new List<bool>();
            try
            {
                List<string> list = GetTime(uint.Parse(Request.Params["dateone"]), uint.Parse(Request.Params["datetwo"]));
                _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
                VAMQuery.account_year = Convert.ToUInt32(Request.Params["dateOne"]);
                VAMQuery.account_month = Convert.ToUInt32(Request.Params["dateTwo"]);
                query.search_start_time = list[0];
                query.search_end_time = list[1];
                int tempFreightDelivery_Normal = 0;
                int tempFreightDelivery_Low = 0;
                for (int i = 1; i <= 34; i++)
                {
                    dtHZ.Columns.Add("", typeof(String));
                }
                //供應商信息
                DataTable vendorDt = _IVAMMgr.GetVendorAccountMonthInfo(VAMQuery);
                DataTable dtPiCi = _IVAMMgr.BatchOrderDetail(query);
                for (int m = 0; m < vendorDt.Rows.Count; m++)
                {
                    vendorQuery.vendor_id = Convert.ToUInt32(vendorDt.Rows[m]["vendor_id"]);
                    VAMQuery.vendor_id = Convert.ToUInt32(vendorDt.Rows[m]["vendor_id"]);
                    query.vendor_id = Convert.ToUInt32(vendorDt.Rows[m]["vendor_id"]);
                    //調度倉運費
                    DataTable dt = _IVAMMgr.GetFreightMoney(query, out tempFreightDelivery_Normal, out tempFreightDelivery_Low);
                    // 查供應商總帳
                    DataTable tempTemp = _IVAMMgr.GetVendorAccountMonthZongZhang(VAMQuery);

                    //供應商信息
                    vendorQuery = _IVAMMgr.GetVendorInfoByCon(vendorQuery);
                    dtHZ = GetTitle(dtHZ, vendorQuery, tempTemp, tempFreightDelivery_Normal, tempFreightDelivery_Low);
                    List<VendorAccountCustom> liStore = _IVAMMgr.VendorAccountDetailExport(query);
                    dtHZ = GetData(dtHZ, liStore, 2, tempTemp, tempFreightDelivery_Normal, tempFreightDelivery_Low);//1：單獨 2：全部 3：批次
                }
                comName.Add(false);
                Elist.Add(dtHZ);
                NameList.Add("應付金額與商品明細");
                DataTable dtYF = new DataTable();
                if (dtPiCi.Rows.Count > 0)
                {
                    dtYF.Columns.Add("批次出貨單號");
                    dtYF.Columns.Add("常溫商品總額");
                    dtYF.Columns.Add("低溫商品總額");
                    dtYF.Columns.Add("批次出貨明細");
                    dtYF.Columns.Add("廠商出貨單編號");
                    dtYF.Columns.Add("出貨時間");
                    dtYF.Columns.Add("付款單號");
                    for (int s = 0; s < dtPiCi.Rows.Count; s++)
                    {
                        DataRow yfDr = dtYF.NewRow();
                        yfDr[0] = dtPiCi.Rows[s]["code_num"];
                        yfDr[1] = dtPiCi.Rows[s]["normal_subtotal"];
                        yfDr[2] = dtPiCi.Rows[s]["hypothermia_subtotal"];
                        yfDr[3] = dtPiCi.Rows[s]["code_num"];
                        yfDr[4] = dtPiCi.Rows[s]["slave_id"];
                        yfDr[5] = dtPiCi.Rows[s]["deliver_time"];
                        yfDr[6] = dtPiCi.Rows[s]["order_id"];
                        dtYF.Rows.Add(yfDr);
                    }
                    DataRow tempdr1 = dtYF.NewRow();
                    tempdr1[0] = "常溫運費補貼:" + tempFreightDelivery_Normal;
                    dtYF.Rows.Add(tempdr1);
                    DataRow tempdr2 = dtYF.NewRow();
                    tempdr2[0] = "低溫運費補貼:" + tempFreightDelivery_Normal;
                    dtYF.Rows.Add(tempdr2);
                    comName.Add(true);
                    Elist.Add(dtYF);
                    NameList.Add("調度倉運費");

                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = "供應商對賬報表-" + VAMQuery.account_year + "-" + VAMQuery.account_month + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDTNoColumns(Elist, NameList, comName);
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

        #region 批次匯出各供應商的業績明細 + HttpResponseBase AllExportVendorAccountMonthDetail()

        /// <summary>
        /// 批次匯出各供應商的業績明細
        /// </summary>
        public HttpResponseBase AllExportVendorAccountMonthDetail()
        {
            string json = string.Empty;
            List<string> strPath = new List<string>();
            VendorQuery vendorQuery = new VendorQuery();
            try
            {
                List<string> list = GetTime(uint.Parse(Request.Params["dateone"]), uint.Parse(Request.Params["datetwo"]));
                _IVAMMgr = new VendorAccountMonthMgr(mySqlConnectionString);
                //Dictionary<int, DataTable> tempDT = new Dictionary<int, DataTable>();
                VendorAccountMonthQuery VAMQueryTemp = new VendorAccountMonthQuery();

                VAMQueryTemp.account_year = Convert.ToUInt32(Request.Params["dateOne"]);
                VAMQueryTemp.account_month = Convert.ToUInt32(Request.Params["dateTwo"]);
                DataTable vendorDt = _IVAMMgr.GetVendorAccountMonthInfo(VAMQueryTemp);
                for (int l = 0; l < vendorDt.Rows.Count; l++)
                {
                    DataTable dtHZ = new DataTable();
                    VendorAccountDetailQuery query = new VendorAccountDetailQuery();
                    VendorAccountMonthQuery VAMQuery = new VendorAccountMonthQuery();
                    int tempFreightDelivery_Normal = 0;
                    int tempFreightDelivery_Low = 0;
                    vendorQuery.vendor_id = Convert.ToUInt32(vendorDt.Rows[l]["vendor_id"]);
                    VAMQuery.vendor_id = Convert.ToUInt32(vendorDt.Rows[l]["vendor_id"]);
                    VAMQuery.account_year = VAMQueryTemp.account_year;
                    VAMQuery.account_month = VAMQueryTemp.account_month;
                    query.vendor_id = Convert.ToUInt32(vendorDt.Rows[l]["vendor_id"]);
                    query.search_start_time = list[0];
                    query.search_end_time = list[1];
                    //調度倉運費
                    DataTable dt = _IVAMMgr.GetFreightMoney(query, out tempFreightDelivery_Normal, out tempFreightDelivery_Low);
                    // 查供應商總帳
                    DataTable tempTemp = _IVAMMgr.GetVendorAccountMonthZongZhang(VAMQuery);

                    DataTable dtPiCi = _IVAMMgr.BatchOrderDetail(query);
                    //tempDT.Add(0, tempTemp);
                    //供應商信息
                    vendorQuery = _IVAMMgr.GetVendorInfoByCon(vendorQuery);
                    for (int i = 1; i <= 34; i++)
                    {
                        dtHZ.Columns.Add("", typeof(String));
                    }
                    dtHZ = GetTitle(dtHZ, vendorQuery, tempTemp, tempFreightDelivery_Normal, tempFreightDelivery_Low);
                    List<VendorAccountCustom> liStore = _IVAMMgr.VendorAccountDetailExport(query);
                    dtHZ = GetData(dtHZ, liStore, 3, tempTemp, tempFreightDelivery_Normal, tempFreightDelivery_Low);//1：單獨 2：全部 3：批次
                    List<DataTable> Elist = new List<DataTable>();
                    List<string> NameList = new List<string>();
                    List<bool> comName = new List<bool>();
                    comName.Add(false);
                    Elist.Add(dtHZ);
                    NameList.Add("對賬報表");
                    DataTable dtYF = GetDZ(dt, dtPiCi, tempFreightDelivery_Normal, tempFreightDelivery_Low);
                    comName.Add(true);
                    Elist.Add(dtYF);
                    NameList.Add("調度倉運費");
                    if (liStore.Count > 0)
                    {
                        string fileName = vendorQuery.vendor_id + "-" + vendorQuery.vendor_code + "-供應商對帳報表" + vendorQuery.vendor_name_full + VAMQuery.account_year + "-" + VAMQuery.account_month + ".xls";
                        MemoryStream ms = ExcelHelperXhf.ExportDTNoColumns(Elist, NameList, comName);
                        //MemoryStream m = new MemoryStream();

                        FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/" + fileName), FileMode.OpenOrCreate);
                        BinaryWriter w = new BinaryWriter(fs);
                        w.Write(ms.ToArray());
                        fs.Close();
                        ms.Close();
                        strPath.Add(Server.MapPath("../ImportUserIOExcel/" + fileName));
                    }
                }
                string strZipPath = Server.MapPath("../ImportUserIOExcel/供應商對賬報表.zip");
                string strZipTopDirectoryPath = Server.MapPath("../ImportUserIOExcel/");
                int intZipLevel = 6;
                string strPassword = "";
                SharpZipLibHelp szlh = new SharpZipLibHelp();
                szlh.Zip(strZipPath, strZipTopDirectoryPath, intZipLevel, strPassword, strPath);
                json = "{success:'true'}";
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
    }
}
