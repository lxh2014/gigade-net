using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
    public interface IProductCategorySetTempImplDao
    {
        List<ProductCategorySetTemp> Query(ProductCategorySetTemp queryTemp);
        int Save(ProductCategorySetTemp saveTemp);
        int Delete(ProductCategorySetTemp delTemp, string deStr = "0");
        string TempMoveCategory(ProductCategorySetTemp proCategorySetTemp);
        string TempDelete(ProductCategorySetTemp proCategorySetTemp);
        string SaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp);


        string TempDeleteByVendor(ProductCategorySetTemp proCategorySetTemp);
        int DeleteByVendor(ProductCategorySetTemp delTemp);
        int SaveByVendor(ProductCategorySetTemp saveTemp);
        List<ProductCategorySetTemp> QueryByVendor(ProductCategorySetTemp queryTemp);
        string VendorSaveFromCategorySet(ProductCategorySetTemp proCategorySetTemp, string old_product_Id);

        #region 與供應商商品相關
        string Vendor_TempMoveCategory(ProductCategorySetTemp proCategorySetTemp);
        string Vendor_TempDelete(ProductCategorySetTemp proCategorySetTemp); 
        #endregion
    }
}
