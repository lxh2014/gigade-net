using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductCustom :Product
    {
        public int Vendor_Status { get; set; }//供應商狀態
        public int Brand_Status { get; set; } //品牌狀態
        public int Price_Status { get; set; } //價格狀態
        public int Price { get; set; } // 價格
        public int Item_Stock { get; set; } //庫存

        public ProductCustom()
        {
            Vendor_Status = 0;
            Brand_Status = 0;
            Price_Status = 0;
            Price = 0;
            Item_Stock = 0;
        }
    }
}
