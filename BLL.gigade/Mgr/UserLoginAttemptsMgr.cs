using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class UserLoginAttemptsMgr
    {
        private UserLoginAttemptsDao _ulaDao;
        public UserLoginAttemptsMgr(string connectionstring)
        {
            _ulaDao = new UserLoginAttemptsDao(connectionstring);
        }
        public DataTable GetUserLoginAttemptsList(UserLoginAttempts ula, out int totalCount)
        {
            try
            {
                return _ulaDao.GetUserLoginAttemptsList(ula, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("UserLoginAttemptsMgr-->GetUserLoginAttemptsList" + ex.Message, ex);
            }

        }
        /// <summary>
        /// 插入數據
        /// </summary>
        /// <param name="ula"></param>
        /// <returns></returns>
        public int Insert(UserLoginAttempts ula)
        {
            try
            {
                return _ulaDao.Insert(ula);
            }
            catch (Exception ex)
            {
                throw new Exception("UserLoginAttemptsMgr-->Insert" + ex.Message, ex);
            }
        }
    }
}
