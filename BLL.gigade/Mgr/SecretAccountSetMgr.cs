using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Dao;
namespace BLL.gigade.Mgr
{
    public class SecretAccountSetMgr
    {
        private SecretAccountSetDao sasDao;
        public SecretAccountSetMgr(string connectionstring)
        {
            sasDao = new SecretAccountSetDao(connectionstring);
        }
        public DataTable GetSecretSetList(SecretAccountSetQuery sasq, out int totalCount)
        {
            try
            {
                return sasDao.GetSecretSetList(sasq, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetMgr-->GetSecretSetList" + ex.Message, ex);
            }
        }
        public List<SecretAccountSet> GetSecretSetList(SecretAccountSet query)
        {
            try
            {
                return sasDao.GetSecretSetList(query);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetMgr-->GetSecretSetList(SecretAccountSet query)" + ex.Message, ex);
            }
        }
        public int Insert(SecretAccountSet sas)
        {
            try
            {
                return sasDao.Insert(sas);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetMgr-->Insert" + ex.Message, ex);
            }
        }
        public int Update(SecretAccountSet sas)
        {
            try
            {
                return sasDao.Update(sas);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetMgr-->Update" + ex.Message, ex);
            }
        }

        /// <summary>
        /// 查詢登入錯誤時使字段user_login_attempts加1，至5時改變status為停用
        /// </summary>
        /// <returns></returns>
        public int LoginError(SecretAccountSet sas)
        {
            try
            {
                if (sas.user_login_attempts == 5)
                {
                    sas.status = 0;
                }
                return sasDao.Update(sas);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretAccountSetMgr-->UpdateState" + ex.Message, ex);
            }

        }

        public SecretAccountSet Select(SecretAccountSet model)
        {
            try
            {
                return sasDao.Select(model);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogMgr-->Select-->" + ex.Message, ex);
            }
        }
        public SecretAccountSet SelectByUserIP(SecretAccountSet model)
        {
            try
            {
                return sasDao.SelectByUserIP(model);
            }
            catch (Exception ex)
            {
                throw new Exception("SecretInfoLogMgr-->SelectByUserIP-->" + ex.Message, ex);
            }
        }
    }
}
