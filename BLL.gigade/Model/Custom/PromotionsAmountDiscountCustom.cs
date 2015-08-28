using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class PromotionsAmountDiscountCustom : PromotionsAmountDiscount
    {
        public string banner_image { get; set; }
        public string category_link_url { get; set; }
        public uint category_updatedate { get; set; }
        public string category_ipfrom { get; set; }
        public string event_id { get; set; }
        public DateTime date_state { get; set; }
        public DateTime date_end { get; set; }
        public uint category_father_id { get; set; }

        public string group_name { get; set; }
        public int searchStore { get; set; }
        public string create_user { get; set; }
        public string site_name { get; set; }
        public string update_user { get; set; }
        public PromotionsAmountDiscountCustom()
        {

            banner_image = string.Empty;
            category_link_url = string.Empty;
            category_updatedate = 0;
            category_ipfrom = string.Empty;
            event_id = string.Empty;
            date_state = DateTime.MinValue;
            date_end = DateTime.MinValue;
            category_father_id = 0;
            group_name = string.Empty;
            searchStore = 1;
            create_user = string.Empty;
            site_name = string.Empty;
            update_user = string.Empty;
        }
    }
}
