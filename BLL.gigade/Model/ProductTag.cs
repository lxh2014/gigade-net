/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductTag 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:33:48 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductTag : PageBase
    {
        public uint tag_id { get; set; }
        public string tag_name { get; set; }
        public uint tag_sort { get; set; }
        public uint tag_status { get; set; }
        public string tag_filename { get; set; }
        public uint tag_createdate { get; set; }
        public uint tag_updatedate { get; set; }
        public string tag_ipfrom { get; set; }

        public ProductTag()
        {
            tag_id = 0;
            tag_name = string.Empty;
            tag_sort = 0;
            tag_status = 0;
            tag_filename = string.Empty;
            tag_createdate = 0;
            tag_updatedate = 0;
            tag_ipfrom = string.Empty;
        }
    }
}
