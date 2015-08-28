using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class OrderDeliverMgr : IOrderDeliverImplMgr
    {
        private IOrderDeliverImplDao _orderdeliverDao;
        public OrderDeliverMgr(string connectionString)
        {
            _orderdeliverDao = new OrderDeliverDao(connectionString);
        }
        public List<OrderDeliverQuery> GetOrderDeliverList(OrderDeliverQuery query, out int totalCount)
        {
            try
            {
                return _orderdeliverDao.GetOrderDeliverList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->GetOrderDeliverList-->" + ex.Message, ex);
            }
        }
        public int DismantleSlave(int slave_id, string select_did, DataTable dt)
        {
            try
            {
                return _orderdeliverDao.DismantleSlave(slave_id, select_did, dt);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderDetailMgr-->DismantleSlave-->" + ex.Message, ex);
            }
        }
        
    }
}
