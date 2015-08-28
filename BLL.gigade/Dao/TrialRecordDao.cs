using BLL.gigade.Dao.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class TrialRecordDao : ITrialRecordImplDao
    {
        private IDBAccess _access;
        private string connStr;
        public TrialRecordDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
             
        }
        public DataTable GetShareList(TrialShareQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            totalCount = 0;
            try
            { 
                sqlCount.Append("select count(ts.share_id) as totalCount ");
                sql.Append(" select ts.share_id,ts.trial_id,pat.`name` as event_name ,ts.user_id,u.user_name as  real_name, ts.is_show_name,ts.user_name,ts.user_name as after_name,case when ts.user_gender=0 then '小姐' else '先生' end as gender,ts.content,ts.share_time,ts.`status`  ");
                sqlFrom.Append(" from trial_share ts ");
                sqlFrom.Append(" LEFT JOIN users u on u.user_id=ts.user_id ");
                sqlFrom.Append(" LEFT JOIN promotions_amount_trial pat on  pat.id = ts.trial_id ");
                sqlWhere.AppendFormat(" where 1 =1 ");
                sqlWhere.AppendFormat(" and ts.trial_id='{0}' ", query.trial_id);
                if (query.share_id != 0)
                {
                    sqlWhere.AppendFormat(" and ts.share_id='{0}' ", query.share_id);
                }
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                }
                sqlWhere.AppendFormat(" order by ts.share_id desc limit {0},{1};", query.Start, query.Limit);
                return _access.getDataTable(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                //    return _access.getDataTableForObj<TrialShareQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordDao-->GetShareList-->" + ex.Message + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString(), ex);
            }
        }
        public List<Model.Query.TrialRecordQuery> GetTrialRecordList(TrialRecordQuery store, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            totalCount = 0;
            sqlCount.Append("select count(tr.record_id) as totalCount ");
            sql.Append(" SELECT tr.record_id,tr.trial_id,tr.user_id,tr.apply_time,tr.`status` ,pat.`name`,pat.event_type,pat.paper_id,pat.event_id as trial_id,u.user_email,u.user_name ");
            sqlFrom.Append(" from trial_record tr ");
            sqlFrom.Append(" LEFT JOIN users u on u.user_id=tr.user_id ");
            sqlFrom.Append(" LEFT JOIN promotions_amount_trial pat on  pat.id = tr.trial_id ");
            sqlWhere.Append(" where 1 =1 and apply_time<>'0001-01-01 00:00:00' and ! ISNULL(apply_time) ");
            sqlWhere.AppendFormat(" and tr.trial_id='{0}' ", store.trial_id);
            if (store.record_id != 0)
            {
                sqlWhere.AppendFormat(" and tr.record_id='{0}' ", store.record_id);
            }
            if (store.status != 0)
            {
                sqlWhere.AppendFormat(" and tr.status='{0}' ", store.status);
            }
            if (store.IsPage)
            {
                DataTable _dt = _access.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                }
            }
            sqlWhere.AppendFormat(" order by tr.record_id desc limit {0},{1};", store.Start, store.Limit);
            try
            {
                return _access.getDataTableForObj<TrialRecordQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordDao-->GetTrialRecordList-->" + ex.Message + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString(), ex);
            }
        }
        #region 更新試用狀態+int TrialRecordUpdate(TrialRecordQuery query)
        /// <summary>
        /// 更新試用狀態
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int TrialRecordUpdate(TrialRecordQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("set sql_safe_updates = 0; update trial_record set status='{0}' where record_id='{1}'; set sql_safe_updates = 1;", query.status, query.record_id);
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TrialRecordDao-->TrialRecordUpdate-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 保存試用分享信息+ int TrialRecordSave(TrialRecordQuery query)
        /// <summary>
        /// 保存試用分享信息
        /// </summary>
        /// <param name="cq"></param>
        /// <returns></returns>
        public int TrialRecordSave(TrialShareQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {

                strSql.AppendFormat("update trial_share set ");
                strSql.AppendFormat("user_name='{0}',content='{1}',is_show_name={2},status={3},user_gender={4} ", query.user_name, query.content, query.is_show_name, query.status, query.user_gender);
                strSql.AppendFormat(" where share_id='{0}'", query.share_id);
                return _access.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TrialRecordDao-->TrialRecordSave-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 根據id獲取一條記錄的信息+TrialRecordQuery GetTrialRecordById(TrialRecordQuery query)
        /// <summary>
        /// 根據id獲取一條記錄的信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public TrialRecordQuery GetTrialRecordById(TrialRecordQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("select trial_id,user_email,user_name,is_show_name,user_skin,content,share_time,kuser,kdate,muser,mdate from trial_record where record_id='{0}'", query.record_id);
                return _access.getSinggleObj<TrialRecordQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("TrialRecordDao-->GetTrialRecordById-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion


        public DataTable GetSumCount(PromotionsAmountTrialQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select  trial_id,sum(recordCount) as recordCount ,sum(shareCount) as shareCount from ( ");
                sql.Append(" select trial_id, count(*) as recordCount,0 as shareCount from trial_record GROUP BY trial_id   UNION  ");
                sql.Append("  (select trial_id,0 as recordCount, count(*) as shareCount from trial_share  GROUP BY trial_id)   ) aaa ");

                sql.AppendFormat(" where aaa.trial_id='{0}'", query.id);

                sql.Append(" GROUP BY trial_id ");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordDao-->GetSumCount-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public bool VerifyMaxCount(TrialRecordQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            try
            {
                sql.AppendFormat("select show_number from promotions_amount_trial where id={0};", query.trial_id);
                sqlCount.AppendFormat("select count(trial_id) as Count from trial_record where trial_id={0} and `status`={1};", query.trial_id, query.status);
                DataTable _dtSql = _access.getDataTable(sql.ToString());
                DataTable _dtSqlCount = _access.getDataTable(sqlCount.ToString());
                if (Convert.ToInt32(_dtSql.Rows[0]["show_number"]) > Convert.ToInt32(_dtSqlCount.Rows[0]["Count"]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordDao-->VerifyMaxCount-->" + sql.ToString() + sqlCount.ToString() + ex.Message, ex);
            }
            throw new NotImplementedException();
        }
        public TrialShare GetTrialShare(TrialShare model)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select  user_id,user_name from  trial_share");
                sql.AppendFormat(" where user_id='{0}'", model.user_id);
                sql.AppendFormat(" and share_id='{0}'", model.share_id);
                return _access.getSinggleObj<TrialShare>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TrialRecordDao-->GetTrialShare-->" + sql.ToString() + ex.Message, ex);
            }
        }

    }
}
