using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class EdmGroup : PageBase
    {

        public uint group_id { get; set; }
        public string group_name { get; set; }
        public uint group_total_email { get; set; }
        public uint status { get; set; }
        public uint group_createdate { get; set; }
        public uint group_updatedate { get; set; }

        public Int64 total_content { get; set; }
        public string s_group_createdate { get; set; }
        public string s_group_updatedate { get; set; }
        public string selectType { get; set; }   //查詢條件
        public string search_con { get; set; }   //查詢關鍵字
        public uint start { get; set; }//查詢開始日期
        public uint end { get; set; }//查詢結束日期
        public int dateCondition { get; set; }//查詢條件
      
        public EdmGroup()
        {
            group_id = 0;
            group_name = string.Empty;
            group_total_email = 0;
            status = 0;
            group_createdate = 0;
            group_updatedate = 0;
            total_content = 0;
            s_group_createdate = string.Empty;
            s_group_updatedate = string.Empty;

            selectType = string.Empty;
            search_con = string.Empty;
            start = 0;
            end = 0;
            dateCondition = -1;
        }
    }
}
