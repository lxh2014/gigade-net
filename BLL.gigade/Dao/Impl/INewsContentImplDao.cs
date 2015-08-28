using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface INewsContentImplDao
    {
        List<NewsContentQuery> GetNewsList(NewsContentQuery store, out int totalCount);
        int NewsContentSave(NewsContentQuery store);
        NewsContentQuery OldQuery(uint newsId);
        List<NewsContentQuery> GetNewContent();
        List<NewsLogQuery> GetNewsLogList(NewsLogQuery store, out int totalCount) ;
    }
}
