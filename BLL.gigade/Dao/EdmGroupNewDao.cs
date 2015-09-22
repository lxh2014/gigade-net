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
   public  class EdmGroupNewDao
    {
       private IDBAccess _access;
       public EdmGroupNewDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

       public List<EdmGroupNewQuery> GetEdmGroupNewList(EdmGroupNewQuery query, out int totalCount)
       {
           StringBuilder str = new StringBuilder();
           StringBuilder strcont = new StringBuilder();
           totalCount = 0;
           try
           {
               str.AppendFormat(" select group_id,group_name,is_member_edm,enabled from edm_group_new egn ");
               strcont.AppendFormat(" where 1=1 ");

               strcont.AppendFormat(" and egn.group_name like '%{0}%'", query.group_name);

               str.Append(strcont);

               if (query.IsPage)
               {
                   StringBuilder strpage = new StringBuilder();//  
                   StringBuilder strcontpage = new StringBuilder();
                   strpage.AppendFormat("SELECT count(biao.group_id) as totalCount FROM(select  egn.group_id from edm_group_new egn  ");
                   strpage.Append(strcont);
                   strpage.AppendFormat(")as biao ");
                   DataTable _dt = _access.getDataTable(strpage.Append(strcontpage).ToString());
                   if (_dt.Rows.Count > 0)
                   {
                       totalCount = Convert.ToInt32(_dt.Rows[0]["totalCount"]);
                       str.AppendFormat(" limit {0},{1}", query.Start, query.Limit);
                   }
               }
               return _access.getDataTableForObj<EdmGroupNewQuery>(str.ToString());// 獲取查詢記錄

           }
           catch (Exception ex)
           {
               throw new Exception("EdmGroupNewDao-->GetEdmGroupNewList-->" + ex.Message);
           }
       } // add by yachao1120j 2015-9-21


       public string UpdateStatus(EdmGroupNewQuery query)  // add by yachao1120j 2015-9-21
       {
           StringBuilder strSql = new StringBuilder();
           try
           {
               strSql.AppendFormat(@"Update edm_group_new set enabled='{0}' WHERE group_id={1}", query.enabled, query.group_id);
               return strSql.ToString();
           }
           catch (Exception ex)
           {
               throw new Exception("EdmGroupNewDao-->UpdateStatus-->" + ex.Message + strSql.ToString(), ex);
           }
       } 


       //插入人员信息
       public int EdmGroupNewInsert(EdmGroupNewQuery query)
       {
           StringBuilder sql = new StringBuilder();
           query.Replace4MySQL();
           try
           {
               sql.Append("insert into edm_group_new (group_name, is_member_edm, sort_order,description)values ");
               sql.AppendFormat("('{0}','{1}','{2}','{3}')", query.group_name, query.is_member_edm, query.sort_order, query.description);

               return _access.execCommand(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("EdmGroupNewDao-->EdmGroupNewInsert-->" + sql.ToString() + ex.Message);
           }
       }
       //更新人员信息
       public int EdmGroupNewUpdate(EdmGroupNewQuery query)
       {
           StringBuilder sql = new StringBuilder();
           query.Replace4MySQL();
           try
           {
               sql.AppendFormat("update edm_group_new set group_name = '{0}', is_member_edm = '{1}', sort_order = '{2}',description='{3}'  where group_id='{4}' ", query.group_name, query.is_member_edm, query.sort_order, query.description,query.group_id);
               return _access.execCommand(sql.ToString());
           }
           catch (Exception ex)
           {
               throw new Exception("EdmGroupNewDao-->EdmGroupNewUpdate-->" + sql.ToString() + ex.Message);
           }
       }



       //保存新增信息
       //public int SaveEdmGroupNewAdd(EdmGroupNewQuery query)  // add by yachao1120j 2015-9-22
       //{
       //    StringBuilder sql = new StringBuilder();
       //    query.Replace4MySQL();
       //    try
       //    {
       //        sql.Append("insert into edm_group_new (group_name, is_member_edm, sort_order,description)values ");
       //        sql.AppendFormat("('{0}','{1}','{2}','{3}')", query.group_name, query.is_member_edm, query.sort_order, query.description);
       //        return _access.execCommand(sql.ToString());
       //    }
       //    catch (Exception ex)
       //    {
       //        throw new Exception("EdmGroupNewDao-->SaveEdmGroupNewAdd" + ex.Message);
       //    }
       //}


    }
}
