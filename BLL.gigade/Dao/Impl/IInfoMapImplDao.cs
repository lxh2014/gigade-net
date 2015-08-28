using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao.Impl
{
    public interface IInfoMapImplDao
    {
        List<InfoMapQuery> GetInfoMapList(InfoMapQuery query, out int totalCount);
        int SaveInfoMap(InfoMapQuery query);
        int UpdateInfoMap(InfoMapQuery query);
        bool SelectInfoMap(InfoMapQuery query);
        InfoMapQuery GetOldModel(InfoMapQuery query);
    }
}
