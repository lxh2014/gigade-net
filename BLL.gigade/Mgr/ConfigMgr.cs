/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：ConfigMgr 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：shuangshuang0420j 
 * 完成日期：2014/10/07 13:48:06 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using BLL.gigade.Dao;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;

namespace BLL.gigade.Mgr
{
    public class ConfigMgr : IConfigImplMgr
    {
        private IConfigImplDao _configDao;

        public ConfigMgr(string connectionString)
        {
            _configDao = new ConfigDao(connectionString);
        }

        public List<ConfigQuery> Query(string paramername, int parameternumber)
        {
            try
            {
                return _configDao.Query(paramername, parameternumber);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->Query-->" + ex.Message, ex);
            }

        }


        public uint QueryByEmail(string email)
        {
            try
            {
                return _configDao.QueryByEmail(email);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->QueryByEmail-->" + ex.Message, ex);
            }

        }

        public uint QueryByName(string name)
        {
            try
            {
                return _configDao.QueryByName(name);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->QueryByName-->" + ex.Message, ex);
            }


        }

        public List<Model.ManageUser> getUserPm(string nameStr)
        {
            try
            {
                return _configDao.getUserPm(nameStr);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->getUserPm-->" + ex.Message, ex);
            }
        }

        #region 獲取config表中的值
        public System.Data.DataTable GetConfig(ConfigQuery query)
        {
            try
            {
                return _configDao.GetConfig(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->GetConfig-->" + ex.Message, ex);
            }
        }
        #endregion


        public int InsertConfig(ConfigQuery query)
        {
            try
            {
                return _configDao.InsertConfig(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->InsertConfig-->" + ex.Message, ex);
            }
        }

        public int UpdateConfig(ConfigQuery query)
        {
            try
            {
                return _configDao.UpdateConfig(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->UpdateConfig-->" + ex.Message, ex);
            }
        }

        #region 檢查config_name是否存在
        public System.Data.DataTable ConfigCheck(ConfigQuery query)
        {
            try
            {
                return _configDao.ConfigCheck(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->ConfigCheckName-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 根據like語句查詢
        public System.Data.DataTable GetConfigByLike(ConfigQuery query)
        {
            try
            {
                return _configDao.GetConfigByLike(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->GetConfigByLike-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 獲取優化名稱store
        public System.Data.DataTable GetUser(ManageUserQuery query)
        {
            try
            {
                return _configDao.GetUser(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->GetUser-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 新增時查看收件人名稱是否重複
        public List<ConfigQuery> CheckSingleConfig(ConfigQuery query)
        {
            try
            {
                return _configDao.CheckSingleConfig(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->CheckSingleConfig-->" + ex.Message, ex);
            }
        }
        #endregion

        #region 編輯時檢查用戶名是否重複
        public List<ConfigQuery> CheckUserName(ConfigQuery query)
        {
            try
            {
                return _configDao.CheckUserName(query);
            }
            catch (Exception ex)
            {
                throw new Exception("ConfigMgr-->CheckUserName-->" + ex.Message, ex);
            }
        }
        #endregion
    }
}
