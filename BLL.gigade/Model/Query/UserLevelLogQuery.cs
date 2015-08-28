using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class UserLevelLogQuery : UserLevelLog
    {
        public string user_name { get; set; }
        public string user_email { get; set; }
        public bool isSecret { get; set; }
        public string key { get; set; }
        public int searchStatus { get; set; }
        public int leveltype { get; set; }
        public string leveltypeid { get; set; }
        public UserLevelLogQuery()
        {
            user_name = string.Empty;
            user_email = string.Empty;
            isSecret = true;
            key = string.Empty;
            searchStatus = 0;
            leveltype = 0;//定義1表示老會員2 表示新會員
            leveltypeid = string.Empty;
        }
    }
}
