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
namespace BLL.gigade.Dao
{
    public class OrderCancelMasterDao : IOrderCancelMasterImplDao
    {
        private IDBAccess _accessMySql;
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
                sqlstr.AppendFormat(@"update order_cancel_master set cancel_status='{0}',cancel_note='{1}',bank_note='{2}',", ocm.cancel_status, ocm.cancel_note, ocm.bank_note);
                sqlstr.AppendFormat(@"cancel_createdate='{0}',cancel_updatedate='{1}',cancel_ipfrom='{2}'", CommonFunction.GetPHPTime(ocm.cancel_createdate.ToString("yyyy-MM-dd 0:0:0")), CommonFunction.GetPHPTime(ocm.cancel_updatedate.ToString("yyyy-MM-dd 0:0:0")), ocm.cancel_ipfrom);
                sqlstr.AppendFormat(@" where cancel_id='{0}';", ocm.cancel_id);
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
                        sqlstr.AppendFormat(@"update order_master set	money_cancel = money_cancel +{0} where order_id ='{1}';", cancel_money, ocm.order_id);
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

                                    sqlstr.AppendFormat(@"update product_item set item_stock = item_stock +{0} where item_id ='{1}';", Convert.ToInt32(drp["buy_num"]) * Convert.ToInt32(drp["parent_num"]), drp["item_id"].ToString());
                                }
                            }
                        }
                        else
                        {
                            //modify_item_stock((int)dr["item_id"], (int)dr["buy_num"]);

                            sqlstr.AppendFormat(@"update product_item set item_stock = item_stock +{0} where item_id ='{1}';", dr["buy_num"], dr["item_id"]);
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
                        sqlstr.AppendFormat("update order_master set priority = 0 where order_id='{0}';", ocm.order_id);
                        sqlstr.AppendFormat(" update users set first_time = 0 where user_id ='{0}';", user_id);

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
                        sqlstr.AppendFormat(" update user_recommend set is_recommend = 0 where id ='{0}';", _dtodRow.Rows[0]["id"]);

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
                if (_accessMySql.getDataTable(sql).Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(_accessMySql.getDataTable(sql).Rows[0]["master_balance"].ToString()))
                    {
                        master_balance = Convert.ToInt32(_accessMySql.getDataTable(sql).Rows[0]["master_balance"]);
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
            int user_bonus = GetUserBonus(user_id, 1);
            int bonus_num = deduct_bonus;
            string return_ipfrom = string.Empty;
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            if (ips.Count() > 0)
            {
                return_ipfrom = ips[0].ToString();
            }
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();

            try
            {

                if (user_bonus >= deduct_bonus)
                {

                    if (bonus_num < 0)
                    {
                        bonus_num = bonus_num * -1;
                    }
                    sql.AppendFormat(@"select * from bonus_master where user_id='{0}'", user_id);
                    sql.AppendFormat(@" and master_start<='{0}' and master_end>='{1}'", CommonFunction.GetPHPTime(DateTime.Now.ToString()), CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    sql.AppendFormat(@" and master_balance > 0 and bonus_type = 1 order by master_end asc;");

                    DataTable bonus_master = _accessMySql.getDataTable(sql.ToString());
                    sql.Clear();
                    int temp_num = 0;
                    if (bonus_master.Rows.Count > 0)
                    {
                        foreach (DataRow dr in bonus_master.Rows)
                        {
                            if (bonus_num > 0)
                            {
                                temp_num = bonus_num > Convert.ToInt32(dr["master_balance"]) ? Convert.ToInt32(dr["master_balance"]) : bonus_num;
                                sqlstr.Append(_serialDao.Update(28));
                                sqlstr.AppendFormat(@" insert into bonus_record (record_id,master_id,type_id,order_id,record_use,record_note,record_writer,record_createdate,record_updatedate,record_ipfrom)");
                                sqlstr.AppendFormat("  values((select serial_value from serial where serial_id=28),'{0}','{1}','{2}',", dr["master_id"], 32, order_id);
                                sqlstr.AppendFormat(@" '{0}','{1}','{2}',", temp_num, "訂單取消", "");
                                sqlstr.AppendFormat(@"'{0}','{1}','{2}');", CommonFunction.GetPHPTime(DateTime.Now.ToString()), CommonFunction.GetPHPTime(DateTime.Now.ToString()), return_ipfrom);
                                sqlstr.AppendFormat(@"update bonus_master set master_balance=master_balance-{0},master_updatedate='{1}'", temp_num, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                                sqlstr.AppendFormat(@" where master_id='{0}';", dr["master_id"].ToString());
                                bonus_num -= temp_num;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    sqlstr.AppendFormat(@"insert into users_deduct_bonus (deduct_bonus,user_id,createdate,order_id,status)");
                    sqlstr.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}');", deduct_bonus, user_id, CommonFunction.GetPHPTime(DateTime.Now.ToString()), order_id, 1);

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
                    sb.Append(Bonus_Master_Add(Convert.ToInt32(_dtOrderMaster.Rows[0]["user_id"]), 4, bonus1, Master_Start, Master_End, order_id.ToString(), "system", 2, ip));
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
                    sql.AppendFormat(@"insert into  hg_deduct_reverse (merchant_pos,terminal_pos,enc_idno,chk_sum,token,order_id,date,time,code,message,created,modified)");
                    sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}',", 6601000081, 13999501, _dtHg.Rows[0]["enc_idno"].ToString(), _dtHg.Rows[0]["chk_sum"].ToString(), _dtHg.Rows[0]["token"].ToString());
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}');", _dtHg.Rows[0]["order_id"].ToString(), date, time, Code, Message, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    if (Code == "5000")
                    {
                        if (deduct_happygo != 0 && order_id != 0)
                        {
                            sql.AppendFormat(@"insert into hg_deduct_refund (enc_idno,chk_sum,transaction_date,");
                            sql.AppendFormat(@"merchant,terminal,refund_point,category,wallet,note,order_id)");
                            sql.AppendFormat(@" values('{0}','{1}','{2}','{3}',", _dtHg.Rows[0]["enc_idno"].ToString(), _dtHg.Rows[0]["chk_sum"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "6601000081");
                            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", "13999501", deduct_happygo, "N0699999", "991991");
                            sql.AppendFormat(@"'{0}','{1}');", "吉甲地台灣好市集訂單編號" + order_id + "歸還點數:" + deduct_happygo + "點", order_id);

                        }
                        else
                        {
                            //扣除HappyGo點數失敗;
                        }
                    }

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
    }
}
