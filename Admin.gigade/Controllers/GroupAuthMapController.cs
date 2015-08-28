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
    public class GroupAuthMapController : Controller
    {
        //
        // GET: /GroupAuthMap/
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IGroupAuthMapImplMgr _groupAuthMap;
        private ITFGroupImplMgr _tfgroup;
        public ActionResult Index()
        {
            return View();
        }

            #region 查询权限列表+GroupAuthMapList()
        /// <summary>
        ///权限设定列表
        /// </summary>
        /// <returns>Store</returns>
         public HttpResponseBase GroupAuthMapList()
        {
            List<GroupAuthMapQuery> stores = new List<GroupAuthMapQuery>();

            string json = string.Empty;
            try
            {
                GroupAuthMapQuery query = new GroupAuthMapQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                _groupAuthMap = new GroupAuthMapMgr(mySqlConnectionString);

                if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["table_name"]))
                {
                    query.table_name = Request.Params["table_name"];
                }
                int totalCount = 0;
                stores = _groupAuthMap.QueryAll(query, out totalCount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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

            #region 添加或修改权限设定
                     public HttpResponseBase AddAuthMap()
                     {
                         string json = string.Empty;
                         try
                         {
                             GroupAuthMapQuery query = new GroupAuthMapQuery();
                             query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");//用於分頁的變量
                             query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");//用於分頁的變量
                             _groupAuthMap = new GroupAuthMapMgr(mySqlConnectionString);
                             if (!string.IsNullOrEmpty(Request.Params["group_id"]))
                             {
                                 query.group_id = Convert.ToInt32(Request.Params["group_id"]);
                             }
                             if (!string.IsNullOrEmpty(Request.Params["table_name"]))
                             {
                                 query.table_name = Request.Params["table_name"];
                             }
                             if (!string.IsNullOrEmpty(Request.Params["table_alias_name"]))
                             {
                                 query.table_alias_name = Request.Params["table_alias_name"];
                             }
                             if (!string.IsNullOrEmpty(Request.Params["column_name"]))
                             {
                                 query.column_name= Request.Params["column_name"];
                             }
                             if (!string.IsNullOrEmpty(Request.Params["value"]))
                             {
                                 query.value = Request.Params["value"];
                             }
                             if(!string.IsNullOrEmpty(Request.Params["content_id"]))//修改
                             {
                                query.content_id=Convert.ToInt32(Request.Params["content_id"]);
                                _groupAuthMap.UpGroupAuthMapQuery(query);
                             }
                             else//添加
                             {
                                 query.create_date = DateTime.Now;
                                 query.create_user_id =(System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString(); 
                                 _groupAuthMap.AddGroupAuthMapQuery(query);
                             }
                             IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                             //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                             timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                             json = "{success:true,msg:\"" + "" + "\"}";
                         }
                         catch (Exception ex)
                         {
                             Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                             logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                             logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                             log.Error(logMessage);
                             json = "{success:true,msg:\"" + ex.Message + "\"}";
                         }
                         this.Response.Clear();
                         this.Response.Write(json);
                         this.Response.End();
                         return this.Response;
                     }
	        #endregion

            #region 修改权限状态
                     public JsonResult UpStatus()
                     {
                         int content_id = Convert.ToInt32(Request.Params["id"]);
                         int status = Convert.ToInt32(Request.Params["active"]);


                         _groupAuthMap = new GroupAuthMapMgr(mySqlConnectionString);
                         if ( _groupAuthMap.UpStatus(content_id,status ) > 0)
                         {
                             return Json(new { success = "true", msg = "" });
                         }
                         else
                         {
                             return Json(new { success = "false", msg = "" });
                         }
                     }
        #endregion

        #region 获取群组列表
        /// <summary>
                     /// 获取群组列表+TFGroupList() 绑定下拉框
        /// </summary>
        /// <returns></returns>
          public HttpResponseBase TFGroupList() 
            {

                List<TFGroup> stores = new List<TFGroup>();

                string json = string.Empty;
                try
                {
                    TFGroup query = new TFGroup();

                    _tfgroup = new TFGroupMgr(mySqlConnectionString);
                    stores = _tfgroup.QueryAll(query);
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    json = "{success:true,data:"+ JsonConvert.SerializeObject(stores, Formatting.Indented, timeConverter) + "}";//返回json數據
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
