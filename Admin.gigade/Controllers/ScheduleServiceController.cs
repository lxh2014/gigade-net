using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Common;
using BLL.gigade.Mgr;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using BLL.gigade.Mgr.Schedules;

namespace Admin.gigade.Controllers
{
    public class ScheduleServiceController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        string xmlPath = System.Configuration.ConfigurationManager.AppSettings["SiteConfig"];//
        private ScheduleServiceMgr _secheduleServiceMgr;
        // GET: /Schedule/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Schedule_Log_Seacrh()
        {
            return View();
        }
        /// <summary>
        /// 獲取需要執行的排程列表并逐個執行，添加日誌和更新排程；
        /// </summary>
        /// <returns></returns>
        public HttpResponseBase GetExeScheduleServiceList()
        {
            //獲取需要執行的排程

            List<ScheduleMasterQuery> store = new List<ScheduleMasterQuery>();
            _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);

            try
            {
                ScheduleMasterQuery query = new ScheduleMasterQuery();
                query.schedule_state = 1;
                store = _secheduleServiceMgr.GetExeScheduleMasterList(query);
                foreach (ScheduleMasterQuery item in store)
                {
                    try
                    {
                            //執行排程
                            bool result = ExeScheduleService(item.schedule_api,item.schedule_code);
                            
                            //更新SchedulePeriod表的current_nums;ScheduleMaster表的previous_execute_time、next_execute_time、state；
                            //更新current_nums欄位；
                            SchedulePeriodQuery query_period = new SchedulePeriodQuery();
                            query_period.rowid = item.schedule_period_id;
                            query_period = _secheduleServiceMgr.GetSchedulePeriod(query_period);
                            if (query_period != null)
                            {
                                query_period.current_nums += 1;
                                _secheduleServiceMgr.UpdateSchedulePeriod(query_period);
                            }

                            //更新ScheduleMaster表的previous_execute_time、next_execute_time、state；
                            item.previous_execute_time = (int)CommonFunction.GetPHPTime();
                            //獲取next_execute_time和schedule_period_id
                            int schedule_period_id = 0;
                            item.next_execute_time = _secheduleServiceMgr.GetNext_Execute_Time(item.schedule_code, out schedule_period_id);
                            item.schedule_period_id = schedule_period_id;
                            if (item.next_execute_time == 0)
                            {
                                item.schedule_state = 0;
                            }
                            _secheduleServiceMgr.UpdateScheduleMaster(item);
                    }
                    catch (Exception ex)
                    {
                        Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                        logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                        logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                        log.Error(logMessage);
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            //this.Response.Clear();
            //this.Response.Write();
            //this.Response.End();
            return this.Response;
        }
        /// <summary>
        /// 執行單個排程；只添加日誌
        /// </summary>
        /// <param name="schedule_api"></param>
        /// <param name="schedule_code"></param>
        /// <returns></returns>
        public bool ExeScheduleService(string schedule_api,string schedule_code)
        {
            bool result = false;
            try
            {
                //執行排程
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlPath);
                SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                BLL.gigade.Model.SiteConfig NETDoMain_Name = _siteConfigMgr.GetConfigByName("NETDoMain_Name");

                string api = "http://" + NETDoMain_Name.Value + "/" + schedule_api + "?schedule_code=" + schedule_code; ;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                result = _secheduleServiceMgr.ExeScheduleService(api);
                if (result)
                {
                    //添加排程日誌
                    ScheduleAddLog(schedule_code);
                }
                
            }
            catch (Exception ex)
            {
                Log4NetCustom.LogMessage logMessage = new Log4NetCustom.LogMessage();
                logMessage.Content = string.Format("TargetSite:{0},Source:{1},Message:{2}", ex.TargetSite.Name, ex.Source, ex.Message);
                logMessage.MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                log.Error(logMessage);
            }
            return result;
        }        

        public HttpResponseBase GetScheduleMasterList()
        {
            string json = string.Empty;
            int totalcount = 0;
            try
            {
                ScheduleMasterQuery query = new ScheduleMasterQuery();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                List<ScheduleMasterQuery> list = _secheduleServiceMgr.GetScheduleMasterList(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                // timeConverter.DateTimeFormat = "yyyy-MM-dd ";
                json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(list, Formatting.Indented, timeConverter) + "}";
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
            return Response;
 
        } // 獲取 master數據

        public HttpResponseBase GetScheduleConfigList()
        {
            string json = string.Empty;
            ScheduleConfigQuery query = new ScheduleConfigQuery();
            if (!string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                query.schedule_code = Request.Params["schedule_code"];
            }
            try
            {
                List<ScheduleConfigQuery> ipodStore = new List<ScheduleConfigQuery>();
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                ipodStore = _secheduleServiceMgr.GetScheduleConfigList(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(ipodStore, Formatting.Indented, timeConverter) + "}";//返回json數據
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

        }// 獲取 config數據

        public HttpResponseBase GetSchedulePeriodList()// 獲取period數據
        {
            string json = string.Empty;
            SchedulePeriodQuery query = new SchedulePeriodQuery();
            if (!string.IsNullOrEmpty(Request.Params["schedule_code"]))
            {
                query.schedule_code = Request.Params["schedule_code"];
            }
            try
            {
                List<SchedulePeriodQuery> ipodStore = new List<SchedulePeriodQuery>();
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                ipodStore = _secheduleServiceMgr.GetSchedulePeriodList(query);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                json = "{success:true,data:" + JsonConvert.SerializeObject(ipodStore, Formatting.Indented, timeConverter) + "}";//返回json數據
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


        public HttpResponseBase GetScheduleLogList()// 獲取Log數據
        {
            string json = string.Empty;
            int totalcount = 0;
            ScheduleLogQuery query = new ScheduleLogQuery();
            try
            {
                List<ScheduleLogQuery> Store = new List<ScheduleLogQuery>();
                query.Start = Convert.ToInt32(Request.Params["start"] ?? "0");
                query.Limit = Convert.ToInt32(Request.Params["limit"] ?? "25");
                if (!string.IsNullOrEmpty(Request.Params["schedule_code"]))
                {
                    query.schedule_code = Request.Params["schedule_code"];
                }

                if (!string.IsNullOrEmpty(Request.Params["start_time"]))//開始時間
                {
                    query.start_time = (int)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["start_time"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
                if (!string.IsNullOrEmpty(Request.Params["end_time"]))//結束時間
                {
                    query.end_time = (int)CommonFunction.GetPHPTime(Convert.ToDateTime(Request.Params["end_time"]).ToString("yyyy-MM-dd HH:mm:ss"));
                }
              

                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                Store = _secheduleServiceMgr.GetScheduleLogList(query, out totalcount);
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
                //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式     
                timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
               // json = "{success:true,data:" + JsonConvert.SerializeObject(Store, Formatting.Indented, timeConverter) + "}";//返回json數據
                json = "{success:true,totalCount:" + totalcount + ",data:" + JsonConvert.SerializeObject(Store, Formatting.Indented, timeConverter) + "}";
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


        //schedule_master 中的狀態啟用
        public HttpResponseBase UpdateStats_Schedule_master()
        {
            string json = string.Empty;
            try
            {
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                ScheduleMasterQuery query = new ScheduleMasterQuery();

                if (!string.IsNullOrEmpty(Request.Params["id"]))
                {
                    query.rowid = Convert.ToInt32(Request.Params["id"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["active"]))
                {
                    query.schedule_state = Convert.ToInt32(Request.Params["active"]);
                }
                query.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;

                json = _secheduleServiceMgr.UpdateStats_Schedule_master(query);
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


        //保存排程_master信息 
        public HttpResponseBase SaveScheduleMasterInfo()
        {
            string json = string.Empty;
            try
            {
                ScheduleMasterQuery query = new ScheduleMasterQuery();
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);

                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    query.rowid = Convert.ToInt32(Request.Params["rowid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_code"]))
                {
                    query.schedule_code = Request.Params["schedule_code"];
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_name"]))
                {
                    query.schedule_name = Request.Params["schedule_name"];
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_api"]))
                {
                    query.schedule_api = Request.Params["schedule_api"];
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_description"]))
                {
                    query.schedule_description = Request.Params["schedule_description"];
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_state"]))
                {
                    query.schedule_state = Convert.ToInt32(Request.Params["schedule_state"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_period_id"]))
                {
                    query.schedule_period_id = Convert.ToInt32(Request.Params["schedule_period_id"]);
                }
                query.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                //判斷該schedule_code是否已存在
                if (query.rowid == 0)//新增
                {
                    ScheduleMasterQuery query_chongfu = new ScheduleMasterQuery();
                    query_chongfu.schedule_code = query.schedule_code;
                    query_chongfu = _secheduleServiceMgr.GetScheduleMaster(query_chongfu);
                    if (query_chongfu != null)
                    {
                        json = "{success:false,msg:3}";
                    }
                    else
                    {
                        int _dt = _secheduleServiceMgr.SaveScheduleMasterInfo(query);

                        if (_dt > 0)
                        {
                            json = "{success:true}";
                        }
                        else
                        {
                            json = "{success:false,msg:2}";
                        }
                    }
                }
                else
                {
                    int _dt = _secheduleServiceMgr.SaveScheduleMasterInfo(query);

                    if (_dt > 0)
                    {
                        json = "{success:true}";
                    }
                    else
                    {
                        json = "{success:false,msg:2}";
                    }
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
            return Response;

        }

         
        //保存排程_config信息 
        public HttpResponseBase SaveScheduleConfigInfo()
        {
            string json = string.Empty;
            try
            {
                ScheduleConfigQuery query = new ScheduleConfigQuery();
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);

                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    query.rowid = Convert.ToInt32(Request.Params["rowid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_code"]))
                {
                    query.schedule_code = Request.Params["schedule_code"];
                }
                if (!string.IsNullOrEmpty(Request.Params["parameterCode"]))
                {
                    query.parameterCode = Request.Params["parameterCode"];
                }
                if (!string.IsNullOrEmpty(Request.Params["value"]))
                {
                    query.value = Request.Params["value"];
                    query.value = query.value.Replace("\\","\\\\");
                }
                if (!string.IsNullOrEmpty(Request.Params["parameterName"]))
                {
                    query.parameterName = Request.Params["parameterName"];
                }  
                query.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int _dt = _secheduleServiceMgr.SaveScheduleConfigInfo(query);

                if (_dt > 0)
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
                json = "{success:false,totalCount:0,data:[]}";

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return Response;

        }

        //保存排程_period信息 
        public HttpResponseBase SaveSchedulePeriodInfo()
        {
            string json = string.Empty;
            try
            {
                SchedulePeriodQuery query = new SchedulePeriodQuery();
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);

                if (!string.IsNullOrEmpty(Request.Params["rowid"]))
                {
                    query.rowid = Convert.ToInt32(Request.Params["rowid"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["schedule_code"]))
                {
                    query.schedule_code = Request.Params["schedule_code"];
                }
                if (!string.IsNullOrEmpty(Request.Params["period_type"]))
                {
                    query.period_type = Convert.ToUInt32(Request.Params["period_type"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["period_nums"]))
                {
                    query.period_nums = Convert.ToUInt32(Request.Params["period_nums"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["current_nums"]))
                {
                    query.current_nums = Convert.ToUInt32(Request.Params["current_nums"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["limit_nums"]))
                {
                    query.limit_nums = Convert.ToUInt32(Request.Params["limit_nums"]);
                }
                if (!string.IsNullOrEmpty(Request.Params["begin_datetime"]))
                {
                    query.begin_datetime = (int)CommonFunction.GetPHPTime(Request.Params["begin_datetime"]);
                }
                query.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                query.change_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
                int _dt = _secheduleServiceMgr.SaveSchedulePeriodInfo(query);

                if (_dt > 0)
                {
                    json = "{success:true}";
                    //根據schedule_code獲取相應的ScheduleMaster信息
                    ScheduleMasterQuery query_master = new ScheduleMasterQuery();
                    query_master.schedule_code = query.schedule_code;
                    ScheduleMasterQuery item = _secheduleServiceMgr.GetScheduleMaster(query_master);
                    //更新ScheduleMaster表的previous_execute_time、next_execute_time、state；
                    
                    //獲取next_execute_time和schedule_period_id
                    int schedule_period_id = 0;
                    int next_execute_time = _secheduleServiceMgr.GetNext_Execute_Time(item.schedule_code, out schedule_period_id);
                    if (item.next_execute_time > next_execute_time || (item.next_execute_time == 0 && item.next_execute_time < next_execute_time))
                    {
                        item.next_execute_time = next_execute_time; ;
                        item.schedule_period_id = schedule_period_id;
                        _secheduleServiceMgr.UpdateScheduleMaster(item);
                    }
                    
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
                json = "{success:false,totalCount:0,data:[]}";

            }
            this.Response.Clear();
            this.Response.Write(json);
            this.Response.End();
            return Response;

        }

        //可以多行刪除數據_master
        public HttpResponseBase ScheduleMasterDelete()
        {
            string json = string.Empty;
            ScheduleMasterQuery query = new ScheduleMasterQuery();
            _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
            try
            {
                string id = Request.Params["id"];
                string[] ids = id.Split(',');
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    query.rowid = int.Parse(ids[i].ToString());
                    _secheduleServiceMgr.ScheduleMasterDelete(query.rowid.ToString());

                }
                json = "{success:true}";
            }
            #region 只刪除一行數據時的代碼段
            //if (!string.IsNullOrEmpty(Request.Params["id"]))
            //{
            //    query.id = Convert.ToUInt32(Request.Params["id"]);
            //}

            //  int _dt = informationMgr.PersonInfromationDelete(query);

            //if (_dt > 0)
            //{
            //    json = "{success:true}";
            //}
            //else
            //{
            //    json = "{success:false}";
            //}  
            #endregion
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

        
        //立即執行選中的排程
        public HttpResponseBase ScheduleMasterRunOnce()
        {
            string json = string.Empty;
            bool result = false;
            ScheduleMasterQuery query = new ScheduleMasterQuery();
            _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
            try
            {
                string id = Request.Params["id"];
                string[] ids = id.Split(',');
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    string[] scheduleapis = ids[i].Split('&');
                    if(scheduleapis.Length==2)
                    {
                        
                        if (!string.IsNullOrEmpty(scheduleapis[0].ToString())&&!string.IsNullOrEmpty(scheduleapis[1].ToString()))
                        {
                            result = ExeScheduleService(scheduleapis[0].ToString(),scheduleapis[1].ToString());
                        }
                    }
                }
                if (result)
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
        //可以多行刪除數據_config
        public HttpResponseBase ScheduleConfigDelete()
        {
            string json = string.Empty;
            ScheduleConfigQuery query = new ScheduleConfigQuery();
            _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
            try
            {
                string id = Request.Params["id"];
                string[] ids = id.Split(',');
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    query.rowid = int.Parse(ids[i].ToString());
                    _secheduleServiceMgr.ScheduleConfigDelete(query.rowid.ToString());

                }
                json = "{success:true}";
            }
            #region 只刪除一行數據時的代碼段
            //if (!string.IsNullOrEmpty(Request.Params["id"]))
            //{
            //    query.id = Convert.ToUInt32(Request.Params["id"]);
            //}

            //  int _dt = informationMgr.PersonInfromationDelete(query);

            //if (_dt > 0)
            //{
            //    json = "{success:true}";
            //}
            //else
            //{
            //    json = "{success:false}";
            //}  
            #endregion
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

        //可以多行刪除數據_period
        public HttpResponseBase SchedulePeriodDelete()
        {
            string json = string.Empty;
            SchedulePeriodQuery period = new SchedulePeriodQuery();
            _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
            try
            {
                string id = Request.Params["id"];
                id = id.Substring(0, id.Length - 1).ToString();
                string[] ids = id.Split(',');
                if (ids.Length>0)
                {

                    period.rowid = int.Parse(ids[0].ToString());
                    period = _secheduleServiceMgr.GetSchedulePeriod(period);

                    ScheduleMasterQuery query_master = new ScheduleMasterQuery();
                    query_master.schedule_code = period.schedule_code;
                    
                    //刪除
                    int result = _secheduleServiceMgr.SchedulePeriodDelete(id);
                    if (result > 0)
                    {
                         ScheduleMasterQuery item = _secheduleServiceMgr.GetScheduleMaster(query_master);
                        //更新ScheduleMaster表的previous_execute_time、next_execute_time、state；

                        //獲取next_execute_time和schedule_period_id
                        int schedule_period_id = 0;
                        int next_execute_time = _secheduleServiceMgr.GetNext_Execute_Time(item.schedule_code, out schedule_period_id);
                        //if (item.next_execute_time > next_execute_time || (item.next_execute_time == 0 && item.next_execute_time < next_execute_time))
                        {
                            item.next_execute_time = next_execute_time;
                            if (item.next_execute_time == 0)
                            {
                                item.schedule_state = 0;
                            }
                            item.schedule_period_id = schedule_period_id;
                            //修改ScheduleMaster
                            _secheduleServiceMgr.UpdateScheduleMaster(item);
                        }
                    }

                }
                json = "{success:true}";
            }
            #region 只刪除一行數據時的代碼段
            //if (!string.IsNullOrEmpty(Request.Params["id"]))
            //{
            //    query.id = Convert.ToUInt32(Request.Params["id"]);
            //}

            //  int _dt = informationMgr.PersonInfromationDelete(query);

            //if (_dt > 0)
            //{
            //    json = "{success:true}";
            //}
            //else
            //{
            //    json = "{success:false}";
            //}  
            #endregion
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

        //添加排程執行日誌
        private void ScheduleAddLog(string schedule_code)
        {
            ScheduleLogQuery query_log = new ScheduleLogQuery();
            query_log.schedule_code = schedule_code;

            try
            {
                //如果通過瀏覽器登陸；
                query_log.create_user = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            }
            catch (Exception)
            {
                //根據schedule_code獲取相應period的change_user
                ScheduleMasterQuery query_master = new ScheduleMasterQuery();
                query_master.schedule_code = schedule_code;
                _secheduleServiceMgr = new ScheduleServiceMgr(mySqlConnectionString);
                ScheduleMasterQuery master = _secheduleServiceMgr.GetScheduleMaster(query_master);
                if (master.schedule_period_id != 0)
                {
                    SchedulePeriodQuery query_period = new SchedulePeriodQuery();
                    query_period.rowid = master.schedule_period_id;
                    query_period = _secheduleServiceMgr.GetSchedulePeriod(query_period);
                    query_log.create_user = query_period.change_user;
                }

            }

            query_log.ipfrom = BLL.gigade.Common.CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
            _secheduleServiceMgr.AddScheduleLog(query_log);
        }

        /************************排程******************************/
        
    }
}
