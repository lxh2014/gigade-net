using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IPromotionsBonusSerialHistoryImplDao
    {
        List<Model.Query.PromotionsBonusSerialHistoryQuery> QueryById(int id);
    }
}
