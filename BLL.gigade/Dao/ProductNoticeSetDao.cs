/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNoticeSetDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:51:35 
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
    public class ProductNoticeSetDao : IProductNoticeSetImplDao
    {
        private IDBAccess _dbAccess;
        public ProductNoticeSetDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region IProductNoticeSetImplDao 成员

        public List<Model.ProductNoticeSet> Query(Model.ProductNoticeSet productNoticeSet)
        {
            StringBuilder strSql = new StringBuilder("select product_id,notice_id from product_notice_set where 1=1");
            if (productNoticeSet.product_id != 0)
            {
                strSql.AppendFormat(" and product_id={0}", productNoticeSet.product_id);
            }
            return _dbAccess.getDataTableForObj<Model.ProductNoticeSet>(strSql.ToString());
        }

        public string Delete(Model.ProductNoticeSet productNoticeSet)
        {
            StringBuilder strSql = new StringBuilder("delete from product_notice_set where ");
            strSql.AppendFormat(" product_id={0};", productNoticeSet.product_id);
            return strSql.ToString();
        }

        public string Save(Model.ProductNoticeSet productNoticeSet)
        {
            StringBuilder strSql = new StringBuilder("insert into product_notice_set(`product_id`,`notice_id`)");
            strSql.AppendFormat("values({0},{1});", productNoticeSet.product_id, productNoticeSet.notice_id);
            return strSql.ToString();
        }

        public string SaveFromOtherPro(Model.ProductNoticeSet productNoticeSet)
        {
            StringBuilder strSql = new StringBuilder("insert into product_notice_set(`product_id`,`notice_id`) select {0} as product_id,notice_id");
            strSql.AppendFormat("  from product_notice_set where product_id={0}", productNoticeSet.product_id);
            return strSql.ToString();
        }
        #endregion
    }
}
