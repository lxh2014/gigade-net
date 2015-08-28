using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{

    public class EventPromoAdditionalPriceDao
    {
        private IDBAccess access;
        public EventPromoAdditionalPriceDao(string connectionStr)
        {
            access = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<EventPromoAdditionalPriceQuery> GetList(EventPromoAdditionalPrice model, out int count)
        {
            count = 0;
            model.Replace4MySQL();
            StringBuilder sbStrAll = new StringBuilder();
            StringBuilder sbStrJoin = new StringBuilder();
            StringBuilder sbStrPage = new StringBuilder();
            StringBuilder sbWhr = new StringBuilder();
            try
            {
                sbStrAll.Append("SELECT ep.row_id,ep.modify_user,ep.event_id,ep.group_id,ep.event_name,ep.event_start,ep.event_end,mu.user_username,ep.event_desc,ep.site_id,ep.condition_type,ep.device,ep.user_condition_id,ep.quantity,ep.amount,ep.num_limit,ep.event_status ");
                sbStrJoin.Append(" FROM event_promo_additional_price AS ep LEFT JOIN manage_user mu ON ep.modify_user=mu.user_id where 1=1 ");
                if (!string.IsNullOrEmpty(model.event_id))
                {
                    model.event_id = model.event_id.Replace(",", "\',\'");
                    sbWhr.AppendFormat(" and ep.event_id in ('{0}') ", model.event_id);
                }
                if (!string.IsNullOrEmpty(model.event_name))
                {
                    sbWhr.AppendFormat(" and ep.event_name like N'%{0}%'", model.event_name);
                }
                if (model.IsPage)
                {
                    count = int.Parse(access.getDataTable("select count(ep.event_id)" + sbStrJoin.ToString() + sbWhr.ToString()).Rows[0][0].ToString());
                    sbWhr.AppendFormat(" limit {0},{1}", model.Start, model.Limit);
                }
                return access.getDataTableForObj<EventPromoAdditionalPriceQuery>(sbStrAll.ToString() + sbStrJoin.ToString() + sbWhr.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EventPromoAdditionalPriceDao-->GetList-->" + ex.Message + sbStrAll.ToString() + sbStrJoin.ToString(), ex);
            }

        }

        public string AddOrUpdate(EventPromoAdditionalPrice model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                if (model.row_id == 0)
                {
                    sql.Append(@"insert into event_promo_additional_price(event_name,event_desc,event_start,event_end,event_type,event_id,site_id,create_user,create_time,modify_user,modify_time,user_condition_id,condition_type,quantity,amount,num_limit,group_id,device,event_status)");
                    sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}'", model.event_name, model.event_desc, Common.CommonFunction.DateTimeToString(model.event_start), Common.CommonFunction.DateTimeToString(model.event_end), model.event_type, model.event_id);
                    sql.AppendFormat(",'{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},{8},{9},{10}", model.site_id, model.create_user, Common.CommonFunction.DateTimeToString(model.create_time), model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.user_condition_id, model.condition_type, model.quantity, model.amount, model.num_limit, model.group_id);
                    sql.AppendFormat(",'{0}','{1}');select @@identity;", model.device, model.event_status);
                }
                else
                {
                    sql.AppendFormat("update event_promo_additional_price set  event_name='{0}',event_desc='{1}',event_start='{2}',event_end='{3}',event_type='{4}',", model.event_name, model.event_desc, Common.CommonFunction.DateTimeToString(model.event_start), Common.CommonFunction.DateTimeToString(model.event_end), model.event_type);
                    sql.AppendFormat(" site_id='{0}',modify_user='{1}',modify_time='{2}',user_condition_id='{3}',condition_type='{4}',", model.site_id, model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.user_condition_id, model.condition_type);
                    sql.AppendFormat(" device='{0}',event_status='{1}',quantity={2},amount={3},num_limit={4},group_id={5},event_id='{6}' where row_id='{7}';", model.device, model.event_status, model.quantity, model.amount, model.num_limit, model.group_id,model.event_id, model.row_id);
                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceDao-->AddOrUpdate-->" + ex.Message, ex);
            }

        }
        public string UpdateActive(EventPromoAdditionalPrice model)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                model.Replace4MySQL();
                sql.AppendFormat("set sql_safe_updates = 0;update event_promo_additional_price set event_status ='{0}', modify_user='{1}',modify_time='{2}'  where event_id = '{3}';set sql_safe_updates = 1;", model.event_status, model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.event_id);

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceDao-->UpdateActive-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
