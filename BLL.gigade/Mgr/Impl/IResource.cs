/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IResource 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/5 10:18:30 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IResource
    {
        /// <summary>
        /// 获取对应资源.
        /// </summary>
        /// <param name="name">资源名称，即KEY.</param>
        /// <returns>对应的资源.</returns>
        string GetResource(string name);
    }

}
