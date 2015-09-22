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
   public  class EdmTemplateMgr
    {
       private EdmTemplateDao _edmtemplatedao;
       private DBAccess.IDBAccess _dbAccess;
       public EdmTemplateMgr(string connectionString)
       {
           _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
           _edmtemplatedao = new EdmTemplateDao(connectionString);
       }
       public List<EdmTemplateQuery> GetEdmTemplateList(EdmTemplateQuery query, out int totalCount) // add by yachao1120j 2015-9-22
       {
           try
           {
               List<EdmTemplateQuery> store = new List<EdmTemplateQuery>();
               store = _edmtemplatedao.GetEdmTemplateList(query, out totalCount);
               return store;
           }
           catch (Exception ex)
           {
               throw new Exception("EdmTemplateMgr->GetEdmTemplateList" + ex.Message);
           }
       }
    }
}
