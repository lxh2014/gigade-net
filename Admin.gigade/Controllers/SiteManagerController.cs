#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：SiteManagerController.cs      
* 摘 要：                                                                               
* 網頁元數據管理
* 当前版本：v1.0                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2015/5/6
* 修改歷史：                                                                     
*        
*/

#endregion



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model;
using BLL.gigade.Mgr;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Data;
using BLL.gigade.Common;
using System.Configuration;
using System.IO;
using gigadeExcel.Comment;
namespace Admin.gigade.Controllers
{
    public class SiteManagerController : Controller
    {
        // 
        // GET: /SiteManager/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private PageMetaDataMgr _pageMetaMgr;
        private SiteStatisticsMgr ssMgr;
        private SiteAnalyticsMgr _siteAnalytics;
        private SiteMgr _siteMgr;

        #region 視圖
        /// <summary>
        /// 網頁元數據管理
        /// </summary>
        /// <returns></returns>
        public ActionResult PageMeta()
        {
            return View();
        }
        public ActionResult SiteStatisticsList()
        {
            return View();
        }

        public ActionResult SiteAnalytics()
        {

            return View();
        }
        #endregion

        #region 獲取page_metadate網頁元數據列表
        public HttpResponseBase GetPageMetaList()
        {

            string json = string.Empty;
            try
            {
                PageMetaData query = new PageMetaData();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                else
                {
                    query.Limit = 25;
                }
                if (!string.IsNullOrEmpty(Request.Params["search_content"]))
                {
                    query.pm_page_name = Request.Params["search_content"].ToString();
                    query.pm_title = query.pm_page_name;
                    query.pm_keywords = query.pm_page_name;
                }
                int totalCount = 0;
                _pageMetaMgr = new PageMetaDataMgr(mySqlConnectionString);
                List<PageMetaData> stores = _pageMetaMgr.GetPageMetaDataList(query, ref totalCount);

                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 網頁元數據保存
        public HttpResponseBase SavePageMeta()
        {

            string json = "{success:false}";
            try
            {
                PageMetaData query = new PageMetaData();
                if (!string.IsNullOrEmpty(Request.Params["pm_id"]))
                {
                    query.pm_id = Convert.ToInt32(Request.Params["pm_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["pm_page_name"]))
                {
                    query.pm_page_name = Request.Params["pm_page_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["pm_title"]))
                {
                    query.pm_title = Request.Params["pm_title"];
                }
                if (!string.IsNullOrEmpty(Request.Params["pm_url_para"]))
                {
                    query.pm_url_para = Request.Params["pm_url_para"];
                }
                if (!string.IsNullOrEmpty(Request.Params["pm_keywords"]))
                {
                    query.pm_keywords = Request.Params["pm_keywords"];
                }
                if (!string.IsNullOrEmpty(Request.Params["pm_description"]))
                {
                    query.pm_description = Request.Params["pm_description"];
                }
                _pageMetaMgr = new PageMetaDataMgr(mySqlConnectionString);
                if (query.pm_id == 0)
                {
                    query.pm_created = DateTime.Now;
                    query.pm_modified = query.pm_created;
                    query.pm_create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    query.pm_modify_user = query.pm_create_user;
                    if (_pageMetaMgr.InsertPageMeta(query) > 0)
                    {
                        json = "{success:true}";
                    }
                }
                else
                {
                    query.pm_modified = query.pm_created;
                    query.pm_modify_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                    if (_pageMetaMgr.UpdatePageMeta(query) > 0)
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

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 刪除網頁元數據
        public HttpResponseBase DeleteEdmPageMeta()
        {

            string json = "{success:false}";
            try
            {
                string rowID = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))
                {
                    rowID = Request.Params["rowID"];
                    _pageMetaMgr = new PageMetaDataMgr(mySqlConnectionString);
                    if (_pageMetaMgr.DeletePageMeta(rowID) > 0)
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
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 站台訪問量統計
        public HttpResponseBase GetSiteStatisticsList()
        {
            string json = string.Empty;
            try
            {
                if (Request.Files.Count > 0)
                {
                    string path = Request.Params["ImportExcel"];
                    HttpPostedFileBase excelFile = Request.Files["ImportExcel"];
                    FileManagement fileManagement = new FileManagement();
                    string savePath=Server.MapPath(excelPath) + "statistics";
                    string newExcelName = savePath+ fileManagement.NewFileName(excelFile.FileName);
                    excelFile.SaveAs(newExcelName);
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    DataTable _dt = helper.ExcelToTableForXLSX();
                    ssMgr = new SiteStatisticsMgr(mySqlConnectionString);
                    json = ssMgr.ImportExcelToDt(_dt);//匯入成功 
                }
                else
                {
                    SiteStatistics query = new SiteStatistics();
                    ssMgr = new SiteStatisticsMgr(mySqlConnectionString);
                    query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                    query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                    query.ss_code = Request.Params["ss_code"];
                    if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                    {
                        query.sss_date = Convert.ToDateTime(DateTime.Parse(Request.Params["startdate"]).ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                    {
                        query.ess_date = Convert.ToDateTime(DateTime.Parse(Request.Params["enddate"]).ToString("yyyy-MM-dd 23:59:59"));
                    }
                    #region  用來判斷相同的廠家代碼和時間是否已經存在
                    if (!string.IsNullOrEmpty(Request.Params["ss_id"]))
                    {
                        query.ss_id = int.Parse(Request.Params["ss_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["ss_date"]))
                    {
                        query.ss_date = DateTime.Parse(Request.Params["ss_date"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["ispage"]))
                    {
                        query.IsPage = bool.Parse(Request.Params["ispage"]);
                    }
                    #endregion
                    int totalCount = 0;
                    DataTable dt = ssMgr.GetSiteStatisticsList(query, out totalCount);
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    //listUser是准备转换的对象
                    json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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
        public HttpResponseBase SiteStatisticsEdit()
        {
            string json = string.Empty;
            SiteStatistics ss = new SiteStatistics();
            ssMgr = new SiteStatisticsMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["ss_show_num"]))
                {
                    ss.ss_show_num = int.Parse(Request.Params["ss_show_num"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ss_click_num"]))
                {
                    ss.ss_click_num = int.Parse(Request.Params["ss_click_num"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ss_click_through"]))
                {
                    ss.ss_click_through = float.Parse(Request.Params["ss_click_through"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ss_cost"]))
                {
                    ss.ss_cost = float.Parse(Request.Params["ss_cost"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ss_converted_newuser"]))
                {
                    ss.ss_converted_newuser = int.Parse(Request.Params["ss_converted_newuser"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ss_sum_order_amount"]))
                {
                    ss.ss_sum_order_amount = int.Parse(Request.Params["ss_sum_order_amount"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ss_newuser_number"]))
                {
                    ss.ss_newuser_number = int.Parse(Request.Params["ss_newuser_number"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ss_date"]))
                {
                    ss.ss_date = DateTime.Parse(Request.Params["ss_date"]);
                }
                ss.ss_code = Request.Params["ss_code"];
                ss.ss_create_user = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                ss.ss_create_time = DateTime.Now;
                ss.ss_modify_user = ss.ss_create_user;
                ss.ss_modify_time = ss.ss_create_time;
                #region 新增
                if (String.IsNullOrEmpty(Request.Params["ss_id"]))
                {
                    if (ssMgr.Insert(ss) > 0)
                    {
                        json = "{success:true,msg:'新增成功！'}";
                    }

                }
                #endregion
                #region 編輯
                else
                {
                    ss.ss_id = int.Parse(Request.Params["ss_id"]);
                    if (ssMgr.Update(ss) > 0)
                    {
                        json = "{success:true,msg:'修改成功！'}";
                    }

                }
                #endregion
            }
            catch (Exception ex)
            {
                json = "{success:false,msg:'異常！'}";
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
        /// <summary>
        /// 刪除
        /// </summary>
        /// <returns></returns>
        public JsonResult Delete()
        {
            int id = Convert.ToInt32(Request.Params["id"]);
            ssMgr = new SiteStatisticsMgr(mySqlConnectionString);
            SiteStatistics ss = new SiteStatistics();
            ss.ss_id = id;
            if (ssMgr.Delete(ss) > 0)
            {
                return Json(new { success = "true", msg = "" });
            }
            else
            {
                return Json(new { success = "false", msg = "" });
            }
        }


        public void ExportExcelStatistics()
        {
            try
            {
                int totalCount = 0;
                SiteStatistics query = new SiteStatistics();
                ssMgr = new SiteStatisticsMgr(mySqlConnectionString);
                query.ss_code = Request.Params["ss_code"];
                if (!string.IsNullOrEmpty(Request.Params["startdate"]))
                {
                    query.sss_date = Convert.ToDateTime(DateTime.Parse(Request.Params["startdate"]).ToString("yyyy-MM-dd 00:00:00"));
                }
                if (!string.IsNullOrEmpty(Request.Params["enddate"]))
                {
                    query.ess_date = Convert.ToDateTime(DateTime.Parse(Request.Params["enddate"]).ToString("yyyy-MM-dd 23:59:59"));
                }
                DataTable _dt = ssMgr.GetSiteStatisticsList(query, out totalCount);
                DataTable _newDt = new DataTable();
                _newDt.Columns.Add("日期", typeof(string));
                _newDt.Columns.Add("曝光", typeof(string));
                _newDt.Columns.Add("點擊", typeof(string));
                _newDt.Columns.Add("點閱率", typeof(string));
                _newDt.Columns.Add("費用", typeof(string));
                _newDt.Columns.Add("會員數", typeof(string));
                //_newDt.Columns.Add("新會員成本", typeof(string));
                _newDt.Columns.Add("實際轉換", typeof(string));
                // _newDt.Columns.Add("轉換率", typeof(string));
                //_newDt.Columns.Add("平均訂單金額", typeof(string));
                _newDt.Columns.Add("訂單金額", typeof(string));
                _newDt.Columns.Add("廠商代碼", typeof(string));
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow newRow = _newDt.NewRow();
                    newRow["日期"] = Convert.ToDateTime(_dt.Rows[i]["ss_date"]).ToString("MM/dd/yyyy"); ;
                    newRow["曝光"] = GetString(_dt.Rows[i]["ss_show_num"].ToString());
                    newRow["點擊"] = GetString(_dt.Rows[i]["ss_click_num"].ToString());
                    newRow["點閱率"] = _dt.Rows[i]["ss_click_through"].ToString();
                    newRow["費用"] = _dt.Rows[i]["ss_cost"].ToString();
                    newRow["會員數"] = GetString(_dt.Rows[i]["ss_newuser_number"].ToString());
                    newRow["實際轉換"] = GetString(_dt.Rows[i]["ss_converted_newuser"].ToString());
                    newRow["訂單金額"] = GetString(_dt.Rows[i]["ss_sum_order_amount"].ToString());
                    newRow["廠商代碼"] = _dt.Rows[i]["ss_code"].ToString();
                    //newRow[1] = GetString(_dt.Rows[i]["ss_show_num"].ToString());
                    //newRow[2] = GetString(_dt.Rows[i]["ss_click_num"].ToString());

                    _newDt.Rows.Add(newRow);
                }
                //_dt.Columns["sa_date"].ColumnName = "日索引";
                //_dt.Columns["sa_work_stage"].ColumnName = "工作階段";
                //_dt.Columns["sa_user"].ColumnName = "使用者";
                string fileName = "站臺訪問量統計" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(_newDt, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
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

        #region SiteAnalytics目標對
        public HttpResponseBase GetSiteAnalyticsList()
        {
            string json = string.Empty;
            try
            {
                SiteAnalytics query = new BLL.gigade.Model.SiteAnalytics();
                List<SiteAnalytics> store = new List<BLL.gigade.Model.SiteAnalytics>();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                {
                    query.search_con = Convert.ToInt32(Request.Params["search_con"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["serch_sa_date"]))
                {
                    query.s_sa_date = (Convert.ToDateTime(Request.Params["serch_sa_date"]).ToString("yyyy-MM-dd"));
                }
                _siteAnalytics = new SiteAnalyticsMgr(mySqlConnectionString);
                store = _siteAnalytics.GetSiteAnalyticsList(query, out totalCount);

                foreach (var item in store)
                {
                    item.s_sa_date = (item.sa_date).ToString("yyyy-MM-dd");
                    item.s_sa_create_time = (item.sa_create_time).ToString("yyyy-MM-dd HH:mm:ss");
                    item.sa_modify_time_query = (item.sa_modify_time).ToString("yyyy-MM-dd HH:mm:ss");
                }
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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

        public HttpResponseBase ImportExcel()
        {
            string json = string.Empty;
            try
            {
                BLL.gigade.Model.SiteAnalytics query = new BLL.gigade.Model.SiteAnalytics();
                if (Request.Files.Count > 0)
                {
                    string path = Request.Params["ImportExcel"];
                    HttpPostedFileBase excelFile = Request.Files["ImportExcel"];
                    FileManagement fileManagement = new FileManagement();
                    string newExcelName = Server.MapPath(excelPath) + "analytics" + fileManagement.NewFileName(excelFile.FileName);
                    excelFile.SaveAs(newExcelName);
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    DataTable _dt = helper.ExcelToTableForXLSX();
                    _siteAnalytics = new SiteAnalyticsMgr(mySqlConnectionString);
                    if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                    {
                        query.search_con = Convert.ToInt32(Request.Params["search_con"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["serch_sa_date"]))
                    {
                        query.s_sa_date = (Convert.ToDateTime(Request.Params["serch_sa_date"]).ToString("yyyy-MM-dd"));
                    }
                    json = _siteAnalytics.ImportExcelToDt(_dt);//匯入成功 
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

        public void ExportExcel()
        {
            try
            {
                BLL.gigade.Model.SiteAnalytics query = new BLL.gigade.Model.SiteAnalytics();
                _siteAnalytics = new SiteAnalyticsMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                {
                    query.search_con = Convert.ToInt32(Request.Params["search_con"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["serch_sa_date"]))
                {
                    query.s_sa_date = (Convert.ToDateTime(Request.Params["serch_sa_date"]).ToString("yyyy-MM-dd"));
                }
                DataTable _dt = _siteAnalytics.SiteAnalyticsDt(query);
                DataTable _newDt = new DataTable();
                _newDt.Columns.Add("日索引", typeof(string));
                _newDt.Columns.Add("造訪數", typeof(string));
                _newDt.Columns.Add("使用者", typeof(string));

                _newDt.Columns.Add("瀏覽量", typeof(string));
                _newDt.Columns.Add("單次造訪頁數", typeof(string));
                _newDt.Columns.Add("跳出率", typeof(string));
                _newDt.Columns.Add("平均停留時間", typeof(string));


                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    DataRow newRow = _newDt.NewRow();
                    newRow[0] = Convert.ToDateTime(_dt.Rows[i]["sa_date"]).ToString("yyyy-MM-dd"); ;
                    newRow[1] = GetString(_dt.Rows[i]["sa_session"].ToString());
                    newRow[2] = GetString(_dt.Rows[i]["sa_user"].ToString());

                    newRow[3] = GetString(_dt.Rows[i]["sa_pageviews"].ToString());
                    newRow[4] = GetString(_dt.Rows[i]["sa_pages_session"].ToString());
                    newRow[5] = _dt.Rows[i]["sa_bounce_rate"].ToString();
                    newRow[6] = _dt.Rows[i]["sa_avg_session_duration"].ToString();
                    _newDt.Rows.Add(newRow);
                }
                //_dt.Columns["sa_date"].ColumnName = "日索引";
                //_dt.Columns["sa_work_stage"].ColumnName = "工作階段";
                //_dt.Columns["sa_user"].ColumnName = "使用者";
                string fileName = "SiteAnalytics目標對" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(_newDt, "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
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

        public JsonResult SaveSiteAnalytics()
        {
            string json = string.Empty;
            SiteAnalytics query = new SiteAnalytics();
            DateTime date;
            int number = 0;
            float number1 = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["sa_pageviews"]))
                {
                    if (int.TryParse(Request.Params["sa_pageviews"], out number))
                    {
                        query.sa_pageviews = number;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["sa_pages_session"]))
                {
                    if (float.TryParse(Request.Params["sa_pages_session"], out number1))
                    {
                        query.sa_pages_session = number1;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["sa_bounce_rate"]))
                {
                    if (float.TryParse(Request.Params["sa_bounce_rate"], out number1))
                    {
                        query.sa_bounce_rate = number1;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["sa_avg_session_duration"]))
                {
                    if (float.TryParse(Request.Params["sa_avg_session_duration"], out number1))
                    {
                        query.sa_avg_session_duration = number1;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["sa_id"]))
                {
                    query.sa_id = Convert.ToInt32(Request.Params["sa_id"]);
                }

                if (DateTime.TryParse(Request.Params["sa_date"], out date))
                {
                    query.s_sa_date = date.ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrEmpty(Request.Params["sa_session"]))
                {
                    if (int.TryParse(Request.Params["sa_session"], out number))
                    {
                        query.sa_session = number;
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["sa_user"]))
                {
                    if (int.TryParse(Request.Params["sa_user"], out number))
                    {
                        query.sa_user = number;
                    }
                }

                query.sa_create_user = (Session["caller"] as Caller).user_id;
                query.sa_create_time=DateTime.Now;
                _siteAnalytics = new SiteAnalyticsMgr(mySqlConnectionString);
                query.sa_modify_user = query.sa_create_user;
                query.sa_modify_time=query.sa_create_time;
                if (query.sa_id == 0)
                {
                    if (_siteAnalytics.InsertSiteAnalytics(query) > 0)
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
                }
                else
                {
                    if (_siteAnalytics.UpdateSiteAnalytics(query) > 0)
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = "false" });
                    }
                }
            }
            catch (Exception ex)
            {

                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }
        }

        public JsonResult DeleteSiteAnalyticsById()
        {
            try
            {
                SiteAnalytics query = new SiteAnalytics();
                if (!string.IsNullOrEmpty(Request.Params["ids"]))
                {
                    query.sa_ids = Request.Params["ids"].TrimEnd(',');
                }
                _siteAnalytics = new SiteAnalyticsMgr(mySqlConnectionString);
                if (_siteAnalytics.DeleteSiteAnalytics(query) > 0)
                {
                    return Json(new { success = "true" });
                }
                return Json(new { success = "false" });
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = "false" });
            }
        }

        public JsonResult CheckSiteAnalytics()
        {
            try
            {
                SiteAnalytics query = new SiteAnalytics();
                _siteAnalytics = new SiteAnalyticsMgr(mySqlConnectionString);
                DateTime date;
                if (DateTime.TryParse(Request.Params["sa_date"], out date))
                {
                    query.s_sa_date = date.ToString("yyyy-MM-dd");
                    int num = _siteAnalytics.IsExistSiteAnalytics(query);
                    if (num > 0)
                    {
                        return Json(new { success = "true" });
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
            return Json(new { success = "false" });
        }
        #endregion

        #region 獲取有效站台(store)
        public HttpResponseBase GetSiteStore()
        {
            string json = "{success:false,data:[]}";
            try
            {
                _siteMgr = new SiteMgr(mySqlConnectionString);
                List<Site> stores = _siteMgr.Query(new Site { });
                json = "{success:true,data:" + JsonConvert.SerializeObject(stores) + "}";//返回json數據
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
    }
}
