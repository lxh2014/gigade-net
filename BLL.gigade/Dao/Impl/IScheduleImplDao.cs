using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao.Impl
{
    public interface IScheduleImplDao
    {
        //add by wwei0216 2015/2/9
        /// <summary>
        /// 保存Freight排程的執行信息
        /// </summary>
        /// <param name="fst">FreightSetTime對象</param>
        /// <returns>執行后受影響的行數</returns>
        int Save(Schedule s);
        int Update(Schedule schedule);

        //add by wwei0216 2015/2/10
        /// <summary>
        /// 查詢排成的執行信息
        /// </summary>
        /// <param name="columns">要查詢的列名</param>
        /// <returns>FreightSetTime的集合</returns>
        List<Schedule> Query(Model.Query.ScheduleQuery schedule);

        ////add by wwei0216w 2015/2/25
        ///// <summary>
        ///// 為排程賦予product_id
        ///// </summary>
        ///// <returns>受影響的行數</returns>
        //int UpdateProductId(uint productId);

        //add by wwei0216w 2015/2/25
        /// <summary>
        /// 根據商品Id刪除排程
        /// </summary>
        /// <param name="Schedule">Schedule</param>
        /// <returns>受影響的行數</returns>
        int Delete(Schedule s);
        int Delete(int[] ids);
    }
}
