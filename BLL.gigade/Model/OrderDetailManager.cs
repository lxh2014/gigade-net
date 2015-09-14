using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderDetailManager:PageBase
    {
        public int odm_id { get; set; }
        public uint odm_user_id { get; set; }
        public string odm_user_name { get; set; }
        public int odm_status { get; set; }
        public DateTime odm_createdate { get; set; }
        public int odm_createuser { get; set; }

        public OrderDetailManager()
        {
            odm_id = 0;
            odm_user_id = 0;
            odm_user_name = string.Empty;
            odm_status = 0;
            odm_createdate = DateTime.MinValue;
            odm_createuser = 0;
        }
    }
}
