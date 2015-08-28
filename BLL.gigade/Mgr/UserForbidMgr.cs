using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
    public class UserForbidMgr : IUserForbidImplMgr
    {
        private IUserForbidImplDao _IuserForbidDao;
        public UserForbidMgr(string connectionstring)
        {
            _IuserForbidDao = new UserForbidDao(connectionstring);
        }
        /// <summary>
        /// 黑名單列表頁
        /// </summary>
        /// <param name="store"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<UserForbidQuery> GetUserForbidList(UserForbidQuery store, out int totalCount)
        {
            try
            {
                return _IuserForbidDao.GetUserForbidList(store, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("UserForbidMgr-->GetUserForbidList-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 新增黑名單
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UserForbidInsert(UserForbidQuery query)
        {
            try
            {
                return _IuserForbidDao.UserForbidInsert(query);
            }
            catch (Exception ex)
            {

                throw new Exception("UserForbidMgr-->UserForbidInsert-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 刪除黑名單
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int UserForbidDelete(UserForbidQuery query)
        {
            try
            {
                return _IuserForbidDao.UserForbidDelete(query);
            }
            catch (Exception ex)
            {

                throw new Exception("UserForbidMgr-->UserForbidDelete-->" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 判斷黑名單中是否已經存在IP
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public int GetUserForbidIp(UserForbidQuery query)
        {
            try
            {
                return _IuserForbidDao.GetUserForbidIp(query);
            }
            catch (Exception ex)
            {

                throw new Exception("UserForbidMgr-->GetUserForbidIp-->" + ex.Message, ex);
            }
        }
    }
}
