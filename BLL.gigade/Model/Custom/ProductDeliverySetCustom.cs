using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Model.Custom
{
    public class ProductDeliverySetCustom: ProductDeliverySet
    {
        public string Freight_big_area_name { get; set; }
        public string Freight_type_name { get; set; }
        public string Product_name { get; set; } //add by wwei0216w 2015/1/12 
        public string Brand_name { get; set; }//添加 品牌  Brand_name   add by zhuoqin0830w  2015/04/24

        public ProductDeliverySetCustom() 
        {
            Freight_big_area_name = string.Empty;
            Freight_type_name = string.Empty;
            Product_name = string.Empty;
            Brand_name = string.Empty;//添加 品牌  Brand_name   add by zhuoqin0830w  2015/04/24
        }
    }
}
