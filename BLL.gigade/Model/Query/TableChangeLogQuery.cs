using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class TableChangeLogQuery : TableChangeLog
    {
        public int key_type { get; set; }
        public string key { get; set; }//用於批次查詢
        public int d_type { get; set; }
        public DateTime date_one { get; set; }
        public DateTime date_two { get; set; }
    
        public TableChangeLogQuery()
    {
            key_type = 0;
            key = string.Empty;
            d_type = 0;
            date_one = DateTime.MinValue;
            date_two = DateTime.MinValue;
          

        }

    }
}
