using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IWebContentType6ImplMgr
    {
        List<WebContentType6Query> QueryAll(WebContentType6Query query, out int totalCount);
        int Insert(WebContentType6 m);
        int Update2(WebContentType6 m);
        int Update(WebContentType6 m);
        WebContentType6 GetModel(WebContentType6 model);
        int delete(WebContentType6 model);
        int GetDefault(WebContentType6 model);
    }
}
