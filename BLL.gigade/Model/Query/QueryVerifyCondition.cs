using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class QueryVerifyCondition : PageBase
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
        public string site_ids { get; set; }
        public int product_type { get; set; }//商品形態 add by wwei0216w 2015/6/1

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

        /// <summary>
        /// 按鈕條件 add by jiajun 2014/08/20
        /// </summary>
        public int priceCondition { get; set; }
        public int temp_status { get; set; }
        /// <summary>
        /// 商品名稱 支持促銷項目維護模糊查詢 add by shuangshuang0420j 2014.10.28 14:30:00
        /// </summary>
        public string product_name { get; set; }

        /// <summary>
        /// 庫存狀態 1.庫存為0還可販售 2.補貨中停止販售 3.庫存數<1
        /// </summary>
        public int StockStatus { get; set; }

        //add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
        public int Prepaid { get; set; }
        public string brand_ids { get; set; }

        //add by zhuoqin0830w  2015/06/30  失格商品篩選
        public int off_grade { get; set; }

         //add by guodong1130w 2015/09/16 預購商品篩選
        public int purchase_in_advance { get; set; }

        public int outofstock_days_stopselling { get; set; }
        public QueryVerifyCondition()
        {
            product_name = string.Empty;
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
            priceCondition = 1;
            temp_status = -1;
            site_ids = String.Empty;
           
            StockStatus = 0;
            //add by zhuoqin0830w  2015/03/11  已買斷商品的篩選功能
            Prepaid = -1;
            product_type = -1;
            brand_ids = string.Empty;
            //add by zhuoqin0830w  2015/06/30  失格商品篩選
            off_grade = 0;
            //add by guodong1130w 2015/09/16 預購商品篩選
            purchase_in_advance = 0;
            //add by dongya 2015/10/22 缺貨下架天數
            outofstock_days_stopselling = 0;
        }
    }
}
