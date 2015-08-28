using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Mgr.Impl
{
    public interface IIlocImplMgr
    {
        int IlocInsert(Iloc loc);
        int IlocEdit(Model.Iloc loc);
        List<IlocQuery> GetIocList(IlocQuery loc, out int totalCount);
        int GetLoc_id(Iloc loc);
        string  GetLoc_idByRow_id(int row_id);

        int DeleteLocidByIloc(Model.Iloc loc);
        int UpdateIlocLock(Iloc loc);

        int SetIlocUsed(Iloc loc);
        List<IlocQuery> Export(IlocQuery loc);//匯出數據

        List<IlocQuery> GetIlocExportList(IlocQuery loc);

        int SaveBySql(string str);
        int HashAll();

        string GetLocidByHash(string loc_id);
    }
}
