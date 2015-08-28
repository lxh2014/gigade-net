using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
   public  interface IOrderImplDao
    {
        bool ThingsMethod(string[] rows, OrderDeliver order, OrderSlaveMaster master, string Descriptions);
        bool SelfThingsMethod(DataTable _dtSms, OrderDeliver query, string Descriptions);
    }
}
