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
    public class PromoShareConditionDao
    {
        private IDBAccess _access;
        private string connString;
        public PromoShareConditionDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">PromoShareCondition對象</param>
        /// <returns>新增后的標識</returns>
        public int Add(PromoShareCondition model)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("INSERT INTO promo_share_condition(condition_name,condition_value,promo_id) ");
            sbSql.AppendFormat(" VALUES('{0}','{1}','{2}');", model.condition_name, model.condition_value, model.promo_id);
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
                throw new Exception("PromoShareConditionDao-->Add-->" + ex.Message + sbSql.ToString(), ex);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">PromoShareCondition對象</param>
        /// <returns>新增數據的Sql語句</returns>
        public string AddSql(PromoShareCondition model)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("INSERT INTO promo_share_condition(condition_name,condition_value,promo_id) ");
            sbSql.AppendFormat(" VALUES('{0}','{1}','{2}');", model.condition_name, model.condition_value, model.promo_id);
            return sbSql.ToString();
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">PromoShareCondition對象</param>
        /// <returns>更新結果</returns>
        public int Update(PromoShareCondition model)
        {
            int result = 0;
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("set sql_safe_updates=0;");
            sbSql.AppendFormat("UPDATE promo_share_condition set condition_type_id='{0}',condition_name='{1}',condition_value='{2}',promo_id='{3}' WHERE condition_id='{4}';", model.condition_type_id, model.condition_name,model.condition_value, model.promo_id,model.condition_id);
            sbSql.Append("set sql_safe_updates=1;");
            try
            {
                result = _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionDao-->Update-->" + ex.Message + sbSql.ToString(), ex);
            }
            return result;
        } 
        #endregion

        #region 查詢
        #region 通過id獲取PromoShareCondition對象
        /// <summary>
        /// 通過id獲取PromoShareCondition對象
        /// </summary>
        /// <param name="condition_id">編號</param>
        /// <returns>PromoShareCondition對象</returns>
        public PromoShareCondition Get(int condition_id)
        {
            PromoShareCondition model = new PromoShareCondition();
            string sql = "SELECT condition_id,condition_type_id,condition_name,condition_value,promo_id FROM promo_share_condition WHERE condition_id=" + condition_id;
            try
            {
                model = _access.getSinggleObj<PromoShareCondition>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareCondition-->Get-->" + ex.Message + sql, ex);
            }
            return model;
        }
        #endregion

        public DataTable Get(string[] condition,PromoShareCondition query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" select promo_id ");
                for (int i = 0; i < condition.Length; i++)
                {
                    sql.AppendFormat(",max(case psc.condition_name when '{0}' then psc.condition_value end)  '{1}'", condition[i], condition[i]);
                }
                sql.AppendFormat("from   promo_share_condition psc where promo_id='{0}';", query.promo_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionDao-->Get-->"+sql.ToString()+ex.Message,ex);
            }
        }

        #region 通過查詢條件獲取PromoShareCondition列表
        /// <summary>
        /// 通過查詢條件獲取PromoShareCondition列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>PromoShareCondition列表</returns>
        public List<PromoShareConditionQuery> GetList(PromoShareConditionQuery query)
        {
            List<PromoShareConditionQuery> list = new List<PromoShareConditionQuery>();
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCondition = new StringBuilder();
            sbSql.Append("SELECT condition_id,condition_name,condition_value,promo_id ");
            sbSqlCondition.Append(" FROM promo_share_condition WHERE 1=1 ");
            if (query.condition_id != 0)
            {
                sbSqlCondition.AppendFormat(" and condition_id in({0}) ", query.condition_id);
            }
            if (query.condition_type_id!=0)
            {
                sbSqlCondition.AppendFormat(" and condition_type_id in({0}) ", query.condition_type_id);
            }
            if (!string.IsNullOrEmpty(query.condition_name))
            {
                sbSqlCondition.AppendFormat(" and condition_name like '%{0}%' ", query.condition_name);
            }
            if (!string.IsNullOrEmpty(query.condition_value))
            {
                sbSqlCondition.AppendFormat(" and condition_value like '%{0}%' ", query.condition_value);
            }
            if (query.promo_id!=0)
            {
                sbSqlCondition.AppendFormat(" and promo_id in({0}) ", query.promo_id);
            }
            try
            {
                list = _access.getDataTableForObj<PromoShareConditionQuery>(sbSql.Append(sbSqlCondition).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionDao-->GetList-->" + ex.Message + sbSql.Append(sbSqlCondition).ToString(), ex);
            }
            return list;
        }
        #endregion 

        public int GetPromoShareConditionCount(PromoShareCondition query)
        {
            string sql = "SELECT promo_id FROM promo_share_condition WHERE promo_id=" + query.promo_id;
            try
            {
                return _access.getDataTable(sql).Rows.Count;
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareCondition-->GetPromoShareConditionCount-->" + ex.Message + sql, ex);
            }
        }
        #endregion

        #region 刪除
        public int Delete(int promo_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates=0;delete from promo_share_condition where promo_id='{0}';set sql_safe_updates=1;", promo_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareConditionTypeDao-->Delete-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
