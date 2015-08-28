/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNoticeSetTempMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 16:15:33 
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
    public class ProductNoticeSetTempMgr : IProductNoticeSetTempImplMgr
    {
        private IProductNoticeSetTempImplDao _productNoticeSetDao;
        public ProductNoticeSetTempMgr(string connectStr)
        {
            _productNoticeSetDao = new ProductNoticeSetTempDao(connectStr);
        }

        #region IProductNoticeSetTempImplmgr 成员

        public List<Model.ProductNoticeSetTemp> Query(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.Query(productNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Delete(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.Delete(productNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string DeleteVendor(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.DeleteVendor(productNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->DeleteVendor-->" + ex.Message, ex);
            }
        }
        public string Save(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.Save(productNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public string MoveNotice(Model.ProductNoticeSetTemp proNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.MoveNotice(proNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->MoveNotice-->" + ex.Message, ex);
            }
        }

        public string SaveFromProNotice(Model.ProductNoticeSetTemp proNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.SaveFromProNotice(proNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->SaveFromProNotice-->" + ex.Message, ex);
            }
        }

        public List<Model.ProductNoticeSetTemp> QueryVendorProdNotice(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.QueryVendorProdNotice(productNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->QueryVendorProdNotice-->" + ex.Message, ex);
            }
        }
        public string Save_Vendor(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.Save_Vendor(productNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->Save_Vendor-->" + ex.Message, ex);
            }
        }

        public string VendorSaveFromProNotice(Model.ProductNoticeSetTemp proNoticeSetTemp,string old_id)
        {// 20140905 供應商複製用
            try
            {
                return _productNoticeSetDao.VendorSaveFromProNotice(proNoticeSetTemp, old_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->VendorSaveFromProNotice-->" + ex.Message, ex);
            }
        }

        #endregion

        #region 與供應商商品相關
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品的公告信息由臨時表移動到正式表
        /// </summary>
        /// <param name="proNoticeSetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_MoveNotice(Model.ProductNoticeSetTemp proNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.Vendor_MoveNotice(proNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->Vendor_MoveNotice-->" + ex.Message, ex);
            }
        }
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品的公告信息由臨時表移除
        /// </summary>
        /// <param name="productNoticeSetTemp">臨時表中的數據對象</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(Model.ProductNoticeSetTemp productNoticeSetTemp)
        {
            try
            {
                return _productNoticeSetDao.Vendor_Delete(productNoticeSetTemp);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetTempMgr-->Vendor_Delete-->" + ex.Message, ex);
            }
        } 
        #endregion
    }
}
