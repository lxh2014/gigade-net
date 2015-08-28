/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ChannelOrder 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/22 13:29:57 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ChannelOrder:PageBase
    {
        public int Serial_Number { get; set; }
        public int Channel_Id { get; set; }
        public string Order_Id { get; set; }
        public string Channel_Detail_Id { get; set; }
        public string Store_Dispatch_File { get; set; }
        public string Dispatch_Seq { get; set; }
        public DateTime Createtime { get; set; }
        public DateTime Ordertime { get; set; }
        public DateTime Latest_Deliver_Date { get; set; }

        public ChannelOrder()
        {
            Serial_Number = 0;
            Channel_Id = 0;
            Order_Id = string.Empty;
            Channel_Detail_Id = string.Empty;
            Store_Dispatch_File = string.Empty;
            Dispatch_Seq = string.Empty;
            Createtime = DateTime.MinValue;
            Ordertime = DateTime.MinValue;
            Latest_Deliver_Date = DateTime.MinValue;
        }
    }
}
