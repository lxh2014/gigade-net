/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ItemPriceDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 10:58:14 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Collections;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class ItemPriceDao : IItemPriceImplDao
    {
        private IDBAccess _dbAccess;
        public ItemPriceDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public List<Model.Custom.ItemPriceCustom> Query(Model.ItemPrice itemPrice)
        {

            try
            {
                StringBuilder strSql = new StringBuilder("select item_price.item_price_id,item_price.item_id,item_price.price_master_id,item_price.item_money,");
                strSql.Append("item_price.item_cost,item_price.event_money,item_price.event_cost,b.spec_name as spec_name_1,c.spec_name as spec_name_2");
                strSql.Append(" from item_price right join product_item a on a.item_id=item_price.item_id left join product_spec b on a.spec_id_1=b.spec_id ");
                strSql.Append(" left join product_spec c on a.spec_id_2=c.spec_id where 1=1");
                if (itemPrice.item_price_id != 0)
                {
                    strSql.AppendFormat(" and item_price.item_price_id={0}", itemPrice.item_price_id);
                }
                if (itemPrice.item_id != 0)
                {
                    strSql.AppendFormat(" and item_price.item_id={0}", itemPrice.item_id);
                }
                if (itemPrice.price_master_id != 0)
                {
                    strSql.AppendFormat(" and item_price.price_master_id={0}", itemPrice.price_master_id);
                }
                if (itemPrice.IsPage)
                {
                    strSql.AppendFormat(" limit {0},{1}", itemPrice.Start, itemPrice.Limit);
                }
                return _dbAccess.getDataTableForObj<Model.Custom.ItemPriceCustom>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceDao.Query-->" + ex.Message, ex);
            }
        }

        public List<Model.Custom.ItemPriceCustom> QueryNewAdd(Model.ItemPrice itemPrice)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select item_id,");
                strSql.Append("b.spec_name as spec_name_1,c.spec_name as spec_name_2 ");
                strSql.Append("from product_item a left join product_spec b on a.spec_id_1=b.spec_id ");
                strSql.Append("left join product_spec c on a.spec_id_2=c.spec_id ");
                strSql.AppendFormat("where a.product_id=(select product_id from price_master where price_master_id={0}) ", itemPrice.price_master_id);
                strSql.AppendFormat("and a.item_id not in (select item_id from item_price where price_master_id={0})", itemPrice.price_master_id);
                return _dbAccess.getDataTableForObj<Model.Custom.ItemPriceCustom>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceDao.QueryNewAdd-->" + ex.Message, ex);
            }
        }

        public List<Model.ItemPrice> itemPriceQuery(Model.ItemPrice query)
        {
            StringBuilder sql = new StringBuilder("select item_price_id,item_id,price_master_id,item_money,item_cost,event_money,event_cost from item_price where 1 = 1");
            if (query.price_master_id != 0)
            {
                sql.AppendFormat(" and price_master_id = {0}", query.price_master_id);
            }
            return _dbAccess.getDataTableForObj<Model.ItemPrice>(sql.ToString());
        }

        public string Save(Model.ItemPrice itemPrice)
        {
            StringBuilder strSql = new StringBuilder();
            if (itemPrice.price_master_id != 0)
            {
                strSql.AppendFormat("insert into item_price(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`)values({0},", itemPrice.price_master_id);
                strSql.AppendFormat("{0},{1},{2},", itemPrice.item_id, itemPrice.item_money, itemPrice.item_cost);
                strSql.AppendFormat("{0},{1});", itemPrice.event_money, itemPrice.event_cost);
            }
            else
            {
                strSql.Append("insert into item_price(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`)values({0},");
                strSql.AppendFormat("{0},{1},{2},", itemPrice.item_id, itemPrice.item_money, itemPrice.item_cost);
                strSql.AppendFormat("{0},{1});", itemPrice.event_money, itemPrice.event_cost);
            }

            return strSql.ToString();
        }

        public string SaveNoItemId(Model.ItemPrice itemPrice)
        {
            StringBuilder strSql = new StringBuilder("insert into item_price(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`)values({0},{1},");
            strSql.AppendFormat("{0},{1},", itemPrice.item_money, itemPrice.item_cost);
            strSql.AppendFormat("{0},{1})", itemPrice.event_money, itemPrice.event_cost);
            return strSql.ToString();
        }

        public string SaveFromItem(int writeId,string product_id)
        {
            //StringBuilder strSql = new StringBuilder("insert into item_price(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`) select {0} as price_master_id,");
            //strSql.AppendFormat("item_id,item_money,item_cost,event_item_money as event_money,event_item_cost as event_cost from product_item_temp where product_id='{0}'", product_id);
            //strSql.AppendFormat(" and writer_id={0} ",writeId);
            //return strSql.ToString();
            StringBuilder strSql = new StringBuilder("insert into item_price(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`)"); 
            strSql.Append("select {0} as price_master_id,{1} as item_id,");
            strSql.AppendFormat("item_money,item_cost,event_item_money as event_money,event_item_cost as event_cost from product_item_temp where product_id='{0}'", product_id);
            strSql.AppendFormat(" and writer_id={0} ", writeId);
            strSql.Append(" and item_id={2}");
            return strSql.ToString();
        }

        public string Update(Model.ItemPrice itemPrice)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;update item_price set ");
            if (itemPrice.item_price_id == 0)
            {
                strSql.AppendFormat(" price_master_id={0},item_money={1},", itemPrice.price_master_id, itemPrice.item_money);
                strSql.AppendFormat(" item_cost={0},event_money={1},event_cost={2}", itemPrice.item_cost, itemPrice.event_money, itemPrice.event_cost);
                strSql.AppendFormat(" where price_master_id={0}", itemPrice.price_master_id);
            }
            else
            {
                strSql.AppendFormat(" item_id={0},price_master_id={1},item_money={2},", itemPrice.item_id, itemPrice.price_master_id, itemPrice.item_money);
                strSql.AppendFormat(" item_cost={0},event_money={1},event_cost={2}", itemPrice.item_cost, itemPrice.event_money, itemPrice.event_cost);
                strSql.AppendFormat(" where item_price_id={0}", itemPrice.item_price_id);
            }
            strSql.AppendFormat(";set sql_safe_updates=1;");
            return strSql.ToString();
        }

        //從item_price_ts更新item_price add by xiangwang0413w 2014/07/21
        public string UpdateFromTs(Model.ItemPrice itemPrice)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;");
            if (itemPrice.item_price_id == 0)
            {
                strSql.Append("update item_price a inner join item_price_ts b on a.item_price_id= b.item_price_id ");
                strSql.Append(" set a.item_id=b.item_id,a.item_money=b.item_money,");
                strSql.Append("a.item_cost=b.item_cost,a.event_money=b.event_money,a.event_cost=b.event_cost");
                strSql.AppendFormat(" where a.price_master_id={0} and apply_id={1};", itemPrice.price_master_id, itemPrice.apply_id);
            }
            else
            {
                strSql.Append("update item_price a inner join item_price_ts b on a.item_price_id= b.item_price_id ");
                strSql.Append(" set a.item_id=b.item_id,a.item_price_id=b.item_price_id,a.item_money=b.item_money,");
                strSql.Append("a.item_cost=b.item_cost,a.event_money=b.event_money,a.event_cost=b.event_cost");
                strSql.AppendFormat(" where a.item_price_id={0} and apply_id={1};", itemPrice.item_price_id, itemPrice.apply_id);
            }
            strSql.Append("set sql_safe_updates=1;");
            return strSql.ToString();
        }

        public string DeleteByProductId(int product_Id)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete item_price from price_master");
            strSql.AppendFormat(" left join item_price on item_price.price_master_id=price_master.price_master_id where price_master.product_id={0};", product_Id);
            strSql.Append("set sql_safe_updates=1;");
            return strSql.ToString();
        }

        /// <summary>
        /// 添加規格時添加子商品價格
        /// <param name="list">存放需要執行的sql語句集合</param>
        /// <param name="connectionStr">連接字符串</param>
        /// <param name="return">返回執行后的結果</param>
        /// add by wangwei 0216w 2014/9/22
        /// </summary>
        public bool AddItemPricBySpec(ArrayList list, string connectionStr)
        {
            MySqlDao msd = new MySqlDao(connectionStr);
            return msd.ExcuteSqls(list);            
        }

        /// <summary>
        /// 根據item_price_id獲得所屬的product_id
        /// </summary>
        /// <param name="itemPriceId">item_price_id</param>
        /// <returns>product_id</returns>
        public int GetProductId(int itemPriceId)
        {
            //add by wwei0216w 2015/1/30
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT pm.product_id FROM item_price ip
                                    INNER JOIN price_master pm ON pm.price_master_id =ip.price_master_id
                                WHERE item_price_id = {0}",itemPriceId);
                return Convert.ToInt32(_dbAccess.getSinggleObj<PriceMaster>(sb.ToString()).product_id);
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceDao-->GetProductId"+ex.Message,ex);
            }
        }

        /// <summary>
        /// 根據item_price_id獲得item的信息
        /// </summary>
        /// <param name="itemPriceId">item_price_id</param>
        /// <returns>product_id</returns>
        public List<ItemPrice>  GetItem(int productId)
        {
            //add by wwei0216w 2015/3/4
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT ip.item_price_id,ip.item_id,ip.price_master_id,ip.item_money,ip.item_cost,ip.event_money FROM item_price ip
                                    INNER JOIN price_master pm ON pm.price_master_id =ip.price_master_id
                                WHERE pm.product_id = {0}", productId);
                return _dbAccess.getDataTableForObj<ItemPrice>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceDao-->GetItem" + ex.Message, ex);
            }
        }
    }
}
