using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao.Impl
{
   public interface IWebContentType8ImplDao
    {
       List<WebContentType8Query> QueryAll(WebContentType8Query query, out int totalCount);
       int Insert(WebContentType8 model);
       int Update(WebContentType8 model);
       WebContentType8 GetModel(WebContentType8 model);
       int delete(WebContentType8 model);
       int GetDefault(WebContentType8 model);

    }
}
