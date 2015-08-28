/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：FunctionGroupMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：lhInc 
 * 完成日期：2013/8/2 16:37:14 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;

namespace BLL.gigade.Mgr
{
    public class FunctionGroupMgr:IFunctionGroupImplMgr
    {
        private IFunctionGroupImplDao functionGroupDao;
        public FunctionGroupMgr(string connectionStr)
        {
            functionGroupDao = new FunctionGroupMySqlDao(connectionStr);
        }

        public int Save(Model.FunctionGroup functionGroup)
        {
            try
            {
                return functionGroupDao.Save(functionGroup);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMgr-->SingleCompareSave-->" + ex.Message, ex);
            }
        }

        public bool Save(int[] funIds, int groupId, string kuser)
        {
            try
            {
                return functionGroupDao.Save(funIds,groupId,kuser);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMgr-->Save(List<Model.FunctionGroup> functionGroups)-->" + ex.Message, ex);
            }

        }

        public int Delete(int RowId)
        {
            
            try
            {
                return functionGroupDao.Delete(RowId);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public List<Model.Function> CallerAuthorityQuery(Model.Query.AuthorityQuery query)
        {
            try
            {
                return functionGroupDao.CallerAuthorityQuery(query);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMgr-->CallerAuthorityQuery-->" + ex.Message, ex);
            }
            
        }

        public List<Model.Function> GroupAuthorityQuery(Model.Query.AuthorityQuery query, int controlType)
        {
            
            try
            {
                if(controlType==2)
                {
                    return functionGroupDao.GetEditFunctionByGroup(query);
                }
                else
                {
                    return functionGroupDao.GroupAuthorityQuery(query);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMgr-->GroupAuthorityQuery-->" + ex.Message, ex);
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
            try
            {
                return functionGroupDao.GetEditFunctionByGroup(query);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMgr-->GetEditFunctionByGroup -->" + ex.Message, ex);
            }
        }

        //add bywwei 0216w 2015/1/6
        /// <summary>
        /// 更新編輯權限
        /// </summary>
        /// <param name="functionIds">需要更新的權限functionIds集合</param>
        /// <returns>true or false</returns>
        public bool UpdateEditFunction(string[] rowId,int groupId)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                return functionGroupDao.UpdateEditFunction(rowId, groupId);
            }
            catch (Exception ex)
            {
                throw new Exception("FunctionGroupMySqlDao-->UpdateEditFunction-->" + ex.Message + sb.ToString(), ex);
            }            
        }
    }
}
