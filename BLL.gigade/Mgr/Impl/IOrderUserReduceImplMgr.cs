﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderUserReduceImplMgr
    {
        List<PromotionsAmountReduceMemberQuery> GetOrderUserReduce(PromotionsAmountReduceMemberQuery store, out int totalCount);
        List<PromotionsAmountReduceMemberQuery> GetReduceStore();
        List<VipUserGroup> GetVipUserGroupStore();
    }
}
