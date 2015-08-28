/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductTagSetImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/10/30 15:08:57 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductTagSetImplMgr
    {
        List<Model.ProductTagSet> Query(Model.ProductTagSet productTagSet);
        string Delete(Model.ProductTagSet productTagSet);
        string Save(Model.ProductTagSet productTagSet);
        string SaveFromOtherPro(Model.ProductTagSet productTagSet);
    }
}
