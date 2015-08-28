/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTagSetMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:11:04 
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
    public class ProductTagSetMgr : IProductTagSetImplMgr
    {
        private IProductTagSetImplDao _productTagSetDao;
        public ProductTagSetMgr(string connectionStr)
        {
            _productTagSetDao = new ProductTagSetDao(connectionStr);
        }

        #region IProductTagSetImplMgr 成员

        public List<Model.ProductTagSet> Query(Model.ProductTagSet productTagSet)
        {
            try
            {
                return _productTagSetDao.Query(productTagSet);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetMgr-->Query-->" + ex.Message, ex);
            }
        }

        public string Delete(Model.ProductTagSet productTagSet)
        {
            try
            {
                return _productTagSetDao.Delete(productTagSet);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public string Save(Model.ProductTagSet productTagSet)
        {
            try
            {
                return _productTagSetDao.Save(productTagSet);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagSetMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public string SaveFromOtherPro(Model.ProductTagSet productTagSet)
        {
            return _productTagSetDao.SaveFromOtherPro(productTagSet);
        }

        #endregion
    }
}
