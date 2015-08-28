﻿using BLL.gigade.Mgr;
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

        //BLL.gigade.Mgr.OrderDetailMgr orderDetailMgr = new BLL.gigade.Mgr.OrderDetailMgr(mySqlConnectionString);
        BLL.gigade.Mgr.ProductItemMgr productItemMgr = new BLL.gigade.Mgr.ProductItemMgr(mySqlConnectionString);
        private IItemIpoCreateLogImplMgr _ItemIpoMgr;// ItemIpoMgr = new BLL.gigade.Mgr.ProductItemMgr(mySqlConnectionString);
        IParametersrcImplMgr _paraMgr;
        #region 視圖
        //
        // GET: /ProductPurchase/

        public ActionResult Index()
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
            query.vendor_name = Request.Params["vendor_name"] ?? "";/*供應商名稱*/
            try
            {
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
            query.vendor_name = Request.Params["vendor_name"] ?? "";/*供應商名稱*/
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
                dtExcel.Columns.Add("商品名稱", typeof(String));
                dtExcel.Columns.Add("規格一", typeof(String));
                dtExcel.Columns.Add("規格二", typeof(String));
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
                dtExcel.Columns.Add("販售狀態", typeof(String));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = Convert.ToInt64(dt.Rows[i]["vendor_id"]);
                    newRow[1] = dt.Rows[i]["vendor_name_simple"];
                    newRow[2] = dt.Rows[i]["product_id"];
                    newRow[3] = dt.Rows[i]["item_id"];
                    newRow[4] = dt.Rows[i]["product_name"];
                    newRow[5] = dt.Rows[i]["spec_title_1"].ToString() + dt.Rows[i]["spec_id_1"].ToString();
                    newRow[6] = dt.Rows[i]["spec_title_2"].ToString() + dt.Rows[i]["spec_id_2"].ToString();
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
                    newRow[19] = dt.Rows[i]["sale_name"];
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
    }
}