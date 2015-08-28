using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class RecommendedProductAttributeMgr
    {
        RecommendedProductAttributeDao _rProductAttribute;
        public RecommendedProductAttributeMgr(string connectionStr)
        {
            _rProductAttribute = new RecommendedProductAttributeDao(connectionStr);
        }

        public int GetMsgByProductId(int productId)
        {
            try
            {
                return _rProductAttribute.GetMsgByProductId(productId);
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeMgr-->GetMsgByProductId-->" + ex.Message, ex);
            }
        }

        public int Save(RecommendedProductAttribute rPA)
        {
            try
            {
                return _rProductAttribute.Save(rPA);
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->Save-->" + ex.Message, ex);
            }
        }

        public int Update(RecommendedProductAttribute rPA)
        {
            try
            {
                return _rProductAttribute.Update(rPA);
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->Update-->" + ex.Message, ex);
            }
        }

        public int Delete(int productId)
        {
            try
            {
                return _rProductAttribute.Delete(productId);
            }
            catch (Exception ex)
            {
                throw new Exception("RecommendedProductAttributeDao-->Delete-->" + ex.Message, ex);
            }
        }
    }
}
