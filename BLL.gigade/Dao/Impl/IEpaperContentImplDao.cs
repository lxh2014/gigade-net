using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao.Impl
{
  public  interface IEpaperContentImplDao
    {
      List<EpaperContent> GetEpaperContentLimit();
      List<Model.Query.EpaperContentQuery> GetEpaperContent();
      EpaperContentQuery GetEpaperContentById(EpaperContentQuery query);
      List<EpaperContentQuery> GetEpaperContentList(EpaperContentQuery query, out int totalCount);
      List<EpaperLogQuery> GetEpaperLogList(EpaperLogQuery query, out int totalCount);
      int SaveEpaperContent(EpaperContentQuery query);
    }
}
