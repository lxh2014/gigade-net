/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNoticeSet 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:42:20 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductNoticeSet
    {
        public uint product_id { get; set; }
        public uint notice_id { get; set; }
        public ProductNoticeSet()
        {
            product_id = 0;
            notice_id = 0;

        }
    }
}
