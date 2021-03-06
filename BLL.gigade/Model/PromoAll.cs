﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PromoAll:PageBase
    {
        public int rid { get; set; }
        public string event_id { get; set; }
        public string event_type { get; set; }
        public int brand_id { get; set; }
        public int class_id { get; set; }
        public int product_id { get; set; }
        public int category_id { get; set; }
        public DateTime startTime { get; set; }
        public DateTime end { get; set; }
        public string kuser { get; set; }
        public DateTime kdate { get; set; }
        public string muser { get; set; }
        public DateTime mdate { get; set; }
        public int status { get; set; }

        public PromoAll()
        {
            rid = 0;
            event_id = string.Empty;
            event_type = string.Empty;
            brand_id = 0;
            class_id = 0;
            product_id = 0;
            category_id = 0;
            startTime = DateTime.MinValue;
            end = DateTime.MinValue;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
            muser = string.Empty;
            mdate = DateTime.MinValue;
            status = 1;
            
        }
    }
}
