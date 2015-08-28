/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderSlaveDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 16:03:17 
 * chaojie_zz添加 GetOrderWaitDeliver方法于2014/12/01 05:40PM ,完成：出貨管理>供應商出貨單>待出貨訂單列表
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao
{
    public class OrderSlaveDao : IOrderSlaveImplDao
    {
        private IDBAccess _dbAccess;
        public OrderSlaveDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public string Save(BLL.gigade.Model.OrderSlave orderSlave)
        {
            orderSlave.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("insert into order_slave(`slave_id`,`order_id`,`vendor_id`,`slave_product_subtotal`,`slave_amount`,");
            strSql.Append("`slave_status`,`slave_note`,`slave_ipfrom`)values({0},{1},");
            strSql.AppendFormat("{0},{1},", orderSlave.Vendor_Id, orderSlave.Slave_Product_Subtotal);
            strSql.AppendFormat("{0},{1},'{2}','{3}')", orderSlave.Slave_Amount, orderSlave.Slave_Status, orderSlave.Slave_Note, orderSlave.Slave_Ipfrom);
            return strSql.ToString();
        }
        public int Delete(int slaveId)
        {
            StringBuilder strSql = new StringBuilder("delete from order_slave where slave_id=" + slaveId);
            return _dbAccess.execCommand(strSql.ToString());
        }

        #region 待出貨訂單+ List<OrderSlaveQuery> GetOrderWaitDeliver(OrderSlaveQuery store,string str ,out int totalCount)
        /// <summary>
        /// 出貨管理>供應商出貨單>待出貨訂單
        /// </summary>
        /// <param name="query">實體</param>
        /// <param name="str">拼接的SQL語句 查詢條件</param>
        /// <param name="totalCount">分頁顯示</param>
        /// <returns></returns>
        public List<OrderSlaveQuery> GetOrderWaitDeliver(OrderSlaveQuery query, string str, out int totalCount)
        {
            StringBuilder sqlColumn = new StringBuilder();
            StringBuilder sqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            StringBuilder sqlSort = new StringBuilder();

            totalCount = 0;
            try
            {
                sqlColumn.AppendLine(@"SELECT	os.order_id,v.vendor_name_simple,om.order_name, om.delivery_name,FROM_UNIXTIME(om.order_date_pay) as order_date_pay,");
                sqlColumn.AppendLine(@" v.dispatch,om.note_order ,om.delivery_mobile, order_payment,om.order_mobile,v.vendor_id, ");
                sqlColumn.AppendLine(@" om.delivery_zip,om.delivery_address,om.note_order ");
                sqlTable.AppendLine(@" from order_slave os ");
                sqlTable.AppendLine(@" inner join order_master om on os.order_id = om.order_id inner join vendor v on os.vendor_id = v.vendor_id ");
                sqlCondition.AppendLine(@" where 1=1 AND v.assist = 1 	 AND	os.slave_status = '2' ");
                if (!string.IsNullOrEmpty(str))
                {
                    sqlCondition.Append(str);
                }
                if (query.IsPage)
                {
                    string sql = "select count(os.order_id) as total_count " + sqlTable.ToString() + sqlCondition.ToString();
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sql);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["total_count"].ToString());
                    }
                    sqlSort.AppendLine(@"ORDER BY order_createdate DESC  ");
                    sqlSort.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                else
                {
                    sqlSort.AppendLine(@"ORDER BY order_createdate DESC  ");
                }
                string sqlstr = sqlColumn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlSort.ToString();
                return _dbAccess.getDataTableForObj<OrderSlaveQuery>(sqlstr);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveDao-->GetOrderWaitDeliver-->" + ex.Message + sqlColumn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlSort.ToString(), ex);
            }
        }

        #endregion
        /// <summary>
        /// 供應商後台>訂單管理>供應商自行出貨列表
        /// </summary>
        /// <param name="store"></param>
        /// <param name="str"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<OrderSlaveQuery> GetVendorWaitDeliver(OrderSlaveQuery store, string str, out int totalCount)
        {
            StringBuilder sqlClomn = new StringBuilder();
            StringBuilder sqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            StringBuilder sqlGroup = new StringBuilder();
            StringBuilder sqlSort = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlClomn.AppendLine(@"SELECT	os.order_id,os.slave_status,os.slave_id,om.order_name, om.delivery_name,FROM_UNIXTIME(om.order_date_pay) as order_date_pay,");
                sqlClomn.AppendLine(@" om.delivery_mobile, order_payment,om.order_mobile, om.delivery_store,om.estimated_arrival_period,om.holiday_deliver, ");
                sqlClomn.AppendLine(@" om.delivery_zip,om.delivery_address,om.note_order ");
                sqlTable.AppendLine(@" from order_slave os ");
                sqlTable.AppendLine(@" INNER JOIN order_master om USING (order_id)INNER JOIN order_detail od USING (slave_id) ");
                sqlCondition.AppendFormat(@" where 1=1 and os.vendor_id='{0}' and od.product_mode=1 and od.detail_status=2 ", store.vendor_id);
                sqlGroup.AppendLine(@" GROUP BY os.slave_id  ");
                if (!string.IsNullOrEmpty(str))
                {
                    sqlCondition.Append(str);
                }

                if (store.IsPage)
                {
                    string sql = "select os.order_id as total_count " + sqlTable.ToString() + sqlCondition.ToString() + sqlGroup.ToString();
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sql);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sqlSort.AppendLine(@"  ORDER BY order_createdate DESC ");
                    sqlSort.AppendFormat(" limit {0},{1};", store.Start, store.Limit);
                }
                string sqlstr = sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlGroup.ToString() + sqlSort.ToString();
                return _dbAccess.getDataTableForObj<OrderSlaveQuery>(sqlstr);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveDao-->GetVendorWaitDeliver-->" + ex.Message + sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlGroup.ToString() + sqlSort.ToString(), ex);
            }
        }
        /// <summary>
        /// 後臺管理>供應商待出貨訂單
        /// </summary>
        /// <param name="store"></param>
        /// <param name="str"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<OrderSlaveQuery> GetAllOrderWait(OrderSlaveQuery store, string str, out int totalCount)
        {
            StringBuilder sqlClomn = new StringBuilder();
            StringBuilder sqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            StringBuilder sqlGroup = new StringBuilder();
            StringBuilder sqlSort = new StringBuilder();
            totalCount = 0;
            try
            {//
                sqlClomn.AppendLine(@"SELECT	os.slave_id as Slave_Id,FROM_UNIXTIME(om.order_date_pay) as order_date_pay ,order_payment,os.order_id ,om.order_name, ");
                sqlClomn.AppendLine(@" om.delivery_name,os.slave_status,om.note_order ,os.order_id,om.delivery_mobile,om.order_mobile ");
                sqlTable.AppendLine(@"  FROM	order_slave os, order_master om ,order_detail od  ");
                sqlCondition.AppendLine(@"  WHERE os.order_id = om.order_id AND os.slave_id = od.slave_id  ");
                sqlCondition.AppendLine(@" AND od.detail_status = 2	AND od.product_mode IN (2 , 3) AND os.slave_status = 2 ");
                if (store.vendor_id != 0)
                {
                    sqlCondition.AppendFormat(@" AND os.vendor_id = '{0}' ", store.vendor_id);
                }

                if (!string.IsNullOrEmpty(str))
                {
                    sqlCondition.Append(str);
                }
                sqlGroup.Append(@" GROUP BY os.slave_id");
                if (store.IsPage)
                {
                    string sql = "select count(totalcount) as total from (select count(os.slave_id) as totalcount  " + sqlTable.ToString() + sqlCondition.ToString() + sqlGroup.ToString() + " )t";
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sql);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["total"].ToString());
                    }
                    sqlSort.AppendLine(@"  ORDER BY om.order_date_pay DESC ");
                    sqlSort.AppendFormat(" limit {0},{1};", store.Start, store.Limit);
                }
                return _dbAccess.getDataTableForObj<OrderSlaveQuery>(sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlGroup.ToString() + sqlSort.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveDao-->GetAllOrderWait-->" + ex.Message + sqlClomn.ToString() + sqlTable.ToString() + sqlCondition.ToString() + sqlGroup.ToString() + sqlGroup.ToString(), ex);
            }
        }
        /// <summary>
        /// chaojie1124j_zz添加于2014/12/27
        /// 實現通過slave_id查詢一個時間。
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public OrderSlaveQuery GetOrderDatePay(OrderSlaveQuery query)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@" select FROM_UNIXTIME(om.order_date_pay) as order_date_pay from order_slave os left join order_master om on os.order_id=om.order_id  where 1=1 ");
            try
            {
                if (query.Slave_Id != 0)
                {
                    sql.AppendFormat(" and slave_id='{0}' ", query.Slave_Id);
                }
                return _dbAccess.getSinggleObj<OrderSlaveQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveDao-->GetOrderDatePay-->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataTable GetList(OrderSlaveQuery query,out int totalCount)
        {
            StringBuilder sbSqlColumn = new StringBuilder();
            StringBuilder sbSqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            StringBuilder sqlSort = new StringBuilder();
            string sqlStr = string.Empty;

            totalCount = 0;
            try
            {
                sbSqlColumn.Append(@"select	os.slave_id,os.order_id,os.vendor_id,os.slave_freight_normal,os.slave_freight_low,os.slave_product_subtotal,os.slave_amount,os.slave_status,os.slave_note,os.slave_date_delivery,os.slave_date_cancel,os.slave_date_return,os.slave_date_close,os.account_status,os.slave_updatedate,os.slave_ipfrom,om.order_name,om.order_gender,om.order_mobile,om.order_zip,om.order_address,om.delivery_same,om.delivery_name,om.delivery_gender,om.delivery_mobile,om.delivery_zip,om.delivery_address,om.note_order,FROM_UNIXTIME(om.order_date_pay) as order_date_pay,om.estimated_arrival_period,FROM_UNIXTIME(om.order_createdate) as order_createdate ");
                sbSqlTable.Append(@" from order_slave os ");
                sbSqlTable.Append(@" inner join order_master om on os.order_id = om.order_id ");
                sqlCondition.Append(@" where 1=1 ");

                if (query.order_id != 0)
                {
                    sqlCondition.AppendFormat(" and os.order_id='{0}'", query.order_id);
                }
                if (query.vendor_id!=0)
                {
                    sqlCondition.AppendFormat(" and os.vendor_id='{0}'", query.vendor_id);
                }
                if (query.Slave_Status!=0)
                {
                    sqlCondition.AppendFormat(" and os.slave_status='{0}'", query.Slave_Status);
                }
                if (query.IsPage)
                {
                    string sql = "select count(os.order_id) as total_count " + sbSqlTable.ToString() + sqlCondition.ToString();
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sql);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0]["total_count"].ToString());
                    }
                    sqlSort.AppendLine(@" order by order_createdate asc ");
                    sqlSort.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                else
                {
                    sqlSort.AppendLine(@" order by order_createdate asc ");
                }
                sqlStr = sbSqlColumn.ToString() + sbSqlTable.ToString() + sqlCondition.ToString() + sqlSort.ToString();
                return _dbAccess.getDataTable(sqlStr);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveDao-->GetList-->" + ex.Message + sqlStr, ex);
            }
        }
        public DataTable GetListPrint(OrderSlaveQuery query, string addsql=null)
        {
            StringBuilder sbSqlColumn = new StringBuilder();
            StringBuilder sbSqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            StringBuilder sqlSort = new StringBuilder();
            string sqlStr = string.Empty;
            try
            {
                sbSqlColumn.Append(@"select	os.slave_id,os.order_id,os.vendor_id,os.slave_freight_normal,os.slave_freight_low,os.slave_product_subtotal,os.slave_amount,os.slave_status,os.slave_note,os.slave_date_delivery,os.slave_date_cancel,os.slave_date_return,os.slave_date_close,os.account_status,os.slave_updatedate,os.slave_ipfrom,om.order_name,om.order_gender,om.order_mobile,om.order_zip,om.order_address,om.delivery_same,om.delivery_name,om.delivery_gender,om.delivery_mobile,om.delivery_zip,om.delivery_address,om.note_order,FROM_UNIXTIME(om.order_date_pay) as order_date_pay,om.estimated_arrival_period,FROM_UNIXTIME(om.order_createdate) as order_createdate ");
                sbSqlTable.Append(@" from order_slave os ");
                sbSqlTable.Append(@" inner join order_master om on os.order_id = om.order_id ");
                sbSqlTable.Append(@" INNER JOIN order_detail od  on os.slave_id = od.slave_id 	AND	os.slave_status = 2 AND od.detail_status = 2 AND product_mode = 1");
                sqlCondition.Append(@" where 1=1 ");
              
                if (query.vendor_id != 0)
                {
                    sqlCondition.AppendFormat(" and os.vendor_id='{0}'", query.vendor_id);
                }
                if (!string.IsNullOrEmpty(addsql))
                {
                    sqlCondition.Append(addsql);
                }
                if (query.Slave_Status != 0)
                {
                    sqlCondition.AppendFormat(" and os.slave_status='{0}'", query.Slave_Status);
                }

                sqlSort.AppendLine(@" GROUP BY os.slave_id order by order_createdate asc ");
           
                sqlStr = sbSqlColumn.ToString() + sbSqlTable.ToString() + sqlCondition.ToString() + sqlSort.ToString();
                return _dbAccess.getDataTable(sqlStr);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveDao-->GetListPrint-->" + ex.Message + sqlStr, ex);
            }
        }

     
    }
}