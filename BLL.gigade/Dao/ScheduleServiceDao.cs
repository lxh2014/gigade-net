﻿using System;
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
          private DBAccess.IDBAccess _dbAccess;
          public ScheduleServiceDao(string connectionstring)
          {
                _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
                _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionstring);
          }
          public List<ScheduleMasterQuery> GetExeScheduleMasterList(ScheduleMasterQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat("SELECT * FROM `schedule_master` WHERE schedule_state = '{0}' AND  next_execute_time<='{1}' and next_execute_time > 0 ;", query.schedule_state, Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));

                  return _access.getDataTableForObj<ScheduleMasterQuery>(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->GetExeScheduleMasterList-->" + ex.Message, ex);
              }
          }

          public List<ScheduleMasterQuery> GetScheduleMasterList(ScheduleMasterQuery query)
          {
              StringBuilder sql = new StringBuilder();
              StringBuilder str = new StringBuilder();
              try
              {
                  sql.AppendFormat(" SELECT sm.rowid,sm.schedule_code,sm.schedule_name,sm.schedule_api,sm.schedule_description,sm.schedule_state,sm.previous_execute_time,sm.next_execute_time,sm.schedule_period_id, mu1.user_username as create_username,sm.create_time,mu2.user_username as change_username,sm.change_time  FROM schedule_master sm ");
                  str.Append(" LEFT JOIN manage_user mu1 on mu1.user_id=sm.create_user ");
                  str.Append(" LEFT JOIN manage_user mu2 on mu2.user_id=sm.change_user ");
                  sql.Append(str.ToString());
                  return _access.getDataTableForObj<ScheduleMasterQuery>(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->GetScheduleMasterList-->" + ex.Message, ex);
              }
          }

          public List<ScheduleConfigQuery> GetScheduleConfigList(ScheduleConfigQuery query)
          {
              StringBuilder sql = new StringBuilder();
               StringBuilder sqlCondi = new StringBuilder();
              try
              {
                  sql.AppendFormat("SELECT sc.schedule_code,sc.parameterCode,sc.value,sc.description, mu1.user_username as create_username,sc.create_time, mu2.user_username as change_username,sc.change_time  FROM schedule_config sc ");
                  sqlCondi.Append(" LEFT JOIN schedule_master sm on sm.schedule_code=sc.schedule_code ");
                  sqlCondi.Append(" LEFT JOIN manage_user mu1 on mu1.user_id=sc.create_user ");
                  sqlCondi.Append(" LEFT JOIN manage_user mu2 on mu2.user_id=sc.change_user ");
                  sqlCondi.Append(" where 1=1 ");
                  if (!string.IsNullOrEmpty(query.schedule_code))
                  {
                      sqlCondi.AppendFormat(" and sm.schedule_code='{0}' ", query.schedule_code);
                  }
                  sql.Append(sqlCondi.ToString());
                  return _access.getDataTableForObj<ScheduleConfigQuery>(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->GetScheduleMasterList-->" + ex.Message, ex);
              }
          }

          public List<SchedulePeriodQuery> GetSchedulePeriodList(SchedulePeriodQuery query)
          {
              StringBuilder sql = new StringBuilder();
              StringBuilder sqlCondi = new StringBuilder();
              try
              {
                  sql.AppendFormat("SELECT sp.schedule_code,sp.period_type,sp.period_nums,sp.begin_datetime,sp.current_nums,sp.limit_nums,mu1.user_username as create_username,mu2.user_username as change_username,sp.create_time,sp.change_time  FROM schedule_period sp");
                  sqlCondi.Append(" LEFT JOIN schedule_master sm on sm.schedule_code=sp.schedule_code ");
                  sqlCondi.Append(" LEFT JOIN manage_user mu1 on mu1.user_id=sp.create_user ");
                  sqlCondi.Append(" LEFT JOIN manage_user mu2 on mu2.user_id=sp.change_user ");
                  sqlCondi.Append(" where 1=1 ");
                  if (!string.IsNullOrEmpty(query.schedule_code))
                  {
                      sqlCondi.AppendFormat(" and sm.schedule_code='{0}' ", query.schedule_code);
                  }
                  sql.Append(sqlCondi.ToString());
                  return _access.getDataTableForObj<SchedulePeriodQuery>(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->GetSchedulePeriodList-->" + ex.Message, ex);
              }
          }

          public string UpdateStats_Schedule_master(ScheduleMasterQuery query)  // master 狀態更新
          {
              StringBuilder strSql = new StringBuilder();
              try
              {
                  strSql.AppendFormat(@"Update schedule_master set schedule_state='{0}' WHERE rowid='{1}'", query.schedule_state, query.rowid);
                  return strSql.ToString();
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->UpdateStats_Schedule_master-->" + ex.Message + strSql.ToString(), ex);
              }
          }

          //插入schedule_master信息
          public int ScheduleMasterInfoInsert(ScheduleMasterQuery query)
          {
              StringBuilder sql = new StringBuilder();
              query.Replace4MySQL();
              try
              {
                  sql.Append("insert into schedule_master ( schedule_code, schedule_name, schedule_api,schedule_description,schedule_state,schedule_period_id,create_user,change_user, create_time, change_time, previous_execute_time,next_execute_time) values ");
                  sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", query.schedule_code, query.schedule_name, query.schedule_api, query.schedule_description,query.schedule_state, query.schedule_period_id, query.create_user, query.change_user, query.create_time, query.change_time, query.previous_execute_time,query.next_execute_time);

                  return _access.execCommand(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->ScheduleMasterInfoInsert-->" + sql.ToString() + ex.Message);
              }
          }
          //更新人员信息
          public int ScheduleMasterInfoUpdate(ScheduleMasterQuery query)
          {
              StringBuilder sql = new StringBuilder();
              query.Replace4MySQL();
              try
              {
                  sql.AppendFormat("update schedule_master set schedule_code = '{0}', schedule_name = '{1}', schedule_api = '{2}',schedule_description='{3}',schedule_state='{4}',schedule_period_id='{5}',create_user='{6}',change_user='{7}',create_time='{8}',change_time='{9}', previous_execute_time='{10}', next_execute_time='{11}' where rowid='{12}'", query.schedule_code, query.schedule_name, query.schedule_api, query.schedule_description, query.schedule_state, query.schedule_period_id, query.create_user, query.change_user, query.create_time, query.change_time, query.previous_execute_time, query.next_execute_time,query.rowid);
                  return _access.execCommand(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->ScheduleMasterInfoUpdate-->" + sql.ToString() + ex.Message);
              }
          }




          public ScheduleMasterQuery GetExeScheduleMaster(ScheduleMasterQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat("SELECT * FROM `schedule_master` WHERE schedule_code = '{0}';", query.schedule_code);

                  return _access.getSinggleObj<ScheduleMasterQuery>(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->GetExeScheduleMaster-->" + ex.Message, ex);
              }
          }
          public List<ScheduleConfigQuery> GetScheduleConfig(ScheduleConfigQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat("SELECT * FROM `schedule_config` WHERE schedule_code = '{0}';", query.schedule_code);
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
                  sql.AppendFormat(@"SELECT `rowid`, `schedule_code`, `period_type`, `period_nums`, `begin_datetime`, `current_nums`, `limit_nums`, `create_user`, `create_time`, `change_user`, `change_time` 
 FROM `schedule_period` WHERE rowid = '{0}';", query.rowid);
                  return _access.getSinggleObj<SchedulePeriodQuery>(sql.ToString());
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->GetSchedulePeriod-->" + ex.Message, ex);
              }
          }
          public List<SchedulePeriodQuery> GetSchedulePeriodBySchedule(string schedule_code)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat(@"SELECT `rowid`, `schedule_code`, `period_type`, `period_nums`,  `begin_datetime`, `current_nums`, `limit_nums`, `create_user`, `create_time`, `change_user`, `change_time` 
 FROM `schedule_period` WHERE schedule_code = '{0}';", schedule_code);
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
                  sql.AppendFormat("SELECT * FROM `schedule_config` WHERE schedule_code = '{0}';", query.schedule_code);
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
                  sql.AppendFormat(@"UPDATE `schedule_master` SET `schedule_code`='{0}', `schedule_name`='{1}', `schedule_api`='{2}', `schedule_description`='{3}', `schedule_state`='{4}', `previous_execute_time`='{5}', `next_execute_time`='{6}',`schedule_period_id`='{7}', `create_user`='{8}', `create_time`='{9}', `change_user`='{10}', `change_time`='{11}' WHERE `rowid`='{12}';", query.schedule_code, query.schedule_name, query.schedule_api, query.schedule_description, query.schedule_state, query.previous_execute_time, query.next_execute_time, query.schedule_period_id, query.create_user, query.create_time, query.change_user, Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()), query.rowid);
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
UPDATE  `schedule_period` SET `schedule_code`='{0}', `period_type`='{1}', `period_nums`='{2}', `begin_datetime`='{3}', `current_nums`='{4}', `limit_nums`='{5}', `create_user`='{6}', `create_time`='{7}', `change_user`='{8}', `change_time`='{9}' WHERE `rowid`='{10}'
;", query.schedule_code, query.period_type, query.period_nums, query.begin_datetime, query.current_nums, query.limit_nums, query.create_user, query.create_time, query.change_user, Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()), query.rowid);
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
                  sql.AppendFormat(@"INSERT INTO `schedule_log` ( `schedule_code`,  `create_user`, `create_time`, `ipfrom`) VALUES ('{0}', '{1}', '{2}', '{3}');", query.schedule_code, query.create_user, Common.CommonFunction.GetPHPTime(), query.ipfrom);
                  return _access.execCommand(sql.ToString());
              }
              catch (Exception ex)
              {
                  throw new Exception("ScheduleServiceDao-->AddScheduleLog-->" + ex.Message, ex);
              }
          }
    }
}
