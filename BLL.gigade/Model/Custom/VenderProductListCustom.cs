using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class VenderProductListCustom
    {
        //copy WuHan QueryandVerifyCustom
        public string product_image { get; set; }
        public string product_id { get; set; }
        public string brand_name { get; set; }
        public string product_name { get; set; }// 商品名稱
        public string prod_sz { get; set; }

        /// <summary>
        /// 商品類型
        /// </summary>
        public string combination { get; set; }
        public string price_type { get; set; }// 價格類型
        public int price_type_id { get; set; }
        /// <summary>
        /// 商品類型
        /// </summary>
        public uint combination_id { get; set; }
        public string product_spec { get; set; }
        public string product_status { get; set; }
        public uint product_status_id { get; set; }
        public uint product_sort { get; set; }
        public uint product_createdate { get; set; }
        public string site_name { get; set; }
        public uint site_id { get; set; }//站臺ID
        public uint master_user_id { get; set; }//add by hufeng0813w 2014/05/22 price_master表中的User_id
        public string user_level { get; set; }//會員等級
        public uint level { get; set; }//會員等級ID
        public string user_email { get; set; }
        public uint user { get; set; }
        public string price_status { get; set; }
        public int price { get; set; }
        public int cost { get; set; }//成本 add by xiangwang0413w 2014/07/30
        public int event_price { get; set; }
        public int event_cost { get; set; }// 活動成本 add by xiangwang0413w 2014/07/30
        public uint event_start { get; set; }
        public uint event_end { get; set; }
        public DateTime apply_time { get; set; }
        public string apply_user { get; set; }
        public uint online_mode { get; set; }
        public uint price_master_id { get; set; }
        public uint brand_id { get; set; }
        public string temp_status { get; set; }
        public int create_channel { get; set; }
        /// <summary>
        /// 建立人
        /// </summary>
        public string user_name { get; set; }
        public int user_id { get; set; } //20140929 edit jialei 臨時數據提前user_id數據轉換錯誤
        /// <summary>
        /// 建議售價
        /// </summary>
        public uint product_price_list { get; set; }
        /// <summary>
        /// 申請類型
        /// </summary>
        public string prev_status { get; set; }
        /// <summary>
        /// 運送方式
        /// </summary>
        public string product_freight_set { get; set; }
        /// <summary>
        /// 出貨方式
        /// </summary>
        public string product_mode { get; set; }
        /// <summary>
        /// 營業稅
        /// </summary>
        public int tax_type { get; set; }
        /// <summary>
        /// 上架時間 
        /// </summary>
        public uint product_start { get; set; }
        /// <summary>
        /// 下架時間
        /// </summary>
        public uint product_end { get; set; }

        public string CanSel
        {
            get
            {//記得回覆原樣！！！！！！！！！！！！！！！！！！！！！
                return "0";//user_id.ToString() ==(System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString() ? "0" : "1";
            }
        }

        public string CanDo
        {
            get
            {
                return "0";//user_id.ToString() != (System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString() ? "0" : "1";
            }
        }

        public string CanReplace
        {
            get
            {
                return (product_status == "新建立商品" || product_status == "下架") ? "0" : "1";
            }
        }

        public VenderProductListCustom()
        {
            product_image = string.Empty;
            product_id = string.Empty;
            brand_name = string.Empty;
            brand_id = 0;
            product_name = string.Empty;
            prod_sz = string.Empty;
            combination = string.Empty;
            price_type = string.Empty;//add by hufeng0813w 2014/06/18
            price_type_id = 0;
            product_spec = string.Empty;
            product_status = string.Empty;
            product_status_id = 0;
            tax_type = 0;
            product_sort = 0;
            product_createdate = 0;
            product_start = 0;
            product_end = 0;
            site_name = string.Empty;
            site_id = 0;
            master_user_id = 0;
            user_level = string.Empty;
            level = 0;
            user_email = string.Empty;
            user = 0;
            price_status = string.Empty;
            price = 0;
            cost = 0;
            event_price = 0;
            event_cost = 0;
            event_start = 0;
            event_end = 0;
            apply_time = DateTime.MinValue;
            apply_user = string.Empty;
            online_mode = 0;
            user_name = string.Empty;
            user_id = 0;
            product_price_list = 0;
            prev_status = string.Empty;
            product_freight_set = string.Empty;
            product_mode = string.Empty;
            price_master_id = 0;
            temp_status = string.Empty;
            create_channel =0;
        }
    }
}
