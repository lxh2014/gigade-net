using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class GroupAuthMap:PageBase
    {
        public int content_id { set; get; }
        public int group_id { set; get; }
        public string table_name { set; get; }
        public string column_name { set; get; }
        public string value { set; get; }
        public int status { set; get; }
        public string create_user_id { set; get; }
        public DateTime create_date { set; get; }
        public string table_alias_name { set; get; }

        public GroupAuthMap()
        {
            group_id = 0;
            table_name = String.Empty;
            column_name = String.Empty;
            value = String.Empty; 
            status = 0;
            create_user_id = string.Empty;
            create_date = DateTime.Now;
            table_alias_name = string.Empty;
        }


    }
}
