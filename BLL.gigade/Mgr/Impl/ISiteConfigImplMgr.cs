/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：ISiteConfigImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/9/12 13:50:35 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface ISiteConfigImplMgr
    {
        List<BLL.gigade.Model.SiteConfig> Query();
        BLL.gigade.Model.SiteConfig GetConfigByName(string configName);
        bool UpdateNode(BLL.gigade.Model.SiteConfig newConfig);
    }
}
