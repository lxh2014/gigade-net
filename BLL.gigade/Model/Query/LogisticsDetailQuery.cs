using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class LogisticsDetailQuery : LogisticsDetail
    {
        public string delivery_store_name { set; get; }
        public string logistics_type { set; get; }
        public string order_id { set; get; }
        public LogisticsDetailQuery() 
        {
            delivery_store_name = string.Empty;
            logistics_type = string.Empty;
            order_id =string.Empty;
        }
    }
}
