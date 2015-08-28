/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductMapSetImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/12/18 15:51:48 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    interface IProductMapSetImplDao
    {
        List<Model.ProductMapSet> Query(Model.ProductMapSet query);
        List<Model.ProductMapSet> Query(uint product_id);
        List<Model.ProductMapSet> Query(Model.ProductItemMap query);
        string Delete(Model.ProductMapSet delete);
        string Save(Model.ProductMapSet save);
        List<Model.ProductMapSet> Query(uint map_id, uint item_id);
    }
}
