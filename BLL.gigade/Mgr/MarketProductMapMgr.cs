using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Mgr
{
    public class MarketProductMapMgr : IMarketProductMapImplMgr
    {
        private IMarketProductMapImplDao _IMarketProductMapDao;
        private IDBAccess _access;
        private string connStr;
        public MarketProductMapMgr(string connectionString)
        {
            _IMarketProductMapDao = new MarketProductMapDao(connectionString);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public DataTable GetMarketProductMapList(MarketProductMapQuery query, out int totalCount)
        {
            try
            {
                return _IMarketProductMapDao.GetMarketProductMapList(query,out totalCount);
            }
            catch (Exception ex)
            {
              throw new Exception("MarketProductMapMgr-->GetMarketProductMapList" + ex.Message, ex);
            }
        }

        public int SavetMarketProductMap(MarketProductMapQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(_IMarketProductMapDao.SelectProductMapCount(query));
                DataTable _dt = _access.getDataTable(sb.ToString());
                if(Convert.ToInt32(_dt.Rows[0]["num"].ToString()) > 0)
                 return -1;
                sb.Clear();
                sb.Append(_IMarketProductMapDao.SavetMarketProductMap(query));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("MarketProductMapMgr-->SavetMarketProductMap" + ex.Message, ex);
            }
        }
        public int DeleteMarketProductMap(string row_id)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_IMarketProductMapDao.DeleteMarketProductMap(row_id));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex) 
            {
                throw new Exception("MarketProductMapMgr-->DeleteMarketProductMap" + ex.Message, ex);
            }
        }
    }
}
