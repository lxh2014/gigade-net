using System;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using gigadeExcel.Comment;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class EdmNewController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        public EdmGroupNewMgr edmgroupmgr;
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
        #endregion

        #region 電子報類型

        #region 電子報類型列表頁
        public HttpResponseBase GetEdmGroupNewList()
        {
            string json = string.Empty;
            int totalcount = 0;
            EdmGroupNewQuery query=new EdmGroupNewQuery ();
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
            edmgroupmgr = new EdmGroupNewMgr(mySqlConnectionString);
            if (!string.IsNullOrEmpty(Request.Params["group_name"]))
            {
                query.group_name = Request.Params["group_name"];
            }
            List<EdmGroupNewQuery> list = edmgroupmgr.GetEdmGroupNewList(query, out totalcount);
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            timeConverter.DateTimeFormat = "yyyy-MM-dd";
            json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return Response;
 
        }
        #endregion

        #region 狀態啟用 
        public HttpResponseBase UpdateStats()
        {
            string json = string.Empty;
            try
            {
                edmgroupmgr = new EdmGroupNewMgr (mySqlConnectionString);
                EdmGroupNewQuery query = new EdmGroupNewQuery();

                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.group_id = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.enabled = Convert.ToInt32(Request.Params["active"]);
                }
                json = edmgroupmgr.UpdateStatus(query);
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

        #endregion

    }
}
