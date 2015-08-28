using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IIialgImplMgr
    {
        int insertiialg(IialgQuery iagQuery);
        int HuiruInsertiialg(DataRow[] dr, out int iialgtotal, out int iinvdtotal);//批量匯入到iialg表中
        List<IialgQuery> GetIialgList(IialgQuery q, out int totalCount); 

        List<IialgQuery> GetExportIialgList(IialgQuery q);
        int addIialgIstock(IialgQuery q);
    }
}
