using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProdVdReqQuery : ProdVdReq
    {
        public string vendor_name { set; get; }
        public DateTime time_start { set; get; }
        public DateTime time_end { set; get; }
        public int product_status { set; get; }
        public string statusName { set; get; }
        public uint brand_id { set; get; }
        public string brand_name { set; get; }

        public ProdVdReqQuery()
        {
            vendor_name = string.Empty;
            time_start = DateTime.MinValue;
            time_end = DateTime.MinValue;
            statusName = string.Empty;
            brand_id = 0;
            brand_name = string.Empty;
        }
    }
}
