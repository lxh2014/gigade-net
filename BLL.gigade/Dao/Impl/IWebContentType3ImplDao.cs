using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IWebContentType3ImplDao
    {
        List<WebContentType3Query> QueryAll(WebContentType3Query query, out int totalCount);
        int Insert(WebContentType3 model);
        int Update(WebContentType3 model);
        WebContentType3 GetModel(WebContentType3 model);
        int delete(WebContentType3 model);
        int GetDefault(WebContentType3 model);

    }
}
