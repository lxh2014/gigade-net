using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Model.Query
{
    public class ProductClickQuery : ProductClick
    {
        public uint brand_id { get; set; }
        public uint product_status { get; set; }
        public uint prod_classify { get; set; }
        public uint sclick_id { get; set; }
        public uint eclick_id { get; set; }
        public int sclick_year { get; set; }
        public int eclick_year { get; set; }
        public int sclick_month { get; set; }
        public int eclick_month { get; set; }
        public int sclick_day { get; set; }
        public int eclick_day { get; set; }
        public string pids { get; set; }
        public ProductClickQuery()
        {
            brand_id = 0;
            product_status = 999;
            prod_classify = 0;
            sclick_id = 0;
            eclick_id = 0;
            sclick_year = 0;
            eclick_year = 0;
            sclick_month = 0;
            eclick_month = 0;
            sclick_day = 0;
            eclick_day = 0;
            pids = string.Empty;
        }
    }
}
