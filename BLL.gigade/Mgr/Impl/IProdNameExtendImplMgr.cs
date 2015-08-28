using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IProdNameExtendImplMgr
    {
        /// <summary>
        /// 查詢語句
        /// </summary>
        /// <param name="beginTimeStart">開始時間段的開始時間</param>
        /// <param name="beginTimeEnd">開始時間段的結束時間</param>
        /// <param name="endTimeStart">結束時間段的開始時間</param>
        /// <param name="endTimeOver">結束時間段的結束時間</param>
        /// <param name="productId">商品id</param>
        /// <returns>符合條件的集合</returns>
        List<ProdNameExtendCustom> Query(ProdNameExtendCustom pec,string ids);

        bool Update(List<ProdNameExtend> prodExt);

        /// <summary>
        /// 更新,保存ProdNameExtend表
        /// </summary>
        /// <param name="pn">需要保存的數據</param>
        /// <returns>保存結果</returns>
        bool SaveByList(List<ProdNameExtendCustom> listpn,Caller caller);

        /// <summary>
        /// 刪除前後綴名稱
        /// </summary>
        /// <param name="days">過期天數(超多多少天的前後綴自動刪除)</param>
        /// <returns>受影響的行數</returns>
        int ResetExtendName(Caller callId, uint product_id = 0);

        /// <summary>
        /// 加上商品前後綴
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        bool AddProductExtentName(Caller caller);

        /// <summary>
        /// 根據條件更新後綴結束時間
        /// </summary>
        /// <param name="pn">ProdNameExtendCustom對象</param>
        /// <returns>更新時間的sql語句</returns>
        bool UpdateTime(List<ProdNameExtendCustom> pns);

        List<ProdNameExtend> QueryByFlag(int flag);
    }
}
