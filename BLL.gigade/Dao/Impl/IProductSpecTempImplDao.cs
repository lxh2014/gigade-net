using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    interface IProductSpecTempImplDao
    {
        int Save(ProductSpecTemp saveTemp);
        int Delete(ProductSpecTemp delTemp);
        string Update(ProductSpecTemp pSpec, string updateType);
        List<ProductSpecTemp> Query(ProductSpecTemp queryTemp);
        string TempMoveSpec(ProductSpecTemp proSpecTemp);
        string TempDelete(ProductSpecTemp proSpecTemp);
        string SaveFromSpec(ProductSpecTemp proSpecTemp);
        string UpdateCopySpecId(ProductSpecTemp proSpecTemp);


        string TempDeleteByVendor(ProductSpecTemp proSpecTemp);
        int SaveByVendor(ProductSpecTemp saveTemp);
        int DeleteByVendor(ProductSpecTemp delTemp);
        string VendorSaveFromSpec(ProductSpecTemp proSpecTemp, string old_product_id);
        string VendorTempMoveSpec(ProductSpecTemp proSpecTemp);
        string VendorTempDelete(ProductSpecTemp proSpecTemp);
        List<ProductSpecTemp> VendorQuery(ProductSpecTemp queryTemp);
    }
}
