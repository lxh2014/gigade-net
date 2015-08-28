using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class VoteDetailDao
    {
        private IDBAccess _access;
        private string connString;
        public VoteDetailDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">VoteDetail對象</param>
        /// <returns>新增后的標識</returns>
        public int Add(VoteDetail model)
        {
            StringBuilder sbSql = new StringBuilder();
            model.Replace4MySQL();
            sbSql.Append("INSERT INTO vote_detail(article_id,user_id,ip,vote_status,create_user,update_user,create_time,update_time) ");
            sbSql.AppendFormat(" VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');SELECT @@IDENTITY;", model.article_id, model.user_id, model.ip, model.vote_status, model.create_user, model.update_user, CommonFunction.DateTimeToString(model.create_time), CommonFunction.DateTimeToString(model.update_time));
            try
            {
                DataTable _dt = _access.getDataTable(sbSql.ToString());
                if (_dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(_dt.Rows[0][0]);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailDao-->Add-->" + ex.Message + sbSql.ToString(), ex);
            }
        } 
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">VoteDetail對象</param>
        /// <returns>更新結果</returns>
        public int Update(VoteDetail model)
        {
            int result = 0;
            StringBuilder sbSql = new StringBuilder();
            model.Replace4MySQL();
            sbSql.Append("set sql_safe_updates=0;");
            sbSql.AppendFormat("UPDATE vote_detail set article_id='{0}',user_id='{1}',ip='{2}',create_user='{3}',update_user='{4}',create_time='{5}',update_time='{6}' WHERE vote_id='{7}';", model.article_id, model.user_id, model.ip, model.create_user, model.update_user, CommonFunction.DateTimeToString(model.create_time), CommonFunction.DateTimeToString(model.update_time), model.vote_id);
            sbSql.Append("set sql_safe_updates=1;");
            try
            {
                result = _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailDao-->Update-->" + ex.Message + sbSql.ToString(), ex);
            }
            return result;
        } 
        #endregion

        #region 查詢
        #region 通過id獲取VoteDetail對象
        /// <summary>
        /// 通過id獲取VoteDetail對象
        /// </summary>
        /// <param name="vote_id">編號</param>
        /// <returns>VoteDetail對象</returns>
        public VoteDetail Get(int vote_id)
        {
            VoteDetail model = new VoteDetail();
            string sql = "SELECT vote_id,article_id,user_id,ip,vote_status,create_user,update_user,create_time,update_time FROM vote_detail WHERE vote_id=" + vote_id;
            try
            {
                model = _access.getSinggleObj<VoteDetail>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailDao-->Get-->" + ex.Message + sql, ex);
            }
            return model;
        }
        #endregion

        #region 通過查詢條件獲取VoteDetail列表
        /// <summary>
        /// 通過查詢條件獲取VoteDetail列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>VoteDetail列表</returns>
        public List<VoteDetailQuery> GetList(VoteDetailQuery query, out int totalCount)
        {
            List<VoteDetailQuery> list = new List<VoteDetailQuery>();
            StringBuilder sbSqlColumn = new StringBuilder();
            StringBuilder sbSqlCount = new StringBuilder();
            StringBuilder sbSqlTable = new StringBuilder();
            StringBuilder sbSqlCondition = new StringBuilder();
            totalCount = 0;
            sbSqlColumn.Append("SELECT vote_id,vd.article_id,va.article_title,m.user_name,vd.user_id,ip,vote_status,vd.create_user,vd.update_user,vd.create_time,vd.update_time ");
            sbSqlTable.Append(" FROM vote_detail as vd ");
            sbSqlTable.Append(" LEFT JOIN vote_article as va ON  vd.article_id=va.article_id ");
            sbSqlTable.Append(" LEFT JOIN users m ON vd.user_id=m.user_id ");
            sbSqlCondition.Append(" WHERE 1=1 ");
            sbSqlCount.Append("select count(vote_id) as totalCount ");
            if (query.vote_id != 0)
            {
                //sbSqlCondition.AppendFormat(" and vote_id in({0}) ", query.vote_id);
                sbSqlCondition.AppendFormat(" and vd.vote_id ='{0}' ", query.vote_id);
            }
            if (query.article_id != 0)
            {
                //sbSqlCondition.AppendFormat(" and vd.article_id in({0}) ", query.article_id);
                sbSqlCondition.AppendFormat(" and vd.article_id ='{0}' ", query.article_id);
            }
            if (!string.IsNullOrEmpty(query.searchContent))
            {
                sbSqlCondition.AppendFormat(" and (vd.article_id like N'%{0}%' or va.article_title like N'%{0}%' or vd.user_id like N'%{0}%') ", query.searchContent);
            }
            if (query.start_time != null)
            {
                sbSqlCondition.AppendFormat(" and vd.create_time > '{0}' ",  CommonFunction.DateTimeToString(query.start_time));
            }
            if (query.end_time != null)
            {
                sbSqlCondition.AppendFormat(" and vd.create_time < '{0}' ",  CommonFunction.DateTimeToString(query.end_time));
            }
            if (query.vote_status != -1)
            {
                
                //sbSqlCondition.AppendFormat(" and vote_status in ({0}) ", query.vote_status);
                sbSqlCondition.AppendFormat(" and vote_status ='{0}' ", query.vote_status);
            }
            try 
            {
                if (query.IsPage)
                {
                    sbSqlCount.Append(sbSqlTable.ToString()+sbSqlCondition.ToString());
                    DataTable dt = _access.getDataTable( sbSqlCount.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                }
                sbSqlCondition.Append(" order by vote_id desc ");
                sbSqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                sbSqlColumn.Append(sbSqlTable.ToString()+sbSqlCondition.ToString());
                list = _access.getDataTableForObj<VoteDetailQuery>(sbSqlColumn.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailDao-->GetList-->" + ex.Message + sbSqlColumn.ToString() + sbSqlCount.ToString(), ex);
            }
            return list;
        }
        #endregion 
        #region 匯出Excel
        
       
        public DataTable GetDtVoteDetail(VoteDetailQuery query, out int totalCount)
        {
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCondition = new StringBuilder();
            DataTable dt = new DataTable();
            totalCount = 0;
            try
            {
                sbSql.Append("select vd.vote_id,p.product_id,p.product_name,ve.event_id,ve.event_name,va.article_id,va.article_title,vd.user_id,u.user_name,vd.create_time,vd.vote_status ");
                sbSql.Append(" from vote_detail vd ");
                sbSqlCondition.Append(" LEFT JOIN vote_article va on va.article_id=vd.article_id ");
                sbSqlCondition.Append(" left join vote_event ve on ve.event_id=va.event_id ");
                sbSqlCondition.Append(" LEFT JOIN users u on u.user_id=vd.user_id ");
                sbSqlCondition.Append(" left join product p on va.product_id=p.product_id ");
                sbSqlCondition.Append(" where 1=1 ");
                if (query.vote_id != 0)
                {
                    //sbSqlCondition.AppendFormat(" and vd.vote_id in({0}) ", query.vote_id);
                    sbSqlCondition.AppendFormat(" and vd.vote_id ='{0}' ", query.vote_id);
                }
                if (query.article_id != 0)
                {
                    //sbSqlCondition.AppendFormat(" and vd.article_id in({0}) ", query.article_id);
                    sbSqlCondition.AppendFormat(" and vd.article_id ='{0}' ", query.article_id);
                }
                if (!string.IsNullOrEmpty(query.searchContent))
                {
                    sbSqlCondition.AppendFormat(" and (vd.article_id like N'%{0}%' vd.or user_id like N'%{0}%') ", query.searchContent);
                }
                if (query.start_time != null)
                {
                    sbSqlCondition.AppendFormat(" and vd.create_time > '{0}' ",  CommonFunction.DateTimeToString(query.start_time));
                }
                if (query.end_time != null)
                {
                    sbSqlCondition.AppendFormat(" and vd.create_time < '{0}' ", query.end_time.ToString("yyyy-MM-dd 23:59:59"));
                }
                if (query.vote_status != -1)
                {
                    sbSqlCondition.AppendFormat(" and vd.vote_status = {0} ", query.vote_status);
                }
              
                if (query.IsPage)
                {

                    dt = _access.getDataTable("select count(vd.vote_id) as totalCount " + sbSqlCondition.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                    sbSqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                }
               
                dt = _access.getDataTable(sbSql.Append(sbSqlCondition).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailDao-->GetDtVoteDetail-->" + ex.Message + sbSql.Append(sbSqlCondition).ToString(), ex);
            }
            return dt;
        }
        #endregion
        #region 更改狀態


        public int UpdateVoteDetaiStatus(VoteDetailQuery query)
        {
            int result = 0;
            StringBuilder sql = new StringBuilder();
            query.Replace4MySQL();
            try
            {
                sql.Append("set sql_safe_updates=0;");
                sql.AppendFormat("update vote_detail set vote_status='{0}', update_time='{1}'", query.vote_status, CommonFunction.DateTimeToString(query.update_time));
                sql.AppendFormat(" ,update_user='{0}',ip='{1}' where vote_id='{2}'; ", query.update_user, query.ip, query.vote_id);
                sql.Append("set sql_safe_updates=1;");
                result = _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VoteDetailDao.UpdateVoteDetaiStatus-->" + ex.Message + sql.ToString(), ex);
            }
            return result;
        }
        #endregion
        #endregion
    }
}