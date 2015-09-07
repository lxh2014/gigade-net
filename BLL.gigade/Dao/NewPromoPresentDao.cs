using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class NewPromoPresentDao : INewPromoPresentImplDao
    {
        private IDBAccess _access;
        public NewPromoPresentDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public DataTable GetNewPromoPresentList(NewPromoPresentQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            try
            {//from new_promo_present npp LEFT JOIN product pt on pt.product_id=npp.gift_id  LEFT JOIN vip_user_group vug on npp.group_id=vug.group_id 
                sql.Append(@" select  npp.row_id as trow_id,npp.event_id,npp.welfare_mulriple,npp.gift_type,npp.ticket_name,npp.ticket_serial,npp.gift_id,npp.deduct_welfare,npp.gift_amount ,npp.gift_amount_over,npp.`status`,npp.freight_price,pt.product_name,npp.start as tstart,npp.end as tend,npp.group_id as tgroup_id ,npp.bonus_expire_day,npp.use_span_day,vug.group_name,npp.muser,mu.user_username ");
                sqlCondition.Append(" from new_promo_present npp ");
                sqlCondition.Append(" LEFT JOIN product pt on pt.product_id=npp.gift_id ");
                sqlCondition.Append(" LEFT JOIN vip_user_group vug on npp.group_id=vug.group_id ");
                sqlCondition.Append(" LEFT JOIN manage_user mu ON npp.muser=mu.user_id ");
                sqlCount.Append(@" select count(row_id) as totalcount ");
                sqlCondition.Append(" where 1=1 ");
                if (!string.IsNullOrEmpty(query.event_id))
                {
                    sqlCondition.AppendFormat(" and npp.event_id LIKE N'%{0}%'", query.event_id);
                }
                totalCount = 0;
                if (query.IsPage)
                {
                    sqlCount.Append(sqlCondition.ToString());
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() );
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                    }
                    sqlCondition.Append(" order by npp.row_id desc ");
                    sqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
                else
                {
                    sqlCondition.Append(" order by npp.row_id desc ");
                }
                sql.Append(sqlCondition.ToString());

                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.GetNewPromoPresentList-->" + ex.Message + sql.ToString() + sqlCount.ToString(), ex);
            }

        }

        public string InsertNewPromoPresent(NewPromoPresentQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"insert into new_promo_present (event_id,gift_type,group_id,start,end,ticket_name,ticket_serial,gift_id,deduct_welfare,row_id,gift_amount ,gift_amount_over,status,freight_price,bonus_expire_day,welfare_mulriple,created,modified,kuser,muser,use_span_day)");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}' ", store.event_id, store.gift_type, store.group_id, CommonFunction.DateTimeToString(store.start), CommonFunction.DateTimeToString(store.end));
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}','{4}'", store.ticket_name, store.ticket_serial, store.gift_id, store.deduct_welfare, store.row_id);
                sql.AppendFormat(@",'{0}','{1}','{2}','{3}','{4}','{5}'", store.gift_amount, store.gift_amount_over, 1, store.freight_price, store.bonus_expire_day, store.welfare_mulriple);
                sql.AppendFormat(@",'{0}','{1}',{2},{3},{4});",CommonFunction.DateTimeToString( store.created),CommonFunction.DateTimeToString(store.modified),store.kuser,store.muser,store.use_span_day);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.InsertNewPromoPresent-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdateNewPromoPresent(NewPromoPresentQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update   new_promo_present set event_id='{0}',gift_type='{1}',group_id='{2}',start='{3}',end='{4}' ", store.event_id, store.gift_type, store.group_id, CommonFunction.DateTimeToString(store.start), CommonFunction.DateTimeToString(store.end));
                sql.AppendFormat(@",ticket_name='{0}',ticket_serial='{1}',gift_id='{2}',deduct_welfare='{3}',welfare_mulriple='{4}'", store.ticket_name, store.ticket_serial, store.gift_id, store.deduct_welfare, store.welfare_mulriple);
                sql.AppendFormat(@",gift_amount='{0}',gift_amount_over='{1}',freight_price='{2}',bonus_expire_day='{6}',muser={4},modified='{5}',use_span_day='{7}' where row_id='{3}'", store.gift_amount, store.gift_amount_over, store.freight_price, store.row_id, store.muser, CommonFunction.DateTimeToString(store.modified), store.bonus_expire_day,store.use_span_day);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.UpdateNewPromoPresent-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdateActive(int status, string event_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update   new_promo_present set status='{0}' where event_id='{1}';", status, event_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.UpdateActive-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int DeleteNewPromoPresent(int row_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"delete from  new_promo_present  where row_id='{0}';", row_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.DeleteNewPromoPresent-->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 根據活動編號獲取有效的贈品個數
        /// </summary>
        /// <param name="event_id"></param>
        /// <returns></returns>
        public int GetNewPromoPresent(string event_id)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append(@" select count(row_id) as num ,event_id from new_promo_present ");

                sql.Append(" where `status`=1 ");
                if (!string.IsNullOrEmpty(event_id))
                {
                    sql.AppendFormat(" and event_id='{0}'", event_id);
                }

                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return int.Parse(_dt.Rows[0]["num"].ToString());
                }
                else
                {
                    return 0;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.GetNewPromoPresent-->" + ex.Message + sql.ToString(), ex);
            }

        }


        public string GetProductnameById(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select product_name from  product where product_id='{0}';", id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return _dt.Rows[0][0].ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.GetProductnameById-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public string UpdateActive(NewPromoPresentQuery newPresent)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update new_promo_present set status='{0}',muser={2},modified='{3}' where row_id='{1}'", newPresent.status, newPresent.row_id,newPresent.muser,CommonFunction.DateTimeToString(newPresent.modified));

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.UpdateActive-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public string DeleteNewPromoPresent(NewPromoPresentQuery newPresent)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"delete from new_promo_present where row_id='{0}'", newPresent.row_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.DeleteNewPromoPresent-->" + ex.Message + sql.ToString(), ex);
            }
        }


        public int GetNewPromoPresentMaxId()
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select max(row_id) from new_promo_present");
                if (!string.IsNullOrEmpty(_access.getDataTable(sql.ToString()).Rows[0][0].ToString()))
                {
                    return Convert.ToInt32(_access.getDataTable(sql.ToString()).Rows[0][0]) + 1;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("NewPromoPresentDao.GetNewPromoPresentMaxId-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
