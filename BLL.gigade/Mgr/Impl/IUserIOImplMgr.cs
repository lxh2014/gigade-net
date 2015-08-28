using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IUserIOImplMgr
    {
        DataTable GetExcelTable(string sql);
    }
}
