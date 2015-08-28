using BLL.gigade.Dao.Impl;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Dao
{
    public class EventPromoAmountGiftDao : IEventPromoAmountGiftImplDao
    {
        private IDBAccess _dbAccess;
        public EventPromoAmountGiftDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<Model.Query.EventPromoAmountGiftQuery> GetList(Model.Query.EventPromoAmountGiftQuery epQuery, out int totalCount)
        {
            epQuery.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sql.Append("select eg.row_id,eg.event_id,eg.event_name,eg.event_desc,eg.event_start,eg.event_end,eg.event_type,eg.site_id,eg.user_condition_id,eg.device,eg.event_status,eg.condition_type,eg.modify_user");
                sql.Append(" ,mu.user_username as user_name");
                sqlWhere.Append(" from event_promo_amount_gift eg ");
                sqlWhere.Append(" left join manage_user mu on mu.user_id= eg.modify_user ");
                sqlWhere.Append(" where eg.event_id !='' ");
                if (!string.IsNullOrEmpty(epQuery.event_id))
                {
                    epQuery.event_id = epQuery.event_id.Replace(",", "\',\'");
                    sqlWhere.AppendFormat(" and eg.event_id in ('{0}') ", epQuery.event_id);
                }
                if (!string.IsNullOrEmpty(epQuery.event_name))
                {
                    sqlWhere.AppendFormat(" and eg.event_name like N'%{0}%'", epQuery.event_name);
                }
                totalCount = 0;
                if (epQuery.IsPage)
                {
                    DataTable _dt = _dbAccess.getDataTable(" select count(eg.event_id) as totalCount " + sqlWhere.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat(" order by eg.row_id desc limit {0},{1};", epQuery.Start, epQuery.Limit);
                }
                sql.Append(sqlWhere.ToString());
                return _dbAccess.getDataTableForObj<Model.Query.EventPromoAmountGiftQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAmountGiftDao-->GetList-->" + ex.Message, ex);
            }
        }

        public string AddOrUpdate(Model.EventPromoAmountGift model)
        {
            model.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                if (model.row_id == 0)
                {
                    sql.Append(@"insert into event_promo_amount_gift(event_name,event_desc,event_start,event_end,event_type,event_id,
                             site_id,create_user,create_time,modify_user,modify_time,user_condition_id,condition_type,
                             device,event_status)");
                    sql.AppendFormat(" values('{0}','{1}','{2}','{3}','{4}','{5}'", model.event_name, model.event_desc, Common.CommonFunction.DateTimeToString(model.event_start), Common.CommonFunction.DateTimeToString(model.event_end), model.event_type, model.event_id);
                    sql.AppendFormat(",'{0}','{1}','{2}','{3}','{4}','{5}','{6}'", model.site_id, model.create_user, Common.CommonFunction.DateTimeToString(model.create_time), model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.user_condition_id, model.condition_type);
                    sql.AppendFormat(",'{0}','{1}');select @@identity;", model.device, model.event_status);
                }
                else
                {
                    sql.AppendFormat("update event_promo_amount_gift set  event_name='{0}',event_desc='{1}',event_start='{2}',event_end='{3}',event_type='{4}',", model.event_name, model.event_desc, Common.CommonFunction.DateTimeToString(model.event_start), Common.CommonFunction.DateTimeToString(model.event_end), model.event_type);
                    sql.AppendFormat(" site_id='{0}',modify_user='{1}',modify_time='{2}',user_condition_id='{3}',condition_type='{4}',", model.site_id, model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.user_condition_id, model.condition_type);
                    sql.AppendFormat(" device='{0}',event_status='{1}' where row_id='{2}';", model.device, model.event_status, model.row_id);
                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAmountGiftMgr-->AddOrUpdate-->" + ex.Message, ex);
            }

        }

        public string UpdateEventId(int row_id, string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;update event_promo_amount_gift set event_id ='{0}' where row_id = '{1}';set sql_safe_updates = 1;", event_id, row_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAmountGiftMgr-->UpdateEventId-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdateActive(Model.EventPromoAmountGift model)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                model.Replace4MySQL();
                sql.AppendFormat("set sql_safe_updates = 0;update event_promo_amount_gift set event_status ='{0}', modify_user='{1}',modify_time='{2}'  where event_id = '{3}';set sql_safe_updates = 1;", model.event_status, model.modify_user, Common.CommonFunction.DateTimeToString(model.modify_time), model.event_id);

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("EventPromoAmountGiftMgr-->UpdateActive-->" + ex.Message + sql.ToString(), ex);
            }
        }

    }
}
