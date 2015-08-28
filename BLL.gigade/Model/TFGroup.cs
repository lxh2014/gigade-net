using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public  class TFGroup
    {
        public int rowid { set; get; }
        public string groupName { set; get; }
        public string groupCode {set;get;}
        public string remark { set; get; }
        public string kuser { set; get; }
        public DateTime kdate { set; get; }
        public TFGroup()
        {
            rowid = 0;
            groupName = string.Empty;
            groupCode = string.Empty;
            remark = string.Empty;
            kuser = string.Empty;
            kdate = DateTime.Now;
        }
    }
}
