using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class PromotionBannerQuery:PromotionBanner
    {
        public string createusername { get; set; }
        public string updateusername { get; set; }
        public DateTime date_start { get; set; }
        public DateTime date_end { get; set; }
        public int dateCon { get; set; }
        public string brand_name { get; set; }
        public int showStatus { get; set; }
        public int singleBrand_id { get; set; }
        public int multi { get; set; }
        public int changeMode { get; set; }
        public string brandIDS { get; set; }
        public PromotionBannerQuery()
        {
            createusername = string.Empty;
            updateusername = string.Empty;
            date_start = DateTime.MinValue;
            date_end = DateTime.MinValue;
            dateCon = 0;
            brand_name = string.Empty;
            showStatus = 0;
            singleBrand_id = 0;
            multi = 0;// 0不允許多圖 1允許多圖
            changeMode = 0;// 0不改變模式 1改變模式
            brandIDS = string.Empty;
        }
    }
}
