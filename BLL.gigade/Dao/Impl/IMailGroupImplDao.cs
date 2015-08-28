using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IMailGroupImplDao
    {
        List<MailGroupQuery> MailGroupList(MailGroupQuery query,out int totalCount);
        List<MailGroupQuery> MailGroupQuery(MailGroupQuery query);
        int SaveMailGroup(MailGroupQuery query);
        string DeleteMailGroup(MailGroupQuery query);
        int UpMailGroupStatus(MailGroupQuery query);
        List<MailGroupMapQuery> QueryUserById(MailGroupMapQuery query);
        int DeleteMailMap(int group_id);
        string SaveMailMap(MailGroupMapQuery query);
        int GetStatus(int user_id);
    }
}
