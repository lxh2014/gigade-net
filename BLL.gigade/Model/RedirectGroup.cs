using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class RedirectGroup : PageBase
    {

        public uint group_id { get; set; }
        public string group_name { get; set; }
        public uint group_createdate { get; set; }
        public uint group_updatedate { get; set; }
        public string group_ipfrom { get; set; }
        public string group_type { get; set; }

        public RedirectGroup()
        {
            group_id = 0;
            group_name = string.Empty;
            group_createdate = 0;
            group_updatedate = 0;
            group_ipfrom = string.Empty;
            group_type = string.Empty;

        }
    }
}
