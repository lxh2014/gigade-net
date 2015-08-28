using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class ProductCategoryBrandMgr
    {
        private ProductCategoryBrandDao cateBrandDao;
        private MySqlDao _mysqlDao;
        private DBAccess.MySqlAccess _access;
        public ProductCategoryBrandMgr(string connectionstring)
        {
            cateBrandDao = new ProductCategoryBrandDao(connectionstring);
            _mysqlDao = new MySqlDao(connectionstring);
            _access = new DBAccess.MySqlAccess(connectionstring);
        }
        public List<ProductCategoryBrandQuery> GetCateBrandList(ProductCategoryBrandQuery cateBeandQuery, out int totalCount)
        {
            try
            {
                return cateBrandDao.GetCateBrandList(cateBeandQuery, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandMgr-->GetCateBrandList" + ex.Message, ex);
            }
        }
        public List<ProductCategoryBrand> GetSaledProduct(uint XGCateID, int banner_cateid)
        {
            try
            {
                return cateBrandDao.GetSaledProduct(XGCateID, banner_cateid);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandMgr-->GetSaledProduct" + ex.Message, ex);
            }
        }
        public List<ProductCategoryBrand> GetProductByCondi(uint XGCateId, int banner_cateid)
        {
            try
            {
                return cateBrandDao.GetProductByCondi(XGCateId, banner_cateid);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandMgr-->GetProductByCondi" + ex.Message, ex);
            }
        }
        //public bool InsertCateBrand(List<ProductCategoryBrand> tpcQueryLi, int banner_cate_id)
        //{
        //    try
        //    {
        //        ArrayList arryList = new ArrayList();
        //        arryList.Add(cateBrandDao.DeleteCateBrand(banner_cate_id));
        //        for (int i = 0; i < tpcQueryLi.Length; i++)
        //        {
        //            arryList.Add(cateBrandDao.InsertCateBrand(tpcQueryLi[i].Split(','), banner_cate_id));
        //        }
        //        //foreach (var item in tpcQueryLi)
        //        //{
        //        //    arryList.Add(cateBrandDao.InsertCateBrand(item, banner_cate_id));
        //        //}
        //        return _mysqlDao.ExcuteSqls(arryList);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ProductCategoryBrandMgr-->InsertCateBrand-->" + ex.Message, ex);
        //    }
        //}
        public bool InsertCateBrand(string[] tpcQueryLi, int banner_cate_id)
        {
            try
            {
                ArrayList arryList = new ArrayList();
                arryList.Add(cateBrandDao.DeleteCateBrand(banner_cate_id));
                for (int i = 0; i < tpcQueryLi.Length; i++)
                {
                    arryList.Add(cateBrandDao.InsertCateBrand(tpcQueryLi[i].Split(','), banner_cate_id));
                }
                //foreach (var item in tpcQueryLi)
                //{
                //    arryList.Add(cateBrandDao.InsertCateBrand(item, banner_cate_id));
                //}
                return _mysqlDao.ExcuteSqls(arryList);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandMgr-->InsertCateBrand-->" + ex.Message, ex);
            }
        }
        public bool DeleteCateBrand(int banner_cate_id)
        {
            try
            {
                ArrayList insertList = new ArrayList();
                insertList.Add(cateBrandDao.DeleteCateBrand(banner_cate_id));
                return _mysqlDao.ExcuteSqls(insertList);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategoryBrandMgr-->InsertCateBrand-->" + ex.Message, ex);
            }
        }

    }
}
