#region 文件信息
/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：IVendorLoginListImplMgr.cs 
 * 摘   要： 
 *      供應商管理-->供應商登入記錄
 * 当前版本：v1.1 
 * 作   者： changjian0408j
 * 完成日期：2014/10/7
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr.Impl
{
   public interface IVendorLoginListImplMgr
    {
       List<VendorLoginQuery> Query(VendorLoginQuery store, out int totalCount);
    }
}
