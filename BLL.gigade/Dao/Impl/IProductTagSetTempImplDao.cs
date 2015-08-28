/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductTagSetTempImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:42:07 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductTagSetTempImplDao
    {
        List<Model.ProductTagSetTemp> Query(Model.ProductTagSetTemp productTagSetTemp);
        string Delete(Model.ProductTagSetTemp productTagSetTemp);
        string DeleteVendor(Model.ProductTagSetTemp productTagSetTemp);
        string Save(Model.ProductTagSetTemp productTagSetTemp);
        string MoveTag(Model.ProductTagSetTemp productTagSetTemp);
        string SaveFromTag(Model.ProductTagSetTemp proTagSetTemp);
        List<Model.ProductTagSetTemp> QueryVendorTagSet(Model.ProductTagSetTemp productTagSetTemp);
        string VendorTagSetTempSave(Model.ProductTagSetTemp productTagSetTemp);
        string VendorSaveFromTag(Model.ProductTagSetTemp proTagSetTemp, string old_product_id);

        #region 與供應商商品相關
        string Vendor_MoveTag(Model.ProductTagSetTemp proTagSetTemp);
        string Vendor_Delete(Model.ProductTagSetTemp productTagSetTemp);
        #endregion
    }
}
