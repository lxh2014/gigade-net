using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
namespace BLL.gigade.Mgr
{
   public class OrderCancelMasterMgr:IOrderCancelMasterImplMgr
    {
        private IOrderCancelMasterImplDao _orderCancelMaster;
        public OrderCancelMasterMgr(string connectionStr)
        {
            _orderCancelMaster = new OrderCancelMasterDao(connectionStr);
        }

        public List<OrderCancelMaster> GetOrderCancelMasterList(OrderCancelMaster ocm, out int totalCount)
        {
            try
            {
                return _orderCancelMaster.GetOrderCancelMasterList(ocm, out totalCount);

            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMasterMgr.GetOrderCancelMasterList-->" + ex.Message, ex);
            }
        }
        public int Update(OrderCancelMaster ocm)
        {
            try
            {
                return _orderCancelMaster.Update(ocm);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMasterMgr.Update-->" + ex.Message, ex);
            }
        }
    }
}
