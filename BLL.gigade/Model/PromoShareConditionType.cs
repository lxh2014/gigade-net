using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
 public   class PromoShareConditionType:PageBase
    {
     public int condition_type_id { get; set; }
     public string condition_type_desc { get; set; }
     public string  condition_type_name { get; set; }
     public int condition_type_status { get; set; }

     public PromoShareConditionType()
     {
         condition_type_id = 0;
         condition_type_desc = string.Empty;
         condition_type_name = string.Empty;
         condition_type_status = 0;
     }
    }
}
