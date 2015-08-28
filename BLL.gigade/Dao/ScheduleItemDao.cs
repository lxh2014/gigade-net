using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class ScheduleItemDao : IScheduleItemImplDao
    {
        private IDBAccess _access;
        private string conStr = "";
        public ScheduleItemDao(string connectionString)
        {
            //connectionString = "Server=127.0.0.1;database=test;user=root;pwd=198929;charset='utf8'";
            //connectionString = "Server=192.168.18.166;database=test;user=root;pwd=198929;charset='utf8'";
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            conStr = connectionString;
        }

        public string Save(ScheduleItem si)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat("INSERT INTO schedule_item(`schedule_id`,`type`,`key1`,`key2`,`key3`,`value1`,`value2`,`value3`,`item_name`)VALUES({0},{1},{2},{3},{4},'{5}','{6}','{7}','{8}');", si.schedule_Id, si.type, si.key1, si.key2, si.key3, si.value1, si.value2, si.value3,si.item_name);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemDao-->Save" + ex.Message,ex);
            }
        }

        public List<ScheduleItemCustom> Query(ScheduleItem si)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT si.id,si.item_name,si.schedule_id,s.schedule_name,si.`type`,si.`key1`,si.value1,v.vendor_name_full AS valueStr
                                  FROM schedule_item si
                                      INNER JOIN schedule s ON s.schedule_id =si.schedule_id
                                      INNER JOIN vendor v ON v.vendor_id = si.value1
                                  WHERE si.schedule_id = {0}", si.schedule_Id);
                return _access.getDataTableForObj<ScheduleItemCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemDao-->Query" + ex.Message, ex);
            }
        }

        public string Update(ScheduleItem si)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"UPDATE schedule_item SET `item_name` = '{0}',`schedule_id` = {1},`type` = {2},`key1` = {3},`value1` = '{4}' 
                                WHERE id = {5};",si.item_name,si.schedule_Id,si.type,si.key1,si.value1,si.id);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemDao-->Update" + ex.Message, ex);
            }    
        }

        public string Delete(int schedule,string ids)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SET sql_safe_updates = 0; DELETE FROM schedule_item WHERE schedule_id = {0} AND id IN ({1});SET sql_safe_updates = 1;", schedule, ids);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemDao-->Update" + ex.Message, ex);
            }    
        }

        public List<ScheduleItemCustom> QueryByCondition(ScheduleItem si)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT s.schedule_id,s.schedule_name,s.desc,si.id,si.item_name,si.schedule_id,si.type,si.key1,si.value1 FROM schedule_item si
	                                        LEFT JOIN schedule s ON s.schedule_id = si.schedule_id
                                  WHERE si.type = {0}", si.type);
                return _access.getDataTableForObj<ScheduleItemCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleItemDao-->Update" + ex.Message, ex);
            }
        }
        
    }
}
