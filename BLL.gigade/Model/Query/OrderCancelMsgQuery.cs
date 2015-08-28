using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Model.Query
{
    public class OrderCancelMsgQuery:OrderCancelMsg
    {
        public string scancel_type { get; set; }
        public string sorder_payment { get; set; }
        public string sorder_status { get; set; }
        public string user_email { get; set; }
        public string order_name { get; set; }
        public uint order_amount { get; set; }
        public string order_mobile { get; set; }
        public uint order_payment { get; set; }
        public uint order_status { get; set; }
        #region changjian0408j 于2014/10/30添加
        public uint response_id { get; set; }
        public string response_content { get; set; }
        public DateTime response_createdate { get; set; }
        
        #endregion
        public OrderCancelMsgQuery()
        {
            scancel_type = string.Empty;
            sorder_payment = string.Empty;
            sorder_status = string.Empty;
            user_email = string.Empty;
            order_name = string.Empty;
            order_amount = 0;
            order_mobile = string.Empty;
            order_payment = 0;
            order_status = 0;
        }
    }
}
