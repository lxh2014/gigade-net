using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class WebContentTypeSetup:PageBase
    {
        public int content_id { set; get; }
        public string web_content_type { set; get; }
        public int site_id { set; get; }
        public int page_id { set; get; }
        public int area_id { set; get; }
        public int type_id { set; get; }
        public string default_link_url { set; get; }
        public string area_name { set; get; }
        public int content_default_num { get; set; }
        public int content_status_num { get; set; }
        public WebContentTypeSetup()
        {
            content_id = 0;
            web_content_type = String.Empty;
            site_id = 0;
            page_id = 0;
            area_id = 0;
            type_id = 0;
            default_link_url = String.Empty;
            area_name = String.Empty;
            content_default_num = 0;
            content_status_num = 0;
        }
    }
}
