using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class WebContentType5 : PageBase
    {
        public int content_id { set; get; }
        public int site_id { set; get; }
        public int page_id { set; get; }
        public int area_id { set; get; }
        public int brand_id { set; get; }
        public int type_id { set; get; }

        public string content_title { set; get; }
        public string content_image { set; get; }

        public int content_default { set; get; }
        public int content_status { set; get; }
        public string link_url { set; get; }
        public int link_mode { set; get; }
        public DateTime update_on { set; get; }
        public DateTime created_on { set; get; }

        public WebContentType5()
        {
            content_id = 0;
            site_id = 7;
            page_id = 0;
            area_id = 0;
            type_id = 0;
            brand_id = 0;

            content_title = String.Empty;
            content_image = String.Empty;

            content_default = 0;
            content_status = 1;
            link_url = String.Empty;
            link_mode = 1;
            update_on = DateTime.Now;
            created_on = DateTime.Now;

        }
    }
}
