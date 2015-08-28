using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{

    public class EventPromoAdditionalPriceGroupDao
    {
        IDBAccess access;
        public EventPromoAdditionalPriceGroupDao(string connectionStr)
        {
            access = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public List<EventPromoAdditionalPriceGroupQuery> GetList(EventPromoAdditionalPriceGroup model)
        {
            StringBuilder sbAll = new StringBuilder();
            try
            {
                sbAll.Append("SELECT eg.group_id,mu.user_username,eg.group_name,eg.group_status,eg.create_user,eg.create_date,eg.modify_user,eg.modify_time FROM event_promo_additional_price_group  AS eg ");
                sbAll.Append("left join manage_user mu on eg.create_user=mu.user_id ");
                if(model.group_name!=string.Empty)
                {
                    sbAll.AppendFormat(" where eg.group_name='{0}'",model.group_name);
                }
                return access.getDataTableForObj<EventPromoAdditionalPriceGroupQuery>(sbAll.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceGroupDao-->GetList-->" + ex.Message+sbAll.ToString(), ex);
            }
        }
        public int InsertModel(EventPromoAdditionalPriceGroup model)
        {
            StringBuilder sbAll = new StringBuilder();
            try
            {
                sbAll.Append("insert into event_promo_additional_price_group(group_name,create_user,create_date,modify_user,modify_time,group_status)");
                sbAll.AppendFormat("values('{0}',{1},'{2}',{3},'{4}',{5})",model.group_name,model.create_user,Common.CommonFunction.DateTimeToString(model.create_date),model.modify_user,Common.CommonFunction.DateTimeToString(model.modify_time),model.group_status);
                return access.execCommand(sbAll.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceGroupDao-->InsertModel-->" + ex.Message + sbAll.ToString(), ex);
            }
        }
        public int UpdateModel(EventPromoAdditionalPriceGroup model)
        {
            StringBuilder sbAll = new StringBuilder();
            try
            {
                sbAll.AppendFormat("update event_promo_additional_price_group set group_name='{0}',group_status={1},modify_user={2},modify_time='{3}' where group_id={4}",model.group_name,model.group_status,model.modify_user,Common.CommonFunction.DateTimeToString(model.modify_time),model.group_id);
                return access.execCommand(sbAll.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("EventPromoAdditionalPriceGroupDao-->UpdateModel-->" + ex.Message + sbAll.ToString(), ex);
            }
        }

        public int DeleteModel(EventPromoAdditionalPriceGroupQuery query)
        {
            StringBuilder sbAll = new StringBuilder();
            try
            {
                sbAll.AppendFormat("set sql_safe_updates=0;delete from event_promo_additional_price_group where group_id in({0});set sql_safe_updates=1;",query.group_ids);
                return access.execCommand(sbAll.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceGroupDao-->DeleteModel-->" + ex.Message + sbAll.ToString(), ex);
            }
        }
    }
}
