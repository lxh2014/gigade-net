using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPayEasyImplMgr
    {
        List<PayEasyQuery> Query(PayEasyQuery query);
        DataTable QueryExcel(PayEasyQuery query);
    }
}
