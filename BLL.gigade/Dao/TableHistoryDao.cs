/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：TableHistoryDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/22 14:40:50 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;


namespace BLL.gigade.Dao
{
    public class TableHistoryDao : ITableHistoryImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public TableHistoryDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        #region ITableHistoryImplDao 成员

        public int QueryExists(Model.TableHistory tableHistory)
        {
            StringBuilder strSql = new StringBuilder("select rowid from t_table_history where ");
            strSql.AppendFormat("table_name='{0}' and pk_name='{1}' and pk_value='{2}' order by rowid desc limit 1", tableHistory.table_name, tableHistory.pk_name, tableHistory.pk_value);
            System.Data.DataTable _dt = _dbAccess.getDataTable(strSql.ToString());
            if (_dt != null && _dt.Rows.Count > 0)
            {
                return Convert.ToInt32(_dt.Rows[0][0]);
            }
            else
            {
                return -1;
            }
        }

        public string Save(Model.TableHistory tableHistory)
        {
            StringBuilder strSql = new StringBuilder("insert into t_table_history(`table_name`,`functionid`,`pk_name`,`pk_value`,`batchno`) values(");
            strSql.AppendFormat("'{0}',{1},'{2}','{3}','{4}');select @@identity;", tableHistory.table_name, tableHistory.functionid, tableHistory.pk_name, tableHistory.pk_value, tableHistory.batchno);
            return strSql.ToString();
        }

        public string Query_TB_PK(string tb_Name)
        {
            StringBuilder strSql = new StringBuilder("select column_name from information_schema.key_column_usage ");
            strSql.AppendFormat("where table_name='{0}' and referenced_column_name is null and constraint_name = 'primary'", tb_Name);
            return _dbAccess.getDataTable(strSql.ToString()).Rows[0][0].ToString();
        }

        public List<Model.Column> Query_COL_Comment(string tb_Name, string table_schema)
        {
            StringBuilder strSql = new StringBuilder("select `column_name`, column_comment from information_schema.columns where ");
            strSql.AppendFormat("table_name = '{0}' and  table_schema='{1}' and column_comment<>'' ", tb_Name, table_schema);

            return _dbAccess.getDataTableForObj<Column>(strSql.ToString());
            //return _dbAccess.getDataTable(strSql.ToString()) == null ? "沒有描述" : _dbAccess.getDataTable(strSql.ToString()).Rows[0][0].ToString();
        }

        public List<Model.TableHistory> Query(Model.TableHistory query)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select rowid,table_name,functionid,pk_name,pk_value,batchno from t_table_history where 1=1");
                if (!string.IsNullOrEmpty(query.batchno))
                {
                    strSql.AppendFormat(" and batchno='{0}'", query.batchno);
                }
                return _dbAccess.getDataTableForObj<Model.TableHistory>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryDao.Query-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 無法從才程序中確定哪些是價格的表，哪些是商品的表，
        /// 用TableHistory.table_name來判斷
        /// 有傳值則是商品表，無則是價格表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Model.TableHistory QueryLastModifyByProductId(Model.TableHistory query)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select rowid,table_name,functionid,pk_name,pk_value,batchno from t_table_history where ");
                strSql.AppendFormat("batchno like '%_{0}%' and table_name {1}('price_master','item_price') order by rowid desc limit 3", query.pk_value, string.IsNullOrEmpty(query.table_name) ? "in" : "not in");
                return _dbAccess.getSinggleObj<Model.TableHistory>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryDao.QueryLastModifyByProductId-->" + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// 執行語句返回datatable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public System.Data.DataTable ExcuteSql(string sql)
        {
            return _dbAccess.getDataTable(sql);
        }

        /// <summary>
        /// 表欄位數據更新批量插入  
        /// 新插入數據直接記錄、
        /// 已有數據更新時插入原欄位數據及更新后欄位數據、
        /// 已有數據并已有記錄則插入新記錄,與新紀錄對應的欄位更新狀態為0
        /// </summary>
        /// <param name="histories"></param>
        /// <param name="historyItems"></param>
        /// <param name="otherSql"></param>
        /// <returns></returns>
        public bool SaveHistory(ArrayList histories, ArrayList historyItems, ArrayList otherSql)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                string sqlStr = "";
                for (int i = 0; i < otherSql.Count; i++)
                {
                    sqlStr += ";" + otherSql[i].ToString();
                }
                if (!string.IsNullOrEmpty(sqlStr))
                {
                    mySqlCmd.CommandText = sqlStr.Remove(0, 1).Replace(";;", ";");
                    mySqlCmd.ExecuteNonQuery();
                }
                for (int i = 0; i < histories.Count; i++)
                {
                    mySqlCmd.CommandText = histories[i].ToString();
                    int rowId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                    mySqlCmd.CommandText = string.Format(historyItems[i].ToString(), rowId);
                    mySqlCmd.ExecuteNonQuery();
                }
                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                mySqlCmd.Transaction.Rollback();
                throw;
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }

        //add by wwei0216w 2015/1/20
        /// <summary>
        /// 根據條件獲得批次號
        /// </summary>
        /// <param name="th">條件</param>
        public List<TableHistoryCustom> QueryBatchno(TableHistory th, out int total)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb2 =new StringBuilder();
//                sb.AppendFormat(@"SELECT DISTINCT th.batchno,mu.user_username,tb.kdate FROM t_table_history th
//                                      LEFT JOIN t_history_batch tb ON tb.batchno =  th.batchno
//                                      LEFT JOIN  manage_user mu ON mu.user_email = tb.kuser 
//                                  WHERE th.table_name = '{0}' AND th.pk_value = {1}", th.table_name, th.pk_value);
//                sb2.AppendFormat("    ORDER BY tb.kdate DESC LIMIT {0},{1}", th.Start, th.Limit); 
                //edit by wwei0216w 2015/6/18            注:之前語句查詢時間超過1秒,新語句查詢時間降至0.1秒 
                //edit by wwei0216w 2015/7/06 第二次修改 注:歷史記錄查詢 之前使用對應表的主鍵進行查詢,為查詢方便,現統一使用product_id,因此th.pk_value 為前臺text中輸入的值
                sb.AppendFormat(@"SELECT DISTINCT tb.batchno,mu.user_username,tb.kuser,tb.kdate FROM t_history_batch tb 
	                                  INNER JOIN t_table_history th ON th.batchno = tb.batchno AND th.table_name = '{0}' AND SUBSTRING_INDEX(th.batchno,'_',-1) = {1}
	                                  LEFT JOIN manage_user mu ON mu.user_id = tb.kuser 
                                  WHERE SUBSTRING_INDEX(tb.batchno,'_',-1) = {1} ", th.table_name, th.pk_value);
                sb2.AppendFormat(@" ORDER BY tb.kdate DESC LIMIT {0},{1}", th.Start, th.Limit);

                List<TableHistoryCustom> listSum = _dbAccess.getDataTableForObj<TableHistoryCustom>(sb.ToString());
                List<TableHistoryCustom> list = _dbAccess.getDataTableForObj<TableHistoryCustom>(sb.ToString() + sb2.ToString());
                total = listSum.Count;//獲得結果總數
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryDao-->QueryBatchno" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢t_table_history中出現的表名稱
        /// </summary>
        /// <returns>包含表名稱的TableHistory集合</returns>
        //add by wwei0216w 2015/1/22
        public List<TableHistory> QueryTableName()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("SELECT DISTINCT table_name FROM t_table_history");
                return _dbAccess.getDataTableForObj<TableHistory>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TableHistoryDao-->QueryTableName" + ex.Message,ex);
            }
        }
    }
}
