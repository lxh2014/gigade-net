/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductMapSetImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/12/18 16:37:08 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductMapSetImplMgr
    {
        List<Model.ProductMapSet> Query(Model.ProductMapSet query);
        List<Model.ProductMapSet> Query(uint product_id);
        List<Model.ProductMapSet> Query(uint map_id, uint item_id);
    }
}
