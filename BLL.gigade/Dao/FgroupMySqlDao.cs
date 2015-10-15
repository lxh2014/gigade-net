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
    public class FgroupMySqlDao : IFgroupImplDao
    {
        private IDBAccess _access;
        string strSql = string.Empty;

        public FgroupMySqlDao(string connectionString)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionString);
        }

        public List<Fgroup> QueryAll()
        {

            return _access.getDataTableForObj<Fgroup>(@"select a.rowid,groupname,groupcode,count(b.rowid) as callid,remark from t_fgroup a left join t_groupcaller b on a.rowid=b.groupid 
 LEFT JOIN manage_user mu on b.callid=mu.user_email  WHERE mu.user_status !=0 and mu.user_status !=2 
 group by a.rowid,groupname,groupcode,remark");
        }

        public List<Fgroup> Query(string callid, string groupCode)
        {
            return _access.getDataTableForObj<Fgroup>(string.Format("select g.* from t_fgroup g inner join t_groupcaller gc on g.rowid=gc.groupId where gc.callid='{0}' and g.groupCode='{1}';", callid, groupCode));
        }

        public List<ManageUser> QueryCallid()
        {
            return _access.getDataTableForObj<ManageUser>("select user_username as name,user_email as callid from manage_user where user_status != 0 and user_status !=2 ");
        }

        public int Save(Fgroup fg)
        {
            fg.Replace4MySQL();
            if (fg.rowid == 0)
            {
                if (_access.getDataTable(string.Format("select rowid from t_fgroup where (groupname='{0}' or groupcode='{1}')", fg.groupName, fg.groupCode)).Rows.Count > 0)
                {
                    return -1;//群組名稱或群組編碼已存在
                }
                else
                {
                    _access.execCommand(string.Format("insert into t_fgroup(groupname,groupcode,remark,kuser,kdate) values('{0}','{1}','{2}','{3}',now())", fg.groupName, fg.groupCode, fg.remark, fg.kuser));
                    return 1;//新增成功
                }
            }
            else
            {
                if (_access.getDataTable(string.Format("select rowid from t_fgroup where (groupname='{1}' or groupcode='{2}') and rowid <> {0}", fg.rowid, fg.groupName, fg.groupCode)).Rows.Count > 0)
                {
                    return -1;//群組名稱或群組編碼已存在
                }
                else
                {
                    _access.execCommand(string.Format("update t_fgroup set groupname='{1}',groupcode='{2}',remark='{3}' where rowid={0}", fg.rowid, fg.groupName, fg.groupCode, fg.remark));
                    return 2;//修改成功
                }
            }
        }

        public int Delete(Fgroup fg)
        {
            fg.Replace4MySQL();
            strSql = string.Format(@"delete from t_groupcaller where groupid={0}", fg.rowid);
            _access.execCommand(strSql);

            strSql = string.Format(@"delete from t_functiongroup where groupid={0}", fg.rowid);
            _access.execCommand(strSql);

            strSql = string.Format(@"delete from t_fgroup where rowid={0}", fg.rowid);
            _access.execCommand(strSql);
            return 1;
        }


        public DataTable GetFgroupList()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT rowid,groupName FROM t_fgroup;");
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMySqlDao.GetFgroupList-->" + ex.Message + sb.ToString(), ex);
            }

        }

        public DataTable GetUsersByGroupId(int groupid)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT mu.user_username,mu.user_email,tf.groupName FROM t_fgroup tf LEFT JOIN t_groupcaller tg on tg.groupId=tf.rowid 
LEFT JOIN manage_user mu on tg.callid=mu.user_email WHERE tf.rowid='{0}';", groupid);
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->GetUsersByGroupId-->" + ex.Message, ex);
            }
        }


        public DataTable GetAuthorityByGroupId(int groupid)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"SELECT tableone.pname,tableone.cname,tableone.tname,tabletwo.tpname,tabletwo.tcname,tabletwo.ttname,tabletwo.functionId
FROM
(SELECT tft.functionGroup as pname,tft.functionName as cname,tftt.functionName tname,CASE WHEN tftt.rowid >0 then tftt.rowid ELSE tft.rowid end as row_id FROM t_function tft 
LEFT JOIN t_function tftt on tft.rowid=tftt.topValue 
WHERE tft.topValue =0 order by tft.functionGroup,tft.functionName) as tableone LEFT JOIN  
(SELECT thistb.tpname,thistb.tcname,thistb.ttname,thistb.row_id,thistb.rowid,tfp.functionId FROM(SELECT tft.functionGroup as tpname,tft.functionName tcname,tftt.functionName ttname,CASE WHEN tftt.rowid >0 then tftt.rowid ELSE tft.rowid end as row_id,tft.rowid,tftt.rowid as newrow_id FROM t_fgroup tf 
LEFT JOIN t_functiongroup tftg on tftg.groupId=tf.rowid 
LEFT JOIN t_function tft on tft.rowid=tftg.functionId 
LEFT JOIN t_function tftt on tft.rowid=tftt.topValue 
WHERE tft.topValue =0 and tf.rowid='{0}' and tftg.groupId='{0}') as thistb LEFT JOIN t_functiongroup tfp on thistb.newrow_id=tfp.functionId and tfp.groupId='{0}') as tabletwo on tableone.row_id=tabletwo.row_id;", groupid);
            try
            {
                return _access.getDataTable(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("FgroupMgr-->GetAuthorityByGroupId-->" + ex.Message, ex);
            }

        }


        public Fgroup GetSingle(Fgroup model)//add by mengjuan0826j 獲取單個model  根據groupname和groupcode 
        {
            model.Replace4MySQL();
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("select rowid,groupName,groupCode,remark,kuser,kdate ");
                strSql.Append(" from t_fgroup ");
                strSql.Append(" where 1=1 ");
                //if (!string.IsNullOrEmpty(model.groupName))
                //{
                //    strSql.AppendFormat(" and groupName='{0}'", model.groupName);
                //}
                if (!string.IsNullOrEmpty(model.groupCode))
                {
                    strSql.AppendFormat(" and groupCode='{0}'", model.groupCode);
                }
                return _access.getSinggleObj<Fgroup>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" FgroupDao-->GetSingle-->" + ex.Message + strSql.ToString(), ex);
            }
        }

        public DataTable GetFgroupLists()
        {
            StringBuilder strSql = new StringBuilder();
            try
            {
                strSql.Append("select mu.user_id,mu.user_username from t_fgroup tfg  ");
                strSql.Append(" LEFT JOIN t_groupcaller tg on  tfg.rowid=tg.groupid ");
                strSql.Append(" left join manage_user mu on mu.user_email=tg.callid");
                strSql.Append(" where groupCode='picking' ;");
                return _access.getDataTable(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(" FgroupDao-->GetFgroupLists-->" + ex.Message + strSql.ToString(), ex);
            }

        }
    }
}
