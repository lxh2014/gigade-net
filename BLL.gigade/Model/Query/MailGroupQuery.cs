using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class MailGroupQuery : MailGroup
    {
        public Int64 callid { get; set; }
        public string user_mail { get; set; }//用戶郵箱
        public string user_name { get; set; }//用戶名稱
        public MailGroupQuery()
    {
            callid = 0;
            user_mail = string.Empty;
            user_name = string.Empty;

    }
}
}
