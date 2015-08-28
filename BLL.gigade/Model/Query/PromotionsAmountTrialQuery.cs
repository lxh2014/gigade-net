using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionsAmountTrialQuery : PromotionsAmountTrial
    {
        public int expired { get; set; }//查詢是否過期
        public string key { get; set; }//關鍵字查詢
        public string group_name { get; set; }
        public string brand_name { get; set; }
        public string category_name { get; set; }
        public string condition_name { get; set; }
        public string freight { get; set; }
        public string eventtype { get; set; }
        public string payment { get; set; }
        public string device_name { get; set; }
        public string paper_name { get; set; }
        public string sale_product_name { get; set; }
        public int recordCount { get; set; }
        public int shareCount { get; set; }
        public string user_username { get; set; }

        public PromotionsAmountTrialQuery()
        {
            expired = 0;
            key = string.Empty;
            group_name = string.Empty;
            category_name = string.Empty;
            brand_name = string.Empty;
            condition_name = string.Empty;
            freight = string.Empty;
            eventtype = string.Empty;
            payment = string.Empty;
            device_name = string.Empty;
            paper_name = string.Empty;
            sale_product_name = string.Empty;
            recordCount = 0;
            shareCount = 0;
            user_username = string.Empty;
        }

    }
}
