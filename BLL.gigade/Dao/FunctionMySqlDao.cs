
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DBAccess;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model;
using BLL.gigade.Model.Custom;

namespace BLL.gigade.Dao
{
    public class FunctionMySqlDao : IFunctionImplDao
    {
        private IDBAccess _dbAccess;
        public FunctionMySqlDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.MySql, connectionStr);
        }

        public int Save(Model.Function function)
        {
            function.Replace4MySQL();
            int result = 0;
            if (_dbAccess.execCommand(string.Format("select rowid from t_function where functiongroup='{0}' and functionname='{1}' and functiontype='{2}'", function.FunctionGroup, function.FunctionName, function.FunctionType)) <= 0)
            {
                result = _dbAccess.execCommand(string.Format("insert into t_function( functiontype ,functiongroup , functionname , functioncode ,iconcls ,remark , kuser , kdate , topvalue, isEdit)values  ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}', now(),'{7}', {8})", function.FunctionType, function.FunctionGroup, function.FunctionName, function.FunctionCode, function.IconCls, function.Remark, function.Kuser, function.TopValue,function.IsEdit));
            }
            //edit by wwei0216w 2014/1/5 添加isEdit列,用來保存該控件是否可編輯
            return result;
        }

        /// <summary>
        /// 根據條件更新權限功能
        /// </summary>
        /// <param name="function">更新的條件</param>
        /// <returns>受影響的行數</returns>
        public int Update(Model.Function function)
        {
            function.Replace4MySQL();
            int result = 0;
            if (_dbAccess.execCommand(string.Format("select rowid from t_function where functiongroup='{0}' and functionname='{1}' and functiontype='{2}' and rowid<>{3}", function.FunctionGroup, function.FunctionName, function.FunctionType,function.RowId)) <= 0)
            {
                result = _dbAccess.execCommand(string.Format("update  t_function set functiontype = '{0}' ,functiongroup = '{1}' ,functionname = '{2}' ,functioncode = '{3}' ,iconcls='{4}',remark = '{5}',kuser ='{6}',isEdit={7} where rowid = {8}", function.FunctionType, function.FunctionGroup, function.FunctionName, function.FunctionCode, function.IconCls, function.Remark, function.Kuser, function.IsEdit,function.RowId));
            }
            //eidt by wwei0216w 2015/1/5 添加isEdit 用來保存更新控件是否可編輯的狀態
            return result;

        }

        public int Delete(int rowId)
        {
            int result = 0;
            _dbAccess.execCommand(string.Format("set sql_safe_updates=0;delete from t_functiongroup where functionid={0};set sql_safe_updates=1;", rowId));
            _dbAccess.execCommand(string.Format("set sql_safe_updates=0;delete from t_functiongroup2  where functionid={0};set sql_safe_updates=1;", rowId)); //add by shiwei0620j
            System.Data.DataTable _dt=_dbAccess.getDataTable(string.Format("select rowid from t_function where rowid={0} and functiontype=1", rowId));
            if (_dt != null && _dt.Rows.Count > 0)
            {
                result = _dbAccess.execCommand(string.Format("set sql_safe_updates=0;delete from  t_functiongroup  where   functionid in ( select  rowid  from    t_function  where   functiontype = 2 and topvalue = {0} );set sql_safe_updates=1;", rowId));
                result = _dbAccess.execCommand(string.Format("set sql_safe_updates=0;delete from  t_functiongroup2  where   functionid in ( select  rowid  from    t_function  where   functiontype = 2 and topvalue = {0} );set sql_safe_updates=1;", rowId));//add by shiwei0620j
                result = _dbAccess.execCommand(string.Format("set sql_safe_updates=0;delete from  t_function   where   functiontype = 2 and topvalue = {0};set sql_safe_updates=1;", rowId));
            }
            _dbAccess.execCommand(string.Format("delete from t_function where rowid={0}", rowId));
            return result;
        }

        public List<Model.Function> Query(Model.Function query)
        {
            query.Replace4MySQL();
            List<Model.Function> result = new List<Model.Function>();
            if (query.TopValue.ToString() != "" && query.RowId == 0 && query.FunctionType == 2)
            {
                result = _dbAccess.getDataTableForObj<Model.Function>(string.Format("select rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue,isEdit  from t_function where functiontype=2 and topvalue='{0}'", query.TopValue));
            }
            else if (query.RowId != 0 && query.FunctionType != 0)
            {
                result = _dbAccess.getDataTableForObj<Model.Function>(string.Format("select rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue,isEdit  from t_function where rowid={0} and functiontype={1}", query.RowId, query.FunctionType));
            }
            else if (query.RowId != 0)
            {
                result = _dbAccess.getDataTableForObj<Model.Function>(string.Format("select rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue,isEdit  from t_function where rowid={0}", query.RowId));
            }
            else if (query.FunctionType == -1)
            {
                //result = _dbAccess.getDataTableForObj<Model.Function>("select rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue,isEdit  from t_function where functiontype<>2");
                result = _dbAccess.getDataTableForObj<Model.Function>(@"SELECT  tf.rowid,tf.functiontype,tf.functiongroup,tf.functionname,tf.functioncode,tf.iconcls,tf.remark,tf.kuser,tf.topvalue,tf.isEdit,fh.count FROM t_function tf 
	LEFT JOIN (SELECT function_id,COUNT(*) as count  FROM function_history  GROUP BY  function_id) fh ON fh.function_id = tf.rowid 
WHERE functiontype<>2
");  // add by wwei0216w 2015/4/14 添加點擊次數列 count
            }
            else if (query.FunctionType != 0)
            {
                result = _dbAccess.getDataTableForObj<Model.Function>(string.Format("select rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue,isEdit  from t_function where functiontype={0}", query.FunctionType));
            }
            else if (!string.IsNullOrEmpty(query.FunctionCode))
            {
                result = _dbAccess.getDataTableForObj<Model.Function>(string.Format("select rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue,isEdit from t_function where functioncode='{0}'", query.FunctionCode));
            }
            else
            {
                result = _dbAccess.getDataTableForObj<Model.Function>("select rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue,isEdit from t_function");
            }
            return result;
        }

        //add by wwei0216 2015/1/5
        /// <summary>
        /// 查詢可編輯的控件
        /// </summary>
        /// <param name="groupId">角色權限Id</param>
        /// <returns>可編輯的控件的集合</returns>
        public List<Model.Function> QueryByFunctionType(int groupId)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //sb.Append(@"SELECT rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue,isEdit FROM t_function WHERE isEdit = 1");
                sb.AppendFormat(@"SELECT tf.rowid,tf.functiontype,tf.functiongroup,tf.functionname,tf.functioncode,tf.iconcls,tf.remark,tf.kuser,tf.topvalue,tf.isEdit FROM t_function tf 
                                    INNER JOIN  t_functiongroup tfg ON tfg.functionId = tf.rowid AND tfg.groupId = {0}
                                  WHERE tf.isEdit = 1", groupId);
            }
            catch(Exception ex)
            {
                throw new Exception("FunctionMySqlDao-->QueryByFunctionType-->" + ex.Message + sb.ToString(), ex);
            }
            return _dbAccess.getDataTableForObj<Model.Function>(sb.ToString());
        }


        public Model.Function QueryFunction(string childCode,string parentCode)
        {
            try
            {
                StringBuilder strSql = new StringBuilder("select rowid,functiontype,functiongroup,functionname,functioncode,iconcls,remark,kuser,topvalue from t_function ");
                strSql.AppendFormat("where functiontype=2 and functioncode='{0}' and topvalue in(select rowid from t_function where functiontype=1 and functioncode='{1}')", childCode, parentCode);
                return _dbAccess.getSinggleObj<Model.Function>(strSql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionMySqlDao-->QueryFunction-->" + ex.Message , ex);
            }
        }
        

        /// <summary>
        /// 根據funcitonId獲取擁有該功能的user集合
        /// </summary>
        /// <returns>(rowId,callid,groupName)的集合</returns>
        ///add by wwei0216w 2015/7/1
        public List<FunctionCustom> GetUserById(Function fun)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendFormat(@"SELECT c.rowId,c.callid,mu.user_username,fg.groupName,c.groupId
                                      FROM t_groupcaller c
                                  INNER JOIN t_functiongroup f ON c.groupId = f.groupId AND f.functionId = {0}
                                  INNER JOIN t_fgroup fg ON fg.rowid = c.groupId
	                              INNER JOIN manage_user mu ON mu.user_email = c.callid", fun.RowId);
                return _dbAccess.getDataTableForObj<FunctionCustom>(sb.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionMySqlDao-->GetUserById" + ex.Message,ex);
            }
        }
    }
}
