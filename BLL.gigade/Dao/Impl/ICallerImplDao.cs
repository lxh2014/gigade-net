﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
namespace BLL.gigade.Dao.Impl
{
    public interface ICallerImplDao
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="caller">用戶信息</param>
        /// <returns></returns>
        int Save(Caller caller);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="caller">用戶信息</param>
        /// <returns></returns>
        int Update(Caller caller);
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="rowID">行號</param>
        /// <returns></returns>
        int Delete(int rowID);
        /// <summary>
        /// 查詢用戶
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<Caller> Query(Caller caller, out int totalCount);
        /// <summary>
        /// 登錄（通過登錄信箱獲取用戶信息）
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Caller Login(string email);
        /// <summary>
        /// 通過用戶ID獲取用戶信息
        /// </summary>
        /// <param name="user_id"></param>
        /// <returns></returns>
        Caller GetUserById(int user_id);
        /// <summary>
        /// 生成密碼加密串
        /// </summary>
        /// <returns></returns>
        string Add_Challenge();
        /// <summary>
        /// 刪除密碼加密串
        /// </summary>
        /// <param name="challenge_id"></param>
        void Kill_Challenge_Id(string challenge_id);
        /// <summary>
        /// 得到密碼加密串
        /// </summary>
        /// <param name="challenge_id">加密串ID</param>
        /// <returns></returns>
        string Get_Challenge_Key(string challenge_id);
        /// <summary>
        /// 增加密碼錯誤次數
        /// </summary>
        /// <param name="user_id">用戶ID</param>
        void Add_Login_Attempts(int user_id);
        /// <summary>
        /// 添加用戶登錄記錄
        /// </summary>
        /// <param name="user_id">用戶ID</param>
        void Add_Manage_Login(int user_id);
        /// <summary>
        /// 修改用戶登錄數據
        /// </summary>
        /// <param name="user_id">用戶ID</param>
        void Modify_User_Login_Data(int user_id);
        /// <summary>
        /// 修改用戶狀態
        /// </summary>
        /// <param name="user_id">用戶ID</param>
        /// <param name="status">用戶狀態</param>
        void Modify_User_Status(int user_id, int status);
        /// <summary>
        /// 修改異動密碼時的確認碼
        /// </summary>
        /// <param name="user_id">用戶ID</param>
        /// <param name="user_confirm_code">確認碼</param>
        void Modify_User_Confirm_Code(int user_id, string user_confirm_code);
        /// <summary>
        /// 修改用戶密碼
        /// </summary>
        /// <param name="user_id">用戶ID</param>
        /// <param name="passwd">密碼</param>
        void Modify_User_Password(int user_id ,string passwd);
    }
}
