using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Admin.gigade.Controllers
{
    public class EmsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IEmsImplMgr _IEmsMgr;

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmsGoal()
        {
            return View();
        }
        public ActionResult EmsActual()
        {
            return View();
        }

        //目標業績列表頁
        public HttpResponseBase GetEmsGoalList()
        {
            List<EmsGoalQuery> store = new List<EmsGoalQuery>();
            EmsGoalQuery query = new EmsGoalQuery();
            int totalCount = 0;
            string json = string.Empty;
            _IEmsMgr = new EmsMgr(mySqlConnectionString);
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["departgoal"]))
                {
                    query.department_code = Request.Params["departgoal"].ToString();
                }
                //if (!string.IsNullOrEmpty(Request.Params["searchDategoal"]))
                //{
                //    query.searchdate = Convert.ToInt32(Request.Params["searchDategoal"].ToString());
                //}
                if (!string.IsNullOrEmpty(Request.Params["dategoal"]))
                {
                    query.date = Convert.ToDateTime(Request.Params["dategoal"].ToString());
                }
                store = _IEmsMgr.GetEmsGoalList(query, out totalCount);
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

        //下拉框
        public HttpResponseBase GetDepartmentStore()
       {
           List<EmsGoalQuery> store = new List<EmsGoalQuery>();
           string json = string.Empty;
           _IEmsMgr = new EmsMgr(mySqlConnectionString);
           try
           {
               store = _IEmsMgr.GetDepartmentStore();
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

        //保存目標業績
       public HttpResponseBase SaveEmsGoal()
       {
           string json = string.Empty;
           EmsGoalQuery query = new EmsGoalQuery();
           try
           {
               if (!string.IsNullOrEmpty(Request.Params["department_code"]))
               {
                   query.department_code = Request.Params["department_code"].ToString();
               }
               if (!string.IsNullOrEmpty(Request.Params["year"]))
               {
                    query.year = Convert.ToInt32(Request.Params["year"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["month"]))
               {
                   query.month = Convert.ToInt32(Request.Params["month"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["goal_amount"]))
               {
                   query.goal_amount = Convert.ToInt32(Request.Params["goal_amount"]);
               }
               query.create_time = DateTime.Now;
               query.user_userid = (Session["caller"] as Caller).user_id;
                string time = query.year + "-" + query.month;
                //DateTime now = DateTime.Now;
                //DateTime pre = Convert.ToDateTime(time);
                _IEmsMgr = new EmsMgr(mySqlConnectionString);
                 json = _IEmsMgr.SaveEmsGoal(query);
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

        //保存實際業績
       public HttpResponseBase SaveEmsActual()
       {
           string json = string.Empty;
           EmsActualQuery query = new EmsActualQuery();
           try
           {
               if (!string.IsNullOrEmpty(Request.Params["department_code"]))
               {
                   query.department_code = Request.Params["department_code"].ToString();
                 
               }
               if (!string.IsNullOrEmpty(Request.Params["year"]))
               {
                   query.year = Convert.ToInt32(Request.Params["year"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["month"]))
               {
                   query.month = Convert.ToInt32(Request.Params["month"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["day"]))
               {
                   query.day = Convert.ToInt32(Request.Params["day"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["cost_sum"]))
               {
                   query.cost_sum = Convert.ToInt32(Request.Params["cost_sum"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["order_count"]))
               {
                   query.order_count = Convert.ToInt32(Request.Params["order_count"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["amount_sum"]))
               {
                   query.amount_sum = Convert.ToInt32(Request.Params["amount_sum"]);
               }
               query.create_time = DateTime.Now;
               query.user_userid = (Session["caller"] as Caller).user_id;
               _IEmsMgr = new EmsMgr(mySqlConnectionString);
               json = _IEmsMgr.SaveEmsActual(query);
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

        //編輯目標業績
       public HttpResponseBase EditEmsGoal()
       {
           string json = string.Empty;
           try
           {
                EmsGoalQuery EgQuery = new EmsGoalQuery();
                _IEmsMgr = new EmsMgr(mySqlConnectionString);
                if (!string.IsNullOrEmpty(Request.Params["EmsValue"]))
                {
                    EgQuery.goal_amount = Convert.ToInt32(Request.Params["EmsValue"]);
                }
                EgQuery.row_id = Convert.ToInt32(Request.Params["id"]);
                json= _IEmsMgr.EditEmsGoal(EgQuery);
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

        //編輯實際業績
       public HttpResponseBase EditEmsActual()
       {
           string json = string.Empty;
           EmsActualQuery query = new EmsActualQuery();
           try
           {
               if (!string.IsNullOrEmpty(Request.Params["id"]))
               {
                   query.row_id = Convert.ToInt32(Request.Params["id"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["EmsActual"]))
               {
                   query.EmsActual = (Request.Params["EmsActual"]);
               }
               if (!string.IsNullOrEmpty(Request.Params["EmsValue"]))
               {
                   query.EmsValue = Convert.ToInt32(Request.Params["EmsValue"]);
               }
                   _IEmsMgr = new EmsMgr(mySqlConnectionString);
                   json = _IEmsMgr.EditEmsActual(query);
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

        //實際業績列表頁
       public HttpResponseBase GetEmsActualList()
       {
           List<EmsActualQuery> store = new List<EmsActualQuery>();
           EmsActualQuery query = new EmsActualQuery();
           int totalCount = 0;
           string json = string.Empty;
           _IEmsMgr = new EmsMgr(mySqlConnectionString);
           query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
           query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
           try
           {
               if (!string.IsNullOrEmpty(Request.Params["departactual"]))
               {
                   query.department_code = Request.Params["departactual"].ToString();
               }
               if (!string.IsNullOrEmpty(Request.Params["dateactual"]))
               {
                   query.date = Convert.ToDateTime(Request.Params["dateactual"].ToString());
               }
               if (!string.IsNullOrEmpty(Request.Params["datatype"]))
               {
                   query.type = Convert.ToInt32(Request.Params["datatype"].ToString());
               }
               else
               {
                   query.type = 0;
               }
               //列表頁數據查詢出來之前判斷前一天數據是否存在，不存在則插入默認值
               query.predate = DateTime.Now.AddDays(-1);
               query.user_userid = (Session["caller"] as Caller).user_id;
               store = _IEmsMgr.GetEmsActualList(query, out totalCount);
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


    }
}
