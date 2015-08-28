/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNoticeSetMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:11:20 
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
    public class ProductNoticeSetMgr : IProductNoticeSetImplMgr
    {
        private IProductNoticeSetImplDao _productNoticeSetDao;
        public ProductNoticeSetMgr(string connectionStr)
        {
            _productNoticeSetDao = new ProductNoticeSetDao(connectionStr);
        }

        #region IProductNoticeTagImplMgr 成员

        public List<Model.ProductNoticeSet> Query(Model.ProductNoticeSet productNoticeSet)
        {
            try
            {
                return _productNoticeSetDao.Query(productNoticeSet);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Delete(Model.ProductNoticeSet productNoticeSet)
        {
            try
            {
                return _productNoticeSetDao.Delete(productNoticeSet);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string Save(Model.ProductNoticeSet productNoticeSet)
        {
            try
            {
                return _productNoticeSetDao.Save(productNoticeSet);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeSetMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public string SaveFromOtherPro(Model.ProductNoticeSet productNoticeSet)
        {
            return _productNoticeSetDao.SaveFromOtherPro(productNoticeSet);
        }
        #endregion
    }
}
