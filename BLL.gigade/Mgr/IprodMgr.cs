using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
namespace BLL.gigade.Mgr
{
    public class IprodMgr : IIprodImplMgr
    {
        private IIprodImplDao _iprodDao;

        public IprodMgr(string connectionStr)
        {
            _iprodDao = new IprodDao(connectionStr);
        }
        public int AddIprod(Iprod iprod)
        {
            try
            {
                return _iprodDao.AddIprod(iprod);
            }
            catch (Exception ex)
            {
                throw new Exception("IprodMgr-->AddIprod-->" + ex.Message, ex);
            }
        
        }
    }
}
