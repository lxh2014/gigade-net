using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IManageUserImplMgr
    {
        List<ManageUserQuery> GetNameMail(ManageUserQuery query, out int totalcount);
        List<ManageUser> GetManageUser(ManageUser m);
        int UnlockManagerUser(ManageUser m);
        List<ManageUserQuery> GetManageUserList(ManageUserQuery query, out int totalcount);
        int ManageUserAdd(ManageUserQuery q);
        int ManageUserUpd(ManageUserQuery query);
        int UpdPassword(ManageUserQuery q);
        int CheckEmail(ManageUserQuery query);//新增判斷郵件是否存在
        int UpdStatus(ManageUserQuery query);
        List<ManageUser> GetUserIdByEmail(string email);
    } 
}
