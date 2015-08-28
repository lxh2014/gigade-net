using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class ScheduleRelationDao : IScheduleRelationImplDao
    {
        private IDBAccess _access;
        private string connectionString;
        public ScheduleRelationDao(string connectionString)
        {
            //connectionString = "Server=127.0.0.1;database=test;user=root;pwd=198929;charset='utf8'";
            this.connectionString = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public int Delete(string relation,int relation_id)
        {
            try
            {
             return  _access.execCommand(string.Format("SET sql_safe_updates = 0;DELETE FROM schedule_relation WHERE relation_table='{0}' and  relation_id = {1};SET sql_safe_updates = 1;",relation, relation_id));
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleRelationDao-->Delete" + ex.Message, ex);
            }
        }

        public string Delete(string relation, string relation_id,int schedule_id)
        {
            try
            {
                return string.Format(@"SET sql_safe_updates = 0;
                                           DELETE FROM schedule_relation 
                                       WHERE relation_table='{0}' AND relation_id IN ({1}) AND schedule_id = {2};
                                       SET sql_safe_updates = 1;", relation, relation_id,schedule_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleRelationDao-->Delete" + ex.Message, ex);
            }
        }

        public List<ScheduleRelation> Query(int schedule_id)
        {
            var scheRel = _access.getDataTableForObj<ScheduleRelation>(string.Format("SELECT  rowid,schedule_id,relation_table,relation_id  FROM schedule_relation WHERE schedule_id = {0}", schedule_id));
            return scheRel;
        }

        public List<ScheduleRelation> Query(ScheduleRelation sr)
        {
            try
            {

                var scheRel = _access.getDataTableForObj<ScheduleRelation>(string.Format("SELECT  rowid,schedule_id,relation_table,relation_id  FROM schedule_relation WHERE relation_id={0} AND relation_table='{1}';", sr.relation_id, sr.relation_table));
                foreach (var item in scheRel)
                {
                    item.schedule = new ScheduleDao(connectionString).Query(new Model.Query.ScheduleQuery { schedule_id = item.schedule_id }).FirstOrDefault();
                }
                return scheRel;
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleRelationDao-->Query" + ex.Message, ex);
            }
        }

        public int Save(ScheduleRelation sr)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"INSERT INTO schedule_relation(`schedule_id`,`relation_table`,`relation_id`)VALUES({0},'{1}',{2});",sr.schedule_id,sr.relation_table,sr.relation_id);
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleRelationDao-->Save" + ex.Message,ex);
            }
        }

        public int Update(ScheduleRelation sr)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"set sql_safe_updates=0;update schedule_relation set`schedule_id`={0} where `relation_table`='{1}' AND `relation_id`={2};set sql_safe_updates=1;", sr.schedule_id, sr.relation_table, sr.relation_id); ///add by wwei0216w 2015/3/30 修改原因 代碼執行至該處出現異常 在'{1}'后將','改為'AND'
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ScheduleRelationDao-->Save" + ex.Message, ex);
            }
        }
    }
}
