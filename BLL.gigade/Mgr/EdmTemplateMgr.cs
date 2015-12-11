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
using System.Data;

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

       public string UpdateStats_ET(EdmTemplateQuery query)  // add by yachao1120j 2015-9-22
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
               sql = _edmtemplatedao.UpdateStats_ET(query);
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
               throw new Exception("EdmTemplateMgr-->UpdateStats_ET-->" + ex.Message + sql, ex);
           }
       }

       //判断是新增 还是 编辑 
       public int SaveEdmTemplateAdd(EdmTemplateQuery query) // add by yachao1120j 2015-9-22
       {
           if (query.template_id == 0)//新增
           {

               return _edmtemplatedao.EdmTemplateInsert(query);
           }
           else//編輯
           {
               return _edmtemplatedao.EdmTemplateUpdate(query);
           }
       }
       /// <summary>
       /// 為true是靜態範本
       /// </summary>
       /// <param name="template_id"></param>
       /// <returns>為true是靜態範本</returns>
       public bool GetStaticTemplate(int template_id)
       {
           bool static_template=true;
           try
           {
               DataTable _dt = _edmtemplatedao.GetStaticTemplate(template_id);
               if (_dt != null && _dt.Rows.Count > 0)
               {
                   if (Convert.ToInt32(_dt.Rows[0][0]) ==0)//動態範本
                   {
                       static_template = false;
                   }
               }
               return static_template;
           }
           catch (Exception ex)
           {
               throw new Exception("EdmTemplateMgr-->GetStaticTemplate-->" + ex.Message +  ex);
           }
       }
  
    }
}
