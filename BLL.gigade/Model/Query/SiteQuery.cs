using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public  class SiteQuery:SiteModel
    {
        //cart_delivery對應顯示的站臺名稱
        public string csitename { get; set; }
        public SiteQuery()
        {
            csitename = string.Empty;
        }

    }
}
