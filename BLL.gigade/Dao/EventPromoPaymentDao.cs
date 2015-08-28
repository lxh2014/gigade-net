using BLL.gigade.Dao.Impl;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EventPromoPaymentDao : IEventPromoPaymentImplDao
    {
        private IDBAccess _dbAccess;
        public EventPromoPaymentDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<Model.EventPromoPayment> GetList(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select row_id, event_type,site_id,payment_id,event_id");
                sql.Append(",event_status,create_user,create_time,modify_user,modify_time,event_start,event_end");
                sql.AppendFormat(" from event_promo_payment where event_id='{0}';", event_id);
                return _dbAccess.getDataTableForObj<Model.EventPromoPayment>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoPaymentDao-->GetList" + ex.Message + sql.ToString(), ex);
            }
        }

        public string AddOrUpdate(Model.EventPromoPayment epQuery)
        {
            epQuery.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                if (epQuery.row_id == 0)
                {
                    sql.Append("insert into event_promo_payment(event_type,site_id,payment_id,event_id");
                    sql.Append(",event_status,create_user,create_time,modify_user,modify_time,event_start,event_end)");
                    sql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}',", epQuery.event_type, epQuery.site_id, epQuery.payment_id, epQuery.event_id, epQuery.event_status);
                    sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", epQuery.create_user, Common.CommonFunction.DateTimeToString(epQuery.create_time), epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), Common.CommonFunction.DateTimeToString(epQuery.event_start));
                    sql.AppendFormat("'{0}');", Common.CommonFunction.DateTimeToString(epQuery.event_end));

                }
                else
                {
                    sql.AppendFormat(" update event_promo_payment set event_type='{0}',site_id='{1}',payment_id='{2}',event_id='{3}',event_status='{4}'", epQuery.event_type, epQuery.site_id, epQuery.payment_id, epQuery.event_id, epQuery.event_status);
                    sql.AppendFormat(" ,modify_user='{0}',modify_time='{1}',event_start='{2}'", epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), Common.CommonFunction.DateTimeToString(epQuery.event_start));
                    sql.AppendFormat(" ,event_end='{0}' where row_id='{1}';", Common.CommonFunction.DateTimeToString(epQuery.event_end), epQuery.row_id);

                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoPaymentDao-->AddOrUpdate" + ex.Message, ex);
            }
        }

        public string Delete(string event_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;delete from event_promo_payment where event_id='{0}';set sql_safe_updates = 1;", event_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoPaymentDao-->Delete" + ex.Message, ex);
            }
        }

        public string UpdateActive(Model.EventPromoPayment epQuery)
        {
            epQuery.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;update event_promo_payment set event_status ='{0}', modify_user='{1}',modify_time='{2}'  where event_id = '{3}';set sql_safe_updates = 1;", epQuery.event_status, epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), epQuery.event_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoPaymentDao-->UpdateActive" + ex.Message, ex);
            }
        }
    }
}
