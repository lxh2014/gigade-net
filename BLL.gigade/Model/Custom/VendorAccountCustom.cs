using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class VendorAccountCustom : VendorAccountDetail
    {
        public uint Detail_Id { get; set; }
        public uint Item_Id { get; set; }
        public uint Item_Vendor_Id { get; set; }
        public uint Product_Freight_Set { get; set; }
        public uint Product_Mode { get; set; }
        public string Product_Name { get; set; }
        public string Product_Spec_Name { get; set; }
        public uint Single_Cost { get; set; }
        public uint Event_Cost { get; set; }
        public uint Single_Price { get; set; }
        public uint Single_Money { get; set; }
        public uint Deduct_Bonus { get; set; }
        public uint Deduct_Welfare { get; set; }
        public int Deduct_Happygo { get; set; }
        public int Deduct_Happygo_Money { get; set; }
        public uint Deduct_Account { get; set; }
        public uint Buy_Num { get; set; }
        public uint Detail_Status { get; set; }
        public string Deduct_Account_Note { get; set; }
        public int Accumulated_Bonus { get; set; }
        public int Accumulated_Happygo { get; set; }
        public uint item_mode { get; set; }
        public string free_tax { set; get; }//免稅金額
        public string tax_amount { set; get; }//營業稅
        public string total_amount { set; get; }//應稅金額
        public uint slave_date_delivery { set; get; }
        public DateTime slavedate_delivery { set; get; }
        public uint slave_date_close { get; set; }
        public uint Order_Payment { get; set; }
        public uint Order_Createdate { get; set; }
        public int parent_id { get; set; }
        public DateTime ordercreatedate { get; set; }
        public string Note_Admin { get; set; }
        public int tax_type { get; set; }
        public string taxtype { get; set; }
        public string paymentname { get; set; }
        public string product_freight { get; set; }
        public string order_status_name { get; set; }
        public DateTime accountdate { get; set; }
        public uint od_bag_check_money { get; set; }
        public uint single_cost_subtotal { get; set; }
        public string ProductMode { get; set; }
        public string itemmode { get; set; }
        public uint parent_num { get; set; }
        public string upc_id { get; set; }
        public VendorAccountCustom()
        {
            Detail_Id = 0;
            Item_Id = 0;
            Item_Vendor_Id = 0;
            Product_Freight_Set = 0;
            Product_Mode = 0;
            Product_Name = string.Empty;
            Product_Spec_Name = string.Empty;
            Single_Cost = 0;
            Event_Cost = 0;
            Single_Price = 0;
            Single_Money = 0;
            Deduct_Bonus = 0;
            Deduct_Welfare = 0;
            Deduct_Happygo = 0;
            Deduct_Happygo_Money = 0;
            Deduct_Account = 0;
            Deduct_Account_Note = string.Empty;
            Accumulated_Bonus = 0;
            Accumulated_Happygo = 0;
            Buy_Num = 0;
            Detail_Status = 0;
            item_mode = 0;
            free_tax = string.Empty;
            tax_amount = string.Empty;
            total_amount = string.Empty;
            slave_date_delivery = 0;
            slavedate_delivery = DateTime.MinValue;
            slave_date_close = 0;
            Order_Payment = 0;
            Order_Createdate = 0;
            ordercreatedate = DateTime.MinValue;
            Note_Admin = string.Empty;
            tax_type = 0;
            paymentname = string.Empty;
            product_freight = string.Empty;
            order_status_name = string.Empty;
            accountdate = DateTime.MinValue;
            od_bag_check_money = 0;
            single_cost_subtotal = 0;
            ProductMode = string.Empty;
            itemmode = string.Empty;
            parent_id = 0;
            taxtype = string.Empty;
            parent_num = 0;
            upc_id = string.Empty;
        }
    }
}
