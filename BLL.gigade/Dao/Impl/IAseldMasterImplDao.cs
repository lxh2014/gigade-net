using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IAseldMasterImplDao
    {
        string Insert(AseldMaster m);
        int SelectCount(AseldMaster m);
    }
}
