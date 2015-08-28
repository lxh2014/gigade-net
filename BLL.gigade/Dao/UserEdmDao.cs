/*
 *hongfei0416j 
 * 092201437
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Common;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    public class UserEdmDao : IUserEdmImplDao
    {
        private IDBAccess _accessMySql;
        private string connStr;
        public UserEdmDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            connStr = connectionString;
        }
        public EdmGroupQuery GetEdmGroupById()
        {
            string strSql = string.Format("select group_id,group_name,group_total_email,group_createdate,group_updatedate from edm_group where group_id=1");
            EdmGroupQuery endGroupInfo = new EdmGroupQuery();
            try
            {
                endGroupInfo = _accessMySql.getSinggleObj<EdmGroupQuery>(strSql.ToString());
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.GetEdmGroupById-->" + ex.Message + "sql--" + strSql, ex);
            }
            return endGroupInfo;
        }
        public void AddEdmGroup()
        {
            string strSql = String.Empty;
            try
            {
                EdmGroupQuery edmGroup = new EdmGroupQuery();
                edmGroup.group_id = 1;
                edmGroup.group_name = "All Member";
                edmGroup.group_total_email = 0;
                edmGroup.group_createdate = (uint)Convert.ToInt32(DateTime.Now.ToString());
                edmGroup.group_updatedate = (uint)Convert.ToInt32(DateTime.Now.ToString());
                edmGroup.Replace4MySQL();
                strSql = string.Format("insert into edm_group (group_id,group_name,group_total_email,group_createdate,group_updatedate) values({0},'{1}',{2},{3},{4})", edmGroup.group_id, edmGroup.group_name, edmGroup.group_total_email, edmGroup.group_createdate, edmGroup.group_updatedate);
                _accessMySql.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.AddEdmGroup-->" + ex.Message + "sql--" + strSql, ex);
            }

        }
        #region 獲取用戶狀態不為5的用戶信息
        public List<Users> GetUserInfo()
        {
            List<Users> userList = new List<Users>();
            string strSql = string.Format("select user_id,user_status,user_name,user_email from users where user_status<>5");
            try
            {
                userList = _accessMySql.getDataTableForObj<Users>(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.GetUserInfo-->" + ex.Message + "sql--" + strSql, ex);
            }
            return userList;
        }
        #endregion

        #region 根據email_address 查詢詳細信息
        public UserEdm GetUserEmail(string email_address)
        {
            UserEdm userEmail = new UserEdm();
            string strSql = string.Format("select email_id,email_name,email_address,email_check,email_click,email_createdate,email_updatedate from edm_email where email_address='{0}'", email_address);
            userEmail = _accessMySql.getSinggleObj<UserEdm>(strSql.ToString());
            return userEmail;
        }
        #endregion
        private uint GetEmailId()
        {
            uint serialvalue = 0;
            string strserialInsert = String.Empty;
            string strSql = String.Empty;
            try
            {
                strSql = string.Format("select serial_value from serial where serial_id=51");
                serialvalue = (uint)Convert.ToInt64(_accessMySql.getDataTable(strSql).Rows[0][0].ToString());

                if (serialvalue == 0)
                {
                    serialvalue = 2;
                    strserialInsert = string.Format("insert into serial (serial_id,serial_value)values(51,{0})", serialvalue);
                    _accessMySql.execCommand(strserialInsert);
                }
                else
                {
                    serialvalue = serialvalue + 1;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.GetEmailId-->" + ex.Message + "sql--" + strSql + strserialInsert, ex);
            }
            return serialvalue;
        }
        public void ModifyEmailId(uint serialvalue)
        {
            string strupdate = string.Format("update serial set serial_value={0} where serial_id={1}", serialvalue, 51);
            try
            {
                _accessMySql.execCommand(strupdate);
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.ModifyEmailId-->" + ex.Message + "sql--" + strupdate, ex);
            }
        }
        public uint AddEmail(string emailName, string userName)
        {

            uint emailId = GetEmailId();
            UserEdm useredm = new UserEdm();
            useredm.email_id = emailId;
            useredm.email_name = userName;
            useredm.email_address = emailName;
            useredm.email_check = 0;
            useredm.email_createdate = (uint)CommonFunction.GetPHPTime();
            useredm.email_updatedate = (uint)CommonFunction.GetPHPTime();
            useredm.Replace4MySQL();
            string strSql = string.Format("insert into edm_email (email_id,email_name,email_address,email_check,email_createdate,email_updatedate)values({0},'{1}','{2}',{3},{4},{5})", useredm.email_id, useredm.email_name, useredm.email_address, useredm.email_check, useredm.email_createdate, useredm.email_updatedate);
            try
            {
                _accessMySql.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.AddEmail-->" + ex.Message + "sql--" + strSql, ex);
            }
            return emailId;
        }
        //edm_group_email表插入數據
        public void AddGroupEmail(uint groupId, uint emailId, string emailName, uint emailStatus = 1)
        {
            string strSql = string.Empty;

            try
            {
                if (emailName.Length>0)
                {
                    string tempStr = emailName.Substring(emailName.Length - 1, 1);
                    if (tempStr == "\\")
                    {
                        emailName = emailName + "\\";
                    }
                }
                uint email_status = (uint)(emailStatus == 1 ? 1 : 2);
                EdmGroupEmail ege = new EdmGroupEmail();
                ege.email_id = emailId;
                ege.group_id = groupId;
                ege.email_name = emailName;
                ege.email_status = email_status;
                ege.email_createdate = (uint)CommonFunction.GetPHPTime();
                ege.email_updatedate = (uint)CommonFunction.GetPHPTime();
                ege.Replace4MySQL();
                strSql = string.Format("insert into edm_group_email (group_id,email_id,email_name,email_status,email_createdate,email_updatedate)values({0},{1},'{2}',{3},{4},{5})", ege.group_id, ege.email_id, ege.email_name, ege.email_status, ege.email_createdate, ege.email_updatedate);
                _accessMySql.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.AddGroupEmail-->" + ex.Message + "sql--" + strSql, ex);
            }

        }
        public EdmGroupEmail getEdmGroupEmail(uint groupid, uint emailId)
        {
            EdmGroupEmail ege = new EdmGroupEmail();
            string strSql = string.Format("select group_id,email_id,email_name,email_status,email_createdate,email_updatedate from edm_group_email where group_id={0} and email_id={1}", groupid, emailId);
            try
            {
                ege = _accessMySql.getSinggleObj<EdmGroupEmail>(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.AddEmail-->" + ex.Message + "sql--" + strSql, ex);
            }
            return ege;
        }
        public void modifyEmailName(uint emailId, string emailName)
        {
            EdmGroupEmail ege = new EdmGroupEmail();
            ege.email_name = emailName;
            ege.email_id = emailId;
            ege.Replace4MySQL();
            string strSql = string.Format("update edm_email set email_name='{0}',email_updatedate={1} where email_id={2}", ege.email_name, CommonFunction.GetPHPTime(), ege.email_id);
            try
            {
                _accessMySql.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.modifyEmailName-->" + ex.Message + "sql--" + strSql, ex);
            }

        }
        public void ModifyGroupEmail(uint groupId, uint emailId, string emailName, uint emailStatus = 1)
        {
            uint email_status = (uint)(emailStatus == 1 ? 1 : 2);
            EdmGroupEmail ege = new EdmGroupEmail();
            ege.email_id = emailId;
            ege.group_id = groupId;
            ege.email_name = emailName;
            ege.email_status = email_status;
            ege.email_updatedate = (uint)CommonFunction.GetPHPTime();
            ege.Replace4MySQL();
            string strSql = string.Format("update edm_group_email set email_name='{0}',email_status={1},email_updatedate={2} where email_id={3} and group_id={4}", ege.email_name, ege.email_status, (uint)CommonFunction.GetPHPTime(), ege.email_id, ege.group_id);
            try
            {
                _accessMySql.execCommand(strSql);
            }
            catch (Exception ex)
            {

                throw new Exception("UserEdmDao.modifyEmailName-->" + ex.Message + "sql--" + strSql, ex);
            }
        }
    }
}
