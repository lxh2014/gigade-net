/*
* 文件名稱 :SystemKeyWordController.cs
* 文件功能描述 :系統關鍵字列表控制器
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-31
* 修改備註 :無
*/
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class SystemKeyWordController : Controller
    {
        //
        // GET: /SystemKeyWord/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string SqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private static DataTable DTExcel = new DataTable();
        private static DataTable DTIupcExcel = new DataTable();
        private static DataTable DTIplasExcel = new DataTable();
        private static DataTable DTIplasEnterExcel = new DataTable();
        private static DataTable DTIlocExcel = new DataTable();
        SphinxExcludeMgr sphinxExcludeMgr;
        SystemKeyWordMgr swMgr = null;
        #region View
        public ActionResult Index()
        {
            return View();
        }
        #region 系統關鍵字列表 KeyWordIndex()
        /// <summary>
        /// 系統關鍵字列表
        /// </summary>
        /// <returns></returns>
        public ActionResult KeyWordIndex()
        {
            return View();
        }
        #endregion
        #region SphinxExclude
        public ActionResult SphinxExclude()
        {
            return View();
        }
        #endregion
        #endregion
        #region 獲取系統關鍵字 + GetSystemKeyWord()
        public HttpResponseBase GetSystemKeyWord()
        {
            int totalCount = 0;
            string json = string.Empty;
            List<SphinxKeywordQuery> stores = new List<SphinxKeywordQuery>();
            SphinxKeywordQuery query = new SphinxKeywordQuery();
            swMgr = new SystemKeyWordMgr(SqlConnectionString);
            DateTime time;
            try
            {
                query.Start = Convert.ToInt32(Request.Params["Start"] ?? "0");
                if (!string.IsNullOrEmpty(Request.Params["Limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["Limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["searchKey"]))
                {
                    query.searchKey = Request.Params["searchKey"];
                }
                if (DateTime.TryParse(Request.Params["startTime"], out time))
                {
                    query.startTime = time;
                }
                if (DateTime.TryParse(Request.Params["endTime"], out time))
                {
                    query.endTime = time;
                }
                stores = swMgr.GetSystemKeyWord(query, out totalCount);
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;

        }
        #endregion
        #region 新增/編輯系統關鍵字列表  + SaveSystemKeyWord()
        public HttpResponseBase SaveSystemKeyWord()
        {
            string json = string.Empty;
            ///返回的状态
            int state = 0;
            List<SphinxKeywordQuery> stores = new List<SphinxKeywordQuery>();
            SphinxKeywordQuery query = new SphinxKeywordQuery();
            swMgr = new SystemKeyWordMgr(SqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    query.row_id = Convert.ToInt32(Request.Params["row_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["operateType"]))
                {
                    query.operateType = Convert.ToInt32(Request.Params["operateType"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["flag"]))
                {
                    query.flag = Request.Params["flag"];
                }
                if (!string.IsNullOrEmpty(Request.Params["key_word"]))
                {
                    query.key_word = Request.Params["key_word"];
                }
                query.user_name = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_username;
                state = swMgr.SaveSystemKeyWord(query);
                //IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                ////这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                //timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,state:" + state + "}";//返回json數據
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
        #region 刪除系統關鍵字  + DelSystemKeyWord()
        public HttpResponseBase DelSystemKeyWord()
        {
            string json = string.Empty;
            ///返回的状态
            int state = 0;
            List<SphinxKeywordQuery> stores = new List<SphinxKeywordQuery>();
            SphinxKeywordQuery query = new SphinxKeywordQuery();
            swMgr = new SystemKeyWordMgr(SqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["row_id"]))
                {
                    string[] strId = Request.Params["row_id"].Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    query.ArrId = strId;
                }
                query.user_name = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_username;
                state = swMgr.DelSystemKeyWord(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,state:" + state + "}";//返回json數據
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
        #region 檢查關鍵字是否存在
        public HttpResponseBase CheckKeyWordExsit()
        {
            string json = string.Empty;
            SphinxKeywordQuery query = new SphinxKeywordQuery();
            swMgr = new SystemKeyWordMgr(SqlConnectionString);
            int msg = 0;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["key_word"]))
                {
                    query.key_word = Request.Params["key_word"].Trim();
                }
                msg = swMgr.CheckKeyWordExsit(query.key_word);
                json = "{success:true,msg:" + msg + "}";//返回json數據
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
        #region 系統關鍵字匯出
        public void KeyWordExcelList()
        {
            string json = string.Empty;
            SphinxKeywordQuery skQuery = new SphinxKeywordQuery();
            DataTable dtExcel = new DataTable();
            List<SphinxKeywordQuery> store = new List<SphinxKeywordQuery>();
            try
            {
                DateTime time;
                if (!string.IsNullOrEmpty(Request.Params["searchKey"].Trim()))
                {
                    skQuery.searchKey = Request.Params["searchKey"].ToString();
                }
                if (DateTime.TryParse(Request.Params["startTime"].ToString(), out time))
                {
                    skQuery.startTime = time;
                }
                if (DateTime.TryParse(Request.Params["endtime"].ToString(), out time))
                {
                    skQuery.endTime = time;
                }

                swMgr = new SystemKeyWordMgr(SqlConnectionString);
                store = swMgr.GetKeyWordExportList(skQuery);
                dtExcel.Columns.Add("keyword", typeof(String));
                dtExcel.Columns.Add("foodkeyword(1:是食安關鍵字,0不是食安關鍵字)", typeof(String));
                for (int i = 0; i < store.Count; i++)
                {
                    DataRow newRow = dtExcel.NewRow();
                    newRow[0] = store[i].key_word.ToString();
                    newRow[1] = store[i].flag.ToString();
                    dtExcel.Rows.Add(newRow);
                }
                if (dtExcel.Rows.Count > 0)
                {
                    string fileName = "系統關鍵字_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
        #endregion
        #region 系統關鍵字-匯入功能
        public HttpResponseBase KeyWordUploadExcel()
        {
            string newName = string.Empty;
            string json = string.Empty;
            List<SphinxKeywordQuery> store = new List<SphinxKeywordQuery>();
            HashEncrypt hashpt = new HashEncrypt();
            try
            {
                DTIlocExcel.Clear();
                DTIlocExcel.Columns.Clear();
                DTIlocExcel.Columns.Add("keyword", typeof(String));
                DTIlocExcel.Columns.Add("foodkeyword(1:是食安關鍵字,0不是食安關鍵字)", typeof(String));
                DTIlocExcel.Columns.Add("failureMessage", typeof(String));
                int result = 0;
                int count = 0;//總匯入數
                //int entercount = 0;//插入失敗個數
                int errorcount = 0;//數據異常個數
                int repeat = 0;//數據已存在個數
                string create_user = (Session["caller"] as Caller).user_username;
                if (Request.Files["ImportExcelFile"] != null && Request.Files["ImportExcelFile"].ContentLength > 0)
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportExcelFile"];
                    newName = Server.MapPath(excelPath) + excelFile.FileName;
                    excelFile.SaveAs(newName);
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newName);
                    dt = helper.SheetData();
                    if (dt.Rows.Count > 0)
                    {
                        swMgr = new SystemKeyWordMgr(SqlConnectionString);
                        int i = 0;
                        foreach (DataRow dr in dt.Rows)
                        {
                            StringBuilder strsql = new StringBuilder();
                            SphinxKeywordQuery query = new BLL.gigade.Model.Query.SphinxKeywordQuery();
                            i++;
                            try
                            {
                                if (!string.IsNullOrEmpty(dr[0].ToString()) && !string.IsNullOrEmpty(dr[1].ToString()))
                                {
                                    int row_id_exsit = swMgr.CheckKeyWordExsit(dr[0].ToString());//判斷關鍵字是否存在
                                    query.user_name = create_user;
                                    query.key_word = dr[0].ToString();
                                    query.flag = dr[1].ToString();
                                    query.operateType = 1;
                                    if (row_id_exsit > 0)
                                    {
                                        DataRow drtwo = DTIlocExcel.NewRow();
                                        drtwo[0] = dr[0].ToString();
                                        drtwo[1] = dr[1].ToString();
                                        drtwo[2] = "系統關鍵字已存在";
                                        DTIlocExcel.Rows.Add(drtwo);
                                        repeat++;
                                        continue;
                                    }
                                    else//關鍵字不存在
                                    {
                                        if (query.flag == "0" || query.flag == "1")
                                        {
                                            byte[] strBt = Encoding.Unicode.GetBytes(query.key_word);
                                            //關鍵字最多為25個中文字或50個英文字
                                            if (strBt.Length <= 50)
                                            {
                                                result = swMgr.SaveSystemKeyWord(query);
                                                if (result > 0)
                                                {
                                                    count++;
                                                    continue;
                                                }
                                                else
                                                {
                                                    DataRow drtwo = DTIlocExcel.NewRow();
                                                    drtwo[0] = dr[0].ToString();
                                                    drtwo[1] = dr[1].ToString();
                                                    drtwo[2] = "關鍵字插入數據庫時失敗";
                                                    DTIlocExcel.Rows.Add(drtwo);
                                                    errorcount++;
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                DataRow drtwo = DTIlocExcel.NewRow();
                                                drtwo[0] = dr[0].ToString();
                                                drtwo[1] = dr[1].ToString();
                                                drtwo[2] = "關鍵字最多為25個中文字或50個英文字";
                                                DTIlocExcel.Rows.Add(drtwo);
                                                errorcount++;
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            DataRow drtwo = DTIlocExcel.NewRow();
                                            drtwo[0] = dr[0].ToString();
                                            drtwo[1] = dr[1].ToString();
                                            drtwo[2] = "'foodkeyword的值只能為0或1'";
                                            DTIlocExcel.Rows.Add(drtwo);
                                            errorcount++;
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    DataRow drtwo = DTIlocExcel.NewRow();
                                    drtwo[0] = dr[0].ToString();
                                    drtwo[1] = dr[1].ToString();
                                    drtwo[2] = "keyword或foodkeyword不符合格式";
                                    DTIlocExcel.Rows.Add(drtwo);
                                    errorcount++;
                                    continue;
                                }
                            }
                            catch
                            {
                                DataRow drtwo = DTIlocExcel.NewRow();
                                drtwo[0] = dr[0].ToString();
                                drtwo[1] = dr[1].ToString();
                                drtwo[2] = "數據異常";
                                DTIlocExcel.Rows.Add(drtwo);
                                errorcount++;
                                continue;
                            }
                        }
                        if (count > 0)
                        {
                            json = "{success:true,total:" + count + ",error:" + errorcount + ",repeat:" + repeat+"}";
                        }
                        else
                        {
                            json = "{success:true,total:" + 0 + ",error:" + errorcount + ",repeat:" + repeat+"}";
                        }
                    }
                    else
                    {
                        json = "{success:true,total:" + 0 + ",error:" + 0 + ",repeat" + repeat + "}";
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #region 匯入模板
        public void UpdownTemplate()
        {
            string json = string.Empty;
            DataTable dtTemplateExcel = new DataTable();
            try
            {
                dtTemplateExcel.Columns.Add("keyword", typeof(String));
                dtTemplateExcel.Columns.Add("foodkeyword(1:是食安關鍵字,0不是食安關鍵字)", typeof(String));
                DataRow newRow = dtTemplateExcel.NewRow();
                dtTemplateExcel.Rows.Add(newRow);
                string fileName = "系統關鍵字匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(dtTemplateExcel, "");//"條碼維護匯入模板_" + DateTime.Now.ToString("yyyyMMddHHmmss")
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
        }
        #endregion
        #endregion
        #region 下載匯入失敗的數據
        /// <summary>
        /// 顯示匯入失敗的數據
        /// </summary>
        public void Updownmessage()
        {
            string json = string.Empty;
            try
            {
                if (DTIlocExcel.Rows.Count > 0)
                {
                    string fileName = "系統關鍵字已存在或格式有誤" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    MemoryStream ms = ExcelHelperXhf.ExportDT(DTIlocExcel, "");
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                    Response.BinaryWrite(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,data:" + "" + "}";
            }
        }
       
        #endregion

        #region SphinxExclude
        public HttpResponseBase GetSphinxExclude()
        {
            string json = string.Empty;
            try
            {
                SphinxExcludeQuery query = new SphinxExcludeQuery();
                query.Start = string.IsNullOrEmpty(Request.Params["start"]) ? 0 : int.Parse(Request.Params["start"]);
                query.Limit = string.IsNullOrEmpty(Request.Params["limit"]) ? 25 : int.Parse(Request.Params["limit"]);
                query.product_name = string.IsNullOrEmpty(Request.Params["productname"]) ? "" : Request.Params["productname"].Trim();
                System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^\d+$");
                string oid = Request.Params["oid"].Trim();
                if (oid != "")
                {
                    if (rex.IsMatch(oid))
                    {
                        query.product_id = int.Parse(oid);
                    }
                    else
                    {
                        query.product_id = -1;
                    }
                }
                DateTime datetime;
                if (DateTime.TryParse(Request.Params["time_start"], out datetime))
                {
                    query.created_start = DateTime.Parse(datetime.ToString("yyyy-MM-dd 00:00:00"));
                }
                if (DateTime.TryParse(Request.Params["time_end"], out datetime))
                {
                    query.created_end = DateTime.Parse(datetime.ToString("yyyy-MM-dd 23:59:59"));
                }
                int totalCount = 0;
                sphinxExcludeMgr = new SphinxExcludeMgr(SqlConnectionString);
                List<SphinxExcludeQuery> list = sphinxExcludeMgr.GetList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        public JsonResult SphinxExcludeSave()
        {
            SphinxExcludeQuery query = new SphinxExcludeQuery();
            query.kuser = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
            query.kdate = DateTime.Now;
            string pid = Request.Params["pid"].Trim();
            query.product_id_old = int.Parse(pid);
            if (!string.IsNullOrEmpty(Request.Params["productid"].Trim()))
            {
                query.product_id = int.Parse(Request.Params["productid"].Trim());
            }
            try
            {
                sphinxExcludeMgr = new SphinxExcludeMgr(SqlConnectionString);
                if (pid == "0")
                {
                    int row = sphinxExcludeMgr.InsertModel(query);
                    if (row == 1)
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = row });
                    }
                }
                else
                {
                    int row = sphinxExcludeMgr.UpdateModel(query);
                    if (row == 1)
                    {
                        return Json(new { success = "true" });
                    }
                    else
                    {
                        return Json(new { success = row });
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

        public JsonResult DeleteById()
        {
            SphinxExcludeQuery query = new SphinxExcludeQuery();
            if (!string.IsNullOrEmpty(Request.Params["rid"]))
            {
                query.product_ids = Request.Params["rid"].TrimEnd(',');
            }
            try
            {
                sphinxExcludeMgr = new SphinxExcludeMgr(SqlConnectionString);
                if (sphinxExcludeMgr.DeleteModel(query) > 0)
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

    }
}
