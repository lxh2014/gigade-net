using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
  public  class VGroupCallerMgr : IVGroupCallerImplMgr
    {
        private IVGroupCallerImplDao _gcDao;
          public VGroupCallerMgr(string connectionString)
        {
            _gcDao = new VGroupCallerMySqlDao(connectionString);
        }
        public string QueryCallidById(Model.groupCaller gc)
        {
            try
            {
                List<groupCaller> gcResult = _gcDao.QueryCallidById(gc);
                StringBuilder stb = new StringBuilder();

                foreach (groupCaller gcs in gcResult)
                {
                    stb.Append(string.Format("{0}", gcs.callid));
                    stb.Append(",");
                }
                return stb.ToString().Length > 0 ? stb.ToString().Substring(0, stb.ToString().Length - 1) : "";
            }
            catch (Exception ex)
            {
                throw new Exception("VGroupCallerMgr-->QueryCallidById-->" + ex.Message, ex);
            }
        }

        public int Save(Model.groupCaller gc)
        {
            try
            {
                return _gcDao.Save(gc);
            }
            catch (Exception ex)
            {
                throw new Exception("VGroupCallerMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public int Delete(Model.groupCaller gc)
        {
            try
            {
                return _gcDao.Delete(gc);
            }
            catch (Exception ex)
            {
                throw new Exception("VGroupCallerMgr-->Delete-->" + ex.Message, ex);
            }
        }
    }
}
