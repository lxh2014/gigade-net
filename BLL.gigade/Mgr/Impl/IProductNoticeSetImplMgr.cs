/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductNoticeTagImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:09:12 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductNoticeSetImplMgr
    {
        List<Model.ProductNoticeSet> Query(Model.ProductNoticeSet productNoticeSet);
        string Delete(Model.ProductNoticeSet productNoticeSet);
        string Save(Model.ProductNoticeSet productNoticeSet);
        string SaveFromOtherPro(Model.ProductNoticeSet productNoticeSet);
    }
}
