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
    class GroupCallerDao : IGroupCallerImplDao
    {
        private IDBAccess _access;

        public GroupCallerDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.SqlServer,connectionString);
        }

        #region IGroupCallerImplDao 成员

        public List<groupCaller> QueryCallidById(groupCaller gc)
        {
            SqlParameter[] paras = new SqlParameter[]{
                new SqlParameter("@groupId",gc.groupId)
            };
            return _access.getDataTableForObj<groupCaller>("sp_GroupCaller_QueryCallidById_xuanzi0404h", paras);
        }

        public int Save(groupCaller gc)
        {
            SqlParameter[] paras = new SqlParameter[]{
                new SqlParameter("@groupId",gc.groupId),
                new SqlParameter("@callid",gc.callid)
            };

            return _access.execCommand("sp_GroupCaller_Save_xuanzi0404h", paras);
        }

        public int Delete(groupCaller gc)
        {
            SqlParameter[] paras = new SqlParameter[]{
                new SqlParameter("@groupId",gc.groupId)
            };

            return _access.execCommand("sp_GroupCaller_Delete_xuanzi0404h", paras);
        }

        #endregion
    }
}
