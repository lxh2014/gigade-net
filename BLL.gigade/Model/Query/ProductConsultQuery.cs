using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BLL.gigade.Model.Query
{
    public class ProductConsultQuery : ProductConsult
    {
        public string brand_name { get; set; }
        public string product_name { get; set; }
        public string user_email { get; set; }
        public string parameterName { get; set; }
        public string user_name { get; set; }
        public string manage_name { get; set; }
        public string product_image { get; set; }
        public string product_image1 { get; set; }
        public string product_image2 { get; set; }
        public string spec_name1 { get; set; }
        public string spec_name2 { get; set; }
        public int huifu { get; set; }
        public int event_price { get; set; }
        public uint item_money { get; set; }
        public uint event_money { get; set; }
        public uint event_start { get; set; }
        public uint event_end { get; set; }
        public string productIds { get; set; }
        public string parameterCode { get; set; }
        public DateTime beginTime { get; set; }
        public DateTime endTime { get; set; }
        public string consultType { get; set; }
        public int prod_classify { get; set; }
        public string ckShopClass { get; set; }
        public string group_code { get; set; }
        public ProductConsultQuery()
        {
            product_name = string.Empty;
            user_email = string.Empty;
            parameterName = string.Empty;
            user_name = string.Empty;
            manage_name = string.Empty;
            product_image = string.Empty;
            brand_name = string.Empty;
            spec_name1 = string.Empty;
            spec_name2 = string.Empty;
            event_start = 0;
            event_end = 0;
            item_money = 0;
            event_money = 0;
            product_image1 = string.Empty;
            product_image2 = string.Empty;
            huifu = 0;
            event_price = 0;
            productIds = string.Empty;
            parameterCode = string.Empty;
            beginTime = DateTime.MinValue;
            endTime = DateTime.MinValue;
            consultType = string.Empty;
            prod_classify = 0;
            ckShopClass = string.Empty;
            group_code = string.Empty;
        }
    }
}
