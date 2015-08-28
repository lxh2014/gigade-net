using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderReturnContent:PageBase
    {
        public int orc_id { set; get; }
        public int orc_order_id { set; get; }
        public string orc_deliver_code { set; get; }
        public DateTime orc_deliver_date { set; get; }
        public string orc_deliver_time { set; get; }
        public string orc_name { set; get; }
        public int orc_phone { set; get; }
        public string orc_zipcode { set; get; }
        public string orc_address { set; get; }
        public string orc_remark { set; get; }
        public int orc_type { set; get; }
        public string orc_service_remark { set; get; }
        public OrderReturnContent()
        {
            orc_id = 0;
            orc_order_id = 0;
            orc_deliver_code = string.Empty;
            orc_deliver_date = DateTime.MinValue;
            orc_deliver_time = string.Empty;
            orc_name = string.Empty;
            orc_phone = 0;
            orc_zipcode = string.Empty;
            orc_address = string.Empty;
            orc_remark = string.Empty;
            orc_type = 0;
            orc_service_remark = string.Empty;
        }
    }
}
