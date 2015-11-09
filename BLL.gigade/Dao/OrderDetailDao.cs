/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderDetailDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 17:01:29 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Common;
using BLL.gigade.Model.Custom;


namespace BLL.gigade.Dao
{
    public class OrderDetailDao : IOrderDetailImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public OrderDetailDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public string Save(BLL.gigade.Model.OrderDetail orderDetail)
        {
            //edit by castle 2014/07/09
            //訂單匯入時，order_detail.event_cost請勿填寫，只需寫single_cost 若為組合也是，父項及子項的event_cost都不應寫入。
            orderDetail.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("insert into order_detail(`detail_id`,`slave_id`,`item_id`,`item_vendor_id`,`product_freight_set`,`product_mode`,`product_name`,`product_spec_name`,`single_cost`,");
            strSql.Append("`single_price`,`single_money`,`deduct_bonus`,`deduct_welfare`,`deduct_happygo`,`deduct_account`,`deduct_account_note`,");
            strSql.Append("`buy_num`,`detail_status`,`detail_note`,`item_code`,`channel_detail_id`,`combined_mode`,`item_mode`,`parent_id`,`sub_order_id`,`pack_id`,`parent_name`,`price_master_id`,`bag_check_money`,`parent_num`,`prepaid`,`site_id`,`event_cost`,`accumulated_bonus`)values({0},{1},");// edit by zhuoqin0830w  添加 活動成本參數  2015/03/24  Ahon林志鴻 說需要更改
            strSql.AppendFormat("{0},{1},{2},{3},", orderDetail.Item_Id, orderDetail.Item_Vendor_Id, orderDetail.Product_Freight_Set, orderDetail.Product_Mode);
            strSql.AppendFormat("'{0}','{1}','{2}',", orderDetail.Product_Name, orderDetail.Product_Spec_Name, orderDetail.Single_Cost);
            strSql.AppendFormat("{0},{1},{2},{3},{4},", orderDetail.Single_Price, orderDetail.Single_Money, orderDetail.Deduct_Bonus, orderDetail.Deduct_Welfare, orderDetail.Deduct_Happygo);
            strSql.AppendFormat("{0},'{1}',{2},{3},", orderDetail.Deduct_Account, orderDetail.Deduct_Account_Note, orderDetail.Buy_Num, orderDetail.Detail_Status);
            strSql.AppendFormat("'{0}','{1}','{2}',{3},{4},", orderDetail.Detail_Note, orderDetail.Item_Code, orderDetail.Channel_Detail_Id, orderDetail.Combined_Mode, orderDetail.item_mode);
            strSql.AppendFormat("{0},'{1}',{2},'{3}',{4},{5},", orderDetail.Parent_Id, orderDetail.Sub_Order_Id, orderDetail.pack_id, orderDetail.parent_name, orderDetail.price_master_id, orderDetail.Bag_Check_Money);
            strSql.AppendFormat("{0},{1},{2},{3},{4})", orderDetail.parent_num, orderDetail.Prepaid, orderDetail.Site_Id, orderDetail.Event_Cost, orderDetail.Accumulated_Bonus);  // edit by zhuoqin0830w  添加 活動成本參數  2015/03/24  Ahon林志鴻 說需要更改
            return strSql.ToString();
        }
        public int Delete(int detailId)
        {
            StringBuilder strSql = new StringBuilder("delete from order_detail where detail_id=" + detailId);
            return _dbAccess.execCommand(strSql.ToString());
        }
        public List<OrderDetail> QueryReturnDetail(uint return_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" select 	od.detail_id,od.single_money,od.deduct_bonus,od.buy_num,od.deduct_welfare,");
                sql.Append("	od.accumulated_bonus,od.accumulated_happygo,od.deduct_happygo,od.deduct_happygo_money ");
                sql.Append("from	order_detail od left join 	order_return_detail ord on od.detail_id=ord.detail_id ");
                sql.AppendFormat(" where ord.return_id='{0}'", return_id);
                return _dbAccess.getDataTableForObj<OrderDetail>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.QueryReturnDetail -->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 獲取訂單內容的訂單列表
        /// add by shuangshuang0420j 20141029 11:00:00
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<Model.Query.OrderDetailQuery> GetOrderDetailList(Model.Query.OrderDetailQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@"  select od.detail_note,od.detail_id,od.slave_id,od.item_id,od.item_vendor_id,od.product_freight_set,od.product_mode,od.product_spec_name,od.single_cost,
od.single_cost,od.event_cost,od.single_price,od.single_money,od.deduct_bonus,od.deduct_welfare,od.deduct_happygo,od.deduct_happygo_money,od.deduct_account,od.deduct_account_note,od.accumulated_bonus,od.accumulated_happygo,od.buy_num,od.parent_num ,od.detail_status,od.combined_mode,od.item_mode,od.parent_id,od.product_name,od.price_master_id,od.site_id,os.order_id,os.slave_freight_normal,os.slave_freight_low,os.slave_product_subtotal,os.slave_amount,os.slave_status,os.vendor_id,os.slave_note,os.slave_date_delivery,os.slave_date_cancel,os.slave_date_return,os.slave_date_close,FROM_UNIXTIME( os.slave_date_close) as clos_date,v.vendor_name_simple,v.dispatch,od.product_mode,os.account_status,p.tax_type,vb.brand_name,tps.remark as slave_status_str,ds_tps.remark as detail_status_str, pf_tps.parameterName as product_freight_set_str,pm_tps.parameterName as Product_Mode_Name,pit.product_id,s.site_name as channel_name_simple,v.vendor_email,v.vendor_name_simple,v.company_phone,v.company_person  ");
                sqlCondi.Append(" FROM order_master om left join order_slave os ON om.order_id=os.order_id ");
                sqlCondi.Append(" left join order_detail od on  os.slave_id=od.slave_id");
                sqlCondi.Append(" left join vendor v on os.vendor_id=v.vendor_id");
                sqlCondi.Append(" left join product_item pit on pit.item_id=od.item_id");
                sqlCondi.Append(" left join product p on pit.product_id=p.product_id");
                sqlCondi.Append(" left join vendor_brand vb on p.brand_id=vb.brand_id");
                sqlCondi.Append(" left join site s on s.site_id=od.site_id");
                sqlCondi.Append(" left join (SELECT parameterType,parameterCode,remark from t_parametersrc where parameterType='order_status' ) tps on tps.parameterCode=os.slave_status ");
                sqlCondi.Append(" left join (SELECT parameterType,parameterCode,remark from t_parametersrc where parameterType='order_status' ) ds_tps on ds_tps.parameterCode=od.detail_status ");
                sqlCondi.Append(" left join (SELECT parameterName,parameterCode,remark from t_parametersrc where parameterType='product_freight' ) pf_tps on pf_tps.parameterCode=od.product_freight_set ");
                sqlCondi.Append(" left join (select parameterCode,parameterName,remark from t_parametersrc where parametertype='product_mode') pm_tps on od.product_mode=pm_tps.parameterCode");
                sqlCondi.Append(" LEFT JOIN channel c ON om.channel=c.channel_id ");
                sqlCondi.Append(" where 1=1 ");
                if (query.Order_Id != 0)
                {
                    sqlCondi.AppendFormat(" and os.order_id='{0}' ", query.Order_Id);
                }
                if (query.Slave_Id != 0)
                {
                    sqlCondi.AppendFormat(" and os.slave_id='{0}' ", query.Slave_Id);
                }
                //sqlCondi.AppendFormat(" where os.order_id ='{0}'", query.Order_Id);
                //訂單內容中用於是否用item_mode作為條件判斷的依據，無其他意義。-1：不做判斷，0：非子商品即item_mode!=2,1是子商品，item_mode=2
                if (query.isChildItem != -1)
                {
                    if (query.isChildItem == 0)
                    {
                        sqlCondi.Append(" and od.item_mode!=2 ");
                    }
                    else if (query.isChildItem == 1)
                    {
                        sqlCondi.Append(" and od.item_mode=2 ");
                        if (query.Parent_Id != 0)
                        {
                            sqlCondi.AppendFormat(" and od.parent_id='{0}'", query.Parent_Id);
                        }
                        if (query.pack_id != 0)
                        {
                            sqlCondi.AppendFormat(" and od.pack_id='{0}'", query.pack_id);
                        }
                    }
                }
                sqlCondi.Append(" order by od.slave_id asc,od.product_freight_set asc, od.detail_id asc ,od.item_id asc");

                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable("select count(*) as totalCount " + sqlCondi.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlCondi.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                }
                return _dbAccess.getDataTableForObj<Model.Query.OrderDetailQuery>(sql.ToString() + sqlCondi.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.GetOrderDetailList -->" + ex.Message + sql.ToString() + sqlCondi.ToString(), ex);
            }
        }
        #region 傳票明細
        /// <summary>
        /// 出貨管理>批次出貨編號:檢索>傳票明細
        /// chaojie1124j添加于 2014/12/15 05:32 PM 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<BLL.gigade.Model.Query.OrderDetailQuery> SubPoenaDetail(BLL.gigade.Model.Query.OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {//
                sql.Append(@" select dm.order_id as order_num,dm.deliver_id as Deliver_Id,dm.delivery_name as Delivery_Name,");
                sql.Append("  od.product_name as Product_Name,od.product_spec_name as Product_Spec_Name,od.buy_num as Buy_Num, ");
                sql.Append(" od.combined_mode as Combined_Mode,od.item_mode,od.parent_id, od.item_id as Item_Id, ");
                sql.Append("  od.detail_id as Detail_Id,od.pack_id ,od.parent_num,od.detail_status as Detail_Status,od.product_freight_set as Product_Freight_Set ");
                sql.Append(" from deliver_master dm LEFT JOIN  ticket t on  t.ticket_id=dm.ticket_id ");
                sql.Append(" left join deliver_detail dd on dd.deliver_id=dm.deliver_id left join order_detail od on od.detail_id=dd.detail_id ");
                sql.AppendFormat(" where 1=1 and  dm.ticket_id='{0}' ", query.Ticket_Id);
                sql.Append(" ORDER BY dm.order_id,dm.deliver_id asc ");

