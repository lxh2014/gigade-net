/* 
 * 武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IBonusRecordImplMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作    者：zhuoqin0830w 
 * 完成日期：2015/08/25
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IBonusRecordImplMgr
    {
        /// <summary>
        /// 向 bonus_record 裱中添加 數據 add by zhuoqin0830w 2015/08/25
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        string InsertBonusRecord(BonusRecord br);
    }
}