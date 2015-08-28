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
    public class IpoMgr : IIpoImplMgr
    {
       private IIpoImplDao _IpoDao;

       public IpoMgr(string connectionString)
        {
            _IpoDao = new IpoDao(connectionString);
        }
       public List<IpoQuery> GetIpoList(IpoQuery query, out int totalcount)
        {
            try
            {
                return _IpoDao.GetIpoList(query, out totalcount);
            }
            catch (Exception ex)
            {

                throw new Exception("IpoMgr-->GetIpoList-->" + ex.Message, ex);
            }
        }
       public int AddIpo(IpoQuery query)
       {
           try
           {
               return _IpoDao.AddIpo(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->AddIpo-->" + ex.Message, ex);
           }
       }
       public int UpdateIpo(IpoQuery query)
       {
           try
           {
               return _IpoDao.UpdateIpo(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->UpdateIpo-->" + ex.Message, ex);
           }
       }


       public int SelectIpoCountByIpo(string ipo)
       {
           try
           {
               return _IpoDao.SelectIpoCountByIpo(ipo);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->SelectIpoCountByIpo-->" + ex.Message, ex);
           }
       }
       public int DeletIpo(IpoQuery query)
       {
           try
           {
               return _IpoDao.DeletIpo(query);
           }
           catch (Exception ex)
           {

               throw new Exception("IpoMgr-->DeletIpo-->" + ex.Message, ex);
           }
       
       }
    }
}
