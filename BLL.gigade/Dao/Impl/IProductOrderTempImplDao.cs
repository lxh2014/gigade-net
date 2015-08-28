using BLL.gigade.Model.Temp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductOrderTempImplDao
    {
        List<ProductOrderTemp> QuerySingle(ProductOrderTemp pot);
        List<ProductOrderTemp> QueryParent(ProductOrderTemp pot);
        List<ProductOrderTemp> QueryChild(DateTime dt, string parentIds);
    }
}
