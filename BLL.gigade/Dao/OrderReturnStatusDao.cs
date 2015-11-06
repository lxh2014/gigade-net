using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class OrderReturnStatusDao
    {
        private IDBAccess _access;
        private string connStr;
        private IOrderMasterImplDao _ordermasterdao;
        private IOrderDetailImplDao _orderDetailDao;
        private IVipUserGroupImplDao _vipuserGroupDao;
        private IOrderMoneyReturnImplDao _orderMoneyReturnDao;
        private IProductItemImplDao _itemDao;
        private IUserRecommendIDao _userRecommendDao;
        private MySqlDao _mySqlDao;
        public OrderReturnStatusDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
            _ordermasterdao = new OrderMasterDao(connectionstring);
            _orderDetailDao = new OrderDetailDao(connectionstring);
            _orderMoneyReturnDao = new OrderMoneyReturnDao(connectionstring);
            _vipuserGroupDao = new VipUserGroupDao(connectionstring);
            _itemDao = new ProductItemDao(connectionstring);
            _userRecommendDao = new UserRecommendDao(connectionstring);
            _mySqlDao = new MySqlDao(connectionstring);
            this.connStr = connectionstring;
        }

        public DataTable CheckOrderId(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("select ors.ors_status from order_return_status ors   where ors.ors_order_id='{0}'  and return_id='{1}' order by ors_status desc; ", query.ors_order_id, query.return_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return _dt;

            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->CheckOrderId-->" + ex.Message + sql.ToString(), ex);
            }

        }

        public DataTable CheckTransport(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                query.Replace4MySQL();
                sql.AppendFormat("select orc.orc_deliver_code from order_return_content orc   where orc.orc_order_id='{0}' and return_id='{1}' ; ", query.orc_order_id,query.return_id);
                DataTable _dt = _access.getDataTable(sql.ToString());
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->CheckTransport-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public OrderMaster GetOrderInfo(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                //  query.Replace4MySQL();
                sql.AppendFormat("select om.delivery_name,om.delivery_mobile,om.delivery_phone,om.delivery_zip,om.delivery_address from order_master om where om.order_id='{0}';", order_id);
                return _access.getSinggleObj<OrderMaster>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->GetOrderInfo-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable OrderIdIsExist(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                // query.Replace4MySQL();
                sql.AppendFormat("select order_id,return_id from order_return_master where order_id='{0}' and return_status=0;", order_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->OrderIdIsExist-->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string InsertOrderReturnContent(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into order_return_content(orc_order_id,orc_deliver_code,orc_deliver_date,orc_deliver_time,orc_name,orc_phone,orc_zipcode,orc_address,orc_remark,orc_type,orc_service_remark,return_id,orc_send) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}');", query.orc_order_id, query.orc_deliver_code, CommonFunction.DateTimeToString(query.orc_deliver_date), query.orc_deliver_time, query.orc_name, query.orc_phone, query.orc_zipcode, query.orc_address, query.orc_remark,query.orc_type,query.orc_service_remark,query.return_id,query.orc_send);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->InsertOrderReturnContent-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string InsertOrderReturnStatus(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into order_return_status(ors_order_id,ors_status,ors_remark,ors_createdate,ors_createuser,return_id) values('{0}','{1}','{2}','{3}','{4}','{5}');", query.ors_order_id, query.ors_status, query.ors_remark, CommonFunction.DateTimeToString(query.ors_createdate), query.ors_createuser,query.return_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->InsertOrderReturnStatus-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public bool ExecSqls(ArrayList arrList)
        {
            try
            {
                MySqlDao myDao = new MySqlDao(connStr);
                return myDao.ExcuteSqlsThrowException(arrList);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->ExecSqls-->" + arrList + ex.Message, ex);
            }
        }

        public string CouldReturn(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into order_return_status(ors_order_id,ors_status,ors_remark,ors_createdate,ors_createuser,return_id) values('{0}','{1}','{2}','{3}','{4}','{5}');", query.ors_order_id, query.ors_status, query.ors_remark, CommonFunction.DateTimeToString(DateTime.Now), query.ors_createuser, query.return_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->CouldReturn-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string UpOrderReturnMaster(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;update order_return_master set return_status=1 where return_id='{0}';set sql_safe_updates = 1;", query.return_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->UpOrderReturnMaster-->"+sql.ToString()+ex.Message,ex);
            }
        }

        public List<OrderReturnStatusQuery> CouldGridList(OrderReturnStatusQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            try
            {
                totalCount = 0;
                sql.Append("select orm.order_id,od.item_id,od.product_name,para.parameterName as 'product_mode',od.product_spec_name,od.buy_num,od.single_money ,om.order_payment,orm.bank_name,orm.bank_branch,orm.bank_account,orm.account_name  ");
                sqlFrom.Append(" from order_return_master  orm  LEFT JOIN order_return_detail ord on orm.return_id=ord.return_id ");
                sqlFrom.Append("LEFT JOIN order_detail  od on ord.detail_id=od.detail_id ");
                sqlFrom.Append(" LEFT JOIN (select parameterCode,parameterName,remark from t_parametersrc where parametertype='product_mode') para on od.product_mode=para.parameterCode  ");
                sqlFrom.Append(" LEFT JOIN order_master om on om.order_id = orm.order_id ");
                sqlWhere.AppendFormat(" where orm.return_id='{0}'  ", query.return_id);
                if (query.itemStr!="")
                {
                    sqlWhere.AppendFormat(" and od.item_id not in({0}) ",query.itemStr);
                }
                if (query.IsPage)
                {
                    DataTable _dt = _access.getDataTable("select count(orm.order_id) as 'totalCount'  " + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0][0]);
                    }
                }
                sqlWhere.AppendFormat(" limit {0},{1}; ", query.Start, query.Limit);
                sql.Append(sqlFrom.ToString() + sqlWhere.ToString());
                return _access.getDataTableForObj<OrderReturnStatusQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->CouldGridList-->" + sql.ToString() + ex.Message, ex);
            }

        }

        #region could resolved by one method ,to bo continued...
        public DataTable GetOrcTypeStore()
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append("select parameterCode, parameterName from t_parametersrc where parameterType='orc_type';");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->GetOrcTypeStore-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetInvoiceDealStore()
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append("select parameterCode, parameterName from t_parametersrc where parameterType='invoice_deal';");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->GetInvoiceDealStore-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public DataTable GetPackageStore()
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append("select parameterCode, parameterName from t_parametersrc where parameterType='package';");
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->GetInvoiceDealStore-->" + sql.ToString() + ex.Message, ex);
            }
        }
        #endregion

        #region 確認入庫
        //更新庫存的操作
        public bool PlaceOnFile(OrderReturnStatusQuery query)
        {
            //選中要退的商品進行入庫，未選中的則不入庫，將未選中的記錄到order_return_status里
            try
            {
                bool b = true;
                ArrayList arrList = new ArrayList();
              
                if (query.ors_status == 3)
                {
                    List<OrderReturnStatusQuery> ORStore = new List<OrderReturnStatusQuery>();

                    if (query.data != "")
                    {
                        string[] dataSplit = query.data.Split(';');
                        for (int i = 0; i < dataSplit.Length; i++)
                        {
                            if (dataSplit[i] != "")
                            {
                                string[] newData = dataSplit[i].Split('*');
                                query.item_id = Convert.ToUInt32(newData[0]);
                                query.itemStr += query.item_id + ",";
                                query.buy_num = Convert.ToUInt32(newData[1]);
                                DataTable odli = _orderDetailDao.OrderDetailTable(Convert.ToUInt32(query.return_id), query.item_id);
                                if (Convert.ToInt32(odli.Rows[0]["item_mode"]) == 1)//组合商品
                                {
                                    List<OrderDetail> childDetail = Get_Combined_Product(Convert.ToUInt32(query.ors_order_id), Convert.ToUInt32(odli.Rows[0]["Parent_Id"]), Convert.ToUInt32(odli.Rows[0]["pack_id"]));
                                    foreach (var child in childDetail)
                                    {
                                        arrList.Add(_itemDao.UpdateItemStock(child.Item_Id, child.Buy_Num * child.parent_num));//更新庫存
                                    }
                                }
                                else
                                {
                                    arrList.Add(_itemDao.UpdateItemStock(query.item_id, query.buy_num));//更新庫存
                                }
                            }
                        }
                    }
                        int totalCount = 0;
                        query.IsPage = false;
                        query.itemStr = query.itemStr.TrimEnd(',');
                       // query.nonSelected = true;
                        ORStore = CouldGridList(query, out totalCount);
                        query.itemStr = string.Empty;
                        foreach (var item in ORStore)
                        {
                            query.itemStr += item.item_id + ",";
                        }
                        if (query.itemStr != "")
                        {
                            query.ors_remark = "未入庫的商品細項編號為:" + query.itemStr.TrimEnd(',');
                        }
                    arrList.Add(CouldReturn(query));
                }
                if (_mySqlDao.ExcuteSqlsThrowException(arrList))
                {
                    b = true;
                }
                else
                {
                    b = false;
                }
                return b;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->PlaceOnFile-->" + ex.Message, ex);
            }

        }

        //確認退款單---執行之前的歸檔邏輯但是除去更新庫存的操作
        public bool CouldReturnMoney(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            ArrayList arrList = new ArrayList();
            try
            {
                arrList.Add(CouldReturn(query));
                arrList.Add(UpOrderReturnMaster(query));
                HgBatchAccumulateRefund hgBatch = new HgBatchAccumulateRefund();
               //HGLogin hgLogin = GetHGLoginData((uint)query.ors_order_id);
                OrderMaster om = _ordermasterdao.GetOrderMasterByOrderId4Change(Convert.ToInt32(query.ors_order_id));
                DataTable _returnDt = GetReturnId(query);
                DataTable odli = _orderDetailDao.OrderDetailTable(Convert.ToUInt32(query.return_id),0);
                OrderReturnMasterQuery omQuery = new OrderReturnMasterQuery();
                SerialDao serialDao = new SerialDao(connStr);
                MySqlCommand mySqlCmd = new MySqlCommand();
                MySqlConnection mySqlConn = new MySqlConnection(connStr);
                if (odli != null)
                {
                    #region hg個字段賦值
                  /*為方便測試暫時先注釋
                    if (hgLogin != null)
                    {
                        #region hg_batch_accumulate_refund
                        hgBatch.order_id = (uint)query.ors_order_id;
                        hgBatch.head = "B";
                        hgBatch.card_no = ""; 
                        hgBatch.card_checksum = ""; 
                        hgBatch.category_id = "N0699999";
                        hgBatch.wallet = "991991"; 
                        hgBatch.enc_idno = hgLogin.enc_idno;
                        hgBatch.checksum = hgLogin.chk_sum;
                        hgBatch.transaction_time = hgLogin.transaction_time;
                        hgBatch.merchant_pos = hgLogin.merchant_pos;
                        hgBatch.terminal_pos = hgLogin.terminal_pos;
                        hgBatch.refund_point = om.Deduct_Happygo;
                        //hgBatch.order_note = "吉甲地台灣好市集訂單編號" + hgBatch.order_id + "返還" + hgBatch.refund_point + "點";
                        //吉甲地台灣好市集訂單編號131210039累點取消4點
                        hgBatch.batch_status = 0;
                        hgBatch.billing_checked = 0;
                       // hgBatch.batch_import_time = Convert.ToInt32(CommonFunction.GetPHPTime());
                        #endregion
                    }
                    */
                    #endregion
                    int paymoney = 0;
                    int returnmoney = 0;
                    int deductbonus = 0;
                    int deductcash = 0;
                    int accumulated_bonus = 0;
                    int accumulated_happygo = 0;
                    int deduct_happygo = 0;
                    int deduct_happygo_money = 0;
                    #region 退款金額 = 商品購買金額 - 購物金
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
                    #endregion
                    #region 判斷是否有付款
                    if (om.Money_Collect_Date > 0 && om.Order_Amount > 0 && returnmoney > 0)
                    {
                        #region 新增退款單
                        string serial_sql = serialDao.Update(46);//46 退款單流水號
                        DataTable _moneyDt = GetMoneyIDBySerial(serial_sql);
                        uint moneyId = Convert.ToUInt32(_moneyDt.Rows[0][0]);
                        OrderMoneyReturn omr = new OrderMoneyReturn();
                        omQuery.order_id = Convert.ToUInt32(query.ors_order_id);
                        omr.money_type = om.Order_Payment;
                        omr.money_total = Convert.ToUInt32(returnmoney);
                        omr.money_status = 0;
                        omr.money_source = "return_id:" + query.return_id;
                        omr.money_id = moneyId;
                        omQuery.return_ipfrom = "";
                        omQuery.bank_name = query.bank_name;
                        omr.bank_branch = query.bank_branch;
                        omr.bank_account = query.bank_account;
                        omr.account_name = query.account_name;
                        string insertSql = _orderMoneyReturnDao.InsertSql(omQuery, omr);

                        arrList.Add(insertSql);
                        #endregion
                        //取回給予的購物金與happygo點數
                        if (accumulated_bonus > 0)
                        {
                            Deduct_User_Bonus(accumulated_bonus, om);
                        }
                        //扣除給予會員的hg點數
                        if (accumulated_happygo > 0)
                        {
                         
                            //插入hg_batch_accumulate_refund
                              /*為方便測試暫時先注釋
                             arrList.Add(hg_batch_accumulate_refund(hgBatch));
                            */
                        }
                    }
                    #endregion
                    #region 黑貓例外處理
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
                            //插入hg_batch_accumulate_refund
                             /*為方便測試暫時先注釋
                            arrList.Add(hg_batch_accumulate_refund(hgBatch));
                              */
                        }
                    }
                    #endregion
                    //寫入付款單退款金額
                    arrList.Add(_ordermasterdao.UpdateMoneycanale(returnmoney, om.Order_Id));
                    //退回購買扣抵的hp點數
                    #region 判斷退回購買扣抵的hp點數  判斷是否要退回購物金
                    if (deduct_happygo > 0)
                    {
                        //插入hg_deduct_refund
                        /*為方便測試暫時先注釋
                        arrList.Add(hg_batch_deduct_refund(hgBatch));
                         */ 
                    }
                    if (deductbonus > 0)
                    {
                        Deduct_Refund(om, deductbonus, 0, 0, omQuery);
                    }
                    #endregion
                    DataTable _dt = GetTotalCount(om.Order_Id, 1);
                    if (Convert.ToInt32(_dt.Rows[0][0]) == 0)
                    {
                        if (om.Priority == 1)
                        {
                            arrList.Add(UpdatePriority(om.Order_Id));
                            arrList.Add(UpdateFirstTime(om.user_id));
                        }
                    }
                    DataTable _dt2 = GetTotalCount(om.Order_Id, 2);
                    #region  更改推薦
                    if (Convert.ToInt32(_dt2.Rows[0][0]) == 0)
                    {
                        List<UserRecommend> usRecommandLi = _userRecommendDao.QueryByOrderId(om.Order_Id);
                        string idStr = string.Empty;
                        if (usRecommandLi.Count != 0)
                        {
                            foreach (var record in usRecommandLi)
                            {
                                idStr += "," + record.id;
                            }
                             idStr=  idStr.TrimStart(',');
                            arrList.Add(_userRecommendDao.UpdateIsCommend(idStr));
                        }
                    }
                    #endregion
                    if (_mySqlDao.ExcuteSqlsThrowException(arrList))
                    {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->CouldReturnMoney-->" + ex.Message, ex);
            }
            return true;
        }

        public DataTable GetReturnId(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select return_id from order_return_master where order_id='{0}';", query.ors_order_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->GetReturnId-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #region 扣除給予會員的購物金
        public void Deduct_User_Bonus(int accumulated_bonus, OrderMaster om)
        {
            StringBuilder sql = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            SerialDao serialDao = new SerialDao(connStr);
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
                int userbonus = GetUserBonus(om.User_Id, 1);
                if (userbonus >= accumulated_bonus)
                {
                    string ip = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList.FirstOrDefault().ToString();
                    mySqlCmd.CommandText = CheckBonusTypeExist(32);//32代表 退貨產生的購物金
                    //判斷type_id是否存在
                    if (mySqlCmd.ExecuteScalar().ToString() == null)
                    {
                        mySqlCmd.Transaction.Rollback();
                    }
                    int bonusnum = accumulated_bonus;
                    // 會員目前可用購物金
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
                            BonusRecord brModel = new BonusRecord();
                            brModel.record_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());
                            brModel.master_id = item.master_id;
                            brModel.type_id = 32;
                            brModel.order_id = om.Order_Id;
                            brModel.record_ipfrom = ip;
                            brModel.record_use = uint.Parse(tempNum.ToString());
                            // 更新購物金結餘
                            mySqlCmd.CommandText += UpdateBonusMaster(tempNum, item.master_id);
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
                    sql.AppendFormat(" insert into  users_deduct_bonus (deduct_bonus,user_id,createdate,order_id) values ('{0}','{1}','{2}','{3}');",
                      accumulated_bonus, om.User_Id, CommonFunction.GetPHPTime(DateTime.Now.ToString()), om.Order_Id);
                    mySqlCmd.CommandText += sql;
                   
                }
                mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderReturnStatusDao-->Deduct_User_Bonus-->" + ex.Message, ex);
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
                sql.AppendFormat("select type_id from bonus_type where type_id={0};", type_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.CheckBonusTypeExist-->" + ex.Message + sql.ToString(), ex);
            }
        }
        // 找出目前可用購物金,以到期日排序
        public List<BonusMaster> QueryBonusById(uint user_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"select master_id,master_balance from bonus_master where user_id={0}", user_id);
                sql.AppendFormat(@" and master_start<={0} and master_end>={0}", CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                sql.AppendFormat(@" and master_balance > 0 and bonus_type = 1 order by master_end asc");
                return _access.getDataTableForObj<BonusMaster>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.CheckBonusTypeExist-->" + ex.Message + sql.ToString(), ex);
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
                throw new Exception("OrderReturnStatusDao.InsertBonusRecord-->" + ex.Message + sql.ToString(), ex);
            }
        }
        // 更新購物金結餘
        public string UpdateBonusMaster(int temp_num, uint master_id)
        {

            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"update bonus_master set master_balance=master_balance-{0}", temp_num);
                sql.AppendFormat(@" where master_id={0};", master_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.UpdateBonusMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        public int GetUserBonus(uint user_id, int bonus_type)
        {
            int master_balance = 0;
            if (bonus_type == 0)
            {
                bonus_type = 1;
            }
            string sql = string.Format("select sum(master_balance) as master_balance from bonus_master where user_id={0} and master_start<={1} and master_end>={1} and master_balance>0 and bonus_type={2}",
                user_id, CommonFunction.GetPHPTime(DateTime.Now.ToString()), bonus_type);
            if (_access.getDataTable(sql).Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(_access.getDataTable(sql).Rows[0]["master_balance"].ToString()))
                {
                    master_balance = Convert.ToInt32(_access.getDataTable(sql).Rows[0]["master_balance"]);
                }
            }

            return master_balance;
        }
        public int Deduct_User_Happy_Go(int accumulated_happygo, uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connStr);
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

                    if (hg_deduct != null)
                    {
                        mySqlCmd.CommandText = InsertHGAccRefund(hg_deduct, accumulated_happygo, order_id);
                        //     sql.Append(mySqlCmd.CommandText);
                        result = mySqlCmd.ExecuteNonQuery();
                        sql.Clear();
                    }
                }
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderReturnStatusDao-->Deduct_User_Happy_Go-->" + ex.Message, ex);
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
                sql.AppendFormat(@"select order_id from hg_deduct where order_id={0} limit 0,1", order_id);
                return _access.getSinggleObj<HgDeduct>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.GetHappyGoEncIdno-->" + ex.Message + sql.ToString(), ex);
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
                throw new Exception("OrderReturnStatusDao.InsertHGAccRefund-->" + ex.Message + sql.ToString(), ex);
            }
        }
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
                if (vipuserLi.Count!=0)
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
                #endregion
                bm.master_writer = "訂單退貨返還購物金";
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
                bm.master_writer = "訂單退貨返還抵用券";
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
            SerialDao serialDao = new SerialDao(connStr);
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
                //type_id 4 // 訂單退回購物金
                mySqlCmd.CommandText = CheckBonusTypeExist(4);//32代表 退貨產生的購物金
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
                string  master_sql= serialDao.Update(27);//27 購物金流水號
                DataTable _dt = GetMasterID(master_sql);
                bm.master_id = Convert.ToUInt32(_dt.Rows[0][0]);
                mySqlCmd.CommandText += InsertBonusMaster(bm);
                mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderReturnStatusDao-->Bonus_Master_Add-->" + ex.Message, ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
        }
        public DataTable GetMasterID(string sql)
        {
            try
            {
                DataTable _dt = _access.getDataTable(sql);
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.GetMasterID-->" + ex.Message + sql.ToString(), ex);
            }
        }
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
                throw new Exception("OrderReturnStatusDao.InsertBonusMaster-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public List<OrderDetail> Get_Combined_Product(uint order_id, uint detail_id, uint pack_id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select od.item_id, od.buy_num,od.parent_num from order_detail od,order_slave os where os.order_id={0}", order_id);
            sql.AppendFormat(@" and	os.slave_id = od.slave_id and od.item_mode = 2 and od.pack_id ={0} and od.parent_id ={1} ", pack_id, detail_id);
            return _access.getDataTableForObj<OrderDetail>(sql.ToString());

        }
        public DataTable GetTotalCount(uint order_id, int type)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"SELECT count(order_detail.detail_id) total FROM ");
                sql.Append(@"order_detail INNER JOIN order_slave on order_detail.slave_id = order_slave.slave_id ");
                sql.Append(@"	WHERE order_detail.detail_status not in (89,90,91)");
                sql.AppendFormat(" and  order_slave.order_id ={0}", order_id);
                if (type == 2)
                {
                    sql.AppendFormat(" and order_detail.site_id = 7;");
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.GetTotalCount-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdatePriority(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_master set priority = 0 where order_id='{0}';", order_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.UpdatePriority-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateFirstTime(uint user_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update users set first_time = 0 where user_id='{0}';", user_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.UpdateFirstTime-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public DataTable IsRecommend(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select id from user_recommend where order_id='{0}';", order_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.IsRecommend-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateRecommend(int id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update user_recommend set is_recommend = 0 = 0 where id='{0}';", id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.UpdateRecommend-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public DataTable GetMoneyIDBySerial(string serial_sql)
        {
            try
            {
                return _access.getDataTable(serial_sql);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.GetMoneyIDBySerial-->" + ex.Message + serial_sql.ToString(), ex);
            }
        }

        public string InsertTransport(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("set sql_safe_updates = 0;update order_return_content set orc_deliver_date='{0}' ,orc_deliver_time='{1}',orc_deliver_code='{2}' where orc_order_id='{3}' and return_id='{4}' ;set sql_safe_updates=1;", CommonFunction.DateTimeToString(query.orc_deliver_date), query.orc_deliver_time, query.orc_deliver_code, query.orc_order_id,query.return_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.InsertTransport-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string UpdateORS(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into order_return_status(ors_order_id,ors_status,ors_remark,ors_createdate,ors_createuser,return_id) values('{0}','{1}','{2}','{3}','{4}','{5}');", query.ors_order_id, query.ors_status, query.ors_remark, CommonFunction.DateTimeToString(DateTime.Now), query.ors_createuser, query.return_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao.UpdateORS-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
        public DataTable GetOrderIdByReturnId(uint return_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select order_id from order_return_master where return_id='{0}'  and return_status='0';",return_id);
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->GetOrderIdByReturnId-->" + sql.ToString() + ex.Message, ex);
            }
        }

        //收到商品可退
        public string CouldReturnPurchase(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into order_return_status(ors_order_id,ors_status,ors_remark,ors_createdate,ors_createuser,return_id) values('{0}','{1}','{2}','{3}','{4}','{5}');", query.ors_order_id, query.ors_status, query.ors_remark, CommonFunction.DateTimeToString(DateTime.Now), query.ors_createuser, query.return_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->CouldReturn-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string UpdateORM(OrderReturnStatusQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_return_master  set ");
                if (query.invoice_deal == 0 && query.ormpackage != 0)
                {
                    sql.AppendFormat("   package='{0}' ",query.ormpackage);
                }
                else   if (query.invoice_deal != 0 && query.ormpackage == 0)
                {
                    sql.AppendFormat(" invoice_deal='{0}'  ", query.invoice_deal);
                }
                sql.AppendFormat(" where return_id='{0}'; ", query.return_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->UpdateORM-->" + sql.ToString() + ex.Message, ex);
            }
        }

        public string hg_batch_accumulate_refund(HgBatchAccumulateRefund query)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.AppendFormat("insert into hg_batch_accumulate_refund(order_id,head,card_no,card_checksum,enc_idno,");
                sql.AppendFormat("checksum,merchant_pos,terminal_pos,refund_point,category_id, ");
                sql.AppendFormat("order_note,wallet,batch_error_code,batch_status,");
                sql.AppendFormat("created_time,modified_time,billing_checked)");
                sql.AppendFormat("values('{0}','{1}','{2}','{3}','{4}',", query.order_id, query.head, query.card_no, query.card_checksum, query.enc_idno);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',",query.checksum,query.merchant_pos,query.terminal_pos,query.refund_point,query.category_id);
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", "吉甲地台灣好市集訂單編號" + query.order_id + "累點取消" + query.refund_point + "點", query.wallet, query.batch_error_code, query.batch_status);
                sql.AppendFormat("'{0}','{1}',{2});",CommonFunction.DateTimeToString(DateTime.Now),CommonFunction.DateTimeToString(DateTime.Now),query.billing_checked);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->hg_batch_accumulate_refund-->" + sql.ToString() + ex.Message, ex);
            }

            return sql.ToString();
        }
        /// <summary>
        /// hg_batch_deduct_refund表，hg_batch_accumulate_refund表結構一模一樣
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string hg_batch_deduct_refund(HgBatchAccumulateRefund query)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                sql.AppendFormat("insert into hg_batch_deduct_refund(order_id,head,card_no,card_checksum,enc_idno,");
                sql.AppendFormat("checksum,merchant_pos,terminal_pos,refund_point,category_id, ");
                sql.AppendFormat("order_note,wallet,batch_error_code,batch_status,");
                sql.AppendFormat("created_time,modified_time,billing_checked)");
                sql.AppendFormat("values('{0}','{1}','{2}','{3}','{4}',", query.order_id, query.head, query.card_no, query.card_checksum, query.enc_idno);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.checksum, query.merchant_pos, query.terminal_pos, query.refund_point, query.category_id);
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", "吉甲地台灣好市集訂單編號" + query.order_id + "返還" + query.refund_point + "點", query.wallet, query.batch_error_code, query.batch_status);
                sql.AppendFormat("'{0}','{1}',{2});", CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), query.billing_checked);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->hg_batch_deduct_refund-->" + sql.ToString() + ex.Message, ex);
            }

            return sql.ToString();
        }

        public HGLogin GetHGLoginData(uint order_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select order_id,merchant_pos,terminal_pos,response_code,response_message,");
                sql.AppendFormat("enc_idno,chk_sum,remain_point,token,mask_name,mask_id,transaction_time,createdAt  from hg_login where order_id='{0}';",order_id);
                return _access.getSinggleObj<HGLogin>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderReturnStatusDao-->GetHGLoginData-->" + sql.ToString() + ex.Message, ex);
            }
        }

        //public DataTable HgDeduct(uint order_id)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    try
        //    {
        //        sql.AppendFormat("select ");
        //    }
        //    catch (Exception ex)
        //    {
 
        //    }
        //}
    }
}
