using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IInfoMapImplMgr
    {
        List<InfoMapQuery> GetInfoMapList(InfoMapQuery query, out int totalCount);
        int SaveInfoMap(InfoMapQuery query);
        int UpdateInfoMap(InfoMapQuery query);
        bool SelectInfoMap(InfoMapQuery query);
        InfoMapQuery GetOldModel(InfoMapQuery query);
    }
}
