using BLL.gigade.Common;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Dao
{
    public class ReturnMasterDao : IReturnMasterImplDao
    {
        private IDBAccess _accessMySql;
        public string connStr;
        public ReturnMasterDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }
        #region send
        public DataTable GetOrderReturnCount(OrderReturnUserQuery store)
        { 
            StringBuilder sqlcount = new StringBuilder();
            try{

               
                sqlcount.AppendFormat(@" select count(*) as search_total from order_return_detail  where detail_id='{0}' ", store.detail_id);

                return _accessMySql.getDataTable(sqlcount.ToString());
              }
            catch(Exception ex)
            {
                throw new Exception("ReturnMasterDao-->GetOrderReturnCount-->" + ex.Message + "sql:" + sqlcount.ToString() , ex);
            }
        }
        public int InsertOrderReturnMaster(OrderReturnUserQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" INSERT INTO order_return_master (return_id,order_id,vendor_id,return_status,return_note,return_zip,return_address,return_createdate,return_updatedate,return_ipfrom)");
                sql.AppendFormat(@" values('{0}','{1}','{2}',", store.return_id, store.order_id, store.item_vendor_id);
                sql.AppendFormat(@" '{0}','{1}','{2}','{3}',", store.return_status, store.user_note, store.return_zip, store.return_address);
                sql.AppendFormat(@" '{0}','{1}','{2}')", store.return_createdate, store.return_updatedate, store.return_ipfrom);
                string s = sql.ToString();
                return _accessMySql.execCommand(s);
            }
            catch (Exception ex)
            {
                throw new Exception("ReturnMasterDao-->InsertOrderReturnMaster-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }
          
        }
        public int InsertOrderReturnDetail(OrderReturnUserQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" insert into order_return_detail(return_id,detail_id) values('{0}','{1}')", store.return_id, store.detail_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ReturnMasterDao-->InsertOrderReturnDetail-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }
            
        }
        public int InsertOrderMasterStatus(OrderMasterStatus OMS)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" insert into order_master_status(serial_id,order_id,order_status,status_description,status_ipfrom,status_createdate)");
                sql.AppendFormat(@" values('{0}','{1}','{2}','{3}','{4}','{5}')", OMS.serial_id, OMS.order_id, OMS.order_status, OMS.status_description, OMS.status_ipfrom, OMS.status_createdate);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ReturnMasterDao-->InsertOrderMasterStatus-->" + ex.Message + "sql:" + sql.ToString(), ex);
            }
           
        }
        public List<OrderReturnUserQuery> OrderMasterQuery(OrderReturnUserQuery store)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"select order_status,invoice_status,order_invoice from order_master where order_id='{0}'", store.order_id);
                return _accessMySql.getDataTableForObj<OrderReturnUserQuery>(sql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("ReturnMasterDao.OrderMasterQuery-->" + _accessMySql.ToString() + ex.Message, ex);
            }
        }
        public int UpdateOrderMaster(OrderReturnUserQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {

                sql.AppendFormat(" update order_master set order_status='{0}',order_updatedate='{1}',order_ipfrom='{2}',order_date_close='{3}',order_date_cancel='{4}' where order_id='{5}'", store.order_status, store.order_updatedate, store.order_ipfrom, store.order_date_close, store.order_date_cancel, store.order_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.UpdateOrderMaster-->" + sql.ToString() + ex.Message, ex);
            }
        }
        public int UpdateOrderDetailStatus(OrderReturnUserQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(" update order_detail set detail_status='{0}' where detail_id='{1}'", store.detail_status, store.detail_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.UpdateOrderDetailStatus-->" + sql.ToString() + ex.Message, ex);
            }
        }

        #endregion

        #region secend send
        public DataTable SelOrderMaster(OrderReturnUserQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT invoice_status,order_invoice,* from order_master where order_id='{0}';", store.order_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.SelOrderMaster-->" + sql.ToString() + ex.Message, ex);
            }
            
        }
        public DataTable Seltime(OrderReturnUserQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" SELECT invoice_status,invoice_date,invoice_attribute from invoice_master_record where order_id='{0}' ", store.order_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Seltime-->" + sql.ToString() + ex.Message, ex);
            }
            
        }
        public DataTable SelCon(OrderReturnUserQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT count(*) AS search_total from order_detail od LEFT JOIN order_slave os ON os.slave_id=od.slave_id where os.order_id='{0}' and od.detail_status in (0,4)", store.order_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.SelCon-->" + sql.ToString() + ex.Message, ex);
            }
           
        }
        public DataTable SelInvoiceid(int invoice_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT order_id from invoice_master_record where invoice_id='{0}' and invoice_attribute in (1,4)", invoice_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.SelInvoiceid-->" + sql.ToString() + ex.Message, ex);
            }
          
        }
        public int Updinvoice(OrderReturnUserQuery store)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" UPDATE order_master SET order_invoice='',invoice_status=2 where order_id='' ", store.order_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Updinvoice-->" + sql.ToString() + ex.Message, ex);
            }
           
        }
        public int Delinvoice(int invoice_id)
        {//清空invoice_attribute
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" UPDATE invoice_master_record SET invoice_attribute='' where invoice_id='{0}' ", invoice_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Delinvoice-->" + sql.ToString() + ex.Message, ex);
            }
           
        }//
        public List<OrderMaster> Selpay(OrderMaster store)
        {//查詢付款單
            StringBuilder sql = new StringBuilder();
            try
            {
                List<OrderMaster> list = new List<OrderMaster>();
                sql.AppendFormat(@" SELECT * from order_master where order_deliver_success = 1 amd order_status<>'' AND order_id='{0}' ", store.Order_Id);
                list = _accessMySql.getDataTableForObj<OrderMaster>(sql.ToString());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Selpay-->" + sql.ToString() + ex.Message, ex);
            }
           
        }
        public DataTable Seldetail(OrderReturnUserQuery store, string status)
        {//查詢明細
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" SELECT od.product_name,od.product_spec_name,od.single_money,od.item_id,od.deduct_bonus,od.detail_status,od.buy_num,os.order_id from order_detail od LEFT JOIN order_slave os on os.slave_id=od.slave_id where os.order_id='{0}' and od.detail_status in ({1})", store.order_id, status);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Seldetail-->" + sql.ToString() + ex.Message, ex);
            }
           
        }
        public int Insertdb(InvoiceMasterRecord Imr,InvoiceSliveInfo Isi)
        {//修改數據
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" UPDATE invoice_master_record SET invoice_status='{0}',free_tax='{1}',sales_amount='{2}',tax_amount='{3}',total_amount='{4}',deduct_bonus='{5}',order_freight_normal='{6}',order_freight_normal_notax='{7}',order_freight_low='{8}',order_freight_low_notax='{9}',status_createdate='{10}' where invoice_id='{11}';", Imr.invoice_status, Imr.free_tax, Imr.sales_amount, Imr.tax_amount, Imr.total_amount, Imr.deduct_bonus, Imr.order_freight_normal, Imr.order_freight_normal_notax, Imr.order_freight_low, Imr.order_freight_low_notax, Imr.status_createdate, Imr.invoice_id);
                sql.AppendFormat(@" INSERT INTO invoice_slive_info (invoice_slive_info,invoice_id,order_id,item_id,product_name,product_spec_name,sort,single_money,sub_deduct_bonus,buy_num,subtotal,slive_note,slive_createdate) VALUES ();", Isi.buy_num);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Insertdb-->" + sql.ToString() + ex.Message, ex);
            }
          
        }
        public int Updcount(InvoiceAllowanceRecord m)
        {//修改數據
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE invoice_allowance_record SET allownace_total='{0}',allowance_amount='{1}',allowance_tax='{2}' where allowance_id='{3}'; ", m.allownace_total, m.allowance_amount, m.allowance_tax, m.allowance_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Updcount-->" + sql.ToString() + ex.Message, ex);
            }
           
        }
        public DataTable Selmaster(int invoice_id)
        {//查詢發票id 開立折讓單
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@" SELECT * FROM invoice_master_record where invoice_id='{0}'; ", invoice_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Selmaster-->" + sql.ToString() + ex.Message, ex);
            }
           
        }
        public DataTable Selslive(int invoice_id)
        {//查詢發票發票向下品項 開立折讓單
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT * FROM invoice_slive_info where invoice_id='{0}' and invoice_allowance=0;", invoice_id);
                return _accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.Selslive-->" + sql.ToString() + ex.Message, ex);
            }
           
        }
        public int UpdMaster(int invoice_id)
        {//異動屬性
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"UPDATE invoice_master_record SET invoice_attribute=3 WHERE invoice_id='{0}'", invoice_id);
                return _accessMySql.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderTempReturnListDao.UpdMaster-->" + sql.ToString() + ex.Message, ex);
            }
            
        }
        #endregion
        #region 暫存退貨單棄用
        //public List<OrderReturnUserQuery> GetOrderTempReturnList(OrderReturnUserQuery store, out int totalCount)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    StringBuilder sqlcount = new StringBuilder();
        //    StringBuilder sqlwhere = new StringBuilder();
        //    List<OrderReturnUserQuery> list = new List<OrderReturnUserQuery>();
        //    try
        //    {
        //        sql.Append(@"  SELECT ORU.user_return_id,ORU.detail_id,ORU.return_reason,ORU.gift,ORU.temp_status,ORU.user_note,ORU.return_zip,ORU.return_address,ORU.bank_name,ORU.bank_branch,ORU.bank_account,ORU.account_name,ORU.user_return_createdate,ORU.user_return_updatedate,ORU.user_return_ipfrom,OS.order_id,OS.slave_status,OD.item_vendor_id FROM order_return_user ORU  INNER JOIN order_detail OD  ON ORU.detail_id=OD.detail_id INNER JOIN order_slave OS ON OD.slave_id=OS.slave_id where 1=1 ");
        //        sqlcount.AppendFormat(@" SELECT count(*) as search_total FROM order_return_user WHERE 1=1 ");
        //        if (store.selecttype == "1")
        //        {
        //            sqlwhere.AppendFormat(" and order_return_user.detail_id like '%{0}%' ", store.searchcon);
        //        }
        //        if (store.seldate == "1")
        //        {
        //            sqlwhere.AppendFormat(" and user_return_createdate>='{0}' and user_return_createdate<='{1}' ", store.timestart, store.timeend);
        //        }
        //        if (store.temp_status != 3)
        //        {
        //            sqlwhere.AppendFormat(" and temp_status='{0}' ", store.temp_status);
        //        }
        //        sql.Append(sqlwhere.ToString());
        //        totalCount = 0;
        //        if (store.IsPage)
        //        {
        //            System.Data.DataTable _dt = _accessMySql.getDataTable(sqlcount.ToString() + sqlwhere.ToString());
        //            if (_dt != null && _dt.Rows.Count > 0)
        //            {
        //                totalCount = Convert.ToInt32(_dt.Rows[0]["search_total"]);
        //            }
        //            sql.AppendFormat(" limit {0},{1}", store.Start, store.Limit);
        //        }
        //        list = _accessMySql.getDataTableForObj<OrderReturnUserQuery>(sql.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ReturnMasterDao-->GetOrderTempReturnList-->" + ex.Message + "sql:" + sql.ToString() + sqlwhere.ToString(), ex);
        //    }
        //    return list;
        //}
        //public int UpdateTempStatus(OrderReturnUserQuery store)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    try
        //    {
        //        sql.AppendFormat(" UPDATE order_return_user  SET temp_status='{0}' ,user_return_updatedate='{1}' WHERE user_return_id='{2}'", store.temp_status, store.user_return_updatedate, store.user_return_id);
        //        return _accessMySql.execCommand(sql.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("OrderTempReturnListDao.UpdateTempStatus-->" + sql.ToString() + ex.Message, ex);
        //    }

        //}
        #endregion
    }
}
