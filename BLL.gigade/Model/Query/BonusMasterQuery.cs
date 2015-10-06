using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class BonusMasterQuery : BonusMaster
    {
        public string user_name { get; set; }
        public string type_admin_link { get; set; }
        public string type_description { get; set; }
        public int search_total { get; set; }
        public int now_time { get; set; }
        public DateTime smaster_start { get; set; }
        public DateTime smaster_end { get; set; }
        public DateTime smaster_createtime { get; set; }
        public DateTime smaster_updatedate { get; set; }
        public string password { get; set; }
        public int master_type { get; set; }
        public string writer { get; set; }
        public int master_status { get; set; }

        public int record_id { get; set; }
        public string masterid { get; set; }
        public string usebonus { get; set; }

        public bool use { get; set; }  //尚未開通
        public bool useing { get; set; }  //未使用
        public bool used { get; set; }  // 已過期 
        public bool useings { get; set; }  //尚餘點數
        public bool useds { get; set; }  // 已用完 
        public string user_email { get; set; }//會員郵箱
        public BonusMasterQuery()
        {
            user_name = string.Empty;
            type_admin_link = string.Empty;
            type_description = string.Empty;
            search_total = 0;
            now_time = 0;
            smaster_start = DateTime.MinValue;
            smaster_end = DateTime.MinValue;
            smaster_createtime = DateTime.MinValue;
            smaster_updatedate = DateTime.MinValue;
            password = string.Empty;
            master_type = 0;
            writer = string.Empty;
            master_status = 0;
            use = false;
            useing = false;
            used = false;
            useings = false;
            useds = false;
            user_email = string.Empty;
        }
    }
}
