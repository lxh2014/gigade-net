using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class WebContentType1:PageBase
    {
        public int content_id { get; set; }
        public int site_id { get; set; }
        public int page_id { get; set; }
        public int area_id { get; set; }
        public int type_id { get; set; }
        public string content_title { get; set; }
        public string content_image { get; set; }
        public int content_default { get; set; }
        public int content_status { get; set; }
        public string link_url { get; set; }
        public string link_page { get; set; }
        public int link_mode { get; set; }
        public DateTime update_on { get; set; }
        public DateTime created_on { get; set; }

        public WebContentType1()
        {
            content_id = 0;
            site_id = 7;
            page_id = 0;
            area_id = 0;
            type_id = 0;
            content_title = string.Empty;
            content_image = string.Empty;
            content_default = 0;
            content_status = 1;
            link_url = string.Empty;
            link_page = string.Empty;
            link_mode = 1;
            update_on = DateTime.MinValue;
            created_on = DateTime.MinValue;
        }
    }
}
