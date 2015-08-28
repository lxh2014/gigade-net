using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IItemPriceTsImplMgr
    {
        string UpdateTs(Model.ItemPrice itemPrice);
        string DeleteTs(Model.ItemPrice itemPrice);
    }
}
