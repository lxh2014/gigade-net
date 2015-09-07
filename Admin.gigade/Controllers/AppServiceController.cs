using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Model.Query;
using System.Text;

namespace Admin.gigade.Controllers
{
    public class AppServiceController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        private ISiteConfigImplMgr siteConfigMgr;
        IAppcategoryImplMgr _iappcategoryMgr;
        IAppmessageImplMgr _iappmessageMgr;
        IAppNotifyPoolImplMgr _iappnotifypoolMgr;
        IAppversionsImplMgr _iappversionsMgr;
        //
        // GET: /AppService/
        /// <summary>
        /// 分類表管理
        /// </summary>
        /// <returns></returns>
        public ActionResult AppCategroyIndex()
        {
            return View();
        }
        /// <summary>
        /// 訊息公告
        /// </summary>
        /// <returns></returns>
        public ActionResult AppMessageIndex()
        {
            return View();
        }
        /// <summary>
        /// 上架版本
        /// </summary>
        /// <returns></returns>
        public ActionResult AppversionsIndex()
        {
            return View();
        }
        /// <summary>
        /// 推播設定
        /// </summary>
        /// <returns></returns>
        public ActionResult AppNotifyPoolIndex()
        {
            return View();
        }
        #region 推播設定 (肖國棟2015.08.21)
        /// <summary>
        /// 通過條件得到推播設定列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetAppNotifyPoolInfo()
        {
            string json = string.Empty;
            AppNotifyPoolQuery ap = new AppNotifyPoolQuery();
            try
            {
                _iappnotifypoolMgr = new AppNotifyPoolMgr(mySqlConnectionString);
                ap.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                ap.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                string starttime = Request.Params["timestart"].ToString();
                string startemdtime = Request.Params["timestartend"].ToString();
                string endtime = Request.Params["timeendstart"].ToString();
                string endendtime = Request.Params["timeendend"].ToString();
                if (!string.IsNullOrEmpty(starttime))
                {
                    ap.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(starttime));
                }
                if (!string.IsNullOrEmpty(endtime))
                {
                    ap.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(endtime));
                }
                if (!string.IsNullOrEmpty(startemdtime))
                {
                    ap.startendtime = Convert.ToInt32(CommonFunction.GetPHPTime(startemdtime));
                }
                if (!string.IsNullOrEmpty(endendtime))
                {
                    ap.endendtime = Convert.ToInt32(CommonFunction.GetPHPTime(endendtime));
                }
                //調用查詢事件
                json = _iappnotifypoolMgr.GetAppnotifypool(ap);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "";
            }
            return BackAjaxData(json);
        }
        /// <summary>
        /// 通過頁面編輯播設定列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase EditAppNotifyPoolInfo()
        {
            string json = string.Empty;
            try
            {
                _iappnotifypoolMgr = new AppNotifyPoolMgr(mySqlConnectionString);
                //獲得頁面SaveReport方法提交的參數
                AppNotifyPoolQuery anpq = new AppNotifyPoolQuery();
                anpq.alert = Request.Params["txtafalert"].ToString();
                anpq.isAddOrEidt = Request.Params["isAddOrEidt"].ToString();
                anpq.notified = Convert.ToInt32(Request.Params["now_state"].ToString());
                anpq.title = Request.Params["txttitle"].ToString();
                anpq.to = Request.Params["txtto"].ToString();
                anpq.url = Request.Params["txturl"].ToString();
                string starttime = Request.Params["datevalid_start"].ToString();
                string endtime = Request.Params["datevalid_end"].ToString();
                if (!string.IsNullOrEmpty(starttime))
                {
                    anpq.valid_start = Convert.ToInt32(CommonFunction.GetPHPTime(starttime));
                }
                if (!string.IsNullOrEmpty(endtime))
                {
                    anpq.valid_end = Convert.ToInt32(CommonFunction.GetPHPTime(endtime));
                }
                //調用編輯事件
                json = _iappnotifypoolMgr.EditAppNotifyPoolInfo(anpq);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "";
            }
            return BackAjaxData(json);
        }
        //回傳Ajax參數
        public HttpResponseBase BackAjaxData(string resultStr)
        {
            this.Response.Clear();
            this.Response.Write(resultStr);
            this.Response.End();
            return this.Response;
        }
        #endregion

        #region 上架版本 (肖國棟2015.08.25)
        /// <summary>
        /// 通過條件得到上架版本列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetAppversionsInfo()
        {
            string json = string.Empty;
            AppversionsQuery asq = new AppversionsQuery();
            try
            {
                _iappversionsMgr = new AppversionsMgr(mySqlConnectionString);
                asq.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                asq.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                string cmbdriver = Request.Params["cmbdriver"].ToString();
                if (!string.IsNullOrEmpty(cmbdriver))
                {
                    asq.drive = Convert.ToInt32(cmbdriver);
                }
                //調用查詢事件
                json = _iappversionsMgr.GetAppversionsList(asq);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "";
            }
            return BackAjaxData(json);
        }
        /// <summary>
        /// 上架版本通過ID刪除事件
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AppversionsDelete()
        {
            string jsonStr = String.Empty;
            _iappversionsMgr = new AppversionsMgr(mySqlConnectionString);
            string rowid = Request.Params["rowid"].ToString();
            if (!String.IsNullOrEmpty(rowid))
            {
                try
                {
                    //處理ID
                    rowid = rowid.TrimEnd(',');
                    //調用刪除方法
                    jsonStr = _iappversionsMgr.DeleteAppversionsById(rowid);
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    jsonStr = "{success:false}";
                }
            }
            return BackAjaxData(jsonStr);
        }
        /// <summary>
        /// 通過頁面編輯上架版本
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase EditAppversionsInfo()
        {
            string json = string.Empty;
            try
            {
                _iappversionsMgr = new AppversionsMgr(mySqlConnectionString);
                //獲得頁面SaveReport方法提交的參數
                AppversionsQuery asq = new AppversionsQuery();
                asq.drive = Convert.ToInt32(Request.Params["cmbdriverEdit"].ToString());
                asq.isAddOrEidt = Request.Params["isAddOrEidt"].ToString();
                asq.versions_code = Convert.ToInt32(Request.Params["txtversions_code"].ToString());
                asq.versions_desc = Request.Params["txtversions_desc"].ToString();
                asq.versions_id = Convert.ToInt32(Request.Params["txtversions_id"].ToString());
                asq.versions_name = Request.Params["txtversions_name"].ToString();
                string releasedateQuerytime = Request.Params["daterelease_date"].ToString();
                if (!string.IsNullOrEmpty(releasedateQuerytime))
                {
                    asq.release_date = Convert.ToInt32(CommonFunction.GetPHPTime(releasedateQuerytime));
                }
                //調用編輯事件
                json = _iappversionsMgr.EditAppversionsInfo(asq);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false}";
            }
            return BackAjaxData(json);
        }
        #endregion

        #region 分類表管理 (白明威2015.08.27)
        /// <summary>
        /// 查詢下拉框的參數數據
        /// </summary>
        /// <returns></returns>
        public string QueryPara()
        {
            Appcategory appQuery = new Appcategory();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    _iappcategoryMgr = new AppcategoryMgr(mySqlConnectionString);
                    string para = Request.Params["paraType"].ToString();
                    appQuery.category = Request.Params["selectCondition"];
                    appQuery.category1 = Request.Params["select1Condition"];
                    appQuery.category2 = Request.Params["select2Condition"];
                    json = _iappcategoryMgr.GetParaList(para, appQuery);
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
        /// <summary>
        /// 獲取表單的數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetAppCategoryList()
        {
            List<Appcategory> stores = new List<Appcategory>();

            string json = string.Empty;
            try
            {
                Appcategory query = new Appcategory();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Form["category"]))//判斷館別是否為空
                {
                    query.category = Request.Form["category"];
                }
                if (!string.IsNullOrEmpty(Request.Form["category1"]))//判斷分類一是否為空
                {
                    query.category1 = Request.Form["category1"];
                }
                if (!string.IsNullOrEmpty(Request.Form["category2"]))
                {
                    query.category2 = Request.Form["category2"];
                }
                if (!string.IsNullOrEmpty(Request.Form["category3"]))
                {
                    query.category3 = Request.Form["category3"];
                }
                if (!string.IsNullOrEmpty(Request.Form["product_id"]))//判斷商品ID是否為空
                {
                    query.product_id = int.Parse(Request.Form["product_id"]);
                }
                _iappcategoryMgr = new AppcategoryMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _iappcategoryMgr.GetAppcategoryList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";
                //返回json數據
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
        /// 刪除數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AppCategoryDelete()
        {
            string jsonStr = String.Empty;
            _iappcategoryMgr = new AppcategoryMgr(mySqlConnectionString);
            Appcategory query = new Appcategory();
            if (!String.IsNullOrEmpty(Request.Params["rowid"]))
            {
                try
                {
                    foreach (string rid in Request.Params["rowid"].ToString().Split('|'))
                    {
                        if (!string.IsNullOrEmpty(rid))
                        {
                            query.category_id = Convert.ToInt32(rid);
                            _iappcategoryMgr.AppcategoryDelete(query);
                        }
                    }
                    jsonStr = "{success:true}";
                }
                catch (Exception ex)
                {
                    Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                    logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                    logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                    log.Error(logMessage);
                    jsonStr = "{success:false}";
                }
            }
            this.Response.Clear();
            this.Response.Write(jsonStr);
            this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 導入Excel數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AppCategoryUpExcel()
        {
            string json = string.Empty;//json字符串
            int successcount = 0;
            int failcount = 0;
            int typeerror = 0;//要匯入的數據類型錯誤
            try
            {
                if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)//判斷文件是否為空
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];//獲取文件流
                    FileManagement fileManagement = new FileManagement();//實例化 FileManagement
                    StringBuilder str = new StringBuilder();
                    string fileLastName = excelFile.FileName;
                    string newExcelName = Server.MapPath(excelPath) + "App功能管理" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名
                    excelFile.SaveAs(newExcelName);//上傳文件
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    dt = helper.SheetData();//根據這個表,插入到數據庫中
                    DataRow[] dr = dt.Select(); //定义一个DataRow数组,读取ds里面所有行
                    _iappcategoryMgr = new AppcategoryMgr(mySqlConnectionString);
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            Appcategory appCategory = new Appcategory();
                            try
                            {
                                appCategory.brand_id = Convert.ToInt32(dt.Rows[i]["brand_id"]);
                                appCategory.brand_name = dt.Rows[i]["brand_name"].ToString();
                                appCategory.category = dt.Rows[i]["category"].ToString();
                                appCategory.category1 = dt.Rows[i]["category1"].ToString();
                                appCategory.category2 = dt.Rows[i]["category2"].ToString();
                                appCategory.category3 = dt.Rows[i]["category3"].ToString();
                                appCategory.product_id = Convert.ToInt32(dt.Rows[i]["product_id"]);
                                appCategory.property = dt.Rows[i]["property"].ToString();
                            }
                            catch (Exception ex)
                            {
                                str.Append(i + 2 + " ");
                                failcount++;
                                continue;
                            }
                            int results=  _iappcategoryMgr.AppcategorySave(appCategory);
                            if (results > 0)
                            {
                                successcount++;
                            }
                            else
                            {
                                str.Append(i + 2 + " ");
                                failcount++;
                            }
                        }
                        if (str.Length > 1)
                        {
                            str.Length -= 1;
                            json = "{success:true,total:" + successcount + ",fail:" + failcount + ",errorRow:\"" + str.ToString() + "\"}";
                        }
                        else
                        {
                            json = "{success:true,total:" + successcount + ",fail:" + failcount + "}";
                        }
                    }
                    else
                    {
                        json = "{success:false,msg:\"" + "此表內沒有數據或數據有誤,請檢查后再次匯入!" + "\"}";
                    }
                  
                }
                else
                {
                    json = "{success:false,msg:\"" + "請選擇要匯入的Excel表" + "\"}";
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

        /// <summary>
        /// 下載Excel模板
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase CategoryTemplate()
        {
            
            siteConfigMgr = new SiteConfigMgr(Server.MapPath(xmlPath));
            SiteConfig config = siteConfigMgr.GetConfigByName("Template_Appcategory_Path");
            if (config != null)
            {
                string template = Server.MapPath(string.IsNullOrEmpty(config.Value) ? config.DefaultValue : config.Value);
                if (System.IO.File.Exists(template))
                {
                    this.Response.Clear();
                    this.Response.ContentType = "application/ms-excel";
                    this.Response.AppendHeader("Content-Disposition", "attachment;filename=t_category.xls");
                    this.Response.WriteFile(template);
                    this.Response.End();
                }
            }
            return this.Response;
        }
        #endregion

        #region 訊息公告 (白明威2015.08.27)
        /// <summary>
        /// 獲取表單數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetAppMessageList()
        {
            List<AppmessageQuery> stores = new List<AppmessageQuery>();

            string json = string.Empty;
            try
            {
                AppmessageQuery query = new AppmessageQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Form["msg_start_first"]))//判斷分類一是否為空
                {
                    query.msg_start_first = uint.Parse(CommonFunction.GetPHPTime(Request.Form["msg_start_first"]).ToString());
                }
                if (!string.IsNullOrEmpty(Request.Form["msg_start_second"]))//判斷分類一是否為空
                {
                    query.msg_start_second = uint.Parse(CommonFunction.GetPHPTime(Request.Form["msg_start_second"]).ToString()) + 86399;
                    //時間戳，86399表示時間是23時59分59秒，用於比較時間的大小進行查詢
                }
                if (!string.IsNullOrEmpty(Request.Form["msg_end_first"]))//判斷分類一是否為空
                {
                    query.msg_end_first = uint.Parse(CommonFunction.GetPHPTime(Request.Form["msg_end_first"]).ToString());
                }
                if (!string.IsNullOrEmpty(Request.Form["msg_end_second"]))//判斷分類一是否為空
                {
                    query.msg_end_second = uint.Parse(CommonFunction.GetPHPTime(Request.Form["msg_end_second"]).ToString()) + 86399;
                }
                _iappmessageMgr = new AppmessageMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _iappmessageMgr.GetAppmessageList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";
                //返回json數據
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
        /// 獲取下拉框參數數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetAppMessagePara()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.QueryString["paraType"]))
                {
                    _iappmessageMgr = new AppmessageMgr(mySqlConnectionString);
                    string para = Request.Params["paraType"].ToString();
                    json = _iappmessageMgr.GetParaList(para);
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
        /// <summary>
        /// 新增Message數據
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase AppMessageInsert()
        {
            Appmessage query = new Appmessage();
            string json = string.Empty;
            try
            {
                _iappmessageMgr = new AppmessageMgr(mySqlConnectionString);
                query.title = Request.Form["title"] ?? "";
                query.content = Request.Form["content"] ?? "";
                query.linkurl = Request.Form["linkurl"] ?? "";
                if (!string.IsNullOrEmpty(Request.Form["msg_start"]))
                {
                    query.msg_start = uint.Parse(CommonFunction.GetPHPTime(Request.Form["msg_start"]).ToString());
                }
                if (!string.IsNullOrEmpty(Request.Form["msg_end"]))
                {
                    query.msg_end = uint.Parse(CommonFunction.GetPHPTime(Request.Form["msg_end"]).ToString());
                }
                query.appellation = Request.Form["appellation"] ?? "";
                query.fit_os = Request.Form["fit_os"] ?? "";
                if (!string.IsNullOrEmpty(Request.Form["display_type"]))
                {
                    query.display_type = int.Parse(Request.Form["display_type"]);
                }
                else
                {
                    query.display_type = 1;
                }
                query.type = 1;
                query.messagedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                int result = _iappmessageMgr.AppMessageInsert(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,result:" + result + "}";
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


    }
}
