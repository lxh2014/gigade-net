using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductCategorySetImplMgr
    {
        List<ProductCategorySet> Query(ProductCategorySet queryModel);
        List<ProductCategorySet> QueryMsg(ProductCategorySetQuery queryModel);
        List<ProductCategorySetCustom> Query(ProductCategorySetCustom query);
        string Save(ProductCategorySet save);
        string SaveFromOtherPro(ProductCategorySet saveModel);
        string Delete(ProductCategorySet delModel, string deStr = "0");
        /// <summary>
        /// 更新 Product CategorySet
        /// </summary>
        /// <param name="cateList"></param>
        /// <param name="pro"></param>
        /// <returns></returns>
        bool ProductCateUpdate(List<ProductCategorySet> cateList, Product pro);

        string SaveNoPrid(ProductCategorySet save);
        bool DeleteProductByModelArry(string bids, string cids, string pids);
        DataTable QueryBrand(string webtype, int content_id);
        DataTable QueryProduct(string category_id);

        //add by wwei0216w 2015/2/24
        /// <summary>
        /// 根據商品id修改品牌id
        /// </summary>
        /// <param name="pcs">一個ProductCategorySet對象</param>
        /// <returns>將要執行的sql語句</returns>
        string UpdateBrandId(ProductCategorySet pcs);
        DataTable GetCateByProds(string prods, string cateids);
    }
}
