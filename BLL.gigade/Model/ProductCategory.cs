using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductCategory : PageBase
    {
        public uint category_id { get; set; }
        public uint category_father_id { get; set; }
        public string category_name { get; set; }
        public uint category_sort { get; set; }
        public uint category_display { get; set; }
        public uint category_show_mode { get; set; }
        public string category_image_in { get; set; }
        public string category_image_out { get; set; }
        public uint category_link_mode { get; set; }
        public string category_link_url { get; set; }
        public string banner_image { get; set; }
        public uint banner_status { get; set; }
        public uint banner_link_mode { get; set; }
        public string banner_link_url { get; set; }
        public uint banner_show_start { get; set; }
        public uint banner_show_end { get; set; }
        public uint category_createdate { get; set; }
        public uint category_updatedate { get; set; }
        public string category_ipfrom { get; set; }
        public int status { get; set; }
        public string short_description { get; set; }
        public ProductCategory()
        {
            category_id = 0;
            category_father_id = 0;
            category_name = string.Empty;
            category_sort = 0;
            category_display = 1;               //默認可用
            category_show_mode = 0;
            category_image_in = string.Empty;
            category_image_out = string.Empty;
            category_link_mode = 0;
            category_link_url = string.Empty;
            banner_image = string.Empty;
            banner_status = 0;
            banner_link_mode = 0;
            banner_link_url = string.Empty;
            banner_show_start = 0;
            banner_show_end = 0;
            category_createdate = 0;
            category_updatedate = 0;
            category_ipfrom = string.Empty;
            short_description = string.Empty;
            status = 0;
        }
    }
}
