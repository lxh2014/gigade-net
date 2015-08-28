using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IPaperImplDao
    {
        List<Paper> GetPaperList(Paper p, out int totalCount);
        int Add(Paper p);
        int Update(Paper p);
        int UpdateState(Paper p);
    }
}
