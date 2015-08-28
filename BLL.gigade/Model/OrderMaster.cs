/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderMaster 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/20 14:54:52 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderMaster:Custom.Users
    {
        public uint Order_Id { get; set; }
        public uint User_Id { get; set; }
        public uint Bonus_Receive { get; set; }
        public uint Bonus_Discount_Percent { get; set; }
        public uint Bonus_Convert_NT { get; set; }
        public int Deduct_Happygo_Convert { get; set; }
        public uint Deduct_Bonus { get; set; }
        public uint Deduct_Welfare { get; set; }
        public int Deduct_Happygo { get; set; }
        public int Deduct_Card_Bonus { get; set; }
        public uint Deduct_Account { get; set; }
        public int Accumulated_Bonus { get; set; }
        public int Accumulated_Happygo { get; set; }
        public uint Order_Freight_Normal { get; set; }
        public uint Order_Freight_Low { get; set; }
        public uint Order_Product_Subtotal { get; set; }
        public uint Order_Amount { get; set; }
        public uint Money_Cancel { get; set; }
        public uint Money_Return { get; set; }
        public uint Order_Status { get; set; }
        public uint Order_Payment { get; set; }
        public bool Order_Deliver_Success { get; set; }
        public string Order_Name { get; set; }
        public bool Order_Gender { get; set; }
        public string Order_Mobile { get; set; }
        public string Order_Phone { get; set; }
        public uint Order_Zip { get; set; }
        public string Order_Address { get; set; }
        public bool Delivery_Same { get; set; }
        public string Delivery_Name { get; set; }
        public bool Delivery_Gender { get; set; }
        public string Delivery_Mobile { get; set; }
        public string Delivery_Phone { get; set; }
        public uint Delivery_Zip { get; set; }
        public string Delivery_Address { get; set; }
        public int Estimated_Arrival_Period { get; set; }
        public bool Company_Write { get; set; }
        public string Company_Invoice { get; set; }
        public string Company_Title { get; set; }
        public uint Invoice_Id { get; set; }
        public string Order_Invoice { get; set; }
        public uint Invoice_Status { get; set; }
        public string Note_Order { get; set; }
        public string Note_Admin { get; set; }
        public uint Order_Date_Pay { get; set; }
        public uint Order_Date_Close { get; set; }
        public uint Order_Date_Cancel { get; set; }
        public uint Order_Compensate { get; set; }
        public uint Cart_Id { get; set; }
        public uint Order_Createdate { get; set; }
        public uint Order_Updatedate { get; set; }
        public string Order_Ipfrom { get; set; }
        public uint Source_Trace { get; set; }
        public string Source_Cookie_Value { get; set; }
        public string Source_Cookie_Name { get; set; }
        public uint Note_Order_Modifier { get; set; }
        public uint Note_Order_Modify_Time { get; set; }
        public bool Error_Check { get; set; }
        public string Fax_SN { get; set; }
        public uint Channel { get; set; }
        public uint Bonus_Type { get; set; }
        public string Channel_Order_Id { get; set; }
        public uint Delivery_Store { get; set; }
        public bool Billing_Checked { get; set; }
        public DateTime Import_Time { get; set; }
        public uint Retrieve_Mode { get; set; }
        public int Export_Flag { get; set; }
        public int Money_Collect_Date { get; set; }
        public int Priority { get; set; }
        public int BonusExpireDay { set; get; }
        public int Holiday_Deliver { get; set; }// 假日可出貨? 1為可以,2為不可以

        public OrderMasterPattern OrderMasterPattern { get; set; }

        public OrderMaster()
        {
            Order_Id = 0;
            User_Id = 0;
            Bonus_Receive = 0;
            Bonus_Discount_Percent = 0;
            Bonus_Convert_NT = 0;
            Deduct_Happygo_Convert = 0;
            Deduct_Bonus = 0;
            Deduct_Welfare = 0;
            Deduct_Happygo = 0;
            Deduct_Card_Bonus = 0;
            Deduct_Account = 0;
            Accumulated_Bonus = 0;
            Accumulated_Happygo = 0;
            Order_Freight_Normal = 0;
            Order_Freight_Low = 0;
            Order_Product_Subtotal = 0;
            Order_Amount = 0;
            Money_Cancel = 0;
            Money_Return = 0;
            Order_Status = 0;
            Order_Payment = 0;
            Order_Deliver_Success = false;
            Order_Name = string.Empty;
            Order_Gender = false;
            Order_Mobile = string.Empty;
            Order_Phone = string.Empty;
            Order_Zip = 0;
            Order_Address = string.Empty;
            Delivery_Same = false;
            Delivery_Name = string.Empty;
            Delivery_Gender = false;
            Delivery_Mobile = string.Empty;
            Delivery_Phone = string.Empty;
            Delivery_Zip = 0;
            Delivery_Address = string.Empty;
            Estimated_Arrival_Period = 0;
            Company_Write = false;
            Company_Invoice = string.Empty;
            Company_Title = string.Empty;
            Invoice_Id = 0;
            Order_Invoice = string.Empty;
            Invoice_Status = 0;
            Note_Order = string.Empty;
            Note_Admin = string.Empty;
            Order_Date_Pay = 0;
            Order_Date_Close = 0;
            Order_Date_Cancel = 0;
            Order_Compensate = 0;
            Cart_Id = 0;
            Order_Createdate = 0;
            Order_Updatedate = 0;
            Order_Ipfrom = string.Empty;
            Source_Trace = 0;
            Source_Cookie_Value = string.Empty;
            Source_Cookie_Name = string.Empty;
            Note_Order_Modifier = 0;
            Note_Order_Modify_Time = 0;
            Error_Check = false;
            Fax_SN = string.Empty;
            Channel = 0;
            Bonus_Type = 0;
            Channel_Order_Id = string.Empty;
            Delivery_Store = 0;
            Billing_Checked = false;
            Import_Time = DateTime.MinValue;
            Retrieve_Mode = 0;
            Export_Flag = 0;
            Money_Collect_Date = 0;
            Priority = 0;
            OrderMasterPattern = null;
        }
    }
}
