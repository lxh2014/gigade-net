using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class BannerNewsContent : PageBase
    {
        public uint news_id { get; set; }
        public uint news_site_id { get; set; }
        public string news_title { get; set; }
        public string news_content { get; set; }
        public string news_link_url { get; set; }
        public int news_link_mode { get; set; }
        public uint news_sort { get; set; }
        public uint news_status { get; set; }
        public uint news_start { get; set; }
        public uint news_end { get; set; }
        public uint news_createdate { get; set; }
        public uint news_updatedate { get; set; }
        public string news_ipfrom { get; set; }
        public BannerNewsContent()
        {
            news_id = 0;
            news_site_id = 0;
            news_title = string.Empty;
            news_content = string.Empty;
            news_link_url = string.Empty;
            news_link_mode = 0;
            news_sort = 0;
            news_status = 0;
            news_start = 0;
            news_end = 0;
            news_createdate = 0;
            news_updatedate = 0;
            news_ipfrom = string.Empty;
        
        }
    }
}
