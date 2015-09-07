using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class IpodMgr : IIpodImplMgr
    {
        private IIpodImplDao _IpodDao;

       public IpodMgr(string connectionString)
        {
            _IpodDao = new IpodDao(connectionString);
        }
       public List<IpodQuery> GetIpodList(IpodQuery query, out int totalcount)
        {
            try
            {
                return _IpodDao.GetIpodList(query, out totalcount);
            }
            catch (Exception ex)
            {

                throw new Exception("IpoMgr-->GetIpodList-->" + ex.Message, ex);
            }
        }
       public int GetPodID(IpodQuery query)
       {
           try
           {
               return _IpodDao.GetPodID(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->GetPodID-->" + ex.Message, ex);
           }
       }
       public int AddIpod(IpodQuery query)
       {
           try
           {
               return _IpodDao.AddIpod(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->AddIpod-->" + ex.Message, ex);
           }
       
       }
       public int UpdateIpod(IpodQuery query)
       {
           try
           {
               return _IpodDao.UpdateIpod(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->UpdateIpod-->" + ex.Message, ex);
           }
       }
       public int UpdateIpodCheck(IpodQuery query)
       {
           try
           {
               return _IpodDao.UpdateIpodCheck(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->UpdateIpodCheck-->" + ex.Message, ex);
           }
       }
       public int DeletIpod(IpodQuery query)
       {
           try
           {
               return _IpodDao.DeletIpod(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->DeletIpod-->" + ex.Message, ex);
           }
       }
       public List<IpodQuery> GetIpodListExprot(IpodQuery query)
       {
           try
           {
               return _IpodDao.GetIpodListExprot(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->GetIpodListExprot-->" + ex.Message, ex);
           }
       }
       public bool GetIpodfreight(string po_id, int freight)
       {
           try
           {
               return _IpodDao.GetIpodfreight(po_id, freight);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->GetIpodfreight-->" + ex.Message, ex);
           }
       }
    }
}
