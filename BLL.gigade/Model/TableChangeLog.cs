using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class TableChangeLog : PageBase
    {
        public int row_id { get; set; }
        public int user_type { get; set; }
        public int pk_id { get; set; }
        public string change_table { get; set; }
        public string change_field { get; set; }
        public string old_value { get; set; }
        public string new_value { get; set; }
        public int create_user { get; set; }
        public DateTime create_time { get; set; }

        public string field_ch_name { get; set; }
        
        public TableChangeLog() 
        {
            row_id = 0; 
            user_type = 0;
            pk_id = 0;
            change_table = string.Empty;
            change_field = string.Empty;
            old_value = string.Empty;
            new_value = string.Empty;
            create_user = 0;
            create_time = DateTime.MinValue;
            field_ch_name = string.Empty;

        }
    }
}
