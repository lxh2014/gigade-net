using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao.Impl;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class InvoiceWinningNumberDao 
    {
        private IDBAccess _access;
        private string connection;
        public InvoiceWinningNumberDao(string connectionstring)
        {
            connection = connectionstring;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }

        public string ReturnInsertSql(int year, int month, string code, string code_name)
        {
            string result = string.Empty;
           
            StringBuilder str = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(code_name))//是否為空
                {
                    str.AppendFormat(@"SELECT row_id as mycount FROM invoice_winning_number WHERE year='{0}' and month='{1}' and winning_type='{2}';", year, month, code);
                    sqlstr.AppendFormat(@"insert into invoice_winning_number(year,month,winning_type,winning_value,winning_status)values('{0}','{1}','{2}','{3}','{4}');", year, month, code, code_name, 1);
                    DataTable _dt = _access.getDataTable(str.ToString());
                    if (_dt.Rows.Count <= 0)//表示數據庫中該月數據不存在
                    {
                        result = sqlstr.ToString();
                    }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceWinningNumberDao-->ReturnInsertSql-->" + ex.Message + str.ToString(), ex);
            }
        }

        public int ResultOfExeInsertSql(string sql)
        {
            int result = 0;
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connection);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = sql.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
                return result;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();               
                throw new Exception("InvoiceWinningNumberDao-->ResultOfExeInsertSql-->" + ex.Message + sql.ToString(), ex);                              
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }
    }
}
