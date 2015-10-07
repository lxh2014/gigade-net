using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr.Impl
{
    public interface IElementDetailImplMgr
    {
        List<ElementDetailQuery> QueryAll(ElementDetailQuery query, out int totalCount);
        int Save(ElementDetail model);
        int Update(ElementDetail model);
        ElementDetail GetModel(ElementDetail query);
        int UpdateStatus(Model.Query.ElementDetailQuery query);
        List<ElementDetail> QueryElementDetail();
        List<ElementDetailQuery> QueryAllWares(ElementDetailQuery query, out int totalCount);
        List<ElementDetailQuery> QueryPacketProd(ElementDetail model);
        bool DeleteElementDetail(string[] newRowID);
    }
}
