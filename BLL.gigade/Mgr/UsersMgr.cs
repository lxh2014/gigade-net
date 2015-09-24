using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class UsersMgr : IUsersImplMgr
    {
        private IUsersImplDao _usersDao;
        private ISerialImplMgr _serialImplMgr;

        private static int SERIAL_ID_USER = 22;

        public UsersMgr(string connectionString)
        {
            _usersDao = new UsersDao(connectionString);
            _serialImplMgr = new SerialMgr(connectionString);
        }

        #region IUsersImplMgr 成员

        public DataTable Query(string strUserEmail)
        {
            try
            {
                return _usersDao.Query(strUserEmail);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersMgr-->Query-->" + ex.Message, ex);
            }

        }

        public List<Users> Query(Users query)
        {
            return _usersDao.Query(query);
        }

        public int SelSaveID(Users u)
        {
            try
            {
                UInt64 user_id = UInt64.Parse(_serialImplMgr.GetSerialById(SERIAL_ID_USER).Serial_Value.ToString()) + 1;
                _serialImplMgr.Update(new Serial() { Serial_id = SERIAL_ID_USER, Serial_Value = user_id });

                u.user_id = user_id;

                return _usersDao.SelSaveID(u);
            }
            catch (Exception ex)
            {

                throw new Exception("UsersMgr-->SelSaveID-->" + ex.Message, ex);
            }

        }



        #region 獲取會員購買記錄排行+List<Model.Query.UserVipListQuery> GetVipList(Model.Query.UserVipListQuery vipList, ref int totalCount)
        public List<Model.Query.UserVipListQuery> GetVipList(Model.Query.UserVipListQuery vipList, ref int totalCount)
        {
            try
            {
                return _usersDao.GetVipList(vipList, ref totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("UsersMgr-->GetVipList-->" + ex.Message, ex);
            }
        }
        #endregion
        #region 獲取normal，low，ct，ht
        //public UserVipListQuery GetNormalProd(UserVipListQuery uvlq)
        //{
        //    try
        //    {
        //        return _usersDao.GetNormalProd(uvlq);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception("UsersMgr-->GetNormalProd-->" + ex.Message, ex);
        //    }
        //}

        //public UserVipListQuery GetLowProd(UserVipListQuery uvlq)
        //{
        //    try
        //    {
        //        return _usersDao.GetLowProd(uvlq);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception("UsersMgr-->GetLowProd-->" + ex.Message, ex);
        //    }
        //}

        //public UserVipListQuery GetProdCT(UserVipListQuery uvlq)
        //{
        //    try
        //    {
        //        return _usersDao.GetProdCT(uvlq);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception("UsersMgr-->GetProdCT-->" + ex.Message, ex);
        //    }
        //}

        //public UserVipListQuery GetProdHT(UserVipListQuery uvlq)
        //{
        //    try
        //    {
        //        return _usersDao.GetProdHT(uvlq);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception("UsersMgr-->GetProdHT-->" + ex.Message, ex);
        //    }
        //}
        #endregion

        #region 判斷是否是vip會員+DataTable IsVipUserId(uint user_id)
        public DataTable IsVipUserId(uint user_id)
        {
            try
            {
                return _usersDao.IsVipUserId(user_id);
            }
            catch (Exception ex)
            {

                throw new Exception("UsersMgr-->IsVipUserId-->" + ex.Message, ex);
            }
        }

        #endregion


        public DataTable QueryByUserMobile(string userMobile)
        {
            try
            {
                return _usersDao.QueryByUserMobile(userMobile);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersMgr-->QueryByUserMobile-->" + ex.Message, ex);
            }
        }

        public int SaveUserPhone(Model.Query.UserQuery u)
        {
            try
            {
                return _usersDao.SaveUserPhone(u);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersMgr-->SaveUserPhone-->" + ex.Message, ex);
            }
        }




        public string QueryBySID(uint user_id)
        {
            throw new NotImplementedException();
        }

        public Users SelectUseById(int id)
        {
            throw new NotImplementedException();
        }

        public List<UserQuery> GetBonusList(UserQuery query, ref int totalCount)
        {

            try
            {
                return _usersDao.GetBonusList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersMgr-->GetBonusList-->" + ex.Message, ex);
            }
        }

        #endregion

        /// <summary>
        /// TEXT提示框獲得的信息
        /// </summary>
        /// <param name="condition">搜索條件</param>
        /// <returns>符合要求的集合</returns>
        /// add by wangwei0216w 2014/10/27 
        public List<Users> GetUserInfoByTest(string condition)
        {
            return _usersDao.GetUserInfoByTest(condition);
        }


        public List<UserVipListQuery> ExportVipListCsv(UserVipListQuery query)
        {
            try
            {
                return _usersDao.ExportVipListCsv(query);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersMgr-->ExportVipListCsv-->" + ex.Message, ex);
            }

        }
        public List<UserQuery> Query(Model.Custom.Users query)
        {
            try
            {
                return _usersDao.Query(query);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersMgr-->Query-->" + ex.Message, ex);
            }
        }
        public List<Users> GetUser(Users u)
        {
            try
            {
                return _usersDao.GetUser(u);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersMgr-->GetUser-->" + ex.Message, ex);
            }
        }
        public List<UserQuery> GetUserByEmail(string mail, uint group_id)
        {
            try
            {
                return _usersDao.GetUserByEmail(mail, group_id);
            }
            catch (Exception ex)
            {
                throw new Exception("UsersMgr-->GetUserByEmail-->" + ex.Message, ex);
            }
        }
    }
}
