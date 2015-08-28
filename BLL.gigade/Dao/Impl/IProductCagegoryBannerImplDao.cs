#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IProductCategoryBannerImplDao.cs
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
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductCagegoryBannerImplDao
    {
        List<ProductCategoryBannerQuery> GetProCateBanList(ProductCategoryBannerQuery query, out int totalCount);
        List<ProductCategoryBannerQuery> QueryAll(ProductCategoryBannerQuery query);
        string GetProdsByCategorys(string banner_cateid);
        string Save(string[] values, ProductCategoryBannerQuery query);
        int DeleteProCateBan(int rowId);
        List<ProductCategory> GetProductCategoryInfo(string categoryIds);
        List<Model.ProductCategory> GetXGCate();
        int UpdateState(ProductCategoryBannerQuery query);
        string DeleteByBanCateId(ProductCategoryBannerQuery query);
        string GetProductByCateId(string category_ids, uint banner_cateid);
      
    }
}
