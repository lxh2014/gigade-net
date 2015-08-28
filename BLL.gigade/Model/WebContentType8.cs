using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class WebContentType8 : PageBase
    {
        public int content_id { set; get; }
        public int site_id { set; get; }
        public int page_id { set; get; }
        public int area_id { set; get; }
        public int type_id { set; get; }
        public string home_title { set; get; }
        /// <summary>
        /// 大標
        /// </summary>
        public string big_title { get; set; }
        /// <summary>
        /// 小標
        /// </summary>
        public string small_title { get; set; }
        public string content_title { set; get; }
        public string content_html { set; get; }
        public string home_image { set; get; }
        public int content_default { set; get; }
        public int content_status { set; get; }
        public string link_url { set; get; }
        public int link_mode { set; get; }
        public DateTime update_on { set; get; }
        public DateTime created_on { set; get; }
        public int sort { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }

        public WebContentType8()
        {
            content_id = 0;
            site_id = 7;
            page_id = 0;
            area_id = 0;
            type_id = 0;
            home_title = String.Empty;
            big_title = string.Empty;
            small_title = string.Empty;
            content_title = String.Empty;
            content_html = String.Empty;
            home_image = String.Empty;
            content_default = 0;
            content_status = 1;
            link_url = String.Empty;
            link_mode = 1;
            update_on = DateTime.Now;
            created_on = DateTime.Now;
            sort = 0;
            start_time = DateTime.Now;
            end_time = DateTime.Now;
        }
    }
}