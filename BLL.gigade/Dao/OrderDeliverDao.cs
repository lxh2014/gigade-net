using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using MySql.Data.MySqlClient;
using BLL.gigade.Common;
using System.Net;
using System.Data;

namespace BLL.gigade.Dao
{
    public class OrderDeliverDao : IOrderDeliverImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public OrderDeliverDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public List<OrderDeliverQuery> GetOrderDeliverList(OrderDeliverQuery store, out int totalCount)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder search = new StringBuilder();

            List<VendorBrandSetQuery> list = new List<VendorBrandSetQuery>();
            StringBuilder strCount = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"SELECT os.order_id,v.vendor_name_simple,od.deliver_id,od.slave_id,od.deliver_status,od.deliver_store,od.deliver_code,FROM_UNIXTIME(od.deliver_time) as delivertime,od.deliver_note,FROM_UNIXTIME(od.deliver_createdate) as delivercre,FROM_UNIXTIME(od.deliver_updatedate) as deliverup  FROM order_deliver od, order_slave os , vendor v WHERE 1=1  AND os.slave_id=od.slave_id AND os.vendor_id = v.vendor_id ");
                strCount = strCount.Append("SELECT count(*) AS search_total FROM order_deliver od, order_slave os , vendor v WHERE 1=1 AND os.slave_id = od.slave_id AND os.vendor_id = v.vendor_id ");
                switch (store.serchs)
                {
                    case "1":
                        search.AppendFormat(" and os.order_id   like   '%{0}%' ", store.search);
                        break;
                    case "2":
                        search.AppendFormat(" and os.slave_id  like  '%{0}%' ", store.search);
                        break;
                    case "3":
                        search.AppendFormat(" and od.deliver_code LIKE '%{0}%' ", store.search);
                        break;
                    default:
                        break;
                }
                if (store.seldate != 0)
                {
                    search.AppendFormat(" and od.deliver_time >='{0}'  and od.deliver_time <='{1}'  ", store.deliverstart, store.deliverend);
                }
                if (store.selven != 0)
                {
                    search.AppendFormat(" and os.vendor_id = '{0}' ", store.selven);
                }
                totalCount = 0;
                if (store.IsPage)
                {
                    strSql.AppendFormat(search.ToString());
                    strCount.AppendFormat(search.ToString());
                    System.Data.DataTable _dt = _dbAccess.getDataTable(strCount.ToString());
                    if (_dt != null && _dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
                    }
                    strSql.AppendFormat(" ORDER BY os.order_id  limit {0},{1}", store.Start, store.Limit);
                }
                else 
                {
                    strSql.AppendFormat(search.ToString());
                }
                return _dbAccess.getDataTableForObj<OrderDeliverQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDeliverDao-->GetOrderDeliverList" + ex.Message + strSql.ToString(), ex);
            }
        }


        public int DismantleSlave(int slave_id, string select_did, DataTable dt)
        {

            int NoldSlaveProductSubtotal = 0;
            int NnewSlaveProductSubtotal = 0;
            string AoldOrderSlave = string.Empty;
            string AnewOrderSlave = string.Empty;
            string AllDid = string.Empty;
            if (dt.Rows.Count > 0)
            {
                //算原本slave總價,與記錄要異動的detail slave_id
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NnewSlaveProductSubtotal += Convert.ToInt32(dt.Rows[i]["single_money"]) * Convert.ToInt32(dt.Rows[i]["buy_num"]);
                    AllDid += dt.Rows[i]["detail_id"].ToString() + ',';
                    if (!select_did.Contains(dt.Rows[i]["detail_id"].ToString()))
                    {
                        AnewOrderSlave += dt.Rows[i]["detail_id"].ToString() + ',';
                    }
                }
                //算出拆單後 要出貨slave總價
                select_did = select_did.Substring(0, select_did.LastIndexOf(","));
                string[] newstrs = select_did.Split(',');
                for (int j = 0; j < newstrs.Length; j++)
                {
                    if (AllDid.Contains(newstrs[j]))
                    {
                        NoldSlaveProductSubtotal += Convert.ToInt32(dt.Rows[j]["single_money"]) * Convert.ToInt32(dt.Rows[j]["buy_num"]);
                        //unset($aAll_Did[$key]);
                    }
                }
                NnewSlaveProductSubtotal -= NoldSlaveProductSubtotal;
                //
                //事務開始       
                StringBuilder sql = new StringBuilder();//查詢
                sql.AppendFormat(@" SELECT	* FROM	order_slave  WHERE slave_id  = '{0}';",slave_id);
                DataTable mydt = _dbAccess.getDataTable(sql.ToString());
                StringBuilder mysql = new StringBuilder();
                mysql.AppendFormat("set sql_safe_updates = 0;update order_slave set slave_amount ='{0}' and slave_product_subtotal='{1}'   WHERE slave_id ='{2}';", NnewSlaveProductSubtotal, NnewSlaveProductSubtotal,slave_id);
                int MaxSlaveId = Convert.ToInt32(_dbAccess.getDataTable("SELECT max(slave_id) FROM order_slave;").Rows[0][0]) + 1;//獲取到最大的slave_id+1
                mysql.AppendFormat(@"INSERT into order_slave 
                                    (slave_id,order_id,vendor_id,slave_freight_normal,slave_freight_low,
                                    slave_product_subtotal,slave_amount,slave_status,slave_note,slave_date_delivery,
                                    slave_date_cancel,slave_date_return,slave_date_close,slave_updatedate,slave_ipfrom)
                                    VALUES
                                    ('{0}','{1}','{2}','{3}','{4}',
                                    '{5}','{6}','{7}','{8}','{9}',
                                    '{10}','{11}','{12}','{13}','{14}');"
                                    , MaxSlaveId,mydt.Rows[0]["order_id"],mydt.Rows[0]["vendor_id"],0,0,
                                    NnewSlaveProductSubtotal, NnewSlaveProductSubtotal, mydt.Rows[0]["slave_status"], mydt.Rows[0]["slave_note"], mydt.Rows[0]["slave_date_delivery"],
                                    mydt.Rows[0]["slave_date_cancel"],mydt.Rows[0]["slave_date_return"],mydt.Rows[0]["slave_date_close"],mydt.Rows[0]["slave_updatedate"],mydt.Rows[0]["slave_ipfrom"]);
                mysql.AppendFormat("update order_detail set slave_id ='{0}' WHERE detail_id in ({1});",slave_id,AnewOrderSlave.Substring(0,AnewOrderSlave.LastIndexOf(',')));

                StringBuilder sqltwo = new StringBuilder();
                sqltwo.AppendFormat(@"SELECT dm.*,dd.* FROM  deliver_master dm
INNER JOIN deliver_detail dd on dd.deliver_id=dm.deliver_id WHERE  detail_id in({0}) ;", AnewOrderSlave.Substring(0,AnewOrderSlave.LastIndexOf(',')));
                DataTable newdt = _dbAccess.getDataTable(sqltwo.ToString());
                if (newdt.Rows.Count > 0)
                {
                   int MaxDeliverId = Convert.ToInt32(_dbAccess.getDataTable("select max(deliver_id) from deliver_master").Rows[0][0])+1;
                   mysql.AppendFormat(@"INSERT INTO deliver_master (deliver_id,order_id,ticket_id,type,export_id,
                                                                     import_id,freight_set,delivery_status,delivery_name,delivery_mobile,
                                                                     delivery_phone,delivery_zip,delivery_address,delivery_store,delivery_code,
                                                                     delivery_freight_cost,delivery_date,pickup_date,estimated_delivery_date,estimated_arrival_period,
                                                                     created,modified,export_flag,data_chg,work_status  )values
                                                                    ('{0}','{1}','{2}','{3}','{4}',
                                                                    '{5}','{6}','{7}','{8}','{9}',
                                                                    '{10}','{11}','{12}','{13}','{14}',
                                                                    '{15}','{16}','{17}','{18}','{19}',
                                                                    '{20}','{21}','{22}','{23}','{24}');",
                                                                    MaxDeliverId, newdt.Rows[0]["order_id"], newdt.Rows[0]["ticket_id"], newdt.Rows[0]["type"], newdt.Rows[0]["export_id"],
                                                                    newdt.Rows[0]["import_id"], newdt.Rows[0]["freight_set"], newdt.Rows[0]["delivery_status"], newdt.Rows[0]["delivery_name"], newdt.Rows[0]["delivery_mobile"],
                                                                    newdt.Rows[0]["delivery_phone"], newdt.Rows[0]["delivery_zip"], newdt.Rows[0]["delivery_address"], newdt.Rows[0]["delivery_store"], newdt.Rows[0]["delivery_code"],
                                                                    newdt.Rows[0]["delivery_freight_cost"], newdt.Rows[0]["delivery_date"], newdt.Rows[0]["pickup_date"], newdt.Rows[0]["estimated_delivery_date"], newdt.Rows[0]["estimated_arrival_period"],
                                                                    CommonFunction.DateTimeToString(DateTime.Now), CommonFunction.DateTimeToString(DateTime.Now), newdt.Rows[0]["export_flag"], newdt.Rows[0]["data_chg"], newdt.Rows[0]["work_status"]);

                  mysql.AppendFormat("UPDATE deliver_detail SET deliver_id = '{0}' WHERE detail_id IN ({1});", newdt.Rows[0]["deliver_id"],AnewOrderSlave.Substring(0,AnewOrderSlave.LastIndexOf(',')));
                  mysql.AppendFormat("set sql_safe_updates = 1; ");
                }
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
                    mySqlCmd.CommandText = mysql.ToString();
                    result = mySqlCmd.ExecuteNonQuery();

                    mySqlCmd.Transaction.Rollback();
                    //mySqlCmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    mySqlCmd.Transaction.Rollback();
                    throw new Exception("IplaseDao.DeleteIplasById-->" + ex.Message + sql.ToString(), ex);
                }
                finally
                {
                    if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                    {
                        mySqlConn.Close();
                    }
                }
                return result;
                //事務結束
            }
            else//dt中不存在數據
            {
                return 0; //表示dt中不存在數據
            }
        }
    }
}
