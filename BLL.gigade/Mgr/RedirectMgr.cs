using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using MySql.Data.MySqlClient;
using DBAccess;
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Mgr
{
    public class RedirectMgr : IRedirectImplMgr
    {
        private IRedirectImplDao _redirectDao;
        private IDBAccess _access;
        private string connectionStr;
        public RedirectMgr(string connectionString)
        {
            _redirectDao = new RedirectDao(connectionString);
            this.connectionStr = connectionString;
            _access = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }
        public DataTable GetRedirectList(RedirectQuery query, out int totalcount)
        {
            try
            {
                return _redirectDao.GetRedirectList(query, out totalcount);
            }
            catch (Exception ex)
            {

                throw new Exception("RedirectMgr-->IRedirectImplDao-->" + ex.Message, ex);
            }

        }
        public List<RedirectQuery> GetRedirect(RedirectQuery query, out int totalcount)
        {
            try
            {
                return _redirectDao.GetRedirect(query, out totalcount);
            }
            catch (Exception ex)
            {

                throw new Exception("RedirectMgr-->IRedirectImplDao-->" + ex.Message, ex);
            }
        }


        public int Save(Model.Redirect query)
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
                serial.Serial_id = 4;
                mySqlCmd.CommandText = _serialDao.Update(serial.Serial_id);//獲取鏈接管理在serial裡面的值
                sbExSql.Append(mySqlCmd.CommandText);
                query.redirect_id = Convert.ToUInt32(mySqlCmd.ExecuteScalar());
                sbExSql.Append(_redirectDao.Save(query));
                id = _access.execCommand(_redirectDao.Save(query));
                mySqlCmd.Transaction.Commit();
            }
            catch (Exception ex)
            {
                mySqlCmd.Transaction.Rollback();
                throw new Exception("RedirectMgr-->Save-->" + ex.Message + sbExSql.ToString(), ex);
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

        public int Update(Model.Redirect query)
        {
            try
            {
                return _redirectDao.Update(query);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectMgr.Update-->" + ex.Message, ex);
            }
        }

        public int EnterInotRedirect(RedirectQuery query)
        {
            try
            {
                return _redirectDao.EnterInotRedirect(query);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectMgr.EnterInotRedirect-->" + ex.Message, ex);
            }
        }
        public string GetSum(RedirectQuery query)
        {
            try
            {
                return _redirectDao.GetSum(query);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectMgr.GetSum-->" + ex.Message, ex);
            }
        }

        public DataTable GetRedirectListCSV(RedirectQuery query)
        {
            try
            {
                return _redirectDao.GetRedirectListCSV(query);
            }
            catch (Exception ex)
            {
                throw new Exception("RedirectMgr.GetRedirectListCSV-->" + ex.Message, ex);
            }
        }
    }
}
