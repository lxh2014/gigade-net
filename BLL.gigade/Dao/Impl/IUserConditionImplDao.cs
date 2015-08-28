#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：IUserConditionImplDao.cs
* 摘 要：
* 會員條件設定dao接口
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
using BLL.gigade.Model;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IUserConditionImplDao
    {
        DataTable Add(UserCondition uc);
        int Update(UserCondition uc);
        int Delete(UserCondition uc);
        UserCondition GetModelById(UserCondition uc);
        UserCondition Select(UserCondition uc);
    }
}
