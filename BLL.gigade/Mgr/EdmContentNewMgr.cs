using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
  public class EdmContentNewMgr
    {

      private EdmContentNewDao _edmContentNewDao;

      public EdmContentNewMgr(string  connectionString)
      {
          _edmContentNewDao = new EdmContentNewDao(connectionString);
      }
      /// <summary>
      /// 電子報列表
      /// </summary>
      /// <param name="query"></param>
      /// <param name="totalCount"></param>
      /// <returns></returns>
      public List<EdmContentNew> GetECNList(EdmContentNew query, out int totalCount)
      {
          try
          {
              return _edmContentNewDao.GetECNList(query, out totalCount);
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->GetECNList-->"+ex.Message,ex);
          }
      }
      /// <summary>
      /// 電子報類型
      /// </summary>
      /// <returns></returns>
      public List<EdmGroupNew> GetEdmGroupNewStore()
      {
          try
          {
              return _edmContentNewDao.GetEdmGroupNewStore();
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->GetEdmGroupNewStore-->" + ex.Message, ex);
          }
      }
      
      //**將它改為EdmTemplate**//todo
      /// <summary>
      /// 電子報模版
      /// </summary>
      /// <returns></returns>
      public List<EdmGroupNew> GetEdmTemplateStore()
      {
          try
          {
              return _edmContentNewDao.GetEdmTemplateStore();
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->GetEdmTemplateStore-->" + ex.Message, ex);
          }
      }
      /// <summary>
      /// 電子報新增/編輯
      /// </summary>
      /// <param name="query"></param>
      /// <returns></returns>
      public string SaveEdmContentNew(EdmContentNew query)
      {
          int result = 0;
          string json = string.Empty;
          try
          {
              if (query.content_id == 0)//新增
              {
                result=  _edmContentNewDao.InsertEdmContentNew(query);
              }
              else
              {
                  result = _edmContentNewDao.UpdateEdmContentNew(query);
              }
              if (result > 0)
              {
                  json = "{success:true}";
              }
              else
              {
                  json = "{success:false}";
              }
              return json;
          }
          catch (Exception ex)
          {
              throw new Exception("EdmContentNewMgr-->SaveEdmContentNew-->" + ex.Message, ex);
          }
      }
    }
}
