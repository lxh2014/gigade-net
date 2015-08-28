using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class IstockChangeMgr
    {
        private IstockChangeDao _IstockDao;
        public IstockChangeMgr(string connectionString)
        {
            _IstockDao = new IstockChangeDao(connectionString);
        }

        public int insert(IstockChange q)
        {
            try
            {
                return _IstockDao.insert(q);
            }
            catch (Exception ex)
            {
                throw new Exception("IstockChangeMgr-->insert-->" + ex.Message, ex);
            }
        }

        public List<IstockChangeQuery> GetIstockChangeList(IstockChangeQuery q, out int totalCount)
        {
            try
            {
                return _IstockDao.GetIstockChangeList(q,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IstockChangeMgr-->GetIstockChangeList-->" + ex.Message, ex);
            }
        }

        public int insertistocksome(CbjobDetailQuery cbQuery)
        {
           try
           {
               return _IstockDao.insertistocksome(cbQuery);
            }
            catch (Exception ex)
            {
                throw new Exception("IstockChangeMgr-->insertistocksome-->" + ex.Message, ex);
            }
        }
    }
}
