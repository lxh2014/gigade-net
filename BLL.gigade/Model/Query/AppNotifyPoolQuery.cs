using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class AppNotifyPoolQuery : AppNotifyPool
    {
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public DateTime notifytime { get; set; }
        public string isAddOrEidt { get; set; }
        public int startendtime { get; set; }
        public int endendtime { get; set; }

        public AppNotifyPoolQuery()
        {
            starttime = DateTime.Now;
            endtime = DateTime.Now;
            notifytime = DateTime.Now;
            isAddOrEidt = string.Empty;
            startendtime = 0;
            endendtime = 0;
        }
    }
}
