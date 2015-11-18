using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class DeliverChangeLog:PageBase
    {
        public int dcl_id { get; set; }
        public int deliver_id { get; set; }//出貨單編號
        public int dcl_create_user { get; set; }//創建人(用戶)
        public DateTime dcl_create_datetime { get; set; }//創建時間
        public int dcl_create_muser { get; set; }//創建人(管理員)
        public int dcl_create_type { get; set; }//物流配送模式
        public string dcl_note { get; set; }//備註
        public string dcl_ipfrom { get; set; }
        public DateTime expect_arrive_date { get; set; }//期望到貨日
        public int expect_arrive_period { get; set; }//期望到貨時段

        public DeliverChangeLog()
        {
            dcl_id = 0;
            deliver_id = 0;
            dcl_create_user = 0;
            dcl_create_datetime = DateTime.MinValue;
            dcl_create_muser = 0;
            dcl_create_type = 1;
            dcl_note = string.Empty;
            dcl_ipfrom = string.Empty;
            expect_arrive_date = DateTime.MinValue;
            expect_arrive_period = 0;
        }
    }
}
