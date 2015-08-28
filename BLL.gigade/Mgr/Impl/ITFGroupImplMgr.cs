using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
  public  interface ITFGroupImplMgr
    {
      List<TFGroup> QueryAll(TFGroup m);
    }
}
