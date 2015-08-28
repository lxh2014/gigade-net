using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProdPromo:PageBase
    {
        public int rid { get; set; }
        public int product_id { get; set; }
        public string event_id { get; set; }
        public string event_type { get; set; }
        public string event_desc { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string page_url { get; set; }
        public int user_specified { get; set; }
        public string kuser { get; set; }
        public DateTime  kdate { get; set; }
        public string muser { get; set; }
        public DateTime mdate { get; set; }
        public int status { get; set; }
        public string SearchContent { get; set; }
        public ProdPromo()
        {
            rid = 0;
            product_id = 0;
            event_id = string.Empty;
            event_type = string.Empty;
            event_desc = string.Empty;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            page_url = string.Empty;
            user_specified = 0;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
            muser = string.Empty;
            mdate = DateTime.MinValue;
            status = 1;
            SearchContent = string.Empty;
        }
    }
}
