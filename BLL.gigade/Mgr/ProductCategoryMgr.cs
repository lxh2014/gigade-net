using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class ProductCategoryMgr : IProductCategoryImplMgr
    {
        IProductCategoryImplDao _cateDao;
        public ProductCategoryMgr(string connectionStr)
        {
            _cateDao = new ProductCategoryDao(connectionStr);
        }

        public List<ProductCategoryCustom> Query(int fatherId, int status = 1)
        {
            return _cateDao.Query(fatherId);
        }

        public List<Model.ProductCategory> QueryAll(Model.ProductCategory query)
        {
            return _cateDao.QueryAll(query);
        }

        public List<Model.ProductCategory> GetProductCate(Model.ProductCategory query)
        {
            return _cateDao.GetProductCate(query);
        }

        public List<ProductCategoryCustom> cateQuery(int fatherId)
        {

            try
            {
                return _cateDao.cateQuery(fatherId);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->cateQuery-->" + ex.Message, ex);
            }
        }
        public uint GetCateID(string eventId)
        {

            try
            {
                return _cateDao.GetCateID(eventId);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->GetMaxCateID-->" + ex.Message, ex);
            }
        }

        public int Save(Model.ProductCategory model)
        {

            try
            {
                return _cateDao.Save(model);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->Save->" + ex.Message, ex);
            }
        }
        public int Update(Model.ProductCategory model)
        {

            try
            {
                return _cateDao.Update(model);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->Update->" + ex.Message, ex);
            }
        }
        public int Detete(Model.ProductCategory model)
        {
            try
            {
                return _cateDao.Delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->Delete->" + ex.Message, ex);
            }
        }
        public ProductCategory GetModelById(uint id)
        {
            try
            {
                return _cateDao.GetModelById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->GetModelById->" + ex.Message, ex);
            }
        }


        public string SaveCategory(Model.ProductCategory model)
        {
            try
            {
                return _cateDao.SaveCategory(model);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->SaveCategory->" + ex.Message, ex);
            }
        }



        public int Delete(ProductCategory model)
        {
            try
            {
                return _cateDao.Delete(model);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->Delete->" + ex.Message, ex);
            }
        }


        public List<ProdPromoQuery> GetList(ProdPromo store, out int totalCount)
        {
            try
            {
                return _cateDao.GetList(store,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->GetList->" + ex.Message, ex);
            }
        }
        public int UpStatus(ProdPromoQuery store)
        {
            try
            {
                return _cateDao.UpStatus(store);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->UpStatus->" + ex.Message, ex);
            }
        }


        public int UpdateUrl(ProdPromo store)
        {
            try
            {
                return _cateDao.UpdateUrl(store);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryMgr-->UpdateUrl->" + ex.Message, ex);
            }
        }

        public DataTable GetProductCategoryStore()
        {
            try
            {
                return _cateDao.GetProductCategoryStore();
            }
            catch (Exception ex)
            {
               throw new Exception("ProductCategoryMgr-->GetProductCategoryStore->" + ex.Message, ex);
            }
        }
      
    }
}
