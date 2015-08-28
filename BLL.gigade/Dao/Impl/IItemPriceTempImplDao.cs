/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IItemPriceTempImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/3/11 11:29:33 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IItemPriceTempImplDao
    {
        List<Model.ItemPrice> itemPriceQuery(Model.ItemPrice query);
        string Save(Model.ItemPrice itemPrice);
        string Move2ItemPrice();
        string SaveFromItemPriceByMasterId();
        string Update(Model.ItemPrice itemPrice);
        string Delete(string product_Id, int combo_type, int writer_id);
        /// <summary>
        /// 刪除子商品對應的item價格數據
        /// </summary>
        /// <param name="product_id"></param>
        /// <param name="combo_type"></param>
        /// <param name="writer_id"></param>
        /// <returns></returns>
        string ChildDelete(string product_id, int combo_type, int writer_id);


        string DeleteByVendor(string product_Id, int combo_type, int writer_id);
        List<Model.Custom.ItemPriceCustom> QueryByVendor(Model.ItemPrice itemPrice);
        List<Model.Custom.ItemPriceCustom> QueryNewAdd(Model.ItemPrice itemPrice);
        string UpdateTs(Model.ItemPrice itemPrice);

        #region 與供應商商品相關
        string Vendor_Delete(string product_Id, int combo_type, int writerId);
        string VendorMove2ItemPrice();
        #endregion
    }
}
