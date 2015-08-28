/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：DeliveryStoreQuery 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/27 13:28:14 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Query
{
    public class DeliveryStoreQuery:DeliveryStore
    {
        public string delivery_store_name { get; set; }

        public DeliveryStoreQuery()
        {
            delivery_store_name = string.Empty;
        }
    }
}
