using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class GroupCallerMgr :IGroupCallerImplMgr
    {
        private IGroupCallerImplDao _gcDao;

        public GroupCallerMgr(string connectionString)
        {
            _gcDao = new GroupCallerMySqlDao(connectionString);
        }

        #region IGroupCallerImplMgr 成员

        public string QueryCallidById(groupCaller gc)
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
                throw new Exception("GroupCallerMgr-->QueryCallidById-->" + ex.Message, ex);
            }
            
        }

        public int Save(groupCaller gc)
        {
            
            try
            {
                return _gcDao.Save(gc);
            }
            catch (Exception ex)
            {
                throw new Exception("GroupCallerMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public int Delete(groupCaller gc)
        {
            
            try
            {
                return _gcDao.Delete(gc);
            }
            catch (Exception ex)
            {
                throw new Exception("GroupCallerMgr-->Delete-->" + ex.Message, ex);
            }
        }

        #endregion
    }
}
