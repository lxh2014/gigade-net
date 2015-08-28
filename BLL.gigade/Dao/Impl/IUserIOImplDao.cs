using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    interface IUserIOImplDao
    {
        DataTable GetExcelTable(string sql);
    }
}
