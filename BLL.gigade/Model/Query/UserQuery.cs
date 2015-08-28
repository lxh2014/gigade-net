using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class UserQuery : Model.Custom.Users
    {
        public string file_name { get; set; }
        public string content { get; set; }
        public string ip { get; set; }
        public string kuser_name { get; set; }
        public uint kuser_id { get; set; }
        public DateTime created { get; set; }
        public uint date_start { get; set; }//用於檢索註冊日期的起始時間user_reg_date
        public uint date_end { get; set; }//用於檢索註冊日期的終止時間user_reg_date
        public int is_select_status { get; set; }//用於是否檢索user_status，0表否，1表是
        public UserQuery()
        {
            file_name = "";
            content = "";
            ip = "";
            kuser_name = "";
            kuser_id = 0;
            created = DateTime.MinValue;
            date_start = 0;
            date_end = 0;
            is_select_status = 0;
        }
    }
}
