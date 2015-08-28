/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：MySqlDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/28 17:18:20 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;

namespace BLL.gigade.Dao
{
    public class MySqlDao
    {
        private MySqlConnection mySqlConn;
        public MySqlDao(string connectionStr)
        {
            try
            {
                if (mySqlConn == null)
                {
                    mySqlConn = new MySqlConnection(connectionStr);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 打開連接
        /// </summary>
        protected void OpenConn()
        {
            try
            {
                if (mySqlConn != null && mySqlConn.State == ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 關閉連接
        /// </summary>
        protected void CloseConn()
        {
            try
            {
                if (mySqlConn != null && mySqlConn.State == ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 執行一組sql 返回執行結果
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExcuteSqls(ArrayList sqls)
        {
            this.OpenConn();
            MySqlCommand mySqlCmd = new MySqlCommand();
            try
            {
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                for (int i = 0; i < sqls.Count; i++)
                {
                    mySqlCmd.CommandText = sqls[i].ToString();
                    mySqlCmd.ExecuteNonQuery();
                }
                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                mySqlCmd.Transaction.Rollback();
                return false;
                throw;
            }
            finally
            {
                this.CloseConn();
            }
        }


        /// <summary>
        /// 執行sql語句,如果sql有錯拋出異常
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExcuteSqlsThrowException(ArrayList sqls)
        {
            this.OpenConn();
            MySqlCommand mySqlCmd = new MySqlCommand();
            try
            {
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                for (int i = 0; i < sqls.Count; i++)
                {
                    mySqlCmd.CommandText = sqls[i].ToString();
                    mySqlCmd.ExecuteNonQuery();
                }
                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                this.CloseConn();
            }
        }
    }
}
