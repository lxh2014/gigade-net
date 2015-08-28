using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface INewPromoPresentImplDao
    {
        DataTable GetNewPromoPresentList(NewPromoPresentQuery query, out int totalCount);
        string InsertNewPromoPresent(NewPromoPresentQuery store);
        string UpdateNewPromoPresent(NewPromoPresentQuery store);
        string UpdateActive(int status, string event_id);
        int DeleteNewPromoPresent(int row_id);
        string GetProductnameById(int id);
        string UpdateActive(NewPromoPresentQuery newPresent);
        string DeleteNewPromoPresent(NewPromoPresentQuery newPresent);
        int GetNewPromoPresent(string event_id);
        int GetNewPromoPresentMaxId();
    }
}
