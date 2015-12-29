using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IPromotionsBonusSerialImplDao
    {
        List<Model.PromotionsBonusSerial> QueryById(int id);
        int Save(string serials, int id);
        List<Model.PromotionsBonusSerial> QueryById(PromotionsBonusSerial query, out int TotalCount);
        int AddPromoBonusSerial(StringBuilder str);
    }
}
