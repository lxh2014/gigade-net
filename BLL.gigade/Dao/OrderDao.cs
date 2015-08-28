using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Dao
{
    public class OrderDao : IOrderImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public OrderDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }
        public static string Description = "";//供應商ID+名稱+批次出貨
        public static string IPAddress = "";//獲取本機的IP地址
        #region 調度商品出貨


        /// <summary>
        /// 調度商品出貨
        /// </summary>
        /// <param name="rows">要出的商品</param>
        /// <param name="order">商品詳細信息</param>
        /// <param name="master"></param>
        /// <param name="Descriptions">描述</param>
        /// <returns></returns>
        public bool ThingsMethod(string[] rows, OrderDeliver order, OrderSlaveMaster master, string Descriptions)
        {
            Description = Descriptions;
            System.Net.IPAddress[] addlist = Dns.GetHostByName(Dns.GetHostName()).AddressList;
            if (addlist.Length > 0)
            {
                IPAddress = addlist[0].ToString();
            }
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
                SerialDao serial = new SerialDao(connStr);
                mySqlCmd.CommandText = serial.Update(64);// 廠商出貨單總表流水號
                int Master_Slave_Id = int.Parse(mySqlCmd.ExecuteScalar().ToString());//--用這個值新增至出貨單詳情表(order_slave_detail),出貨單主表(order_slave_master)
                foreach (var item in rows)//循環讀取數據
                {
                    sb.AppendFormat(@"select order_id from order_slave  where slave_id='{0}';", item);
                    mySqlCmd.CommandText = sb.ToString();
                    string Order_Id = mySqlCmd.ExecuteScalar().ToString();//--訂單編號,後面會用來修改付款單狀態和查詢訂單流水號詳情
                    sb.Clear();

                    mySqlCmd.CommandText = serial.Update(41);// 廠商出貨單總表流水號
                    int Deliver_Id = int.Parse(mySqlCmd.ExecuteScalar().ToString());//用來新增信息到物流單

                    sb.Append(@"insert into order_deliver (deliver_id,slave_id,deliver_status,deliver_store,deliver_code,deliver_time,");///*新增物流單*/
                    sb.Append("deliver_note,deliver_createdate,deliver_updatedate,deliver_ipfrom )value(");
                    sb.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", Deliver_Id, item, 1, order.deliver_store, order.deliver_code);
                    sb.AppendFormat("'{0}','{1}','{2}','{3}','{4}');", order.deliver_time, order.deliver_note, CommonFunction.GetPHPTime(DateTime.Now.ToString()), 0, order.deliver_ipfrom);
                    mySqlCmd.CommandText = sb.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    sb.Clear();
                    int[] arrays = { 2, 3 };//通過這個數組查詢訂單詳情

                    #region vendor.gigade100.com/order/all_order_deliver.php第357行
                    //vendor.gigade100.com/order/all_order_deliver.php第357行
                    sb.Append(update_detail_for_product_mode(item, arrays, order.deliver_time));//執行事務-參考自includes/order/order.php第1672行方法
                    mySqlCmd.CommandText = sb.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    sb.Clear();
                    //vendor.gigade100.com/order/all_order_deliver.php第360行
                    sb.Append(order_slave_status_record(item, 6, Description));
                    mySqlCmd.CommandText = sb.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    sb.Clear();
                    #endregion
                    // vendor.gigade100.com/order/all_order_deliver.php第361行
                    string Modify_Master_Order_Status = check_vendor_deliver_order_master_status(Order_Id);
                    if (Modify_Master_Order_Status == "false")//參考自(vendor.gigade100.com/order/all_order_deliver.php第363行方法)
                    {
                        mySqlCmd.Transaction.Rollback();
                    }
                    //vendor.gigade100.com/order/all_order_deliver.php第367
                    sb.Append(modify_order_master_status(Order_Id, Modify_Master_Order_Status));
                    mySqlCmd.CommandText = sb.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    sb.Clear();
           
                    #region vendor.gigade100.com/order/all_order_deliver.php第368行方法*/
                    //vendor.gigade100.com/order/all_order_deliver.php第368行方法*/
                    sb.Append(order_master_status_record(Order_Id, Modify_Master_Order_Status, Description));
                    mySqlCmd.CommandText = sb.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    sb.Clear();
                    #endregion


                    //寫入批次出貨單細項
                    /*vendor.gigade100.com/order/all_order_deliver.php 第370行*/
                    sb.AppendFormat(@"INSERT INTO order_slave_detail  (slave_master_id, slave_id) VALUES ('{0}','{1}');", Master_Slave_Id, item);
                    mySqlCmd.CommandText = sb.ToString();
                    mySqlCmd.ExecuteNonQuery();
                    sb.Clear();

                }
                //批次出貨單
                /*vendor.gigade100.com/order/all_order_deliver.php 第384行*/
                sb.AppendFormat("select count(*)  from order_slave_master where 1=1 and creator='{0}' and code_num='{1}';", master.creator, master.code_num);//供应商编号，批次编号
                mySqlCmd.CommandText = sb.ToString();
                int Total_Search = int.Parse(mySqlCmd.ExecuteScalar().ToString());
                sb.Clear();
                Total_Search += 1;
                //vendor.gigade100.com/order/all_order_deliver.php 第412行行方法*/
                sb.Append(@"insert into order_slave_master(slave_master_id,code_num,paper,order_freight_normal,order_freight_low,");
                sb.Append("normal_subtotal,hypothermia_subtotal,deliver_store,deliver_code,deliver_time,deliver_note,createdate,creator) value( ");
                sb.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", Master_Slave_Id, master.code_num, Total_Search, master.order_freight_normal, master.order_freight_low);
                sb.AppendFormat("'{0}','{1}','{2}','{3}','{4}',", master.normal_subtotal, master.hypothermia_subtotal, order.deliver_store, order.deliver_code, master.deliver_time);
                sb.AppendFormat("'{0}','{1}','{2}');", order.deliver_note, CommonFunction.GetPHPTime(DateTime.Now.ToString()), master.creator);
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();

                mySqlCmd.Transaction.Commit();
                // mySqlCmd.Transaction.Rollback();//---功能完成，用来测试
                return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderDao.ThingsMethod -->" + ex.Message + sb.ToString(), ex);
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

        #region 自出商品出貨
        public bool SelfThingsMethod( DataTable _dtSms ,OrderDeliver query, string Descriptions)
        {
             Description = Descriptions;
           
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
                sb.AppendFormat(@"select order_id from order_slave  where slave_id='{0}';", query.slave_id);
                mySqlCmd.CommandText = sb.ToString();
                string Order_Id = mySqlCmd.ExecuteScalar().ToString();//--訂單編號,後面會用來修改付款單狀態和查詢訂單流水號詳情
                sb.Clear();

                SerialDao serial = new SerialDao(connStr);
           
                sb.AppendFormat(serial.Update(41));  // 出貨單流水號參考自vendor.gigade100.com/order/orerd_deliver.php第226行
                sb.Append(@"insert into order_deliver(deliver_id,slave_id,deliver_status,deliver_store,deliver_code,deliver_time,deliver_note");
                sb.Append(",deliver_createdate,deliver_updatedate,deliver_ipfrom)value((select serial_value from serial where serial_id=41),");
                sb.AppendFormat(" '{0}','{1}','{2}','{3}','{4}',",query.slave_id,1,query.deliver_store,query.deliver_code,query.deliver_time);
                sb.AppendFormat(" '{0}','{1}',", query.deliver_note, CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
                sb.AppendFormat(" '{0}','{1}');", CommonFunction.GetPHPTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")), query.deliver_ipfrom);
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                int[] Product_Mode = { 1 };
                //參考自vendor.gigade100.com/order/orerd_deliver.php第246行
                sb.Append(update_detail_for_product_mode(query.slave_id.ToString(), Product_Mode,query.deliver_time));
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                //參考自vendor.gigade100.com/order/orerd_deliver.php第252行
                string Modify_Master_Order_Status = check_vendor_deliver_order_master_status(Order_Id);
                if (Modify_Master_Order_Status == "false")//參考自(vendor.gigade100.com/order/order_deliver.php第253行方法)
                {
                    mySqlCmd.Transaction.Rollback();
                }

                // 修改付款單狀態參考自(vendor.gigade100.com/order/order_deliver.php第259行方法)
                sb.Append(modify_order_master_status(Order_Id, Modify_Master_Order_Status));
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();
                // 新增付款單狀態參考自(vendor.gigade100.com/order/order_deliver.php第263行方法)
                sb.Append(order_master_status_record(Order_Id, Modify_Master_Order_Status,Descriptions));
                mySqlCmd.CommandText = sb.ToString();
                mySqlCmd.ExecuteNonQuery();
                sb.Clear();

                // 異動商品明細狀態為已出貨態 參考自(vendor.gigade100.com/order/order_deliver.php第266行方法)
                //簡訊通知
                //參考自(vendor.gigade100.com/order/order_deliver.php第269行方法)
             List<OrderDetailQuery> OrderDetailList=new List<OrderDetailQuery>();
             sb.AppendFormat(@"select item_id,product_name from order_detail where slave_id='{0}';", query.slave_id);
             OrderDetailList = _dbAccess.getDataTableForObj<OrderDetailQuery>(sb.ToString());
             sb.Clear();
             //參考自(vendor.gigade100.com/order/order_deliver.php第281行方法)
             sb.AppendFormat(@"select delivery_mobile from order_master where order_id='{0}';", Order_Id);
             OrderMaster ordermaster = new OrderMaster();
             ordermaster=_dbAccess.getSinggleObj<OrderMaster>(sb.ToString());
             sb.Clear();

                 foreach (var item in OrderDetailList)
                 {//參考自(vendor.gigade100.com/order/order_deliver.php第276行方法)
                     DataRow[] rows = _dtSms.Select("sms_id='" + item.Item_Id + "'");
                     foreach (DataRow row in rows)//篩選出的最多只有一條數據，
                     {
                         if (!string.IsNullOrEmpty(row["sms_id"].ToString()))
                         {
                             string content = "您好,您所訂購的商品 " + item.Product_Name + " 已於今天送出,到貨前會由宅配人員與您確認到貨時段, 謝謝您.吉甲地";
                             sb.AppendFormat(@"insert into sms(type,order_id,mobile,subject,content,estimated_send_time)value(4,'{0}',", Order_Id);
                             sb.AppendFormat(" '{0}','','{1}','{2}');", ordermaster.Delivery_Mobile, content, "00-00-00 00:00:00");
                             mySqlCmd.CommandText = sb.ToString();
                             mySqlCmd.ExecuteNonQuery();
                             sb.Clear();
                             break;
                         }
                     }
                 }
                 //---------------------------------------------這個PHP傳入的是Slave_id那個是錯誤的，但是我先傳入Order_id以便實現現在的功能
                 #region 參考自(vendor.gigade100.com/order/order_deliver.php第364行方法)


                 sb.AppendFormat(@"select user_id,order_name from order_master where order_id='{0}';", Order_Id);
                 ordermaster = _dbAccess.getSinggleObj<OrderMaster>(sb.ToString());
                 sb.Clear();
                 sb.AppendFormat(@"select user_email from users where user_id='{0}';", ordermaster.user_id);
                 Users user = new Users();
                 user = _dbAccess.getSinggleObj<Users>(sb.ToString());
                 sb.Clear();
            //    $aParamenter['s_username']	= $sOrder_Username;-----------------ordermaster.Order_Name;
            //$aParamenter['s_deliver_time']	= $sDeliver_Time;-----------------query.deliver_time
            //$aParamenter['s_deliver_store']	= $aLang['Deliver_Store'][$nDeliver_Store];----------------地圖，需要查詢
            //$aParamenter['s_deliver_code']	= $sDeliver_Code;-------------------query.deliver_code
            //$sFrom_Name			= $amego_config->get_single_config('system_mail_from_name');
            //$sFrom_Email			= $amego_config->get_single_config('system_mail_from_email');
            //$sMailer_Root			= MAIL_TEMPLATE_ROOT;
                 Config conf = new Config();
                 sb.AppendFormat(@"select *from config where config_name='system_mail_from_name';");
                 conf = _dbAccess.getSinggleObj<Config>(sb.ToString());
                 sb.Clear();
                 string From_Name = conf.config_value;//--------------------有了
                 sb.AppendFormat(@"select *from config where config_name='system_mail_from_email';");
                 conf = _dbAccess.getSinggleObj<Config>(sb.ToString());
                 sb.Clear();
                 string From_Email = conf.config_value;//--------------------有了
                 //出貨單通知信_新增
                 int Mailer_Number = 406;
                //----------------------------------下面的不知道怎麼賦值啊！！
                 string Mailer_Root = "";////define('MAIL_TEMPLATE_ROOT',			GIGADE_WWW_DIR . '/mailer/');			// 郵件版型位置,在各實體目錄中的相對位置
                 #endregion




                 mySqlCmd.Transaction.Rollback();
            return true;
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("OrderDao.SelfThingsMethod -->" + ex.Message + sb.ToString(), ex);
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

        /// <summary>
        /// 自出&調度並行
        /// 參考自(includes/order/order.php第1626行方法)
        /// </summary>
        /// <param name="Slave_Id"></param>
        /// <param name="Product_Mode"></param>
        /// <param name="Deliver_Time"></param>
        /// <returns></returns>
        public string update_detail_for_product_mode(string Slave_Id, int[] Product_Mode, uint Deliver_Time)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbt = new StringBuilder();
            StringBuilder Mode = new StringBuilder();
            if (Product_Mode != null && Product_Mode.Length > 0)
            {
                for (int i = 0; i < Product_Mode.Length; i++)
                {
                    Mode.Append(Product_Mode[i] + ",");
                }
            }
            int len = Mode.ToString().Length;
            if (len > 0)
            {
                Mode.Remove(Mode.ToString().LastIndexOf(','), 1);
            }
            if (Deliver_Time == 0)
            {
                Deliver_Time = uint.Parse((CommonFunction.GetPHPTime(DateTime.Now.ToString())).ToString());
            }
            try
            {
                sb.Append("set sql_safe_updates = 0;");
                sb.AppendFormat(@" update order_detail set detail_status='{0}' ", Product_Mode[0] == 1? 4 : 6);//已出貨，進倉中
                sb.AppendFormat(" where slave_id ='{0}'  and product_mode in('{1}') ", Slave_Id, Mode.ToString());
                sb.AppendFormat(" and detail_status ='{0}';", 2);//待出貨
                sb.Append("set sql_safe_updates = 1;");
                sbt.AppendFormat(@"SELECT COUNT(*) as search_total  FROM order_detail where slave_id='{0}' and  detail_status in(2,3,4,6,7,9) and detail_status=2 ;", Slave_Id);
                DataTable total = _dbAccess.getDataTable(sbt.ToString());
                sbt.Clear();

                int search_total = int.Parse(total.Rows[0]["search_total"].ToString());
                if (search_total == 0)
                {
                    sb.Append("set sql_safe_updates = 0;");
                    sb.AppendFormat(@"update order_slave set slave_status='{0}',slave_date_delivery='{1}'  WHERE   slave_id = '{2}';", 6, Deliver_Time, Slave_Id);//
                    sb.Append("set sql_safe_updates = 1;");
                }

                sbt.AppendFormat(@"select order_id from order_slave  where slave_id='{0}';", Slave_Id);
                total = _dbAccess.getDataTable(sbt.ToString());
                if (total.Rows.Count > 0)
                {
                    string order_id = total.Rows[0]["order_id"].ToString();
                    string Modify_Master_Order_Status = check_vendor_deliver_order_master_status(order_id);
                    sb.Append(modify_order_master_status(order_id, Modify_Master_Order_Status));
                    if (Modify_Master_Order_Status == "4")
                    {
                        sb.Append(order_master_status_record(order_id, Modify_Master_Order_Status, Modify_Master_Order_Status));
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDao.update_detail_for_product_mode -->" + ex.Message + sb.ToString(), ex);
            }

        }
        /// <summary>
        /// 查詢數據,只能用在供應商出貨時,判斷付款單為出貨中／已出貨狀態。參考自(includes/order/order.php第525行方法)
        /// </summary>
        /// <param name="Order_Id"></param>
        /// <returns></returns>
        public string check_vendor_deliver_order_master_status(string Order_Id)
        {
            string result = "4";//已出貨
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"select slave_id,slave_status from order_slave where order_id='{0}' ;", Order_Id);
                List<OrderSlave> store = new List<OrderSlave>();
                store = _dbAccess.getDataTableForObj<OrderSlave>(sb.ToString());

                foreach (var item in store)
                {
                    if (item.Slave_Status == 2 || item.Slave_Status == 4 || item.Slave_Status == 99 || item.Slave_Status == 90 || item.Slave_Status == 6 || item.Slave_Status == 7 || item.Slave_Status == 100)
                    {
                        if (item.Slave_Status == 2 || item.Slave_Status == 6 || item.Slave_Status == 7)
                        {
                            result = "3";
                        }
                    }
                    else
                    {
                        return result = "false";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDao.check_vendor_deliver_order_master_status -->" + ex.Message + sb.ToString(), ex);
            }
        }

        /// <summary>
        /// 更改付款單狀態代號：方法參考自(includes/order/order.php第355行方法)
        /// </summary>
        /// <param name="Order_Id"></param>
        /// <param name="Modify_Master_Order_Status"></param>
        public string modify_order_master_status(string Order_Id, string Modify_Master_Order_Status)
        {
            OrderMaster master = new OrderMaster();
            master.Order_Status = uint.Parse(Modify_Master_Order_Status);
            master.Order_Updatedate = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
            master.Order_Ipfrom = IPAddress;
            StringBuilder sb = new StringBuilder();
            try
            {
                if (Modify_Master_Order_Status == "90")//訂單取消
                {
                    master.Order_Date_Cancel = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                }
                else if (Modify_Master_Order_Status == "99")//訂單歸檔
                {
                    master.Order_Date_Close = uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString());
                }
                sb.Append("set sql_safe_updates = 0;");
                sb.AppendFormat(@"UPDATE	order_master set order_status = '{0}', order_updatedate ='{1}', ", master.Order_Status, master.Order_Updatedate);
                sb.AppendFormat(" order_ipfrom = '{0}' WHERE	order_id = '{1}';", IPAddress, Order_Id);
                sb.Append("set sql_safe_updates = 1;");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDao.modify_order_master_status -->" + ex.Message + sb.ToString(), ex);
            }
        }
        /// <summary>
        /// 參考自(includes/order/order.php第499行方法)
        /// </summary>
        /// <param name="Serial_id"></param>
        /// <param name="item"></param>
        /// <param name="status"></param>
        /// <param name="descreption"></param>
        /// <returns></returns>
        public string order_slave_status_record(string item, int status, string descreption = "")
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                SerialDao serial = new SerialDao(connStr);
                Serial se = new Serial();
                se = serial.GetSerialById(31);// 訂單狀態流水號
                //  se.Serial_Value += 1;
                sb.AppendFormat(serial.Update(31));
                //  serial.Update(se);
                sb.Append(@"insert order_slave_status(serial_id,slave_id,order_status,status_description,status_ipfrom,status_createdate)");
                sb.AppendFormat(" value( (select serial_value from serial where serial_id=31),'{0}','{1}' ,", item, 6);//進倉中
                sb.AppendFormat(" '{0}','{1}','{2}'); ", Description, IPAddress, uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDao.order_slave_status_record -->" + ex.Message + sb.ToString(), ex);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 記錄付款單狀態方法參考自(includes/order/order.php第391行方法)
        /// </summary>
        /// <param name="Order_Id"></param>
        /// <param name="Modify_Master_Order_Status"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public string order_master_status_record(string Order_Id, string Order_Status, string description = "")
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbt = new StringBuilder();
            try
            {
                SerialDao serial = new SerialDao(connStr);
                Serial se = new Serial();
                se = serial.GetSerialById(29);// 訂單主檔狀態流水號
                //se.Serial_Value += 1;
                sb.AppendFormat(serial.Update(29));
                // serial.Update(se);
                sb.Append(@"insert into order_master_status(serial_id,order_id,order_status,status_description,status_ipfrom,status_createdate)");
                sb.AppendFormat(" value((select serial_value from serial where serial_id=29),'{0}','{1}' ,", Order_Id, Order_Status);
                sb.AppendFormat(" '{0}','{1}','{2}'); ", Description, IPAddress, uint.Parse(CommonFunction.GetPHPTime(DateTime.Now.ToString()).ToString()));
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDao.order_slave_status_record -->" + ex.Message + sb.ToString(), ex);
            }
        }
      
       
    }
}