/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductMigrationMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/13 14:21:03 
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
    public class ProductMigrationMgr:IProductMigrationImplMgr
    {
        private IProductMigrationImplDao _productMigrationDao;
        public ProductMigrationMgr(string connectionString)
        {
            _productMigrationDao = new ProductMigrationDao(connectionString);
        }

        public Model.ProductMigrationMap GetSingle(Model.ProductMigrationMap query)
        {
            return _productMigrationDao.GetSingle(query);
        }
        public string SaveNoPrid(Model.ProductMigrationMap pMap)
        {
           return _productMigrationDao.SaveNoPrid(pMap);
        }
    }
}
