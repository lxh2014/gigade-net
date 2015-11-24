using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromoPair : PageBase
    {
        public int id { get; set; }
        public string event_name { get; set; }
        public string event_desc { get; set; }
        public string event_type { get; set; }
        public int condition_id { get; set; }
        public int group_id { get; set; }
        public DateTime starts { get; set; }
        public DateTime end { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public Boolean active { get; set; }
        public int deliver_type { get; set; }
        public string device { get; set; }
        public string payment_code { get; set; }
        public int cate_red { get; set; }
        public int cate_green { get; set; }
        public string kuser { get; set; }
        public string muser { get; set; }
        public string website { get; set; }//edit 20140926
        public int category_id { get; set; }
        public int price { get; set; }
        public int discount { get; set; }
        public int status { get; set; }
        public int vendor_coverage { get; set; }

        public PromoPair()
        {
            id = 0;
            event_name = string.Empty;
            event_desc = string.Empty;
            event_type = string.Empty;
            condition_id = 0;
            group_id = 0;
            starts = DateTime.MinValue;
            end = DateTime.MinValue;
            created = DateTime.MinValue;
            modified = DateTime.MinValue;
            active = false;
            deliver_type = 0;
            device = string.Empty;
            payment_code = string.Empty;
            cate_red = 0;
            cate_green = 0;
            kuser = string.Empty;
            muser = string.Empty;
            category_id = 0;
            price = 0;
            discount = 0;
            status = 1;
            website = string.Empty;
            vendor_coverage = 0;
        }

    }
}
