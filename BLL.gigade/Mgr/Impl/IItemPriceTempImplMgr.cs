/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IItemPriceTempImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/3/11 11:47:13 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BLL.gigade.Mgr.Impl
{
    public interface IItemPriceTempImplMgr
    {
        List<Model.ItemPrice> itemPriceQuery(Model.ItemPrice query);
        string Save(Model.ItemPrice itemPrice);
        string Move2ItemPrice();
        string SaveFromItemPriceByMasterId();
        string Update(Model.ItemPrice itemPrice);
        string Delete(string product_Id, int combo_type, int writer_id);

        string DeleteByVendor(string product_Id, int combo_type, int writer_id);
        List<Model.Custom.ItemPriceCustom> QueryByVendor(Model.ItemPrice itemPrice);
        List<Model.Custom.ItemPriceCustom> QueryNewAdd(Model.ItemPrice itemPrice);
        string UpdateTs(Model.ItemPrice itemPrice);
        bool UpdateByVendor(ArrayList exculsql);
        string VendorMove2ItemPrice();
        string Vendor_Delete(string product_Id, int combo_type, int writerId);
    }
}
