/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderMasterDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 16:02:17 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Dao.Impl;
using MySql.Data.MySqlClient;
using System.Collections;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Common;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    class OrderMasterDao : IOrderMasterImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        private SerialDao _serial;
        public OrderMasterDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            _serial = new SerialDao(connectionStr);
            this.connStr = connectionStr;
        }
        /// <summary>
        /// 根據order_id查詢訂單狀態
        /// </summary>
        /// <param name="om"></param>
        /// <returns></returns>
        public OrderMaster GetOrderMaster(OrderMaster om)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append(@"select order_id,order_status from order_master 
            om where 1=1");
            if (0 != om.Order_Id)
            {
                sbSql.AppendFormat(" and om.order_id={0}", om.Order_Id);
            }
            return _dbAccess.getSinggleObj<OrderMaster>(sbSql.ToString());
        }
        /// <summary>
        /// 修改訂單狀態
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int OrderStatusModify(string cmdSql)
        {
            int id = 0;
            StringBuilder sb = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    mySqlConn.Open();
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = cmdSql;
                id = Convert.ToInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.Transaction.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.OrderStatusModify -->" + ex.Message + cmdSql.ToString(), ex);
            }
        }
        /// <summary>
        /// 插入訂單主表sql
        /// </summary>
        /// <param name="orderMaster"></param>
        /// <returns></returns>
        public string Save(BLL.gigade.Model.OrderMaster orderMaster)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                orderMaster.Replace4MySQL();
                strSql.Append("insert into order_master(`order_id`,`user_id`,`order_gender`,`delivery_gender`,`bonus_receive`,`deduct_happygo_convert`,`deduct_bonus`,`deduct_welfare`,");
                strSql.Append("`deduct_account`,`order_freight_normal`,`order_freight_low`,`order_product_subtotal`,`order_amount`,");
                strSql.Append("`order_status`,`order_payment`,`order_name`,`order_mobile`,`order_phone`,`order_address`,`delivery_name`,`delivery_mobile`,");
                strSql.Append("`delivery_phone`,`delivery_zip`,`delivery_address`,`estimated_arrival_period`,`company_invoice`,`company_title`,");
                strSql.Append("`invoice_id`,`order_invoice`,`invoice_status`,`note_order`,`note_admin`,`order_date_pay`,`order_createdate`,`order_updatedate`,");
                strSql.Append("`order_ipfrom`,`source_trace`,`source_cookie_value`,`source_cookie_name`,`note_order_modifier`,`note_order_modify_time`,`error_check`,`channel`,");
                strSql.Append("`channel_order_id`,`delivery_store`,`billing_checked`,`order_zip`,`retrieve_mode`,`holiday_deliver`,`import_time`,`export_flag`,`cart_id`,`accumulated_bonus`)values({0},");
                strSql.AppendFormat("{0},{1},{2},{3},", orderMaster.User_Id, orderMaster.Order_Gender, orderMaster.Delivery_Gender, orderMaster.Bonus_Receive);//add by wwei0216w 添加訂購人和收件人性別orderMaster.Order_Gender,orderMaster.Delivery_Gender 2015/1/21
                strSql.AppendFormat("{0},{1},{2},{3},", orderMaster.Deduct_Happygo_Convert, orderMaster.Deduct_Bonus, orderMaster.Deduct_Welfare, orderMaster.Deduct_Account);
                strSql.AppendFormat("{0},{1},{2},{3},{4},", orderMaster.Order_Freight_Normal, orderMaster.Order_Freight_Low, orderMaster.Order_Product_Subtotal, orderMaster.Order_Amount, orderMaster.Order_Status);
                strSql.AppendFormat("{0},'{1}','{2}','{3}','{4}',", orderMaster.Order_Payment, orderMaster.Order_Name, orderMaster.Order_Mobile, orderMaster.Order_Phone, orderMaster.Order_Address);
                strSql.AppendFormat("'{0}','{1}','{2}',{3},'{4}',", orderMaster.Delivery_Name, orderMaster.Delivery_Mobile, orderMaster.Delivery_Phone, orderMaster.Delivery_Zip, orderMaster.Delivery_Address);
                strSql.AppendFormat("{0},'{1}','{2}',{3},'{4}',", orderMaster.Estimated_Arrival_Period, orderMaster.Company_Invoice, orderMaster.Company_Title, orderMaster.Invoice_Id, orderMaster.Order_Invoice);
                strSql.AppendFormat("{0},'{1}','{2}',{3},{4},", orderMaster.Invoice_Status, orderMaster.Note_Order, orderMaster.Note_Admin, orderMaster.Order_Date_Pay, orderMaster.Order_Createdate);
                strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", orderMaster.Order_Updatedate, orderMaster.Order_Ipfrom, orderMaster.Source_Trace, orderMaster.Source_Cookie_Value, orderMaster.Source_Cookie_Name);
                strSql.AppendFormat("{0},'{1}',{2},'{3}','{4}',", orderMaster.Note_Order_Modifier, orderMaster.Note_Order_Modify_Time, orderMaster.Error_Check, orderMaster.Channel, orderMaster.Channel_Order_Id);
                strSql.AppendFormat("{0},{1},{2},{3},{4},", orderMaster.Delivery_Store, orderMaster.Billing_Checked, orderMaster.Order_Zip, orderMaster.Retrieve_Mode, orderMaster.Holiday_Deliver);
                strSql.AppendFormat(orderMaster.Import_Time == DateTime.MinValue ? "null)" : "'" + orderMaster.Import_Time.ToString("yyyy/MM/dd HH:mm:ss") + "',{0},{1},{2})", orderMaster.Export_Flag, orderMaster.Cart_Id, orderMaster.Accumulated_Bonus);//edit by zhuoqin0830w 2015/09/01 添加 accumulated_bonus 欄位
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.Save -->" + ex.Message + strSql.ToString(), ex);
            }
        }
        /// <summary>
        /// 根據order_id刪除order_master數據
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public int Delete(int orderId)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("delete from order_master where order_id='{0}'", orderId);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.Delete -->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public bool SaveOrder(string orderMaster, string orderMasterPattern, string orderPayment, ArrayList orderSlaves, ArrayList orderDetails, ArrayList otherSqls, string bonusMaster, string bonusRecord)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            SerialDao serialDao = new SerialDao("");
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;

                #region OrderMaster

                string yearDay = DateTime.Now.ToString("yy") + Common.CommonFunction.Supply(DateTime.Now.DayOfYear.ToString(), "0", 3);
                int firstPart = Convert.ToInt32(yearDay) - 1;//當前天數減1天，同PHP系統

                mySqlCmd.CommandText = serialDao.Update(firstPart);
                long orderId = Convert.ToInt64(firstPart + Common.CommonFunction.Supply(mySqlCmd.ExecuteScalar().ToString(), "0", 4));

                mySqlCmd.CommandText = string.Format(orderMaster, orderId);
                mySqlCmd.ExecuteNonQuery();

                #endregion

                #region orderMasterPattern
                if (!string.IsNullOrEmpty(orderMasterPattern))
                {
                    mySqlCmd.CommandText = string.Format(orderMasterPattern, orderId);
                    mySqlCmd.ExecuteNonQuery();
                }
                #endregion

                #region bonusMaster  add by zhuoqin0830w 2015/08/24
                if (!string.IsNullOrEmpty(bonusMaster))
                {
                    mySqlCmd.CommandText = string.Format(bonusMaster);
                    mySqlCmd.ExecuteNonQuery();
                }
                #endregion

                #region bonusRecord  add by zhuoqin0830w 2015/08/25
                if (!string.IsNullOrEmpty(bonusRecord))
                {
                    mySqlCmd.CommandText = string.Format(bonusRecord, orderId);
                    mySqlCmd.ExecuteNonQuery();
                }
                #endregion

                #region 華南賬戶（虛擬帳號）  add by zhuoqin0830w  2015/05/13
                if (!string.IsNullOrEmpty(orderPayment))
                {
                    mySqlCmd.CommandText = string.Format(orderPayment, orderId);
                    mySqlCmd.ExecuteNonQuery();
                }
                #endregion

                #region OrderSlave OrderDetail

                for (int i = 0; i < orderSlaves.Count; i++)
                {
                    mySqlCmd.CommandText = serialDao.Update(30);//30為order_slave在serial的內編號
                    int slaveId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                    mySqlCmd.CommandText = string.Format(orderSlaves[i].ToString(), slaveId, orderId);
                    mySqlCmd.ExecuteNonQuery();

                    foreach (var detail in orderDetails[i] as ArrayList)
                    {
                        mySqlCmd.CommandText = serialDao.Update(32);//32為order_detail在serial的內編號
                        int detailId = Convert.ToInt32(mySqlCmd.ExecuteScalar());

                        mySqlCmd.CommandText = string.Format(detail.ToString(), detailId, slaveId);
                        mySqlCmd.ExecuteNonQuery();
                    }
                }
                #endregion

                #region OtherSqls

                if (otherSqls != null)
                {
                    foreach (var sql in otherSqls)
                    {
                        mySqlCmd.CommandText = sql.ToString();
                        mySqlCmd.ExecuteNonQuery();
                    }
                }
                #endregion

                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderMasterDao.SaveOrder-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }
        /// <summary>
        /// 根據訂單號查詢訂單支付方式
        /// </summary>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public OrderMaster GetPaymentById(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select order_id,order_payment  from order_master where order_id='{0}';", order_id);
                return _dbAccess.getSinggleObj<OrderMaster>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.GetPaymentById -->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 訂單查詢sql
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sqladdstr"></param>
        /// <param name="totalCount"></param>
        /// <param name="addSerch"></param>
        /// <returns></returns>
        public List<OrderMasterQuery> getOrderSearch(OrderMasterQuery query, string sqladdstr, out int totalCount, string addSerch)
        {
            StringBuilder sqlClm = new StringBuilder();
            StringBuilder sqljoin = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sqlClm.AppendFormat(@"SELECT DISTINCT om.order_id,u.user_id,channel_name_simple as channel_name_full,om.deduct_happygo_convert,om.export_flag,om.order_name,om.delivery_name,om.order_amount,om.order_payment,om.order_status,om.order_createdate,om.channel as channel_id,om.note_admin,om.source_trace,om.deduct_welfare,om.deduct_happygo,om.deduct_bonus,u.user_email,imr.invoice_number,redirect.redirect_name as redirect_name,redirect.redirect_url as redirect_url,u.user_id,user_password,user_name,user_gender,user_mobile,user_phone,user_zip,user_address,user_reg_date , CASE user_type  when '1' THEN '網路會員' else'電話會員' END as mytype ,send_sms_ad,adm_note ,concat(user_birthday_year,'/',user_birthday_month,'/',user_birthday_day) as birthday");
                string sqlcount = "select count(DISTINCT om.order_id) AS search_total ";
                sqljoin.AppendFormat(@" FROM order_master om INNER JOIN users u ON u.user_id = om.user_id LEFT JOIN invoice_master_record imr ON om.order_id =  imr.order_id LEFT  JOIN channel on om.channel=channel.channel_id left join redirect on redirect.redirect_id=om.source_trace");
                sqljoin.AppendFormat(" LEFT JOIN deliver_master dm ON dm.order_id = om.order_id ");
                sqlwhere.AppendFormat(" WHERE om.user_id = u.user_id ");
                #region where
                if (query.group_id > 0)
                {
                    sqlwhere.AppendFormat(" AND vu.group_id = '{0}'", query.group_id);
                    sqljoin.Append("  INNER JOIN vip_user vu ON om.user_id =  vu.user_id ");
                }
                if (query.invoice != 0)
                {
                    sqlwhere.AppendFormat("  and imr.invoice_id IS NULL AND (om.money_cancel + om.money_return) != om.order_amount");
                }
                if (query.Channel > 0)
                {
                    sqlwhere.AppendFormat(" AND  om.channel = '{0}'", query.Channel);
                }
                if (query.pay_status > 0)
                {
                    switch (query.pay_status)
                    {
                        case 1:
                            sqlwhere.AppendFormat(" and om.money_collect_date <> 0 ");
                            break;
                        case 2:
                            sqlwhere.AppendFormat(" and om.money_collect_date = 0 ");
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(query.searchcon))
                {
                    switch (query.selecttype)
                    {
                        case "1":
                            sqlwhere.AppendFormat(" AND om.order_id LIKE '%{0}%'", query.searchcon);
                            break;
                        case "2":
                            sqlwhere.AppendFormat(" AND om.order_name LIKE '%{0}%'", query.searchcon);
                            break;
                        case "3":
                            sqlwhere.AppendFormat(" AND u.user_email LIKE '%{0}%'", query.searchcon);
                            break;
                        case "4":
                            sqlwhere.AppendFormat(" AND om.delivery_name LIKE '%{0}%'", query.searchcon);
                            break;
                        case "5":
                            sqlwhere.AppendFormat(" AND om.source_trace LIKE '%{0}%'", query.searchcon);
                            break;
                        case "6":
                            sqlwhere.AppendFormat(" AND om.order_mobile LIKE '%{0}%'", query.searchcon);
                            break;
                        case "7":
                            sqlwhere.AppendFormat(" AND om.channel_order_id LIKE '%{0}%'", query.searchcon);
                            break;
                        case "8":
                            sqlwhere.AppendFormat(" AND om.delivery_mobile LIKE '%{0}%'", query.searchcon);
                            break;
                        case "9":
                            sqlwhere.AppendFormat(" AND (om.order_address LIKE '%{0}%' or om.delivery_address LIKE '%{0}%') ", query.searchcon);
                            break;
                        case "10":
                            sqlwhere.AppendFormat(" AND ( om.order_phone LIKE '%{0}%' or m.delivery_phone LIKE  '%{0}%') ", query.searchcon);
                            break;
                        case "11":
                            sqlwhere.AppendFormat(" AND dm.delivery_code LIKE '%{0}%'", query.searchcon);
                            break;
                        default:
                            break;
                    }
                }

                switch (query.dateType)
                {
                    case 1:
                        if (query.datestart > DateTime.MinValue && query.dateend > DateTime.MinValue)
                        {
                            sqlwhere.AppendFormat(" AND om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(query.datestart.ToString("yyyy-MM-dd HH:mm:ss")));
                            sqlwhere.AppendFormat(" AND om.order_createdate <= '{0}'", CommonFunction.GetPHPTime(query.dateend.ToString("yyyy-MM-dd HH:mm:ss")));
                        }
                        break;
                    case 2:
                        if (query.datestart > DateTime.MinValue && query.dateend > DateTime.MinValue)
                        {
                            sqlwhere.AppendFormat(" AND (om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(query.datestart.ToString("yyyy-MM-dd HH:mm:ss")));
                            sqlwhere.AppendFormat(" AND om.order_createdate <= '{0}' or", CommonFunction.GetPHPTime(query.dateend.ToString("yyyy-MM-dd HH:mm:ss")));
                            sqlwhere.AppendFormat("  om.money_collect_date >= '{0}' ", CommonFunction.GetPHPTime(query.datestart.ToString("yyyy-MM-dd HH:mm:ss")));
                            sqlwhere.AppendFormat(" AND om.money_collect_date <= '{0}') ", CommonFunction.GetPHPTime(query.dateend.ToString("yyyy-MM-dd HH:mm:ss")));
                        }
                        break;
                    default:
                        break;
                }
                if (query.dateType == 1)
                {
                    if (query.datestart > DateTime.MinValue)
                    {
                        sqlwhere.AppendFormat(" AND om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(query.datestart.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                    if (query.dateend > DateTime.MinValue)
                    {
                        sqlwhere.AppendFormat(" AND om.order_createdate <= '{0}'", CommonFunction.GetPHPTime(query.dateend.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                }
                if (!string.IsNullOrEmpty(query.orderStatus) && query.orderStatus != "-1")
                {
                    sqlwhere.AppendFormat(" and om.order_status = {0} ", query.orderStatus);
                    // AND imr.invoice_id IS NULL AND (om.money_cancel + om.money_return) != om.order_amount
                }
                if (query.Order_Payment > 0)
                {
                    sqlwhere.AppendFormat(" and om.order_payment={0}", query.Order_Payment);
                }
                #endregion
                //sqljoin.AppendFormat(sqladdstr);
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sqlcount + sqljoin.ToString() + sqlwhere.ToString());
                    if (Convert.ToInt32(_dt.Rows[0][0]) > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sqlwhere.AppendFormat(" GROUP BY om.order_id  ORDER BY om.order_id DESC limit {0},{1}", query.Start, query.Limit);
                }
                return _dbAccess.getDataTableForObj<OrderMasterQuery>(sqlClm.ToString() + sqljoin.ToString() + sqlwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.getOrderSearch -->" + ex.Message + sqlClm.ToString() + sqljoin.ToString() + sqlwhere.ToString(), ex);
            }
        }
        /// <summary>
        /// 訂單匯出Excel-sql
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sqladdstr"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<OrderMasterQuery> Export(OrderMasterQuery query, string sqladdstr, out int totalCount)
        {
            StringBuilder sqlClm = new StringBuilder();
            StringBuilder sqljoin = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            try
            {
                sqlClm.AppendFormat(@" SELECT DISTINCT om.deduct_card_bonus,om.money_cancel,om.money_return,om.deduct_happygo_convert,om.order_date_pay,u.user_id,om.order_freight_low,om.order_freight_normal,om.order_mobile,om.order_id,channel_name_simple as channel_name_full,om.order_name,om.delivery_name,om.order_amount,om.order_payment,om.order_status,om.order_createdate, ");
                sqlClm.AppendFormat(" om.note_admin,om.deduct_welfare,om.deduct_happygo,om.deduct_bonus  ");
                sqlClm.AppendFormat(" ,u.user_email,redirect.redirect_name as redirect_name,ct.offsetamt ,rg.group_name,td.utm_id,td.utm_source,oph.hncb_id,om.source_trace,om.channel_order_id ");//imr.invoice_number,           
                sqljoin.AppendFormat(@" FROM	order_master om INNER JOIN users u ON u.user_id = om.user_id 
              LEFT  JOIN channel on om.channel=channel.channel_id left join redirect on redirect.redirect_id=om.source_trace 
               LEFT JOIN order_payment_ct ct ON ct.lidm = om.order_id   LEFT JOIN redirect_group rg  ON redirect.group_id = rg.group_id  LEFT JOIN track_detail td ON om.source_cookie_value = td.detail_id
               LEFT JOIN order_payment_hncb oph  ON oph.order_id = om.order_id LEFT JOIN deliver_master dm ON dm.order_id = om.order_id  ");//  ,om.source_trace,om.channel as channel_id,
                sqlwhere.AppendFormat(" WHERE 1=1 and om.user_id = u.user_id ");
                #region where
                if (query.group_id > 0)
                {
                    sqlwhere.AppendFormat(" AND vu.group_id = '{0}'", query.group_id);

                    sqljoin.Append(" INNER JOIN vip_user vu ON om.user_id =  vu.user_id ");
                }
                if (query.invoice != 0)
                {
                    sqlwhere.AppendFormat("  and imr.invoice_id IS NULL AND (om.money_cancel + om.money_return) != om.order_amount");
                    sqljoin.Append(" LEFT JOIN invoice_master_record imr ON om.order_id =  imr.order_id ");
                }
                if (query.Channel > 0)
                {
                    sqlwhere.AppendFormat(" AND  om.channel = '{0}'", query.Channel);
                }
                if (query.pay_status > 0)
                {
                    switch (query.pay_status)
                    {
                        case 1:
                            sqlwhere.AppendFormat(" and om.money_collect_date <> 0 ");
                            break;
                        case 2:
                            sqlwhere.AppendFormat(" and om.money_collect_date = 0 ");
                            break;
                        default:
                            break;
                    }
                }
                if (!string.IsNullOrEmpty(query.searchcon))
                {
                    switch (query.selecttype)
                    {
                        case "1":
                            sqlwhere.AppendFormat(" AND om.order_id LIKE '%{0}%'", query.searchcon);

                            break;
                        case "2":
                            sqlwhere.AppendFormat(" AND om.order_name LIKE '%{0}%'", query.searchcon);
                            break;
                        case "3":
                            sqlwhere.AppendFormat(" AND u.user_email LIKE '%{0}%'", query.searchcon);
                            break;
                        case "4":
                            sqlwhere.AppendFormat(" AND om.delivery_name LIKE '%{0}%'", query.searchcon);
                            break;
                        case "5":
                            sqlwhere.AppendFormat(" AND om.source_trace LIKE '%{0}%'", query.searchcon);
                            break;
                        case "6":
                            sqlwhere.AppendFormat(" AND om.order_mobile LIKE '%{0}%'", query.searchcon);
                            break;
                        case "7":
                            sqlwhere.AppendFormat(" AND om.channel_order_id LIKE '%{0}%'", query.searchcon);
                            break;
                        case "8":
                            sqlwhere.AppendFormat(" AND om.delivery_mobile LIKE '%{0}%'", query.searchcon);
                            break;
                        case "9":
                            sqlwhere.AppendFormat(" AND (om.order_address LIKE '%{0}%' or om.delivery_address LIKE '%{0}%') ", query.searchcon);
                            break;
                        case "10":
                            sqlwhere.AppendFormat(" AND ( om.order_phone LIKE '%{0}%' or m.delivery_phone LIKE  '%{0}%') ", query.searchcon);
                            break;
                        case "11":
                            sqlwhere.AppendFormat(" AND dm.delivery_code LIKE '%{0}%'", query.searchcon);
                            break;
                        default:
                            break;
                    }
                }
                switch (query.dateType)
                {
                    case 1:
                        if (query.datestart > DateTime.MinValue && query.dateend > DateTime.MinValue)
                        {
                            sqlwhere.AppendFormat(" AND om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(query.datestart.ToString("yyyy-MM-dd HH:mm:ss")));
                            sqlwhere.AppendFormat(" AND om.order_createdate <= '{0}'", CommonFunction.GetPHPTime(query.dateend.ToString("yyyy-MM-dd HH:mm:ss")));
                        }
                        break;
                    case 2:
                        if (query.datestart > DateTime.MinValue && query.dateend > DateTime.MinValue)
                        {
                            sqlwhere.AppendFormat(" AND (om.order_createdate  >= '{0}' ", CommonFunction.GetPHPTime(query.datestart.ToString("yyyy-MM-dd HH:mm:ss")));
                            sqlwhere.AppendFormat(" AND om.order_createdate <= '{0}' or", CommonFunction.GetPHPTime(query.dateend.ToString("yyyy-MM-dd HH:mm:ss")));
                            sqlwhere.AppendFormat("  om.money_collect_date >= '{0}' ", CommonFunction.GetPHPTime(query.datestart.ToString("yyyy-MM-dd HH:mm:ss")));
                            sqlwhere.AppendFormat(" AND om.money_collect_date <= '{0}') ", CommonFunction.GetPHPTime(query.dateend.ToString("yyyy-MM-dd HH:mm:ss")));
                        }
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(query.orderStatus) && query.orderStatus != "-1")
                {
                    sqlwhere.AppendFormat(" and om.order_status = {0} ", query.orderStatus);
                    // AND imr.invoice_id IS NULL AND (om.money_cancel + om.money_return) != om.order_amount 
                }
                if (query.Order_Payment > 0)
                {
                    sqlwhere.AppendFormat(" and om.order_payment={0}", query.Order_Payment);
                }
                #endregion
                //sqljoin.AppendFormat(sqladdstr);
                sqlwhere.AppendFormat(" ORDER BY om.order_id DESC");
                totalCount = 0;
                return _dbAccess.getDataTableForObj<OrderMasterQuery>(sqlClm.ToString() + sqljoin.ToString() + sqlwhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.Export -->" + ex.Message + sqlClm.ToString() + sqljoin.ToString() + sqlwhere.ToString(), ex);
            }
        }
        /// <summary>
        /// 根據order_id更新取消金額
        /// </summary>
        /// <param name="return_money"></param>
        /// <param name="order_id"></param>
        /// <returns></returns>
        public string UpdateMoneyReturn(int return_money, uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update order_master set money_return=money_return+'{0}' where order_id ='{1}';", return_money, order_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.UpdateMoneyReturn -->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdatePriority(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE   order_master SET	priority = 0 WHERE	order_id = {0}", order_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.UpdatePriority -->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 根據訂單id獲取數據
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public OrderShowMasterQuery GetData(uint orderId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select om.order_id,om.order_payment , om.order_date_pay ,om.money_collect_date , om.order_status ");
                sql.AppendFormat(" ,om.order_createdate ,om.order_date_close, om.company_write, om.company_invoice ,om.company_title ");
                sql.AppendFormat(",om.channel ,om.import_time ,om.channel_order_id ,om.retrieve_mode ,om.delivery_store ");
                sql.AppendFormat(",om.billing_checked ,om.holiday_deliver ,om.note_order ,om.order_product_subtotal ");
                sql.AppendFormat(",om.deduct_bonus ,om.accumulated_bonus ,om.deduct_happygo ,om.accumulated_happygo ,om.deduct_welfare ");
                sql.AppendFormat(",om.order_freight_normal ,om.order_freight_low ,om.order_amount ,om.money_cancel ,om.money_return ");
                sql.AppendFormat(",om.user_id ,om.order_name ,om.order_gender ,om.order_phone,om.order_mobile ,om.order_zip ");
                sql.AppendFormat(",om.order_address ,om.delivery_same ,om.delivery_name ,om.delivery_gender,om.delivery_phone,om.delivery_mobile  ,om.delivery_zip ");
                sql.AppendFormat(",om.delivery_address,  om.delivery_address  as 'delivery_address_str' , om.cart_id, om.note_admin,om.estimated_arrival_period,om.deduct_card_bonus,om.deduct_happygo_convert ");
                sql.AppendFormat(",om.note_order_modify_time,chan.channel_name_simple,mu.user_id as manager_id,mu.user_username as manager_name,para.remark as order_status_str,  payments.parameterName as payment_string,deliver.parameterName as deliver_str,om.cart_id    ");
                sql.AppendFormat("   from order_master om LEFT JOIN manage_user mu on om.note_order_modifier=mu.user_id  ");
                sql.AppendFormat(" LEFT JOIN channel chan on om.channel =chan.channel_id  ");
                sql.AppendFormat(" LEFT JOIN( select parameterType,parameterCode,remark from t_parametersrc WHERE parameterType='order_status') para ON para.parameterCode=om.order_status ");
                sql.AppendFormat(" LEFT JOIN (select parameterType,parameterCode,parameterName from t_parametersrc where parameterType='payment' ) payments on payments.parameterCode=om.order_payment   ");
                sql.AppendFormat(" LEFT JOIN( select parameterType,parameterCode,parameterName from t_parametersrc where parameterType='Deliver_Store' ) deliver on deliver.parameterCode=om.delivery_store ");
                sql.AppendFormat(" where om.order_id={0};", orderId);
                return _dbAccess.getSinggleObj<OrderShowMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetData-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string VerifyData(uint orderId)
        {
            StringBuilder sqlSite = new StringBuilder();
            StringBuilder sqlPay = new StringBuilder();
            StringBuilder sqlSinopa = new StringBuilder();
            StringBuilder sqlChannel = new StringBuilder();
            StringBuilder sqlHncb = new StringBuilder();
            StringBuilder sqlComWrite = new StringBuilder();
            StringBuilder sqlRetrieveMode = new StringBuilder();

            string siteName = string.Empty;
            string sinopa = string.Empty;
            string channel = string.Empty;
            string hncb = string.Empty;
            string companyWrite = string.Empty;
            string importTime = string.Empty;
            string channelOderId = string.Empty;
            string retrieve_mode = string.Empty;
            string delivery_same = string.Empty;
            string delivery_name = string.Empty;
            string delivery_gender = string.Empty;
            try
            {
                sqlSite.AppendFormat("select * from site where cart_delivery =(select cart_id from order_master where order_id={0}); ", orderId);
                sqlSinopa.AppendFormat("select order_id,sinopac_id  from order_payment_sinopac where order_id={0}; ", orderId);
                sqlChannel.AppendFormat("select om.order_id,chan.channel_id,chan.channel_name_simple from order_master om right join channel chan  on  om.channel = chan.channel_id where om.order_id={0};", orderId);
                sqlHncb.AppendFormat(" SELECT order_id , hncb_id  FROM    order_payment_hncb   WHERE   order_id ={0}; ", orderId);
                sqlComWrite.AppendFormat(" select company_write,import_time,channel_order_id,delivery_name,delivery_gender,delivery_same   from order_master WHERE   order_id ={0}; ", orderId);
                sqlRetrieveMode.AppendFormat(" SELECT parameterName FROM    t_parametersrc  WHERE   parameterType = 'retrieve_mode' AND parameterCode =(select  retrieve_mode from order_master where order_id={0});  ", orderId);
                DataTable _siteDt = _dbAccess.getDataTable(sqlSite.ToString());
                DataTable _sinpoDt = _dbAccess.getDataTable(sqlSinopa.ToString());
                DataTable _channelDt = _dbAccess.getDataTable(sqlChannel.ToString());
                DataTable _hncbDt = _dbAccess.getDataTable(sqlHncb.ToString());
                DataTable _comWriteDt = _dbAccess.getDataTable(sqlComWrite.ToString());
                DataTable _retrieModeDt = _dbAccess.getDataTable(sqlRetrieveMode.ToString());
                if (_siteDt.Rows.Count > 0)
                {
                    siteName = _siteDt.Rows[0]["site_name"].ToString();
                }
                else
                {
                    siteName = "";
                }
                if (_sinpoDt.Rows.Count > 0)
                {
                    sinopa = _sinpoDt.Rows[0]["sinopac_id"].ToString();
                }
                else
                {
                    sinopa = "";
                }
                if (_channelDt.Rows.Count > 0)
                {
                    if (_channelDt.Rows[0]["channel_id"].ToString() == "0")
                    {
                        channel = "全部";
                    }
                    else
                    {
                        channel = _channelDt.Rows[0]["channel_name_simple"].ToString();
                    }
                }
                if (_hncbDt.Rows.Count > 0)
                {
                    hncb = _hncbDt.Rows[0]["hncb_id"].ToString();
                }
                if (_comWriteDt.Rows.Count > 0)
                {
                    companyWrite = _comWriteDt.Rows[0]["company_write"].ToString();
                    importTime = _comWriteDt.Rows[0]["import_time"].ToString();
                    channelOderId = _comWriteDt.Rows[0]["channel_order_id"].ToString();
                    delivery_same = _comWriteDt.Rows[0]["delivery_same"].ToString();
                    delivery_name = _comWriteDt.Rows[0]["delivery_name"].ToString();
                    delivery_gender = _comWriteDt.Rows[0]["delivery_gender"].ToString();
                }
                if (_retrieModeDt.Rows.Count > 0)
                {
                    retrieve_mode = _retrieModeDt.Rows[0]["parameterName"].ToString();
                }
                return siteName + ";" + sinopa + ";" + channel + ";" + hncb + ";" + companyWrite + ";" + importTime + ";" + channelOderId + ";" + retrieve_mode + ";" + delivery_same + ";" + delivery_name + ";" + delivery_gender;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->VerifyData-->" + sqlSite.ToString() + ex.Message, ex);
            }
        }

        public int SaveNoteOrder(OrderShowMasterQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update  order_master set note_order='{0}',note_order_modifier={1},note_order_modify_time={2}  where order_id={3}", store.note_order, store.user_id, CommonFunction.GetPHPTime(store.NoteOrderModifyTime.ToString()), store.order_id);
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->SaveNoteOrder-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public int SaveNoteAdmin(OrderShowMasterQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_master set note_admin='{0}' where order_id={1};", store.note_admin, store.order_id);
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->SaveNoteAdmin-->" + sql.ToString() + ex.Message, ex);
            }
        }
        //define('SERIAL_ID_ORDER_MASTER_STATUS',		29);	// 訂單主檔狀態流水號
        #region 得到加1后的serial_value
        public int GetSerialValue(int serialId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select serial_value from serial where serial_id={0};", serialId);
                DataTable _dt = _dbAccess.getDataTable(sql.ToString());
                return Convert.ToInt32(_dt.Rows[0]["serial_value"]) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetSerialValue-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        #region 更新serial_value
        public string UpdateSerialVal(int serialValue, int serialId)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update serial set serial_value={0}  where serial_id={1};", serialValue, serialId);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpdateSerialVal-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion
        public int SaveStatus(OrderShowMasterQuery store)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
            int re = 0;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    store.serial_id = GetSerialValue(29); //得到serial_value,是order_master_status的id
                    mySqlConn.Open();
                    mySqlCmd.Connection = mySqlConn;
                    mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                    mySqlCmd.CommandType = System.Data.CommandType.Text;
                    mySqlCmd.CommandText = UpdateSerialVal(store.serial_id, 29);
                    mySqlCmd.CommandText += InsertOrderMasterStatus(store);
                    re = mySqlCmd.ExecuteNonQuery();
                    mySqlCmd.Transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderShowBaseInfoDao-->SaveStatus-->" + mySqlCmd.ToString() + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return re;
        }

        public string InsertOrderMasterStatus(OrderShowMasterQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert  into order_master_status  (serial_id,order_id,order_status,");
                sql.Append("status_description,status_ipfrom,status_createdate) ");
                sql.AppendFormat("values({0},{1},{2},", store.serial_id, store.order_id, store.order_status);
                sql.AppendFormat("'{0}','{1}',{2});", store.status_description, store.status_ipfrom, CommonFunction.GetPHPTime(store.StatusCreateDate.ToString()));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->InsertOrderMasterStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public UsersListQuery GetUserInfo(uint user_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("SELECT  users.user_id,user_email,user_new_email,user_status,user_password,user_newpasswd,user_name, ");
                sql.Append(" user_gender,user_mobile,user_phone,user_zip,user_address, ");
                sql.Append(" user_login_attempts,user_actkey,user_reg_date,user_updatedate,user_old_password,user_company_id,user_source,user_fb_id, ");
                sql.Append(" user_country,user_ref_user_id,user_province,user_city,source_trace ,  ");
                sql.Append(" CASE user_type  when '1' THEN '網路會員' else'電話會員' END as mytype , ");
                sql.Append(" send_sms_ad,paper_invoice,adm_note ,concat(user_birthday_year,'/',user_birthday_month,'/',user_birthday_day)  ");
                sql.Append(" as birthday,first_time,last_time,be4_last_time FROM users  ");
                sql.AppendFormat("  where users.user_id={0}; ", user_id);
                return _dbAccess.getSinggleObj<UsersListQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetUserInfo-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 根據條件查詢出貨列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <param name="totalCount">返回數據總條數</param>
        /// <returns>出貨列表信息</returns>
        public List<OrderMasterQuery> GetShipmentList(OrderMasterQuery query, out int totalCount)
        {
            try
            {
                query.Replace4MySQL();
                string strSql = string.Empty;

                StringBuilder sbCols = new StringBuilder("SELECT om.order_id as OrderId,FROM_UNIXTIME(om.order_date_pay) as OrderDatePay,FROM_UNIXTIME(om.money_collect_date) as MoneyCollectDate,om.order_status as Order_Status,om.note_order,dm.deliver_id,dm.freight_set,dm.type,v.vendor_id,v.vendor_name_simple,od.product_name,od.product_spec_name,od.buy_num,om.order_name,dm.delivery_name,dm.delivery_status,od.detail_status,od.combined_mode,od.parent_name,od.item_mode ");//om.money_collect_date,

                StringBuilder sbTbls = new StringBuilder();
                sbTbls.Append(" FROM order_master AS om ");
                sbTbls.Append(" LEFT JOIN deliver_master AS dm ON (om.order_id = dm.order_id) ");
                sbTbls.Append(" LEFT JOIN deliver_detail AS dd ON (dm.deliver_id = dd.deliver_id) ");
                sbTbls.Append(" LEFT JOIN order_detail AS od ON (od.detail_id = dd.detail_id) ");
                sbTbls.Append(" LEFT JOIN vendor AS v ON (v.vendor_id = od.item_vendor_id) ");

                StringBuilder sbCondition = new StringBuilder(" WHERE 1=1 ");
                if (!string.IsNullOrEmpty(query.order_date_pay_startTime.ToString()))
                {
                    sbCondition.AppendFormat(" AND order_date_pay >= {0} ", CommonFunction.GetPHPTime(query.order_date_pay_startTime.ToString()));
                }
                if (!string.IsNullOrEmpty(query.order_date_pay_endTime.ToString()))
                {
                    sbCondition.AppendFormat(" AND order_date_pay <= {0} ", CommonFunction.GetPHPTime(query.order_date_pay_endTime.ToString()));
                }
                if (query.delay == 0)//admin/Controller/DeliversController/all_in_1()方法，第443和458的判斷
                {
                    sbCondition.Append(" AND order_status IN (0, 2, 3) ");
                    sbCondition.Append(" AND dm.deliver_id IS NOT NULL ");
                }
                else
                {
                    sbCondition.Append(" AND dm.delivery_status IN (0,1, 2, 5) ");
                }
                sbCondition.Append(" AND dm.type IN (1, 2, 3, 4, 5, 6) ");
                sbCondition.Append(" ORDER BY om.order_id ASC,dm.deliver_id ASC,v.vendor_id ASC ");

                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(" select count(1) as search_total " + sbTbls + sbCondition);
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    sbCondition.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                strSql = sbCols.ToString() + sbTbls + sbCondition;
                return _dbAccess.getDataTableForObj<OrderMasterQuery>(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao.GetShipmentList-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢購買次數
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>購買次數</returns>
        public int GetBuyCount(OrderMasterQuery query)
        {
            int buyCount = 0;
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder("SELECT count(1) as totalCount ");
            StringBuilder sqlTable = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();

            sqlTable.Append(" FROM order_master om ");
            sqlTable.Append(" LEFT JOIN order_slave os ON os.order_id=om.order_id ");
            sqlTable.Append(" LEFT JOIN order_detail od ON od.slave_id=os.slave_id ");
            sqlCondition.Append(" WHERE 1=1 ");
            if (query.User_Id != 0)
            {
                sqlCondition.AppendFormat(" AND om.user_id={0} ", query.User_Id);
            }
            if (query.Item_Id != 0)
            {
                sqlCondition.AppendFormat(" AND od.item_id={0} ", query.Item_Id);
            }

            try
            {
                sql.Append(sqlCount).Append(sqlTable).Append(sqlCondition);
                DataTable dt = _dbAccess.getDataTable(sql.ToString());
                if (dt.Rows.Count > 0)
                {
                    buyCount = Convert.ToInt32(dt.Rows[0]["totalCount"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("BrowseDataDao-->GetBrowseDataList -->" + ex.Message + sql.ToString(), ex);
            }
            return buyCount;
        }

        #region 匯入會計賬款實收時間
        /// <summary>
        /// 匯入會計賬款實收時間
        /// </summary>
        /// <param name="dt">匯入入賬時間表</param>
        /// <returns></returns>
        public string UpdateOac(DataTable DtTemp, OrderAccountCollection model)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;update order_account_collection set ");

                sql.AppendFormat("  order_id='{0}'   ", model.order_id);
                if (string.IsNullOrEmpty(DtTemp.Rows[0]["account_collection_time"].ToString()))
                {
                    if (model.account_collection_time != DateTime.MinValue && model.account_collection_time != null)
                    {
                        sql.AppendFormat(" , account_collection_time='{0}' ", model.account_collection_time.ToString("yyyy-MM-dd"));
                        sql.AppendFormat(" ,account_collection_money='{0}' ", model.account_collection_money);
                        sql.AppendFormat(" , poundage='{0}' ", model.poundage);
                    }
                }
                if (string.IsNullOrEmpty(DtTemp.Rows[0]["return_collection_time"].ToString()))
                {
                    if (model.return_collection_time != DateTime.MinValue && model.return_collection_time != null)
                    {
                        sql.AppendFormat("  ,return_collection_time='{0}'  ", model.return_collection_time.ToString("yyyy-MM-dd"));
                        sql.AppendFormat(" ,return_collection_money='{0}' ", model.return_collection_money);
                        sql.AppendFormat(" ,return_poundage='{0}'  ", model.return_poundage);
                    }
                }
                if (string.IsNullOrEmpty(DtTemp.Rows[0]["invoice_date_manual"].ToString()))
                {
                    if (model.invoice_date_manual != DateTime.MinValue && model.invoice_date_manual != null)
                    {
                        sql.AppendFormat("  ,invoice_date_manual='{0}'  ", model.invoice_date_manual.ToString("yyyy-MM-dd"));
                        sql.AppendFormat(" ,invoice_sale_manual='{0}' ", model.invoice_sale_manual);
                        sql.AppendFormat(" ,invoice_tax_manual='{0}'  ", model.invoice_tax_manual);
                    }
                }
                if (string.IsNullOrEmpty(DtTemp.Rows[0]["remark"].ToString()))
                {
                    sql.AppendFormat("  ,remark='{0}'  ", model.remark);
                }
                sql.AppendFormat("  where order_id='{0}';set sql_safe_updates = 1;", model.order_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpdateOac -->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string InsertOac(OrderAccountCollection model)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                str.Append(@"insert into order_account_collection (order_id,remark,account_collection_time,account_collection_money,poundage,return_collection_time,return_collection_money,return_poundage, ");
                str.Append("  invoice_date_manual,invoice_sale_manual,invoice_tax_manual ) ");
                str.AppendFormat(" values('{0}','{1}'", model.order_id, model.remark);
                if (model.account_collection_time != null && model.account_collection_time != DateTime.MinValue)
                {
                    str.AppendFormat(" ,'{0}','{1}','{2}'", Common.CommonFunction.DateTimeToString(model.account_collection_time), model.account_collection_money, model.poundage);
                }
                else
                {
                    str.AppendFormat(" ,NULL ,NULL  ,NULL  ");
                }
                if (model.return_collection_time != null && model.return_collection_time != DateTime.MinValue)
                {
                    str.AppendFormat(" ,'{0}','{1}','{2}' ", Common.CommonFunction.DateTimeToString(model.return_collection_time), model.return_collection_money, model.return_poundage);
                }
                else
                {
                    str.AppendFormat(" , NULL  ,NULL  ,NULL ");
                } if (model.invoice_date_manual != null && model.invoice_date_manual != DateTime.MinValue)
                {
                    str.AppendFormat(" ,'{0}','{1}','{2}'", Common.CommonFunction.DateTimeToString(model.invoice_date_manual), model.invoice_sale_manual, model.invoice_tax_manual);
                }
                else
                {
                    str.AppendFormat(" ,NULL ,NULL  ,NULL  ");
                }
                str.AppendFormat(" );");
                return str.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->InsertOac -->" + ex.Message + str.ToString(), ex);
            }
        }
        /// <summary>
        /// 判斷是否存在訂單編號
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool IsNotOrderId(uint orderId)
        {
            string str = string.Empty;
            try
            {
                str = string.Format("select * from order_master where order_id={0}", orderId);
                DataTable dt = _dbAccess.getDataTable(str);
                if (dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["order_id"]) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterDao-->IsNotOrderId -->" + ex.Message + str.ToString(), ex);
            }
        }
        #endregion

        /// <summary>
        /// 會計入賬匯出列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public DataTable OrderMasterExportList(OrderMasterQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            try
            {
                sql.AppendFormat("select oac.row_id, om.order_id,om.order_name,om.deduct_card_bonus,imr.invoice_date,'' as invoicedate,");
                sql.AppendFormat("  SUM(imr.free_tax) as free_tax,SUM(imr.sales_amount) as sales_amount,SUM(imr.tax_amount) as tax_amount,SUM(imr.total_amount) imramount,");
                sql.AppendFormat("om.money_cancel,om.money_return,om.delivery_name,om.order_amount,om.order_payment,'' as parameterName,om.order_createdate, '' as ordercreatedate,");
                sql.AppendFormat("oac.account_collection_time,oac.account_collection_money,oac.poundage,oac.return_collection_time,oac.return_collection_money,oac.return_poundage,oac.remark,");
                sql.AppendFormat(" oac.invoice_date_manual,oac.invoice_sale_manual,oac.invoice_tax_manual , ");
                sql.AppendFormat(" '' as oacamount ,'' as invoice_diff ");
                sqlCondition.Append(" from  order_master om  ");
                sqlCondition.Append(" left join order_account_collection oac   on om.order_id  = oac.order_id ");
                sqlCondition.AppendFormat(" left join invoice_master_record imr  on imr.order_id=om.order_id   and invoice_attribute=1 ");

                sqlCount.AppendFormat("SELECT count(om.order_id) as 'count' ");
                sqlCondition.AppendFormat(" where 1=1 ");
                if (query.dateType == 1)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.account_collection_time>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.account_collection_time<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                else if (query.dateType == 3)
                {
                    if (query.first_time != 0)
                    {
                        sqlCondition.AppendFormat(" and om.order_createdate>='{0}' ", query.first_time);
                    }
                    if (query.last_time != 0)
                    {
                        sqlCondition.AppendFormat(" and om.order_createdate<='{0}' ", query.last_time);
                    }
                }
                else if (query.dateType == 4)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.return_collection_time>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.return_collection_time<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                else if (query.dateType == 2)
                {
                    if (query.first_time != 0)
                    {
                        sqlCondition.AppendFormat(" and imr.invoice_date>='{0}' ", query.first_time);
                    }
                    if (query.last_time != 0)
                    {
                        sqlCondition.AppendFormat(" and imr.invoice_date<='{0}' ", query.last_time);
                    }
                }
                else if (query.dateType == 5)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.invoice_date_manual>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.invoice_date_manual<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                if (query.show_type == 1)
                {
                    sqlCondition.AppendFormat(" and oac.row_id!='' ");
                }
                else if (query.show_type == 2)
                {
                    sqlCondition.AppendFormat(" and   ISNULL(oac.row_id) ");
                }
                if (query.invoice_type == 1)
                {
                    sqlCondition.AppendFormat(" and (imr.invoice_id!='' or  oac.invoice_date_manual!='' ) ");
                }
                else if (query.invoice_type == 2)
                {
                    sqlCondition.AppendFormat(" and  ISNULL(imr.invoice_id) and  ISNULL(oac.invoice_date_manual)  ");
                }

                if (query.Order_Id != 0)
                {
                    sqlCondition.AppendFormat(" and om.order_id='{0}' ", query.Order_Id);
                }
                if (query.Order_Payment != 0)
                {
                    sqlCondition.AppendFormat(" and om.order_payment='{0}' ", query.Order_Payment);
                }
                sqlCondition.AppendFormat(" GROUP BY om.order_id ");
                sqlCount.AppendFormat(sqlCondition.ToString());
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sqlCount.ToString());
                    totalCount = _dt.Rows.Count;
                    sql.AppendFormat(sqlCondition.ToString());
                    sql.AppendFormat(" ORDER BY imr.invoice_date desc, om.order_id desc ");
                    sql.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.GetAllKindType("payment");

                DataTable dt = _dbAccess.getDataTable(sql.ToString());
                Int64 order_creat = 0;
                Int64 invoi_date = 0;
                Int64 poun = 0;
                Int64 coll = 0;
                Int64 Rpoun = 0;
                Int64 Rcoll = 0;
                Int64 totalMoney = 0;
                Int64 imramount = 0;
                Int64 invoice = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    var alist = parameterList.Find(m => m.ParameterCode == dr["order_payment"].ToString());
                    if (alist != null)
                    {
                        dr["parameterName"] = alist.parameterName;
                    }
                    invoi_date = 0;
                    if (Int64.TryParse(dr["invoice_date"].ToString(), out invoi_date))
                    {
                        dr["invoicedate"] = CommonFunction.GetNetTime(invoi_date).ToShortDateString();
                    }
                    order_creat = 0;

                    if (Int64.TryParse(dr["order_createdate"].ToString(), out order_creat))
                    {
                        dr["ordercreatedate"] = CommonFunction.GetNetTime(order_creat).ToShortDateString();
                    }
                    totalMoney = 0;
                    poun = 0;
                    if (!string.IsNullOrEmpty(dr["poundage"].ToString()))
                    {
                        if (Int64.TryParse(dr["poundage"].ToString(), out poun))
                        {
                            totalMoney += poun;
                        }
                    }
                    coll = 0;
                    if (!string.IsNullOrEmpty(dr["account_collection_money"].ToString()))
                    {
                        if (Int64.TryParse(dr["account_collection_money"].ToString(), out coll))
                        {
                            totalMoney += coll;
                        }
                    }
                    Rpoun = 0;
                    if (!string.IsNullOrEmpty(dr["return_poundage"].ToString()))
                    {
                        if (Int64.TryParse(dr["return_poundage"].ToString(), out Rpoun))
                        {
                            totalMoney += Rpoun;
                        }
                    }
                    Rcoll = 0;
                    if (!string.IsNullOrEmpty(dr["return_collection_money"].ToString()))
                    {
                        if (Int64.TryParse(dr["return_collection_money"].ToString(), out Rcoll))
                        {
                            totalMoney += Rcoll;
                        }
                    }
                    dr["oacamount"] = totalMoney;
                    imramount = 0;
                    invoice = 0;
                    if (!string.IsNullOrEmpty(dr["imramount"].ToString()))
                    {
                        if (Int64.TryParse(dr["imramount"].ToString(), out invoice))
                        {
                            imramount += Convert.ToInt64(dr["imramount"].ToString());
                        }
                    }
                    if (!string.IsNullOrEmpty(dr["invoice_sale_manual"].ToString()))
                    {
                        if (Int64.TryParse(dr["invoice_sale_manual"].ToString(), out invoice))
                        {
                            imramount += Convert.ToInt64(dr["invoice_sale_manual"].ToString());
                        }
                    }
                    if (!string.IsNullOrEmpty(dr["invoice_tax_manual"].ToString()))
                    {
                        if (Int64.TryParse(dr["invoice_tax_manual"].ToString(), out invoice))
                        {
                            imramount += Convert.ToInt64(dr["invoice_tax_manual"].ToString());
                        }
                    }
                    dr["imramount"] = imramount;
                    if (!string.IsNullOrEmpty(dr["oacamount"].ToString()) || !string.IsNullOrEmpty(dr["imramount"].ToString()) || !string.IsNullOrEmpty(dr["invoice_tax_manual"].ToString()) || !string.IsNullOrEmpty(dr["invoice_sale_manual"].ToString()))
                    {
                        dr["invoice_diff"] = totalMoney - imramount;//J=E-H
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->ArrorOrderList -->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 會計入賬匯出數據匯總
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public DataTable OrderMasterHuiZong(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlCondition = new StringBuilder();
            try
            {
                //sql.Append("select   sum(free_tax)as FreeTax,sum(tax_amount)as SalesAmount,sum(AccountCollectionMoney) as  AccountCollectionMoney, ");
                //sql.Append(" sum(ZPoundage) as ZPoundage ");
                //sql.Append("from (");
                //sql.Append(" select om.order_id,SUM(imr.free_tax)  free_tax ,SUM(imr.tax_amount) tax_amount,oac.account_collection_money+oac.return_collection_money as  AccountCollectionMoney ,oac.poundage++oac.return_poundage as ZPoundage ");

                sql.Append("      select  sum(IFNULL(free_tax,0)+invoice_sale_manual)as FreeTax,sum(IFNULL(tax_amount,0)+invoice_tax_manual)as SalesAmount,sum(IFNULL(AccountCollectionMoney,0)) as  AccountCollectionMoney,  sum(IFNULL(ZPoundage,0)) as ZPoundage  from  ( ");
                sql.Append(" select SUM( IFNULL(imr.free_tax,0))  free_tax ,  ");
                sql.Append(" SUM( IFNULL(imr.tax_amount,0)) tax_amount, ");
                sql.Append(" IFNULL(oac.invoice_sale_manual,0) invoice_sale_manual, ");
                sql.Append(" IFNULL(oac.invoice_tax_manual,0) invoice_tax_manual, ");
                sql.Append(" IFNULL(oac.account_collection_money,0)+IFNULL(oac.return_collection_money,0) as  AccountCollectionMoney ,  ");
                sql.Append(" IFNULL(oac.poundage,0)+IFNULL(oac.return_poundage,0) as ZPoundage    ");
                sqlCondition.AppendFormat("  from  order_master om left join order_account_collection oac  on oac.order_id=om.order_id  ");
                sqlCondition.AppendFormat(" left join invoice_master_record imr  on imr.order_id=om.order_id   and invoice_attribute=1 ");
                sqlCondition.AppendFormat(" left join t_parametersrc tp on om.order_payment=tp.parameterCode and tp.parameterType='payment' ");
                sqlCondition.AppendFormat(" where 1=1 ");
                if (query.dateType == 1)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.account_collection_time>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.account_collection_time<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                else if (query.dateType == 3)
                {
                    if (query.first_time != 0)
                    {
                        sqlCondition.AppendFormat(" and om.order_createdate>='{0}' ", query.first_time);
                    }
                    if (query.last_time != 0)
                    {
                        sqlCondition.AppendFormat(" and om.order_createdate<='{0}' ", query.last_time);
                    }
                }
                else if (query.dateType == 4)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.return_collection_time>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.return_collection_time<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                else if (query.dateType == 2)
                {
                    if (query.first_time != 0)
                    {
                        sqlCondition.AppendFormat(" and imr.invoice_date>='{0}' ", query.first_time);
                    }
                    if (query.last_time != 0)
                    {
                        sqlCondition.AppendFormat(" and imr.invoice_date<='{0}' ", query.last_time);
                    }
                }
                else if (query.dateType == 5)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.invoice_date_manual>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sqlCondition.AppendFormat(" and oac.invoice_date_manual<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                if (query.show_type == 1)
                {
                    sqlCondition.AppendFormat(" and oac.row_id!='' ");
                }
                else if (query.show_type == 2)
                {
                    sqlCondition.AppendFormat(" and   ISNULL(oac.row_id) ");
                }
                if (query.invoice_type == 1)
                {
                    sqlCondition.AppendFormat(" and (imr.invoice_id!='' or  oac.invoice_date_manual!='' ) ");
                }
                else if (query.invoice_type == 2)
                {
                    sqlCondition.AppendFormat(" and  ISNULL(imr.invoice_id) and  ISNULL(oac.invoice_date_manual) ");
                }
                if (query.Order_Id != 0)
                {
                    sqlCondition.AppendFormat(" and om.order_id='{0}' ", query.Order_Id);
                }
                if (query.Order_Payment != 0)
                {
                    sqlCondition.AppendFormat(" and om.order_payment='{0}' ", query.Order_Payment);
                }
                sqlCondition.AppendFormat(" GROUP BY om.order_id ");
                sql.AppendFormat(sqlCondition.ToString());
                sql.Append(" )totalMoney ");
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->OrderMasterHuiZong -->" + ex.Message + sql.ToString(), ex);
            }
        }

        #region 會計入賬匯出+DataTable OrderMasterExport(OrderMasterQuery query)
        /// <summary>
        /// 會計入賬匯出
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable OrderMasterExport(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select oac.row_id, om.order_id,om.order_name,om.deduct_card_bonus,imr.invoice_date,'' as invoicedate,");
                sql.AppendFormat("  SUM(imr.free_tax) as free_tax,SUM(imr.sales_amount) as sales_amount,SUM(imr.tax_amount) as tax_amount,SUM(imr.total_amount) imramount,");
                sql.AppendFormat("om.money_cancel,om.money_return,om.delivery_name,om.order_amount,om.order_payment,'' as parameterName,om.order_createdate, '' as ordercreatedate,");
                sql.AppendFormat("oac.account_collection_time,oac.account_collection_money,oac.poundage,oac.return_collection_time,oac.return_collection_money,oac.return_poundage,oac.remark,");
                sql.AppendFormat(" oac.invoice_date_manual,oac.invoice_sale_manual,oac.invoice_tax_manual , ");
                sql.AppendFormat(" '' as oacamount , '' as invoice_diff ");
                sql.Append(" from  order_master om  ");
                sql.Append(" left join order_account_collection oac   on om.order_id  = oac.order_id ");
                sql.AppendFormat(" left join invoice_master_record imr  on imr.order_id=om.order_id   and invoice_attribute=1 ");
                sql.AppendFormat(" where 1=1 ");
                if (query.dateType == 1)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sql.AppendFormat(" and oac.account_collection_time>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sql.AppendFormat(" and oac.account_collection_time<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                else if (query.dateType == 3)
                {
                    if (query.first_time != 0)
                    {
                        sql.AppendFormat(" and om.order_createdate>='{0}' ", query.first_time);
                    }
                    if (query.last_time != 0)
                    {
                        sql.AppendFormat(" and om.order_createdate<='{0}' ", query.last_time);
                    }
                }
                else if (query.dateType == 4)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sql.AppendFormat(" and oac.return_collection_time>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sql.AppendFormat(" and oac.return_collection_time<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                else if (query.dateType == 2)
                {
                    if (query.first_time != 0)
                    {
                        sql.AppendFormat(" and imr.invoice_date>='{0}' ", query.first_time);
                    }
                    if (query.last_time != 0)
                    {
                        sql.AppendFormat(" and imr.invoice_date<='{0}' ", query.last_time);
                    }
                }
                else if (query.dateType == 5)
                {
                    if (query.order_date_pay_startTime != DateTime.MinValue)
                    {
                        sql.AppendFormat(" and oac.invoice_date_manual>='{0}' ", query.order_date_pay_startTime.ToString("yyyy-MM-dd 00:00:00"));
                    }
                    if (query.order_date_pay_endTime != DateTime.MinValue)
                    {
                        sql.AppendFormat(" and oac.invoice_date_manual<='{0}' ", query.order_date_pay_endTime.ToString("yyyy-MM-dd 23:59:59"));
                    }
                }
                if (query.show_type == 1)
                {
                    sql.AppendFormat(" and oac.row_id!='' ");
                }
                else if (query.show_type == 2)
                {
                    sql.AppendFormat(" and   ISNULL(oac.row_id) ");
                }
                if (query.invoice_type == 1)
                {
                    sql.AppendFormat(" and (imr.invoice_id!='' or  oac.invoice_date_manual!='' ) ");
                }
                else if (query.invoice_type == 2)
                {
                    sql.AppendFormat(" and   ISNULL(imr.invoice_id) and  ISNULL(oac.invoice_date_manual)  ");
                }
                if (query.Order_Id != 0)
                {
                    sql.AppendFormat(" and om.order_id='{0}' ", query.Order_Id);
                }
                if (query.Order_Payment != 0)
                {
                    sql.AppendFormat(" and om.order_payment='{0}' ", query.Order_Payment);
                }
                sql.AppendFormat(" GROUP BY om.order_id ");
                sql.AppendFormat(" ORDER BY imr.invoice_date desc, om.order_id desc ");

                IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.GetAllKindType("payment");

                DataTable dt = _dbAccess.getDataTable(sql.ToString());
                Int64 order_creat = 0;
                Int64 invoi_date = 0;
                Int64 poun = 0;
                Int64 coll = 0;
                Int64 Rpoun = 0;
                Int64 Rcoll = 0;
                Int64 totalMoney = 0;
                Int64 imramount = 0;
                Int64 invoice = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    var alist = parameterList.Find(m => m.ParameterCode == dr["order_payment"].ToString());
                    if (alist != null)
                    {
                        dr["parameterName"] = alist.parameterName;
                    }
                    invoi_date = 0;
                    if (Int64.TryParse(dr["invoice_date"].ToString(), out invoi_date))
                    {
                        dr["invoicedate"] = CommonFunction.GetNetTime(invoi_date).ToShortDateString();
                    }
                    order_creat = 0;
                    if (Int64.TryParse(dr["order_createdate"].ToString(), out order_creat))
                    {
                        dr["ordercreatedate"] = CommonFunction.GetNetTime(order_creat).ToShortDateString();
                    }
                    totalMoney = 0;
                    poun = 0;
                    if (!string.IsNullOrEmpty(dr["poundage"].ToString()))
                    {
                        if (Int64.TryParse(dr["poundage"].ToString(), out poun))
                        {
                            totalMoney += poun;
                        }
                    }
                    coll = 0;
                    if (!string.IsNullOrEmpty(dr["account_collection_money"].ToString()))
                    {
                        if (Int64.TryParse(dr["account_collection_money"].ToString(), out coll))
                        {
                            totalMoney += coll;
                        }
                    }
                    Rpoun = 0;
                    if (!string.IsNullOrEmpty(dr["return_poundage"].ToString()))
                    {
                        if (Int64.TryParse(dr["return_poundage"].ToString(), out Rpoun))
                        {
                            totalMoney += Rpoun;
                        }
                    }
                    Rcoll = 0;
                    if (!string.IsNullOrEmpty(dr["return_collection_money"].ToString()))
                    {
                        if (Int64.TryParse(dr["return_collection_money"].ToString(), out Rcoll))
                        {
                            totalMoney += Rcoll;
                        }
                    }
                    dr["oacamount"] = totalMoney;
                    imramount = 0;
                    invoice = 0;
                    if (!string.IsNullOrEmpty(dr["imramount"].ToString()))
                    {
                        if (Int64.TryParse(dr["imramount"].ToString(), out invoice))
                        {
                            imramount += Convert.ToInt64(dr["imramount"].ToString());
                        }
                    }
                    if (!string.IsNullOrEmpty(dr["invoice_sale_manual"].ToString()))
                    {
                        if (Int64.TryParse(dr["invoice_sale_manual"].ToString(), out invoice))
                        {
                            imramount += Convert.ToInt64(dr["invoice_sale_manual"].ToString());
                        }
                    }
                    if (!string.IsNullOrEmpty(dr["invoice_tax_manual"].ToString()))
                    {
                        if (Int64.TryParse(dr["invoice_tax_manual"].ToString(), out invoice))
                        {
                            imramount += Convert.ToInt64(dr["invoice_tax_manual"].ToString());
                        }
                    }
                    dr["imramount"] = imramount;
                    if (!string.IsNullOrEmpty(dr["oacamount"].ToString()) || !string.IsNullOrEmpty(dr["imramount"].ToString()) || !string.IsNullOrEmpty(dr["invoice_tax_manual"].ToString()) || !string.IsNullOrEmpty(dr["invoice_sale_manual"].ToString()))
                    {
                        dr["invoice_diff"] = totalMoney - imramount;//J=E-H
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterDao-->OrderMasterExport -->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        /// <summary>
        /// 異常訂單列表
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns></returns>
        public DataTable ArrorOrderList(OrderMasterQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            try
            {
                sql.Append(@"SELECT om.order_id,om.order_status,FROM_UNIXTIME(om.order_createdate,'%Y/%m/%d') as order_createdate,od.detail_id,od.parent_id,od.pack_id,od.combined_mode,od.item_mode,count(od.detail_id) as cout ");
                sqlCount.Append(@" from order_detail od LEFT JOIN order_slave os USING(slave_id) LEFT JOIN order_master om USING(order_id) ");
                sqlCount.Append(" where od.item_mode=1 ");
                //LEFT JOIN (select * from t_parametersrc where parameterType ='order_status') as os on os.parameterCode=om.order_status 
                //left join (select * from t_parametersrc where parameterType ='Combo_Type') as ct on ct.parameterCode=od.combined_mode 
                if (query.first_time != 0 && query.last_time != 0)
                {
                    sqlCount.AppendFormat(" and om.order_createdate between '{0}' and '{1}' ", query.first_time, query.last_time);
                }
                sqlCount.AppendFormat(" GROUP BY om.order_id,od.parent_id,od.pack_id HAVING cout>1  ");
                sql.Append(sqlCount.ToString());
                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(" select count(a.order_id) as totalCount from ( " + sql.ToString() + ")a");
                    if (_dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"].ToString());
                    }

                    sql.AppendFormat(" limit {0},{1};", query.Start, query.Limit);
                }
                DataTable _list = _dbAccess.getDataTable(sql.ToString());
                if (_list.Rows.Count > 0)
                {
                    IParametersrcImplDao _parameterDao = new ParametersrcDao(connStr);
                    List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("order_status", "Combo_Type");
                    _list.Columns.Add("modeName");
                    _list.Columns.Add("remark");
                    foreach (DataRow dr in _list.Rows)
                    {
                        var alist = parameterList.Find(m => m.ParameterType == "order_status" && m.ParameterCode == dr["order_status"].ToString());
                        var blist = parameterList.Find(m => m.ParameterType == "Combo_Type" && m.ParameterCode == dr["combined_mode"].ToString());

                        if (alist != null)
                        {
                            dr["remark"] = alist.remark;
                        }
                        if (blist != null)
                        {
                            dr["modeName"] = blist.parameterName;
                        }

                    }
                }
                return _list;

            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->ArrorOrderList -->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable ExportArrorOrderExcel(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("SELECT om.order_id,om.order_status,od.detail_id,od.parent_id,od.pack_id,od.combined_mode,od.item_mode,count(od.detail_id) as cout ");
                sql.Append(@" from order_detail od 
                         LEFT JOIN order_slave os ON os.slave_id=od.slave_id
                         LEFT JOIN order_master om ON om.order_id=os.order_id
                         where od.item_mode=1 ");
                if (query.first_time != 0 && query.last_time != 0)
                {
                    sql.AppendFormat(" and om.order_createdate between {0} and {1} ", query.first_time, query.last_time);
                }
                sql.Append(" GROUP BY om.order_id,od.parent_id,od.pack_id HAVING  cout>1  ");
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->ExportArrorOrderExcel -->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable IsExistOrderId(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select * from order_account_collection where order_id='{0}' ", order_id);
                DataTable dt = _dbAccess.getDataTable(sql.ToString());
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->IsExistOrderId -->" + ex.Message + sql.ToString(), ex);
            }
        }

        #region 現金,外站,貨到付款對賬

        #region 獲取泛用對賬列表 + List<OrderMasterQuery> GetOBCList(OrderMaster query, out int totalCount)
        public List<OrderMasterQuery> GetOBCList(OrderMasterQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sql1 = new StringBuilder();
            StringBuilder sql2 = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.AppendFormat(@"SELECT om.order_id,om.order_payment,tp.parameterName AS order_pay_message,om.money_cancel,om.money_return,om.order_amount,om.billing_checked,om.order_createdate ");
                sql1.AppendFormat(@" FROM order_master om LEFT JOIN (SELECT tp1.parameterCode,tp1.parameterName FROM t_parametersrc tp1 WHERE tp1.parameterType='payment') tp ON tp.parameterCode=om.order_payment");
                sql1.AppendFormat(@" LEFT JOIN (SELECT c.channel_id,c.channel_name_simple FROM channel c) ch ON om.channel=ch.channel_id ");
                sql1.AppendFormat(@" where 1=1 AND om.order_createdate >= 1356969600 AND om.order_amount != (om.money_cancel + om.money_return) ");
                if (query.Order_Createdate != 0)
                {
                    sql1.AppendFormat(@" AND om.order_createdate>='{0}'", query.Order_Createdate);
                    sql1.AppendFormat(@" AND om.order_createdate<='{0}'", query.Order_Updatedate);
                }
                if (query.Order_Id != 0)
                {
                    sql1.AppendFormat(@" AND om.order_id like '%{0}%'", query.Order_Id);
                }
                if (query.Order_Payment != 0)
                {
                    sql1.AppendFormat(@" AND om.order_payment='{0}'", query.Order_Payment);
                }
                if (query.Channel != 0)
                {
                    sql1.AppendFormat(@" AND om.channel='{0}'", query.Channel);
                }
                if (query.billing_check != -1)
                {
                    sql1.AppendFormat(@" AND om.billing_checked='{0}'", query.billing_check);
                }
                sql1.AppendFormat(@" order by om.order_id desc");
                sql2.AppendFormat(@" limit {0},{1}", query.Start, query.Limit);
                DataTable _dt = _dbAccess.getDataTable("select count(*) as totalCount " + sql1.ToString());
                if (_dt != null)
                {
                    totalCount = int.Parse(_dt.Rows[0]["totalCount"].ToString());
                }
                return _dbAccess.getDataTableForObj<OrderMasterQuery>(sql.ToString() + sql1.ToString() + sql2.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterDao-->GetOBCList -->" + ex.Message + sql.ToString() + sql1.ToString() + sql2.ToString(), ex);
            }
        }
        #endregion

        #region  賣場store + DataTable GetChannelList(Channel query)
        public List<Channel> GetChannelList(Channel query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT c.channel_id,c.channel_name_simple,c.channel_name_full FROM channel c");
                return _dbAccess.getDataTableForObj<Channel>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetChannelList -->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 付款方式store + List<Parametersrc> GetPaymentList(OrderMasterQuery query)
        public List<Parametersrc> GetPaymentList(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT tp.parameterCode,tp.parameterName FROM t_parametersrc tp WHERE tp.parameterType='payment'");
                return _dbAccess.getDataTableForObj<Parametersrc>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetPaymentList -->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 修改確認對賬狀態 + int UpdateOrderBilling(OrderMaster query)
        public int UpdateOrderBilling(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE order_master om SET om.money_collect_date='{0}',om.billing_checked={1} WHERE 1=1 ", query.Money_Collect_Date, query.Billing_Checked);
                if (query.OrderId != 0)
                {
                    sql.AppendFormat(" AND om.order_id='{0}'", query.OrderId);
                }
                if (query.Order_Amount != 0)
                {
                    sql.AppendFormat(" AND om.order_amount='{0}'", query.Order_Amount);
                }
                if (query.Money_Cancel != 0)
                {
                    sql.AppendFormat(" AND om.order_amount='{0}'", query.Money_Cancel);
                }
                if (query.billing_check != -1)
                {
                    sql.AppendFormat(" AND om.billing_checked='{0}'", query.billing_check);
                }
                return _dbAccess.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpdateOrderBilling -->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 泛用對賬匯出 + DataTable ReportOrderBillingExcel(OrderMasterQuery query)
        public DataTable ReportOrderBillingExcel(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT om.order_id,tp.parameterName AS order_pay_message,om.money_cancel,om.money_return,om.order_amount,om.billing_checked,om.order_createdate,om.money_collect_date ");
                sql.AppendFormat(@" FROM order_master om LEFT JOIN (SELECT tp1.parameterCode,tp1.parameterName FROM t_parametersrc tp1 WHERE tp1.parameterType='payment') tp ON tp.parameterCode=om.order_payment");
                sql.AppendFormat(@" LEFT JOIN (SELECT c.channel_id,c.channel_name_simple FROM channel c) ch ON om.channel=ch.channel_id ");
                sql.AppendFormat(@" where 1=1 AND om.order_createdate >= 1356969600 AND om.order_amount != (om.money_cancel + om.money_return) ");
                if (query.Order_Createdate != 0)
                {
                    sql.AppendFormat(@" AND om.order_createdate>='{0}'", query.Order_Createdate);
                    sql.AppendFormat(@" AND om.order_createdate<='{0}'", query.Order_Updatedate);
                }
                if (query.Order_Id != 0)
                {
                    sql.AppendFormat(@" AND om.order_id like '%{0}%'", query.Order_Id);
                }
                if (query.Order_Payment != 0)
                {
                    sql.AppendFormat(@" AND om.order_payment='{0}'", query.Order_Payment);
                }
                if (query.Channel != 0)
                {
                    sql.AppendFormat(@" AND om.channel='{0}'", query.Channel);
                }
                if (query.billing_check != -1)
                {
                    sql.AppendFormat(@" AND om.billing_checked='{0}'", query.billing_check);
                }
                sql.AppendFormat(@" order by om.order_id desc");
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterDao-->ReportOrderBillingExcel -->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 泛用對賬匯入對賬檢測 + DataTable CheckedImport(OrderMasterQuery query)
        public DataTable CheckedImport(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT om.order_id,tp.parameterName AS order_pay_message,om.money_cancel,om.money_return,om.order_amount ");
                sql.AppendFormat(@" FROM order_master om LEFT JOIN (SELECT tp1.parameterCode,tp1.parameterName FROM t_parametersrc tp1 WHERE tp1.parameterType='payment') tp ON tp.parameterCode=om.order_payment");
                sql.AppendFormat(@" LEFT JOIN (SELECT c.channel_id,c.channel_name_simple FROM channel c) ch ON om.channel=ch.channel_id ");
                sql.AppendFormat(@" where 1=1 ");
                if (query.OrderId != 0)
                {
                    sql.AppendFormat(@" AND om.order_id = '{0}'", query.OrderId);
                }
                if (query.Order_Amount != 0)
                {
                    sql.AppendFormat(@" AND om.order_amount='{0}'", query.Order_Amount);
                }
                sql.AppendFormat(@" AND om.order_amount='{0}'", query.Money_Cancel);
                //if (query.billing_check != -1)
                //{
                //    sql.AppendFormat(@" AND om.billing_checked='{0}'", query.billing_check);
                //}
                sql.AppendFormat(@" order by om.order_id desc");
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("OrderMasterDao-->CheckedImport -->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #endregion

        #region 根據定單編號獲取定單信息
        /// <summary>
        /// 根據定單編號獲取定單信息
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <returns>定單信息</returns>
        public OrderMaster GetOrderMasterByOrderId(int order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("SELECT order_id,user_id,bonus_receive,bonus_discount_percent,bonus_convert_nt,deduct_happygo_convert,deduct_bonus,deduct_welfare,deduct_happygo,deduct_card_bonus,deduct_account,accumulated_bonus,accumulated_happygo,order_freight_normal,order_freight_low,order_product_subtotal,order_amount,money_cancel,money_return,order_status,order_payment,order_name,order_mobile,order_phone,order_zip,order_address,delivery_name,delivery_gender,delivery_mobile,delivery_phone,delivery_zip,delivery_address,estimated_arrival_period,company_write,company_invoice,company_title,invoice_id,order_invoice,invoice_status,note_order,note_admin,order_date_pay,money_collect_date,order_date_close,order_date_cancel,order_compensate,cart_id,order_createdate,order_updatedate,order_ipfrom,source_trace,source_cookie_value,source_cookie_name,note_order_modifier,note_order_modify_time,error_check,fax_sn,channel,bonus_type,multi_bonus,channel_order_id,delivery_store,billing_checked,import_time,retrieve_mode,bonus_expire_day,priority,holiday_deliver,export_flag,data_chg,paper_invoice,deliver_stno,dcrono,stnm,account_collection_time ");
                sql.Append(" FROM order_master ");
                sql.AppendFormat(" WHERE order_id={0};", order_id);
                return _dbAccess.getSinggleObj<OrderMaster>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetOrderMasterByOrderId-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 根據定單編號更新定單狀態以開立發票
        /// <summary>
        /// 根據定單編號更新定單狀態以開立發票
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <returns>執行結果</returns>
        public int UpdateOrderToOpenInvoice(int order_id)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("UPDATE order_master as om,order_slave as os SET order_status = 99,order_date_close = NOW(),slave_status = 99,slave_date_close = NOW(),slave_date_delivery = NOW() ");
            sbSql.AppendFormat(" WHERE om.order_id = os.order_id AND om.order_id={0} ", order_id);
            try
            {
                return _dbAccess.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpdateOrderToOpenInvoice-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        #endregion

        #region 獲取需要開立發票的訂單
        /// <summary>
        /// 獲取需要開立發票的訂單
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>需要開立發票的訂單</returns>
        public OrderMasterQuery GetOrderMasterInvoice(OrderMasterQuery query)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("SELECT om.order_id,om.order_payment,om.order_freight_normal,om.order_freight_low,om.deduct_happygo_convert,om.bonus_type,om.company_invoice,om.company_title,om.order_name,om.order_zip,om.order_address,om.invoice_status,imr.invoice_number,om.deduct_card_bonus ");
            sbSql.Append(" FROM order_master om ");
            sbSql.Append(" LEFT JOIN invoice_master_record imr ON om.order_id=imr.order_id ");
            sbSql.Append(" WHERE 1=1 ");
            sbSql.AppendFormat(" AND om.order_id={0} ", query.Order_Id);
            sbSql.Append(" AND om.order_amount>0 ");
            if (query.status_description == "modify")
            {
                sbSql.Append(" AND imr.invoice_number IS NOT NULL ");
            }
            try
            {
                return _dbAccess.getSinggleObj<OrderMasterQuery>(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetOrderMasterInvoice-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        #endregion

        public bool UpdateOrderMaster(string sql)
        {
            try
            {
                return _dbAccess.execCommand(sql) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->IsExistOrderId -->" + ex.Message + sql.ToString(), ex);
            }
        }

        #region 根據定單編號獲取定單信息
        /// <summary>
        /// 根據定單編號獲取定單信息
        /// </summary>
        /// <param name="order_id">定單編號</param>
        /// <returns>定單信息</returns>
        public OrderMaster GetOrderMasterByOrderId4Change(int order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("SELECT order_id,user_id,deduct_happygo_convert,deduct_bonus,deduct_welfare,deduct_happygo,deduct_card_bonus,deduct_account,accumulated_bonus,accumulated_happygo,order_product_subtotal,order_amount,money_cancel,money_return,order_status,order_payment,order_name,order_mobile,order_phone,order_zip,order_address,delivery_name,delivery_mobile,delivery_phone,delivery_zip,delivery_address,estimated_arrival_period,company_invoice,company_title,invoice_id,order_invoice,invoice_status,note_order,note_admin,order_date_pay,money_collect_date,order_date_close,order_date_cancel,cart_id,order_createdate,order_updatedate,order_ipfrom,source_trace,source_cookie_value,source_cookie_name,note_order_modifier,note_order_modify_time,fax_sn,multi_bonus,channel_order_id,priority ");
                sql.Append(" FROM order_master ");
                sql.AppendFormat(" WHERE order_id={0};", order_id);
                return _dbAccess.getSinggleObj<OrderMaster>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetOrderMasterByOrderId4Change-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        public DataTable GetOrderidAndName(int order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT order_id,order_name as user_name,order_phone,order_mobile,order_address,delivery_name,delivery_phone,delivery_mobile,delivery_address,order_zip,delivery_zip FROM order_master LEFT JOIN users ON order_master.user_id=users.user_id WHERE order_id={0}", order_id);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetOrderidAndName-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string UpdateOrderMaster(OrderMasterQuery query, OrderShowMasterQuery osmQuery)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_master set delivery_store='{0}' ", query.Delivery_Store);
                if (query.payment == "T_CAT")
                {
                    sql.AppendFormat(" ,order_freight_normal='{0}',order_freight_low='{1}',order_payment='{2}',order_date_pay='{3}' ", query.Order_Freight_Normal, query.Order_Freight_Low, query.Order_Payment, CommonFunction.GetPHPTime());
                    if (query.t_cat_amount != 0)
                    {
                        sql.AppendFormat(",order_amount='{0}' ", query.Order_Amount);
                    }
                }
                else if (osmQuery.order_status == 0)
                {
                    sql.AppendFormat(" ,order_freight_normal='{0}',order_freight_low='{1}',order_amount='{2}',order_payment='{3}',order_date_pay='{4}' ", query.Order_Freight_Normal, query.Order_Freight_Low, query.Order_Amount, query.Order_Payment, CommonFunction.GetPHPTime());
                }
                sql.AppendFormat(" where order_id='{0}';", query.Order_Id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpDeliveryStore-->" + sql.ToString() + ex.Message, ex);
            }
        }


        public string UpOrderMaster(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (query.mdd)
                {
                    sql.AppendFormat("update order_master set delivery_name='{0}',delivery_gender='{1}',delivery_mobile='{2}',delivery_phone='{3}',delivery_zip='{4}',delivery_address='{5}',delivery_same='0',note_order='{6}' where order_id='{7}';", query.delivery_name, query.user_gender, query.Delivery_Mobile, query.Delivery_Phone, query.Delivery_Zip, query.Delivery_Address, query.note_order, query.Order_Id);
                }
                else
                {
                    sql.AppendFormat("update order_master set order_status='2',order_date_pay='{0}',order_date_cancel='{1}',money_cancel=0,export_flag=1 where order_id='{1}';", CommonFunction.GetPHPTime(), query.Order_Id);
                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpOrderMaster-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string UpOrderSlave(OrderSlaveQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_slave set  slave_status where slave_id='{0}';", query.Slave_Id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpOrderSlave-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string UpOrderDetail(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_detail set  detail_status where detail_id='{0}';", query.Detail_Id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpOrderSlave-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public DataTable GetInfo(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select  om.order_id,os.slave_id,od.detail_id from order_master om LEFT JOIN order_slave os on om.order_id=os.order_id LEFT JOIN order_detail od on os.slave_id=od.slave_id where om.order_id='{0}';", query.Order_Id);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetInfo-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public DataTable GetNextSerial(Serial serial)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                DataTable _dt = _dbAccess.getDataTable(_serial.Update(serial.Serial_id));
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetNextSerial-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string OMSRecord(OrderMasterStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert into order_master_status(serial_id,order_id,order_status,status_description,status_ipfrom,status_createdate) values( ");
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}');", query.serial_id, query.order_id, query.order_status, query.status_description, query.status_ipfrom, CommonFunction.GetPHPTime());
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->OMSRecord-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string UpDeliveryMaster(uint order_id, uint delivery_store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update deliver_master set delivery_store='{0}' where order_id='{1}';", delivery_store, order_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpDeliveryMaster-->" + sql.ToString() + ex.Message, ex);
            }
        }

        /// <summary>
        /// 更新訂單狀態sql
        /// </summary>
        /// <param name="om"></param>
        /// <returns></returns>
        public string UpdateOrderMasterStatus(OrderMaster om)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("set sql_safe_updates = 0; ");
            sql.AppendFormat("UPDATE order_master SET order_date_pay = 0,order_status='{0}',order_updatedate ='{1}',order_ipfrom='{2}' ", om.Order_Status, CommonFunction.GetPHPTime(DateTime.Now.ToString()), om.Order_Ipfrom);
            sql.AppendFormat(" WHERE order_id={0} ; ", om.Order_Id);
            sql.Append(" set sql_safe_updates = 1;");
            try
            {
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpdateOrderMasterStatus-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable IsVendorDeliver(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select count(om.order_id) as search_total from order_detail od INNER JOIN order_slave os on od.slave_id=os.slave_id INNER JOIN order_master om on os.order_id=om.order_id where 1=1 and od.product_mode=1  and om.order_id='{0}' ;", order_id);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->IsVendorDeliver-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string ModifyOrderStatus(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("UPDATE order_master om,order_slave os,order_detail od SET om.order_status='2',os.slave_status='2',od.detail_status='2',om.order_date_pay='{0}',om.order_date_cancel=0,om.money_cancel=0,om.export_flag=1 where om.order_id='{1}' AND om.order_id=os.order_id AND os.slave_id=od.slave_id;", CommonFunction.GetPHPTime(), query.Order_Id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->ModifyOrderStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public DataTable IsFirstTime(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select user_id from users where user_id='{0}' and first_time=0; ", query.user_id);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->IsFirstTime-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public string UpFirstTime(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_master om,users u set om.priority=1,u.first_time='{0}' where om.order_id='{1}' and om.user_id=u.user_id;", CommonFunction.GetPHPTime(), query.Order_Id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpFirstTime-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable CheckDeliveryStatus(OrderMasterQuery om)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select deliver_id,delivery_status from deliver_master where order_id='{0}';", om.Order_Id);
                return _dbAccess.getDataTable(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->CheckDeliveryStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string UpdateDM(OrderMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update deliver_master set delivery_name='{0}',delivery_mobile='{1}',delivery_phone='{2}',delivery_zip='{3}',delivery_address='{4}' where order_id='{5}';", query.delivery_name, query.Delivery_Mobile, query.Delivery_Phone, query.Delivery_Zip, query.Delivery_Address, query.Order_Id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->UpdataDM-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable VerifySession(uint user_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select odm_user_id from order_detail_manager where odm_user_id='{0}' and odm_status=1;", user_id);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->VerifySession-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable OrderSerchExport(OrderMasterQuery query)
        {
            DataTable _dt = new DataTable();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("SELECT u.user_name,FROM_UNIXTIME(om.order_createdate) 'order_createdate',om.order_id,om.order_payment, om.order_amount,om.order_status, od.item_id,");
                sql.Append(" od.detail_status,v.vendor_name_simple,v.vendor_code,od.product_name,od.buy_num,od.single_money,od.deduct_bonus,od.deduct_welfare,od.single_money*buy_num 'total_money' ,od.item_mode,od.parent_num, ");
                sql.Append(" od.single_cost,od.bag_check_money,od.single_cost*od.buy_num 'total_cost' , FROM_UNIXTIME(os.slave_date_close) 'slave_date_close',mu.user_username as 'pm',om.source_trace as 'ID',redirect_name, od.product_mode   ");
                sql.Append(" from order_master om LEFT JOIN order_slave os ON om.order_id=os.order_id  ");
                sql.Append(" LEFT JOIN order_detail od ON os.slave_id=od.slave_id  ");
                sql.Append("LEFT JOIN vendor v ON v.vendor_id = od.item_vendor_id left join redirect r on r.redirect_id=om.source_trace  ");
                sql.Append(" LEFT JOIN redirect_group rg  ON r.group_id = rg.group_id  LEFT JOIN manage_user mu on mu.user_id=v.product_manage LEFT JOIN users u on u.user_id=om.user_id ");
                sql.AppendFormat(" where  od.item_mode in (0,2) and om.order_date_pay<>0 and om.order_createdate>='{0}' and  om.order_createdate<='{1}'; ", CommonFunction.GetPHPTime(query.datestart.ToString()), CommonFunction.GetPHPTime(query.dateend.ToString()));
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->OrderSerchExport-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetPara(string type, int order_status)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select  remark   from t_parametersrc where parameterType='{0}'  and parameterCode='{1}';", type, order_status);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetParaByOrderStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetParaByPayment(int payment)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select  parameterName   from t_parametersrc where parameterType='payment'  and parameterCode='{0}';", payment);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetParaByPayment-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetOrderFreight(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select order_freight_normal,order_freight_low from order_master where order_id='{0}';", order_id);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetOrderFreight-->" + sql.ToString() + ex.Message, ex);
            }
        }
      
        //訂單明細匯出sql1
        public DataTable GetOrderDetialExportOrderid(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlSingle = new StringBuilder();
            StringBuilder sqlFather = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sql.Append(@"SELECT * from (SELECT order_id,SUM(money) as amount from ( ");
                sqlSingle.AppendFormat(@"
SELECT om.order_id,od.detail_id,od.single_money*od.buy_num-od.deduct_bonus-od.deduct_welfare as money FROM order_master om 
LEFT JOIN order_slave os ON om.order_id=os.order_id
LEFT JOIN order_detail od ON os.slave_id=od.slave_id
INNER JOIN product_item pi ON od.item_id=pi.item_id
INNER JOIN product_category_set pcs ON pi.product_id=pcs.product_id
WHERE od.item_mode=0  AND pcs.category_id={0} AND od.detail_status NOT IN (89,90,91) AND om.order_status NOT IN(90,91)", query.category_id);
                sqlFather.AppendFormat(@" 
SELECT om.order_id,od.detail_id,od.single_money*od.buy_num-od.deduct_bonus-od.deduct_welfare as money FROM order_master om 
LEFT JOIN order_slave os ON om.order_id=os.order_id
LEFT JOIN order_detail od ON os.slave_id=od.slave_id
INNER JOIN product_category_set pcs ON od.parent_id=pcs.product_id
WHERE od.item_mode=1  AND pcs.category_id={0} AND od.detail_status NOT IN (89,90,91) AND om.order_status NOT IN(90,91)", query.category_id);
                if (query.category_status != 0)
                {
                    sqlWhere.AppendFormat(" AND om.money_collect_date > 0");
                }
                if (query.date_stauts != 0)
                {
                    if (query.date_start != DateTime.MinValue && query.date_end != DateTime.MinValue)
                    {
                        sqlWhere.AppendFormat(" AND om.order_createdate>='{0}' and  om.order_createdate<='{1}'", CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.date_start)), CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.date_end)));
                    }
                }
                sql.Append(sqlSingle.ToString() + sqlWhere.ToString() + " UNION " + sqlFather.ToString() + sqlWhere.ToString() + " ) amount  GROUP BY order_id) c_amount ");
                if (query.c_money > 0 || query.c_money1 > 0)
                {
                    sql.AppendFormat(" where amount>={0} and amount<={1} ",query.c_money,query.c_money1);
                }
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetOrderDetialExportOrderid-->" + sqlSingle.ToString() + ex.Message, ex);
            }
        }
        //訂單明細匯出sql2
        public DataTable OrderDetialExportInfo(int order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT DISTINCT od.detail_id,om.order_name,om.order_createdate,'' as order_createdate_format,om.order_id,om.order_payment,'' as  payment_name,
om.order_amount,om.order_status,'' as order_status_name,od.item_id,os.slave_status,'' as slave_status_name,od.product_name,
od.deduct_bonus,od.deduct_welfare,od.single_money,od.buy_num,od.parent_num,od.single_cost,od.bag_check_money,od.single_cost,os.slave_date_close,'' as slave_date_close_format,
od.product_mode ,'' as product_mode_name,od.item_mode,om.delivery_name,om.delivery_address,'' as amount,'' as cost_amount,od.event_cost,od.parent_id,pi.product_id
FROM order_master om
LEFT JOIN order_slave os ON  om.order_id=os.order_id
LEFT JOIN order_detail od ON os.slave_id=od.slave_id                              
INNER JOIN product_item pi ON od.item_id=pi.item_id                   
WHERE om.order_id='{0}' AND od.detail_status NOT IN (89,90,91) ORDER BY detail_id; ", order_id);
                return _dbAccess.getDataTable(sql.ToString()); 
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->OrderDetialExportInfo-->" + sql.ToString() + ex.Message, ex);
            }
        }

        //類別訂單明細匯出sql
//        public DataTable CagegoryDetialExport(OrderDetailQuery query)
//        {
//            StringBuilder sqlSingle = new StringBuilder();
//            StringBuilder sql = new StringBuilder();
//            StringBuilder sqlFather = new StringBuilder();
//            StringBuilder sqlJoin1 = new StringBuilder();
//            StringBuilder sqlJoin2 = new StringBuilder();
//            StringBuilder sqlWhere = new StringBuilder();
//            try
//            {
//                sql.Append(@"SELECT * from (
//SELECT order_id,SUM(money) as money,category_id,order_name,order_createdate,'' as order_createdate_format,order_payment,'' as  payment_name,order_amount,order_status,'' as order_status_name,
//item_id,slave_status,'' as slave_status_name,product_name,deduct_bonus,deduct_welfare,single_cost,bag_check_money,slave_date_close,'' as slave_date_close_format,'' as amount,
//'' as cost_amount,product_mode,'' as product_mode_name,item_mode,delivery_name,delivery_address,detail_id,event_cost,product_id,deliver_id,delivery_code,delivery_store,
//'' as deliver_name,buy_num,single_money from (");
//                sqlSingle.AppendFormat(@"(SELECT om.order_id,pcs.category_id,om.order_name,od.buy_num,od.single_money,od.buy_num*od.single_money-od.deduct_bonus-od.deduct_welfare as money,om.order_createdate,om.order_payment,
//om.order_amount,om.order_status,od.item_id,os.slave_status,od.product_name,od.deduct_bonus,od.deduct_welfare,od.single_cost,od.bag_check_money,os.slave_date_close,
//od.product_mode,od.item_mode,om.delivery_name,om.delivery_address ,od.detail_id,od.event_cost,pi.product_id,dd.deliver_id,dm.delivery_code,dm.delivery_store FROM order_master om ");

//                sqlFather.AppendFormat(@"(SELECT om.order_id,pcs.category_id,om.order_name,od.buy_num,od.single_money,od.buy_num*od.single_money-od.deduct_bonus-od.deduct_welfare as money,om.order_createdate,om.order_payment,
//om.order_amount,om.order_status,od.item_id,os.slave_status,od.product_name,od.deduct_bonus,od.deduct_welfare,od.single_cost,od.bag_check_money,os.slave_date_close,
//od.product_mode,od.item_mode,om.delivery_name,om.delivery_address ,od.detail_id,od.event_cost,od.parent_id as product_id,dd.deliver_id,dm.delivery_code,dm.delivery_store FROM order_master om ");
//                sqlJoin1.AppendFormat(@" 
//LEFT JOIN order_slave os ON om.order_id=os.order_id
//LEFT JOIN order_detail od ON os.slave_id=od.slave_id                                       
//INNER JOIN product_item pi ON od.item_id=pi.item_id
//INNER JOIN product_category_set pcs ON pi.product_id=pcs.product_id
//LEFT JOIN deliver_detail dd ON od.detail_id=dd.detail_id
//LEFT JOIN deliver_master dm ON dd.deliver_id=dm.deliver_id
//WHERE od.item_mode=0  AND pcs.category_id={0}
//AND od.detail_status NOT IN(89,90,91) AND om.order_status NOT IN(90,91)", query.category_id);
//                sqlJoin2.AppendFormat(@"
//LEFT JOIN order_slave os ON om.order_id=os.order_id
//LEFT JOIN order_detail od ON os.slave_id=od.slave_id
//INNER JOIN product_category_set pcs ON od.parent_id=pcs.product_id
//LEFT JOIN deliver_detail dd ON od.detail_id=dd.detail_id
//LEFT JOIN deliver_master dm ON dd.deliver_id=dm.deliver_id
//WHERE od.item_mode=1  AND pcs.category_id={0}
//AND od.detail_status NOT IN(89,90,91) AND om.order_status NOT IN(90,91)", query.category_id);
//                if (query.category_status != 0)
//                {
//                    sqlWhere.AppendFormat(" AND om.money_collect_date > 0");
//                }
//                if (query.date_stauts != 0)
//                {
//                    if (query.date_start != DateTime.MinValue && query.date_end != DateTime.MinValue)
//                    {
//                        sqlWhere.AppendFormat(" AND om.order_createdate>='{0}' and  om.order_createdate<='{1}'", CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.date_start)), CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.date_end)));
//                    }
//                }
//                sqlWhere.AppendFormat(" GROUP BY  od.detail_id ");
//                sql.Append(sqlSingle.ToString() + sqlJoin1.ToString() + sqlWhere.ToString() + ")UNION" + sqlFather.ToString() + sqlJoin2.ToString() + sqlWhere.ToString() + ") ) amount GROUP BY order_id) c_amount ");

//                if (query.c_money > 0 || query.c_money1 > 0)
//                {
//                    sql.AppendFormat(" where money>={0} and money<={1} ", query.c_money, query.c_money1);
//                }
//                return _dbAccess.getDataTable(sql.ToString());
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("OrderMasterDao-->CategoryDetialExportInfo-->" + sqlSingle.ToString() + ex.Message, ex);
//            }
//        }

        public DataTable CagegoryDetialExport(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlSingle = new StringBuilder();
            StringBuilder sqlFather = new StringBuilder();
            StringBuilder sqlJoin1 = new StringBuilder();
            StringBuilder sqlJoin2 = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                try
                {
                    sql.Append(@"
SELECT order_id,category_id,order_name,order_createdate,'' as order_createdate_format,order_payment,'' as  payment_name,order_amount,order_status,'' as order_status_name,
item_id,slave_status,'' as slave_status_name,product_name,deduct_bonus,deduct_welfare,single_cost,bag_check_money,slave_date_close,'' as slave_date_close_format,'' as amount,
'' as cost_amount,product_mode,'' as product_mode_name,item_mode,delivery_name,delivery_address,detail_id,event_cost,product_id,deliver_id,delivery_code,delivery_store,
'' as deliver_name,buy_num,single_money from (");
                    sqlSingle.Append(@"
( SELECT om.order_id,pcs.category_id,om.order_name,od.buy_num,od.single_money,om.order_createdate,om.order_payment,
om.order_amount,om.order_status,od.item_id,os.slave_status,od.product_name,od.deduct_bonus,od.deduct_welfare,od.single_cost,od.bag_check_money,os.slave_date_close,
od.product_mode,od.item_mode,om.delivery_name,om.delivery_address ,od.detail_id,od.event_cost,pi.product_id,dd.deliver_id,dm.delivery_code,dm.delivery_store FROM order_master om ");
                    sqlFather.Append(@"
( SELECT om.order_id,pcs.category_id,om.order_name,od.buy_num,od.single_money,om.order_createdate,om.order_payment,
om.order_amount,om.order_status,od.item_id,os.slave_status,od.product_name,od.deduct_bonus,od.deduct_welfare,od.single_cost,od.bag_check_money,os.slave_date_close,
od.product_mode,od.item_mode,om.delivery_name,om.delivery_address ,od.detail_id,od.event_cost,od.parent_id as product_id,dd.deliver_id,dm.delivery_code,dm.delivery_store FROM order_master om");
                    sqlJoin1.AppendFormat(@"
LEFT JOIN order_slave os ON om.order_id=os.order_id
LEFT JOIN order_detail od ON os.slave_id=od.slave_id                                       
INNER JOIN product_item pi ON od.item_id=pi.item_id
INNER JOIN product_category_set pcs ON pi.product_id=pcs.product_id
LEFT JOIN deliver_detail dd ON od.detail_id=dd.detail_id
LEFT JOIN deliver_master dm ON dd.deliver_id=dm.deliver_id
WHERE od.item_mode=0  AND pcs.category_id='{0}'
AND od.detail_status NOT IN(89,90,91) AND om.order_status NOT IN(90,91) AND om.order_id='{1}' )", query.category_id,query.Order_Id);
                    sqlJoin2.AppendFormat(@"
LEFT JOIN order_slave os ON om.order_id=os.order_id
LEFT JOIN order_detail od ON os.slave_id=od.slave_id
INNER JOIN product_category_set pcs ON od.parent_id=pcs.product_id
LEFT JOIN deliver_detail dd ON od.detail_id=dd.detail_id
LEFT JOIN deliver_master dm ON dd.deliver_id=dm.deliver_id
WHERE od.item_mode=1 AND pcs.category_id='{0}' AND od.detail_status NOT IN(89,90,91) AND om.order_status NOT IN(90,91) AND om.order_id='{1}' )", query.category_id, query.Order_Id);
                    sql.Append(sqlSingle.ToString() + sqlJoin1.ToString() + " UNION " + sqlFather.ToString() + sqlJoin2.ToString() + ") c_amount ");
                    return _dbAccess.getDataTable(sql.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("OrderMasterDao-->OrderDetialExportInfo-->" + sql.ToString() + ex.Message, ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->CategoryDetialExportInfo-->" + sqlSingle.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetInvoiceData(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" select invoice_number,total_amount,invoice_date from invoice_master_record where order_id='{0}' order by invoice_id DESC limit 1;", order_id);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetInvoiceData-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public DataTable GetInvoice(uint order_id, uint pid)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("SELECT invoice_number,invoice_date,imr.tax_type,total_amount,order_id from product p LEFT JOIN invoice_master_record  imr ON p.tax_type=imr.tax_type WHERE  imr.tax_type IN (1,3) AND p.product_id='{0}' and imr.order_id='{1}' ;", pid, order_id);
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetInvoiceData-->" + sql.ToString() + ex.Message, ex);
            }
        }

        #region MyRegion
        
       
        /// <summary>
        /// chaojie1124j add by 2015/12/14 12/32am
        /// 用於實現付款金額統計排程
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetCheckOrderAmount(OrderMasterQuery query)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder strCondi = new StringBuilder();
            try
            {
                sb.Append(" SELECT  order_id,money_collect_date,order_amount,us.user_name,us.user_email,om.order_ipfrom,om.order_id,FROM_UNIXTIME(om.money_collect_date)as new_time  ");
                sb.Append(" FROM order_master om LEFT JOIN users us on us.user_id=om.user_id ");
                strCondi.Append(" where 1=1 and om.order_amount>(select parameterName from t_parametersrc where parameterType='auto_paramer' and parameterCode='order_amount_limit' ) ");
                if (query.user_id != 0)
                {
                    strCondi.AppendFormat(" and om.user_id='{0}' ", query.user_id);
                }
                if (query.Money_Collect_Date != 0)
                {
                    strCondi.AppendFormat(" and om.money_collect_date>'{0}' ", query.Money_Collect_Date);
                }
                return _dbAccess.getDataTable(sb.ToString()+strCondi.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetCheckOrderAmount-->" + sb.ToString() + strCondi .ToString() + ex.Message, ex);
            }

        }
        /// <summary>
        /// chaojie1124j add by 2015/12/14 01/36pm
        ///  用於實現付款金額統計排程
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable GetUsersOrderAmount(OrderMasterQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
               sb.Append(@"SELECT user_id,odcount  from (  SELECT count(1) as odcount,user_id FROM ( ");
               sb.Append(" select order_amount,user_id from  order_master where order_amount>(select parameterName from t_parametersrc where parameterType='auto_paramer' and parameterCode='order_amount_limit' ) ");
               sb.AppendFormat("  and money_collect_date>'{0}' ) as new_table GROUP BY user_id) as new_tabletwo  ",query.Money_Collect_Date);
               sb.Append(" WHERE odcount>(select parameterName from t_parametersrc where parameterType='auto_paramer' and parameterCode='order_count_limit') ");
                return _dbAccess.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterDao-->GetUsersOrderAmount-->" + sb.ToString() + ex.Message, ex);
            }
        }
        #endregion

    }
}