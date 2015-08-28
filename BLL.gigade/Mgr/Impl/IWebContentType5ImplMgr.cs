using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IWebContentType5ImplMgr
    {
        List<WebContentType5Query> QueryAll(WebContentType5Query query, out int totalCount);
        int Insert(WebContentType5 m);
        int Update(WebContentType5 m);
        WebContentType5 GetModel(WebContentType5 model);
        int delete(WebContentType5 model);
        int GetDefault(WebContentType5 model);
    }
}
