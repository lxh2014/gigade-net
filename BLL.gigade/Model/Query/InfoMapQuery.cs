using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class InfoMapQuery : InfoMap
    {
        public string site_name { get; set; }
        public string page_name { get; set; }
        public string area_name { get; set; }
        public string info_name { get; set; }
        public string update_user_name { get; set; }

        public InfoMapQuery()
        {
            site_name = string.Empty;
            page_name = string.Empty;
            area_name = string.Empty;
            info_name = string.Empty;
            update_user_name = string.Empty;

        }
    }
}
