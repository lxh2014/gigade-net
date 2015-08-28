using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    [Serializable]
   public class VendorAccountMonth:PageBase
    {
        public uint vendor_id { get; set; }
        public uint account_year { get; set; }
        public uint account_month { get; set; }
        public uint m_product_money { get; set; }
        public uint m_product_cost { get; set; }
        public uint m_money_creditcard_1 { get; set; }
        public uint m_money_creditcard_3 { get; set; }
        public uint m_freight_delivery_low { get; set; }
        public uint m_freight_delivery_normal { get; set; }
        public uint m_dispatch_freight_delivery_normal { get; set; }
        public uint m_dispatch_freight_delivery_low { get; set; }
        public uint m_freight_return_low { get; set; }
        public int m_account_amount { get; set; }
        public uint m_all_deduction { get; set; }
        public uint m_gift { get; set; }
        public uint dispatch { get; set; }
        public uint m_bag_check_money { get; set; }
        public uint m_freight_return_normal { get; set; }
        public VendorAccountMonth()
        {
            account_year = 0;
            account_month = 0;
            m_product_money = 0;
            m_product_cost = 0;
            m_money_creditcard_1 = 0;
            m_money_creditcard_3 = 0;
            m_freight_delivery_low = 0;
            m_freight_delivery_normal = 0;
            m_dispatch_freight_delivery_normal = 0;
            m_dispatch_freight_delivery_low = 0;
            m_freight_return_low = 0;
            m_freight_return_normal = 0;
            m_account_amount = 0;
            m_all_deduction = 0;
            m_gift = 0;
            dispatch = 0;
            m_bag_check_money = 0;
        }
    }
}
