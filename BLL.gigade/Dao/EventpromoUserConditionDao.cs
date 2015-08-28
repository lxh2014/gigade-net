using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class EventpromoUserConditionDao : IEventPromoUserConditionImplDao
    {
        private IDBAccess _dbAccess;
        public EventpromoUserConditionDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        #region 查詢
        public DataTable GetList(Model.Query.EventPromoUserConditionQuery epQuery, out int totalCount)
        {
            StringBuilder strsql = new StringBuilder();
            StringBuilder strcondition = new StringBuilder();
            totalCount = 0;
            try
            {

                strsql.Append(@"SELECT vug.group_name,ml.ml_name,epuc.condition_id,epuc.condition_name,epuc.level_id,FROM_UNIXTIME(epuc.first_buy_time)as first_buy_time,FROM_UNIXTIME(epuc.reg_start)as reg_start,FROM_UNIXTIME(epuc.reg_end)as reg_end,epuc.buy_times_min,epuc.buy_times_max,epuc.buy_amount_min,epuc.buy_amount_max,epuc.group_id");
                strcondition.Append(@" FROM event_promo_user_condition epuc
                left join member_level ml on epuc.level_id =ml.ml_code 
                left join vip_user_group vug on epuc.group_id =vug.group_id where 1=1 ");

                if (!string.IsNullOrEmpty(epQuery.condition_name))
                {
                    strcondition.AppendFormat(" and epuc.condition_name like '%{0}%' ", epQuery.condition_name);
                }
                if (epQuery.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable("select count(epuc.condition_id) as totalCount  " + strcondition.ToString());
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    strcondition.AppendFormat(" limit {0},{1};", epQuery.Start, epQuery.Limit);
                }
                strsql.Append(strcondition.ToString());
                return _dbAccess.getDataTable(strsql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventpromoUserConditionDao-->GetList--" + ex.Message + strsql.ToString(), ex);
            }
        }

        /// <summary>
        /// 獲取所有的會員條件名稱
        /// </summary>
        /// <returns></returns>
        public DataTable GetEventCondi(Model.Query.EventPromoUserConditionQuery epQuery)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select condition_id,condition_name from event_promo_user_condition ");
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventpromoUserConditionDao-->GetEventCondi--" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        public int AddOrUpdate(Model.Query.EventPromoUserConditionQuery epQuery)
        {
            StringBuilder sb = new StringBuilder();
            epQuery.Replace4MySQL();
            try
            {
                if (epQuery.condition_id > 0)//編輯
                {
                    sb.AppendFormat(@"update event_promo_user_condition set condition_name='{0}',level_id='{1}',first_buy_time='{2}',reg_start='{3}',reg_end='{4}',buy_times_min='{5}',buy_times_max='{6}',buy_amount_min='{7}',
                                     buy_amount_max='{8}', group_id='{9}',modify_user='{10}',modify_time='{11}' where condition_id='{12}'; ",
                                     epQuery.condition_name, epQuery.level_id, epQuery.first_buy_time, epQuery.reg_start, epQuery.reg_end, epQuery.buy_times_min, epQuery.buy_times_max, epQuery.buy_amount_min,
                                     epQuery.buy_amount_max, epQuery.group_id, epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time), epQuery.condition_id);
                    return _dbAccess.execCommand(sb.ToString());
                }
                else
                {
                    sb.AppendFormat(@"INSERT INTO event_promo_user_condition(condition_name,level_id,first_buy_time,reg_start,reg_end,buy_times_min,buy_times_max,
                    buy_amount_min,buy_amount_max,group_id,create_user,create_time,modify_user,modify_time)");
                    sb.AppendFormat("VALUES('{0}','{1}',{2},'{3}',{4},'{5}','{6}',", epQuery.condition_name, epQuery.level_id, epQuery.first_buy_time, epQuery.reg_start, epQuery.reg_end, epQuery.buy_times_min, epQuery.buy_times_max);
                    sb.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}','{6}');", epQuery.buy_amount_min, epQuery.buy_amount_max, epQuery.group_id, epQuery.create_user, Common.CommonFunction.DateTimeToString(epQuery.create_time), epQuery.modify_user, Common.CommonFunction.DateTimeToString(epQuery.modify_time));
                    return _dbAccess.execCommand(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EventpromoUserConditionDao-->AddOrUpdate--" + ex.Message, ex);
            }
        }

        public int Delete(Model.Query.EventPromoUserConditionQuery epQuery)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"delete from event_promo_user_condition where condition_id in({0});", epQuery.condition_id_tostring);
                return _dbAccess.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("EventpromoUserConditionDao-->Delete--" + ex.Message, ex);
            }
        }
    }
}
