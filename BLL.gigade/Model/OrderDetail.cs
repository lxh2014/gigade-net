/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderDetail 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/20 15:43:12 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderDetail : PageBase
    {
        public uint Detail_Id { get; set; }
        public uint Slave_Id { get; set; }
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
        public string Deduct_Account_Note { get; set; }
        public int Accumulated_Bonus { get; set; }
        public int Accumulated_Happygo { get; set; }
        public uint Buy_Num { get; set; }
        public uint Detail_Status { get; set; }
        public string Detail_Note { get; set; }
        public string Item_Code { get; set; }
        public bool Arrival_Status { get; set; }
        public uint Delay_Till { get; set; }
        public string Lastmile_Deliver_Serial { get; set; }
        public uint Lastmile_Deliver_Datetime { get; set; }
        public string Lastmile_Deliver_Agency { get; set; }
        public uint Bag_Check_Money { get; set; }
        public string Channel_Detail_Id { get; set; }
        public int Combined_Mode { get; set; }
        public uint item_mode { get; set; }
        public int Parent_Id { get; set; }
        public string Sub_Order_Id { get; set; }
        public uint pack_id { get; set; }
        public string parent_name { get; set; }
        public uint parent_num { get; set; }
        public uint price_master_id { get; set; }
        public int Site_Id { get; set; }
        public string event_id { get; set; }
        /// <summary>
        /// 已買斷的商品  0:否  1:是
        /// </summary>
        public int Prepaid { get; set; }
        public int receiving_status { get; set; }
        public int is_gift { get; set; }//是否為贈品

        public int rdeduct_happygo { get; set; }
        public int rdeduct_happygo_money { get; set; }
        public int raccumulated_bonus { get; set; }
        public int raccumulated_happygo { get; set; }
        public int rcombined_mode { get; set; }
        public int rparent_id { get; set; }
        public uint rarrival_status { get; set; }

        public OrderDetail()
        {
            Detail_Id = 0;
            Slave_Id = 0;
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
            Detail_Note = string.Empty;
            Item_Code = string.Empty;
            Arrival_Status = false;
            Delay_Till = 0;
            Lastmile_Deliver_Serial = string.Empty;
            Lastmile_Deliver_Datetime = 0;
            Lastmile_Deliver_Agency = string.Empty;
            Bag_Check_Money = 0;
            Channel_Detail_Id = string.Empty;
            Combined_Mode = 0;
            item_mode = 0;
            Parent_Id = 0;
            Sub_Order_Id = string.Empty;
            pack_id = 0;
            parent_name = string.Empty;
            parent_num = 0;
            price_master_id = 0;
            Prepaid = 0;
            Site_Id = 0;
            event_id = string.Empty;
            is_gift = 0;
            raccumulated_bonus=0;
            raccumulated_happygo = 0;
            rdeduct_happygo = 0;
            rdeduct_happygo_money = 0;
            rcombined_mode = 0;
            rparent_id = 0;
            rarrival_status = 0;

        }
    }
}
