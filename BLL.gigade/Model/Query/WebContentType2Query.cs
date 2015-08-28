using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class WebContentType2Query : WebContentType2
    {
        public string site_name { get; set; }
        public string page_name { get; set; }
        public string area_name { get; set; }
        public string product_name { get; set; }
        public string prod_sz { get; set; }

        public WebContentType2Query()
        {
            site_name = string.Empty;
            page_name = string.Empty;
            area_name = string.Empty;
            product_name = string.Empty;
            prod_sz = string.Empty;
        }
    }
}
