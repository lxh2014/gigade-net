using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
  public  class OrderDetailManagerMgr
    {
      private OrderDetailManagerDao _ODMDao;

      public OrderDetailManagerMgr(string connectionString)
      {
          _ODMDao = new OrderDetailManagerDao(connectionString);
      }
      /// <summary>
      /// 列表頁
      /// </summary>
      /// <param name="query"></param>
      /// <param name="totalCount"></param>
      /// <returns></returns>
     public List<OrderDetailManagerQuery> GetODMList(OrderDetailManagerQuery query, out int totalCount)
      {
          try
          {
              return _ODMDao.GetODMList(query, out totalCount);
          }
          catch (Exception ex)
          {
              throw new Exception("OrderDetailManagerMgr-->GetODMList-->"+ex.Message,ex);
          }
      }
      /// <summary>
      /// 新增
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
     public string InsertODM(OrderDetailManagerQuery query)
     {
         string json = string.Empty;
         try
         {

             //查看是否重複
             DataTable _dt = _ODMDao.IsExist(query);
             if (_dt.Rows.Count==0)//不重複
             {
                 if (_ODMDao.InsertODM(query) > 0)
                 {
                     json = "{success:true}";//新增成功
                 }
                 else
                 {
                     json = "{success:false}";//新增失敗
                 }
             }
             else
             {
                 json = "{success:true,msg:'1'}";//數據重複
             }
             return json;
         }
       
         catch (Exception ex)
         {
             throw new Exception("OrderDetailManagerMgr-->InsertODM-->" + ex.Message, ex);
         }
     }
      /// <summary>
      /// 變更狀態
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
     public int UpODMStatus(OrderDetailManagerQuery query)
     {
         try
         {
             return _ODMDao.UpODMStatus(query);
         }
         catch (Exception ex)
         {
             throw new Exception("OrderDetailManagerMgr-->UpODMStatus-->" + ex.Message, ex);
         }
     }
      /// <summary>
     /// ManageUserStore
      /// </summary>
      /// <returns></returns>
     public List<ManageUserQuery> ManageUserStore()
     {
         try
         {
             return _ODMDao.ManageUserStore();
         }
         catch (Exception ex)
         {
             throw new Exception("OrderDetailManagerMgr-->ManageUserStore-->" + ex.Message, ex);
         }
     }
    }
}
