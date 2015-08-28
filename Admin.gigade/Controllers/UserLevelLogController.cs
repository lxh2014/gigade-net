using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class UserLevelLogController : Controller
    {
        //
        // GET: /UserLevelLog/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private UserLevelLogMgr _userLevelLog;
        private MemberLevelMgr _memberMgr;
        public ActionResult Index()
        {
            return View();
        }

        public HttpResponseBase GetUserLevelLogList()
        {
            string json = string.Empty;
            int totalCount = 0;
            UserLevelLogQuery query = new UserLevelLogQuery();
            List<UserLevelLogQuery> store = new List<UserLevelLogQuery>();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["searchStatus"]))
                {
                    query.searchStatus = Convert.ToInt32(Request.Params["searchStatus"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["key"]))
                {
                    query.key = Request.Params["key"];
                }
                 if (!string.IsNullOrEmpty(Request.Params["leveltype"]))
                {
                    query.leveltype = Convert.ToInt32(Request.Params["leveltype"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["levelid"]))
                {
                    query.leveltypeid = Request.Params["levelid"];
                }
                _userLevelLog = new UserLevelLogMgr(mySqlConnectionString);
                store = _userLevelLog.GetUserLevelLogList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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

        public HttpResponseBase GetVipLevel()
        {
            DataTable _dt = new DataTable();
            string json = string.Empty;
            try
            {
                _memberMgr = new MemberLevelMgr(mySqlConnectionString);
                _dt = _memberMgr.GetLevel();
                VipUserGroup Dmodel = new VipUserGroup();
                json = "{success:true,data:" + JsonConvert.SerializeObject(_dt) + "}";//返回json數據

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
    }
}
