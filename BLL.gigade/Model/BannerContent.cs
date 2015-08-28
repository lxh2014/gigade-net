using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class BannerContent : PageBase
    {
        public uint banner_content_id { get; set; }
        public uint banner_site_id { get; set; }
        public string banner_title { get; set; }
        public string banner_link_url { get; set; }
        public int banner_link_mode { get; set; }
        public uint banner_sort { get; set; }
        public uint banner_status { get; set; }
        public string banner_image { get; set; }
        public DateTime banner_start { get; set; }
        public DateTime banner_end { get; set; }
        public DateTime banner_createdate { get; set; }
        public DateTime banner_updatedate { get; set; }
        public string banner_ipfrom { get; set; }

        public BannerContent()
        {
            banner_content_id = 0;
            banner_site_id = 0;
            banner_title = string.Empty;
            banner_link_url = string.Empty;
            banner_link_mode = 0;
            banner_sort = 0;
            banner_status = 0;
            banner_image = string.Empty;
            banner_start =Common.CommonFunction.GetNetTime(0);
            banner_end = Common.CommonFunction.GetNetTime(0); ;
            banner_createdate = Common.CommonFunction.GetNetTime(0); 
            banner_updatedate = Common.CommonFunction.GetNetTime(0); 
            banner_ipfrom = string.Empty;
        }
    }
}
