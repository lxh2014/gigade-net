using BLL.gigade.Mgr;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using gigadeExcel.Comment;
using System.Configuration;
using BLL.gigade.Common;

namespace Admin.gigade.Controllers
{
    public class EdmSController : Controller
    {
        //
        // GET: /EdmS/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string sqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        EdmListConditionMainMgr _edmlistmainMgr;
        EdmListConditoinSubMgr _edmlistsubMgr;
        static string excelPath_export = ConfigurationManager.AppSettings["ImportUserIOExcel"];

        public ActionResult Index()
        {
            return View();
        }

        public HttpResponseBase GetConditionList()
        {
            string json = string.Empty;
            List<EdmListConditionMain> store = new List<EdmListConditionMain>();
            EdmListConditionMain item = new EdmListConditionMain();
        
            _edmlistmainMgr = new EdmListConditionMainMgr(sqlConnectionString);
            try
            {
            
                store = _edmlistmainMgr.GetConditionList();
                item.elcm_id = 0;
                item.elcm_name = "無";
                store.Add(item);
                store.Insert(0, item);
              //  store.Insert(0,
                json = "{success:true" + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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
        public HttpResponseBase LoadCondition()
        {
            string json = string.Empty;
            List<EdmListConditoinSubQuery> store = new List<EdmListConditoinSubQuery>();
            EdmListConditoinSubQuery query = new EdmListConditoinSubQuery();
            _edmlistsubMgr = new EdmListConditoinSubMgr(sqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["conditionName"]))
                {
                    query.elcm_name = Request.Params["conditionName"];
                }
                store = _edmlistsubMgr.LoadCondition(query);
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
        public HttpResponseBase DeleteListInfo()
        {
            string json = string.Empty;
            _edmlistmainMgr = new EdmListConditionMainMgr(sqlConnectionString);
            EdmListConditionMain query = new EdmListConditionMain();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["elcm_name"]))
                {
                    query.elcm_name = Request.Params["elcm_name"];
                }
                int i = _edmlistmainMgr.DeleteListInfo(query);
                if (i > 0)
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
        public HttpResponseBase SaveListInfo()
        {
            string json = string.Empty;
            _edmlistmainMgr = new EdmListConditionMainMgr(sqlConnectionString);
            _edmlistsubMgr = new EdmListConditoinSubMgr(sqlConnectionString);
            EdmListConditoinSubQuery query = new EdmListConditoinSubQuery();
            int id = 0;
            int msg = 0;
            try
            {
                SetQueryValue(query);
                query.elcm_creator_id = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                int i = _edmlistmainMgr.SaveListInfoName(query, out id, out msg);
                if (i > 0)
                {
                    query.elcm_id = id;
                    _edmlistsubMgr.SaveListInfoCondition(query);
                    json = "{success:true}";
                }
                else if (msg == 1)
                {
                    json = "{success:false,msg:1}"; //篩選條件名稱已存在
                }
                else
                {
                    json = "{success:false,msg:0}"; //保存篩選條件名稱失敗
                }
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
        public HttpResponseBase GetUserNum()
        {
            string json = string.Empty;
            DataTable store = new DataTable();
            int totalCount = 0;
            _edmlistmainMgr = new EdmListConditionMainMgr(sqlConnectionString);
            EdmListConditoinSubQuery query = new EdmListConditoinSubQuery();
            try
            {
                SetQueryValue(query);
                store = _edmlistmainMgr.GetUserNum(query);
                if (store != null && store.Rows.Count > 0)
                {
                    totalCount = store.Rows.Count;
                }
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented) + "}";
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
        public HttpResponseBase Export()
        {
            string json = string.Empty;
            EdmListConditoinSubQuery query=new EdmListConditoinSubQuery();
            try
            {
                _edmlistmainMgr = new EdmListConditionMainMgr(sqlConnectionString);
                SetQueryValue(query);
                DataTable _dt = _edmlistmainMgr.GetUserNum(query);
                string filename = "EDM名單篩選_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
                string newFileName = Server.MapPath(excelPath_export + filename);
                string[] colName = { "電子信箱" };
                DataTable _newdt = new DataTable();
                DataRow dr;
                _newdt.Columns.Add("user_email", typeof(string));              
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    dr = _newdt.NewRow();
                    _newdt.Rows.Add(dr);
                    _newdt.Rows[i]["user_email"] = _dt.Rows[i]["user_email"];                                     
                }
                CsvHelper.ExportDataTableToCsv(_newdt, newFileName, colName, true);
                json = "{success:true,fileName:\'" + filename + "\'}";
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
        public HttpResponseBase UpdateCondition() 
        {
            string json = string.Empty;
            _edmlistmainMgr = new EdmListConditionMainMgr(sqlConnectionString);
            _edmlistsubMgr = new EdmListConditoinSubMgr(sqlConnectionString);
            EdmListConditoinSubQuery query = new EdmListConditoinSubQuery();        
            try
            {
                SetQueryValue(query);            
                int i = _edmlistmainMgr.UpdateCondition(query);
                if (i > 0)
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
                json = "{success:false}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        public EdmListConditoinSubQuery SetQueryValue(EdmListConditoinSubQuery query) 
        {            
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["elcm_name"]))
                {
                    query.elcm_name = Request.Params["elcm_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["chkGender"]))
                {
                    if (Request.Params["chkGender"] == "true")
                    {
                        query.chkGender = true;
                        if (!string.IsNullOrEmpty(Request.Params["genderCondition"]))
                        {
                            query.genderCondition = Convert.ToInt32(Request.Params["genderCondition"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkBuy"]))
                {
                    if (Request.Params["ChkBuy"] == "true")
                    {
                        query.ChkBuy = true;
                        if (!string.IsNullOrEmpty(Request.Params["buyCondition"]))
                        {
                            query.buyCondition = Convert.ToInt32(Request.Params["buyCondition"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["buyTimes"]))
                        {
                            query.buyTimes = Convert.ToInt32(Request.Params["buyTimes"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["buyTimeMin"]))
                        {
                            query.buyTimeMin = Convert.ToDateTime(Request.Params["buyTimeMin"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["buyTimeMax"]))
                        {
                            query.buyTimeMax = Convert.ToDateTime(Request.Params["buyTimeMax"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkAge"]))
                {
                    if (Request.Params["ChkAge"] == "true")
                    {
                        query.ChkAge = true;
                        if (!string.IsNullOrEmpty(Request.Params["ageMin"]))
                        {
                            query.ageMin = Convert.ToInt32(Request.Params["ageMin"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["ageMax"]))
                        {
                            query.ageMax = Convert.ToInt32(Request.Params["ageMax"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkCancel"]))
                {
                    if (Request.Params["ChkCancel"] == "true")
                    {
                        query.ChkCancel = true;
                        if (!string.IsNullOrEmpty(Request.Params["cancelCondition"]))
                        {
                            query.cancelCondition = Convert.ToInt32(Request.Params["cancelCondition"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["cancelTimes"]))
                        {
                            query.cancelTimes = Convert.ToInt32(Request.Params["cancelTimes"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["cancelTimeMin"]))
                        {
                            query.cancelTimeMin = Convert.ToDateTime(Request.Params["cancelTimeMin"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["cancelTimeMax"]))
                        {
                            query.cancelTimeMax = Convert.ToDateTime(Request.Params["cancelTimeMax"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkRegisterTime"]))
                {
                    if (Request.Params["ChkRegisterTime"] == "true")
                    {
                        query.ChkRegisterTime = true;
                        if (!string.IsNullOrEmpty(Request.Params["registerTimeMin"]))
                        {
                            query.registerTimeMin = Convert.ToDateTime(Request.Params["registerTimeMin"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["registerTimeMax"]))
                        {
                            query.registerTimeMax = Convert.ToDateTime(Request.Params["registerTimeMax"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkReturn"]))
                {
                    if (Request.Params["ChkReturn"] == "true")
                    {
                        query.ChkReturn = true;
                        if (!string.IsNullOrEmpty(Request.Params["returnCondition"]))
                        {
                            query.returnCondition = Convert.ToInt32(Request.Params["returnCondition"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["returnTimes"]))
                        {
                            query.returnTimes = Convert.ToInt32(Request.Params["returnTimes"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["returnTimeMin"]))
                        {
                            query.returnTimeMin = Convert.ToDateTime(Request.Params["returnTimeMin"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["returnTimeMax"]))
                        {
                            query.returnTimeMax = Convert.ToDateTime(Request.Params["returnTimeMax"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkLastOrder"]))
                {
                    if (Request.Params["ChkLastOrder"] == "true")
                    {
                        query.ChkLastOrder = true;
                        if (!string.IsNullOrEmpty(Request.Params["lastOrderMin"]))
                        {
                            query.lastOrderMin = Convert.ToDateTime(Request.Params["lastOrderMin"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["lastOrderMax"]))
                        {
                            query.lastOrderMax = Convert.ToDateTime(Request.Params["lastOrderMax"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkNotice"]))
                {
                    if (Request.Params["ChkNotice"] == "true")
                    {
                        query.ChkNotice = true;
                        if (!string.IsNullOrEmpty(Request.Params["noticeCondition"]))
                        {
                            query.noticeCondition = Convert.ToInt32(Request.Params["noticeCondition"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["noticeTimes"]))
                        {
                            query.noticeTimes = Convert.ToInt32(Request.Params["noticeTimes"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkLastLogin"]))
                {
                    if (Request.Params["ChkLastLogin"] == "true")
                    {
                        query.ChkLastLogin = true;
                        if (!string.IsNullOrEmpty(Request.Params["lastLoginMin"]))
                        {
                            query.lastLoginMin = Convert.ToDateTime(Request.Params["lastLoginMin"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["lastLoginMax"]))
                        {
                            query.lastLoginMax = Convert.ToDateTime(Request.Params["lastLoginMax"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkTotalConsumption"]))
                {
                    if (Request.Params["ChkTotalConsumption"] == "true")
                    {
                        query.ChkTotalConsumption = true;
                        if (!string.IsNullOrEmpty(Request.Params["totalConsumptionMin"]))
                        {
                            query.totalConsumptionMin = Convert.ToInt32(Request.Params["totalConsumptionMin"]);
                        }
                        if (!string.IsNullOrEmpty(Request.Params["totalConsumptionMax"]))
                        {
                            query.totalConsumptionMax = Convert.ToInt32(Request.Params["totalConsumptionMax"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Request.Params["ChkBlackList"]))
                {
                    if (Request.Params["ChkBlackList"] == "true")
                    {
                        query.ChkBlackList = true;
                    }
                }
                return query;
            }
            catch (Exception ex)
            {
                throw new Exception(" EdmListConditionMainMgr-->SetQueryValue " + ex.Message, ex);
            }
           
        }
    }
}
