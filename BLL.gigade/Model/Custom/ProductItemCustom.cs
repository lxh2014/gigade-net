using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ProductItemCustom : ProductItem
    {
        public string Brand_Name { get; set; }
        public string Product_Name { get; set; }
        public string Prod_Sz { get; set; }
        public string Product_Mode { get; set; }
        public int Product_Mode_Id { get; set; }
        public string Freight_Set { get; set; }
        public int Freight_Set_Id { get; set; }
        public int Prepaid { get; set; }
        public string IsPrepaid { get { return Prepaid == 0 ? "否" : "是"; } }
        public string Product_Status { get; set; }
        public int Product_Status_Id { get; set; }
        //add by zhuoqin0830w  2015/02/25  添加匯出庫存資料中添加匯出庫存資料中規格一，二的狀態顯示
        public int Spec_Status1 { get; set; }
        public string IsStatus1 { get { return Spec_Status1 == -1 ? "" : (Spec_Status1 == 0 ? "隱藏" : "顯示"); } }
        public int Spec_Status2 { get; set; }
        public string IsStatus2 { get { return Spec_Status2 == -1 ? "" : (Spec_Status2 == 0 ? "隱藏" : "顯示"); } }
        public int Deliver_Days { get; set; }

        public ProductItemCustom()
        {
            Brand_Name = string.Empty;
            Product_Name = string.Empty;
            Product_Mode = string.Empty;
            Freight_Set_Id = 0;
            Freight_Set = string.Empty;
            Prepaid = 0;
            Product_Status = string.Empty;
            Product_Status_Id = 0;
            Prod_Sz = string.Empty;
            Spec_Status1 = -1; // 表示在查詢過程中 Spec_Status1 和 Spec_Status2 的值 為 null 
            Spec_Status2 = -1;
            Deliver_Days = 0;
        }
    }
}