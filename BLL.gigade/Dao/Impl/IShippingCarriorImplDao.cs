using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Model;

namespace BLL.gigade.Dao.Impl
{
    //add by wangwei0216w 2014/11/3
    interface IShippingCarriorImplDao
    {
        /// <summary>
        /// 新增插入信息
        /// </summary>
        /// <param name="sc">一個Shipping_carrior對象</param>
        /// <returns>受影響的行數</returns>
        int InsertShippingCarrior(ShippingCarrior sc);

        /// <summary>
        /// 刪除滿足條件的信息
        /// </summary>
        /// <param name="sc">刪除條件</param>
        /// <returns>受影響的行數</returns>
        int DeleteShippingCarrior(string rids);

        /// <summary>
        /// 查詢符合條件的ShippintCarrior集合
        /// </summary>
        /// <param name="sc">查詢的條件</param>
        /// <returns>符合條件的集合</returns>
        List<Model.Custom.ShippingCarriorCustom> QueryAll(ShippingCarrior sc, out int totalCount);

        /// <summary>
        /// 更新Shiiping_carrior表
        /// </summary>
        /// <param name="sc">需要更新的數據</param>
        /// <returns>受影響的行數</returns>
        int UpdateShippingCarrior(ShippingCarrior sc);
        /// <summary>
        /// 獲取ShippingCarrior表的list
        /// </summary>
        /// <param name="sc">查詢條件</param>
        /// <param name="totalCount">頁數</param>
        /// <returns></returns>
        DataTable GetShippingCarriorList(Model.Query.ShippingCarriorQuery sc,out int totalCount);
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
        /// 物流信息更新保存
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
