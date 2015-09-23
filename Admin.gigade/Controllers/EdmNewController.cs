using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Data;
using BLL.gigade.Model;

namespace Admin.gigade.Controllers
{
    public class EdmNewController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        EmailBlockListMgr _emailBlockListMgr;
        //
        // GET: /EdmNew/
        #region view
        public ActionResult Index()
        {
            return View();
        }
        //電子報
        public ActionResult EdmContentNew()
        {
            return View();
        }
        //擋信名單
        public ActionResult EmailBlockList()
        {
            return View();
        }
        #endregion

        #region 電子報類型

        #region 電子報類型列表頁

        #endregion

        #region 電子報類型新增編輯

        #endregion

        #endregion

        #region 電子報範本

        #region  電子報範本列表頁

        #endregion

        #region  電子報範本新增編輯

        #endregion


        #endregion

        #region 電子報

        #region 電子報列表

        #endregion

        #region 電子報新增編輯

        #endregion

        #endregion

        #region 擋信名單管理
        public HttpResponseBase GetEmailBlockList()
        {
            string json = string.Empty;//

            try
            {
                _emailBlockListMgr = new EmailBlockListMgr(mySqlConnectionString);
                DataTable store = new DataTable();
                EmailBlockListQuery query = new EmailBlockListQuery();
                if (!string.IsNullOrEmpty(Request.Params["email"]))
                {
                    query.email_address = Request.Params["email"];
                }
                store = _emailBlockListMgr.GetEmailBlockList(query);
                if (store != null)
                {
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd";
                    json = "{success:true" + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase AddorEdit() 
        {
            string json = string.Empty; 
            _emailBlockListMgr = new EmailBlockListMgr(mySqlConnectionString);
            try
            {
                EmailBlockListQuery query = new EmailBlockListQuery();
                query.block_update_userid = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                if (!string.IsNullOrEmpty(Request.Params["reason"]))
                {
                    query.block_reason = Request.Params["reason"].ToString().Replace("\\", "\\\\"); ;
                    query.block_create_userid = query.block_update_userid;
                    if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                    {
                        query.email_address = Request.Params["email_address"].ToString().Replace("\\", "\\\\"); ;
                    }
                    int i = _emailBlockListMgr.Add(query);
                    if (i > 0)
                    {
                        json = "{success:true}";
                    }
                    else if (i == -1)
                    {
                        json = "{success:false,msg:\'0\'}"; //郵箱已添加過
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Params["block_reason"]))
                    {
                        query.block_reason = Request.Params["block_reason"].ToString().Replace("\\", "\\\\"); ;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                    {
                        query.email_address = Request.Params["email_address"].ToString();
                    }
                    int i = _emailBlockListMgr.Update(query);
                    if (i > 0)
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
                json = "{success:false,data:[]}"; 
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public HttpResponseBase UnBlock()
        {
            string json = string.Empty;
            bool result = false;
            try
            {
                _emailBlockListMgr = new EmailBlockListMgr(mySqlConnectionString);
                EmailBlockLogQuery query = new EmailBlockLogQuery();
                if (!string.IsNullOrEmpty(Request.Params["email_address"]))
                {
                    query.email_address = Request.Params["email_address"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["unblock_reason"]))
                {
                    query.unblock_reason = Request.Params["unblock_reason"].ToString().Replace("\\", "\\\\"); ;
                }
                query.unblock_create_userid = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                result = _emailBlockListMgr.UnBlock(query);
                if (result)
                {
                    json = "{success:true}";
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
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion

    }
}
