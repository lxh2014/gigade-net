/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductTagSetImplDao 
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

namespace BLL.gigade.Mgr.Impl
{
   public interface IVendorProductTagSetImplMgr
    {
        List<Model.VendorProductTagSet> Query(Model.VendorProductTagSet  vendorProductTagSet);
        string Delete(Model.VendorProductTagSet vendorProductTagSet);
        string Save(Model.VendorProductTagSet vendorProductTagSet);
        string SaveFromOtherPro(Model.VendorProductTagSet vendorProductTagSet);
    }
}
