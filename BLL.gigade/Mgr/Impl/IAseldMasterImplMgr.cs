using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IAseldMasterImplMgr
    {
        string Insert(AseldMaster m);
        int SelectCount(AseldMaster m);
    }
}
