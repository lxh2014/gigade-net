using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using DBAccess;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class RedirectGroupMgr : IRedirectGroupImplMgr
    {
        private IRedirectGroupImplDao _RdGroupdao;
        private IDBAccess _access;
        private string connectionStr;
        public RedirectGroupMgr(string connectionString)
        {
            _RdGroupdao = new RedirectGroupDao(connectionString);
            this.connectionStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public List<RedirectGroupQuery> QueryAll(RedirectGroup query, out int totalCount)
        {
            try
            {
                return _RdGroupdao.QueryAll(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupMgr.QueryAll-->" + ex.Message, ex);
            }
        }



        public int Save(RedirectGroup query)
        {
            MySqlCommand mySqlCmd = new MySqlCommand();
            MySqlConnection mySqlConn = new MySqlConnection(connectionStr);
            StringBuilder sbExSql = new StringBuilder();
            SerialDao _serialDao = new SerialDao(connectionStr);
            int id = 0;
            try
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Closed)
                {
                    mySqlConn.Open();
                }
                mySqlCmd.Connection = mySqlConn;
                //開啟事物
                mySqlCmd.Transaction = mySqlConn.BeginTransaction();
                mySqlCmd.CommandType = System.Data.CommandType.Text;
                StringBuilder Sql = new StringBuilder();
                //修改表serial
                Serial serial = new Serial();
                serial.Serial_id = 3;
                mySqlCmd.CommandText = _serialDao.Update(serial.Serial_id);//獲取鏈接管理在serial裡面的值
                sbExSql.Append(mySqlCmd.CommandText);
                query.group_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());
                sbExSql.Append(_RdGroupdao.Save(query));
                id = _access.execCommand(_RdGroupdao.Save(query));
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("RedirectGroupMgr-->Save-->" + ex.Message + sbExSql.ToString(), ex);
            }
            finally
            {
                if (mySqlConn != null && mySqlConn.State == System.Data.ConnectionState.Open)
                {
                    mySqlConn.Close();
                }
            }
            return id;
        }

        public int Update(RedirectGroup query)
        {
            try
            {
                return _RdGroupdao.Update(query);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupMgr.Update-->" + ex.Message, ex);
            }
        }




        public List<Redirect> QueryRedirectAll(uint group_id)
        {
            try
            {
                return _RdGroupdao.QueryRedirectAll(group_id);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupMgr.QueryRedirectAll-->" + ex.Message, ex);
            }
        }

        public List<RedirectClick> QueryRedirectClictAll(RedirectClickQuery rcModel)
        {
            try
            {
                return _RdGroupdao.QueryRedirectClictAll(rcModel);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupMgr.QueryRedirectClictAll-->" + ex.Message, ex);
            }
        }


        public string GetGroupName(int group_id)
        {
            try
            {
                return _RdGroupdao.GetGroupName(group_id);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectGroupMgr.GetGroupName-->" + ex.Message, ex);
            }
        }
    }
}
