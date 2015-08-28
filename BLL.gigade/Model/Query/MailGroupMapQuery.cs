using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class MailGroupMapQuery : MailGroupMap
    {
        public string user_mail { get; set; }
        public string group_name { get; set; }
        public MailGroupMapQuery()
        {
            user_mail = string.Empty;
            group_name = string.Empty;
        }
    }
}
