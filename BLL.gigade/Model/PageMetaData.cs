using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PageMetaData : PageBase
    {
        public int pm_id { get; set; }
        public string pm_url_para { get; set; }
        public string pm_page_name { get; set; }
        public string pm_title { get; set; }
        public string pm_keywords { get; set; }
        public string pm_description { get; set; }
        public DateTime pm_created { get; set; }
        public DateTime pm_modified { get; set; }
        public int pm_modify_user { get; set; }
        public int pm_create_user { get; set; }

        public PageMetaData()
        {
            pm_id = 0;
            pm_url_para = string.Empty;
            pm_page_name = string.Empty;
            pm_title = string.Empty;
            pm_keywords = string.Empty;
            pm_description = string.Empty;
            pm_created = DateTime.MinValue;
            pm_modified = DateTime.MinValue;
            pm_modify_user = 0;
            pm_create_user = 0;
        }
    }
}
