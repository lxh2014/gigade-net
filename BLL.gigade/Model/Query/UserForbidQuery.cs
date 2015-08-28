using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class UserForbidQuery : UserForbid
    {
        public DateTime create_time { get; set; }
        public string user_username { get; set; }
        public string rowIds { get; set; }
        public string timestart { get; set; }
        public string timeend { get; set; }
        public UserForbidQuery()
        {
            create_time = DateTime.MinValue;
            user_username = string.Empty;
            rowIds = string.Empty;
            timestart = string.Empty;
            timeend = string.Empty;
        }
    }
}
