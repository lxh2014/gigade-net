using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface INewPromoPresentImplMgr
    {
        DataTable NewPromoPresentList(NewPromoPresentQuery query, out int totalCount);
        int InsertNewPromoPresent(NewPromoPresentQuery newPresent);
        int UpdateNewPromoPresent(NewPromoPresentQuery newPresent);
        string GetProductnameById(int id);
      
        int UpdateActive(NewPromoPresentQuery newPresent);
        int DeleteNewPromoPresent(NewPromoPresentQuery newPresent);
        int GetNewPromoPresent(string event_id);
        int GetNewPromoPresentMaxId();
    }
}
