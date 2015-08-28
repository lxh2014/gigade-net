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
    public class PromoShareConditionTypeDao
    {
        private IDBAccess _access;
        private string connString;
        public PromoShareConditionTypeDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">PromoShareConditionType對象</param>
        /// <returns>新增后的標識</returns>
        public int Add(PromoShareConditionType model)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("INSERT INTO promo_share_condition(condition_type_desc,condition_type_name,condition_type_status) ");
            sbSql.AppendFormat(" VALUES('{0}','{1}','{2}');SELECT @@IDENTITY;", model.condition_type_desc, model.condition_type_name, model.condition_type_status);
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
                throw new Exception("PromoShareConditionTypeDao-->Add-->" + ex.Message + sbSql.ToString(), ex);
            }
        } 
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">PromoShareConditionType對象</param>
        /// <returns>更新結果</returns>
        public int Update(PromoShareConditionType model)
        {
            int result = 0;
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("set sql_safe_updates=0;");
            sbSql.AppendFormat("UPDATE promo_share_condition_type set condition_type_desc='{0}',condition_type_name='{1}',condition_type_status='{2}' WHERE promo_id='{3}';", model.condition_type_desc, model.condition_type_name, model.condition_type_status, model.condition_type_id);
            sbSql.Append("set sql_safe_updates=1;");
            try
            {
                result = _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionTypeDao-->Update-->" + ex.Message + sbSql.ToString(), ex);
            }
            return result;
        } 
        #endregion

        #region 查詢
        #region 通過id獲取PromoShareConditionType對象
        /// <summary>
        /// 通過id獲取PromoShareConditionType對象
        /// </summary>
        /// <param name="condtion_type_id">編號</param>
        /// <returns>PromoShareConditionType對象</returns>
        public PromoShareConditionType Get(int condtion_type_id)
        {
            PromoShareConditionType model = new PromoShareConditionType();
            string sql = "SELECT condition_type_id,condition_type_desc,condition_type_name,condition_type_status FROM promo_share_condition_type WHERE condition_type_id=" + condtion_type_id;
            try
            {
                model = _access.getSinggleObj<PromoShareConditionType>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionType-->Get-->" + ex.Message + sql, ex);
            }
            return model;
        }
        #endregion

        #region 通過查詢條件獲取PromoShareConditionType列表
        /// <summary>
        /// 通過查詢條件獲取PromoShareConditionType列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>PromoShareConditionType列表</returns>
        public List<PromoShareConditionTypeQuery> GetList(PromoShareConditionTypeQuery query, out int totalCount)
        {
            List<PromoShareConditionTypeQuery> list = new List<PromoShareConditionTypeQuery>();
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCondition = new StringBuilder();
            totalCount = 0;
            sbSql.Append("SELECT condition_type_id,condition_type_desc,condition_type_name,condition_type_status ");
            sbSqlCondition.Append(" FROM promo_share_condition_type WHERE 1=1 ");
            if (query.condition_type_id != 0)
            {
                sbSqlCondition.AppendFormat(" and condition_type_id in({0}) ", query.condition_type_id);
            }
            if (!string.IsNullOrEmpty(query.condition_type_desc))
            {
                sbSqlCondition.AppendFormat(" and condition_type_desc like '%{0}%' ", query.condition_type_desc);
            }
            if (!string.IsNullOrEmpty(query.condition_type_name))
            {
                sbSqlCondition.AppendFormat(" and condition_type_name like '%{0}%' ", query.condition_type_name);
            }
            try
            {
                if (query.IsPage)
                {
                    DataTable dt = _access.getDataTable("select count(*) as totalCount " + sbSqlCondition.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                    }
                }
                sbSqlCondition.AppendFormat(" limit {0},{1} ", query.Start, query.Limit);
                list = _access.getDataTableForObj<PromoShareConditionTypeQuery>(sbSql.Append(sbSqlCondition).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionTypeDao-->GetList-->" + ex.Message + sbSql.Append(sbSqlCondition).ToString(), ex);
            }
            return list;
        }
        #endregion 
        #endregion
    }
}
