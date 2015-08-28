using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductCategorySetTempImplMgr
    {
        List<ProductCategorySetTemp> Query(ProductCategorySetTemp queryTemp);
        bool Save(List<ProductCategorySetTemp> pcTempList);
        bool Delete(ProductCategorySetTemp delTemp, string deStr = "0");
        string TempMoveCategory(ProductCategorySetTemp proCategorySetTemp);
        string TempDelete(ProductCategorySetTemp proCategorySetTemp);
        string SaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp);



        string TempDeleteByVendor(ProductCategorySetTemp proCategorySetTemp);
        bool DeleteByVendor(ProductCategorySetTemp delTemp);
        bool SaveByVendor(List<ProductCategorySetTemp> saveTempList);
        List<ProductCategorySetTemp> QueryByVendor(ProductCategorySetTemp queryTemp);
        string VendorSaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp, string old_product_Id);
    }
}
