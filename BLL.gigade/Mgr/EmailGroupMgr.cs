using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BLL.gigade.Mgr
{
 public   class EmailGroupMgr
    {
     private EmailGroupDao _emailGroupDao;
     private MySqlDao _mySqlDao;
     public EmailGroupMgr(string connectionString)
     {
         _emailGroupDao = new EmailGroupDao(connectionString);
         _mySqlDao = new MySqlDao(connectionString);
     }

     public List<EmailGroup> EmailGroupList(EmailGroup query, out int totalCount)
     {
         try
         {
             return _emailGroupDao.EmailGroupList(query, out totalCount);
         }
         catch (Exception ex)
         {
             throw new Exception("EmailGroupMgr-->EmailGroupList-->"+ex.Message,ex);
         }
     }

     public bool ImportEmailList(DataTable _dt,int group_id)
     {
         EmailGroup query = new EmailGroup();
         ArrayList arrList = new ArrayList();
         bool result = true;
         try
         {
             for (int i = 0; i < _dt.Rows.Count; i++)
             {
                 query = new EmailGroup();
                 query.group_id = group_id;
                 query.email_address = _dt.Rows[i][0].ToString();
                 query.name = _dt.Rows[i][1].ToString();
                 arrList.Add(_emailGroupDao.ImportEmailList(query));
             }
             if (arrList.Count > 0)
             {
                 result = _mySqlDao.ExcuteSqlsThrowException(arrList);
             }
             return result;
         }
         catch (Exception ex)
         {
             throw new Exception("EmailGroupMgr-->ImportEmailList-->" + ex.Message, ex);
         }
     }

     public DataTable Export(int group_id)
     {
         try
         {
             return _emailGroupDao.Export(group_id);
         }
         catch (Exception ex)
         {
             throw new Exception("EmailGroupDao-->Export-->" + ex.Message, ex);
         }
     }

     public bool SaveEmailGroup(EmailGroup query)
     {
         ArrayList arrList = new ArrayList();
         try
         {
             if (query.group_id == 0)
             {
                 arrList.Add(_emailGroupDao.InsertEmailGroup(query));
             }
             else
             {
                 arrList.Add(_emailGroupDao.UpdateEmailGroup(query));
             }
             return _mySqlDao.ExcuteSqlsThrowException(arrList);
         }
         catch (Exception ex)
         {
             throw new Exception("EmailGroupDao-->Export-->" + ex.Message, ex);
         }
       
     }




    }
}
