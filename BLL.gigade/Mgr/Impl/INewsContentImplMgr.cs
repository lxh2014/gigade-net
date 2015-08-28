using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface INewsContentImplMgr
    {
        List<NewsContentQuery> GetNewsList(NewsContentQuery store, out int totalCount);
        string GetNewsContent();
        int NewsContentSave(NewsContentQuery store);
        NewsContentQuery OldQuery(uint newsId);
        List<NewsLogQuery> GetNewsLogList(NewsLogQuery store, out int totalCount);
    }
}
