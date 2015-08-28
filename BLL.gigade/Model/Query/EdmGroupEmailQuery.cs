using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class EdmGroupEmailQuery:EdmGroupEmail
    {
        public string email_address { get; set; }
        public DateTime email_createdate_tostring { get; set; }
        public DateTime email_updatedate_tostring { get; set; }
        public string email_ids { get; set; }//刪除時批量刪除使用
        public string group_name { get; set; }

        public string selectType { get; set; }   //查詢條件
        public string search_con { get; set; }   //查詢關鍵字
        public EdmGroupEmailQuery()
        {
            email_address = string.Empty;
            email_createdate_tostring = DateTime.MinValue;
            email_updatedate_tostring = DateTime.MinValue;
            email_ids = string.Empty;
            group_name = string.Empty;

            selectType = string.Empty;
            search_con = string.Empty;
        }
    }
}
