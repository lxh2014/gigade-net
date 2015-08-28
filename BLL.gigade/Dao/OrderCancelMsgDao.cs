using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using MySql.Data.MySqlClient;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class OrderCancelMsgDao:IOrderCancelMsgImplDao
    {
        private IDBAccess _accessMySql;
        public string connString;
        SerialDao _serialDao = new SerialDao("");
        public OrderCancelMsgDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }
        #region 取消訂單通知列表
        /// <summary>
        /// 取消訂單通知列表
        /// </summary>
        /// <param name="ocm"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<OrderCancelMsgQuery> Query(OrderCancelMsgQuery ocm, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"select ocm.cancel_id,ocm.order_id,ocm.cancel_type,ocm.cancel_status,ocm.cancel_content,");
            sql.AppendLine(@"FROM_UNIXTIME(ocm.cancel_createdate) as cancel_createdate,ocm.cancel_ipfrom,");
            sql.AppendLine(@"om.order_amount,om.order_status,om.order_payment,om.order_name,om.order_mobile,u.user_email ");
            sql.AppendLine(@" from	order_cancel_msg  ocm,order_master  om, users  u");
            sql.AppendLine(@" where	ocm.cancel_status = 0 and	ocm.order_id = om.order_id and om.user_id = u.user_id ");
            sql.AppendLine(@" order by cancel_id asc");
            //分頁
            totalCount = 0;
            try
            {
                if (ocm.IsPage)
                {
                    System.Data.DataTable _dt = _accessMySql.getDataTable(sql.ToString());

                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }

                    sql.AppendFormat(" limit {0},{1}", ocm.Start, ocm.Limit);
                }
                return _accessMySql.getDataTableForObj<OrderCancelMsgQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" OrderCancelMsgDao-->Query-->" + ex.Message + sql.ToString() + sql.ToString(), ex);
            }
        }
        #endregion
        #region 對用戶的問題與意見進行回覆
        /// <summary>
        /// 對用戶的問題與意見進行回覆
        /// </summary>
        /// <param name="cancel_id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int Reply(OrderCancelResponse ocr)
        {
            string sql = string.Empty;
            int i = 0;
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
                sql = string.Format("update	order_cancel_msg set cancel_status = 1 where cancel_id ={0} and	cancel_status = 0", ocr.cancel_id);
                mySqlCmd.CommandText = sql;
                i += mySqlCmd.ExecuteNonQuery();
                _serialDao = new SerialDao(connString);
                mySqlCmd.CommandText = _serialDao.Update(37);
                string response_id = mySqlCmd.ExecuteScalar().ToString();
                sql = string.Format("insert into order_cancel_response(response_id,cancel_id,user_id,response_content,response_createdate,response_ipfrom)");
                sql += string.Format(" values({0},{1},{2},'{3}',{4},'{5}')", response_id, ocr.cancel_id,ocr.user_id,ocr.response_content,CommonFunction.GetPHPTime(DateTime.Now.ToString()), ocr.response_ipfrom);
                mySqlCmd.CommandText = sql;
                i += mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderCancelMsgDao-->Reply-->" + ex.Message+sql.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return i;

        }
        #endregion
    }
}
