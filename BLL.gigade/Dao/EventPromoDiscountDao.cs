using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
namespace BLL.gigade.Dao
{
    public class EventPromoDiscountDao:IEventPromoDiscountImplDao
    {

        DBAccess.IDBAccess access;
        public EventPromoDiscountDao(string connectionStr)
        {
            access = DBAccess.DBFactory.getDBAccess(DBAccess.DBType.MySql, connectionStr);
        }
        public List<Model.EventPromoDiscount> GetList(string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select discount_name,discount,quantity,amount,discount_id,");
                sql.Append("event_id,create_user,create_time,modify_user,modify_time");
                sql.AppendFormat(" from event_promo_discount where event_id='{0}'", event_id);
                return access.getDataTableForObj<Model.EventPromoDiscount>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoDiscountDao-->GetList-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string AddOrUpdate(Model.EventPromoDiscount model)
        {
            StringBuilder sql = new StringBuilder();
            model.Replace4MySQL();
            try
            {
                if (model.discount_id == 0)
                {
                    sql.Append("insert into event_promo_discount(discount_name,discount,quantity,amount,event_id,create_user,create_time,modify_user,modify_time)");
                    sql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}',",model.discount_name,model.discount,model.quantity,model.amount,model.event_id);
                    sql.AppendFormat("'{0}','{1}','{2}','{3}');", model.create_user, Common.CommonFunction.DateTimeToString(model.create_time), model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time));
                }
                else
                {
                    sql.AppendFormat(" update event_promo_discount set discount_name='{0}',discount='{1}',quantity='{2}',amount='{3}',event_id='{4}'", model.discount_name, model.discount, model.quantity, model.amount, model.event_id);
                    sql.AppendFormat(" ,create_user='{0}',create_time='{1}',modify_user='{2}',modify_time='{3}' where discount_id='{4}';", model.create_user, Common.CommonFunction.DateTimeToString(model.create_time), model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time),model.discount_id);
                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoDiscountDao-->AddOrUpdate-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string Delete(string event_id, string discount_ids)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("delete from event_promo_discount where event_id='{0}' and discount_id in({1});", event_id, discount_ids);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoDiscountDao-->Delete" + ex.Message, ex);
            }
        }
    }
}
