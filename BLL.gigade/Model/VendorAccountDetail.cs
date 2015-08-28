using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VendorAccountDetail : PageBase
    {
        public uint slave_id { get; set; }
        public int account_amount { get; set; }
        public uint vendor_id { get; set; }
        public uint order_id { get; set; }
        public uint creditcard_1_percent { get; set; }
        public uint creditcard_3_percent { get; set; }
        public uint sales_limit { get; set; }
        public uint bonus_percent { get; set; }
        public uint freight_low_limit { get; set; }
        public uint freight_low_money { get; set; }
        public uint freight_normal_limit { get; set; }
        public uint freight_normal_money { get; set; }
        public uint freight_return_low_money { get; set; }
        public uint freight_return_normal_money { get; set; }
        public uint product_money { get; set; }
        public uint product_cost { get; set; }
        public uint money_creditcard_1 { get; set; }
        public uint money_creditcard_3 { get; set; }
        public uint freight_delivery_low { get; set; }
        public uint freight_delivery_normal { get; set; }
        public uint freight_return_low { get; set; }
        public uint freight_return_normal { get; set; }
        public uint account_date { get; set; }
        public uint gift { get; set; }
        public uint deduction { get; set; }
        public uint bag_check_money { get; set; }
        public VendorAccountDetail()
        {
            account_amount = 0;
            creditcard_1_percent = 0;
            creditcard_3_percent = 0;
            sales_limit = 0;
            bonus_percent = 0;
            freight_low_limit = 0;
            freight_low_money = 0;
            freight_normal_limit = 0;
            freight_normal_money = 0;
            freight_return_low_money = 0;
            freight_return_normal_money = 0;
            product_money = 0;
            product_cost = 0;
            money_creditcard_1 = 0;
            money_creditcard_3 = 0;
            freight_delivery_low = 0;
            freight_delivery_normal = 0;
            freight_return_low = 0;
            freight_return_normal = 0;
            account_date = 0;
            gift = 0;
            deduction = 0;
            bag_check_money = 0;
        }
    }
}
