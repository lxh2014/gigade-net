#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：UserConditionMgr.cs
* 摘 要：
* 會員條件設定Mgr
* 当前版本：v1.1
* 作 者： mengjuan0826j
* 完成日期：2014/6/20 
* 修改歷史：
*         v1.1修改日期：2014/8/15 
*         v1.1修改人員：mengjuan0826j 
*         v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋 
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Mgr
{
    public class UserConditionMgr : IUserConditionImplMgr
    {
        private IUserConditionImplDao _ucDao;
        public UserConditionMgr(string connectionstring)
        {
            _ucDao = new UserConditionDao(connectionstring);
        }
        public DataTable Add(Model.UserCondition uc)
        {
            try
            {
                return _ucDao.Add(uc);
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Add-->" + ex.Message, ex);
            }
        }


        public int Update(Model.UserCondition uc)
        {
            try
            {
                return _ucDao.Update(uc);
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Update-->" + ex.Message, ex);
            }
        }

        public int Delete(Model.UserCondition uc)
        {
            try
            {
                return _ucDao.Delete(uc);
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Delete-->" + ex.Message, ex);
            }
        }

        public Model.UserCondition GetModelById(Model.UserCondition uc)
        {
            try
            {
                return _ucDao.GetModelById(uc);
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->GetModelById-->" + ex.Message, ex);
            }
        }

        public UserCondition Select(Model.UserCondition uc)
        {
            try
            {
                return _ucDao.Select(uc);
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Select-->" + ex.Message, ex);
            }
        }
    }
}
