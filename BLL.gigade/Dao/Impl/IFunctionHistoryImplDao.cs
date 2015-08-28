using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IFunctionHistoryImplDao
    {
        int Save(FunctionHistory fh);
        List<FunctionHistoryCustom> Query(int function_id, int start, int limit, string condition, DateTime timeStart, DateTime timeEnd, out int total);
    }
}
