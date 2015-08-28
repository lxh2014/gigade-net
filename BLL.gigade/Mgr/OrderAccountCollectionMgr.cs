using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class OrderAccountCollectionMgr
    {
        private OrderAccountCollectionDao _orderCollectionDao;
        public OrderAccountCollectionMgr(string connectionString)
        {
            _orderCollectionDao = new OrderAccountCollectionDao(connectionString);
        }
        public DataTable GetOrderAccountCollectionList(Model.OrderAccountCollection query, out int totalCount)
        {

            try
            {
                return _orderCollectionDao.GetOrderAccountCollectionList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionMgr-->GetOrderAccountCollectionList-->" + ex.Message, ex);
            }
        }

        public int SaveOrEdit(OrderAccountCollection query)
        {

            try
            {
                return _orderCollectionDao.SaveOrEdit(query);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionMgr-->SaveOrEdit-->" + ex.Message, ex);
            }
        }

        public int delete(string str_row_id)
        {

            try
            {
                return _orderCollectionDao.Delete(str_row_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionMgr-->delete-->" + ex.Message, ex);
            }
        }

        public int YesOrNoOrderId(string order_id)
        {

            try
            {
                return _orderCollectionDao.YesOrNoOrderId(order_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionMgr-->YesOrNoOrderId-->" + ex.Message, ex);
            }
        }

        public int IncludeOrderId(string order_id)
        {
            try
            {
                return _orderCollectionDao.IncludeOrderId(order_id);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccountCollectionMgr-->IncludeOrderId-->" + ex.Message, ex);
            }
        }

    }
}
