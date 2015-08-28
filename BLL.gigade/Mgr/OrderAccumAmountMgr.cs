using BLL.gigade.Dao;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model.Query;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Mgr
{
    public class OrderAccumAmountMgr : IOrderAccumAmountImplMgr
    {
        private IOrderAccumAmountImplDao _IOrderAccumAmountDao;
        private IDBAccess _access;
        private string connStr;
        public OrderAccumAmountMgr(string connectionString)
        {
            _IOrderAccumAmountDao = new OrderAccumAmountDao(connectionString);
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
            this.connStr = connectionString;
        }
        public DataTable GetOrderAccumAmountTable(OrderAccumAmountQuery query, out int totalCount)
        {
            try
            {
                return _IOrderAccumAmountDao.GetOrderAccumAmountTable(query, out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccumAmountMgr-->GetOrderAccumAmountTable" + ex.Message, ex);
            }
        }
        public int AddOrderAccumAmount(OrderAccumAmountQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(_IOrderAccumAmountDao.AddOrderAccumAmount(query));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccumAmountMgr-->AddOrderAccumAmount" + ex.Message + sb.ToString(), ex);
            }

        }
        public int UPOrderAccumAmount(OrderAccumAmountQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(_IOrderAccumAmountDao.UPOrderAccumAmount(query));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccumAmountMgr-->UPOrderAccumAmount" + ex.Message + sb.ToString(), ex);
            }

        }
        public int UpdateActive(OrderAccumAmountQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(_IOrderAccumAmountDao.UpdateActive(query));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccumAmountMgr-->UpdateActive" + ex.Message + sb.ToString(), ex);
            }
        }
        public int DelOrderAccumAmount(OrderAccumAmountQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(_IOrderAccumAmountDao.DelOrderAccumAmount(query));
                return _access.execCommand(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("OrderAccumAmountMgr-->DelOrderAccumAmount" + ex.Message + sb.ToString(), ex);
            }
        
        }
    }
}
