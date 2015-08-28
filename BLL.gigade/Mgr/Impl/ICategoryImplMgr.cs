using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICategoryImplMgr
    {
        List<CategoryQuery> GetCategoryList(CategoryQuery query, out int totalCount);
        string GetSum(CategoryQuery query);
        List<CategoryQuery> GetCategory();
        List<CategoryQuery> GetProductCategoryList(CategoryQuery cq, out int totalCount);
        int ProductCategorySave(CategoryQuery cq);
        CategoryQuery GetProductCategoryById(CategoryQuery cq);
        int UpdateActive(CategoryQuery model);
        List< Model.ProductCategory> GetProductCategoryCSV(CategoryQuery query);
    }
}
