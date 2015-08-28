using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface ICategoryImplDao
    {
        List<CategoryQuery> GetCategoryList(CategoryQuery store, out int totalCount);
        string GetSum(CategoryQuery store);
        List<CategoryQuery> GetCategory();
        List<CategoryQuery> GetProductCategoryList(CategoryQuery cq, out int totalCount);
        int ProductCategorySave(CategoryQuery cq);
        CategoryQuery GetProductCategoryById(CategoryQuery cq);
        int UpdateActive(CategoryQuery model);
        List<ProductCategory> GetProductCategoryCSV(CategoryQuery query);
    }
}
