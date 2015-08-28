/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：PriceUpdateApply 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/12/6 13:19:48 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class PriceUpdateApply
    {
        public uint apply_id { get; set; }
        public uint price_master_id { get; set; }
        public DateTime apply_time { get; set; }
        public uint apply_user { get; set; }

        public PriceUpdateApply()
        {
            apply_id = 0;
            price_master_id = 0;
            apply_time = DateTime.MinValue;
            apply_user = 0;
        }
    }
}
