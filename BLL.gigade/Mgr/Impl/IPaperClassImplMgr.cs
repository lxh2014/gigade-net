﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPaperClassImplMgr
    {
        List<PaperClass> GetPaperClassList(PaperClass pc, out int totalCount);
        int Add(PaperClass pc);
        int Update(PaperClass pc);
        int UpdateClassID(PaperClass pc);
        int UpdateState(string id, int status);
        int Delete(PaperClass pc);
    }
}
