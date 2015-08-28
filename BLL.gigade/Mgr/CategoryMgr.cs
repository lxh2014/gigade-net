using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class CategoryMgr : ICategoryImplMgr
    {
        private ICategoryImplDao _categoryDao;
        public CategoryMgr(string connectionString)
        {
            _categoryDao = new CategoryDao(connectionString);
        }

        public List<CategoryQuery> GetCategoryList(CategoryQuery query, out int totalCount)
        {
            try
            {
                return _categoryDao.GetCategoryList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryMgr-->GetCategoryList-->" + ex.Message, ex);
            }
        }
        public string GetSum(CategoryQuery query)
        {
            try
            {
                return _categoryDao.GetSum(query);
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryMgr-->GetSum-->" + ex.Message, ex);
            }
        }
        public List<CategoryQuery> GetCategory()
        {
            try
            {
                return _categoryDao.GetCategory();
            }
            catch (Exception ex)
            {
                throw new Exception("CategoryMgr-->GetCategory-->" + ex.Message, ex);
            }
        }
        public List<CategoryQuery> GetProductCategoryList(CategoryQuery cq, out int totalCount)
        {
            try
            {
                return _categoryDao.GetProductCategoryList(cq, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("CategoryMgr-->GetProductCategoryList-->" + ex.Message, ex);
            }

        }
        public int ProductCategorySave(CategoryQuery cq)
        {
            try
            {
                return _categoryDao.ProductCategorySave(cq);
            }
            catch (Exception ex)
            {

                throw new Exception("CategoryMgr-->ProductCategorySave-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查詢一條類別信息根據編號
        /// </summary>
        /// <param name="cq"></param>
        /// <returns></returns>
        public CategoryQuery GetProductCategoryById(CategoryQuery cq)
        {
            try
            {
                return _categoryDao.GetProductCategoryById(cq);
            }
            catch (Exception ex)
            {

                throw new Exception("CategoryMgr-->ProductCategorySave-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更改狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateActive(CategoryQuery model)
        {
            return _categoryDao.UpdateActive(model);
        }

        public List<Model.ProductCategory> GetProductCategoryCSV(CategoryQuery query)
        {

            try
            {
                return _categoryDao.GetProductCategoryCSV(query);
            }
            catch (Exception ex)
            {

                throw new Exception("CategoryMgr-->GetProductCategoryCSV-->" + ex.Message, ex);
            }
        }
    }
}
