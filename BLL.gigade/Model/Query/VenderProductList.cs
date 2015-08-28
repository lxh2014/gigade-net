using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class VenderProductList : PageBase
    {   
        public uint brand_id { get; set; }
        public uint site_id { get; set; }
        public uint user_level { get; set; }
        public uint user_id { get; set; }
        public int combination { get; set; }
        public int product_status { get; set; }
        public string date_type { get; set; }
        public string time_start { get; set; }
        public int cost { get; set; }
        public int event_cost { get; set; }
        public string time_end { get; set; }
        public uint freight { get; set; }
        public uint mode { get; set; }
        public uint tax_type { get; set; }
        public bool price_check { get; set; }
        public uint price_status { get; set; }
        public string name_number { get; set; }
        public string temp_status { get; set; } //add jialei 20140902
        public string create_channel { get; set; }
        /// <summary>
        /// 申請類型
        /// </summary>
        public int prev_status { get; set; }
        /// <summary>
        /// 品類分類
        /// </summary>
        public string cate_id { get; set; }
        /// <summary>
        /// 前台分類
        /// </summary>
        public uint category_id { get; set; }
        /// <summary>
        /// 供應商編號 add by shuangshuang0420j 2014.08.20 10:00:00
        /// </summary>
        public uint vendor_id { get; set; }

        /// <summary>
        /// 按鈕條件 add by jiajun 2014/08/20
        /// </summary>
        public int price_type { get; set; }

        public VenderProductList()
        {
            brand_id = 0;
            site_id = 0;
            user_level = 0;
            user_id = 0;
            cost = 0;//成本
            event_cost = 0;//活動成本
            combination = 0;
            product_status = -1;
            date_type = string.Empty;
            time_start = string.Empty;
            time_end = string.Empty;
            category_id = 0;
            freight = 0;
            mode = 0;
            tax_type = 0;
            price_check = false;
            price_status = 0;
            name_number = string.Empty;
            prev_status = -1;
            cate_id = string.Empty;
            price_type = 1;
            temp_status = string.Empty;
            create_channel = string.Empty;
        }
    }
}
