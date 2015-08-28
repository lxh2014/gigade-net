/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ItemPriceMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 10:58:40 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Collections;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class ItemPriceMgr : IItemPriceImplMgr
    {
        private IItemPriceImplDao _itemPriceDao;
        public ItemPriceMgr(string connectionStr)
        {
            _itemPriceDao = new ItemPriceDao(connectionStr);
        }

        public List<Model.Custom.ItemPriceCustom> Query(Model.ItemPrice itemPrice)
        {
            return _itemPriceDao.Query(itemPrice);
        }

        public List<Model.Custom.ItemPriceCustom> QueryNewAdd(Model.ItemPrice itemPrice)
        {
            return _itemPriceDao.QueryNewAdd(itemPrice);
        }

        public List<Model.ItemPrice> itemPriceQuery(Model.ItemPrice query)
        {
            return _itemPriceDao.itemPriceQuery(query);
        }

        public string Save(Model.ItemPrice itemPrice)
        {
            return _itemPriceDao.Save(itemPrice);
        }

        public string SaveNoItemId(Model.ItemPrice itemPrice)
        {
            return _itemPriceDao.SaveNoItemId(itemPrice);
        }

        public string SaveFromItem(int writeId,string productId)
        {
            return _itemPriceDao.SaveFromItem(writeId, productId);
        }

        public string Update(Model.ItemPrice itemPrice)
        {
            return _itemPriceDao.Update(itemPrice);
        }

        public string UpdateFromTs(Model.ItemPrice itemPrice)
        {
            return _itemPriceDao.UpdateFromTs(itemPrice);
        }

        public string DeleteByProductId(int product_Id)
        {
            return _itemPriceDao.DeleteByProductId(product_Id);
        }

        /// <summary>
        /// 添加規格時添加子商品價格
        /// <param name="list">存放需要執行的sql語句集合</param>
        /// <param name="connectionStr">連接字符串</param>
        /// <param name="return">返回執行后的結果</param>
        /// add by wangwei0216w
        /// </summary>
        public bool AddItemPricBySpec(ArrayList list, string connectionStr)
        {
            ItemPriceDao i = new ItemPriceDao(connectionStr);
            return i.AddItemPricBySpec(list, connectionStr);
        }

        /// <summary>
        /// 根據item_price_id獲得所屬的product_id
        /// </summary>
        /// <param name="itemPriceId">item_price_id</param>
        /// <returns>product_id</returns>
        public int GetProductId(int itemPriceId)
        {
            return _itemPriceDao.GetProductId(itemPriceId);
        }

        /// <summary>
        /// 根據item_price_id獲得item的信息
        /// </summary>
        /// <param name="itemPriceId">item_price_id</param>
        /// <returns>product_id</returns>
        public List<ItemPrice> GetItem(int productId)
        {
            return _itemPriceDao.GetItem(productId);
        }
    }
}
