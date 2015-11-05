using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Text;
using BLL.gigade.Model;

namespace Admin.gigade.Controllers
{
    public class ReportManagementController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IDeliverMasterImplMgr _delverMgr = new DeliverMasterMgr(mySqlConnectionString);
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private IDeliverMasterImplMgr _DeliverMsterMgr;
        private IParametersrcImplMgr _ptersrc;
        //
        // GET: /ReportManagement/

        public ActionResult Index()//訂單物流狀態查詢
        {
            return View();
        }
        public ActionResult OrderAllSearch()//訂單細項查詢
        {
            return View();
        }
        #region 列表頁
        public HttpResponseBase GetReportManagementList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DeliverMasterQuery dmQuery = new DeliverMasterQuery();
            dmQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            dmQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    dmQuery.sorder_id = Request.Params["order_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))
                {
                    dmQuery.sdeliver_id = Request.Params["deliver_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["logistics_type"]))
                {
                    dmQuery.logisticsType = Convert.ToInt32(Request.Params["logistics_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["delivery_status"]))
                {
                    dmQuery.ideliver_status = Convert.ToInt32(Request.Params["delivery_status"]);
                }
                dmQuery.deliver_store = Convert.ToInt32(Request.Params["shipment_id"]);
                dmQuery.i_order_status = Convert.ToInt32(Request.Params["order_status_id"]);
                dmQuery.payment = Convert.ToInt32(Request.Params["payment_id"]);
                dmQuery.order_time_begin = Convert.ToDateTime(Convert.ToDateTime(Request.Params["dateStart"]).ToString("yyyy-MM-dd 00:00:00"));//建立時間
                dmQuery.order_time_end = Convert.ToDateTime(Convert.ToDateTime(Request.Params["dateEnd"]).ToString("yyyy-MM-dd 23:59:59"));

                int totalCount = 0;
                _dt = _delverMgr.GetReportManagementList(dmQuery, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
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

        #region 匯出Excel信息
        public void ReportManagementExcelList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            DeliverMasterQuery dmQuery = new DeliverMasterQuery();
            try
            {
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("出貨單號", typeof(String));
                dtHZ.Columns.Add("訂單編號", typeof(String));
                dtHZ.Columns.Add("物流單號", typeof(String));
                dtHZ.Columns.Add("付款方式", typeof(String));
                dtHZ.Columns.Add("物流業者", typeof(String));
                dtHZ.Columns.Add("訂單狀態", typeof(String));
                dtHZ.Columns.Add("出貨單狀態", typeof(String));
                dtHZ.Columns.Add("物流狀態", typeof(String));
                dtHZ.Columns.Add("訂單日期", typeof(String));
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    dmQuery.sorder_id = Request.Params["order_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))
                {
                    dmQuery.sdeliver_id = Request.Params["deliver_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["delivery_status"]))
                {
                    dmQuery.ideliver_status = Convert.ToInt32(Request.Params["delivery_status"]);
                }
                dmQuery.deliver_store = Convert.ToInt32(Request.QueryString["shipment_id"]);
                dmQuery.i_order_status = Convert.ToInt32(Request.QueryString["order_status_id"]);
                dmQuery.payment = Convert.ToInt32(Request.QueryString["payment_id"]);
                dmQuery.order_time_begin = Convert.ToDateTime(Convert.ToDateTime(Request.Params["dateStart"]).ToString("yyyy-MM-dd 00:00:00"));//建立時間
                dmQuery.order_time_end = Convert.ToDateTime(Convert.ToDateTime(Request.Params["dateEnd"]).ToString("yyyy-MM-dd 23:59:59"));
                if (!string.IsNullOrEmpty(Request.Params["logistics_type"]))
                {
                    dmQuery.logisticsType = Convert.ToInt32(Request.Params["logistics_type"]);
                }
                _dt = _delverMgr.ReportManagementExcelList(dmQuery);
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = _dt.Rows[i]["deliver_id"];
                    dr[1] = _dt.Rows[i]["order_id"];
                    dr[2] = _dt.Rows[i]["delivery_code"];
                    dr[3] = _dt.Rows[i]["order_payment"];
                    dr[4] = _dt.Rows[i]["delivery_store"];
                    dr[5] = _dt.Rows[i]["order_status"];
                    dr[6] = _dt.Rows[i]["delivery_status"];
                    dr[7] = _dt.Rows[i]["logisticsTypes"];
                    dr[8] = Convert.ToDateTime(_dt.Rows[i]["order_date"]).ToString("yyyy-MM-dd HH:mm:ss");
                    dtHZ.Rows.Add(dr);
                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = DateTime.Now.ToString("訂單物流狀態報表_yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "訂單物流狀態報表_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
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

        #region 訂單細項信息
        public HttpResponseBase GetDeliversList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DeliverMasterQuery dmQuery = new DeliverMasterQuery();
            dmQuery.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            dmQuery.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    dmQuery.sorder_id = Request.Params["order_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))
                {
                    dmQuery.sdeliver_id = Request.Params["deliver_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["logistics_type"]))
                {
                    dmQuery.logisticsType = Convert.ToInt32(Request.Params["logistics_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["delivery_status"]))
                {
                    dmQuery.ideliver_status = Convert.ToInt32(Request.Params["delivery_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["product_mode"]))
                {
                    dmQuery.product_mode = Convert.ToInt32(Request.Params["product_mode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["serch_msg"]))
                {
                    dmQuery.serch_msg = Convert.ToInt32(Request.Params["serch_msg"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["serch_where"].Trim()))
                {
                    dmQuery.serch_where = Request.Params["serch_where"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["order_day"]))
                {
                    dmQuery.order_day = int.Parse(Request.Params["order_day"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["t_days"]))
                {
                    dmQuery.t_days = Convert.ToInt32(Request.Params["t_days"]);
                }
                else
                {
                    dmQuery.t_days = -1;
                }
                dmQuery.time_type = Convert.ToInt32(Request.Params["serch_time"]);
                dmQuery.deliver_store = Convert.ToInt32(Request.Params["shipment_id"]);
                dmQuery.i_order_status = Convert.ToInt32(Request.Params["order_status_id"]);
                dmQuery.i_slave_status = Convert.ToInt32(Request.Params["slave_status_id"]);
                dmQuery.i_detail_status = Convert.ToInt32(Request.Params["detail_status_id"]);
                dmQuery.payment = Convert.ToInt32(Request.Params["payment_id"]);
                dmQuery.order_time_begin = Convert.ToDateTime(Convert.ToDateTime(Request.Params["dateStart"]).ToString("yyyy-MM-dd 00:00:00"));//建立時間
                dmQuery.order_time_end = Convert.ToDateTime(Convert.ToDateTime(Request.Params["dateEnd"]).ToString("yyyy-MM-dd 23:59:59"));

                int totalCount = 0;
                _dt = _delverMgr.GetDeliveryMsgList(dmQuery, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
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
            this.Response.Write(json.ToString());
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 匯出Excel信息
        public void GetDeliversExcelList()
        {
            string json = string.Empty;
            DataTable _dt = new DataTable();
            DataTable dtHZ = new DataTable();
            DeliverMasterQuery dmQuery = new DeliverMasterQuery();
            try
            {
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("訂單成立天數", typeof(String));
                dtHZ.Columns.Add("付款單成立日期", typeof(String));
                dtHZ.Columns.Add("訂單編號", typeof(String));
                dtHZ.Columns.Add("出貨商簡稱", typeof(String));
                dtHZ.Columns.Add("供應商簡稱",typeof(String));

                dtHZ.Columns.Add("運送方式", typeof(String));
                dtHZ.Columns.Add("收貨人", typeof(String));
                dtHZ.Columns.Add("細項編號", typeof(String));
                dtHZ.Columns.Add("商品名稱", typeof(String));
                dtHZ.Columns.Add("出貨方式", typeof(String));

                dtHZ.Columns.Add("數量", typeof(String));
                dtHZ.Columns.Add("付款方式", typeof(String));
                dtHZ.Columns.Add("付款單狀態", typeof(String));
                dtHZ.Columns.Add("訂單狀態", typeof(String));
                dtHZ.Columns.Add("商品狀態", typeof(String));

                dtHZ.Columns.Add("訂單備註",typeof(String));
                dtHZ.Columns.Add("管理員備註", typeof(String));
                dtHZ.Columns.Add("出貨單狀態", typeof(String));
                dtHZ.Columns.Add("物流狀態", typeof(String));
                dtHZ.Columns.Add("可出貨日期", typeof(String));

                dtHZ.Columns.Add("預計到貨時段", typeof(String));
                dtHZ.Columns.Add("預計出貨時間", typeof(String));//最近出貨時間預計出貨時間
                dtHZ.Columns.Add("預計到貨時間", typeof(String));//貨物運達時間預計到貨時間
                dtHZ.Columns.Add("出貨時間", typeof(String));
                dtHZ.Columns.Add("到貨時間", typeof(String));
              
                dtHZ.Columns.Add("物流單號", typeof(String));
                dtHZ.Columns.Add("物流業者", typeof(String));
                if (!string.IsNullOrEmpty(Request.Params["order_id"]))
                {
                    dmQuery.sorder_id = Request.Params["order_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["deliver_id"]))
                {
                    dmQuery.sdeliver_id = Request.Params["deliver_id"];
                }
                if (!string.IsNullOrEmpty(Request.Params["logistics_type"]))
                {
                    dmQuery.logisticsType = Convert.ToInt32(Request.Params["logistics_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["delivery_status"]))
                {
                    dmQuery.ideliver_status = Convert.ToInt32(Request.Params["delivery_status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["product_mode"]))
                {
                    dmQuery.product_mode = Convert.ToInt32(Request.Params["product_mode"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["serch_msg"]) && Request.Params["serch_msg"] != "null")
                {
                    dmQuery.serch_msg = Convert.ToInt32(Request.Params["serch_msg"]);
                }

                if (!string.IsNullOrEmpty(Request.Params["serch_where"].Trim()))
                {
                    dmQuery.serch_where = Request.Params["serch_where"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["order_day"]) && Request.Params["order_day"] != "null")
                {
                    dmQuery.order_day = int.Parse(Request.Params["order_day"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["t_days"]) && Request.Params["t_days"] != "null")
                {
                    dmQuery.t_days = Convert.ToInt32(Request.Params["t_days"]);
                }
                else
                {
                    dmQuery.t_days = -1;
                }
                dmQuery.time_type= Convert.ToInt32(Request.Params["serch_time"]);
                dmQuery.deliver_store = Convert.ToInt32(Request.Params["shipment_id"]);
                dmQuery.i_order_status = Convert.ToInt32(Request.Params["order_status_id"]);
                dmQuery.i_slave_status = Convert.ToInt32(Request.Params["slave_status_id"]);
                dmQuery.i_detail_status = Convert.ToInt32(Request.Params["detail_status_id"]);
                dmQuery.payment = Convert.ToInt32(Request.Params["payment_id"]);
                dmQuery.order_time_begin = Convert.ToDateTime(Convert.ToDateTime(Request.Params["dateStart"]).ToString("yyyy-MM-dd 00:00:00"));//建立時間
                dmQuery.order_time_end = Convert.ToDateTime(Convert.ToDateTime(Request.Params["dateEnd"]).ToString("yyyy-MM-dd 23:59:59"));

                _dt = _delverMgr.GetDeliveryMsgExcelList(dmQuery);
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow();
                    dr[0] = _dt.Rows[i]["overdue_day"];
                    dr[1] = _dt.Rows[i]["order_date"];
                    dr[2] = _dt.Rows[i]["order_id"];
                    dr[3] = _dt.Rows[i]["dvendor_name_simple"];
                    dr[4] = _dt.Rows[i]["vendor_name_simple"];

                    dr[5] = _dt.Rows[i]["freight_set"];
                    dr[6] = _dt.Rows[i]["delivery_name"];
                    dr[7] = _dt.Rows[i]["item_id"];
                    dr[8] = _dt.Rows[i]["product_name"];
                    dr[9] = _dt.Rows[i]["product_mode"];

                    dr[10] = _dt.Rows[i]["buy_num"];
                    dr[11] = _dt.Rows[i]["order_payment"];
                    dr[12] = _dt.Rows[i]["order_status"];
                    dr[13] = _dt.Rows[i]["slave_status"];
                    dr[14] = _dt.Rows[i]["detail_status"];


                    dr[15] = _dt.Rows[i]["note_order"];
                    dr[16] = _dt.Rows[i]["note_admin"];
                    dr[17] = _dt.Rows[i]["delivery_status"];
                    dr[18] = _dt.Rows[i]["logisticsTypes"];
                    dr[19] = _dt.Rows[i]["deliver_master_date"];


                    if (Convert.ToInt32(_dt.Rows[i]["estimated_arrival_period"]) == 0)
                    {
                        dr[20] = _dt.Rows[i]["estimated_arrival_period"];
                    }

                    else if (Convert.ToInt32(_dt.Rows[i]["estimated_arrival_period"]) == 1)
                    {
                        dr[20] = "12:00以前";
                    }
                    else if (Convert.ToInt32(_dt.Rows[i]["estimated_arrival_period"]) == 2)
                    {
                        dr[20] = "12:00-17:00";
                    }
                    else if (Convert.ToInt32(_dt.Rows[i]["estimated_arrival_period"]) == 3)
                    {
                        dr[20] = "17:00-20:00";
                    }
                    else
                    {
                        dr[20] = Convert.ToInt32(_dt.Rows[i]["estimated_arrival_period"]);
                    }
                    dr[21] = _dt.Rows[i]["estimated_delivery_date"];
                    dr[22] = _dt.Rows[i]["estimated_arrival_date"];
                    dr[23] = _dt.Rows[i]["delivery_date"];
                    dr[24] = _dt.Rows[i]["arrival_date"];

                    dr[25] = _dt.Rows[i]["delivery_code"];
                    dr[26] = _dt.Rows[i]["delivery_store"];
                    dtHZ.Rows.Add(dr);
                }
                if (dtHZ.Rows.Count > 0)
                {
                    string fileName = DateTime.Now.ToString("訂單細項報表_yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(dtHZ, "訂單細項報表_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
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
    }
}
