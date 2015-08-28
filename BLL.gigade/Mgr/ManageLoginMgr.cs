using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class ManageLoginMgr
    {
        private ManageLoginDao _ManageLoginDao;
        private MySqlDao _mysqlDao;
        public ManageLoginMgr(string connectionStr)
        {
            _ManageLoginDao = new ManageLoginDao(connectionStr);
            _mysqlDao = new MySqlDao(connectionStr);
        }
        public List<ManageLoginQuery> GetManageLoginList(ManageLoginQuery query, out int totalCount)
        {
            try
            {
                return _ManageLoginDao.GetManageLoginList(query,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("ManageLoginMgr-->GetManageLoginList-->"+ex.Message,ex);
            }
        }
    }
}
