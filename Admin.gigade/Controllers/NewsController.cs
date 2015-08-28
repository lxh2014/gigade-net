using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using BLL.gigade.Common;
using System.Configuration;
using BLL.gigade.Model;
using System.Net;

namespace Admin.gigade.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /News/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private INewsContentImplMgr _INewsContentMgr;
        #region view
        /// <summary>
        /// 最新消息列表頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            return View();
        }

        public ActionResult LogList()
        {
            ViewBag.newsId = Request.Params["news_id"];
            return View();
        }
        #endregion

        #region 最新消息列表
        public HttpResponseBase GetNewsList()
        {
            string json = string.Empty;
            List<NewsContentQuery> store = new List<NewsContentQuery>();
            NewsContentQuery query = new NewsContentQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                _INewsContentMgr = new NewsContentMgr(mySqlConnectionString);
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["searchCon"]))
                {
                    query.searchCon = Request.Params["searchCon"];
                }
                if (!string.IsNullOrEmpty(Request.Params["search_con"]))
                {
                    query.search_con = Request.Params["search_con"];
                }
                if (!string.IsNullOrEmpty(Request.Params["date"]))
                {
                    query.date = Request.Params["date"];
                }
                if (!string.IsNullOrEmpty(Request.Params["start_time"]))
                {
                    query.start_time = Convert.ToDateTime(Request.Params["start_time"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))
                {
                    query.end_time = Convert.ToDateTime(Request.Params["end_time"]);
                }
                store = _INewsContentMgr.GetNewsList(query, out totalCount);
                foreach (var item in store)
                {
                    item.s_news_show_start = CommonFunction.GetNetTime(item.news_show_start);
                    item.s_news_show_end = CommonFunction.GetNetTime(item.news_show_end);
                    item.s_news_createdate = CommonFunction.GetNetTime(item.news_createdate);
                    item.s_news_updatedate = CommonFunction.GetNetTime(item.news_updatedate);
                    item.news_content = Server.HtmlDecode(Server.HtmlDecode(item.news_content));
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

        #region 保存最新消息
        public HttpResponseBase NewsContentSave()
        {
            string json = string.Empty;
            List<NewsContentQuery> store = new List<NewsContentQuery>();
            NewsContentQuery OldQuery = new NewsContentQuery();
            NewsContentQuery query = new NewsContentQuery();
            _INewsContentMgr = new NewsContentMgr(mySqlConnectionString);
            try
            {
                query.user_id = uint.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                if (string.IsNullOrEmpty(Request.Params["news_id"]))//新增
                {
                    if (!string.IsNullOrEmpty(Request.Params["news_title"]))
                    {
                        query.news_title = Request.Params["news_title"].ToString().Replace("\\", "\\\\");
                    }
                    if (!string.IsNullOrEmpty(Request.Params["news_content"]))
                    {
                        query.news_content = Request.Params["news_content"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["news_sort"]))
                    {
                        query.news_sort = Convert.ToUInt32(Request.Params["news_sort"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["news_status"]))
                    {
                        query.news_status = Convert.ToUInt32(Request.Params["news_status"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["s_news_show_start"]))
                    {
                        query.s_news_show_start = Convert.ToDateTime(Request.Params["s_news_show_start"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["s_news_show_end"]))
                    {
                        query.s_news_show_end = Convert.ToDateTime(Request.Params["s_news_show_end"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["kendoEditor"]))
                    {
                        query.news_content = Request.Params["kendoEditor"].ToString().Replace("\\", "\\\\");
                    }
                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        if (Request.Params["type"] == "direct")//直接上稿
                        {
                            query.news_status = 1;
                        }
                        else
                        {
                            query.news_status = 0;
                        }
                    }
                    //System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                    //if (addlist.Length > 0)
                    //{
                    //    query.news_ipfrom = addlist[0].ToString();
                    //}
                    query.news_ipfrom = GetIP4Address(Request.UserHostAddress.ToString());
                    query.s_news_createdate = DateTime.Now;
                    query.s_news_updatedate = query.s_news_createdate;
                    query.log_description = "add";
                    _INewsContentMgr.NewsContentSave(query);
                    json = "{success:true}";
                }
                else
                {
                    query.news_id = Convert.ToUInt32(Request.Params["news_id"]);
                    OldQuery = _INewsContentMgr.OldQuery(query.news_id);
                    if (!string.IsNullOrEmpty(Request.Params["news_title"]))
                    {
                        query.news_title = Request.Params["news_title"].ToString().Replace("\\", "\\\\");
                    }
                    else
                    {
                        query.news_title = OldQuery.news_title;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["kendoEditor"]))
                    {
                        query.news_content = Request.Params["kendoEditor"].ToString().Replace("\\", "\\\\");
                    }
                    else
                    {
                        query.news_content = OldQuery.news_content;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["news_sort"]))
                    {
                        query.news_sort = Convert.ToUInt32(Request.Params["news_sort"]);
                    }
                    else
                    {
                        query.news_sort = OldQuery.news_sort;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["news_status"]))
                    {
                        query.news_status = Convert.ToUInt32(Request.Params["news_status"]);
                    }
                    else
                    {
                        query.news_status = OldQuery.news_status;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["s_news_show_start"]))
                    {
                        query.s_news_show_start = Convert.ToDateTime(Request.Params["s_news_show_start"]);
                    }
                    else
                    {
                        query.s_news_show_start = CommonFunction.GetNetTime(query.news_show_start);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["s_news_show_end"]))
                    {
                        query.s_news_show_end = Convert.ToDateTime(Request.Params["s_news_show_end"]);
                    }
                    else
                    {
                        query.s_news_show_end = CommonFunction.GetNetTime(query.news_show_end);
                    }
                    System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                    if (addlist.Length > 0)
                    {
                        query.news_ipfrom = addlist[0].ToString();
                    }
                    query.s_news_updatedate = DateTime.Now;
                    query.log_description = "modify,status=" + query.news_status;
                    _INewsContentMgr.NewsContentSave(query);
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
        #endregion

        #region 記錄
        public HttpResponseBase GetNewsLogList()
        {
            string json = string.Empty;
            List<NewsLogQuery> store = new List<NewsLogQuery>();
            NewsLogQuery query = new NewsLogQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["newsId"]))
                {
                    query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                    query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                    query.news_id = Convert.ToUInt32(Request.Params["newsId"]);
                    _INewsContentMgr = new NewsContentMgr(mySqlConnectionString);
                    int totalCount = 0;
                    store = _INewsContentMgr.GetNewsLogList(query, out  totalCount);
                    foreach (var item in store)
                    {
                        item.LogCreateDate = CommonFunction.GetNetTime(item.log_createdate);
                    }
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
                }
                else
                {
                    json = "{success:false,totalCount:0,data:[]}";
                }

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

        public static string GetIP4Address(string s)
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(s))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }
    }


}
