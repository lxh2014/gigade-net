using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderAccumAmountQuery : OrderAccumAmount
    {
        public string event_id_in { set; get; }
        public int dateCondition { get; set; }
        public OrderAccumAmountQuery()
        {
            event_id_in = string.Empty;
            dateCondition = 0;
        }
    }
}
