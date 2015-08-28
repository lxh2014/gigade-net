/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Serial 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/23 9:39:20 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class Serial
    {
        public int Serial_id { get; set; }
        public UInt64 Serial_Value { get; set; }
        public string Serial_Description { get; set; }

        public Serial()
        {
            Serial_id = 0;
            Serial_Value = 0;
            Serial_Description = string.Empty;
        }
    }
}
