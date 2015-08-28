using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class UserRecommendMgr : IUserRecommendIMgr
    {
        private IUserRecommendIDao _UserRDao;
        public UserRecommendMgr(string connectionString)
        {
            _UserRDao = new UserRecommendDao(connectionString);
        }

        public List<UserRecommendQuery> QueryAll(UserRecommendQuery store, out int totalCount)
        {
            try
            {
                return _UserRDao.QueryAll(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("UserRecommendMgr-->QueryAll-->" + ex.Message, ex);
            }
        }
        public DataTable getUserInfo(int id)
        {
            try
            {
                return _UserRDao.getUserInfo(id);
            }
            catch (Exception ex)
            {
                throw new Exception("UserRecommendMgr-->getModel-->" + ex.Message, ex);
            }
        }
    }
}
