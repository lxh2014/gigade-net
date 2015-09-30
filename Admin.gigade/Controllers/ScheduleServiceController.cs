using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL.gigade.Mgr;
using BLL.gigade.Model.Query;

namespace Admin.gigade.Controllers
{
    public class ScheduleServiceController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string mySqlConnectionString = System.Configuration.ConfigurationManager.AppSettings["MySqlConnectionString"].ToString();
        private ScheduleServiceMgr _secheduleServiceMgr;
        // GET: /Schedule/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 獲取需要執行的排程列表；
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
                query.state = 1;
                store = _secheduleServiceMgr.GetExeScheduleMasterList(query);
                foreach (ScheduleMasterQuery item in store)
                {
                    //獲取該排程參數
                    List<ScheduleConfigQuery> store_config = new List<ScheduleConfigQuery>();
                    ScheduleConfigQuery query_config = new ScheduleConfigQuery();
                    query_config.schedule_id = item.rowid;
                    store_config = _secheduleServiceMgr.GetScheduleConfig(query_config);
                    
                    //執行排程
                    bool result = _secheduleServiceMgr.ExeScheduleService(item.api);
                    if (result)
                    {
                        //記錄排程執行記錄
                        ScheduleLogQuery query_log = new ScheduleLogQuery();
                        query_log.schedule_id = item.rowid;
                        query_log.schedule_period_id = item.schedule_period_id;
                        query_log.create_user = int.Parse((System.Web.HttpContext.Current.Session["caller"] as BLL.gigade.Model.Caller).user_id.ToString());

                        query_log.ipfrom = BLL.gigade.Common.CommonFunction.GetIP4Address(Request.UserHostAddress.ToString());
                        _secheduleServiceMgr.AddScheduleLog(query_log);
                    }

                    //更新SchedulePeriod表的current_nums;ScheduleMaster表的previous_execute_time、next_execute_time、state；
                    //更新current_nums欄位；
                    SchedulePeriodQuery query_period = new SchedulePeriodQuery();
                    query_period.rowid = item.schedule_period_id;
                    query_period = _secheduleServiceMgr.GetSchedulePeriod(query_period);
                    query_period.current_nums += 1;
                    _secheduleServiceMgr.UpdateSchedulePeriod(query_period);

                    //更新ScheduleMaster表的previous_execute_time、next_execute_time、state；
                    item.previous_execute_time = item.next_execute_time;
                    //獲取next_execute_time和schedule_period_id
                    int schedule_period_id = 0;
                    item.next_execute_time = _secheduleServiceMgr.GetNext_Execute_Time(item.rowid, out schedule_period_id);
                    item.schedule_period_id = schedule_period_id;
                    if (item.next_execute_time == 0)
                    {
                        item.state = 0;
                    }
                    _secheduleServiceMgr.UpdateScheduleMaster(item);
                                        

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
        

    }
}
