/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTagSetTempMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 16:15:15 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class ProductTagSetTempMgr : IProductTagSetTempImplMgr
    {
        private IProductTagSetTempImplDao _productTagSetTempDao;
        public ProductTagSetTempMgr(string connectStr)
        {
            _productTagSetTempDao = new ProductTagSetTempDao(connectStr);
        }

        #region IProductTagSetTempImplMgr 成员

        public List<Model.ProductTagSetTemp> Query(Model.ProductTagSetTemp productTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.Query(productTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Delete(Model.ProductTagSetTemp productTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.Delete(productTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->Delete-->" + ex.Message, ex);
            }
        }
        public string DeleteVendor(Model.ProductTagSetTemp productTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.DeleteVendor(productTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->DeleteVendor-->" + ex.Message, ex);
            }
        }
        public string Save(Model.ProductTagSetTemp productTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.Save(productTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public string MoveTag(Model.ProductTagSetTemp proTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.MoveTag(proTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->MoveTag-->" + ex.Message, ex);
            }
        }

        public string SaveFromTag(Model.ProductTagSetTemp proTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.SaveFromTag(proTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->SaveFromTag-->" + ex.Message, ex);
            }
        }
        public List<Model.ProductTagSetTemp> QueryVendorTagSet(Model.ProductTagSetTemp proTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.QueryVendorTagSet(proTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->QueryVendorTagSet-->" + ex.Message, ex);
            }
        }


        public string VendorTagSetTempSave(Model.ProductTagSetTemp productTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.VendorTagSetTempSave(productTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->VendorTagSetTempSave-->" + ex.Message, ex);
            }
        }
        public string VendorSaveFromTag(Model.ProductTagSetTemp proTagSetTemp,string old_id)
        {
            try
            {
                return _productTagSetTempDao.VendorSaveFromTag(proTagSetTemp, old_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->VendorSaveFromTag-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 與供應商商品相關
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品標籤信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proTagSetTemp">臨時表中的商品標籤信息數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_MoveTag(Model.ProductTagSetTemp proTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.Vendor_MoveTag(proTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->Vendor_MoveTag-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 管理員核可供應商建立的商品時將商品標籤信息由臨時表移除
        /// </summary>
        /// <param name="productTagSetTemp">臨時表中的商品標籤信息數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(Model.ProductTagSetTemp productTagSetTemp)
        {
            try
            {
                return _productTagSetTempDao.Vendor_Delete(productTagSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetTempMgr-->Vendor_Delete-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
