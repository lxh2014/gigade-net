/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Product_Site 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/25 10:51:34 
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductPicture : PageBase
    {
        public int product_id { get; set; }
        public string image_filename { get; set; }
        public uint image_sort { get; set; }
        public uint image_state { get; set; }
        public int image_createdate { get; set; }
        public Int64 pic_type { get; set; }
        public ProductPicture()
        {
            product_id = 0;
            image_filename = "";
            image_sort = 0;
            image_state = 1;
            image_createdate = 0;
            pic_type = 0;
        }
    }
}
