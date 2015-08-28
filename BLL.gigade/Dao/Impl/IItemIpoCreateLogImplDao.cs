using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
     public interface IItemIpoCreateLogImplDao
    {
        int AddItemIpoCreate(ItemIpoCreateLogQuery query);
    }
}
