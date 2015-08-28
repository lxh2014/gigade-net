using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr.Impl
{
    public interface IScheduleImplMgr
    {
        //add by wwei0216 2015/2/9
        /// <summary>
        /// 保存Freight排程的執行信息
        /// </summary>
        /// <param name="fst">FreightSetTime對象</param>
        /// <returns>執行后受影響的行數</returns>
        bool Save(Schedule s);

        //add by wwei0216 2015/2/10
        /// <summary>
        /// 查詢排成的執行信息
        /// </summary>
        /// <param name="fst">FreightSetTime對象</param>
        /// <returns>FreightSetTime的集合</returns>
        List<Schedule> Query(Model.Query.ScheduleQuery s);

        ////add by wwei0216w 2015/2/25
        ///// <summary>
        ///// 為排程賦予product_id
        ///// </summary>
        ///// <returns>受影響的行數</returns>
        //int UpdateProductId(uint schedule_id);

        //add by wwei0216w 2015/2/25
        /// <summary>
        /// 根據商品Id刪除排程
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <returns>受影響的行數</returns>
        bool Delete(params int[] ids);

        /// <summary>
        /// 獲得對應信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        List<Parametersrc> GetRelevantInfo(string path,string type);

        /// <summary>
        /// 獲得value的信息
        /// </summary>
        /// <param name="keyValue">key 值</param>
        /// <returns>對應的值信息集合</returns>
        List<Parametersrc> GetValue(int keyValue);
    }
}
