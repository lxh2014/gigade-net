using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class OrderVendorProducesMgr
    {
        public OrderVendorProducesDao _iOrderVendorDao;
        public OrderVendorProducesMgr(string connectionString)
        {
            _iOrderVendorDao = new OrderVendorProducesDao(connectionString);
        }
        public List<OrderVendorProducesQuery> GetOrderVendorProduces(OrderVendorProducesQuery store, out int totalCount)/*返回供應商訂單查詢列表*/
        {
            try
            {
                return _iOrderVendorDao.GetOrderVendorProduces(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderVendorProducesMgr.GetOrderVendorProduces-->" + ex.Message, ex);
            }

        }

        public DataTable ExportCsv(OrderVendorProducesQuery store)
        {
            try
            {
                return _iOrderVendorDao.ExportCsv(store);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderVendorProducesMgr.ExportCsv-->" + ex.Message, ex);
            }
        
        }
        public DataTable GetProductItem()
        {
            try
            {
                return _iOrderVendorDao.GetProductItem();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderVendorProducesMgr.GetProductItem-->" + ex.Message, ex);
            }
        }
    }
}
