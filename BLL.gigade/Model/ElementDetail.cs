using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Attributes;

namespace BLL.gigade.Model
{
    [DBTableInfo("element_detail")]
    public class ElementDetail : PageBase
    {

        public int element_id { get; set; }
        //public int element_area_id { get; set; }
        public int element_type { get; set; }
        //public string element_image { get; set; }
        public string element_content { get; set; }
        public uint product_id { get; set; }
        public string element_name { get; set; }
        public string element_link_url { get; set; }
        public int element_link_mode { get; set; }
        public int element_sort { get; set; }
        public int element_status { get; set; }
        public DateTime element_start { get; set; }
        public DateTime element_end { get; set; }
        public DateTime element_createdate { get; set; }
        public DateTime element_updatedate { get; set; }
        public int create_userid { get; set; }
        public int update_userid { get; set; }
        public string element_remark { get; set; }
        public int packet_id { get; set; }
        public int packet_status { get; set; }
        public uint category_id { get; set; }
        public string category_name { get; set; }
        public ElementDetail()
        {
            element_id = 0;
            //element_area_id = 0;
            element_type = 0;
            element_content = string.Empty;
            element_name = string.Empty;
            element_link_url = string.Empty;
            element_link_mode = 1;
            element_sort = 0;
            element_status = 1;
            element_start = DateTime.MinValue;
            element_end = DateTime.MinValue;
            element_createdate = DateTime.MinValue;
            element_updatedate = DateTime.MinValue;
            create_userid = 0;
            update_userid = 0;
            element_remark = string.Empty;
            packet_id = 0;
            packet_status = 0;
            category_id = 0;
            category_name = string.Empty ;
        }
    }
}