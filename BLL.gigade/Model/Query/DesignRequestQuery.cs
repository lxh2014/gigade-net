using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class DesignRequestQuery:DesignRequest
    {
        public string dr_status_tostring { get; set; }
        public string dr_type_tostring { get; set; }
        public string dr_assign_to_name { get; set; }
        public string dr_requester_id_name { get; set; }
        public string dr_ids { get; set; }
        public string product_name { get; set; }
        public int date_type { get; set; }//查詢條件限定
        public string start_time { get; set; }//開始時間
        public string end_time { get; set; }//結束時間
        public int login_id { get; set; }
        public int day { get; set; }
        public int Isgq { get; set; }
        public int Istake { get; set; }
        public DesignRequestQuery()
        {
            dr_status_tostring = string.Empty;
            dr_type_tostring = string.Empty;
            dr_assign_to_name = string.Empty;
            dr_requester_id_name = string.Empty;
            dr_ids = string.Empty;
            product_name = string.Empty;
            date_type = 0;
            start_time = string.Empty;
            end_time = string.Empty;
            day = 0;
            Isgq = 0;
            Istake=0;
        }
    }
}
