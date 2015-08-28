using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IIpodImplDao
    {
        List<IpodQuery> GetIpodList(IpodQuery query, out int totalcount);
        int GetPodID(IpodQuery query);
        int AddIpod(IpodQuery query);
        int UpdateIpod(IpodQuery query);
        int DeletIpod(IpodQuery query);
        List<IpodQuery> GetIpodListExprot(IpodQuery query);
    }
}
