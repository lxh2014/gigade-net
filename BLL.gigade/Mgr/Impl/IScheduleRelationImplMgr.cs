using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IScheduleRelationImplMgr
    {
        /// <summary>
        /// 根據relation_id刪除數據
        /// </summary>
        /// <param name="relation_id">relation_id</param>
        /// <returns>一個sql執行語句</returns>
        bool Delete(string relation,int relation_id);

        /// <summary>
        /// 獲得符合要求的list集合
        /// </summary>
        /// <param name="sr">一個ScheduleRelation 對象</param>
        /// <returns>list</returns>
        List<ScheduleRelation> Query(ScheduleRelation sr);
        List<ScheduleRelation> Query(int schedule_id);

        /// <summary>
        /// 保存ScheduleRelaiton
        /// </summary>
        /// <param name="sr">一個ScheduleRelation,裏面包含需要保存的信息</param>
        /// <returns>success or false</returns>
        bool Save(ScheduleRelation sr);

        /// <summary>
        /// 獲取最近出貨時間
        /// </summary>
        /// <returns>最近出貨時間</returns>
        DateTime GetRecentlyTime(int relationId, string relationType);

        
    }
}
