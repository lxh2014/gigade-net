using BLL.gigade.Dao;
using BLL.gigade.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class TFunctionMgr
    {
        private TFunctionDao _tFunctionDao;
        public TFunctionMgr(string connectionStr)
        {
            _tFunctionDao = new TFunctionDao(connectionStr);
        }
        public DataTable GetModel(TFunction query)
        {
            try
            {
                return _tFunctionDao.GetModel(query);
            }
            catch (Exception ex)
            {              
              throw new Exception("TFunctionMgr-->GetModel-->" + ex.Message, ex);
            }
        }
    }
}
