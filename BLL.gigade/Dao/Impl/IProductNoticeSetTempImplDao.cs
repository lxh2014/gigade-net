/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductNoticeSetTempImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:41:48 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IProductNoticeSetTempImplDao
    {
        List<Model.ProductNoticeSetTemp> Query(Model.ProductNoticeSetTemp productNoticeSetTemp);
        string Delete(Model.ProductNoticeSetTemp productNoticeSetTemp);
        string DeleteVendor(Model.ProductNoticeSetTemp productNoticeSetTemp);
        string Save(Model.ProductNoticeSetTemp productNoticeSetTemp);
        string MoveNotice(Model.ProductNoticeSetTemp productNoticeSetTemp);
        string SaveFromProNotice(Model.ProductNoticeSetTemp proNoticeSetTemp);
        List<Model.ProductNoticeSetTemp> QueryVendorProdNotice(Model.ProductNoticeSetTemp productNoticeSetTemp);
        string Save_Vendor(Model.ProductNoticeSetTemp productNoticeSetTemp);
        string VendorSaveFromProNotice(Model.ProductNoticeSetTemp proNoticeSetTemp, string old_product_id);

        #region 與供應商商品相關
        string Vendor_MoveNotice(Model.ProductNoticeSetTemp proNoticeSetTemp);
        string Vendor_Delete(Model.ProductNoticeSetTemp productNoticeSetTemp);
        #endregion
    }
}
