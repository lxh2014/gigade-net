using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
  public  class EmsGoal:PageBase
    { 
      public int row_id { get; set; }
      public string department_code { get; set; }
      public int year { get; set; }
      public int month { get; set; }
      public int goal_amount { get; set; }
      public int status { get; set; }

      public EmsGoal()
      {
          row_id = 0;
          department_code = string.Empty;
          year = 2000;
          month =1;
          goal_amount = 0;
          status = 1;
      }
    }
}
