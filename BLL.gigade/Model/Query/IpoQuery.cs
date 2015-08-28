using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IpoQuery : Ipo
    {
        public string vendor_name_simple { set; get; }
        public string row_ids { set; get; }
        public string user_username { set; get; }
        public IpoQuery() 
        {
            vendor_name_simple = string.Empty;
            row_ids = string.Empty;
            user_username = string.Empty;
        }
    }
}
