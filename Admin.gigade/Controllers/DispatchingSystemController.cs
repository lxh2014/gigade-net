using System;
using System.Collections.Generic;
using System.Data;
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
    public class DispatchingSystemController : Controller
    {//派工系統
        //
        // GET: /DispatchingSystem/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        public DesignRequestMgr _DesignRequestMgr;
        public IParametersrcImplMgr _IparameterImpMgr;
        private DisableKeywordsMgr _dkMgr;
        #region view
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DesignRequest()
        {
            return View();
        }
        public ActionResult DisabledKeyWords()
        {
            return View();
        }
        #endregion
        #region 派工系統列表
        #region 獲取design_request表的list
        public HttpResponseBase GetDesignRequestList()
        {
            string json = string.Empty;
            int totalCount = 0;
            DesignRequestQuery query = new DesignRequestQuery();
            
            List<DesignRequestQuery> store = new List<DesignRequestQuery>();
            _DesignRequestMgr = new DesignRequestMgr(mySqlConnectionString);
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["dr_requester"]))
                {
                    query.dr_requester_id_name = Request.Params["dr_requester"].ToString().Trim();
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_type"]))
                {
                    query.dr_type = Convert.ToInt32(Request.Params["dr_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["assign_to"]))
                {
                    query.dr_assign_to = Convert.ToInt32(Request.Params["assign_to"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["status"]))
                {
                    query.dr_status = Convert.ToInt32(Request.Params["status"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd 00:00:00");
                }
                if (!string.IsNullOrEmpty(Request.Params["search_date"]))
                {
                    query.date_type = Convert.ToInt32(Request.Params["search_date"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time =Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd 23:59:59");
                }
                query.login_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                store = _DesignRequestMgr.GetList(query,out totalCount);
                foreach (var item in store)
                {
                    if (item.dr_expected < DateTime.Now.Date)
                    {
                        item.Isgq = 1;
                    }
                }
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
        #region 獲取需求類型store
        public HttpResponseBase GetType()
        {
            string json = string.Empty;
            Parametersrc query = new Parametersrc();
            DataTable store = new DataTable();
            _IparameterImpMgr = new ParameterMgr(mySqlConnectionString);
            try
            {
                query.ParameterType="job_type";
                store = _IparameterImpMgr.GetParametercode(query); 
                json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";
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
        #region 新增/編輯保存
        public HttpResponseBase DesignRequestEdit()
        {
            string json = string.Empty;
            DesignRequestQuery query = new DesignRequestQuery();
            _DesignRequestMgr = new DesignRequestMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["dr_id"]))
                {
                    query.dr_id = Convert.ToUInt32(Request.Params["dr_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_content_text"]))
                {
                    query.dr_content_text = Request.Params["dr_content_text"].ToString().Replace("\\", "\\\\"); 
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_description"]))
                {
                    query.dr_description = Request.Params["dr_description"].Replace("\\", "\\\\");
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_document_path"]))
                {
                    query.dr_document_path = Request.Params["dr_document_path"];
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_resource_path"]))
                {
                    query.dr_resource_path = Request.Params["dr_resource_path"];
                }
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["product_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_type"]))
                {
                    int num = 0;
                    if (int.TryParse(Request.Params["dr_type"], out num))
                    {
                        query.dr_type = num;
                    }
                    else
                    {
                        query.dr_type_tostring = Request.Params["dr_type"].ToString();
                    }
                }
                query.dr_created = DateTime.Now;
                query.dr_status = 1;
                query.dr_requester_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                json = _DesignRequestMgr.DesignRequestEdit(query);
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
        #region 刪除
        public HttpResponseBase DelDesignRequest()
        {
            string json = string.Empty;
            DesignRequestQuery query = new DesignRequestQuery();
            _DesignRequestMgr = new DesignRequestMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["dr_id"]))
                {
                    query.dr_ids = Request.Params["dr_id"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_assign_to"]))
                {
                    query.dr_assign_to = int.Parse(Request.Params["dr_assign_to"].ToString());
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_status"]))
                {
                    query.dr_status = int.Parse(Request.Params["dr_status"].ToString());
                }
                query.dr_ids = query.dr_ids.Substring(0, query.dr_ids.Length - 1);
                query.login_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                json = _DesignRequestMgr.DelDesignRequest(query);                
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
        #region 獲取設計人員
        public HttpResponseBase GetDesign()
        {
            string json = string.Empty;
            ManageUserQuery q = new ManageUserQuery();
            DataTable store = new DataTable();
            _DesignRequestMgr = new DesignRequestMgr(mySqlConnectionString);
            try
            {
                store = _DesignRequestMgr.GetDesign(q);
                json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";

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
        #region 根據product_id獲取product_name
        public HttpResponseBase GetProductName()
        {
            string json = string.Empty;
            int id = 0;
           DataTable dt = new DataTable();
           _DesignRequestMgr = new DesignRequestMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    id = Convert.ToInt32(Request.Params["id"]);
                }
                dt = _DesignRequestMgr.GetPorductNameByProductId(id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    json = "{success:true,msg:'" + dt.Rows[0]["product_name"] + "'}";
                }
                else
                {
                    json = "{success:true,msg:0}";
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
        #endregion
        //狀態變更
        public HttpResponseBase UpdStatus()
        {
             string json = string.Empty;
            DesignRequestQuery query = new DesignRequestQuery();
            _DesignRequestMgr = new DesignRequestMgr(mySqlConnectionString);
            try
            {
                //獲取id
                if (!string.IsNullOrEmpty(Request.Params["dr_id"]))
                {
                    query.dr_id = Convert.ToUInt32(Request.Params["dr_id"]);
                }
                //獲取狀態
                if (!string.IsNullOrEmpty(Request.Params["dr_status"]))
                {
                    query.dr_status = Convert.ToInt32(Request.Params["dr_status"]) + 1;
                }
                //獲取派工人員
                if (!string.IsNullOrEmpty(Request.Params["dr_assign_to"]))
                {
                    query.dr_assign_to = Convert.ToInt32(Request.Params["dr_assign_to"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["dr_type"]))
                {
                    query.dr_type = Convert.ToInt32(Request.Params["dr_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["product_id"]))
                {
                    query.product_id = Convert.ToUInt32(Request.Params["product_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["product_detail_text"]))
                {
                    query.dr_content_text =Request.Params["product_detail_text"];
                }
                query.dr_requester_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.login_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                json = _DesignRequestMgr.UpdStatus(query);
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
        #region 指派人員保存
        public HttpResponseBase DesigneeSave()
        {
            string json = string.Empty;
            DesignRequestQuery query = new DesignRequestQuery();
            _DesignRequestMgr = new DesignRequestMgr(mySqlConnectionString);
            try
            {
                //獲取id
                if (!string.IsNullOrEmpty(Request.Params["dr_id"]))
                {
                    query.dr_id = Convert.ToUInt32(Request.Params["dr_id"]);
                }
                //獲取狀態
                if (!string.IsNullOrEmpty(Request.Params["dr_status"]))
                {
                    query.dr_status = Convert.ToInt32(Request.Params["dr_status"]) + 1;
                }
                //獲取派工人員
                if (!string.IsNullOrEmpty(Request.Params["dr_assign_to"]))
                {
                    query.dr_assign_to = Convert.ToInt32(Request.Params["dr_assign_to"]);
                }
                query.login_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                json = _DesignRequestMgr.UpdStatus(query);
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
        #region 認領工作
        public HttpResponseBase TakeJob()
        {
            string json = string.Empty;
            DesignRequestQuery query = new DesignRequestQuery();
            _DesignRequestMgr = new DesignRequestMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["dr_id"]))
                {
                    query.dr_ids = Request.Params["dr_id"].ToString();
                }
                query.dr_ids = query.dr_ids.Substring(0, query.dr_ids.Length - 1);
                query.login_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.dr_assign_to = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.Istake = 1;//認領工作是全部設計人員都可認領
                json = _DesignRequestMgr.UpdStatus(query);
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

        #region 需求狀態store
        public HttpResponseBase GetStatus()
        {
            string json = string.Empty;
            Parametersrc query = new Parametersrc();
            _IparameterImpMgr = new ParameterMgr(mySqlConnectionString);
            DataTable store = new DataTable();
            try
            {
                query.ParameterType = "job_status";
                store = _IparameterImpMgr.GetParametercode(query); 
                json = "{success:true,data:" + JsonConvert.SerializeObject(store) + "}";
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

        #endregion

        #region 禁用關鍵字列表
        public HttpResponseBase GetKeyWordsList()
        {
            string json = string.Empty;
            int totalCount = 0;
            DisableKeywordsQuery query = new DisableKeywordsQuery();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "20");//用於分頁的變量
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["starttime"]) && !string.IsNullOrEmpty(Request.Params["endtime"]))
                {
                    query.start = Convert.ToDateTime(Request.Params["starttime"]);
                    query.end = Convert.ToDateTime(Request.Params["endtime"]).AddDays(1);
                }
                if (!string.IsNullOrEmpty(Request.Params["text"]))
                {
                    query.search_text = Request.Params["text"].ToString().Replace("\\", "\\\\");
                }
                _dkMgr = new DisableKeywordsMgr(mySqlConnectionString);
                DataTable store = _dkMgr.GetKeyWordsList(query, out totalCount);
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
        public HttpResponseBase AddOrEdit()
        {
            string json = string.Empty;
            int result = 0;
            _dkMgr = new DisableKeywordsMgr(mySqlConnectionString);
            try
            {
                DisableKeywordsQuery query = new DisableKeywordsQuery();
                #region 編輯
                if (!string.IsNullOrEmpty(Request.Params["dk_id"]))
                {
                    query.dk_string = Request.Params["dk_string"].ToString().Replace("\\", "\\\\");
                    query.dk_id = Convert.ToInt32(Request.Params["dk_id"]);
                    result = _dkMgr.UpdateKeyWords(query);
                    if (result > 0)
                    {
                        json = "{success:true,msg:\'1\'}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                #endregion
                #region 新增
                else
                {
                    query.dk_string = Request.Params["dk_string"].ToString().Replace("\\", "\\\\");
                    query.user_id = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    query.dk_created = DateTime.Now;
                    result = _dkMgr.AddKeyWords(query);
                    if (result > 0)
                    {
                        json = "{success:true,msg:\'0\'}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }
                }
                #endregion
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
        public HttpResponseBase DeleteKeyWords()
        {
            string json = string.Empty;
            int result = 0;
            _dkMgr = new DisableKeywordsMgr(mySqlConnectionString);
            try
            {
                DisableKeywordsQuery query = new DisableKeywordsQuery();
                if (!string.IsNullOrEmpty(Request.Params["ids"]))
                {
                    query.ids = Request.Params["ids"].ToString();
                }
                result = _dkMgr.DeleteKeyWords(query);
                if (result > 0)
                {
                    json = "{success:true,msg:\'1\'}";
                }
                else
                {
                    json = "{success:false}";
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

        public HttpResponseBase GetCount()
        {
            string json = string.Empty;
            int result = 0;
            _dkMgr = new DisableKeywordsMgr(mySqlConnectionString);
            try
            {
                string dk_string = string.Empty;
                if (!string.IsNullOrEmpty(Request.Params["dk_string"]))
                {
                 dk_string = Request.Params["dk_string"].ToString();
                }
                result = _dkMgr.GetCount(dk_string);
                if (result > 0)//如果大於0表示已存在
                {
                    json = "{success:true,msg:"+result+"}";
                }
                else
                {
                    json = "{success:true,msg:" + 0 + "}";
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
        public HttpResponseBase UpdateStats()
        {
            string json = string.Empty;
            try
            {
                _dkMgr = new DisableKeywordsMgr(mySqlConnectionString);
                DisableKeywordsQuery query = new DisableKeywordsQuery();

                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.dk_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.dk_active = Convert.ToInt32(Request.Params["active"]);
                }
                json = _dkMgr.UpdateStatus(query);
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
 
    }
}
