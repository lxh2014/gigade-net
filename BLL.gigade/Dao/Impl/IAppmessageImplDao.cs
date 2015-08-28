using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IAppmessageImplDao
    {
        List<AppmessageQuery> GetAppmessageList(AppmessageQuery appmsg, out int totalCount);
        List<Appmessage> GetParaList(string sql);
        int AppMessageInsert(Appmessage appmsg);
    }
}
