using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class BoilerRelationController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        static string excelPath = ConfigurationManager.AppSettings["ImportUserIOExcel"];//關於導入的excel文件的限制
        private IBoilerrelationImplMgr _boillationMgr;
        //
        // GET: /BoilerRelation/
        /// <summary>
        /// 匯入鍋的數據到BoilerRelation表中
        /// </summary>
        /// <returns></returns>
        public ActionResult BoilerMessageInsert()
        {
            return View();
        }

        public ActionResult BoilerRelationList()
        {
            return View();
        }
        #region 獲取excel表中的數據,並且以DataSet格式顯示出來 廢除
        //public DataSet ExecleDs(string filenameurl, string table)
        //{
        //    try //如果存在問題 比如說隨便輸入一個execl檔
        //    {
        //        string strConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + filenameurl + ";Extended Properties='Excel 8.0; HDR=YES; IMEX=1'";
        //        OleDbConnection conn = new OleDbConnection(strConn);
        //        conn.Open();
        //        DataSet ds = new DataSet();
        //        string tablename = "象印";
        //        //這裡表可能是漢字.解決方法("select * from ["+ tablename+"$]")
        //        OleDbDataAdapter odda = new OleDbDataAdapter("select * from [" + tablename + "$]", conn);
        //        //OleDbDataAdapter odda = new OleDbDataAdapter("select * from [Sheet1$]", conn);
        //        odda.Fill(ds, table);
        //        return ds;
        //    }
        //    catch(Exception ex)
        //    {
        //        DataSet dsSet = new DataSet();
        //        DataTable dtTable = new DataTable();
        //        dtTable = dsSet.Tables.Add();
        //        return dsSet;
        //    }     
        //}
        #endregion

        public HttpResponseBase InsertBoilerMessage()
        {
            int j = 0;
            string json = string.Empty;//json字符串
            int total=0;
            try
            {
                if (Request.Files["ImportFileMsg"] != null && Request.Files["ImportFileMsg"].ContentLength > 0)//判斷文件是否為空
                {
                    HttpPostedFileBase excelFile = Request.Files["ImportFileMsg"];//獲取文件流
                    FileManagement fileManagement = new FileManagement();//實例化 FileManagement
                    //string fileLastName = excelFile.FileName.Substring((excelFile.FileName).LastIndexOf('.')).ToLower().Trim();
                    string fileLastName = excelFile.FileName;
                    string newExcelName = Server.MapPath(excelPath) + "BoilerRelation" + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名
                    excelFile.SaveAs(newExcelName);//上傳文件
                    DataTable dt = new DataTable();
                    NPOI4ExcelHelper helper = new NPOI4ExcelHelper(newExcelName);
                    dt = helper.SheetData();
                    //DataSet ds = ExecleDs(newExcelName, fileLastName);//调用自定义方法
                    ////excelHelper = new NPOI4ExcelHelper(newExcelName);
                    DataRow[] dr = dt.Select(); //定义一个DataRow数组,读取ds里面所有行
                    int rowsnum = dt.Rows.Count;
                    if(rowsnum !=0)//判斷是否是這個表
                    {
                     if (dr[0][2].ToString().Trim() != "外鍋型號 (依款式&字母順序排列)" && dr[0][3].ToString().Trim() != "內鍋型號" && dr[0][4].ToString().Trim() != "對應安康內鍋型號" && dr[0][5].ToString().Trim() != "備註")
                    {
                        rowsnum = 0;
                    }
                    }
                   
                    if (rowsnum == 0)
                    {
                        json = "{success:true,total:0,msg:\"" + "此表內沒有數據或數據有誤,請檢查后再次匯入!" + "\"}";
                        this.Response.Clear();
                        this.Response.Write(json);
                        this.Response.End();
                        return this.Response;
                    }
                    else
                    {
                        _boillationMgr = new BoilerrelationMgr(mySqlConnectionString);
                        j = _boillationMgr.GetintoBoilerrelation(dr,out total);
                        if (j > 0)
                        {
                            json = "{success:true,total:\"" + total + "\",msg:\"" + "匯入安康內鍋型號對照表成功!" + "\"}";
                        }
                        else
                        {
                            json = "{success:true,total:\"" + total + "\",msg:\"" + "匯入數據標準不對,請嚴格按照模板匯入!" + "\"}";
                        }
                    
                    }
                }
                else//當直接點擊時會產生,
                {
                    json = "{success:true,msg:\"" + "請匯入安康內鍋型號對照表" + "\"}";
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


        public HttpResponseBase GetBoilerRelationList()
        {
            List<boilerrelationQuery> store = new List<boilerrelationQuery>();
            string json = string.Empty;
            try
            {
                boilerrelationQuery query = new boilerrelationQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");

                if (!string.IsNullOrEmpty(Request.Params["limit"]))
                {
                    query.Limit = Convert.ToInt32(Request.Params["limit"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["boiler_type_describe"]))
                {
                    query.Boiler_type_describe = Request.Params["boiler_type_describe"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["out_boiler_type"]))
                {
                    query.out_boiler_number = Request.Params["out_boiler_type"].Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["innner_boiler_type"]))
                {
                    query.inner_boiler_number = Request.Params["innner_boiler_type"].Trim();
                }
                _boillationMgr = new BoilerrelationMgr(mySqlConnectionString);
                int totalCount = 0;
                store = _boillationMgr.QueryBoilerRelationAll(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
               
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + ",}";//返回json數據
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
    }
}
