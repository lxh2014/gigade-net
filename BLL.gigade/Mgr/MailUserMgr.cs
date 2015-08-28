using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class MailUserMgr : IMailUserImplMgr
    {
        private IMailUserImplDao _mailUserDao;

        public MailUserMgr(string connectionString)
        {
            _mailUserDao = new MailUserDao(connectionString);
        }
        public List<MailUserQuery> GetMailUserStore(MailUserQuery query, out int totalcount)
        {
            try
            {
                return _mailUserDao.GetMailUserStore(query, out totalcount);
            }
            catch (Exception ex)
            {

                throw new Exception("MailUserMgr-->GetMailUserStore-->" + ex.Message, ex);
            }
        }
        public int SaveMailUser(MailUserQuery query)
        {
            try
            {
                return _mailUserDao.SaveMailUser(query);
            }
            catch (Exception ex)
            {

                throw new Exception("MailUserMgr-->SaveMailUser-->" + ex.Message, ex);
            }
        }
        public int DeleteMailUser(MailUserQuery query)
        {
            try
            {
                return _mailUserDao.DeleteMailUser(query);
            }
            catch (Exception ex)
            {

                throw new Exception("MailUserMgr-->DeleteMailUser-->" + ex.Message, ex);
            }
        }
        public int UpdateMailUserStatus(MailUserQuery query)
        {
            try
            {
                return _mailUserDao.UpdateMailUserStatus(query);
            }
            catch (Exception ex)
            {

                throw new Exception("MailUserMgr-->UpdateMailUserStatus-->" + ex.Message, ex);
            }
        }
        public DataTable GetUserInfo(int r_id)
        {
            try
            {
                return _mailUserDao.GetUserInfo(r_id);
            }
            catch (Exception ex)
            {

                throw new Exception("MailUserMgr-->GetUserInfo-->" + ex.Message, ex);
            }
        }
    }
}
