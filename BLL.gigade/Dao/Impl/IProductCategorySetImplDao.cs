using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductCategorySetImplDao
    {
        List<ProductCategorySet> Query(ProductCategorySet queryModel);
        List<ProductCategorySet> QueryMsg(ProductCategorySetQuery queryModel);
        List<ProductCategorySetCustom> Query(ProductCategorySetCustom query);
        string Save(ProductCategorySet saveModel);
        int Insert(ProductCategorySet saveModel);
        string SaveFromOtherPro(ProductCategorySet saveModel);
        string Delete(ProductCategorySet delModel, string deStr = "0");
        int DeleteProductByModel(ProductCategorySet delModel);
        string DeleteProductByModelStr(ProductCategorySet delModel);
        string SaveNoPrid(ProductCategorySet save);

        DataTable QueryBrand(string webtype, int content_id);
        DataTable QueryProduct(string category_id);
        DataTable GetCateByProds(string prods, string cateids);

        //add by wwei0216w 2015/2/24
        /// <summary>
        /// 根據商品id修改品牌id
        /// </summary>
        /// <param name="pcs">一個ProductCategorySet對象</param>
        /// <returns></returns>
        string UpdateBrandId(ProductCategorySet pcs);
    }
}
