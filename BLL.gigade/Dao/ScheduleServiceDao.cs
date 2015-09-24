using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using DBAccess;

namespace BLL.gigade.Dao
{
    public class ScheduleServiceDao
    {
          private IDBAccess _access;
          public ScheduleServiceDao(string connectionstring)
          {
                _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
          }
          public List<ScheduleServiceQuery> GetExeScheduleServiceList(ScheduleServiceQuery query)
          {
              StringBuilder sql = new StringBuilder();
              try
              {
                  sql.AppendFormat("select ");
                  return new List<ScheduleServiceQuery>();
              }
              catch (Exception ex)
              {

                  throw new Exception("ScheduleServiceDao-->GetExeScheduleServiceList-->" + ex.Message, ex);
              }
          }

    }
}
