using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;


namespace BLL.gigade.Mgr
{
    public class OrderCancelMsgMgr : IOrderCancelMsgImplMgr
    {
        private IOrderCancelMsgImplDao icancelDao;
        public OrderCancelMsgMgr(string connectionStr)
        {
            icancelDao = new OrderCancelMsgDao(connectionStr);
        }
        public List<OrderCancelMsgQuery> Query(OrderCancelMsgQuery ocm, out int totalCount)
        {
            try
            {
                return icancelDao.Query(ocm, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMsgMgr.Query-->" + ex.Message, ex);
            }
        }
        public int Reply(OrderCancelResponse ocr)
        {
            try
            {
                return icancelDao.Reply(ocr);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderCancelMsgMgr.Reply-->" + ex.Message, ex);
            }
        }
    }
}
