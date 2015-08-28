using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class NewPromoRecordMgr : INewPromoRecordImplMgr
    {
          private INewPromoRecordImplDao _newRecordDao;
          public NewPromoRecordMgr(string connectionString)
       {
           _newRecordDao = new NewPromoRecordDao(connectionString);
       }

          public List<Model.Query.NewPromoRecordQuery> NewPromoRecordList(Model.Query.NewPromoRecordQuery query, out int totalCount)
          {
              try
              {
                  return _newRecordDao.NewPromoRecordList(query, out totalCount);
              }
              catch (Exception ex)
              {
                  throw new Exception("NewPromoRecordMgr-->NewPromoRecordList-->" + ex.Message, ex);
              }
          }
    }
}
