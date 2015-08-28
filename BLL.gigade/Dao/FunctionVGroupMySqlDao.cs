using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using DBAccess;

namespace BLL.gigade.Dao
{
  public   class FunctionVGroupMySqlDao : IFunctionVGroupImplDao
    {
      private IDBAccess _dbAccess;

      public FunctionVGroupMySqlDao(string connectionStr)
      {
          _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
      }
        public int Save(Model.FunctionGroup functionGroup)
        {
            functionGroup.Replace4MySQL();
            return _dbAccess.execCommand(string.Format("insert  into t_functiongroup2( functionid , groupid , kdate , kuser )values  ( '{0}' ,  '{1}' , now() , '{2}')", functionGroup.FunctionId, functionGroup.GroupId, functionGroup.Kuser));
        }

        public int Delete(int RowId)
        {
            return _dbAccess.execCommand(string.Format("delete from t_functiongroup2 where groupid={0}", RowId));
        }

        public List<Model.Function> CallerAuthorityQuery(Model.Query.AuthorityQuery query)
        {
            if (query.RowId == 0)
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format("select  rowid ,functiongroup ,functionname ,functioncode ,iconcls,remark ,kuser from    t_function where   rowid in (select distinct  t_functiongroup2.functionid from    t_functiongroup2	 left  join t_fgroup2 on t_functiongroup2.groupid = t_fgroup2.rowid left  join t_groupcaller2  on t_fgroup2.rowid = t_groupcaller2.groupid			where   callid = '{0}' ) 	and functiontype = 1", query.CallId));
            }
            else if (query.Type == 1)
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format("select  rowid ,	functiongroup ,	functionname ,	functioncode ,iconcls,remark ,	kuser from    t_function	where   rowid in (	select distinct					t_functiongroup2.functionid from    t_functiongroup2 	left  join t_fgroup2 on t_functiongroup2.groupid = t_fgroup2.rowid left join t_groupcaller2 on t_fgroup2.rowid = t_groupcaller2.groupid			where   callid = '{0}' ) 	and functiontype = 1 and functiongroup=(select functiongroup from t_function where functiontype=1 and rowid={1})", query.CallId, query.RowId));
            }
            else if (query.Type == 2)
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format("	select  rowid ,functiongroup ,	functionname ,	functioncode ,iconcls,remark ,	kuser from    t_function where   rowid in (select distinct					t_functiongroup2.functionid	from    t_functiongroup2  left join t_fgroup2 on t_functiongroup2.groupid = t_fgroup2.rowid	left join t_groupcaller2 on t_fgroup2.rowid = t_groupcaller2.groupid	where   callid = '{0}' )	and functiontype = 2	and topvalue='{1}'", query.CallId, query.RowId));
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
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format("select  rowid ,	functiongroup ,	functionname ,functioncode ,iconcls,remark ,kuser from    t_function where   rowid in (	select distinct					t_functiongroup2.functionid from    t_functiongroup2 	left join t_fgroup2 on t_functiongroup2.groupid = t_fgroup2.rowid where   t_functiongroup2.groupid = '{0}')", query.GroupId));
            }
            else
            {
                return _dbAccess.getDataTableForObj<Model.Function>(string.Format("select  rowid ,functiontype ,functiongroup ,functionname ,functioncode ,iconcls,remark ,kuser ,topvalue from    t_function where   rowid in (select distinct t_functiongroup2.functionid	from    t_functiongroup2  left join t_fgroup2  on t_functiongroup2.groupid = t_fgroup2.rowid where   t_functiongroup2.groupid = '{0}' )	and functiontype = '{1}'", query.GroupId, query.Type));
            } 
        }
    }
}
