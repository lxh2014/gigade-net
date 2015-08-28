using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromotionsBonusSerialImplMgr
    {
        List<Model.PromotionsBonusSerial> QueryById(int id);
        int Save(string serials, int id);
    }
}
