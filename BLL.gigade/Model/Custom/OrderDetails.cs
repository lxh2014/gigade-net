using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
   public class OrderDetails:PageBase
    {
        public uint detail_id { get; set; }
        public uint slave_id { get; set; }
        public uint item_id { get; set; }
        public uint item_vendor_id { get; set; }
        public uint product_freight_set { get; set; }
        public uint product_mode { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public uint single_cost { get; set; }
        public uint event_cost { get; set; }
        public uint single_price { get; set; }
        public uint single_money { get; set; }
        public uint deduct_bonus { get; set; }
        public uint deduct_welfare { get; set; }
        public int deduct_happygo { get; set; }
        public int deduct_happygo_money { get; set; }
        public uint deduct_account { get; set; }
        public string deduct_account_note { get; set; }
        public int accumulated_bonus { get; set; }
        public int accumulated_happygo { get; set; }
        public uint buy_num { get; set; }
        public uint detail_status { get; set; }
        public string detail_note { get; set; }
        public string item_code { get; set; }
        public uint arrival_status { get; set; }
        public uint delay_till { get; set; }
        public string lastmile_deliver_serial { get; set; }
        public uint lastmile_deliver_datetime { get; set; }
        public string lastmile_deliver_agency { get; set; }
        public uint bag_check_money { get; set; }
        public string channel_detail_id { get; set; }
        public int combined_mode { get; set; }
        public uint item_mode { get; set; }
        public int parent_id { get; set; }
        public string sub_order_id { get; set; }
        public uint pack_id { get; set; }
        public string parent_name { get; set; }
        public uint parent_num { get; set; }
        public uint price_master_id { get; set; }
        public int site_id { get; set; }
        public string event_id { get; set; }
        public int prepaid { get; set; }
        public int is_gift { get; set; }
        public OrderDetails()
        {
            
            detail_id = 0;
            slave_id = 0;
            item_id = 0;
            item_vendor_id = 0;
            product_freight_set = 0;
            product_mode = 0;
            product_name = string.Empty;
            product_spec_name = string.Empty;
            single_cost = 0;
            event_cost = 0;
            single_price = 0;
            single_money = 0;
            deduct_bonus = 0;
            deduct_welfare = 0;
            deduct_happygo = 0;
            deduct_happygo_money = 0;
            deduct_account = 0;
            deduct_account_note = string.Empty;
            accumulated_bonus = 0;
            accumulated_happygo = 0;
            buy_num = 0;
            detail_status = 0;
            detail_note = string.Empty;
            item_code = string.Empty;
            arrival_status = 0;
            delay_till = 0;
            lastmile_deliver_serial = string.Empty;
            lastmile_deliver_datetime = 0;
            lastmile_deliver_agency = string.Empty;
            bag_check_money = 0;
            channel_detail_id = string.Empty;
            combined_mode = 0;
            item_mode = 0;
            parent_id = 0;
            sub_order_id = string.Empty;
            pack_id = 0;
            parent_name = string.Empty;
            price_master_id = 0;
            parent_num = 0;
            site_id = 0;
            prepaid = 0;
            is_gift = 0;
        }
    }
}
