using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Mgr
{
    public class ProductConsultMgr : IProductConsultImplMgr
    {

        private IProductConsultImplDao _productConsultImplDao;
        public ProductConsultMgr(string connectionString)
        {
            _productConsultImplDao = new ProductConsultDao(connectionString);
        }
        public List<ProductConsultQuery> Query(ProductConsultQuery store, out int totalCount)
        {
            try
            {
                return _productConsultImplDao.Query(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultMgr-->Query-->" + ex.Message, ex);
            }
        }
        public int UpdateActive(ProductConsultQuery model)
        {
            try
            {
                return _productConsultImplDao.UpdateActive(model);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }
        public int SaveProductConsultAnswer(ProductConsultQuery query)
        {
            try
            {
                return _productConsultImplDao.SaveProductConsultAnswer(query);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultMgr-->SaveProductConsultAnswer-->" + ex.Message, ex);
            }
        }

        public ProductConsultQuery GetProductInfo(ProductConsultQuery query)
        {
            try
            {
                return _productConsultImplDao.GetProductInfo(query);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultMgr-->UpdateActive-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 更改回覆狀態
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateAnswerStatus(ProductConsultQuery model)
        {
            try
            {
                return _productConsultImplDao.UpdateAnswerStatus(model);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultMgr-->UpdateAnswerStatus-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 更改諮詢類型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateConsultType(ProductConsultQuery model)
        {
            try
            {
                return _productConsultImplDao.UpdateConsultType(model);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultMgr-->UpdateConsultType-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 查詢郵件群組的編號
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public DataTable QueryMailGroup(ProductConsultQuery query)
        {
            try
            {
                return _productConsultImplDao.QueryMailGroup(query);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductConsultMgr-->QueryMailGroup-->" + ex.Message, ex);
            }
        }

    }
}
