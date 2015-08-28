using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
   public class UserEdmMgr:IUserEdmImplMgr
    {
       private IUserEdmImplDao _edmIplDao;
       public UserEdmMgr(string connectionString)
       {
           _edmIplDao = new UserEdmDao(connectionString);
       }
       public string UpdateEdm()
       {
           string str = string.Empty;
           try
           {
               int addCount = 0;
               int UpdateCount = 0;
               int repeatCount = 0;
               EdmGroupQuery edmgroup = _edmIplDao.GetEdmGroupById();
               if (edmgroup == null)
               {
                   _edmIplDao.AddEdmGroup();
               }
               List<Users> UserList = _edmIplDao.GetUserInfo();
               foreach (var userInfo in UserList)
               {
                   string temUserName = userInfo.user_name;
                   string temUserEmail = userInfo.user_email;
                   uint Temp_Order_News = 1;
                   UserEdm edmEmailInfo = _edmIplDao.GetUserEmail(temUserEmail);
                   if (edmEmailInfo == null)
                   {
                       uint emailId = _edmIplDao.AddEmail(temUserEmail, temUserName);
                       if (emailId != 0)
                       {
                           _edmIplDao.ModifyEmailId(emailId);
                           _edmIplDao.AddGroupEmail(1, emailId, temUserName, Temp_Order_News);
                           addCount++;
                       }

                   }
                   else
                   {
                       uint edmEmailId = edmEmailInfo.email_id;
                       string edmEmailName = edmEmailInfo.email_name;
                       EdmGroupEmail ege = _edmIplDao.getEdmGroupEmail(1, edmEmailId);

                       if (ege == null)
                       {
                           _edmIplDao.AddGroupEmail(1, edmEmailId, temUserName, Temp_Order_News);
                           addCount++;
                       }
                       else
                       {
                           if (temUserName != ege.email_name)
                           {
                               _edmIplDao.modifyEmailName(edmEmailId, temUserName);
                           }
                           string temEmailName = ege.email_name;
                           uint status = ege.email_status;
                           if ((temUserName != temEmailName) || status != Temp_Order_News)
                           {
                               _edmIplDao.ModifyGroupEmail(1, edmEmailId, temUserName, Temp_Order_News);
                               UpdateCount++;
                           }
                           else
                           {
                               repeatCount++;
                           }
                       }
                   }
               }
               str = "新增" + addCount + "筆；更新" + UpdateCount + "筆；覆蓋" + repeatCount + "筆";
           }
           catch (Exception ex)
           {
               str = "失敗";
               throw new Exception("UserEdmMgr.UpdateEdm-->" + ex.Message, ex);
           }
           
           return str;
       }
       //public string ccc()
       //{
       //    uint addCount = 0;
       //    uint UpdateCount = 0;
       //    uint repeatCount = 123456;
       //    string str = "新增" + addCount + "更新" + UpdateCount + "覆蓋" + repeatCount;
       //    return str;
       //}
    }
}
