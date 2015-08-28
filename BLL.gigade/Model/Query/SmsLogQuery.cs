using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class SmsLogQuery:SmsLog
    {
        public bool sucess_status { set; get; }
        public SmsLogQuery()
        {
            sucess_status = false;
;
        }
    }
}
