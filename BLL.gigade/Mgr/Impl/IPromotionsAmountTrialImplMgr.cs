//#region 文件信息
///* 
//* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
//* All rights reserved. 
//* 
//* 文件名称：IPromotionsAmountTrialImplMgr.cs
//* 摘 要：促銷試用
//* 
//* 当前版本：v1.1
//* 作 者： shuangshuang0420j
//* 完成日期：2014/11/18 
//* 修改歷史：
//*         
//*/

//#endregion


using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using System.Data;
namespace BLL.gigade.Mgr.Impl
{
    public interface IPromotionsAmountTrialImplMgr
    {

        List<PromotionsAmountTrialQuery> Query(PromotionsAmountTrialQuery query, out int totalCount);
        int Save(PromotionsAmountTrialQuery query);
        int Update(PromotionsAmountTrialQuery query);
        int Delete(int id, string event_id);

        PromotionsAmountTrialQuery Select(int id);

        int UpdateActive(PromotionsAmountTrialQuery model);

        PromotionsAmountTrial GetModel(int id);
    }
}
