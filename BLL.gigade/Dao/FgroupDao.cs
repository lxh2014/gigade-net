/*
* 文件名稱 :FgroupDao.cs
* 文件功能描述 :群組
* 版權宣告 :
* 開發人員 : 天娥璇子
* 版本資訊 : 1.0
* 日期 : 2013/08/02
* 修改人員 :
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/

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
    class FgroupDao : IFgroupImplDao
    {
        private IDBAccess _access;
        private IDBAccess _accessMySql;

        public FgroupDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.SqlServer, connectionString);
            _accessMySql = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        #region IFgroupImplDao 成员

        public List<Fgroup> QueryAll()
        {
            return _accessMySql.getDataTableForObj<Fgroup>("sp_Fgroup_QueryAll_xuanzi0404h", null);
        }

        public List<Fgroup> Query(string callid, string groupCode)
        {
            return _access.getDataTableForObj<Fgroup>(string.Format("select g.* from t_fgroup g inner join t_groupcaller gc on g.rowid=gc.groupId where gc.callid='{0}' and g.groupCode='{1}' "));
        }
        public int Save(Fgroup fg)
        {
            fg.Replace4MySQL();
            SqlParameter[] paras = new SqlParameter[6];
            paras[0] = new SqlParameter("@rowid", fg.rowid);
            paras[1] = new SqlParameter("@groupName", fg.groupName);
            paras[2] = new SqlParameter("@groupCode", fg.groupCode);
            paras[3] = new SqlParameter("@remark", fg.remark);
            paras[4] = new SqlParameter("@kuser", fg.kuser);
            paras[5] = new SqlParameter();
            paras[5].Direction = ParameterDirection.ReturnValue;

            _access.execCommand("sp_Fgroup_Save_xuanzi0404h", paras);
            return Int32.Parse(paras[5].Value.ToString());
        }

        public int Delete(Fgroup fg)
        {
            fg.Replace4MySQL();
            SqlParameter[] paras = new SqlParameter[]{
                new SqlParameter("@rowid",fg.rowid)
            };

            return _access.execCommand("sp_Fgroup_Delete_xuanzi0404h", paras);
        }

        public List<ManageUser> QueryCallid()
        {
            return _accessMySql.getDataTableForObj<ManageUser>("select user_username as name,user_email as callid from manage_user where user_status = '1'");
        }

        #endregion





        public DataTable GetFgroupList()
        {
            throw new NotImplementedException();
        }

        public DataTable GetUsersByGroupId(int groupid)
        {
            throw new NotImplementedException();
        }


        public DataTable GetAuthorityByGroupId(int groupid)
        {
            throw new NotImplementedException();
        }
        public Fgroup GetSingle(Fgroup model) //add by mengjuan0826j 獲取單個model  根據groupname和groupcode 
        {
            throw new NotImplementedException();
        }
    }
}
