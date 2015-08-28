using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IRedirectImplDao
    { 
        DataTable GetRedirectList(RedirectQuery query, out int totalcount);
        string Save(Redirect query);
        int Update(Redirect query);
        int EnterInotRedirect(RedirectQuery query);
        List<RedirectQuery> GetRedirect(RedirectQuery query, out int totalcount);
        string GetSum(RedirectQuery query);
        DataTable GetRedirectListCSV(RedirectQuery query);
    }
}
