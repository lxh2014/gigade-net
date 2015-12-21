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
        List<IpodQuery> GetIpodListNo(IpodQuery query, out int totalcount);
        int GetPodID(IpodQuery query);
        int AddIpod(IpodQuery query);
        int UpdateIpod(IpodQuery query);
        string UpdateIpodCheck(IpodQuery query);
        int DeletIpod(IpodQuery query);
        List<IpodQuery> GetIpodListExprot(IpodQuery query);
        bool GetIpodfreight(string po_id, int freight);
        BLL.gigade.Model.ProductItem GetStockHistorySql(IpodQuery query, out string Stock);
        BLL.gigade.Model.Product GetIgnoreHistorySql(IpodQuery query, out string Shortage);
        string GetInsertIpoNvdSql(IpoNvdQuery query);
    }
}
