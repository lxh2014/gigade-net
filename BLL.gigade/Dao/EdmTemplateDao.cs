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
       public EdmTemplateDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

       public List<EdmTemplateQuery> GetEdmTemplateList(EdmTemplateQuery query, out int totalCount)
       {
           StringBuilder str = new StringBuilder();
           StringBuilder strcont = new StringBuilder();
           totalCount = 0;
           try
           {
               str.AppendFormat(" select template_id,template_name,edit_url,content_url,enabled,template_create_userid,template_update_userid,template_updatedate from edm_template  order by enabled desc, template_name  ");
              // strcont.AppendFormat(" where 1=1 ");
               str.Append(strcont);

               if (query.IsPage)
               {
                   StringBuilder strpage = new StringBuilder();//  
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
    }
}
