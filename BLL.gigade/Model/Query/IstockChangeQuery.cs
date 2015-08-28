using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IstockChangeQuery : IstockChange
    {
        public string typename{get;set;}
        public string istockwhy { get; set; }
        public string manager { get; set; }
        public string product_name { get; set; }
        /// <summary>
        /// 條碼
        /// </summary>
        public string upc_id { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public string specname { get; set; }
        public int item_stock { get; set; }
        public IstockChangeQuery()
        {
            typename = string.Empty;
            istockwhy = string.Empty;
            manager = string.Empty;
            product_name = string.Empty;
            upc_id = string.Empty;
            starttime = DateTime.MinValue;
            endtime = DateTime.Now;
            specname = string.Empty;
            item_stock = 0;
        }
    }
}
