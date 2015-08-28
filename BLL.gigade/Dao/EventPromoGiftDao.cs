using BLL.gigade.Dao.Impl;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EventPromoGiftDao : IEventPromoGiftImplDao
    {
        private IDBAccess _dbAccess;
        public EventPromoGiftDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<Model.Query.EventPromoGiftQuery> GetList(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select gift_id,gift_name,product_id,product_num,bonus,bonus_multiple,welfare,welfare_multiple");
                sql.Append(",gift_type,quantity,amount,event_id,create_user,create_time,modify_user,modify_time");
                sql.AppendFormat(" from event_promo_gift where event_id='{0}'", event_id);
                return _dbAccess.getDataTableForObj<Model.Query.EventPromoGiftQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoGiftDao-->GetList-->" + ex.Message + sql.ToString(), ex);
            }
        }



        public string AddOrUpdate(Model.Query.EventPromoGiftQuery epQuery)
        {
            StringBuilder sql = new StringBuilder();
            epQuery.Replace4MySQL();
            try
            {
                if (epQuery.gift_id == 0)
                {
                    sql.Append("insert into event_promo_gift(gift_name,product_id,product_num,bonus,bonus_multiple,welfare,welfare_multiple");
                    sql.Append(",gift_type,quantity,amount,event_id,create_user,create_time,modify_user,modify_time)");
                    sql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}',", epQuery.gift_name, epQuery.product_id, epQuery.product_num, epQuery.bonus, epQuery.bonus_multiple);
                    sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", epQuery.welfare, epQuery.welfare_multiple, epQuery.gift_type, epQuery.quantity, epQuery.amount);
                    sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}');", epQuery.event_id, epQuery.create_user, Common.CommonFunction.DateTimeToString(epQuery.create_time), epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time));

                }
                else
                {
                    sql.AppendFormat(" update event_promo_gift set gift_name='{0}',product_id='{1}',product_num='{2}',bonus='{3}',bonus_multiple='{4}'", epQuery.gift_name, epQuery.product_id, epQuery.product_num, epQuery.bonus, epQuery.bonus_multiple);
                    sql.AppendFormat(" ,welfare='{0}',welfare_multiple='{1}',gift_type='{2}',quantity='{3}',amount='{4}'", epQuery.welfare, epQuery.welfare_multiple, epQuery.gift_type, epQuery.quantity, epQuery.amount);
                    sql.AppendFormat(" ,event_id='{0}',modify_user='{1}',modify_time='{2}' where gift_id='{3}';", epQuery.event_id, epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), epQuery.gift_id);

                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoGiftDao-->AddOrUpdate-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public string Delete(string event_id, string gift_ids)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("delete from event_promo_gift where event_id='{0}' and gift_id in({1});", event_id, gift_ids);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoGiftDao-->Delete" + ex.Message, ex);
            }
        }

    }
}
