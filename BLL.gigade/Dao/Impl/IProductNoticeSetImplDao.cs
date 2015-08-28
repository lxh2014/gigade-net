/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductNoticeSetImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 14:44:43 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IProductNoticeSetImplDao
    {
        List<Model.ProductNoticeSet> Query(Model.ProductNoticeSet productNoticeSet);
        string Delete(Model.ProductNoticeSet productNoticeSet);
        string Save(Model.ProductNoticeSet productNoticeSet);
        string SaveFromOtherPro(Model.ProductNoticeSet productNoticeSet);
    }
}
