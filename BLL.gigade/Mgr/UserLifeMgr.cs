using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace BLL.gigade.Mgr
{
    public class UserLifeMgr
    {
        private Dao.UserLifeDao _userLifeDao;


        public UserLifeMgr(string connectionString)
        {
            _userLifeDao = new Dao.UserLifeDao(connectionString);

        }

        public List<Model.UserLife> GetUserLife(uint userId)
        {
            try
            {
                return _userLifeDao.GetUserLife(userId);
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Number.ToString() + ":UserLifeMgr-->GetUserLife" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("UserLifeMgr-->GetUserLife-->" + ex.Message, ex);
            }
        }
    }
}
