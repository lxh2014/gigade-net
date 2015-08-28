using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductSpecTempImplMgr
    {
        bool Save(List<ProductSpecTemp> saveTempList);
        List<ProductSpecTemp> Query(ProductSpecTemp queryTemp);
        bool Delete(ProductSpecTemp delTemp);
        bool Update(List<ProductSpecTemp> pSpecList, string updateType);
        string QuerySpecOne(ProductSpecTemp psTemp);
        string TempMoveSpec(ProductSpecTemp proSpecTemp);
        string TempDelete(ProductSpecTemp proSpecTemp);
        string SaveFromSpec(ProductSpecTemp proSpecTemp);
        string UpdateCopySpecId(ProductSpecTemp proSpecTemp);
        string TempDeleteByVendor(ProductSpecTemp proSpecTemp);
        bool SaveByVendor(List<ProductSpecTemp> saveTempList);
        bool DeleteByVendor(ProductSpecTemp delTemp);
        string VendorSaveFromSpec(ProductSpecTemp proSpecTemp, string old_id);
        string VendorTempMoveSpec(ProductSpecTemp proSpecTemp);
        string VendorTempDelete(ProductSpecTemp proSpecTemp);
        List<ProductSpecTemp> VendorQuery(ProductSpecTemp queryTemp);
    }
}
