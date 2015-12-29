using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class ReceiptShelvesController : Controller
    {
        //
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        IpoNvdMgr _IpoNvdMgr;
        IpoNvdLogMgr ipoNvdLogMgr;

        #region View

        // GET: /ReceiptShelves/
        /// <summary>
        /// 收貨上架列表
        /// </summary>
        /// <returns></returns>
        public ActionResult IpoNvdList()
        {
            return View();
        }
        // GET: /ReceiptShelves/
        /// <summary>
        /// IpoNvd表查詢與匯出
        /// </summary>
        /// <returns></returns>
        public ActionResult IpoNvdExport()
        {
            return View();
        } 
        /// <summary>
        /// 收貨上架輸入工作單號頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiptShelvesWorkid()
        {
            return View();
        }
        /// <summary>
        /// 收貨上架輸入條碼或Itemid頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiptShelvesItemid()
        {
            ViewBag.number = Request.Params["number"];
            return View();
        }

        /// <summary>
        /// 收貨上架
        /// </summary>
        /// <returns></returns>
        public ActionResult ReceiptShelvesIpoNvd()
        {
            ViewBag.number = Request.Params["number"];
            ViewBag.itemid = Request.Params["itemid"];
            return View();
        }
        /// <summary>
        /// ipo_nvd_log查詢頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult IpoNvdLogList()
        {
            return View();
        }
        
        #endregion


        //採購收穫上架記錄
        public HttpResponseBase GetIpoNvdList()// 
        {
            string json = string.Empty;
            IpoNvdQuery query = new IpoNvdQuery();
            int totalCount = 0;
            try
            {
                query.work_status = "AVL";

                if (!string.IsNullOrEmpty(Request.Params["work_id"]))
                {
                    query.work_id = Request.Params["work_id"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                {
                    query.item_id =Convert.ToUInt32(Request.Params["item_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["locid_allownull"]))
                {
                    query.locid_allownull = Convert.ToBoolean(Request.Params["locid_allownull"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["search_type"]))
                {
                    if (Request.Params["search_type"].ToString() == "ipo_id")
                    {
                        query.ipo_id = Request.Params["search_con"].ToString().Trim();
                    }
                    else if (Request.Params["search_type"].ToString() == "item_id")
                    {
                        query.item_id = Convert.ToUInt32(Request.Params["search_con"]);
                    }
                    else if (Request.Params["search_type"].ToString() == "work_id")
                    {
                        query.work_id = Request.Params["search_con"].ToString().Trim();
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["work_status"]))
                {
                    query.work_status = Request.Params["work_status"];
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"));
                }

                List<SchedulePeriodQuery> ipodStore = new List<SchedulePeriodQuery>();
                _IpoNvdMgr = new IpoNvdMgr(mySqlConnectionString);
                List<IpoNvdQuery> store = _IpoNvdMgr.GetIpoNvdList(query, out totalCount);
                int msg = 0;
                if (totalCount == 0)
                {
                    query.work_status = string.Empty;
                    _IpoNvdMgr.GetIpoNvdList(query, out msg);
                }

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                //timeConverter.DateTimeFormat = "yyyy-MM-dd";
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",msg:" + msg + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        //採購收穫上架記錄匯出
        public void ExportIpoNvdList()
        {
            string json = string.Empty;
            IpoNvdQuery query = new IpoNvdQuery();
            int totalCount = 0;
            try
            {
                query.work_status = "AVL";

                if (!string.IsNullOrEmpty(Request.Params["work_id"]))
                {
                    query.work_id = Request.Params["work_id"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["item_id"]))
                {
                    query.item_id = Convert.ToUInt32(Request.Params["item_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["locid_allownull"]))
                {
                    query.locid_allownull = Convert.ToBoolean(Request.Params["locid_allownull"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["search_type"]))
                {
                    if (Request.Params["search_type"].ToString() == "ipo_id")
                    {
                        query.ipo_id = Request.Params["search_con"].ToString().Trim();
                    }
                    else if (Request.Params["search_type"].ToString() == "item_id")
                    {
                        query.item_id = Convert.ToUInt32(Request.Params["search_con"]);
                    }
                    else if (Request.Params["search_type"].ToString() == "work_id")
                    {
                        query.work_id = Request.Params["search_con"].ToString().Trim();
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["work_status"]))
                {
                    query.work_status = Request.Params["work_status"];
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59"));
                }

                List<SchedulePeriodQuery> ipodStore = new List<SchedulePeriodQuery>();
                _IpoNvdMgr = new IpoNvdMgr(mySqlConnectionString);
                List<IpoNvdQuery> store = _IpoNvdMgr.GetIpoNvdList(query, out totalCount);
                
                //匯出excel
                //////
                DataTable dtExcel = new DataTable();
                dtExcel.Columns.Add("工作單號", typeof(String));
                dtExcel.Columns.Add("採購單編號", typeof(String));
                dtExcel.Columns.Add("商品細項編號", typeof(String));
                dtExcel.Columns.Add("採購單驗收數量", typeof(String));
                dtExcel.Columns.Add("未收貨上架數量", typeof(String));
                dtExcel.Columns.Add("完成收穫上架數量", typeof(String));
                dtExcel.Columns.Add("有效日期", typeof(String));
                dtExcel.Columns.Add("製造日期", typeof(String));
                dtExcel.Columns.Add("收穫上架狀態", typeof(String));
                dtExcel.Columns.Add("創建人", typeof(String));
                dtExcel.Columns.Add("創建時間", typeof(String));
                dtExcel.Columns.Add("修改人", typeof(String));
                dtExcel.Columns.Add("修改時間", typeof(String));
                for (int i = 0; i < store.Count; i++)
                {
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = store[i].work_id.ToString();
                    newRow[1] = store[i].ipo_id.ToString()+" ";
                    newRow[2] = store[i].item_id.ToString();
                    newRow[3] = store[i].ipo_qty.ToString();
                    newRow[4] = store[i].out_qty.ToString();
                    newRow[5] = store[i].com_qty.ToString();
                    newRow[6] = store[i].cde_dt.ToString("yyyy-MM-dd");
                    newRow[7] = store[i].made_date.ToString("yyyy-MM-dd");
                    newRow[8] = store[i].work_status.ToString();
                    if (store[i].work_status.ToString() == "AVL")
                    {
                        newRow[8] = "未處理";
                    }
                    else if (store[i].work_status.ToString() == "SKP")
                    {
                        newRow[8] = "已處理但未完成";
                    }
                    else if (store[i].work_status.ToString() == "COM")
                    {
                        newRow[8] = "已完成";
                    }

                    newRow[9] = store[i].create_username.ToString();
                    newRow[10] = store[i].create_datetime.ToString("yyyy-MM-dd HH:mm:ss");
                    newRow[11] = store[i].modify_username.ToString();
                    newRow[12] = store[i].modify_datetime.ToString("yyyy-MM-dd HH:mm:ss");

                    dtExcel.Rows.Add(newRow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "收貨上架_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    System.IO.MemoryStream ms = gigadeExcel.Comment.ExcelHelperXhf.ExportDT(dtExcel, fileName);
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
        //生成採購收穫上架工作單號
        public HttpResponseBase CreateTallyList()
        {
            string json = String.Empty;
            json = "{success:false}";
            IpoNvdQuery query = new IpoNvdQuery();
            string id=string.Empty;
            try 
	        {	        
		        if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    id = Request.Params["id"].ToString();
                    id = id.Substring(0, id.Length - 1).ToString();
                    query.modify_user = (Session["caller"] as Caller).user_id;
                }
                query.work_id = "IN" + DateTime.Now.ToString("yyyyMMddHHmmss");
                if(!string.IsNullOrEmpty(id))
                {
                    _IpoNvdMgr = new IpoNvdMgr(mySqlConnectionString);
                    int result = _IpoNvdMgr.CreateTallyList( query, id);
                    if (result > 0)
                    {
                        json = "{success:true,work_id:\""+query.work_id+"\"}";
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

        /// <summary>
        /// 收貨上架提交
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveReceiptShelves()
        {
            string json = String.Empty;
            json = "{success:false}";
            IpoNvdQuery query = new IpoNvdQuery();
            try
            {
                query.row_id = Convert.ToInt32(Request.Params["row_id"]);
                query.modify_user = (Session["caller"] as Caller).user_id;
                query.made_date = Convert.ToDateTime(Request.Params["made_date"]);
                query.cde_dt = Convert.ToDateTime(Request.Params["cde_dt"]);
                query.loc_id = Request.Params["loc_id"].ToString();
                int pick_num = Convert.ToInt32(Request.Params["pick_num"]);

                if (query.row_id != 0)
                {
                    _IpoNvdMgr = new IpoNvdMgr(mySqlConnectionString);
                    bool result = _IpoNvdMgr.SaveReceiptShelves(query,pick_num);

                    if (result)
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// IpoNvdLog查詢
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetIpoNvdLogList()
        {
            string json = string.Empty;
            int totalcount = 0;
            IpoNvdLogQuery query = new IpoNvdLogQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            ipoNvdLogMgr = new IpoNvdLogMgr(mySqlConnectionString);
            if (!string.IsNullOrEmpty(Request.Params["work_id"].Trim()))
            {
                query.work_id = Request.Params["work_id"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.Params["ipo_id"].Trim()))
            {
                query.ipo_id = Request.Params["ipo_id"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.Params["itemId_or_upcId"].Trim()))
            {
                uint itemId = Convert.ToUInt32(Request.Params["itemId_or_upcId"].Trim());
                bool result = ipoNvdLogMgr.GetInfoByItemId(itemId);

                if (result == false)
                {
                    query.upc_id = Request.Params["itemId_or_upcId"].Trim();//條碼
                }
                else
                {
                    query.item_id = Convert.ToUInt32(Request.Params["itemId_or_upcId"].Trim());
                }
            }
            if (!string.IsNullOrEmpty(Request.Params["loc_id"].Trim()))
            {
                query.loc_id = Request.Params["loc_id"].Trim();
            }
            if (!string.IsNullOrEmpty(Request.Params["time_start"]))//開始時間
            {
                query.start_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["time_start"]).ToString("yyyy-MM-dd 00:00:00"));
            }
            if (!string.IsNullOrEmpty(Request.Params["time_end"]))//結束時間
            {
                query.end_time = Convert.ToDateTime(Convert.ToDateTime(Request.Params["time_end"]).ToString("yyyy-MM-dd 23:59:59"));
            }
            try
            {
                List<IpoNvdLogQuery> list = ipoNvdLogMgr.GetIpoNvdLogList(query, out totalcount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";
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
            return Response;
        }

    }
}
