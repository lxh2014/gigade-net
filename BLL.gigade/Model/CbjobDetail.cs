using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class CbjobDetail:PageBase
    {
        public int row_id { get; set; }
        public string cb_jobid { get; set; }
        public int cb_newid { get; set; }
        public int iinvd_id { get; set; }
        public DateTime create_datetime { get; set; }
        public int create_user { get; set; }
        public DateTime change_datetime { get; set; }
        public int chang_user { get; set; }
        public int status { get; set; }

        public CbjobDetail()
        {
            row_id = 0;
            cb_jobid = string.Empty;
            cb_newid = 0;
            iinvd_id = 0;
            create_datetime = DateTime.MinValue;
            create_user = 0;
            chang_user = 0;
            change_datetime = DateTime.MinValue;
            status = 1;
        }
    }
}
