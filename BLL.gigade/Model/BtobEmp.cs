using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public   class BtobEmp:PageBase
    {
      public uint id { get; set; }
      public string emp_id { get; set; }
      public string group_id { get; set; }
      public uint status { get; set; }
      public int create_date { get; set; }
      public int update_date { get; set; }

      public BtobEmp()
      {
          id = 0;
          emp_id = string.Empty;
          group_id = string.Empty;
          status = 1;
          create_date = 0;
          update_date = 0;

      }
    }
}
