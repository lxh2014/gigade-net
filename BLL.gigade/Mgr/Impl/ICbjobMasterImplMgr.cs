﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface ICbjobMasterImplMgr
    {
        int Insert(CbjobMaster m);
        string Insertsql(CbjobMaster m);
        List<CbjobMasterQuery> GetjobMaster(CbjobMasterQuery m, out int totalCount);
    }
}
