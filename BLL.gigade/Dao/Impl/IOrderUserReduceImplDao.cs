using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    public interface IOrderUserReduceImplDao
    {
        List<PromotionsAmountReduceMemberQuery> GetOrderUserReduce(PromotionsAmountReduceMemberQuery store, out int totalCount);
        List<PromotionsAmountReduceMemberQuery> GetReduceStore();
        List<VipUserGroup> GetVipUserGroupStore();
    }
}
