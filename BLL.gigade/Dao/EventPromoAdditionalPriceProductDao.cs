using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    public class EventPromoAdditionalPriceProductDao
    {
        IDBAccess access;
        public EventPromoAdditionalPriceProductDao(string connectionStr)
        {
            access = DBFactory.getDBAccess(DBAccess.DBType.MySql, connectionStr);
        }
        public List<EventPromoAdditionalPriceProductQuery> GetList(EventPromoAdditionalPriceProductQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select ep.row_id,ep.group_id,ep.product_id,ep.add_price,eg.group_name,p.product_name  from event_promo_additional_price_product ep left join event_promo_additional_price_group eg on ep.group_id=eg.group_id");
                sql.AppendFormat(" left join product p on ep.product_id=p.product_id");
                sql.AppendFormat(" where ep.group_id={0}",query.group_id);
                return access.getDataTableForObj<EventPromoAdditionalPriceProductQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceProductDao-->GetList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<EventPromoAdditionalPriceGroup> GetPromoProductGroup(EventPromoAdditionalPriceGroup model)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select * from event_promo_additional_price_product ");
                sql.AppendFormat(" where group_id={0}", model.group_id);
                return access.getDataTableForObj<Model.EventPromoAdditionalPriceGroup>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceProductDao-->GetPromoProductGroup-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string AddOrUpdate(EventPromoAdditionalPriceProduct model)
        {
            StringBuilder sbStr = new StringBuilder();
            try
            {
                if (model.row_id == 0)
                {
                    sbStr.Append("insert into event_promo_additional_price_product(group_id,product_id,create_user,create_time,add_price,modify_user,modify_time,product_status)");
                    sbStr.AppendFormat("values({0},{1},{2},'{3}',{4},{5},'{6}',{7})", model.group_id, model.product_id, model.create_user, Common.CommonFunction.DateTimeToString(model.create_time), model.add_price, model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.product_status);
                }
                else
                {
                    sbStr.AppendFormat("update event_promo_additional_price_product set group_id={0},product_id={1},add_price={2},modify_user={3},modify_time='{4}',product_status={5} where row_id={6}", model.group_id, model.product_id, model.add_price, model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.product_status, model.row_id);
                }
                return sbStr.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceProductDao-->AddOrUpdate-->" + ex.Message + sbStr.ToString(), ex);
            }
        }


        public string Delete(string group_id, string row_ids)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;delete from event_promo_additional_price_product where group_id='{0}' and row_id in({1});set sql_safe_updates=1;",group_id,row_ids);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAdditionalPriceProductDao-->Delete" + ex.Message, ex);
            }
        }

    }
}
