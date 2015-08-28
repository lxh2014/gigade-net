using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data.SqlClient;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Web;
using BLL.gigade.Common;
namespace BLL.gigade.Dao
{
    public class CallerDao : ICallerImplDao
    {
        private IDBAccess _access;
        public string strTemp { get; set; }
        public int _intCurrentTime { get; set; }
        public string _strIP { get; set; }
        public CallerDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            _intCurrentTime = (int)BLL.gigade.Common.CommonFunction.GetPHPTime();
            //_strIP = BLL.gigade.Common.CommonFunction.GetClientIP();
            _strIP = CommonFunction.GetIP4Address(CommonFunction.GetClientIP());
        }

        public int Save(Caller caller)
        {
            return 0;
        }

        public int Update(Caller caller)
        {
            return 0;
        }

        public int Delete(int rowID)
        {
            return 0;
        }

        public List<Caller> Query(Caller caller, out int totalCount)
        {
            totalCount = 0;
            List<Caller> callers = new List<Caller>();
            return callers;
        }


        public Caller Login(string email)
        {
            strTemp = string.Format("select user_id,user_email,user_username,user_password,user_status,user_login_attempts,user_confirm_code from manage_user where user_email = '{0}'", email);
            return _access.getSinggleObj<Caller>(strTemp);
        }

        /// <summary>
        /// 通過用戶編號得到用戶信息
        /// </summary>
        /// <param name="user_id">用戶編號</param>
        /// <returns></returns>
        public Caller GetUserById(int user_id)
        {
            strTemp = string.Format("select user_id,user_email,user_username,user_password,user_status,user_login_attempts,user_confirm_code from manage_user where user_id = {0}", user_id);
            return _access.getSinggleObj<Caller>(strTemp);
        }

        /// <summary>
        /// 添加輸入密碼錯誤次數
        /// </summary>
        /// <param name="user_id">用戶編號</param>
        public void Add_Login_Attempts(int user_id)
        {
            _access.execCommand(string.Format("update manage_user set user_login_attempts = user_login_attempts + 1 where user_id = {0}", user_id));
        }

        public string Add_Challenge()
        {
            string sChallenge_Id = generate_uniqe_md5();
            string sChallenge_Key = generate_user_rand_string();
            _access.execCommand(string.Format("insert into challenge (challenge_id,challenge_key,challenge_ip,challenge_createdate) values ('{0}','{1}','{2}',{3})", sChallenge_Id, sChallenge_Key, _strIP, _intCurrentTime));
            return sChallenge_Id;
        }

        public void Kill_Challenge_Id(string challenge_id)
        {
            _access.execCommand(string.Format("delete from challenge where challenge_id = '{0}'", challenge_id));
        }

        public string Get_Challenge_Key(string challenge_id)
        {
            string challenge_key = "";
            if (challenge_id != "")
            {
                DataTable dt = _access.getDataTable(string.Format("select challenge_key from challenge where challenge_id = '{0}'", challenge_id));
                if (dt.Rows.Count > 0)
                {
                    challenge_key = dt.Rows[0][0].ToString();
                }
            }
            return challenge_key;

        }

        /// <summary>
        /// 添加登錄記錄
        /// </summary>
        /// <param name="user_id">用戶編號</param>
        public void Add_Manage_Login(int user_id)
        {
            int next_login_sid = serial_get_next_value("serial", 2);
            _access.execCommand(string.Format("insert into manage_login (login_id,user_id,login_ipfrom,login_createdate) values ({0},{1},'{2}',{3})", next_login_sid, user_id, _strIP, _intCurrentTime));
        }

        public void Modify_User_Login_Data(int user_id)
        {
            _access.execCommand(string.Format("update manage_user set user_login_attempts = {0},user_confirm_code = '',user_lastvisit = {1},user_last_login={1} where user_id = {2}", 0, _intCurrentTime, user_id));
        }

        public void Modify_User_Status(int user_id, int status)
        {
            _access.execCommand(string.Format("update manage_user set user_status = {0},user_confirm_code = '',user_updatedate = {1} where user_id = {2}", status, _intCurrentTime, user_id));
        }

        /// <summary>
        /// 異動/修改使用者的確認碼,用來做變更密碼判斷用,並非更改使用者的登入密碼
        /// </summary>
        /// <param name="user_id">使用者編號</param>
        /// <param name="user_confirm_code">使用者確認碼</param>
        public void Modify_User_Confirm_Code(int user_id, string user_confirm_code)
        {
            _access.execCommand(string.Format("update manage_user set user_confirm_code = '{0}' where user_id={1}", user_confirm_code, user_id));
        }


        public void Modify_User_Password(int user_id, string passwd)
        {
            _access.execCommand(string.Format("update manage_user set user_password='{0}',user_confirm_code='',user_updatedate={1} where user_id={2}", passwd, BLL.gigade.Common.CommonFunction.GetPHPTime(), user_id));
        }



        public int serial_get_next_value(string table, int serial_id)
        {
            int serial_value = serial_get_current_value(table, serial_id);
            if (serial_value == 0)
            {
                serial_value = 2;
                serial_insert_value(table, serial_id, serial_value);
            }
            else
            {
                serial_value++;
                serial_update_value(table, serial_id, serial_value);
            }
            return serial_value;
        }

        public int serial_get_current_value(string table, int nid)
        {
            int serial_id = int.Parse(_access.getDataTable(string.Format("select serial_value from {0} where serial_id = {1}", table, nid)).Rows[0][0].ToString());
            if (serial_id == null)
            {
                serial_id = 0;
            }
            return serial_id;
        }

        public void serial_insert_value(string table, int serial_id, int serial_value)
        {
            _access.execCommand(string.Format("insert into {0} (serial_id,serial_value) values ({1},{2})", table, serial_id, serial_value));
        }

        public void serial_update_value(string table, int serial_id, int serial_value)
        {
            _access.execCommand(string.Format("update {0} set serial_value = {1} where serial_id = {2}", table, serial_value, serial_id));
        }

        public string generate_uniqe_md5()
        {
            char[] aChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

            int nMax_Chars = aChars.Count() - 1;
            int nGet_Length = 32;
            string sRand_String = "";
            Random rand = new Random();
            for (int i = 0; i < nGet_Length; i++)
            {
                sRand_String += aChars[rand.Next(0, nMax_Chars)];
            }

            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sRand_String, "md5");
        }

        public string generate_user_rand_string()
        {
            char[] aChars = {'2', '3', '4', '5', '6', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y'};


            int nMax_Chars = aChars.Count() - 1;
            int nGet_Lenght = 6;
            string sRand_String = "";
            Random rand = new Random();
            for (int i = 0; i < nGet_Lenght; i++)
            {
                sRand_String += aChars[rand.Next(0, nMax_Chars)];
            }

            return sRand_String;
        }



    }
}
