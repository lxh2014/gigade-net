using BLL.gigade.Dao;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class GroupCommitteContactMgr
    {
        private IDBAccess _accessMySql;
        private GroupCommitteContactDao _gccDao;
        public GroupCommitteContactMgr(string connectionStr)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            _gccDao = new GroupCommitteContactDao(connectionStr);
        }
        public List<GroupCommitteContact> GetGroupCommitteContact(GroupCommitteContact query)
        {
            List<GroupCommitteContact> store = new List<GroupCommitteContact>();
            try
            {
                string sql = _gccDao.GetGroupCommitteContact(query);
                store = _accessMySql.getDataTableForObj<GroupCommitteContact>(sql);
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception(" GroupCommitteContactMgr -->GetGroupCommitteContact " + ex.Message, ex);
            }
        }
        public int SaveGCC(GroupCommitteContact query)
        {
            try
            {
                int i = _accessMySql.execCommand(_gccDao.SaveGCC(query));
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception(" GroupCommitteContactMgr -->SaveGCC " + ex.Message, ex);
            }
        }
        public int UpdateGCC(GroupCommitteContact query)
        {
            try
            {
                int i = _accessMySql.execCommand(_gccDao.UpdateGCC(query));
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception(" GroupCommitteContactMgr -->UpdateGCC " + ex.Message, ex);
            }

        }
        public int DeleteGCC(GroupCommitteContact query)
        {
            try
            {
                int i = _accessMySql.execCommand(_gccDao.DeleteGCC(query));
                return i;
            }
            catch (Exception ex)
            {
                throw new Exception(" GroupCommitteContactMgr -->UpdateGCC " + ex.Message, ex);
            }

        }
    }
}
