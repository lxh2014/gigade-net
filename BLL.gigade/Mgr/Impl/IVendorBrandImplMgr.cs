/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IVendorBrandImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:44:36 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query; 

namespace BLL.gigade.Mgr.Impl
{
    public interface IVendorBrandImplMgr
    {
        BLL.gigade.Model.VendorBrand GetProductBrand(BLL.gigade.Model.VendorBrand query);
        string QueryBrand(VendorBrand brand, int hideOffGrade = 0);
        List<VendorBrand> GetProductBrandList(VendorBrand brand);
        DataTable GetBandList(string sqlconcat);
        DataTable GetVendorBrandStory(VendorBrandQuery query, out int totalCount);
        int AddVendorBrandStory(VendorBrandQuery query);
        int GetClassify(VendorBrandQuery query);
        List<VendorBrand> GetVendorBrand(VendorBrandQuery query);
        int DelPromoPic(int brand_id,string type);
        List<VendorBrandQuery> GetBandList(VendorBrandQuery query);
    }
}
