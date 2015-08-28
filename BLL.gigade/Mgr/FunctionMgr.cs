/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：FunctionMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/1 16:18:26 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Model.Custom;
using BLL.gigade.Model;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Xml.Linq;
using gigadeExcel.Comment;

namespace BLL.gigade.Mgr
{
    public class FunctionMgr : IFunctionImplMgr
    {
        private IFunctionImplDao _functionDao;
        public FunctionMgr(string connectionStr)
        {
            _functionDao = new FunctionMySqlDao(connectionStr);
        }

        public int Save(Model.Function function)
        {
            try
            {
                return _functionDao.Save(function);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionMgr-->Save -->" + ex.Message, ex);
            }
        }

        public int Update(Model.Function function)
        {
            try
            {
                return _functionDao.Update(function);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionMgr-->Update -->" + ex.Message, ex);
            }
        }
        public int Delete(int rowId)
        {
            try
            {
                return _functionDao.Delete(rowId);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionMgr-->Delete -->" + ex.Message, ex);
            }
        }

        public List<Model.Function> Query(Model.Function query, int controlType, int groupId)
        {
            try
            {
                if(controlType==2) //edit by wwei0216w 2015/1/6
                {
                    List<Model.Function> functions = _functionDao.QueryByFunctionType(groupId);
                    foreach (Model.Function f in functions.FindAll(p => p.FunctionType == 1))
                    {
                        if (functions.Find(m => m.TopValue == f.RowId) == null)
                            functions.Remove(f);
                    }
                    return functions;
                }
                else
                {
                    return _functionDao.Query(query);                  
                }
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionMgr-->Query -->" + ex.Message, ex);
            }
        }

        public Model.Function QueryFunction(string childCode, string parentCode)
        {
            return _functionDao.QueryFunction(childCode, parentCode);
        }
         

        /// <summary>
        /// 根據funcitonId獲取擁有該功能的user集合
        /// </summary>
        /// <returns>(rowId,callid,groupName)的集合</returns>
        ///add by wwei0216w 2015/7/1
        public List<FunctionCustom> GetUserById(Function fun)
        {
            try
            {
                return _functionDao.GetUserById(fun);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionMgr-->GetUserById" + ex.Message,ex);
            }
        }

        /// <summary>
        /// 將擁有某一權限的用戶集合匯出excel表
        /// </summary>
        /// <returns>(rowId,callid,groupName)的集合</returns>
        ///add by wwei0216w 2015/7/2
        public MemoryStream ExcelOut(Function fun, string functionName)
        {
            try
            {
                //獲得要匯出的數據集合  edit by zhuoqin0830w  2015/07/07  不使用 XML 文檔進行匯出
                Dictionary<string, string> columns = new Dictionary<string, string>();
                columns.Add("User_UserName", "用戶名");
                columns.Add("CallId", "用戶郵箱");
                columns.Add("GroupName", "群組名稱");
                List<FunctionCustom> list = GetUserById(fun);
                return ExcelHelperXhf.ExportExcel(list, columns, null,functionName);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionMgr-->ExcelOut" + ex.Message, ex);
            }
        }
    }
}
