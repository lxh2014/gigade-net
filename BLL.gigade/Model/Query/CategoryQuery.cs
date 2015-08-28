using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class CategoryQuery : ProductCategory
    {
        public string amo { get; set; }
        public string sum { get; set; }
        public int serchs { get; set; }
        public int brand_status { get; set; }
        public int starttime { get; set; }
        public int endtime { get; set; }
        public int seldate { get; set; }
        public string category_father_name { get; set; }
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public CategoryQuery()
        {
            amo = String.Empty;
            sum = String.Empty;
            serchs = 0;
            brand_status = 0;
            starttime = 0;
            endtime = 0;
            seldate = 0;
            category_father_name = string.Empty;
        }
    }
}
