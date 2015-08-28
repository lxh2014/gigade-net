using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class AreaPacket:PageBase
    {
        public int packet_id { get; set; }
        public string packet_name { get; set; }
        public int show_number { get; set; }
        public int packet_sort { get; set; }
        public int element_type { get; set; }
        public int packet_status { get; set; }
        public string packet_desc { get; set; }
        public DateTime packet_createdate { get; set; }
        public DateTime packet_updatedate { get; set; }
        public int create_userid { get; set; }
        public int update_userid { get; set; }

        public AreaPacket()
        {
            packet_id = 0;
            packet_name = string.Empty;
            show_number = 0;
            packet_sort = 0;
            element_type = 0;
            packet_status = 1;
            packet_desc = string.Empty;
            packet_createdate = DateTime.MinValue;
            packet_updatedate = DateTime.MinValue;
            create_userid = 0;
            update_userid = 0;
        }
    }
}
