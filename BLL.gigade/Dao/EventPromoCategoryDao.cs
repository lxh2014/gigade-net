using BLL.gigade.Dao.Impl;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EventPromoCategoryDao : IEventPromoCategoryImplDao
    {
        private IDBAccess _dbAccess;
        public EventPromoCategoryDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<Model.EventPromoCategory> GetList(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select epc.row_id, epc.event_type,epc.site_id,epc.category_id,epc.event_id,pc.category_name");
                sql.Append(",epc.event_status,epc.create_user,epc.create_time,epc.modify_user,epc.modify_time,epc.event_start,epc.event_end");
                sql.Append(" from event_promo_category epc inner join product_category pc on pc.category_id=epc.category_id");
                sql.AppendFormat(" where epc.event_id='{0}';", event_id);
                return _dbAccess.getDataTableForObj<Model.EventPromoCategory>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoCategoryDao-->GetList" + ex.Message + sql.ToString(), ex);
            }
        }


        public string AddOrUpdate(Model.EventPromoCategory epQuery)
        {
            epQuery.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                if (epQuery.row_id == 0)
                {
                    sql.Append("insert into event_promo_category(event_type,site_id,category_id,event_id");
                    sql.Append(",event_status,create_user,create_time,modify_user,modify_time,event_start,event_end)");
                    sql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}',", epQuery.event_type, epQuery.site_id, epQuery.category_id, epQuery.event_id, epQuery.event_status);
                    sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", epQuery.create_user, Common.CommonFunction.DateTimeToString(epQuery.create_time), epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), Common.CommonFunction.DateTimeToString(epQuery.event_start));
                    sql.AppendFormat("'{0}');", Common.CommonFunction.DateTimeToString(epQuery.event_end));

                }
                else
                {
                    sql.AppendFormat(" update event_promo_category set event_type='{0}',site_id='{1}',category_id='{2}',event_id='{3}',event_status='{4}'", epQuery.event_type, epQuery.site_id, epQuery.category_id, epQuery.event_id, epQuery.event_status);
                    sql.AppendFormat(" ,modify_user='{0}',modify_time='{1}',event_start='{2}'", epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), Common.CommonFunction.DateTimeToString(epQuery.event_start));
                    sql.AppendFormat(" ,event_end='{0}' where row_id='{1}';", Common.CommonFunction.DateTimeToString(epQuery.event_end), epQuery.row_id);

                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoCategoryDao-->AddOrUpdate" + ex.Message, ex);
            }
        }

        public string Delete(string event_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;delete from event_promo_category where event_id='{0}';set sql_safe_updates = 1;", event_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoCategoryDao-->Delete" + ex.Message, ex);
            }
        }

        public string UpdateActive(Model.EventPromoCategory epQuery)
        {
            epQuery.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;update event_promo_category set event_status ='{0}', modify_user='{1}',modify_time='{2}'  where event_id = '{3}';set sql_safe_updates = 1;", epQuery.event_status, epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), epQuery.event_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoCategoryDao-->UpdateActive" + ex.Message, ex);
            }
        }

    }
}
