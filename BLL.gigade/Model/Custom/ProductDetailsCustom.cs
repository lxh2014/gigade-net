using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductDetailsCustom
    {
        public uint product_type { get; set; }  //code
        public string brand_name { get; set; }
        public string product_name { get; set; }
        public int sale_status { get; set; }
        public string prod_sz { get; set; }
        public uint product_sort { get; set; }
        public string product_vendor_code { get; set; }
        public uint product_start { get; set; }
        public uint product_end { get; set; }
        public uint expect_time { get; set; }
        public string product_freight_set { get; set; }
        public int product_freight_set_id { get; set; }
        public string product_mode { get; set; }
        public int product_mode_id { get; set; }
        public int tax_type { get; set; }
        public string combination { get; set; }
        public int combination_id { get; set; }
        public string page_content_1 { get; set; }
        public string page_content_2 { get; set; }
        public string page_content_3 { get; set; }
        public uint product_buy_limit { get; set; }
        public string product_keywords { get; set; }
        public uint fortune_quota { get; set; }
        public uint fortune_freight { get; set; }
        public string product_status { get; set; }
        public int product_status_id { get; set; }
        public int price_type { get; set; }
        public string expect_msg { get; set; }
        public int create_channel { get; set; }
        public int show_in_deliver { get; set; } //add by wangwei 2014/9/17
        public int prepaid { get; set; }//add by wangwei 2014/9/17
        public string product_kind { get; set; }//add by wangwei 2014/9/17
        public int product_kind_id { get; set; }
        public string process_type { get; set; }//add by wangwei 2014/9/17
        public int process_type_id { get; set; }
        public string sale_name { get; set; }//add by wangwei 2014/9/17
        public string course_name { get; set; }
        public int course_id { get; set; } //// add by wwei0216w 2015/3/12
        public UInt64 product_id { get; set; }//add by shuangshuang0420j 2014/10/20
        public UInt64 brand_id { get; set; }//add by shuangshuang0420j 2014/10/20
        public UInt64 category_id { get; set; }//add by shuangshuang0420j 2014/10/20
        public string category_name { get; set; }//add by shuangshuang0420j 2014/10/22
        public string product_image { get; set; }//add by mengjuan0826j 2015/02/10

        //add by zhuoqin0830w  增加5個欄位
        public int deliver_days { get; set; }  //供應商出貨天數
        public int min_purchase_amount { get; set; }  //最小採購數量
        public double safe_stock_amount { get; set; }  //安全存量細數
        public int extra_days { get; set; }  //寄倉天數/調度天數
        public int purchase_in_advance { get; set; }///add by wwei0216w 2015/7/30 是否預購
        public uint purchase_in_advance_start { get; set; }///add by wwei0216w 2015/7/30 預購開始時間
        public uint purchase_in_advance_end { get; set; }///add by wwei0216w 2015/7/30 預購結束時間


        public ProductDetailsCustom()
        {
            course_id = 0;// add by wwei0216w 2015/3/12
            course_name = string.Empty;
            category_id = 0;
            brand_id = 0;
            product_id = 0;
            brand_name = string.Empty;
            product_name = string.Empty;
            prod_sz = string.Empty;
            sale_status = 0;
            product_sort = 0;
            product_vendor_code = string.Empty;
            product_start = 0;
            product_end = 0;
            expect_time = 0;
            product_freight_set = string.Empty;
            product_freight_set_id = 0;
            product_mode = string.Empty;
            product_mode_id = 0;
            tax_type = 0;
            combination = string.Empty;
            combination_id = 0;
            page_content_1 = string.Empty;
            page_content_2 = string.Empty;
            page_content_3 = string.Empty;
            product_buy_limit = 0;
            product_keywords = string.Empty;
            fortune_quota = 0;
            fortune_freight = 0;
            product_status = string.Empty;
            product_status_id = 0;
            price_type = 0;
            expect_msg = string.Empty;
            create_channel = 0;
            show_in_deliver = 0;////add by wangwei 2014/9/17
            prepaid = 0;//add by wangwei 2014/9/17
            product_kind = string.Empty;//add by wangwei 2014/9/17
            product_kind_id = 0;
            process_type = string.Empty;//add by wangwei 2014/9/17
            process_type_id = 0;
            sale_name = string.Empty;//add by wangwei 2014/9/17
            category_name = string.Empty;
            product_image = string.Empty;
            
            //add by zhuoqin0830w  增加5個欄位
            deliver_days= 0; 
            min_purchase_amount= 0;
            safe_stock_amount= 0;
            extra_days = 0;

            purchase_in_advance = 0;
            purchase_in_advance_start = 0;
            purchase_in_advance_end = 0;
        }
    }
}
