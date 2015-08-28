using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
   public  class Vgroup : PageBase
    {
         public int rowid { get; set; }
        public string groupName { get; set; }
        public string groupCode { get; set; }
        public Int64 callid { get; set; }
        public string remark { get; set; }
        public string kuser { get; set; }
        public DateTime kdate { get; set; }

        public Vgroup()
        {
            rowid = 0;
            groupName = string.Empty;
            groupCode = string.Empty;
            callid = 0;
            remark = string.Empty;
            kuser = string.Empty;
            kdate = DateTime.MinValue;
        }
    }
}
