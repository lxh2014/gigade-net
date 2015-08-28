using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
     public  class OrderSlaveMaster : PageBase
    {
         public uint slave_master_id { set; get; }
         public string code_num { set; get; }
         public uint paper { set; get; }
         public uint order_freight_normal { set; get; }
         public uint order_freight_low { set; get; }
         public uint normal_subtotal { set; get; }
         public uint hypothermia_subtotal { set; get; }
         public uint deliver_store { set; get; }
         public string deliver_code { set; get; }
         public uint deliver_time { set; get; }
         public string deliver_note { set; get; }
         public uint createdate { set; get; }
         public uint creator { set; get; }
         public uint on_check { set; get; }

         public OrderSlaveMaster() 
         {
             slave_master_id = 0;
             code_num = string.Empty;
             paper = 0;
             order_freight_normal = 0;
             order_freight_low = 0;
             normal_subtotal = 0;
             hypothermia_subtotal = 0;
             deliver_store = 0;
             deliver_code = string.Empty;
             deliver_time = 0;
             deliver_note = string.Empty;
             createdate = 0;
             creator = 0;
             on_check = 0;

         }

    }
}
