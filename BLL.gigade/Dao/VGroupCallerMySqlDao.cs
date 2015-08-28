using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class VGroupCallerMySqlDao : IVGroupCallerImplDao
    {
        private IDBAccess _access;
        public VGroupCallerMySqlDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql,connectionString);
        }
        public List<Model.groupCaller> QueryCallidById(Model.groupCaller gc)
        {
            return _access.getDataTableForObj<groupCaller>("select callid from t_groupcaller2 where groupid = " + gc.groupId);
        }

        public int Save(Model.groupCaller gc)
        {
            return _access.execCommand("insert into t_groupcaller2(groupid,callid) values('" + gc.groupId + "','" + gc.callid + "')");
        }

        public int Delete(Model.groupCaller gc)
        {
            return _access.execCommand("delete from t_groupcaller2 where groupid=" + gc.groupId);
        }
    }
}
