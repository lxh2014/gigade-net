using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IRedirectGroupImplMgr
    {
        List<RedirectGroupQuery> QueryAll(RedirectGroup query, out int totalCount);
        int Save(RedirectGroup query);
        int Update(RedirectGroup query);
        List<Redirect> QueryRedirectAll(uint group_id);
        string GetGroupName(int group_id);
        List<RedirectClick> QueryRedirectClictAll(RedirectClickQuery rcModel);
    }
}
