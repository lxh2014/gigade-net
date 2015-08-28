using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
  public   class FunctionVGroupMgr : IFunctionVGroupImplMgr
    {
      private IFunctionVGroupImplDao functionVGroupDao;
      public FunctionVGroupMgr(string connectionStr)
      {
          functionVGroupDao = new FunctionVGroupMySqlDao(connectionStr);
      }
        public int Save(Model.FunctionGroup functionGroup)
        {
            try
            {
                return functionVGroupDao.Save(functionGroup);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionVGroupMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public int Delete(int RowId)
        {
            try
            {
                return functionVGroupDao.Delete(RowId);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionVGroupMgr-->Delete-->" + ex.Message, ex);
            }

        }

        public List<Model.Function> CallerAuthorityQuery(Model.Query.AuthorityQuery query)
        {
            try
            {
                return functionVGroupDao.CallerAuthorityQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionVGroupMgr-->CallerAuthorityQuery-->" + ex.Message, ex);

            }
        }

        public List<Model.Function> GroupAuthorityQuery(Model.Query.AuthorityQuery query)
        {
            try
            {
                return functionVGroupDao.GroupAuthorityQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionVGroupMgr-->GroupAuthorityQuery-->" + ex.Message, ex);

            }
        }
    }
}
