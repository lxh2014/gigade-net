using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IItemPriceTsImplDao
    {
        string UpdateTs(Model.ItemPrice itemPrice);
        string DeleteTs(Model.ItemPrice itemPrice);
    }
}
