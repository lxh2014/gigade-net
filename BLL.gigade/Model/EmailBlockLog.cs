using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EmailBlockLog:PageBase
    {
        public int block_id { get; set; }
        public string email_address { get; set; }
        public DateTime block_start { get; set; }
        public DateTime block_end { get; set; }
        public string block_reason { get; set; }
        public string unblock_reason { get; set; }
        public int block_create_userid { get; set; }
        public int unblock_create_userid { get; set; }
        public EmailBlockLog() 
        {
            block_id = 0;
            email_address = string.Empty;
            block_start = DateTime.MinValue;
            block_end = DateTime.MinValue;
            block_reason = string.Empty;
            unblock_reason = string.Empty;
            block_create_userid = 0;
            unblock_create_userid = 0;

        }
    }
}