using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Custom;
using System.Collections;

namespace BLL.gigade.Dao
{
    public class ScheduleDao : IScheduleImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr = "";
        public ScheduleDao(string connectionStr)
        {
            //connectionStr = "Server=192.168.18.166;database=test;user=root;pwd=198929;charset='utf8'";
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            connStr = connectionStr;
        }

        //add by wwei0216 2015/2/9
        /// <summary>
        /// 保存Freight排程的執行信息
        /// </summary>
        /// <param name="fst">FreightSetTime對象</param>
        /// <returns>執行后受影響的行數</returns>
        public int Save(Schedule s)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"INSERT INTO `schedule`  (schedule_name,`type`,`execute_type`,`day_type`,`month_type`,`date_value`,`repeat_count`,
                `repeat_hours`,`time_type`,`week_day`,`start_time`,`end_time`,`duration_start`,`duration_end`,`desc`,create_user,create_date,trigger_time,execute_days)
                VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13},'{14}','{15}','{16}','{17}','{18}');",
                 s.schedule_name, s.type, s.execute_type, s.day_type, s.month_type, s.date_value, s.repeat_count, s.repeat_hours, s.time_type,
                 s.week_day, s.start_time.ToString("HH:mm:ss"), s.end_time.ToString("HH:mm:ss"),
                 s.duration_start.ToString("yyyy-MM-dd"),
                 s.duration_end == DateTime.MinValue ? "null" : "'" + s.duration_end.ToString("yyyy-MM-dd") + "'",
                 s.desc, s.create_user, s.create_date.ToString("yyyy-MM-dd HH:mm:ss"),s.trigger_time,s.execute_days);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleDao-->Save" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public int Update(Schedule schedule)
        {
            string sqlStr = string.Format(@"update `schedule` set  `type`='{0}',`execute_type`='{1}',`day_type`='{2}',`month_type`='{3}',`date_value`='{4}',
                `repeat_count`='{5}',`repeat_hours`='{6}',`time_type`='{7}',`week_day`='{8}',
                `start_time`='{9}',`end_time`='{10}',`duration_start`='{11}',`duration_end`={12},schedule_name='{13}',
                `desc`='{14}',create_user={15},create_date='{16}',trigger_time='{17}',execute_days='{18}'  WHERE schedule_id = {19}",
                schedule.type, schedule.execute_type, schedule.day_type, schedule.month_type, schedule.date_value,
                schedule.repeat_count, schedule.repeat_hours,
                schedule.time_type, schedule.week_day, schedule.start_time.ToString("HH:mm:ss"), schedule.end_time.ToString("HH:mm:ss"),
                schedule.duration_start.ToString("yyyy-MM-dd"),
                schedule.duration_end == DateTime.MinValue ? "null" : "'" + schedule.duration_end.ToString("yyyy-MM-dd") + "'",
                schedule.schedule_name, schedule.desc, schedule.create_user,
                schedule.create_date.ToString("yyyy-MM-dd HH:mm:ss"),schedule.trigger_time,schedule.execute_days,schedule.schedule_id);

            return _dbAccess.execCommand(sqlStr);
        }

        //add by wwei0216 2015/2/10
        /// <summary>
        /// 查詢排成的執行信息
        /// </summary>
        /// <param name="columns">需要查找的列名</param>
        /// <returns>List集合</returns>
        public List<Schedule> Query(Model.Query.ScheduleQuery schedule)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"SELECT s.schedule_name,s.schedule_id,s.TYPE,s.execute_type,s.month_type,s.day_type,s.date_value,s.time_type,s.repeat_count,s.repeat_hours,s.week_day,
                    s.start_time,s.end_time,s.duration_start,s.duration_end,s.`desc`,m.user_username AS create_user_name, create_date,trigger_time,execute_days 
                    FROM `schedule` s 
                LEFT JOIN manage_user m ON s.create_user=m.user_id  
            WHERE 1=1");
                if (schedule.schedule_id != 0)
                {
                    sb.AppendFormat(" and schedule_id = {0} ", schedule.schedule_id);
                }
                //add by zhuoqin0830w  2015/09/09  排程查詢添加 名稱 查詢條件
                if (!string.IsNullOrEmpty(schedule.schedule_name))
                {
                    sb.AppendFormat(" and schedule_name like '%{0}%' ", schedule.schedule_name);
                }
                if (schedule.SearchType != 0)
                {
                    switch (schedule.SearchType)
                    {
                        case 1:
                            sb.Append(" and  create_date ");
                            break;
                        case 2:
                            sb.Append(" and  duration_start ");
                            break;
                        //sb.Append(" and  duration_end "); ///add by wwei0216w 2015/3/30 修改原因:看不懂該段代碼,且按照結束時間查詢結果集不正確,將該段代碼移至case 3處
                        case 3:
                            sb.Append(" and  duration_end ");
                            break;

                    }
                    sb.AppendFormat("  between '{0}' and '{1}'", schedule.Start.ToString("yyyy-MM-dd"), schedule.End.ToString("yyyy-MM-dd"));
                }

                return _dbAccess.getDataTableForObj<Schedule>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleDao-->" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2015/2/25
        /// <summary>
        /// 根據商品Id刪除排程
        /// </summary>
        /// <param name="Schedule">Schedule</param>
        /// <returns>受影響的行數</returns>
        public int Delete(Schedule s)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SET sql_safe_updates = 0;DELETE FROM schedule WHERE schedule_id ={0};SET sql_safe_updates = 1;", s.schedule_id);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleDao-->Delete" + ex.Message, ex);
            }
        }

        public int Delete(int[] ids)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SET sql_safe_updates = 0;DELETE FROM schedule WHERE schedule_id in ({0});SET sql_safe_updates = 1;", string.Join(",", ids));
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleDao-->Delete" + ex.Message, ex);
            }
        }
    }
}
