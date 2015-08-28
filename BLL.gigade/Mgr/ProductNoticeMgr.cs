/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNoticeMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:10:52 
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
    public class ProductNoticeMgr : IProductNoticeImplMgr
    {
        private IProductNoticeImplDao _productNoticeDao;
        public ProductNoticeMgr(string connectionStr)
        {
            _productNoticeDao = new ProductNoticeDao(connectionStr);
        }

        #region IProductNoticeImplMgr 成员

        public List<Model.ProductNotice> Query(Model.ProductNotice productNotice)
        {
            try
            {
                return _productNoticeDao.Query(productNotice);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductNoticeMgr-->Query-->" + ex.Message, ex);
            }
        }

        #endregion
    }
}
