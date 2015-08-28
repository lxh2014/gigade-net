/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ItemPriceTempDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/3/11 11:29:59 
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
    class ItemPriceTempDao : IItemPriceTempImplDao
    {
        private IDBAccess _dbAccess;
        public ItemPriceTempDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public List<Model.ItemPrice> itemPriceQuery(Model.ItemPrice query)
        {
            try
            {
                StringBuilder sql = new StringBuilder("select item_price_id,item_id,price_master_id,item_money,item_cost,event_money,event_cost from item_price_temp where 1 = 1");
                if (query.price_master_id != 0)
                {
                    sql.AppendFormat(" and price_master_id = {0}", query.price_master_id);
                }
                return _dbAccess.getDataTableForObj<Model.ItemPrice>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.itemPriceQuery-->" + ex.Message, ex);
            }
        }

        public string Save(Model.ItemPrice itemPrice)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("insert into item_price_temp(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`)values({0},");
                strSql.AppendFormat("{0},{1},{2},", itemPrice.item_id, itemPrice.item_money, itemPrice.item_cost);
                strSql.AppendFormat("{0},{1})", itemPrice.event_money, itemPrice.event_cost);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.Save-->" + ex.Message, ex);
            }
        }

        public string Move2ItemPrice()
        {
            try
            {
                StringBuilder strSql = new StringBuilder("insert into item_price(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`) select {0} as price_master_id,");
                //strSql.Append("item_id,item_money,item_cost,event_money,event_cost from item_price_temp where price_master_id={1}");
                strSql.Append("item_id,item_money,item_cost,event_money,event_cost from item_price_temp where price_master_id={0}");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.Move2ItemPrice-->" + ex.Message, ex);
            }
        }

        public string SaveFromItemPriceByMasterId()
        {
            try
            {
                StringBuilder strSql = new StringBuilder("insert into item_price_temp(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`) select {0} as price_master_id,");
                strSql.Append("item_id,item_money,item_cost,event_money,event_cost from item_price_temp where price_master_id={1}");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.SaveFromItemPriceByMasterId-->" + ex.Message, ex);
            }
        }

        public string Update(Model.ItemPrice itemPrice)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;update item_price_temp set ");
                if (itemPrice.item_price_id == 0)
                {
                    strSql.AppendFormat(" item_money={0},item_cost={1},", itemPrice.item_money, itemPrice.item_cost);
                    strSql.AppendFormat(" event_money={0},event_cost={1}", itemPrice.event_money, itemPrice.event_cost);
                    strSql.AppendFormat(" where price_master_id={0}", itemPrice.price_master_id);
                }
                else
                {
                    strSql.AppendFormat(" item_id={0},price_master_id={1},item_money={2},", itemPrice.item_id, itemPrice.price_master_id, itemPrice.item_money);
                    strSql.AppendFormat(" item_cost={0},event_money={1},event_cost={2}", itemPrice.item_cost, itemPrice.event_money, itemPrice.event_cost);
                    strSql.AppendFormat(" where item_price_id={0}", itemPrice.item_price_id);
                }
                strSql.Append(";set sql_safe_updates=1;");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.Update-->" + ex.Message, ex);
            }
        }

        public string Delete(string product_Id, int combo_type, int writer_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete item_price_temp from price_master_temp");
                strSql.Append(" left join item_price_temp on item_price_temp.price_master_id=price_master_temp.price_master_id ");
                strSql.AppendFormat("where price_master_temp.product_id='{0}' and price_master_temp.combo_type={1} and price_master_temp.writer_id = {2};set sql_safe_updates=1;", product_Id, combo_type, writer_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.Delete-->" + ex.Message, ex);
            }
        }

        public string ChildDelete(string product_id,int combo_type,int writer_id)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("set sql_safe_udpates = 0;delete from item_price_temp from price_master_temp");
                strSql.Append(" left join price_master_temp on item_price_temp.price_master_id = price_master_temp.price_master_id");
                strSql.AppendFormat(" where price_master_temp.product_id = '{0}' and price_master_temp.combo_type = {1} and price_master_temp.writer_id = {2} and price_master_temp.child_id <> 0;set sql_safe_updates = 1", product_id, combo_type, writer_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.ChildDelete-->" + ex.Message, ex);
            }
        }

        #region 刪除供應商新增的臨時數據
        public string DeleteByVendor(string product_Id, int combo_type, int writer_id)
        {
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append("set sql_safe_updates=0;delete item_price_temp from price_master_temp");
                strSql.Append(" left join item_price_temp on item_price_temp.price_master_id=price_master_temp.price_master_id ");
                strSql.AppendFormat("where price_master_temp.product_id='{0}' and price_master_temp.combo_type={1} and price_master_temp.writer_id = {2};set sql_safe_updates=1;", product_Id, combo_type, writer_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.DeleteByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        #endregion

        #region 查詢供應商新增商品的itemprice信息+List<Model.Custom.ItemPriceCustom> QueryByVendor(Model.ItemPrice itemPrice)
        public List<Model.Custom.ItemPriceCustom> QueryByVendor(Model.ItemPrice itemPrice)
        {
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append("select i.item_price_id,i.item_id,i.price_master_id,i.item_money,");
                strSql.Append("i.item_cost,i.event_money,i.event_cost,b.spec_name as spec_name_1,c.spec_name as spec_name_2");
                strSql.Append(" from item_price_temp i  right join product_item_temp a on a.item_id=i.item_id left join product_spec_temp b on a.spec_id_1=b.spec_id ");
                strSql.Append(" left join product_spec_temp c on a.spec_id_2=c.spec_id where 1=1");
                if (itemPrice.item_price_id != 0)
                {

                    strSql.AppendFormat(" and i.item_price_id={0}", itemPrice.item_price_id);
                }
                if (itemPrice.item_id != 0)
                {
                    strSql.AppendFormat(" and i.item_id={0}", itemPrice.item_id);
                }
                if (itemPrice.price_master_id != 0)
                {
                    strSql.AppendFormat(" and i.price_master_id={0}", itemPrice.price_master_id);
                }
                if (itemPrice.IsPage)
                {
                    strSql.AppendFormat(" limit {0},{1}", itemPrice.Start, itemPrice.Limit);
                }
                return _dbAccess.getDataTableForObj<Model.Custom.ItemPriceCustom>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.QueryByVendor-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        #endregion

        public List<Model.Custom.ItemPriceCustom> QueryNewAdd(Model.ItemPrice itemPrice)
        {
            StringBuilder strSql = new StringBuilder("");
            try
            {
                strSql.Append("select item_id,item_money,item_cost,event_item_cost as event_cost,event_item_money as event_money,");
                strSql.Append("b.spec_name as spec_name_1,c.spec_name as spec_name_2 ");
                strSql.Append("from product_item_temp a left join product_spec_temp b on a.spec_id_1=b.spec_id ");
                strSql.Append("left join product_spec_temp c on a.spec_id_2=c.spec_id ");
                strSql.AppendFormat("where a.product_id=(select product_id from price_master_temp where price_master_id={0}) ", itemPrice.price_master_id);
                strSql.AppendFormat("and a.item_id not in (select item_id from item_price_temp where price_master_id={0})", itemPrice.price_master_id);
                return _dbAccess.getDataTableForObj<Model.Custom.ItemPriceCustom>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.QueryNewAdd-->" + ex.Message + strSql.ToString(), ex);
            }
        }


        public string UpdateTs(Model.ItemPrice itemPrice)
        {
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0; ");
            if (itemPrice.item_price_id == 0)
            {

                strSql.AppendFormat("update item_price_temp set price_master_id={0},item_money={1},", itemPrice.price_master_id, itemPrice.item_money);
                strSql.AppendFormat(" item_cost={0},event_money={1},event_cost={2}", itemPrice.item_cost, itemPrice.event_money, itemPrice.event_cost);
                strSql.AppendFormat(" where price_master_id={0}", itemPrice.price_master_id);
            }
            else
            {

                strSql.AppendFormat("update item_price_temp set item_id={0},price_master_id={1},item_money={2},", itemPrice.item_id, itemPrice.price_master_id, itemPrice.item_money);
                strSql.AppendFormat(" item_cost={0},event_money={1},event_cost={2}", itemPrice.item_cost, itemPrice.event_money, itemPrice.event_cost);
                strSql.AppendFormat(" where item_price_id={0}", itemPrice.item_price_id);
            }
            strSql.AppendFormat(";set sql_safe_updates=1;");
            return strSql.ToString();
        }

        #region 與供應商商品相關
        /// <summary>
        /// 管理員核可供應商建立的商品時將商品價格信息由臨時表移動到正式表
        /// </summary>
        /// <param name="product_Id">商品編號</param>
        /// <param name="combo_type">商品類型</param>
        /// <param name="writerId">操作人</param>
        /// <returns>此操作的sql語句</returns>
        public string Vendor_Delete(string product_Id, int combo_type, int writerId)
        {//edit jialei 20140917 加判斷writerId
            StringBuilder strSql = new StringBuilder("set sql_safe_updates=0;delete item_price_temp from price_master_temp");
            strSql.Append(" left join item_price_temp on item_price_temp.price_master_id=price_master_temp.price_master_id where 1=1");
            if (writerId!=0)
            {
                strSql.AppendFormat(" and price_master_temp.writer_id = {0}",writerId);

            }
            strSql.AppendFormat(" and price_master_temp.product_id='{0}' and price_master_temp.combo_type={1} ;set sql_safe_updates=1;", product_Id, combo_type);
            return strSql.ToString();
        }
        public string VendorMove2ItemPrice()
        {
            try
            {
                StringBuilder strSql = new StringBuilder("insert into item_price(`price_master_id`,`item_id`,`item_money`,`item_cost`,`event_money`,`event_cost`) ");
                strSql.Append("select {0} as price_master_id,item_id,item_money,item_cost,event_money,event_cost from item_price_temp where price_master_id={0}");
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("ItemPriceTempDao.VendorMove2ItemPrice-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
