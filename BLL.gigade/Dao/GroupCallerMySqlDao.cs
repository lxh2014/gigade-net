using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Model;
using BLL.gigade.Dao.Impl;
using System.Data;
using System.Data.SqlClient;

namespace BLL.gigade.Dao
{
    public class GroupCallerMySqlDao : IGroupCallerImplDao
    {
        private IDBAccess _access;

        public GroupCallerMySqlDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql,connectionString);
        }
        public List<groupCaller> QueryCallidById(groupCaller gc)
        {
            return _access.getDataTableForObj<groupCaller>("select callid from t_groupcaller where groupid = " + gc.groupId);
        }

        public int Save(groupCaller gc)
        {
            return _access.execCommand("insert into t_groupcaller(groupid,callid) values('"+gc.groupId+"','"+gc.callid+"')");
        }

        public int Delete(groupCaller gc)
        {
            return _access.execCommand("delete from t_groupcaller where groupid=" + gc.groupId);
        }
    }
}
