using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class SinopacDetailMgr : ISinopacDetailImplMgr
    {
       private ISinopacDetailImplDao _ISinopacDao;
       public SinopacDetailMgr(string connectionString)
        {
            _ISinopacDao = new SinopacDetailDao(connectionString);
        }


       public DataTable GetSinopacDetai(SinopacDetail store,string sql)
       {
           try
           {
               return _ISinopacDao.GetSinopacDetai(store,sql);
           }
           catch (Exception ex)
           {
               throw new Exception("SinopacDetailMgr-->GetSinopacDetai-->" + ex.Message, ex);
           }
       }
    }
}
