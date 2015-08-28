/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductMapSetDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/12/18 15:52:06 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class ProductMapSetDao : IProductMapSetImplDao
    {
        private IDBAccess _dbAccess;
        public ProductMapSetDao(string connectionString)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Model.ProductMapSet> Query(Model.ProductMapSet query)
        {
            StringBuilder strSql = new StringBuilder("select rid,map_rid,item_id,set_num from product_map_set where 1=1 ");
            if (query.map_rid != 0)
            {
                strSql.AppendFormat("  and map_rid={0}", query.map_rid);
            }
            if (query.item_id != 0)
            {
                strSql.AppendFormat("  and item_id={0}", query.item_id);
            }
            return _dbAccess.getDataTableForObj<Model.ProductMapSet>(strSql.ToString());
        }

        public List<Model.ProductMapSet> Query(uint product_id)
        {
            string strSql = string.Format("select * from product_map_set where map_rid in (select rid from product_item_map where product_id = {0})", product_id);
            return _dbAccess.getDataTableForObj<Model.ProductMapSet>(strSql.ToString());
        }

        public List<Model.ProductMapSet> Query(ProductItemMap proItemMap)
        {
            string strSql = string.Format("select * from product_map_set where map_rid in (select rid from product_item_map where channel_id = {0} and channel_detail_id='{1}' and price_master_id={2})",proItemMap.channel_id, proItemMap.channel_detail_id,proItemMap.price_master_id);
            return _dbAccess.getDataTableForObj<Model.ProductMapSet>(strSql.ToString());
        }
        
        public string Delete(Model.ProductMapSet delete)
        {
            return string.Format("set sql_safe_updates=0;delete from product_map_set where map_rid = {0};set sql_safe_updates = 1;", delete.map_rid);
        }

        public string Save(Model.ProductMapSet save)
        {
            StringBuilder sqls = new StringBuilder("insert into product_map_set (`map_rid`, `item_id`, `set_num`) values ({0},");
            sqls.AppendFormat("{0},{1})", save.item_id, save.set_num);
            return sqls.ToString();
        }
        //根據item_map rid 和 item_id 查詢 set_num
        public List<Model.ProductMapSet> Query(uint map_id,uint item_id)
        {
            string strSql = string.Format("select * from product_map_set where map_rid={0} and item_id={1}",map_id,item_id);
            return _dbAccess.getDataTableForObj<Model.ProductMapSet>(strSql.ToString());
        }
    }
}
