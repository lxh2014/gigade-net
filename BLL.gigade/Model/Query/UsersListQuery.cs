using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class UsersListQuery : Custom.Users
    {
        public DateTime serchstart { get; set; }
        public DateTime serchend { get; set; }
        public string serchtype { get; set; }
        public string content { get; set; }
        public string types { get; set; }
        public string checks { get; set; }
        public string birthday { get; set; }
        public string mytype { get; set; }
        public DateTime sfirst_time { get; set; }
        public DateTime slast_time { get; set; }
        public DateTime sbe4_last_time { get; set; }
        public DateTime suser_reg_date { get; set; }
        public float s_id { get; set; }
        public string redirect_name { get; set; }
        public string redirect_url { get; set; }
        public string master_ipfrom { get; set; }
        public uint master_total { get; set; }
        public uint master_balance { get; set; }
        public string group_name { get; set; }
        public DateTime reg_date { get; set; }
        public string userLevel { get; set; }
        public int update_user { get; set; }
        public int bonus_type { get; set; }
        public int bonus_type1 { get; set; }
        public string bonus_typename { get; set; }
        public string bonus_typenamequan { get; set; }
        public UsersListQuery()
        {

            serchstart = DateTime.MinValue;
            serchend = DateTime.MinValue;
            serchtype = string.Empty;
            content = string.Empty;
            types = string.Empty;
            checks = string.Empty;
            birthday = string.Empty;
            mytype = string.Empty;
            sfirst_time = DateTime.MinValue;
            slast_time = DateTime.MinValue;
            sbe4_last_time = DateTime.MinValue;
            suser_reg_date = DateTime.MinValue;
            s_id = 0;
            redirect_name = string.Empty;
            redirect_url = string.Empty;
            master_ipfrom = string.Empty;
            group_name = string.Empty;
            master_total = 0;
            master_balance = 0;
            reg_date = DateTime.Now;
            userLevel = string.Empty;
            update_user = 0;
            bonus_type = 0;
            bonus_type1 = 0;
            bonus_typename = string.Empty;
            bonus_typenamequan = string.Empty;
        }
    }
}
