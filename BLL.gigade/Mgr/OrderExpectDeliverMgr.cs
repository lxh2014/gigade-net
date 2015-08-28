/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：OrderExpectDelieveMgr.js 
 * 摘   要： 預購單
 *  
 * 当前版本：1.0 
 * 作   者：shuangshuang0420j 
 * 完成日期：2014/10/21 14:10:10
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Collections;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{

    public class OrderExpectDeliverMgr : IOrderExpectDeliverImplMgr
    {
        private IOrderExpectDeliverImplDao _orderExpectDeliverDao;
        public OrderExpectDeliverMgr(string connectionstring)
        {
            _orderExpectDeliverDao = new OrderExpectDeliverDao(connectionstring);
        }

        public List<OrderExpectDeliverQuery> GetOrderExpectList(OrderExpectDeliverQuery query, out int totalCount)
        {
            try
            {
                return _orderExpectDeliverDao.GetOrderExpectList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderExpectDeliverMgr.GetOrderExpectList-->" + ex.Message, ex);
            }
        }
        public int OrderExpectModify(OrderExpectDeliverQuery store)
        {
            try
            {
                return _orderExpectDeliverDao.OrderExpectModify(store);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderExpectDeliverMgr.OrderExpectModify-->" + ex.Message, ex);
            }
        }
        public List<OrderExpectDeliverQuery> GetModel(OrderExpectDeliverQuery store)
        {
            try
            {
                return _orderExpectDeliverDao.GetModel(store);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderExpectDeliverMgr.GetModel-->" + ex.Message, ex);
            }
        }

    }
}
