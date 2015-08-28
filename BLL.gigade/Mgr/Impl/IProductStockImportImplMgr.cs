using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductStockImportImplMgr
    {
        int UpdateStock(ProductItem pi);
    }
}
