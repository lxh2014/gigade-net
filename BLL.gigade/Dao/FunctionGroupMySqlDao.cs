using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data.SqlClient;
using BLL.gigade.Dao.Impl;
using System.Collections;

namespace BLL.gigade.Dao
{
    public class FunctionGroupMySqlDao : IFunctionGroupImplDao
    {
        private IDBAccess _dbAccess;
        private MySqlDao mysqlDao;
        public FunctionGroupMySqlDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
            mysqlDao = new MySqlDao(connectionStr);
        }

        public int Save(Model.FunctionGroup functionGroup)
        {
            functionGroup.Replace4MySQL();
            return _dbAccess.execCommand(string.Format("insert  into t_functiongroup( functionid , groupid , kdate , kuser )values  ( '{0}' ,  '{1}' , now() , '{2}')", functionGroup.FunctionId, functionGroup.GroupId, functionGroup.Kuser));
        }

        public bool Save(int[] funIds, int groupId,string kuser)
        {
            ArrayList arrayList=new ArrayList();
            arrayList.Add(string.Format("delete from t_functiongroup where groupid={0};", groupId));
            StringBuilder sqlStr = new StringBuilder("insert  into t_functiongroup( functionid , groupid , kdate , kuser )values  ");
            foreach (var funId in funIds)
            {
                sqlStr.AppendFormat("( '{0}' ,  '{1}' , now() , '{2}'),", funId, groupId, kuser);
            }
            sqlStr.Remove(sqlStr.Length - 1, 1);
            arrayList.Add(sqlStr.ToString());
            return mysqlDao.ExcuteSqls(arrayList);
        }



        public int Delete(int RowId)
        {
            return _dbAccess.execCommand(string.Format("delete from t_functiongroup where groupid={0}", RowId));
        }

        public List<Model.Function> CallerAuthorityQuery(Model.Query.AuthorityQuery query)
        {
            if (query.RowId == 0)
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format("select  rowid ,functiongroup ,functionname ,functioncode ,iconcls,remark ,kuser,isEdit,ueidt from  t_function inner join (	select distinct t_functiongroup.functionid,isedit as ueidt from  t_functiongroup	left join t_fgroup on t_functiongroup.groupid = t_fgroup.rowid 	left join t_groupcaller on t_fgroup.rowid = t_groupcaller.groupid where   callid = '{0}' ) a on a.functionId = t_function.rowid	where functiontype = 1", query.CallId));
            }
            else if (query.Type == 1)
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format(@"select  rowid ,	functiongroup ,	functionname ,	functioncode ,iconcls,remark ,	kuser,isedit,uedit from    t_function	
                                                                                                                            inner join  (	
	                                                                                                                            select distinct	t_functiongroup.functionid,isEdit as uedit  from    t_functiongroup	
	                                                                                                                            inner join t_fgroup on t_functiongroup.groupid = t_fgroup.rowid 
	                                                                                                                            inner join t_groupcaller on t_fgroup.rowid = t_groupcaller.groupid
	                                                                                                                            where   callid = '{0}' ) a 
                                                                                                                            on a.functionid = t_function.rowid
                                                                                                                            where functiontype = 1 and functiongroup=(select functiongroup from t_function where functiontype=1 and rowid={1})", query.CallId, query.RowId));
            }
            else if (query.Type == 2)
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format(@"	select  rowid ,functiongroup , functionname , functioncode ,iconcls,remark , kuser,isEdit,case when uedit > 0 then 1 else 0 end uedit from t_function 
                                                                                                                                inner join (
	                                                                                                                                select distinct t_functiongroup.functionid,sum(isEdit) as uedit	from    t_functiongroup 
	                                                                                                                                left join t_fgroup on t_functiongroup.groupid = t_fgroup.rowid	
	                                                                                                                                left join t_groupcaller on t_fgroup.rowid = t_groupcaller.groupid	where   callid = '{0}' group by functionId )a
                                                                                                                                on a.functionId = t_function.rowid
                                                                                                                                where functiontype = 2	and topvalue='{1}'", query.CallId, query.RowId));
            }
            else
            {
                return null;
            }
        }

        public List<Model.Function> GroupAuthorityQuery(Model.Query.AuthorityQuery query)
        {
            if (query.Type == 0)
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format("select  rowid ,	functiongroup ,	functionname ,functioncode ,iconcls,remark ,kuser from    t_function where   rowid in (	select distinct					t_functiongroup.functionid from    t_functiongroup	left join t_fgroup on t_functiongroup.groupid = t_fgroup.rowid where   t_functiongroup.groupid = '{0}')", query.GroupId));
            } else 
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format("select  rowid ,functiontype ,functiongroup ,functionname ,functioncode ,iconcls,remark ,kuser ,topvalue from    t_function where   rowid in (select distinct t_functiongroup.functionid	from    t_functiongroup left join t_fgroup on t_functiongroup.groupid = t_fgroup.rowid where   t_functiongroup.groupid = '{0}' )	and functiontype = '{1}'", query.GroupId, query.Type));
            } 
        }

        //add by wwei0216w 2015/1/6
        /// <summary>
        /// 根據用戶類型獲得對應的可編輯權限集合
        /// </summary>
        /// <param name="query">查詢類型的條件對象</param>
        /// <returns>符合條件的功能集合</returns>
        public List<Model.Function> GetEditFunctionByGroup(Model.Query.AuthorityQuery query)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(@"SELECT rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue FROM t_function
 WHERE rowid IN(SELECT DISTINCT functionId FROM t_functiongroup  WHERE isEdit = 1 ");
                if(query.GroupId!=0)
                {
                    sb.AppendFormat(" AND groupId ={0}",query.GroupId);
                }
                if(query.Type!=0)
                {
                    sb.AppendFormat(" AND functiontype = {0}",query.Type);
                }
                sb.Append(") ");
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMySqlDao-->GetEditFunctionByGroup-->" + ex.Message + sb.ToString(), ex);
            }

            return _dbAccess.getDataTableForObj<Model.Function>(sb.ToString());
        }

        //add bywwei 0216w 2015/1/6
        /// <summary>
        /// 更新編輯權限
        /// </summary>
        /// <param name="functionIds">需要更新的權限functionIds集合</param>
        /// <returns>true or false</returns>
        public bool UpdateEditFunction(string[] functionIds,int groupId)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"UPDATE t_functiongroup SET isEdit = 0 WHERE groupId = {0};", groupId);
                sb.AppendFormat(@"UPDATE t_functiongroup SET isEdit = 1 WHERE groupId = {0} AND functionId IN({1});", groupId,string.Join(",",functionIds));
                return _dbAccess.execCommand(sb.ToString()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMySqlDao-->UpdateEditFunction-->" + ex.Message + sb.ToString(), ex);
            }
        }
    }
}
