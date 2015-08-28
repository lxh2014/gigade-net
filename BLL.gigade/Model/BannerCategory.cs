using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class BannerCategory : PageBase
    {
        public int category_id { get; set; }
        public int category_father_id { get; set; }
        public int category_sort { get; set; }
        public string category_name { get; set; }
        public string content_type { get; set; }
        public int content_id { get; set; }
        public string description { get; set; }
        public int activity { get; set; }
        public DateTime created_on { get; set; }
        public DateTime updated_on { get; set; }

        public string fcategory_name { get; set; }
        public string banner_site_name { get; set; }
        public BannerCategory()
        {
            category_id = 0;
            category_father_id = 0;
            category_sort = 0;
            category_name = string.Empty;
            content_type = string.Empty;
            content_id = 0;
            description = string.Empty;
            activity = 0;
            created_on = DateTime.MinValue;
            updated_on = DateTime.MinValue;
            fcategory_name = string.Empty;
            banner_site_name = string.Empty;
        }
    }
}
