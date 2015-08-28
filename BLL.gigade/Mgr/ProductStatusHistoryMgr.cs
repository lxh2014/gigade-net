using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
namespace BLL.gigade.Mgr
{
    public class ProductStatusHistoryMgr : BLL.gigade.Mgr.Impl.IProductStatusHistoryImplMgr
    {
        private BLL.gigade.Dao.Impl.IProductStatusHistoryImplDao _historyDao;
        public ProductStatusHistoryMgr(string connectionString)
        {
            _historyDao = new BLL.gigade.Dao.ProductStatusHistoryDao(connectionString);
        }

        public string Save(ProductStatusHistory save)
        {
            try
            {
                return _historyDao.Save(save);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryMgr-->SaveNoProductId" + ex.Message, ex);
            }
        }

        public string SaveNoProductId(BLL.gigade.Model.ProductStatusHistory save)
        {
            try
            {
                return _historyDao.SaveNoProductId(save);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryMgr-->SaveNoProductId" + ex.Message, ex);
            }
        }

        public string Delete(BLL.gigade.Model.ProductStatusHistory history)
        {
            try
            {
                return _historyDao.Delete(history);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryMgr-->Delete" + ex.Message, ex);
            }
        }

        public List<ProductStatusHistoryCustom> HistoryQuery(ProductStatusHistoryCustom query)
        {
            try
            {
                return _historyDao.HistoryQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryMgr-->HistoryQuery" + ex.Message, ex);
            }
        }

        public int UpdateColumn(BLL.gigade.Model.ProductStatusHistory save)
        {
            try
            {
                return _historyDao.UpdateColumn(save);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusHistoryMgr-->UpdateColumn" + ex.Message, ex);
            }
        }
    }
}
