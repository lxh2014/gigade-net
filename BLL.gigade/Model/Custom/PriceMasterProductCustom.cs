using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class PriceMasterProductCustom:PriceMaster
    {
        public int price_type { get; set; }
        public uint product_price_list { get; set; }
        public uint bag_check_money { get; set; }
        public uint show_listprice { get; set; }
        public uint product_mode { get; set; }
        public PriceMasterProductCustom()
        {
            price_type = 0;
            product_price_list = 0;
            bag_check_money = 0;
            show_listprice = 0;
            product_mode = 0;
        }

    }
}
