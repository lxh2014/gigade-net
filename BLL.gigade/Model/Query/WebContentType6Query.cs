using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class WebContentType6Query : WebContentType6
    {
        public string site_name { get; set; }
        public string page_name { get; set; }
        public string area_name { get; set; }
        public string serchwhere { get; set; }


        public WebContentType6Query()
        {
            site_name = string.Empty;
            page_name = string.Empty;
            area_name = string.Empty;
            serchwhere = string.Empty;
        }
    }
}
