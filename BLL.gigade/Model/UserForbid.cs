using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    [Serializable]
    public class UserForbid : PageBase
    {
        public int forbid_id { get; set; }
        public string forbid_ip { get; set; }
        public int forbid_createdate { get; set; }
        public int forbid_createuser { get; set; }
        public UserForbid()
        {
            forbid_id = 0;
            forbid_ip = string.Empty;
            forbid_createdate = 0;
            forbid_createuser = 0;
        }
    }
}
