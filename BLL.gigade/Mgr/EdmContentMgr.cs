using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System.Text.RegularExpressions;
using DBAccess;
using System.Data;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class EdmContentMgr : IEdmContentImplMgr
    {
        private IEdmContentImplDao _IEdmContentDao;
        private EdmContentDao _edmContentDao;
        private EdmEmailDao _edmEmailDao;
        private UsersDao _usersDao;
        private VipUserDao _vipUserDao;
        private IDBAccess _accessMySql;
        private EdmGroupEmailDao _edmGroupEmailDao;
        private string conn;
        public EdmContentMgr(string connectionString)
        {
            _IEdmContentDao = new EdmContentDao(connectionString);
            _edmContentDao = new EdmContentDao(connectionString);
            _edmEmailDao = new EdmEmailDao(connectionString);
            _vipUserDao = new VipUserDao(connectionString);
            _usersDao = new UsersDao(connectionString);
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
            _edmGroupEmailDao = new EdmGroupEmailDao(connectionString);
            conn = connectionString;
        }

        public List<Model.Query.EdmContentQuery> GetEdmContentList(Model.Query.EdmContentQuery store, out int totalCount)
        {
            try
            {
                return _IEdmContentDao.GetEdmContentList(store, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentMgr-->GetSitePageList-->" + ex.Message, ex);
            }
        }


        public int DeleteEdm(int contentId)
        {
            try
            {
                return _IEdmContentDao.DeleteEdm(contentId);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentMgr-->DeleteEdm-->" + ex.Message, ex);
            }
        }
        public int EdmContentSave(EdmContentQuery store)
        {
            try
            {
                return _IEdmContentDao.EdmContentSave(store);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentMgr-->EdmContentSave-->" + ex.Message, ex);
            }
        }
        public List<EdmContentQuery> GetEdmGroup()
        {
            try
            {
                return _IEdmContentDao.GetEdmGroup();
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentMgr-->GetEdmGroup-->" + ex.Message, ex);
            }
        }
        public EdmContentQuery GetEdmContentById(EdmContentQuery query)
        {
            try
            {
                return _IEdmContentDao.GetEdmContentById(query);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentMgr-->GetEdmContentById-->" + ex.Message, ex);
            }
        }
        public string GetEdmContent()
        {
            try
            {

                List<EdmContentQuery> store = _IEdmContentDao.GetEdmContent();
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,data:["));

                foreach (EdmContentQuery para in store)
                {

                    stb.Append("{");
                    if (para.content_title.EndsWith("\\"))
                    {
                        para.content_title += "\\\\\\ ";
                    }
                    stb.Append(string.Format("\"info_id\":\"{0}\",\"info_name\":\"{1}\"", para.content_id, para.content_title)); //, para.content_id, Regex.Replace(para.content_title, "\\$", "\\")
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentMgr-->GetEdmContent-->" + ex.Message, ex);
            }
        }

        public int CancelEdm(string mail, uint update_id, out uint vid)
        {
            int result = 0;
            string sqlEmail = string.Empty;
            string sqlUpdate = string.Empty;
            uint userID = 0;
            vid = 0;
            int sum = 0;
            uint status = 0;
            VipUserQuery query = new VipUserQuery();
            try
            {
                if (_edmEmailDao.GetModel(mail) == null)//郵箱不存在
                {
                    result = -1;
                    return result;
                }
                else
                {
                    result = _IEdmContentDao.CancelEdm(mail);
                    int num = updateGroupCount(mail);
                    if (result > 0 && num > 0)
                    {
                        DataTable dt = _accessMySql.getDataTable(_usersDao.GetUserIDbyEmail(mail));//判斷郵箱是否有對應的用戶id
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            userID = Convert.ToUInt32(dt.Rows[0][0]);
                        }
                        if (string.IsNullOrEmpty(userID.ToString()) || userID == 0)
                        {
                            result = -2;
                            return result;//郵箱不存在對應的用戶
                        }
                        else
                        {
                            sqlEmail = _vipUserDao.SelectEmail(userID);//根據用戶id查詢郵箱是否已經加入黑名單
                            DataTable temail = _accessMySql.getDataTable(sqlEmail);
                            uint phpTime = (uint)Common.CommonFunction.GetPHPTime();
                            #region 將取消電子報的時間和人員加入會員生活信息表（user_life）edit by shuangshuang0420j 20150814 11:00
                            //將取消電子報的時間和人員加入會員生活信息表（user_life）edit by shuangshuang0420j 20150814 11:00
                            UserLifeDao _userLifeDao = new UserLifeDao(conn);
                           
                            _accessMySql.execCommand(_userLifeDao.UpdateEdmTime(userID, phpTime, (int)update_id));

                            #endregion
                            if (temail != null && temail.Rows.Count > 0)
                            {
                                sum = Convert.ToInt32(temail.Rows[0][0]);
                            }
                            if (sum > 0)
                            {
                                status = Convert.ToUInt32(temail.Rows[0][1]);
                                vid = Convert.ToUInt32(temail.Rows[0][2]);
                                if (status == 0)
                                {
                                    result = -3;
                                    return result;//該用戶已加入黑名單且狀態為解鎖
                                }
                                else if (status == 1)
                                {
                                    result = -4;
                                    return result;//該用戶已加入黑名單,狀態為鎖定
                                }
                            }
                            else
                            {
                                query.vuser_email = mail;
                                query.v_id = userID;
                                query.create_id = update_id;
                                query.update_id = update_id;
                                query.createdate = phpTime;
                                query.updatedate = phpTime;
                                sqlUpdate = _vipUserDao.UpdateBlackList(query);
                                result = _accessMySql.execCommand(sqlUpdate);
                                return result;
                            }

                        }
                    }
                    else
                    {
                        result = -5;
                    }
                    return result;
                }


            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentMgr-->CancelEdm-->" + ex.Message, ex);
            }
        }
        public int EditStatus(EdmContentQuery query)
        {
            try
            {
                string sql = _IEdmContentDao.EditStatus(query);
                int i = _accessMySql.execCommand(sql);
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentMgr-->EditStatus-->" + ex.Message, ex);
            }
        }

        public DataTable GetAllTestEmail()
        {
            try
            {
                return _edmContentDao.GetAllTestEmail();
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentMgr-->GetAllTestEmail-->" + ex.Message, ex);
            }
        }
        public DataTable GetTestEmailById(uint content_id)
        {
            try
            {
                return _edmContentDao.GetTestEmailById(content_id);
            }
            catch (Exception ex)
            {
                throw new Exception("EdmContentMgr-->GetTestEmailById-->" + ex.Message, ex);
            }
        }
        public int updateGroupCount(string email)
        {
            DataTable gids = new DataTable();
            EdmGroupQuery query = new EdmGroupQuery();
            int num = 0;
            try
            {
                gids = _edmGroupEmailDao.GetGroupID(email);
                if (gids != null && gids.Rows.Count > 0)
                {
                    for (int i = 0; i < gids.Rows.Count; i++)
                    {
                        int group_id = Convert.ToInt32(gids.Rows[i][0]);
                        DataTable count = new DataTable();
                        count = _edmGroupEmailDao.getCount(group_id);
                        if (count != null && count.Rows.Count > 0)
                        {
                            query.group_total_email = Convert.ToUInt32(count.Rows[0][0]);
                            query.group_id = Convert.ToUInt32(group_id);
                            num = _edmGroupEmailDao.updateEdmGroupCount(query);
                            if (num > 0)
                            {
                                continue;
                            }
                            else
                            {
                                return num;
                            }
                        }
                    }
                }
                return num;
            }
            catch (Exception ex)
            {

                throw new Exception("EdmContentMgr-->updateGroupCount-->" + ex.Message, ex);
            }

        }
    }
}
