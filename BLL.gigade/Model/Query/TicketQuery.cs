using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class TicketQuery : Ticket
    {
        public string vendor_name_simple { get; set; }
        public string deliver_ys_type { get; set; }//運送方式
        public int deliver_ys_type_id { get; set; }
        public uint order_id { get; set; }
        public UInt64 orderId { get; set; }
        public uint order_date_pay { get; set; }
        public int money_collect_date { get; set; }
        public uint om_money_collect_date { get; set; }
        public string moneyCollectDate { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public string spec_name { get; set; }
        public string spec_name1 { get; set; }
        public uint buy_num { get; set; }//
        public int combined_mode { get; set; }
        public int item_mode { get; set; }
        public int parent_id { get; set; }
        public uint pack_id { get; set; }
        public uint parent_num { get; set; }
        public uint deliver_id { get; set; }//
        public string delivery_name { get; set; }
        public string order_name { get; set; }
        public int product_freight_set { get; set; }
        public string ticketIds { get; set; }
        public string delivery_mobile { get; set; }
        public uint delivery_zip { get; set; }
        public string delivery_address { get; set; }
        public int estimated_arrival_period { get; set; }
        public string arrival_period_name { get; set; }
        public uint detail_id { get; set; }
        public uint item_id { get; set; }
        public uint detail_status { get; set; }
        public uint order_createdate { get; set; }
        public string orderCreatedate { get; set; }
        public string note_order { get; set; }
        public uint order_freight_normal { get; set; }
        public uint order_freight_low { get; set; }
        public int channel { get; set; }
        public int retrieve_mode { get; set; }
        public uint priority { get; set; }
        public uint holiday_deliver { get; set; }
        public string holidayDeliver { get; set; }
        public string freight_set_name { get; set; }
        public string zip_name { get; set; }
        public string deliveryStore { get; set; }

        
        public string ticket_idto_str { get; set; }//出貨單通過,拼接成的字符串
        public int type_id { get; set; }//類型 撿貨單1 出貨明細2 貨運單3

        public TicketQuery()
        {
            vendor_name_simple = string.Empty;
            deliver_ys_type = string.Empty;
            deliver_ys_type_id = 0;
            order_id = 0;
            order_date_pay = 0;
            money_collect_date = 0;
            product_name = string.Empty;
            spec_name = string.Empty;
            spec_name1 = string.Empty;
            combined_mode = 0;
            item_mode = 0;
            parent_id = 0;
            pack_id = 0;
            parent_num = 0;
            deliver_id = 0;
            delivery_name = string.Empty;
            order_name = string.Empty;
            product_freight_set = 0;
            ticketIds = string.Empty;
            delivery_mobile = string.Empty;
            delivery_zip = 0;
            delivery_address = string.Empty;
            estimated_arrival_period = 0;
            detail_id = 0;
            item_id = 0;
            product_spec_name = string.Empty;
            detail_status = 0;
            order_createdate = 0;
            note_order = string.Empty;
            order_freight_normal = 0;
            order_freight_low = 0;
            channel = 0;
            retrieve_mode = 0;
            priority = 0;
            holiday_deliver = 0;
            arrival_period_name = string.Empty;
            freight_set_name = string.Empty;
            zip_name = string.Empty;
            moneyCollectDate = string.Empty;
            orderCreatedate = string.Empty;
            holidayDeliver = string.Empty;
            deliveryStore = string.Empty;

            ticket_idto_str = string.Empty;
            type_id = 0;
        }
    }
}
