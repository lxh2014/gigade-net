/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ISiteConfigImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/12 13:49:56 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface ISiteConfigImplDao
    {
        string XmpPath { get; set; }
        List<BLL.gigade.Model.SiteConfig> Query();
        BLL.gigade.Model.SiteConfig GetConfigByName(string configName);
        bool UpdateNode(BLL.gigade.Model.SiteConfig newConfig);
    }
}
