using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class GroupCommitteContact : PageBase
    {
        public int gcc_id { get; set; }
        public int group_id { get; set; }
        public string gcc_chairman { get; set; }
        public string gcc_phone { get; set; }
        public string gcc_mail { get; set; }
        public int k_user { get; set; }
        public DateTime k_date { get; set; }
        public int m_user { get; set; }
        public DateTime m_date { get; set; }

        public GroupCommitteContact()
        {
            gcc_id = 0;
            group_id = 0;
            gcc_chairman = string.Empty;
            gcc_phone = string.Empty;
            gcc_mail = string.Empty;
            k_user = 0;
            k_date = DateTime.MinValue;
            m_user = 0;
            m_date = DateTime.MinValue;
        }
    }
}
