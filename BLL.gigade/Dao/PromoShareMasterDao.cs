using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class PromoShareMasterDao
    {
        private IDBAccess _access;
        private string connString;
        public PromoShareMasterDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model">PromoShareMaster對象</param>
        /// <returns>新增后的標識</returns>
        public int Add(PromoShareMaster model)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("INSERT INTO promo_share_master(promo_name,promo_desc,promo_start,promo_end,promo_active) ");
            sbSql.AppendFormat(" VALUES('{0}','{1}','{2}','{3}','{4}');SELECT @@IDENTITY;", model.promo_name, model.promo_desc, CommonFunction.DateTimeToString(model.promo_start), CommonFunction.DateTimeToString(model.promo_end), model.promo_active);
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
                throw new Exception("PromoShareMasterDao-->Add-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">PromoShareMaster對象</param>
        /// <returns>更新結果</returns>
        public int Update(PromoShareMaster model)
        {
            int result = 0;
            model.Replace4MySQL();
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("set sql_safe_updates=0;");
            if (model.eventId)
            {
                sbSql.AppendFormat("UPDATE promo_share_master set promo_event_id='{0}' WHERE promo_id='{1}';", model.promo_event_id, model.promo_id);
            }
            else
            {
            sbSql.AppendFormat("UPDATE promo_share_master set promo_name='{0}',promo_desc='{1}',promo_start='{2}',promo_end='{3}',promo_active='{4}' WHERE promo_id='{5}';", model.promo_name, model.promo_desc, CommonFunction.DateTimeToString(model.promo_start),CommonFunction.DateTimeToString(model.promo_end), model.promo_active, model.promo_id);
            }
            sbSql.Append("set sql_safe_updates=1;");
            try
            {
                result = _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterDao-->Update-->" + ex.Message + sbSql.ToString(), ex);
            }
            return result;
        }
        #endregion
        #region 修改其狀態
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model">PromoShareMaster對象</param>
        /// <returns>更新結果</returns>
        public int UpdateActivePromoShareMaster(PromoShareMaster model)
        {
            int result = 0;
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("set sql_safe_updates=0;");
            sbSql.AppendFormat("UPDATE promo_share_master set promo_active='{0}' WHERE promo_id='{1}';", model.promo_active, model.promo_id);
            sbSql.Append("set sql_safe_updates=1;");
            try
            {
                result = _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterDao-->UpdateActivePromoShareMaster-->" + ex.Message + sbSql.ToString(), ex);
            }
            return result;
        }
        #endregion
        #region 查詢
        #region 通過id獲取PromoShareMaster對象
        /// <summary>
        /// 通過id獲取PromoShareMaster對象
        /// </summary>
        /// <param name="promo_id">編號</param>
        /// <returns>PromoShareMaster對象</returns>
        public PromoShareMaster Get(int promo_id)
        {
            PromoShareMaster model = new PromoShareMaster();
            string sql = "SELECT promo_id,promo_name,promo_desc,FROM_UNIXTIME(promo_start) as promo_start,FROM_UNIXTIME(promo_end) as promo_end,promo_active FROM promo_share_master WHERE promo_id=" + promo_id;
            try
            {
                model = _access.getSinggleObj<PromoShareMaster>(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterDao-->Get-->" + ex.Message + sql, ex);
            }
            return model;
        }
        #endregion

        #region 通過查詢條件獲取PromoShareMaster列表
        /// <summary>
        /// 通過查詢條件獲取PromoShareMaster列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>PromoShareMaster列表</returns>
        public DataTable GetList(PromoShareMasterQuery query, out int totalCount)
        {
            DataTable _dtable = new DataTable();
            StringBuilder sbSql = new StringBuilder();
            StringBuilder sbSqlCondition = new StringBuilder();
            totalCount = 0;
            sbSql.Append("  SELECT psm.promo_id,psm.promo_event_id,psm.promo_name,psm.promo_desc,psm.promo_start,psm.promo_end,psm.promo_active,psc.this_promo_id ");
            sbSqlCondition.Append(" FROM promo_share_master psm   LEFT JOIN ( SELECT DISTINCT(promo_id) as this_promo_id FROM promo_share_condition ) psc on psc.this_promo_id=psm.promo_id WHERE 1=1 ");
            if (!string.IsNullOrEmpty(query.promo_active.ToString()))
            {
                if (query.promo_active != 2)
                {
                    sbSqlCondition.AppendFormat(" and psm.promo_active= '{0}' ", query.promo_active);
                }
            }
            if (!string.IsNullOrEmpty(query.promo_name))
            {
                sbSqlCondition.AppendFormat(" and psm.promo_name like '%{0}%' ", query.promo_name);
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
                sbSqlCondition.AppendFormat("order by psm.promo_id desc limit {0},{1} ", query.Start, query.Limit);
                _dtable = _access.getDataTable(sbSql.Append(sbSqlCondition).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("PromoShareMasterDao-->GetList-->" + ex.Message + sbSql.Append(sbSqlCondition).ToString(), ex);
            }
            return _dtable;
        }
        #endregion
        #endregion


        public int DeletePromoShareMessage(string str)
        {
            StringBuilder sbSql = new StringBuilder();
            int result = 0;
            sbSql.Append("set sql_safe_updates=0;");
            sbSql.AppendFormat("delete from promo_share_master where promo_id in ({0});", str);
            sbSql.AppendFormat("delete from promo_share_condition where promo_id in ({0});", str);
            sbSql.Append("set sql_safe_updates=1;");
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;

                mySqlCmd.CommandText = sbSql.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("IialgDao.HuiruInsertiialg-->" + ex.Message + sbSql.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }

        public int PromoCon(PromoShareMaster query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select count(*) as totalCount from promo_share_condition where promo_id='{0}';",query.promo_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0]["totalCount"]);
            }
            catch (Exception ex)
            {
                throw new Exception("PromoCon");
            }
        }
    }
}
