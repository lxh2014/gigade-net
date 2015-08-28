using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IMailUserImplMgr
    {
        List<MailUserQuery> GetMailUserStore(MailUserQuery query, out int totalcount);
        int SaveMailUser(MailUserQuery query);
        int DeleteMailUser(MailUserQuery query);
        int UpdateMailUserStatus(MailUserQuery query);
        DataTable GetUserInfo(int r_id);
    }
}
