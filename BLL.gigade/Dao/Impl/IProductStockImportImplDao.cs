using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IProductStockImportImplDao
    {
        //DataTable GetType(string item_id);
        int UpdateStock(ProductItem pi);
    }
}
