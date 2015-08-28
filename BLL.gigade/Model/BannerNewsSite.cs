using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class BannerNewsSite:PageBase
    {
        public uint news_site_id { get; set; }
        public uint news_site_sort { get; set; }
        public uint news_site_status { get; set; }
        public uint news_site_mode { get; set; }
        public string news_site_name { get; set; }
        public string news_site_description { get; set; }
        public uint news_site_createdate { get; set; }
        public uint news_site_updatedate { get; set; }
        public string news_site_ipfrom { get; set; }

        public BannerNewsSite()
        { 
        
        }
    }
}
