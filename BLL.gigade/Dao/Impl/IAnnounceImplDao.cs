using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IAnnounceImplDao
    {
        List<Model.Query.AnnounceQuery> GetAnnounce();
        AnnounceQuery GetAnnounce(AnnounceQuery query);
        int AnnounceSave(AnnounceQuery store);
        List<AnnounceQuery> GetAnnounceList(AnnounceQuery store, out int totalCount);
    }
}
