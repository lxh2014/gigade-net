using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Redirect : PageBase
    {
        public uint redirect_id { get; set; }
        public uint group_id { get; set; }
        public int user_group_id { get; set; }
        public string redirect_name { get; set; }
        public string redirect_url { get; set; }
        public uint redirect_status { get; set; }
        public uint redirect_total { get; set; }
        public string redirect_note { get; set; }
        public uint redirect_createdate { get; set; }
        public uint redirect_updatedate { get; set; }
        public string redirect_ipfrom { get; set; }

        public Redirect()
        {
            redirect_id = 0;
            group_id = 0;
            user_group_id = 0;
            redirect_name = string.Empty;
            redirect_url = string.Empty;
            redirect_status = 1;
            redirect_total = 0;
            redirect_note = string.Empty;
            redirect_createdate = 0;
            redirect_updatedate = 0;
            redirect_ipfrom = string.Empty;

        }

    }
}
