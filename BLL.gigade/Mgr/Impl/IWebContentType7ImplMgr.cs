using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IWebContentType7ImplMgr
    {
        List<WebContentType7Query> QueryAll(WebContentType7Query query, out int totalCount);
        int Insert(WebContentType7 m);
        //int Update2(Model.WebContentType7 model);
        int Update(WebContentType7 m);
        WebContentType7 GetModel(WebContentType7 model);
        int delete(WebContentType7 model);
        int GetDefault(WebContentType7 model);
    }
}
