using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using System.Text.RegularExpressions;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Common;
using System.IO;
using System.Text;
using gigadeExcel.Comment;
using System.Data;
using System.Configuration;
using BLL.gigade.Model.Custom;
using System.Net.Mail;
using System.Net;
namespace Admin.gigade.Controllers
{
    public class ProductCommentController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];//郵件服務器的設置
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        string FromName = ConfigurationManager.AppSettings["FromName"];//發件人姓名
        string EmailTile = ConfigurationManager.AppSettings["CommentEmailTile"];//郵件標題
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private IProductCommentImplMgr _proCommentImpl;
        //
        // GET: /ProductComment/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ChangeLog()  //異動記錄
        {
            return View();
        }
        #region 用戶評論列表頁+HttpResponseBase GetProductCommentList()
        /// <summary>
        /// 用戶評論列表頁
        /// </summary>
        /// <returns></returns>
        [CustomHandleError]
        public HttpResponseBase GetProductCommentList()
        {
            DataTable store = new DataTable();
            string json = string.Empty;
            string str = string.Empty;
            try
            {
                _proCommentImpl = new ProductCommentMgr(mySqlConnectionString);
                ProductCommentQuery query = new ProductCommentQuery();
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]))
                {
                    query.comment_type = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["commentsel"]))
                {
                    query.commentsel = Request.Params["commentsel"];
                }
                if (!string.IsNullOrEmpty(Request.Params["productName"]))
                {
                    query.product_name = Request.Params["productName"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["userName"]))
                {
                    query.user_name = Request.Params["userName"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["userEmail"]))
                {
                    query.user_email = Request.Params["userEmail"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_name"]))
                {
                    query.brand_name = Request.Params["brand_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.beginTime = Convert.ToInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["timestart"].ToString()).ToString("yyyy-MM-dd 00:00:00")));
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.endTime = Convert.ToInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["timeend"].ToString()).ToString("yyyy-MM-dd 23:59:59")));
                }
                if (!string.IsNullOrEmpty(Request.Params["productId"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["productId"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["shopClass"]))
                {
                    query.productIds = Request.Params["shopClass"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["isReplay"]))
                {
                    query.isReplay = Convert.ToInt32(Request.Params["isReplay"].ToString());
                }

                int totalCount = 0;
                store = _proCommentImpl.Query(query, out totalCount);
                for (int i = 0; i < store.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(store.Rows[i]["user_name"].ToString()))
                    {
                        store.Rows[i]["user_name"] = store.Rows[i]["user_name"].ToString().Substring(0, 1) + "**";
                    }
                    store.Rows[i]["user_email"] = store.Rows[i]["user_email"].ToString().Split('@')[0] + "@***";
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
                json = "{success:true,totalCount:0,data:[]}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 匯出評價列表csv表格
        /// </summary>
        public void IndexExport()
        {
            string json = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                DataTable store = new DataTable();
                #region 前提查詢條件

                ProductCommentQuery query = new ProductCommentQuery();
                query.Start = Convert.ToInt32(Request["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ddlSel"]) && Request.Params["ddlSel"] != "null")
                {
                    query.comment_type = Convert.ToInt32(Request.Params["ddlSel"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["commentsel"]) && Request.Params["commentsel"] != "null")
                {
                    query.commentsel = Request.Params["commentsel"];
                }
                if (!string.IsNullOrEmpty(Request.Params["productName"]))
                {
                    query.product_name = Request.Params["productName"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["userName"]))
                {
                    query.user_name = Request.Params["userName"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["userEmail"]))
                {
                    query.user_email = Request.Params["userEmail"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["isReplay"]))
                {
                    query.isReplay = Convert.ToInt32(Request.Params["isReplay"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["brand_name"]))
                {
                    query.brand_name = Request.Params["brand_name"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]) && Request.Params["timestart"] != "null")
                {
                    query.beginTime = Convert.ToInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["timestart"].ToString()).ToString("yyyy-MM-dd 00:00:00")));
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]) && Request.Params["timeend"] != "null")
                {
                    query.endTime = Convert.ToInt32(CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["timeend"].ToString()).ToString("yyyy-MM-dd 23:59:59")));
                }
                if (!string.IsNullOrEmpty(Request.Params["productId"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["productId"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["shopClass"]) && Request.Params["shopClass"] != "null")
                {
                    query.productIds = Request.Params["shopClass"].ToString();
                }
                query.IsPage = false;
                #endregion
                _proCommentImpl = new ProductCommentMgr(mySqlConnectionString);
                string fileName = "用戶評價信息" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                int totalCount = 0;
                store = _proCommentImpl.Query(query, out totalCount);
                DataTable dtHZ = new DataTable();
                string newExcelName = string.Empty;
                dtHZ.Columns.Add("編號", typeof(String));
                dtHZ.Columns.Add("訂單編號", typeof(String));
                dtHZ.Columns.Add("商品編號", typeof(String));
                dtHZ.Columns.Add("商品名稱", typeof(String));
                dtHZ.Columns.Add("品牌名稱", typeof(String));
                dtHZ.Columns.Add("用戶姓名", typeof(String));
                //dtHZ.Columns.Add("用戶郵箱", typeof(String));
                dtHZ.Columns.Add("留言狀態", typeof(String));
                dtHZ.Columns.Add("留言內容", typeof(String));//--處理
                dtHZ.Columns.Add("留言回覆", typeof(String));
                dtHZ.Columns.Add("回覆是否顯示", typeof(String));
                dtHZ.Columns.Add("商品描述相符度", typeof(String));
                dtHZ.Columns.Add("客戶服務滿意度", typeof(String));
                dtHZ.Columns.Add("網站整體服務滿意度", typeof(String));
                dtHZ.Columns.Add("配送速度滿意度", typeof(String));
                dtHZ.Columns.Add("回覆人", typeof(String));
                dtHZ.Columns.Add("回覆時間", typeof(String));
                dtHZ.Columns.Add("狀態", typeof(String));


                for (int i = 0; i < store.Rows.Count; i++)
                {
                    DataRow dr = dtHZ.NewRow(); 
                    dr[0] = store.Rows[i]["comment_id"];
                    string comment_info = store.Rows[i]["product_name"].ToString();
                    comment_info = comment_info.Replace(',', '，');
                    comment_info = comment_info.Replace("\n", "");
                    dr[1] = store.Rows[i]["order_id"].ToString() == "" ? "0" : store.Rows[i]["order_id"];
                    dr[2] = store.Rows[i]["product_id"];
                    dr[3] = comment_info;
                    comment_info = store.Rows[i]["brand_name"].ToString();
                    comment_info = comment_info.Replace(',', '，');
                    comment_info = comment_info.Replace("\n", "");
                    dr[4] = comment_info;
                    dr[5] = store.Rows[i]["user_name"];
                    //dr[4] = store[i].user_email;
                    comment_info = store.Rows[i]["is_show_name"].ToString();
                    if (comment_info != "")
                    {
                        dr[6] = Convert.ToInt32(comment_info) == 0 ? "匿名" : "公開";//0-匿名，1-公開
                    }
                    else {
                        dr[6] = "";
                    }
                    comment_info = store.Rows[i]["comment_info"].ToString();
                    comment_info = comment_info.Replace(',', '，');
                    comment_info = comment_info.Replace("\n", "");
                    dr[7] = comment_info;
                    //dr[6] = store[i].comment_answer;
                    dr[8] = store.Rows[i]["comment_answer"].ToString().Replace(',', '，').Replace("\r", "").Replace("\n", "");
                    //   dtHZ.Columns.Add("留言回覆", typeof(String));
                    comment_info = store.Rows[i]["answer_is_show"].ToString();
                    if (comment_info != "")
                    {
                        dr[9] = Convert.ToInt32(comment_info) == 0 ? "隱藏" : "顯示";
                    }
                    else {
                        dr[9] = "";
                    }
                    dr[10] = Satisfaction(Convert.ToInt32(store.Rows[i]["product_desc"].ToString()));//滿意度
                    dr[11] = Satisfaction(Convert.ToInt32(store.Rows[i]["seller_server"].ToString()));
                    dr[12] = Satisfaction(Convert.ToInt32(store.Rows[i]["web_server"].ToString()));
                    dr[13] = Satisfaction(Convert.ToInt32(store.Rows[i]["logistics_deliver"].ToString()));
                    dr[14] = store.Rows[i]["s_reply_user"];
                    if (!string.IsNullOrEmpty(store.Rows[i]["reply_time"].ToString()))
                    {
                        if (CommonFunction.DateTimeToString(Convert.ToDateTime(store.Rows[i]["reply_time"].ToString())) == "1970-01-01 08:00:00")
                        {
                            dr[15] = "";
                        }                    
                        else
                        {
                            dr[15] = CommonFunction.DateTimeToString(Convert.ToDateTime(store.Rows[i]["reply_time"].ToString()));                           
                        }
                    }
                    else {
                        dr[15] = "";
                    }
                    comment_info = store.Rows[i]["status"].ToString();
                    if (comment_info != "")
                    {
                        dr[16] = Convert.ToInt32(comment_info) == 1 ? "開啟" : "關閉";//1,開啟。2.關閉
                    }
                    else {
                        dr[16] = "";
                    }
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

        public string Satisfaction(int type)
        {
            string Evaluation = "";//評價

            switch (type)
            {
                case 1:
                    Evaluation = "非常不滿意";
                    break;
                case 2:
                    Evaluation = "不滿意";
                    break;
                case 3:
                    Evaluation = "一般";
                    break;
                case 4:
                    Evaluation = "滿意";
                    break;
                case 5:
                    Evaluation = "非常滿意";
                    break;
                default:
                    Evaluation = type.ToString();
                    break;
            }
            return Evaluation;
        }

        #endregion

        #region 更改活動使用狀態 + HttpResponseBase UpdateActive()
        /// <summary>
        /// 更改活動使用狀態
        /// </summary>
        /// <returns>數據庫操作結果</returns>
        public JsonResult UpdateActive()
        {
            string jsonStr = string.Empty;
            try
            {
                ProductCommentQuery model = new ProductCommentQuery();
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    model.status = Convert.ToInt32(Request.Params["active"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    model.comment_id = Convert.ToInt32(Request.Params["id"]);
                }
                _proCommentImpl = new ProductCommentMgr(mySqlConnectionString);
                if (_proCommentImpl.UpdateActive(model) > 0)
                {
                    return Json(new { success = "true" });
                }
                else
                {
                    return Json(new { success = "false" });
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
        #endregion

        #region 異動記錄查詢
        #region 獲取表名
        [HttpPost]
        [CustomHandleError]
        public HttpResponseBase GetTableName()
        {
            string json = string.Empty;
            try
            {
                _proCommentImpl = new ProductCommentMgr(mySqlConnectionString);
                DataTable _dt = _proCommentImpl.QueryTableName();
                json = "{success:true,data:" + JsonConvert.SerializeObject(_dt, Formatting.Indented) + "}";//返回json數據
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
        public HttpResponseBase GetChangeLogList()
        {
            string json = "{success:false,data:0}";
            try
            {
                _proCommentImpl = new ProductCommentMgr(mySqlConnectionString);
                ProductCommentQuery query = new ProductCommentQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量

                query.change_table = Request.Params["table_name"].ToString();
                if (!string.IsNullOrEmpty(Request.Params["comment_id"]))
                {
                    query.comment_id = Convert.ToInt32(Request.Params["comment_id"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Request.Params["start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time = Convert.ToDateTime(Request.Params["end_time"]);
                }

                int totalCount = 0;
                DataTable store = _proCommentImpl.GetChangeLogList(query, out totalCount);
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

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase GetChangeLogDetailList()
        {
            string json = "{success:false,data:0}";
            try
            {
                _proCommentImpl = new ProductCommentMgr(mySqlConnectionString);
                int pk_id = 0;
                string create_time = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["pk_id"]))
                {
                    pk_id = Convert.ToInt32(Request.Params["pk_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["create_time"]))
                {
                    create_time = CommonFunction.DateTimeToString(Convert.ToDateTime(Request.Params["create_time"]));
                }
                BLL.gigade.Model.Custom.TableChangeLogCustom store = _proCommentImpl.GetChangeLogDetailList(pk_id, create_time);
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

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase ProductCommentLogExport()
        {
            string newCsvName = string.Empty;
            string json = string.Empty;

            _proCommentImpl = new ProductCommentMgr(mySqlConnectionString);
            ProductCommentQuery query = new ProductCommentQuery();

            query.change_table = Request.Params["table_name"].ToString();
            if (!string.IsNullOrEmpty(Request.Params["comment_id"]))
            {
                query.comment_id = Convert.ToInt32(Request.Params["comment_id"].ToString());
            }
            if (!string.IsNullOrEmpty(Request.Params["start_time"]))
            {
                query.start_time = Convert.ToDateTime(Request.Params["start_time"]);
            }
            if (!string.IsNullOrEmpty(Request.Params["end_time"]))
            {
                query.end_time = Convert.ToDateTime(Request.Params["end_time"]);
            }
            try
            {
                DataTable _dt = _proCommentImpl.ProductCommentLogExport(query);

                string filename = "ProductCommentLog_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                if (!System.IO.Directory.Exists(Server.MapPath(excelPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(excelPath));
                }
                newCsvName = Server.MapPath(excelPath) + filename;

                if (System.IO.File.Exists(newCsvName))
                {
                    //設置文件的屬性，以防刪除文件的時候因為文件的屬性造成無法刪除
                    System.IO.File.SetAttributes(newCsvName, FileAttributes.Normal);
                    System.IO.File.Delete(newCsvName);
                }
                string[] colname = { "評價編號", "評價內容", "修改人", "修改時間", "變動欄位", "欄位中文名", "修改前值", "修改后值" };
                CsvHelper.ExportDataTableToCsv(_dt, newCsvName, colname, true);
                json = "true," + filename + "," + excelPath;

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "false, ";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

        public HttpResponseBase ProductCommentSave()
        {
            string json = string.Empty;
            int send_mail = 1;
            int isSave = 0;//保存未成功
            ProductCommentQuery query = new ProductCommentQuery();
            try
            {
                string path = Server.MapPath(xmlPath);
                SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                SiteConfig Mail_From = _siteConfigMgr.GetConfigByName("Mail_From");
                SiteConfig Mail_Host = _siteConfigMgr.GetConfigByName("Mail_Host");
                SiteConfig Mail_Port = _siteConfigMgr.GetConfigByName("Mail_Port");
                SiteConfig Mail_UserName = _siteConfigMgr.GetConfigByName("Mail_UserName");
                SiteConfig Mail_UserPasswd = _siteConfigMgr.GetConfigByName("Mail_UserPasswd");
                string EmailFrom = Mail_From.Value;//發件人郵箱
                string SmtpHost = Mail_Host.Value;//smtp服务器
                string SmtpPort = Mail_Port.Value;//smtp服务器端口
                string EmailUserName = Mail_UserName.Value;//郵箱登陸名
                string EmailPassWord = Mail_UserPasswd.Value;//郵箱登陸密碼

                ProductCommentQuery store = new ProductCommentQuery();
                _proCommentImpl = new ProductCommentMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["comment_id"]))
                {
                    query.comment_id = Convert.ToInt32(Request.Params["comment_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["comment_answer"]))
                {
                    query.comment_answer = Request.Params["comment_answer"];
                    query.comment_answer = query.comment_answer.Replace("\\", "\\\\");
                }
                if (!string.IsNullOrEmpty(Request.Params["comment_answer"]))
                {
                    query.old_comment_answer = Request.Params["old_comment_answer"];
                    query.old_comment_answer = query.old_comment_answer.Replace("\\", "\\\\");
                }
                if (!string.IsNullOrEmpty(Request.Params["answer_is_show"]))
                {
                    query.answer_is_show = Convert.ToInt32(Request.Params["answer_is_show"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["old_answer_is_show"]))
                {
                    query.old_answer_is_show = Request.Params["old_answer_is_show"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["isReplay"]))
                {
                    if (Convert.ToInt32(Request.Params["isReplay"]) == 1)
                    {
                        query.old_answer_is_show = string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(Request.Params["comment_detail_id"]))
                {
                    query.comment_detail_id = Convert.ToInt32(Request.Params["comment_detail_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["send_mail"]))
                {
                    send_mail = Convert.ToInt32(Request.Params["send_mail"]);//0 發送  1 不發送
                    query.send_mail = Convert.ToInt32(Request.Params["send_mail"]);//0 發送  1 不發送 2 發送失敗
                }
                query.reply_user = (Session["caller"] as Caller).user_id;
                store = _proCommentImpl.GetUsetInfo(query);
                string user_email = store.user_email;
                string user_name = store.user_name;
                string product_name = store.product_name;
                string comment_info = store.comment_info;
                if (_proCommentImpl.ProductCommentSave(query) > 0)
                {

                    json = "{success:'true',msg:'0'}";//保存成功，但不發送郵件
                    isSave = 1;//指示保存成功
                    if (send_mail == 0)
                    {
                        FileStream fs = new FileStream(Server.MapPath("../ImportUserIOExcel/CommentReplay.html"), FileMode.OpenOrCreate, FileAccess.Read);
                        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                        string strTemp = sr.ReadToEnd();
                        sr.Close();
                        fs.Close();
                        //zhejiang0304j@gigade.com.tw
                        strTemp = strTemp.Replace("{{$username$}}", user_name);
                        strTemp = strTemp.Replace("{{$productName$}}", product_name);
                        strTemp = strTemp.Replace("{{$commentAnwser$}}", Request.Params["comment_answer"]);
                        strTemp = strTemp.Replace("{{$commentInfo$}}", comment_info);
                        if (sendmail(EmailFrom, FromName, user_email, user_name, EmailTile, strTemp, "", SmtpHost, Convert.ToInt32(SmtpPort), EmailUserName, EmailPassWord))
                        {
                            json = "{success:'true',msg:'1'}";//保存成功,郵件發送成功！ 
                        }
                        else
                        {
                            json = "{success:'true',msg:'2'}";//保存成功,郵件發送失敗！
                            query.send_mail = 2;
                            _proCommentImpl.ProductCommentSave(query);
                        }
                    }
                }
                else
                {
                    json = "{success:'false'}";//保存失敗
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                if (isSave == 1)//保存成功,可能郵件發送失敗！
                {
                    json = "{success:'true',msg:'2'}";//保存成功,郵件發送失敗！
                    query.send_mail = 2;
                    _proCommentImpl.ProductCommentSave(query);

                }
                else//保存失敗
                {
                    json = "{success:'false'}";
                }

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #region C#发送邮件函数
        /// <summary>
        /// C#发送邮件函数
        /// </summary>
        /// <param name="from">发送者邮箱</param>
        /// <param name="fromer">发送人</param>
        /// <param name="sto">接受者邮箱</param>
        /// <param name="toer">收件人</param>
        /// <param name="Subject">主题</param>
        /// <param name="Body">内容</param>
        /// <param name="file">附件地址</param>
        /// <param name="SMTPHost">smtp服务器</param>
        /// <param name="SMTPuser">邮箱</param>
        /// <param name="SMTPpass">密码</param>
        /// <returns></returns>
        public bool sendmail(string sfrom, string sfromer, string sto, string stoer, string sSubject, string sBody, string sfile, string sSMTPHost, int sSMTPPort, string sSMTPuser, string sSMTPpass)
        {
            ////设置from和to地址
            MailAddress from = new MailAddress(sfrom, sfromer);
            MailAddress to = new MailAddress(sto, stoer);

            ////创建一个MailMessage对象
            MailMessage oMail = new MailMessage(from, to);
            //// 添加附件
            if (sfile != "")
            {
                oMail.Attachments.Add(new Attachment(sfile));
            }
            ////邮件标题
            oMail.Subject = sSubject;
            ////邮件内容
            //oMail.Body = sBody;
            AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(sBody, null, "text/html");
            oMail.AlternateViews.Add(htmlBody);
            ////邮件格式
            oMail.IsBodyHtml = true;
            ////邮件采用的编码
            oMail.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");
            ////设置邮件的优先级为高
            oMail.Priority = MailPriority.Normal;
            ////发送邮件
            SmtpClient client = new SmtpClient();
            ////client.UseDefaultCredentials = false;
            client.Host = sSMTPHost;
            client.Port = sSMTPPort;
            client.Credentials = new NetworkCredential(sSMTPuser, sSMTPpass);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(oMail);
                return true;
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return false;
            }
            finally
            {
                ////释放资源
                oMail.Dispose();
            }

        }
        #endregion

    }
}
