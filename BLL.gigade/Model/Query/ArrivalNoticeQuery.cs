using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ArrivalNoticeQuery : ArrivalNotice
    {

        public string user_name { get; set; }
        public string product_name { get; set; }
        public DateTime s_create_time { get; set; }
        public int condition { get; set; }
        public string searchCon { get; set; }
        public int item_stock { get; set; }
        public string user_email { get; set; }

        public string product_spec { get; set; }
        public Int64 ri_nums { get; set; }
        public uint vendor_id { get; set; }
        public string vendor_name_full { get; set; }
        public string vendor_name_full_OR_vendor_id { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public string ssend_notice_time { get; set; }
        public string user_username { get; set; }
        public string muser_name { get; set; }
        public string screate_time { get; set; }

        public int user_status { get; set; }
        public string sstatus { get; set; }
        public string spec_title_1 { get; set; }
        public string spec_title_2 { get; set; }

        //商品库存查询--新增字段
        public uint product_status { get; set; }
        public string product_status_string { get; set; }
        public int ignore_stock { get; set; }
        public string ignore_stock_string { get; set; }
        public uint brand_id { get; set; }
        public string brand_name { get; set; }

        public string product_id_OR_product_name { get; set; }
        public string brand_id_OR_brand_name { get; set; }
        public int item_stock_start { get; set; }
        public int item_stock_end { get; set; }
        public ArrivalNoticeQuery()
        {
            ignore_stock_string = string.Empty;
            product_status_string = string.Empty;
            product_id_OR_product_name = string.Empty;
            brand_id_OR_brand_name = string.Empty;
            item_stock_start = 0;
            item_stock_end = 0;

            product_status = 0;
            ignore_stock = 0;
            brand_id = 0;
            brand_name = string.Empty;

            user_name = string.Empty;
            product_name = string.Empty;
            s_create_time = DateTime.MinValue;
            condition = 0;
            searchCon = string.Empty;
            item_stock = 0;
            user_email = string.Empty;
            muser_name = string.Empty;
            product_spec = string.Empty;
            ri_nums = 0;
            vendor_id = 0;
            vendor_name_full = string.Empty;
            vendor_name_full_OR_vendor_id = string.Empty;
            start_time = string.Empty;
            end_time = string.Empty;
            screate_time = string.Empty;
            ssend_notice_time = string.Empty;
            user_status = 0;
            spec_title_1 = string.Empty;
            spec_title_2 = string.Empty;
            user_username = string.Empty;
            sstatus = string.Empty;
        }
    }
}
