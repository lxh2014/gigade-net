using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IWebContentType4ImplMgr
    {
        List<WebContentType4Query> QueryAll(WebContentType4Query query, out int totalCount);
        int Insert(WebContentType4 m);
        int Update(WebContentType4 m);
        WebContentType4 GetModel(WebContentType4 model);
        int delete(WebContentType4 model);
        int GetDefault(WebContentType4 model);
    }
}
