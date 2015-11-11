using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class OrderReturnStatusQuery : OrderReturnStatus
    {
        public int orc_id { get; set; }
        public int orc_order_id { get; set; }
        public string orc_deliver_code { get; set; }
        public DateTime orc_deliver_date { get; set; }
        public string orc_deliver_time { get; set; }
        public string orc_name { get; set; }
        public  string orc_phone { get; set; }
        public string orc_zipcode { get; set; }
        public string orc_address { get; set; }
        public string orc_remark { get; set; }
        public int orc_type { get; set; }
        public string orc_service_remark { get; set; }

        public uint order_id { get; set; }
        public uint item_id { get; set; }
        public string product_name { get; set; }
        public string product_spec_name { get; set; }
        public uint buy_num { get; set; }
        public uint single_money { get; set; }

        public string bank_name { get; set; }
        public string bank_branch { get; set; }
        public string bank_account { get; set; }
        public string account_name { get; set; }
        public string bank_note { get; set; }
        public string product_mode { get; set; }
        public uint order_payment { get; set; }

        public int parameterCode { get; set; }
        public string parameterName { get; set;}
        public int invoice_deal { get; set; }

        public uint return_id { get; set; }
        public int ormpackage { get; set; }

        public int orc_send { get; set; }
        public string data { get; set; }
        public bool nonSelected { get; set; }
        public string itemStr { get; set; }
        public OrderReturnStatusQuery()
        {
            orc_id = 0;
            orc_order_id = 0;
            orc_deliver_code = string.Empty;
            orc_deliver_date = DateTime.MinValue;
            orc_deliver_time = string.Empty;
            orc_name = string.Empty;
            orc_phone = string.Empty;
            orc_zipcode = string.Empty;
            orc_address = string.Empty;
            orc_remark = string.Empty;
            orc_type = 0;
            orc_service_remark = string.Empty;
            order_id = 0;
            item_id = 0;
            product_name = string.Empty;
            product_spec_name = string.Empty;
            buy_num = 0;
            single_money = 0;
            product_mode = string.Empty;
            order_payment = 0;
            parameterCode = 0;
            parameterName = string.Empty;
            invoice_deal = 0;
            return_id = 0;
            ormpackage = 0;
            orc_send = 0;
            data = string.Empty;
            nonSelected = false;
            itemStr = string.Empty;
        }
    }
}
