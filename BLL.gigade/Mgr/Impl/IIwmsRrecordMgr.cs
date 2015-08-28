using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IIwmsRrecordMgr
    {
        List<IwmsRrecordQuery> GetIwmsRrecordList(IwmsRrecordQuery query, out int totalCount);
        List<ManageUser> GetUserslist(string code);
    }
}
