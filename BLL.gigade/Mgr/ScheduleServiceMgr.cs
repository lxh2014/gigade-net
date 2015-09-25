using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class ScheduleServiceMgr
    {
        private ScheduleServiceDao _secheduleServiceDao;
        public ScheduleServiceMgr(string connectionString)
        {
            try
            {
                _secheduleServiceDao = new ScheduleServiceDao(connectionString);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->SecheduleServiceMgr-->" + ex.Message, ex);
            }

        }
        public List<ScheduleMasterQuery> GetExeScheduleMasterList(ScheduleMasterQuery query)
        {
            try
            {
                return _secheduleServiceDao.GetExeScheduleMasterList(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetExeScheduleMasterList-->" + ex.Message, ex);
            }
        }
        public List<ScheduleConfigQuery> GetScheduleConfig(ScheduleConfigQuery query)
        {
            try
            {
                return _secheduleServiceDao.GetScheduleConfig(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetScheduleConfig-->" + ex.Message, ex);
            }
        }
        public SchedulePeriodQuery GetSchedulePeriod(SchedulePeriodQuery query)
        {
            try
            {
                return _secheduleServiceDao.GetSchedulePeriod(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetSchedulePeriod-->" + ex.Message, ex);
            }
        }


        public int UpdateScheduleMaster(ScheduleMasterQuery query)
        {
            try
            {
                return _secheduleServiceDao.UpdateScheduleMaster(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->UpdateScheduleMaster-->" + ex.Message, ex);
            }
        }
        public int UpdateSchedulePeriod(SchedulePeriodQuery query)
        {
            try
            {
                return _secheduleServiceDao.UpdateSchedulePeriod(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->UpdateSchedulePeriod-->" + ex.Message, ex);
            }
        }
        public bool ExeScheduleService(string api)
        {
            bool result = false;
            try
            {
                if(!string.IsNullOrEmpty(api))
                {
                    //api=System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())[2].ToString()+"/"+api;
                    api = "http://localhost:11804/" + api;

                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(api);
                    httpRequest.Timeout = 2000;
                    httpRequest.Method = "GET";
                    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                    result = true;
                }
                return result;

            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->ExeScheduleService-->" + ex.Message, ex);
            }
        }
        public int AddScheduleLog(ScheduleLogQuery query)
        {
            try
            {
                return _secheduleServiceDao.AddScheduleLog(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->AddScheduleLog-->" + ex.Message, ex);
            }
        }
        public int GetNext_Execute_Time(int schedule_id, out int schedule_period_id)
        {
            try
            {
                int time = 0;
                schedule_period_id = 0;
                List<SchedulePeriodQuery> store = _secheduleServiceDao.GetSchedulePeriodBySchedule(schedule_id);
                foreach(SchedulePeriodQuery item in store)
                {
                    if (item.current_nums >= item.limit_nums)
                    {
                        continue;
                    }
                    int begin_datetime = item.begin_datetime;
                    int now = (int)Common.CommonFunction.GetPHPTime();
                    if (item.period_type=="year")
                    {
                        if (item.year > 0)
                        {
                            while (begin_datetime < now)
                            {
                                begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddMonths(item.year).ToString());
                            }
                            if (time > begin_datetime || time==0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                        
                    }
                    else if (item.period_type=="month")
                    {
                        if (item.month > 0)
                        {
                            while (begin_datetime < now)
                            {
                                    begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddMonths(item.month).ToString());
                                
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                    }
                    else if (item.period_type=="week")
                    {
                        if (item.week > 0)
                        {
                            while (begin_datetime < now)
                            {
                                    begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddDays(item.week*7).ToString());
                               
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }

                    }
                    else if (item.period_type=="day")
                    {
                        if (item.day > 0)
                        {
                            while (begin_datetime < now)
                            {
                                    begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddDays(item.day).ToString());
                                
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                    }
                    else if (item.period_type=="hour")
                    {
                        if (item.hour > 0)
                        {
                            while (begin_datetime < now)
                            {
                                    begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddHours(item.hour).ToString());
                                
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                    }
                    else if (item.period_type=="minute")
                    {
                        if (item.minute > 0)
                        {
                            while (begin_datetime < now)
                            {
                                    begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddMinutes(item.minute).ToString());
                                
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                    }
                    
                }
                return time;
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetNext_Execute_Time-->" + ex.Message, ex);
            }
        }

    }
}
