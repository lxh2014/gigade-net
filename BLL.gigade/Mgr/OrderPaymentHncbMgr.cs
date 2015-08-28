/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderPaymentHncbMgr 
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
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class OrderPaymentHncbMgr : IOrderPaymentHncbImplMgr
    {
        private IOrderPaymentHncbImplDao _orderPaymentHncbDao;
        public OrderPaymentHncbMgr(string connectionString)
        {
            _orderPaymentHncbDao = new OrderPaymentHncbDao(connectionString);
        }

        #region 華南賬戶（虛擬帳號）  add by zhuoqin0830w  2015/05/13
        /// <summary>
        /// 華南賬戶（虛擬帳號）  add by zhuoqin0830w  2015/05/13
        /// </summary>
        /// <param name="orderPayment"></param>
        /// <returns></returns>
        public string AddPaymentHncb(OrderPaymentHncb orderPayment)
        {
            try
            {
                return _orderPaymentHncbDao.AddPaymentHncb(orderPayment);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderPaymentHncbDao-->AddPaymentHncb-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}