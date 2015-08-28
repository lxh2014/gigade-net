using BLL.gigade.Dao;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class DisableKeywordsMgr
    {
        private DBAccess.IDBAccess _dbAccess;
        private DisableKeywordsDao _dkDao;
        public DisableKeywordsMgr(string connectStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectStr);
            _dkDao = new DisableKeywordsDao(connectStr);
        }



        public int GetCount(string keyword)
        {
            int result = 0;
            try
            {
                string sql = _dkDao.GetCount(keyword);
                result = _dbAccess.getDataTable(sql).Rows.Count;//如果大於0表示 已包含
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->GetCount-->" + ex.Message, ex);
            }
        }
        public DataTable GetKeyWordsList(DisableKeywordsQuery query,out int totalCount)
        {
            try
            {
                DataTable store = new DataTable();
                store = _dkDao.GetKeyWordsList(query,out totalCount);
                return store;
            }
            catch (Exception ex)
            {
                throw new Exception("DisableKeywordsDao-->GetKeyWordsList-->" + ex.Message, ex);
            }
        }
        public int AddKeyWords(DisableKeywordsQuery query)
        {
            int result = 0;
            try
            {
                string sql = _dkDao.AddKeyWords(query);
                result = _dbAccess.execCommand(sql);
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->AddKeyWords-->" + ex.Message, ex);
            }
        }
        public int UpdateKeyWords(DisableKeywordsQuery query)
        {
            int result = 0;
            try
            {
                string sql = _dkDao.UpdateKeyWords(query);
                result = _dbAccess.execCommand(sql);
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->AddKeyWords-->" + ex.Message, ex);
            }
        }
        public int DeleteKeyWords(DisableKeywordsQuery query)
        {
            int result = 0;
            string sql = string.Empty;
            int dk_id=0;
            try
            {
                string[] arrID = query.ids.Split(',');
                foreach (var id in arrID)
                {
                    if (int.TryParse(id, out dk_id))
                    {
                        sql = _dkDao.DeleteKeyWords(id);
                        result = _dbAccess.execCommand(sql);
                        if (result > 0)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        return result;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {

                throw new Exception("DisableKeywordsDao-->DeleteKeyWords-->" + ex.Message, ex);
            }
        }


        public string UpdateStatus(DisableKeywordsQuery query)
        {
            string josn;
            string sql="";
            try
            {
                if (query.dk_active == 0)
                {
                    query.dk_active = 1;
                }
                else
                {
                    query.dk_active = 0;
                }
                sql = _dkDao.UpdateStatus(query);
                if (_dbAccess.execCommand(sql)>0)
                {
                    josn = "{success:true}";
                }
                else
                {
                    josn = "{success:false}";
                }
                return josn;
            }
            catch (Exception ex)
            {
                throw new Exception("DisableKeywordsDao-->UpdateStatus-->" + ex.Message + sql, ex);
            }
        }
    }
}
