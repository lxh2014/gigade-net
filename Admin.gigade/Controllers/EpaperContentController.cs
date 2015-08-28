#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：EpaperContentController.cs      
* 摘 要：                                                                               
* 活動頁面列表
* 当前版本：v1.1                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/12/04
* 修改歷史：                                                                     
*      
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Admin.gigade.CustomError;
using BLL.gigade.Model.Query;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using BLL.gigade.Model;
using BLL.gigade.Common;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Mgr;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Net;
using System.Data;
namespace Admin.gigade.Controllers
{
    public class EpaperContentController : Controller
    {
        //
        // GET: /EpaperContent/
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private IEpaperContentImplMgr _IEpaperContentMgr;
        private IBrowseDataImplMgr _IBrowseDataMgr;
        private IProductItemImplMgr _IProductItemMgr;
        private IOrderMasterImplMgr _IOrderMasterMgr;

        #region 視圖
        public ActionResult EpaperContentList()
        {
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            return View();
        }
        public ActionResult EpaperLogList()
        {
            ViewBag.epaperId = Request.Params["epaperId"];
            return View();
        }

        public ActionResult EpaperContentAdd()
        {
            ViewBag.epaperId = Request.Params["epaper_id"];
            ViewBag.BaseAddress = ConfigurationManager.AppSettings["webDavBaseAddress"];
            ViewBag.path = ConfigurationManager.AppSettings["webDavImage"];
            return View();
        }

        /// <summary>
        /// 商品點擊查詢
        /// </summary>
        /// <returns>商品點擊查詢頁面</returns>
        public ActionResult BrowseData()
        {
            return View();
        }
        #endregion

