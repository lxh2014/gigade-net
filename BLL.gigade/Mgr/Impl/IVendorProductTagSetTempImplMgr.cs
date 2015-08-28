/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductTagSetTempImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：shiwei0620j 
 * 完成日期：
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IVendorProductTagSetTempImplMgr
    {
        List<ProductTagSetTemp> Query(ProductTagSetTemp vendorProductTagSetTemp);
        string Delete(ProductTagSetTemp vendorProductTagSetTemp);
        string DeleteVendor(ProductTagSetTemp vendorProductTagSetTemp);
        string Save(ProductTagSetTemp vendorProductTagSetTemp);
        string MoveTag(ProductTagSetTemp vendorProductTagSetTemp);
        string SaveFromTag(ProductTagSetTemp vendorProductTagSetTemp);
    }
}
