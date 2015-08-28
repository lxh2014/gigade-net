using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EpaperContentQuery : Model.EpaperContent
    {
        public DateTime epaperShowStart { get; set; }
        public DateTime epaperShowEnd { get; set; }
        public DateTime epaperCreateDate { get; set; }
        public DateTime epaperUpdateDate { get; set; }
        public string log_description { get; set; }
        public int log_id { get; set; }
        public string searchCon { get; set; }
        public string search_text { get; set; }
        public string dateCon { get; set; }
        public string user_username { get; set; }
        public string epaperStatus { get; set; }
        public EpaperContentQuery()
        {
            epaperShowStart = DateTime.MinValue;
            epaperShowEnd = DateTime.MinValue;
            epaperCreateDate = DateTime.MinValue;
            epaperUpdateDate = DateTime.MinValue;
            log_description = string.Empty;
            searchCon = string.Empty;
            search_text = string.Empty;
            dateCon = string.Empty;
            user_username = string.Empty;
            log_id = 0;
            epaperStatus = string.Empty;
        }
    }
}
