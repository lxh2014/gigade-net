/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IOrderMasterPatternImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/02/26
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IOrderMasterPatternImplMgr
    {
        // 保存 公關單與報廢單 中 新增加 的數據
        string Save(OrderMasterPattern op);
    }
}