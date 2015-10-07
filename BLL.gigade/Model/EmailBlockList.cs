using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EmailBlockList : PageBase
    {
        public string email_address { get; set; }
        public string block_reason { get; set; }
        public DateTime block_createdate { get; set; }
        public DateTime block_updatedate { get; set; }
        public int block_create_userid { get; set; }
        public int block_update_userid { get; set; }
        public EmailBlockList()
        {
            email_address = string.Empty;
            block_reason = block_reason;
            block_createdate = DateTime.Now;
            block_updatedate = DateTime.Now;
            block_create_userid = 0;
            block_update_userid = 0;
        }
    }
}
