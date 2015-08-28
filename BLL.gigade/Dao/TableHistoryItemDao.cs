/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：TableHistoryItemDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 14:42:00 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.Data;


namespace BLL.gigade.Dao
{
    public class TableHistoryItemDao:ITableHistoryItemImplDao
    {
        private IDBAccess _dbAccess;
        public TableHistoryItemDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region ITableHistoryItemImplDao 成员

        public List<Model.TableHistoryItem> Query(Model.TableHistoryItem tableHistoryItem)
        {
            StringBuilder strSql = new StringBuilder("select rowid,tablehistoryid,col_name,col_chsname,col_value,old_value,type from t_table_historyitem");
            strSql.AppendFormat(" where tablehistoryid={0}",tableHistoryItem.tablehistoryid);
            return _dbAccess.getDataTableForObj<Model.TableHistoryItem>(strSql.ToString());
        }

        public string Save(Model.TableHistoryItem tableHistoryItem)
        {
            tableHistoryItem.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("insert into t_table_historyitem(`tablehistoryid`,`col_name`,`col_chsname`,`col_value`,`old_value`,`type`) values('{0}',");
            strSql.AppendFormat("'{0}','{1}',", tableHistoryItem.col_name, tableHistoryItem.col_chsname);
            strSql.AppendFormat("'{0}','{1}',{2});", tableHistoryItem.col_value, tableHistoryItem.old_value, tableHistoryItem.type);
            return strSql.ToString();
        }

        public string UpdateType(Model.TableHistoryItem tableHistoryItem)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;update t_table_historyitem set type=0 where type=1");
            strSql.AppendFormat(" and col_name='{0}' and tablehistoryid in (select a.rowid from t_table_history a ", tableHistoryItem.col_name);
            strSql.AppendFormat("inner join(select table_name,pk_name,pk_value from t_table_history where rowid={0}) b on ", tableHistoryItem.tablehistoryid);
            strSql.Append("a.table_name=b.table_name and a.pk_name=b.pk_name and a.pk_value=b.pk_value);set sql_safe_updates=1;");
            return strSql.ToString();
        }

        public string UpdateType(List<Model.TableHistoryItem> tableHistoryItems, int tablehistoryid)
        {
            string colNames = string.Empty;
            tableHistoryItems.ForEach(t => colNames += "','" + t.col_name);
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;update t_table_historyitem set type=0 where type=1");
            strSql.AppendFormat(" and col_name in('{0}') and tablehistoryid in (select a.rowid from t_table_history a ", colNames.Remove(0, 3));
            strSql.AppendFormat("inner join(select table_name,pk_name,pk_value from t_table_history where rowid={0}) b on ", tablehistoryid);
            strSql.Append("a.table_name=b.table_name and a.pk_name=b.pk_name and a.pk_value=b.pk_value);set sql_safe_updates=1;");
            return strSql.ToString();

        }

        public List<Model.TableHistoryItem> Query4Batch(Model.Query.TableHistoryItemQuery query)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select rowid,tablehistoryid,col_name,col_chsname,col_value,old_value,type from t_table_historyitem where ");
                strSql.Append("tablehistoryid in(select rowid from t_table_history where 1=1 ");
                if (!string.IsNullOrEmpty(query.table_name))
                {
                    strSql.AppendFormat(" and table_name='{0}'", query.table_name);
                }
                if (!string.IsNullOrEmpty(query.pk_name))
                {
                    strSql.AppendFormat(" and pk_name='{0}'", query.pk_name);
                }
                if (!string.IsNullOrEmpty(query.pk_value))
                {
                    strSql.AppendFormat(" and pk_value='{0}'", query.pk_value);
                }
                if (!string.IsNullOrEmpty(query.batchno))
                {
                    strSql.AppendFormat(" and batchno='{0}'", query.batchno);
                }
                strSql.Append(")");
                return _dbAccess.getDataTableForObj<Model.TableHistoryItem>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryItemDao.Query4Notice-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據條件獲得歷史記錄信息
        /// </summary>
        /// <param name="th">查詢條件</param>
        /// <returns>符合條件的集合</returns>
        public DataTable GetHistoryInfoByCondition(TableHistory th)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT th.rowid,th.table_name,f.functionName,th.PK_name,th.PK_value,th.batchno,item.col_name,item.col_chsName,item.col_value,item.old_value,item.type 
        FROM t_table_history th
        LEFT JOIN t_table_historyitem item ON th.rowid = item.tableHistoryId
        LEFT JOIN t_function f ON f.rowid =th.functionId ");
                sb.AppendFormat(" WHERE th.batchno IN ('{0}') ORDER BY table_name DESC", th.batchno);
                return _dbAccess.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryItemDao-->GetHistoryInfoByCondition" + ex.Message, ex);
            }
        }

        #endregion
    }
}
