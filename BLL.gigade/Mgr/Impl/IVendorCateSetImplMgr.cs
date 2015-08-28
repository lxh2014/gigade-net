using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public interface IVendorCateSetImplMgr
    {
        string GetMaxCodeSerial(VendorCateSet vcs);
    }
}
