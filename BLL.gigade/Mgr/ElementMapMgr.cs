using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
     public  class ElementMapMgr:IElementMapImplMgr
    {
           IElementMapImplDao _ielementmap;
           public ElementMapMgr(string connectionString)
            {
                _ielementmap = new ElementMapDao(connectionString);
            }

           #region 元素關係列表頁
                 public List<ElementMapQuery> GetElementMapList(ElementMapQuery query, out int totalCount)
               {
                   try
                   {
                       return _ielementmap.GetElementMapList(query, out totalCount);
                   }
                   catch (Exception ex)
                   {
                       throw new Exception("ElementMapMgr-->GetElementMapList-->" + ex.Message, ex);
                   }
               }
           #endregion

        #region 元素關係新增
                 public int upElementMap(ElementMapQuery query) 
                 {
                     try
                     {
                         return _ielementmap.upElementMap(query);
                     }
                     catch (Exception ex)
                     {
                         throw new Exception("ElementMapMgr-->upElementMap-->" + ex.Message, ex);
                     }
                 }
                
        #endregion

        #region 元素關係修改
                 public int AddElementMap(ElementMapQuery query) 
                 {
                     try
                     {
                         return _ielementmap.AddElementMap(query);
                     }
                     catch (Exception ex)
                     {
                         throw new Exception("ElementMapMgr-->AddElementMap-->" + ex.Message, ex);
                     }
                 }
        #endregion
                 public bool SelectElementMap(ElementMapQuery query)
                 {
                     return _ielementmap.SelectElementMap(query);
                 }


                 public bool GetAreaCount(int areaId)
                 {

                     try
                     {
                         return _ielementmap.GetAreaCount(areaId);
                     }
                     catch (Exception ex)
                     {
                         throw new Exception("ElementMapMgr-->GetAreaCount-->" + ex.Message, ex);
                     }
                 }
    }
}
