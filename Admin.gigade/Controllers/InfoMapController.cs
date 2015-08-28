
#region 文件信息
/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：InfoMapController.cs      
* 摘 要：                                                                               
* 臺新專區關係設定
* 当前版本：v1.0                                                                 
* 作 者： shuangshuang0420j                                           
* 完成日期：2014/12/05 
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


namespace Admin.gigade.Controllers
{
    public class InfoMapController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();

        private IInfoMapImplMgr _infoMapMgr;
        private IEpaperContentImplMgr _epaperContentMgr;
        private INewsContentImplMgr _newsContentMgr;
        private IAnnounceImplMgr _announceMgr;
        private IEdmContentImplMgr _edmContentMgr;
        //
        // GET: /InfoMap/
        #region 視圖

        public ActionResult InfoMapList()
        {
            return View();
        }

        #endregion

        #region 獲取關係設定的列表
        public HttpResponseBase GetInfoMapList()
        {
            List<InfoMapQuery> store = new List<InfoMapQuery>();
            string json = string.Empty;
            InfoMapQuery query = new InfoMapQuery();
            try
            {
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["siteName"]))
                {
                    query.site_name = Request.Params["siteName"];
                }
                if (!string.IsNullOrEmpty(Request.Params["searchType"]))
                {
                    query.type = Convert.ToInt32(Request.Params["searchType"]);
                }
                _infoMapMgr = new InfoMapMgr(mySqlConnectionString);
                int totalCount = 0;

                store = _infoMapMgr.GetInfoMapList(query, out totalCount);
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

        #region 數據源
        public HttpResponseBase GetEpaperContent()
        {
            string json = string.Empty;
            try
            {
                _epaperContentMgr = new EpaperContentMgr(mySqlConnectionString);

                json = _epaperContentMgr.GetEpaperContent();
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
        public HttpResponseBase GetNewsContent()
        {

            string json = string.Empty;
            try
            {
                _newsContentMgr = new NewsContentMgr(mySqlConnectionString);

                json = _newsContentMgr.GetNewsContent();
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

        public HttpResponseBase GetAnnounce()
        {
            string json = string.Empty;
            try
            {
                _announceMgr = new AnnounceMgr(mySqlConnectionString);

                json = _announceMgr.GetAnnounce();
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
        public HttpResponseBase GetEdmContent()
        {

            string json = string.Empty;
            try
            {
                _edmContentMgr = new EdmContentMgr(mySqlConnectionString);

                json = _edmContentMgr.GetEdmContent();
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

        #region 元素關係保存+SaveInfoMap()
        /// <summary>
        /// 元素關係保存
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase SaveInfoMap()
        {
            string json = string.Empty;
            int isTranInt = 0;
            try
            {
                _infoMapMgr = new InfoMapMgr(mySqlConnectionString);
                InfoMapQuery emQuery = new InfoMapQuery();
                if (!string.IsNullOrEmpty(Request.Params["map_id"]))//修改
                {
                    emQuery.map_id = Convert.ToInt32(Request.Params["map_id"]);
                    InfoMapQuery oldQuery = _infoMapMgr.GetOldModel(emQuery);

                    if (!string.IsNullOrEmpty(Request.Params["site_id"]))
                    {
                        emQuery.site_id = Convert.ToInt32(Request.Params["site_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["page_id"]))
                    {
                        emQuery.page_id = Convert.ToInt32(Request.Params["page_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["area_id"]))
                    {
                        emQuery.area_id = Convert.ToInt32(Request.Params["area_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        emQuery.type = Convert.ToInt32(Request.Params["type"]);
                    }
                    switch (emQuery.type)
                    {
                        case 1:
                            if (int.TryParse(Request.Params["info_id1"], out isTranInt))
                            {
                                emQuery.info_id = Convert.ToInt32(Request.Params["info_id1"]);
                            }
                            else
                            {
                                emQuery.info_id = oldQuery.info_id;
                            }
                            break;
                        case 2:
                            if (int.TryParse(Request.Params["info_id2"], out isTranInt))
                            {
                                emQuery.info_id = Convert.ToInt32(Request.Params["info_id2"]);
                            }
                            else
                            {
                                emQuery.info_id = oldQuery.info_id;
                            }
                            break;
                        case 3:
                            if (int.TryParse(Request.Params["info_id3"], out isTranInt))
                            {
                                emQuery.info_id = Convert.ToInt32(Request.Params["info_id3"]);
                            }
                            else
                            {
                                emQuery.info_id = oldQuery.info_id;
                            }
                            break;
                        case 4:
                            if (int.TryParse(Request.Params["info_id4"], out isTranInt))
                            {
                                emQuery.info_id = Convert.ToInt32(Request.Params["info_id4"]);
                            }
                            else
                            {
                                emQuery.info_id = oldQuery.info_id;
                            }
                            break;

                    }

                    if (!string.IsNullOrEmpty(Request.Params["sort"]))
                    {
                        emQuery.sort = Convert.ToInt32(Request.Params["sort"]);
                    }
                    emQuery.update_date = DateTime.Now;
                    emQuery.update_user_id = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());

                    if (_infoMapMgr.SelectInfoMap(emQuery))
                    {
                        _infoMapMgr.UpdateInfoMap(emQuery);
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }

                }
                else //新增
                {
                    if (!string.IsNullOrEmpty(Request.Params["site_id"]))
                    {
                        emQuery.site_id = Convert.ToInt32(Request.Params["site_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["page_id"]))
                    {
                        emQuery.page_id = Convert.ToInt32(Request.Params["page_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["area_id"]))
                    {
                        emQuery.area_id = Convert.ToInt32(Request.Params["area_id"]);
                    }
                    if (!string.IsNullOrEmpty(Request.Params["type"]))
                    {
                        emQuery.type = Convert.ToInt32(Request.Params["type"]);
                    }
                    switch (emQuery.type)
                    {
                        case 1:
                            if (int.TryParse(Request.Params["info_id1"], out isTranInt))
                            {
                                emQuery.info_id = Convert.ToInt32(Request.Params["info_id1"]);
                            }

                            break;
                        case 2:
                            if (int.TryParse(Request.Params["info_id2"], out isTranInt))
                            {
                                emQuery.info_id = Convert.ToInt32(Request.Params["info_id2"]);
                            }

                            break;
                        case 3:
                            if (int.TryParse(Request.Params["info_id3"], out isTranInt))
                            {
                                emQuery.info_id = Convert.ToInt32(Request.Params["info_id3"]);
                            }

                            break;
                        case 4:
                            if (int.TryParse(Request.Params["info_id4"], out isTranInt))
                            {
                                emQuery.info_id = Convert.ToInt32(Request.Params["info_id4"]);
                            }

                            break;

                    }


                    if (!string.IsNullOrEmpty(Request.Params["sort"]))
                    {
                        emQuery.sort = Convert.ToInt32(Request.Params["sort"]);
                    }
                    emQuery.create_date = DateTime.Now;
                    emQuery.update_date = emQuery.create_date;
                    emQuery.create_user_id = int.Parse((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                    emQuery.update_user_id = emQuery.create_user_id;

                    if (_infoMapMgr.SelectInfoMap(emQuery))
                    {
                        _infoMapMgr.SaveInfoMap(emQuery);
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false}";
                    }

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
