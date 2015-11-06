
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class ProductItemQuery : ProductItem
    {/*chaojie1124j 添加三個字段：isAllStock,sumDays,periodDays用來實現商品建議採購量功能*/
        public int stockScope { get; set; }/*庫存範圍--*/
        public int sumDays { get; set; }/*總天數*/
        public int periodDays { get; set; }/*查詢天數*/
        //public int ChannelId { get; set; }
        public int prepaid { set; get; }/*是否買斷*/
        public int Is_pod { set; get; }/*是否已下單採購*/
        public string vendor_name { set; get; }/*供應商名稱*/

        public string category_ID_IN { set; get; }
        public string product_name { set; get; }/*chaojie1124j添加商品庫存查詢*/
        public uint vendor_id { set; get; }/*chaojie1124j添加商品庫存查詢*/
        public string vendor_name_full { set; get; }/*chaojie1124j添加商品庫存查詢*/
        public uint brand_id { set; get; }/*chaojie1124j添加商品庫存查詢*/
        public string brand_name { set; get; }/*chaojie1124j添加商品庫存查詢*/
        public string product_status_string { set; get; }/*chaojie1124j添加商品庫存查詢*/
        public int ignore_stock { set; get; }/*chaojie1124j添加商品庫存查詢*/
        public string product_spec { set; get; }/*chaojie1124j添加商品庫存查詢*/
        public string ignore_stock_string { set; get; }
        public string sale_status_string { get; set; }
        public uint product_id { set; get; }
        public uint item_id { set; get; }
        public int item_stock { set; get; }
        public uint product_status { get; set; }
        public uint sale_status { get; set; }
        public string product_id_OR_product_name { get; set; }
        public string brand_id_OR_brand_name { get; set; }
        public int item_stock_start { get; set; }
        public int item_stock_end { get; set; }
        public string vendor_name_full_OR_vendor_id { get; set; }

        //等待料位報表
        public uint product_freight_set { get; set; }
        public string product_freight_set_string { get; set; }
        public int start_time { get; set; }
        public int end_time { get; set; }
        public uint combination { get; set; }
        public string combination_string { get; set; }
        public string product_createdate_string { get; set; }
        public uint product_createdate { get; set; }
        public string product_start_string { get; set; }
        public string product_mode_string { get; set; }
        public int delivery_freight_set { get; set; }
        public uint product_start { get; set; }
        public int product_mode { get; set; }
        public string product_fenlei_xiaolei { get; set; }
        public string product_fenlei_dalei { get; set; }
        public uint spec_id_1 { get; set; }
        public uint spec_id_2 { get; set; }
        public string cate_id { get; set; }
        public string spec_title_1 { get; set; }
        public string spec_title_2 { get; set; }
        public string plas_id_string { get; set; }
        public string po_id { get; set; }
        public ProductItemQuery()
        {
            stockScope = 0;
            sumDays = 0;
            periodDays = 0;
            prepaid = -1;
            Is_pod = 0;
            vendor_name = string.Empty;
            category_ID_IN = string.Empty;
            vendor_id = 0;
            vendor_name_full = string.Empty;
            brand_id = 0;
            brand_name = string.Empty;
            product_status_string = string.Empty;
            sale_status_string = string.Empty;
            ignore_stock = 0;
            product_spec = string.Empty;
            ignore_stock_string = string.Empty;
            product_id = 0;
            item_id = 0;
            item_stock = 0;
            product_status = 0;
            sale_status = 0;
            product_id_OR_product_name = string.Empty;
            brand_id_OR_brand_name = string.Empty;
            item_stock_start = 0;
            item_stock_end = 0;
            vendor_name_full_OR_vendor_id = string.Empty;
            product_freight_set = 0;
            start_time = 0;
            end_time = 0;
            combination = 0;
            combination_string = string.Empty;
            product_freight_set_string = string.Empty;
            product_createdate_string = string.Empty;
            product_createdate = 0;
            product_start_string = string.Empty;
            product_mode_string = string.Empty;
            delivery_freight_set = 0;
            product_start = 0;
            product_mode = 0;
            product_fenlei_xiaolei = string.Empty;
            product_fenlei_dalei = string.Empty;
            spec_id_1 = 0;
            spec_id_2 = 0;
            cate_id = string.Empty ;
            spec_title_1 = string.Empty;
            spec_title_2 = string.Empty;
            plas_id_string = string.Empty ;
            po_id = string.Empty;

        }
    }
}
