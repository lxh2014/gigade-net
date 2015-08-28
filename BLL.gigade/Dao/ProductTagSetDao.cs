/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTagSetDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:52:12 
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
    public class ProductTagSetDao : IProductTagSetImplDao
    {
        private IDBAccess _dbAccess;
        public ProductTagSetDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region IProductTagSetImplDao 成员

        public List<Model.ProductTagSet> Query(Model.ProductTagSet productTagSet)
        {
            StringBuilder strSql = new StringBuilder("select id,product_id,tag_id from product_tag_set where 1=1");
            if (productTagSet.product_id != 0)
            {
                strSql.AppendFormat(" and product_id={0}", productTagSet.product_id);
            }
            return _dbAccess.getDataTableForObj<Model.ProductTagSet>(strSql.ToString());
        }

        public string Delete(Model.ProductTagSet productTagSet)
        {
            StringBuilder strSql = new StringBuilder("delete from product_tag_set where ");
            strSql.AppendFormat(" product_id={0};", productTagSet.product_id);
            return strSql.ToString();
        }

        public string Save(Model.ProductTagSet productTagSet)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set(`product_id`,`tag_id`)");
            strSql.AppendFormat("values({0},{1});", productTagSet.product_id, productTagSet.tag_id);
            return strSql.ToString();
        }

        public string SaveFromOtherPro(Model.ProductTagSet productTagSet)
        {
            StringBuilder strSql = new StringBuilder("insert into product_tag_set(`product_id`,`tag_id`) select {0} as product_id,tag_id");
            strSql.AppendFormat(" from product_tag_set where product_id={0}", productTagSet.product_id);
            return strSql.ToString();
        }

        #endregion
    }
}
