using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using DBAccess;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class BrowseDataDao : IBrowseDataImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;

        public BrowseDataDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        /// <summary>
        /// 獲取商品點擊信息
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">數據總條數</param>
        /// <returns>商品點擊信息</returns>
        public DataTable GetBrowseDataList(BrowseDataQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlColumn = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder("SELECT count(bd.id) as totalCount ");
            StringBuilder sqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            query.Replace4MySQL();
            sqlColumn.Append("SELECT bd.id,bd.user_id,bd.product_id,bd.type,bd.count,u.user_name,p.product_name ");
            sqlTable.Append(" FROM browse_data bd ");
            sqlTable.Append(" LEFT JOIN users u ON bd.user_id=u.user_id ");
            sqlTable.Append(" LEFT JOIN product p ON bd.product_id=p.product_id ");
            sqlCondition.Append(" WHERE 1=1 ");
            if (query.id != 0)
            {
                sqlCondition.AppendFormat( " and bd.id='{0}' ",query.id);
            }
            if (query.type != 0)
            {
                //sqlCondition.AppendFormat(" AND db.type IN({0}) ", query.type);
                sqlCondition.AppendFormat(" AND bd.type ='{0}' ",query.type);
            }
            if (query.SearchType != 0)
            {
                if (!string.IsNullOrEmpty(query.SearchCondition))
                {
                    switch (query.SearchType) 
                    {
                        case 1:
                            sqlCondition.AppendFormat(" AND bd.user_id={0} ", query.SearchCondition);
                            break;
                        case 2:
                            sqlCondition.AppendFormat(" AND bd.product_id={0} ", query.SearchCondition);
                            break;
                        case 3:
                            sqlCondition.AppendFormat(" AND u.user_name LIKE '%{0}%' ", query.SearchCondition);
                            break;
                        case 4:
                            sqlCondition.AppendFormat(" AND p.product_name LIKE '%{0}%' ", query.SearchCondition);
                            break;
                    }
                    //sqlCondition.AppendFormat(" and  (bd.user_id='{0}' or bd.product_id='{0}' or u.user_name LIKE '%{0}%' or  p.product_name LIKE '%{0}%'  ) ", query.SearchCondition);
                    //sqlCondition.AppendFormat(" OR bd.user_id={0} ", query.SearchCondition);
                    //sqlCondition.AppendFormat(" OR bd.product_id={0} ", query.SearchCondition);
                    //sqlCondition.AppendFormat(" OR u.user_name LIKE '%{0}%' ", query.SearchCondition);
                    //sqlCondition.AppendFormat(" OR p.product_name LIKE '%{0}%' ", query.SearchCondition);
                }
            }
            sqlCondition.Append(" ORDER BY bd.count DESC ");
            totalCount = 0;
            try
            {
                if (query.IsPage)
                {
                    DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString() + sqlTable.ToString() + sqlCondition.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlCondition.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                sql.Append(sqlColumn).Append(sqlTable).Append(sqlCondition);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BrowseDataDao-->GetBrowseDataList -->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}