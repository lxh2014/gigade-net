﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Mgr
{
    public class ScheduleServiceMgr
    {
        private ScheduleServiceDao _secheduleServiceDao;
        private DBAccess.IDBAccess _dbAccess;
        public ScheduleServiceMgr(string connectionString)
        {
            try
            {
                _secheduleServiceDao = new ScheduleServiceDao(connectionString);
                _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
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
        public ScheduleMasterQuery GetScheduleMaster(ScheduleMasterQuery query)
        {
            try
            {
                return _secheduleServiceDao.GetScheduleMaster(query);
            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetScheduleMaster-->" + ex.Message, ex);
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
                if (!string.IsNullOrEmpty(api))
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
                foreach (SchedulePeriodQuery item in store)
                {
                    if (item.current_nums >= item.limit_nums && item.limit_nums != 0)
                    {
                        continue;
                    }
                    int begin_datetime = item.begin_datetime;
                    int now = (int)Common.CommonFunction.GetPHPTime();
                    if (item.period_type == 1)
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
                    else if (item.period_type == 2)
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
                    else if (item.period_type == 3)
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
                    else if (item.period_type == 4)
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
                    else if (item.period_type == 5)
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
                    else if (item.period_type == 6)
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


        public List<ScheduleMasterQuery> GetScheduleMasterList(ScheduleMasterQuery query) // master 
        {
            try
            {
                List<ScheduleMasterQuery> store = new List<ScheduleMasterQuery>();
                store = _secheduleServiceDao.GetScheduleMasterList(query);
                foreach (var item in store)
                {
                    if (item.schedule_state == 0)
                    {
                        item.sschedule_state = "停用";
                    }
                    if (item.schedule_state == 1)
                    {
                        item.sschedule_state = "啟用";
                    }
                    if (item.previous_execute_time == 0)
                    {
                        item.show_previous_execute_time = string.Empty;
                    }
                    else
                    {
                        item.show_previous_execute_time = CommonFunction.GetNetTime(item.previous_execute_time).ToString("yyyy-MM-dd HH:mm:ss ");
                    }
                    if (item.next_execute_time == 0)
                    {
                        item.show_next_execute_time = string.Empty;
                    }
                    else
                    {
                        item.show_next_execute_time = CommonFunction.GetNetTime(item.next_execute_time).ToString("yyyy-MM-dd HH:mm:ss ");
                    }
                    item.show_create_time = CommonFunction.GetNetTime(item.create_time).ToString("yyyy-MM-dd HH:mm:ss ");
                    item.show_change_time = CommonFunction.GetNetTime(item.change_time).ToString("yyyy-MM-dd HH:mm:ss ");
                }
                return store;

            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetScheduleMasterList-->" + ex.Message, ex);
            }
        }

        public List<ScheduleConfigQuery> GetScheduleConfigList(ScheduleConfigQuery query)// config 
        {
            try
            {
                List<ScheduleConfigQuery> store = new List<ScheduleConfigQuery>();
                store = _secheduleServiceDao.GetScheduleConfigList(query);
                foreach (var item in store)
                {
                    item.show_create_time = CommonFunction.GetNetTime(item.create_time).ToString("yyyy-MM-dd  HH:mm:ss ");
                    item.show_change_time = CommonFunction.GetNetTime(item.change_time).ToString("yyyy-MM-dd HH:mm:ss ");
                }
                return store;

            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetScheduleConfigList-->" + ex.Message, ex);
            }
        }

        public List<SchedulePeriodQuery> GetSchedulePeriodList(SchedulePeriodQuery query)// period 
        {
            try
            {
                List<SchedulePeriodQuery> store = new List<SchedulePeriodQuery>();
                store = _secheduleServiceDao.GetSchedulePeriodList(query);
                foreach (var item in store)
                {
                    item.show_create_time = CommonFunction.GetNetTime(item.create_time).ToString("yyyy-MM-dd HH:mm:ss ");
                    item.show_change_time = CommonFunction.GetNetTime(item.change_time).ToString("yyyy-MM-dd HH:mm:ss ");
                   // item.show_begin_datetime = CommonFunction.GetNetTime(item.begin_datetime).ToString("yyyy-MM-dd HH:mm:ss ");
                    item.show_begin_datetime = CommonFunction.GetNetTime(item.begin_datetime);
                    if (item.period_type == 1)
                    {
                        item.show_period_type = "year";
                    }
                    if (item.period_type == 2)
                    {
                        item.show_period_type = "month";
                    }
                    if (item.period_type == 3)
                    {
                        item.show_period_type = "week";
                    }
                    if (item.period_type == 4)
                    {
                        item.show_period_type = "day";
                    }
                    if (item.period_type == 5)
                    {
                        item.show_period_type = "hour";
                    }
                    if (item.period_type == 6)
                    {
                        item.show_period_type = "minute";
                    }
                }
                return store;

            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetSchedulePeriodList-->" + ex.Message, ex);
            }
        }

        public List<ScheduleLogQuery> GetScheduleLogList(ScheduleLogQuery query, out int totalCount)// Log 
        {
            try
            {
                List<ScheduleLogQuery> store = new List<ScheduleLogQuery>();
                store = _secheduleServiceDao.GetScheduleLogList(query, out totalCount);
                foreach (var item in store)
                {
                    item.show_create_time = CommonFunction.GetNetTime(item.create_time).ToString("yyyy-MM-dd HH:mm:ss ");
                }
                return store;

            }
            catch (Exception ex)
            {

                throw new Exception("SecheduleServiceMgr-->GetScheduleLogList-->" + ex.Message, ex);
            }
        }

        public string UpdateStats_Schedule_master(ScheduleMasterQuery query)  
        {
            string json;
            string sql = "";
            try
            {
                if (query.schedule_state == 0)
                {
                    query.schedule_state = 1;
                }
                else
                {
                    query.schedule_state = 0;
                }
                sql = _secheduleServiceDao.UpdateStats_Schedule_master(query);
                if (_dbAccess.execCommand(sql) > 0)
                {
                    json = "{success:true}";
                }
                else
                {
                    json = "{success:false}";
                }
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("SecheduleServiceMgr-->UpdateStats_Schedule_master-->" + ex.Message + sql, ex);
            }
        }// master 的狀態改變

        //schedule_master判断是新增 还是 编辑 
        public int SaveScheduleMasterInfo(ScheduleMasterQuery query)
        {
            try
            {
                if (query.rowid == 0)//新增
                {
                    return _secheduleServiceDao.ScheduleMasterInfoInsert(query);
                }
                else//編輯
                {
                    return _secheduleServiceDao.ScheduleMasterInfoUpdate(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceMgr-->SaveScheduleMasterInfo-->" + ex.Message);
            }
        }

        //schedule_config判断是新增 还是 编辑 
        public int SaveScheduleConfigInfo(ScheduleConfigQuery query)
        {
            try
            {
                if (query.rowid == 0)//新增
                {
                    return _secheduleServiceDao.ScheduleConfigInfoInsert(query);
                }
                else//編輯
                {
                    return _secheduleServiceDao.ScheduleConfigInfoUpdate(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceMgr-->SaveScheduleConfigInfo-->" + ex.Message);
            }
        }

        //schedule_period判断是新增 还是 编辑 
        public int SaveSchedulePeriodInfo(SchedulePeriodQuery query)
        {
            try
            {
                if (query.rowid == 0)//新增
                {
                    return _secheduleServiceDao.SchedulePeriodInfoInsert(query);
                }
                else//編輯
                {
                    return _secheduleServiceDao.SchedulePeriodInfoUpdate(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceMgr-->SaveSchedulePeriodInfo-->" + ex.Message);
            } 
        }

        //可以多行刪除數據_master
        public int ScheduleMasterDelete(string ids)
        {
            try
            {
                return _secheduleServiceDao.ScheduleMasterDelete(ids);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceMgr-->ScheduleMasterDelete-->" + ex.Message);
            }
            
        }

        //可以多行刪除數據_config
        public int ScheduleConfigDelete(string ids)
        {
            try
            {
                return _secheduleServiceDao.ScheduleConfigDelete(ids);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceMgr-->ScheduleConfigDelete-->" + ex.Message);
            }  
            
        }

        //可以多行刪除數據_period
        public int SchedulePeriodDelete(string ids)
        {
            try
            {
                return _secheduleServiceDao.SchedulePeriodDelete(ids);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceMgr-->SchedulePeriodDelete-->" + ex.Message);
            }    
           
        }
        public Boolean SendMail(MailModel mailModel, string GroupCode, string MailTitle, string MailBody, Boolean IsSeparate = false, Boolean IsDisplyName = false)
        {
            Boolean result = false;
            try
            {
                mailModel = GetDeafultMailConfig(mailModel);
                MailHelper mail = new MailHelper(mailModel);

                MailBody = MailBody + " ";
                mail.SendToGroup(GroupCode, MailTitle, MailBody, IsSeparate, IsDisplyName);
                
                //根據群組代碼獲取收件人信息
                
            }
            catch (Exception ex)
            {
                throw new Exception(" ScheduleServiceMgr-->SendMail-->" + ex.Message, ex);
            }

            return result;
        }

        public MailModel GetDeafultMailConfig(MailModel mailModel)
        {
            try
            {
                string xmlPath = System.Configuration.ConfigurationManager.AppSettings["SiteConfig"];//
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlPath);
                SiteConfigMgr _siteConfigMgr = new SiteConfigMgr(path);
                BLL.gigade.Model.SiteConfig NETDoMain_Name;

                if (string.IsNullOrEmpty(mailModel.MailFromAddress))
                {
                    NETDoMain_Name = _siteConfigMgr.GetConfigByName("Mail_From");
                    mailModel.MailFromAddress = NETDoMain_Name.Value;
                }
                if (string.IsNullOrEmpty(mailModel.MailFromUser))
                {
                    NETDoMain_Name = _siteConfigMgr.GetConfigByName("Mail_UserName");
                    mailModel.MailFromUser = NETDoMain_Name.Value;
                }
                if (string.IsNullOrEmpty(mailModel.MailFormPwd))
                {
                    NETDoMain_Name = _siteConfigMgr.GetConfigByName("Mail_UserPasswd");
                    mailModel.MailFormPwd = NETDoMain_Name.Value;
                }
                if (string.IsNullOrEmpty(mailModel.MailPort))
                {
                    NETDoMain_Name = _siteConfigMgr.GetConfigByName("Mail_Port");
                    mailModel.MailPort = NETDoMain_Name.Value;
                }
                if (string.IsNullOrEmpty(mailModel.MailHost))
                {
                    NETDoMain_Name = _siteConfigMgr.GetConfigByName("Mail_Host");
                    mailModel.MailHost = NETDoMain_Name.Value;
                }
                
                return mailModel;

            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleServiceMgr-->GetDeafultMailConfig-->" + ex.Message);
            }
        }
      

    }
}
