/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ItemPriceCustom 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 11:23:05 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model.Custom
{
    public class ItemPriceCustom:ItemPrice
    {
        public string spec_name_1 { get; set; }
        public string spec_name_2 { get; set; }

        public ItemPriceCustom()
        {
            spec_name_1 = string.Empty;
            spec_name_2 = string.Empty;
        }
    }
}
