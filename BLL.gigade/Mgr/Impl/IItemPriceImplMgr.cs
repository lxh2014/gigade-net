﻿/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IItemPriceImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 10:58:29 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IItemPriceImplMgr
    {
        List<Model.Custom.ItemPriceCustom> Query(Model.ItemPrice itemPrice);
        List<Model.Custom.ItemPriceCustom> QueryNewAdd(Model.ItemPrice itemPrice);
        List<Model.ItemPrice> itemPriceQuery(Model.ItemPrice query);
        string Save(Model.ItemPrice itemPrice);
        string SaveNoItemId(Model.ItemPrice itemPrice);
        string SaveFromItem(int writeId,string productId);
        string Update(Model.ItemPrice itemPrice);
        string UpdateFromTs(Model.ItemPrice itemPrice);
        string DeleteByProductId(int product_Id);
        bool AddItemPricBySpec(ArrayList list, string connectionStr); //add by wangwei0216w 2014/9/22
        /// <summary>
        /// 根據item_price_id獲得所屬的product_id
        /// </summary>
        /// <param name="itemPriceId">item_price_id</param>
        /// <returns>product_id</returns>
        int GetProductId(int itemPriceId);

        /// <summary>
        /// 根據item_price_id獲得item的信息
        /// </summary>
        /// <param name="itemPriceId">item_price_id</param>
        /// <returns>product_id</returns>
        List<ItemPrice> GetItem(int productId);
    }
}
