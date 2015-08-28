using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
  public  interface ISinopacDetailImplDao
    {
      DataTable GetSinopacDetai(SinopacDetail store, string sql);
    }
}
