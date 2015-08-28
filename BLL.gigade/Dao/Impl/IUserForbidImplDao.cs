using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao.Impl
{
    public interface IUserForbidImplDao
    {
        List<UserForbidQuery> GetUserForbidList(UserForbidQuery store, out int totalCount);
        int UserForbidInsert(UserForbidQuery query);
        int UserForbidDelete(UserForbidQuery query);
        int GetUserForbidIp(UserForbidQuery query);
    }
}
