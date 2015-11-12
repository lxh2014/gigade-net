using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class DeliverMaster:PageBase
    {
       public uint deliver_id { get; set; }
       public int order_id { get; set; }
       public int ticket_id { get; set; }
       public uint type { get; set; }
       public int export_id { get; set; }
       public int import_id { get; set; }
       public uint freight_set { get; set; }
       public uint delivery_status { get; set; }
       public string delivery_name { get; set; }
       public string delivery_mobile { get; set; }
       public string delivery_phone { get; set; }
       public uint delivery_zip { get; set; }
       public string delivery_address { get; set; }
       public uint delivery_store { get; set; }
       public string delivery_code { get; set; }
       public int delivery_freight_cost { get; set; }
       public DateTime delivery_date { get; set; }
       public DateTime sms_date { get; set; }
       public DateTime arrival_date { get; set; }
       public DateTime estimated_delivery_date { get; set; }
       public DateTime estimated_arrival_date { get; set; }
       public int estimated_arrival_period { get; set; }
       public int creator { get; set; }
       public int verifier { get; set; }
       public DateTime created { get; set; }
       public DateTime modified { get; set; }
       public int export_flag { get; set; }
       public int data_chg { get; set; }
       public int work_status { get; set; }
       public DateTime expect_arrive_date { get; set; }
       public int expect_arrive_period { get; set; }
       public DeliverMaster()
       {
           deliver_id = 0;
           order_id = 0;
           ticket_id = 0;
           type = 1;
           freight_set = 1;
           delivery_status = 0;
           delivery_mobile = string.Empty;
           delivery_phone = string.Empty;
           delivery_zip = 0;
           delivery_address = string.Empty;
           delivery_store = 0;
           delivery_code = string.Empty;
           delivery_freight_cost = 0;
           delivery_date = DateTime.MinValue;
           sms_date = DateTime.MinValue;
           arrival_date = DateTime.MinValue;
           estimated_delivery_date = DateTime.MinValue;
           estimated_arrival_date = DateTime.MinValue;
           estimated_arrival_period = 0;
           created = DateTime.MinValue;
           modified = DateTime.MinValue;
           export_flag = 0;
           data_chg = 0;
           work_status = 0;
           expect_arrive_date = DateTime.MinValue;
           expect_arrive_period = 0;
       }
    }
}
