#region 文件信息
/* 
 * Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司
 * All rights reserved. 
 *  
 * 文件名称：IUserLoginLogImplMgr.cs 
 * 摘   要： 
 *      會員登入記錄與資料庫交互方法
 * 当前版本：v1.2 
 * 作   者： dongya0410j
 * 完成日期：2014/6/20
 * 修改歷史：
 *      v1.1修改日期：2014/9/23
 *      v1.1修改人員：changjian0408j
 *      v1.1修改内容：在抛出異常的時候將Sql語句抛出，合并代碼，添加注釋
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using BLL.gigade.Model.Query;
namespace BLL.gigade.Dao.Impl
{
    public interface IUserLoginLogImplDao
    {
      List<UsersLoginQuery> Query(UsersLoginQuery store, out int totalCount);
      System.Data.DataTable GetUserInfo(int rid);
    }
}
