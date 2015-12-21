using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IpoNvdQuery:IpoNvd
    {
        public string create_username { get; set; }
        public string modify_username { get; set; }
        public bool locid_allownull { get; set; } //主料位是否允許為空
        public string description { get; set; }
        public string prod_sz { get; set; }
        public string loc_id { get; set; }
        public string pwy_dte_ctl { set; get; } //是否為有效期控管
        public int cde_dt_incr { set; get; } //有效期天數
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }

        public IpoNvdQuery()
        {
            create_username = string.Empty;
            modify_username = string.Empty;
            locid_allownull = true;
            description = string.Empty;
            prod_sz = string.Empty;
            loc_id = string.Empty;
            pwy_dte_ctl = "N";
            cde_dt_incr = 0;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;

        }
    }
}
