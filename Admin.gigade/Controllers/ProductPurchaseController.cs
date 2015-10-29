using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    /// <summary>
    /// 商品採購
    /// </summary>
    public class ProductPurchaseController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();

        public ArrivalNoticeMgr arrivalnoticemgr;

        //BLL.gigade.Mgr.OrderDetailMgr orderDetailMgr = new BLL.gigade.Mgr.OrderDetailMgr(mySqlConnectionString);
        BLL.gigade.Mgr.ProductItemMgr productItemMgr = new BLL.gigade.Mgr.ProductItemMgr(mySqlConnectionString);
        private IItemIpoCreateLogImplMgr _ItemIpoMgr;// ItemIpoMgr = new BLL.gigade.Mgr.ProductItemMgr(mySqlConnectionString);
        IParametersrcImplMgr _paraMgr;
        IArrivalNoticeImplMgr _arrivalMgr;
       
        IUsersImplMgr _usersMgr;
        #region 視圖
        //
        // GET: /ProductPurchase/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RIS()//補貨通知
        {
            return View();
        }
        public ActionResult StatusLowerShelf()
        {
            return View();
        }
        #endregion

        #region 根據條件獲取需要建議採購的商品信息
        /// <summary>
        /// 根據條件獲取需要建議採購的商品信息
        /// </summary>
        /// <returns>商品信息列表</returns>
        public HttpResponseBase GetSuggestPurchase()
        {
            string json = string.Empty;

            DataTable dt = new DataTable();
            ProductItemQuery query = new ProductItemQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            query.stockScope = int.Parse(Request.Params["stockScope"] ?? "0");//所有庫存
            query.sumDays = int.Parse(Request.Params["sumDays"] ?? "90");   //總天數
            query.periodDays = int.Parse(Request.Params["periodDays"] ?? "7"); //周期天數
            //     perpaid: Ext.getCmp('perpaid').getValue().perpaidValue,/*是否已下單採購*/
            //Is_pod: Ext.getCmp('Is_pod').getValue().Is_podValue,/*是否 買斷*/
            //vendor_name:Ext.getCmp('vendor_name').getValue(),/*供應商名稱*/
            query.prepaid = int.Parse(Request.Params["perpaid"] ?? "-1");/*是否買斷*/
            query.Is_pod = int.Parse(Request.Params["Is_pod"] ?? "0");/*是否已下單採購*/
            query.sale_status = uint.Parse(Request.Params["sale_status"] ?? "100");/*販售狀態*/
           // query.vendor_name = Request.Params["vendor_name"] ?? "";/*供應商名稱*/
            try
            {
                if(!string.IsNullOrEmpty(Request.Params["serchType"] ))
                {
                    int serchType=int.Parse(Request.Params["serchType"]);
                    if (!string.IsNullOrEmpty(Request.Params["serchName"]))
                    {
                        switch (serchType)
                        {
                            case 1:
                                query.vendor_id = uint.Parse(Request.Params["serchName"].Trim());
                                break;
                            case 2:
                                query.vendor_name_full = Request.Params["serchName"].Trim();
                                break;
                            case 3:
                                query.vendor_name = Request.Params["serchName"].Trim();
                                break;
                            case 4:
                                query.Erp_Id = Request.Params["serchName"].Trim();
                                break;
                            default:
                                break;
                        }
                    }
                }
                _paraMgr = new ParameterMgr(mySqlConnectionString);
                Parametersrc p = new Parametersrc();
                List<Parametersrc> list = new List<Parametersrc>();
                p.ParameterType = "Food_Articles";
                list = _paraMgr.GetAllKindType(p.ParameterType);
                for (int i = 0; i < list.Count; i++)/*要禁用的食品錧和用品館的商品*/
                {
                    if (!string.IsNullOrEmpty(list[i].ParameterCode))
                    {
                        query.category_ID_IN += list[i].ParameterCode + ",";
                    }
                }
                query.category_ID_IN = query.category_ID_IN.TrimEnd(',');
                int totalCount = 0;
                dt = productItemMgr.GetSuggestPurchaseInfo(query, out totalCount);
                if (dt.Rows.Count > 0)
                {
                    //添加兩列用於存儲"平均平均量"與"建議採購量"
                    dt.Columns.Add("averageCount", typeof(string));
                    dt.Columns.Add("suggestPurchaseCount", typeof(string));
                    //循環每一行數據計算"平均平均量"與"建議採購量"
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        double sum_total = 0;
                        int safe_stock_amount = 0;
                        int item_stock = 0;
                        int item_alarm = 0;
                        int procurement_days = 0;
                        if (double.TryParse(dt.Rows[i]["sum_total"].ToString(), out sum_total))
                        {
                            sum_total = Convert.ToDouble(dt.Rows[i]["sum_total"]);
                        }
                        if (int.TryParse(dt.Rows[i]["safe_stock_amount"].ToString(), out safe_stock_amount))
                        {
                            safe_stock_amount = Convert.ToInt32(dt.Rows[i]["safe_stock_amount"]);
                        }
                        if (int.TryParse(dt.Rows[i]["item_stock"].ToString(), out item_stock))
                        {
                            item_stock = Convert.ToInt32(dt.Rows[i]["item_stock"]);
                        }
                        if (int.TryParse(dt.Rows[i]["item_alarm"].ToString(), out item_alarm))
                        {
                            item_alarm = Convert.ToInt32(dt.Rows[i]["item_alarm"]);
                        }
                        if (int.TryParse(dt.Rows[i]["procurement_days"].ToString(), out procurement_days))
                        {
                            procurement_days = Convert.ToInt32(dt.Rows[i]["procurement_days"]);
                        }

                        if (string.IsNullOrEmpty(dt.Rows[i]["sum_total"].ToString()))
                        {
                            dt.Rows[i]["averageCount"] = 0;
                            dt.Rows[i]["suggestPurchaseCount"] = 0;
                        }
                        else
                        {
                            //週期平均量
                            string averageCount = (sum_total / query.sumDays * query.periodDays).ToString();
                            if (averageCount.Contains('.'))
                            {
                                if (averageCount.Substring(averageCount.IndexOf('.'), averageCount.Length - averageCount.IndexOf('.')).Length > 5)
                                {
                                    dt.Rows[i]["averageCount"] = averageCount.Substring(0, averageCount.IndexOf('.') + 5);
                                }
                                else
                                {
                                    dt.Rows[i]["averageCount"] = averageCount;
                                }
                            }
                            else
                            {
                                dt.Rows[i]["averageCount"] = averageCount;
                            }

                            //當前庫存量-供應商的採購天數*平均銷售數量(最小值為1))<=安全存量時,就需要採購
                            if (item_stock - procurement_days * sum_total / query.sumDays * query.periodDays <= item_alarm)
                            {
                                //建議採購量:供應商的進貨天數*採購調整系數*近3個月的平均每周銷售數量(最小值為1)
                                //(供應商採購天數+安全係數)*週期平均量+（安全存量-庫存）
                               //double suggestPurchaseTemp = procurement_days * safe_stock_amount * (sum_total / query.sumDays) * query.periodDays;
                                double suggestPurchaseTemp = (procurement_days + safe_stock_amount) * (sum_total / query.sumDays) * query.periodDays + ((item_alarm - item_stock) > 0 ? (item_alarm - item_stock): 0);
                                if (suggestPurchaseTemp <= int.Parse(dt.Rows[i]["min_purchase_amount"].ToString()))   //最小值為1
                                {
                                    dt.Rows[i]["suggestPurchaseCount"] = dt.Rows[i]["min_purchase_amount"];
                                }
                                else
                                {
                                    if (suggestPurchaseTemp.ToString().Contains('.'))
                                    {
                                        int suggestPurchase = Convert.ToInt32(suggestPurchaseTemp);
                                        if (suggestPurchase < suggestPurchaseTemp)
                                        {
                                            dt.Rows[i]["suggestPurchaseCount"] = Convert.ToInt32(suggestPurchaseTemp) + 1;
                                        }
                                        else 
                                        {
                                            dt.Rows[i]["suggestPurchaseCount"] = Convert.ToInt32(suggestPurchaseTemp);
                                        }
                                    }
                                    else
                                    {
                                        dt.Rows[i]["suggestPurchaseCount"] = Convert.ToInt32(suggestPurchaseTemp);
                                    }
                                }
                            }
                            else
                            {
                                dt.Rows[i]["suggestPurchaseCount"] = "暫不需採購";
                            }
                        }
                    }
                }

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,'msg':'user',totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 匯出商品建議採購信息
        /// <summary>
        /// 匯出商品建議採購信息
        /// </summary>
        public void ReportSuggestPurchaseExcel()
        {
            _paraMgr = new ParameterMgr(mySqlConnectionString);
            Parametersrc p = new Parametersrc();
            List<Parametersrc> Paralist = new List<Parametersrc>();
          
            DataTable dt = new DataTable();
            ProductItemQuery query = new ProductItemQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            query.stockScope = int.Parse(Request.Params["stockScope"] ?? "0");//所有庫存
            query.sumDays = int.Parse(Request.Params["sumDays"] ?? "90");   //總天數
            query.periodDays = int.Parse(Request.Params["periodDays"] ?? "7"); //周期天數
            query.prepaid = int.Parse(Request.Params["perpaid"] ?? "-1");/*是否買斷*/
            query.Is_pod = int.Parse(Request.Params["Is_pod"] ?? "0");/*是否已下單採購*/
            //query.vendor_name = Request.Params["vendor_name"] ?? "";/*供應商名稱*/
            if (!string.IsNullOrEmpty(Request.Params["sale_status"]) && Request.Params["sale_status"] != "null")
            {
                query.sale_status = uint.Parse(Request.Params["sale_status"]);/*販售狀態*/
            }
            else 
            {
                query.sale_status = 100;
            }
            if (!string.IsNullOrEmpty(Request.Params["serchType"]) && Request.Params["serchType"]!="null")
            {
                int serchType = int.Parse(Request.Params["serchType"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["serchName"] ?? ""))
                {
                    switch (serchType)
                    {
                        case 1:
                            query.vendor_id = uint.Parse(Request.Params["serchName"].Trim());
                            break;
                        case 2:
                            query.vendor_name_full = Request.Params["serchName"];
                            break;
                        case 3:
                            query.vendor_name = Request.Params["serchName"];
                            break;
                        case 4:
                            query.Erp_Id = Request.Params["serchName"];
                            break;
                        default:
                            break;
                    }
                }
            }
            p.ParameterType = "Food_Articles";
            Paralist = _paraMgr.GetAllKindType(p.ParameterType);
            for (int i = 0; i < Paralist.Count; i++)/*要禁用的食品錧和用品館的商品*/
            {
                if (!string.IsNullOrEmpty(Paralist[i].ParameterCode))
                {
                    query.category_ID_IN += Paralist[i].ParameterCode + ",";
                }
            }
           
            query.category_ID_IN = query.category_ID_IN.TrimEnd(',');
           
            query.IsPage = false;
            DataTable dtExcel = new DataTable();
            try
            {

                int totalCount = 0;
                dt = productItemMgr.GetSuggestPurchaseInfo(query, out totalCount);

                //添加兩列用於存儲"平均平均量"與"建議採購量"
                dt.Columns.Add("averageCount", typeof(string));
                dt.Columns.Add("suggestPurchaseCount", typeof(string));
                dtExcel.Columns.Add("供應商編號", typeof(String));
                dtExcel.Columns.Add("供應商簡稱", typeof(String));
                dtExcel.Columns.Add("商品編號", typeof(String));
                dtExcel.Columns.Add("商品細項編號", typeof(String));
                dtExcel.Columns.Add("商品ERP編號", typeof(String));
                dtExcel.Columns.Add("商品名稱", typeof(String));
                dtExcel.Columns.Add("規格", typeof(String));
                //dtExcel.Columns.Add("規格二", typeof(String));
                dtExcel.Columns.Add("出貨方式", typeof(String));
                dtExcel.Columns.Add("是否買斷", typeof(String));
                dtExcel.Columns.Add("庫存量", typeof(String));
                dtExcel.Columns.Add("安全存量", typeof(String));
                dtExcel.Columns.Add("購買總數", typeof(String));
                dtExcel.Columns.Add("週期平均量", typeof(String));
                dtExcel.Columns.Add("建議採購量", typeof(String));
                dtExcel.Columns.Add("最小採購量", typeof(String));
                dtExcel.Columns.Add("供應商採購天數", typeof(String));
                dtExcel.Columns.Add("補貨通知人數", typeof(String));
                dtExcel.Columns.Add("售價(單價)", typeof(String));
                dtExcel.Columns.Add("成本(單價)", typeof(String));
                dtExcel.Columns.Add("商品狀態", typeof(String));
                dtExcel.Columns.Add("販售狀態", typeof(String));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = Convert.ToInt64(dt.Rows[i]["vendor_id"]);
                    newRow[1] = dt.Rows[i]["vendor_name_simple"];
                    newRow[2] = dt.Rows[i]["product_id"];
                    newRow[3] = dt.Rows[i]["item_id"];
                    newRow[4] = " "+dt.Rows[i]["erp_id"];
                    newRow[5] = dt.Rows[i]["product_name"];
                    newRow[6] = dt.Rows[i]["spec_title_1"];
                   // newRow[7] = dt.Rows[i]["spec_title_2"].ToString() + dt.Rows[i]["spec_id_2"].ToString();
                    newRow[7] = dt.Rows[i]["product_mode_name"];
                    newRow[8] = dt.Rows[i]["prepaid"];
                    if (!string.IsNullOrEmpty(dt.Rows[i]["prepaid"].ToString()))
                    {
                        int prepaid = Convert.ToInt32(dt.Rows[i]["prepaid"]);
                        if (prepaid == 0)
                            newRow[8] = "否";
                        if (prepaid == 1)
                            newRow[8] = "是";
                    }
                    newRow[9] = dt.Rows[i]["item_stock"];
                    newRow[10] = dt.Rows[i]["item_alarm"];
                    newRow[11] = dt.Rows[i]["sum_total"];

                    if (string.IsNullOrEmpty(dt.Rows[i]["sum_total"].ToString()))
                    {
                        newRow[12] = 0;
                        newRow[13] = 0;
                    }
                    else
                    {
                        double sum_total = 0;
                        int safe_stock_amount = 0;
                        int item_stock = 0;
                        int item_alarm = 0;
                        int procurement_days = 0;
                        if (double.TryParse(dt.Rows[i]["sum_total"].ToString(), out sum_total))
                        {
                            sum_total = Convert.ToDouble(dt.Rows[i]["sum_total"]);
                        }
                        if (int.TryParse(dt.Rows[i]["safe_stock_amount"].ToString(), out safe_stock_amount))
                        {
                            safe_stock_amount = Convert.ToInt32(dt.Rows[i]["safe_stock_amount"]);
                        }
                        if (int.TryParse(dt.Rows[i]["item_stock"].ToString(), out item_stock))
                        {
                            item_stock = Convert.ToInt32(dt.Rows[i]["item_stock"]);
                        }
                        if (int.TryParse(dt.Rows[i]["item_alarm"].ToString(), out item_alarm))
                        {
                            item_alarm = Convert.ToInt32(dt.Rows[i]["item_alarm"]);
                        }
                        if (int.TryParse(dt.Rows[i]["procurement_days"].ToString(), out procurement_days))
                        {
                            procurement_days = Convert.ToInt32(dt.Rows[i]["procurement_days"]);
                        }

                        //週期平均量
                        //newRow[11] = sum_total / query.sumDays * query.periodDays;
                        string averageCount = (sum_total / query.sumDays * query.periodDays).ToString();
                        if (averageCount.Contains('.') && averageCount.Substring(averageCount.IndexOf('.'), averageCount.Length - averageCount.IndexOf('.')).Length > 5)
                        {
                            newRow[12] = averageCount.Substring(0, averageCount.IndexOf('.') + 5);
                        }
                        else
                        {
                            newRow[12] = averageCount;
                        }

                        //當前庫存量-供應商的採購天數*平均銷售數量(最小值為1))<=安全存量時,就需要採購
                        if (item_stock - procurement_days * sum_total / query.sumDays * query.periodDays <= item_alarm)
                        {
                            //建議採購量:供應商的進貨天數*採購調整系數*近3個月的平均每周銷售數量(最小值為1)
                            //double suggestPurchaseTemp = procurement_days * safe_stock_amount * (sum_total / query.sumDays) * query.periodDays;
                            double suggestPurchaseTemp = (procurement_days + safe_stock_amount) * (sum_total / query.sumDays) * query.periodDays + ((item_alarm - item_stock) > 0 ? (item_alarm - item_stock) : 0);
                            //if (suggestPurchaseTemp <= 1)   //最小值為1
                            //{
                            //    newRow[12] = 1;
                            //}
                            if (suggestPurchaseTemp <= int.Parse(dt.Rows[i]["min_purchase_amount"].ToString()))   //最小值為1
                            {
                                 newRow[13] = dt.Rows[i]["min_purchase_amount"];
                            }
                            else
                            {
                                int suggestPurchase = Convert.ToInt32(suggestPurchaseTemp);
                                if (suggestPurchase < suggestPurchaseTemp)
                                {
                                    newRow[13] = Convert.ToInt32(suggestPurchaseTemp) + 1;
                                }
                                else
                                {
                                    newRow[13] = Convert.ToInt32(suggestPurchaseTemp);
                                }
                            }
                        }
                        else
                        {
                            newRow[13] = "暫不需採購";
                        }
                    }

                    newRow[14] = dt.Rows[i]["min_purchase_amount"];
                    newRow[15] = dt.Rows[i]["procurement_days"];
                    newRow[16] = dt.Rows[i]["NoticeGoods"];
                    newRow[17] = dt.Rows[i]["item_money"];
                    newRow[18] = dt.Rows[i]["item_cost"];
                    newRow[19] = dt.Rows[i]["product_status_string"];
                    newRow[20] = dt.Rows[i]["sale_name"];
                    dtExcel.Rows.Add(newRow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "商品建議採購量_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtExcel, "商品建議採購量_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
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

        public HttpResponseBase AddItemIpo()
        {
           
            ItemIpoCreateLogQuery query = new ItemIpoCreateLogQuery();
            string json = string.Empty;
            try
            {
                string Item_id = "";
                if (!string.IsNullOrEmpty(Request.Params["Items"]))
                {
                    Item_id = Request.Params["Items"];
                    Item_id = Item_id.TrimEnd(',');
                    query.item_id_in = Item_id;
                }
                query.create_datetime = DateTime.Now;
                query.create_user = (Session["caller"] as Caller).user_id;
                _ItemIpoMgr = new ItemIpoCreateLogMgr(mySqlConnectionString);

                int result = _ItemIpoMgr.AddItemIpoCreate(query);
                if (result > 0)
                {

                    json = "{success:true,msg:\"" + result + "\"}";
                }
                else
                {
                    json = "{success:false,msg:\"" + result + "\"}";
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:'0'}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #region 補貨通知人數統計
        public HttpResponseBase GetArrNoticeList()// createTime 2015/8/25 by yachao1120j
        {
            string json = string.Empty;
            int totalcount = 0;
            ArrivalNoticeQuery query = new ArrivalNoticeQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            arrivalnoticemgr = new ArrivalNoticeMgr(mySqlConnectionString);

            if (!string.IsNullOrEmpty(Request.Params["vendor_name_full_OR_vendor_id"]))
            {
                query.vendor_name_full_OR_vendor_id = Request.Params["vendor_name_full_OR_vendor_id"];//供應商名稱/供應商編號
            }
            // 要修改商品編號  改為 商品編號/名稱

            //if (!string.IsNullOrEmpty(Request.Params["product_id"]))//商品編號
            //{
            //    query.product_id = Convert.ToUInt32(Request.Params["product_id"]);
            //}

            if (!string.IsNullOrEmpty(Request.Params["product_id_OR_product_name"]))
            {
                query.product_id_OR_product_name = Request.Params["product_id_OR_product_name"];//商品编号/名称
            }

            if (!string.IsNullOrEmpty(Request.Params["start_time"]))//開始時間
            {
                query.start_time = Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00");
            }
            if (!string.IsNullOrEmpty(Request.Params["end_time"]))//結束時間
            {
                query.end_time = Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59");
            }
            List<ArrivalNoticeQuery> list = arrivalnoticemgr.GetArrNoticeList(query, out totalcount);
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return Response;

        }
        #endregion

        #region 顯示補貨通知詳情 
        public HttpResponseBase ShowArrByUserList()
        {
            string json = string.Empty;
            int totalcount = 0;
            ArrivalNoticeQuery query = new ArrivalNoticeQuery();

            //Ris.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            //Ris.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

            query.item_id = Convert.ToUInt32( Request.Params["item_id"]);
            arrivalnoticemgr = new ArrivalNoticeMgr(mySqlConnectionString);
            
            if (!string.IsNullOrEmpty(Request.Params["item_id"]))
            {
                string item_id = Request.Params["item_id"];
            }
            if (!string.IsNullOrEmpty(Request.Params["startTime"]))
            {
                query.start_time = Convert.ToDateTime(Request.Params["startTime"]).ToString("yyyy-MM-dd 00:00:00");
            }
            if (!string.IsNullOrEmpty(Request.Params["endTime"]))
            {
                query.end_time = Convert.ToDateTime(Request.Params["endTime"]).ToString("yyyy-MM-dd 23:59:59");
            }
            List<ArrivalNoticeQuery> List = arrivalnoticemgr.ShowArrByUserList(query, out totalcount);
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(List, Formatting.Indented,timeConverter) + "}";
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return Response;
        }
        #endregion

        #region 補貨通知人數新增
        public HttpResponseBase SaveArrivaleNotice()
        {
            ArrivalNotice arr = new ArrivalNotice();
           
             string jsonStr = String.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                { 
                    arr.id=uint.Parse(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    arr.product_id = uint.Parse(Request.Params["product_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                {
                    arr.item_id = uint.Parse(Request.Params["item_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["user_id"]))
                {
                    arr.user_id = uint.Parse(Request.Params["user_id"]);
                }
                arr.source_type = 2;
                arr.status = 0;
                arr.send_notice_time = 0;
                arr.create_time = uint.Parse(CommonFunction.GetPHPTime().ToString());
                arr.muser_id = (Session["caller"] as Caller).user_id;
                _arrivalMgr = new ArrivalNoticeMgr(mySqlConnectionString);
                if (arr.id == 0)//新增
                {
                    int result=_arrivalMgr.SaveArrivaleNotice(arr);
                    if (result == 1)
                    {
                        jsonStr = "{success:true,msg:\"" + 1 + "\"}";
                    }
                    if (result == 98)
                    {
                        jsonStr = "{success:true,msg:\"" + 98 + "\"}";//此人員已在未通知列表中
                    }
                    else 
                    {
                        jsonStr = "{success:true,msg:\"" + 99 + "\"}";
                    }
                   
                }
                else //修改
                {
                    int result = _arrivalMgr.UpArrivaleNoticeStatus(arr);
                    if (result==100)
                    {
                        jsonStr = "{success:true,msg:\"" + 100 + "\"}";
                        //jsonStr = "{success:true,msg:\"此此人不在補貨通知以內貨已取消通知！\"}";
                    }
                    else
                    {
                        jsonStr = "{success:true,msg:\"" + 99 + "\"}";
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

        public HttpResponseBase GetUserName()
        {
             Users user = new Users();
            if (!string.IsNullOrEmpty(Request.Params["user_id"]))
            {
                user.user_id = ulong.Parse(Request.Params["user_id"]);
            }
           
          
            _usersMgr = new UsersMgr(mySqlConnectionString);
            string jsonStr =string.Empty;
            try
            {
                List<Users> userList = new List<Users>();
                userList = _usersMgr.GetUser(user);
                if (userList.Count > 0)
                {
                    jsonStr = "{success:true,msg:\"" + userList[0].user_name + "\"}";
                }
                else 
                {
                    jsonStr = "{success:true,msg:\"" +100+ "\"}";
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
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase GetProductName()
        {
            ProductItemQuery pItem = new ProductItemQuery();
            if (!string.IsNullOrEmpty(Request.Params["item_id"]))
            {
                pItem.Item_Id = uint.Parse(Request.Params["item_id"]);
            }


            productItemMgr = new ProductItemMgr(mySqlConnectionString);
            string jsonStr = string.Empty;

            try
            {
                List<ProductItemQuery> ItemList = new List<ProductItemQuery>();
                ItemList = productItemMgr.GetProductItemByID(pItem);
                if (ItemList.Count > 0)
                {
                    jsonStr = "{success:true,msg:\"[" + ItemList[0].Product_Id + "]" + ItemList[0].Remark +" "+ ItemList[0].Spec_Name_1 + ItemList[0].Spec_Name_2 + "\",product_id:\"" + ItemList[0].Product_Id + "\"}";
                }
                else
                {
                    jsonStr = "{success:true,msg:\"" + 100 + "\"}";
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
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        public void ExportCSV()
        {
            ArrivalNoticeQuery query = new ArrivalNoticeQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["vendor_name_full_OR_vendor_id"]))
                {
                    query.vendor_name_full_OR_vendor_id = Request.Params["vendor_name_full_OR_vendor_id"];//供應商名稱/供應商編號
                }
                if (!string.IsNullOrEmpty(Request.Params["product_id_OR_product_name"]))//商品編號或商品名稱
                {
                    query.product_id_OR_product_name = Request.Params["product_id_OR_product_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))//開始時間
                {
                    query.start_time = Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))//結束時間
                {
                    query.end_time = Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59");
                }
                DataTable dtHZ = new DataTable();
                int totalcount = 0;
                query.IsPage = false;
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("商品編號", typeof(String));
                dtHZ.Columns.Add("商品名稱", typeof(String));
                dtHZ.Columns.Add("商品細項編號", typeof(String));
                dtHZ.Columns.Add("商品規格", typeof(String));
                dtHZ.Columns.Add("供應商編號", typeof(String));
                dtHZ.Columns.Add("供應商名稱", typeof(String));
                dtHZ.Columns.Add("補貨通知人數", typeof(String));
                List<ArrivalNoticeQuery> list = new List<ArrivalNoticeQuery>();
                arrivalnoticemgr = new ArrivalNoticeMgr(mySqlConnectionString);
                list = arrivalnoticemgr.GetArrNoticeList(query, out totalcount);

                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        DataRow dr = dtHZ.NewRow();
                        dr[0] = list[i].product_id;
                        dr[1] = list[i].product_name;
                        dr[2] = list[i].item_id;
                        dr[3] = list[i].product_spec;
                        dr[4] = list[i].vendor_id;
                        dr[5] = list[i].vendor_name_full;
                        dr[6] = list[i].ri_nums;
                        dtHZ.Rows.Add(dr);
                    }
                    string fileName = "補貨通知統計匯出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
                else
                {
                    Response.Clear();
                    this.Response.Write("無數據存在<br/>");
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

        public HttpResponseBase GetStatusListLowerShelf()
        {
            string json = string.Empty;

            DataTable dt = new DataTable();
            ProductQuery query = new ProductQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            int totalCount = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Short"]))
                {
                    query.Shortage = int.Parse(Request.Params["Short"]);
                }
                int p = 0;
                if (!string.IsNullOrEmpty(Request.Params["searchcon"].Trim()))
                {
                    if (int.TryParse(Request.Params["searchcon"].Trim(), out p))
                    {
                        if (Request.Params["searchcon"].Trim().Length == 5)
                        {
                            query.Product_Id = uint.Parse(Request.Params["searchcon"].Trim());
                        }
                        else if (Request.Params["searchcon"].Trim().Length == 6)
                        {
                            query.item_id = uint.Parse(Request.Params["searchcon"].Trim());
                        }
                        else
                        {
                            query.Product_Name = Request.Params["searchcon"].Trim();
                        }

                    }
                    else
                    {
                        query.Product_Name = Request.Params["searchcon"].Trim();
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["startIloc"].Trim()))
                {
                    query.loc_id = Request.Params["startIloc"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["endIloc"].Trim()))
                {
                    query.loc_id2 = Request.Params["endIloc"].Trim()+"Z";
                }
                if (!string.IsNullOrEmpty(Request.Params["fright_set"]))
                {
                    query.product_freight = int.Parse(Request.Params["fright_set"]);
                }
                DataTable _td = productItemMgr.GetStatusListLowerShelf(query, out totalCount);



                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(_td, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        public void GetStatusListLowerShelfExcel()
        {
            ProductQuery query = new ProductQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

            DataTable dt = new DataTable();
            query.IsPage = false;
            DataTable dtExcel = new DataTable();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["Short"]))
                {
                    query.Shortage = int.Parse(Request.Params["Short"]);
                }
                int p = 0;
                if (!string.IsNullOrEmpty(Request.Params["searchcon"].Trim()))
                {
                    if (int.TryParse(Request.Params["searchcon"].Trim(), out p))
                    {
                        if (Request.Params["searchcon"].Trim().Length == 5)
                        {
                            query.Product_Id = uint.Parse(Request.Params["searchcon"].Trim());
                        }
                        else if (Request.Params["searchcon"].Trim().Length == 6)
                        {
                            query.item_id = uint.Parse(Request.Params["searchcon"].Trim());
                        }
                        else
                        {
                            query.Product_Name = Request.Params["searchcon"].Trim();
                        }

                    }
                    else
                    {
                        query.Product_Name = Request.Params["searchcon"].Trim();
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["startIloc"].Trim()))
                {
                    query.loc_id = Request.Params["startIloc"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["endIloc"].Trim()))
                {
                    query.loc_id2 = Request.Params["endIloc"].Trim() + "Z";
                }
                if (!string.IsNullOrEmpty(Request.Params["fright_set"]))
                {
                    query.product_freight = int.Parse(Request.Params["fright_set"]);
                }

                int totalCount = 0;
                dt = productItemMgr.GetStatusListLowerShelf(query, out totalCount);

                //添加兩列用於存儲"平均平均量"與"建議採購量"
                dtExcel.Columns.Add("商品編號", typeof(String));
                dtExcel.Columns.Add("商品名稱", typeof(String));
                dtExcel.Columns.Add("料位編號", typeof(String));
                dtExcel.Columns.Add("商品細項編號", typeof(String));

                dtExcel.Columns.Add("組合Y/N", typeof(String));

                dtExcel.Columns.Add("前台庫存量", typeof(String));
                dtExcel.Columns.Add("後台庫存量", typeof(String));

                dtExcel.Columns.Add("溫層", typeof(String));
                dtExcel.Columns.Add("是否買斷", typeof(String));
                dtExcel.Columns.Add("商品狀態", typeof(String));
                dtExcel.Columns.Add("有庫存掛補", typeof(String));


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = dt.Rows[i]["product_id"];
                    newRow[1] = dt.Rows[i]["product_name"];
                    newRow[2] = dt.Rows[i]["loc_id"];
                    newRow[3] = dt.Rows[i]["item_id"];

                    if (!string.IsNullOrEmpty(dt.Rows[i]["combination"].ToString()))
                    {
                        int combination = Convert.ToInt32(dt.Rows[i]["combination"]);
                        switch (combination)
                        {

                            case 1:
                                newRow[4] = "N";
                                break;
                            case 2:
                            case 3:
                            case 4:
                                newRow[4] = "Y";
                                break;
                            default:
                                newRow[4] = combination;
                                break;
                        }
                    }
                    newRow[5] = dt.Rows[i]["item_stock"];
                    newRow[6] = dt.Rows[i]["iinvd_stock"];


                    if (!string.IsNullOrEmpty(dt.Rows[i]["product_freight"].ToString()))
                    {
                        int product_freight = Convert.ToInt32(dt.Rows[i]["product_freight"]);
                        if (product_freight == 1)
                            newRow[7] = "常溫";
                        if (product_freight == 2)
                            newRow[7] = "冷凍";
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[i]["prepaid"].ToString()))
                    {
                        int prepaid = Convert.ToInt32(dt.Rows[i]["prepaid"]);
                        if (prepaid == 0)
                            newRow[8] = "否";
                        if (prepaid == 1)
                            newRow[8] = "是";
                    }

                    newRow[9] = dt.Rows[i]["product_status_string"];

                    if (!string.IsNullOrEmpty(dt.Rows[i]["shortage"].ToString()))
                    {
                        int shortage = Convert.ToInt32(dt.Rows[i]["shortage"]);
                        if (shortage == 0)
                            newRow[10] = "否";
                        if (shortage == 1)
                            newRow[10] = "是";
                    }


                    dtExcel.Rows.Add(newRow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "下架狀態明細表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtExcel, "");
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
    }
}