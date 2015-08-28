#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IProductCategoryBannerImplMgr.cs
* 摘 要：
* 專區商品類別設置
* 当前版本：v1.0
* 作 者： shuangshuang0420j
* 完成日期：2014/12/30 
* 修改歷史：
*         
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductCategoryBannerImplMgr
    {
        List<ProductCategoryBannerQuery> GetProCateBanList(ProductCategoryBannerQuery query, out int totalCount);
        List<ProductCategoryBannerQuery> QueryAll(ProductCategoryBannerQuery query);
        DataTable isSaleProd(string cateIDs, uint banner_cateid);

        bool Save(string[] values, ProductCategoryBannerQuery query);
        int DeleteProCateBan(int rowId);
        List<Model.ProductCategory> GetXGCate();
        List<ProductCategory> GetProductCategoryInfo(string categoryIds);
        int UpdateState(ProductCategoryBannerQuery query);
        DataTable GetXGCateByBanner(string category_ids, uint banner_cateid);
        bool DeleteByCateId(ProductCategoryBannerQuery query);
    }
}
