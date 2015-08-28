/*
* 文件名稱 :VgroupMySqlDao.cs
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

namespace BLL.gigade.Dao
{
 public   class VgroupMySqlDao : IVgroupImplDao
     
    {
        private IDBAccess _access;
        string strSql = string.Empty;

        public VgroupMySqlDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Model.Vgroup> QueryAll()
        {
            return _access.getDataTableForObj<Vgroup>("select a.rowid,groupname,groupcode,count(b.rowid) as callid,remark from t_fgroup2 a left join t_groupcaller2  b on a.rowid=b.groupid group by a.rowid,groupname,groupcode,remark");

        }

        public List<Model.ManageUser> QueryCallid()
        {
            //return _access.getDataTableForObj<ManageUser>("select user_username as name,user_email as callid from manage_user where user_status = '1'");
            return _access.getDataTableForObj<ManageUser>("select vendor_name_simple as name,vendor_email as callid from vendor where vendor_status = '1'");
        }

        public int Save(Model.Vgroup vg)
        {
           vg.Replace4MySQL();
            if (vg.rowid == 0)
            {
                if (_access.execCommand(string.Format("select rowid from t_fgroup2 where (groupname='{0}' or groupcode='{1}')", vg.groupName, vg.groupCode)) > 0)
                {
                    return -1;//群組名稱或群組編碼已存在
                }
                else
                {
                    _access.execCommand(string.Format("insert into t_fgroup2(groupname,groupcode,remark,kuser,kdate) values('{0}','{1}','{2}','{3}',now())", vg.groupName, vg.groupCode, vg.remark,vg.kuser));
                    return 1;//新增成功
                }
            }
            else
            {
                if (_access.execCommand(string.Format("select rowid from t_fgroup2 where (groupname='{1}' or groupcode='{2}') and rowid <> {0}", vg.rowid, vg.groupName, vg.groupCode)) > 0)
                {
                    return -1;//群組名稱或群組編碼已存在
                }
                else
                {
                    _access.execCommand(string.Format("update t_fgroup2 set groupname='{1}',groupcode='{2}',remark='{3}' where rowid={0}",vg.rowid, vg.groupName, vg.groupCode, vg.remark));
                    return 2;//修改成功
                }
            }
        }

        public int Delete(Model.Vgroup vg)
        {
           vg.Replace4MySQL();
            strSql = string.Format(@"delete from t_groupcaller where groupid={0}",vg.rowid);
            _access.execCommand(strSql);

            strSql = string.Format(@"delete from t_functiongroup2 where groupid={0}",vg.rowid);
            _access.execCommand(strSql);

            strSql = string.Format(@"delete from t_fgroup2 where rowid={0}",vg.rowid);
            _access.execCommand(strSql);
            return 1;
        }
    }
}
