/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：FunctionGroupDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/2 16:37:03 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBAccess;
using System.Data.SqlClient;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Dao
{
    public class FunctionGroupDao:IFunctionGroupImplDao
    {
        private IDBAccess _dbAccess;
        public FunctionGroupDao(string connectionStr)
        {
            _dbAccess = DBFactory.getDBAccess(DBType.SqlServer, connectionStr);
        }

        public int Save(Model.FunctionGroup functionGroup)
        {
            functionGroup.Replace4MySQL();
            SqlParameter[] paras = new SqlParameter[3]{
                new SqlParameter("@functionId",functionGroup.FunctionId),
                new SqlParameter("@groupId",functionGroup.GroupId),
                new SqlParameter("@kuser",functionGroup.Kuser),
            };
            return _dbAccess.execCommand("sp_FunctionGroup_Insert_xiaobo0330w", paras);
        }

        public int Delete(int RowId)
        {
            SqlParameter[] paras = new SqlParameter[1]{
                new SqlParameter("@groupId",RowId)
            };
            return _dbAccess.execCommand("sp_FunctionGroup_Delete_xiaobo0330w", paras);
        }

        public List<Model.Function> CallerAuthorityQuery(Model.Query.AuthorityQuery query)
        {
            SqlParameter[] paras = new SqlParameter[3]{
                new SqlParameter("@rowId",query.RowId),
                new SqlParameter("@type",query.Type),
                new SqlParameter("@callid",query.CallId)
            };
            return _dbAccess.getDataTableForObj<Model.Function>("sp_FunctionGroup_Authority_xiaobo0330w", paras);
        }

        public List<Model.Function> GroupAuthorityQuery(Model.Query.AuthorityQuery query)
        {
            SqlParameter[] paras = new SqlParameter[2]{
                new SqlParameter("@groupId",query.GroupId),
                new SqlParameter("@type",query.Type)
            };
            return _dbAccess.getDataTableForObj<Model.Function>("sp_FunctionGroup_GroupAuthority_xiaobo0330w", paras);
        }


        public bool Save(int[] funIds, int groupId, string kuser)
        {
            throw new NotImplementedException();
        }

        public List<Model.Function> GetEditFunctionByGroup(Model.Query.AuthorityQuery query)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEditFunction(string[] functionIds, int groupId)
        {
            throw new NotImplementedException();
        }
    }
}
