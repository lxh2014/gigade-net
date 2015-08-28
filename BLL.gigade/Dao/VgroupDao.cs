/*
* 文件名稱 :VgroupDao.cs
* 文件功能描述 :供應商群組
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :shiwei0620j
* 版本資訊 : 
* 日期 : 2014/08/18
* 修改備註 : 
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;
using BLL.gigade.Model;
using System.Data.SqlClient;
using System.Data;

namespace BLL.gigade.Dao
{
    class VgroupDao : IVgroupImplDao
    {
        private IDBAccess _access;
        private IDBAccess _accessMySql;

        public VgroupDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.SqlServer, connectionString);
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Vgroup> QueryAll()
        {
            return _accessMySql.getDataTableForObj<Vgroup>("sp_Fgroup_QueryAll_xuanzi0404h", null);
        }

        public List<ManageUser> QueryCallid()
        {
            return _accessMySql.getDataTableForObj<ManageUser>("select user_username as name,user_email as callid from manage_user where user_status = '1'");

        }

        public int Save(Model.Vgroup vg)
        {
            vg.Replace4MySQL();
            SqlParameter[] paras = new SqlParameter[6];
            paras[0] = new SqlParameter("@rowid", vg.rowid);
            paras[1] = new SqlParameter("@groupName", vg.groupName);
            paras[2] = new SqlParameter("@groupCode", vg.groupCode);
            paras[3] = new SqlParameter("@remark", vg.remark);
            paras[4] = new SqlParameter("@kuser", vg.kuser);
            paras[5] = new SqlParameter();
            paras[5].Direction = ParameterDirection.ReturnValue;

            _access.execCommand("sp_Fgroup_Save_xuanzi0404h", paras);
            return Int32.Parse(paras[5].Value.ToString());
        }

        public int Delete(Model.Vgroup vg)
        {
            vg.Replace4MySQL();
            SqlParameter[] paras = new SqlParameter[]{
                new SqlParameter("@rowid",vg.rowid)
            };

            return _access.execCommand("sp_Fgroup_Delete_xuanzi0404h", paras);
        }
    }
}
