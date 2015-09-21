using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VipUserGroupQuery:VipUserGroup
    {

        public string screatedate { get; set; }
        public string list { get; set; }
        public string ip { get; set; }
        public uint create_dateOne { get; set; }
        public uint create_dateTwo { get; set; }
        public string group_committe_other { get; set; }
        public string tax_name { get; set; }
        public string create_user { get; set; }
        public string update_user { get; set; }
        public bool isSecret;

        public string group_id_or_group_name { get; set; }

        public int create_id { get; set; }
        public int update_id { get; set; }
        
        public VipUserGroupQuery()
        {
            screatedate =string.Empty;
            list = string.Empty;
            ip = string.Empty;
            create_dateOne = 0;
            create_dateTwo = 0;
            tax_name = string.Empty;
            group_committe_other = string.Empty;
            isSecret = true;
            create_id = 0;
            update_id = 0;

            group_id_or_group_name = string.Empty;
        }
    }
}
