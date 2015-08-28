/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：SiteConfig 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/12 13:36:21 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Model
{
    public class SiteConfig
    {
        public string Name { get; set; }
        public string Remark { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }

        public SiteConfig()
        {
            Name = string.Empty;
            Remark = string.Empty;
            Value = string.Empty;
            DefaultValue = string.Empty;
        }
    }
}
