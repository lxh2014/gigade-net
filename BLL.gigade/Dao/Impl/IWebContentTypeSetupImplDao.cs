using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IWebContentTypeSetupImplDao
    {
        List<WebContentTypeSetup> Query(WebContentTypeSetup model);
        DataTable QueryPageStore(WebContentTypeSetup model);
        DataTable QueryAreaStore(WebContentTypeSetup model);
    }
}
