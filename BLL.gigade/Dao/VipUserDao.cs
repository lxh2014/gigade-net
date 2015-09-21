using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
using System.Data;
using BLL.gigade.Common;

namespace BLL.gigade.Dao
{
    public class VipUserDao
    {
        private IDBAccess _dbAccess;
        private string connStr;
        public VipUserDao(string connstr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connstr);
            this.connStr = connstr;
        }

        public DataTable GetBlackList(VipUserQuery query, out int totalCount)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            StringBuilder count = new StringBuilder();
            totalCount = 0;
            try
            {
                strSql.AppendFormat(@"SELECT vu.v_id,vu.user_email,vu.user_id,source,status,(SELECT user_username FROM manage_user WHERE manage_user.user_id=vu.create_id ) as createUsername,(SELECT user_username FROM manage_user WHERE manage_user.user_id=vu.update_id ) as updateUsername,FROM_UNIXTIME(updatedate) as updatedate,FROM_UNIXTIME(createdate) as 'create' FROM vip_user vu  WHERE vu.group_id=48 ");
                count.AppendFormat(@"SELECT count(v_id) as totalCount FROM vip_user where group_id=48");
                if (query.search_state != 2)
                {
                    strSql.AppendFormat(" and status={0}", query.search_state);
                    count.AppendFormat(" and status={0}", query.search_state);
                }
                if (!string.IsNullOrEmpty(query.serchtype))
                {
                    strSql.AppendFormat(" AND vu.user_email like N'%{0}%'", query.serchtype);
                    count.AppendFormat(" AND user_email like N'%{0}%'", query.serchtype);
                }
                if (query.start != DateTime.MinValue && query.end != DateTime.MinValue)
                {
                    strSql.AppendFormat(" AND createdate BETWEEN '{0}' AND '{1}'", CommonFunction.GetPHPTime(query.start.ToString()), CommonFunction.GetPHPTime(query.end.ToString()));
                    count.AppendFormat(" AND createdate BETWEEN '{0}' AND '{1}'", CommonFunction.GetPHPTime(query.start.ToString()), CommonFunction.GetPHPTime(query.end.ToString()));
                }
                if (query.source != 0)
                {
                    if (query.source == 1)
                    {
                        strSql.AppendFormat(" AND source !=2 ");
                        count.AppendFormat(" AND source!=2");
                    }
                    else {
                        strSql.AppendFormat(" AND source={0}", query.source);
                        count.AppendFormat(" AND source={0}", query.source);
                    }
                    
                }
                if (query.User_Id != 0)
                {
                    strSql.AppendFormat(" and vu.user_id={0}", query.User_Id);
                    count.AppendFormat(" and user_id={0}", query.User_Id);
                }
                if (query.IsPage)
                {
                    DataTable dt = _dbAccess.getDataTable(count.ToString());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        totalCount = Convert.ToInt32(dt.Rows[0][totalCount]);
                    }
                    strSql.AppendFormat(" order by v_id limit {0},{1} ", query.Start, query.Limit);
                }
                DataTable store = _dbAccess.getDataTable(strSql.ToString());
                return store;
            }
            catch (Exception ex)
            {

                throw new Exception("VipUserDao-->GetBlackList-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string UpdateState(VipUserQuery query)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"UPDATE vip_user set status={0},update_id={1},updatedate={2} WHERE v_id={3};", query.status, query.update_id, query.updatedate, query.v_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("VipUserDao-->UpdateState-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public string SelectEmail(uint userID)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("SELECT count(v_id) as sum,status,v_id from vip_user WHERE user_id='{0}' AND group_id= 48", userID);
                return strSql.ToString();
            }
            catch (Exception ex)
            {

                throw new Exception("VipUserDao-->SelectEmail-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string UpdateBlackList(VipUserQuery query)
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(" insert into vip_user(user_email,user_id,status,group_id,create_id,update_id,createdate,updatedate,source) values('{0}',{1},1,48,{2},{3},{4},{5},2);", query.vuser_email, query.v_id, query.create_id, query.update_id, query.createdate, query.updatedate);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserDao-->UpdateBlackList-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public string GetSingleByID(int v_id)
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(@"SELECT v_id,user_email,user_id,status,group_id,createdate FROM vip_user ");
                strSql.AppendFormat("  where v_id='{0}'", v_id);
                return strSql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserDao-->GetSingleByID-->" + ex.Message + strSql.ToString(), ex);
            }
        }
        public int AddVipUser(VipUserQuery query)//add by chaojie1124j 添加于2015/9/21用於實現添加會員至群組
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat(" insert into vip_user(user_email,user_id,status,group_id,create_id,update_id,createdate,updatedate,source) values('{0}','{1}',1,'{2}',{3},'{4}','{5}','{6}',2);", query.user_email,query.user_id,query.group_id,query.create_id, query.update_id, query.createdate, query.updatedate);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserDao-->AddVipUser-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public int DeleVipUser(VipUserQuery query)//add by chaojie1124j 添加于2015/9/21用於實現刪除會員至群組
        {
            query.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.AppendFormat("DELETE from vip_user where v_id='{0}';", query.v_id);
                return _dbAccess.execCommand(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("VipUserDao-->AddVipUser-->" + ex.Message + strSql.ToString(), ex);
            }
        }
    }
}