                return _dbAccess.getDataTableForObj<Model.Query.OrderDetailQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.SubPoenaDetail -->" + ex.Message + sql.ToString(), ex);
            }

        }
        /// <summary>
        /// 傳票明細,未到貨。
        /// chaojie1124j添加于 2014/12/17 05:06 PM 
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool no_delivery(BLL.gigade.Model.Query.OrderDetailQuery query)
        {//www/Model/Deliver/第532行 no_delivery
            StringBuilder sb = new StringBuilder();
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

                sb.AppendFormat(@"update order_detail set detail_status=2 where detail_id='{0}';", query.Detail_Id);//待出貨
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                sb.AppendFormat(@" select slave_id from order_detail where detail_id='{0}';", query.Detail_Id);
                mySqlCmd.CommandText = sb.ToString();
                string s = mySqlCmd.ExecuteScalar().ToString();
                int slave_id = int.Parse(mySqlCmd.ExecuteScalar().ToString());
                sb.Clear();
                sb.AppendFormat(@"update order_slave set slave_status=2 where slave_id='{0}';", slave_id);//待出貨
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                sb.AppendFormat(@"update deliver_detail set delivery_status=5 where detail_id='{0}';", query.Detail_Id);//未到貨
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderDetailDao.no_delivery -->" + ex.Message + sb.ToString(), ex);
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
        /// 傳票明細,拆分細項。
        /// chaojie1124j添加于 2014/12/17 06:10 PM 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int split_detail(BLL.gigade.Model.Query.OrderDetailQuery query)
        {//www/Model/Deliver/第575行 split_detail
            StringBuilder sb = new StringBuilder();
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
                OrderDetailQuery orderdetail = new OrderDetailQuery();//
                sb.Append(@"select detail_id,slave_id,item_id,item_vendor_id,product_freight_set,product_mode,product_name,product_spec_name,single_cost,");
                sb.Append(" event_cost,single_price,single_money,deduct_bonus,deduct_welfare,deduct_happygo,deduct_happygo_money,deduct_account,");
                sb.Append(" deduct_account_note,accumulated_bonus,accumulated_happygo,buy_num,detail_status,detail_note,item_code,arrival_status,delay_till,");
                sb.Append(" lastmile_deliver_serial,lastmile_deliver_datetime,lastmile_deliver_agency,bag_check_money,channel_detail_id,combined_mode,");
                sb.Append(" item_mode,parent_id,parent_name,parent_num,price_master_id,pack_id,sub_order_id,site_id,event_id,prepaid from order_detail");
                sb.AppendFormat(" where detail_id='{0}';", query.Detail_Id);
                mySqlCmd.CommandText = sb.ToString();
                orderdetail = _dbAccess.getSinggleObj<OrderDetailQuery>(sb.ToString());
                sb.Clear();
                uint Detail_Id = query.Detail_Id;
                if (orderdetail.Buy_Num < 2)//不可拆分
                    return 0;

                #region 拆分商品，并修改數據
                uint buynum = orderdetail.Buy_Num;
                orderdetail.Buy_Num = buynum - 1;
                uint Deduct_Bonus = orderdetail.Deduct_Bonus;
                uint Deduct_Welfare = orderdetail.Deduct_Welfare;
                int Deduct_Happygo = orderdetail.Deduct_Happygo;
                int Accumulated_Bonus = orderdetail.Accumulated_Bonus;
                int Accumulated_Happygo = orderdetail.Accumulated_Happygo;

                orderdetail.Deduct_Bonus = uint.Parse(Math.Round((decimal)(orderdetail.Deduct_Bonus / orderdetail.Buy_Num * (buynum - 1)), 0, MidpointRounding.AwayFromZero).ToString());
                orderdetail.Deduct_Welfare = uint.Parse(Math.Round((decimal)(orderdetail.Deduct_Welfare / orderdetail.Buy_Num * (buynum - 1)), 0, MidpointRounding.AwayFromZero).ToString());
                orderdetail.Deduct_Happygo = int.Parse(Math.Round((decimal)(orderdetail.Deduct_Happygo / orderdetail.Buy_Num * (buynum - 1)), 0, MidpointRounding.AwayFromZero).ToString());
                orderdetail.Accumulated_Bonus = int.Parse(Math.Round((decimal)(orderdetail.Accumulated_Bonus / orderdetail.Buy_Num * (buynum - 1)), 0, MidpointRounding.AwayFromZero).ToString());
                orderdetail.Accumulated_Happygo = int.Parse(Math.Round((decimal)(orderdetail.Accumulated_Happygo / orderdetail.Buy_Num * (buynum - 1)), 0, MidpointRounding.AwayFromZero).ToString());
                //保存
                mySqlCmd.CommandText = UpOrderDetail(orderdetail);//601行
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                #endregion
                #region 拆分出來的，賦值并保存


                sb.Append(" select serial_value from serial where serial_id=32;");
                mySqlCmd.CommandText = sb.ToString();
                orderdetail.Detail_Id = uint.Parse(mySqlCmd.ExecuteScalar().ToString()) + 1;
                orderdetail.Buy_Num = 1;
                orderdetail.Deduct_Bonus = Deduct_Bonus - orderdetail.Deduct_Bonus;
                orderdetail.Deduct_Welfare = Deduct_Welfare - orderdetail.Deduct_Welfare;
                orderdetail.Deduct_Happygo = Deduct_Happygo - orderdetail.Deduct_Happygo;
                orderdetail.Accumulated_Bonus = Accumulated_Bonus - orderdetail.Accumulated_Bonus;
                orderdetail.Accumulated_Happygo = Accumulated_Happygo - orderdetail.Accumulated_Happygo;
                mySqlCmd.CommandText = AddOrderDetail(orderdetail);//610行
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                #endregion
                ///INSERT INTO deliver_detail (deliver_id, detail_id) SELECT deliver_id, 612849 FROM deliver_detail AS `DeliverDetail`   WHERE `detail_id` = 152551 
                ///
                #region 增加記錄deliver_detail
                sb.AppendFormat(@"INSERT INTO deliver_detail (deliver_id, detail_id,delivery_status) SELECT deliver_id, '{0}','{1}' FROM  deliver_detail ", orderdetail.Detail_Id, 0);
                sb.AppendFormat(@" where detail_id='{0}';", Detail_Id);

                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                #endregion
                mySqlCmd.Transaction.Commit();
                return 1;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderDetailDao.no_delivery -->" + ex.Message + sb.ToString(), ex);
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
        /// 返回新增的sql 语句
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string AddOrderDetail(OrderDetailQuery query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder(@"insert into order_detail (detail_id,slave_id,item_id,item_vendor_id,product_freight_set,product_mode,product_name,product_spec_name,single_cost,");
            strSql.Append("event_cost,single_price,single_money,deduct_bonus,deduct_welfare,deduct_happygo,deduct_happygo_money,deduct_account,deduct_account_note,");

            strSql.Append("accumulated_bonus,accumulated_happygo,buy_num,detail_status,detail_note,item_code,arrival_status,delay_till,lastmile_deliver_serial,");
            strSql.Append("lastmile_deliver_datetime,lastmile_deliver_agency,bag_check_money,channel_detail_id,combined_mode,item_mode,parent_id,parent_name,parent_num,");
            strSql.Append("price_master_id,pack_id,sub_order_id,site_id,event_id,prepaid) values(");
            strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.Detail_Id, query.Slave_Id, query.Item_Id, query.Item_Vendor_Id, query.Product_Freight_Set);
            strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.Product_Mode, query.Product_Name, query.Product_Spec_Name, query.Single_Cost, query.Event_Cost);
            strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.Single_Price, query.Single_Money, query.Deduct_Bonus, query.Deduct_Welfare, query.Deduct_Happygo);
            strSql.AppendFormat("'{0}','{1}','{2}','{3}',", query.Deduct_Happygo_Money, query.Deduct_Account, query.Deduct_Account_Note, query.Accumulated_Bonus);
            strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.Accumulated_Happygo, query.Buy_Num, query.Detail_Status, query.Detail_Note, query.Item_Code);
            strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.Arrival_Status, query.Delay_Till, query.Lastmile_Deliver_Serial, query.Lastmile_Deliver_Datetime, query.Lastmile_Deliver_Agency);
            strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.Bag_Check_Money, query.Channel_Detail_Id, query.Combined_Mode, query.item_mode, query.Parent_Id);
            strSql.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", query.parent_name, query.parent_num, query.price_master_id, query.pack_id, query.Sub_Order_Id);
            strSql.AppendFormat("'{0}','{1}','{2}');", query.Site_Id, query.event_id, query.Prepaid);
            return strSql.ToString();
        }
        /// <summary>
        /// 修改reder_dail 表中的所有数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string UpOrderDetail(OrderDetailQuery query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder(@"update order_detail set  ");
            strSql.AppendFormat(" slave_id='{0}',item_id='{1}',item_vendor_id='{2}',product_freight_set='{3}',", query.Slave_Id, query.Item_Id, query.Item_Vendor_Id, query.Product_Freight_Set);
            strSql.AppendFormat(" product_mode='{0}',single_price='{1}',product_spec_name='{2}',single_cost='{3}',", query.Product_Mode, query.Single_Price, query.Product_Spec_Name, query.Single_Cost);
            strSql.AppendFormat(" event_cost='{0}',product_name='{1}',single_money='{2}',deduct_bonus='{3}',", query.Event_Cost, query.Product_Name, query.Single_Money, query.Deduct_Bonus);
            strSql.AppendFormat(" deduct_welfare='{0}',deduct_happygo='{1}',deduct_happygo_money='{2}',", query.Deduct_Welfare, query.Deduct_Happygo, query.Deduct_Happygo_Money);
            strSql.AppendFormat(" deduct_account='{0}',deduct_account_note='{1}',accumulated_bonus='{2}',", query.Deduct_Account, query.Deduct_Account_Note, query.Accumulated_Bonus);
            strSql.AppendFormat(" accumulated_happygo='{0}',buy_num='{1}',detail_status='{2}',", query.Accumulated_Happygo, query.Buy_Num, query.Detail_Status);
            strSql.AppendFormat(" delay_till='{0}',lastmile_deliver_serial='{1}',lastmile_deliver_datetime='{2}',", query.Delay_Till, query.Lastmile_Deliver_Serial, query.Lastmile_Deliver_Datetime);
            strSql.AppendFormat(" lastmile_deliver_agency='{0}',bag_check_money='{1}',channel_detail_id='{2}',", query.Lastmile_Deliver_Agency, query.Bag_Check_Money, query.Channel_Detail_Id);
            strSql.AppendFormat(" combined_mode='{0}',item_mode='{1}',parent_id='{2}',parent_name='{3}',", query.Combined_Mode, query.item_mode, query.Parent_Id, query.parent_name);
            strSql.AppendFormat(" parent_num='{0}',price_master_id='{1}',pack_id='{2}',sub_order_id='{3}',", query.parent_num, query.price_master_id, query.pack_id, query.Sub_Order_Id);
            strSql.AppendFormat(" site_id='{0}',event_id='{1}',prepaid='{2}' ", query.Site_Id, query.event_id, query.Prepaid);
            strSql.AppendFormat(" where detail_id='{0}';", query.Detail_Id);
            return strSql.ToString();
        }
        #endregion

        #region 批次檢貨明細列印
        /// <summary>
        /// chaojie_zz添加於2014/12/24 09:59
        /// 用於實現供應商調度出貨，顯示一個或多個批次檢貨單下面的商品
        /// </summary>
        /// <param name="query">實體，通過某些字段來查詢數據</param>
        /// <param name="sqlappend">新增的sql 語句，多個slave_id</param>
        /// <returns></returns>
        public List<Model.Query.OrderDetailQuery> VendorPickPrint(Model.Query.OrderDetailQuery query, out int totalCount, string sqlappend = null)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@" SELECT  od.slave_id,od.detail_id,od.parent_name, od.item_id,od.product_name,od.product_freight_set ");
                sql.Append(" ,od.buy_num,od.single_price,od.parent_num,od.item_code,od.combined_mode ");
                sql.Append(" ,om.order_id,om.order_name,om.delivery_name, om.note_order,od.item_mode ");
                sql.Append("  FROM  order_detail od,order_slave os,order_master om ");
                sql.Append(" WHERE os.slave_id = od.slave_id  AND os.order_id = om.order_id ");
                sql.Append(" AND od.detail_status = 2 AND od.product_mode IN (2,3) AND os.slave_status = 2 ");
                if (query.Vendor_Id != 0)
                {
                    sql.AppendFormat(" and  os.vendor_id='{0}' ", query.Vendor_Id);
                }
                if (!string.IsNullOrEmpty(sqlappend))
                {
                    sql.Append(sqlappend);
                }

                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;
                    }
                }
                return _dbAccess.getDataTableForObj<Model.Query.OrderDetailQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.VendorPickPrint -->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 這個是確認出貨時候顯示的貨物信息，需要分頁的
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <param name="sqlappend"></param>
        /// <returns></returns>
        public List<Model.Query.OrderDetailQuery> AllOrderDeliver(Model.Query.OrderDetailQuery query, out int totalCount, string sqlappend = null)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@" SELECT  od.slave_id,od.detail_id,od.parent_name, od.item_id,od.product_name,od.product_freight_set ");
                sql.Append(" ,od.buy_num,od.single_price,od.parent_num,od.item_code,od.combined_mode,od.item_mode ");
                sql.Append("  FROM  order_detail od,order_slave os ");
                sql.Append(" WHERE os.slave_id = od.slave_id  ");
                sql.Append(" AND od.detail_status = 2 AND od.product_mode IN (2,3) AND os.slave_status = 2 ");
                if (query.Vendor_Id != 0)
                {
                    sql.AppendFormat(" and  os.vendor_id='{0}' ", query.Vendor_Id);
                }
                if (!string.IsNullOrEmpty(sqlappend))
                {
                    sql.Append(sqlappend);
                }

                totalCount = 0;
                if (query.IsPage)
                {
                    System.Data.DataTable _dt = _dbAccess.getDataTable(sql.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = _dt.Rows.Count;

                    }
                }
                return _dbAccess.getDataTableForObj<Model.Query.OrderDetailQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.AllOrderDeliver -->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 供應商後台>調度出貨
        /// <summary>
        /// 計算出貨商品的各個價格
        /// </summary>
        /// <returns></returns>
        public List<OrderDetailQuery> GetOrderDetailToSum(uint vendor_id, long time)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string t = GetOrderDetailSql(vendor_id, time);
                sb.AppendFormat(@" select od.item_mode,od.product_freight_set,od.single_price,od.parent_num,od.buy_num");
                sb.AppendFormat(" FROM	order_detail od,order_slave os WHERE	os.vendor_id = '{0}' ", vendor_id);
                sb.AppendFormat(" 	AND	os.slave_id = od.slave_id   AND os.slave_id in ({0}) ", GetOrderDetailSql(vendor_id, time));
                sb.AppendFormat("  ORDER BY od.slave_id ASC , combined_mode ASC , item_mode ASC; ");
                return _dbAccess.getDataTableForObj<OrderDetailQuery>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.GetOrderDetailList -->" + ex.Message + sb.ToString(), ex);
            }

        }

        public string GetOrderDetailSql(uint vendor_id, long endtime)
        {
            long starttime = endtime - 86400;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@" select 	os.slave_id 	FROM	order_slave os, order_master om ");
            sb.AppendFormat("  WHERE	os.vendor_id ='{0}' AND	os.order_id = om.order_id  AND om.order_date_pay >='{1}' ", vendor_id, starttime);
            sb.AppendFormat(" AND om.order_date_pay <= '{0}' ORDER BY om.order_date_pay DESC ", endtime);
            //List<OrderSlave> query1 = new List<OrderSlave>();
            //string result = "";
            //query1 = _dbAccess.getDataTableForObj<OrderSlave>(sb.ToString());
            //for (int i = 0; i < query1.Count; i++)
            //{
            //    result += query1[i].Slave_Id;
            //    if (i == query1.Count - 1)
            //    {
            //        return result;
            //    }
            //    result += ",";
            //}
            return sb.ToString();
        }


        #endregion

        #region 供應商後台>自行出貨列表>出貨:顯示要出貨物的信息功能
        public List<OrderDetailQuery> DeliveryInformation(OrderDetailQuery query, String str = null)
        {
            StringBuilder sqlclomn = new StringBuilder();//要查詢的列
            StringBuilder sqlcondition = new StringBuilder();//查詢條件
            StringBuilder sqltable = new StringBuilder(); //要連接的表
            StringBuilder sqlgroupby = new StringBuilder();//聚合條件
            StringBuilder sqlsort = new StringBuilder();//排序條件
            string sql = "";
            try
            {
                sqlclomn.Append(@"SELECT od.detail_id,od.product_name,od.product_freight_set,od.product_spec_name,od.single_money ");
                sqlclomn.Append(",od.parent_name,od.parent_num,combined_mode,od.item_mode,od.buy_num ");
                sqltable.Append(" FROM order_detail od ");
                sqltable.Append(" LEFT JOIN order_slave os on od.slave_id=os.slave_id ");
                sqltable.Append(" LEFT JOIN vendor v on v.vendor_id=os.vendor_id ");
                sqlcondition.Append(" where 1=1 ");
                sqlcondition.AppendFormat(" and os.order_id = ( select order_id from order_slave where slave_id='{0}') ", query.Slave_Id);
                sqlcondition.AppendFormat(" and  os.slave_id ='{0}'  ", query.Slave_Id);
                sqlcondition.Append(" and od.product_mode in (1)  ");
                sqlcondition.Append(" and  od.detail_status = 2 ");//待出貨；
                sqlsort.Append(" ORDER BY os.slave_id asc ,product_freight_set asc, combined_mode ASC,item_mode asc ");
                if (query.Vendor_Id != 0)
                {
                    sqlcondition.AppendFormat(" and os.vendor_id='{0}' ", query.Vendor_Id);
                }
                if (!string.IsNullOrEmpty(str))
                {
                    sqlcondition.Append(str);
                }
                sql = sqlclomn.ToString() + sqltable.ToString() + sqlcondition.ToString() + sqlsort.ToString();


            }
            catch (Exception ex)
            {

                throw new Exception("OrderDetailDao.DeliveryInformation -->" + ex.Message + sqlclomn.ToString(), ex);
            }

            return _dbAccess.getDataTableForObj<OrderDetailQuery>(sql);
        }

        #endregion

        public DataTable GetLeaveproductList(OrderDetailQuery query, out int totalCount, string sqlappend = null)
        {
            StringBuilder sqlcount = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlstr = new StringBuilder();
            StringBuilder limitpage = new StringBuilder();
            totalCount = 0;
            try
            {
                sqlcount.AppendFormat("select os.order_id ");
                sql.AppendFormat(@"SELECT os.order_id,od.detail_id
                                    ,CONCAT(od.product_name,od.product_spec_name) as product_spec_name_new,
                                    tp.remark as state,
                                    CASE od.item_mode WHEN 2 THEN od.buy_num*od.parent_num ELSE od.buy_num END as number_count,
                                    CASE os.slave_date_delivery when 0 THEN '未出貨' ELSE FROM_UNIXTIME(os.slave_date_delivery,'%Y/%m/%d %H:%i:%s') END as chuhuoriqi,
                                    CASE od.item_mode  WHEN 0 THEN '單一' 
                                    WHEN 1 THEN '父商品'
                                    ELSE '子商品' END AS  comboxtype");
                sqlstr.AppendFormat(@" FROM order_detail od RIGHT JOIN order_slave os on od.slave_id=os.slave_id LEFT JOIN t_parametersrc tp on od.detail_status=tp.parameterCode 
                                    where tp.parameterType = 'order_status' and item_vendor_id = '{0}'
			AND  od.product_mode = 2 			
			AND	od.slave_id = os.slave_id AND od.detail_status in (2,4,5,6,9) ", query.Vendor_Id);
                sqlstr.AppendFormat(sqlappend.ToString());
                sqlstr.AppendFormat("ORDER BY od.detail_id DESC ");
                limitpage.AppendFormat("limit {0},{1}; ", query.Start, query.Limit);
                totalCount = _dbAccess.getDataTable(sqlcount.ToString() + sqlstr.ToString()).Rows.Count;
                return _dbAccess.getDataTable(sql.ToString() + sqlstr.ToString() + limitpage.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.GetLeaveproductList -->" + ex.Message + sql.ToString(), ex);
            }
        }

        public List<OrderDetailQuery> OrderVendorProducesList(OrderDetailQuery query, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCount = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();

            totalCount = 0;
            try
            {
                sqlCount.Append(" SELECT count(om.order_id ) as totalCount ");
                sql.Append(" select os.order_id,v.vendor_name_simple,od.product_name,img.spec_image,od.product_spec_name,od.buy_num ,om.order_name,os.slave_status,para.remark as slave_status_string ,od.product_mode,pramode.remark as product_mode_string,os.slave_date_delivery,om.note_order,od.item_mode,od.parent_num  ");
                sqlFrom.Append("from order_detail od LEFT JOIN order_slave os on os.slave_id=od.slave_id LEFT JOIN order_master om on os.order_id=om.order_id LEFT JOIN vendor v on v.vendor_id=os.vendor_id ");
                sqlFrom.Append(" left JOIN (select parameterCode,remark from t_parametersrc where parameterType='order_status') para ON para.parameterCode=os.slave_status  ");
                sqlFrom.Append("LEFT JOIN (select parameterCode,remark from t_parametersrc where parameterType='product_mode') pramode on pramode.parameterCode=od.product_mode  ");
                sqlFrom.Append(" LEFT JOIN (SELECT pim.item_id,pim.spec_id_1 , psc.spec_image FROM	product_item  pim left join product_spec psc ON	pim.spec_id_1 = psc.spec_id) img on img.item_id=od.item_id  ");
                sqlWhere.AppendFormat("where 1=1 and os.vendor_id={0}   ", query.Vendor_Id);
                if (query.promodel == 1)
                {
                    sqlWhere.AppendFormat(" and od.product_mode in  (1) ");
                }
                else if (query.promodel == 2)
                {
                    sqlWhere.AppendFormat(" and od.product_mode in  (2,3) ");
                }
                //日期區間
                if (query.date != 0)
                {
                    sqlWhere.AppendFormat(" and om.order_date_pay>='{0}' and om.order_date_pay<='{1}'  ", CommonFunction.GetPHPTime(query.start_time.ToString()), CommonFunction.GetPHPTime(query.end_time.ToString()));
                }
                //關鍵字查詢
                if (query.select_type != 0)
                {
                    sqlWhere.AppendFormat(" and od.product_name LIKE '%{0}%'  ", query.select_con);
                }
                //訂單狀態
                if (query.radiostatus == 4)
                {
                    sqlWhere.Append(" and os.slave_status in (4,6,7) ");
                }
                else if (query.radiostatus != -1)
                {
                    sqlWhere.AppendFormat(" and os.slave_status ={0} ", query.radiostatus);
                }
                else
                {
                    sqlWhere.Append(" and os.slave_status in (2,4,99,6,7) ");
                }
                if (query.IsPage)
                {
                    DataTable _dt = _dbAccess.getDataTable(sqlCount.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                    }
                    sqlWhere.AppendFormat("  ORDER BY  os.slave_date_delivery DESC,om.order_createdate DESC  limit {0},{1};", query.Start, query.Limit);
                }
                return _dbAccess.getDataTableForObj<OrderDetailQuery>(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao-->OrderVendorProducesList -->" + ex.Message + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString(), ex);
            }
        }

        public DataTable ProduceGroupCsv(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sql.Append("select od.item_id,od.product_name,sum(od.buy_num) as total ");
                sqlFrom.Append(" from order_detail od ");
                sqlFrom.Append(" LEFT JOIN order_slave os on os.slave_id=od.slave_id  ");
                sqlFrom.Append(" left JOIN vendor v on  v.vendor_id=os.vendor_id  ");
                sqlFrom.Append(" LEFT JOIN order_master  om on os.order_id =om.order_id  ");
                //  sqlFrom.Append("");
                sqlWhere.AppendFormat(" where od.item_mode!=1 and os.vendor_id={0}   ", query.Vendor_Id);
                if (query.date != 0)
                {
                    sqlWhere.AppendFormat(" and om.order_createdate>'{0}' and om.order_createdate<='{1}'  ", CommonFunction.GetPHPTime(query.start_time.ToString()), CommonFunction.GetPHPTime(query.end_time.ToString()));
                }
                //關鍵字查詢
                if (query.select_type != 0)
                {
                    sqlWhere.AppendFormat(" and od.product_name LIKE '%{0}%'  ", query.select_con);
                }
                //訂單狀態
                if (query.radiostatus != -1)
                {
                    sqlWhere.AppendFormat(" and os.slave_status ={0} ", query.radiostatus);
                }
                else
                {
                    sqlWhere.Append(" and os.slave_status in (0,2,4,99) ");
                }
                sqlWhere.Append(" GROUP BY od.item_id; ");
                return _dbAccess.getDataTable(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao-->ProduceGroupCsv -->" + ex.Message + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString(), ex);
            }
        }

        public DataTable ProduceGroupExcel(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlFrom = new StringBuilder();
            StringBuilder sqlWhere = new StringBuilder();
            try
            {
                sql.Append(" select os.order_id,v.vendor_name_simple,od.product_name,od.product_spec_name,od.buy_num ,om.order_name,para.remark as slave_status_string ,os.slave_date_delivery,om.note_order,img.spec_image,os.slave_status,od.product_mode,pramode.remark as product_mode_string,od.item_mode,od.parent_num  ");
                sqlFrom.Append("from order_detail od LEFT JOIN order_slave os on os.slave_id=od.slave_id LEFT JOIN order_master om on os.order_id=om.order_id LEFT JOIN vendor v on v.vendor_id=os.vendor_id ");
                sqlFrom.Append(" left JOIN (select parameterCode,remark from t_parametersrc where parameterType='order_status') para ON para.parameterCode=os.slave_status  ");
                sqlFrom.Append("LEFT JOIN (select parameterCode,remark from t_parametersrc where parameterType='product_mode') pramode on pramode.parameterCode=od.product_mode  ");
                sqlFrom.Append(" LEFT JOIN (SELECT pim.item_id,pim.spec_id_1 , psc.spec_image FROM	product_item  pim left join product_spec psc ON	pim.spec_id_1 = psc.spec_id) img on img.item_id=od.item_id  ");
                sqlWhere.AppendFormat("where 1=1 and os.vendor_id={0}  ", query.Vendor_Id);
                //日期區間
                if (query.date != 0)
                {
                    sqlWhere.AppendFormat(" and om.order_date_pay>'{0}' and om.order_date_pay<='{1}'  ", CommonFunction.GetPHPTime(query.start_time.ToString()), CommonFunction.GetPHPTime(query.end_time.ToString()));
                }
                //關鍵字查詢
                if (query.select_type != 0)
                {
                    sqlWhere.AppendFormat(" and od.product_name LIKE '%{0}%'  ", query.select_con);
                }
                //訂單狀態

                if (query.radiostatus != -1)
                {
                    sqlWhere.AppendFormat(" and os.slave_status ={0} ", query.radiostatus);
                }
                else
                {
                    sqlWhere.Append(" and os.slave_status in (0,2,4,99) ");
                }
                sqlWhere.AppendFormat("  ORDER BY  om.order_createdate DESC limit 65500 ;");
                return _dbAccess.getDataTable(sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao-->ProduceGroupExcel -->" + ex.Message + sql.ToString() + sqlFrom.ToString() + sqlWhere.ToString(), ex);
            }
        }

        #region 查詢購買金額與應稅類型
        /// <summary>
        /// 查詢購買金額與應稅類型
        /// </summary>
        /// <param name="query">查詢條件</param>
        /// <returns>購買金額與應稅類型列表</returns>
        public DataTable GetBuyAmountAndTaxType(OrderDetailQuery query)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("SELECT SUM(od.buy_num * od.single_money) as amount,tax_type ");
            sbSql.Append(" FROM order_detail od ");
            sbSql.Append(" INNER JOIN product_item pit USING(item_id) ");
            sbSql.Append(" INNER JOIN product  p USING(product_id) ");
            sbSql.Append(" INNER JOIN order_slave os USING(slave_id) ");
            sbSql.Append(" INNER JOIN order_master om USING(order_id) ");
            sbSql.Append(" where 1=1 ");
            sbSql.AppendFormat(" and om.order_id = {0} ", query.Order_Id);
            sbSql.Append(" AND od.item_mode in (0,1) ");
            sbSql.Append(" group by tax_type ");
            try
            {
                return _dbAccess.getDataTable(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao-->GetBuyAmountAndTaxType-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        #endregion

        /// add by wwei0216w 2015/5/20 獲得運送達天數以付款時間
        public List<OrderDetailCustom> GetArriveDay(uint detail_id)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                ///
                sb.AppendFormat(@"SELECT od.item_id,om.order_date_pay,od.item_mode,od.parent_id AS parentId
	                                  FROM order_detail od
                                      INNER JOIN order_slave os ON os.slave_id = od.slave_id
                                      INNER JOIN order_master om ON om.order_id = os.order_id
                                  WHERE od.detail_id = {0}", detail_id);

                return _dbAccess.getDataTableForObj<OrderDetailCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ProductDao-->GetArriveDay" + ex.Message, ex);
            }
        }

        public List<OrderDetailQuery> OrderDetail(uint return_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" select 	od.detail_id,od.slave_id,od.buy_num,od.item_mode,od.parent_id,od.pack_id ");
                sql.Append("from	order_detail od left join 	order_return_detail ord on od.detail_id=ord.detail_id ");
                sql.AppendFormat(" where ord.return_id='{0}'", return_id);
                return _dbAccess.getDataTableForObj<OrderDetailQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.QueryReturnDetail -->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdateOrderDetailSome(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_detail set detail_status=4 where slave_id='{0}' and parent_id='{1}' and pack_id='{2}'; ",query.Slave_Id,query.Parent_Id,query.pack_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.UpdateOrderDetailSome -->" + ex.Message + sql.ToString(), ex);
            }
        }

        public string UpdateOrderDetail(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update order_detail set detail_status=4 where detail_id='{0}'; ", query.Detail_Id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.UpdateOrderDetail -->" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable OrderDetailTable(uint return_id,uint item_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(" select 	od.detail_id,	od.single_money,od.deduct_bonus,od.buy_num,od.deduct_welfare,od.accumulated_bonus,od.accumulated_happygo,od.deduct_happygo,od.deduct_happygo_money,od.item_id,od.item_mode,od.parent_id,od.pack_id ");
                sql.Append("from	order_detail od left join 	order_return_detail ord on od.detail_id=ord.detail_id ");
                sql.AppendFormat(" where ord.return_id='{0}' ", return_id);
                if (item_id!=0)
                {
                    sql.AppendFormat("and od.item_id='{0}';", item_id);
                }

                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.OrderDetail -->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 品牌營業額統計的數據
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<OrderDetailQuery> GetOrderDetailList(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(" select od.single_money,od.buy_num,od.deduct_bonus,od.event_cost,od.single_cost, vb.brand_id,vb.brand_name,vb.vendor_id,v.vendor_name_simple,om.order_createdate ");
                sql.Append(" from order_detail od,order_slave os,vendor v,order_master om,product_item pi,product p,vendor_brand vb ");
                sql.Append(" where os.slave_id = od.slave_id	AND	os.order_id = om.order_id AND v.vendor_id = od.item_vendor_id AND pi.item_id = od.item_id AND p.product_id = pi.product_id AND vb.brand_id = p.brand_id AND item_mode in (0 , 1) ");
                if (!string.IsNullOrEmpty(query.Brand_Id_In))
                {
                    string[] quanxuan = query.Brand_Id_In.Split(',');
                    if (!quanxuan.Contains("0"))
                    {
                        sqlCondi.AppendFormat(" and p.brand_id in ({0}) ", query.Brand_Id_In);
                    }
                }
                if (query.product_manage != 0)
                {
                    sqlCondi.AppendFormat(" AND v.product_manage='{0}' ", query.product_manage);
                }
                if (query.channel != 0)
                {
                    sqlCondi.AppendFormat(" AND om.channel='{0}' ", query.channel);
                }
                switch (query.Status)
                {
                    case -1:
                        sqlCondi.AppendFormat(" and os.slave_status in(0,2,4,99,5,6)");
                            break;
                        default:
                            sqlCondi.AppendFormat(" and os.slave_status={0}", query.Status);
                            break;
                }
                if (query.order_payment != 0)
                {
                    sqlCondi.AppendFormat(" and om.order_payment={0}", query.order_payment);
                }
                if (!string.IsNullOrEmpty(query.select_con))
                {
                    switch (query.select_type)
                    {
                        case 1:
                            sqlCondi.AppendFormat(" AND od.product_name LIKE  '%{0}%'", query.select_con);
                            break;
                        case 2://會員編號
                            sqlCondi.AppendFormat(" AND om.user_id LIKE '%{0}%'", query.select_con);
                            break;
                        case 3:
                            sqlCondi.AppendFormat(" om.order_name LIKE   '%{0}%'", query.select_con);
                            break;
                        default:
                            break;
                    }
                }
                if (query.time_start > 0)
                {
                    sqlCondi.AppendFormat(" and om.order_createdate >= '{0}' ", query.time_start);
                }
                if (query.time_end > 0)
                {
                    sqlCondi.AppendFormat(" and om.order_createdate <= '{0}' ", query.time_end);
                }
                sql.Append(sqlCondi);
                sql.Append(" ORDER BY brand_sort, brand_name ");
                return _dbAccess.getDataTableForObj<OrderDetailQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.GetOrderDetailList -->" + ex.Message + sql.ToString(), ex);
            }        
        }

        public List<Vendor> GetVendor(Vendor query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlCondi = new StringBuilder();
            try
            {
                sql.Append(@" select vendor_id,vendor_email,vendor_name_simple,company_phone,company_person from vendor  ");
                if (query.vendor_id > 0)
                {
                    sql.AppendFormat(" where vendor_id='{0}';  ", query.vendor_id);
                }
                return _dbAccess.getDataTableForObj<Vendor>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.GetOrderDetailList -->" + ex.Message + sql.ToString(), ex);
            }

        }

        public DataTable GetCategorySummary(OrderDetailQuery query)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder count = new StringBuilder();
            DataTable dt = new DataTable();         
            try
            {
                sql.AppendFormat(@" select pcs.category_id, sum(od.single_money * buy_num) as amount
        from order_detail od 
        inner join product_item pit using(item_id)
        INNER JOIN order_slave os USING (slave_id)
        INNER JOIN order_master om USING (order_id)
        inner join product p using (product_id)
        inner join product_category_set pcs using(product_id)		
where 1=1  and  pcs.category_id={0} ", query.category_id);
                sql.AppendFormat(" AND od.detail_status <> 90");
                if (query.category_status != 0)
                {
                    sql.AppendFormat(" AND om.money_collect_date > 0");           
                }
                if(query.date_stauts!=0)
                {
                    if (query.date_start != DateTime.MinValue && query.date_end != DateTime.MinValue)
                    {
                        sql.AppendFormat(" AND om.order_createdate>='{0}' and  om.order_createdate<='{1}'", CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.date_start)), CommonFunction.GetPHPTime(CommonFunction.DateTimeToString(query.date_end)));
                    }
                }                                     
                return _dbAccess.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailDao.GetCategorySummaryList -->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}