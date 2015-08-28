using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class UserLife : PageBase
    {
        public int row_id { get; set; }
        public uint user_id { get; set; }
        public string info_type { get; set; }
        public string info_code { get; set; }
        public string info_name { get; set; }
        public string remark { get; set; }
        public uint kdate { get; set; }
        public int kuser { get; set; }

        public UserLife()
        {
            row_id = 0;
            user_id = 0;
            info_type = string.Empty;
            info_code = string.Empty;
            info_name = string.Empty;
            remark = string.Empty;
            kdate = 0;
            kuser = 0;

        }

    }
}
