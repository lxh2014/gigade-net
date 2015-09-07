using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class OrderMasterStatusDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public OrderMasterStatusDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        /// <summary>
        /// 新增到order_master_status表sql語句
        /// </summary>
        /// <param name="oms"></param>
        /// <returns></returns>
        public string Insert(OrderMasterStatusQuery oms)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"insert into order_master_status (serial_id,order_id,order_status,status_description,status_ipfrom,status_createdate) Values (");
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}')", oms.serial_id, oms.order_id, oms.order_status, oms.status_description, oms.status_ipfrom,CommonFunction.GetPHPTime(DateTime.Now.ToString()));

                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterStatusDao.Insert -->" + ex.Message + sql.ToString(), ex);
            }
        }

        /// <summary>
        /// 新增到order_slave_status表sql語句
        /// </summary>
        /// <param name="oms"></param>
        /// <returns></returns>
        public string InsertSlave(OrderMasterStatusQuery oms)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"insert into order_slave_status (serial_id,slave_id,order_status,status_description,status_ipfrom,status_createdate) Values (");
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}','{5}')", oms.serial_id, oms.slave_id, oms.order_status, oms.status_description, oms.status_ipfrom, CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderMasterStatusDao.InsertSlave -->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
