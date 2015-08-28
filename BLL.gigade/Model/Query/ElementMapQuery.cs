using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Query
{
    public class ElementMapQuery : ElementMap
    {
        public string site_name { get; set; }
        public string page_name { get; set; }
        public string area_name { get; set; }
        public string packet_name { get; set; }
        public string create_user_name { get; set; }
        public int element_type { get; set; }
        public ElementMapQuery()
        {
            site_name = string.Empty;
            page_name = string.Empty;
            area_name = string.Empty;
            packet_name = string.Empty;
            create_user_name = string.Empty;
            element_type = 0;
        }
    }
}
