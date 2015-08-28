using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class EmsActual:PageBase
    {
       public int row_id { get; set; }
       public string department_code { get; set; } 
       public int year { get; set; }
       public int month { get; set; }
       public int day { get; set; }
       public int type { get; set; }
       public int cost_sum { get; set; }
       public int order_count { get; set; }
       public int amount_sum { get; set; }
       public int status { get; set; }

       public EmsActual()
       {
           row_id = 0;
           department_code = string.Empty;
           year = 2000;
           month = 1;
           day = 1;
           type =2;
           cost_sum = 0;
           order_count = 0;
           amount_sum = 0;
           status = 1;
       }
    }
}
