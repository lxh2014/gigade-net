using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class BannerSite : PageBase
    {
        public uint banner_site_id { get; set; }
        public uint banner_site_sort { get; set; }
        public uint banner_site_status { get; set; }
        public string banner_site_name { get; set; }
        public string banner_site_description { get; set; }
        public uint banner_site_createdate { get; set; }
        public uint banner_site_updatedate { get; set; }
        public string banner_site_ipfrom { get; set; }

        public BannerSite()
        {
            banner_site_id = 0;
            banner_site_sort = 0;
            banner_site_status = 0;
            banner_site_name = string.Empty;
            banner_site_description = string.Empty;
            banner_site_createdate = 0;
            banner_site_updatedate = 0;
            banner_site_ipfrom = string.Empty;
        }

    }
}
