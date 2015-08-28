/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTagSet 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:40:49 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductTagSet
    {
        public int id { get; set; }
        public uint product_id { get; set; }
        public uint tag_id { get; set; }

        public ProductTagSet()
        {
            id = 0;
            product_id = 0;
            tag_id = 0;
        }
    }
}
