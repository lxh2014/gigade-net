using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class SphinxExcludeQuery : SphinxExclude
    {
        public string user_username { get; set; }
        public DateTime created_start { get; set; }
        public DateTime created_end { get; set; }
        public string product_name { get; set; }
        public int product_id_old { get; set; }
        public string product_ids { get; set; }
        public SphinxExcludeQuery()
        {
            user_username = string.Empty;
            created_end = DateTime.MinValue;
            created_start = DateTime.MinValue;
            product_name = string.Empty;
            product_id_old = 0;
            product_ids = string.Empty;
        }
    }
}
