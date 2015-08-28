using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IPromotionsAmountReduceImplDao
    {
        string Save(Model.PromotionsAmountReduce model);
        string Update(Model.PromotionsAmountReduce model);
        string Delete(int id);
    }
}
