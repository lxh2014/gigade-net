using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Data;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Common;

namespace Admin.gigade.Controllers
{
    public class UserForbidController : Controller
    {
        //
        // GET: /UserForbid/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private UserLoginAttemptsMgr ulaMgr;
        private IUserForbidImplMgr _IuserForbidMgr;
        #region 页面
        /// <summary>
        /// 登入失敗統計功能
        /// </summary>
        /// <returns></returns>
        public ActionResult UserLoginAttempts()
        {
            return View();
        }
        /// <summary>
        /// 黑名單列表
        /// </summary>
        /// <returns></returns>
        public ActionResult UserForbid()
        {
            return View();
        }
        #endregion 
        #region 用戶登入失敗統計
        /// <summary>
        /// 列表頁
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetUserLoginList()
        {
            string json = string.Empty;
            try
            {
                UserLoginAttempts query = new UserLoginAttempts();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                query.login_mail = Request.Params["login_mail"];
                query.login_ipfrom = Request.Params["login_ipfrom"];
                if (!string.IsNullOrEmpty(Request.Params["start_date"]))
                {
                    query.slogin_createdate = (int)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["start_date"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end"]))
                {
                    query.elogin_createdate = (int)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["end"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(Request.Params["sumtotal"]))
                {
                    query.sumtotal = int.Parse(Request.Params["sumtotal"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["ismail"]))
                {
                    query.ismail = int.Parse(Request.Params["ismail"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["login_type"]))
                {
                    query.login_type = int.Parse(Request.Params["login_type"]);
                }
                int totalCount = 0;
                ulaMgr = new UserLoginAttemptsMgr(mySqlConnectionString);

                DataTable dt = ulaMgr.GetUserLoginAttemptsList(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dt, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        #region 黑名單列表
        /// <summary>
        /// 黑名單列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetUserForbidList()
        {
            UserForbidQuery query = new UserForbidQuery();
            List<UserForbidQuery> stores = new List<UserForbidQuery>();
            string json = string.Empty;
            try
            {

                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                if (!string.IsNullOrEmpty(Request.Params["serchcontent"]))
                {
                    query.forbid_ip = Request.Params["serchcontent"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["timestart"]))
                {
                    query.timestart = Convert.ToDateTime(Request.Params["timestart"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (!string.IsNullOrEmpty(Request.Params["timeend"]))
                {
                    query.timeend = Convert.ToDateTime(Request.Params["timeend"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                _IuserForbidMgr = new UserForbidMgr(mySqlConnectionString);
                int totalCount = 0;
                stores = _IuserForbidMgr.GetUserForbidList(query, out totalCount);
                //foreach (var item in stores)
                //{
                //    item.screatedate = CommonFunction.GetNetTime(item.createdate);

                //}
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                //listUser是准备转换的对象
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據

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

        /// <summary>
        /// 新增黑名單
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveUserFoid()
        {
            string json = string.Empty;
            UserForbidQuery query = new UserForbidQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["forbid_id"]))//修改
                {
                    query.forbid_id = Convert.ToInt32(Request.Params["forbid_id"]);
                    query.forbid_ip = Request.Params["forbid_ip"];
                    //_siteMgr = new SiteMgr(mySqlConnectionString);
                    //_siteMgr.UpSite(query);
                }
                else//新增
                {
                    query.forbid_ip = Request.Params["forbid_ip"];
                    query.forbid_createuser = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    query.forbid_createdate = Convert.ToInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    _IuserForbidMgr = new UserForbidMgr(mySqlConnectionString);
                    int j = _IuserForbidMgr.GetUserForbidIp(query);
                    if (j > 0)
                    {
                        json = "{success:true,msg:\"" + "1" + "\"}";
                    }
                    else
                    {
                        _IuserForbidMgr.UserForbidInsert(query);
                        json = "{success:true,msg:\"" + "" + "\"}";
                    }
                }


            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + ex.Message + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 刪除黑名單
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase DeleteUserFoid()
        {
            string json = string.Empty;
            UserForbidQuery query = new UserForbidQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["rowID"]))//修改
                {
                    string str = Request.Params["rowID"].ToString();
                    str = str.Remove(str.LastIndexOf(','));
                    query.rowIds = str;

                    _IuserForbidMgr = new UserForbidMgr(mySqlConnectionString);
                    _IuserForbidMgr.UserForbidDelete(query);
                }
                json = "{success:true}";
                //返回json數據

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:\"" + ex.Message + "\"}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
        #endregion
    }
}
