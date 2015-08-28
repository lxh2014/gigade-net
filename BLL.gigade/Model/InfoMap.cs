using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class InfoMap : PageBase
    {
        public int map_id { get; set; }
        public int site_id { get; set; }
        public int page_id { get; set; }
        public int area_id { get; set; }
        public int info_id { get; set; }
        public int type { get; set; }
        public int sort { get; set; }
        public DateTime create_date { get; set; }
        public int create_user_id { get; set; }
        public DateTime update_date { get; set; }
        public int update_user_id { get; set; }
        public InfoMap()
        {
            map_id = 0;
            site_id = 0;
            page_id = 0;
            area_id = 0;
            info_id = 0;
            type = 0;
            sort = 0;
            create_date = DateTime.Now;
            update_date = DateTime.Now;
            create_user_id = 0;
            update_user_id = 0;
        }
    }
}
