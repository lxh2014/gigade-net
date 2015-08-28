/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTagDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:51:03 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Dao
{
    public class ProductTagDao : IProductTagImplDao
    {
        private IDBAccess _dbAccess;
        public ProductTagDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public List<Model.ProductTag> Query(Model.ProductTag productTag)
        {
            StringBuilder strSql = new StringBuilder("select tag_id,tag_name,tag_sort,tag_status,tag_filename,tag_createdate,tag_updatedate,tag_ipfrom from product_tag where ");
            strSql.AppendFormat(" tag_status={0}", productTag.tag_status);
            return _dbAccess.getDataTableForObj<Model.ProductTag>(strSql.ToString());
        }
    }
}
