using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class TableHistoryCustom:TableHistory
    {
        public string kuser { get; set; }
        public DateTime kdate { get; set; }
        public string user_username { get; set; }

        public TableHistoryCustom()
        {
            user_username = string.Empty;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
        }
    }
}
