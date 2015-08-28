using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public  class DeliveryAddress:PageBase
    {
       public int da_id { get; set; }
       public int user_id { get; set; }
       public string da_title { get; set; }
       public string da_name { get; set; }
       public int da_gender { get; set; }
       public string da_mobile_no { get; set; }
       public string da_tel_no { get; set; }
       public string da_dist { get; set; }
       public string da_address { get; set; }
       public int da_default { get; set; }
       public DateTime da_created { get; set; }
       public int da_modified { get; set; }

       public DeliveryAddress()
       {
           da_id = 0;
           user_id = 0;
           da_title = string.Empty;
           da_name = string.Empty;
           da_gender = 0;
           da_mobile_no = string.Empty;
           da_tel_no = string.Empty;
           da_dist = string.Empty;
           da_address = string.Empty;
           da_default = 0;
           da_created = DateTime.Now;
           da_modified = 0;
       }
    }
}
