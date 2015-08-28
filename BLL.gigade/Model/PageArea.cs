using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public class PageArea:PageBase
    {
       public int area_id { get; set; }
       //public int page_id { get; set; }
       public string area_name { get; set; }
       public string area_desc { get; set; }
       public string area_element_id { get; set; }
       public int area_status { get; set; }
       public DateTime area_createdate { get; set; }
       public DateTime area_updatedate { get; set; }
       public int create_userid { get; set; }
       public int update_userid { get; set; }
       public int show_number { get; set; }
       public int element_type { get; set; }
       public PageArea()
       {
           area_id = 0;
           //page_id = 0;
           area_name = string.Empty;
           area_desc = string.Empty;
           area_element_id = string.Empty;
           area_status = 0;
           area_createdate = DateTime.MinValue;
           area_updatedate = DateTime.MinValue;
           create_userid = 0;
           update_userid = 0;
           show_number = 0;
           element_type = 0;
       }
    }
}
