using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System.Data;
using Newtonsoft.Json.Converters;
using BLL.gigade.Mgr;
using System.IO;
using gigadeExcel.Comment;
using BLL.gigade.Common;
using System.Configuration;
using System.Text;


namespace Admin.gigade.Controllers
{
    public class PayEasyController : Controller
    {
        //
        // GET: /PayEasy/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IPayEasyImplMgr _payeasemgr;
        private PayEasyQuery model;
        private Parametersrc psmodel;
        private IParametersrcImplMgr _parameter = new ParameterMgr(mySqlConnectionString);
        private NPOI4ExcelHelper excelHelper;
        private string excelPath = ConfigurationManager.AppSettings["ImportCompareExcel"];
        //private ISiteConfigImplMgr siteConfigMgr;
        static string xmlPath = ConfigurationManager.AppSettings["SiteConfig"];
        public ActionResult Index()
        {
            return View();
        }
        public void OutExcel()
        {
            List<Parametersrc> store = new List<Parametersrc>();
            model = new PayEasyQuery();
            psmodel = new Parametersrc();
            _payeasemgr = new PayEasyMgr(mySqlConnectionString);
            try
            {
                try
                {
                    model.rid = Convert.ToInt32(Request.Params["product_id"] ?? "0");
                }
                catch (Exception)
                {
                    model.rid = 0;
                }
                psmodel.ParameterType = "payeasy";
                psmodel.ParameterCode = "1";
                store = _parameter.QueryUsed(psmodel);
                foreach (var item in store)
                {
                    model.chnanel_id = Int32.Parse(item.ParameterProperty);
                }
                psmodel.ParameterCode = "2";
                store = _parameter.QueryUsed(psmodel);
                foreach (var item in store)
                {
                    model.category_id = Int32.Parse(item.ParameterProperty);
                }
                //MemoryStream ms = ExcelHelperXhf.ExportDT(_payeasemgr.QueryExcel(model), "匯出");
                //Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.csv", DateTime.Now.ToString("yyyyMMddhhmmss")));
                //Response.BinaryWrite(ms.ToArray());
                //ms.Close();
                //ms.Dispose();

                string fileName = DateTime.Now.ToString("6727_yyyyMMddHHmm") + ".xls";
                MemoryStream ms = ExcelHelperXhf.ExportDT(_payeasemgr.QueryExcel(model), "");
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                Response.BinaryWrite(ms.ToArray());

                //StringWriter sw = ExcelHelperXhf.SetCsvFromData(_payeasemgr.QueryExcel(model), fileName);
                //Response.Clear();
                //Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                //Response.ContentType = "application/ms-excel";
                //Response.ContentEncoding = Encoding.Default;
                //Response.Write(sw);
                //Response.End();
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
        }
        public void OutExcel2()
        {
            try
            {
                FileManagement fileManagement = new FileManagement();//實例化 FileManagement 
                HttpPostedFileBase excelFile = Request.Files[0];//獲取文件流
                string fileLastName = excelFile.FileName.Substring((excelFile.FileName).LastIndexOf('.')).ToLower().Trim();
                if (fileLastName.Equals(".xlsx") || fileLastName.Equals(".xls"))
                {
                    string newExcelName = Server.MapPath(excelPath) + fileManagement.NewFileName(excelFile.FileName);//處理文件名，獲取新的文件名 
                    excelFile.SaveAs(newExcelName);//上傳文件 
                    //string fileName = DateTime.Now.ToString("6727_yyyyMMddHHmm") + ".xls";
                    string fileName = DateTime.Now.ToString("6727_yyyyMMddHHmm") + ".csv";
                    
                    Stream stream = excelFile.InputStream;
                    NPOI4ExcelHelper npoiHelper = new NPOI4ExcelHelper(newExcelName);
                    DataTable dtMain = npoiHelper.SheetData();
                    DataTable new_Table = new DataTable();
                    #region new_Table表頭
                    new_Table.Columns.Add(new DataColumn("廠商流水號"));
                    new_Table.Columns.Add(new DataColumn("網站別流水號"));
                    new_Table.Columns.Add(new DataColumn("廠商康迅商品代碼"));
                    new_Table.Columns.Add(new DataColumn("商品名稱"));
                    new_Table.Columns.Add(new DataColumn("付款期數"));
                    new_Table.Columns.Add(new DataColumn("商品售價"));
                    #endregion
                    for (int i = 0; i < dtMain.Rows.Count; i++)
                    {
                        DataRow dr1 = new_Table.NewRow();
                        dr1["廠商流水號"] = "6727";
                        dr1["網站別流水號"] = "6037";
                        dr1["廠商康迅商品代碼"] = dtMain.Rows[i]["外站商品編號"].ToString();
                        dr1["商品名稱"] = dtMain.Rows[i]["外站商品名稱"].ToString();
                        dr1["付款期數"] = "0";
                        dr1["商品售價"] = dtMain.Rows[i]["外站商品售價"].ToString();
                        new_Table.Rows.Add(dr1);
                    }
                    //MemoryStream ms = ExcelHelperXhf.ExportDT(new_Table, "");
                    //Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                    //Response.BinaryWrite(ms.ToArray());

                    StringWriter sw = ExcelHelperXhf.SetCsvFromData(new_Table, fileName);
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileName));
                    Response.ContentType = "application/ms-excel";
                    Response.ContentEncoding = Encoding.Default;
                    Response.Write(sw);
                    Response.End();
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

