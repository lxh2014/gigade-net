using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
namespace BLL.gigade.Mgr.Impl
{
    public interface IProductCategoryImplMgr
    {
        /// <summary>
        /// 前台分類查詢
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<Model.ProductCategory> QueryAll(Model.ProductCategory query);
        List<Model.ProductCategory> GetProductCate(Model.ProductCategory query);
        /// <summary>
        /// 品類分類查詢
        /// </summary>
        /// <param name="fatherId"></param>
        /// <returns></returns>
        List<ProductCategoryCustom> cateQuery(int fatherId);


        List<ProductCategoryCustom> Query(int fatherId, int status = 1);
        uint GetCateID(string eventId);
        int Save(Model.ProductCategory model);
        int Update(Model.ProductCategory model);
        int Delete(Model.ProductCategory model);
        ProductCategory GetModelById(uint id);

        string SaveCategory(Model.ProductCategory model);
        List<ProdPromoQuery> GetList(ProdPromo store, out int totalCount);
        int UpStatus(ProdPromoQuery store);
        int UpdateUrl(ProdPromo store);
        DataTable GetProductCategoryStore();
    }
}
