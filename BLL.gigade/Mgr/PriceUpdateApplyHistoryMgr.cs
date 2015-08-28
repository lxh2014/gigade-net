using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using System.Collections;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class PriceUpdateApplyHistoryMgr : IPriceUpdateApplyHistoryImplMgr
    {
        private IPriceUpdateApplyHistory _pHDao;
        string connectionString = string.Empty;
        public PriceUpdateApplyHistoryMgr(string connectionString)
        {
            _pHDao = new PriceUpdateApplyHistoryDao(connectionString);
            this.connectionString = connectionString;
        }

        public string SaveSql(PriceUpdateApplyHistory priceUpdateApplyHistory)
        {
            
            try
            {
                return _pHDao.Save(priceUpdateApplyHistory);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceUpdateApplyHistoryMgr-->SaveSql-->" + ex.Message, ex);
            }
        }

        public bool Save(List<PriceUpdateApplyHistory> pHList)
        {
            
            try
            {
                ArrayList aList = new ArrayList();
                foreach (var item in pHList)
                {
                    aList.Add(_pHDao.Save(item));
                }
                MySqlDao mySqlDao = new MySqlDao(connectionString);
                return mySqlDao.ExcuteSqls(aList);
            }
            catch (Exception ex)
            {
                throw new Exception("PriceUpdateApplyHistoryMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

    }
}
