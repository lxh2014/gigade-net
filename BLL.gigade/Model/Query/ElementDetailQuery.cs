using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ElementDetailQuery : ElementDetail
    {
        public string key { set; get; }
        public string element_type_name { set; get; }
        public string element_linkmode { set; get; }
        public string product_name { get; set; }
        public string packet_name { get; set; }
        public string kendo_editor { get; set; }
        public string searchcate { get; set; }
        public uint product_status { get; set; }
        public ElementDetailQuery()
        {
            key = string.Empty;
            element_type_name = string.Empty;
            element_linkmode = string.Empty;
            product_name = string.Empty;
            packet_name = string.Empty;
            kendo_editor = string.Empty;
            searchcate = string.Empty;
            product_status = 0;
        }
    }
}
