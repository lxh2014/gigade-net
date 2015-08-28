using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BLL.gigade.Model.Query
{
    public class OrderSlaveMasterQuery : OrderSlaveMaster
    {
        public string vendor_name_simple { get; set; }
        public uint date_start { get; set; }
        public uint date_end { get; set; }
        public uint date_type { get; set; }
        public int is_check { get; set; }
        public DateTime deliver_date { get; set; }
        public DateTime create_date { get; set; }
        public string deliver_name { get; set; }
        public uint slave_id { get; set; }
        public uint order_id { get; set; }
        public uint item_id { get; set; }
        public uint buy_num { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public uint deliver_id { get; set; }
        public uint detail_status { get; set; }

        public OrderSlaveMasterQuery()
        {
            vendor_name_simple = string.Empty;
            date_start = 0;
            date_end = 0;
            date_type = 0;//1:創建時間2:出貨日期
            is_check = -1;//判斷是否在搜索條件中加on_check查詢
            deliver_date = DateTime.MinValue;
            create_date = DateTime.MinValue;
            deliver_name = string.Empty;
            slave_id = 0;
            order_id = 0;
            item_id = 0;
            buy_num = 0;
            product_name = string.Empty;
            product_spec_name = string.Empty;
            deliver_id = 0;
            detail_status = 0;
        }
    }
}
