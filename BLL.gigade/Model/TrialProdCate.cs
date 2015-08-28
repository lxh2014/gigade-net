using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class TrialProdCate : PageBase
    {
        public int id { get; set; }
        public string event_id { get; set; }
        public int type { get; set; }
        public uint category_id { get; set; }
        public uint product_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }

        public TrialProdCate()
        {
            id = 0;
            event_id = string.Empty;
            type = 0;
            category_id = 0;
            product_id = 0;
            start_date = DateTime.MinValue;
            end_date = DateTime.MinValue;
        }
    }
}
