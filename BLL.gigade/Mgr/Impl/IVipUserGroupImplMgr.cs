using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public  interface IVipUserGroupImplMgr
    {
        List<VipUserGroup> GetAllUserGroup();
        List<VipUserGroupQuery> QueryAll(VipUserGroupQuery query, out int totalCount);
        string GetVuserCount(VipUserGroup query);
        VipUserGroup GetModelById(uint group_id);
        int Insert(VipUserGroup userGroup);
        int Update(VipUserGroup userGroup);//update
        List<VipUserQuery> GetVipUserList(VipUserQuery vu, out int totalCount);
        DataTable GetVipUser(VipUser vu);
        int InsertVipUser(VipUser vu);
        int UpdateUserState(int state, int id);/*更改會員狀態*/
        DataTable GetUser(string sqlwhere);
        DataTable BtobEmp(string group_id);
        int UpdateEmp(string group_id, string erp_id, int k);
        int InsertEmp(string group_id, string emp_id);
    }
}
