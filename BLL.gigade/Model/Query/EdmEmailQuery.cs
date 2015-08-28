using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EdmEmailQuery : EdmEmail
    {
        public int group_count { get; set; }
        public int group_id { get; set; }
        public string group_name { get; set; }
        public int email_status { get; set; }
        public EdmEmailQuery()
        {
            group_count = 0;
            group_id = 0;
            group_name = string.Empty;
            email_status = 0;
        }
    }
}
