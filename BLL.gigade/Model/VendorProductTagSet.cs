/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：VendorProductTagSet 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：shiwei0620j 
 * 完成日期：
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class VendorProductTagSet
    {
        public int id { get; set; }
        public string product_id { get; set; }
        public uint tag_id { get; set; }

        public VendorProductTagSet()
        {
            id = 0;
            product_id = string.Empty;
            tag_id = 0;
        }
    }
}
