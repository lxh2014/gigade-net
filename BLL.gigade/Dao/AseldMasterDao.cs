using BLL.gigade.Dao.Impl;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Model;
using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.gigade.Dao
{
    public class AseldMasterDao : IAseldMasterImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public AseldMasterDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
         public string Insert(AseldMaster m)
         {
             StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append(@"INSERT INTO aseld_master (assg_id,complete_time,create_time,create_user) VALUES (");
                sql.AppendFormat(@"'{0}','{1}','{2}','{3}') ;", m.assg_id,Common.CommonFunction.DateTimeToString(m.complete_time),Common.CommonFunction.DateTimeToString(m.create_time),m.create_user);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("AseldMasterDao-->Insert-->" + ex.Message + sql.ToString(), ex);
            }
         }



         public int SelectCount(AseldMaster m)
         {
             StringBuilder sql = new StringBuilder();
             try
             {
                 sql.AppendFormat(@"select Count(seld_id) as sum from aseld  where assg_id='{0}' and commodity_type=2 and wust_id !='COM' ", m.assg_id);
                 return int.Parse(_access.getDataTable(sql.ToString()).Rows[0]["sum"].ToString());//返回條數
             }
             catch (Exception ex)
             {
                 throw new Exception("AseldMasterDao-->SelectCount-->" + ex.Message + sql.ToString(), ex);
             }
         }
    }
}
