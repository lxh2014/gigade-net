/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：Resource 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/5 10:21:32 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public static class Resource
    {
        private static IResource coreMessage;

        /// <summary>
        /// 获取或设置资源对象
        /// </summary>
        public static IResource CoreMessage
        {
            get { return coreMessage; }
            set { coreMessage = value; }
        }
    }

}
