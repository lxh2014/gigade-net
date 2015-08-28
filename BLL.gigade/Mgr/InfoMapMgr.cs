using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class InfoMapMgr : IInfoMapImplMgr
    {
        IInfoMapImplDao _infoMapDao;
        public InfoMapMgr(string connectionString)
        {
            _infoMapDao = new InfoMapDao(connectionString);
        }
        public List<InfoMapQuery> GetInfoMapList(InfoMapQuery query, out int totalCount)
        {
            try
            {
                return _infoMapDao.GetInfoMapList(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapMgr-->GetInfoMapList-->" + ex.Message, ex);
            }
        }
        public int SaveInfoMap(InfoMapQuery query)
        {
            try
            {
                return _infoMapDao.SaveInfoMap(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapMgr-->SaveInfoMap-->" + ex.Message, ex);
            }
        }
        public int UpdateInfoMap(InfoMapQuery query)
        {
            try
            {
                return _infoMapDao.UpdateInfoMap(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapMgr-->UpdateInfoMap-->" + ex.Message, ex);
            }
        }
        public bool SelectInfoMap(InfoMapQuery query)
        {
            try
            {
                return _infoMapDao.SelectInfoMap(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapMgr-->SelectInfoMap-->" + ex.Message, ex);
            }
        }
        public InfoMapQuery GetOldModel(InfoMapQuery query)
        {
            try
            {
                return _infoMapDao.GetOldModel(query);
            }
            catch (Exception ex)
            {
                throw new Exception("InfoMapMgr-->GetOldModel-->" + ex.Message, ex);
            }
        }
      

    }
}
