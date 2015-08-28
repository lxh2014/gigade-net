/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：OrderSlave 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/20 15:32:40 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class OrderSlave : PageBase
    {
        public uint Slave_Id { get; set; }
        public uint Order_Id { get; set; }
        public uint Vendor_Id { get; set; }
        public uint Slave_Freight_Normal { get; set; }
        public uint Slave_Freight_Low { get; set; }
        public uint Slave_Product_Subtotal { get; set; }
        public uint Slave_Amount { get; set; }
        public uint Slave_Status { get; set; }
        public string Slave_Note { get; set; }
        public uint Slave_Date_Delivery { get; set; }
        public uint Slave_Date_Cancel { get; set; }
        public uint Slave_Date_Return { get; set; }
        public uint Slave_Date_Close { get; set; }
        public bool Account_Status { get; set; }
        public uint Slave_Updatedate { get; set; }
        public string Slave_Ipfrom { get; set; }
        public List<OrderDetail> Details { get; set; }
        public OrderSlave()
        {
            Slave_Id = 0;
            Order_Id = 0;
            Vendor_Id = 0;
            Slave_Freight_Normal = 0;
            Slave_Freight_Low = 0;
            Slave_Product_Subtotal = 0;
            Slave_Amount = 0;
            Slave_Status = 0;
            Slave_Note = string.Empty;
            Slave_Date_Delivery = 0;
            Slave_Date_Cancel = 0;
            Slave_Date_Return = 0;
            Slave_Date_Close = 0;
            Account_Status = false;
            Slave_Updatedate = 0;
            Slave_Ipfrom = string.Empty;
            Details = new List<OrderDetail>();
        }
    }
}
