using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class ProductCategorySetTempMgr : IProductCategorySetTempImplMgr
    {
        private IProductCategorySetTempImplDao _cateSetDao;
        public ProductCategorySetTempMgr(string conStr)
        {
            _cateSetDao = new ProductCategorySetTempDao(conStr);
        }

        public List<ProductCategorySetTemp> Query(ProductCategorySetTemp queryTemp)
        {

            try
            {
                return _cateSetDao.Query(queryTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->Query-->" + ex.Message, ex);
            }
        }



        public bool Save(List<ProductCategorySetTemp> saveTempList)
        {
            bool result = true;
            try
            {

                foreach (ProductCategorySetTemp item in saveTempList)
                {
                    if (_cateSetDao.Save(item) <= 0)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw new Exception("ProductCategorySetTempMgr-->SingleCompareSave-->" + ex.Message, ex); ;
            }
            return result;
        }

        public bool Delete(ProductCategorySetTemp delTemp,string deStr="")
        {

            try
            {
                if (deStr == "")
                    return true;
                return _cateSetDao.Delete(delTemp, deStr) <= 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string TempMoveCategory(ProductCategorySetTemp proCategorySetTemp)
        {

            try
            {
                return _cateSetDao.TempMoveCategory(proCategorySetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->TempMoveCategory-->" + ex.Message, ex);
            }
        }
        public string TempDelete(ProductCategorySetTemp proCategorySetTemp)
        {

            try
            {
                return _cateSetDao.TempDelete(proCategorySetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->TempDelete-->" + ex.Message, ex);
            }
        }

        public string SaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp)
        {
            try
            {
                return _cateSetDao.SaveFromCategorySet(proCategorySetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->SaveFromCategorySet-->" + ex.Message, ex);
            }
        }
        #region 供應商商品處理
        public string TempDeleteByVendor(ProductCategorySetTemp proCategorySetTemp)
        {
            try
            {
                return _cateSetDao.TempDeleteByVendor(proCategorySetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->TempDeleteByVendor-->" + ex.Message, ex);
            }
        }

        public bool DeleteByVendor(ProductCategorySetTemp delTemp)
        {
            try
            {
                return _cateSetDao.DeleteByVendor(delTemp) <= 0 ? false : true;
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->DeleteByVendor-->" + ex.Message, ex);
            }
        }

        public bool SaveByVendor(List<ProductCategorySetTemp> saveTempList)
        {
            bool result = true;
            try
            {
                _cateSetDao.DeleteByVendor(saveTempList[0]);

                foreach (ProductCategorySetTemp item in saveTempList)
                {
                    if (_cateSetDao.SaveByVendor(item) <= 0)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                _cateSetDao.DeleteByVendor(saveTempList[0]);
                throw new Exception("ProductCategorySetTempMgr-->SaveByVendor-->" + ex.Message, ex); ;
            }

            try
            {
                if (!result)
                {
                    _cateSetDao.DeleteByVendor(saveTempList[0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->SaveByVendor-->" + ex.Message, ex);
            }

            return result;
        }

        public List<ProductCategorySetTemp> QueryByVendor(ProductCategorySetTemp queryTemp)
        {
            try
            {
                return _cateSetDao.QueryByVendor(queryTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->QueryByVendor-->" + ex.Message, ex);
            }
        }
        public string VendorSaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp,string  old_product_Id)
        {
            try
            {
                return _cateSetDao.VendorSaveFromCategorySet(proCategorySetTemp, old_product_Id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->VendorSaveFromCategorySet-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品類別信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proCategorySetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_TempMoveCategory(ProductCategorySetTemp proCategorySetTemp)
        {
            try
            {
                return _cateSetDao.Vendor_TempMoveCategory(proCategorySetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->Vendor_TempMoveCategory-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品類別信息由臨時表移除
        /// </summary>
        /// <param name="proCategorySetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_TempDelete(ProductCategorySetTemp proCategorySetTemp)
        {
            try
            {
                return _cateSetDao.Vendor_TempDelete(proCategorySetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductCategorySetTempMgr-->Vendor_TempDelete-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
