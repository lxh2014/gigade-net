using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EpaperContent : PageBase
    {
        public uint epaper_id { get; set; }
        public uint user_id { get; set; }
        public string epaper_title { get; set; }
        public string epaper_short_title { get; set; }
        public string epaper_content { get; set; }
        public uint epaper_sort { get; set; }
        public uint epaper_status { get; set; }
        public string epaper_size { get; set; }
        public uint epaper_show_start { get; set; }
        public uint epaper_show_end { get; set; }
        public string fb_description { get; set; }
        public uint epaper_createdate { get; set; }
        public uint epaper_updatedate { get; set; }
        public string epaper_ipfrom { get; set; }
        public uint type { get; set; }

        public EpaperContent()
        {
            epaper_id = 0;
            user_id = 0;
            epaper_title = string.Empty;
            epaper_short_title = string.Empty;
            epaper_content = string.Empty;
            epaper_sort = 0;
            epaper_status = 0;
            epaper_size = string.Empty;
            epaper_show_start = 0;
            epaper_show_end = 0;
            fb_description = string.Empty;
            epaper_createdate = 0;
            epaper_updatedate = 0;
            epaper_ipfrom = string.Empty;
            type = 0;
        }


    }
}
