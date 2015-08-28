/*
* 文件名稱 :ConsumeStatisticMgr.cs
* 文件功能描述 :處理會員消費金額統計邏輯
* 版權宣告 :鄭州分公司
* 開發人員 : 文博
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改人員 :無
* 版本資訊 : 1.0
* 日期 : 2015-7-28
* 修改備註 :無
 */
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class ConsumeStatisticMgr
    {
        string connectionstring = string.Empty;
        ConsumeStatisticDao dao;
        public ConsumeStatisticMgr(string _connectionstring)
        {
            this.connectionstring = _connectionstring;
            dao = new ConsumeStatisticDao(connectionstring);
        }
        #region 獲取會員消費統計列表數據 + GetUserOrdersSubtotal()
        public List<UserOrdersSubtotalQuery> GetUserOrdersSubtotal(UserOrdersSubtotalQuery query, out int totalCount)
        {
            try
            {
                return dao.GetUserOrdersSubtotal(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ConsumeStatisticMgr->GetUserOrdersSubtotal:" + ex.Message);
            }
        } 
        #endregion
    }
}
