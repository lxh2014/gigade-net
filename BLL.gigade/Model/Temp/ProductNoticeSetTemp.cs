/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ProductNoticeSetTemp 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:39:53 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class ProductNoticeSetTemp : ProductNoticeSet
    {
        public new string product_id { get; set; }
        public int Writer_Id { get; set; }
        public int Combo_Type { get; set; }

        public ProductNoticeSetTemp()
        {
            product_id = "0";
            Writer_Id = 0;
            Combo_Type = 0;
        }
    }
}
