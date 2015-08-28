using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IPayEasyImplDao
    {
        List<PayEasyQuery> Query(PayEasyQuery query);
        DataTable QueryExcel(PayEasyQuery query);
    }
}
