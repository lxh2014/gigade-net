using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao
{
    public class LogInLogeDao:ILogInLogeImplDao
    { 
        private IDBAccess _accessMySql;
        private string tempStr = "";
        public LogInLogeDao(string connectionString)
        {
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Model.Query.LogInLogeQuery> QueryList(LogInLogeQuery logInLogeQuery, out int totalCount)
        {           
            DataTable dt = new DataTable();

            tempStr = string.Format("select mu.user_id,mu.user_username,ml.login_id,ml.login_ipfrom,ml.login_createdate,convert(ml.login_createdate,char(100)) as strlogindate from  manage_login  ml inner join manage_user  mu where ml.user_id = mu.user_id order by login_id desc limit {0},{1}", logInLogeQuery.Start, logInLogeQuery.Limit);                    
     
            dt = _accessMySql.getDataTable(tempStr);
            totalCount = int.Parse(_accessMySql.getDataTable("select count(*) as totalcount from  manage_login  ml inner join manage_user  mu where ml.user_id = mu.user_id").Rows[0][0].ToString());
            return _accessMySql.getObjByTable<Model.Query.LogInLogeQuery>(dt);

        }
    }
}
