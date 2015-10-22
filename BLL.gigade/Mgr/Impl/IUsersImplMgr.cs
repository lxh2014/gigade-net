using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IUsersImplMgr
    {
        /// <summary>
        /// 根據會員email查詢
        /// </summary>
        /// <param name="strUserEmail"></param>
        /// <returns></returns>
        DataTable Query(string strUserEmail);

        List<Users> Query(Users query);

        /// <summary>
        /// 用作新增外站時的user新增并獲得新增的user_id
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        int SelSaveID(Users u);
        /// <summary>
        /// 會員購買記錄排行
        /// </summary>
        /// <param name="vipList"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Model.Query.UserVipListQuery> GetVipList(Model.Query.UserVipListQuery vipList, ref int totalCount);
        #region 獲取normal，low，ct，ht
        //UserVipListQuery GetNormalProd(UserVipListQuery uvlq);
        //UserVipListQuery GetLowProd(UserVipListQuery uvlq);
        //UserVipListQuery GetProdCT(UserVipListQuery uvlq);
        //UserVipListQuery GetProdHT(UserVipListQuery uvlq);
        #endregion

        DataTable IsVipUserId(uint user_id);

        string QueryBySID(uint user_id);

        /// <summary>
        /// 根據會員user_mobile查詢
        /// </summary>
        /// <param name="userMobile"></param>
        /// <returns></returns>
        DataTable QueryByUserMobile(string userMobile);

        /// <summary>
        /// 新增電話會員
        /// </summary>
        /// <param name="UserQuery"></param>
        /// <returns></returns>
        int SaveUserPhone(UserQuery u);

        /// <summary>
        /// 通過ID查詢信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Users SelectUseById(int id);

        List<Users> GetUserInfoByTest(string condition);// add by wangwei0216w 2014/10/27

        List<UserQuery> GetBonusList(UserQuery query, ref int totalCount);//add by shuangshuang0420j 2015.1.26 購物金資訊
        List<UserVipListQuery> ExportVipListCsv(UserVipListQuery query, ref int totalCount);
        List<UserQuery> Query(Model.Custom.Users query);

        List<Users> GetUser(Users u);
        List<UserQuery> GetUserByEmail(string mail, uint group_id);
    }
}
