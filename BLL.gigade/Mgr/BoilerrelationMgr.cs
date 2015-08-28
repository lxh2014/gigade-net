using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class BoilerrelationMgr : IBoilerrelationImplMgr
    {
        private IBoilerrelationImplDao _boilLation;
        public BoilerrelationMgr(string connectionString)
        {
          _boilLation = new BoilerrelationDao(connectionString);
        }

        public int GetintoBoilerrelation(DataRow[] dr,out int total)
        {
            try
            {
                return _boilLation.GetintoBoilerrelation(dr,out total);
            }
            catch (Exception ex)
            {
                throw new Exception("BoilerrelationMgr-->GetintoBoilerrelation-->" + ex.Message, ex);
            }
        }


        public List<Model.Query.boilerrelationQuery> QueryBoilerRelationAll(Model.Query.boilerrelationQuery boilQuery, out int totalCount)
        {
            try
            {
                return _boilLation.QueryBoilerRelationAll(boilQuery, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("BoilerrelationMgr-->QueryBoilerRelationAll-->" + ex.Message, ex);
            }
        }
    }
}
