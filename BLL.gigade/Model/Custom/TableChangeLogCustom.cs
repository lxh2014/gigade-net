using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class TableChangeLogCustom
    {
        public int vendor_id { get; set; }
        public string vendor_name_full { get; set; }
        public int pk_id { get; set; }
        public string change_table { get; set; }
        public string change_table_function { get; set; }
        public List<TableChangeLog> tclModel { get; set; }

        public TableChangeLogCustom()
        { 
            vendor_id = 0;
            vendor_name_full = string.Empty;
            pk_id = 0;
            change_table = string.Empty; 
            change_table_function = string.Empty; 
            tclModel = new List<TableChangeLog>();
        }
    }
}
