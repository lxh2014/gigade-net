using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Dao
{
    public class InvoiceMasterRecordDao : IInvoiceMasterRecordImplDao
    {
        private IDBAccess _access;
        public InvoiceMasterRecordDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }
        public DataTable GetInvoiceList(InvoiceMasterRecordQuery imrq, out int totalCount)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder sqlfield = new StringBuilder();
            StringBuilder sqlfrom = new StringBuilder();
            StringBuilder sqlwhere = new StringBuilder();
            sqlfield.AppendLine(@"SELECT imr.invoice_id,imr.order_id,FROM_UNIXTIME(om.order_date_pay) AS order_date_pay,");
            sqlfield.AppendLine(@"para.parameterName AS order_payment,ops.sinopac_id,CAST(imr.invoice_status AS CHAR) AS invoice_status,imr.invoice_number, ");
            sqlfield.AppendLine(@"FROM_UNIXTIME(imr.invoice_date) AS invoice_date,imr.free_tax,imr.tax_amount,imr.sales_amount,");
            sqlfield.AppendLine(@"CAST(imr.buyer_type AS CHAR) AS buyer_type,imr.buyer_name,imr.company_invoice,imr.company_title,");
            sqlfield.AppendLine(@"CAST(imr.order_zip AS CHAR) AS order_zip,imr.order_address,imr.order_address AS aorder_address,");
            sqlfield.AppendLine(@"FROM_UNIXTIME(imr.print_post_createdate) AS print_post_createdate,FROM_UNIXTIME(imr.status_createdate) AS status_createdate,");
            sqlfield.AppendLine(@"imr.invoice_note,imr.invoice_attribute,imr.total_amount,om.order_amount,om.money_cancel,om.money_return");
            sqlfield.AppendLine(@",SUBSTRING(imr.invoice_number,2,8) AS invoice_number2 ");
            sqlfrom.AppendLine(@" FROM invoice_master_record  imr  ");
            sqlfrom.AppendLine(@" LEFT JOIN order_master  om  ON imr.order_id = om.order_id");
            sqlfrom.AppendLine(@" LEFT JOIN order_payment_sinopac ops ON om.order_id = ops.order_id ");
            sqlfrom.AppendLine(@" LEFT JOIN  (SELECT parameterCode,parameterName FROM t_parametersrc WHERE parameterType='payment') para ON para.parameterCode=om.order_payment");
            sqlwhere.AppendLine(@" WHERE 1=1 ");
            sqlwhere.AppendLine(imrq.sqlwhere);
            sql.Append(sqlfield.ToString() + sqlfrom.ToString() + sqlwhere.ToString());
            sql.AppendLine(@" ORDER BY imr.invoice_id DESC ");
            totalCount = 0;
            if (imrq.IsPage)
            {
                sql.AppendFormat(" limit {0},{1}", imrq.Start, imrq.Limit);
                System.Data.DataTable _dt = _access.getDataTable(" SELECT imr.invoice_id " + sqlfrom.ToString() + sqlwhere.ToString());
                totalCount = _dt.Rows.Count;
            }
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordDao-->GetInvoiceList" + ex.Message + sql.ToString(), ex);
            }
        }

        public DataTable ExportTXT(InvoiceMasterRecordQuery imrq)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@" SELECT invoice_number,invoice_date,company_invoice,tax_type,tax_amount,free_tax,CAST(invoice_status AS CHAR) AS invoice_status, ");
            sql.AppendLine(@" total_amount,invoice_attribute,FROM_UNIXTIME(status_createdate) AS status_createdate");
            sql.AppendLine(@"  FROM invoice_master_record WHERE 1=1");
            sql.Append(imrq.sqlwhere);
            if (imrq.order_id != 0)
            {
                sql.AppendFormat(@" AND order_id='{0}' ",imrq.order_id);
            }
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordDao-->ExportTXT" + ex.Message + sql.ToString(), ex);
            }

        }
        public DataTable ExportExlF(InvoiceMasterRecordQuery imrq)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT DISTINCT order_id ,om.order_name,om.order_amount,om.order_status,om.order_payment,om.order_createdate,");
            sql.AppendFormat(@"om.order_freight_normal,om.order_freight_low,om.bonus_type,om.deduct_happygo,om.deduct_happygo_convert,");
            sql.AppendFormat(@" ct.offsetamt FROM order_master om  LEFT JOIN invoice_master_record  imr  ");
            sql.AppendFormat(@" USING (order_id) LEFT JOIN order_payment_ct ct ON ct.lidm = om.order_id WHERE 1=1 ");
            sql.Append(imrq.sqlwhere);
            sql.AppendFormat(@" ORDER BY imr.invoice_id DESC");
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordDao-->ExportExlF" + ex.Message + sql.ToString(), ex);
            }

        }
        //public DataTable ExportExlS(string order_id)
        //{
        //    StringBuilder sql = new StringBuilder();
        //    sql.AppendFormat(@"SELECT *  FROM  invoice_master_record imr ");
        //    sql.AppendFormat(@" WHERE 1=1   AND order_id ={0} ", order_id);
        //    try
        //    {
        //        return _access.getDataTable(sql.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("InvoiceMasterRecordDao-->ExportExlS" + ex.Message + sql.ToString(), ex);
        //    }
        //}
        public DataTable ExportExlS(string order_id)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT od.item_id,od.product_name,od.buy_num,od.single_money,od.detail_status,");
            sql.AppendFormat(@" deduct_bonus,deduct_welfare,deduct_happygo,od.event_cost,od.single_cost,od.bag_check_money,");
            sql.AppendFormat(@" os.slave_date_close,v.vendor_name_simple,v.vendor_code,os.order_id,v.product_manage,p.tax_type  ");
            sql.AppendFormat(@" FROM order_detail  od ,order_slave  os ,vendor  v ,product_item  pit , ");
            sql.AppendFormat(@"  product  p  WHERE 1=1 AND od.slave_id = os.slave_id AND v.vendor_id = od.item_vendor_id ");
            sql.AppendFormat(@" AND pit.item_id = od.item_id AND pit.product_id = p.product_id and item_mode in (0,1) AND os.order_id = {0}", order_id);
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordDao-->ExportExlS" + ex.Message + sql.ToString(), ex);
            }
        }
        public int Update(InvoiceMasterRecord imr)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"update invoice_master_record set company_title='{0}',company_invoice='{1}' ", imr.company_title, imr.company_invoice);
            sql.AppendFormat(@" ,order_zip='{0}',order_address='{1}' ", imr.order_zip, imr.order_address);
            sql.AppendFormat(@" where invoice_id={0} ;", imr.invoice_id);
            try
            {
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordDao-->Update" + ex.Message + sql.ToString(), ex);
            }

        }
        public DataTable InvoicePrint(uint invoice_id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 獲取地址下拉列表
        /// </summary>
        /// <param name="zip"></param>
        /// <returns></returns>
        public List<Zip> GetZipAddress(Zip zip)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT zipcode,CONCAT_WS(' ',zipcode,middle,small) as middle from t_zip_code WHERE 1=1");
            if (!string.IsNullOrEmpty(zip.zipcode))
            {
                sql.AppendFormat(@" AND zipcode='{0}' ", zip.zipcode);
            }
            try
            {
                return _access.getDataTableForObj<Zip>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordDao-->GetZipAddress" + ex.Message + sql, ex);
            }

        }
        /// <summary>
        /// 獲得參數表的數據
        /// </summary>
        /// <param name="parameterType">什麼類型</param>
        /// <param name="key">取的是parameterName還是remark</param>
        /// <returns></returns>
        public DataTable GetParametersrc(string parameterType, string key)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT parameterCode, ");
            if (key.Trim() == "parameterName")
            {
                sql.AppendFormat(@" parameterName ");
            }
            else if (key.Trim() == "remark")
            {
                sql.AppendFormat(@" remark ");
            }
            sql.AppendFormat(@" FROM t_parametersrc  WHERE parameterType='{0}'; ", parameterType.Trim());
            try
            {
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("InvoiceMasterRecordDao-->GetParametersrc" + ex.Message + sql, ex);
            }
        }
    }
}
