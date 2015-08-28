using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IVendorProductTagSetImplDao
    {
        List<Model.VendorProductTagSet> Query(Model.VendorProductTagSet vendorProductTagSet);
        string Delete(Model.VendorProductTagSet vendorProductTagSet);
        string Save(Model.VendorProductTagSet vendorProductTagSet);
        string SaveFromOtherPro(Model.VendorProductTagSet vendorProductTagSet);
    }
}
