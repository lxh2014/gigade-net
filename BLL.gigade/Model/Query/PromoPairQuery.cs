using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromoPairQuery : PromoPair
    {
        public string category_link_url { get; set; }
        public string banner_image { get; set; }
        public string group_name { get; set; }
        public string condition_name { get; set; }
        public string parameterName { get; set; }
        public string payment_name { get; set; }
        public string PN1 { get; set; }
        public int expired { get; set; }
        public string PTname { get; set; }
        public string event_id { get; set; }
        public string bank { get; set; }//select語句沒有查詢
        public string category_ipfrom { get; set; }
        public string user_username { get; set; }

        public PromoPairQuery()
        {
            banner_image = string.Empty;
            category_link_url = string.Empty;
            group_name = string.Empty;
            condition_name = string.Empty;
            payment_name = string.Empty;
            bank = string.Empty;
            PN1 = string.Empty;
            expired = 0;
            PTname = string.Empty;
            event_id = string.Empty;
            category_ipfrom = string.Empty;
            user_username = string.Empty;
        }
    }
}
