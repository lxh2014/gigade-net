using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;
using System.Data;
using MySql.Data.MySqlClient;
using BLL.gigade.Dao;
using BLL.gigade.Common;
using System.Net;
using System.Xml;
using System.Configuration;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    public class OrderCancelMasterDao : IOrderCancelMasterImplDao
    {
        private IDBAccess _accessMySql;
        private IBonusMasterImplDao _bonus;
        private ISerialImplDao _serial;
        private string connString;
        SerialDao _serialDao = new SerialDao("");
        public OrderCancelMasterDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }
        #region 獲取取消單列表+List<OrderCancelMaster> GetOrderCancelMasterList(OrderCancelMaster ocm, out int totalCount)
        /// <summary>
        /// 獲取取消單列表
        /// </summary>
        /// <param name="ocm"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<OrderCancelMaster> GetOrderCancelMasterList(OrderCancelMaster ocm, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select cancel_id,order_id,cancel_status,cancel_note,bank_note,FROM_UNIXTIME(cancel_createdate)as cancel_createdate,FROM_UNIXTIME(cancel_updatedate)as cancel_updatedate,cancel_ipfrom from order_cancel_master order by cancel_createdate desc");
                //分頁
                totalCount = 0;
                if (ocm.IsPage)
                {
                    System.Data.DataTable _dt = _accessMySql.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                    sql.AppendFormat(" limit {0},{1}", ocm.Start, ocm.Limit);
                }
                return _accessMySql.getDataTableForObj<OrderCancelMaster>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnlMasterDao-->GetOrderCancelMasterList-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 取消單歸檔修改+int Update(OrderCancelMaster ocm)
        /// <summary>
        /// 取消單歸檔修改
        /// </summary>
        /// <param name="ocm"></param>
        /// <returns></returns>
        public int Update(OrderCancelMaster ocm)
        {
            _serialDao = new SerialDao(connString);
            DataTable ordermaster = GetOrderMaster(ocm.order_id);
            string user_id = string.Empty;
            uint order_date_pay = 0;
            uint order_amount = 0;
            uint order_payment = 0;
            int total_product_pay_money = 0;
            int total_product_deduct_bonus = 0;
            int total_product_deduct_cash = 0;

            int accumulated_bonus = 0;
            int accumulated_happygo = 0;
            int deduct_happygo = 0;
            int deduct_happygo_money = 0;
            int cancel_money = 0;
            if (ordermaster.Rows.Count > 0)
            {
                user_id = ordermaster.Rows[0]["user_id"].ToString();
                if (!string.IsNullOrEmpty(ordermaster.Rows[0]["order_date_pay"].ToString()))
                {
                    order_date_pay = (uint)ordermaster.Rows[0]["order_date_pay"];
                }
                if (!string.IsNullOrEmpty(ordermaster.Rows[0]["order_amount"].ToString()))
                {
                    order_amount = (uint)ordermaster.Rows[0]["order_amount"];
                }
                if (!string.IsNullOrEmpty(ordermaster.Rows[0]["order_payment"].ToString()))
                {
                    order_payment = Convert.ToUInt32(ordermaster.Rows[0]["order_payment"]);
                }
            }
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            int result = 0;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sqlstr.AppendFormat(@"set sql_safe_updates=0;update order_cancel_master set cancel_status='{0}',cancel_note='{1}',bank_note='{2}',", ocm.cancel_status, ocm.cancel_note, ocm.bank_note);
                sqlstr.AppendFormat(@"cancel_createdate='{0}',cancel_updatedate='{1}',cancel_ipfrom='{2}'", CommonFunction.GetPHPTime(ocm.cancel_createdate.ToString("yyyy-MM-dd 0:0:0")), CommonFunction.GetPHPTime(ocm.cancel_updatedate.ToString("yyyy-MM-dd 0:0:0")), ocm.cancel_ipfrom);
                sqlstr.AppendFormat(@" where cancel_id='{0}';set sql_safe_updates=1;", ocm.cancel_id);
                mySqlCmd.CommandText = sqlstr.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                sqlstr.Clear();

                #region 若歸檔,判斷是否成立退款單
                if (ocm.cancel_status == 1)
                {
                    sql.AppendFormat(@"select od.detail_id,od.item_id,od.item_mode,od.parent_id,od.pack_id,od.single_money,");
                    sql.AppendFormat(@"od.deduct_bonus,od.buy_num,od.deduct_welfare,od.accumulated_bonus,od.accumulated_happygo,");
                    sql.AppendFormat(@"od.deduct_happygo,od.deduct_happygo_money");
                    sql.AppendFormat(@" from	order_detail od,order_cancel_detail  ocd");
                    sql.AppendFormat(@" where 1=1 and od.item_mode in (0 , 1) and ocd.detail_id = od.detail_id and ocd.cancel_id ={0}", ocm.cancel_id);
                    DataTable dt = _accessMySql.getDataTable(sql.ToString());
                    sql.Clear();
                    sql.Append("select parameterCode,parameterName,remark from  t_parametersrc where parameterType='refundment'");
                    DataTable _dtpayment = _accessMySql.getDataTable(sql.ToString());
                    sql.Clear();
                    Dictionary<uint, string> payment = new Dictionary<uint, string>();
                    foreach (DataRow item in _dtpayment.Rows)
                    {
                        if (!payment.Keys.Contains(uint.Parse(item["parameterCode"].ToString())))
                        {
                            payment.Add(uint.Parse(item["parameterCode"].ToString()), item["remark"].ToString());
                        }
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(dr["single_money"].ToString()) && !string.IsNullOrEmpty(dr["buy_num"].ToString()))
                        {
                            total_product_pay_money += Convert.ToInt32(dr["single_money"]) * Convert.ToInt32(dr["buy_num"]);
                        }
                        if (!string.IsNullOrEmpty(dr["deduct_bonus"].ToString()))
                        {
                            total_product_deduct_bonus += Convert.ToInt32(dr["deduct_bonus"]);
                        }
                        if (!string.IsNullOrEmpty(dr["deduct_welfare"].ToString()))
                        {
                            total_product_deduct_cash += Convert.ToInt32(dr["deduct_welfare"]);
                        }
                        if (!string.IsNullOrEmpty(dr["accumulated_bonus"].ToString()))
                        {
                            accumulated_bonus += Convert.ToInt32(dr["accumulated_bonus"]);
                        }
                        if (!string.IsNullOrEmpty(dr["accumulated_happygo"].ToString()))
                        {
                            accumulated_happygo += Convert.ToInt32(dr["accumulated_happygo"]);
                        }
                        if (!string.IsNullOrEmpty(dr["deduct_happygo"].ToString()))
                        {
                            deduct_happygo += Convert.ToInt32(dr["deduct_happygo"]);
                        }
                        if (!string.IsNullOrEmpty(dr["deduct_happygo_money"].ToString()))
                        {
                            deduct_happygo_money += Convert.ToInt32(dr["deduct_happygo_money"]);
                        }
                    }

                    // 退款金額 = 商品購買金額 - 購物金+抵用券
                    cancel_money = total_product_pay_money - (total_product_deduct_bonus + total_product_deduct_cash + deduct_happygo_money);
                    if (!string.IsNullOrEmpty(ordermaster.Rows[0]["money_collect_date"].ToString()))
                    {
                        #region 判斷是否有付款
                        if (Convert.ToInt32(ordermaster.Rows[0]["money_collect_date"]) > 0 && order_amount > 0 && cancel_money > 0)
                        {
                            // 退款方式
                            int money_type = 0;
                            if (payment.Keys.Contains(order_payment))
                            {
                                money_type = int.Parse(payment[order_payment]);
                            }
                            sqlstr.Append(_serialDao.Update(46));
                            mySqlCmd.CommandText = sqlstr.ToString();
                            result = mySqlCmd.ExecuteNonQuery();
                            sqlstr.Clear();
                            //新增退款單
                            sqlstr.AppendFormat(@"insert into order_money_return (money_id,order_id,money_type,money_total,");
                            sqlstr.AppendFormat(@"money_status,money_note,bank_note,money_source,money_createdate,money_updatedate,money_ipfrom)");
                            sqlstr.AppendFormat(@" values((select serial_value from serial where serial_id=46),'{0}','{1}','{2}',", ocm.order_id, money_type, cancel_money);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}','{3}',", 0, " ", ocm.bank_note, "cancel_id:" + ocm.cancel_id);
                            sqlstr.AppendFormat(@"'{0}','{1}','{2}');", CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(ocm.cancel_updatedate)), CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(ocm.cancel_updatedate)), ocm.cancel_ipfrom);
                            mySqlCmd.CommandText = sqlstr.ToString();
                            result = mySqlCmd.ExecuteNonQuery();
                            sqlstr.Clear();

                            int bonus_num = accumulated_bonus;
                            int user_bonus = GetUserBonus(user_id, 1);
                            // 有付款才去扣給予的購物金
                            if (accumulated_bonus > 0)
                            {
                                sql.AppendFormat("select count(type_id) as num from bonus_type where type_id={0}", 32);
                                DataTable _dtType = _accessMySql.getDataTable(sql.ToString());
                                sql.Clear();
                                if (accumulated_bonus > bonus_num)
                                {
                                    mySqlCmd.Transaction.Rollback();
                                    return 1;//bonus not enough;
                                }
                                if (Convert.ToInt32(_dtType.Rows[0][0]) <= 0)
                                {
                                    mySqlCmd.Transaction.Rollback();
                                    return 2;//bonus type error 
                                }
                                else
                                {
                                    sqlstr.Append(Deduct_User_Bonus(accumulated_bonus, ocm.order_id.ToString(), user_id));
                                    mySqlCmd.CommandText = sqlstr.ToString();
                                    result = mySqlCmd.ExecuteNonQuery();
                                    sqlstr.Clear();
                                }
                            }
                            // 有付款才去扣給予的HG點數
                            if (accumulated_happygo > 0)
                            {
                                if (accumulated_happygo != 0 && !string.IsNullOrEmpty(ocm.order_id.ToString()))
                                {
                                    sql.AppendFormat(@"select * from hg_deduct where order_id={0} limit 0,1", ocm.order_id.ToString());
                                    DataTable hg_deduct = _accessMySql.getDataTable(sql.ToString());
                                    sql.Clear();
                                    if (hg_deduct.Rows.Count > 0)
                                    {
                                        sqlstr.Append(Deduct_User_Happy_Go(accumulated_happygo, ocm.order_id.ToString(), hg_deduct));
                                        mySqlCmd.CommandText = sqlstr.ToString();
                                        result = mySqlCmd.ExecuteNonQuery();
                                        sqlstr.Clear();
                                    }
                                    else
                                    {
                                        mySqlCmd.Transaction.Rollback();
                                        return 3;//取得身分證字號失敗;
                                    }
                                }
                                else
                                {
                                    mySqlCmd.Transaction.Rollback();
                                    return 4;//扣除HappyGo點數失敗;
                                }


                            }


                        }
                        #endregion
                    }
                    // 寫入付款單退款金額
                    if (cancel_money > 0)
                    {
                        sqlstr.AppendFormat(@"set sql_safe_updates=0;update order_master set	money_cancel = money_cancel +{0} where order_id ='{1}';set sql_safe_updates=1;", cancel_money, ocm.order_id);
                        mySqlCmd.CommandText = sqlstr.ToString();
                        result = mySqlCmd.ExecuteNonQuery();
                        sqlstr.Clear();
                    }
                    sql.AppendFormat(" select *from bonus_type where type_id='{0}';", 4);
                    DataTable _dtbonus = _accessMySql.getDataTable(sql.ToString());
                    sql.Clear();
                    //判斷是否要退回購物金
                    if (total_product_deduct_bonus > 0)
                    {

                        if (_dtbonus.Rows.Count <= 0)
                        {
                            mySqlCmd.Transaction.Rollback();
                            return 2;//bonus type error !
                        }
                        sqlstr.Append(Deduct_Refund(ocm.order_id, total_product_deduct_bonus, 0, 0, ocm.cancel_ipfrom));
                        if (!string.IsNullOrEmpty(sqlstr.ToString()))
                        {
                            mySqlCmd.CommandText = sqlstr.ToString();
                            result = mySqlCmd.ExecuteNonQuery();
                            sqlstr.Clear();
                        }
                    }

                    // 退回購買扣抵的hp點數
                    if (deduct_happygo > 0)
                    {
                        if (ocm.order_id != 0 && deduct_happygo != 0)
                        {
                            sqlstr.Append(Deduct_Refund(ocm.order_id, 0, 0, deduct_happygo, ocm.cancel_ipfrom));
                            mySqlCmd.CommandText = sqlstr.ToString();
                            result = mySqlCmd.ExecuteNonQuery();
                            sqlstr.Clear();
                        }
                        else
                        {
                            return 4;//扣除HappyGo點數失敗;
                        }
                    }
                    #region 商品數量補回
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["item_mode"].ToString() == "1")
                        {
                            DataTable p = Get_Combined_Product(ocm.order_id, dr["parent_id"].ToString(), dr["pack_id"].ToString());
                            if (p.Rows.Count > 0)
                            {
                                foreach (DataRow drp in p.Rows)
                                {
                                    //modify_item_stock((int)drp["item_id"], (int)drp["buy_num"] * (int)drp["parent_num"]);

                                    sqlstr.AppendFormat(@"set sql_safe_updates=0;update product_item set item_stock = item_stock +{0} where item_id ='{1}';set sql_safe_updates=1;", Convert.ToInt32(drp["buy_num"]) * Convert.ToInt32(drp["parent_num"]), drp["item_id"].ToString());
                                }
                            }
                        }
                        else
                        {
                            //modify_item_stock((int)dr["item_id"], (int)dr["buy_num"]);

                            sqlstr.AppendFormat(@"set sql_safe_updates=0;update product_item set item_stock = item_stock +{0} where item_id ='{1}';set sql_safe_updates=1;", dr["buy_num"], dr["item_id"]);
                        }
                    }
                    #endregion

                }
                if (!string.IsNullOrEmpty(sqlstr.ToString()))
                {
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                sql.AppendFormat("SELECT count(*) total FROM	order_detail INNER JOIN order_slave on order_detail.slave_id = order_slave.slave_id WHERE order_detail.detail_status not in (89,90,91) and order_slave.order_id ='{0}'", ocm.order_id);

                DataTable _dtrow = _accessMySql.getDataTable(sql.ToString());
                sql.Clear();
                if (Convert.ToInt32(_dtrow.Rows[0][0]) == 0)
                {
                    sql.AppendFormat("select priority FROM order_master WHERE order_id ='{0}';", ocm.order_id);
                    DataTable _dtpriority = _accessMySql.getDataTable(sql.ToString());
                    sql.Clear();
                    if (Convert.ToInt32(_dtpriority.Rows[0]["priority"]) == 1)
                    {
                        sqlstr.AppendFormat("set sql_safe_updates=0;update order_master set priority = 0 where order_id='{0}';set sql_safe_updates=1;", ocm.order_id);
                        sqlstr.AppendFormat("set sql_safe_updates=0; update users set first_time = 0 where user_id ='{0}';set sql_safe_updates=1;", user_id);

                    }

                }
                if (!string.IsNullOrEmpty(sqlstr.ToString()))
                {
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                sql.AppendFormat("SELECT count(*) total FROM order_detail INNER JOIN order_slave on order_detail.slave_id = order_slave.slave_id WHERE order_detail.detail_status not in (89,90,91)and order_detail.site_id = 7 and order_slave.order_id ='{0}';", ocm.order_id);
                DataTable _dtodRow = _accessMySql.getDataTable(sql.ToString());
                sql.Clear();
                if (Convert.ToInt32(_dtodRow.Rows[0][0]) == 0)
                {
                    sql.AppendFormat(" select *from user_recommend where order_id='{0}';", ocm.order_id);
                    _dtodRow = _accessMySql.getDataTable(sql.ToString());
                    sql.Clear();
                    if (_dtodRow.Rows.Count > 0)
                    {
                        sqlstr.AppendFormat(" set sql_safe_updates=0;update user_recommend set is_recommend = 0 where id ='{0}';set sql_safe_updates=1;", _dtodRow.Rows[0]["id"]);

                    }

                }
                #endregion

                if (!string.IsNullOrEmpty(sqlstr.ToString()))
                {
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                mySqlCmd.Transaction.Commit();
                return 100;//完成!
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderCancelMasterDao-->Update-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }

        }
        /**
         *Add by chaojie1124j 2015/08/20
         *實現訂單內容裡面的<取消整筆訂單>
         */
        public int ReturnAllOrder(OrderMaster om)
        {
            _serialDao = new SerialDao(connString);
            DataTable ordermaster = GetOrderMaster(om.Order_Id);
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            om.Replace4MySQL();
            int result = 0;
            int accumulated_bonus = 0;
            int accumulated_happygo = 0;
            int deduct_happygo = 0;
            if (ordermaster.Rows.Count <= 0)
            {
                return 0;//Order error;
            }
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                List<OrderSlave> orderSlaveList = new List<OrderSlave>();
                sql.AppendFormat("select slave_id,slave_status from order_slave where order_id='{0}';", om.Order_Id);

                Dictionary<uint, string> SendMail = new Dictionary<uint, string>();
                orderSlaveList = _accessMySql.getDataTableForObj<OrderSlave>(sql.ToString());
                sql.Clear();

                List<OrderDetail> orderDetailList = new List<OrderDetail>();
                sql.Append("select od.detail_id,od.item_id,od.buy_num,od.parent_num,od.item_mode,od.accumulated_bonus,od.accumulated_happygo,od.deduct_happygo");
                sql.AppendFormat("  FROM order_detail od,order_slave os WHERE os.order_id ='{0}' AND os.slave_id = od.slave_id ;", om.Order_Id);
                orderDetailList = _accessMySql.getDataTableForObj<OrderDetail>(sql.ToString());
                sql.Clear();
                 accumulated_bonus =string.IsNullOrEmpty(ordermaster.Rows[0]["accumulated_bonus"].ToString())?0:Convert.ToInt32(ordermaster.Rows[0]["accumulated_bonus"]);
                 accumulated_happygo = string.IsNullOrEmpty(ordermaster.Rows[0]["accumulated_happygo"].ToString()) ? 0 : Convert.ToInt32(ordermaster.Rows[0]["accumulated_happygo"]);
                 deduct_happygo = string.IsNullOrEmpty(ordermaster.Rows[0]["deduct_happygo"].ToString()) ? 0 : Convert.ToInt32(ordermaster.Rows[0]["deduct_happygo"]);

                //for (int i = 0; i < orderDetailList.Count; i++)
                //{
                //    if (orderDetailList[i].item_mode == 2)
                //        continue;
                //    accumulated_bonus += orderDetailList[i].Accumulated_Bonus;
                //    accumulated_happygo += orderDetailList[i].Accumulated_Happygo;
                //    deduct_happygo += orderDetailList[i].Deduct_Happygo;
                //}

                //int user_bonus = GetUserBonus( ordermaster.Rows[0]["user_id"].ToString(), 1);
                //if (accumulated_bonus > user_bonus)
                //{
                //    return 1;//消費者購物金餘額不足，無法扣除給予購物金。
                //}
                if (!check_order_process(Convert.ToInt32(ordermaster.Rows[0]["order_status"]), 90) && Convert.ToInt32(ordermaster.Rows[0]["deduct_card_bonus"]) == 0)
                {
                    return 2;//error master
                }
                for (int i = 0; i < orderSlaveList.Count; i++)
                {
                    if (!check_order_process(Convert.ToInt32(orderSlaveList[i].Slave_Status), 90) && Convert.ToInt32(ordermaster.Rows[0]["deduct_card_bonus"]) == 0)
                    {
                        return 3;//error slave
                    }
                }
                sqlstr.Append(modify_order_master_status(om.Order_Id.ToString(), 90, om.Order_Ipfrom));
                mySqlCmd.CommandText = sqlstr.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                //'Writer : (' . $aUser_Data['user_id'] . ')' . $aUser_Data['user_username'] . "\r\n" . $sDescription;
                om.Note_Order = "Writer : " + om.User_Id + " " + om.user_name + "\r\n" + om.Note_Order;
                sqlstr.Clear();
                sqlstr.Append(order_master_status_record(int.Parse(om.Order_Id.ToString()), 90, om.Note_Order, om.Order_Ipfrom));
                mySqlCmd.CommandText = sqlstr.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                sqlstr.Clear();

                for (int i = 0; i < orderSlaveList.Count; i++)
                {
                    sqlstr.Append(modify_order_slave_status(orderSlaveList[i].Slave_Id, 90, om.Order_Ipfrom));
                    sqlstr.Append(order_slave_status_record(orderSlaveList[i].Slave_Id, 90, om.Order_Ipfrom, om.Note_Order));

                    if (Convert.ToInt32(ordermaster.Rows[0]["order_date_pay"]) > 0)//只要付款了就發郵件&& Convert.ToInt32(ordermaster.Rows[0]["order_amount"]) > 0
                    {
                        if (!SendMail.Keys.Contains(orderSlaveList[i].Slave_Id))
                        {
                            SendMail.Add(orderSlaveList[i].Slave_Id, " ");//發郵件給供應商
                        }
                    }

                }
                if (!string.IsNullOrEmpty(sqlstr.ToString()))
                {
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                if (orderDetailList.Count > 0)
                {
                    for (int i = 0; i < orderDetailList.Count; i++)
                    {
                        sqlstr.AppendFormat(" set sql_safe_updates=0;update order_detail SET detail_status ='{0}' WHERE detail_id ='{1}';set sql_safe_updates=1; ", 90, orderDetailList[i].Detail_Id);
                    }
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }

                // 回存庫存量
                for (int i = 0; i < orderDetailList.Count; i++)
                {
                    if (orderDetailList[i].item_mode == 1)
                        continue;
                    uint Buy_Num = orderDetailList[i].Buy_Num;
                    if (orderDetailList[i].item_mode == 2)
                    {
                        Buy_Num = orderDetailList[i].Buy_Num * orderDetailList[i].parent_num;
                    }
                    sqlstr.AppendFormat(" set sql_safe_updates=0;UPDATE product_item SET	item_stock = item_stock +'{0}' WHERE	item_id ='{1}';set sql_safe_updates=1;", Buy_Num, orderDetailList[i].Item_Id);

                }
                if (!string.IsNullOrEmpty(sqlstr.ToString()))
                {
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                //取回減免數量

                int[] nums = new int[] { 3, 4, 5, 99 };
                bool or_status = nums.Contains(Convert.ToInt32(ordermaster.Rows[0]["order_status"]));
                if (!or_status)
                {
                    sqlstr.AppendFormat(" set sql_safe_updates=0;UPDATE promotions_amount_reduce_member SET order_status = 0 where order_id ='{0}' ;set sql_safe_updates=1;", om.Order_Id);
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                int collect_date = string.IsNullOrEmpty(ordermaster.Rows[0]["money_collect_date"].ToString()) ? 0 : Convert.ToInt32(ordermaster.Rows[0]["money_collect_date"]);//付款日期有null的判斷
                if (collect_date > 0 && (Convert.ToInt32(ordermaster.Rows[0]["order_amount"]) > 0 || Convert.ToInt32(ordermaster.Rows[0]["deduct_card_bonus"]) > 0))
                {
                    int Money_Type = 0;
                    if (Convert.ToInt32(ordermaster.Rows[0]["order_amount"]) == 0)
                    {
                        ordermaster.Rows[0]["order_amount"] = ordermaster.Rows[0]["deduct_card_bonus"];
                    }
                    Money_Type = Convert.ToInt32(ordermaster.Rows[0]["order_payment"]);
                    sqlstr.Append(_serialDao.Update(46));
                    sqlstr.Append(" select serial_value from serial where serial_id=31;");
                    sqlstr.Append(" insert into order_money_return(money_id,order_id,money_type,money_total,money_status,money_note,money_source,money_createdate,money_updatedate,money_ipfrom)value( ");
                    sqlstr.AppendFormat("(select serial_value from serial where serial_id=31),'{0}','{1}', ", om.Order_Id, Money_Type);
                    sqlstr.AppendFormat("'{0}','{1}','{2}', ", Convert.ToInt32(ordermaster.Rows[0]["order_amount"]), 0, "");
                    sqlstr.AppendFormat("'{0}','{1}','{2}', ", "order master cancel", CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")), CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
                    sqlstr.AppendFormat("'{0}'); ", om.Order_Ipfrom);
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                    if (accumulated_happygo > 0)
                    {
                        sql.AppendFormat(" select status,order_id from hg_accumulate where order_id='{0}';", om.Order_Id);
                        DataTable _dtHG = _accessMySql.getDataTable(sql.ToString());
                        sql.Clear();
                        if (_dtHG.Rows.Count > 0 && Convert.ToInt32(_dtHG.Rows[0]["status"]) == 0)
                        {
                            sqlstr.AppendFormat(" set sql_safe_updates=0;update hg_accumulate set error_type='{0}',status='{1}' where order_id='{2}';set sql_safe_updates=1;", "", 2, om.Order_Id);
                        }
                        else
                        {
                            sql.AppendFormat(@"select * from hg_deduct where order_id={0} limit 0,1", om.Order_Id);
                            DataTable hg_deduct = _accessMySql.getDataTable(sql.ToString());
                            sql.Clear();
                            if (hg_deduct.Rows.Count > 0)
                            {
                                sqlstr.Append(Deduct_User_Happy_Go(accumulated_happygo, om.Order_Id.ToString(), hg_deduct));
                                mySqlCmd.CommandText = sqlstr.ToString();
                                result = mySqlCmd.ExecuteNonQuery();
                                sqlstr.Clear();
                            }
                            else
                            {
                                return 4;//取得身分證字號失敗
                            }
                        }
                    }
                }
                //回收購物金在外判斷
                if (accumulated_bonus > 0)
                {
                    sqlstr.Append(Deduct_User_Bonus(accumulated_bonus, om.Order_Id.ToString(), ordermaster.Rows[0]["user_id"].ToString()));
                    if (!string.IsNullOrEmpty(sqlstr.ToString()))
                    {
                        mySqlCmd.CommandText = sqlstr.ToString();
                        result = mySqlCmd.ExecuteNonQuery();
                        sqlstr.Clear();
                    }
                }
                //if (Convert.ToInt32(ordermaster.Rows[0]["order_payment"]) == 8 && Convert.ToInt32(ordermaster.Rows[0]["money_collect_date"]) == 0)
                //{
                //    if(accumulated_bonus > 0)
                //    {
                //        sqlstr.Append(Deduct_User_Bonus(accumulated_bonus, om.Order_Id.ToString(), ordermaster.Rows[0]["user_id"].ToString()));
                //        mySqlCmd.CommandText = sqlstr.ToString();
                //        result = mySqlCmd.ExecuteNonQuery();
                //        sqlstr.Clear();
                //    }
                //}
                if (Convert.ToInt32(ordermaster.Rows[0]["order_amount"]) > 0)
                {
                    sqlstr.AppendFormat(" set sql_safe_updates=0;UPDATE order_master SET money_cancel = '{0}' where order_id = '{1}';set sql_safe_updates=1;", Convert.ToInt32(ordermaster.Rows[0]["order_amount"]), om.Order_Id);
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                if (Convert.ToInt32(ordermaster.Rows[0]["priority"]) == 1)
                {
                    sqlstr.AppendFormat(" set sql_safe_updates=0;UPDATE order_master SET priority = 0 WHERE	order_id ='{0}';set sql_safe_updates=1;", om.Order_Id);
                    sqlstr.AppendFormat(" set sql_safe_updates=0;UPDATE users SET first_time = 0 WHERE	user_id ='{0}';set sql_safe_updates=1;", Convert.ToInt32(ordermaster.Rows[0]["user_id"]));
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                sql.AppendFormat(" select *from user_recommend  where order_id='{0}';", om.Order_Id);
                DataTable _dtrecom = _accessMySql.getDataTable(sql.ToString());
                sql.Clear();
                if (_dtrecom.Rows.Count > 0)
                {
                    sqlstr.AppendFormat(" set sql_safe_updates=0;UPDATE user_recommend SET is_recommend = 0 WHERE	id ='{0}';set sql_safe_updates=1;", Convert.ToInt32(_dtrecom.Rows[0]["id"]));
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                if (deduct_happygo > 0)
                {
                    sqlstr.Append(Deduct_Refund(om.Order_Id, 0, 0, deduct_happygo, om.Order_Ipfrom));
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                if (Convert.ToInt32(ordermaster.Rows[0]["deduct_bonus"]) > 0)
                {
                    sqlstr.Append(Deduct_Refund(om.Order_Id, Convert.ToInt32(ordermaster.Rows[0]["deduct_bonus"]), 0, 0, om.Order_Ipfrom));
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                if (Convert.ToInt32(ordermaster.Rows[0]["deduct_welfare"]) > 0)
                {
                    sqlstr.Append(Deduct_Refund(om.Order_Id, 0, Convert.ToInt32(ordermaster.Rows[0]["deduct_welfare"]), 0, om.Order_Ipfrom));
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                sqlstr.AppendFormat(" set sql_safe_updates=0;UPDATE deliver_master SET delivery_status = 6 WHERE	order_id = '{0}' AND type in (1 , 2);set sql_safe_updates=1;", om.Order_Id);
                if (Convert.ToInt32(ordermaster.Rows[0]["deduct_card_bonus"]) > 0)
                {
                    sqlstr.AppendFormat(" set sql_safe_updates=0;UPDATE order_slave set slave_status=90,slave_date_delivery = 0 where order_id = '{0}';set sql_safe_updates=1;", om.Order_Id);
                }
                mySqlCmd.CommandText = sqlstr.ToString();
                result = mySqlCmd.ExecuteNonQuery();
                sqlstr.Clear();
                sqlstr.Append(check_and_modify_flag(om.Order_Id, 3, om.Order_Ipfrom));
                if (!string.IsNullOrEmpty(sqlstr.ToString()))
                {
                    mySqlCmd.CommandText = sqlstr.ToString();
                    result = mySqlCmd.ExecuteNonQuery();
                    sqlstr.Clear();
                }
                 mySqlCmd.Transaction.Commit();
                // mySqlCmd.Transaction.Rollback();

                send_cancel_mail_for_vendor(SendMail);
                return 100;//完成
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderCancelMasterDao-->ReturnAllOrder-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }

        }
        #endregion

        public DataTable GetOrderMaster(uint order_id)
        {
            try
            {
                string sql = string.Format("select * from order_master where order_id={0}", order_id);
                return _accessMySql.getDataTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnlMasterDao-->GetOrderMaster-->" + ex.Message, ex);
            }

        }
        public int GetUserBonus(string user_id, int bonus_type)
        {
            int master_balance = 0;
            if (bonus_type == 0)
            {
                bonus_type = 1;
            }
            string sql = string.Format("select sum(master_balance) as master_balance from bonus_master where user_id='{0}' and master_start<='{1}' and master_end>='{2}' and master_balance>0 and bonus_type='{3}'", user_id, CommonFunction.GetPHPTime(DateTime.Now.ToString()), CommonFunction.GetPHPTime(DateTime.Now.ToString()), bonus_type);
            try
            {
                DataTable _dtUserBonus = _accessMySql.getDataTable(sql);
                if (_dtUserBonus.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(_dtUserBonus.Rows[0]["master_balance"].ToString()))
                    {
                        master_balance = Convert.ToInt32(_dtUserBonus.Rows[0]["master_balance"]);
                    }
                }

                return master_balance;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMaster-->GetUserBonus" + ex.Message + sql.ToString(), ex);
            }
        }

        public string Deduct_User_Bonus(int deduct_bonus, string order_id, string user_id)
        {
            BonusMasterQuery b = new BonusMasterQuery();
            BonusRecord br = new BonusRecord();
            Serial s = new Serial();
            _serial = new SerialDao(connString);
            _bonus = new BonusMasterDao(connString);
            List<BonusMaster> store = new List<BonusMaster>();
            List<BonusMaster> store2 = new List<BonusMaster>();
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            if (ips.Count() > 0)
            {
                b.master_ipfrom = ips[0].ToString();
            }
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();
            try
            {
                int bonus_num = deduct_bonus;
                //是否發放購物金 
                b.master_note = order_id;
                b.master_total = uint.Parse(deduct_bonus.ToString());
                b.bonus_type = 1;
                store = _bonus.GetBonus(b);
                if (store.Count == 1)
                {
                    bool user = false;
                    foreach (var item in store)
                    {
                        if (item.master_total == item.master_balance)
                        {//判斷發放的購物金是否使用 
                            user = true;
                            bonus_num = 0;
                        }
                        else
                        {
                            bonus_num = bonus_num - item.master_balance;
                        }
                        b.master_id = item.master_id;
                        b.masterid = item.master_id.ToString() + ",";
                    }
                    s = _serial.GetSerialById(28);
                    uint a = 1;
                    #region 發放購物金扣除
                    if (deduct_bonus - bonus_num > 0)
                    {


                        //先清除bonus_master表裏面的發放的購物金
                        b.master_balance = deduct_bonus - bonus_num;
                        b.master_writer = "訂單取消";
                        sqlstr.Append(_bonus.UpBonusMaster(b));
                        //并記錄到購物金記錄表中
                        sqlstr.Append(_serial.Update(28));
                        br.record_id = uint.Parse(s.Serial_Value.ToString()) + a;
                        a++;
                        br.master_id = b.master_id;
                        br.type_id = 32;
                        br.order_id = uint.Parse(b.master_note);
                        br.record_use = uint.Parse(b.master_balance.ToString());
                        br.record_note = order_id;
                        br.record_writer = "訂單整筆取消";
                        br.record_ipfrom = b.master_ipfrom;
                        sqlstr.Append(_bonus.InsertBonusRecord(br));
                    }
                    if (!user)
                    {//發放購物金被使用 
                        b.user_id = uint.Parse(user_id.ToString());
                        b.masterid = b.masterid.TrimEnd(',');
                        uint order = uint.Parse(b.master_note.ToString());
                        b.master_note = null;
                        b.master_total = 0;
                        b.usebonus = "K";
                        //該用戶剩餘可用購物金 
                        store2 = _bonus.GetBonus(b);

                        foreach (var item in store2)
                        {
                            if (bonus_num > item.master_balance)
                            {
                                b.master_balance = item.master_balance;
                                bonus_num = bonus_num - item.master_balance;
                            }
                            else
                            {
                                b.master_balance = bonus_num;
                                bonus_num = 0;
                            }
                            //變更bonus_master表裏面的發放的購物金
                            b.master_id = item.master_id;
                            sqlstr.Append(_bonus.UpBonusMaster(b));
                            //記錄到購物金記錄表中 
                            sqlstr.Append(_serial.Update(28));
                            br.record_id = uint.Parse(s.Serial_Value.ToString()) + a;
                            a++;
                            br.master_id = b.master_id;
                            br.type_id = 32;
                            br.order_id = order;
                            br.record_use = uint.Parse(b.master_balance.ToString());
                            br.record_note = order_id;
                            br.record_writer = "訂單取消被使用,額外扣除購物金";
                            br.record_ipfrom = b.master_ipfrom;
                            sqlstr.Append(_bonus.InsertBonusRecord(br));

                            if (bonus_num == 0)
                            {
                                break;
                            }
                        }
                    }
                    if (bonus_num > 0)
                    {//該用戶剩餘的購物金不夠扣剩下記錄到表中
                        sqlstr.AppendFormat(@"insert into users_deduct_bonus (deduct_bonus,user_id,createdate,order_id)");
                        sqlstr.AppendFormat(@" values('{0}','{1}','{2}','{3}');", bonus_num, user_id, CommonFunction.GetPHPTime(DateTime.Now.ToString()), order_id);
                    }
                    #endregion
                }
                return sqlstr.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnlMasterDao-->Deduct_User_Bonus-->" + ex.Message, ex);
            }
        }

        public string Deduct_User_Happy_Go(int accumulated_happygo, string order_id, DataTable hg_deduct)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (hg_deduct.Rows.Count > 0)
                {
                    DataRow dr = hg_deduct.Rows[0];
                    sql.AppendFormat(@"insert into hg_accumulate_refund (enc_idno,chk_sum,transaction_date,");
                    sql.AppendFormat(@"merchant,terminal,refund_point,category,wallet,note,order_id)");
                    sql.AppendFormat(@" values('{0}','{1}','{2}','{3}',", dr["enc_idno"].ToString(), dr["chk_sum"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "6601000081");//
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", "13999501", accumulated_happygo, "N0699999", "991991");
                    sql.AppendFormat(@"'{0}','{1}');", "吉甲地台灣好市集訂單編號" + order_id + "扣除點數:" + accumulated_happygo + "點", order_id);
                }
                return sql.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("OrderReturnMasterDao-->Deduct_User_Happy_Go-->" + ex.Message, ex);
            }

        }

        public string Deduct_Refund(uint order_id, int bonus, int bonus1, int hg, string ip)
        {
            try
            {
                //true 不回滚，false 回滚
                DataTable _dtOrderMaster = GetOrderMaster(order_id);
                StringBuilder sb = new StringBuilder();

                if (bonus > 0)
                {
                    int Expire_Day = 90;
                    //string sql = string.Format("select * from vip_user vu where 1=1  and user_id ={0} and status ={1}", _dtOrderMaster.Rows[0]["user_id"], 1);
                    //DataTable _dtVIPUser = _accessMySql.getDataTable(sql.ToString());
                    ////這下面的根本不會走
                    //if (_dtVIPUser.Rows.Count > 0)
                    //{
                    //    Expire_Day = Convert.ToInt32(_dtVIPUser.Rows[0]["bonus_expire_day"]);
                    //    if (_dtOrderMaster.Rows.Count > 0)
                    //    {
                    //        _dtOrderMaster.Rows[0]["accumulated_bonus"] = Convert.ToInt32(_dtOrderMaster.Rows[0]["accumulated_bonus"]) * Convert.ToInt32(_dtVIPUser.Rows[0]["bonus_rate"]);
                    //    }
                    //  
                    //} 
                    string Master_Start = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    string Master_End = DateTime.Now.AddDays(Expire_Day - 1).ToString("yyyy-MM-dd 23:59:59");
                    sb.Append(Bonus_Master_Add(Convert.ToInt32(_dtOrderMaster.Rows[0]["user_id"]), 4, bonus, Master_Start, Master_End, order_id.ToString(), "訂單取消退還使用購物金", 1, ip));
                }
                if (bonus1 > 0)
                {
                    string Master_Start = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");
                    string Master_End = DateTime.Now.AddDays(90).ToString("yyyy-MM-dd 23:59:59");
                    sb.Append(Bonus_Master_Add(Convert.ToInt32(_dtOrderMaster.Rows[0]["user_id"]), 4, bonus1, Master_Start, Master_End, order_id.ToString(), "訂單取消退還使用抵用券", 2, ip));
                }

                if (hg > 0)
                {
                    sb.Append(Hg_Deduct_Reverse(hg, order_id));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception(" OrderCancelMasterDao-->Deduct_Refund-->" + ex.Message, ex);
            }

        }

        public string Bonus_Master_Add(int user_id, int type_id, int master_total, string master_start, string master_end, string master_note, string master_writer, int bonus_type, string ip)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                master_end = DateTime.Parse(master_end).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");
                sql.AppendFormat(_serialDao.Update(27));
                sql.AppendFormat(@"insert into bonus_master (master_id,user_id,type_id,master_total,master_balance,");
                sql.AppendFormat(@"master_note,master_writer,master_start,master_end,master_createdate,master_updatedate,master_ipfrom,bonus_type)");
                sql.AppendFormat(@" values((select serial_value from serial where serial_id=27),'{0}','{1}','{2}','{3}',", user_id, type_id, master_total, master_total);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", master_note, master_writer, CommonFunction.GetPHPTime(master_start), CommonFunction.GetPHPTime(master_end), CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                sql.AppendFormat(@"'{0}','{1}','{2}');", CommonFunction.GetPHPTime(DateTime.Now.ToString()), ip, bonus_type);
                return sql.ToString();

            }
            catch (Exception ex)
            {

                throw new Exception(" OrderCancelMasterDao-->bonus_master_add-->" + ex.Message, ex);
            }

        }

        public string Hg_Deduct_Reverse(int deduct_happygo, uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            //var wc = new WebClient();
            //var html = wc.DownloadString("http://zhidao.baidu.com/question/499087825.html?seed=0");


            string Code = "5000";//這兩個參數是獲取的，
            string Message = "交易成功";
            try
            {
                sql.AppendFormat("select * from  hg_deduct where order_id={0} limit 0,1", order_id);
                DataTable _dtHg = _accessMySql.getDataTable(sql.ToString());
                sql.Clear();
                string date = DateTime.Now.Month + "" + DateTime.Now.Day;
                string time = DateTime.Now.Hour + "" + DateTime.Now.Minute + DateTime.Now.Second;


                XmlDocument xmldoc = new XmlDocument();
                string xmlPath = ConfigurationManager.AppSettings["REDEEM_XML"];//郵件服務器的設置
                string path = System.Web.HttpContext.Current.Server.MapPath(xmlPath);
                if (System.IO.File.Exists(path))
                {
                    xmldoc.Load(path);//加載XML
                    System.Xml.XmlNode xn = xmldoc.SelectSingleNode("RESPONSE");//找到XML中的節點
                    //System.Xml.XmlNodeList xnl = xn.ChildNodes;// 得到根节点的所有子节点 ;

                    foreach (System.Xml.XmlNode node in xmldoc.ChildNodes)
                    {
                        if (node.Name == "REVERSAL_FULL_REDEEM")
                        {
                            foreach (System.Xml.XmlNode item in node.ChildNodes)
                            {
                                if (item.Name == "RESPONSE")
                                {
                                    foreach (System.Xml.XmlNode nod in item.ChildNodes)
                                    {
                                        if (nod.Name == "CODE")
                                        {
                                            Code = nod.InnerText;
                                        }
                                        if (nod.Name == "MESSAGE")
                                        {
                                            Message = nod.InnerText;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }


                if (_dtHg.Rows.Count > 0)
                {
                    // 成功失敗都新增到這個表裡面 hg_deduct_reversal
                    //////sql.AppendFormat(@"insert into  hg_deduct_reverse (merchant_pos,terminal_pos,enc_idno,chk_sum,token,order_id,date,time,code,message,created,modified)");
                    //////sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}',", 6601000081, 13999501, _dtHg.Rows[0]["enc_idno"].ToString(), _dtHg.Rows[0]["chk_sum"].ToString(), _dtHg.Rows[0]["token"].ToString());
                    //////sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}');", _dtHg.Rows[0]["order_id"].ToString(), date, time, Code, Message, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    //////if (Code == "5000")
                    //////{
                    //////    if (deduct_happygo != 0 && order_id != 0)
                    //////    {
                    //////        sql.AppendFormat(@"insert into hg_deduct_refund (enc_idno,chk_sum,transaction_date,");
                    //////        sql.AppendFormat(@"merchant,terminal,refund_point,category,wallet,note,order_id)");
                    //////        sql.AppendFormat(@" values('{0}','{1}','{2}','{3}',", _dtHg.Rows[0]["enc_idno"].ToString(), _dtHg.Rows[0]["chk_sum"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "6601000081");
                    //////        sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", "13999501", deduct_happygo, "N0699999", "991991");
                    //////        sql.AppendFormat(@"'{0}','{1}');", "吉甲地台灣好市集訂單編號" + order_id + "歸還點數:" + deduct_happygo + "點", order_id);

                    //////    }
                    //////    else
                    //////    {
                    //////        //扣除HappyGo點數失敗;
                    //////    }
                    //////}
                    //////else 
                    //////{
                    //////    // insert hg_batch_deduct_refund
                    //////}

                    //如果失敗還往這張表裡面插入數據 insert hg_batch_deduct_refund

                }
                return sql.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception(" OrderCancelMasterDao-->hg_deduct_reverse-->" + ex.Message, ex);
            }

        }

        public DataTable Get_Combined_Product(uint order_id, string detail_id, string pack_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select od.* from order_detail od,order_slave os where os.order_id='{0}'", order_id);
                sql.AppendFormat(@" and	os.slave_id = od.slave_id and od.item_mode = 2 and od.pack_id ='{0}' and od.parent_id ='{1}' ", pack_id, detail_id);
                return _accessMySql.getDataTable(sql.ToString());

            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMasterDao-->Get_Combined_Product" + ex.Message + sql.ToString(), ex);
            }
        }
        public bool check_order_process(int Order_status, int status)
        {
            bool result = false;
            try
            {
                if (Order_status == 90)
                {
                    result = false;
                }
                if (Order_status == 91)
                {
                    result = false;
                }
                if (Order_status == 99)
                {
                    result = false;
                }
                if (Order_status == 0)
                {
                    if (status == 1 || status == 2 || status == 10 || status == 90)
                    {
                        result = true;
                    }
                }
                else if (Order_status == 1)
                {
                    if (status == 0 || status == 10 || status == 90)
                    {
                        result = true;
                    }
                }
                else if (Order_status == 2)
                {
                    if (status == 0 || status == 3 || status == 4 || status == 90 || status == 89)
                    {
                        result = true;
                    }
                }
                else if (Order_status == 3)
                {
                    if (status == 4 || status == 91 || status == 99 || status == 89)
                    {
                        result = true;
                    }
                }
                else if (Order_status == 4)
                {
                    if (status == 2 || status == 91 || status == 92 || status == 99)
                    {
                        result = true;
                    }
                }
                else if (Order_status == 5)
                {
                    if (status == 91 || status == 99 || status == 89)
                    {
                        result = true;
                    }
                }
                else if (Order_status == 10)
                {
                    if (status == 0 || status == 90)
                    {
                        result = true;
                    }
                }
                else if (Order_status == 20)
                {
                    if (status == 0 || status == 2 || status == 90)
                    {
                        result = true;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMasterDao-->check_order_process" + ex.Message, ex);
            }
        }

        public string modify_order_master_status(string order_id, int order_status, string ip)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                OrderMaster om = new OrderMaster();
                om.Order_Status = uint.Parse(order_status.ToString());
                om.Order_Updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).ToString());
                om.Order_Ipfrom = ip;
                om.Order_Date_Cancel = 0;
                om.Order_Date_Close = 0;
                if (order_status == 90)
                {
                    om.Order_Date_Cancel = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).ToString());
                }
                else if (order_status == 99)
                {
                    om.Order_Date_Close = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).ToString());
                }
                sb.AppendFormat("set sql_safe_updates=0;update order_master set order_status='{0}',order_updatedate='{1}',order_ipfrom='{2}' ", om.Order_Status, om.Order_Updatedate, om.Order_Ipfrom);
                if (om.Order_Date_Cancel != 0)
                {
                    sb.AppendFormat(" ,order_date_cancel='{0}' ", om.Order_Date_Cancel);
                }
                if (om.Order_Date_Close != 0)
                {
                    sb.AppendFormat(" ,order_date_close='{0}' ", om.Order_Date_Close);
                }
                sb.AppendFormat(" where order_id='{0}';set sql_safe_updates=1; ", order_id);
                return sb.ToString();

            }
            catch (Exception ex)
            {

                throw new Exception("OrderCancelMasterDao-->check_order_process" + ex.Message + sb.ToString(), ex);
            }
        }

        public string order_master_status_record(int order_id, int Order_Status, string order_note, string ip)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(_serialDao.Update(29));
                sb.Append("select serial_value from serial where serial_id=29;insert into order_master_status(serial_id,order_id,order_status,status_description,status_ipfrom,status_createdate)value(");
                sb.AppendFormat(" (select serial_value from serial where serial_id=29),'{0}','{1}','{2}','{3}','{4}'); ", order_id, Order_Status, order_note, ip, CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
                return sb.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("OrderCancelMasterDao-->order_master_status_record" + ex.Message + sb.ToString(), ex);
            }

        }

        public string modify_order_slave_status(uint stave_id, int order_status, string ip, long Deliver_Time = 0)
        {
            StringBuilder sb = new StringBuilder();
            long slave_date_delivery = 0;
            long slave_date_cancel = 0;
            long slave_date_return = 0;
            long slave_date_close = 0;
            long times = CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            try
            {
                if (Deliver_Time == 0)
                {
                    Deliver_Time = times;
                }
                if (order_status == 4)
                {
                    slave_date_delivery = Deliver_Time;
                }
                if (order_status == 90)
                {
                    slave_date_cancel = times;
                    sb.AppendFormat(" set sql_safe_updates=0;update order_detail SET detail_status='{0}'where slave_id='{1}';set sql_safe_updates=1;", order_status, stave_id);
                }
                if (order_status == 2)
                {
                    sb.AppendFormat(" set sql_safe_updates=0;update order_detail SET detail_status='{0}'where slave_id='{1}';set sql_safe_updates=1;", order_status, stave_id);
                }

                if (order_status == 6)
                {
                    sb.AppendFormat(" set sql_safe_updates=0;update order_detail SET detail_status='{0}'where slave_id='{1}' AND detail_status = '{2}';set sql_safe_updates=1;", order_status, stave_id, 2);
                    slave_date_delivery = times;
                }
                if (order_status == 7)
                {
                    sb.AppendFormat(" set sql_safe_updates=0;update order_detail SET detail_status='{0}'where slave_id='{1}' AND detail_status = '{2}';set sql_safe_updates=1;", order_status, stave_id, 6);
                }
                if (order_status == 91)
                {
                    slave_date_return = times;
                }
                if (order_status == 99)
                {
                    slave_date_close = times;
                }
                sb.AppendFormat(" set sql_safe_updates=0;update order_slave set slave_status='{0}',slave_updatedate='{1}',slave_ipfrom='{2}' ", order_status, times, ip);
                if (slave_date_delivery != 0)
                {
                    sb.AppendFormat(" ,slave_date_delivery ='{0}' ", slave_date_delivery);
                }
                if (slave_date_cancel != 0)
                {
                    sb.AppendFormat(" ,slave_date_cancel ='{0}' ", slave_date_cancel);
                }
                if (slave_date_return != 0)
                {
                    sb.AppendFormat(" ,slave_date_return ='{0}' ", slave_date_cancel);
                }
                if (slave_date_close != 0)
                {
                    sb.AppendFormat(" ,slave_date_close ='{0}' ", slave_date_cancel);
                }
                sb.AppendFormat(" where slave_id = '{0}' ;set sql_safe_updates=1;", stave_id);

                return sb.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("OrderCancelMasterDao-->modify_order_slave_status" + ex.Message + sb.ToString(), ex);
            }
        }

        public string order_slave_status_record(uint slave_id, int order_status, string ip, string order_note)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(_serialDao.Update(31));
                sb.Append("select serial_value from serial where serial_id=31;insert into order_slave_status(serial_id,slave_id,order_status,status_description,status_ipfrom,status_createdate)value(");
                sb.AppendFormat(" (select serial_value from serial where serial_id=31),'{0}','{1}','{2}','{3}','{4}'); ", slave_id, order_status, order_note, ip, CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
                return sb.ToString();

            }
            catch (Exception ex)
            {

                throw new Exception("OrderCancelMasterDao-->order_slave_status_record" + ex.Message + sb.ToString(), ex);
            }
        }

        public string check_and_modify_flag(uint order_id, int Flag, string ip)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                DataTable ordermaster = GetOrderMaster(order_id);
                if (Convert.ToInt32(ordermaster.Rows[0]["export_flag"]) > 10000)
                {
                    sb.Append("select  parameterCode,parameterName from   t_parametersrc where parameterType='cosmos_status';");
                    DataTable _dtCoustomer = _accessMySql.getDataTable(sb.ToString());
                    sb.Clear();
                    sb.AppendFormat(" set sql_safe_updates=0;UPDATE order_master set export_flag='{0}' where order_id='{1}';set sql_safe_updates=1;", Flag, order_id);
                    int export_flag = Convert.ToInt32(ordermaster.Rows[0]["export_flag"]);
                    string status_str_old = "";
                    string status_str_new = "";
                    foreach (DataRow item in _dtCoustomer.Rows)
                    {
                        if (Convert.ToInt32(item["parameterCode"]) == export_flag)
                        {
                            status_str_old = item["parameterName"].ToString();
                        }
                        if (Convert.ToInt32(item["parameterCode"]) == Flag)
                        {
                            status_str_new = item["parameterName"].ToString();
                        }
                    }
                    if (string.IsNullOrEmpty(status_str_old))
                    {
                        status_str_old = "已拋轉";
                    }
                    if (string.IsNullOrEmpty(status_str_new))
                    {
                        status_str_new = "已拋轉";
                    }
                    sb.Append(order_master_status_record(Convert.ToInt32(order_id), 90, "ERP拋轉狀態:" + status_str_old + "->" + status_str_new, ip));//90->Convert.ToInt32(ordermaster.Rows[0]["order_status"]),邏輯的更改
                }
                return sb.ToString();

            }
            catch (Exception ex)
            {

                throw new Exception("OrderCancelMasterDao-->check_and_modify_flag" + ex.Message + sb.ToString(), ex);
            }
        }
        public bool send_cancel_mail_for_vendor(Dictionary<uint, string> SendMail)
        {
            MailHelper mail = new MailHelper();
            bool result = false;
            try
            {
                DataTable _dtVendor = new DataTable();
                StringBuilder sb = new StringBuilder();
                sb.Append(" select parameterCode from t_parametersrc where parameterType ='develop';");
                DataTable _dtDEV = _accessMySql.getDataTable(sb.ToString());
                sb.Clear();
                DataTable _dtTest = new DataTable();
                sb.Append(" select remark from t_parametersrc where parameterType ='TestMail';");
                _dtTest = _accessMySql.getDataTable(sb.ToString());
                sb.Clear();
                string TestMail = "";
                if (_dtTest.Rows.Count > 0)
                {
                    TestMail = _dtTest.Rows[0]["remark"].ToString();
                }
                string DEV = "false";
                if (_dtDEV.Rows.Count > 0)
                {
                    DEV = _dtDEV.Rows[0]["parameterCode"].ToString();
                }
                foreach (uint item in SendMail.Keys)
                {

                    sb.AppendFormat(" select os.order_id,v.vendor_email,v.vendor_name_full from order_slave os left join vendor v on v.vendor_id=os.vendor_id  where slave_id='{0}';", item);
                    _dtVendor = _accessMySql.getDataTable(sb.ToString());
                    sb.Clear();
                    if (_dtVendor.Rows.Count > 0)
                    {
                        string MailTitle = "取消出貨通知";
                        string MailBody = _dtVendor.Rows[0]["vendor_name_full"].ToString() + "公司您好,<br/>吉甲地市集平台訂單, 付款單號 :" + _dtVendor.Rows[0]["order_id"].ToString();
                        MailBody += "客戶因故取消部分商品, 請協助再次確認出貨品項無誤,以免衍生後續不便,<br/>感謝您。吉甲地在地好物市集敬上 ";
                        if (DEV == "true")
                        {
                            result = mail.SendMailAction(TestMail, MailTitle, MailBody);
                        }
                        else
                        {
                            result = mail.SendMailAction(_dtVendor.Rows[0]["vendor_email"].ToString(), MailTitle, MailBody);
                        }
                    }

                }
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMasterDao-->send_cancel_mail_for_vendor" + ex.Message, ex);
            }

        }

        /***判斷用戶的購物金是否夠扣除*/
        public int returnMsg(OrderMaster om)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                _bonus = new BonusMasterDao(connString);
                BonusMasterQuery query = new BonusMasterQuery();
                query.master_note = om.Order_Id.ToString();
                query.bonus_type = 1;
                List<BonusMasterQuery> bonusMasterStore = new List<BonusMasterQuery>();
                bonusMasterStore = _bonus.IsExtendBonus(query);

                if (bonusMasterStore.Count > 0)
                {
                    ///用戶購物金
                    sb.AppendFormat("SELECT sum(od.accumulated_bonus) as accumulated_bonus FROM order_detail od,order_slave os WHERE	os.order_id = '{0}'AND	os.slave_id = od.slave_id and od.item_mode<>2; ", om.Order_Id);
                    DataTable _dtbonus = _accessMySql.getDataTable(sb.ToString());
                    sb.Clear();
                    //訂單購物金
                    sb.AppendFormat("select user_id from order_master where order_id='{0}';", om.Order_Id);
                    DataTable _dtUser = _accessMySql.getDataTable(sb.ToString());
                    int deductuser = 0;
                    if (!string.IsNullOrEmpty(_dtUser.Rows[0]["user_id"].ToString()))
                    {
                        deductuser = GetUserBonus(_dtUser.Rows[0]["user_id"].ToString(), 1);
                    }
                    else
                    {
                        return 1;//訂單錯誤！
                    }
                    int orderbonus = string.IsNullOrEmpty(_dtbonus.Rows[0]["accumulated_bonus"].ToString()) ? 0 : Convert.ToInt32(_dtbonus.Rows[0]["accumulated_bonus"]);
                    if (orderbonus > deductuser)
                    {
                        return 99;//消費者購物金餘額不足，無法扣除給予購物金
                    }
                    else
                    {
                        return 100;//不顯示那個信息
                    }
                }
                else
                {
                    return 100;//不顯示那個信息
                }

            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMasterDao-->returnMsg" + ex.Message, ex);
            }
        }
    }
}
