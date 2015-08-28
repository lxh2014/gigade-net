using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class FunctionHistoryMgr : IFunctionHistoryImplMgr
    {
        private IFunctionHistoryImplDao _fhDao;
        private string conStr = "";
        public FunctionHistoryMgr(string connectionString)
        {
            _fhDao = new FunctionHistoryDao(connectionString);
            conStr = connectionString;
        }


        public bool Save(FunctionHistory fh)
        {
            try
            {
                if (fh ==null) return false;
                return _fhDao.Save(fh) >= 0;
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionHistory-->Save" + ex.Message,ex);
            }
        }

        public List<FunctionHistoryCustom> Query(int function_id,int start, int limit ,string condition, DateTime timeStart,DateTime timeEnd,out int total)
        {
            try
            {
                return _fhDao.Query(function_id,start,limit,condition,timeStart,timeEnd, out total);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionHistory-->Query" + ex.Message, ex);
            }
        }
    }
}
