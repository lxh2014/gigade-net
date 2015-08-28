using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class IlocChangeDetailQuery:IlocChangeDetail
    {
        public string productids { get; set; }
        public string product_name { get; set; }
        public string user_username { get; set; }

        public string product_sz { get; set; }
        public int prepaid { get; set; }
        public string prepa_name { get; set; }
        public DateTime made_date { get; set; }
        public int cde_dt_incr { get; set; }
        public DateTime cde_dt { get; set; }
        public int prod_qty { get; set; }
        public string pwy_dte_ctl { get; set; }
        public string isjq { get; set; }
        public string isgq { get; set; }
        public int cde_dt_var { get; set; }
        public int cde_dt_shp { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public string startloc { get; set; }
        public string endloc { get; set; }
        public string icd_id_In { get; set; }//料位管理->料位移動查詢，批次搬移

        public IlocChangeDetailQuery()
        {
            productids = string.Empty;
            product_name = string.Empty;
            user_username = string.Empty;
            product_sz = string.Empty;
            start_time = DateTime.MinValue;
            end_time = DateTime.MinValue;
            startloc = string.Empty;
            endloc = string.Empty;
            icd_id_In = string.Empty;
        }
    }
}
