using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProdPromoImplMgr
    {
        List<ProdPromo> Select(ProdPromo model, out int totalCount);
        int Save(ProdPromo model);
        int Update(ProdPromo model);
        
    }
}
