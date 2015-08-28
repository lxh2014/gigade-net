using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Dao
{
    public class SalesUserDao : ISalesUserImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;
        public SalesUserDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }
        public List<Model.Query.SalesUserQuery> Query(Model.Query.SalesUserQuery store, out int totalCount)
        {
            throw new NotImplementedException();
        }

        #region 新增數據到saleuser表中 +int SaveUserList(Model.Query.SalesUserQuery usr)
        public int SaveUserList(Model.Query.SalesUserQuery usr)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                str.AppendFormat("insert into sales_user(s_id,user_id,status,type,creator,createdate)values('{0}','{1}','{2}','{3}','{4}','{5}')", usr.s_id, usr.user_id, usr.status, usr.type, usr.creator, usr.createdate);
                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SalesUserDao-->updateuser_master-->" + ex.Message + str.ToString(), ex);
            }
        }
        #endregion

        public Model.Custom.Users getModel(int id)
        {
            throw new NotImplementedException();
        }

        #region 獲取表中最大id +int Selectbigsid()
        public int Selectbigsid()
        {
            int bigs_id = 0;
           
            StringBuilder str = new StringBuilder();
            try
            {
                str.AppendFormat("select max(s_id)as maxid from sales_user");
                System.Data.DataTable _dt = _access.getDataTable(str.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    bigs_id = Convert.ToInt32(_dt.Rows[0]["maxid"]);
                }
                return bigs_id;
            }
            catch (Exception ex)
            {
                throw new Exception("SalesUserDao-->Selectbigsid-->" + ex.Message + str.ToString(), ex);
            }
        }
        #endregion

        #region 更新saleuser表 +int updatesaleuser(Model.Query.SalesUserQuery usr)
        public int updatesaleuser(Model.Query.SalesUserQuery usr)
        {
            StringBuilder str = new StringBuilder();
            try
            {
                str.AppendFormat("update sales_user set status='{0}',type='{1}', modifier = '{2}', modify_time = '{3}' where user_id = '{4}'", usr.status, usr.type, usr.modifier, usr.modify_time, usr.user_id);
                return _access.execCommand(str.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("SalesUserDao-->updatesaleuser-->" + ex.Message + str.ToString(), ex);
            }
        }
        #endregion

        #region 根據id獲取到salesuser表的某行 +Model.SalesUser MySalesUser(int id)
        public Model.SalesUser MySalesUser(int id)
        {
            Model.SalesUser slur = new Model.SalesUser();
            StringBuilder str = new StringBuilder();
            try
            {
                str.AppendFormat("select * from sales_user where user_id='{0}'", id);
                System.Data.DataTable _dt = _access.getDataTable(str.ToString());
                if (_dt != null && _dt.Rows.Count > 0)
                {
                    string types = _dt.Rows[0]["type"].ToString();
                    string status = _dt.Rows[0]["status"].ToString();
                    slur.type = Convert.ToUInt32(types);
                    slur.status = Convert.ToUInt32(status);
                }
                return slur;
            }
            catch (Exception ex)
            {
                throw new Exception("SalesUserDao-->MySalesUser-->" + ex.Message + str.ToString(), ex);
            }
        }
        #endregion
    }
}
