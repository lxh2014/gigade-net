using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr.Impl
{
   public interface IEpaperContentImplMgr
    {
        string GetEpaperContent();
        List<EpaperContent> GetEpaperContentLimit();
        EpaperContentQuery GetEpaperContentById(EpaperContentQuery query);
        List<EpaperContentQuery> GetEpaperContentList(EpaperContentQuery query, out int totalCount);
        List<EpaperLogQuery> GetEpaperLogList(EpaperLogQuery query, out int totalCount);
        int SaveEpaperContent(EpaperContentQuery query);

    }
}
