using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface INewPromoCarnetImplDao
    {
        DataTable NewPromoCarnetList(NewPromoCarnetQuery query, out int totalCount);
        string InsertNewPromoCarnet(NewPromoCarnetQuery query);
        string UpdateNewPromoCarnet(NewPromoCarnetQuery query);
        string GetModel(NewPromoCarnetQuery query);
        string UpdateActive(NewPromoCarnetQuery query);
        int DeleteNewPromoCarnet(string row_ids);
        int GetNewPromoCarnetMaxId();
    }
}
