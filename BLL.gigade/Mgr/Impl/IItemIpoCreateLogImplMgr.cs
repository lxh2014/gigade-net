using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IItemIpoCreateLogImplMgr
    {
        int AddItemIpoCreate(ItemIpoCreateLogQuery query);
    }
}
