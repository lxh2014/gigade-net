using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Mgr
{
    public class VipUserMgr
    {
        private VipUserDao _vipUserDao;
        private IDBAccess _dbAccess;
        private UsersDao _usersDao;
        private string conn;

        public VipUserMgr(string connStr)
        {
            _vipUserDao = new VipUserDao(connStr);
            _usersDao = new UsersDao(connStr);
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connStr);
            conn = connStr;
        }
        public DataTable GetBlackList(VipUserQuery query, out int totalCount)
        {
            try
            {
                return _vipUserDao.GetBlackList(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("VipUserMgr-->GetBlackList-->" + ex.Message, ex);
            }
        }
        public int UpdateState(VipUserQuery query)
        {
            string sql = string.Empty;
            int result = 0;
            try
            {
                query.updatedate = Convert.ToUInt32(Common.CommonFunction.GetPHPTime(DateTime.Now.ToString()));
                sql = _vipUserDao.UpdateState(query);
                UserLifeDao _userLifeDao = new UserLifeDao(conn);
                if (query.status == 1)//z狀態改為鎖定時記錄會員取消電子報的時間和操作人
                {
                    //將取消電子報的時間和人員加入會員生活信息表（user_life）edit by shuangshuang0420j 20150814 11:00
                  
                    sql = sql + _userLifeDao.UpdateEdmTime(query.user_id, query.updatedate, (int)query.update_id);
                }
                else if (query.user_status == 0)//狀態改為解鎖時清空會員取消電子報的信息
                {
                    sql = sql + _userLifeDao.UpdateEdmTime(query.user_id, 0, (int)query.update_id);
                }
                result = _dbAccess.execCommand(sql);
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception("VipUserMgr-->UpdateState-->" + ex.Message, ex);
            }
        }
        public int UpdateBlackList(string email)
        {
            string sqlEmail = string.Empty;
            string sqlUserID = string.Empty;
            string sqlUpdate = string.Empty;
            uint userID = 0;
            int sum = 0;
            try
            {
                sqlUserID = _usersDao.GetUserIDbyEmail(email);
                DataTable users = _dbAccess.getDataTable(sqlUserID);
                if (users != null && users.Rows.Count > 0)
                {
                    userID = Convert.ToUInt32(users.Rows[0][0]);
                }
                if (string.IsNullOrEmpty(userID.ToString()) || userID == 0)
                {
                    return -1;//郵箱不存在對應的用戶
                }
                else
                {
                    sqlEmail = _vipUserDao.SelectEmail(userID);
                    DataTable temail = _dbAccess.getDataTable(sqlEmail);
                    if (temail != null && temail.Rows.Count > 0)
                    {
                        sum = Convert.ToInt32(temail.Rows[0][0]);
                    }
                    if (sum > 0)
                    {
                        return -2;//該用戶已加入黑名單
                    }
                    else
                    {
                        uint update_id = Convert.ToUInt32((System.Web.HttpContext.Current.Session["caller"] as Caller).user_id.ToString());
                        uint phpTime = (uint)Common.CommonFunction.GetPHPTime();
                        VipUserQuery query = new VipUserQuery();
                        query.vuser_email = email;
                        query.v_id = userID;
                        query.create_id = update_id;
                        query.update_id = update_id;
                        query.createdate = phpTime;
                        query.updatedate = phpTime;
                        sqlUpdate = _vipUserDao.UpdateBlackList(query);
                        //將取消電子報的時間和人員加入會員生活信息表（user_life）edit by shuangshuang0420j 20150814 11:00
                        UserLifeDao _userLifeDao = new UserLifeDao(conn);
                      
                        sqlUpdate = sqlUpdate + _userLifeDao.UpdateEdmTime(userID, phpTime, (int)update_id);

                        return _dbAccess.execCommand(sqlUpdate);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("VipUserMgr-->UpdateBlackList-->" + ex.Message, ex);
            }
        }
        public VipUser GetSingleByID(int v_id)
        {
            try
            {
                return _dbAccess.getSinggleObj<VipUser>(_vipUserDao.GetSingleByID(v_id));
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserMgr-->GetSingleByID-->" + ex.Message, ex);
            }

        }
        public int AddVipUser(VipUserQuery query)//add by chaojie1124j 添加于2015/9/21用於實現添加會員至群組
        {
            try
            {
                return _vipUserDao.AddVipUser(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserMgr-->AddVipUser-->" + ex.Message, ex);
            }

        }
        public int DeleVipUser(VipUserQuery query)//add by chaojie1124j 添加于2015/9/21用於實現刪除會員至群組
        {
            try
            {
                return _vipUserDao.DeleVipUser(query);
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserMgr-->DeleVipUser-->" + ex.Message, ex);
            }
        }
    }
}
