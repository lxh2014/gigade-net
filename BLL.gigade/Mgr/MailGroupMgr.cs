using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class MailGroupMgr : IMailGroupImplMgr
    {
        private IMailGroupImplDao _IMailGroupDao;
        private MySqlDao _mysqlDao;
        public MailGroupMgr(string connectionString)
        {
            _IMailGroupDao = new MailGroupDao(connectionString);
            _mysqlDao = new MySqlDao(connectionString);
        }
        public List<MailGroupQuery> MailGroupList(MailGroupQuery query, out int totalCount)
        {
            try
            {
                return _IMailGroupDao.MailGroupList(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->MailGroupList-->"+ex.Message,ex);
            }
        }


        public int SaveMailGroup(MailGroupQuery query)
        {
            try
            {
                return _IMailGroupDao.SaveMailGroup(query);
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->SaveMailGroup-->" + ex.Message, ex);
            }
        }


        public bool DeleteMailGroup(List<MailGroupQuery> list)
        {
            ArrayList arrayList = new ArrayList();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    arrayList.Add(_IMailGroupDao.DeleteMailGroup(list[i]));
                }
                return _mysqlDao.ExcuteSqls(arrayList);
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->DeleteMailGroup-->"+arrayList.ToString()+ ex.Message, ex);
            }
        }


        public int UpMailGroupStatus(MailGroupQuery query)
        {
            try
            {
                return _IMailGroupDao.UpMailGroupStatus(query);
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->UpMailGroupStatus-->" + ex.Message, ex);
            }
        }


        public string QueryUserById(MailGroupMapQuery query)
        {
            try
            {
                List<MailGroupMapQuery> store = _IMailGroupDao.QueryUserById(query);
                StringBuilder stb = new StringBuilder();
                foreach (var item in store)
                {
                    stb.Append(string.Format("{0}", item.user_id));
                    stb.Append(",");
                }
                return stb.ToString().Length > 0 ? stb.ToString().Substring(0, stb.ToString().Length - 1) : "";
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->QueryUserById-->" + ex.Message, ex);
            }
        }


        public int DeleteMailMap(int group_id)
        {
            try
{
                return _IMailGroupDao.DeleteMailMap(group_id);
            }
            catch (Exception ex)
    {
                throw new Exception("MailGroupMgr-->DeleteMailMap-->" + ex.Message, ex);
            }
        }


        public bool SaveMailMap(List<MailGroupMapQuery> list)
        {
            ArrayList arrList = new ArrayList();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    arrList.Add(_IMailGroupDao.SaveMailMap(list[i]));
                }
                return _mysqlDao.ExcuteSqls(arrList);
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->DeleteMailMap-->" + arrList.ToString() + ex.Message, ex);
            }
        }


        public int GetStatus(int user_id)
        {
            try
            {
                return _IMailGroupDao.GetStatus(user_id);
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->GetStatus-->" + ex.Message, ex);
            }
        }


        public List<MailGroupMapQuery> QueryUserInfo(MailGroupMapQuery query)
        {
            try
            {
                List<MailGroupMapQuery> store = _IMailGroupDao.QueryUserById(query);
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception("MailGroupMgr-->QueryUserInfo-->" + ex.Message, ex);
            }
        }
    }
}
