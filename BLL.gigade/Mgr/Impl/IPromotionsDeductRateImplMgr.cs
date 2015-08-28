/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IPromotionsDeductRateImplMgr.cs
* 摘 要：
* 點數抵用與資料庫交互方法  
* 当前版本：v1.1
* 作 者：hongfei0416j    
* 完成日期：2014/6/20 
* 修改歷史:
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：zhejiang0304j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IPromotionsDeductRateImplMgr
    {
        int Save(Model.PromotionsDeductRate promoDRate);
        int Update(Model.PromotionsDeductRate promoDRate);
        int Delete(Model.PromotionsDeductRate pdr);
        List<PromotionsDeductRateQuery> Query(PromotionsDeductRateQuery store, out int totalCount);
        PromotionsDeductRate GetMOdel(int id);
        int UpdateActive(PromotionsDeductRate store);
    }
}
