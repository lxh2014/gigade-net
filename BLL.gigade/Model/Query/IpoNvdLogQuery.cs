using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IpoNvdLogQuery:IpoNvdLog
    {
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public string upc_id { get; set; }
        public string create_user_string { get; set; }
        public string loc_id { get; set; }
        public string pwy_dte_ctl { get; set; }

        public IpoNvdLogQuery()
        {
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
            upc_id = string.Empty;
            create_user_string = string.Empty;
            loc_id = string.Empty;
            pwy_dte_ctl = string.Empty;
        }
    }
}
