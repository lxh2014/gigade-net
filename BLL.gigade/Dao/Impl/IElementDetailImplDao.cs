using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IElementDetailImplDao
    {
        List<ElementDetailQuery> QueryAll(ElementDetailQuery query, out int totalCount);
        int Save(ElementDetail model);
        int Update(ElementDetail model);
        ElementDetail GetModel(ElementDetail query);
        int UpdateStatus(ElementDetailQuery m);
        List<ElementDetail> QueryElementDetail();
        List<ElementDetailQuery> QueryAllWares(ElementDetailQuery query, out int totalCount);
        List<ElementDetailQuery> QueryPacketProd(ElementDetail model);
        string DeleteElementDetail(int element_id);
    }
}
