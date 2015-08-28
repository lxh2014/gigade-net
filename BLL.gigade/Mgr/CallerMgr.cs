using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr.Impl
{
    public class CallerMgr : ICallerImplMgr
    {
        private Dao.Impl.ICallerImplDao _callerDao;

        public CallerMgr(string connectionString)
        {
            _callerDao = new Dao.CallerDao(connectionString);
        }


        public Caller Login(string email)
        {
            try
            {
                Caller caller = _callerDao.Login(email);
                return caller;
            }
            catch (Exception ex)
            {

                throw new Exception("CallerMgr-->Login-->" + ex.Message, ex);
            }

        }

        public Caller GetUserById(int user_id)
        {
            try
            {
                Caller caller = _callerDao.GetUserById(user_id);
                return caller;
            }
            catch (Exception ex)
            {

                throw new Exception("CallerMgr-->GetUserById-->" + ex.Message, ex);
            }

        }

        public string Add_Challenge()
        {
            try
            {
                return _callerDao.Add_Challenge();
            }
            catch (Exception ex)
            {

                throw new Exception("CallerMgr-->Add_Challenge-->" + ex.Message, ex);
            }

        }

        public void Kill_Challenge_Id(string challenge_id)
        {
            try
            {
                _callerDao.Kill_Challenge_Id(challenge_id);
            }
            catch (Exception ex)
            {
                throw new Exception("CallerMgr-->Kill_Challenge_Id-->" + ex.Message, ex);
            }

        }

        public string Get_Challenge_Key(string challenge_id)
        {
            try
            {
                return _callerDao.Get_Challenge_Key(challenge_id);
            }
            catch (Exception ex)
            {
                throw new Exception("CallerMgr-->Get_Challenge_Key-->" + ex.Message, ex);
            }

        }

        public void Add_Login_Attempts(int user_id)
        {
            try
            {
                _callerDao.Add_Login_Attempts(user_id);
            }
            catch (Exception ex)
            {
                throw new Exception("CallerMgr-->Add_Login_Attempts-->" + ex.Message, ex);
            }

        }

        public void Add_Manage_Login(int user_id)
        {
            try
            {
                _callerDao.Add_Manage_Login(user_id);
            }
            catch (Exception ex)
            {
                throw new Exception("CallerMgr-->Add_Manage_Login-->" + ex.Message, ex);
            }

        }

        public void Modify_User_Login_Data(int user_id)
        {
            try
            {
                _callerDao.Modify_User_Login_Data(user_id);
            }
            catch (Exception ex)
            {
                throw new Exception("CallerMgr-->Modify_User_Login_Data-->" + ex.Message, ex);
            }

        }

        public void Modify_User_Status(int user_id, int status)
        {
            try
            {
                _callerDao.Modify_User_Status(user_id, status);
            }
            catch (Exception ex)
            {
                throw new Exception("CallerMgr-->Modify_User_Status-->" + ex.Message, ex);
            }

        }

        public void Modify_User_Confirm_Code(int user_id, string user_confirm_code)
        {
            try
            {
                _callerDao.Modify_User_Confirm_Code(user_id, user_confirm_code);
            }
            catch (Exception ex)
            {
                throw new Exception("CallerMgr-->Modify_User_Confirm_Code-->" + ex.Message, ex);
            }

        }

        public void Modify_User_Password(int user_id, string passwd)
        {
            try
            {
                _callerDao.Modify_User_Password(user_id, passwd);
            }
            catch (Exception ex)
            {
                throw new Exception("CallerMgr-->Modify_User_Confirm_Code-->" + ex.Message, ex);
            }
        }




    }
}
