using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IWebContentType2ImplDao
    {
        List<WebContentType2Query> QueryAll(WebContentType2Query query, out int totalCount);
        int Insert(WebContentType2 model);
        int Update(WebContentType2 model);
        WebContentType2 GetModel(WebContentType2 model);
        int delete(WebContentType2 model);
        int GetDefault(WebContentType2 model);
    }
}
