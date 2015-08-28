using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Text;
using Admin.gigade.CustomError;

namespace Admin.gigade.Controllers
{
     [HandleError]
    public class FunctionVGroupController : Controller
    {
        //
        // GET: /FunctionVGroup/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static string connectionString = ConfigurationManager.AppSettings["MySqlConnectionString"];
        private IFunctionVGroupImplMgr functionVGroupMgr;
        private IFunctionImplMgr functionMgr;

        #region 權限模組

        [CustomHandleError]
        [HttpPost]
        public JsonResult GetAuthorityGroup()
        {
            try
            {
                string callId = (Session["caller"] as Caller).user_email;
                AuthorityQuery query = new AuthorityQuery { Type = 1, CallId = callId };
                functionVGroupMgr = new FunctionVGroupMgr(connectionString);
                List<Function> functions = functionVGroupMgr.CallerAuthorityQuery(query);
                var result = from f in functions
                             group f by f.FunctionGroup into fgroup
                             select new { Id = fgroup.Min(m => m.RowId), Text = fgroup.Key };
                return Json(result);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json("[]");
        }
        #endregion

        #region 權限功能

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase GetAuthorityFun(string id)
        {
            string json = string.Empty;
            try
            {
                string callId = (Session["caller"] as Caller).user_email;
                AuthorityQuery query = new AuthorityQuery { Type = 1, CallId = callId };
                if (!string.IsNullOrEmpty(id))
                {
                    query.RowId = Convert.ToInt32(id);
                }
                functionVGroupMgr = new FunctionVGroupMgr(connectionString);
                List<Function> functions = functionVGroupMgr.CallerAuthorityQuery(query);
                StringBuilder strJson = new StringBuilder("[");
                foreach (Function fun in functions)
                {
                    strJson.Append("{");
                    strJson.AppendFormat("text:\"{0}\",url:\"{1}\",id:\"{2}\",iconCls:\"{3}\",leaf:true", fun.FunctionName, fun.FunctionCode, fun.RowId, fun.IconCls);
                    strJson.Append("}");
                }
                strJson.Append("]");
                json = strJson.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                json = "[]";
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

        #region 權限按鈕

        [CustomHandleError]
        [HttpPost]
        public JsonResult GetAuthorityTool()
        {
            try
            {
                string callId = (Session["caller"] as Caller).user_email;
                AuthorityQuery query = new AuthorityQuery { Type = 2, CallId = callId };
                if (!string.IsNullOrEmpty(Request.Form["RowId"]))
                {
                    query.RowId = Convert.ToInt32(Request.Form["RowId"]);
                    functionVGroupMgr = new FunctionVGroupMgr(connectionString);
                    List<Function> functions = functionVGroupMgr.CallerAuthorityQuery(query);
                    var result = from f in functions
                                 select new { id = f.FunctionCode };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return Json("");
        }

        [CustomHandleError]
        [HttpPost]
        public JsonResult GetAuthorityToolByUrl()
        {
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["Url"]))
                {
                    functionMgr = new FunctionMgr(connectionString);
                    Function fun = functionMgr.Query(new Function { FunctionCode = Request.Form["Url"] }).FirstOrDefault();
                    if (fun != null)
                    {
                        string callId = (Session["caller"] as Caller).user_email;
                        AuthorityQuery query = new AuthorityQuery { Type = 2, CallId = callId };

                        query.RowId = fun.RowId;
                        functionVGroupMgr = new FunctionVGroupMgr(connectionString);
                        List<Function> functions = functionVGroupMgr.CallerAuthorityQuery(query);
                        var result = from f in functions
                                     select new { id = f.FunctionCode };
                        return Json(result);
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
            return Json("");
        }
        #endregion

        #region 組別權限

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase GetFgroupAuthority()
        {
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Form["RowId"]))
                {
                    //所有模組、按鈕
                    functionMgr = new FunctionMgr(connectionString);
                    List<Function> all = functionMgr.Query(new Function());

                    //該組別擁有權限的模組、按鈕
                    functionVGroupMgr = new FunctionVGroupMgr(connectionString);
                    AuthorityQuery query = new AuthorityQuery { GroupId = Convert.ToInt32(Request.Form["RowId"]) };
                    List<Function> functions = functionVGroupMgr.GroupAuthorityQuery(query);
                    //所有模組
                    var groups = from f in all
                                 where f.FunctionType == 1
                                 group f by f.FunctionGroup into fgroup
                                 select new { Fgroup = fgroup.Key };
                    //所有頁面
                    var pages = from f in all
                                where f.FunctionType != 2
                                select f;
                    //所有按鈕
                    var tools = from f in all
                                where f.FunctionType == 2
                                select f;
                    StringBuilder strJson = new StringBuilder("[");
                    foreach (var g in groups)
                    {
                        strJson.Append("{FunctionGroup:\"" + g.Fgroup + "\",items:[");
                        foreach (var p in pages.Where(m => m.FunctionGroup == g.Fgroup))
                        {
                            bool isAuth = functions.Where(m => m.RowId == p.RowId).Count() > 0;
                            strJson.Append("{RowId:" + p.RowId + ",Name:\"" + p.FunctionName + "\",checked:\"" + isAuth + "\",tools:[");
                            foreach (var t in tools.Where(m => m.TopValue == p.RowId))
                            {
                                isAuth = functions.Where(m => m.RowId == t.RowId).Count() > 0;
                                strJson.Append("{RowId:" + t.RowId + ",Name:\"" + t.FunctionName + "\",checked:\"" + isAuth + "\"}");
                            }
                            strJson.Append("]}");
                        }
                        strJson.Append("]}");
                    }
                    strJson.Append("]");
                    json = strJson.ToString().Replace("}{", "},{");
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

        #region 授權操作

        [CustomHandleError]
        [HttpPost]
        public HttpResponseBase SaveAuthority()
        {
            string json = string.Empty;
            try
            {
                FunctionGroup functionGroup = new FunctionGroup();
                functionGroup.Kuser = (Session["caller"] as Caller).user_email;
                if (!string.IsNullOrEmpty(Request.Form["GroupId"]))
                {
                    functionGroup.GroupId = Convert.ToInt32(Request.Form["GroupId"]);
                }
                if (!string.IsNullOrEmpty(Request.Form["rowID"]))
                {
                    string[] rowId = Request.Form["rowID"].Split('|');

                    functionVGroupMgr = new FunctionVGroupMgr(connectionString);
                    functionVGroupMgr.Delete(functionGroup.GroupId);
                    {
                        foreach (var str in rowId)
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                functionGroup.FunctionId = Convert.ToInt32(str);
                                functionVGroupMgr.Save(functionGroup);
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

    }
}
