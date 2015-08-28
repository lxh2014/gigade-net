using BLL.gigade.Dao;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class IdiffcountbookMgr
    {

        private IdiffcountbookDao idiffDao;

        public IdiffcountbookMgr(string connectionString)
        {
            idiffDao = new IdiffcountbookDao(connectionString);
        }

        public DataTable GetIdiffCountBookList(IdiffcountbookQuery idiffQuery, out int totalCount)
        {
            try
            {
                return idiffDao.GetIdiffCountBookList(idiffQuery, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IdiffcountbookMgr-->GetIdiffCountBookList-->" + ex.Message, ex);
            }
        }
    }
}
