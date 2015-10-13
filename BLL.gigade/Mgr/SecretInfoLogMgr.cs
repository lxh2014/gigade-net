using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Mgr
{
    public class SecretInfoLogMgr
    {
        private SecretInfoLogDao _secretlogDao;
        private IParametersrcImplDao _paraDao;
        private string connStr;
        public SecretInfoLogMgr(string connectionStr)
        {
            _secretlogDao = new SecretInfoLogDao(connectionStr);
            _paraDao = new ParametersrcDao(connectionStr);
            connStr = connectionStr;
        }
        public DataTable GetSecretInfoLog(SecretInfoLog query, out int totalCount)
        {
            try
            {
                DataTable _dt = _secretlogDao.GetSecretInfoLog(query, out totalCount);

                ParametersrcDao _parameterDao = new ParametersrcDao(connStr);
                List<Parametersrc> parameterList = _parameterDao.QueryParametersrcByTypes("secret_type");
                _dt.Columns.Add("type_name");
                foreach (DataRow q in _dt.Rows)
                {
                    var alist = parameterList.Find(m => m.ParameterType == "secret_type" && m.ParameterCode == q["type"].ToString());
                    if (alist != null)
                    {
                        q["type_name"] = alist.parameterName;
                    }
                }
                return _dt;
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogMgr-->GetSecretInfoLog-->" + ex.Message, ex);
            }
        }
        public List<SecretInfoLog> GetSecretInfoLog(SecretInfoLog query)
        {
            try
            {
                return _secretlogDao.GetSecretInfoLog(query);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogMgr-->GetSecretInfoLog-->" + ex.Message, ex);
            }
        }
        public List<SecretInfoLog> GetMaxCreateLog(SecretInfoLog query)
        {
            try
            {
                return _secretlogDao.GetMaxCreateLog(query);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogMgr-->GetMaxCreateLog-->" + ex.Message, ex);
            }
        }
        public int InsertSecretInfoLog(SecretInfoLog query)
        {
            try
            {
                return _secretlogDao.InsertSecretInfoLog(query);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogMgr-->InsertSecretInfoLog-->" + ex.Message, ex);
            }
        }
        public int UpdateSecretInfoLog(SecretInfoLog query)
        {
            try
            {
                return _secretlogDao.UpdateSecretInfoLog(query);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogMgr-->UpdateSecretInfoLog-->" + ex.Message, ex);
            }
        }
        public string QuerySecretType(string strParaType, int used = 1)
        {
            try
            {
                List<Parametersrc> paraResult = _paraDao.Query(new Parametersrc { ParameterType = strParaType, Used = used });
                StringBuilder stb = new StringBuilder();

                stb.Append("{");
                stb.Append(string.Format("success:true,items:["));
                stb.Append("{");
                stb.AppendFormat("\"parameterCode\":\"{0}\",\"parameterName\":\"{1}\"", 0, "全部");
                stb.Append("}");
                foreach (Parametersrc para in paraResult)
                {
                    stb.Append("{");
                    stb.Append(string.Format("\"parameterCode\":\"{0}\",\"parameterName\":\"{1}\"", para.ParameterCode, para.parameterName));
                    stb.Append("}");
                }
                stb.Append("]}");
                return stb.ToString().Replace("}{", "},{");
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogMgr-->QuerySecretType-->" + ex.Message, ex);
            }
        }
    }
}
