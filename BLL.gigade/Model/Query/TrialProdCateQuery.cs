using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class TrialProdCateQuery : TrialProdCate
    {
        public string event_name { get; set; }
        public string category_name { get; set; }
        public string product_name { get; set; }
        public string event_type { get; set; }
        public int t_id { get; set; }
        public string site { get; set; }
        public int device { get; set; }

        public TrialProdCateQuery()
        {
            event_name = string.Empty;
            category_name = string.Empty;
            product_name = string.Empty;
            event_type = string.Empty;
            t_id = 0;
            site = string.Empty;
            device = 0;
        }
    }
}
