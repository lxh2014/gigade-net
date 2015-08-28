using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
   public  class SmsQuery:Sms
    {
       public string estimated_time { get; set; }
       public string created_time { get; set; }
       public string modified_time { get; set; }
       public DateTime StartTime { get; set; }
       public DateTime EndTime { get; set; }

       public SmsQuery()
       {
           estimated_time = string.Empty;
           created_time = string.Empty;
           modified_time = string.Empty;
           StartTime = DateTime.MinValue;
           EndTime = DateTime.MinValue;
       }
    }
}
