/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTagMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:10:35 
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
    public class ProductTagMgr : IProductTagImplMgr
    {
        private IProductTagImplDao _productTagDao;
        public ProductTagMgr(string connectionStr)
        {
            _productTagDao = new ProductTagDao(connectionStr);
        }

        #region IProductTagImplMgr 成员

        public List<Model.ProductTag> Query(Model.ProductTag productTag)
        {
            try
            {
                return _productTagDao.Query(productTag);
            }
            catch (Exception ex)
            {
                throw new Exception("ProductTagMgr-->Query-->" + ex.Message, ex);
            }
        }

        #endregion
    }
}
