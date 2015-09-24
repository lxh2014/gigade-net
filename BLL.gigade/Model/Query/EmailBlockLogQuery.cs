using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EmailBlockLogQuery : EmailBlockLog
    {
        public string log_block_start { get; set; }
        public string log_block_end { get; set; }
        public EmailBlockLogQuery()
        {
            log_block_start = string.Empty;
            log_block_end = string.Empty;
        }
    }
}
