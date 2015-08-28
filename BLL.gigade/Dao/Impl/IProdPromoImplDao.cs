using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    interface IProdPromoImplDao
    {
        List<ProdPromo> Select(ProdPromo query, out int totalCount);
        int Save(ProdPromo model);
        int Update(ProdPromo model);
        
    }
}
