using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class UserLevelLogMgr
    {

        private UserLevelLogDao _userLevelDao;
        public UserLevelLogMgr(string connectionStr)
        {
            _userLevelDao = new UserLevelLogDao(connectionStr);
        }

        public List<UserLevelLogQuery> GetUserLevelLogList(UserLevelLogQuery query, out int totalCount)
        {
            try
            {
                List<UserLevelLogQuery> store = new List<UserLevelLogQuery>();
                store = _userLevelDao.GetUserLevelLogList(query, out totalCount);
                if (query.isSecret)
                {
                    foreach (var item in store)
                    {
                        if (!string.IsNullOrEmpty(item.user_name))
                        {
                            item.user_name = item.user_name.Substring(0, 1) + "**";
                        }
                        if (!string.IsNullOrEmpty(item.user_email))
                        {
                            item.user_email = item.user_email.Split('@')[0] + "@***";
                        }
                    }
                }
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception("UserLevelLogMgr-->GetUserLevelLogList-->" + ex.Message, ex);
            }
        }


    }
}
