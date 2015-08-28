using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface INewPromoCarnetImplMgr
    {
        DataTable NewPromoCarnetList(NewPromoCarnetQuery query, out int totalCount);
        int InsertNewPromoCarnet(NewPromoCarnetQuery query);
        bool UpdateNewPromoCarnet(NewPromoCarnetQuery query);
        NewPromoCarnetQuery GetModel(NewPromoCarnetQuery query);
        bool UpdateActive(NewPromoCarnetQuery query);
        int DeleteNewPromoCarnet(string row_ids);
        int GetNewPromoCarnetMaxId();
    }
}
