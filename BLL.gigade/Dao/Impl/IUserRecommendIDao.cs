using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IUserRecommendIDao
    {
        List<UserRecommendQuery> QueryAll(UserRecommendQuery query, out int totalCount);
        List<UserRecommend> QueryByOrderId(uint order_id);
        string UpdateIsCommend(string idStr);
        DataTable getUserInfo(int id);
    }
}
