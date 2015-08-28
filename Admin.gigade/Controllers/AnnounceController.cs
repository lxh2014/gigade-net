using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.gigade.Controllers
{
    public class AnnounceController : Controller
    {
        //
        // GET: /Announce/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IAnnounceImplMgr _announcemgr;


        #region view
        /// <summary>
        /// 訊息公告列表頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            return View();
        }
        #endregion

        #region 訊息公告列表頁 +HttpResponseBase GetAnnounceList()
        public HttpResponseBase GetAnnounceList()
        {
            string json = string.Empty;
            List<AnnounceQuery> store = new List<AnnounceQuery>();
            AnnounceQuery query = new AnnounceQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                _announcemgr = new AnnounceMgr(mySqlConnectionString);
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["typeCon"]))
                {
                    query.type = uint.Parse(Request.Params["typeCon"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["statusCon"]))
                {
                    query.con_status = int.Parse(Request.Params["statusCon"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["searchCon"]))
                {
                    query.key = Request.Params["searchCon"];
                }
                store = _announcemgr.GetAnnounceList(query, out totalCount);
                foreach (var item in store)
                {
                    item.content = Server.HtmlDecode(Server.HtmlDecode(item.content));
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
        #region 保存訊息公告 +HttpResponseBase AnnounceContentSave()

        public HttpResponseBase AnnounceContentSave()
        {
            string json = string.Empty;
            List<AnnounceQuery> store = new List<AnnounceQuery>();
            AnnounceQuery query = new AnnounceQuery();
            AnnounceQuery oldquery = new AnnounceQuery();
            _announcemgr = new AnnounceMgr(mySqlConnectionString);
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["announce_id"]))//編輯
                {
                    query.announce_id = Convert.ToUInt32(Request.Params["announce_id"]);
                    oldquery = _announcemgr.GetAnnounce(query);
                    query.modify_date = DateTime.Now;
                    query.modifier = uint.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());

                }
                else
                {
                    query.create_date = DateTime.Now;
                    query.creator = uint.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    query.modify_date = query.create_date;
                    query.modifier = query.creator;
                }
                if (!string.IsNullOrEmpty(Request.Params["title"].ToString()))
                {
                    query.title = Request.Params["title"].ToString();
                }
                else
                {
                    query.title = "";
                }
                uint isUint = 0;
                if (!string.IsNullOrEmpty(Request.Params["type"].ToString()))
                {

                    if (uint.TryParse(Request.Params["type"].ToString(), out isUint))
                    {
                        query.type = Convert.ToUInt32(Request.Params["type"].ToString());
                    }
                    else
                    {
                        query.type = oldquery.type;
                    }

                }
                if (!string.IsNullOrEmpty(Request.Params["sort"].ToString()))
                {
                    query.sort = Convert.ToUInt32(Request.Params["sort"].ToString());
                }
                else
                {
                    query.sort = 0;
                }
                if (!string.IsNullOrEmpty(Request.Params["content"].ToString()))
                {
                    query.content = Request.Params["content"].ToString();
                }
                else
                {
                    query.content = "";
                }
                if (!string.IsNullOrEmpty(Request.Params["status"].ToString()))
                {
                    if (uint.TryParse(Request.Params["status"].ToString(), out isUint))
                    {
                        query.status = Convert.ToUInt32(Request.Params["status"].ToString());
                    }
                    else
                    {
                        query.status = oldquery.status;
                    }

                }

                if (_announcemgr.AnnounceSave(query) > 0)
                {
                    json = "{success:true}";
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
        #endregion

    }
}
