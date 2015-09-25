using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Collections;
using System.IO;
using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao
{
   public  class EdmTemplateDao
    {
       private IDBAccess _access;
       private IDBAccess _dbAccess;
       public EdmTemplateDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

       public List<EdmTemplateQuery> GetEdmTemplateList(EdmTemplateQuery query, out int totalCount)
       {
           query.Replace4MySQL();
           StringBuilder str = new StringBuilder();
           StringBuilder strcont = new StringBuilder();
           totalCount = 0;
           try
           {
               str.AppendFormat(" select et.template_id,et.template_name,et.edit_url,et.content_url,et.enabled,mu1.user_username as template_create_user,mu2.user_username as template_update_user,et.template_createdate,et.template_updatedate from edm_template et  ");
               str.Append(" LEFT JOIN manage_user mu1 on mu1.user_id=et.template_create_userid  ");
               str.Append(" LEFT JOIN manage_user mu2 on mu2.user_id=et.template_update_userid ");
               str.Append(" order by enabled desc, template_name ");
               str.Append(strcont);
               if (query.IsPage)
               {
                   StringBuilder strpage = new StringBuilder();
                   StringBuilder strcontpage = new StringBuilder();
                   strpage.AppendFormat(" SELECT count(biao.template_id) as totalCount FROM(select  et.template_id from edm_template et  ");
                   strpage.Append(strcont);
                   strpage.AppendFormat(")as biao ");
                   DataTable _dt = _access.getDataTable(strpage.Append(strcontpage).ToString());
                   if (_dt.Rows.Count > 0)
                   {
                       totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                       str.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                   }
               }
               return _access.getDataTableForObj<EdmTemplateQuery>(str.ToString());// 獲取查詢記錄

           }
           catch (Exception ex)
           {
               throw new Exception("EdmTemplateDao-->GetEdmTemplateList-->" + ex.Message);
           }
       }

       public string UpdateStats_ET(EdmTemplateQuery query)  // add by yachao1120j 2015-9-22
       {
           StringBuilder strSql = new StringBuilder();
           try
           {
               strSql.AppendFormat(@"Update edm_template set enabled='{0}' WHERE template_id='{1}'", query.enabled, query.template_id);
               return strSql.ToString();
           }
           catch (Exception ex)
           {
               throw new Exception("EdmTemplateDao-->UpdateStats_ET-->" + ex.Message + strSql.ToString(), ex);
           }
       }

       //插入人员信息
       public int EdmTemplateInsert(EdmTemplateQuery query)
       {
           StringBuilder sql = new StringBuilder();
           query.Replace4MySQL();
           try
           {
               //sql.Append("insert into edm_template (template_name, edit_url, content_url,template_createdate,template_updatedate,template_create_userid,template_update_userid)values ");
               //sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", query.template_name, query.edit_url, query.content_url, CommonFunction.DateTimeToString(query.template_createdate), CommonFunction.DateTimeToString(query.template_updatedate), query.template_create_userid, query.template_update_userid);

               sql.Append("insert into edm_template (template_name, edit_url, content_url,template_create_userid,template_update_userid)values ");
               sql.AppendFormat("('{0}','{1}','{2}','{3}','{4}')", query.template_name, query.edit_url, query.content_url, query.template_create_userid, query.template_update_userid);

               return _access.execCommand(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("EdmTemplateDao-->EdmTemplateInsert-->" + sql.ToString() + ex.Message);
           }
       }
       //更新信息
       public int EdmTemplateUpdate(EdmTemplateQuery query)
       {
           StringBuilder sql = new StringBuilder();
           query.Replace4MySQL();
           try
           {
               //sql.AppendFormat("update edm_template set template_name = '{0}', edit_url = '{1}', content_url = '{2}',template_update_userid='{3}',template_updatedate='{4}'  where   template_id='{5}' ", query.template_name, query.edit_url, query.content_url, query.template_update_userid, CommonFunction.DateTimeToString(query.template_updatedate), query.template_id);

               sql.AppendFormat("update edm_template set template_name = '{0}', edit_url = '{1}', content_url = '{2}',template_update_userid='{3}'  where   template_id='{4}' ", query.template_name, query.edit_url, query.content_url, query.template_update_userid, query.template_id);
               return _access.execCommand(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("EdmTemplateDao-->EdmTemplateUpdate-->" + sql.ToString() + ex.Message);
           }
       }
    }
}
