using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IBonusMasterImplMgr
    {
       
        List<BonusMasterQuery> BonusTypeStore();
        List<BonusMasterQuery> GetBonusMasterList(BonusMasterQuery query, ref int totalCount);
        bool BonusMasterAdd(List<BonusMasterQuery> list);
        bool BonusMasterUpdate(BonusMasterQuery store);

        /// <summary>
        /// 向 bonus_master 裱中 添加 一筆數據  add by zhuoqin0830w 2015/08/24
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        string AddBonusMaster(BonusMaster bm);
        /// <summary>
        /// 根據 br.user_id 獲取 bonus_master 裱中 購物金的總和  add by zhuoqin0830w 2015/08/24
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        int GetSumBouns(BonusRecord br);
        /// <summary>
        /// 根據 User_id 和 時間 的倒序查詢出相應的列表  add by zhuoqin0830w 2015/08/24
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        List<BonusMaster> GetBonusMasterByEndTime(BonusRecord br);
        /// <summary>
        /// 根據 master_id 修改數據   add by zhuoqin0830w 2015/08/24
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        string UpdateBonusMasterBalance(BonusMaster bm);
    }
}
