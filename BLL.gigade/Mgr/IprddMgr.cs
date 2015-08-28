using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Mgr
{
   public class IprddMgr:IIprddImplMgr
    {
         private IIprddImplDao _iprddmgr;
         public IprddMgr(string connectionString)
        {
            _iprddmgr = new IprddDao (connectionString);
        }

         #region IIprddImplMgr 成员

         public int InsertIprdd(Iprdd iprd)
         {
             try
             {
                 return _iprddmgr.InsertIprdd(iprd);
             }
             catch (Exception ex)
             {

                 throw new Exception("IprddMgr-->InsertIprdd-->"+ex.Message,ex);
             }
             
         }

         #endregion
    }
}
