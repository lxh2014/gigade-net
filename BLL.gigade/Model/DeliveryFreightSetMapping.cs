/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：DeliveryFreightSetMapping 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/21 16:55:05 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class DeliveryFreightSetMapping:PageBase
    {
        public int Product_Freight_Set { get; set; }
        public int Delivery_Freight_Set { get; set; }

        public DeliveryFreightSetMapping()
        {
            Product_Freight_Set = 0;
            Delivery_Freight_Set = 0;
        }
    }
}
