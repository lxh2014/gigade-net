/* 
* 文件名稱 :OrderUserReduceMgr.cs 
* 文件功能描述 :促銷減免查詢數據操作 
* 版權宣告 : 
* 開發人員 : shiwei0620j 
* 版本資訊 : 1.0 
* 日期 : 2014/10/17
* 修改人員 : 
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
   public class OrderUserReduceMgr : IOrderUserReduceImplMgr
    {
       private IOrderUserReduceImplDao _IOrderUserReduceDao;
       public OrderUserReduceMgr(string connectionString)
       {
           _IOrderUserReduceDao = new OrderUserReduceDao(connectionString);
       }
       #region 促銷減免查詢列表頁 +List<PromotionsAmountReduceMemberQuery> GetOrderUserReduce()
       public List<PromotionsAmountReduceMemberQuery> GetOrderUserReduce(PromotionsAmountReduceMemberQuery store, out int totalCount)
        {
            try
            {
              return  _IOrderUserReduceDao.GetOrderUserReduce(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderUserReduceMgr-->GetOrderUserReduce-->"+ex.Message,ex);
            }
             
        }
       #endregion
       #region 查詢條件之減免活動 +List<PromotionsAmountReduceMemberQuery> GetReduceStore()
       public List<PromotionsAmountReduceMemberQuery> GetReduceStore()
        {
            try
            {
                return _IOrderUserReduceDao.GetReduceStore();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderUserReduceMgr-->GetReduceStore-->" + ex.Message, ex);
            }
        }
       #endregion
       #region 查詢條件之群組會員 +List<VipUserGroup> GetVipUserGroupStore()
       public List<VipUserGroup> GetVipUserGroupStore()
        {
            try
            {
                return _IOrderUserReduceDao.GetVipUserGroupStore();
            }
            catch (Exception ex)
            {
                throw new Exception("OrderUserReduceMgr-->GetVipUserGroupStore-->" + ex.Message, ex);
            }
        }
       #endregion
    }
}
