using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Dao.Impl;
using System.Data;

namespace BLL.gigade.Dao
{
    public class ViewCheckAlarmDao:IViewCheckAlarmImplDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public ViewCheckAlarmDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            this.connStr = connectionStr;
        }

        public DataTable QueryStockAlarm()
        {
            return _dbAccess.getDataTable("select * from view_gigade_checkstockalam");
        }

    }
}
