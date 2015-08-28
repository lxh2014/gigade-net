using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model.Query;
using System.Collections;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class EdmTestMgr
    {
        private EdmTestDao _etestDao;
        private EdmEmailDao _edmemailDao;
        private MySqlDao _mysqlDao;
        private uint id;
        private uint largestid;
        private string name;
        public EdmTestMgr(string connectionString)
        {
            try
            {
                _mysqlDao = new MySqlDao(connectionString);
                _etestDao = new EdmTestDao(connectionString);
                _edmemailDao = new EdmEmailDao(connectionString);
                id = 0;
                largestid = 0;
                name = string.Empty;
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->EdmTestMgr-->", ex);
            }
            
        }
        public DataTable GetEdmTestList(EdmTestQuery query, out int totalCount)
        {
            try
            {
                return _etestDao.GetEdmTestList(query, out totalCount);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->GetEdmTestList-->", ex);
            }
            
        }
        public bool AddEdmTest(EdmTestQuery query,out int msg)
        {
            ArrayList arryList = new ArrayList();
            query.Replace4MySQL();
            msg = 0;
            try
            {
                _edmemailDao.GetData(query.email_address, out largestid, out id, out name);
                if (id != 0)
                {
                    query.email_id = id;
                    if (_etestDao.SelectExists(id))//存在id是true 不存在是false
                    {
                        msg = 1;
                        return false;
                    }
                    else
                    {
                        if (name != query.test_username)
                        {
                            arryList.Add(_etestDao.UpdateEdmEmailName(query));
                        }                      
                            arryList.Add(_etestDao.InsertEdmTest(query));                      
                    }
                }
                else
                {
                    query.email_id = largestid + 1;
                    arryList.Add(_etestDao.InsertEdmEmail(query));
                    arryList.Add(_etestDao.InsertEdmTest(query));
                }               
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->AddEdmTest-->", ex);
            }
            return _mysqlDao.ExcuteSqls(arryList);
        }
        public bool EditEdmTest(EdmTestQuery query,out int msg)
        {
            query.Replace4MySQL();
            msg = 0;
            ArrayList arryList = new ArrayList();
            try
            {
                arryList.Add(_etestDao.UpdateEdmEmailName(query));
                EdmEmail e = _edmemailDao.GetModel(query.email_address);
                if (e == null || e.email_id == query.email_id)
                {
                    arryList.Add(_etestDao.UpdateEdmEmailAddress(query));
                    arryList.Add(_etestDao.UpdateEdmTest(query));
                }
                else
                {
                    msg = 1;
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->EditEdmTest-->", ex);
            }
            return _mysqlDao.ExcuteSqls(arryList);
        }
        public int DeleteEdmTest(int  id)
        {
            try
            {
                return _etestDao.DeleteEdmTest(id);
            }
            catch (Exception ex)
            {

                throw new Exception("EdmTestMgr-->DeleteEdmTest-->", ex);
            }
            
        }
        public List<EdmTestQuery> GetModel(EdmTestQuery query)
        {
            List<EdmTestQuery> model = new List<EdmTestQuery>();
            DataTable dt = new DataTable();
            EdmTestQuery qu = new EdmTestQuery();
            try
            {
                dt = _edmemailDao.GetEmailByID(query.email_id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    
                    qu.email_id = query.email_id;
                    qu.email_address = dt.Rows[0]["email_address"].ToString();
                    qu.test_username = dt.Rows[0]["email_name"].ToString();
                    model.Add(qu);
                }
                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("EdmTestMgr-->GetModel" + ex.Message, ex);
            }
        }
    }
}
