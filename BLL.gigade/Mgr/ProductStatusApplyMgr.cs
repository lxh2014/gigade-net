using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class ProductStatusApplyMgr : BLL.gigade.Mgr.Impl.IProductStatusApplyImplMgr
    {
        BLL.gigade.Dao.Impl.IProductStatusApplyImplDao _applyDao;
        public ProductStatusApplyMgr(string connectionString)
        {
            _applyDao = new BLL.gigade.Dao.ProductStatusApplyDao(connectionString);
        }

        public ProductStatusApply Query(ProductStatusApply query)
        {
            try
            {
                return _applyDao.Query(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusApplyMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Save(ProductStatusApply apply)
        {
            try
            {
                return _applyDao.Save(apply);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusApplyMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public string Delete(ProductStatusApply apply)
        {
            try
            {
                return _applyDao.Delete(apply);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductStatusApplyMgr-->Delete-->" + ex.Message, ex);
            }
        }
    }
}
