using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class DesignRequest:PageBase
    {
        public uint dr_id { get; set; }
        public uint product_id { get; set; }
        public int dr_requester_id { get; set; }
        public int dr_type { get; set; }
        public int dr_assign_to { get; set; }
        public string dr_content_text { get; set; }
        public string dr_description { get; set; }
        public string dr_resource_path { get; set; }
        public string dr_document_path { get; set; }
        public int dr_status { get; set; }
        public DateTime dr_created { get; set; }
        public DateTime dr_modified { get; set; }
        public DateTime dr_expected { get; set; }

        public DesignRequest()
        {
            dr_id = 0;
            product_id = 0;
            dr_requester_id = 0;
            dr_type = 0;
            dr_assign_to = 0;
            dr_content_text =string.Empty;
            dr_description = string.Empty;
            dr_resource_path = string.Empty;
            dr_document_path = string.Empty;
            dr_status = 0;
            dr_created = DateTime.MinValue;
            dr_modified = DateTime.MinValue;
            dr_expected = DateTime.MinValue;
        }
    }
}
