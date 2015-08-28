using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    interface IPageErrorLogMgr
    {
        string  QueryPara(string strParaType);
        List<PageErrorLogQuery> GetData(PageErrorLogQuery query, out int totalCount);
    }
}
