using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductStatusApplyImplMgr
    {
        ProductStatusApply Query(ProductStatusApply query);
        string Save(ProductStatusApply apply);
        string Delete(ProductStatusApply apply);
    }
}
