using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Mgr
{
   public  class EdmGroupNewMgr
    {
       private EdmGroupNewDao _edmgroupdao;
       private DBAccess.IDBAccess _dbAccess;
       public EdmGroupNewMgr(string connectionString)
       {
           _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
           _edmgroupdao = new EdmGroupNewDao(connectionString);
       }
       public List<EdmGroupNewQuery> GetEdmGroupNewList(EdmGroupNewQuery query, out int totalCount) // add by yachao1120j 2015-9-21
       {
           try
           {
               List<EdmGroupNewQuery> store = new List<EdmGroupNewQuery>();
               store = _edmgroupdao.GetEdmGroupNewList(query, out totalCount);
               foreach(var item in store)
               {
                   if (item.is_member_edm == 0) 
                   {
                       item.is_member_edm_string = "";
                   }
                   if (item.is_member_edm == 1)
                   {
                       item.is_member_edm_string = "會員電子報";
                   }
               }
               return store;
           }
           catch (Exception ex)
           {
               throw new Exception("EdmGroupNewMgr->GetEdmGroupNewList" + ex.Message);
           }
       }
       public string UpdateStatus(EdmGroupNewQuery query)  // add by yachao1120j 2015-9-21
       {
           string josn;
           string sql = "";
           try
           {
               if (query.enabled == 0)
               {
                   query.enabled = 1;
               }
               else
               {
                   query.enabled = 0;
               }
               sql = _edmgroupdao.UpdateStatus(query);
               if (_dbAccess.execCommand(sql) > 0)
               {
                   josn = "{success:true}";
               }
               else
               {
                   josn = "{success:false}";
               }
               return josn;
           }
           catch (Exception ex)
           {
               throw new Exception("EdmGroupNewMgr-->UpdateStatus-->" + ex.Message + sql, ex);
           }
       }
       //判断是新增 还是 编辑 
       public int SaveEdmGroupNewAdd(EdmGroupNewQuery query)
       {
           if (query.group_id == 0)//新增
           {
               return _edmgroupdao.EdmGroupNewInsert(query);
           }
           else//編輯
           {
               return _edmgroupdao.EdmGroupNewUpdate(query);
           }
       }
  
    }
}
