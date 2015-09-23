using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Model;
using System.Collections;

namespace BLL.gigade.Mgr
{
   public class EmailBlockListMgr
    {
       private EmailBlockListDao _emailBlockListDao;
       private EmailBlockLogDao _emailBlockLogeDao;
       private MySqlDao _mysqlDao;
       public EmailBlockListMgr(string sqlConnectionString)
       {
           _emailBlockListDao = new EmailBlockListDao(sqlConnectionString);
           _emailBlockLogeDao = new EmailBlockLogDao(sqlConnectionString);
           _mysqlDao = new MySqlDao(connectionString);
       }
       public DataTable GetEmailBlockList(EmailBlockListQuery query)
       {
           try
           {
               return _emailBlockListDao.GetEmailBlockList(query);
           }
           catch (Exception ex)
           {            
               throw new Exception("EmailBlockListMgr-->GetEmailBlockList-->" + ex.Message, ex);
           }       
       }

       public int Add(EmailBlockListQuery query)
       {
           try
           {
               DataTable model = _emailBlockListDao.GetModel(query.email_address);
               if (model != null && model.Rows.Count > 0)
               {
                   return -1;
               }
               else
               {
                   return _emailBlockListDao.Add(query);
               }
               
           }
           catch (Exception ex)
           {              
               throw new Exception("EmailBlockListMgr-->Add-->" + ex.Message, ex);
           }
       }
       public int Update(EmailBlockListQuery query)
       {
           try
           {
               return _emailBlockListDao.Update(query);

           }
           catch (Exception ex)
           {
               throw new Exception("EmailBlockListMgr-->Update-->" + ex.Message, ex);
           }
       }

       public bool UnBlock(EmailBlockLogQuery query)
       {
           ArrayList arryList = new ArrayList();
           query.Replace4MySQL();
           try
           {
               DataTable model = _emailBlockListDao.GetModel(query.email_address);
               EmailBlockLogQuery logQuery = new EmailBlockLogQuery();
               if (!string.IsNullOrEmpty(model.Rows[0]["email_address"].ToString()))
               {
                   logQuery.email_address = model.Rows[0]["email_address"].ToString();
               }
               if (!string.IsNullOrEmpty(model.Rows[0]["block_createdate"].ToString()))
               {
                   logQuery.block_start = Convert.ToDateTime(Convert.ToDateTime(model.Rows[0]["block_createdate"]).ToString("yyyy-MM-dd hh:mm:ss"));                        
               }
               logQuery.block_end =  Convert.ToDateTime( DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));                  
               if (!string.IsNullOrEmpty(model.Rows[0]["block_reason"].ToString()))
               {
                   logQuery.block_reason = model.Rows[0]["block_reason"].ToString();
               }
               logQuery.unblock_reason = model.Rows[0]["email_address"].ToString();
               if (!string.IsNullOrEmpty(model.Rows[0]["block_create_userid"].ToString()))
               {
                   logQuery.block_create_userid = Convert.ToInt32(model.Rows[0]["block_create_userid"].ToString());
               }
               logQuery.unblock_create_userid = Convert.ToInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
               arryList.Add( _emailBlockLogeDao.AddUnBlockLog(logQuery));
               arryList.Add(_emailBlockListDao.Delete(query.email_address));
               return _mysqlDao.ExcuteSqls(arryList);
           }
           catch (Exception ex)
           {
               throw new Exception("EmailBlockListMgr-->UnBlock-->" + ex.Message, ex);
           }
       }
    }
}
