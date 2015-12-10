using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace BLL.gigade.Dao
{
    public class OrderReturnMasterDao
    {
        private IDBAccess _accessMySql;
        private string connString;
        private IOrderMasterImplDao _ordermasterdao;
        private IOrderDetailImplDao _orderDetailDao;
        private IOrderMoneyReturnImplDao _orderMoneyReturnDao;
        private IVipUserGroupImplDao _vipuserGroupDao;
        private IProductItemImplDao _itemDao;
        private IUsersImplDao _usersDao;
        private IUserRecommendIDao _userRecommendDao;
        string ipfrom = "";
        public OrderReturnMasterDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
            _ordermasterdao = new OrderMasterDao(connectionString);
            _orderMoneyReturnDao = new OrderMoneyReturnDao(connectionString);
            _vipuserGroupDao = new VipUserGroupDao(connectionString);
            _itemDao = new ProductItemDao(connectionString);
            _usersDao = new UsersDao(connectionString);
            _userRecommendDao = new UserRecommendDao(connectionString);
            _orderDetailDao = new OrderDetailDao(connectionString);

        }
        public List<OrderReturnMasterQuery> GetReturnMaster(OrderReturnMasterQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sqlCount.Append("select Count(orm.return_id) as 'totalCount' ");
                sql.Append(@"select  orm.return_id,orm.order_id,orm.vendor_id,v.vendor_name_simple vendor_name,orm.return_status,orm.return_note,orc.orc_remark,orc.orc_service_remark,");
                sql.Append(@" orm.bank_note,orm.invoice_deal,orm.package,orm.return_zip,orm.return_address,orm.deliver_code,");
                sql.Append(@"orm.return_createdate,FROM_UNIXTIME(orm.return_createdate) as createdate,orm.return_ipfrom,");
                sql.Append(@" orm.return_updatedate,CASE  orm.return_updatedate when 0 then null ELSE FROM_UNIXTIME(orm.return_updatedate)  end  as updatedate");
                sqlFrom.Append(@" from order_return_master orm left join vendor  v on  orm.vendor_id=v.vendor_id ");
                sqlFrom.Append(" LEFT JOIN order_return_content orc on orm.return_id=orc.return_id ");
           
                totalCount = 0;
                sqlWhere.Append(" where 1=1   ");
                if (query.ven_type == 1)
                {
                    sqlWhere.AppendFormat(" and orm.order_id like  '%{0}%' ", query.content);
                }
                else  if (query.ven_type == 2)
                {
                    sqlWhere.AppendFormat(" and orm.return_id like  '%{0}%' ", query.content);
                }
                if (query.date_type == 1)
                {
                  
                    sqlWhere.AppendFormat("  and  orm.return_createdate >='{0}' and orm.return_createdate<='{1}'  ", CommonFunction.GetPHPTime(query.time_start), CommonFunction.GetPHPTime(query.time_end));
                }
                else  if (query.date_type == 2)
                {
                    sqlWhere.Append(" left join order_return_detail  ord on ord.return_id = orm.return_id   ");
                    sqlWhere.Append("  left join deliver_detail dd  on dd.detail_id = ord.detail_id ");
                    sqlWhere.Append("   left join  deliver_master dm on dm.deliver_id = dd.deliver_id ");
                    sqlWhere.AppendFormat(" and  dm.delivery_date>='{0}' and dm.delivery_date<='{1}'  ",query.time_start,query.time_end);
                }
                if (query.return_status != 3)
                {
                    sqlWhere.AppendFormat(" and   orm.return_status='{0}'   ",query.return_status);
                }
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _accessMySql.getDataTable(sqlCount.ToString() + sqlFrom.ToString()+sqlWhere.ToString());
                    if (int.Parse(_dt.Rows[0][0].ToString()) > 0)
                    {
                        totalCount = int.Parse(_dt.Rows[0][0].ToString());
                    }
                    sqlWhere.Append(@" order by orm.return_createdate desc ");
                    sqlWhere.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                sql.Append(sqlFrom.ToString()+sqlWhere.ToString());
                return _accessMySql.getDataTableForObj<OrderReturnMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.GetReturnMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public OrderReturnUserQuery GetReturnDetailById(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select oru.bank_name,oru.bank_branch,oru.bank_account,oru.account_name from order_return_master orm left join order_return_user oru on orm.return_id=ord.return_id ");
                sql.AppendFormat(" left join order_return_detail  ord on oru.detail_id=ord.detail_id  ");
                sql.AppendFormat(" where orm.order_id='{0}'; ", order_id);
                return _accessMySql.getSinggleObj<OrderReturnUserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.GetReturnDetailById-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateSql(OrderReturnMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();

                sql.Append(" set sql_safe_updates =0;");
                sql.AppendFormat(@"update order_return_master set return_status='{0}',return_note='{1}',invoice_deal='{2}',package='{3}',", query.return_status, query.return_note, query.invoice_deal, query.package);
                sql.AppendFormat(@"return_zip='{0}',bank_note='{1}',return_address='{2}',return_updatedate='{3}',", query.return_zip, query.bank_note, query.return_address, query.return_updatedate);
                sql.AppendFormat(@"deliver_code='{0}',return_ipfrom='{1}'", query.deliver_code, query.return_ipfrom);
                sql.AppendFormat(@" where return_id={0};", query.return_id);
                sql.Append(" set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.UpdateSql-->" + ex.Message + sql.ToString(), ex);
            }
        }
        //退款單
        public string InsertOrderMoneyReturnSql(OrderReturnMasterQuery ormQuery)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" set sql_safe_updates = 0;");
                sql.AppendFormat(@"update order_return_master set return_status={0},return_note={1},invoice_deal={2},package={3},", ormQuery.return_status, ormQuery.return_note, ormQuery.invoice_deal, ormQuery.package);
                sql.AppendFormat(@"return_zip={0},bank_note={1},return_address={2},return_updatedate={3},", ormQuery.return_zip, ormQuery.bank_note, ormQuery.return_address, ormQuery.return_updatedate);
                sql.AppendFormat(@"deliver_code={0},return_ipfrom={1} ", ormQuery.deliver_code, ormQuery.return_ipfrom);
                sql.AppendFormat(@" where return_id={0}", ormQuery.return_id);
                sql.Append(" set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.GetReturnMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 退貨單歸檔
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int Save(OrderReturnMasterQuery query)
        {
            int id = 0;
            SerialDao serialDao = new SerialDao(connString);
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            StringBuilder sql = new StringBuilder();
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                //修改退貨單
                mySqlCmd.CommandText = UpdateSql(query);
                sql.Append(mySqlCmd.CommandText);
                mySqlCmd.ExecuteNonQuery();







                //若歸檔 判斷是否成立退款單
                #region 歸檔
                if (query.return_status == 1)
                {
                    //查詢訂單內容
                    OrderMaster om = _ordermasterdao.GetOrderMasterByOrderId4Change(Convert.ToInt32(query.order_id));
                    // 查退貨單內容詳情
                    DataTable odli = _orderDetailDao.OrderDetailTable(query.return_id,0);
                    if (odli != null)
                    {
                        int paymoney = 0;
                        int returnmoney = 0;
                        int deductbonus = 0;
                        int deductcash = 0;
                        int accumulated_bonus = 0;
                        int accumulated_happygo = 0;
                        int deduct_happygo = 0;
                        int deduct_happygo_money = 0;
                        foreach (DataRow od in odli.Rows)
                        {

                            // 退款金額 = 商品購買金額 - 購物金
                            paymoney += (Convert.ToInt32(od["single_money"]) * Convert.ToInt32(od["buy_num"]));
                            deductbonus += Convert.ToInt32(od["deduct_bonus"]);
                            deductcash += Convert.ToInt32(od["deduct_welfare"]);
                            accumulated_bonus += Convert.ToInt32(od["accumulated_bonus"]);
                            accumulated_happygo += Convert.ToInt32(od["accumulated_happygo"]);
                            deduct_happygo += Convert.ToInt32(od["deduct_happygo"]);
                            deduct_happygo_money += Convert.ToInt32(od["deduct_happygo_money"]);
                        }
                        returnmoney += paymoney - (deductbonus + deductcash + deduct_happygo_money);//計算應退還的money
                        // 判斷是否有付款
                        if (om.Money_Collect_Date > 0 && om.Order_Amount > 0 && returnmoney > 0)
                        {
                            //退款方式
                            uint moneytype = 0;
                            //if (om.Order_Payment == 1 || om.Order_Payment == 2 || om.Order_Payment == 13 || om.Order_Payment == 16)
                            //{
                            //    moneytype = om.Order_Payment;
                            //}
                            if (om.Order_Payment == 1 || om.Order_Payment == 22)
                            {
                                moneytype = 1;
                            }
                            else if (om.Order_Payment == 2)
                            {
                                moneytype = 2;
                            }
                            else if (om.Order_Payment == 13 || om.Order_Payment == 21)
                            {
                                moneytype = 13;
                            }
                            else if (om.Order_Payment == 16)
                            {
                                moneytype = 16;
                            }
                            #region 新增退款單
                            mySqlCmd.CommandText = serialDao.Update(46);//46 退款單流水號
                            sql.Append(mySqlCmd.CommandText);
                            uint moneyId = Convert.ToUInt32(mySqlCmd.ExecuteScalar());
                            OrderMoneyReturn omr = new OrderMoneyReturn();
                            omr.money_id = moneyId;
                            omr.money_type = moneytype;
                            omr.money_total = Convert.ToUInt32(returnmoney);
                            omr.money_status = 0;
                            omr.money_source = "return_id:" + query.return_id;
                            mySqlCmd.CommandText = _orderMoneyReturnDao.InsertSql(query, omr);

                            sql.Append(mySqlCmd.CommandText);
                            mySqlCmd.ExecuteNonQuery();
                            #endregion
                            //取回給予的購物金與happygo點數

                            //扣除給予會員的購物金
                            if (accumulated_bonus > 0)
                            {
                                Deduct_User_Bonus(accumulated_bonus, om);
                            }
                            //扣除給予會員的hg點數
                            if (accumulated_happygo > 0)
                            {
                                Deduct_User_Happy_Go(accumulated_happygo, om.Order_Id);
                            }
                        }
                        //黑貓例外處理
                        if (om.Money_Collect_Date == 0 && om.Order_Payment == 8)
                        {
                            //扣除給予會員的購物金
                            if (accumulated_bonus > 0)
                            {
                                Deduct_User_Bonus(accumulated_bonus, om);
                            }
                            //扣除給予會員的hg點數
                            if (accumulated_happygo > 0)
                            {
                                Deduct_User_Happy_Go(accumulated_happygo, om.Order_Id);
                            }
                        }
                        #region  寫入付款單退款金額
                        if (returnmoney > 0)
                        {
                            mySqlCmd.CommandText = _ordermasterdao.UpdateMoneyReturn(returnmoney, om.Order_Id);
                            sql.Append(mySqlCmd.CommandText);
                            mySqlCmd.ExecuteNonQuery();
                            sql.Clear();
                        }
                        #endregion

                        #region 判斷退回購買扣抵的hp點數  判斷是否要退回購物金
                        if (deduct_happygo > 0)
                        {
                            Deduct_Refund(om, 0, 0, deduct_happygo, query);
                        }
                        if (accumulated_happygo > 0)
                        {
                            Deduct_Refund(om, deductbonus, 0, 0, query);
                        }
                        #endregion

                        //商品數量補回
                        //foreach (var od in odli)
                        //{
                        //    if (od.item_mode == 1)//组合商品
                        //    {
                        //        List<OrderDetail> childDetail = Get_Combined_Product(query.order_id, od.Parent_Id, od.pack_id);
                        //        foreach (var child in childDetail)
                        //        {
                        //            _itemDao.UpdateItemStock(child.Item_Id, child.Buy_Num * child.parent_num);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        _itemDao.UpdateItemStock(od.Item_Id, od.Buy_Num);
                        //    }
                        //}

                        //
                        mySqlCmd.CommandText = GetTotalCount(query.order_id);
                        sql.Append(mySqlCmd.CommandText);
                        int total = mySqlCmd.ExecuteNonQuery();
                        //比較訂單全部detail的筆數與（退貨單+取消單）的筆數
                        if (total == 0)
                        {
                            // 首購會員優惠，因取消訂單時要自動回覆功能
                            if (om.Priority == 1)
                            {
                                mySqlCmd.CommandText = _ordermasterdao.UpdatePriority(om.Order_Id);
                                sql.Append(mySqlCmd.CommandText);
                                mySqlCmd.ExecuteNonQuery();

                                mySqlCmd.CommandText = _usersDao.UpdateFirstTime(om.User_Id);
                                sql.Append(mySqlCmd.CommandText);
                                mySqlCmd.ExecuteNonQuery();
                            }
                            //2.0活動筆數
                            List<UserRecommend> usRecommandLi = _userRecommendDao.QueryByOrderId(om.Order_Id);
                            string idStr = string.Empty;
                            if (usRecommandLi != null)
                            {
                                foreach (var record in usRecommandLi)
                                {
                                    if (string.IsNullOrEmpty(idStr))
                                    {
                                        idStr += record;
                                    }
                                    else
                                    {
                                        idStr += "," + record.id;
                                    }
                                }
                                mySqlCmd.CommandText = _userRecommendDao.UpdateIsCommend(idStr);
                            }
                            sql.Append(mySqlCmd.CommandText);
                            mySqlCmd.ExecuteNonQuery();

                        }
                    }
                }
                #endregion








                #region 取消退款
                if (query.return_status == 2)
                {
                    OrderMasterStatusQuery statusquery = new OrderMasterStatusQuery();
                    OrderDetailQuery detailquery = new OrderDetailQuery();
                    string description = "Writer:(" + query.user_id + ")" + query.user_username + ",return_id:" + query.return_id + ",取消退貨請協助通知營管貨品確實出貨";
                    string sqlSerial = serialDao.Update(29);//訂單主檔狀態流水號
                    DataTable _dt = _accessMySql.getDataTable(sqlSerial);
                    statusquery.serial_id = Convert.ToUInt64(_dt.Rows[0][0]);
                    statusquery.order_id = query.order_id;
                    statusquery.order_status = query.return_status;
                    statusquery.status_description = description;
                    statusquery.status_ipfrom = query.return_ipfrom;
                    mySqlCmd.CommandText += InsertOrderMasterS(statusquery);
                    List<OrderDetailQuery> odli = _orderDetailDao.OrderDetail(query.return_id);

                    foreach (var item in odli)
                    {
                        detailquery.Slave_Id = item.Slave_Id;
                        detailquery.Parent_Id = item.Parent_Id;
                        detailquery.pack_id = item.pack_id;
                        detailquery.Detail_Id = item.Detail_Id;
                        if (item.item_mode == 1)
                        {
                            mySqlCmd.CommandText = _orderDetailDao.UpdateOrderDetailSome(detailquery);
                        }
                        else
                        {
                            mySqlCmd.CommandText = _orderDetailDao.UpdateOrderDetail(detailquery);
                        }
                    }
                    mySqlCmd.ExecuteNonQuery();
                }
                #endregion

                mySqlCmd.Transaction.Commit();
                id = 1;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderReturnlMasterMgr-->Save-->" + ex.Message + sql.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return 0;
        }

        public int GetUserBonus(uint user_id, int bonus_type)
        {
            int master_balance = 0;
            if (bonus_type == 0)
            {
                bonus_type = 1;
            }
            string sql = string.Format("select sum(master_balance) as master_balance from bonus_master where user_id={0} and master_start<={1} and master_end>={1} and master_balance>0 and bonus_type={2}",
                user_id, CommonFunction.GetPHPTime(DateTime.Now.ToString()), bonus_type);
            if (_accessMySql.getDataTable(sql).Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(_accessMySql.getDataTable(sql).Rows[0]["master_balance"].ToString()))
                {
                    master_balance = Convert.ToInt32(_accessMySql.getDataTable(sql).Rows[0]["master_balance"]);
                }
            }

            return master_balance;
        }
        #region 扣除給予會員的購物金
        public void Deduct_User_Bonus(int accumulated_bonus, OrderMaster om)
        {
            StringBuilder sql = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            SerialDao serialDao = new SerialDao(connString);
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
                int userbonus = GetUserBonus(om.User_Id, 1);
                if (userbonus >= accumulated_bonus)
                {
                    //add_bonus_record($nUser_Id, 32, $nOrder_ Id, $deduct_bonus,'訂單取消');
                    //($nUser_Id, $nType_Id, $nOrder_Id, $nBonus_Num, $sUse_Note = '', $sUse_Writer = '')

                    string ip = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();

                    mySqlCmd.CommandText = CheckBonusTypeExist(32);//32代表 退貨產生的購物金
                    sql.Append(mySqlCmd.CommandText);
                    // mySqlCmd.ExecuteNonQuery();
                    //判斷type_id是否存在
                    if (mySqlCmd.ExecuteScalar().ToString() == null)
                    {
                        mySqlCmd.Transaction.Rollback();
                    }
                    int bonusnum = accumulated_bonus;
                    // 會員目前可用購       物金
                    if (bonusnum < 0)
                    {
                        bonusnum = bonusnum * -1;
                    }
                    if (bonusnum > userbonus)
                    {
                        mySqlCmd.Transaction.Rollback();
                    }
                    // 找出目前可用購物金,以到期日排序

                    List<BonusMaster> bmList = QueryBonusById(om.User_Id);
                    foreach (var item in bmList)
                    {
                        if (bonusnum > 0)
                        {
                            // 記錄點數數量
                            int tempNum = bonusnum > item.master_balance ? item.master_balance : bonusnum;

                            // 取得下一個流水號
                            mySqlCmd.CommandText = serialDao.Update(28);//28 // 購物金使用記錄流水號
                            sql.Append(mySqlCmd.CommandText);
                            BonusRecord brModel = new BonusRecord();
                            brModel.record_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());
                            brModel.master_id = item.master_id;
                            brModel.type_id = 32;
                            brModel.order_id = om.Order_Id;
                            brModel.record_ipfrom = ip;
                            brModel.record_use = uint.Parse(tempNum.ToString());
                            mySqlCmd.CommandText = InsertBonusRecord(brModel);
                            sql.Append(mySqlCmd.CommandText);
                            mySqlCmd.ExecuteNonQuery();

                            // 更新購物金結餘
                            mySqlCmd.CommandText = UpdateBonusMaster(tempNum, item.master_id);
                            sql.Append(mySqlCmd.CommandText);
                            mySqlCmd.ExecuteNonQuery();
                            bonusnum -= tempNum;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                else
                {
                    sql.AppendFormat(" insert into  users_deduct_bonus (deduct_bonus,user_id,createdate,order_id) values ({0},{1},{2},{4})",
                      accumulated_bonus, om.User_Id, CommonFunction.GetPHPTime(DateTime.Now.ToString()), om.Order_Id);
                }
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderReturnMasterDao-->Deduct_User_Bonus-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }
        public string CheckBonusTypeExist(uint type_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select * from bonus_type where type_id={0};", type_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.CheckBonusTypeExist-->" + ex.Message + sql.ToString(), ex);
            }
        }
        // 找出目前可用購物金,以到期日排序
        public List<BonusMaster> QueryBonusById(uint user_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select * from bonus_master where user_id={0}", user_id);
                sql.AppendFormat(@" and master_start<={0} and master_end>={1}", CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                sql.AppendFormat(@" and master_balance > 0 and bonus_type = 2 order by master_end asc");
                return _accessMySql.getDataTableForObj<BonusMaster>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.CheckBonusTypeExist-->" + ex.Message + sql.ToString(), ex);
            }
        }
        // 記錄
        public string InsertBonusRecord(BonusRecord brModel)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"insert into bonus_record (record_id,master_id,type_id,");
                sql.AppendFormat(@"order_id,record_use,record_note,record_writer");
                sql.AppendFormat(@"record_createdate,record_updatedate,record_ipfrom)");
                sql.AppendFormat(@" values({0},{1},{2},{3},", brModel.record_id, brModel.master_id, 32, brModel.order_id);
                sql.AppendFormat(@"{0},{1},{2},", brModel.record_use, "訂單取消", "");
                sql.AppendFormat(@"{0},{1},{2});", CommonFunction.GetPHPTime(DateTime.Now.ToString()), CommonFunction.GetPHPTime(DateTime.Now.ToString()), brModel.record_ipfrom);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.InsertBonusRecord-->" + ex.Message + sql.ToString(), ex);
            }
        }
        // 更新購物金結餘
        public string UpdateBonusMaster(int temp_num, uint master_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" set sql_safe_updates = 0;");
                sql.AppendFormat(@"update bonus_master set master_balance=master_balance-{0}", temp_num);
                sql.AppendFormat(@" where master_id={0};", master_id);
                sql.Append(" set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.UpdateBonusMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 扣除給予會員的hg點數
        public int Deduct_User_Happy_Go(int accumulated_happygo, uint order_id)
        {
            StringBuilder sql = new StringBuilder();
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
                if (accumulated_happygo != 0 && order_id != 0)
                {
                    HgDeduct hg_deduct = GetHappyGoEncIdno(order_id);
                    sql.Clear();
                    if (hg_deduct != null)
                    {
                        mySqlCmd.CommandText = InsertHGAccRefund(hg_deduct, accumulated_happygo, order_id);
                        sql.Append(mySqlCmd.CommandText);
                        result = mySqlCmd.ExecuteNonQuery();
                        sql.Clear();
                    }
                }
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderReturnMasterDao-->Deduct_User_Happy_Go-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return result;
        }

        public HgDeduct GetHappyGoEncIdno(uint order_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select enc_idno,chk_sum,token,order_id from hg_deduct where order_id={0} limit 0,1", order_id);
                return _accessMySql.getSinggleObj<HgDeduct>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.GetHappyGoEncIdno-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string InsertHGAccRefund(HgDeduct hg_deduct, int accumulated_happygo, uint order_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"insert into hg_accumulate_refund (enc_idno,chk_sum,transaction_date,");
                sql.AppendFormat(@"merchant,terminal,refund_point,category,wallet,note,order_id)");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}',", hg_deduct.enc_idno, hg_deduct.chk_sum, DateTime.Now.ToString("yyyyMMddHHmmss"), "6601000081");
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", "13999501", accumulated_happygo, "N0699999", "991991");
                sql.AppendFormat(@"'{0}','{1}')", "吉甲地台灣好市集訂單編號" + order_id + "扣除點數:" + accumulated_happygo + "點", order_id);

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.InsertHGAccRefund-->" + ex.Message + sql.ToString(), ex);
            }
        }

        #endregion
        #region 返回扣除的購物金..等
        public void Deduct_Refund(OrderMaster om, int deductbonus, int deductcash, int deducthg, OrderReturnMasterQuery query)
        {
            #region 購金
            int expireDay = 90;
            BonusMaster bm = new BonusMaster();
            bm.user_id = om.User_Id;
            bm.type_id = 4;
            bm.bonus_type = 1;
            DateTime master_start = DateTime.Now;
            bm.master_start = Convert.ToUInt32(CommonFunction.GetPHPTime(master_start.ToString("yyyy-MM-dd 00:00:00")));
            bm.master_createdate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
            bm.master_updatedate = Convert.ToUInt32(CommonFunction.GetPHPTime(DateTime.Now.ToString()));
            bm.master_ipfrom = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
            bm.master_note = om.Order_Id.ToString();
            bm.master_writer = "system";
            if (deductbonus > 0)
            {

                List<VipUserGroup> vipuserLi = _vipuserGroupDao.GetVipUserByOrderId(om.User_Id, 0, 1);
                if (vipuserLi != null)
                {
                    VipUserGroup vug = vipuserLi.FirstOrDefault();
                    expireDay = vug.bonus_expire_day;
                    om.Accumulated_Bonus = om.Accumulated_Bonus * vug.bonus_rate;
                }
                #region BonusMaster Model
                bm.master_total = Convert.ToUInt32(deductbonus);
                bm.master_balance = Convert.ToInt32(deductbonus);
                DateTime master_end = master_start.AddDays(Convert.ToDouble(expireDay));
                bm.master_end = Convert.ToUInt32(CommonFunction.GetPHPTime(master_end.ToString("yyyy-MM-dd 23:59:59")));

                //bonus_master_add($aOrder['user_id'], BONUS_TYPE_ID_ORDER_CANCEL, $nBonus, 
                //$nMaster_Start, $nMaster_End, $aOrder['order_id'], 'system');


                #endregion

                Bonus_Master_Add(bm);
            }
            #endregion
            #region 抵用券
            if (deductcash > 0)
            {
                bm.master_total = Convert.ToUInt32(deductcash);
                bm.master_balance = Convert.ToInt32(deductcash);
                DateTime master_end = master_start.AddDays(Convert.ToDouble(expireDay));
                bm.master_end = Convert.ToUInt32(CommonFunction.GetPHPTime(master_end.ToString("yyyy-MM-dd 23:59:59")));
                Bonus_Master_Add(bm);
            }
            #endregion
            #region HG點
            if (deducthg > 0)
            {
                //hg_deduct_reverse(deducthg, om.Order_Id, query);
            }
            #endregion


        }
        public void Bonus_Master_Add(BonusMaster bm)
        {
            StringBuilder sql = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            SerialDao serialDao = new SerialDao("");
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
                //type_id 4 // 訂單退回購物金
                mySqlCmd.CommandText = CheckBonusTypeExist(4);//32代表 退貨產生的購物金
                sql.Append(mySqlCmd.CommandText);
                // mySqlCmd.ExecuteNonQuery();
                //判斷type_id是否存在
                if (mySqlCmd.ExecuteScalar().ToString() == null)
                {
                    mySqlCmd.Transaction.Rollback();
                }
                // 限制不能為負值
                if (bm.master_total < 0)
                {
                    mySqlCmd.Transaction.Rollback();
                }
                // 判斷日期
                if (bm.master_start > bm.master_end)
                {
                    mySqlCmd.Transaction.Rollback();
                }
                // 取得下一個流水號
                mySqlCmd.CommandText = serialDao.Update(27);//27 購物金流水號
                sql.Append(mySqlCmd.CommandText);
                bm.master_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());
                mySqlCmd.CommandText = InsertBonusMaster(bm);
                sql.Append(mySqlCmd.CommandText);
                mySqlCmd.ExecuteScalar();

                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderReturnMasterDao-->Bonus_Master_Add-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }
        // 更新購物金結餘
        public string InsertBonusMaster(BonusMaster bm)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"insert into bonus_master (master_id,user_id,type_id,master_total,master_balance,");
                sql.AppendFormat(@"master_note,master_writer,master_start,master_end,master_createdate,master_updatedate,master_ipfrom,bonus_type)");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}',", bm.master_id, bm.user_id, bm.type_id, bm.master_total, bm.master_balance);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", bm.master_note, bm.master_writer, bm.master_start, bm.master_end, bm.master_createdate);
                sql.AppendFormat(@"'{0}','{1}','{2}')", bm.master_updatedate, bm.master_ipfrom, bm.bonus_type);

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.InsertBonusMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region happyGo即時歸還hg點數
        public void Hg_Deduct_Reverse(int deduct_happygo, uint order_id, OrderReturnMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
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
                HgDeduct hd = GetHappyGoEncIdno(order_id);

                // https://www.gimg.com/shoppings/happygo_deduct_reverse?
                //MERCHANT_POS=6601000081
                //    &TERMINAL_POS=13999501
                //    &ENC_IDNO=hd.enc_idno
                // &CHK_SUM=hd.chk_sum
                //   &TOKEN=hd.token
                //  &ORDER_ID=hd.order_id
                //	$url = ''; 
                //$url .= HG_REVERSALFULLREDEEM_URL.'?MERCHANT_POS='.HG_MERCHANDID.     https://www.gimg.com/shoppings/happygo_deduct_reverse
                //		'&TERMINAL_POS='.HG_TERMINALID.'&ENC_IDNO='.$hg_deduct['enc_idno'].'&CHK_SUM='.$hg_deduct['chk_sum'].'&TOKEN='.$hg_deduct['token'].'&ORDER_ID='.$hg_deduct['order_id']; 

                if (hd != null)
                {
                    #region phpCode
                    StreamReader sr = GetUrl(query, hd);
                    string strLine = "";
                    HgDeductReverse hdr = new HgDeductReverse();

                    //while ((strLine = sr.ReadLine()) != null)
                    //{
                    //    hdr.merchant_pos = query.HgMerchandID;
                    //    hdr.terminal_pos = query.HgTeminalID;
                    //    hdr.enc_idno=
                    //}
                    //$curl = curl_init($url);

                    //curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);
                    //curl_setopt ($curl, CURLOPT_HEADER, 0);
                    //$data = curl_exec($curl);//傳回值
                    //curl_close($curl);

                    //$xml = simplexml_load_string($data);


                    //$happygo = array(
                    //       'merchant_pos' => (string)$xml->DATA->MERCHANT_POS,
                    //       'terminal_pos' => (string)$xml->DATA->TERMINAL_POS,
                    //       'enc_idno' => (string)$xml->DATA->ENC_IDNO,
                    //       'chk_sum' =>(string) $xml->DATA->CHK_SUM,
                    //       'token' => (string)$xml->DATA->TOKEN,
                    //       'order_id' => (string)$xml->DATA->ORDER_ID,
                    //       'date' =>(string) $xml->DATA->DATE,
                    //       'time' => (string)$xml->DATA->TIME,
                    //       'code' =>(string) $xml->RESPONSE->CODE,
                    //       'message' => (string)$xml->RESPONSE->MESSAGE
                    //);
                    //$sSQL = 'INSERT INTO  hg_deduct_reverse   ' . $amego_db->sql_build_array('INSERT', $happygo );
                    //$amego_db->sql_query($sSQL);

                    // if(!$xml->RESPONSE->CODE == 5000)
                    // {
                    //    //insert hg_deduct_refund
                    //    $this ->accumulated_user_happy_go($deduct_happygo,$nOrder_Id);
                    // }



                    mySqlCmd.CommandText = AccumulatedUserHG(deduct_happygo, hd);
                    sql.Append(mySqlCmd.CommandText);
                    mySqlCmd.ExecuteNonQuery();
                    sql.Clear();
                    //                    * 
                    //     * 還回購物時扣除的hg點數
                    //     * @param $deduct_hg 
                    //     */
                    //    function accumulated_user_happy_go( $deduct_happygo = 0 , $nOrder_Id = 0)
                    //    {
                    //        global $amego_db;	

                    //        $happygo = array(
                    //            'enc_idno'		=> '',
                    //            'chk_sum'		=> '',
                    //            'transaction_date'		=> date('Y/m/d H:i:s', (time())),
                    //            'merchant'		=> HG_MERCHANDID,
                    //            'terminal'		=> HG_TERMINALID,
                    //            'refund_point'		=> (int) $deduct_happygo,
                    //            'category'		=> HG_CATEGORY,
                    //            'wallet'		=> HG_WALLET,
                    //            'note'		=> '吉甲地台灣好市集訂單編號'.$nOrder_Id.'歸還點數:'.$deduct_happygo.'點',
                    //            'order_id'	=> (int) $nOrder_Id
                    //        );

                    //        if($deduct_happygo != 0 && $nOrder_Id != 0)
                    //        {
                    //            $hg_deduct = $this ->get_happy_go_enc_idno($nOrder_Id); //QueryHgDecut(order_id)
                    //            if($hg_deduct == false)
                    //            {
                    //                die('取得身分證字號失敗');
                    //            }
                    //            $happygo['enc_idno'] = $hg_deduct['enc_idno'];
                    //            $happygo['chk_sum'] = $hg_deduct['chk_sum'];
                    //            $sSQL = 'INSERT INTO  hg_deduct_refund   ' . $amego_db->sql_build_array('INSERT', $happygo );
                    ////			echo $sSQL.'歸還抵扣HG點數<br/>';;
                    //            $amego_db->sql_query($sSQL);
                    //        }	
                    //        else 
                    //        {
                    //            die('扣除HappyGo點數失敗');
                    //        }

                    //    }
                    #endregion

                }
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception(" OrderCancelMasterDao-->Hg_Deduct_Reverse-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }

        }

        //還回購物時扣除的hg點數
        public string AccumulatedUserHG(int deductHg, HgDeduct hd)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (deductHg != 0 && (!string.IsNullOrEmpty(hd.order_id)))
                {
                    sql.Append(" INSERT INTO  hg_accumulate_refund ");
                    sql.Append(" (enc_idno,chk_sum,transaction_date,merchant,terminal,refund_point,category,wallet,note,order_id) ");
                    sql.AppendFormat(" values ('{0}','{1}','{2}','{3}','{4}'", hd.enc_idno, hd.chk_sum, DateTime.Now, 6601000081, 13999501);
                    sql.AppendFormat(" ,'{0}','{1}','{2}','{3}','{4}')", deductHg, "N0699999", 991991, "吉甲地台灣好市集訂單編號" + hd.order_id + "扣除點數:" + deductHg + "點", hd.order_id);
                }
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.AccumulatedUserHG-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        public List<OrderDetail> Get_Combined_Product(uint order_id, uint detail_id, uint pack_id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select od.* from order_detail od,order_slave os where os.order_id={0}", order_id);
            sql.AppendFormat(@" and	os.slave_id = od.slave_id and od.item_mode = 2 and od.pack_id ={0} and od.parent_id ={1} ", pack_id, detail_id);
            return _accessMySql.getDataTableForObj<OrderDetail>(sql.ToString());

        }
        public string GetTotalCount(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"SELECT count(*) total FROM");
                sql.Append(@"order_detail INNER JOIN order_slave on order_detail.slave_id = order_slave.slave_id ");
                sql.Append(@"	WHERE order_detail.detail_status not in (89,90,91)");
                sql.AppendFormat(" and  order_slave.order_id ={0}", order_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.GetTotalCount-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public StreamReader GetUrl(OrderReturnMasterQuery query, HgDeduct hd)
        {
            WebClient wc = new WebClient();
            string url = query.HgReturnUrl + "?MERCHANT_POS=" + query.HgMerchandID + "&TERMINAL_POS=" + query.HgTeminalID +
                "&ENC_IDNO=" + hd.enc_idno + "&CHK_SUM=" + hd.chk_sum + "&TOKEN=" + hd.token + "&ORDER_ID=" + hd.order_id;
            Stream stream = wc.OpenRead(url);
            StreamReader sr = new StreamReader(stream);
            return sr;
        }

        #region 後台訂單退貨
        public List<OrderDetailQuery> GetOrderreturn(OrderReturnMasterQuery q)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
            sql.AppendFormat(@"
SELECT	od.detail_id,od.slave_id,od.item_id,od.item_vendor_id,od.product_name,od.item_code,od.parent_id,od.pack_id,od.single_money,
od.buy_num,od.product_freight_set,os.order_id,os.vendor_id,v.vendor_name_simple
FROM order_detail od,order_slave os,vendor v
WHERE	od.detail_id IN ({0})  AND	os.slave_id = od.slave_id AND od.detail_status in (4,6,7) AND os.vendor_id = v.vendor_id AND os.slave_status <> '99' ORDER BY slave_id ASC, product_freight_set ASC, item_id ASC;", q.detailId);
            return _accessMySql.getDataTableForObj<OrderDetailQuery>(sql.ToString());
        }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.GetOrderreturn-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string InsertOrderReturnMaster(OrderReturnMaster q)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"insert into order_return_master(return_id,order_id,vendor_id,return_status,return_note,return_createdate,return_updatedate,return_ipfrom) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}') ;", q.return_id,q.order_id,q.vendor_id,q.return_status,q.return_note,CommonFunction.GetPHPTime(DateTime.Now.ToString()),CommonFunction.GetPHPTime(DateTime.Now.ToString()),q.return_ipfrom);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.InsertOrderReturnMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        //修改訂單狀態
        public string UpdOrderMaster(uint order_id,string order_status)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0; ");
                sql.AppendFormat(@"update order_master set order_status='{0}',order_updatedate='{1}',order_ipfrom='{2}' ",order_status,CommonFunction.GetPHPTime(DateTime.Now.ToString()), ipfrom);
                sql.AppendFormat(@" where order_id='{0}' ;", order_id);
                sql.Append(" set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.InsertOrderReturnMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        //新增訂單狀態記錄
        public string InsertOrderMasterStatus(ulong serial_id,uint order_id, string order_status, string status_description)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"insert into order_master_status(serial_id,order_id,order_status,status_description,status_ipfrom,status_createdate) values('{0}','{1}','{2}','{3}','{4}','{5}');", serial_id, order_id, order_status, status_description, ipfrom, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.InsertOrderMasterStatus-->" + ex.Message + sql.ToString(), ex);
            }
        }
        //新增退貨單細項
        public string InsertOrderReturnDetail(string return_id, string detail_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"insert into order_return_detail(return_id,detail_id) values('{0}','{1}');", return_id, detail_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.InsertOrderReturnDetail-->" + ex.Message + sql.ToString(), ex);
            }
        }
        //獲取組合商品
        public List<OrderDetail> Getdetail(string slave_id, string parent_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select detail_id from order_detail where  slave_id='{0}' and parent_id='{1}' ", slave_id, parent_id);
                return _accessMySql.getDataTableForObj<OrderDetail>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.Getdetail-->" + ex.Message + sql.ToString(), ex);
            }
        }
        //修改訂單細項狀態--組合商品的子商品
        public string UpdOrderDetail(string detail_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {//91 訂單退貨狀態
                sql.Append(@"set sql_safe_updates = 0; ");
                sql.AppendFormat(@"update order_detail set detail_status='{0}' ", "91");
                sql.AppendFormat(@" where detail_id='{0}' ;", detail_id);
                sql.Append("set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.UpdOrderDetail-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public int GetExportFlag(string order_id)
        {
            StringBuilder sql = new StringBuilder();
            DataTable dt = new DataTable();
            try
            {
                if (!string.IsNullOrEmpty(order_id) && order_id != "0")
                {
                    sql.AppendFormat(@"select export_flag from order_master where  order_id='{0}' ;", order_id);
                    dt = _accessMySql.getDataTable(sql.ToString());
                }
                if(dt.Rows.Count>0)
                {
                    return int.Parse(dt.Rows[0]["export_flag"].ToString());
                }
                else
                {
                    return 0;
                }                
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.GetExportFlag-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdExportFlag(string order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"set sql_safe_updates = 0; ");
                sql.AppendFormat(@"update order_master set export_flag='1' where order_id='{0}';", order_id);
                sql.Append(" set sql_safe_updates = 1;");
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao.UpdExportFlag-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public int InsertSql(string sql)
        {//事物插入數據
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            int i = 0;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                mySqlCmd.CommandText = sql;
                i = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("AseldDao.InsertSql-->" + ex.Message + ",sql:" + mySqlCmd.CommandText, ex);
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

        public string InsertOrderMasterS(OrderMasterStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("insert into order_master_status (serial_id,order_id,order_status,status_description,status_ipfrom,status_createdate)values(");
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}');",query.serial_id,query.order_id,query.order_status,query.status_description,query.status_ipfrom,CommonFunction.GetPHPTime());
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao-->InsertOrderMasterS-->"+ex.Message+sql.ToString(),ex);
            }
        }

        public string UpOrderReturnMaster(OrderReturnMasterQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_return_master set return_status='{0}' where return_id='{1}';",query.return_status,query.return_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao-->UpOrderReturnMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable GetSerialID(int serialId)
        {
            string sqlSerial = string.Empty;
            try
            {
                SerialDao serialDao = new SerialDao(connString);
                  sqlSerial = serialDao.Update(29);//訂單主檔狀態流水號
                return _accessMySql.getDataTable(sqlSerial);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao-->GetSerialID-->" + ex.Message + sqlSerial.ToString(), ex);
            }
        }

        public DataTable OrsStatus(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" SELECT ors_status from order_return_status where ors_order_id='{0}' order by   ors_status desc ;", order_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnMasterDao-->OrsStatus-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
