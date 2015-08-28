using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
   public interface IPromotionsAmountReduceImplMgr
    {
       string Save(Model.PromotionsAmountReduce model);
       string Update(Model.PromotionsAmountReduce model);
       string Delete(int id);
    }
}
