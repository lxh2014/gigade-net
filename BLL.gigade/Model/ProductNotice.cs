/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNotice 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:34:25 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductNotice : PageBase
    {
        public uint notice_id { get; set; }
        public string notice_name { get; set; }
        public uint notice_sort { get; set; }
        public uint notice_status { get; set; }
        public string notice_filename { get; set; }
        public uint notice_createdate { get; set; }
        public uint notice_updatedate { get; set; }
        public string notice_ipfrom { get; set; }

        public ProductNotice()
        {
            notice_id = 0;
            notice_name = string.Empty;
            notice_sort = 0;
            notice_status = 0;
            notice_filename = string.Empty;
            notice_createdate = 0;
            notice_updatedate = 0;
            notice_ipfrom = string.Empty;
        }
    }
}
