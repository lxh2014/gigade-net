using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPriceMasterTsImplMgr
    {
        Model.PriceMaster QueryPriceMasterTs(Model.PriceMaster pM);
        List<Model.PriceMaster> QueryByApplyId(Model.PriceMaster pM);
        string UpdateTs(Model.PriceMaster pM);
        //string UpdateEventTs(Model.PriceMaster pM);
        string DeleteTs(Model.PriceMaster pM);
    }
}
