/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductMapSetMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/12/18 16:37:23 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Mgr
{
    public class ProductMapSetMgr : IProductMapSetImplMgr
    {
        private IProductMapSetImplDao _productMapSetDao;
        public ProductMapSetMgr(string connectionString)
        {
            _productMapSetDao = new ProductMapSetDao(connectionString);
        }

        public List<Model.ProductMapSet> Query(Model.ProductMapSet query)
        {
            return _productMapSetDao.Query(query);
        }
        public List<Model.ProductMapSet> Query(uint product_id)
        {
            return _productMapSetDao.Query(product_id);
        }
        public List<Model.ProductMapSet> Query(Model.ProductItemMap pim)
        {
            return _productMapSetDao.Query(pim);
        }
        //根據item_map rid 和 item_id 查詢 set_num
        public List<Model.ProductMapSet> Query(uint map_id, uint item_id)
        {
            return _productMapSetDao.Query(map_id, item_id);
        }
    }
}
