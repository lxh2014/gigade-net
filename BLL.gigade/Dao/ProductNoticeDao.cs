/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNoticeDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:51:20 
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
    public class ProductNoticeDao : IProductNoticeImplDao
    {
        private IDBAccess _dbAccess;
        public ProductNoticeDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        #region IProductNoticeImplDao 成员

        public List<Model.ProductNotice> Query(Model.ProductNotice productNotice)
        {
            StringBuilder strSql = new StringBuilder("select notice_id,notice_name,notice_sort,notice_status,notice_filename,notice_createdate,notice_updatedate,notice_ipfrom from product_notice ");
            strSql.AppendFormat(" where notice_status={0}", productNotice.notice_status);
            return _dbAccess.getDataTableForObj<Model.ProductNotice>(strSql.ToString());
        }

        #endregion
    }
}
