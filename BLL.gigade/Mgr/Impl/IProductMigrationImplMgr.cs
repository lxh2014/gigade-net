/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IProductMigrationImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2014/1/13 14:20:51 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProductMigrationImplMgr
    {
        Model.ProductMigrationMap GetSingle(Model.ProductMigrationMap query);
        string SaveNoPrid(Model.ProductMigrationMap pMap);
    }
}
