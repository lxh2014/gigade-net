using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IBonusMasterImplDao
    {
        int GetUserBonus(int user_id);
        List<BonusMasterQuery> BonusTypeStore();
        List<BonusMasterQuery> GetBonusMasterList(BonusMasterQuery query, ref int totalCount);
        int CheckBonusMasterStatus(BonusMasterQuery query, DateTime now, int master_id = 1);
        string InsertBonusMaster(BonusMasterQuery store);
        bool UpdateBonusMaster(BonusMasterQuery store);

        /// <summary>
        /// 向 bonus_master 裱中 添加 一筆數據
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        string AddBonusMaster(BonusMaster bm);
        /// <summary>
        /// 得到 bonus_master 裱中 購物金的 總和
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        int GetSumBouns(BonusRecord br);
        /// <summary>
        /// 根據 User_id 和 時間 的倒序查詢出相應的列表 購物金
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        List<BonusMaster> GetBonusByEndTime(BonusRecord br);
        /// <summary>
        /// 得到 bonus_master 裱中 抵用券 總和
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        int GetSumWelfare(BonusRecord br);
        /// <summary>
        /// 根據 User_id 和 時間 的倒序查詢出相應的列表 抵用券
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        List<BonusMaster> GetWelfareByEndTime(BonusRecord br);
        /// <summary>
        /// 根據 master_id 修改數據 
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        string UpdateBonusMasterBalance(BonusMaster bm);

        List<BonusMasterQuery> IsExtendBonus(BonusMasterQuery query);
        int BonusAmount(BonusMasterQuery query);
        List<BonusMaster> GetBonus(BonusMasterQuery query);
        string UpBonusMaster(BonusMasterQuery query);
        string InsertBonusRecord(BonusRecord query);
    }
}
