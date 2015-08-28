/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderPaymentHncbDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/05/13
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Dao.Impl;
using MySql.Data.MySqlClient;
using System.Collections;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class OrderPaymentHncbDao : IOrderPaymentHncbImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public OrderPaymentHncbDao(string connectionString)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }

        #region 華南賬戶（虛擬帳號）  add by zhuoqin0830w  2015/05/13
        /// <summary>
        /// 華南賬戶（虛擬帳號）  add by zhuoqin0830w  2015/05/13
        /// </summary>
        /// <param name="orderPayment"></param>
        /// <returns></returns>
        public string AddPaymentHncb(OrderPaymentHncb orderPayment)
        {
            StringBuilder strSql = new StringBuilder();
            int time = Convert.ToInt32(CommonFunction.GetPHPTime(Convert.ToString(DateTime.Now)));
            try
            {
                //根據條件 獲取 serial 中 serial_value 的 值
                StringBuilder strSerialValue = new StringBuilder();
                strSerialValue.Append(@"SELECT serial_value FROM serial WHERE serial_id = CONCAT('1',DATE_FORMAT(NOW(),'%y'),QUARTER(NOW()));");
                string serialValue = _dbAccess.getDataTable(strSerialValue.ToString()).Rows[0][0].ToString();
                // 判斷 獲取的 serial_value 是否 為 空  如果 為空 則 等於 '000001'， 如果 不為空  則 serial_value + 1
                StringBuilder strP_hncb_id_1 = new StringBuilder();
                strP_hncb_id_1.AppendFormat(@"SELECT IF({0} = '','000001',{0} + 1);", serialValue);
                string p_hncb_id_1 = _dbAccess.getDataTable(strP_hncb_id_1.ToString()).Rows[0][0].ToString();
                // 根據 p_hncb_id_1 的 值 得到 華南賬戶（虛擬帳號）
                StringBuilder strP_hncb_id = new StringBuilder();
                strP_hncb_id.AppendFormat(@"SELECT CONCAT('98943',DATE_FORMAT(NOW(),'%y'),QUARTER(NOW()),RIGHT(CONCAT('000000',{0}),6));", p_hncb_id_1);
                string p_hncb_id = _dbAccess.getDataTable(strP_hncb_id.ToString()).Rows[0][0].ToString();

                //根據得到的虛擬帳號 進行新增  p_hncb_id
                strSql.Append("INSERT INTO order_payment_hncb(order_id,bank,entday,txtday,paid,type,hncb_sn,outputbank,e_date,vat_number,error,createdate,updatedate,hncb_id) VALUES({0},");
                strSql.AppendFormat(" '0', 0, 0, 0, 1, 0, 0, 0, 0, '00', {0}, {0}, '{1}');", time, p_hncb_id);
                // 根據獲取的 p_hncb_id_1 進行 修改
                strSql.AppendFormat("UPDATE serial SET serial_value = {0} WHERE serial_id = CONCAT('1',DATE_FORMAT(NOW(),'%y'),QUARTER(NOW()));", p_hncb_id_1);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderPaymentHncbDao-->AddPaymentHncb-->" + strSql.ToString() + ex.Message, ex);
            }
        }
        #endregion
    }
}