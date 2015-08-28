using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IUserRecommendIMgr
    {
        List<UserRecommendQuery> QueryAll(UserRecommendQuery store, out int totalCount);
        DataTable getUserInfo(int id);
    }
}
