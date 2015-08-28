using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SitePage:PageBase
    {
        public int page_id { get; set; }
        //public int site_id { get; set; }
        public string page_name { get; set; }
        public string page_url { get; set; }
        public int page_status { get; set; }
        public string page_html { get; set; }
        public string page_desc { get; set; }
        public DateTime page_createdate { get; set; }
        public DateTime page_updatedate { get; set; }
        public int create_userid { get; set; }
        public int update_userid { get; set; }
       

        public SitePage()
        {
            page_id = 0;
            //site_id =0;
            page_name = string.Empty;
            page_url = string.Empty;
            page_status =0;
            page_html = string.Empty;
            page_desc = string.Empty;
            page_createdate = DateTime.MinValue;
            page_updatedate = DateTime.MinValue;
            create_userid = 0;
            update_userid = 0;
         
        }
    }

}
