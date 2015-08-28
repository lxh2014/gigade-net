using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IialgQuery:iialg
    {

        public string made_dttostr{get;set;}
        public string cde_dttostr{get;set;}
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public string loc_R { get; set; }
        public string product_name { get; set; }
        public string prod_sz { get; set; }
        public int qty { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int pnum { get; set; }//RF-撿貨量
        public string order_id { set; get; }
        public IialgQuery()
        {
            made_dttostr = string.Empty;
            cde_dttostr = string.Empty;
            starttime = DateTime.MinValue;
            endtime = DateTime.MinValue;
            order_id = string.Empty;
        }
    }
}
