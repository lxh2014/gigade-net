/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：CustomLayout 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/26 10:04:03 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net.Util;
using System.IO;
using log4net.Layout.Pattern;
using System.Collections;
using log4net.Core;

namespace Admin.gigade.Log4NetCustom
{
    public class CustomLayout : log4net.Layout.PatternLayout
    {
        public CustomLayout()
        {
            this.AddConverter("property", typeof(MyMessagePatternConverter));
        }
    }
}