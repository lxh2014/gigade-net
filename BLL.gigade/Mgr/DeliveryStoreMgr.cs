using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
    public class DeliveryStoreMgr:IDeliveryStoreImplMgr
    {
        private IDeliveryStoreImplDao _deliveryDao;
        public DeliveryStoreMgr(string connectionString)
        {
            _deliveryDao = new DeliveryStoreDao(connectionString);
        }
        public int Save(DeliveryStore store)
        {
            try
            {
                return _deliveryDao.Save(store);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStoreMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
            
        }
        public int Update(DeliveryStore store)
        {
            
            try
            {
                return _deliveryDao.Update(store);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStoreMgr-->Update-->" + ex.Message, ex);
            }
        }
        public int Delete(int rodId)
        {
            
            try
            {
                return _deliveryDao.Delete(rodId);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStoreMgr-->Delete-->" + ex.Message, ex);
            }
        }
        public List<DeliveryStoreQuery> Query(DeliveryStore store, out int totalCount)
        {
            
            try
            {
                return _deliveryDao.Query(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("DeliveryStoreMgr-->Query-->" + ex.Message, ex);
            }
        }
    }
}
