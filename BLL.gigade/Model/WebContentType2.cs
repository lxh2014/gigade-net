using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class WebContentType2 : PageBase
    {
        public int content_id { get; set; }
        public int site_id { get; set; }
        public int page_id { get; set; }
        public int area_id { get; set; }
        public int type_id { get; set; }
        public string content_title { get; set; }
        public string content_image { get; set; }
        public string home_title { get; set; }
        public string home_text { get; set; }
        public int product_id { get; set; }
        public int content_default { get; set; }
        public int content_status { get; set; }
        public string link_url { get; set; }
        public string link_page { get; set; }
        public int link_mode { get; set; }
        public DateTime start_time { set; get; }//上架時間 add by shuangshuang0420j 2014.11.26 11：04
        public DateTime end_time { set; get; }//下架時間 add by shuangshuang0420j 2014.11.26 11：04
        public DateTime update_on { get; set; }
        public DateTime created_on { get; set; }

        public WebContentType2()
        {
            content_id = 0;
            site_id = 7;
            page_id = 0;
            area_id = 0;
            type_id = 0;
            content_title = string.Empty;
            content_image = string.Empty;
            home_title = string.Empty;
            home_text = string.Empty;
            product_id = 0;
            content_default = 0;
            content_status = 1;
            link_url = string.Empty;
            link_page = string.Empty;
            link_mode = 1;
            update_on = DateTime.MinValue;
            created_on = DateTime.MinValue;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
        }
    }
}
