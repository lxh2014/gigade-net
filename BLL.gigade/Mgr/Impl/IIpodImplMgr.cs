﻿using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
   public interface IIpodImplMgr
    {
       List<IpodQuery> GetIpodList(IpodQuery query, out int totalcount);
       int GetPodID(IpodQuery query);
       int AddIpod(IpodQuery query);
       int UpdateIpod(IpodQuery query);
       int UpdateIpodCheck(IpodQuery query);
       int DeletIpod(IpodQuery query);
       List<IpodQuery> GetIpodListExprot(IpodQuery query);
       bool GetIpodfreight(string po_id, int freight);
    }
}
