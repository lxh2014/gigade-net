/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductSiteCustom 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 10:48:00 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class PriceMasterCustom : PriceMaster
    {
        public string site_name { get; set; }
        public string user_email { get; set; }
        public string user_level_name { get; set; }
        public string status { get; set; }
        public int combination { get; set; }//商品類型
        public int price_type { get; set; }//價格類型
        public int event_price_discount { get; set; }//活動價格折數
        public int event_cost_discount { get; set; }//活動成本折數
        public int price_discount { get; set; }//售價折扣
        public int price_at { get; set; } //現在售價
        public int cost_at { get; set; } //現在成本
        public int cost_discount { get; set; }//成本折扣
        public string prod_sz { get; set; }
        public string type { get; set; } //參數類型 

        /*以下為匯出時所需要的列*/
        //public string product_name { get; set; }
        public uint item_id { get; set; }///其對應的item_id
        public uint item_money { get; set; }///所對應的子項售價
        public uint item_cost { get; set; }
        public uint event_money { get; set; }
        public uint item_event_cost { get; set; }
        //public string site_name { get; set; }///子項所對應的站臺價格
        public string user_level_str { get; set; }///會員信息
        public string price_status_str { get; set; }///價格類型
        public int product_status { get; set; }                                                    
        public string product_status_str { get; set; }///商品狀態字符類型
        public string item_name {get;set;}
        public string spec_title_1 { get; set; }
        public string spec_title_2 { get; set; }

        //public int price {get;set;}
        //public int cost{get;set;}
        //public int event_price{get;set;}
        //public int event_cost { get; set; }
        public string vendor_product_id { get; set; }//儲存臨時表的id
        public PriceMasterCustom()
        {
            site_name = string.Empty;
            user_email = string.Empty;
            //product_name = string.Empty;
            user_level_name = string.Empty;
            status = string.Empty;
            combination = 0;
            price_discount = 0;
            cost_discount = 0;
            price_type = 0;
            event_price_discount = 0;
            event_cost_discount = 0;
            price_at = 0;
            cost_at = 0;
            vendor_product_id = string.Empty;//儲存臨時表的id created 2014/09/15
            prod_sz = string.Empty;
            type = string.Empty;
            item_id = 0;
            item_money = 0;
            item_cost = 0;
            event_money = 0;
            item_event_cost = 0;
            user_level_str = string.Empty;
            price_status_str = string.Empty;
            product_status_str = string.Empty;
            item_name = string.Empty;
            product_status = 0;
            spec_title_1 = string.Empty;
            spec_title_2 = string.Empty;
            //cost = 0;
            //event_cost = 0;
            //event_price = 0;
        }
    }
}
