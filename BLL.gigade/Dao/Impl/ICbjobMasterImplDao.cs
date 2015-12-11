using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface ICbjobMasterImplDao
    {
        int Insert(CbjobMaster m);
        string Insertsql(CbjobMaster m);
        List<CbjobMasterQuery> GetjobMaster(CbjobMasterQuery m, out int totalCount);
    }
}
