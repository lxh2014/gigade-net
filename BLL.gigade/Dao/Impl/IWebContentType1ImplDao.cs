using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IWebContentType1ImplDao
    {
        List<WebContentType1Query> QueryAll(WebContentType1Query query, out int totalCount);
        int Insert(WebContentType1 model);
        int Update(WebContentType1 model);
        WebContentType1 GetModel(WebContentType1 model);
        int delete(WebContentType1 model);
        int GetDefault(WebContentType1 model);

    }
}
