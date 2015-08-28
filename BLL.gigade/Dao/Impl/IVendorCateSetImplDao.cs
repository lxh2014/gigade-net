using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Dao.Impl
{
    interface IVendorCateSetImplDao
    {
        string GetMaxCodeSerial(VendorCateSet vcs);
        string SaveSql(VendorQuery vcsquery);
    }
}
