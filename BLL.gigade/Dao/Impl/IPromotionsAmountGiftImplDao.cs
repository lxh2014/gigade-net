#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IPromotionsAmountGiftImplDao.cs
* 摘 要：
* 滿額滿件送禮dao接口
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：2014/6/20 
* 修改歷史：
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：mengjuan0826j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IPromotionsAmountGiftImplDao
    {
        List<PromotionsAmountGiftQuery> Query(PromotionsAmountGiftQuery query, out int totalCount,string type);
        int Save(PromotionsAmountGiftQuery query);
        int Update(PromotionsAmountGiftQuery query, string oldeventid);
        int Delete(int id, string event_id);
        DataTable SelectDt(PromotionsAmountGift model);
        PromotionsAmountGiftQuery Select(int id);
        PromotionsAmountGift GetModel(int id);
  
      int UpdateActive(PromotionsAmountGiftQuery model);
         
        //操作試吃/紅利抵用
        int TryEatAndDiscountSave(PromotionsAmountGiftQuery query);
        int TryEatAndDiscountUpdate(PromotionsAmountGiftQuery query, string oldeventid);
    }
}
