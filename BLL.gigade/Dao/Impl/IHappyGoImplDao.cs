using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IHappyGoImplDao
    {
        List<HgDeduct> GetHGDeductList(uint order_id);
        List<HgAccumulate> GetHGAccumulateList(uint order_id);
        List<HgAccumulateRefund> GetHGAccumulateRefundList(uint order_id);
        List<HgDeductRefund> GetHgDeductRefundList(uint order_id);
        List<HgDeductReverse> GetHgDeductReverseList(uint order_id);
    }
}
