using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IIialgImplMgr
    {
        int insertiialg(IialgQuery iagQuery);
        int HuiruInsertiialg(DataRow[] dr, out int iialgtotal, out int iinvdtotal);//批量匯入到iialg表中
        List<IialgQuery> GetIialgList(IialgQuery q, out int totalCount); 

        List<IialgQuery> GetExportIialgList(IialgQuery q);
        int addIialgIstock(IialgQuery q);
        List<ManageUser> GetkutiaoUser();//by zhaozhi0623j 庫調管理員store
    }
}
