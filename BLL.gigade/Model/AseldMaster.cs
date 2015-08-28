using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class AseldMaster:PageBase
    {
        public int row_id { get; set; }
        public string assg_id { get; set; }
        public DateTime complete_time { get; set; }
        public DateTime start_time { get; set; }
        public DateTime create_time { get; set; }
        public int create_user { get; set; }
        public DateTime update_time { get; set; }
        public int update_user { get; set; }

        public AseldMaster()
        {
            assg_id = string.Empty;
            complete_time = DateTime.MinValue;
            start_time = DateTime.MinValue;
            create_time = DateTime.Now;
            update_time = DateTime.Now;

        }
    }
}
