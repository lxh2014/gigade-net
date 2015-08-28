using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IGroupAuthMapImplMgr
    {
        string Query(GroupAuthMapQuery query);
        List<GroupAuthMapQuery> QueryAll(GroupAuthMapQuery m, out int totalCount);
        int AddGroupAuthMapQuery(GroupAuthMapQuery query);
        int UpGroupAuthMapQuery(GroupAuthMapQuery query);
        int UpStatus(int content_id, int status);
    }
}
