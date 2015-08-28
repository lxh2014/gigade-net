using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class EmsDepRelationController : Controller
    {
        //
        // GET: /EmsDepRelation/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private EmsDepRelationMgr emsDepRelation;
        public ActionResult Index()
        {
            return View();
        }

        #region 列表頁 EmsDepRelationList()
        public HttpResponseBase EmsDepRelationList()
        {
            List<EmsDepRelation> store = new List<EmsDepRelation>();
            EmsDepRelation query = new EmsDepRelation();
            int totalCount = 0;
            string json = string.Empty;
            emsDepRelation = new EmsDepRelationMgr(mySqlConnectionString);
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["dep_code"]))
                {
                    query.relation_dep = Convert.ToInt32(Request.Params["dep_code"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_type"]))
                {
                    query.re_type = Convert.ToInt32(Request.Params["relation_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["datatype"]))
                {
                    query.datatype = Convert.ToInt32(Request.Params["datatype"]);
                }
               if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.date = Convert.ToDateTime(Request.Params["date"]);
                }
                query.create_user = (Session["caller"] as Caller).user_id;
                query.update_user = (Session["caller"] as Caller).user_id;
                query.predate = DateTime.Now.AddDays(-1);
                store=emsDepRelation.EmsDepRelationList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
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

        #region 下拉列表 GetDepStore()
        public HttpResponseBase GetDepStore()
        {
            string json = string.Empty;
            List<EmsDepRelation> store = new List<EmsDepRelation>();
            EmsDepRelation query = new EmsDepRelation();
            query.dep_name = "請選擇...";
            try
            {
                emsDepRelation = new EmsDepRelationMgr(mySqlConnectionString);
                store= emsDepRelation.GetDepStore();
                store.Insert(0, query);
                json = "{success:true,data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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

        #region 列表頁點擊編輯 EditEmsDepR()
        public HttpResponseBase EditEmsDepR()
        {
            string json = string.Empty;
            EmsDepRelation query = new EmsDepRelation();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["relation_id"]))
                {
                    query.relation_id = Convert.ToInt32(Request.Params["relation_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["emsDep"]))
                {
                    query.emsdep = (Request.Params["emsDep"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["value"]))
                {
                    query.value = Convert.ToInt32(Request.Params["value"]);
                }
                query.update_user = (Session["caller"] as Caller).user_id;
                emsDepRelation=new EmsDepRelationMgr (mySqlConnectionString);
                json = emsDepRelation.EditEmsDepR(query);
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

        #region 新增
        public HttpResponseBase SaveEmsDep()
        {
            EmsDepRelation query = new EmsDepRelation();
            string json = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["relation_dep"]))
                {
                    query.relation_dep = Convert.ToInt32(Request.Params["relation_dep"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_type"]))
                {
                    query.relation_type = Convert.ToInt32(Request.Params["relation_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_year"]))
                {
                    query.relation_year = Convert.ToInt32(Request.Params["relation_year"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_month"]))
                {
                    query.relation_month = Convert.ToInt32(Request.Params["relation_month"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_day"]))
                {
                    query.relation_day = Convert.ToInt32(Request.Params["relation_day"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_order_cost"]))
                {
                    query.relation_order_cost = Convert.ToInt32(Request.Params["relation_order_cost"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["relation_order_count"]))
                {
                    query.relation_order_count = Convert.ToInt32(Request.Params["relation_order_count"]);
                }
                query.create_user = (Session["caller"] as Caller).user_id;
                query.update_user = (Session["caller"] as Caller).user_id;
                emsDepRelation = new EmsDepRelationMgr(mySqlConnectionString);
                json=   emsDepRelation.SaveEmsDepRe(query);
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:true,msg:2}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
    }
}
