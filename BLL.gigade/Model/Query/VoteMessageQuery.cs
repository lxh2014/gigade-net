using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VoteMessageQuery : VoteMessage
    {

        public string update_name { set; get; }//修改人
        public string create_name { set; get; }//創建人
        public string message_id_in { set; get; }//批量刪除
        public string article_title { set; get; }//文章名稱
        public VoteMessageQuery()
        {
            update_name = string.Empty;
            create_name = string.Empty;
            message_id_in = string.Empty;
            article_title = string.Empty;
        }

    }
}