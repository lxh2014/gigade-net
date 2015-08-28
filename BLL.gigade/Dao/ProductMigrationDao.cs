/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductMigrationDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/13 14:20:18 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ProductMigrationDao:IProductMigrationImplDao
    {
        private IDBAccess _dbAccess;
        public ProductMigrationDao(string connectionString)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public Model.ProductMigrationMap GetSingle(Model.ProductMigrationMap query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder("select rowid,product_id,temp_id from product_migration_map where 1=1");
            if (!string.IsNullOrEmpty(query.temp_id))
            {
                strSql.AppendFormat(" and temp_id='{0}'", query.temp_id);
            }

            return _dbAccess.getSinggleObj<Model.ProductMigrationMap>(strSql.ToString());
        }

        public string SaveNoPrid(Model.ProductMigrationMap pMap)
        {
            pMap.Replace4MySQL();
            StringBuilder stb = new StringBuilder("insert into product_migration_map(`product_id`,`temp_id`) values({0},");
            stb.AppendFormat("'{0}')",pMap.temp_id);
            return stb.ToString();
        }
    }
}
