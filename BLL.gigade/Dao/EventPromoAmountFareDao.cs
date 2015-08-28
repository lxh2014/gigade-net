using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using MySql.Data.MySqlClient;
namespace BLL.gigade.Dao
{
    public class EventPromoAmountFareDao : IEventPromoAmountFareImplDao
    {

        private IDBAccess _dbAccess;
        public EventPromoAmountFareDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<Model.EventPromoAmountFare> GetList(Model.EventPromoAmountFare model, out int totalCount)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sql.Append("select ef.row_id,ef.event_id,ef.event_name,ef.event_desc,ef.event_start,ef.event_end,ef.event_type,ef.site_id,ef.user_condition_id,ef.device,ef.event_status,ef.condition_type,ef.modify_user");
                sql.Append(" ,mu.user_username as user_name,ef.quantity,ef.low_quantity,ef.amount,ef.low_amount,ef.normal_quantity,ef.normal_amount,ef.free_freight_low,ef.free_freight_normal");
                sqlWhere.Append(" from event_promo_amount_fare ef ");
                sqlWhere.Append(" left join manage_user mu on mu.user_id= ef.modify_user ");
                sqlWhere.Append(" where ef.event_id !='' ");
                if (!string.IsNullOrEmpty(model.event_id))
                {
                    model.event_id = model.event_id.Replace(",", "\',\'");
                    sqlWhere.AppendFormat(" and ef.event_id in ('{0}') ", model.event_id);
                }
                if (!string.IsNullOrEmpty(model.event_name))
                {
                    sqlWhere.AppendFormat(" and ef.event_name like N'%{0}%'", model.event_name);
                }
                totalCount = 0;
                if (model.IsPage)
                {
                    DataTable _dt = _dbAccess.getDataTable(" select count(ef.event_id) as totalCount " + sqlWhere.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" order by ef.row_id desc limit {0},{1};", model.Start, model.Limit);
                }
                sql.Append(sqlWhere.ToString());
                return _dbAccess.getDataTableForObj<Model.EventPromoAmountFare>(sql.ToString());
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":EventPromoAmountFareDao-->GetList" + ex.Message, ex);
            }
            catch (Exception ex)
            {

                throw new Exception("EventPromoAmountFareDao-->GetList-->" + ex.Message, ex);
            }
        }

        public string AddOrUpdate(Model.EventPromoAmountFare model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            if (model.row_id == 0)
            {
                sql.Append(@"insert into event_promo_amount_fare(event_name,event_desc,event_start,event_end,event_type,event_id,
                             site_id,create_user,create_time,modify_user,modify_time,user_condition_id,condition_type,
                             quantity,low_quantity,amount,low_amount,normal_quantity,normal_amount,free_freight_low,free_freight_normal,
                             device,event_status)");
                sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}'", model.event_name, model.event_desc, Common.CommonFunction.DateTimeToString(model.event_start), Common.CommonFunction.DateTimeToString(model.event_end), model.event_type, model.event_id);
                sql.AppendFormat(",'{0}','{1}','{2}','{3}','{4}','{5}','{6}'", model.site_id, model.create_user, Common.CommonFunction.DateTimeToString(model.create_time), model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.user_condition_id, model.condition_type);
                sql.AppendFormat(",'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'", model.quantity, model.low_quantity, model.amount, model.low_amount, model.normal_quantity, model.normal_amount, model.free_freight_low, model.free_freight_normal);
                sql.AppendFormat(",'{0}','{1}');select @@identity;", model.device, model.event_status);
            }
            else
            {
                sql.AppendFormat("update event_promo_amount_fare set  event_name='{0}',event_desc='{1}',event_start='{2}',event_end='{3}',event_type='{4}',", model.event_name, model.event_desc, Common.CommonFunction.DateTimeToString(model.event_start), Common.CommonFunction.DateTimeToString(model.event_end), model.event_type);
                sql.AppendFormat(" site_id='{0}',modify_user='{1}',modify_time='{2}',user_condition_id='{3}',condition_type='{4}',", model.site_id, model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.user_condition_id, model.condition_type);
                sql.AppendFormat("  quantity='{0}',low_quantity='{1}',amount='{2}',low_amount='{3}',normal_quantity='{4}',normal_amount='{5}',free_freight_low='{6}',free_freight_normal='{7}',", model.quantity, model.low_quantity, model.amount, model.low_amount, model.normal_quantity, model.normal_amount, model.free_freight_low, model.free_freight_normal);
                sql.AppendFormat(" device='{0}',event_status='{1}' where row_id='{2}';", model.device, model.event_status, model.row_id);
            }
            return sql.ToString();
        }

        public string UpdateEventId(int row_id, string event_id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("set sql_safe_updates = 0;update event_promo_amount_fare set event_id ='{0}' where row_id = '{1}';set sql_safe_updates = 1;", event_id, row_id);
            return sql.ToString();
        }

        public string UpdateActive(Model.EventPromoAmountFare model)
        {
            StringBuilder sql = new StringBuilder();
            model.Replace4MySQL();
            sql.AppendFormat("set sql_safe_updates = 0;update event_promo_amount_fare set event_status ='{0}', modify_user='{1}',modify_time='{2}'  where event_id = '{3}';set sql_safe_updates = 1;", model.event_status, model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.event_id);
            return sql.ToString();
        }

    }
}
