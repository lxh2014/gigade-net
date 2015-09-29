using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ScheduleServiceDao
    {
          private IDBAccess _access;
          public ScheduleServiceDao(string connectionstring)
          {
                _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
          }
          public List<ScheduleMasterQuery> GetExeScheduleMasterList(ScheduleMasterQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat("SELECT * FROM `schedule_master` WHERE state = '{0}' AND  next_execute_time<='{1}' and next_execute_time > 0 ;", query.state, Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));

                  System.Data.DataTable _dt = _access.getDataTable(sql.ToString());

                  return _access.getDataTableForObj<ScheduleMasterQuery>(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->GetExeScheduleMasterList-->" + ex.Message, ex);
              }
          }
          public List<ScheduleConfigQuery> GetScheduleConfig(ScheduleConfigQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat("SELECT * FROM `schedule_config` WHERE schedule_id = '{0}';", query.schedule_id);
                  return _access.getDataTableForObj<ScheduleConfigQuery>(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->GetScheduleConfig-->" + ex.Message, ex);
              }
          }
          public SchedulePeriodQuery GetSchedulePeriod(SchedulePeriodQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat(@"SELECT `rowid`, `schedule_id`, `period_type`, `year`, `month`, `week`, `day`, `hour`, `minute`, `begin_datetime`, `current_nums`, `limit_nums`, `create_user`, `create_time`, `change_user`, `change_time` 
 FROM `schedule_period` WHERE rowid = '{0}';", query.rowid);
                  return _access.getSinggleObj<SchedulePeriodQuery>(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->GetSchedulePeriod-->" + ex.Message, ex);
              }
          }
          public List<SchedulePeriodQuery> GetSchedulePeriodBySchedule(int schedule_id)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat(@"SELECT `rowid`, `schedule_id`, `period_type`, `year`, `month`, `week`, `day`, `hour`, `minute`, `begin_datetime`, `current_nums`, `limit_nums`, `create_user`, `create_time`, `change_user`, `change_time` 
 FROM `schedule_period` WHERE schedule_id = '{0}';", schedule_id);
                  return _access.getDataTableForObj<SchedulePeriodQuery>(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->GetSchedulePeriodBySchedule-->" + ex.Message, ex);
              }
          }
          public int UpdateScheduleServicePeriod(SchedulePeriodQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat("SELECT * FROM `schedule_config` WHERE schedule_id = '{0}';", query.schedule_id);
                  return _access.execCommand(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->UpdateScheduleServicePeriod-->" + ex.Message, ex);
              }
          }


          public int UpdateScheduleMaster(ScheduleMasterQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat(@"UPDATE `schedule_master` SET `schedule_code`='{0}', `schedule_name`='{1}', `api`='{2}', `description`='{3}', `state`='{4}', `previous_execute_time`='{5}', `next_execute_time`='{6}',`schedule_period_id`='{7}', `create_user`='{8}', `create_time`='{9}', `change_user`='{10}', `change_time`='{11}' WHERE `rowid`='{12}';", query.schedule_code, query.schedule_name, query.api, query.description, query.state, query.previous_execute_time, query.next_execute_time,query.schedule_period_id, query.create_user, query.create_time, query.change_user, Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()), query.rowid);
                  return _access.execCommand(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->UpdateScheduleMaster-->" + ex.Message, ex);
              }
          }
          public int UpdateSchedulePeriod(SchedulePeriodQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat(@"
UPDATE  `schedule_period` SET `schedule_id`='{0}', `period_type`='{1}', `year`='{2}', `month`='{3}', `week`='{4}', `day`='{5}', `hour`='{6}', `minute`='{7}', `begin_datetime`='{8}', `current_nums`='{9}', `limit_nums`='{10}', `create_user`='{11}', `create_time`='{12}', `change_user`='{13}', `change_time`='{14}' WHERE `rowid`='{15}'
;", query.schedule_id, query.period_type, query.year, query.month, query.week, query.day, query.hour, query.minute, query.begin_datetime, query.current_nums, query.limit_nums, query.create_user, query.create_time, query.change_user, Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()), query.rowid);
                  return _access.execCommand(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->UpdateSchedulePeriod-->" + ex.Message, ex);
              }
          }
          public int AddScheduleLog(ScheduleLogQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat(@"INSERT INTO `schedule_log` ( `schedule_id`, `schedule_period_id`, `create_user`, `create_time`, `state`, `request_cost`, `ipfrom`) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');",query.schedule_id,query.schedule_period_id,query.create_user, Common.CommonFunction.GetPHPTime(), query.state,query.request_cost,query.ipfrom);
                  return _access.execCommand(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->AddScheduleLog-->" + ex.Message, ex);
              }
          }
    }
}
