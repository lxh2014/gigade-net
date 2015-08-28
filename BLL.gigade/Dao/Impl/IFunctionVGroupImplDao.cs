using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IFunctionVGroupImplDao
    {
        int Save(Model.FunctionGroup functionGroup);
        int Delete(int RowId);
        List<Model.Function> CallerAuthorityQuery(Model.Query.AuthorityQuery query);
        List<Model.Function> GroupAuthorityQuery(Model.Query.AuthorityQuery query);
    }
}
