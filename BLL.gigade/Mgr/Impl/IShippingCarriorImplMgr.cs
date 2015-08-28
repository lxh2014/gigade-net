using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr.Impl
{
    public interface IShippingCarriorImplMgr
    {
        /// <summary>
        /// 查詢所有
        /// </summary>
        /// <param name="sc"></param>
        ///  /// <param name="totalCount">數據庫總條數</param>
        /// <returns>ShippingCarrior集合</returns>
        List<Model.Custom.ShippingCarriorCustom> QueryAll(ShippingCarrior sc, out int totalCount);
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sc"></param>
        /// <returns>受影響的行數</returns>
        int Save(ShippingCarrior sc);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sc"></param>sh
        /// <returns>受影響的行數</returns>
        int Update(ShippingCarrior sc);
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="sc"></param>
        /// <returns>受影響的行數</returns>
        int Delete(string rids);
        /// <summary>
        /// 獲取ShippingCarrior表的list
        /// </summary>
        /// <param name="sc">查詢條件</param>
        /// <param name="totalCount">頁數</param>
        /// <returns></returns>
        DataTable GetShippingCarriorList(Model.Query.ShippingCarriorQuery sc ,out int totalCount);
        /// <summary>
        /// 獲取物流名稱
        /// </summary>
        /// <param name="pt">查詢條件</param>
        /// <returns></returns>
        DataTable GetLogisticsName(Parametersrc pt,string name);
        /// <summary>
        /// 物流信息新增保存
        /// </summary>
        /// <param name="sc">保存信息</param>
        /// <returns></returns>
        int LogisticsSave(ShippingCarrior sc);
        /// <summary>
        /// 物流信息修改保存
        /// </summary>
        /// <param name="sc">保存信息</param>
        /// <returns></returns>
        int LogisticsUpdate(ShippingCarrior sc);
        /// <summary>
        /// 物流信息更新狀態
        /// </summary>
        /// <param name="sc">更新信息</param>
        /// <returns></returns>
        int LogisticsUpdateActive(ShippingCarrior sc);
        /// <summary>
        /// 物流信息新增查詢是否存在
        /// </summary>
        /// <param name="sc">限定條件</param>
        /// <returns></returns>
        int LogisticsAddCheck(ShippingCarrior sc);
    }
}
