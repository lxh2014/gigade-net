#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductCategoryBannerMgr.cs
* 摘 要：
* 專區商品類別設置
* 当前版本：v1.0
* 作 者： shuangshuang0420j
* 完成日期：2014/12/30 
* 修改歷史：
*         
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System.Data;
using System.Collections;
namespace BLL.gigade.Mgr
{
    public class ProductCategoryBannerMgr : IProductCategoryBannerImplMgr
    {
        private IProductCagegoryBannerImplDao _prodCategoryBannerDao;
        private IProductCategorySetImplDao _prodCategorySetDao;
        private IProductCategoryImplDao _prodCateDao;
        private IParametersrcImplDao _paremDao;
        private string connectionStr;
        public ProductCategoryBannerMgr(string connectionString)
        {
            _prodCategoryBannerDao = new ProductCategoryBannerDao(connectionString);
            _prodCategorySetDao = new ProductCategorySetDao(connectionString);
            _prodCateDao = new ProductCategoryDao(connectionString);
            _paremDao = new ParametersrcDao(connectionString);
            connectionStr = connectionString;
        }

        //public List<ProductCategoryBannerQuery> GetProCateBanList(ProductCategoryBannerQuery query, out int totalCount)
        //{
        //    try
        //    {
        //        List<ProductCategoryBannerQuery> store = _prodCategoryBannerDao.GetProCateBanList(query, out totalCount);
        //        foreach (var item in store)
        //        {
        //            item.banner_catename = _prodCateDao.GetModelById(item.category_father_id).category_name;
        //            item.category_father_name = _paremDao.Query(new Parametersrc { ParameterType = "banner_cate", ParameterCode = item.banner_cateid.ToString() }).FirstOrDefault().parameterName;
        //        }
        //        return store;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ProductCategoryBannerMgr-->GetProCateBanList-->" + ex.Message, ex);
        //    }
        //}
        public List<ProductCategoryBannerQuery> GetProCateBanList(ProductCategoryBannerQuery query, out int totalCount)
        {
            try
            {
                return _prodCategoryBannerDao.GetProCateBanList(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCategoryBannerMgr-->GetProCateBanList-->" + ex.Message, ex);
            }
           
        }

        public List<ProductCategoryBannerQuery> QueryAll(ProductCategoryBannerQuery query)
        {
            try
            {
                return _prodCategoryBannerDao.QueryAll(query);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCategoryBannerMgr-->QueryAll-->" + ex.Message, ex);
            }
        }



        public DataTable isSaleProd(string cateIDs, uint banner_cateid)
        {
            try
            {
                string prods = string.Empty;
                if (banner_cateid != 754)
                {
                    prods = _prodCategoryBannerDao.GetProdsByCategorys(banner_cateid.ToString());
                }
                else
                {
                    prods = _prodCategoryBannerDao.GetProdsByCategorys(cateIDs);
                }

                if (!string.IsNullOrEmpty(prods))
                {
                    return _prodCategorySetDao.GetCateByProds(prods, cateIDs);
                }
                else
                {
                    return new DataTable();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerMgr-->isSaleProd-->" + ex.Message, ex);
            }
        }

        public int DeleteProCateBan(int rowId)
        {
            try
            {
                return _prodCategoryBannerDao.DeleteProCateBan(rowId);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerMgr-->DeleteProCateBan-->" + ex.Message, ex);

            }
        }
        /// <summary>
        /// 新增數據
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Save(string[] values, ProductCategoryBannerQuery query)
        {
            try
            {
                ArrayList insertList = new ArrayList();
                insertList.Add(_prodCategoryBannerDao.DeleteByBanCateId(query));
                for (int i = 0; i < values.Length; i++)
                {
                    insertList.Add(_prodCategoryBannerDao.Save(values[i].Split(','), query));
                }
                MySqlDao mySqlDao = new MySqlDao(connectionStr);
                return mySqlDao.ExcuteSqls(insertList);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCategoryBannerMgr-->Save-->" + ex.Message, ex);
            }
        }


        public List<ProductCategory> GetXGCate()
        {

            try
            {
                return _prodCategoryBannerDao.GetXGCate();
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerMgr-->GetXGCate-->" + ex.Message, ex);
            }
        }
        public List<ProductCategory> GetProductCategoryInfo(string categoryIds)
        {
            try
            {
                return _prodCategoryBannerDao.GetProductCategoryInfo(categoryIds);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerMgr-->GetProductProductCategory-->" + ex.Message, ex);
            }
        }


        public int UpdateState(ProductCategoryBannerQuery query)
        {
            try
            {
                return _prodCategoryBannerDao.UpdateState(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerMgr-->UpdateState-->" + ex.Message, ex);
            }
        }
        public DataTable GetXGCateByBanner(string category_ids, uint banner_cateid)
        {
            try
            {

                string prods = _prodCategoryBannerDao.GetProductByCateId(category_ids, banner_cateid);
                if (!string.IsNullOrEmpty(prods))
                {
                    return _prodCategorySetDao.GetCateByProds(prods, category_ids);
                }
                else
                {
                    return new DataTable();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBannerMgr-->GetProductByCateId-->" + ex.Message, ex);
            }
        }
        public bool DeleteByCateId(ProductCategoryBannerQuery query)
        {
            try
            {
                MySqlDao mySqlDao = new MySqlDao(connectionStr);
                ArrayList insertList = new ArrayList();
                insertList.Add(_prodCategoryBannerDao.DeleteByBanCateId(query));
                return mySqlDao.ExcuteSqls(insertList);
            }
            catch (Exception ex)
            {

                throw new Exception("ProductCategoryBannerMgr-->DeleteByCateId-->" + ex.Message, ex);
            }
        }
    }
}
