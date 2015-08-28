using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    interface IProductSpecImplDao
    {
        List<ProductSpec> query(int product_id, string type);
        ProductSpec query(int spec_id);
        List<ProductSpec> Query(ProductSpec query);
        string Update(ProductSpec updateModel);
        int UpdateSingle(ProductSpec uSpec);
        string Delete(uint product_Id);
        string SaveFromSpec(ProductSpec proSpec);
        string UpdateCopySpecId(ProductSpec proSpec);
        int Save(ProductSpec saveTemp);
        int Updspecstatus(ProductStockQuery m);
    }
}
