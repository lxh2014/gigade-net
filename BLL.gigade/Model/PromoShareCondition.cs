using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
 public   class PromoShareCondition:PageBase
    {
     public int condition_id { get;set;}
     public int condition_type_id { get; set; }
     public string condition_name { get; set; }
     public string condition_value { get; set; }
     public int promo_id { get; set; }

     public PromoShareCondition()
     {
         condition_id = 0;
         condition_type_id = 0;
         condition_name = string.Empty;
         condition_value = string.Empty;
         promo_id = 0;
     }
    }
}
