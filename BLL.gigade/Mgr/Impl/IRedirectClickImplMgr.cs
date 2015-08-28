using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IRedirectClickImplMgr
    {
        List<RedirectClickQuery> QueryAllById(RedirectClickQuery query);
        List<RedirectClickQuery> GetRedirectClick(RedirectClickQuery query);
        RedirectClickQuery ReturnMinClick();
    }
}
