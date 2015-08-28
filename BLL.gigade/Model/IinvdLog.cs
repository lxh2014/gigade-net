using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class IinvdLog : PageBase
    {
        public int row_id { get; set; }
        public int nvd_id { get; set; }
        public int from_num { get; set; }
        public int change_num { get; set; }
        public int create_user { get; set; }
        public DateTime create_date { get; set; }

        public IinvdLog()
        {
            nvd_id = 0;
            from_num = 0;
            change_num = 0;
            create_user = 0;
            create_date = DateTime.Now;
        }

    }
}
