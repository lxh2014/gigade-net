using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
  public  interface IOrderPaymentNcccImplMgr
    {
      DataTable OrderPaymentNccc(SinopacDetailQuery store,string Sql);

    }
}
