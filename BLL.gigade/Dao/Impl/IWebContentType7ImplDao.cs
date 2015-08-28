using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IWebContentType7ImplDao
    {
        List<WebContentType7Query> QueryAll(WebContentType7Query query, out int totalCount);
        int Insert(WebContentType7 model);
        //int Update2(Model.WebContentType7 model);
        int Update(WebContentType7 model);
        WebContentType7 GetModel(WebContentType7 model);
        int delete(WebContentType7 model);
        int GetDefault(WebContentType7 model);

    }
}
