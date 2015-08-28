using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderQuestionQuery:OrderQuestion
    {
        public DateTime question_createdates { get; set; }
        public DateTime response_createdates { get; set; }
        public uint response_id { get; set; }
        public string response_content { get; set; }
        public uint response_createdate { get; set; }
        //2015/02/06 訂單問題列表頁添加
        public string question_type_name { get; set; }
        public string question_status_name { get; set; }
        //ddlSel
        public int ddlSel { get; set; }
        public string selcontent { get; set; }
        public int ddtSel { get; set; }
        public uint time_start { get; set; }
        public uint time_end { get; set; }
        public string ddlstatus { get; set; }

        public string question_type_str { get; set; }

    }
}
