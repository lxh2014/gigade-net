using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
   public class EventChinatrustDao
    {
         private IDBAccess _access;
         public EventChinatrustDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
       /// <summary>
       /// 中信-一點一元列表頁
       /// </summary>
       /// <param name="query"></param>
       /// <param name="totalCount"></param>
       /// <returns></returns>
         public List<EventChinatrustQuery> GetEventChinatrustList(EventChinatrustQuery query, out int totalCount)
         {
             List<EventChinatrustQuery> list = new List<EventChinatrustQuery>();
             StringBuilder sbSql = new StringBuilder();
             StringBuilder sbSqlCount = new StringBuilder();
             StringBuilder sbSqlCondition = new StringBuilder();

             try
             {
                 sbSql.Append("select ec.row_id,ec.event_type,ec.event_id,ec.event_name,ec.event_desc,ec.event_banner,ec.event_start_time,ec.event_end_time,ec.event_active ");
                 sbSql.Append(",ec.event_create_user,ec.event_update_user,ec.event_create_time,ec.event_update_time,ec.user_register_time, mu.user_username as create_name,mu2.user_username as update_name ");
                 sbSqlCondition.Append(" from event_chinatrust ec ");
                 sbSqlCondition.Append(" left join manage_user mu on mu.user_id=ec.event_create_user ");
                 sbSqlCondition.Append(" left join manage_user mu2 on mu2.user_id=ec.event_update_user ");
                 sbSqlCount.Append("select count(ec.row_id) as totalCount ");
                 sbSqlCondition.Append(" where 1=1 ");
                 if (query.row_id!=0)
                 {
                     sbSqlCondition.AppendFormat(" and ec.row_id= '{0}' ", query.row_id);
                 }
                 if (query.event_active != -1)
                 {
                     sbSqlCondition.AppendFormat(" and ec.event_active ='{0}' ", query.event_active);
                 }
                 if (!string.IsNullOrEmpty(query.event_id_name))
                 {
                     sbSqlCondition.AppendFormat(" and(ec.event_id like N'%{0}%' or ec.event_name like N'%{0}%') ", query.event_id_name);
                 }
                 if (query.event_start_time>DateTime.MinValue)
                 {
                     sbSqlCondition.AppendFormat(" and ec.event_start_time >='{0}' ", query.event_start_time.ToString("yyyy-MM-dd 00:00:00"));
                 }
                 if (query.event_end_time>DateTime.MinValue)
                 {
                     sbSqlCondition.AppendFormat(" and ec.event_end_time <='{0}' ", query.event_end_time.ToString("yyyy-MM-dd 23:59:59"));
                 }
                 totalCount = 0;
                 if (query.IsPage)
                 {
                     sbSqlCount.Append(sbSqlCondition.ToString());
                     DataTable _dt = _access.getDataTable(sbSqlCount.ToString());
                     if (_dt.Rows.Count > 0)
                     {
                         totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                     }
                     sbSqlCondition.Append(" order by ec.row_id desc ");
                     sbSqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                 }
                 else
                 {
                     sbSqlCondition.Append(" order by ec.row_id desc ");
                 }
                 sbSql.Append(sbSqlCondition.ToString());
                 list = _access.getDataTableForObj<EventChinatrustQuery>(sbSql.ToString());
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinatrustDao-->GetEventChinatrustList-->" + ex.Message + sbSql.ToString()+sbSqlCount.ToString(), ex);
             }
             return list;
         }
       /// <summary>
       /// 新增中信一點一元
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
         public int AddEventChinatrust(EventChinatrustQuery model)
         {
             StringBuilder sbSql = new StringBuilder();
             model.Replace4MySQL();
             sbSql.Append("insert into event_chinatrust(event_type,event_id,event_name,event_desc,event_banner,event_start_time,event_end_time,event_create_user,event_update_user,event_create_time,event_update_time,user_register_time)values(");
             sbSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}'", model.event_type, model.event_id, model.event_name, model.event_desc, model.event_banner, CommonFunction.DateTimeToString(model.event_start_time));
             sbSql.AppendFormat(",'{0}','{1}','{2}'", CommonFunction.DateTimeToString(model.event_end_time), model.event_create_user, model.event_update_user);
             sbSql.AppendFormat(",'{0}','{1}','{2}');", CommonFunction.DateTimeToString(model.event_create_time), CommonFunction.DateTimeToString(model.event_update_time), CommonFunction.DateTimeToString(model.user_register_time));
             sbSql.Append("SELECT @@IDENTITY;");
             try
             {
                 DataTable _dt = _access.getDataTable(sbSql.ToString());
                 if (_dt.Rows.Count > 0)
                 {
                     sbSql.Clear();
                     model.row_id = Convert.ToInt32(_dt.Rows[0][0]);
                     model.event_id = model.event_type;
                     for (int i = 0; i < 6 - (model.row_id.ToString().Length); i++)
                     {
                         model.event_id = model.event_id + "0";
                     }
                     model.event_id = model.event_id + model.row_id;
                     model.Replace4MySQL();
                     sbSql.Append("set sql_safe_updates=0;");
                     sbSql.AppendFormat("update event_chinatrust set event_id='{0}' where row_id='{1}';", model.event_id, model.row_id);
                     sbSql.Append("set sql_safe_updates=1;");
                     return _access.execCommand(sbSql.ToString());
                 }
                 else
                 {
                     return 0;
                 }
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinatrustDao-->AddEventChinatrust-->" + ex.Message + sbSql.ToString(), ex);
             }
         }
       /// <summary>
       /// 編輯-中信一點一元
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
         public int UpdateEventChinatrust(EventChinatrustQuery model)
         {
             int result = 0;
             StringBuilder sbSql = new StringBuilder();
             model.Replace4MySQL();
             sbSql.Append("set sql_safe_updates=0;");
             sbSql.AppendFormat("update event_chinatrust set event_type='{0}',event_name='{1}',event_desc='{2}',event_banner='{3}'",model.event_type,model.event_name,model.event_desc,model.event_banner);
             sbSql.AppendFormat(",event_start_time='{0}',event_end_time='{1}' ", CommonFunction.DateTimeToString(model.event_start_time), CommonFunction.DateTimeToString(model.event_end_time));
             sbSql.AppendFormat(",event_update_user='{0}',event_update_time='{1}',user_register_time='{2}' ", model.event_update_user, CommonFunction.DateTimeToString(model.event_update_time), CommonFunction.DateTimeToString(model.user_register_time));
             sbSql.AppendFormat("  where row_id='{0}';",model.row_id);
             sbSql.Append("set sql_safe_updates=1;");
             try
             {
                     result = _access.execCommand(sbSql.ToString());
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinatrustDao-->UpdateEventChinatrust-->" + ex.Message + sbSql.ToString(), ex);
             }
             return result;
         }
       /// <summary>
       /// 修改-中信一點一元狀態
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
         public int UpEventChinatrustStatus(EventChinatrustQuery query)
         {
             int result = 0;
             StringBuilder sql = new StringBuilder();
             query.Replace4MySQL();
             try
             {
                 sql.Append("set sql_safe_updates=0;");
                 sql.AppendFormat("update event_chinatrust set event_active='{0}', event_update_time='{1}'", query.event_active, CommonFunction.DateTimeToString(query.event_update_time));
                 sql.AppendFormat(" ,event_update_user='{0}' where row_id='{1}'; ", query.event_update_user, query.row_id);
                 sql.Append("set sql_safe_updates=1;");
                 result = _access.execCommand(sql.ToString());
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinatrustDao.UpEventChinatrustStatus-->" + ex.Message + sql.ToString(), ex);
             }
             return result;
         }

         public List<EventChinatrustQuery> GetChinaTrustStore()
         {
             StringBuilder sql = new StringBuilder();
             try
             {
                 sql.Append("select event_id,CONCAT(row_id,'-',event_name) as event_name from event_chinatrust; ");
                 return _access.getDataTableForObj<EventChinatrustQuery>(sql.ToString());
             }
             catch (Exception ex)
             {
                 throw new Exception("EventChinatrustDao.GetChinaTrustStore-->" + ex.Message + sql.ToString(), ex);
             }
         }
         
    }
}
