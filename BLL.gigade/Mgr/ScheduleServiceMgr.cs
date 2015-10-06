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
        public ScheduleMasterQuery GetExeScheduleMaster(ScheduleMasterQuery query)
        {
            try
            {
                return _secheduleServiceDao.GetExeScheduleMaster(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetExeScheduleMaster-->" + ex.Message, ex);
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
                    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(api);
                    //httpRequest.Timeout = 10000;
                    httpRequest.Method = "GET";
                    HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                    System.IO.StreamReader sr = new System.IO.StreamReader(httpResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("UTF-8"));
                    string html = sr.ReadToEnd();
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
        public int GetNext_Execute_Time(string schedule_code, out int schedule_period_id)
        {
            try
            {
                int time = 0;
                schedule_period_id = 0;
                List<SchedulePeriodQuery> store = _secheduleServiceDao.GetSchedulePeriodBySchedule(schedule_code);
                foreach(SchedulePeriodQuery item in store)
                {
                    if (item.current_nums >= item.limit_nums && item.limit_nums!=0)
                    {
                        continue;
                    }
                    int begin_datetime = item.begin_datetime;
                    int now = (int)Common.CommonFunction.GetPHPTime();
                    if (item.period_type== 1)
                    {
                        if (item.period_nums > 0)
                        {
                            while (begin_datetime < now)
                            {
                                begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddMonths((int)item.period_nums).ToString());
                            }
                            if (time > begin_datetime || time==0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                        
                    }
                    else if (item.period_type== 2)
                    {
                        if (item.period_nums > 0)
                        {
                            while (begin_datetime < now)
                            {
                                begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddMonths((int)item.period_nums).ToString());
                                
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                    }
                    else if (item.period_type== 3)
                    {
                        if (item.period_nums > 0)
                        {
                            while (begin_datetime < now)
                            {
                                begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddDays((int)item.period_nums * 7).ToString());
                               
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }

                    }
                    else if (item.period_type== 4)
                    {
                        if (item.period_nums > 0)
                        {
                            while (begin_datetime < now)
                            {
                                begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddDays((int)item.period_nums).ToString());
                                
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                    }
                    else if (item.period_type== 5)
                    {
                        if (item.period_nums > 0)
                        {
                            while (begin_datetime < now)
                            {
                                begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddHours((int)item.period_nums).ToString());
                                
                            }
                            if (time > begin_datetime || time == 0)
                            {
                                time = begin_datetime;
                                schedule_period_id = item.rowid;
                            }
                        }
                    }
                    else if (item.period_type== 6)
                    {
                        if (item.period_nums > 0)
                        {
                            while (begin_datetime < now)
                            {
                                begin_datetime = (int)Common.CommonFunction.GetPHPTime(Common.CommonFunction.GetNetTime(begin_datetime).AddMinutes((int)item.period_nums).ToString());
                                
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
