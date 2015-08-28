using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EventPromoProductDao : IEventPromoProductImplDao
    {
        private IDBAccess _dbAccess;
        public EventPromoProductDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<Model.EventPromoProduct> GetList(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select e.row_id,e.event_type,e.site_id,e.product_id,p.product_name,e.product_num_limit,e.event_id");
                sql.Append(",e.event_status,e.create_user,e.create_time,e.modify_user,e.modify_time,e.event_start,e.event_end");
                sql.AppendFormat(" from event_promo_product e inner join product p on p.product_id=e.product_id where event_id='{0}';", event_id);
                return _dbAccess.getDataTableForObj<Model.EventPromoProduct>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoProductDao-->GetList" + ex.Message + sql.ToString(), ex);
            }
        }


        public string AddOrUpdate(Model.EventPromoProduct epQuery)
        {
            epQuery.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                if (epQuery.row_id == 0)
                {
                    sql.Append("insert into event_promo_product(event_type,site_id,product_id,product_num_limit,event_id");
                    sql.Append(",event_status,create_user,create_time,modify_user,modify_time,event_start,event_end)");
                    sql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}','{5}',", epQuery.event_type, epQuery.site_id, epQuery.product_id, epQuery.product_num_limit, epQuery.event_id, epQuery.event_status);
                    sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", epQuery.create_user, Common.CommonFunction.DateTimeToString(epQuery.create_time), epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), Common.CommonFunction.DateTimeToString(epQuery.event_start));
                    sql.AppendFormat("'{0}');", Common.CommonFunction.DateTimeToString(epQuery.event_end));

                }
                else
                {
                    sql.AppendFormat(" update event_promo_product set event_type='{0}',site_id='{1}',product_id='{2}',event_id='{3}',event_status='{4}',product_num_limit='{5}'", epQuery.event_type, epQuery.site_id, epQuery.product_id, epQuery.event_id, epQuery.event_status, epQuery.product_num_limit);
                    sql.AppendFormat(" ,modify_user='{0}',modify_time='{1}',event_start='{2}'", epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), Common.CommonFunction.DateTimeToString(epQuery.event_start));
                    sql.AppendFormat(" ,event_end='{0}' where row_id='{1}';", Common.CommonFunction.DateTimeToString(epQuery.event_end), epQuery.row_id);

                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoProductDao-->AddOrUpdate" + ex.Message, ex);
            }
        }

        public string Delete(string event_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;delete from event_promo_product where event_id='{0}';set sql_safe_updates = 1;", event_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoProductDao-->Delete" + ex.Message, ex);
            }
        }

        public string UpdateActive(Model.EventPromoProduct epQuery)
        {
            epQuery.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;update event_promo_product set event_status ='{0}', modify_user='{1}',modify_time='{2}'  where event_id = '{3}';set sql_safe_updates = 1;", epQuery.event_status, epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), epQuery.event_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoProductDao-->UpdateActive" + ex.Message, ex);
            }
        }
    }
}
