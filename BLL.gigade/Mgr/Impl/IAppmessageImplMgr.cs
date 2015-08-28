using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IAppmessageImplMgr
    {
        List<AppmessageQuery> GetAppmessageList(AppmessageQuery appmsg, out int totalCount);
        string GetParaList(string para);
        int AppMessageInsert(Appmessage appmsg);
    }
}
