using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    public interface IProdNameExtendImplDao
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
        List<ProdNameExtendCustom> Query(ProdNameExtendCustom pec, string ids);

        int Update(List<ProdNameExtend> prodExts);

        /// <summary>
        /// 更新,保存ProdNameExtend表
        /// </summary>
        /// <param name="pn">需要保存的數據</param>
        /// <returns>保存結果</returns>
        bool SaveByList(List<ProdNameExtendCustom> listpn);

        /// <summary>
        /// 獲得需要去掉前後綴的商品
        /// </summary>
        /// <returns></returns>
        List<ProdNameExtendCustom> GetPastProduct();

        /// <summary>
        /// 查詢需要增加商品前後綴的商品
        /// </summary>
        /// <returns></returns>
        List<ProdNameExtend> QueryStart();

        /// <summary>
        /// 刪除的Sql語句
        /// </summary>
        /// <returns></returns>
        int DeleteExtendName(int days);

        //add by wwei0216w 2014/12/18
        /// <summary>
        /// 根據條件修改時間
        /// </summary>
        /// <returns>更新時間的sql語句</returns>
        bool UpdateTime(List<ProdNameExtendCustom> pns);

        List<ProdNameExtend> QueryByFlag(int flag);

        List<ProdNameExtendCustom> GetPastProductById(uint product_id);//根據條件條查詢固定商品的信息

    }
}
