/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：FunctionDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/1 16:17:56 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using DBAccess;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class FunctionDao:IFunctionImplDao
    {
        private IDBAccess _dbAccess;
        public FunctionDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.SqlServer, connectionStr);
        }

        public int Save(Model.Function function)
        {
            function.Replace4MySQL();
            SqlParameter[] paras = new SqlParameter[7]{
                new SqlParameter("@functionType",function.FunctionType),
                new SqlParameter("@functionGroup",function.FunctionGroup),
                new SqlParameter("@functionName",function.FunctionName),
                new SqlParameter("@functionCode",function.FunctionCode),
                new SqlParameter("@remark",function.Remark),
                new SqlParameter("@kuser",function.Kuser),
                new SqlParameter("@topValue",function.TopValue)
            };
            return _dbAccess.execCommand("sp_Function_Save_xiaobo0330w", paras);
        }
        public int Update(Model.Function function)
        {
            function.Replace4MySQL();
            SqlParameter[] paras = new SqlParameter[7]{
                new SqlParameter("@rowId",function.RowId),
                new SqlParameter("@functionType",function.FunctionType),
                new SqlParameter("@functionGroup",function.FunctionGroup),
                new SqlParameter("@functionName",function.FunctionName),
                new SqlParameter("@functionCode",function.FunctionCode),
                new SqlParameter("@remark",function.Remark),
                new SqlParameter("@kuser",function.Kuser)
            };
            return _dbAccess.execCommand("sp_Function_Update_xiaobo0330w", paras);
        }
        public int Delete(int rowId)
        {
            SqlParameter[] paras = new SqlParameter[1]{
                new SqlParameter("@rowId",rowId)
            };
            return _dbAccess.execCommand("sp_Function_Delete_xiaobo0330w", paras);
        }

        public List<Model.Function> Query(Model.Function query)
        {
            query.Replace4MySQL();
            SqlParameter[] paras = new SqlParameter[3]{
                new SqlParameter("@rowId",query.RowId),
                new SqlParameter("@functionType",query.FunctionType),
                new SqlParameter("@topValue",query.TopValue)
            };
            return _dbAccess.getDataTableForObj<Model.Function>("sp_Function_Query_xiaobo0330w", paras);
        }

        public Model.Function QueryFunction(string childCode, string parentCode)
        {
            return null;
        }

        #region IFunctionImplDao 成员


        public List<Model.Function> QueryByFunctionType()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFunctionImplDao 成员


        public List<Model.Function> QueryByFunctionType(int groupId)
        {
            throw new NotImplementedException();
        }

        #endregion

        public List<FunctionCustom> GetUserById(Function fun)
        {
            throw new NotImplementedException();
        }
    }
}
