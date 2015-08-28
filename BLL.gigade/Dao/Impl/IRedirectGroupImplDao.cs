﻿using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IRedirectGroupImplDao
    {
        List<RedirectGroupQuery> QueryAll(RedirectGroup query, out int totalCount);
        string Save(RedirectGroup query);
        int Update(RedirectGroup query);
        List<Redirect> QueryRedirectAll(uint group_id);
        List<RedirectClick> QueryRedirectClictAll(RedirectClickQuery rcModel);
        string GetGroupName(int group_id);
    }
}
