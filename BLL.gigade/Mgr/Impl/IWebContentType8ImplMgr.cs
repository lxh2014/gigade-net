using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr.Impl
{
   public interface IWebContentType8ImplMgr
    {
        List<WebContentType8Query> QueryAll(WebContentType8Query query, out int totalCount);
        int Insert(WebContentType8 m);
        int Update(WebContentType8 m);
        WebContentType8 GetModel(WebContentType8 model);
        int delete(WebContentType8 model);
        int GetDefault(WebContentType8 model);
    }
}
