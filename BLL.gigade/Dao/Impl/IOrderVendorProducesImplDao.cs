using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IOrderVendorProducesImplDao
    {
        List<OrderVendorProducesQuery> GetOrderVendorProduces(OrderVendorProducesQuery store, out int totalCount);
        DataTable ExportCsv(string sqlwhere);
        DataTable GetProductItem();
    }
}
