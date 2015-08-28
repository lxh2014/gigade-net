using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class AseldMasterMgr : IAseldMasterImplMgr
    {
        private IAseldMasterImplDao _aseldmasterDao;
        public AseldMasterMgr(string connectionStr)
        {
            _aseldmasterDao = new AseldMasterDao(connectionStr);
        }

        public string Insert(AseldMaster m)
        {
            try
            {
                return _aseldmasterDao.Insert(m);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMasterMgr-->Insert-->" + ex.Message, ex);
            }
        }


        public int SelectCount(AseldMaster m)
        {
            try
            {
                return _aseldmasterDao.SelectCount(m);
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMasterMgr-->SelectCount-->" + ex.Message, ex);
            }
        }
    }
}
