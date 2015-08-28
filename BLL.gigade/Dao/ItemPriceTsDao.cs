/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ItemPriceTsDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/07/18 10:58:14 
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
    public class ItemPriceTsDao:IItemPriceTsImplDao
    {
        private IDBAccess _dbAccess;
        public ItemPriceTsDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }


        //修改站價格不再更新item_price表而是更新item_price_ts表 add by xiangwang0413w 2014/07/17 
        public string UpdateTs(Model.ItemPrice itemPrice)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0; ");
            if (itemPrice.item_price_id == 0)
            {
                strSql.AppendFormat("set sql_safe_updates=0;delete from item_price_ts where price_master_id={0};set sql_safe_updates=1;", itemPrice.price_master_id);
                strSql.Append("insert into item_price_ts(`item_price_id`,`item_id`,`price_master_id`,`item_money`,`item_cost`,`event_money`,`event_cost`,`apply_id`)");
                strSql.AppendFormat(" select `item_price_id`,`item_id`,`price_master_id`,{0} as `item_money`,{1} as `item_cost`,",itemPrice.item_money,itemPrice.item_cost);
                strSql.AppendFormat("{0} as `event_money`,{1} as `event_cost`,{2} as apply_id",itemPrice.event_money,itemPrice.event_cost,itemPrice.apply_id);
                strSql.AppendFormat(" from item_price where price_master_id={0};",itemPrice.price_master_id);
            }
            else
            {
                strSql.AppendFormat("set sql_safe_updates=0;delete from item_price_ts where item_price_id={0};set sql_safe_updates=1;", itemPrice.item_price_id);
                strSql.Append("insert into item_price_ts(`item_price_id`,`item_id`,`price_master_id`,`item_money`,`item_cost`,`event_money`,`event_cost`,`apply_id`)");
                strSql.AppendFormat(" select `item_price_id`,`item_id`,`price_master_id`,{0} as `item_money`,{1} as `item_cost`,",itemPrice.item_money, itemPrice.item_cost);
                strSql.AppendFormat("{0} as `event_money`,{1} as `event_cost`,{2} as apply_id", itemPrice.event_money, itemPrice.event_cost, itemPrice.apply_id);
                strSql.AppendFormat(" from item_price where item_price_id={0};", itemPrice.item_price_id);
            }
            strSql.AppendFormat(";set sql_safe_updates=1;");
            return strSql.ToString();
        }

        public string DeleteTs(Model.ItemPrice itemPrice)
        {
            //return string.Format("set sql_safe_updates=0;delete from item_price_ts where price_master_id={0};set sql_safe_updates=1;", itemPrice.price_master_id);
            return string.Format("set sql_safe_updates=0;delete from item_price_ts where price_master_id={0} and apply_id={1};set sql_safe_updates=1;",itemPrice.price_master_id,itemPrice.apply_id);
        }
    }
}
