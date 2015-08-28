/* 
 * Copyright (c) 2014，武漢聯綿信息技術有限公司
 * All rights reserved. 
 *  
 * 文件名称：IUsersListImplDao 
 * 摘   要： 
 *  
 * 当前版本：1.0 
 * 作   者：dongya0410j 
 * 完成日期：2014/09/22 13:35:21 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model.Query;
using System.Data;

namespace BLL.gigade.Dao.Impl
{
    public interface IUsersListImplDao
    {
        List<UsersListQuery> Query(UsersListQuery store, out int totalCount);
        string SaveUserList(UsersListQuery usr);
        BLL.gigade.Model.Custom.Users getModel(int id);
        List<BonusMasterQuery> bQuery(BonusMasterQuery store, out int totalCount);
        int UpdateUser(Model.Custom.Users usr);
        int updateuser_master(BonusMasterQuery store);
        List<UsersListQuery> Export(UsersListQuery store);
        int UserCancel(UsersListQuery u);
        #region 汇出会员查询列表所需要的几个列
        ///chaojie_zz添加于2014/10/08 
        DataTable GetBonusTotal(DateTime timestart, DateTime timeend, string user_id);/*购物金发放*/
        DataTable GetRecordTotal(DateTime timestart, DateTime timeend, string user_id);/*购物金使用*/
        DataTable GetZipCode();/*查询所有地址*/
        #endregion
    }
}
