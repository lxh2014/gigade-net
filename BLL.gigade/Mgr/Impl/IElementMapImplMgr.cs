using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{  
   public  interface IElementMapImplMgr
    {
        List<ElementMapQuery> GetElementMapList(ElementMapQuery query, out int totalCount);
        int upElementMap(ElementMapQuery query);
        int AddElementMap(ElementMapQuery query);
        bool SelectElementMap(ElementMapQuery query);
        bool GetAreaCount(int areaId);
    }
}