        #region 活動頁面列表
        public HttpResponseBase GetEpaperContentList()
        {
            string json = string.Empty;
            List<EpaperContentQuery> store = new List<EpaperContentQuery>();
            EpaperContentQuery query = new EpaperContentQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                _IEpaperContentMgr = new EpaperContentMgr(mySqlConnectionString);
                int totalCount = 0;
                if (!string.IsNullOrEmpty(Request.Params["searchCon"]))
                {
                    query.searchCon = Request.Params["searchCon"];
                }
                if (!string.IsNullOrEmpty(Request.Params["search_text"]))
                {
                    query.search_text = Request.Params["search_text"];
                }
                if (!string.IsNullOrEmpty(Request.Params["dateCon"]))
                {
                    query.dateCon = Request.Params["dateCon"];
                }
                if (!string.IsNullOrEmpty(Request.Params["date_start"]))
                {
                    query.epaperShowStart = Convert.ToDateTime(Request.Params["date_start"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["date_end"]))
                {
                    query.epaperShowEnd = Convert.ToDateTime(Request.Params["date_end"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["activeStatus"]))
                {
                    query.epaperStatus = Request.Params["activeStatus"].ToString();
                }
                if (!string.IsNullOrEmpty(Request.Params["sizeCon"]))
                {
                    query.epaper_size = Request.Params["sizeCon"];
                }
                store = _IEpaperContentMgr.GetEpaperContentList(query, out totalCount);
                foreach (var item in store)
                {
                    item.epaperShowStart = CommonFunction.GetNetTime(item.epaper_show_start);
                    item.epaperShowEnd = CommonFunction.GetNetTime(item.epaper_show_end);
                    item.epaperCreateDate = CommonFunction.GetNetTime(item.epaper_createdate);
                    item.epaperUpdateDate = CommonFunction.GetNetTime(item.epaper_updatedate);
                    item.epaper_content = Server.HtmlDecode(Server.HtmlDecode(item.epaper_content));
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

        #region 活動歷史記錄列表
        public HttpResponseBase GetEpaperLogList()
        {
            string json = string.Empty;
            List<EpaperLogQuery> store = new List<EpaperLogQuery>();
            EpaperLogQuery query = new EpaperLogQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["epaperId"]))
                {
                    query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                    query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                    query.epaper_id = Convert.ToUInt32(Request.Params["epaperId"]);
                    _IEpaperContentMgr = new EpaperContentMgr(mySqlConnectionString);
                    int totalCount = 0;
                    store = _IEpaperContentMgr.GetEpaperLogList(query, out  totalCount);
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

        #region 活動頁面新增編輯
        public HttpResponseBase SaveEpaperContent()
        {
            string json = string.Empty;
            List<EpaperContentQuery> store = new List<EpaperContentQuery>();
            EpaperContentQuery OldQuery = new EpaperContentQuery();
            EpaperContentQuery query = new EpaperContentQuery();
            _IEpaperContentMgr = new EpaperContentMgr(mySqlConnectionString);
            try
            {
                query.user_id = uint.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                if (string.IsNullOrEmpty(Request.Params["epaper_id"]))//新增
                {
                    if (!string.IsNullOrEmpty(Request.Params["epaper_title"]))
                    {
                        query.epaper_title = Request.Params["epaper_title"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_short_title"]))
                    {
                        query.epaper_short_title = Request.Params["epaper_short_title"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_size"]))
                    {
                        query.epaper_size = Request.Params["epaper_size"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        query.type = Convert.ToUInt32(Request.Params["type"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_sort"]))
                    {
                        query.epaper_sort = Convert.ToUInt32(Request.Params["epaper_sort"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaperShowStart"]))
                    {
                        query.epaperShowStart = Convert.ToDateTime(Request.Params["epaperShowStart"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaperShowEnd"]))
                    {
                        query.epaperShowEnd = Convert.ToDateTime(Request.Params["epaperShowEnd"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["kendoEditor"]))
                    {
                        query.epaper_content = Request.Params["kendoEditor"];
                    }

                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        if (Request.Params["newsType"] == "direct")//直接上稿
                        {
                            query.epaper_status = 1;
                        }
                        else
                        {
                            query.epaper_status = 0;
                        }
                    }
                    System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                    if (addlist.Length > 0)
                    {
                        query.epaper_ipfrom = addlist[0].ToString();
                    }
                    query.epaperCreateDate = DateTime.Now;
                    query.epaperUpdateDate = query.epaperCreateDate;
                    query.log_description = "add";
                    _IEpaperContentMgr.SaveEpaperContent(query);
                    json = "{success:true}";
                }
                else
                {
                    query.epaper_id = Convert.ToUInt32(Request.Params["epaper_id"]);
                    OldQuery = _IEpaperContentMgr.GetEpaperContentById(query);
                    if (!string.IsNullOrEmpty(Request.Params["epaper_title"]))
                    {
                        query.epaper_title = Request.Params["epaper_title"];
                    }
                    else
                    {
                        query.epaper_title = OldQuery.epaper_title;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_short_title"]))
                    {
                        query.epaper_short_title = Request.Params["epaper_short_title"];
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_size"]))
                    {
                        query.epaper_size = Request.Params["epaper_size"];
                    }
                    else
                    {
                        query.epaper_size = OldQuery.epaper_size;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        query.type = Convert.ToUInt32(Request.Params["type"]);
                    }
                    else
                    {
                        query.type = OldQuery.type;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_sort"]))
                    {
                        query.epaper_sort = Convert.ToUInt32(Request.Params["epaper_sort"]);
                    }
                    else
                    {
                        query.epaper_sort = OldQuery.epaper_sort;
                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaperShowStart"]))
                    {
                        query.epaperShowStart = Convert.ToDateTime(Request.Params["epaperShowStart"]);
                    }
                    else
                    {
                        query.epaperShowStart = OldQuery.epaperShowStart;

                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaperShowEnd"]))
                    {
                        query.epaperShowEnd = Convert.ToDateTime(Request.Params["epaperShowEnd"]);
                    }
                    else
                    {
                        query.epaperShowEnd = OldQuery.epaperShowEnd;

                    }
                    if (!string.IsNullOrEmpty(Request.Params["kendoEditor"]))
                    {
                        query.epaper_content = Request.Params["kendoEditor"];
                    }
                    else
                    {
                        query.epaper_content = OldQuery.epaper_content;

                    }
                    if (!string.IsNullOrEmpty(Request.Params["epaper_status"]))
                    {
                        query.epaper_status = Convert.ToUInt32(Request.Params["epaper_status"]);
                    }
                    else
                    {
                        query.epaper_status = OldQuery.epaper_status;

                    }
                    if (!string.IsNullOrEmpty(Request.Params["fb_description"]))
                    {
                        query.fb_description = Request.Params["fb_description"];
                    }
                    else
                    {
                        query.fb_description = OldQuery.fb_description;

                    }
                    System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
                    if (addlist.Length > 0)
                    {
                        query.epaper_ipfrom = addlist[0].ToString();
                    }
                    query.epaperUpdateDate = DateTime.Now;
                    query.log_description = "modify,status=" + query.epaper_status;
                    _IEpaperContentMgr.SaveEpaperContent(query);
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

        public HttpResponseBase GetEpaperById()
        {
            string json = string.Empty;
            EpaperContentQuery store = new EpaperContentQuery();
            EpaperContentQuery query = new EpaperContentQuery();
            try
            {
                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    _IEpaperContentMgr = new EpaperContentMgr(mySqlConnectionString);
                    store.epaper_id = Convert.ToUInt32(Request.Params["rowid"]);
                    store = _IEpaperContentMgr.GetEpaperContentById(store);
                    store.epaper_content = Server.HtmlDecode(store.epaper_content);
                    store.epaperShowStart = CommonFunction.GetNetTime(store.epaper_show_start);
                    store.epaperShowEnd = CommonFunction.GetNetTime(store.epaper_show_end);
                    store.epaperCreateDate = CommonFunction.GetNetTime(store.epaper_createdate);
                    store.epaperUpdateDate = CommonFunction.GetNetTime(store.epaper_updatedate);
                    IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                    json = "{success:true" + ",data:" + JsonConvert.SerializeObject(store, Formatting.Indented, timeConverter) + "}";
                }
                else
                {
                    json = "{success:false,data:[]}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }

        /// <summary>
        /// 商品點擊信息列表
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase BrowseDataList()
        {
            string json = string.Empty;
            DataTable dtBrowseData = new DataTable();
            int totalCount = 0;
            BrowseDataQuery query = new BrowseDataQuery();

            string type=Request.Params["type"];
            string searchContent = Request.Params["searchContent"];
            if (!string.IsNullOrEmpty(type))
            {
                query.type = Convert.ToInt32(type);
            }
            if (!string.IsNullOrEmpty(searchContent))
            {
                query.SearchCondition = searchContent;
            }
            if (!string.IsNullOrEmpty(Request.Params["true"]))
            {
                if (Request.Params["true"] == "true")
                {
                    query.isSecret = true;
                }
                else
                {
                    query.isSecret = false;
                }
            }
            query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
            query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");

            _IBrowseDataMgr = new BrowseDataMgr(mySqlConnectionString);
            _IProductItemMgr=new ProductItemMgr(mySqlConnectionString);
            _IOrderMasterMgr = new OrderMasterMgr(mySqlConnectionString);

            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            try
            {
                dtBrowseData = _IBrowseDataMgr.GetBrowseDataList(query, out totalCount);
                if (dtBrowseData.Rows.Count>0)
                {
                    dtBrowseData.Columns.Add("buyCount");
                    for (int i = 0; i < dtBrowseData.Rows.Count; i++)
                    {
                        string user_id = dtBrowseData.Rows[i]["user_id"].ToString();
                        string product_id = dtBrowseData.Rows[i]["product_id"].ToString();
                        string item_id = string.Empty;
                        if (query.isSecret)
                        {
                            dtBrowseData.Rows[i]["user_name"] = dtBrowseData.Rows[i]["user_name"].ToString().Substring(0, 1) + "**";
                        }
                        List<ProductItem> listProductItem = _IProductItemMgr.GetProductItemByID(Convert.ToInt32(product_id));
                        if (listProductItem.Count > 0)
                        {
                            item_id = listProductItem[0].Item_Id.ToString();
                        }
                        OrderMasterQuery orderMasterQuery = new OrderMasterQuery
                        {
                            User_Id = Convert.ToUInt32(dtBrowseData.Rows[i]["user_id"]),
                            Item_Id = Convert.ToInt32(item_id)
                        };
                        dtBrowseData.Rows[i]["buyCount"] = _IOrderMasterMgr.GetBuyCount(orderMasterQuery);
                    }
                    json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dtBrowseData, Formatting.Indented, timeConverter) + "}";

                }
                else
                {
                    json = "{success:true,totalCount:" + totalCount + ",data:" + JsonConvert.SerializeObject(dtBrowseData, Formatting.Indented, timeConverter) + "}";
                    //json = "{success:false,msg:0}";
                }

            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
                json = "{success:false,msg:0}";
            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return this.Response;
        }
    }
}