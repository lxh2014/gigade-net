using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class DisableKeywordsQuery : DisableKeywords
    {
        public DateTime dk_modify { get; set; }
        public string user_name { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string search_text { get; set; }
        public string ids { get; set; }
        public DisableKeywordsQuery()
        {
            dk_modify = DateTime.MinValue;
            user_name = string.Empty;
            start = DateTime.MinValue;
            end = DateTime.MinValue;
            search_text = string.Empty;
            ids = string.Empty;
        }
    }
}
