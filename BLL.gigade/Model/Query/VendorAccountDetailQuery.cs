using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VendorAccountDetailQuery : VendorAccountDetail
    {
        public uint detail_id { get; set; }
        public uint item_id { get; set; }
        public uint product_freight_set { get; set; }
        public uint product_mode { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public uint single_cost { get; set; }
        public uint event_cost { get; set; }
        public uint single_price { get; set; }
        public uint single_money { get; set; }
        public uint buy_num { get; set; }
        public uint parent_num { get; set; }
        public uint detail_status { get; set; }
        public uint item_mode { get; set; }
        public int parent_id { get; set; }
        public uint pack_id { get; set; }
        public uint order_payment { get; set; }
        public uint order_createdate { get; set; }
        public uint slave_date_delivery { get; set; }
        public uint slave_date_close { get; set; }
        public uint account_year { get; set; }
        public uint account_month { get; set; }
        public DateTime slave_date_deliverys { get; set; }
        public DateTime account_dates { get; set; }
        //public DateTime slave_date_closes { get; set; }
        public DateTime order_createdates { get; set; }
        public string search_start_time { get; set; }
        public string search_end_time { get; set; }
        public string newname { get; set; }
        public string orderIds { get; set; }
        public string upc_id { get; set; }
        public VendorAccountDetailQuery()
        {

            item_id = 0;
            product_freight_set = 1;
            product_mode = 1;
            product_name = string.Empty;
            product_spec_name = string.Empty;
            single_cost = 0;
            event_cost = 0;
            single_price = 0;
            single_money = 0;
            buy_num = 0;
            parent_num = 0;
            detail_status = 0;
            item_mode = 0;
            pack_id = 0;
            order_payment = 0;
            order_createdate = 0;
            slave_date_delivery = 0;
            slave_date_close = 0;
            account_year = 0;
            account_month = 0;
            slave_date_deliverys = DateTime.MinValue;
            //slave_date_closes = DateTime.MinValue;
            order_createdates = DateTime.MinValue;
            newname = string.Empty;
            account_dates = DateTime.MinValue;
            orderIds = string.Empty;
            upc_id = string.Empty;
        }
    }
}
