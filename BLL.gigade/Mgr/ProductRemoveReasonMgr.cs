using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class ProductRemoveReasonMgr:IProductRemoveReasonImplMgr
    {
        private IProductRemoveReasonImplDao _iproductRemoveReason;
        public ProductRemoveReasonMgr(string connectionStr)
        {
            _iproductRemoveReason = new ProductRemoveReasonDao(connectionStr);
        }

        public DataTable GetStockLessThanZero()
        {
            try
            {
                return _iproductRemoveReason.GetStockLessThanZero();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->GetStockLessThanZero-->" + ex.Message, ex);
            }
        }


        public string InsertProductRemoveReason(ProductRemoveReason prr)
        {
           try
            {
                return _iproductRemoveReason.InsertProductRemoveReason(prr);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->InsertProductRemoveReason-->" + ex.Message, ex);
            }
        
        }


        public DataTable GetStockMsg()
        {
            try
            {
                return _iproductRemoveReason.GetStockMsg();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->GetStockMsg-->" + ex.Message, ex);
            }
        
        }


        public string DeleteProductRemoveReason(ProductRemoveReason prr)
        {
            try
            {
                return _iproductRemoveReason.DeleteProductRemoveReason(prr);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->DeleteProductRemoveReason-->" + ex.Message, ex);
            }
        }

        public string UpdateProductStatus(Product pt)
        {
            try
            {
                return _iproductRemoveReason.UpdateProductStatus(pt);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->UpdateProductStatus-->" + ex.Message, ex);
            }
        }

        public string InsertIntoProductStatusHistory(ProductStatusHistory psh)
        {
            try
            {
                return _iproductRemoveReason.InsertIntoProductStatusHistory(psh);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->InsertIntoProductStatusHistory-->" + ex.Message, ex);
            }
        }


        public DataTable GetOutofStockMsg()
        {
            try
            {
                return _iproductRemoveReason.GetOutofStockMsg();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->GetOutofStockMsg-->" + ex.Message, ex);
            }
        }


        public DataTable GetProductRemoveReasonList()
        {
            try
            {
                return _iproductRemoveReason.GetProductRemoveReasonList();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->GetProductRemoveReasonList-->" + ex.Message, ex);
            }
        }



        public int ProductRemoveReasonTransact(string str)
        {
            try
            {
                return _iproductRemoveReason.ProductRemoveReasonTransact(str);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->ProductRemoveReasonTransact-->" + ex.Message, ex);
            }
        }


        public DataTable GetDeleteProductRemoveReasonList()
        {
            try
            {
                return _iproductRemoveReason.GetDeleteProductRemoveReasonList();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductRemoveReasonMgr-->GetDeleteProductRemoveReasonList-->" + ex.Message, ex);
            }
        }

    }
}
