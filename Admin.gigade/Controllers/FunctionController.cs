using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using Admin.gigade.CustomError;
using BLL.gigade.Common;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.IO;
using BLL.gigade.Model.Custom;

namespace Admin.gigade.Controllers
{
    [HandleError]
    public class FunctionController : Controller
    {
        //
        // GET: /Function/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IFunctionImplMgr functionMgr;
        public ActionResult Index()
        {
            return View();
        }

        #region 查詢功能、控件

        [CustomHandleError]
        [HttpPost]
        public JsonResult GetFunction()
        {
            List<Function> functions = new List<Function>();
            try
            {
                int typeId = 1;
                Int32.TryParse(Request.Form["Type"] ?? "-1", out typeId);
                Function function = new Function { FunctionType = typeId };
                if (!string.IsNullOrEmpty(Request.Form["TopValue"]))
                {
                    function.TopValue = Convert.ToInt32(Request.Form["TopValue"]);
                }

                functionMgr = new FunctionMgr(connectionString);
                functions = functionMgr.Query(function);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json(functions);
        }
        #endregion

        #region 查詢模組

        [CustomHandleError]
        [HttpPost]
        public JsonResult GetGroup()
        {
            try
            {
                functionMgr = new FunctionMgr(connectionString);
                List<Function> functions = functionMgr.Query(new Function { FunctionType = 1 });
                var groups = from f in functions
                             group f by f.FunctionGroup into c
                             select new { FunctionGroup = c.Key };
                return Json(groups);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json("[]");
            }
        }
        #endregion

        #region 保存功能、控件

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase SaveFunction()
        {
            Function function = new Function();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["RowId"]))
                {
                    function.RowId = Convert.ToInt32(Request.Form["RowId"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["child_only"]))
                {
                    function.FunctionType = 3;
                }
                else
                {
                    function.FunctionType = Convert.ToInt32(Request.Form["Type"] ?? "1");
                }
                if (!string.IsNullOrEmpty(Request.Form["Group"]))
                {
                    function.FunctionGroup = Request.Form["Group"];
                }
                if (!string.IsNullOrEmpty(Request.Form["Name"]))
                {
                    function.FunctionName = Request.Form["Name"];
                }
                if (!string.IsNullOrEmpty(Request.Form["Code"]))
                {
                    function.FunctionCode = Request.Form["Code"];
                }
                if (!string.IsNullOrEmpty(Request.Form["IconCls"]))
                {
                    function.IconCls = Request.Form["IconCls"];
                }
                if (!string.IsNullOrEmpty(Request.Form["Remark"]))
                {
                    function.Remark = Request.Form["Remark"];
                }
                if (!string.IsNullOrEmpty(Request.Form["TopValue"]))
                {
                    function.TopValue = Convert.ToInt32(Request.Form["TopValue"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["IsEdit"]))
                {

                    function.IsEdit = int.Parse(Request.Form["IsEdit"]);
                }//edit by wwei0216w

                function.Kuser = (Session["caller"] as Caller).user_email;

                functionMgr = new FunctionMgr(connectionString);
                if (function.RowId != 0)
                {
                    if (functionMgr.Update(function) > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                else
                {
                    if (functionMgr.Save(function) > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
            }
            catch (Exception ex)
            {
                json = "{success:false}";
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

        #region 刪除功能、控件

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase DeleteFunction()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string rowIDs = Request.Form["rowID"];
                    if (rowIDs.IndexOf("|") != -1)
                    {
                        functionMgr = new FunctionMgr(connectionString);
                        foreach (string id in rowIDs.Split('|'))
                        {
                            if (!string.IsNullOrEmpty(id))
                            {
                                functionMgr.Delete(Convert.ToInt32(id));
                            }
                        }
                    }
                }
                json = "{success:true}";
            }
            catch (Exception ex)
            {
                json = "{success:false}";
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

        /// add by wwei0216w  2015/04/03
        #region 查詢控件的歷史記錄
        public ActionResult QueryHistory()
        {
            try
            {
                int total = 0;
                int function_id = Convert.ToInt32(Request["function_id"]);
                IFunctionHistoryImplMgr _fhMgr = new FunctionHistoryMgr(connectionString);
                FunctionHistory fh = new FunctionHistory();
                _fhMgr = new FunctionHistoryMgr(connectionString);
                int start = int.Parse(Request.Form["start"] ?? "0");
                int limit = int.Parse(Request.Form["limit"] ?? "0");

                string condition = Request.Form["conditional"];
                DateTime startTime = Request.Form["startTime"] == "" ? DateTime.MinValue : Convert.ToDateTime(Request.Form["startTime"]);
                DateTime startEnd = Request.Form["endTime"] == "" ? DateTime.MaxValue : Convert.ToDateTime(Request.Form["endTime"]);
                var course = _fhMgr.Query(function_id, start, limit, condition, startTime, startEnd, out total);
                IsoDateTimeConverter iso = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
                return Content("{success:true,totalCount:" + total + ",item:" + JsonConvert.SerializeObject(course, Formatting.None, iso) + "}");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return new EmptyResult();
            }
        }
        #endregion

        /// <summary>
        /// 匯出功能
        /// </summary>
        public ActionResult OutToExcel(int rowId = 0, string functionName = "")
        {
            try
            {
                IFunctionImplMgr _funMgr = new FunctionMgr(connectionString);
                Function fun = new Function { RowId = rowId };
                MemoryStream ms = _funMgr.ExcelOut(fun, functionName);
                if (ms == null)
                {
                    return new EmptyResult();
                }
                return File(ms.ToArray(), "application/-excel", "權限人員名單_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls");
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                return Json(new { success = false });
            }
        }

        #region 權限人員查看
        /// <summary>
        /// 權限人員查看   add by zhuoqin0830w  2015/07/06
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPrivilege()
        {
            List<FunctionCustom> functions = new List<FunctionCustom>();
            try
            {
                Function function = new Function();
                if (!string.IsNullOrEmpty(Request.Form["functionId"]))
                {
                    function.RowId = Convert.ToInt32(Request.Form["functionId"]);
                }
                functionMgr = new FunctionMgr(connectionString);
                functions = functionMgr.GetUserById(function);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json(functions);
        }
        #endregion
    }
}
