using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderShowMasterQuery : OrderShowMaster
    {
        public DateTime OrderDatePay { get; set; }
        public DateTime MoneyCollectDate { get; set; }
        public DateTime OrderCreateDate { get; set; }
        public DateTime OrderDateClose { get; set; }
        public string user_username { get; set; }
        public DateTime NoteOrderModifyTime { get; set; }
        public string status_ipfrom { get; set; }
        public DateTime StatusCreateDate { get; set; }
        public int serial_id { get; set; }
        public string channel_name_simple { get; set; }
        public string Hg_Nt { get; set; }
        public uint manager_id { get; set; }
        public string manager_name { get; set; }
        public string order_status_str { get; set; }
        public string payment_string { get; set; }
        public string deliver_str { get; set; }
        public string Amount { get; set; }
        public string Subtotal { get; set; }
        public bool is_vendor_deliver { get; set; }
        public bool isSecret { get; set; }
        public string delivery_address_str { get; set; }
        public bool is_manage_user { get; set; }
        public bool is_send_product { get; set; }
        public  OrderShowMasterQuery()
        {
            OrderDatePay = DateTime.MinValue;
            MoneyCollectDate = DateTime.MinValue;
            OrderCreateDate = DateTime.MinValue;
            OrderDateClose = DateTime.MinValue;
            user_username = string.Empty;
            NoteOrderModifyTime = DateTime.MinValue;
            status_ipfrom = string.Empty;
            StatusCreateDate = DateTime.MinValue;
            serial_id = 0;
            channel_name_simple = string.Empty;
            Hg_Nt = string.Empty;
            manager_id = 0;
            manager_name = string.Empty;
            order_status_str = string.Empty;
            payment_string = string.Empty;
            deliver_str = string.Empty;
            is_vendor_deliver = false;
            isSecret = true;
            delivery_address_str = string.Empty;
            is_manage_user = false;
            is_send_product = false;
        }
    }
}
