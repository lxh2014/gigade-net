using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class ProductDeliverySetTempMgr : IProductDeliverySetTempImplMgr
    {
        #region IProductDeliverySetTempImplMgr 成员
        private IProductDeliverySetTempImplDao _proDeliSetTempDao;
        public ProductDeliverySetTempMgr(string connectionStr)
        {
           _proDeliSetTempDao= new ProductDeliverySetTempDao(connectionStr);
        }

        public bool Save(List<Model.ProductDeliverySetTemp> proDeliSets,int delProdId,int comboType,int writerId)
        {
            return _proDeliSetTempDao.Save(proDeliSets,delProdId,comboType,writerId)>0;
        }

        public List<Model.ProductDeliverySetTemp> QueryByProductId(Model.ProductDeliverySetTemp query)
        {
            return _proDeliSetTempDao.QueryByProductId(query);
        }

        public string MoveProductDeliverySet(Model.ProductDeliverySetTemp proDelSetTemp)
        {
            return _proDeliSetTempDao.MoveProductDeliverySet(proDelSetTemp);
        }

        public string SaveFromProDeliverySet(Model.ProductDeliverySetTemp proDelSetTemp)
        {
            return _proDeliSetTempDao.SaveFromProDeliverySet(proDelSetTemp);
        }

        public string Delete(Model.ProductDeliverySetTemp proDelSetTemp)
        {
            return _proDeliSetTempDao.Delete(proDelSetTemp);
        }

       #endregion
    }
}
