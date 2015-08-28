using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
   public class ManageUserMgr:IManageUserImplMgr
    {
       private IManageUserImplDao _manageUserDao;

       public ManageUserMgr(string connectionString)
        {
            _manageUserDao = new ManageUserDao(connectionString);
        }
       public List<ManageUserQuery> GetNameMail(ManageUserQuery query, out int totalcount)
       {
           try
           {
               return _manageUserDao.GetNameMail(query, out totalcount);
           }
           catch (Exception ex)
           {

               throw new Exception("ManageUserMgr-->GetNameMail-->" + ex.Message, ex);
           }
       }
       public List<ManageUser> GetManageUser(ManageUser m)
       {
           try
           {
               return _manageUserDao.GetManageUser(m);
           }
           catch (Exception ex)
           {

               throw new Exception("ManageUserMgr-->GetProductName-->" + ex.Message, ex);
           }
       }
       public int UnlockManagerUser(ManageUser m)
       {
           try
           {
               return _manageUserDao.UnlockManagerUser(m);
           }
           catch (Exception ex)
           {

               throw new Exception("ManageUserMgr-->UnlockManagerUser-->" + ex.Message, ex);
           }
       }
       public List<ManageUserQuery> GetManageUserList(ManageUserQuery query, out int totalcount)
       {
           try
           {
               return _manageUserDao.GetManageUserList(query, out totalcount);
           }
           catch (Exception ex)
           {

               throw new Exception("ManageUserMgr-->GetManageUserList-->" + ex.Message, ex);
           }
       }

       public int ManageUserAdd(ManageUserQuery q)
       {
           try
           {
               return _manageUserDao.ManageUserAdd(q);
           }
           catch (Exception ex)
           {

               throw new Exception("ManageUserMgr-->ManageUserAdd-->" + ex.Message, ex);
           }
       }
       public int ManageUserUpd(ManageUserQuery q)
       {
           try
           {
               return _manageUserDao.ManageUserUpd(q);
           }
           catch (Exception ex)
           {
               throw new Exception("ManageUserMgr-->ManageUserUpd-->" + ex.Message, ex);
           }
       }
       public int UpdPassword(ManageUserQuery q)
       {
           try
           {
               return _manageUserDao.UpdPassword(q);
           }
           catch (Exception ex)
           {
               throw new Exception("ManageUserMgr-->UpdPassword-->" + ex.Message, ex);
           }
       }

       public int UpdStatus(ManageUserQuery query)
       {
           try
           {
               return _manageUserDao.UpdStatus(query);
           }
           catch (Exception ex)
           {
               throw new Exception("ManageUserMgr-->UpdStatus-->" + ex.Message, ex);
           }
       }

       #region 新增檢查email是否存在
       public int CheckEmail(ManageUserQuery query)
       {
           try
           {
               return _manageUserDao.CheckEmail(query);
           }
           catch (Exception ex)
           {
               throw new Exception("ManageUserMgr-->CheckEmail-->" + ex.Message, ex);
           }
       }
       #endregion

       public List<ManageUser> GetUserIdByEmail(string email)
       {
           try
           {
               return _manageUserDao.GetUserIdByEmail(email);
           }
           catch (Exception ex)
           {
               throw new Exception("ManageUserDao-->GetUserIdByEmail" + ex.Message, ex);
           }
       }
    }
}
