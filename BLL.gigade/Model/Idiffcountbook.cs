using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Idiffcountbook:PageBase
    {
        public int book_id { get; set; }
        public string cb_jobid { get; set; }
        public string loc_id { get; set; }
        public int pro_qty { get; set; }
        public int st_qty { get; set; }
        public int item_id { get; set; }
        public int create_user { get; set; }
        public DateTime create_time { get; set; }
        public DateTime made_date { get; set; }
        public DateTime cde_date { get; set; }

        public Idiffcountbook()
        {
            book_id = 0;
            cb_jobid = string.Empty;
            loc_id = string.Empty;
            pro_qty = 0;
            st_qty = 0;
            item_id = 0;
            create_user = 0;
            create_time = DateTime.MinValue;
            made_date = DateTime.MinValue;
            cde_date = DateTime.MinValue;
        }
    }
}
