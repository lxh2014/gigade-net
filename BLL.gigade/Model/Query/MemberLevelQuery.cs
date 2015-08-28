using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class MemberLevelQuery : MemberLevel
    {
        public string create_user { get; set; }
        public string update_user { get; set; }
        public string code_name { get; set; }
        public int old_ml_seq { get; set; }
        public string old_ml_code { get; set; }
        public MemberLevelQuery()
        {
            create_user = string.Empty;
            update_user = string.Empty;
            code_name = string.Empty;
            old_ml_seq = 0;
            old_ml_code = string.Empty;
        }
    }
}
