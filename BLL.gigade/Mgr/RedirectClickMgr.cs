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
    public class RedirectClickMgr : IRedirectClickImplMgr
    {
         private IRedirectClickImplDao _redirectClickDao;

         public RedirectClickMgr(string connectionString)
        {
            _redirectClickDao = new RedirectClickDao(connectionString);
        }
         public List<RedirectClickQuery> QueryAllById(RedirectClickQuery query)
        {
            try
            {
                return _redirectClickDao.QueryAllById(query); 
            }
            catch (Exception ex)
            {

                throw new Exception("RedirectClickMgr-->QueryAllById-->" + ex.Message, ex);
            }

        }
         public List<RedirectClickQuery> GetRedirectClick(RedirectClickQuery query)
         {
             try
             {
                 return _redirectClickDao.GetRedirectClick(query);
             }
             catch (Exception ex)
             {

                 throw new Exception("RedirectClickMgr-->GetRedirectClick-->" + ex.Message, ex);
             }
         }
         public RedirectClickQuery ReturnMinClick() 
         {
             try
             {
                 return _redirectClickDao.ReturnMinClick();
             }
             catch (Exception ex)
             {

                 throw new Exception("RedirectClickMgr-->ReturnMinClick-->" + ex.Message, ex);
             }
         
         }
    }
}
