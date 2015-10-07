using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BLL.gigade.Dao
{
    public class TFunctionDao
    {
        private IDBAccess _access;
        public TFunctionDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public DataTable GetModel(TFunction query)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat(@"SELECT rowid,functionType,functionGroup,functionName,functionCode,iconCls,remark,kuser,kdate,topValue from t_function WHERE 1=1");
                if (query.functionCode != string.Empty)
                {
                    sql.AppendFormat(" AND functionCode='{0}' ", query.functionCode);
                }
                if (query.functionName != string.Empty)
                {
                    sql.AppendFormat(" AND functionName='{0}'",query.functionName);
                }
                return _access.getDataTable(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("TFunctionDao-->GetModel-->" + ex.Message, ex);
            }
        }
    }
}
