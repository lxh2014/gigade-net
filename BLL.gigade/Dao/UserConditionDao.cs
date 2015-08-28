#region 文件信息
/* 
* Copyright (c) 2013，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：UserConditionDao.cs
* 摘 要：
* 會員條件設定dao
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
using BLL.gigade.Dao.Impl;
using DBAccess;
using System.Data;
using BLL.gigade.Model;

namespace BLL.gigade.Dao
{
    public class UserConditionDao : IUserConditionImplDao
    {
        private IDBAccess _access;
        public UserConditionDao(string connectionstring)
        {
            _access = DBFactory.getDBAccess(DBType.MySql, connectionstring);
        }

        #region 添加會員條件設定 +DataTable Add(Model.UserCondition uc)
        /// <summary>
        /// 添加會員條件設定
        /// </summary>
        /// <param name="uc">UserCondition uc對象</param>
        /// <returns>datatable對象</returns>
        public DataTable Add(Model.UserCondition uc)
        {
            int identityId;
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("insert into user_condition (");
                sql.AppendFormat("condition_name,reg_start,reg_end,reg_interval,buy_times_min,buy_times_max,buy_amount_min,buy_amount_max,");
                sql.AppendFormat("last_time_start,last_time_end,last_time_interval,join_channel,status) values (");
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", uc.condition_name, uc.reg_start, uc.reg_end, uc.reg_interval);
                sql.AppendFormat("'{0}','{1}','{2}','{3}',", uc.buy_times_min, uc.buy_times_max, uc.buy_amount_min, uc.buy_amount_max);
                sql.AppendFormat("'{0}','{1}','{2}','{3}','{4}');select @@identity;", uc.last_time_start, uc.last_time_end, uc.last_time_interval, uc.join_channel, uc.status);
                identityId = Int32.Parse(_access.getDataTable(sql.ToString()).Rows[0][0].ToString());
                StringBuilder sqlDt = new StringBuilder("select * from user_condition where condition_id = (" + identityId + ");");
                sql.Append(sqlDt);
                return _access.getDataTable(sqlDt.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Add-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 根據UserCondition對象的id獲取整個model對象 +UserCondition GetModelById(UserCondition uc)
        /// <summary>
        /// 根據UserCondition對象的id獲取整個model對象
        /// </summary>
        /// <param name="uc"></param>
        /// <returns></returns>
        public UserCondition GetModelById(UserCondition uc)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.Append("select *  from  user_condition");
                sql.AppendFormat("  where 1=1 and status=1 and condition_id={0};", uc.condition_id);
                return _access.getSinggleObj<UserCondition>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->GetModelById-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 根據UserCondition對象的id軟刪除數據 +int Delete(Model.UserCondition uc)
        /// <summary>
        /// 根據UserCondition對象的id軟刪除數據 
        /// </summary>
        /// <param name="uc">UserCondition uc對象</param>
        /// <returns>執行結果</returns>
        public int Delete(Model.UserCondition uc)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update user_condition set status=0 where condition_id={0} and 1=1;", uc.condition_id);
                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Delete-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 根據UserCondition 的id更新數據 +int Update(Model.UserCondition uc)
        /// <summary>
        /// 根據UserCondition 的id更新數據
        /// </summary>
        /// <param name="uc"></param>
        /// <returns>執行結果</returns>
        public int Update(Model.UserCondition uc)
        {
            uc.Replace4MySQL();
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update  user_condition  set ");
                sql.AppendFormat("condition_name='{0}',reg_start='{1}',reg_end='{2}',reg_interval='{3}',buy_times_min='{4}',buy_times_max='{5}',", uc.condition_name, uc.reg_start, uc.reg_end, uc.reg_interval, uc.buy_times_min, uc.buy_times_max);
                sql.AppendFormat("buy_amount_min='{0}',buy_amount_max='{1}',last_time_start='{2}',last_time_end='{3}',", uc.buy_amount_min, uc.buy_amount_max, uc.last_time_start, uc.last_time_end);
                sql.AppendFormat("last_time_interval='{0}',join_channel='{1}',status='{2}'", uc.last_time_interval, uc.join_channel, uc.status);
                sql.AppendFormat("  where condition_id={0};", uc.condition_id);

                return _access.execCommand(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Update-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 查詢數據獲取UserCondition對象 +UserCondition Select(UserCondition uc)
        /// <summary>
        /// 查詢數據
        /// </summary>
        /// <param name="uc"></param>
        /// <returns></returns>
        public UserCondition Select(UserCondition uc)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("select * from user_condition where status = '1';");
                return _access.getSinggleObj<UserCondition>(sql.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->Select-->" + ex.Message + sql.ToString(), ex);
            }
        }
        #endregion

        #region 根據condition_id軟刪除數據 +string DeleteUserCon(int condition_id)
        /// <summary>
        /// 根據condition_id軟刪除數據
        /// </summary>
        /// <param name="condition_id"></param>
        /// <returns></returns>
        public string DeleteUserCon(int condition_id)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                sql.AppendFormat("update user_condition set status=0 where condition_id={0} and 1=1;", condition_id);
                return sql.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("UserConditionMgr-->DeleteUserCon-->" + ex.Message + sql.ToString(), ex);
            }

        }

        #endregion

    }
}
