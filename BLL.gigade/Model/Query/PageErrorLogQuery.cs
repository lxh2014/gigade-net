using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PageErrorLogQuery : PageErrorLog
    {
        public int searchType { get; set; }
        public string searchKey { get; set; }
        public string startT { get; set; }
        public string endT { get; set; }
        public string errorName { get; set; }
        public PageErrorLogQuery()
        {
            this.searchType = 0;
            this.searchKey = string.Empty;
            this.startT = string.Empty;
            this.endT = string.Empty;
            this.errorName = string.Empty;
        
        }

    }
}
