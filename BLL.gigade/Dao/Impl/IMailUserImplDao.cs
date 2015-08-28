using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IMailUserImplDao
    {
        List<MailUserQuery> GetMailUserStore(MailUserQuery query, out int totalcount);
        List<MailUserQuery> MailUserQuery(MailUserQuery query);
        int SaveMailUser(MailUserQuery query);
        int DeleteMailUser(MailUserQuery query);
        int UpdateMailUserStatus(MailUserQuery query);
        DataTable GetUserInfo(int r_id);
    }
}
