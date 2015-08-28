/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderSlaveMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 16:07:06 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class OrderSlaveMgr:IOrderSlaveImplMgr
    {
        private IOrderSlaveImplDao _orderSlaveDao;
        public OrderSlaveMgr(string connectionStr)
        {
            _orderSlaveDao = new OrderSlaveDao(connectionStr);
        }

        public string Save(BLL.gigade.Model.OrderSlave orderSlave)
        {
            return _orderSlaveDao.Save(orderSlave);
        }
        public int Delete(int slaveId)
        {
            try
            {
                return _orderSlaveDao.Delete(slaveId);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMgr-->Delete-->" + ex.Message, ex);
            }
        }
        public List<OrderSlaveQuery> GetOrderWaitDeliver(OrderSlaveQuery store, string str, out int totalCount)
        {
            try
            {
                return _orderSlaveDao.GetOrderWaitDeliver(store,str,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMgr-->GetOrderWaitDeliver-->" + ex.Message, ex);
            }
        }
        public List<OrderSlaveQuery> GetAllOrderWait(OrderSlaveQuery store, string str, out int totalCount)
        {
            try
            {
                return _orderSlaveDao.GetAllOrderWait(store, str, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMgr-->GetAllOrderWait-->" + ex.Message, ex);
            }
        }
        public OrderSlaveQuery GetOrderDatePay(OrderSlaveQuery query)
        {
            try
            {
                return _orderSlaveDao.GetOrderDatePay(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMgr-->GetOrderDatePay-->" + ex.Message, ex);
            }
        }
        public List<OrderSlaveQuery> GetVendorWaitDeliver(OrderSlaveQuery store, string str, out int totalCount) 
        {
            try
            {
                return _orderSlaveDao.GetVendorWaitDeliver(store,str,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMgr-->GetVendorWaitDeliver-->" + ex.Message, ex);
            }
        }


        public DataTable GetList(OrderSlaveQuery query, out int totalCount)
        {
            try
            {
                return _orderSlaveDao.GetList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMgr-->GetList-->" + ex.Message, ex);
            }
        }
        public DataTable GetListPrint(OrderSlaveQuery query, string addsql = null) 
        {
            try
            {
                return _orderSlaveDao.GetListPrint(query, addsql);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderSlaveMgr-->GetListPrint-->" + ex.Message, ex);
            }
        }
    }
}
