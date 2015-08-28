using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CbjobMaster:PageBase
    {
        public int row_id { get; set; }
        public string cbjob_id { get; set; }
        public DateTime create_datetime { get; set; }
        public int create_user { get; set; }
        public int status { get; set; }
        public string sta_id { get; set; }

        public CbjobMaster()
        {
            row_id = 0;
            cbjob_id = string.Empty;
            create_datetime = DateTime.MinValue;
            create_user = 0;
            status = 1;
            sta_id = "CNT";
        }
    }
}
