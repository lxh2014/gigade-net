using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VoteDetail : PageBase
    {
        public int vote_id { get; set; }
        public int article_id { get; set; }
        public int user_id { get; set; }
        public string ip { get; set; }
        public int vote_status { get; set; }
        public int create_user { get; set; }
        public int update_user { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }

        public VoteDetail()
        {
            vote_status = -1;
        }
    }
}