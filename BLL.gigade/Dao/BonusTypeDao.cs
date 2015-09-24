using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class BonusTypeDao
    {
        private IDBAccess accessMySql;
        public BonusTypeDao(string connectionstring)
        {
            accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }

        public DataTable GetBonusTypeList() 
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("SELECT type_id,type_description  FROM bonus_type ");
                return accessMySql.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("BonusTypeDao-->GetBonusTypeList-->" + ex.Message + sql.ToString(), ex);
            }
        }
    }
}
