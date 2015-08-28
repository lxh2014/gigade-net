using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class ProductStockImportMgr:IProductStockImportImplMgr
    {
        IProductStockImportImplDao _iPStockDao;
        public ProductStockImportMgr(string connectionString)
        {
            _iPStockDao = new ProductStockImportDao(connectionString);
        }
        public int UpdateStock(ProductItem pi)
        {
            try
            {
                return _iPStockDao.UpdateStock(pi);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStockImportMgr.UpdateStock-->" + ex.Message, ex);
            }
        }

    }
}
