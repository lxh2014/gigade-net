/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IVendorBrandImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:40:07 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    interface IVendorBrandImplDao
    {
        BLL.gigade.Model.VendorBrand GetProductBrand(BLL.gigade.Model.VendorBrand query);
        List<BLL.gigade.Model.VendorBrand> GetProductBrandList(VendorBrand brand, int hideOffGrade = 0);
        List<BLL.gigade.Model.VendorBrand> GetClassBrandList(BLL.gigade.Model.VendorBrand brand, uint cid, int hideOffGrade = 0);
        DataTable GetBandList(string sqlconcat);
        DataTable GetVendorBrandStory(VendorBrandQuery query, out int totalCount);
        int AddVendorBrandStory(VendorBrandQuery query);
        int GetClassify(VendorBrandQuery query);
        List<VendorBrand> GetVendorBrand(VendorBrandQuery query);
        int DelPromoPic(int brand_id);
        List<VendorBrandQuery> GetBandList(VendorBrandQuery query);
    }
}
