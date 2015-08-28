using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Mgr.Impl;
using DBAccess;
#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：UserIOMgr.cs 
 * 摘   要： 
 *      會員資料探勘與資料庫交互方法
 * 当前版本：v1.2 
 * 作   者： mengjuan0826j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/9/22
 *      v1.1修改人員：zhejiang0304j
 *      v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋
 */
#endregion
using BLL.gigade.Common;
using System.Text.RegularExpressions;
using System.Data;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;

namespace BLL.gigade.Mgr
{
    public class UserIOMgr : IUserIOImplMgr
    {
        private IUserIOImplDao _usersioDao;

        public UserIOMgr(string mySqlConnStr)
        {
                _usersioDao = new UserIODao(mySqlConnStr);
        }

        public DataTable GetExcelTable(string sql)
        {
            try
            {
                return _usersioDao.GetExcelTable(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("UserIOMgr-->GetExcelTable-->" + ex.Message, ex);
            }

        }
    }
}
