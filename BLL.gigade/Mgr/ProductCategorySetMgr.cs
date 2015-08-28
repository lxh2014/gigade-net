using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Collections;
using BLL.gigade.Model.Custom;
using System.Data;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
    public class ProductCategorySetMgr : IProductCategorySetImplMgr
    {
        private IProductCategorySetImplDao _categorySetDao;
        private IProductImplMgr _proMgr;
        private MySqlDao _sqlDao;

        public ProductCategorySetMgr(string conStr)
        {
            _categorySetDao = new ProductCategorySetDao(conStr);
            _proMgr = new ProductMgr(conStr);
            _sqlDao = new MySqlDao(conStr);

        }

        public List<ProductCategorySet> Query(ProductCategorySet queryModel)
        {
            try
            {
                return _categorySetDao.Query(queryModel);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->Query(ProductCategorySet queryModel)-->" + ex.Message, ex);
            }

        }

        public List<ProductCategorySetCustom> Query(ProductCategorySetCustom query)
        {

            try
            {
                return _categorySetDao.Query(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->Query(ProductCategorySetCustom query)-->" + ex.Message, ex);
            }
        }

        public string Save(ProductCategorySet save)
        {

            try
            {
                return _categorySetDao.Save(save);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public string SaveFromOtherPro(ProductCategorySet saveModel)
        {
            return _categorySetDao.SaveFromOtherPro(saveModel);
        }

        public string Delete(ProductCategorySet delModel, string deStr = "0")
        {

            try
            {
                return _categorySetDao.Delete(delModel, deStr);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public bool ProductCateUpdate(List<ProductCategorySet> cateList, Product proModel)
        {

            try
            {
                ArrayList sqls = new ArrayList();

                sqls.Add(_categorySetDao.Delete(cateList[0]));
                foreach (ProductCategorySet item in cateList)
                {
                    sqls.Add(_categorySetDao.Save(item));
                }
                sqls.Add(_proMgr.Update(proModel));
                return _sqlDao.ExcuteSqls(sqls);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->ProductCateUpdate-->" + ex.Message, ex);
            }
        }

        public string SaveNoPrid(ProductCategorySet save)
        {
            try
            {
                return _categorySetDao.SaveNoPrid(save);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->SaveNoPrid-->" + ex.Message, ex);
            }

        }

        public int Insert(ProductCategorySet save)
        {
            try
            {
                return _categorySetDao.Insert(save);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->Insert-->" + ex.Message, ex);
            }
        }

        public int DeleteProductByModel(ProductCategorySet delModel)
        {
            try
            {
                return _categorySetDao.DeleteProductByModel(delModel);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->DeleteProductByModel-->" + ex.Message, ex);
            }
        }


        public bool DeleteProductByModelArry(string bids, string cids, string pids)
        {
            try
            {
                string[] arrybids = bids.Split('|');
                string[] arrycids = cids.Split('|');
                string[] arrypids = pids.Split('|');
                ArrayList sqls = new ArrayList();
                for (int i = 0; i < arrybids.Length; i++)
                {
                    if (arrybids[i].ToString() != "" && arrypids[i].ToString() != "")
                    {
                        ProductCategorySet pcs = new ProductCategorySet();
                        pcs.Brand_Id = Convert.ToUInt32(arrybids[i].ToString());
                        pcs.Category_Id = Convert.ToUInt32(arrycids[i].ToString());
                        pcs.Product_Id = Convert.ToUInt32(arrypids[i].ToString());

                        sqls.Add(_categorySetDao.DeleteProductByModelStr(pcs));
                    }
                }

                return _sqlDao.ExcuteSqls(sqls);

            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->DeleteProductByModelArry-->" + ex.Message, ex);
            }
        }

        public DataTable QueryBrand(string webtype, int content_id)
        {
            try
            {
                return _categorySetDao.QueryBrand(webtype, content_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->QueryBrand-->" + ex.Message, ex);
            }
        }

        public DataTable QueryProduct(string category_id)
        {
            try
            {
                return _categorySetDao.QueryProduct(category_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->QueryProduct-->" + ex.Message, ex);
            }
        }

        //add by wwei0216w 2015/2/24
        /// <summary>
        /// 根據商品id修改品牌id
        /// </summary>
        /// <param name="pcs">一個ProductCategorySet對象</param>
        /// <returns>將要執行的sql語句</returns>
        public string UpdateBrandId(ProductCategorySet pcs)
        {
            try
            {
                return _categorySetDao.UpdateBrandId(pcs);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->UpdateBrandId" + ex.Message, ex);
            }
        }


        public DataTable GetCateByProds(string prods, string cateids)
        {
            try
            {
                return _categorySetDao.GetCateByProds(prods, cateids);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->GetCateByProds" + ex.Message, ex);
            }
        }


        public List<ProductCategorySet> QueryMsg(ProductCategorySetQuery queryModel)
        {
            try
            {
                return _categorySetDao.QueryMsg(queryModel);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetMgr-->Query(ProductCategorySet QueryMsg)-->" + ex.Message, ex);
            }
        }
    }
}
