using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;
using BLL.gigade.Common;
using System.Collections;

namespace BLL.gigade.Dao
{
    public class DeliverDetailDao : IDeliverDetailImplDao
    {
        private IDBAccess _access;
        private string connString;
        public DeliverDetailDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connString = connectionString;
        }

        public List<Model.Query.DeliverDetailQuery> GetDeliverDetail(Model.Query.DeliverDetailQuery dd)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@" SELECT dm.deliver_id,dm.delivery_status,dd.detail_id,");
            sql.AppendLine(@" od.item_id,od.product_name,od.product_spec_name,od.product_freight_set,");
            sql.AppendLine(@" od.buy_num,od.detail_status,tpara.remark as sdetail_status,od.item_vendor_id,od.product_mode,od.combined_mode,od.parent_id,");
            sql.AppendLine(@" od.parent_name,od.item_mode,od.pack_id,od.parent_num,p.product_mode as prod_mode");
            sql.AppendLine(@" from   deliver_master dm  LEFT JOIN deliver_detail dd on dm.deliver_id=dd.deliver_id");
            sql.AppendLine(@" LEFT JOIN order_detail od on od.detail_id=dd.detail_id");
            sql.AppendLine(@" LEFT JOIN product_item pi on pi.item_id=od.item_id");
            sql.AppendLine(@" LEFT JOIN product p on p.product_id=pi.product_id");
            sql.AppendLine(@" LEFT JOIN (SELECT parameterCode,remark FROM t_parametersrc where parameterType = 'order_status')  tpara ON tpara.parameterCode=od.detail_status ");
            sql.AppendFormat(@" WHERE  dm.deliver_id='{0}' ORDER BY item_vendor_id ASC,od.item_id ASC;", dd.deliver_id);
            try
            {
                List<DeliverDetailQuery> store = _access.getDataTableForObj<DeliverDetailQuery>(sql.ToString());
                return OrderDetailRestructuring(store);
                // return _access.getDataTableForObj<DeliverDetailQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailDao.GetDeliverDetail-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 出貨信息
        /// </summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        public List<DeliverMasterQuery> GetDeliverMaster(DeliverMasterQuery dm)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT dm.deliver_id,dm.type,dm.freight_set,dm.delivery_store,");
            sql.AppendLine(@" dm.delivery_code,dm.delivery_date,dm.sms_date,dm.estimated_delivery_date,");
            sql.AppendLine(@" dm.estimated_arrival_date,dm.estimated_arrival_period,dm.delivery_name,");
            sql.AppendLine(@" dm.delivery_mobile,dm.delivery_phone,dm.delivery_zip,dm.delivery_address,");
            sql.AppendLine(@" dm.delivery_status,dm.order_id,om.note_order,om.holiday_deliver");
            sql.AppendLine(@" from deliver_master dm LEFT JOIN order_master om on om.order_id=dm.order_id");
            sql.AppendFormat(@" where dm.deliver_id='{0}' LIMIT 1;", dm.deliver_id);
            try
            {
                return _access.getDataTableForObj<DeliverMasterQuery>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("DeliverDetailDao.GetDeliverMaster-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #region 按鈕
        /// <summary>
        /// 回填物流單號
        /// </summary>
        /// <param name="deliver_id">出貨單號</param>
        /// <param name="delivery_store">物流方式</param>
        /// <param name="delivery_code">物流單號</param>
        /// <param name="delivery_date">出貨日期</param>
        /// <param name="vendor_id">供應商</param>
        public bool DeliveryCode(string deliver_id, string delivery_store, string delivery_code, string delivery_date = null, string vendor_id = "0")
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            StringBuilder sql = new StringBuilder();
            StringBuilder allsql = new StringBuilder();
            object ob;
            sql.AppendLine(@"SELECT dm.order_id,dm.type,dm.freight_set,dm.delivery_mobile,");
            sql.AppendLine(@"dm.delivery_status,dm.delivery_date,om.user_id,om.order_name,");
            sql.AppendLine(@"om.delivery_mobile,om.delivery_phone,om.delivery_address,");
            sql.AppendLine(@"om.note_order,om.note_admin,om.delivery_name");
            sql.AppendLine(@" from deliver_master dm LEFT JOIN order_master om on dm.order_id=om.order_id ");
            sql.AppendFormat(@" where dm.deliver_id='{0}' ", deliver_id);
            if (vendor_id != "0")
            {
                sql.AppendFormat(@" and dm.export_id='{0}'", vendor_id);
            }
            sql.AppendFormat(" limit 1;");
            allsql.Append(sql.ToString());
            DataTable delivermaster = _access.getDataTable(sql.ToString());
            try
            {
                if (delivermaster.Rows.Count > 0)
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                    {
                        mySqlConn.Open();
                    }
                    mySqlCmd.Connection = mySqlConn;
                    mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                    mySqlCmd.CommandType = System.Data.CommandType.Text;
                    string delivery_status = "3"; //已出貨
                    string detail_status = "4"; //已出貨
                    string user_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString();
                    delivery_date = (!string.IsNullOrEmpty(delivery_date)) ? delivery_date : DateTime.Now.ToString("yyyy-MM-dd");
                    if (delivery_store == "12" || delivery_store == "13" || delivery_store == "14")
                    {
                        if (delivermaster.Rows[0]["delivery_status"].ToString() == "0")
                        {
                            delivery_status = "7"; //待取貨
                            detail_status = "9"; //待取貨
                        }
                    }
                    //deliver_master表  update  delivery_status
                    sql.Clear();
                    sql.AppendFormat(@" update deliver_master set delivery_status='{0}', delivery_store='{1}',", delivery_status, delivery_store);
                    sql.AppendFormat(@" delivery_code='{0}', delivery_date='{1}',", delivery_code, (!string.IsNullOrEmpty(delivery_date)) ? delivery_date : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sql.AppendFormat(@" verifier='{0}', modified='{1}'  where deliver_id='{2}';", user_id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), deliver_id);
                    mySqlCmd.CommandText = sql.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    //更新export_flag
                    if (delivermaster.Rows[0]["delivery_status"].ToString() == "0" && string.IsNullOrEmpty(delivermaster.Rows[0]["delivery_date"].ToString()))
                    {
                        sql.Clear();
                        //出貨單押單時，將deliver_master.export_flag設為1
                        sql.AppendFormat(@" update deliver_master set export_flag=1 where deliver_id='{0}';", deliver_id);
                        mySqlCmd.CommandText = sql.ToString();
                        mySqlCmd.ExecuteNonQuery();
                        allsql.Append(sql.ToString());
                    }
                    sql.Clear();
                    //deliver_detail表 update deliver_detail
                    sql.AppendFormat(@"update deliver_detail set delivery_status='{0}'", delivery_status);
                    sql.AppendFormat(@" where deliver_id='{0}' and delivery_status in (0, 1, 2, 7);", deliver_id);
                    mySqlCmd.CommandText = sql.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    // update ticket_status
                    sql.Clear();
                    sql.AppendFormat(@"SELECT ticket_id from deliver_master where deliver_id='{0}';", deliver_id);
                    string ticket_id = string.Empty;
                    mySqlCmd.CommandText = sql.ToString();
                    ob = mySqlCmd.ExecuteScalar();
                    if (ob != null)
                    {
                        ticket_id = ob.ToString();
                    }
                    //string ticket_id = mySqlCmd.ExecuteScalar().ToString();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                    sql.AppendFormat(@"SELECT count(deliver_id) from deliver_master where ticket_id='{0}' ", ticket_id);
                    sql.AppendFormat(@" and delivery_status not in (3, 5);");//已出貨 取消出貨
                    mySqlCmd.CommandText = sql.ToString();
                    ob = mySqlCmd.ExecuteScalar();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                    if (ob.ToString() == "0")
                    {
                        allsql.Append(sql.ToString());
                        sql.Clear();
                        sql.AppendFormat(@" update ticket set ticket_status=1,verifier='{0}', ", user_id);//已出貨
                        sql.AppendFormat(@" modified='{0}' where ticket_id='{1}';", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ticket_id);
                        mySqlCmd.CommandText = sql.ToString();
                        mySqlCmd.ExecuteNonQuery();
                        allsql.Append(sql.ToString());
                    }
                    // update detail_status
                    string ds = delivermaster.Rows[0]["type"].ToString() == "1" ? "(6, 7, 9)" : "(2, 6)";
                    sql.AppendFormat(@" UPDATE order_detail set detail_status='{0}' where detail_id in( SELECT * FROM (", detail_status);
                    sql.AppendFormat(@" SELECT dd.detail_id from deliver_detail dd LEFT JOIN order_detail od on dd.detail_id=od.detail_id");
                    sql.AppendFormat(@" WHERE deliver_id='{0}' and od.detail_status IN {1} ) T );", deliver_id, ds);//6 修正自出被改成調度
                    mySqlCmd.CommandText = sql.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                    // update slave_status
                    sql.AppendFormat(@"UPDATE order_slave set slave_status='4' where slave_id in ");//已出貨
                    sql.AppendFormat(@"(SELECT T.slave_id FROM (SELECT os.slave_id from deliver_master dm LEFT JOIN order_master om ");
                    sql.AppendFormat(@"on dm.order_id=om.order_id LEFT JOIN order_slave os on os.order_id=dm.order_id ");
                    sql.AppendFormat(@"LEFT JOIN order_detail od ON os.slave_id=od.slave_id ");
                    sql.AppendFormat(@"where od.detail_status in(2,3,4,6,7,9) and dm.deliver_id='{0}' GROUP BY os.slave_id ", deliver_id);//需出貨
                    sql.AppendFormat(@"HAVING SUM(od.detail_status!=4)=0) T) AND slave_status != 99;");
                    mySqlCmd.CommandText = sql.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                    //update order_status
                    sql.AppendFormat(@"UPDATE order_master set order_status='4' where order_id in ");
                    sql.AppendFormat(@" (SELECT T.order_id FROM (SELECT dm.order_id from deliver_master dm LEFT JOIN order_master om ");
                    sql.AppendFormat(@"on dm.order_id=om.order_id LEFT JOIN order_slave os on os.order_id=dm.order_id ");
                    sql.AppendFormat(@"where os.slave_status in(2,4,6) and dm.deliver_id='{0}' GROUP BY dm.order_id", deliver_id);
                    sql.AppendFormat(@" HAVING SUM(os.slave_status!=4)=0) T) and order_status!=99;");
                    mySqlCmd.CommandText = sql.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();

                    DeliverStatus dstatus = new DeliverStatus();
                    dstatus.deliver_id = int.Parse(deliver_id);
                    dstatus.state = 3;
                    dstatus.settime = DateTime.Now;
                    dstatus.endtime = DateTime.Now;
                    dstatus.freight_type = int.Parse(delivery_store) == 42 ? 12 : 11;
                    dstatus.Logistics_providers = int.Parse(delivery_store);
                    sql.AppendFormat(@" insert into deliver_status (deliver_id,state,settime,endtime,freight_type,Logistics_providers)");
                    sql.AppendFormat(@" values('{0}','{1}','{2}',", dstatus.deliver_id, dstatus.state, dstatus.settime.ToString("yyyy-MM-dd HH:mm:ss"));
                    sql.AppendFormat(@" '{0}','{1}','{2}');", dstatus.endtime.ToString("yyyy-MM-dd HH:mm:ss"), dstatus.freight_type, dstatus.Logistics_providers);
                    mySqlCmd.CommandText = sql.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                    #region 建立車隊pda資料
                    //建立車隊pda資料
                    //if (delivery_store == "16" || delivery_store == "17")
                    //{
                    //    sql.AppendLine(@" delete from delivery where De004=4;");
                    //    mySqlCmd.CommandText = sql.ToString();
                    //    mySqlCmd.ExecuteNonQuery();
                    //    allsql.Append(sql.ToString());
                    //    sql.Clear();
                    //    sql.AppendFormat(@"SELECT * from delivery where De000='{0}' LIMIT 1;", delivermaster.Rows[0]["order_id"]);
                    //    if (_access.getDataTable(sql.ToString()).Rows.Count == 0)
                    //    {
                    //        Delivery d = new Delivery();
                    //        d.De000 = delivermaster.Rows[0]["order_id"].ToString();//訂單號
                    //        d.De001 = delivermaster.Rows[0]["user_id"].ToString();//客戶代號
                    //        d.De002 = delivermaster.Rows[0]["delivery_address"].ToString();//客戶地址
                    //        d.De003 = 1;//配送順序
                    //        d.De004 = "3";//配送狀態
                    //        d.De005 = (!string.IsNullOrEmpty(delivermaster.Rows[0]["note_order"].ToString())) ? delivermaster.Rows[0]["note_order"].ToString() : delivermaster.Rows[0]["note_admin"].ToString();//配送備註
                    //        d.De006 = delivermaster.Rows[0]["delivery_name"].ToString();//簽收人
                    //        d.De007 = DateTime.Now;//修改時間
                    //        d.De008 = user_id;//新增人員
                    //        d.Cust02 = delivermaster.Rows[0]["delivery_name"].ToString();//客戶名稱
                    //        d.Cust03 = delivermaster.Rows[0]["delivery_mobile"].ToString();//客戶電話一
                    //        d.Cust04 = delivermaster.Rows[0]["delivery_phone"].ToString();//客戶電話二
                    //        d.Cust05 = (!string.IsNullOrEmpty(delivermaster.Rows[0]["note_order"].ToString())) ? delivermaster.Rows[0]["note_order"].ToString() : delivermaster.Rows[0]["note_admin"].ToString();//備註
                    //        sql.AppendFormat(@" insert into delivery (De000,De001,De002,De003,");
                    //        sql.AppendFormat(@"De004,De005,De006,De007,De008,");
                    //        sql.AppendFormat(@"Cust02,Cust03,Cust04,Cust05)");
                    //        sql.AppendFormat(@" values('{0}','{1}','{2}','{3}',", d.De000, d.De001, d.De002, d.De003);
                    //        sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", d.De004, d.De005, d.De006, d.De007.ToString("yyyy-MM-dd HH:mm:ss"), d.De008);
                    //        sql.AppendFormat(@"'{0}','{1}','{2}','{3}')", d.Cust02, d.Cust03, d.Cust04, d.Cust05);
                    //        mySqlCmd.CommandText = sql.ToString();
                    //        mySqlCmd.ExecuteNonQuery();
                    //        allsql.Append(sql.ToString());
                    //    }
                    //    sql.Clear();
                    //    sql.AppendLine(@" delete from pickup  where Pick003=4;");
                    //    mySqlCmd.CommandText = sql.ToString();
                    //    mySqlCmd.ExecuteNonQuery();
                    //    allsql.Append(sql.ToString());
                    //    sql.Clear();
                    //    sql.AppendFormat(@"SELECT * from pickup where Pick001='{0}' LIMIT 1;", "D" + deliver_id.PadLeft(8, '0'));
                    //    if (_access.getDataTable(sql.ToString()).Rows.Count == 0)
                    //    {
                    //        Pickup p = new Pickup();
                    //        p.De000 = delivermaster.Rows[0]["order_id"].ToString();//訂單號;
                    //        p.Pick001 = "D" + delivermaster.Rows[0]["deliver_id"].ToString().PadLeft(8, '0');//出貨單號
                    //        p.Pick002 = 1;//箱數
                    //        p.Pick003 = "3";//狀態
                    //        p.Pick004 = (delivermaster.Rows[0]["freight_set"].ToString() == "1" || delivermaster.Rows[0]["freight_set"].ToString() == "1") ? "1" : "2";//溫層
                    //        p.Pick005 = DateTime.Now;//修改時間
                    //        p.Pick006 = user_id;//修改人員
                    //        sql.Clear();
                    //        sql.AppendFormat(@" insert into pickup (De000,Pick001,Pick002,Pick003,");
                    //        sql.AppendFormat(@"Pick004,Pick005,Pick006) values(");
                    //        sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", p.De000, p.Pick001, p.Pick002, p.Pick003);
                    //        sql.AppendFormat(@"'{0}','{1}','{2}')", p.Pick004, p.Pick005.ToString("yyyy-MM-dd HH:mm:ss"), p.Pick006);
                    //        mySqlCmd.CommandText = sql.ToString();
                    //        mySqlCmd.ExecuteNonQuery();
                    //        allsql.Append(sql.ToString());
                    //    }

                    //}
                    #endregion
                    mySqlCmd.Transaction.Commit();
                    #region
                    if (string.IsNullOrEmpty(delivery_date) && delivermaster.Rows[0]["type"].ToString() == "1" && delivermaster.Rows[0]["delivery_status"].ToString() == "0" && !string.IsNullOrEmpty(delivermaster.Rows[0]["delivery_mobile"].ToString()))
                    {
                        if (delivery_store != "11" && delivery_store != "40" && delivery_store != "99")//7-11取貨(Yahoo) 公司自送 其它
                        {
                            sql.Clear();
                            sql.AppendFormat(@" SELECT c.* FROM order_master om INNER JOIN channel c ON om.channel = c.channel_id  WHERE 1=1 AND om.order_id ='{0}' limit 1;", delivermaster.Rows[0]["order_id"]);
                            DataTable channel = _access.getDataTable(sql.ToString());
                            if (channel.Rows.Count > 0)
                            {
                                //1.  若此訂單對應之channel.notify_sms=0，則不發送出貨通知簡訊。
                                string sms_to_date = string.Empty;
                                string sms_to_data = string.Empty;
                                if (channel.Rows[0]["notify_sms"].ToString() != "0")
                                {
                                    string now = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Second.ToString();
                                    //TimeSpan ts
                                    if (DateTime.Parse(now) <= DateTime.Parse("13:00"))
                                    {
                                        sms_to_date = DateTime.Now.ToString("yyyy-MM-dd") + " 13:30:00";
                                    }
                                    else if (DateTime.Parse(now) >= DateTime.Parse("13:00") && DateTime.Parse(now) <= DateTime.Parse("16:00"))
                                    {
                                        sms_to_date = DateTime.Now.ToString("yyyy-MM-dd") + " 16:30:00";
                                    }
                                    else
                                    {
                                        sms_to_date = DateTime.Now.ToString("yyyy-MM-dd") + " 18:30:00";
                                    }
                                    sms_to_data = "[吉甲地出貨通知]您好，訂單" + delivermaster.Rows[0]["order_id"].ToString() + "(" + delivermaster.Rows[0]["freight_set"].ToString() + ")已於今日" + DateTime.Now.ToString("MM-dd") + "寄出預計明日配達，若有收件問題可回覆此簡訊，客服人員會與您聯繫處理。";
                                    string smssql = Send(4, delivermaster.Rows[0]["order_id"].ToString(), delivermaster.Rows[0]["delivery_mobile"].ToString(), "出貨通知", sms_to_data, sms_to_date, deliver_id);
                                    mySqlCmd.CommandText = smssql;
                                    mySqlCmd.ExecuteNonQuery();
                                    allsql.Append(smssql);
                                    sql.Clear();
                                }
                            }

                        }
                    }
                    #endregion
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                return false;
                throw new Exception("DeliverDetailDao-->DeliveryCode-->" + ex.Message + sql.ToString(), ex);

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
        /// 修改出貨方式 改自出 改寄倉 改調度
        /// </summary>
        /// <param name="deliver_id">出貨單號</param>
        /// <param name="detail_id"></param>
        /// <param name="product_mode">出貨方式</param>
        /// <returns></returns>
        public string ProductMode(string deliver_id, string detail_id, string product_mode)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            SerialDao serialDao = new SerialDao("");
            StringBuilder allsql = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            int i = 0;
            System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            string ip = string.Empty;
            //ip = CommonFunction.GetClientIP();
            if (addlist.Length > 0)
            {
                ip = addlist[0].ToString();
            }
            int user_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sql.AppendLine(@"SELECT od.item_vendor_id,od.product_freight_set,od.single_money,od.buy_num,os.order_id,os.slave_id ");
                sql.AppendFormat(@" from order_detail od LEFT JOIN order_slave os on od.slave_id=os.slave_id WHERE od.detail_id='{0}' limit 1;", detail_id);
                DataTable dt = _access.getDataTable(sql.ToString());
                allsql.Append(sql.ToString());
                sql.Clear();
                string freightset = string.Empty;
                string vendorid = string.Empty;
                string orderid = "0";
                string slaveid = "0";
                string itemvendorid = "0";
                string vendormap = "0";
                uint subtotal = 0;
                object ob;
                if (dt.Rows.Count > 0)
                {
                    switch (dt.Rows[0]["product_freight_set"].ToString())
                    {
                        case "1":
                        case "3":
                            freightset = "1";
                            break;
                        case "2":
                        case "4":
                        case "5":
                        case "6":
                            freightset = "2";
                            break;
                        default:
                            break;
                    }
                    if (freightset == "1")
                    {
                        vendormap = "2";//吉甲地統倉
                    }
                    else if (freightset == "2")
                    {
                        vendormap = "92";//吉甲地冷凍倉
                    }
                    if (!string.IsNullOrEmpty(dt.Rows[0]["single_money"].ToString()) && !string.IsNullOrEmpty(dt.Rows[0]["buy_num"].ToString()))
                    {
                        subtotal = uint.Parse(dt.Rows[0]["single_money"].ToString().Trim()) * uint.Parse(dt.Rows[0]["buy_num"].ToString().Trim());
                    }
                    orderid = dt.Rows[0]["order_id"].ToString();
                    slaveid = dt.Rows[0]["slave_id"].ToString();
                    itemvendorid = dt.Rows[0]["item_vendor_id"].ToString();
                }
                #region 對order_slave表和order_deta表的操作
                vendorid = product_mode == "2" ? vendormap : itemvendorid;
                sql.AppendFormat(@"SELECT slave_id from order_slave  where order_id='{0}' ", orderid);
                sql.AppendFormat(@" and vendor_id='{0}' and slave_status=2;", vendorid);
                mySqlCmd.CommandText = sql.ToString();
                string newslaveid = string.Empty;
                ob = mySqlCmd.ExecuteScalar();
                if (ob != null)
                {
                    newslaveid = ob.ToString();
                }
                allsql.Append(sql.ToString());
                sql.Clear();
                if (string.IsNullOrEmpty(newslaveid))
                {
                    mySqlCmd.CommandText = serialDao.Update(30); //order_slave
                    allsql.Append(mySqlCmd.CommandText);
                    newslaveid = mySqlCmd.ExecuteScalar().ToString();
                    OrderSlave os = new OrderSlave();
                    os.Slave_Id = uint.Parse(newslaveid);
                    os.Order_Id = (!string.IsNullOrEmpty(orderid)) ? uint.Parse(orderid) : 0;
                    os.Vendor_Id = (!string.IsNullOrEmpty(vendorid)) ? uint.Parse(vendorid) : 0;
                    os.Slave_Product_Subtotal = subtotal;
                    os.Slave_Amount = subtotal;
                    os.Slave_Status = 2;//待出貨
                    os.Slave_Updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                    os.Slave_Ipfrom = ip;
                    sql.AppendFormat(@"INSERT into order_slave (slave_id,order_id,vendor_id,slave_freight_normal,slave_freight_low,");
                    sql.AppendFormat(@"slave_product_subtotal,slave_amount,slave_status,slave_note,slave_date_delivery,");
                    sql.AppendFormat(@"slave_date_cancel,slave_date_return,slave_date_close,account_status,slave_updatedate,slave_ipfrom)");
                    sql.AppendFormat(@" VALUES('{0}','{1}','{2}','{3}','{4}',", os.Slave_Id, os.Order_Id, os.Vendor_Id, os.Slave_Freight_Normal, os.Slave_Freight_Low);
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", os.Slave_Product_Subtotal, os.Slave_Amount, os.Slave_Status, os.Slave_Note, os.Slave_Date_Delivery);
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", os.Slave_Date_Cancel, os.Slave_Date_Return, os.Slave_Date_Close, os.Account_Status == true ? 1 : 0);
                    sql.AppendFormat(@"'{0}','{1}');", os.Slave_Updatedate, os.Slave_Ipfrom);
                    mySqlCmd.CommandText = sql.ToString();
                    i = mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                }
                if (newslaveid != slaveid)
                {
                    sql.AppendFormat(@" update order_slave set slave_product_subtotal=slave_product_subtotal+{0},", subtotal);
                    sql.AppendFormat(@" slave_amount=slave_amount+{0},slave_updatedate='{1}',", subtotal, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                    sql.AppendFormat(@" slave_ipfrom='{0}' where slave_id='{1}'; ", ip, newslaveid);
                    mySqlCmd.CommandText = sql.ToString();
                    i = mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                }
                //更新order_detail表的slave_id，product_mode字段
                sql.AppendFormat(@" update order_detail set slave_id='{0}',product_mode='{1}'", newslaveid, product_mode);
                sql.AppendFormat(@" where detail_id='{0}';", detail_id);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                allsql.Append(sql.ToString());
                sql.Clear();
                if (newslaveid != slaveid)
                {
                    sql.AppendFormat(" select count(slave_id) from order_detail where slave_id='{0}';", slaveid);
                    mySqlCmd.CommandText = sql.ToString();
                    string count = mySqlCmd.ExecuteScalar().ToString();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                    if (count == "0")
                    {
                        sql.AppendFormat(@" DELETE FROM order_slave where slave_id='{0}';", slaveid);
                        mySqlCmd.CommandText = sql.ToString();
                        i = mySqlCmd.ExecuteNonQuery();
                        allsql.Append(sql.ToString());
                        sql.Clear();
                    }
                    else
                    {
                        sql.AppendFormat(@" update order_slave set slave_product_subtotal=slave_product_subtotal-{0},", subtotal);
                        sql.AppendFormat(@" slave_amount=slave_amount-{0},slave_updatedate='{1}',", subtotal, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                        sql.AppendFormat(@" slave_ipfrom='{0}' where slave_id='{1}'; ", ip, slaveid);
                        mySqlCmd.CommandText = sql.ToString();
                        i = mySqlCmd.ExecuteNonQuery();
                        allsql.Append(sql.ToString());
                        sql.Clear();
                    }
                }
                #endregion
                #region 對deliver_master表和deliver_detail表的操作
                sql.AppendLine(@"SELECT deliver_id,order_id,ticket_id,type,export_id,import_id,freight_set,delivery_status,");
                sql.AppendLine(@" delivery_name,delivery_mobile,delivery_phone,delivery_zip,delivery_address,delivery_store,delivery_code,");
                sql.AppendLine(@"delivery_freight_cost,delivery_date,sms_date,arrival_date,estimated_delivery_date,estimated_arrival_date,");
                sql.AppendLine(@"estimated_arrival_period,creator,verifier,created,modified,export_flag,data_chg,work_status");
                sql.AppendFormat(@" FROM deliver_master WHERE deliver_id='{0}' LIMIT 1;", deliver_id);
                // sql.AppendFormat(@" select * from deliver_master where deliver_id='{0}' limit 1;", deliver_id);
                List<DeliverMaster> store = _access.getDataTableForObj<DeliverMaster>(sql.ToString());
                DeliverMaster dm = new DeliverMaster();
                if (store.Count > 0)
                {
                    dm = store[0];
                }
                allsql.Append(sql.ToString());
                sql.Clear();
                string exportid = product_mode == "1" ? itemvendorid : vendormap;//是否是改自出
                string deliverytype = product_mode == "1" ? "2" : "1";
                sql.AppendFormat(@"SELECT deliver_id from deliver_master where ");
                sql.AppendFormat(@" order_id='{0}' and type='{1}' and export_id='{2}' and freight_set='{3}' ;", orderid, deliverytype, exportid, freightset);
                mySqlCmd.CommandText = sql.ToString();
                ob = mySqlCmd.ExecuteScalar();
                string newdeliverid = string.Empty;
                if (ob != null)
                {
                    newdeliverid = ob.ToString();
                }
                allsql.Append(sql.ToString());
                sql.Clear();
                if (string.IsNullOrEmpty(newdeliverid))
                {
                    mySqlCmd.CommandText = serialDao.Update(76); //deliver_master
                    allsql.Append(mySqlCmd.CommandText);
                    newdeliverid = mySqlCmd.ExecuteScalar().ToString();
                    dm.ticket_id = 0;
                    dm.type = uint.Parse(deliverytype);//出貨類別,1:統倉出貨,2:供應商自行出貨,3:供應商調度出貨,4:退貨,5:退貨瑕疵,6:瑕疵(目前數據中只有1和2兩種)
                    dm.export_id = int.Parse(exportid);
                    dm.import_id = 0;
                    dm.deliver_id = uint.Parse(newdeliverid);
                    dm.creator = user_id;
                    dm.verifier = user_id;
                    dm.created = DateTime.Now;
                    dm.modified = DateTime.Now;
                    sql.AppendFormat(@"INSERT INTO deliver_master (deliver_id,order_id,ticket_id,type,export_id,import_id,freight_set,");
                    sql.AppendFormat(@"delivery_status,delivery_name,delivery_mobile,delivery_phone,delivery_zip,");
                    sql.AppendFormat(@"delivery_address,delivery_store,delivery_code,delivery_freight_cost,");
                    //sql.AppendFormat(@"sms_date,arrival_date,estimated_delivery_date,estimated_arrival_date,estimated_arrival_period,");
                    sql.AppendFormat(@"estimated_arrival_period, ");
                    sql.AppendFormat(@"creator,verifier,created,modified,export_flag,data_chg,work_status)");
                    sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',", dm.deliver_id, dm.order_id, dm.ticket_id, dm.type, dm.export_id, dm.import_id, dm.freight_set);
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", dm.delivery_status, dm.delivery_name, dm.delivery_mobile, dm.delivery_phone, dm.delivery_zip);
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", dm.delivery_address, dm.delivery_store, dm.delivery_code, dm.delivery_freight_cost);
                    sql.AppendFormat(@"'{0}',", dm.estimated_arrival_period);
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}');", dm.creator, dm.verifier, dm.created.ToString("yyyy-MM-dd HH:mm:ss"), dm.modified.ToString("yyyy-MM-dd HH:mm:ss"), dm.export_flag, dm.data_chg, dm.work_status);
                    mySqlCmd.CommandText = sql.ToString();
                    i = mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                }
                if (newdeliverid != deliver_id)
                {
                    sql.AppendFormat(@" update deliver_detail set deliver_id='{0}' ", newdeliverid);
                    sql.AppendFormat(@" where deliver_id='{0}' and detail_id='{1}';  ", deliver_id, detail_id);
                    mySqlCmd.CommandText = sql.ToString();
                    i = mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                    sql.Append(Shrink(deliver_id));
                    if (sql.Length > 0)
                    {
                        mySqlCmd.CommandText = sql.ToString();
                        mySqlCmd.ExecuteNonQuery();
                        allsql.Append(sql.ToString());
                        sql.Clear();
                    }
                }
                sql.AppendFormat(@" SELECT deliver_id from deliver_master WHERE order_id='{0}'", orderid);
                sql.AppendFormat(@" and type=101 and export_id='{0}' and freight_set='{1}';", itemvendorid, freightset);
                mySqlCmd.CommandText = sql.ToString();
                ob = mySqlCmd.ExecuteScalar();
                string deliver_101_id = string.Empty;
                if (ob != null)
                {
                    deliver_101_id = ob.ToString();
                }
                allsql.Append(sql.ToString());
                sql.Clear();
                //自出 寄倉
                if (product_mode == "1" || product_mode == "2")
                {
                    sql.AppendFormat(@" DELETE  FROM deliver_detail where deliver_id='{0}'", deliver_101_id);
                    sql.AppendFormat(@" and detail_id='{0}'; ", detail_id);
                    mySqlCmd.CommandText = sql.ToString();
                    i = mySqlCmd.ExecuteNonQuery();
                    allsql.Append(sql.ToString());
                    sql.Clear();
                    sql.Append(Shrink(deliver_101_id));
                    if (sql.Length > 0)
                    {
                        mySqlCmd.CommandText = sql.ToString();
                        mySqlCmd.ExecuteNonQuery();
                        allsql.Append(sql.ToString());
                        sql.Clear();
                    }
                }
                else if (product_mode == "3")
                {
                    if (string.IsNullOrEmpty(deliver_101_id))
                    {
                        mySqlCmd.CommandText = serialDao.Update(76); //deliver_master
                        allsql.Append(mySqlCmd.CommandText);
                        deliver_101_id = mySqlCmd.ExecuteScalar().ToString();
                        //DeliverMaster dm = new DeliverMaster();
                        dm.ticket_id = 0;
                        dm.type = 101;
                        dm.export_id = int.Parse(itemvendorid);
                        dm.import_id = 0;
                        dm.deliver_id = uint.Parse(deliver_101_id);
                        dm.creator = user_id;
                        dm.verifier = user_id;
                        dm.created = DateTime.Now;
                        dm.modified = DateTime.Now;
                        sql.AppendFormat(@"INSERT INTO deliver_master (deliver_id,order_id,ticket_id,type,export_id,import_id,freight_set,");
                        sql.AppendFormat(@"delivery_status,delivery_name,delivery_mobile,delivery_phone,delivery_zip,");
                        sql.AppendFormat(@"delivery_address,delivery_store,delivery_code,delivery_freight_cost,");
                        //sql.AppendFormat(@"sms_date,arrival_date,estimated_delivery_date,estimated_arrival_date,estimated_arrival_period,");
                        sql.AppendFormat(@" estimated_arrival_period,");
                        sql.AppendFormat(@"creator,verifier,created,modified,export_flag,data_chg,work_status)");
                        sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',", dm.deliver_id, dm.order_id, dm.ticket_id, dm.type, dm.export_id, dm.import_id, dm.freight_set);
                        sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", dm.delivery_status, dm.delivery_name, dm.delivery_mobile, dm.delivery_phone, dm.delivery_zip);
                        sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", dm.delivery_address, dm.delivery_store, dm.delivery_code, dm.delivery_freight_cost);
                        sql.AppendFormat(@"'{0}',", dm.estimated_arrival_period);
                        sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}');", dm.creator, dm.verifier, dm.created.ToString("yyyy-MM-dd HH:mm:ss"), dm.modified.ToString("yyyy-MM-dd HH:mm:ss"), dm.export_flag, dm.data_chg, dm.work_status);
                        mySqlCmd.CommandText = sql.ToString();
                        i = mySqlCmd.ExecuteNonQuery();
                        allsql.Append(sql.ToString());
                        sql.Clear();
                        DeliverDetail dd = new DeliverDetail();
                        dd.deliver_id = uint.Parse(deliver_101_id);
                        dd.detail_id = uint.Parse(detail_id);
                        sql.AppendFormat(@" INSERT INTO deliver_detail (deliver_id,detail_id,delivery_status)");
                        sql.AppendFormat(@" VALUES ('{0}','{1}','{2}');", dd.deliver_id, dd.detail_id, dd.delivery_status);
                        mySqlCmd.CommandText = sql.ToString();
                        i = mySqlCmd.ExecuteNonQuery();
                        allsql.Append(sql.ToString());
                        sql.Clear();
                    }

                }

                #endregion
                mySqlCmd.Transaction.Commit();
                return newdeliverid;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("DeliverDetailDao-->ProductMode-->" + ex.Message + sql.ToString(), ex);
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
        /// 未到貨
        /// </summary>
        /// <param name="deliver_id"></param>
        /// <param name="detail_id"></param>
        public bool NoDelivery(string deliver_id, string detail_id)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            StringBuilder sql = new StringBuilder();
            int i = 0;
            object ob;
            System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            string ip = string.Empty;
            //ip = CommonFunction.GetClientIP();
            if (addlist.Length > 0)
            {
                ip = addlist[0].ToString();
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
                // update detail_status
                sql.AppendFormat(@" update order_detail set detail_status=2 where detail_id='{0}';", detail_id);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                sql.Clear();
                //update slave_status
                sql.AppendFormat(@" select slave_id from order_detail where detail_id='{0}';", detail_id);
                mySqlCmd.CommandText = sql.ToString();
                string slaveid = string.Empty;
                ob = mySqlCmd.ExecuteScalar();
                if (ob != null)
                {
                    slaveid = ob.ToString();
                }
                sql.Clear();
                sql.AppendFormat(@" update order_slave set slave_status=2, slave_updatedate='{0}',", uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString()));
                sql.AppendFormat(@" slave_ipfrom='{0}'  where slave_id='{1}'; ", ip, slaveid);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                sql.Clear();
                //update delivery_status
                sql.AppendFormat(@" update deliver_detail set delivery_status=5 where detail_id='{0}';", detail_id);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                return false;
                throw new Exception("DeliverDetailDao-->NoDelivery-->" + ex.Message + sql.ToString(), ex);
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
        /// 拆分細項 
        /// </summary>
        /// <param name="deliver_id"></param>
        /// <param name="detail_id"></param>
        public bool SplitDetail(string deliver_id, string detail_id)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            SerialDao serialDao = new SerialDao("");
            StringBuilder sql = new StringBuilder();
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
                sql.AppendLine(@"SELECT detail_id,slave_id,item_id,item_vendor_id,product_freight_set,product_mode,product_name,");
                sql.AppendLine(@"product_spec_name,single_cost,event_cost,single_price,single_money,deduct_bonus,");
                sql.AppendLine(@"deduct_welfare,deduct_happygo as sdeduct_happygo ,deduct_happygo_money as sdeduct_happygo_money,deduct_account,deduct_account_note,");
                sql.AppendLine(@"accumulated_bonus as saccumulated_bonus ,accumulated_happygo as saccumulated_happygo ,buy_num,detail_status,detail_note,item_code,");
                sql.AppendLine(@"arrival_status as rarrival_status,delay_till,lastmile_deliver_serial,lastmile_deliver_datetime,");
                sql.AppendLine(@"lastmile_deliver_agency,bag_check_money,channel_detail_id,combined_mode as scombined_mode,item_mode,parent_id as sparent_id,parent_name,");
                sql.AppendLine(@"parent_num,price_master_id,pack_id,sub_order_id,site_id,event_id,prepaid");
                sql.AppendFormat(@" FROM order_detail WHERE detail_id='{0}' LIMIT 1;", detail_id);

                //sql.AppendFormat(@" select * from order_detail where detail_id='{0}' limit 1 ;", detail_id);
                List<OrderDetail> store = _access.getDataTableForObj<OrderDetail>(sql.ToString());
                OrderDetail od = new OrderDetail();
                if (store.Count > 0)
                {
                    od = store[0];
                }
                if (od.Buy_Num < 2)
                {
                    throw new Exception();
                }
                od.Buy_Num = od.Buy_Num - 1;
                od.Deduct_Bonus = uint.Parse((od.Deduct_Bonus / od.Buy_Num * (od.Buy_Num - 1)).ToString("0"));
                od.Deduct_Welfare = uint.Parse((od.Deduct_Welfare / od.Buy_Num * (od.Buy_Num - 1)).ToString("0"));
                od.rdeduct_happygo = int.Parse((od.rdeduct_happygo / od.Buy_Num * (od.Buy_Num - 1)).ToString("0"));
                od.raccumulated_bonus = int.Parse((od.raccumulated_bonus / od.Buy_Num * (od.Buy_Num - 1)).ToString("0"));
                od.raccumulated_happygo = int.Parse((od.raccumulated_happygo / od.Buy_Num * (od.Buy_Num - 1)).ToString("0"));
                sql.AppendFormat(@" update order_detail set buy_num='{0}',deduct_bonus='{1}',", od.Buy_Num, od.Deduct_Bonus);
                sql.AppendFormat(@"deduct_welfare='{0}',deduct_happygo='{1}',accumulated_bonus='{2}',", od.Deduct_Welfare, od.rdeduct_happygo, od.raccumulated_bonus);
                sql.AppendFormat(@"accumulated_happygo='{0}' where detail_id='{1}';", od.raccumulated_happygo, detail_id);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                sql.Clear();
                OrderDetail odd = new OrderDetail();
                if (store.Count > 0)
                {
                    odd = store[0];
                }
                mySqlCmd.CommandText = serialDao.Update(32); //order_detail
                odd.Detail_Id = uint.Parse(mySqlCmd.ExecuteScalar().ToString());
                odd.Buy_Num = 1;
                odd.Deduct_Bonus = odd.Deduct_Bonus - od.Deduct_Bonus;
                odd.Deduct_Welfare = odd.Deduct_Welfare - od.Deduct_Welfare;
                odd.rdeduct_happygo = odd.rdeduct_happygo - od.rdeduct_happygo;
                odd.raccumulated_bonus = odd.raccumulated_bonus - od.raccumulated_bonus;
                odd.raccumulated_happygo = odd.raccumulated_happygo - od.raccumulated_happygo;
                sql.AppendFormat(@"INSERT INTO order_detail");
                sql.AppendFormat(@" (detail_id,slave_id,item_id,item_vendor_id,product_freight_set,product_mode");
                sql.AppendFormat(@" ,product_name,product_spec_name,single_cost,event_cost,single_price");
                sql.AppendFormat(@",single_money,deduct_bonus,deduct_welfare ,deduct_happygo,deduct_happygo_money");
                sql.AppendFormat(@",deduct_account,deduct_account_note,accumulated_bonus,accumulated_happygo");
                sql.AppendFormat(@",buy_num,detail_status,detail_note ,item_code,arrival_status,delay_till");
                sql.AppendFormat(@",lastmile_deliver_serial,lastmile_deliver_datetime,lastmile_deliver_agency");
                sql.AppendFormat(@",bag_check_money,channel_detail_id,combined_mode,item_mode,parent_id");
                sql.AppendFormat(@",parent_name,parent_num,price_master_id,pack_id,sub_order_id");
                sql.AppendFormat(@",site_id,event_id,prepaid)");
                sql.AppendFormat(@" VALUES('{0}','{1}','{2}','{3}','{4}','{5}',", odd.Detail_Id, odd.Slave_Id, odd.Item_Id, odd.Item_Vendor_Id, odd.Product_Freight_Set, odd.Product_Mode);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}','{4}',", odd.Product_Name, odd.Product_Spec_Name, odd.Single_Cost, odd.Event_Cost, odd.Single_Price);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}','{4}',", odd.Single_Money, odd.Deduct_Bonus, odd.Deduct_Welfare, odd.rdeduct_happygo, odd.rdeduct_happygo_money);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}',", odd.Deduct_Account, odd.Deduct_Account_Note, odd.raccumulated_bonus, odd.raccumulated_happygo);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}','{4}','{5}',", odd.Buy_Num, odd.Detail_Status, odd.Detail_Note, odd.Item_Code, odd.rarrival_status, odd.Delay_Till);
                sql.AppendFormat(@" '{0}','{1}','{2}',", odd.Lastmile_Deliver_Serial, odd.Lastmile_Deliver_Datetime, odd.Lastmile_Deliver_Agency);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}','{4}',", odd.Bag_Check_Money, odd.Channel_Detail_Id, odd.rcombined_mode, odd.item_mode, odd.rparent_id);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}','{4}',", odd.parent_name, odd.parent_num, odd.price_master_id, odd.pack_id, odd.Sub_Order_Id);
                sql.AppendFormat(@" '{0}','{1}','{2}')", odd.Site_Id, odd.event_id, odd.Prepaid);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                sql.Clear();
                DeliverDetail dd = new DeliverDetail();
                dd.detail_id = odd.Detail_Id;
                sql.AppendFormat(@" insert into deliver_detail (deliver_id, detail_id) ");
                sql.AppendFormat(@" select deliver_id,{0} from deliver_detail where detail_id='{1}';", dd.detail_id, detail_id);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                mySqlCmd.Transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                return false;
                throw new Exception("DeliverDetailDao-->SplitDetail-->" + ex.Message + sql.ToString(), ex);
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
        /// 下一次出貨
        /// </summary>
        /// <param name="deliver_id"></param>
        /// <param name="detail_id"></param>
        public string Split(string deliver_id, string[] detail_ids)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connString);
            SerialDao serialDao = new SerialDao("");
            StringBuilder sql = new StringBuilder();
            System.Net.IPAddress[] addlist = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            string detail_id = "(";
            foreach (var item in detail_ids)
            {
                detail_id += item + ",";
            }
            detail_id = detail_id.TrimEnd(',');
            detail_id += ")";
            int i = 0;
            string ip = string.Empty;
            if (addlist.Length > 0)
            {
                ip = addlist[0].ToString();
            }
            int user_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                sql.AppendFormat(@" SELECT order_id,type,export_id,import_id,freight_set,delivery_name,delivery_mobile,");
                sql.AppendFormat(@" delivery_phone, delivery_zip,delivery_store,delivery_address  from deliver_master ");
                sql.AppendFormat(@" where deliver_id='{0}';", deliver_id);
                List<DeliverMaster> store = _access.getDataTableForObj<DeliverMaster>(sql.ToString());
                DeliverMaster dm = new DeliverMaster();
                if (store.Count > 0)
                {
                    dm = store[0];
                }
                mySqlCmd.CommandText = serialDao.Update(76); //deliver_master
                dm.deliver_id = uint.Parse(mySqlCmd.ExecuteScalar().ToString());
                dm.creator = user_id;
                dm.verifier = user_id;
                dm.created = DateTime.Now;
                dm.modified = DateTime.Now;
                sql.Clear();
                sql.AppendFormat(@"INSERT INTO deliver_master (deliver_id,order_id,ticket_id,type,export_id,import_id,freight_set,");
                sql.AppendFormat(@"delivery_status,delivery_name,delivery_mobile,delivery_phone,delivery_zip,");
                sql.AppendFormat(@"delivery_address,delivery_store,delivery_code,delivery_freight_cost,");
                //sql.AppendFormat(@"sms_date,arrival_date,estimated_delivery_date,estimated_arrival_date,");
                sql.AppendFormat(@" estimated_arrival_period, ");
                sql.AppendFormat(@"creator,verifier,created,modified,export_flag,data_chg,work_status)");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',", dm.deliver_id, dm.order_id, dm.ticket_id, dm.type, dm.export_id, dm.import_id, dm.freight_set);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", dm.delivery_status, dm.delivery_name, dm.delivery_mobile, dm.delivery_phone, dm.delivery_zip);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", dm.delivery_address, dm.delivery_store, dm.delivery_code, dm.delivery_freight_cost);
                //sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',",null,null, null,null, dm.estimated_arrival_period);
                sql.AppendFormat(@" '{0}',", dm.estimated_arrival_period);
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}','{5}','{6}');", dm.creator, dm.verifier, dm.created.ToString("yyyy-MM-dd HH:mm:ss"), dm.modified.ToString("yyyy-MM-dd HH:mm:ss"), dm.export_flag, dm.data_chg, dm.work_status);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                sql.Clear();
                sql.AppendFormat(@" update deliver_detail set deliver_id='{0}'", dm.deliver_id);
                sql.AppendFormat(@" where deliver_id='{0}' and detail_id in {1};", deliver_id, detail_id);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                sql.Clear();
                if (dm.delivery_store == 42)//到店取貨
                {
                    sql.AppendFormat(@" SELECT order_id FROM split_single_remind WHERE order_id ='{0}'; ", dm.order_id);
                    mySqlCmd.CommandText = sql.ToString();
                    object ob = mySqlCmd.ExecuteScalar();
                    sql.Clear();
                    if (ob == null)
                    {
                        sql.AppendFormat(@" insert into split_single_remind SET order_id='{0}', status=0, set_time='{1}';", dm.order_id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        mySqlCmd.CommandText = sql.ToString();
                        i = mySqlCmd.ExecuteNonQuery();
                        sql.Clear();
                    }
                }
                DeliverStatus dstatus = new DeliverStatus();
                dstatus.deliver_id = int.Parse(deliver_id);
                dstatus.state = 3;
                dstatus.settime = DateTime.Now;
                dstatus.endtime = DateTime.Now;
                dstatus.freight_type = int.Parse(dm.delivery_store.ToString()) == 42 ? 12 : 11;
                dstatus.Logistics_providers = int.Parse(dm.delivery_store.ToString());
                sql.AppendFormat(@" insert into deliver_status (deliver_id,state,settime,endtime,freight_type,Logistics_providers)");
                sql.AppendFormat(@" values('{0}','{1}','{2}',", dstatus.deliver_id, dstatus.state, dstatus.settime.ToString("yyyy-MM-dd HH:mm:ss"));
                sql.AppendFormat(@" '{0}','{1}','{2}');", dstatus.endtime.ToString("yyyy-MM-dd HH:mm:ss"), dstatus.freight_type, dstatus.Logistics_providers);
                mySqlCmd.CommandText = sql.ToString();
                i = mySqlCmd.ExecuteNonQuery();
                sql.Clear();

                //供應商自出
                if (dm.type == 2)
                {
                    uint sumtotal = 0;
                    sql.Clear();
                    sql.AppendFormat(@"SELECT  os.slave_id, order_id,vendor_id, slave_status,");
                    sql.AppendFormat(@"SUM(od.single_money * od.buy_num) AS subtotal");
                    sql.AppendFormat(@" from order_slave os LEFT JOIN order_detail od on os.slave_id=od.slave_id");
                    sql.AppendFormat(@" where od.detail_id in {0} GROUP BY os.slave_id  limit 1;", detail_id);
                    DataTable dt = _access.getDataTable(sql.ToString());
                    for (int j = 0; j < detail_ids.Length; j++)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            DataRow dr = dt.Rows[0];
                            uint subtotal = (!string.IsNullOrEmpty(dr["subtotal"].ToString())) ? uint.Parse(dr["subtotal"].ToString()) : 0;
                            sumtotal += subtotal;
                        }
                    }
                    sql.Clear();
                    sql.AppendFormat(@" update order_slave set slave_product_subtotal=slave_product_subtotal-{0},", sumtotal);
                    sql.AppendFormat(@" slave_amount=slave_amount-{0},slave_updatedate='{1}', ", sumtotal, uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString()));
                    sql.AppendFormat(@" slave_ipfrom='{0}' where slave_id='{1}';", ip, dt.Rows.Count.ToString() != "0" ? dt.Rows[0]["slave_id"].ToString() : "");
                    mySqlCmd.CommandText = sql.ToString();
                    i = mySqlCmd.ExecuteNonQuery();
                    sql.Clear();
                    OrderSlave os = new OrderSlave();
                    mySqlCmd.CommandText = serialDao.Update(30); //order_slave
                    os.Slave_Id = uint.Parse(mySqlCmd.ExecuteScalar().ToString());
                    os.Slave_Product_Subtotal = sumtotal;
                    os.Slave_Amount = sumtotal;
                    if (dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["order_id"].ToString()))
                    {
                        os.Order_Id = uint.Parse(dt.Rows[0]["order_id"].ToString());
                    }
                    if (dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["vendor_id"].ToString()))
                    {
                        os.Vendor_Id = uint.Parse(dt.Rows[0]["vendor_id"].ToString());
                    }
                    if (dt.Rows.Count > 0 && !string.IsNullOrEmpty(dt.Rows[0]["slave_status"].ToString()))
                    {
                        os.Slave_Status = uint.Parse(dt.Rows[0]["slave_status"].ToString());
                    }
                    os.Slave_Ipfrom = ip;
                    os.Slave_Updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                    sql.Clear();
                    sql.AppendFormat(@"INSERT into order_slave (slave_id,order_id,vendor_id,slave_freight_normal,slave_freight_low,");
                    sql.AppendFormat(@"slave_product_subtotal,slave_amount,slave_status,slave_note,slave_date_delivery,");
                    sql.AppendFormat(@"slave_date_cancel,slave_date_return,slave_date_close,account_status,slave_updatedate,slave_ipfrom)");
                    sql.AppendFormat(@" VALUES('{0}','{1}','{2}','{3}','{4}',", os.Slave_Id, os.Order_Id, os.Vendor_Id, os.Slave_Freight_Normal, os.Slave_Freight_Low);
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}','{4}',", os.Slave_Product_Subtotal, os.Slave_Amount, os.Slave_Status, os.Slave_Note, os.Slave_Date_Delivery);
                    sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", os.Slave_Date_Cancel, os.Slave_Date_Return, os.Slave_Date_Close, os.Account_Status == true ? 1 : 0);
                    sql.AppendFormat(@"'{0}','{1}');", os.Slave_Updatedate, os.Slave_Ipfrom);
                    mySqlCmd.CommandText = sql.ToString();
                    i = mySqlCmd.ExecuteNonQuery();
                    sql.Clear();
                    sql.AppendFormat(@" update order_detail set slave_id='{0}' where detail_id in {1};", os.Slave_Id, detail_id);
                    mySqlCmd.CommandText = sql.ToString();
                    i = mySqlCmd.ExecuteNonQuery();

                }
                mySqlCmd.Transaction.Commit();
                return dm.deliver_id.ToString();
            }
            catch (Exception ex)
            {

                mySqlCmd.Transaction.Rollback();
                return null;
                throw new Exception("DeliverDetailDao-->Split-->" + ex.Message + sql.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }

        }
        #region 更新簡訊時間
        /// <summary>
        /// 查詢smsid
        /// </summary>
        /// <param name="sms"></param>
        /// <returns></returns>
        public string GetSmsId(Sms sms)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT id from sms where memo='{0}' and send=0;", sms.memo);
            try
            {
                DataTable sm = _access.getDataTable(sql.ToString());
                if (sm.Rows.Count > 0)
                {
                    return sm.Rows[0]["id"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->GetSmsId-->" + ex.Message + sql.ToString(), ex);
            }
        }
        /// <summary>
        /// 變更出貨簡訊時間
        /// </summary>
        /// <param name="deliver_id">出貨單號</param>
        /// <param name="sms_date">簡訊時間</param>
        /// <param name="sms_id">SMS表中對應的數據id</param>
        /// <returns></returns>
        public int UpSmsTime(string deliver_id, string sms_date, string sms_id)
        {
            /*
             第一步：更新SMS表中簡訊時間和簡訊內容
             第二步：更新deliver_m表的sms_date字段
             */
            int user_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@" SELECT order_id,type,CASE freight_set WHEN 1 THEN '常溫' WHEN 2 THEN '冷凍' ELSE '冷藏' END AS freight_set,");
            sql.AppendFormat(@" delivery_mobile,delivery_status FROM deliver_master where deliver_id='{0}' limit 1;", deliver_id);
            DataTable dt = _access.getDataTable(sql.ToString());
            sql.Clear();
            if (dt.Rows.Count > 0)
            {
                string content = "[吉甲地出貨通知]您好，訂單" + dt.Rows[0]["order_id"] + "(" + dt.Rows[0]["freight_set"] + ")已於今日" + sms_date + " 16:30:00" + "寄出預計明日配達，若有收件問題可回覆此簡訊，客服人員會與您聯繫處理。";
                sql.AppendFormat(@" update sms set estimated_send_time='{0}', content='{1}', ", sms_date, content);
                sql.AppendFormat(@" modified='{0}' where id='{1}';", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), sms_id);
                sql.AppendFormat(@" update deliver_master set sms_date='{0}', ", sms_date + " 16:30:00");
                sql.AppendFormat(@" modified='{0}',verifier='{1}' where deliver_id='{2}';", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), user_id, deliver_id);
            }
            try
            {
                if (sql.Length > 0)
                {
                    return _access.execCommand(sql.ToString());
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->UpSmsTime-->" + ex.Message + sql.ToString(), ex);
            }

        }
        #endregion
        public string Shrink(string deliver_id)
        {
            StringBuilder sql = new StringBuilder();
            string allsql = string.Empty;
            sql.AppendFormat(@" SELECT id from deliver_detail where deliver_id='{0}';", deliver_id);
            if (_access.getDataTable(sql.ToString()).Rows.Count == 0)
            {
                sql.Clear();
                sql.AppendFormat(@"DELETE FROM deliver_master WHERE deliver_id='{0}';", deliver_id);
                allsql += sql.ToString();
            }
            else
            {
                sql.Clear();
                sql.AppendFormat(@" SELECT * from deliver_detail dd LEFT JOIN order_detail od on dd.detail_id=od.detail_id ");
                sql.AppendFormat(@" where od.detail_status not in(1, 89, 90) and deliver_id='{0}';", deliver_id);
                if (_access.getDataTable(sql.ToString()).Rows.Count == 0)
                {
                    sql.Clear();
                    sql.AppendFormat(@" UPDATE deliver_master set delivery_status=6 where deliver_id='{0}';", deliver_id);//取消出貨
                    allsql += sql.ToString();
                }
            }
            return allsql;
        }
        /// <summary>
        /// 編輯
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="type">區分變更到貨時間還是變更收件人資料</param>
        /// <returns></returns>
        public int DeliverMasterEdit(DeliverMaster dm, int type)
        {
            StringBuilder sql = new StringBuilder();
            int user_id = (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id;
            try
            {
                sql.AppendFormat(@" update deliver_master set ");
                if (type == 1)
                {
                    if (dm.estimated_delivery_date != DateTime.MinValue)
                    {
                        sql.AppendFormat(@" estimated_delivery_date='{0}',", dm.estimated_delivery_date.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        sql.AppendFormat(@" estimated_delivery_date=null,");
                    }
                    if (dm.estimated_arrival_date != DateTime.MinValue)
                    {
                        sql.AppendFormat(@" estimated_arrival_date='{0}',", dm.estimated_arrival_date.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        sql.AppendFormat(@" estimated_arrival_date=null,");
                    }
                    sql.AppendFormat(@" estimated_arrival_period='{0}' ", dm.estimated_arrival_period);
                }
                if (type == 2)
                {
                    sql.AppendFormat(@" delivery_name='{0}',delivery_mobile='{1}',delivery_phone='{2}',", dm.delivery_name, dm.delivery_mobile, dm.delivery_phone);
                    sql.AppendFormat(@" delivery_zip='{0}',delivery_address='{1}'", dm.delivery_zip, dm.delivery_address);
                }
                sql.AppendFormat(@" ,modified='{0}',verifier='{1}' ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), user_id);
                sql.AppendFormat(@" where deliver_id='{0}';", dm.deliver_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->DeliverMasterEdit-->" + ex.Message + sql.ToString(), ex);
            }
        }
        public string Send(int type, string order_id, string mobile, string subject, string content, string estimated_send_time, string memo)
        {
            StringBuilder sql = new StringBuilder();
            Sms sms = new Sms();
            sms.type = type;
            sms.order_id = int.Parse(order_id);
            sms.mobile = mobile;
            sms.subject = subject;
            sms.content = content;
            sms.memo = memo;
            SmsLog smslog = new SmsLog();
            if (!string.IsNullOrEmpty(estimated_send_time))
            {
                sms.estimated_send_time = DateTime.Parse(estimated_send_time);
            }
            else
            {
                smslog.provider = 0;
                smslog.success = 0;
                smslog.code = "0";
                smslog.free_sms_id = "";
                if (smslog.free_sms_id.Length >= 12)
                {
                    sms.sms_number = smslog.free_sms_id;
                }
                sms.send = smslog.success;
            }
            sql.AppendFormat(@" select max(id) from sms;");
            DataTable dt = _access.getDataTable(sql.ToString());
            if (dt.Rows.Count > 0)
            {
                smslog.sms_id = int.Parse(dt.Rows[0][0].ToString()) + 1;
            }
            sql.Clear();
            sql.AppendFormat(@" insert into sms (type,order_id,mobile,subject,");
            sql.AppendFormat(@"content,estimated_send_time,send,sms_number,");
            sql.AppendFormat(@"trust_send,memo,created,modified) values(");
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", sms.type, sms.order_id, sms.mobile, sms.subject);
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", sms.content, sms.estimated_send_time.ToString("yyyy-MM-dd HH:mm:ss"), sms.send, sms.sms_number);
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}');", sms.trust_send, sms.memo, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            sql.AppendFormat(@" insert into sms_log (sms_id,provider,success,code,");
            sql.AppendFormat(@"free_sms_id,created,modified ) values(");
            sql.AppendFormat(@"'{0}','{1}','{2}','{3}',", smslog.sms_id, smslog.provider, smslog.success, smslog.code);
            sql.AppendFormat(@"'{0}','{1}','{2}');", smslog.free_sms_id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), smslog.modified.ToString("yyyy-MM-dd HH:mm:ss"));

            return sql.ToString();
        }
        #endregion
        public List<DeliverDetailQuery> OrderDetailRestructuring(List<DeliverDetailQuery> ddq, string type = "0", bool since = false)
        {
            Dictionary<uint, List<DeliverDetailQuery>> OneProduct = new Dictionary<uint, List<DeliverDetailQuery>>();//單一商品
            Dictionary<uint, List<DeliverDetailQuery>> Combination = new Dictionary<uint, List<DeliverDetailQuery>>();//組合商品
            ArrayList CombinationHead = new ArrayList();//組合商品名稱
            ArrayList CombinationTail = new ArrayList();//組合商品中子商品名稱
            // ArrayList NewOrderDetail = new ArrayList();//新商品資料
            ArrayList SinceOrder = new ArrayList();//自出商品
            Dictionary<uint, uint> ProductFreightSet = new Dictionary<uint, uint> { { 1, 1 }, { 2, 2 }, { 3, 1 }, { 4, 2 }, { 5, 5 }, { 6, 5 } };
            Dictionary<uint, string> FreightSet = new Dictionary<uint, string> { { 1, "常溫" }, { 2, "冷凍" }, { 5, "冷藏" } };
            List<DeliverDetailQuery> newStore = new List<DeliverDetailQuery>();//新商品資料
            List<DeliverDetailQuery> ndd = new List<DeliverDetailQuery>();
            foreach (var item in ddq)
            {
                if (item.combined_mode > 1)//order_detail.combined_mode	組合商品 0:一般 1:組合 2:子商品
                {
                    if (item.item_mode == 1)//order_detail.item_mode	0:單一商品, 1:父商品, 2:子商品
                    {
                        CombinationHead.Add(item);
                    }
                    else
                    {
                        CombinationTail.Add(item);
                    }
                }
                else
                {
                    if (since)
                    {
                        if (type == "2")//deliver_master.type	出貨類別,1:統倉出貨,2:供應商自行出貨,3:供應商調度出貨,4:退貨,5:退貨瑕疵,6:瑕疵(目前數據中只有1和2兩種)
                        {
                            SinceOrder.Add(item);
                        }
                    }
                    uint fset = 0;
                    ndd.Add(item);
                    if (ProductFreightSet.TryGetValue(item.product_freight_set, out fset))
                    {

                    }
                    if (OneProduct.ContainsKey(fset))
                    {
                        OneProduct[fset] = ndd;
                    }
                    else
                    {
                        OneProduct.Add(fset, ndd);
                    }

                }
            }
            uint freightset = 0;
            ndd = new List<DeliverDetailQuery>();
            foreach (var item in CombinationHead)
            {
                DeliverDetailQuery dq = new DeliverDetailQuery();
                dq = (DeliverDetailQuery)item;
                if (ProductFreightSet.TryGetValue(dq.product_freight_set, out freightset))
                {

                }

                ndd.Add(dq);
                if (Combination.ContainsKey(freightset))
                {

                    Combination[freightset] = ndd;
                }
                else
                {
                    Combination.Add(freightset, ndd);
                }
                //Combination.Add(freightset,ndd);
                foreach (var i in CombinationTail)
                {
                    DeliverDetailQuery dqt = new DeliverDetailQuery();
                    dqt = (DeliverDetailQuery)i;
                    //如果子商品是此父商品下的子商品
                    if (dq.parent_id == dqt.parent_id && dq.pack_id == dqt.pack_id)
                    {
                        dqt.buy_num = dqt.buy_num * dqt.parent_num;
                        ndd.Add(dqt);
                        if (Combination.ContainsKey(freightset))
                        {

                            Combination[freightset] = ndd;
                        }
                        else
                        {
                            Combination.Add(freightset, ndd);
                        }
                    }
                }
            }

            foreach (var item in FreightSet)
            {
                if (OneProduct.ContainsKey(item.Key))
                {
                    newStore.AddRange(OneProduct[item.Key]);
                }
                if (Combination.ContainsKey(item.Key))
                {
                    newStore.AddRange(Combination[item.Key]);
                }

            }
            foreach (var item in SinceOrder)
            {
                DeliverDetailQuery sddq = new DeliverDetailQuery();
                sddq = (DeliverDetailQuery)item;
                newStore.Add(sddq);
            }
            return newStore;

        }
        #region 匯出PDF
        /// <summary>
        ///訂單出貨明細和 出貨明細
        /// </summary>
        /// <param name="deliver_id"></param>
        /// <param name="type">1 出貨明細 0 訂單出貨明細</param>
        /// <returns></returns>
        public DataTable GetOrderDelivers(string deliver_id, int type = 0)
        {
            //type=0 订单出货明细 1 出货明细
            StringBuilder sql = new StringBuilder();
            //sql.AppendLine(@" SELECT dm.deliver_id,dm.freight_set,dm.delivery_name,dm.delivery_mobile,dm.delivery_zip,dm.delivery_address,");
            //sql.AppendLine(@"            dm.type,dm.delivery_store,dm.estimated_arrival_period,om.order_id,om.order_name,");
            //sql.AppendLine(@"            om.order_createdate,om.order_date_pay,om.money_collect_date,");
            //sql.AppendLine(@"            om.note_order, om.deduct_happygo_convert,om.order_freight_normal,");
            //sql.AppendLine(@"            om.order_freight_low,om.channel,channel.channel_name_simple,om.retrieve_mode,");
            //sql.AppendLine(@"            om.priority, om.holiday_deliver,");
            //sql.AppendLine(@"            od.item_id,od.product_name,od.product_spec_name,");
            //sql.AppendLine(@"            od.product_freight_set,od.single_price,od.single_money,od.buy_num,od.product_mode,");
            //sql.AppendLine(@"            od.combined_mode,od.item_mode,od.parent_id,od.parent_name,");
            //sql.AppendLine(@"            od.detail_id,od.pack_id,od.parent_num,vb.brand_name ");
            //sql.AppendLine(@"            from deliver_master dm LEFT JOIN order_master om on dm.order_id=om.order_id");
            //sql.AppendLine(@"            LEFT JOIN channel channel on channel.channel_id=om.channel");
            //sql.AppendLine(@"            LEFT JOIN deliver_detail dd on dd.deliver_id=dm.deliver_id");
            //sql.AppendLine(@"            LEFT JOIN order_detail od on dd.detail_id=od.detail_id");
            //sql.AppendLine(@"            LEFT JOIN product_item pi on pi.item_id=od.item_id");
            //sql.AppendLine(@"            LEFT JOIN product p on p.product_id=pi.product_id");
            //sql.AppendLine(@"            LEFT JOIN vendor_brand vb on vb.brand_id=p.brand_id");
            //sql.AppendFormat(@"          where dm.deliver_id='{0}' and dm.delivery_status <>6 ", deliver_id);
            if (type == 0)
            {
                sql.AppendLine(@" SELECT dm.deliver_id, dm.freight_set, dm.delivery_name, dm.delivery_mobile, dm.delivery_zip, ");
                sql.AppendLine(@"dm.delivery_address, dm.delivery_store, dm.estimated_arrival_period,om.order_id, om.order_name, ");
                sql.AppendLine(@"om.order_createdate, om.order_date_pay, om.money_collect_date, om.note_order, om.deduct_happygo_convert,");
                sql.AppendLine(@"om.order_freight_normal,om.order_freight_low,om.channel,om.retrieve_mode,om.priority,om.holiday_deliver,channel.channel_name_simple ");
            }
            if (type == 1)
            {
                sql.AppendLine(@"SELECT dm.type,dm.deliver_id,dm.order_id,dm.delivery_name,dm.freight_set,");
                sql.AppendLine(@"dm.estimated_arrival_period,om.order_name,om.order_createdate,om.order_date_pay,");
                sql.AppendLine(@"om.money_collect_date");

            }
            sql.AppendLine(@" FROM deliver_master AS dm LEFT JOIN order_master AS om ON (om.order_id = dm.order_id)  ");
            sql.AppendLine(@"        LEFT JOIN channel channel on channel.channel_id=om.channel ");
            sql.AppendFormat(@" WHERE deliver_id ='{0}' AND delivery_status != 6 ", deliver_id);
            if (type == 0)
            {
                sql.AppendFormat(@"  AND type = 1  ORDER BY om.priority DESC,om.order_id ASC; ");
            }
            if (type == 1)
            {
                sql.AppendFormat(@" ORDER BY dm.order_id ASC; ");
            }
            DataTable deliver = _access.getDataTable(sql.ToString());
            sql.Clear();
            DataTable detail = new DataTable();
            foreach (DataRow dr in deliver.Rows)
            {
                sql.AppendLine(@"SELECT dm.deliver_id as ddeliver_id,dm.type as dtype,od.item_id,od.product_name,od.product_spec_name,od.product_freight_set,");
                sql.AppendLine(@"  od.single_price, od.single_money, od.buy_num, od.product_mode,od.combined_mode,od.item_mode, od.parent_id,");
                sql.AppendLine(@"  od.parent_name, od.detail_id, od.pack_id, od.parent_num,vb.brand_name ");
                sql.AppendLine(@"  FROM deliver_master AS dm ");
                sql.AppendLine(@"  LEFT JOIN deliver_detail AS dd ON (dm.deliver_id = dd.deliver_id) ");
                sql.AppendLine(@"  LEFT JOIN order_detail AS od ON (od.detail_id = dd.detail_id) ");
                sql.AppendLine(@"  LEFT JOIN product_item AS pi ON (pi.item_id = od.item_id) ");
                sql.AppendLine(@"  LEFT JOIN product AS p ON (p.product_id = pi.product_id) ");
                sql.AppendLine(@"  LEFT JOIN vendor_brand AS vb ON (vb.brand_id = p.brand_id)  ");
                if (type == 0)
                {
                    sql.AppendFormat(@"  WHERE dm.order_id ='{0}' AND dm.type IN (1, 2, 3, 4, 5, 6) AND od.detail_status IN (2, 4, 6, 7)   ORDER BY dm.type ASC, dm.deliver_id ASC, od.item_id ASC;", dr["order_id"]);
                }
                else
                {
                    sql.AppendFormat(@" WHERE dm.deliver_id ='{0}' AND dm.delivery_status != 6 ", dr["deliver_id"]);
                    sql.AppendFormat(@" AND od.detail_status in (2,3,4,6,7) ORDER BY  dm.deliver_id ASC, od.detail_id ASC; ");
                }
                detail = _access.getDataTable(sql.ToString());

            }
            if (type == 0)
            {
                DataColumn dc = new DataColumn("receivable", typeof(int));
                deliver.Columns.Add(dc);
                #region 應收金額
                foreach (DataRow dr in deliver.Rows)
                {
                    //不是貨到付款的物流方式
                    if (dr["delivery_store"].ToString() != "10" && dr["delivery_store"].ToString() != "17")
                    {
                        dr["receivable"] = 0;
                    }
                    else
                    {
                        DataTable dt = GetReceivable(dr["deliver_id"].ToString());
                        int product_subtotal = 0;
                        int deduct_bonus = 0;
                        int deduct_welfare = 0;
                        int deduct_happygo = 0;
                        double deduct_happygo_convert = (!string.IsNullOrEmpty(dr["deduct_happygo_convert"].ToString())) ? double.Parse(dr["deduct_happygo_convert"].ToString()) : 0;
                        int fare = 0;
                        if (dt.Rows.Count > 0)
                        {
                            product_subtotal = int.Parse(dt.Rows[0]["product_subtotal"].ToString());
                            deduct_bonus = int.Parse(dt.Rows[0]["deduct_bonus"].ToString());
                            deduct_welfare = int.Parse(dt.Rows[0]["deduct_welfare"].ToString());
                            deduct_happygo = int.Parse(dt.Rows[0]["deduct_happygo"].ToString());
                        }
                        if (dr["freight_set"].ToString() == "1")
                        {
                            fare = (!string.IsNullOrEmpty(dr["order_freight_normal"].ToString())) ? int.Parse(dr["order_freight_normal"].ToString()) : 0;
                        }
                        else
                        {
                            fare = (!string.IsNullOrEmpty(dr["order_freight_low"].ToString())) ? int.Parse(dr["order_freight_low"].ToString()) : 0;
                        }
                        dr["receivable"] = product_subtotal + fare - deduct_bonus - deduct_welfare - Math.Round(Decimal.Parse((deduct_happygo * deduct_happygo_convert).ToString()), 0);
                    }

                }
            }
                #endregion
            deliver.Merge(detail, true, MissingSchemaAction.AddWithKey);
            return deliver;
        }
        /// <summary>
        /// 應收金額
        /// </summary>
        /// <param name="deliver_id"></param>
        /// <returns></returns>
        public DataTable GetReceivable(string deliver_id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT sum(od.single_money*buy_num)  as product_subtotal, sum(od.deduct_bonus) as deduct_bonus,");
            sql.AppendLine(@"sum(od.deduct_welfare) as deduct_welfare,sum(od.deduct_happygo) as deduct_happygo   from deliver_master dm LEFT JOIN deliver_detail dd on dm.deliver_id=dd.deliver_id");
            sql.AppendLine(@"LEFT JOIN order_detail od on dd.detail_id=od.detail_id");
            sql.AppendFormat(@" where od.detail_status not in(89,90,91,92) and od.item_mode <>2 and dm.deliver_id='{0}'  limit 1;", deliver_id);

            return _access.getDataTable(sql.ToString());
        }
        /// <summary>
        /// 货运单
        /// </summary>
        /// <param name="deliver_id"></param>
        /// <returns></returns>
        public DataTable GetWayBills(string deliver_id, string ticketids)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT  dm.deliver_id,dm.freight_set,dm.order_id,dm.delivery_store,");
            sql.AppendLine(@" CASE when om.retrieve_mode=1 THEN '大智通-電子商務組' ELSE  dm.delivery_name END as delivery_name,");
            sql.AppendLine(@" CASE WHEN  om.retrieve_mode=1 THEN '' ELSE dm.delivery_mobile END as delivery_mobile,");
            sql.AppendLine(@" CASE WHEN  om.retrieve_mode=1 THEN '02-26687071' ELSE dm.delivery_phone END AS delivery_phone,");
            sql.AppendLine(@" CASE WHEN  om.retrieve_mode=1 THEN '238' ELSE dm.delivery_zip END AS delivery_zip,");
            sql.AppendLine(@" CASE WHEN  om.retrieve_mode=1 THEN '佳園路2段70-1號' ELSE dm.delivery_address END AS delivery_address,");
            sql.AppendLine(@" dm.estimated_delivery_date,dm.estimated_arrival_period,om.order_name,");
            sql.AppendLine(@" om.order_zip,om.order_address,om.note_order,om.deduct_happygo_convert,");
            sql.AppendLine(@" om.order_freight_normal,om.order_freight_low,om.retrieve_mode,om.holiday_deliver,om.order_payment, ");
            sql.AppendLine(@" om.deliver_stno,om.dcrono, om.stnm,om.money_collect_date  ");
            sql.AppendLine(@" from deliver_master dm LEFT JOIN order_master om on dm.order_id=om.order_id");
            sql.AppendFormat(@" where delivery_status !=6 and dm.deliver_id in ");
            //dm.deliver_id,
            //        dm.freight_set,
            //        dm.order_id,
            //        dm.delivery_store,
            //        dm.delivery_name,
            //        dm.delivery_mobile,
            //        dm.delivery_phone,
            //        dm.delivery_zip,
            //        dm.delivery_address,
            //        dm.estimated_delivery_date,
            //        dm.estimated_arrival_period,
            //        om.order_name,
            //        om.delivery_name,
            //        om.order_zip,
            //        om.order_address,
            //        om.note_order,
            //        om.deduct_happygo_convert,
            //        om.order_freight_normal,
            //        om.order_freight_low,
            //        om.retrieve_mode,
            //        om.holiday_deliver,
            //        om.deliver_stno,
            //        om.dcrono,
            //        om.stnm,
            //        om.order_payment,
            //        om.money_collect_date
            if (string.IsNullOrEmpty(ticketids))
            {
                sql.AppendFormat(@"({0}) ", deliver_id);
            }
            else
            {
                sql.AppendFormat(@" (SELECT deliver_id from deliver_master where ticket_id in ({0}))", ticketids);
            }
            DataTable bills = _access.getDataTable(sql.ToString());
            DataColumn dc = new DataColumn("receivable", typeof(int));
            bills.Columns.Add(dc);
            #region 應收金額
            foreach (DataRow dr in bills.Rows)
            {
                //不是貨到付款的物流方式
                if (dr["delivery_store"].ToString() != "10" && dr["delivery_store"].ToString() != "17" && dr["money_collect_date"].ToString() != "0")
                {
                    dr["receivable"] = 0;
                }
                else
                {
                    DataTable dt = GetReceivable(dr["deliver_id"].ToString());
                    int product_subtotal = 0;
                    int deduct_bonus = 0;
                    int deduct_welfare = 0;
                    int deduct_happygo = 0;
                    double deduct_happygo_convert = (!string.IsNullOrEmpty(dr["deduct_happygo_convert"].ToString())) ? double.Parse(dr["deduct_happygo_convert"].ToString()) : 0;
                    int fare = 0;
                    if (dt.Rows.Count > 0)
                    {
                        product_subtotal = int.Parse(dt.Rows[0]["product_subtotal"].ToString());
                        deduct_bonus = int.Parse(dt.Rows[0]["deduct_bonus"].ToString());
                        deduct_welfare = int.Parse(dt.Rows[0]["deduct_welfare"].ToString());
                        deduct_happygo = int.Parse(dt.Rows[0]["deduct_happygo"].ToString());
                    }
                    if (dr["freight_set"].ToString() == "1")
                    {
                        fare = (!string.IsNullOrEmpty(dr["order_freight_normal"].ToString())) ? int.Parse(dr["order_freight_normal"].ToString()) : 0;
                    }
                    else
                    {
                        fare = (!string.IsNullOrEmpty(dr["order_freight_low"].ToString())) ? int.Parse(dr["order_freight_low"].ToString()) : 0;
                    }
                    dr["receivable"] = product_subtotal + fare - deduct_bonus - deduct_welfare - Math.Round(Decimal.Parse((deduct_happygo * deduct_happygo_convert).ToString()), 0);
                }

            }
            #endregion
            return bills;
        }
        #endregion
        #region 新增
        /// <summary>
        /// 新增deliver_detail數據
        /// </summary>
        /// <param name="dd">DeliverDetail對象</param>
        /// <returns>數據庫操作受影響行數</returns>
        public int Add(DeliverDetail dd)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"insert into deliver_detail(deliver_id,detail_id,delivery_status) values({0},{1},{2})", dd.deliver_id, dd.detail_id, dd.delivery_status);
            try
            {
                return _access.execCommand(sbSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverMasterDao-->Add-->" + ex.Message + sbSql.ToString(), ex);
            }
        }
        #endregion
        #region 外站出貨檔匯出
        public object GetChannelOrderList(DeliverMasterQuery dmq, out int totalCount, int type = 0)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfromm = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            totalCount = 0;
            try
            {
                sql.AppendFormat(@"SELECT om.retrieve_mode,tp1.parameterName as sretrieve_mode,om.channel,channel.channel_name_simple AS schannel,");
                sql.AppendFormat(@"om.channel_order_id,om.order_id as od_order_id,dm.export_id,v.vendor_name_simple AS sexport_id,dm.freight_set,dm.delivery_store,");
                sql.AppendFormat(@"tp2.parameterName AS sdelivery_store,dm.deliver_id,dm.delivery_code,dm.delivery_date,od.sub_order_id,dd.delivery_status AS dd_status");
                sqlfromm.AppendFormat(@" FROM order_master om ");
                sqlfromm.AppendFormat(@"INNER JOIN deliver_master dm USING (order_id) ");
                sqlfromm.AppendFormat(@"INNER JOIN deliver_detail dd USING (deliver_id) ");
                sqlfromm.AppendFormat(@"INNER JOIN order_detail od USING (detail_id) ");
                sqlfrom.AppendFormat(@" LEFT JOIN (SELECT parameterCode,parameterName FROM t_parametersrc where parameterType = 'retrieve_mode' )  tp1 ON om.retrieve_mode = tp1.parameterCode  ");
                sqlfrom.AppendFormat(@" LEFT JOIN (SELECT parameterCode,parameterName FROM t_parametersrc where parameterType = 'Deliver_Store' )  tp2 ON dm.delivery_store = tp2.parameterCode ");
                sqlfrom.AppendFormat(@"LEFT JOIN channel channel ON channel.channel_id = om.channel ");
                sqlfrom.AppendFormat(@"LEFT JOIN vendor v ON v.vendor_id = dm.export_id ");
                sqlfrom.AppendFormat(@"WHERE 1 = 1");

                if (dmq.od_order_id != 0)
                {
                    sqlwhere.AppendFormat(@" AND om.order_id='{0}' ", dmq.od_order_id);
                }
                if (!string.IsNullOrEmpty(dmq.channel_order_id))
                {
                    sqlwhere.AppendFormat(@" AND om.channel_order_id ='{0}' ", dmq.channel_order_id);
                }
                if (!string.IsNullOrEmpty(dmq.sub_order_id))
                {
                    sqlwhere.AppendFormat(@" AND od.sub_order_id ='{0}' ", dmq.sub_order_id);
                }
                if (dmq.delivery_store != 0)
                {
                    sqlwhere.AppendFormat(@" AND dm.delivery_store ='{0}'", dmq.delivery_store);
                }
                if (dmq.channel != 0)
                {
                    sqlwhere.AppendFormat(@" AND  om.channel ='{0}'", dmq.channel);
                }
                if (dmq.dd_status != -1)
                {
                    sqlwhere.AppendFormat(@" AND dd.delivery_status ='{0}'", dmq.dd_status);
                }
                if (dmq.retrieve_mode != -1)
                {
                    sqlwhere.AppendFormat(@" AND om.retrieve_mode ='{0}' ", dmq.retrieve_mode);
                }
                if (!string.IsNullOrEmpty(dmq.sqlwhere))
                {
                    sqlwhere.AppendFormat(dmq.sqlwhere);
                }
                sql.Append(sqlfromm.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
                if (dmq.IsPage)
                {
                    DataTable dt = _access.getDataTable("select count(om.order_id) " + sqlfromm.ToString() + " where 1=1 " + sqlwhere.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        totalCount = int.Parse(dt.Rows[0][0].ToString());
                    }
                }
                if (type == 0)
                {
                    sql.AppendFormat(" limit {0},{1} ;", dmq.Start, dmq.Limit);
                    return _access.getDataTableForObj<DeliverMasterQuery>(sql.ToString());
                }
                else
                {
                    sql.Clear();
                    string outcsv = "select sub_order_id,delivery_code " + sqlfromm.ToString() + " where 1=1 " + sqlwhere.ToString();
                    sql.AppendFormat(outcsv);
                    return _access.getDataTable(sql.ToString());
                }

            }
            catch (Exception ex)
            {
                throw new Exception(" DeliverDetailDao-->GetChannelOrderList-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion
    }
}
