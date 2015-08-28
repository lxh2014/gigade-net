using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class TFGroupDao : ITFGroupImplDao
    {
         private IDBAccess _access;

         public TFGroupDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql,connectionString);
        }
         public List<TFGroup> QueryAll(TFGroup m)
         {
             StringBuilder sbSql = new StringBuilder();
             StringBuilder sbKeySql = new StringBuilder();
             sbSql.AppendFormat(@"select rowid,groupName,groupCode,remark,kuser,kdate from t_fgroup");


             return _access.getDataTableForObj<TFGroup>(sbSql.ToString() + sbKeySql.ToString());
         }
    }
}
